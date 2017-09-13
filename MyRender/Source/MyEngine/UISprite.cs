using System;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;
using MyRender.Debug;
using OpenTK.Input;
using System.Drawing;
using System.Runtime.InteropServices;
using OpenTK;

namespace MyRender.MyEngine
{
    class UISprite : UIBase
    {
        private readonly string guid = "UISprite";

        public UISprite(Rectangle rect, int textureID) : base(rect)
        {
            LocalPosition = new Vector3(rect.X, rect.Y, 0);

            ModelList = new Model[1];
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
            modelData.ReloadBufferVec3Data( Model.BufferType.Vertices);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            // generate render object
            Render render = Render.CreateRender(Resource.Instance.CreateUISpriteM(textureID, GUID), delegate (Render r) {
                var m = r.MaterialData;

                if (m.ShaderProgram != 0)
                {
                    GL.UseProgram(m.ShaderProgram);

                    m.UniformTexture("TEX_COLOR", TextureUnit.Texture0, Material.TextureType.Color, 0);
                    m.Uniform4("BASE_COLOR", 0, 0, 0, 1);

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

            var m = ModelList[0].GetBufferData( Model.BufferType.Vertices);
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

    }
}
