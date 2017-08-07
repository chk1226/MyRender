using OpenTK;
using System;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;
using MyRender.Debug;
using OpenTK.Input;

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
        public Action<MouseButtonEventArgs> OnMouseDown;
        public Action<MouseButtonEventArgs> OnMouseUp;
        public Action<MouseMoveEventArgs> OnMouseMove;
        public Action<MouseWheelEventArgs> OnMouseWheel;
        public Node.Action OnSatrt;


        public void Initial()
        {

            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.DepthTest);
            //GL.Enable(EnableCap.CullFace);

        }

        public void OnRelease()
        {

            Resource.Instance.OnRelease();
        }

        public void RunWithScene(Scene scene)
        {
            if(scene != null)
            {
                if(MainScene != null)
                {
                    // remove old scene
                    MainScene.RemoveAllChild();
                    MainScene.UnregisterCallback(MainScene);

                    // remove resource
                    Resource.Instance.ReleaseTextures();
                    Resource.Instance.ReleaseShaders();
                    Resource.Instance.ReleaseMaterial();
                    Resource.Instance.ReleaseModels();
                    Resource.Instance.ReleaseFont();

                }

                MainScene = scene;
                MainScene.RegisterCallback(MainScene);
            }

        }

        public void OnUpdateFrame(FrameEventArgs e)
        {
            if(MainScene != null)
            {
                // onstart only do once
                if(OnSatrt != null)
                {
                    OnSatrt();
                }

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

            //_renderList.Add(MainScene);
            recursiveTraverseTree(MainScene.Children);

            // for debug
            //Log.Print("***doTraverseTree****");
            //foreach (var i in _renderList)
            //{
            //    Log.Print(i.testid.ToString());
            //}
        }

        // preorder traverse
        private void recursiveTraverseTree(Dictionary<string, Node> node)
        {

            foreach (var pair in node)
            {
                // if model data is empty, continue
                if(pair.Value.ModelList == null || 
                    pair.Value.ModelList.Length == 0)
                {
                    continue;
                }

                // add root to list
                _renderList.Add(pair.Value);
                // left to right traverse
                if(pair.Value.Children.Count == 0)
                {
                    continue;
                }

                recursiveTraverseTree(pair.Value.Children);
            }
            
        }

        private void sortSceneGraph()
        {
            _renderList.Sort(delegate(Node x, Node y)
            {
                float x_z = x.ModelViewPosition().Z;
                float y_z = y.ModelViewPosition().Z;
                
                if(x_z > y_z)
                {
                    return 1;
                }
                else if(x_z < y_z)
                {
                    return -1;
                }

                return 0;
            });

            // for debug
            //Log.Print("***sortSceneGraph****");
            //foreach (var i in _renderList)
            //{
            //    Log.Print(i.testid.ToString());
            //}
        }

        private void doRender(FrameEventArgs e)
        {
            foreach(var node in _renderList)
            {
                node.OnRender(e);
                node.OnRenderFinsh(e);
            }
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

            GL.ClearColor(Color4.Gray);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            doRender(e);
        }

    }
}
