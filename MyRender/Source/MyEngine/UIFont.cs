using MyRender.Debug;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Xml;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System.Text.RegularExpressions;

namespace MyRender.MyEngine
{
    class UIFont : UIBase
    {
        #region Glyphes class
        public class Glyphes
        {
            public string BitmapPath;

            private Dictionary<char, GlyphInfo> glyphes = new Dictionary<char, GlyphInfo>();
            private Vector2 bitmapRect = Vector2.Zero;

            public Vector2 BitmapRect { get { return bitmapRect; } }

            public Dictionary<char, GlyphInfo> GlyphesHash
            {
                get { return glyphes; }
            }

            public bool Loader(string ttfBmpPath, string ttfInfoPath)
            {
                XmlDocument loader = new XmlDocument();
                BitmapPath = ttfBmpPath;

                try
                {
                    loader.Load(Resource.Instance.CurrentPath + ttfInfoPath);
                    // parse xml
                    var exture = loader.SelectSingleNode("TTFTExture");
                    if (exture == null) return false;

                    bitmapRect.X = float.Parse(exture.Attributes["FontWidth"].Value);
                    bitmapRect.Y = float.Parse(exture.Attributes["FontHeight"].Value);

                    var root = exture.FirstChild.ChildNodes;

                    for(int i = 0; i < root.Count; i++)
                    {
                        char c = root[i].Attributes["Character"].Value.ToCharArray()[0];

                        var info = new GlyphInfo();
                        var charInfo = root[i].FirstChild;
                        info.Glyph = c;
                        info.XOffset = float.Parse(charInfo.Attributes["XOffset"].Value);
                        info.SOffset = info.XOffset / bitmapRect.X;
                        info.YOffset = float.Parse(charInfo.Attributes["YOffset"].Value);
                        info.TOffset = info.TOffset / bitmapRect.Y;
                        info.Width = float.Parse(charInfo.Attributes["width"].Value);
                        info.SWidth = info.Width / bitmapRect.X;
                        info.Height = float.Parse(charInfo.Attributes["height"].Value);
                        info.THeight = info.Height / bitmapRect.Y;

                        glyphes[c] = info;
                    }

                }
                catch (Exception e)
                {
                    Log.Print("[Glyphes][Loader] " + e.Message);
                    return false;
                }


                return true;
            }

            public void Release()
            {
                glyphes.Clear();
            }
        }
        #endregion

        #region GlyphInfo class
        public class GlyphInfo
        {
            public char Glyph;
            public float XOffset;
            public float YOffset;
            public float Width;
            public float Height;
            public float SOffset;
            public float TOffset;
            public float SWidth;
            public float THeight;
        }
        #endregion

        private Glyphes glyphes;
        private bool reloadBufferArray = false;

        public Glyphes GetGlyphes
        {
            get { return glyphes; }
        }

        private char[] label;
        public string Label
        {
            get
            {
                return new string(label);
            }
            set
            {
                label = value.ToCharArray();
                updateModelData();
                reloadBufferArray = true;
            }
        }

        public UIFont(string ttfBmpPath, string ttfInfoPath, string str)
        {
            glyphes = Resource.Instance.GetFont(ttfBmpPath, ttfInfoPath);

            ModelList = new Model[1];
            var modelData = Resource.Instance.GetModel(GUID);
            if (modelData == null)
            {
                modelData = Model.CreateUIData();
                modelData.guid = GUID;

                // gen vertex buffer
                modelData.GenVerticesBuffer();

                // gen texture cood buffer
                modelData.GenTexcoordsBuffer();

                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
                Resource.Instance.AddModel(modelData);
            }
            ModelList[0] = modelData;

            MaterialData = Resource.Instance.CreateUIFontM(ttfBmpPath);
            Label = str;

            //set shader action
            SetUpShaderAction.Add(GUID, delegate ()
            {
                // reload vertex
                if(reloadBufferArray)
                {
                    modelData.ReloadVerticesBuffer();
                    modelData.ReloadTexcoordsBuffer();
                    GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
                    reloadBufferArray = false;
                }

                if (MaterialData.ShaderProgram != 0)
                {
                    GL.UseProgram(MaterialData.ShaderProgram);

                    MaterialData.UniformTexture("TEX_COLOR", TextureUnit.Texture0, Material.TextureType.Color, 0);
                }

            });

        }

        public override void OnRender(FrameEventArgs e)
        {
            base.OnRender(e);

            if (MaterialData == null) return;
            if (SetUpShaderAction.ContainsKey(GUID)) SetUpShaderAction[GUID]();

            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.Enable(EnableCap.Blend);

            // bind vertex buffer 
            GL.BindBuffer(BufferTarget.ArrayBuffer, ModelList[0].VBO);
            GL.EnableClientState(ArrayCap.VertexArray);
            GL.VertexPointer(3, VertexPointerType.Float, 0, 0);

            // bind texture coord buffer
            GL.BindBuffer(BufferTarget.ArrayBuffer, ModelList[0].TBO);
            GL.EnableClientState(ArrayCap.TextureCoordArray);
            GL.TexCoordPointer(2, TexCoordPointerType.Float, 0, 0);

            GL.DrawArrays(ModelList[0].DrawType, 0, ModelList[0].Vertices.Length);

            GL.DisableClientState(ArrayCap.VertexArray);
            GL.DisableClientState(ArrayCap.TextureCoordArray);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindTexture(TextureTarget.Texture2D, 0);

            GL.Disable(EnableCap.Blend);
        }

        protected override void updateModelData()
        {
            base.updateModelData();
        
            var modelData = ModelList[0];
            var newStr = Regex.Replace(Label, " ", "");
            modelData.Vertices = new Vector3[newStr.Length * 4];
            modelData.Texcoords = new Vector2[newStr.Length * 4];

            float currentX = 0;
            float currentY = glyphes.BitmapRect.Y;
            float space = 3;
            int arrayIndex = 0;
            for (int i = 0; i < label.Length; i++)
            {
                if(!glyphes.GlyphesHash.ContainsKey(label[i]))
                {
                    if(label[i] == ' ')
                    {
                        currentX += space * 2;
                        continue;
                    }
                    else
                    {
                        Log.Assert("[UIFont][updateModelData] no this char");
                        continue; 
                    }
                }

                var info = glyphes.GlyphesHash[label[i]];

                modelData.Vertices[arrayIndex * 4] = new Vector3(currentX, currentY, depth);
                modelData.Vertices[arrayIndex * 4 + 1] = new Vector3(currentX, 0, depth);
                modelData.Vertices[arrayIndex * 4 + 2] = new Vector3(currentX + info.Width, 0, depth);
                modelData.Vertices[arrayIndex * 4 + 3] = new Vector3(currentX + info.Width, currentY, depth);
                currentX += info.Width + space;

                modelData.Texcoords[arrayIndex * 4] = new Vector2(info.SOffset, 1);
                modelData.Texcoords[arrayIndex * 4 + 1] = new Vector2(info.SOffset, 0);
                modelData.Texcoords[arrayIndex * 4 + 2] = new Vector2(info.SOffset + info.SWidth, 0);
                modelData.Texcoords[arrayIndex * 4 + 3] = new Vector2(info.SOffset + info.SWidth, 1);
                arrayIndex++;
            }


        }

    }
}
