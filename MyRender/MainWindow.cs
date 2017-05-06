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

        static private MainWindow _instance;
        static public MainWindow Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new MainWindow();
                }

                return _instance;
            }
        }

        //public Camera camera;
        //private Vector2 preMousePos;

        private MainWindow() : base(1280,
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
            GameDirect.Instance.OnWindowResize();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            GameDirect.Instance.RunWithScene(new Game.BaseRenderScene());


            // HACK
            //Mouse.WheelChanged += delegate (object sender, OpenTK.Input.MouseWheelEventArgs _e)
            //{
            //    var eye = camera.eye;
            //    eye.Z += _e.Delta;
            //    camera.UpdateEye(eye);
            //    //Log.Print("WHEEL" + _e.Delta.ToString());
            //};
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
            handleKeyboard();
            handleMouse();

            GameDirect.Instance.OnUpdateFrame(e);

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
                //if(preMousePos.X + preMousePos.Y == 0)
                //{
                //    preMousePos.X = Mouse.X;
                //    preMousePos.Y = Mouse.Y;
                //}
                //else
                //{
                //    var dX = Mouse.X - preMousePos.X;
                //    var dY = Mouse.Y - preMousePos.Y;

                //    camera.Rotation(new Quaternion(MathHelper.DegreesToRadians(dY), MathHelper.DegreesToRadians(dX), 0));

                //    preMousePos.X = Mouse.X;
                //    preMousePos.Y = Mouse.Y;

                //    Log.Print(dX.ToString() + " " + dY.ToString());
                //}
            }


        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            Title = $"(Vsync: {VSync}) FPS: {1f / e.Time:0}";

            GameDirect.Instance.OnRenderFrame(e);
            SwapBuffers();
        }

    }
}
