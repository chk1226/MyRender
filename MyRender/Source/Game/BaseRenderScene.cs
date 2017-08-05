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
        private float max_camerz = 70;
        private float min_camerz = 5;
        private float skyboxSize = 70;

        public override void OnStart()
        {
            base.OnStart();

            var light = new Light();
            AddChild(light);

            MainCamera.ResetRotation(45, 75);
            MainCamera.ResetZoomInOut(20, min_camerz, max_camerz);

            UIButton a = new UIButton(new Rectangle(100, 100, 70, 70), Resource.IUIBlack, new Color4(89f, 154f, 171f, 255f));
            AddChild(a);

            var skybox = new Skybox();
            skybox.Scale(skyboxSize, skyboxSize, skyboxSize);
            AddChild(skybox);

            var testCube = new Cube();
            testCube.LocalPosition = new Vector3(0, 10, 0);
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
                //Log.Print(e.X .ToString() + "    " + _regMousePos.X.ToString());


                MainCamera.RotationScreen(dX, dY);

                _regMousePos.X = e.X;
                _regMousePos.Y = e.Y;

                //Log.Print("OnMouseMove");

            }
        }

        public override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);

            MainCamera.ZoomInOut(e.Delta, min_camerz, max_camerz);

        }

    }
}
