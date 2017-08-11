using MyRender.MyEngine;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace MyRender.Game
{
    class HomeModel : DaeModel
    {
        public override bool Loader(string path, bool loadAnimation = true)
        {
            var result = base.Loader(path, loadAnimation);
            if (!result) return result;

            Rotation(1, 0, 0, -90);

            foreach (var model in ModelList)
            {
                // generate render object
                Render render = Render.CreateRender(Resource.Instance.CreateHomeM(), delegate (Render r)
                {
                    var m = r.MaterialData;

                    if (m.ShaderProgram != 0)
                    {
                        GL.UseProgram(m.ShaderProgram);

                        //MaterialData.UniformTexture("TEX_COLOR", TextureUnit.Texture0, Material.TextureType.Color, 0);
                        m.UniformTexture("NORMAL_TEX_COLOR", TextureUnit.Texture0, Material.TextureType.Normal, 0);
                        m.UniformTexture("TEX_SPECULAR", TextureUnit.Texture1, Material.TextureType.Specular, 1);
                        var view_mat = GameDirect.Instance.MainScene.MainCamera.ViewMatrix;
                        m.UniformMatrix4("VIEW_MAT", ref view_mat, true);

                    }
                },
                this,
                model,
                Render.Normal);
                render.AddVertexAttribute("tangent", model.TangentBuffer, 3, false);

                RenderList.Add(render);

            }


            return true;
        }


    }
}
