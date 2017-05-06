using MyRender.MyEngine;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using MyRender.Debug;
using OpenTK.Graphics;

namespace MyRender.Game
{
    class BaseRenderScene : Scene
    {
        public override void OnStart()
        {
            base.OnStart();

        }


        public override void OnUpdate(FrameEventArgs e)
        {
            base.OnUpdate(e);


        }

        public override void OnRender(FrameEventArgs e)
        {
            base.OnRender(e);

            //GL.Begin(PrimitiveType.Quads);

            //GL.Color4(Color4.White);                            //色名で指定
            //GL.Vertex3(-1.0f, 1.0f, 4.0f);
            //GL.Color4(Color4.Blue);  //配列で指定
            //GL.Vertex3(-1.0f, -1.0f, 4.0f);
            //GL.Color4(Color4.Red);                  //4つの引数にfloat型で指定
            //GL.Vertex3(1.0f, -1.0f, 4.0f);
            //GL.Color4(Color4.Yellow);  //byte型で指定
            //GL.Vertex3(1.0f, 1.0f, 4.0f);

            //GL.End();


        }


    }
}
