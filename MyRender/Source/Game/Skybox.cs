using MyRender.MyEngine;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System.Runtime.InteropServices;

namespace MyRender.Game
{
    class Skybox : Node
    {
        private readonly string skyboxGUID = "skybox";

        public override void OnStart()
        {
            base.OnStart();

            //set model
            ModelList = new Model[1];
            var modelData = Resource.Instance.GetModel(skyboxGUID);
            if (modelData == null)
            {
                modelData = Model.CreateCubeData();
                modelData.guid = skyboxGUID;

                // gen vertex buffer
                modelData.VBO = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ArrayBuffer, modelData.VBO);
                int size = modelData.Vertices.Length * Marshal.SizeOf(default(Vector3));
                GL.BufferData(BufferTarget.ArrayBuffer, size, modelData.Vertices, BufferUsageHint.StaticDraw);

                Resource.Instance.AddModel(modelData);
            }
            ModelList[0] = modelData;

            // set material
            MaterialData = Resource.Instance.CreateSkyboxM();

            //set shader action
            SetUpShaderAction.Add(skyboxGUID, delegate ()
            {

                if (MaterialData.ShaderProgram != 0)
                {
                    GL.UseProgram(MaterialData.ShaderProgram);

                    var variable = GL.GetUniformLocation(MaterialData.ShaderProgram, "cubemapTexture");
                    GL.ActiveTexture(TextureUnit.Texture0);
                    GL.BindTexture(TextureTarget.TextureCubeMap, MaterialData.TextureArray[Material.TextureType.Cubemap]);
                    GL.Uniform1(variable, 0);

                }

            });
        }


        public override void OnRender(FrameEventArgs e)
        {
            base.OnRender(e);

            GL.DepthFunc( DepthFunction.Lequal);

            if (MaterialData == null) return;
            if (SetUpShaderAction.ContainsKey(skyboxGUID)) SetUpShaderAction[skyboxGUID]();

            GL.Color4(Color4.White);  //byte型で指定

            // bind vertex buffer 
            GL.BindBuffer(BufferTarget.ArrayBuffer, ModelList[0].VBO);
            GL.EnableClientState(ArrayCap.VertexArray);
            GL.VertexPointer(3, VertexPointerType.Float, 0, 0);


            //GL.BindTexture(TextureTarget.Texture2D, MaterialData.TextureID);
            GL.DrawArrays(ModelList[0].DrawType, 0, ModelList[0].Vertices.Length);

            GL.DisableClientState(ArrayCap.VertexArray);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindTexture(TextureTarget.TextureCubeMap, 0);

            GL.DepthFunc(DepthFunction.Less);

        }
    }

}