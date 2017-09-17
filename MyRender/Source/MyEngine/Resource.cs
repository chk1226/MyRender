using System.Drawing;
using System.IO;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;
using System.Drawing.Imaging;
using MyRender.Debug;
using System;

namespace MyRender.MyEngine
{
    partial class Resource
    {

        static private Resource _instance;
        static public Resource Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Resource();
                }
                return _instance;
            }
        }

        private string _currentPath;
        public string CurrentPath
        {
            get { return _currentPath; }
        }
        private int errorShader;
        public int ErrorShader { get { return errorShader; } }
        private readonly string SError = @"Source\Shader\Error.glsl";
        private Dictionary<string, int> _texArray = new Dictionary<string, int>();
        private Dictionary<string, Material> _materialArray = new Dictionary<string, Material>();
        private Dictionary<string, Model> _modellArray = new Dictionary<string, Model>();
        private Dictionary<string, int> _shaderArray = new Dictionary<string, int>();
        private Dictionary<string, UIFont.Glyphes> _fontArray = new Dictionary<string, UIFont.Glyphes>();
        private Dictionary<FrameBuffer.Type, FrameBuffer> _frameBuffer = new Dictionary<FrameBuffer.Type, FrameBuffer>();

        private string[] cubemapOrder = { "rt", "lf", "up", "dn", "bk", "ft" };

        private Resource()
        {
            _currentPath = Directory.GetCurrentDirectory();

            _currentPath = _currentPath.Remove(_currentPath.IndexOf("bin"));
            errorShader = SetUpShader(SError);
        }

        public FrameBuffer GetFrameBuffer(FrameBuffer.Type type)
        {
            if(_frameBuffer.ContainsKey(type))
            {
                return _frameBuffer[type];
            }

            FrameBuffer f = new FrameBuffer();
            switch(type)
            {
                case FrameBuffer.Type.ShadowmapFrame:
                    f.GenShadowmapFrame();
                    break;
                case FrameBuffer.Type.GaussianXFrame:
                    f.GenGaussianFrame();
                    break;
                case FrameBuffer.Type.GaussianYFrame:
                    f.GenGaussianFrame();
                    break;
                case FrameBuffer.Type.GBufferPN:
                    f.GenGBufferPN();
                    break;
                case FrameBuffer.Type.GBufferPNC:
                    f.GenGBufferPNC();
                    break;
                case FrameBuffer.Type.SSAOFrame:
                    f.GenSSAOFrame();
                    break;
                case FrameBuffer.Type.GaussianRXFrame:
                    f.GenGaussianFrame();
                    break;
                case FrameBuffer.Type.GaussianRYFrame:
                    f.GenGaussianFrame();
                    break;
                case FrameBuffer.Type.ReflectionFrame:
                    f.GenRGBColorDepthFrame();
                    break;
                case FrameBuffer.Type.RefractionFrame:
                    f.GenRGBColorDepthFrame();
                    break;
                case FrameBuffer.Type.RGBFColorDepth:
                    f.GenRGBFColorDepthFrame();
                    break;
                case FrameBuffer.Type.RGBFColorDepth2:
                    f.GenRGBFColorDepthFrame();
                    break;
                case FrameBuffer.Type.RGBFColorDepth3:
                    f.GenRGBFColorDepthFrame();
                    break;
                case FrameBuffer.Type.RGBFColorDepth4:
                    f.GenRGBFColorDepthFrame();
                    break;
                case FrameBuffer.Type.RGBFColorDepth5:
                    f.GenRGBFColorDepthFrame();
                    break;
                case FrameBuffer.Type.RGBAFColorDepth:
                    f.GenRGBAFColorDepthFrame();
                    break;
                case FrameBuffer.Type.RGBAFColorDepth2:
                    f.GenRGBAFColorDepthFrame();
                    break;
                case FrameBuffer.Type.RGBAFColorDepth3:
                    f.GenRGBAFColorDepthFrame();
                    break;
                case FrameBuffer.Type.RGBAFColorDepth4:
                    f.GenRGBAFColorDepthFrame();
                    break;
                default:
                    f = null;
                    break;
            }

            if( f != null)
            {
                f.type = type;
                _frameBuffer.Add(f.type, f);
            }

            return f;
        }

        public UIFont.Glyphes GetFont(string bitmap, string xml)
        {
            var key = bitmap + xml;

            if (_fontArray.ContainsKey(key))
            {
                return _fontArray[key];
            }

            var glyphes = new UIFont.Glyphes();
            
            if (!glyphes.Loader(bitmap, xml))
            {
                return null;
            }

            _fontArray.Add(key, glyphes);

            return glyphes;
        }

        public int GetShader(string str)
        {
            if(_shaderArray.ContainsKey(str))
            {
                return _shaderArray[str];
            }

            var newShader = SetUpShader(str);
            if(newShader == 0)
            {
                return errorShader;
            }

            _shaderArray.Add(str, newShader);

            return newShader;
        }

        public Model GetModel(string str)
        {
            if (_modellArray.ContainsKey(str))
            {
                return _modellArray[str];
            }

            return null;
        }

        public void AddModel(Model model)
        {
            if (model == null) return;

            _modellArray.Add(model.guid, model);
        }

        public Material GetMaterial(string str)
        {
            if (_materialArray.ContainsKey(str))
            {
                return _materialArray[str];
            }

            return null;
        }

        public void AddMaterial(Material material)
        {
            if (material == null) return;

            _materialArray.Add(material.guid, material);
        }

        public int GetCubemapTextureID(string str)
        {
            if (_texArray.ContainsKey(str))
            {
                return _texArray[str];
            }

            var id = LoadCubemapTexture(str);
            if (id != 0)
            {
                AddTexture(str, id);
            }

            return id;

        }


        public int GetTextureID(string str, bool flipY = true)
        {
            if (_texArray.ContainsKey(str))
            {
                return _texArray[str];
            }

            var id = LoadTexture(str, flipY);
            if (id != 0)
            {
                AddTexture(str, id);
            }

            return id;

        }

        public int LoadCubemapTexture(string file_name)
        {
            int id = GL.GenTexture();
            GL.BindTexture(TextureTarget.TextureCubeMap, id);

            try
            {

                for (int i = 0; i < cubemapOrder.Length; i++)
                {
                    var pathName = string.Format(_currentPath + file_name, cubemapOrder[i]);
                    using (Bitmap bmp = new Bitmap(pathName))
                    {
                        //png画像の反転を直す
                        //bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);
                        BitmapData bmp_data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
                                                        ImageLockMode.ReadOnly,
                                                        System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                        GL.TexImage2D(TextureTarget.TextureCubeMapPositiveX + i, 0,
                                    PixelInternalFormat.Rgba,
                                    bmp_data.Width, bmp_data.Height, 0,
                                    OpenTK.Graphics.OpenGL.PixelFormat.Bgra,
                                    PixelType.UnsignedByte, bmp_data.Scan0);

                        bmp.UnlockBits(bmp_data);
                    }


                    GL.TexParameter(TextureTarget.TextureCubeMap,
                                    TextureParameterName.TextureMinFilter,
                                    (int)TextureMinFilter.Linear);
                    GL.TexParameter(TextureTarget.TextureCubeMap,
                                    TextureParameterName.TextureMagFilter,
                                    (int)TextureMagFilter.Linear);
                    GL.TexParameter(TextureTarget.TextureCubeMap,
                                    TextureParameterName.TextureWrapS,
                                    (int)TextureWrapMode.ClampToEdge);
                    GL.TexParameter(TextureTarget.TextureCubeMap,
                                    TextureParameterName.TextureWrapT,
                                    (int)TextureWrapMode.ClampToEdge);
                    GL.TexParameter(TextureTarget.TextureCubeMap,
                                    TextureParameterName.TextureWrapR,
                                    (int)TextureWrapMode.ClampToEdge);
                }

            }
            catch (FileNotFoundException)
            {
                Log.Print("[LoadCubemapTexture]FileNotFound filename : " + file_name);
                return 0;
            }
            catch (Exception e)
            {
                Log.Print("[LoadCubemapTexture] " + e.Message);
                return 0;
            }


            GL.BindTexture(TextureTarget.Texture2D, 0);

            return id;


        }

        public int LoadTexture(string file_name, bool flipY)
        {
            int id = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, id);
            try {
                using (Bitmap bmp = new Bitmap(_currentPath + file_name))
                {
                    //png画像の反転を直す
                    if(flipY)
                    {
                        bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);
                    }
                    BitmapData bmp_data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
                                                    ImageLockMode.ReadOnly,
                                                    System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                    GL.TexImage2D(TextureTarget.Texture2D, 0,
                                PixelInternalFormat.Rgba,
                                bmp_data.Width, bmp_data.Height, 0,
                                OpenTK.Graphics.OpenGL.PixelFormat.Bgra,
                                PixelType.UnsignedByte, bmp_data.Scan0);

                    bmp.UnlockBits(bmp_data);
                }

            }
            catch (FileNotFoundException)
            {
                Log.Print("[Loadtexture]FileNotFound filename : " + file_name);
                return 0;
            }
            catch(Exception e)
            {
                Log.Print("[Loadtexture] " + e.Message);
                return 0;
            }

            //GL.Ext.GenerateMipmap(GenerateMipmapTarget.Texture2D);
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

            GL.BindTexture(TextureTarget.Texture2D, 0);

            return id;
        }

        public void AddTexture(string str, int id)
        {
            if(!_texArray.ContainsKey(str))
            {
                _texArray.Add(str, id);
            }
        }
       

        public void DeleteTexture(int id)
        {
            GL.DeleteTexture(id);
        }

        public int SetUpShader(string filename)
        {
            int vert, frag, geom = 0;
            string v_s = "", f_s = "", g_s = "";
            vert = GL.CreateShader(ShaderType.VertexShader);
            frag = GL.CreateShader(ShaderType.FragmentShader);
            int program = GL.CreateProgram();

            loadShaderFile(filename, ref v_s, ref f_s, ref g_s);
            GL.ShaderSource(vert, v_s);
            GL.ShaderSource(frag, f_s);
            GL.CompileShader(vert);
            GL.CompileShader(frag);

            int result;
            GL.GetShader(vert, ShaderParameter.CompileStatus, out result);
            if (result == 0)
            {
                Log.Print("[SetUpShader][VertexShader)] : " + GL.GetShaderInfoLog(vert));
                return 0;
            }

            GL.GetShader(frag, ShaderParameter.CompileStatus, out result);
            if (result == 0)
            {
                Log.Print("[SetUpShader][FragmentShader)] :" + GL.GetShaderInfoLog(frag));
                return 0;
            }

            GL.AttachShader(program, vert);
            GL.AttachShader(program, frag);

            if (g_s != "")
            {
                geom = GL.CreateShader(ShaderType.GeometryShader);
                GL.ShaderSource(geom, g_s);
                GL.CompileShader(geom);
                GL.GetShader(geom, ShaderParameter.CompileStatus, out result);
                if (result == 0)
                {
                    Log.Print("[SetUpShader][GeometryShader)] :" + GL.GetShaderInfoLog(geom));
                    return 0;
                }
                GL.AttachShader(program, geom);

            }

            GL.LinkProgram(program);

            GL.GetProgram(program, GetProgramParameterName.LinkStatus, out result);
            if (result == 0)
            {
                Log.Print("[SetUpShader][ShderProgram] :" + GL.GetProgramInfoLog(program));
                return 0;
            }
            GL.DeleteShader(vert);
            GL.DeleteShader(frag);

            if(geom != 0)
            {
                GL.DeleteShader(geom);
            }

            return program;
        }

        private void loadShaderFile(string file, ref string v_s, ref string f_s, ref string g_s)
        {
            string s;

            try
            {
                using (StreamReader reader = new StreamReader(_currentPath + file))
                {

                    s = reader.ReadLine();

                    if (s == @"@vertex shader")
                    {
                        s = reader.ReadLine();

                        while (s != @"@fragment shader" && s != @"@geometry shader")
                        {
                            v_s += s;
                            v_s += "\n";
                            s = reader.ReadLine();
                        }

                        if(s == @"@geometry shader")
                        {
                            s = reader.ReadLine();

                            while (s != @"@fragment shader")
                            {
                                g_s += s;
                                g_s += "\n";
                                s = reader.ReadLine();
                            }
                        }
                    }

                    f_s = reader.ReadToEnd();

                }
            }
            catch (Exception e)
            {
                Log.Print("[loadShaderFile] :" + e.ToString());
            }

        }

        public void ReleaseTextures()
        {
            foreach (var tex in _texArray)
            {
                GL.DeleteTexture(tex.Value);
            }
            _texArray.Clear();
        }

        public void ReleaseMaterial()
        {
            foreach (var m in _materialArray)
            {
                m.Value.Release();
            }
            _materialArray.Clear();
        }

        public void ReleaseModels()
        {
            foreach (var m in _modellArray)
            {
                m.Value.Release();
            }
            _modellArray.Clear();
        }

        public void ReleaseShaders()
        {
            foreach (var m in _shaderArray)
            {
                GL.DeleteProgram(m.Value);
            }
            _shaderArray.Clear();
        }

        public void ReleaseFont()
        {
            foreach (var m in _fontArray)
            {
                m.Value.Release();
            }
            _fontArray.Clear();
        }

        public void ReleaseFrameBuffer()
        {
            foreach (var m in _frameBuffer)
            {
                m.Value.Release();
            }
            _frameBuffer.Clear();
        }

        public void OnRelease()
        {
            ReleaseTextures();
            ReleaseMaterial();
            ReleaseModels();
            ReleaseShaders();
            ReleaseFont();
            ReleaseFrameBuffer();
            GL.DeleteProgram(errorShader);
        }
    }
}
