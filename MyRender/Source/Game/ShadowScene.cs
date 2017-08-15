using MyRender.MyEngine;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

// reference
// shadowmap http://www.opengl-tutorial.org/intermediate-tutorials/tutorial-16-shadow-mapping/
// vsm http://fabiensanglard.net/shadowmappingVSM/

namespace MyRender.Game
{
    class ShadowScene : Scene
    {
        private Vector2 _regMousePos = Vector2.Zero;
        private float max_camerz = 70;
        private float min_camerz = 25;

        public override void OnStart()
        {
            base.OnStart();

            var light = new Light();
            light.Specular = Vector4.Zero;
            light.Ambient = new Vector4(0.7f, 0.7f, 0.7f, 1.0f);
            AddChild(light);
            light.EnableLightShadowMap();
            light.IsMove = true;

            MainCamera.ResetRotation(0, 61);
            MainCamera.ResetZoomInOut(70, min_camerz, max_camerz);

            var mrt = new MRT();
            AddChild(mrt);

            var skybox = new Skybox();
            skybox.Scale(70, 70, 70);
            AddChild(skybox);

            var dae = new HomeModel();
            dae.Loader(Resource.MHouse, false);
            dae.LocalPosition = new Vector3(0, 0, 0);
            dae.Scale(4, 4, 4);
            dae.SetFrameBuffer(Resource.Instance.GetFrameBuffer(FrameBuffer.Type.GaussianYFrame));
            AddChild(dae);

            var plane = new Plane(100, 100, 1, 1);
            plane.SetFrameBuffer(Resource.Instance.GetFrameBuffer(FrameBuffer.Type.GaussianYFrame));
            plane.LocalPosition = new Vector3(-50, 0, -50);
            AddChild(plane);


            var gaussian = new ScreenEffect();
            gaussian.EnableGaussian(true);
            gaussian.SetFrameBuffer(Resource.Instance.GetFrameBuffer(FrameBuffer.Type.ShadowmapFrame),
                Resource.Instance.GetFrameBuffer(FrameBuffer.Type.GaussianXFrame));
            AddChild(gaussian);

            gaussian = new ScreenEffect();
            gaussian.EnableGaussian(false);
            gaussian.SetFrameBuffer(Resource.Instance.GetFrameBuffer(FrameBuffer.Type.GaussianXFrame),
                Resource.Instance.GetFrameBuffer(FrameBuffer.Type.GaussianYFrame));
            AddChild(gaussian);

            var ssao = new ScreenEffect();
            ssao.EnableSSAO();
            ssao.SetFrameBuffer(Resource.Instance.GetFrameBuffer(FrameBuffer.Type.GBuffer),
                Resource.Instance.GetFrameBuffer(FrameBuffer.Type.SSAOFrame));
            AddChild(ssao);
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
