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

            var particle = new ParticleSystem(0.01f, 3f, -9.8f, 2, 0.5f, Resource.ICosmic, new Vector2(4, 4));
            particle.SetBlendFuc( BlendingFactorDest.One);
            particle.SetLifeMargin(0.5f);
            particle.SetSpeedMargin(0.8f);
            particle.SetScaleMargin(0.9f);
            particle.SetRandomRotation(true);
            particle.SetDirection(-Vector3.UnitY, 0.3f);
            AddChild(particle);


            particle = new ParticleSystem(0.01f, 5f, -5f, 3, 0.5f, Resource.IStart, new Vector2(1, 1));
            particle.LocalPosition = new Vector3(5,0,-15);
            particle.SetBlendFuc(BlendingFactorDest.One);
            particle.SetLifeMargin(0.5f);
            particle.SetSpeedMargin(0.8f);
            particle.SetScaleMargin(0.9f);
            particle.SetRandomRotation(true);
            particle.SetDirection(Vector3.UnitY, 0.2f);
            AddChild(particle);

            particle = new ParticleSystem(0.01f, 2.5f, 0, 3, 2.5f, Resource.IFire, new Vector2(8, 8));
            particle.LocalPosition = new Vector3(-10, 0, 5);
            particle.SetBlendFuc(BlendingFactorDest.One);
            particle.SetLifeMargin(0.5f);
            particle.SetSpeedMargin(0.8f);
            particle.SetScaleMargin(0.9f);
            particle.SetRandomRotation(true);
            particle.SetDirection(Vector3.UnitY, 0.1f);
            AddChild(particle);

            UIButton a = new UIButton(new Rectangle(25, 25, 120, 70), Resource.IUIBlack, Color4.Orange, new Color4(0.34f, 0.6f, 0.67f, 1f),
                "GoBack");
            a.OnClick += delegate ()
            {
                GameDirect.Instance.RunWithScene(new MenuScene());
            };
            AddChild(a);

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
