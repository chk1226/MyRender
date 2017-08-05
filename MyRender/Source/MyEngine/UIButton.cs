using OpenTK;
using System;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;
using MyRender.Debug;
using OpenTK.Input;
using System.Drawing;
using System.Runtime.InteropServices;

namespace MyRender.MyEngine
{
    class UIButton : UIBase
    {
        private readonly string guid = "UIButton";
        private Color4 baseColor;

        public UIButton(Rectangle rect, string textureName, Color4 color) : base(rect)
        {
            ModelList = new Model[1];
            baseColor = Algorithm.ColorNormalize(ref color);

            var modelData = Resource.Instance.GetModel(guid);
            if (modelData == null)
            {
                modelData = Model.CreateUIData();
                modelData.guid = guid;

                // gen vertex buffer
                modelData.GenVerticesBuffer();

                // gen texture cood buffer
                modelData.GenTexcoordsBuffer();

                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
                Resource.Instance.AddModel(modelData);
            }
            ModelList[0] = modelData;

            MaterialData = Resource.Instance.CreateUISpriteM(textureName);
            
            //set shader action
            SetUpShaderAction.Add(guid, delegate ()
            {
                // reload vertex
                modelData.ReloadVerticesBuffer();
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);


                if (MaterialData.ShaderProgram != 0)
                {
                    GL.UseProgram(MaterialData.ShaderProgram);

                    MaterialData.UniformTexture("TEX_COLOR", TextureUnit.Texture0, Material.TextureType.Color, 0);

                    //variable = GL.GetUniformLocation(MaterialData.ShaderProgram, "NORMAL_TEX_COLOR");
                    //GL.ActiveTexture(TextureUnit.Texture1);
                    //GL.BindTexture(TextureTarget.Texture2D, MaterialData.TextureArray[Material.TextureType.Normal]);
                    //GL.Uniform1(variable, 1);

                    MaterialData.Uniform4("BASE_COLOR", baseColor.R, baseColor.G, baseColor.B, baseColor.A);
                    
                }

            });
        }

        public override void OnRender(FrameEventArgs e)
        {
            base.OnRender(e);
            updateVertices();

            if (MaterialData == null) return;
            if (SetUpShaderAction.ContainsKey(guid)) SetUpShaderAction[guid]();
            
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
        }

        private void updateVertices()
        {
            if (ModelList == null || ModelList[0] == null)
            {
                return;
            }

            var m = ModelList[0];
            m.Vertices[0].X = rect.Left;
            m.Vertices[0].Y = rect.Top;

            m.Vertices[1].X = rect.Left;
            m.Vertices[1].Y = rect.Bottom;

            m.Vertices[2].X = rect.Right;
            m.Vertices[2].Y = rect.Bottom;

            m.Vertices[3].X = rect.Right;
            m.Vertices[3].Y = rect.Top;
        }

    }
}
