using OpenTK;
using System;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;
using MyRender.Debug;

namespace MyRender.MyEngine
{
    class GameDirect
    {
        private GameDirect() { }

        static private GameDirect _instance;
        static public GameDirect Instance {
            get {
                if(_instance == null)
                {
                    _instance = new GameDirect();
                }
                return _instance;
            }
        }

        private Scene _mainScene;
        public Scene MainScene {
            get { return _mainScene; }
            private set { _mainScene = value; }
        }

        private List<Node> _renderList = new List<Node>(100); 

        //public event Action<FrameEventArgs> OnRender;
        public event Action<FrameEventArgs> OnUpdate;

        //private 

        public void RunWithScene(Scene scene)
        {
            if(scene != null)
            {
                MainScene = scene;
                OnUpdate += MainScene.OnUpdate;
            }
        }

        public void OnUpdateFrame(FrameEventArgs e)
        {
            if(MainScene != null)
            {
                OnUpdate(e);
            }
        }

        private void doTraverseTree()
        {
            _renderList.Clear();
            if(MainScene == null)
            {
                return;
            }

            recursiveTraverseTree(MainScene.Children);

            //foreach(var bb in _renderList)
            //{
            //    Log.Print(bb.regTest.ToString());
            //}
        }

        // preorder traverse
        private void recursiveTraverseTree(Dictionary<string, Node> node)
        {

            foreach (var pair in node)
            {
                // add root to list
                _renderList.Add(pair.Value);
                //left to right
                if(pair.Value.Children.Count == 0)
                {
                    continue;
                }

                recursiveTraverseTree(pair.Value.Children);
            }
            
        }

        private void sortSceneGraph()
        {


        }

        public void OnWindowResize()
        {
            if(MainScene != null)
            {
                MainScene.MainCamera.UpdateViewport(MainWindow.Instance.ClientRectangle);
            }
        }

        public void OnRenderFrame(FrameEventArgs e)
        {
            doTraverseTree();
            sortSceneGraph();

            GL.ClearColor(Color4.Black);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            //GL.Begin(PrimitiveType.Quads);

            //GL.Color4(Color4.White);                            //色名で指定
            //GL.Vertex3(-1.0f, 1.0f, 4.0f);
            //GL.Color4(Color4.Blue);  //配列で指定
            //GL.Vertex3(-1.0f, -1.0f, 4.0f);
            //GL.Color4(Color4.Red);                  //4つの引数にfloat型で指定
            //GL.Vertex3(1.0f, -1.0f, 4.0f);
            //GL.Color4(Color4.Yellow);  //byte型で指定
            //GL.Vertex3(1.0f, 1.0f, 4.0f);

            //GL.End();
        }

    }
}
