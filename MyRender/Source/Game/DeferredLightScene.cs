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

        public override void OnStart()
        {
            base.OnStart();

            //var light = new Light();
            //AddChild(light);

            MainCamera.ResetZoomInOut(30, min_camerz, max_camerz);

            var skybox = new Skybox();
            skybox.Scale(skyboxSize, skyboxSize, skyboxSize);
            AddChild(skybox);


            float radius = 10;
            float num = 12;
            float angle = MathHelper.Pi*2 / num;

            for (int i = 0; i < num; i++)
            {
                var cube = new ColorCube();
                cube.Rotation(0, 1, 0, -MathHelper.RadiansToDegrees(i * angle));
                cube.Color = genColor(i);
                cube.LocalPosition = new Vector3(radius * (float)Math.Cos(angle * i), 0, radius * (float)Math.Sin(angle * i));
                cube.AddComponent(new MoveComponent(i * 1f, 3, new Vector3(cube.LocalPosition.X, 3, cube.LocalPosition.Z), cube));
                AddChild(cube);

            }

            var plane = new ColorPlane(50, 50);
            var pos = plane.LocalPosition;
            pos.Y = -1.5f;
            plane.LocalPosition = pos;
            AddChild(plane);

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
    }
}
