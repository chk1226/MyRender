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
        private float max_camerz = 100;
        private float min_camerz = 10;


        public override void OnStart()
        {
            base.OnStart();

            var light = new Light();
            AddChild(light);
            
            testCube = new Cube();
            AddChild(testCube);
            testCube.LocalPosition = new Vector3(0, 0, 0);

            //testCube = new Cube();
            //AddChild(testCube);
            //testCube.LocalPosition = new Vector3(2, 0, 0);

            var dae = new DaeModel();
            dae.Loader(Resource.MRider);
            AddChild(dae);
            dae.LocalPosition = new Vector3(0, 0, 0);


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


                MainCamera.Rotation(new Quaternion(0, MathHelper.DegreesToRadians(dX), MathHelper.DegreesToRadians(dY)));

                _regMousePos.X = e.X;
                _regMousePos.Y = e.Y;

                //Log.Print("OnMouseMove");

            }
        }

        public override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);
            var eye = MainCamera.eye;
            eye.Z += e.Delta;

            if (Math.Abs(eye.Z) >= max_camerz)
            {
                if (eye.Z < 0.0f)
                {
                    eye.Z = -max_camerz;
                }
                else
                {
                    eye.Z =  max_camerz;
                }
            }
            else if (Math.Abs(eye.Z) <= min_camerz)
            {
                if(eye.Z < 0.0f)
                {
                    eye.Z = -min_camerz;
                }
                else
                {
                    eye.Z = min_camerz;
                }
            }

            MainCamera.UpdateEye(eye);

        }

    }
}
