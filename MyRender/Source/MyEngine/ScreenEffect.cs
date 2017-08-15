using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;

namespace MyRender.MyEngine
{
    // ssao reference
    // https://learnopengl.com/#!Advanced-Lighting/SSAO
    class ScreenEffect : Plane
    {
        public enum EffectType
        {
            None,
            Gaussian,
            SSAO
        }

        private EffectType type = EffectType.None;
        //private Vector2 rect;
        static private readonly string screenEffectID = "ScreenEffect";
        private FrameBuffer useFrame;
        private FrameBuffer bindFrame;

        // Gaussian
        private bool isHorizontal;

        // SSAO
        private Vector3[] sampleKernel;
        private Vector3[] rotationNoise;
        private string rotationNoiseTextureID = "SSAO_RotationNoise";


        public void SetFrameBuffer(FrameBuffer use, FrameBuffer bind)
        {
            useFrame = use;
            bindFrame = bind;
        }

        public void EnableGaussian(bool horizontal)
        {
            type = EffectType.Gaussian;
            isHorizontal = horizontal;
        }

        public void EnableSSAO()
        {
            type = EffectType.SSAO;
            // gen sample kernel
            genSampleKernel();

            // gen random kernel
            genRotationNoise();
        }

        public ScreenEffect(float width, float height) : base(width, height, 1, 1, screenEffectID)
        {

            LocalPosition = new Vector3(-rect.X / 2.0f, 0, -rect.Y / 2.0f);
            Rotation(1, 0, 0, -90);

        }

        public override void OnStart()
        {
            base.OnStart();
            var modelData = ModelList[0];

            if (type == EffectType.Gaussian)
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
                        if (!isHorizontal)
                        {
                            m.Uniform2("Offset", 0, 1.0f / rect.Y);
                        }
                        else
                        {
                            m.Uniform2("Offset", 1.0f / rect.X, 0);
                        }

                    }
                },
                this,
                modelData,
                (isHorizontal) ? Render.PrePostrender + 1 : Render.PrePostrender);

                RenderList.Add(render);
            }
            else if(type == EffectType.SSAO)
            {
                var material = new Material();
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
                Render.PrePostrender + 2);

                RenderList.Add(render);
            }
        }

        public override void OnRenderBegin(FrameEventArgs e)
        {

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, bindFrame.FB);
            GL.Clear(ClearBufferMask.ColorBufferBit);

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
