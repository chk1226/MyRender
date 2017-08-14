using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics;
using System;

namespace MyRender.MyEngine
{
    class Plane : Node
    {
        public enum PlaneType
        {
            Normal,
            Gaussian
        }

        private Vector2 rect;
        private int sliceX;
        private int sliceY;
        private PlaneType planeType = PlaneType.Normal;
        private bool isHorizontal;
        private FrameBuffer useFrame;
        private FrameBuffer bindFrame;


        public Plane(float width, float height, uint sliceX, uint sliceY)
        {
            rect = new Vector2(width, height);
            this.sliceX = (int)sliceX;
            this.sliceY = (int)sliceY;
            ModelList = new Model[1];

            var modelData = Resource.Instance.GetModel(GUID);
            if (modelData == null)
            {
                modelData = new Model();
                modelData.DrawType = PrimitiveType.Quads;

                modelData.guid = GUID;

                createPlaneData(modelData);
                // gen vertex buffer
                modelData.GenVerticesBuffer();

                // gen texture cood buffer
                modelData.GenTexcoordsBuffer();

                // gen normal texture cood buffer
                modelData.GenNormalBuffer();

                Resource.Instance.AddModel(modelData);
            }
            ModelList[0] = modelData;
        }

        public void SetPlaneType(PlaneType type, bool horizontal)
        {
            planeType = type;
            isHorizontal = horizontal;
        }

        public void SetFrameBuffer(FrameBuffer use, FrameBuffer bind)
        {
            useFrame = use;
            bindFrame = bind;
        }

        public override void OnStart()
        {
            base.OnStart();
            var modelData = ModelList[0];

            // generate render object
            if (planeType == PlaneType.Normal)
            {
                Render render = Render.CreateRender(Resource.Instance.CreatePlaneM(), delegate (Render r) {
                    var m = r.MaterialData;

                    if (m.ShaderProgram != 0)
                    {
                        GL.UseProgram(m.ShaderProgram);

                        m.UniformTexture("TEX_COLOR", TextureUnit.Texture0, Material.TextureType.Color, 0);

                        Light light;
                        if (GameDirect.Instance.MainScene.SceneLight.TryGetTarget(out light))
                        {
                            var dir = light.GetDirectVector();
                            m.Uniform3("DIR_LIGHT", dir.X, dir.Y, dir.Z);
                            if (light.EnableSadowmap)
                            {
                                if (useFrame != null) m.UniformTexture("SHADOWMAP", TextureUnit.Texture1, useFrame.CB_Texture, 1);
                                var bmvp = light.LightBiasProjectView() * WorldModelMatrix * LocalModelMatrix;
                                m.UniformMatrix4("LIGHT_BPVM", ref bmvp, true);
                            }
                        }

                    }
                },
                this,
                modelData,
                Render.Normal);

                RenderList.Add(render);
            }
            else if (planeType == PlaneType.Gaussian)
            {
                var material = new Material();
                material.ShaderProgram = Resource.Instance.GetShader(Resource.SGaussianBlur);
                Render render = Render.CreateRender(material, delegate (Render r)
                {
                    var m = r.MaterialData;

                    if (m.ShaderProgram != 0)
                    {
                        GL.UseProgram(m.ShaderProgram);

                        if (useFrame != null) m.UniformTexture("TEX_COLOR", TextureUnit.Texture0, useFrame.CB_Texture, 0);
                        var vp = GameDirect.Instance.MainScene.MainCamera.Viewport;
                        if (!isHorizontal)
                        {
                            m.Uniform2("Offset", 0, 1.0f / vp.Height);
                        }
                        else
                        {
                            m.Uniform2("Offset", 1.0f / vp.Width, 0);
                        }

                    }
                },
                this,
                modelData,
                (isHorizontal) ? Render.PrePostrender + 1 : Render.PrePostrender);

                RenderList.Add(render);
            }
        }

        public override void OnRenderBegin(FrameEventArgs e)
        {
            if(planeType == PlaneType.Normal)
            {
                base.OnRenderBegin(e);
            }
            else if(planeType == PlaneType.Gaussian)
            {
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, bindFrame.FB);
                GL.Clear(ClearBufferMask.ColorBufferBit);

                GL.MatrixMode(MatrixMode.Projection);
                GL.PushMatrix();
                var vp = GameDirect.Instance.MainScene.MainCamera.Viewport;
                var vm = Matrix4.CreateOrthographic(vp.Width, vp.Height, 0.125f, 1.125f); ;
                GL.LoadMatrix(ref vm);

                GL.MatrixMode(MatrixMode.Modelview);
                GL.PushMatrix();
                vm = GameDirect.Instance.MainScene.MainCamera.UIViewMatrix * WorldModelMatrix * LocalModelMatrix;
                vm.Transpose();
                GL.LoadMatrix(ref vm);

            }

        }

        public override void OnRenderFinsh(FrameEventArgs e)
        {
            if (planeType == PlaneType.Normal)
            {
                base.OnRenderFinsh(e);
            }
            else if (planeType == PlaneType.Gaussian)
            {
                GL.MatrixMode(MatrixMode.Modelview);
                GL.PopMatrix();
                GL.MatrixMode(MatrixMode.Projection);
                GL.PopMatrix();

                GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            }

        }

        private void createPlaneData(Model model)
        {
            if(model == null)
            {
                return;
            }

            Vector2 offset = new Vector2(rect.X / sliceX, rect.Y / sliceY);
            int total = sliceX * sliceY;
            model.Vertices = new Vector3[total * 4];
            model.Normals = new Vector3[total * 4];
            model.Texcoords = new Vector2[total * 4];

            Vector2 current = Vector2.Zero;
            for(int i = 0; i < sliceY; i++)
            {
                current.X = 0;
                current.Y = i * offset.Y;
                for(int j = 0; j < sliceX; j++)
                {
                    current.X = j * offset.X;

                    model.Vertices[i * (sliceX * 4) + j * 4] = new Vector3(current.X, 0, current.Y);
                    model.Vertices[i * (sliceX * 4) + j * 4 + 1] = new Vector3(current.X, 0, current.Y + offset.Y);
                    model.Vertices[i * (sliceX * 4) + j * 4 + 2] = new Vector3(current.X + offset.X, 0, current.Y + offset.Y);
                    model.Vertices[i * (sliceX * 4) + j * 4 + 3] = new Vector3(current.X + offset.X, 0, current.Y);

                    model.Normals[i * (sliceX * 4) + j * 4] = Vector3.UnitY;
                    model.Normals[i * (sliceX * 4) + j * 4 + 1] = Vector3.UnitY;
                    model.Normals[i * (sliceX * 4) + j * 4 + 2] = Vector3.UnitY;
                    model.Normals[i * (sliceX * 4) + j * 4 + 3] = Vector3.UnitY;

                    model.Texcoords[i * (sliceX * 4) + j * 4] = new Vector2(0, 0);
                    model.Texcoords[i * (sliceX * 4) + j * 4 + 1] = new Vector2(0, 1);
                    model.Texcoords[i * (sliceX * 4) + j * 4 + 2] = new Vector2(1, 1);
                    model.Texcoords[i * (sliceX * 4) + j * 4 + 3] = new Vector2(1, 0);


                }

            }


        }

    }
}
