using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using MyRender.Debug;
using MyRender.MyEngine;
using OpenTK.Graphics;
using OpenTK.Input;

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

        private MouseMoveEventArgs _mouseMoveData;
        public MouseMoveEventArgs MouseMoveData {
            get { return _mouseMoveData; }
            private set { _mouseMoveData = value; }
        }

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
            GameDirect.Instance.Initial();
            GameDirect.Instance.RunWithScene(new Game.BaseRenderScene());

            // HACK
            Mouse.WheelChanged += delegate (object sender, OpenTK.Input.MouseWheelEventArgs _e)
            {

                GameDirect.Instance.OnMouseWheel(_e);
            };
        }

        protected override void OnUnload(EventArgs e)
        {
            base.OnUnload(e);
            GameDirect.Instance.OnRelease();
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

        }


        private void handleMouse()
        {
            
            if(Mouse[OpenTK.Input.MouseButton.Right])
            {
                var state = Mouse.GetState();
                GameDirect.Instance.OnMouseMove(MouseMoveData);
            }

        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            GameDirect.Instance.OnMouseDown(e);
        }

        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            base.OnMouseMove(e);
            MouseMoveData = e;
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            Title = $"(Vsync: {VSync}) FPS: {1f / e.Time:0}";

            GameDirect.Instance.OnRenderFrame(e);
            SwapBuffers();
        }

    }
}
