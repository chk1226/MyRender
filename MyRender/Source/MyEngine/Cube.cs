using MyRender.MyEngine;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System.Runtime.InteropServices;

namespace MyRender.MyEngine
{
    class Cube : Node
    {
        private readonly string cubeGUID = "cube";
        private readonly string setShader = "NormalMapping";

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
                modelData.VBO = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ArrayBuffer, modelData.VBO);
                int size = modelData.Vertices.Length * Marshal.SizeOf(default(Vector3));
                GL.BufferData(BufferTarget.ArrayBuffer, size, modelData.Vertices, BufferUsageHint.StaticDraw);

                // gen texture cood buffer
                modelData.TBO = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ArrayBuffer, modelData.TBO);
                size = modelData.Texcoords.Length * Marshal.SizeOf(default(Vector2));
                GL.BufferData(BufferTarget.ArrayBuffer, size, modelData.Texcoords, BufferUsageHint.StaticDraw);

                // gen texture cood buffer
                modelData.NBO = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ArrayBuffer, modelData.NBO);
                size = modelData.Normals.Length * Marshal.SizeOf(default(Vector3));
                GL.BufferData(BufferTarget.ArrayBuffer, size, modelData.Normals, BufferUsageHint.StaticDraw);

                // gen tangent buffer 
                modelData.TangentBuffer = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ArrayBuffer, modelData.TangentBuffer);
                size = modelData.Tangent.Length * Marshal.SizeOf(default(Vector3));
                GL.BufferData(BufferTarget.ArrayBuffer, size, modelData.Tangent, BufferUsageHint.StaticDraw);

                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

                Resource.Instance.AddModel(modelData);
            } 
            ModelList[0] = modelData;

            MaterialData = Resource.Instance.CreateBricksM();

            //set shader action
            SetUpShaderAction.Add(setShader, delegate()
            {

                if (MaterialData.ShaderProgram != 0)
                {
                    GL.UseProgram(MaterialData.ShaderProgram);

                    var variable = GL.GetUniformLocation(MaterialData.ShaderProgram, "TEX_COLOR");
                    GL.ActiveTexture(TextureUnit.Texture0);
                    GL.BindTexture(TextureTarget.Texture2D, MaterialData.TextureArray[Material.TextureType.Color]);
                    GL.Uniform1(variable, 0);

                    variable = GL.GetUniformLocation(MaterialData.ShaderProgram, "NORMAL_TEX_COLOR");
                    GL.ActiveTexture(TextureUnit.Texture1);
                    GL.BindTexture(TextureTarget.Texture2D, MaterialData.TextureArray[Material.TextureType.Normal]);
                    GL.Uniform1(variable, 1);

                    variable = GL.GetUniformLocation(MaterialData.ShaderProgram, "VIEW_MAT");
                    var view_mat = GameDirect.Instance.MainScene.MainCamera.ViewMatrix;
                    GL.UniformMatrix4(variable, true, ref view_mat);
                }

            });

        }

        public override void OnRender(FrameEventArgs e)
        {
            base.OnRender(e);

            if (MaterialData == null) return;
            if (SetUpShaderAction.ContainsKey(setShader)) SetUpShaderAction[setShader]();

            GL.Color4(Color4.White);  //byte型で指定

            // bind vertex buffer 
            GL.BindBuffer(BufferTarget.ArrayBuffer, ModelList[0].VBO);
            GL.EnableClientState(ArrayCap.VertexArray);
            GL.VertexPointer(3, VertexPointerType.Float, 0, 0);

            // bind normal buffer
            GL.BindBuffer(BufferTarget.ArrayBuffer, ModelList[0].NBO);
            GL.EnableClientState(ArrayCap.NormalArray);
            GL.NormalPointer(NormalPointerType.Float, 0, 0);

            // bind texture coord buffer
            GL.BindBuffer(BufferTarget.ArrayBuffer, ModelList[0].TBO);
            GL.EnableClientState(ArrayCap.TextureCoordArray);
            GL.TexCoordPointer(2, TexCoordPointerType.Float, 0, 0);

            // tangent buffer
            var tangent = GL.GetAttribLocation(MaterialData.ShaderProgram, "tangent");
            GL.EnableVertexAttribArray(tangent);
            GL.BindBuffer(BufferTarget.ArrayBuffer, ModelList[0].TangentBuffer);
            GL.VertexAttribPointer(tangent, 3, VertexAttribPointerType.Float, false, 0, 0);


            //GL.BindTexture(TextureTarget.Texture2D, MaterialData.TextureArray[Material.TextureType.Normal]);
            GL.DrawArrays(ModelList[0].DrawType, 0, ModelList[0].Vertices.Length);

            GL.DisableClientState(ArrayCap.VertexArray);
            GL.DisableClientState(ArrayCap.NormalArray);
            GL.DisableClientState(ArrayCap.TextureCoordArray);

            GL.DisableVertexAttribArray(tangent);


            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindTexture(TextureTarget.Texture2D, 0);

        }


    }
}
