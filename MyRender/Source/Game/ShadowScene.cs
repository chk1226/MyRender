using MyRender.MyEngine;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace MyRender.Game
{
    class ShadowScene : Scene
    {
        private Vector2 _regMousePos = Vector2.Zero;
        private float max_camerz = 70;
        private float min_camerz = 5;

        public override void OnStart()
        {
            base.OnStart();

            var light = new Light();
            light.Ambient = new Vector4(0.0f, 0.5f, 0.5f, 1);
            light.Specular = new Vector4(0.3f, 0.3f, 0.3f, 1);
            //light.Diffuse = new Vector4(0, 0, 0, 1);
            AddChild(light);
            light.EnableLightDepthMap();

            MainCamera.ResetRotation(-163, 61);
            MainCamera.ResetZoomInOut(15, min_camerz, max_camerz);

            var plane = new Plane(30, 30, 1, 1);
            plane.LocalPosition = new Vector3(-15, 0, -15);
            AddChild(plane);

            var dae = new HomeModel();
            dae.Loader(Resource.MHouse, false);
            AddChild(dae);
            

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
