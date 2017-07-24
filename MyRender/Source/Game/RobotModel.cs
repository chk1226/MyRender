using MyRender.MyEngine;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace MyRender.Game
{
    class RobotModel : DaeModel
    {

        public override bool Loader(string path)
        {
            var result = base.Loader(path);
            if (!result) return result;

            MaterialData = Resource.Instance.CreateRobotM();

            Rotation(1, 0, 0, -90);

            foreach (var model in ModelList)
            {
                SetUpShaderAction.Add(model.id, delegate()
                {
                    if (MaterialData.ShaderProgram != 0)
                    {
                        GL.UseProgram(MaterialData.ShaderProgram);

                        int color_id;
                        int normal_id;
                        int glow_id;
                        int specular_id;
                        int layer = 2;
                        if (model.id.Contains("02"))
                        {
                            color_id = MaterialData.TextureArray[Material.TextureType.Color_02];
                            normal_id = MaterialData.TextureArray[Material.TextureType.Normal_02];
                            glow_id = MaterialData.TextureArray[Material.TextureType.Glow2];
                            specular_id = MaterialData.TextureArray[Material.TextureType.Specular2];
                            layer = 0;
                        }
                        else //01
                        {
                            color_id = MaterialData.TextureArray[Material.TextureType.Color];
                            normal_id = MaterialData.TextureArray[Material.TextureType.Normal];
                            glow_id = MaterialData.TextureArray[Material.TextureType.Glow];
                            specular_id = MaterialData.TextureArray[Material.TextureType.Specular];
                            layer = 1;
                        }

                        var variable = GL.GetUniformLocation(MaterialData.ShaderProgram, "TEX_COLOR");
                        GL.ActiveTexture(TextureUnit.Texture0);
                        GL.BindTexture(TextureTarget.Texture2D, color_id);
                        GL.Uniform1(variable, 0);

                        variable = GL.GetUniformLocation(MaterialData.ShaderProgram, "NORMAL_TEX_COLOR");
                        GL.ActiveTexture(TextureUnit.Texture1);
                        GL.BindTexture(TextureTarget.Texture2D, normal_id);
                        GL.Uniform1(variable, 1);

                        variable = GL.GetUniformLocation(MaterialData.ShaderProgram, "TEX_GLOW");
                        GL.ActiveTexture(TextureUnit.Texture2);
                        GL.BindTexture(TextureTarget.Texture2D, glow_id);
                        GL.Uniform1(variable, 2);

                        variable = GL.GetUniformLocation(MaterialData.ShaderProgram, "TEX_SPECULAR");
                        GL.ActiveTexture(TextureUnit.Texture3);
                        GL.BindTexture(TextureTarget.Texture2D, specular_id);
                        GL.Uniform1(variable, 3);

                        variable = GL.GetUniformLocation(MaterialData.ShaderProgram, "VIEW_MAT");
                        var view_mat = GameDirect.Instance.MainScene.MainCamera.ViewMatrix;
                        GL.UniformMatrix4(variable, true, ref view_mat);

                        variable = GL.GetUniformLocation(MaterialData.ShaderProgram, "jointTransforms");
                        var jointTransform = Animation.HashJointToArray((uint)layer);
                        GL.UniformMatrix4(variable, jointTransform.Length, false, jointTransform);
                    }
                });

            }

            return true;
        }

        public override void OnUpdate(FrameEventArgs e)
        {
            base.OnUpdate(e);

            if(Animation != null)
            {
                Animation.animator.Update();
            }

        }

        public override void OnRender(FrameEventArgs e)
        {
            base.OnRender(e);
            GL.Color4(Color4.White);  //byte型で指定

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

                // joint indices buffer
                var jointIndices = GL.GetAttribLocation(MaterialData.ShaderProgram, "jointIndices");
                GL.EnableVertexAttribArray(jointIndices);
                GL.BindBuffer(BufferTarget.ArrayBuffer, model.JointBuffer);
                GL.VertexAttribPointer(jointIndices, 4, VertexAttribPointerType.Float, false, 0, 0);
                
                // weight buffer
                var weight = GL.GetAttribLocation(MaterialData.ShaderProgram, "weight");
                GL.EnableVertexAttribArray(weight);
                GL.BindBuffer(BufferTarget.ArrayBuffer, model.WeightBuffer);
                GL.VertexAttribPointer(weight, 4, VertexAttribPointerType.Float, false, 0, 0);
                
                //GL.BindTexture(TextureTarget.Texture2D, MaterialData.TextureID);
                GL.DrawArrays(model.DrawType, 0, model.Vertices.Length);

                GL.DisableClientState(ArrayCap.VertexArray);
                GL.DisableClientState(ArrayCap.NormalArray);
                GL.DisableClientState(ArrayCap.TextureCoordArray);

                GL.DisableVertexAttribArray(tangent);
                GL.DisableVertexAttribArray(jointIndices);
                GL.DisableVertexAttribArray(weight);


                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
                GL.BindTexture(TextureTarget.Texture2D, 0);

            }


        }

    }
}
