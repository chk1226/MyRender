using MyRender.Debug;
using OpenTK.Graphics.OpenGL;
using System;

namespace MyRender.MyEngine
{
    class FrameBuffer
    {
        public int FB = 0;
        public int DB_Texture = 0;
        public int CB_Texture = 0;
        public int DB = 0;


        public void GenColor32fRG()
        {
            // color
            CB_Texture = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, CB_Texture);
            GL.TexParameter(TextureTarget.Texture2D,
                            TextureParameterName.TextureMinFilter,
                            (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D,
                            TextureParameterName.TextureMagFilter,
                            (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D,
                            TextureParameterName.TextureWrapS,
                            (int)TextureWrapMode.Clamp);
            GL.TexParameter(TextureTarget.Texture2D,
                            TextureParameterName.TextureWrapT,
                            (int)TextureWrapMode.Clamp);
            var vp = GameDirect.Instance.MainScene.MainCamera.Viewport;
            GL.TexImage2D(TextureTarget.Texture2D, 0,
            PixelInternalFormat.Rg32f,
            vp.Width,
            vp.Height,
            0,
            PixelFormat.Rg,
            PixelType.Float,
            IntPtr.Zero);
            GL.BindTexture(TextureTarget.Texture2D, 0);

            //frame buffer
            FB = GL.GenFramebuffer();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, FB);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer,
                                        FramebufferAttachment.ColorAttachment0,
                                        TextureTarget.Texture2D,
                                        CB_Texture, 0);

            if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
                Log.Print("GenDepthBuffer fail...");

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        public void GenDepthColor32fRG()
        {
            // color
            CB_Texture = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, CB_Texture);
            GL.TexParameter(TextureTarget.Texture2D,
                            TextureParameterName.TextureMinFilter,
                            (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D,
                            TextureParameterName.TextureMagFilter,
                            (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D,
                            TextureParameterName.TextureWrapS,
                            (int)TextureWrapMode.Clamp);
            GL.TexParameter(TextureTarget.Texture2D,
                            TextureParameterName.TextureWrapT,
                            (int)TextureWrapMode.Clamp);
            var vp = GameDirect.Instance.MainScene.MainCamera.Viewport;
            GL.TexImage2D(TextureTarget.Texture2D, 0,
            PixelInternalFormat.Rg32f,
            vp.Width,
            vp.Height,
            0,
            PixelFormat.Rg,
            PixelType.Float,
            IntPtr.Zero);
            GL.BindTexture(TextureTarget.Texture2D, 0);

            // depth
            DB = GL.GenRenderbuffer();
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, DB);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer,
                RenderbufferStorage.DepthComponent, 
                vp.Width,
                vp.Height);
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);

            //frame buffer
            FB = GL.GenFramebuffer();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, FB);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer,
                                        FramebufferAttachment.ColorAttachment0,
                                        TextureTarget.Texture2D,
                                        CB_Texture, 0);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer,
                FramebufferAttachment.DepthAttachment,
                RenderbufferTarget.Renderbuffer,
                DB);

            if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
                Log.Print("GenDepthBuffer fail...");

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        //public void GenDepthBuffer()
        //{
        //    GenDepthTexture();

        //    //frame buffer
        //    FB = GL.GenFramebuffer();
        //    GL.BindFramebuffer(FramebufferTarget.Framebuffer, FB);
        //    GL.FramebufferTexture2D( FramebufferTarget.Framebuffer,
        //                                FramebufferAttachment.DepthAttachment,
        //                                TextureTarget.Texture2D,
        //                                DB_Texture, 0);
        //    GL.DrawBuffer(DrawBufferMode.None);

        //    if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
        //        Log.Print("GenDepthBuffer fail...");

        //    GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

        //}

        //public void GenDepthTexture()
        //{
        //    DB_Texture = GL.GenTexture();
        //    GL.BindTexture(TextureTarget.Texture2D, DB_Texture);
        //    GL.TexParameter(TextureTarget.Texture2D,
        //                        TextureParameterName.TextureMinFilter,
        //                        (int)TextureMinFilter.Linear);
        //    GL.TexParameter(TextureTarget.Texture2D,
        //                    TextureParameterName.TextureMagFilter,
        //                    (int)TextureMagFilter.Linear);
        //    GL.TexParameter(TextureTarget.Texture2D,
        //                    TextureParameterName.TextureWrapS,
        //                    (int)TextureWrapMode.Clamp);
        //    GL.TexParameter(TextureTarget.Texture2D,
        //                    TextureParameterName.TextureWrapT,
        //                    (int)TextureWrapMode.Clamp);

        //    var vp = GameDirect.Instance.MainScene.MainCamera.Viewport;
        //    GL.TexImage2D(TextureTarget.Texture2D, 0,
        //    PixelInternalFormat.DepthComponent,
        //    vp.Width,
        //    vp.Height,
        //    0, 
        //    PixelFormat.DepthComponent,
        //    PixelType.Float, 
        //    IntPtr.Zero);

        //    GL.BindTexture(TextureTarget.Texture2D, 0);
        //}

        public void OnRelease()
        {
            if(DB_Texture != 0)
            {
                GL.DeleteTexture(DB_Texture);
            }
            if(FB != 0)
            {
                GL.DeleteFramebuffer(FB);
            }
            if(CB_Texture != 0)
            {
                GL.DeleteTexture(CB_Texture);
            }
            if(DB != 0)
            {
                GL.DeleteRenderbuffer(DB);
            }

        }
    }
}
