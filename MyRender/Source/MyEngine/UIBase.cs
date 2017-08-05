using OpenTK;
using System;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;
using MyRender.Debug;
using OpenTK.Input;
using System.Drawing;

namespace MyRender.MyEngine
{
    class UIBase : Node
    {
        protected Rectangle rect;

        public UIBase(Rectangle rect)
        {
            this.rect = rect; 
        }

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

        public override void OnRender(FrameEventArgs e)
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
            if (MaterialData != null && MaterialData.ShaderProgram != 0)
            {
                GL.UseProgram(0);
            }


            GL.MatrixMode(MatrixMode.Modelview);
            GL.PopMatrix();
            GL.MatrixMode(MatrixMode.Projection);
            GL.PopMatrix();
        }

        
    }
    
}
