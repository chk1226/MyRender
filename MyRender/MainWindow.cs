using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using MyRender.Debug;
using MyRender.MyEngine;
using OpenTK.Graphics;

namespace MyRender
{
    class MainWindow : GameWindow
    {

        public Camera camera;
        private Vector2 preMousePos;

        public MainWindow() : base(1280,
            720,
            OpenTK.Graphics.GraphicsMode.Default,
            "openGL",
            GameWindowFlags.Default,
            DisplayDevice.Default
            )
        {
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            camera.UpdateViewport(ClientRectangle);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            camera = new Camera(new Vector3(0, 0, 20),
                                new Vector3(0, 0, 0),
                                new Vector3(0, 1, 0),
                                45,
                                1,
                                1000,
                                ClientRectangle);

            camera.Apply();


            // HACK
            Mouse.WheelChanged += delegate (object sender, OpenTK.Input.MouseWheelEventArgs _e)
            {
                var eye = camera.eye;
                eye.Z += _e.Delta;
                camera.UpdateEye(eye);
                //Log.Print("WHEEL" + _e.Delta.ToString());
            };
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
            handleKeyboard();
            handleMouse();
        }

        private void handleKeyboard()
        {
            var keyState = Keyboard.GetState();

            if (keyState.IsKeyDown(OpenTK.Input.Key.Escape))
            {
                Exit();
            }
            //else if(keyState.IsKeyDown(OpenTK.Input.Key.A))
            //{
            //    camera.Rotation(new Quaternion(MathHelper.DegreesToRadians(-1), 0 , 0));
            //}
            //else if (keyState.IsKeyDown(OpenTK.Input.Key.D))
            //{
            //    camera.Rotation(new Quaternion(MathHelper.DegreesToRadians(1), 0, 0));
            //}
            //else if (keyState.IsKeyDown(OpenTK.Input.Key.W))
            //{
            //    camera.Rotation(new Quaternion(0, MathHelper.DegreesToRadians(1), 0));
            //}
            //else if (keyState.IsKeyDown(OpenTK.Input.Key.S))
            //{
            //    camera.Rotation(new Quaternion(0, MathHelper.DegreesToRadians(-1), 0));
            //}

        }


        private void handleMouse()
        {
            
            if(Mouse[OpenTK.Input.MouseButton.Right])
            {
                if(preMousePos.X + preMousePos.Y == 0)
                {
                    preMousePos.X = Mouse.X;
                    preMousePos.Y = Mouse.Y;
                }
                else
                {
                    var dX = Mouse.X - preMousePos.X;
                    var dY = Mouse.Y - preMousePos.Y;

                    var m = new Matrix4();
                    
                        
                    camera.Rotation(new Quaternion(MathHelper.DegreesToRadians(dY), MathHelper.DegreesToRadians(dX), 0));

                    preMousePos.X = Mouse.X;
                    preMousePos.Y = Mouse.Y;

                    Log.Print(dX.ToString() + " " + dY.ToString());
                }
            }


        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            Title = $"(Vsync: {VSync}) FPS: {1f / e.Time:0}";


            GL.ClearColor(Color4.Black);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.Begin(PrimitiveType.Quads);

            GL.Color4(Color4.White);                            //色名で指定
            GL.Vertex3(-1.0f, 1.0f, 4.0f);
            GL.Color4(Color4.Blue);  //配列で指定
            GL.Vertex3(-1.0f, -1.0f, 4.0f);
            GL.Color4(Color4.Red);                  //4つの引数にfloat型で指定
            GL.Vertex3(1.0f, -1.0f, 4.0f);
            GL.Color4(Color4.Yellow);  //byte型で指定
            GL.Vertex3(1.0f, 1.0f, 4.0f);

            GL.End();


            SwapBuffers();
            //Log.Print("OnRenderFrame");

        }

    }
}
