﻿using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace MyRender.MyEngine
{
    class ColorPlane : Plane
    {
        public ColorPlane(float width, float height) : base(width, height, 1, 1)
        {
            LocalPosition = new Vector3(-width / 2, 0, -height / 2);
        }

        public override void OnStart()
        {
            base.OnStart();

            var modelData = ModelList[0];

            var material = new Material();
            material.ShaderProgram = Resource.Instance.GetShader(Resource.SColor);

            Render render = Render.CreateRender(material, delegate (Render r) {
                var m = r.MaterialData;

                if (m.ShaderProgram != 0)
                {
                    GL.UseProgram(m.ShaderProgram);

                    m.Uniform3("Color", 0.5f, 0.5f, 0.5f);

                }
            },
            this,
            modelData,
            Render.Normal);

            RenderList.Add(render);

        }

    }
}
