using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;


namespace MyRender.MyEngine
{
    class PreRender : Node
    {
        public enum PreRenderType
        {
            None,
            MRT
        }
        private PreRenderType type = PreRenderType.None;

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
        }

        public override void OnRenderBegin(FrameEventArgs e)
        {
            if(type == PreRenderType.MRT)
            {
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, Resource.Instance.GetFrameBuffer(FrameBuffer.Type.GBuffer).FB);
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            }
        }

        public override void OnRenderFinsh(FrameEventArgs e)
        {
            if(type == PreRenderType.MRT)
            {
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            }
        }

    }
}
