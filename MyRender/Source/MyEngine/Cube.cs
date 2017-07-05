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

            ModelData = Resource.Instance.GetModel(cubeGUID);
            if (ModelData == null)
            {
                ModelData = new Model();
                ModelData.guid = cubeGUID;
                ModelData.DrawType = PrimitiveType.Quads;

                ModelData.Vertices = new[]
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

                ModelData.Normals = new[]
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

                ModelData.Texcoords = new[]
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

                ModelData.ComputeTangentBasis();

                // gen vertex buffer
                ModelData.VBO = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ArrayBuffer, ModelData.VBO);
                int size = ModelData.Vertices.Length * Marshal.SizeOf(default(Vector3));
                GL.BufferData(BufferTarget.ArrayBuffer, size, ModelData.Vertices, BufferUsageHint.StaticDraw);

                // gen texture cood buffer
                ModelData.TBO = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ArrayBuffer, ModelData.TBO);
                size = ModelData.Texcoords.Length * Marshal.SizeOf(default(Vector2));
                GL.BufferData(BufferTarget.ArrayBuffer, size, ModelData.Texcoords, BufferUsageHint.StaticDraw);

                // gen texture cood buffer
                ModelData.NBO = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ArrayBuffer, ModelData.NBO);
                size = ModelData.Normals.Length * Marshal.SizeOf(default(Vector3));
                GL.BufferData(BufferTarget.ArrayBuffer, size, ModelData.Normals, BufferUsageHint.StaticDraw);

                // gen tangent buffer 
                ModelData.TangentBuffer = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ArrayBuffer, ModelData.TangentBuffer);
                size = ModelData.Tangent.Length * Marshal.SizeOf(default(Vector3));
                GL.BufferData(BufferTarget.ArrayBuffer, size, ModelData.Tangent, BufferUsageHint.StaticDraw);

                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

                Resource.Instance.AddModel(ModelData);
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
            GL.BindBuffer(BufferTarget.ArrayBuffer, ModelData.VBO);
            GL.EnableClientState(ArrayCap.VertexArray);
            GL.VertexPointer(3, VertexPointerType.Float, 0, 0);

            // bind normal buffer
            GL.BindBuffer(BufferTarget.ArrayBuffer, ModelData.NBO);
            GL.EnableClientState(ArrayCap.NormalArray);
            GL.NormalPointer(NormalPointerType.Float, 0, 0);

            // bind texture coord buffer
            GL.BindBuffer(BufferTarget.ArrayBuffer, ModelData.TBO);
            GL.EnableClientState(ArrayCap.TextureCoordArray);
            GL.TexCoordPointer(2, TexCoordPointerType.Float, 0, 0);

            // tangent buffer
            var tangent = GL.GetAttribLocation(MaterialData.ShaderProgram, "tangent");
            GL.EnableVertexAttribArray(tangent);
            GL.BindBuffer(BufferTarget.ArrayBuffer, ModelData.TangentBuffer);
            GL.VertexAttribPointer(tangent, 3, VertexAttribPointerType.Float, false, 0, 0);


            //GL.BindTexture(TextureTarget.Texture2D, MaterialData.TextureID);
            GL.DrawArrays(ModelData.DrawType, 0, ModelData.Vertices.Length);

            GL.DisableClientState(ArrayCap.VertexArray);
            GL.DisableClientState(ArrayCap.NormalArray);
            GL.DisableClientState(ArrayCap.TextureCoordArray);

            GL.DisableVertexAttribArray(tangent);


            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindTexture(TextureTarget.Texture2D, 0);

        }


    }
}
