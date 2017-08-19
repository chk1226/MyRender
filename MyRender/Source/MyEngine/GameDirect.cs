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

        private List<Render> _prerenderList = new List<Render>(50);
        private List<Render> _prepostrenderList = new List<Render>(10);
        private List<Render> _renderList = new List<Render>(100);
        private List<Render> _postrenderList = new List<Render>(50);

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
                    OnSatrt -= MainScene.OnStart;
                    MainScene.OnRelease();

                    // remove resource
                    Resource.Instance.ReleaseTextures();
                    Resource.Instance.ReleaseShaders();
                    Resource.Instance.ReleaseMaterial();
                    Resource.Instance.ReleaseModels();
                    Resource.Instance.ReleaseFont();
                    Resource.Instance.ReleaseFrameBuffer();

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
                OnSatrt?.Invoke();

                OnUpdate(e);
            }
        }

        private void doTraverseTree()
        {
            _prerenderList.Clear();
            _prepostrenderList.Clear();
            _renderList.Clear();
            _postrenderList.Clear();

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
                var child = pair.Value;
                // if renderlist is empty, continue
                if(child.RenderList.Count == 0)
                {
                    continue;
                }

                foreach(var render in child.RenderList)
                {
                    if(render.Priority >= Render.Prerender)
                    {
                        _prerenderList.Add(render);
                    }
                    else if(render.Priority >= Render.PrePostrender)
                    {
                        _prepostrenderList.Add(render);
                    }
                    else if(render.Priority <= Render.Postrender)
                    {
                        _postrenderList.Add(render);
                    }
                    else
                    {
                        _renderList.Add(render);
                    }
                }

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
            // prerender sort
            _prerenderList.Sort(delegate (Render x, Render y)
            {
                if (x.Priority > y.Priority)
                {
                    return -1;
                }
                else if (x.Priority < y.Priority)
                {
                    return 1;
                }

                return 0;

            });

            // pre post render sort
            _prepostrenderList.Sort(delegate (Render x, Render y)
            {
                if (x.Priority > y.Priority)
                {
                    return -1;
                }
                else if (x.Priority < y.Priority)
                {
                    return 1;
                }

                return 0;

            });

            // render sort
            _renderList.Sort(delegate(Render x, Render y)
            {
                if(x.Priority > y.Priority)
                {
                    return -1;
                }
                else if(x.Priority < y.Priority)
                {
                    return 1;
                }
                else
                {
                    Node xNode;
                    Node yNode;
                    
                    if(x.WNode.TryGetTarget(out xNode) &&
                    y.WNode.TryGetTarget(out yNode))
                    {
                        float x_z = xNode.ModelViewPosition().Z;
                        float y_z = yNode.ModelViewPosition().Z;

                        if (x_z > y_z)
                        {
                            return 1;
                        }
                        else if (x_z < y_z)
                        {
                            return -1;
                        }

                    }
                    
                    return 0;
                }

            });


            // postrender sort
            //TODO

            // for debug
            //Log.Print("***sortSceneGraph****");
            //foreach (var i in _renderList)
            //{
            //    Log.Print(i.testid.ToString());
            //}
        }

        private void doPrerender(FrameEventArgs e)
        {
            foreach(var pre in _prerenderList)
            {
                // prerender begin
                pre.OnRenderBegin(e);

                foreach(var render in _renderList)
                {
                    if(!render.PassPreRender)
                    {
                        if( pre.PreRenderRange.X <= render.Priority && 
                            render.Priority <= pre.PreRenderRange.Y)
                        {
                            render.ReplaceRender = pre;
                            render.OnRenderBegin(e);
                            render.OnRender(e);
                            render.OnRenderFinsh(e);
                        }
                    }
                }

                // prerender end
                pre.OnRenderFinsh(e);
                
            }


        }

        private void doPrepostrender(FrameEventArgs e)
        {
            foreach (var prepost in _prepostrenderList)
            {
                prepost.OnRenderBegin(e);
                prepost.OnRender(e);
                prepost.OnRenderFinsh(e);

            }
        }


        private void doRender(FrameEventArgs e)
        {
            GL.ClearColor(Color4.Gray);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            foreach(var render in _renderList)
            {
                render.OnRenderBegin(e);
                render.OnRender(e);
                render.OnRenderFinsh(e);
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

            Render.DrawcallCount = 0;
            // prerender
            doPrerender(e);

            // prepost render
            doPrepostrender(e);

            doRender(e);

            // postrender
            //TODO
        }

    }
}
