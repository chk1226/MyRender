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

        public override void OnStart()
        {
            base.OnStart();

            ModelList = new Model[1];

            var modelData = Resource.Instance.GetModel(cubeGUID);
            if (modelData == null)
            {
                modelData = new Model();
                modelData.guid = cubeGUID;
                modelData.DrawType = PrimitiveType.Quads;

                modelData.Vertices = new[]
                {
                    // front face
                    new Vector3(-1.0f, -1.0f,  1.0f),
                    new Vector3( 1.0f, -1.0f,  1.0f),
                    new Vector3( 1.0f,  1.0f,  1.0f),
                    new Vector3(-1.0f,  1.0f,  1.0f),
                    // back face
                    new Vector3( -1.0f, -1.0f, -1.0f),
                    new Vector3( 1.0f, -1.0f, -1.0f),
                    new Vector3( 1.0f, 1.0f, -1.0f),
                    new Vector3( -1.0f, 1.0f, -1.0f),
                    // top face
                    new Vector3( -1.0f, 1.0f, 1.0f),
                    new Vector3( 1.0f, 1.0f, 1.0f),
                    new Vector3( 1.0f, 1.0f, -1.0f),
                    new Vector3( -1.0f, 1.0f, -1.0f),
                    // bottom face
                    new Vector3( -1.0f, -1.0f, 1.0f),
                    new Vector3( 1.0f, -1.0f, 1.0f),
                    new Vector3( 1.0f, -1.0f, -1.0f),
                    new Vector3( -1.0f, -1.0f, -1.0f),
                    // right face
                    new Vector3(1.0f, -1.0f, 1.0f),
                    new Vector3( 1.0f, -1.0f, -1.0f),
                    new Vector3( 1.0f,  1.0f, -1.0f),
                    new Vector3(1.0f,  1.0f, 1.0f),
                    // left face
                    new Vector3(-1.0f, -1.0f, 1.0f),
                    new Vector3( -1.0f, -1.0f, -1.0f),
                    new Vector3( -1.0f,  1.0f, -1.0f),
                    new Vector3(-1.0f,  1.0f, 1.0f)
                };

                modelData.Normals = new[]
                {
                    new Vector3( 0.0f, 0.0f, 1.0f),
                    new Vector3( 0.0f, 0.0f, 1.0f),
                    new Vector3( 0.0f, 0.0f, 1.0f),
                    new Vector3( 0.0f, 0.0f, 1.0f),

                    new Vector3( 0.0f, 0.0f, -1.0f),
                    new Vector3( 0.0f, 0.0f, -1.0f),
                    new Vector3( 0.0f, 0.0f, -1.0f),
                    new Vector3( 0.0f, 0.0f, -1.0f),

                    new Vector3( 0.0f, 1.0f, 0.0f),
                    new Vector3( 0.0f, 1.0f, 0.0f),
                    new Vector3( 0.0f, 1.0f, 0.0f),
                    new Vector3( 0.0f, 1.0f, 0.0f),

                    new Vector3( 0.0f, -1.0f, 0.0f),
                    new Vector3( 0.0f, -1.0f, 0.0f),
                    new Vector3( 0.0f, -1.0f, 0.0f),
                    new Vector3( 0.0f, -1.0f, 0.0f),

                    new Vector3( 1.0f, 0.0f, 0.0f),
                    new Vector3( 1.0f, 0.0f, 0.0f),
                    new Vector3( 1.0f, 0.0f, 0.0f),
                    new Vector3( 1.0f, 0.0f, 0.0f),

                    new Vector3( -1.0f, 0.0f, 0.0f),
                    new Vector3( -1.0f, 0.0f, 0.0f),
                    new Vector3( -1.0f, 0.0f, 0.0f),
                    new Vector3( -1.0f, 0.0f, 0.0f),
                };

                modelData.Texcoords = new[]
                {
                    new Vector2( 0.0f, 0.0f),
                    new Vector2( 0.0f, 1.0f),
                    new Vector2( 1.0f, 1.0f),
                    new Vector2( 1.0f, 0.0f),

                    new Vector2( 0.0f, 0.0f),
                    new Vector2( 0.0f, 1.0f),
                    new Vector2( 1.0f, 1.0f),
                    new Vector2( 1.0f, 0.0f),

                    new Vector2( 0.0f, 0.0f),
                    new Vector2( 0.0f, 1.0f),
                    new Vector2( 1.0f, 1.0f),
                    new Vector2( 1.0f, 0.0f),

                    new Vector2( 0.0f, 0.0f),
                    new Vector2( 0.0f, 1.0f),
                    new Vector2( 1.0f, 1.0f),
                    new Vector2( 1.0f, 0.0f),

                    new Vector2( 0.0f, 0.0f),
                    new Vector2( 0.0f, 1.0f),
                    new Vector2( 1.0f, 1.0f),
                    new Vector2( 1.0f, 0.0f),

                    new Vector2( 0.0f, 0.0f),
                    new Vector2( 0.0f, 1.0f),
                    new Vector2( 1.0f, 1.0f),
                    new Vector2( 1.0f, 0.0f),
                };

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
                ModelList[0] = modelData;
            } 

            MaterialData = Resource.Instance.CreateBricksM();
        }

        public override void SetUpShader()
        {
            base.SetUpShader();

            if (MaterialData != null && MaterialData.ShaderProgram != 0)
            {
                var variable = GL.GetUniformLocation(MaterialData.ShaderProgram, "TEX_COLOR");
                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindTexture(TextureTarget.Texture2D, MaterialData.TextureID);
                GL.Uniform1(variable, 0);

                variable = GL.GetUniformLocation(MaterialData.ShaderProgram, "NORMAL_TEX_COLOR");
                GL.ActiveTexture(TextureUnit.Texture1);
                GL.BindTexture(TextureTarget.Texture2D, MaterialData.NormalTextureID);
                GL.Uniform1(variable, 1);


                //GL.BindTexture(TextureTarget.Texture2D, 0);

                //variable = GL.GetUniformLocation(MaterialData.ShaderProgram, "light_pos");
                //var pos = GameDirect.Instance.MainScene.MainCamera.ViewMatrix * new Vector4(0, 1000, 0, 0);
                //GL.Uniform3(variable, pos.Xyz);

            }
        }

        public override void OnRender(FrameEventArgs e)
        {
            base.OnRender(e);

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


            //GL.BindTexture(TextureTarget.Texture2D, MaterialData.TextureID);
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
