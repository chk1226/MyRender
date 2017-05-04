using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using MyRender.Debug;

namespace MyRender
{
    class MainWindow : GameWindow
    {

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

            GL.Viewport(0, 0, Width, Height);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Log.Print("OnLoad");
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
            handleKeyboard();

            Log.Print("OnUpdateFrame");

        }

        private void handleKeyboard()
        {
            var keyState = Keyboard.GetState();

            if (keyState.IsKeyDown(OpenTK.Input.Key.Escape))
            {
                Exit();
            }
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            Title = $"(Vsync: {VSync}) FPS: {1f / e.Time:0}";


            GL.ClearColor(OpenTK.Graphics.Color4.Black);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            SwapBuffers();
            Log.Print("OnRenderFrame");

        }

    }
}
