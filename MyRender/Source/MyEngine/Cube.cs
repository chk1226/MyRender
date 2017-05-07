using MyRender.MyEngine;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace MyRender.MyEngine
{
    class Cube : Node
    {
        public override void OnStart()
        {
            base.OnStart();

            MaterialData = new Material();

            MaterialData.Vertices = new[]
            {
                //new Vector3(-1.0f, -1.0f,  1.0f),
                //new Vector3( 1.0f, -1.0f,  1.0f),
                //new Vector3( 1.0f,  1.0f,  1.0f),
                //new Vector3(-1.0f,  1.0f,  1.0f),
                //new Vector3(-1.0f, -1.0f, -1.0f),
                //new Vector3( 1.0f, -1.0f, -1.0f),
                //new Vector3( 1.0f,  1.0f, -1.0f),
                //new Vector3(-1.0f,  1.0f, -1.0f)


                new Vector3(-1.0f, 1.0f, 4.0f),
                new Vector3(-1.0f, -1.0f, 4.0f),
                new Vector3(1.0f, -1.0f, 4.0f),
        };

            MaterialData.Normals = new[]
            {
                new Vector3(-1.0f, -1.0f,  1.0f),
                new Vector3( 1.0f, -1.0f,  1.0f),
                new Vector3( 1.0f,  1.0f,  1.0f),
                new Vector3(-1.0f,  1.0f,  1.0f),
                new Vector3(-1.0f, -1.0f, -1.0f),
                new Vector3( 1.0f, -1.0f, -1.0f),
                new Vector3( 1.0f,  1.0f, -1.0f),
                new Vector3(-1.0f,  1.0f, -1.0f),
            };

            MaterialData.Texcoords = new[]
            {
                new Vector2( 0.0f, 0.0f),
                new Vector2( 0.0f, 1.0f),
                new Vector2( 1.0f, 1.0f),
                new Vector2( 1.0f, 0.0f),
                new Vector2( 1.0f, 1.0f),
                new Vector2( 1.0f, 0.0f),
                new Vector2( 0.0f, 0.0f),
                new Vector2( 0.0f, 1.0f),

            };
        }

        public override void OnRender(FrameEventArgs e)
        {
            base.OnRender(e);

            GL.Color4(Color4.White);  //byte型で指定

            GL.EnableClientState(ArrayCap.VertexArray);
            //GL.EnableClientState(ArrayCap.NormalArray);
            //GL.EnableClientState(ArrayCap.TextureCoordArray);

            GL.BindBuffer(BufferTarget.ArrayBuffer, GameDirect.Instance.VBO);
            int size = MaterialData.Vertices.Length * System.Runtime.InteropServices.Marshal.SizeOf(default(Vector3));
            GL.BufferData(BufferTarget.ArrayBuffer, size, MaterialData.Vertices, BufferUsageHint.StaticDraw);

            GL.VertexPointer(3, VertexPointerType.Float, 0, 0);
            //GL.NormalPointer<Vector3>(NormalPointerType.Float, 0, MaterialData.Normals);
            //GL.TexCoordPointer<Vector2>(2, TexCoordPointerType.Float, 0, MaterialData.Texcoords);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 3);


            GL.DisableClientState(ArrayCap.VertexArray);
            //GL.DisableClientState(ArrayCap.NormalArray);
            //GL.DisableClientState(ArrayCap.TextureCoordArray);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);


        }

    }
}
