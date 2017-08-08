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

            MaterialData = Resource.Instance.CreateBricksM();

            //set shader action
            SetUpShaderAction.Add(setShader, delegate()
            {

                if (MaterialData.ShaderProgram != 0)
                {
                    GL.UseProgram(MaterialData.ShaderProgram);

                    MaterialData.UniformTexture("TEX_COLOR", TextureUnit.Texture0, Material.TextureType.Color, 0);
                    MaterialData.UniformTexture("NORMAL_TEX_COLOR", TextureUnit.Texture1, Material.TextureType.Normal, 1);
                    var view_mat = GameDirect.Instance.MainScene.MainCamera.ViewMatrix;
                    MaterialData.UniformMatrix4("VIEW_MAT", ref view_mat, true);
                }

            });

        }

        public override void OnRender(FrameEventArgs e)
        {
            base.OnRender(e);

            if (MaterialData == null) return;
            if (SetUpShaderAction.ContainsKey(setShader)) SetUpShaderAction[setShader]();

            //GL.Color4(Color4.White);  //byte型で指定

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
            GameDirect.Instance.DrawCall(ModelList[0].DrawType, ModelList[0].Vertices.Length);

            GL.DisableClientState(ArrayCap.VertexArray);
            GL.DisableClientState(ArrayCap.NormalArray);
            GL.DisableClientState(ArrayCap.TextureCoordArray);

            GL.DisableVertexAttribArray(tangent);


            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindTexture(TextureTarget.Texture2D, 0);

        }


    }
}
