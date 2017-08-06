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

            UIButton a = new UIButton(new Rectangle(spaceX, spaceY, wX, hY), Resource.IUIBlack, Color4.Orange, new Color4(0.34f, 0.6f, 0.67f, 1f));
            a.OnClick += delegate ()
            {
                GameDirect.Instance.RunWithScene(new BaseRenderScene());
            };

            AddChild(a);

        }
    }
}
