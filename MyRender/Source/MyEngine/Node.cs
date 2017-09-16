using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System;
using System.Collections.Generic;

namespace MyRender.MyEngine
{
    class Node
    {
        // FOR DEUB
        //public static int testflowid = 0;
        //public int testid = 0;

        private string _guid = Guid.NewGuid().ToString();
        public string GUID { get { return _guid; } }

        private readonly Dictionary<string, Node> _children = new Dictionary<string, Node>();
        public Dictionary<string, Node> Children
        {
            get { return _children; }
        }

        private Node _parent;
        public Node Parent
        {
            get { return _parent; }
            private set { _parent = value; }
        }

        private Vector3 _localPosition = Vector3.Zero;
        public Vector3 LocalPosition
        {
            get {
                return _localPosition;
            }
            set {
                var delta = value - _localPosition;
                _localPosition = value;
                var trans = Matrix4.Transpose(Matrix4.CreateTranslation(delta));
                LocalModelMatrix = trans * LocalModelMatrix;
                effectChildWorldModelMatrix(trans);
            }
        }

        public virtual Vector3 ModelViewPosition()
        {
            if(GameDirect.Instance.MainScene == null ||
                GameDirect.Instance.MainScene.MainCamera == null)
            {
                return Vector3.Zero;
            }

            var p = new Vector4(0, 0, 0, 1);
            p = GameDirect.Instance.MainScene.MainCamera.ViewMatrix * WorldModelMatrix * LocalModelMatrix * p;

            return p.Xyz;
        }

        public Matrix4 LocalModelMatrix = Matrix4.Identity;
        public Matrix4 WorldModelMatrix = Matrix4.Identity;
        public Model[] ModelList;
        public delegate void Action();
        public List<Render> RenderList = new List<Render>();
        public bool PassRender = false;

        private List<BaseComponent> componentList = new List<BaseComponent>();

        public Node()
        {
            GameDirect.Instance.OnSatrt += OnStart;

            //testid = testflowid++;
        } 

        public virtual void AddChild(Node child)
        {
            if (child == null) return;

            if (child.Parent != null)
            {
                child.Parent._children.Remove(child._guid);
                UnregisterCallback(child);

            }

            child.Parent = this;
            child.WorldModelMatrix = WorldModelMatrix * LocalModelMatrix;
            _children.Add(child._guid, child);

            RegisterCallback(child);
        }

        public void RemoveAllChild()
        {
            foreach (var child in Children.Values)
            {
                child.Parent = null;
                UnregisterCallback(child);
                GameDirect.Instance.OnSatrt -= child.OnStart;

                child.RemoveAllChild();
                child.OnRelease();
            }

            Children.Clear();

        }

        public void AddComponent(BaseComponent cmp)
        {
            componentList.Add(cmp);
        }

        public virtual void SetParent(Node target)
        {
            if (Parent != null)
            {
                Parent._children.Remove(_guid);
                UnregisterCallback(this);
            }

            Parent = target;
            if(target != null)
            {
                Parent._children.Add(_guid, this);
                this.WorldModelMatrix = Parent.WorldModelMatrix * Parent.LocalModelMatrix;
                RegisterCallback(this);
            }
        }


        public void Rotation(float x, float y, float z, float w)
        {
            var q = Matrix4.CreateFromQuaternion(Algorithm.CreateFromAxisAngle(x, y, z, MathHelper.DegreesToRadians( w )));
            q.Transpose();

            LocalModelMatrix = q * LocalModelMatrix;
            effectChildWorldModelMatrix(q);
        }

        public void Scale(float x, float y, float z)
        {
            var q = Matrix4.CreateScale(x, y, z);
            q.Transpose();

            LocalModelMatrix = q * LocalModelMatrix;
            effectChildWorldModelMatrix(q);
        }

        public virtual void OnStart()
        {
            GameDirect.Instance.OnSatrt -= OnStart;
        }
        public virtual void OnUpdate(FrameEventArgs e)
        {
            foreach(var c in componentList)
            {
                c.OnUpdate(e);
            }
        }

        public virtual void OnRenderBegin(FrameEventArgs e)
        {
            GL.MatrixMode(MatrixMode.Modelview);
            GL.PushMatrix();
            var vm = GameDirect.Instance.MainScene.MainCamera.ViewMatrix * WorldModelMatrix * LocalModelMatrix;
            vm.Transpose();
            GL.LoadMatrix(ref vm);
        }

        public virtual void OnRenderFinsh(FrameEventArgs e)
        {
            GL.PopMatrix();
        }

        public virtual void OnMouseDown(MouseButtonEventArgs e) { }
        public virtual void OnMouseUp(MouseButtonEventArgs e) { }
        public virtual void OnMouseMove(MouseMoveEventArgs e) { }
        public virtual void OnMouseWheel(MouseWheelEventArgs e) { }

        public virtual void OnRelease()
        {
            foreach(var r in RenderList)
            {
                r.Release();
            }

            RenderList.Clear();

            foreach(var c in componentList)
            {
                c.Release();
            }
            componentList.Clear();
        }

        private void effectChildWorldModelMatrix(Matrix4 effect)
        {
            foreach (var pair in Children)
            {
                var child = pair.Value;
                child.WorldModelMatrix = effect * child.WorldModelMatrix;
                child.effectChildWorldModelMatrix(effect);
            }
        }

        public void RegisterCallback(Node node)
        {
            GameDirect.Instance.OnUpdate += node.OnUpdate;
            GameDirect.Instance.OnMouseDown += node.OnMouseDown;
            GameDirect.Instance.OnMouseUp += node.OnMouseUp;
            GameDirect.Instance.OnMouseMove += node.OnMouseMove;
            GameDirect.Instance.OnMouseWheel += node.OnMouseWheel;
        }

        public void UnregisterCallback(Node node)
        {
            GameDirect.Instance.OnUpdate -= node.OnUpdate;
            GameDirect.Instance.OnMouseDown -= node.OnMouseDown;
            GameDirect.Instance.OnMouseUp -= node.OnMouseUp;
            GameDirect.Instance.OnMouseMove -= node.OnMouseMove;
            GameDirect.Instance.OnMouseWheel -= node.OnMouseWheel;
        }

    }
}
