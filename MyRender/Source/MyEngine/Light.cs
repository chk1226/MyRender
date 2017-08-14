using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;

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
        public bool EnableSadowmap = false;
        public bool IsMove = false;

        private Matrix4 lightViewMatrix;
        private Matrix4 lightProjectMatrix;
        private Matrix4 biasMatrix;
        private float shadowmapResolution = 4;
        private float angle = 0;
        private float radius = 50;

        private Matrix4 regViewMatrix;
        private Matrix4 regProjectMatrix;

        public override void OnStart()
        {
            base.OnStart();

            LocalPosition = new Vector3(50, 100, 100);

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


        public override void OnUpdate(FrameEventArgs e)
        {
            base.OnUpdate(e);

            if(IsMove)
            {
                double x = radius * Math.Cos(angle * Algorithm.Radin);
                double y = radius * Math.Sin(angle * Algorithm.Radin);
                var pos = LocalPosition;
                pos.X = (float)x;
                pos.Z = (float)y;
                LocalPosition = pos;

                updateLightMatrix();

                angle += 0.5f;
            }

        }

        public void SetupLight()
        {
            GL.Light(LightName.Light0, LightParameter.Ambient, Ambient);
            GL.Light(LightName.Light0, LightParameter.Diffuse, Diffuse);
            GL.Light(LightName.Light0, LightParameter.Specular, Specular);
        }

        public Matrix4 LightBiasProjectView()
        {
            return biasMatrix * lightProjectMatrix * lightViewMatrix;
        }

        public void EnableLightShadowMap()
        {
            EnableSadowmap = true;
            // generate render object
            var material = new Material();
            material.ShaderProgram = Resource.Instance.GetShader(Resource.SVSM);
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
            lightProjectMatrix = Matrix4.Transpose(Matrix4.CreateOrthographic(c.Viewport.Width / shadowmapResolution, c.Viewport.Height / shadowmapResolution, c.zNear, c.zFar));
        }

        public override void OnRenderBegin(FrameEventArgs e)
        {
            regViewMatrix = GameDirect.Instance.MainScene.MainCamera.ViewMatrix;
            GameDirect.Instance.MainScene.MainCamera.ViewMatrix = lightViewMatrix;

            regProjectMatrix = GameDirect.Instance.MainScene.MainCamera.ProjectMatix;
            GameDirect.Instance.MainScene.MainCamera.ProjectMatix = lightProjectMatrix;

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, Resource.Instance.GetFrameBuffer( FrameBuffer.Type.ShadowmapFrame).FB);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            //GL.Enable(EnableCap.CullFace);
            //GL.CullFace(CullFaceMode.Front);
        }

        public override void OnRenderFinsh(FrameEventArgs e)
        {
            GameDirect.Instance.MainScene.MainCamera.ViewMatrix = regViewMatrix;
            GameDirect.Instance.MainScene.MainCamera.ProjectMatix = regProjectMatrix;
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

            //GL.Disable(EnableCap.CullFace);
        }

        public Vector3 GetDirectVector()
        {
            var p = new Vector4(LocalPosition, 0);
            p = GameDirect.Instance.MainScene.MainCamera.ViewMatrix * WorldModelMatrix * p;
            p.Normalize();
            return p.Xyz;
        }

    }
}
