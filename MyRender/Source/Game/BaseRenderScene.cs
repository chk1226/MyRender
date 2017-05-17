using MyRender.MyEngine;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using MyRender.Debug;
using OpenTK.Graphics;
using OpenTK.Input;
using System;

namespace MyRender.Game
{
    class BaseRenderScene : Scene
    {
        public Cube testCube;
        private Vector2 _regMousePos = Vector2.Zero;

        public override void OnStart()
        {
            base.OnStart();

            var light = new Light();
            AddChild(light);
            
            testCube = new Cube();
            AddChild(testCube);
            testCube.LocalPosition = new Vector3(0, 0, 0);

            testCube = new Cube();
            AddChild(testCube);
            testCube.LocalPosition = new Vector3(2, 0, 0);

        }


        public override void OnUpdate(FrameEventArgs e)
        {
            base.OnUpdate(e);


        }

        public override void OnRender(FrameEventArgs e)
        {
            base.OnRender(e);

            //GL.BindTexture(TextureTarget.Texture2D, Resource.Instance.GetTextureID(Resource.IBricks));
            //GL.Begin(PrimitiveType.Quads);

            //GL.Color4(Color4.White);                            //色名で指定
            //GL.TexCoord2(0, 0);
            //GL.Vertex3(-1.0f, 1.0f, 4.0f);
            //GL.TexCoord2(0, 1);
            //GL.Vertex3(-1.0f, -1.0f, 4.0f);
            //GL.TexCoord2(1, 1);
            //GL.Vertex3(1.0f, -1.0f, 4.0f);
            //GL.TexCoord2(1, 0);
            //GL.Vertex3(1.0f, 1.0f, 4.0f);

            //GL.End();
            //GL.BindTexture(TextureTarget.Texture2D, 0);


        }

        public override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.Mouse.RightButton == ButtonState.Pressed)
            {
                _regMousePos.X = (float)e.Mouse.X;// - MainWindow.Instance.Width / 2;
                _regMousePos.Y = (float)e.Mouse.Y;// - MainWindow.Instance.Height / 2;

                //Log.Print("OnMouseDown");

            }

        }

        public override void OnMouseMove(MouseMoveEventArgs e)
        {
            base.OnMouseMove(e);

            if (e.Mouse.RightButton == ButtonState.Pressed) 
            {
                var dX = e.X - _regMousePos.X;
                var dY = e.Y - _regMousePos.Y;
                //Log.Print(e.X .ToString() + "    " + _regMousePos.X.ToString());


                MainCamera.Rotation(new Quaternion(0, MathHelper.DegreesToRadians(dX), MathHelper.DegreesToRadians(dY)));

                _regMousePos.X = e.X;
                _regMousePos.Y = e.Y;

                //Log.Print("OnMouseMove");

            }
        }


    }
}
