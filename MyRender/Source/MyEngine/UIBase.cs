using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing;

namespace MyRender.MyEngine
{
    class UIBase : Node
    {
        protected Rectangle rect;
        protected float depth = 0;

        private float offsetDepth = 0.0001f;

        public UIBase(Rectangle rect)
        {
            this.rect = rect; 
        }

        public UIBase(){ }

        public override Vector3 ModelViewPosition()
        {
            if (GameDirect.Instance.MainScene == null ||
                GameDirect.Instance.MainScene.MainCamera == null)
            {
                return Vector3.Zero;
            }

            var p = new Vector4(0, 0, 0, 1);
            p = GameDirect.Instance.MainScene.MainCamera.UIViewMatrix * WorldModelMatrix * LocalModelMatrix * p;

            return p.Xyz;

        }

        public override void OnRenderBegin(FrameEventArgs e)
        {
            GL.MatrixMode(MatrixMode.Projection);
            GL.PushMatrix();
            var vm = GameDirect.Instance.MainScene.MainCamera.UIProjectMatrix;
            vm.Transpose();
            GL.LoadMatrix(ref vm);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.PushMatrix();
            vm = GameDirect.Instance.MainScene.MainCamera.UIViewMatrix * WorldModelMatrix * LocalModelMatrix;
            vm.Transpose();
            GL.LoadMatrix(ref vm);
        }

        public override void OnRenderFinsh(FrameEventArgs e)
        {
            GL.MatrixMode(MatrixMode.Modelview);
            GL.PopMatrix();
            GL.MatrixMode(MatrixMode.Projection);
            GL.PopMatrix();
        }

        protected virtual void updateModelData() { }

        public override void AddChild(Node child)
        {
            if (child == null) return;

            base.AddChild(child);

            var uichild = child as UIBase;
            if(uichild != null)
            {
                uichild.depth = this.depth + offsetDepth;
                uichild.updateModelData();
            }
            
        }

        public override void SetParent(Node target)
        {
            base.SetParent(target);

            if(target == null)
            {
                this.depth = 0;
            }
            else
            {
                var uiparent = target as UIBase;
                if (uiparent != null)
                {
                    this.depth = uiparent.depth + offsetDepth;
                }
                else
                {
                    this.depth = 0;
                }
            }

            this.updateModelData();
        }

    }

}
