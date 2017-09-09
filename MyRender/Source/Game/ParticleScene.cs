using MyRender.MyEngine;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System.Drawing;

namespace MyRender.Game
{
    class ParticleScene : Scene
    {
        private Vector2 _regMousePos = Vector2.Zero;
        private float max_camerz = 70;
        private float min_camerz = 20;
        private float skyboxSize = 100;

        public override void OnStart()
        {
            base.OnStart();

            var light = new Light();
            AddChild(light);

            MainCamera.ResetZoomInOut(30, min_camerz, max_camerz);

            var skybox = new Skybox();
            skybox.Scale(skyboxSize, skyboxSize, skyboxSize);
            AddChild(skybox);

            var particle = new ParticleSystem(0.1f, 10f, -9.8f, 2, 1);
            particle.SetLifeMargin(0.5f);
            particle.SetSpeedMargin(0.8f);
            particle.SetDirection(Vector3.UnitY, 0.1f);
            AddChild(particle);

            //var testCube = new Cube();
            //testCube.LocalPosition = new Vector3(0, 0, 0);
            //AddChild(testCube);

        }

        public override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.Mouse.RightButton == ButtonState.Pressed)
            {
                _regMousePos.X = (float)e.Mouse.X;// - MainWindow.Instance.Width / 2;
                _regMousePos.Y = (float)e.Mouse.Y;// - MainWindow.Instance.Height / 2;

            }

        }

        public override void OnMouseMove(MouseMoveEventArgs e)
        {
            base.OnMouseMove(e);

            if (e.Mouse.RightButton == ButtonState.Pressed)
            {
                var dX = e.X - _regMousePos.X;
                var dY = e.Y - _regMousePos.Y;

                MainCamera.RotationScreen(dX, dY);

                _regMousePos.X = e.X;
                _regMousePos.Y = e.Y;


            }
        }

        public override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);

            MainCamera.ZoomInOut(e.Delta, min_camerz, max_camerz);

        }
    }
}
