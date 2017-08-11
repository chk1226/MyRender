using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace MyRender.MyEngine
{
    class Light : Node
    {
        /// <summary>
        /// Ambient default value is Vector4(0.4f, 0.4f, 0.4f, 1.0f)
        /// </summary>
        public Vector4 Ambient = new Vector4(0.4f, 0.4f, 0.4f, 1.0f);
        /// <summary>
        /// Specular default value is Vector4(0.9f, 0.9f, 0.9f, 1.0f)
        /// </summary>
        public Vector4 Specular = new Vector4(0.9f, 0.9f, 0.9f, 1.0f);
        /// <summary>
        /// Diffuse default value is Vector4(0.3f, 0.3f, 0.3f, 1.0f)
        /// </summary>
        public Vector4 Diffuse = new Vector4(0.3f, 0.3f, 0.3f, 1.0f);

        private Matrix4 lightViewMatrix;
        private Matrix4 lightProjectMatrix;
        private Matrix4 biasMatrix;

        private Matrix4 regViewMatrix;
        private Matrix4 regProjectMatrix;

        public override void OnStart()
        {
            base.OnStart();

            LocalPosition = new Vector3(0, 100, 1);

            // Shininess value is problem, sometime shader can't get value
            //GL.Material(MaterialFace.Front, MaterialParameter.Shininess, 64);

            SetupLight();
            updateLightMatrix();
            biasMatrix = new Matrix4(0.5f, 0.0f, 0.0f, 0.5f,
                                    0.0f, 0.5f, 0.0f, 0.5f,
                                    0.0f, 0.0f, 0.5f, 0.5f,
                                    0.0f, 0.0f, 0.0f, 1.0f);

            GameDirect.Instance.MainScene.SceneLight = new System.WeakReference<Light>(this);
        }

        public void SetupLight()
        {
            GL.Light(LightName.Light0, LightParameter.Position, new Vector4(LocalPosition, 1));
            GL.Light(LightName.Light0, LightParameter.Ambient, Ambient);
            GL.Light(LightName.Light0, LightParameter.Diffuse, Diffuse);
            GL.Light(LightName.Light0, LightParameter.Specular, Specular);
        }

        public Matrix4 LightProjectView()
        {
            return biasMatrix * lightProjectMatrix * lightViewMatrix;
        }

        public void EnableLightDepthMap()
        {
            // generate render object
            var material = new Material();
            material.ShaderProgram = Resource.Instance.ErrorShader;
            Render render = Render.CreateRender(material, delegate (Render r) {
                var m = r.MaterialData;

                if (m.ShaderProgram != 0)
                {
                    GL.UseProgram(m.ShaderProgram);
                }
            },
            this,
            null,
            Render.Prerender);
            RenderList.Add(render);

        }

        private void updateLightMatrix()
        {
            lightViewMatrix = Matrix4.Transpose(Matrix4.LookAt(LocalPosition, Vector3.Zero, Vector3.UnitY));
            var c = GameDirect.Instance.MainScene.MainCamera;
            lightProjectMatrix = Matrix4.Transpose(Matrix4.CreateOrthographic(c.Viewport.Width, c.Viewport.Height, c.zNear, c.zFar));
        }

        public override void OnRenderBegin(FrameEventArgs e)
        {
            regViewMatrix = GameDirect.Instance.MainScene.MainCamera.ViewMatrix;
            GameDirect.Instance.MainScene.MainCamera.ViewMatrix = lightViewMatrix;

            regProjectMatrix = GameDirect.Instance.MainScene.MainCamera.ProjectMatix;
            GameDirect.Instance.MainScene.MainCamera.ProjectMatix = lightProjectMatrix;

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, GameDirect.Instance.DepthBuffer.FB);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            
        }

        public override void OnRenderFinsh(FrameEventArgs e)
        {
            GameDirect.Instance.MainScene.MainCamera.ViewMatrix = regViewMatrix;
            GameDirect.Instance.MainScene.MainCamera.ProjectMatix = regProjectMatrix;
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

        }

    }
}
