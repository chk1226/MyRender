using OpenTK.Graphics.OpenGL;
using OpenTK;
using System.Collections.Generic;
using System;
using MyRender.Debug;

namespace MyRender.MyEngine
{
    class ParticleSystem : Node
    {
        private float pps;
        private float speed;
        private float gravity;
        private float lifeLength; 
        private float scale;
        private Vector3 direction = Vector3.Zero;
        private float directionDeviation = 0;
        private float recordTime = 0;
        private string textureName;
        private Vector2 textureInfo;

        // margin variable
        private float speedMargin = 0;
        private float lifeMargin = 0;
        private float scaleMargin = 0;

        private List<Particle> particleList = new List<Particle>(150);
        private Matrix4 boardModelMatrix = Matrix4.Identity;
        private bool randomRotation = false;
        private BlendingFactorDest blendDest = BlendingFactorDest.OneMinusSrcAlpha;

        public ParticleSystem(float pps, float speed, float gravity, float lifeLength, float scale, string textureName, Vector2 textureInfo) : base()
        {
            this.pps = Math.Max(pps, 0.01f);
            this.speed = speed;
            this.gravity = gravity;
            this.lifeLength = lifeLength;
            this.scale = scale;
            this.textureName = textureName;
            this.textureInfo = textureInfo;
            createParticleModel();

        }

        public override void OnStart()
        {
            base.OnStart();

            Material material = new Material();
            material.ShaderProgram = Resource.Instance.GetShader(Resource.SParticle);
            Render render = Render.CreateRender(material, delegate (Render r)
            {
                var m = r.MaterialData;

                if (m.ShaderProgram != 0)
                {
                    GL.UseProgram(m.ShaderProgram);
                    
                    m.Uniform2("Offset", 1 / textureInfo.X, 1 / textureInfo.Y);

                    var model = WorldModelMatrix * LocalModelMatrix;
                    m.UniformMatrix4("MODEL", ref model, true);

                    var view = GameDirect.Instance.MainScene.MainCamera.ViewMatrix;
                    m.UniformMatrix4("VIEW", ref view, true);

                    var project = GameDirect.Instance.MainScene.MainCamera.ProjectMatix;
                    m.UniformMatrix4("PROJECTION", ref project, true);

                    updateBoardModelMatrix();
                    m.UniformMatrix4("BoardMatrix", ref boardModelMatrix, true);

                    m.UniformTexture("TEX_COLOR", TextureUnit.Texture0, Resource.Instance.GetTextureID(textureName), 0);

                }
            },
            this,
            ModelList[0],
            Render.Blend);
            render.ShaderVersion = Render.OPENGL_330;
            render.AddVertexAttribute330("position", ModelList[0].GetBufferData( Model.BufferType.Vertices).BufferID, 3, false, 0);
            render.AddVertexAttribute330("scale", ModelList[0].GetBufferData(Model.BufferType.Scale).BufferID, 1, false, 1);
            render.AddVertexAttribute330("rotation", ModelList[0].GetBufferData(Model.BufferType.Rotation).BufferID, 1, false, 2);
            render.AddVertexAttribute330("blendFactor", ModelList[0].GetBufferData(Model.BufferType.BlendFactor).BufferID, 1, false, 3);
            render.AddVertexAttribute330("texCoord", ModelList[0].GetBufferData(Model.BufferType.ParticleTextureCoord).BufferID, 4, false, 4);
            render.EnableBlend(BlendingFactorSrc.SrcAlpha, blendDest);
            RenderList.Add(render);

        }

        public void SetBlendFuc(BlendingFactorDest dest)
        {
            blendDest = dest;
        }

        private void updateBoardModelMatrix()
        {
            var view = GameDirect.Instance.MainScene.MainCamera.ViewMatrix;
            view.Transpose();
            boardModelMatrix.M11 = view.M11;
            boardModelMatrix.M12 = view.M12;
            boardModelMatrix.M13 = view.M13;
            boardModelMatrix.M21 = view.M21;
            boardModelMatrix.M22 = view.M22;
            boardModelMatrix.M23 = view.M23;
            boardModelMatrix.M31 = view.M31;
            boardModelMatrix.M32 = view.M32;
            boardModelMatrix.M33 = view.M33;
        }

        private void createParticleModel()
        {
            var modelData = new Model();
            modelData.DrawType = PrimitiveType.Points;
            modelData.guid = this.GUID;

            // gen vertex buffer
            modelData.GenVec3Buffer(Model.BufferType.Vertices, new Vector3[0]);

            // gen scale buffer
            modelData.GenFloatBuffer(Model.BufferType.Scale, new float[0]);

            // gen rotation buffer
            modelData.GenFloatBuffer(Model.BufferType.Rotation, new float[0]);

            // gen blend factor buffer
            modelData.GenFloatBuffer(Model.BufferType.BlendFactor, new float[0]);

            // gen texture coordinate
            modelData.GenVec4Buffer(Model.BufferType.ParticleTextureCoord, new Vector4[0]);

            Resource.Instance.AddModel(modelData);
            ModelList = new Model[1];
            ModelList[0] = modelData;
        }

        /// <summary>
        /// param between 0 and 1, where 0 means no margin.
        /// </summary>
        public void SetSpeedMargin(float margin)
        {
            speedMargin = margin * speed;
        }

        /// <summary>
        /// param between 0 and 1, where 0 means no margin.
        /// </summary>
        public void SetLifeMargin(float margin)
        {
            lifeMargin = margin * lifeLength;
        }

        /// <summary>
        /// param between 0 and 1, where 0 means no margin.
        /// </summary>
        public void SetScaleMargin(float margin)
        {
            scaleMargin = margin * scale;
        }

        /// <summary>
        /// direction - The average direction in which particles are emitted.
        /// deviation - A value between 0 and 1 indicating how far from the chosen direction particles can deviate.
        /// </summary>
        public void SetDirection(Vector3 direction, float deviation)
        {
            this.direction = direction;
            directionDeviation = (float)(deviation * Math.PI);
        }
        

        public void SetRandomRotation(bool enable)
        {
            randomRotation = true;
        }

        private float generateValue(float average, float margin)
        {
            float offset = ((float)Algorithm.GetRandom.NextDouble() - 0.5f) * 2f * margin;
            return average + offset;

        }

        public override void OnRenderBegin(FrameEventArgs e)
        {
            base.OnRenderBegin(e);
            GL.Disable(EnableCap.DepthTest);

        }

        public override void OnRenderFinsh(FrameEventArgs e)
        {
            base.OnRenderFinsh(e);
            GL.Enable(EnableCap.DepthTest);

        }

        public override void OnUpdate(FrameEventArgs e)
        {
            base.OnUpdate(e);

            generateParticles((float)e.Time);
            updateParticle((float)e.Time);
        }

        private void updateParticle(float deltaTime)
        {
            var model = WorldModelMatrix * LocalModelMatrix;

            int count = 0;
            foreach (var p in particleList)
            {
                if (p != null && p.IsLife)
                {
                    p.Update(deltaTime, ref model);
                    count++;
                }
            }

            updateModelData(count);
        }

        private void updateModelData(int count)
        {
            var vertices = new Vector3[count];
            var scales = new float[count];
            var rotation = new float[count];
            var blendFactor = new float[count];
            var texCoord = new Vector4[count];

            int index = 0;
            foreach (var p in particleList)
            {
                if (p != null && p.IsLife)
                {
                    vertices[index] = p.Position;
                    scales[index] = p.Scale;
                    rotation[index] = p.Rotation;
                    blendFactor[index] = p.BlendAnimationFactor;
                    texCoord[index] = p.GetTextureCoord();
                    index++;
                }
            }

            ModelList[0].GetBufferData( Model.BufferType.Vertices).vec3Data = vertices;
            ModelList[0].ReloadBufferVec3Data(Model.BufferType.Vertices);

            ModelList[0].GetBufferData(Model.BufferType.Scale).floatData = scales;
            ModelList[0].ReloadBufferFloatData(Model.BufferType.Scale);

            ModelList[0].GetBufferData(Model.BufferType.Rotation).floatData = rotation;
            ModelList[0].ReloadBufferFloatData(Model.BufferType.Rotation);

            ModelList[0].GetBufferData(Model.BufferType.BlendFactor).floatData = blendFactor;
            ModelList[0].ReloadBufferFloatData(Model.BufferType.BlendFactor);

            ModelList[0].GetBufferData(Model.BufferType.ParticleTextureCoord).vec4Data = texCoord;
            ModelList[0].ReloadBufferVec4Data(Model.BufferType.ParticleTextureCoord);
            
        }

        private void generateParticles(float deltaTime)
        {
            recordTime += deltaTime;

            var genNum = (int)Math.Floor(recordTime / pps);

            if(genNum != 0)
            {
                for(int i = 0; i < genNum; i++)
                {
                    emitParticle();
                }

                recordTime -= genNum * pps;

                // do order by elapsed time
                particleList.Sort(delegate (Particle x, Particle y)
                {
                    if (x.Distance > y.Distance)
                    {
                        return -1;
                    }
                    else if (x.Distance < y.Distance)
                    {
                        return 1;
                    }

                    return 0;

                });
            }

        }

        private void emitParticle()
        {
            Vector3 velocity;
            if (direction != Vector3.Zero)
            {
                velocity = generateRandomDircetWithinCone(direction, directionDeviation);
            }
            else
            {
                velocity = generateRandomDircet();
            }
            velocity *= generateValue(speed, speedMargin);
            float scale = generateValue(this.scale, scaleMargin);
            float life = generateValue(lifeLength, lifeMargin);

            bool emitSuccess = false;
            for(int i = 0; i < particleList.Count; i++)
            {
                var p = particleList[i];
                if (p == null)
                {
                    p = new Particle(Vector3.Zero, velocity, lifeLength, gravity, generateRotation(), scale, textureInfo);
                    emitSuccess = true;
                    break;
                }
                else if(!p.IsLife)
                {
                    p.ResetParticle(Vector3.Zero, velocity, lifeLength, gravity, generateRotation(), scale);
                    emitSuccess = true;
                    break;
                }

            }
            
            if(!emitSuccess)
            {
                particleList.Add(new Particle(Vector3.Zero, velocity, lifeLength, gravity, generateRotation(), scale, textureInfo));
            }
        }

        private Vector3 generateRandomDircet()
        {
            float theta = (float)(Algorithm.GetRandom.NextDouble() * 2.0f * MathHelper.Pi);
            float z = ((float)Algorithm.GetRandom.NextDouble() * 2) - 1;
            float rootOneMinusZSquared = (float)Math.Sqrt(1 - z * z);
            float x = (float)(rootOneMinusZSquared * Math.Cos(theta));
            float y = (float)(rootOneMinusZSquared * Math.Sin(theta));

            Vector3 v = new Vector3(x, y, z);
            v.Normalize();
            return v;
        }

        private Vector3 generateRandomDircetWithinCone(Vector3 coneDirection, float angle)
        {
            float cosAngle = (float)Math.Cos(angle);
            Random random = new Random();
            float z = cosAngle + ((float)Algorithm.GetRandom.NextDouble() * (1 - cosAngle));
            float theta = (float)(Algorithm.GetRandom.NextDouble() * 2f * Math.PI);
            float rootOneMinusZSquared = (float)Math.Sqrt(1 - z * z);
            float x = (float)(rootOneMinusZSquared * Math.Cos(theta));
            float y = (float)(rootOneMinusZSquared * Math.Sin(theta));

            Vector4 direction = new Vector4(x, y, z, 1);
            if (coneDirection.X != 0 || coneDirection.Y != 0 || (coneDirection.Z != 1 && coneDirection.Z != -1))
            {
                Vector3 rotateAxis = Vector3.Cross(coneDirection, new Vector3(0, 0, 1));
                rotateAxis.Normalize();
                float rotateAngle = (float)Math.Acos(Vector3.Dot(coneDirection, new Vector3(0, 0, 1)));
                var rotationMatrix = Matrix4.CreateFromAxisAngle(rotateAxis, rotateAngle);
                direction = rotationMatrix * direction;
            }
            else if (coneDirection.Z == -1)
            {
                direction.Z *= -1;
            }

            var result = new Vector3(direction);
            result.Normalize();
            return result;
        }

        private float generateRotation()
        {
            if (randomRotation)
            {
                return (float)Algorithm.GetRandom.NextDouble() * MathHelper.Pi;
            }
            else
            {
                return 0;
            }
        }


    }
}
