using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace MyRender.MyEngine
{
    class Cube : Node
    {
        private readonly string cubeGUID = "cube";

        public override void OnStart()
        {
            base.OnStart();

            ModelList = new Model[1];

            var modelData = Resource.Instance.GetModel(cubeGUID);
            if (modelData == null)
            {
                modelData = Model.CreateCubeData();
                modelData.guid = cubeGUID;
                
                // because normal map 
                modelData.ComputeTangentBasis();

                // gen vertex buffer
                modelData.GenVerticesBuffer();

                // gen texture cood buffer
                modelData.GenTexcoordsBuffer();

                // gen normal texture cood buffer
                modelData.GenNormalBuffer();

                // gen tangent buffer 
                modelData.GenTangentBuffer();

                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

                Resource.Instance.AddModel(modelData);
            } 
            ModelList[0] = modelData;

            // generate render object
            Render render = Render.CreateRender(Resource.Instance.CreateBricksM(), delegate(Render r) {
                var m = r.MaterialData;

                if (m.ShaderProgram != 0)
                {
                    GL.UseProgram(m.ShaderProgram);

                    m.UniformTexture("TEX_COLOR", TextureUnit.Texture0, Material.TextureType.Color, 0);
                    m.UniformTexture("NORMAL_TEX_COLOR", TextureUnit.Texture1, Material.TextureType.Normal, 1);
                    Light Light;
                    if(GameDirect.Instance.MainScene.SceneLight.TryGetTarget(out Light))
                    {
                        var dir = Light.GetDirectVector();
                        m.Uniform3("DIR_LIGHT", dir.X, dir.Y, dir.Z);
                    }

                    if (r.ReplaceRender != null && r.ReplaceRender.Parameter.Count != 0)
                    {
                        var clipPlane = (Vector4)r.ReplaceRender.Parameter[0];
                        if (clipPlane != null)
                        {
                            m.Uniform4("ClipPlane", clipPlane.X, clipPlane.Y, clipPlane.Z, clipPlane.W);
                        }
                    }
                    var modelm = WorldModelMatrix * LocalModelMatrix;
                    m.UniformMatrix4("ModelMatrix", ref modelm, true);

                }
            },
            this,
            modelData,
            Render.Normal);
            render.AddVertexAttribute("tangent", ModelList[0].TangentBuffer, 3, false);

            RenderList.Add(render);
        }



    }
}
