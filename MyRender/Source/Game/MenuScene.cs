using MyRender.Debug;
using MyRender.MyEngine;
using OpenTK;
using OpenTK.Graphics;
using System.Drawing;

namespace MyRender.Game
{
    class MenuScene : Scene
    {
        public override void OnStart()
        {
            base.OnStart();

            var winRect = MainWindow.Instance.ClientRectangle;
            int spaceX = 50;
            int spaceY = 50;
            int wX = winRect.Width / 2 - spaceX * 2;
            int hY = 100;

            UIButton a = new UIButton(new Rectangle(spaceX, spaceY, wX, hY), Resource.IUIBlack, Color4.Orange, new Color4(0.34f, 0.6f, 0.67f, 1f),
                "Skybox, NormalMap, SkeletalAnimation");
            a.OnClick += delegate ()
            {
                GameDirect.Instance.RunWithScene(new BaseRenderScene());
            };
            AddChild(a);

            a = new UIButton(new Rectangle(winRect.Width / 2, spaceY, wX, hY), Resource.IUIBlack, Color4.Orange, new Color4(0.34f, 0.6f, 0.67f, 1f),
                "VSM ShadowMap, SSAO");
            a.OnClick += delegate ()
            {
                GameDirect.Instance.RunWithScene(new ShadowScene());
            };
            AddChild(a);

            a = new UIButton(new Rectangle(spaceX, spaceY * 2 + hY, wX, hY), Resource.IUIBlack, Color4.Orange, new Color4(0.34f, 0.6f, 0.67f, 1f),
                "Terrain, Water, HDR, Bloom, DOF");
            a.OnClick += delegate ()
            {
                GameDirect.Instance.RunWithScene(new TerrainScene());
            };
            AddChild(a);

            a = new UIButton(new Rectangle(winRect.Width / 2, spaceY * 2 + hY, wX, hY), Resource.IUIBlack, Color4.Orange, new Color4(0.34f, 0.6f, 0.67f, 1f),
                "Particle(GeometryShader)");
            a.OnClick += delegate ()
            {
                GameDirect.Instance.RunWithScene(new ParticleScene());
            };
            AddChild(a);

            a = new UIButton(new Rectangle(spaceX, (spaceY + hY)*2 + spaceY, wX, hY), Resource.IUIBlack, Color4.Orange, new Color4(0.34f, 0.6f, 0.67f, 1f),
                "DeferredLight");
            a.OnClick += delegate ()
            {
                GameDirect.Instance.RunWithScene(new DeferredLightScene());
            };
            AddChild(a);

        }
    }
}
