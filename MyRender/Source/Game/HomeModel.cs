using MyRender.MyEngine;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace MyRender.Game
{
    class HomeModel : DaeModel
    {
        private FrameBuffer useFrame;

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

                        m.UniformTexture("NORMAL_TEX_COLOR", TextureUnit.Texture0, Material.TextureType.Normal, 0);
                        Light light;
                        if (GameDirect.Instance.MainScene.SceneLight.TryGetTarget(out light))
                        {
                            var dir = light.GetDirectVector();
                            m.Uniform3("DIR_LIGHT", dir.X, dir.Y, dir.Z);
                            if (light.EnableSadowmap)
                            {
                                if (useFrame != null) m.UniformTexture("SHADOWMAP", TextureUnit.Texture1, useFrame.CB_Texture, 1);
                                var bmvp = light.LightBiasProjectView() * WorldModelMatrix * LocalModelMatrix;
                                m.UniformMatrix4("LIGHT_BPVM", ref bmvp, true);
                            }
                        }

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

        public void SetFrameBuffer(FrameBuffer use)
        {
            useFrame = use;
        }
    }
}
