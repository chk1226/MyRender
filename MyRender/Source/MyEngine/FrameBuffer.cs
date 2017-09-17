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
            GBufferPNC, // position + normal + color
            GBufferPN,  // position + normal
            SSAOFrame,
            GaussianRXFrame,
            GaussianRYFrame,
            ReflectionFrame,
            RefractionFrame,
            RGBFColorDepth,
            RGBFColorDepth2,
            RGBFColorDepth3,
            RGBFColorDepth4,
            RGBFColorDepth5,
            RGBAFColorDepth,
            RGBAFColorDepth2,
            RGBAFColorDepth3,
            RGBAFColorDepth4,

        }

        public int FB = 0;
        public int DB_Texture = 0;
        public int CB_Texture = 0;
        public int Position = 0;
        public int Normal = 0;
        public int DB = 0;
        //public int Color = 0
        public Type type;

        private int genTexture(TextureMinFilter minFilter, TextureMagFilter magFilter, TextureWrapMode wrapMode,
            PixelInternalFormat pixelInternalFormat, int width, int height,
            PixelFormat pixelFormat, PixelType type, float[] borderColor = null)
        {
            var id = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, id);
            GL.TexParameter(TextureTarget.Texture2D,
                            TextureParameterName.TextureMinFilter,
                            (int)minFilter);
            GL.TexParameter(TextureTarget.Texture2D,
                            TextureParameterName.TextureMagFilter,
                            (int)magFilter);
            GL.TexParameter(TextureTarget.Texture2D,
                            TextureParameterName.TextureWrapS,
                            (int)wrapMode);
            GL.TexParameter(TextureTarget.Texture2D,
                            TextureParameterName.TextureWrapT,
                            (int)wrapMode);

            if(wrapMode == TextureWrapMode.ClampToBorder)
            {
                // prevent over sampling problem 
                // https://learnopengl.com/#!Advanced-Lighting/Shadows/Shadow-Mapping
                GL.TexParameter(TextureTarget.Texture2D,
                TextureParameterName.TextureBorderColor,
                borderColor);
            }

            var vp = GameDirect.Instance.MainScene.MainCamera.Viewport;
            GL.TexImage2D(TextureTarget.Texture2D, 0,
            pixelInternalFormat,
            width,
            height,
            0,
            pixelFormat,
            type,
            IntPtr.Zero);
            GL.BindTexture(TextureTarget.Texture2D, 0);

            return id;
        } 

        public void GenSSAOFrame()
        {
            // color
            var vp = GameDirect.Instance.MainScene.MainCamera.Viewport;
            CB_Texture = genTexture(TextureMinFilter.Nearest, TextureMagFilter.Nearest, TextureWrapMode.ClampToEdge,
                 PixelInternalFormat.R16f, vp.Width, vp.Height, PixelFormat.Red, PixelType.Float);

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

        public void GenGBufferPNC()
        {
            var vp = GameDirect.Instance.MainScene.MainCamera.Viewport;

            // position
            Position = genTexture(TextureMinFilter.Nearest, TextureMagFilter.Nearest, TextureWrapMode.ClampToEdge,
                PixelInternalFormat.Rgb16f, vp.Width, vp.Height, PixelFormat.Rgb, PixelType.Float);

            // normal
            Normal = genTexture(TextureMinFilter.Nearest, TextureMagFilter.Nearest, TextureWrapMode.ClampToEdge,
                PixelInternalFormat.Rgb16f, vp.Width, vp.Height, PixelFormat.Rgb, PixelType.Float);

            // color
            CB_Texture = genTexture(TextureMinFilter.Linear, TextureMagFilter.Linear, TextureWrapMode.ClampToEdge,
                 PixelInternalFormat.Rgba, vp.Width, vp.Height, PixelFormat.Rgba, PixelType.UnsignedByte);

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
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer,
                            FramebufferAttachment.ColorAttachment2,
                            TextureTarget.Texture2D,
                            CB_Texture, 0);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer,
                                        FramebufferAttachment.DepthAttachment,
                                        RenderbufferTarget.Renderbuffer,
                                        DB);

            DrawBuffersEnum[] draw = { DrawBuffersEnum.ColorAttachment0, DrawBuffersEnum.ColorAttachment1, DrawBuffersEnum.ColorAttachment2 };
            GL.DrawBuffers(3, draw);

            if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
                Log.Print("GenGBuffer fail...");

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

        }

        public void GenGBufferPN()
        {
            var vp = GameDirect.Instance.MainScene.MainCamera.Viewport;

            // position
            Position = genTexture(TextureMinFilter.Nearest, TextureMagFilter.Nearest, TextureWrapMode.ClampToEdge,
                PixelInternalFormat.Rgb16f, vp.Width, vp.Height, PixelFormat.Rgb, PixelType.Float);

            // normal
            Normal = genTexture(TextureMinFilter.Nearest, TextureMagFilter.Nearest, TextureWrapMode.ClampToEdge,
                PixelInternalFormat.Rgb16f, vp.Width, vp.Height, PixelFormat.Rgb, PixelType.Float);

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

        public void GenRGBAFColorDepthFrame()
        {
            var vp = GameDirect.Instance.MainScene.MainCamera.Viewport;
            // color
            CB_Texture = genTexture(TextureMinFilter.Linear, TextureMagFilter.Linear, TextureWrapMode.ClampToEdge,
                 PixelInternalFormat.Rgba16f, vp.Width, vp.Height, PixelFormat.Rgba, PixelType.Float);

            // depth
            DB_Texture = genTexture(TextureMinFilter.Linear, TextureMagFilter.Linear, TextureWrapMode.Clamp,
                PixelInternalFormat.DepthComponent, vp.Width, vp.Height, PixelFormat.DepthComponent, PixelType.Float);

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
                Log.Print("GenRGBAFColorDepthFrame fail...");

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        public void GenRGBFColorDepthFrame()
        {
            var vp = GameDirect.Instance.MainScene.MainCamera.Viewport;
            // color
            CB_Texture = genTexture(TextureMinFilter.Linear, TextureMagFilter.Linear, TextureWrapMode.ClampToEdge,
                 PixelInternalFormat.Rgb16f, vp.Width, vp.Height, PixelFormat.Rgb, PixelType.Float);

            // depth
            DB_Texture = genTexture(TextureMinFilter.Linear, TextureMagFilter.Linear, TextureWrapMode.Clamp,
                PixelInternalFormat.DepthComponent, vp.Width, vp.Height, PixelFormat.DepthComponent, PixelType.Float);

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
                Log.Print("GenRGBFColorDepthFrame fail...");

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        public void GenRGBColorDepthFrame()
        {
            var vp = GameDirect.Instance.MainScene.MainCamera.Viewport;

            // color
            CB_Texture = genTexture(TextureMinFilter.Linear, TextureMagFilter.Linear, TextureWrapMode.Repeat,
                PixelInternalFormat.Rgb, vp.Width, vp.Height, PixelFormat.Rgb, PixelType.UnsignedByte);

            // depth
            DB_Texture = genTexture(TextureMinFilter.Linear, TextureMagFilter.Linear, TextureWrapMode.Clamp,
                PixelInternalFormat.DepthComponent, vp.Width, vp.Height, PixelFormat.DepthComponent, PixelType.Float);

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
                Log.Print("GenRGBColorDepthFrame fail...");

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        public void GenGaussianFrame()
        {
            var vp = GameDirect.Instance.MainScene.MainCamera.Viewport;
            // color
            float[] borderColor = { 1.0f, 1.0f, 1.0f, 1.0f };
            CB_Texture = genTexture(TextureMinFilter.Linear, TextureMagFilter.Linear, TextureWrapMode.ClampToBorder,
                PixelInternalFormat.Rg16f, vp.Width, vp.Height, PixelFormat.Rg, PixelType.Float, borderColor);

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
            var vp = GameDirect.Instance.MainScene.MainCamera.Viewport;
            // color
            float[] borderColor = { 1.0f, 1.0f, 1.0f, 1.0f };
            CB_Texture = genTexture(TextureMinFilter.Linear, TextureMagFilter.Linear, TextureWrapMode.ClampToBorder,
                PixelInternalFormat.Rg16f, vp.Width, vp.Height, PixelFormat.Rg, PixelType.Float, borderColor);

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
