using MyRender.Debug;
using OpenTK.Graphics.OpenGL;
using System;

namespace MyRender.MyEngine
{
    class FrameBuffer
    {
        public enum Type
        {
            ShadowmapFrame,
            GaussianXFrame,
            GaussianYFrame,
            GBuffer,
            SSAOFrame,
            GaussianRXFrame,
            GaussianRYFrame,
            ReflectionFrame,
            RefractionFrame
        }


        public int FB = 0;
        public int DB_Texture = 0;
        public int CB_Texture = 0;
        public int Position = 0;
        public int Normal = 0;
        public int DB = 0;
        public Type type;

        public void GenGaussianRFrame()
        {
            // color
            CB_Texture = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, CB_Texture);
            GL.TexParameter(TextureTarget.Texture2D,
                            TextureParameterName.TextureMinFilter,
                            (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D,
                            TextureParameterName.TextureMagFilter,
                            (int)TextureMagFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D,
                            TextureParameterName.TextureWrapS,
                            (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D,
                            TextureParameterName.TextureWrapT,
                            (int)TextureWrapMode.ClampToEdge);

            var vp = GameDirect.Instance.MainScene.MainCamera.Viewport;
            GL.TexImage2D(TextureTarget.Texture2D, 0,
            PixelInternalFormat.R32f,
            vp.Width,
            vp.Height,
            0,
            PixelFormat.Red,
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
                Log.Print("GenGaussianRFrame fail...");

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        public void GenSSAOFrame()
        {
            // color
            CB_Texture = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, CB_Texture);
            GL.TexParameter(TextureTarget.Texture2D,
                            TextureParameterName.TextureMinFilter,
                            (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D,
                            TextureParameterName.TextureMagFilter,
                            (int)TextureMagFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D,
                            TextureParameterName.TextureWrapS,
                            (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D,
                            TextureParameterName.TextureWrapT,
                            (int)TextureWrapMode.ClampToEdge);

            var vp = GameDirect.Instance.MainScene.MainCamera.Viewport;
            GL.TexImage2D(TextureTarget.Texture2D, 0,
            PixelInternalFormat.R32f,
            vp.Width,
            vp.Height,
            0,
            PixelFormat.Red,
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
                Log.Print("GenSSAOFrame fail...");

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

        }

        public void GenGBuffer()
        {
            // position
            Position = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, Position);
            GL.TexParameter(TextureTarget.Texture2D,
                            TextureParameterName.TextureMinFilter,
                            (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D,
                            TextureParameterName.TextureMagFilter,
                            (int)TextureMagFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D,
                            TextureParameterName.TextureWrapS,
                            (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D,
                            TextureParameterName.TextureWrapT,
                            (int)TextureWrapMode.ClampToEdge);


            var vp = GameDirect.Instance.MainScene.MainCamera.Viewport;
            GL.TexImage2D(TextureTarget.Texture2D, 0,
            PixelInternalFormat.Rgb16f,
            vp.Width,
            vp.Height,
            0,
            PixelFormat.Rgb,
            PixelType.Float,
            IntPtr.Zero);
            GL.BindTexture(TextureTarget.Texture2D, 0);

            // normal
            Normal = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, Normal);
            GL.TexParameter(TextureTarget.Texture2D,
                            TextureParameterName.TextureMinFilter,
                            (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D,
                            TextureParameterName.TextureMagFilter,
                            (int)TextureMagFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D,
                            TextureParameterName.TextureWrapS,
                            (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D,
                            TextureParameterName.TextureWrapT,
                            (int)TextureWrapMode.ClampToEdge);

            GL.TexImage2D(TextureTarget.Texture2D, 0,
            PixelInternalFormat.Rgb16f,
            vp.Width,
            vp.Height,
            0,
            PixelFormat.Rgb,
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
                                        Position, 0);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer,
                                        FramebufferAttachment.ColorAttachment1,
                                        TextureTarget.Texture2D,
                                        Normal, 0);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer,
                                        FramebufferAttachment.DepthAttachment,
                                        RenderbufferTarget.Renderbuffer,
                                        DB);

            DrawBuffersEnum[] draw = { DrawBuffersEnum.ColorAttachment0, DrawBuffersEnum.ColorAttachment1 };
            GL.DrawBuffers(2, draw);

            if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
                Log.Print("GenGBuffer fail...");

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

        }

        public void GenRGBColorDepthFrame()
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
                            (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D,
                            TextureParameterName.TextureWrapT,
                            (int)TextureWrapMode.Repeat);

            var vp = GameDirect.Instance.MainScene.MainCamera.Viewport;
            GL.TexImage2D(TextureTarget.Texture2D, 0,
            PixelInternalFormat.Rgb,
            vp.Width,
            vp.Height,
            0,
            PixelFormat.Rgb,
            PixelType.Byte,
            IntPtr.Zero);
            GL.BindTexture(TextureTarget.Texture2D, 0);

            // depth
            DB_Texture = GL.GenTexture();
            GL.BindTexture( TextureTarget.Texture2D, DB_Texture);
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
            GL.TexImage2D(TextureTarget.Texture2D, 0,
                PixelInternalFormat.DepthComponent,
                vp.Width,
                vp.Height,
                0,
                PixelFormat.DepthComponent,
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
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer,
                FramebufferAttachment.DepthAttachment,
                TextureTarget.Texture2D,
                DB_Texture, 0);

            if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
                Log.Print("GenGaussianFrame fail...");

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        public void GenGaussianFrame()
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
                            (int)TextureWrapMode.ClampToBorder);
            GL.TexParameter(TextureTarget.Texture2D,
                            TextureParameterName.TextureWrapT,
                            (int)TextureWrapMode.ClampToBorder);
            float[] borderColor = { 1.0f, 1.0f, 1.0f, 1.0f };
            GL.TexParameter(TextureTarget.Texture2D,
                            TextureParameterName.TextureBorderColor,
                            borderColor);

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
                Log.Print("GenGaussianFrame fail...");

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        public void GenShadowmapFrame()
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
            // prevent over sampling problem 
            // https://learnopengl.com/#!Advanced-Lighting/Shadows/Shadow-Mapping
            GL.TexParameter(TextureTarget.Texture2D,
                            TextureParameterName.TextureWrapS,
                            (int)TextureWrapMode.ClampToBorder);
            GL.TexParameter(TextureTarget.Texture2D,
                            TextureParameterName.TextureWrapT,
                            (int)TextureWrapMode.ClampToBorder);
            float[] borderColor = { 1.0f, 1.0f, 1.0f, 1.0f };
            GL.TexParameter(TextureTarget.Texture2D,
                            TextureParameterName.TextureBorderColor,
                            borderColor);

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
                Log.Print("GenShadowmapFrame fail...");

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        public void Release()
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

            if(Position != 0)
            {
                GL.DeleteTexture(Position);
            }

            if (Normal != 0)
            {
                GL.DeleteTexture(Normal);
            }


        }
    }
}
