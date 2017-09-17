using MyRender.Game;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;

namespace MyRender.MyEngine
{
    // ssao reference
    // https://learnopengl.com/#!Advanced-Lighting/SSAO
    // dfo reference
    // http://encelo.netsons.org/2008/04/15/depth-of-field-reloaded/
    class ScreenEffect : Plane
    {
        public enum EffectType
        {
            None,
            Gaussian,
            SSAO,
            BrightFilter,
            CombineBright,
            DepthOfField,
            CombineDeferred,
            DeferredLight
        }

        private EffectType type = EffectType.None;
        static private readonly string screenEffectID = "ScreenEffect";
        private FrameBuffer useFrame = null;
        private FrameBuffer bindFrame = null;
        private int renderPriority;

        // Gaussian
        private bool isHorizontal;

        // SSAO
        private Vector3[] sampleKernel;
        private Vector3[] rotationNoise;
        private string rotationNoiseTextureID = "SSAO_RotationNoise";

        //CombineBright
        private int colorTexture;
        private int blurTexture;
        private float scaleValue;

        // dof
        private int depthTexture;

        // combine deferred
        private int frontID;
        private int midID;
        private int backID;

        // deferred loght
        private Vector4 attenuationInfo;

        // BrightFilter
        float brightness;

        public void SetFrameBuffer(FrameBuffer use, FrameBuffer bind)
        {
            useFrame = use;
            bindFrame = bind;
        }

        public void EnableCombineDeferred(int front, int mid, int back)
        {
            type = EffectType.CombineDeferred;
            frontID = front;
            backID = back;
            midID = mid;
        }

        public void EnableGaussian(bool horizontal, float scale = 1)
        {
            type = EffectType.Gaussian;
            isHorizontal = horizontal;
            scaleValue = scale;
        }

        public void EnableBrightFilter(float brightness)
        {
            type = EffectType.BrightFilter;
            this.brightness = brightness;
        }

        public void EnableDeferredLight(Vector4 att)
        {
            type = EffectType.DeferredLight;
            attenuationInfo = att;
        }

        public void EnableCombineBright(int color, int blur)
        {
            type = EffectType.CombineBright;
            colorTexture = color;
            blurTexture = blur;
        }

        public void EnableDepthOfField(int color, int blur, int depth)
        {
            type = EffectType.DepthOfField;
            colorTexture = color;
            blurTexture = blur;
            depthTexture = depth;
        }

        public void EnableSSAO()
        {
            type = EffectType.SSAO;
            // gen sample kernel
            genSampleKernel();

            // gen random kernel
            genRotationNoise();
        }

        public ScreenEffect(float width, float height, int priority) : base(width, height, 1, 1, screenEffectID)
        {
            renderPriority = priority;
            LocalPosition = new Vector3(-rect.X / 2.0f, 0, -rect.Y / 2.0f);
            Rotation(1, 0, 0, -90);

        }

        public override void OnStart()
        {
            base.OnStart();
            var modelData = ModelList[0];

            var material = new Material();
            if (type == EffectType.Gaussian)
            {
                material.ShaderProgram = Resource.Instance.GetShader(Resource.SGaussianBlur);
                Render render = Render.CreateRender(material, delegate (Render r)
                {
                    var m = r.MaterialData;

                    if (m.ShaderProgram != 0)
                    {
                        GL.UseProgram(m.ShaderProgram);

                        if (useFrame != null) m.UniformTexture("TEX_COLOR", TextureUnit.Texture0, useFrame.CB_Texture, 0);
                        if (!isHorizontal)
                        {
                            m.Uniform2("Offset", 0, 1.0f / rect.Y * scaleValue);
                        }
                        else
                        {
                            m.Uniform2("Offset", 1.0f / rect.X * scaleValue, 0);
                        }

                    }
                },
                this,
                modelData,
                renderPriority);

                RenderList.Add(render);
            }
            else if(type == EffectType.SSAO)
            {
                material.ShaderProgram = Resource.Instance.GetShader(Resource.SSSAO);
                Render render = Render.CreateRender(material, delegate (Render r)
                {
                    var m = r.MaterialData;

                    if (m.ShaderProgram != 0)
                    {
                        GL.UseProgram(m.ShaderProgram);

                        if (useFrame != null)
                        {
                            m.UniformTexture("GPOSITION", TextureUnit.Texture0, useFrame.Position, 0);
                            m.UniformTexture("GNORMAL", TextureUnit.Texture1, useFrame.Normal, 1);
                        }
                        m.UniformTexture("NOISE", TextureUnit.Texture2, Resource.Instance.GetTextureID(rotationNoiseTextureID), 2);
                        var project = GameDirect.Instance.MainScene.MainCamera.ProjectMatix;
                        m.UniformMatrix4("PROJECTION", ref project, true);
                        m.Uniform2("NOISE_SCALE", rect.X / 4, rect.Y / 4);
                        for (int i = 0; i < sampleKernel.Length; i++)
                        {
                            m.Uniform3("SAMPLES[" + i.ToString() + "]", sampleKernel[i].X, sampleKernel[i].Y, sampleKernel[i].Z);
                        }
                        
                    }
                },
                this,
                modelData,
                renderPriority);

                RenderList.Add(render);
            }
            else if(type == EffectType.BrightFilter)
            {
                material.ShaderProgram = Resource.Instance.GetShader(Resource.SBrightFilter);
                Render render = Render.CreateRender(material, delegate (Render r)
                {
                    var m = r.MaterialData;

                    if (m.ShaderProgram != 0)
                    {
                        GL.UseProgram(m.ShaderProgram);

                        if (useFrame != null) m.UniformTexture("TEX_COLOR", TextureUnit.Texture0, useFrame.CB_Texture, 0);
                        m.Uniform1("Brightness", brightness);
                    }
                },
                this,
                modelData,
                renderPriority);

                RenderList.Add(render);
            }
            else if(type == EffectType.CombineBright)
            {
                material.ShaderProgram = Resource.Instance.GetShader(Resource.SCombineBright);
                Render render = Render.CreateRender(material, delegate (Render r)
                {
                    var m = r.MaterialData;

                    if (m.ShaderProgram != 0)
                    {
                        GL.UseProgram(m.ShaderProgram);
                        m.UniformTexture("TEX_COLOR", TextureUnit.Texture0, colorTexture, 0);
                        m.UniformTexture("BLUR", TextureUnit.Texture1, blurTexture, 1);
                    }
                },
                this,
                modelData,
                renderPriority);

                RenderList.Add(render);
            }
            else if(type == EffectType.DepthOfField)
            {
                material.ShaderProgram = Resource.Instance.GetShader(Resource.SDOF);
                Render render = Render.CreateRender(material, delegate (Render r)
                {
                    var m = r.MaterialData;

                    if (m.ShaderProgram != 0)
                    {
                        GL.UseProgram(m.ShaderProgram);
                        m.UniformTexture("TEX_COLOR", TextureUnit.Texture0, colorTexture, 0);
                        m.UniformTexture("BLUR", TextureUnit.Texture1, blurTexture, 1);
                        m.UniformTexture("DEPTH", TextureUnit.Texture2, depthTexture, 2);

                        var mainCamera = GameDirect.Instance.MainScene.MainCamera;
                        m.Uniform1("Near", mainCamera.zNear);
                        m.Uniform1("Far", mainCamera.zFar);
                        m.Uniform1("Zoom", mainCamera.EyeRotation.Z);
                    }
                },
                this,
                modelData,
                renderPriority);

                RenderList.Add(render);
            }
            else if (type == EffectType.CombineDeferred)
            {
                material.ShaderProgram = Resource.Instance.GetShader(Resource.SCombineDeferred);
                Render render = Render.CreateRender(material, delegate (Render r)
                {
                    var m = r.MaterialData;

                    if (m.ShaderProgram != 0)
                    {
                        GL.UseProgram(m.ShaderProgram);

                        m.UniformTexture("BACK", TextureUnit.Texture0, backID, 0);
                        m.UniformTexture("MID", TextureUnit.Texture1, midID, 1);
                        m.UniformTexture("FRONT", TextureUnit.Texture2, frontID, 2);
                    }
                },
                this,
                modelData,
                renderPriority);

                RenderList.Add(render);
            }
            else if(type == EffectType.DeferredLight)
            {
                material.ShaderProgram = Resource.Instance.GetShader(Resource.SDeferredLight);
                Render render = Render.CreateRender(material, delegate (Render r)
                {
                    var m = r.MaterialData;

                    if (m.ShaderProgram != 0)
                    {
                        GL.UseProgram(m.ShaderProgram);

                        m.UniformTexture("TEX_COLOR", TextureUnit.Texture0, useFrame.CB_Texture, 0);
                        m.UniformTexture("POSITION", TextureUnit.Texture1, useFrame.Position, 1);
                        m.UniformTexture("NORMAL", TextureUnit.Texture2, useFrame.Normal, 2);

                        m.Uniform4("AttenuationInfo", attenuationInfo.X, attenuationInfo.Y, attenuationInfo.Z, attenuationInfo.W);
                        var lights = GameDirect.Instance.MainScene.SceneLightCube;
                        for (int i = 0; i < lights.Count; i++)
                        {
                            LightCube lCube;
                            if (lights[i].TryGetTarget(out lCube))
                            {
                                var data = lCube.GetLightInfo();
                                m.Uniform4("LightInfo[" + i.ToString() + "]", data.X, data.Y, data.Z, data.W);
                                m.Uniform3("LightColor[" + i.ToString() + "]", lCube.Color.X, lCube.Color.Y, lCube.Color.Z);
                            }
                        }
                    }
                },
                this,
                modelData,
                renderPriority);

                RenderList.Add(render);
            }
        }

        public override void OnRenderBegin(FrameEventArgs e)
        {
            if(bindFrame == null)
            {
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            }
            else
            {
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, bindFrame.FB);
            }
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.MatrixMode(MatrixMode.Projection);
            GL.PushMatrix();
            var vm = Matrix4.CreateOrthographic(rect.X, rect.Y, 0.125f, 1.125f); ;
            GL.LoadMatrix(ref vm);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.PushMatrix();
            vm = GameDirect.Instance.MainScene.MainCamera.UIViewMatrix * WorldModelMatrix * LocalModelMatrix;
            vm.Transpose();
            GL.LoadMatrix(ref vm);

        }

        public override void OnRenderFinsh(FrameEventArgs e)
        {

            GL.MatrixMode(MatrixMode.Modelview);
            GL.PopMatrix();
            GL.MatrixMode(MatrixMode.Projection);
            GL.PopMatrix();

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            
        }

        private void genSampleKernel()
        {
            sampleKernel = new Vector3[64];
            for (int i = 0; i < sampleKernel.Length; i++)
            {
                float x = (float)(Algorithm.GetRandom.NextDouble() * 2.0 - 1.0);
                float y = (float)(Algorithm.GetRandom.NextDouble() * 2.0 - 1.0);
                float z = (float)(Algorithm.GetRandom.NextDouble());

                sampleKernel[i].X = x;
                sampleKernel[i].Y = y;
                sampleKernel[i].Z = z;

                sampleKernel[i].Normalize();
                float scale = (float)(Algorithm.GetRandom.NextDouble());
                sampleKernel[i] *= scale;

                scale = (float)(i) / sampleKernel.Length;
                scale = Algorithm.Lerp(0.1f, 1.0f, scale * scale);
                sampleKernel[i] *= scale;
            }
        }

        private void genRotationNoise()
        {
            rotationNoise = new Vector3[16];
            for (int i = 0; i < rotationNoise.Length; i++)
            {
                float x = (float)(Algorithm.GetRandom.NextDouble() * 2.0 - 1.0);
                float y = (float)(Algorithm.GetRandom.NextDouble() * 2.0 - 1.0);

                rotationNoise[i].X = x;
                rotationNoise[i].Y = y;
                rotationNoise[i].Z = 0;
            }

            int noise = GL.GenTexture();

            GL.BindTexture(TextureTarget.Texture2D, noise);
            GL.TexParameter(TextureTarget.Texture2D,
                            TextureParameterName.TextureMinFilter,
                            (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D,
                            TextureParameterName.TextureMagFilter,
                            (int)TextureMagFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D,
                            TextureParameterName.TextureWrapS,
                            (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D,
                            TextureParameterName.TextureWrapT,
                            (int)TextureWrapMode.Repeat);

            GL.TexImage2D(TextureTarget.Texture2D, 0,
            PixelInternalFormat.Rgb16f,
            4,
            4,
            0,
            PixelFormat.Rgb,
            PixelType.Float,
            rotationNoise);
            GL.BindTexture(TextureTarget.Texture2D, 0);

            Resource.Instance.AddTexture(rotationNoiseTextureID, noise);
        }

    }
}
