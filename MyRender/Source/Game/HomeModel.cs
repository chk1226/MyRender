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

            MaterialData = Resource.Instance.CreateHomeM();
            Rotation(1, 0, 0, -90);

            foreach (var model in ModelList)
            {
                SetUpShaderAction.Add(model.id, delegate ()
                {
                    if (MaterialData.ShaderProgram != 0)
                    {
                        GL.UseProgram(MaterialData.ShaderProgram);

                        int normal_id = MaterialData.TextureArray[Material.TextureType.Normal];

                        //MaterialData.UniformTexture("TEX_COLOR", TextureUnit.Texture0, Material.TextureType.Color, 0);
                        MaterialData.UniformTexture("NORMAL_TEX_COLOR", TextureUnit.Texture0, Material.TextureType.Normal, 0);
                        MaterialData.UniformTexture("TEX_SPECULAR", TextureUnit.Texture1, Material.TextureType.Specular, 1);
                        var view_mat = GameDirect.Instance.MainScene.MainCamera.ViewMatrix;
                        MaterialData.UniformMatrix4("VIEW_MAT", ref view_mat, true);

                    }
                });

            }


            return true;
        }

        public override void OnRender(FrameEventArgs e)
        {
            base.OnRender(e);

            foreach (var model in ModelList)
            {

                if (SetUpShaderAction.ContainsKey(model.id)) SetUpShaderAction[model.id]();

                // bind vertex buffer 
                GL.BindBuffer(BufferTarget.ArrayBuffer, model.VBO);
                GL.EnableClientState(ArrayCap.VertexArray);
                GL.VertexPointer(3, VertexPointerType.Float, 0, 0);

                // bind normal buffer
                GL.BindBuffer(BufferTarget.ArrayBuffer, model.NBO);
                GL.EnableClientState(ArrayCap.NormalArray);
                GL.NormalPointer(NormalPointerType.Float, 0, 0);

                // bind texture coord buffer
                GL.BindBuffer(BufferTarget.ArrayBuffer, model.TBO);
                GL.EnableClientState(ArrayCap.TextureCoordArray);
                GL.TexCoordPointer(2, TexCoordPointerType.Float, 0, 0);

                // tangent buffer
                var tangent = GL.GetAttribLocation(MaterialData.ShaderProgram, "tangent");
                GL.EnableVertexAttribArray(tangent);
                GL.BindBuffer(BufferTarget.ArrayBuffer, model.TangentBuffer);
                GL.VertexAttribPointer(tangent, 3, VertexAttribPointerType.Float, false, 0, 0);

   
                //GL.BindTexture(TextureTarget.Texture2D, MaterialData.TextureID);
                GameDirect.Instance.DrawCall(model.DrawType, model.Vertices.Length);

                GL.DisableClientState(ArrayCap.VertexArray);
                GL.DisableClientState(ArrayCap.NormalArray);
                GL.DisableClientState(ArrayCap.TextureCoordArray);

                GL.DisableVertexAttribArray(tangent);

                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
                GL.BindTexture(TextureTarget.Texture2D, 0);
            }


        }

    }
}
