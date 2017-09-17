using MyRender.MyEngine;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System;
using System.Drawing;

namespace MyRender.Game
{
    class DeferredLightScene : Scene
    {
        private Vector2 _regMousePos = Vector2.Zero;
        private float max_camerz = 70;
        private float min_camerz = 20;
        private float skyboxSize = 100;

        private Vector4 att; 

        public override void OnStart()
        {
            base.OnStart();

            setting();

            var light = new Light();
            AddChild(light);

            preRender();

            var skybox = new Skybox();
            skybox.Scale(skyboxSize, skyboxSize, skyboxSize);
            skybox.PassRender = true;
            AddChild(skybox);

            // set 1
            float radius = 10;
            float num = 12;
            float angle = MathHelper.Pi*2 / num;
            for (int i = 0; i < num; i++)
            {
                var cube = new LightCube();
                cube.Rotation(0, 1, 0, -MathHelper.RadiansToDegrees(i * angle));
                cube.Color = genColor(i*3);
                cube.LocalPosition = new Vector3(radius * (float)Math.Cos(angle * i), 0, radius * (float)Math.Sin(angle * i));
                cube.AddComponent(new MoveComponent(i * 1f, 3, new Vector3(cube.LocalPosition.X, 3, cube.LocalPosition.Z), cube));
                cube.LightCaculation();
                att = cube.GetAttenuationInfo();
                AddChild(cube);

            }

            // set 2
            radius = 20;
            num = 50;
            angle = MathHelper.Pi * 2 / num;
            for (int i = 0; i < num; i++)
            {
                var cube = new LightCube();
                cube.Rotation(0, 1, 0, -MathHelper.RadiansToDegrees(i * angle));
                cube.Color = genColor(i + 3);
                cube.LocalPosition = new Vector3(radius * (float)Math.Cos(angle * i), 0, radius * (float)Math.Sin(angle * i));
                cube.AddComponent(new MoveComponent(i * 0.1f, 2, new Vector3(cube.LocalPosition.X, 3, cube.LocalPosition.Z), cube));
                cube.LightCaculation();
                att = cube.GetAttenuationInfo();
                AddChild(cube);

            }

            // set 3
            radius = 0;
            num = 1;
            angle = MathHelper.Pi * 2 / num;
            for (int i = 0; i < num; i++)
            {
                var cube = new LightCube();
                cube.Rotation(0, 1, 0, -MathHelper.RadiansToDegrees(i * angle));
                cube.Color = genColor(7);
                cube.LocalPosition = new Vector3(radius * (float)Math.Cos(angle * i), 2, radius * (float)Math.Sin(angle * i));
                cube.AddComponent(new MoveComponent(i * 0.1f, 5, new Vector3(cube.LocalPosition.X, 3, cube.LocalPosition.Z), cube));
                cube.LightCaculation();
                att = cube.GetAttenuationInfo();
                AddChild(cube);

            }

            var plane = new ColorPlane(50, 50);
            var pos = plane.LocalPosition;
            //pos.Y = -1.5f;
            plane.LocalPosition = pos;
            AddChild(plane);

            UIButton a = new UIButton(new Rectangle(25, 25, 120, 70), Resource.IUIBlack, Color4.Orange, new Color4(0.34f, 0.6f, 0.67f, 1f),
                "GoBack");
            a.OnClick += delegate ()
            {
                GameDirect.Instance.RunWithScene(new MenuScene());
            };
            a.PassRender = true;
            AddChild(a);

            postRender();
        }

        private void preRender()
        {
            var mrt = new PreRender();
            mrt.SetType(PreRender.PreRenderType.MRT_PNC);
            mrt.SetBuffer(Resource.Instance.GetFrameBuffer(FrameBuffer.Type.GBufferPNC).FB);
            AddChild(mrt);

            var onlyRender = new PreRender();
            onlyRender.SetType(PreRender.PreRenderType.OnlyRender);
            onlyRender.SetBuffer(Resource.Instance.GetFrameBuffer(FrameBuffer.Type.RGBAFColorDepth).FB);
            onlyRender.SetRanderRange(Render.Skybox, Render.Skybox);
            AddChild(onlyRender);

            onlyRender = new PreRender();
            onlyRender.SetType(PreRender.PreRenderType.OnlyRender);
            onlyRender.SetBuffer(Resource.Instance.GetFrameBuffer(FrameBuffer.Type.RGBAFColorDepth2).FB);
            onlyRender.SetRanderRange(Render.UI, Render.UI);
            AddChild(onlyRender);
        }

        private void setting()
        {
            GL.ClearColor(0, 0, 0, 0);
            MainCamera.ResetZoomInOut(46, min_camerz, max_camerz);
        }

        private void postRender()
        {
            var vp = MainCamera.Viewport;

            var deferred = new ScreenEffect(vp.Width, vp.Height, Render.Postrender);
            deferred.EnableDeferredLight(att);
            deferred.SetFrameBuffer(Resource.Instance.GetFrameBuffer(FrameBuffer.Type.GBufferPNC),
                Resource.Instance.GetFrameBuffer(FrameBuffer.Type.RGBAFColorDepth3));
            AddChild(deferred);


            var result = new ScreenEffect(vp.Width, vp.Height, Render.Postrender - 6);
            result.EnableCombineDeferred(
                Resource.Instance.GetFrameBuffer(FrameBuffer.Type.RGBAFColorDepth2).CB_Texture,
                Resource.Instance.GetFrameBuffer(FrameBuffer.Type.RGBAFColorDepth3).CB_Texture,
                Resource.Instance.GetFrameBuffer(FrameBuffer.Type.RGBAFColorDepth).CB_Texture
            );
            AddChild(result);





        }

        // reference
        // https://krazydad.com/tutorials/makecolors.php
        private Vector3 genColor(int index)
        {
            var frequency = 0.5f;
            Vector3 color;

            color.X = (float)Math.Sin(frequency * index + 0) * 0.496f + 0.5f;
            color.Y = (float)Math.Sin(frequency * index + 2) * 0.496f + 0.5f;
            color.Z = (float)Math.Sin(frequency * index + 4) * 0.496f + 0.5f;

            return color;
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

        public override void OnRelease()
        {
            base.OnRelease();
            GL.ClearColor( Color4.Gray);

        }
    }
}
