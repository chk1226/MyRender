using MyRender.MyEngine;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using MyRender.Debug;
using OpenTK.Graphics;
using OpenTK.Input;
using System;
using System.Drawing;

namespace MyRender.Game
{
    class BaseRenderScene : Scene
    {
        private Vector2 _regMousePos = Vector2.Zero;
        private float max_camerz = 60;
        private float min_camerz = 5;
        private float skyboxSize = 70;

        public override void OnStart()
        {
            base.OnStart();

            var light = new Light();
            AddChild(light);

            MainCamera.ResetRotation(45, 75);
            MainCamera.ResetZoomInOut(30, min_camerz, max_camerz);

            UIButton a = new UIButton(new Rectangle(25, 25, 120, 70), Resource.IUIBlack, Color4.Orange, new Color4(0.34f, 0.6f, 0.67f, 1f),
                "GoBack");
            a.OnClick += delegate ()
            {
                GameDirect.Instance.RunWithScene(new MenuScene());
            };

            AddChild(a);

            var skybox = new Skybox();
            skybox.Scale(skyboxSize, skyboxSize, skyboxSize);
            AddChild(skybox);

            var testCube = new Cube();
            testCube.LocalPosition = new Vector3(0, -1, 0);
            testCube.Scale(5,1,5);
            AddChild(testCube);

            //var dae2 = new RobotModel();
            //dae2.Loader(Resource.MRobot);
            //dae2.LocalPosition = new Vector3(0, 0, 0);
            //AddChild(dae2);

            var dae = new CowboyModel();
            dae.Loader(Resource.MCowboy);
            AddChild(dae);
            
        }


        public override void OnUpdate(FrameEventArgs e)
        {
            base.OnUpdate(e);

        }

        public override void OnRender(FrameEventArgs e)
        {
            base.OnRender(e);


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
