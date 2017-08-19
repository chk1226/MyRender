using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using OpenTK.Input;

namespace MyRender.MyEngine
{
    class PreRender : Node
    {
        public enum PreRenderType
        {
            None,
            MRT,
            Reflection,
            Refraction
        }
        private PreRenderType type = PreRenderType.None;
        private Matrix4 regViewMatrix = Matrix4.Identity;
        private Matrix4 regProjectMatrix = Matrix4.Identity;

        private Matrix4 reflectionViewMatrix = Matrix4.Identity;
        private Matrix4 reflectionProjectMatrix = Matrix4.Identity;
        public Matrix4 ReflectionViewMatrix { get { return reflectionViewMatrix; } }
        public Matrix4 ReflectionProjectMatrix { get { return reflectionProjectMatrix; } }

        public float WaterHeight = 0;

        public void SetType(PreRenderType type)
        {
            this.type = type;
        }

        public override void OnStart()
        {
            base.OnStart();

            if(type == PreRenderType.MRT)
            {
                // gen render
                var material = new Material();
                material.ShaderProgram = Resource.Instance.GetShader(Resource.SMRT);
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
            else if(type == PreRenderType.Reflection)
            {
                // gen render
                var material = new Material();
                Render render = Render.CreateRender(material, 
                null,
                this,
                null,
                Render.Prerender);
                render.Parameter.Add(new Vector4(0, 1, 0, -WaterHeight));
                render.PreRenderRange = new Vector2(Render.Normal, Render.Skybox);
                updateReflectionMatrix();

                RenderList.Add(render);
            }
            else if(type == PreRenderType.Refraction)
            {
                // gen render
                var material = new Material();
                Render render = Render.CreateRender(material,
                null,
                this,
                null,
                Render.Prerender);
                render.Parameter.Add(new Vector4(0, -1, 0, WaterHeight));
                render.PreRenderRange = new Vector2(Render.Normal, Render.Skybox);

                RenderList.Add(render);
            }
        }

        public override void OnRenderBegin(FrameEventArgs e)
        {
            if(type == PreRenderType.MRT)
            {
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, Resource.Instance.GetFrameBuffer(FrameBuffer.Type.GBuffer).FB);
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            }
            else if(type == PreRenderType.Reflection)
            {
                regViewMatrix = GameDirect.Instance.MainScene.MainCamera.ViewMatrix;
                GameDirect.Instance.MainScene.MainCamera.ViewMatrix = reflectionViewMatrix;

                regProjectMatrix = GameDirect.Instance.MainScene.MainCamera.ProjectMatix;
                GameDirect.Instance.MainScene.MainCamera.ProjectMatix = reflectionProjectMatrix;

                GL.BindFramebuffer(FramebufferTarget.Framebuffer, Resource.Instance.GetFrameBuffer(FrameBuffer.Type.ReflectionFrame).FB);
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                GL.Enable(EnableCap.ClipDistance0);
            }
            else if(type == PreRenderType.Refraction)
            {
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, Resource.Instance.GetFrameBuffer(FrameBuffer.Type.RefractionFrame).FB);
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                GL.Enable(EnableCap.ClipDistance0);
            }
        }

        public override void OnRenderFinsh(FrameEventArgs e)
        {
            if (type == PreRenderType.Reflection)
            {
                GameDirect.Instance.MainScene.MainCamera.ViewMatrix = regViewMatrix;
                GameDirect.Instance.MainScene.MainCamera.ProjectMatix = regProjectMatrix;
                GL.Disable(EnableCap.ClipDistance0);
            }
            else if (type == PreRenderType.Refraction)
            {
                GL.Disable(EnableCap.ClipDistance0);
            }

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        private void updateReflectionMatrix()
        {
            var c = GameDirect.Instance.MainScene.MainCamera;
            var e = c.eye;
            Vector3 reflecE = new Vector3(e.X, e.Y - (e.Y - WaterHeight) * 2, e.Z);

            reflectionViewMatrix = Matrix4.Transpose(Matrix4.LookAt(reflecE, c.focus, c.vUp));

            // Calculate aspect ratio, checking for divide by zero
            float aspect = 1.0f;
            if (c.Viewport.Height > 0)
            {
                aspect = (float)c.Viewport.Width / c.Viewport.Height;
            }

            reflectionProjectMatrix = Matrix4.Transpose(Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(c.fovy * 2f), aspect, c.zNear, c.zFar));
        }

        public override void OnUpdate(FrameEventArgs e)
        {
            base.OnUpdate(e);

            if(type == PreRenderType.Reflection)
            {
                updateReflectionMatrix();
            }
        }
    }
}
