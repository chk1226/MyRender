using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace MyRender.MyEngine
{
    class ColorCube : Node
    {
        private readonly string cubeGUID = "ColorCube";

        public Vector3 Color = Vector3.One;

        public override void OnStart()
        {
            base.OnStart();

            ModelList = new Model[1];

            var modelData = Resource.Instance.GetModel(cubeGUID);
            if (modelData == null)
            {
                modelData = Model.CreateCubeData();
                modelData.guid = cubeGUID;
             
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

                Resource.Instance.AddModel(modelData);
            }
            ModelList[0] = modelData;

            // generate material object
            var material = Resource.Instance.GetMaterial(cubeGUID);
            if (material == null)
            {
                material = new Material();
                material.guid = cubeGUID;
                material.ShaderProgram = Resource.Instance.GetShader(Resource.SColor);

                Resource.Instance.AddMaterial(material);
            }


            Render render = Render.CreateRender(material, delegate (Render r) {
                var m = r.MaterialData;

                if (m.ShaderProgram != 0)
                {
                    GL.UseProgram(m.ShaderProgram);

                    m.Uniform3("Color", Color.X, Color.Y, Color.Z);

                }
            },
            this,
            modelData,
            Render.Normal);

            RenderList.Add(render);

        }

    }
}
