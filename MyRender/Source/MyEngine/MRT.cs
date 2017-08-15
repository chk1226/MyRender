using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;

namespace MyRender.MyEngine
{
    class MRT : Node
    {
        public override void OnStart()
        {
            base.OnStart();


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

        public override void OnRenderBegin(FrameEventArgs e)
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, Resource.Instance.GetFrameBuffer(FrameBuffer.Type.GBuffer).FB);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        }

        public override void OnRenderFinsh(FrameEventArgs e)
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

        }
    }
}
