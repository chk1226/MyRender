﻿using MyRender.MyEngine;
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
        private float min_camerz = 5;

        public override void OnStart()
        {
            base.OnStart();

            var light = new Light();
            //light.Specular = new Vector4(0.3f, 0.3f, 0.3f, 1);
            light.Ambient = new Vector4(0.7f, 0.7f, 0.7f, 1.0f);
            AddChild(light);
            light.EnableLightShadowMap();

            MainCamera.ResetRotation(0, 61);
            MainCamera.ResetZoomInOut(15, min_camerz, max_camerz);

            var skybox = new Skybox();
            skybox.Scale(70, 70, 70);
            AddChild(skybox);

            var dae = new HomeModel();
            dae.Loader(Resource.MHouse, false);
            dae.LocalPosition = new Vector3(0, 0, 0);
            dae.SetFrameBuffer(Resource.Instance.GetFrameBuffer(FrameBuffer.Type.GaussianYFrame));
            AddChild(dae);

            var plane = new Plane(100, 100, 1, 1);
            plane.SetFrameBuffer(Resource.Instance.GetFrameBuffer(FrameBuffer.Type.GaussianYFrame), null);
            plane.LocalPosition = new Vector3(-50, 0, -50);
            AddChild(plane);

            //var cube = new Cube();
            //cube.LocalPosition = new Vector3(0, 1, 0);
            //AddChild(cube);

            //cube = new Cube();
            //cube.LocalPosition = new Vector3(1, 3, 1);
            //AddChild(cube);

            //plane = new Plane(10, 10, 1, 1);
            //plane.SetFrameBuffer(GameDirect.Instance.DepthBudder, null);
            //plane.LocalPosition = new Vector3(0, 3, 0);
            //AddChild(plane);

            //plane = new Plane(5, 5, 1, 1);
            //plane.SetFrameBuffer(GameDirect.Instance.DepthBudder, null);
            //plane.LocalPosition = new Vector3(2, 5, 2);
            //AddChild(plane);

            var vp = MainCamera.Viewport;
            var gaussian = new Plane(vp.Width, vp.Height, 1, 1);
            gaussian.SetPlaneType(Plane.PlaneType.Gaussian, false);
            gaussian.LocalPosition = new Vector3(-vp.Width / 2.0f, 0, -vp.Height / 2.0f);
            gaussian.Rotation(1, 0, 0, -90);
            gaussian.SetFrameBuffer(Resource.Instance.GetFrameBuffer( FrameBuffer.Type.ShadowmapFrame),
                Resource.Instance.GetFrameBuffer( FrameBuffer.Type.GaussianXFrame));
            AddChild(gaussian);

            gaussian = new Plane(vp.Width, vp.Height, 1, 1);
            gaussian.SetPlaneType(Plane.PlaneType.Gaussian, true);
            gaussian.LocalPosition = new Vector3(-vp.Width / 2.0f, 0, -vp.Height / 2.0f);
            gaussian.Rotation(1, 0, 0, -90);
            gaussian.SetFrameBuffer(Resource.Instance.GetFrameBuffer(FrameBuffer.Type.GaussianXFrame),
                Resource.Instance.GetFrameBuffer(FrameBuffer.Type.GaussianYFrame));
            AddChild(gaussian);

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
