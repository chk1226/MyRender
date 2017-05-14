using System.Drawing;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;
using System.Drawing.Imaging;
using MyRender.Debug;

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
        private Dictionary<string, int> _texArray = new Dictionary<string, int>();
        private Dictionary<string, Material> _materialArray = new Dictionary<string, Material>();
        private Dictionary<string, Model> _modellArray = new Dictionary<string, Model>();


        private Resource()
        {
            _currentPath = System.IO.Directory.GetCurrentDirectory();

            _currentPath = _currentPath.Remove(_currentPath.IndexOf("bin"));
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

        public int GetTextureID(string str)
        {
            if (_texArray.ContainsKey(str))
            {
                return _texArray[str];
            }

            var id = LoadTexture(str);
            if (id != 0)
            {
                _texArray.Add(str, id);
            }

            return id;

        }

        public int LoadTexture(string file_name)
        {
            int id = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, id);
            var sss = _currentPath + file_name;
            try {
                using (Bitmap bmp = new Bitmap(_currentPath + file_name))
                {
                    bmp.RotateFlip(RotateFlipType.Rotate90FlipY);
                    bmp.RotateFlip(RotateFlipType.Rotate180FlipX);
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
            catch (System.IO.FileNotFoundException)
            {
                Log.Print("[Loadtexture]FileNotFound filename : " + file_name);
                return 0;
            }
            catch(System.Exception e)
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

        public void DeleteTexture(int id)
        {
            GL.DeleteTexture(id);
        }

        public void OnRelease()
        {
            foreach(var tex in _texArray)
            {
                GL.DeleteTexture(tex.Value);
            }
            _texArray.Clear();

            foreach (var m in _materialArray)
            {
                
            }
            _materialArray.Clear();

            foreach (var m in _modellArray)
            {
                m.Value.Release();
            }
            _modellArray.Clear();
        }
    }
}
