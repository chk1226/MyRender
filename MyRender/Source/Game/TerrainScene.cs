using MyRender.MyEngine;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System.Drawing;

namespace MyRender.Game
{
    class TerrainScene : Scene
    {
        private Vector2 _regMousePos = Vector2.Zero;
        private float max_camerz = 70;
        private float min_camerz = 17;
        private float skyboxSize = 80;
        private float waterHeight = 0;

        private float limitY = 83;
        public override void OnStart()
        {
            base.OnStart();

            var light = new Light();
            AddChild(light);

            MainCamera.ResetZoomInOut(17, min_camerz, max_camerz);
            MainCamera.ResetRotation(-93, limitY);
            GameDirect.Instance.ChangeRenderFrame(Resource.Instance.GetFrameBuffer(FrameBuffer.Type.RGBFColorDepth).FB);

            var skybox = new Skybox();
            skybox.Scale(skyboxSize, skyboxSize, skyboxSize);
            AddChild(skybox);

            // terrain plane
            var t = new TerrainPlane(50, 50, 50, 50);
            AddChild(t);

            // water plane
            var water = new WaterPlane(200, 200, waterHeight, 1, 1);
            AddChild(water);

            // reflection
            var pre = new PreRender();
            pre.SetType(PreRender.PreRenderType.Reflection);
            pre.WaterHeight = waterHeight;
            AddChild(pre);
            water.ReflectionNode = new System.WeakReference<PreRender>(pre);
            // refraction
            pre = new PreRender();
            pre.SetType(PreRender.PreRenderType.Refraction);
            pre.WaterHeight = waterHeight;
            AddChild(pre);

            var cube = new Cube();
            cube.LocalPosition = new Vector3(5, 0, -5);
            AddChild(cube);

            cube = new Cube();
            cube.Rotation(1,1,0,60);
            cube.LocalPosition = new Vector3(-7, 0, 30);
            AddChild(cube);

            postRender();

            //var uisprite = new UISprite(new Rectangle(25, 25, 700, 500), Resource.Instance.GetFrameBuffer(FrameBuffer.Type.RGBFColorDepth5).CB_Texture);
            //AddChild(uisprite);

            UIButton a = new UIButton(new Rectangle(25, 25, 120, 70), Resource.IUIBlack, Color4.Orange, new Color4(0.34f, 0.6f, 0.67f, 1f),
                "GoBack");
            a.OnClick += delegate ()
            {
                GameDirect.Instance.RunWithScene(new MenuScene());
            };
            AddChild(a);
        }

        private void postRender()
        {
            ScreenEffect gaussian;
            var vp = MainCamera.Viewport;

            // origin color texture do blur
            gaussian = new ScreenEffect(vp.Width, vp.Height, Render.Postrender);
            gaussian.EnableGaussian(true, 2);
            gaussian.SetFrameBuffer(Resource.Instance.GetFrameBuffer(FrameBuffer.Type.RGBFColorDepth),
                Resource.Instance.GetFrameBuffer(FrameBuffer.Type.RGBFColorDepth4));
            AddChild(gaussian);

            gaussian = new ScreenEffect(vp.Width, vp.Height, Render.Postrender - 1);
            gaussian.EnableGaussian(false, 2);
            gaussian.SetFrameBuffer(Resource.Instance.GetFrameBuffer(FrameBuffer.Type.RGBFColorDepth4),
                Resource.Instance.GetFrameBuffer(FrameBuffer.Type.RGBFColorDepth5));
            AddChild(gaussian);
            //for (int i = 1; i < 2; i++)
            //{
            //    gaussian = new ScreenEffect(vp.Width, vp.Height, Render.Postrender - (i * 2));
            //    gaussian.EnableGaussian(true, 2);
            //    gaussian.SetFrameBuffer(Resource.Instance.GetFrameBuffer(FrameBuffer.Type.RGBFColorDepth5),
            //        Resource.Instance.GetFrameBuffer(FrameBuffer.Type.RGBFColorDepth4));
            //    AddChild(gaussian);

            //    gaussian = new ScreenEffect(vp.Width, vp.Height, Render.Postrender - (i * 2 + 1));
            //    gaussian.EnableGaussian(false, 2);
            //    gaussian.SetFrameBuffer(Resource.Instance.GetFrameBuffer(FrameBuffer.Type.RGBFColorDepth4),
            //        Resource.Instance.GetFrameBuffer(FrameBuffer.Type.RGBFColorDepth5));
            //    AddChild(gaussian);
            //}

            // bright filter
            var filter = new ScreenEffect(vp.Width, vp.Height, Render.Postrender);
            filter.EnableBrightFilter();
            filter.SetFrameBuffer(Resource.Instance.GetFrameBuffer(FrameBuffer.Type.RGBFColorDepth),
                Resource.Instance.GetFrameBuffer(FrameBuffer.Type.RGBFColorDepth2));
            AddChild(filter);

            // bright gaussian
            for (int i = 1; i < 3; i++)
            {
                gaussian = new ScreenEffect(vp.Width, vp.Height, Render.Postrender - (i * 2 - 1));
                gaussian.EnableGaussian(true, 2);
                gaussian.SetFrameBuffer(Resource.Instance.GetFrameBuffer(FrameBuffer.Type.RGBFColorDepth2),
                    Resource.Instance.GetFrameBuffer(FrameBuffer.Type.RGBFColorDepth3));
                AddChild(gaussian);

                gaussian = new ScreenEffect(vp.Width, vp.Height, Render.Postrender - (i * 2));
                gaussian.EnableGaussian(false, 2);
                gaussian.SetFrameBuffer(Resource.Instance.GetFrameBuffer(FrameBuffer.Type.RGBFColorDepth3),
                    Resource.Instance.GetFrameBuffer(FrameBuffer.Type.RGBFColorDepth2));
                AddChild(gaussian);
            }

            // dof
            var dof = new ScreenEffect(vp.Width, vp.Height, Render.Postrender - 49);
            dof.EnableDepthOfField(Resource.Instance.GetFrameBuffer(FrameBuffer.Type.RGBFColorDepth).CB_Texture,
                Resource.Instance.GetFrameBuffer(FrameBuffer.Type.RGBFColorDepth5).CB_Texture,
                Resource.Instance.GetFrameBuffer(FrameBuffer.Type.RGBFColorDepth).DB_Texture);
            dof.SetFrameBuffer(null, Resource.Instance.GetFrameBuffer(FrameBuffer.Type.RGBFColorDepth4));
            AddChild(dof);

            var result = new ScreenEffect(vp.Width, vp.Height, Render.Postrender - 50);
            result.EnableCombineBright(Resource.Instance.GetFrameBuffer(FrameBuffer.Type.RGBFColorDepth4).CB_Texture,
                Resource.Instance.GetFrameBuffer(FrameBuffer.Type.RGBFColorDepth2).CB_Texture);
            AddChild(result);
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

                if(MainCamera.EyeRotation.Y + dY > limitY)
                {
                    MainCamera.ResetRotation(MainCamera.EyeRotation.X + dX, limitY);
                }
                else
                {
                    MainCamera.RotationScreen(dX, dY);
                }

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
