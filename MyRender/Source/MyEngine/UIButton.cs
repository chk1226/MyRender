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
        private Color4 leaveColor;
        private Color4 enterColor;
        private Color4 nowColor;
        private bool clickState = false;

        private UIFont text;
        public event Action OnClick;


        public UIButton(Rectangle rect, string textureName, Color4 enterColor, Color4 leaveColor, string text) : base(rect)
        {
            LocalPosition = new Vector3(rect.X, rect.Y, 0);
            this.text = new UIFont(Resource.ITTFBitmap, Resource.XTTFBitmap, text);
            this.text.LocalPosition = new Vector3(10, rect.Height/2 - this.text.GetGlyphes.BitmapRect.Y/2, 0);
            AddChild(this.text);

            ModelList = new Model[1];
            this.enterColor = enterColor;
            this.leaveColor = leaveColor;
            nowColor = this.leaveColor;

            var modelData = Resource.Instance.GetModel(guid);
            if (modelData == null)
            {
                modelData = Model.CreateUIData();
                modelData.guid = guid;
                
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
                Resource.Instance.AddModel(modelData);
            }
            ModelList[0] = modelData;
            updateModelData();
            modelData.ReloadBufferVec3Data(Model.BufferType.Vertices);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);


            // generate render object
            Render render = Render.CreateRender(Resource.Instance.CreateUISpriteM(textureName), delegate (Render r) {
                var m = r.MaterialData;

                if (m.ShaderProgram != 0)
                {
                    GL.UseProgram(m.ShaderProgram);

                    m.UniformTexture("TEX_COLOR", TextureUnit.Texture0, Material.TextureType.Color, 0);
                    m.Uniform4("BASE_COLOR", nowColor.R, nowColor.G, nowColor.B, nowColor.A);

                }
            },
            this,
            modelData,
            Render.UI);
            RenderList.Add(render);

        }

        protected override void updateModelData()
        {
            base.updateModelData();
        
            if (ModelList == null || ModelList[0] == null)
            {
                return;
            }

            var m = ModelList[0].GetBufferData(Model.BufferType.Vertices);
            m.vec3Data[0].X = 0;
            m.vec3Data[0].Y = 0;
            m.vec3Data[0].Z = depth;

            m.vec3Data[1].X = 0;
            m.vec3Data[1].Y = rect.Height;
            m.vec3Data[1].Z = depth;

            m.vec3Data[2].X = rect.Width;
            m.vec3Data[2].Y = rect.Height;
            m.vec3Data[2].Z = depth;

            m.vec3Data[3].X = rect.Width;
            m.vec3Data[3].Y = 0;
            m.vec3Data[3].Z = depth;

        }

        private bool clickTest(int x, int y)
        {
            return rect.Contains(x, y);
        }

        public override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            if(clickTest(e.X, e.Y))
            {
                clickState = true;
            }
            else
            {
                clickState = false;
            }
        }

        public override void OnMouseMove(MouseMoveEventArgs e)
        {
            base.OnMouseMove(e);

            if (clickTest(e.X, e.Y))
            {
                nowColor = Algorithm.ColorLearp(ref nowColor, ref enterColor, 0.3f);
            }
            else
            {
                nowColor = Algorithm.ColorLearp(ref nowColor, ref leaveColor, 0.3f);
            }

        }

        public override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);

            if (clickTest(e.X, e.Y) &&
                clickState)
            {
                if(OnClick != null)
                {
                    OnClick();
                }
            }

            clickState = false;
            
        }

    }
}
