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

        public Vector3 ModelViewPosition
        {
            get {
                if(GameDirect.Instance.MainScene == null ||
                    GameDirect.Instance.MainScene.MainCamera == null)
                {
                    return Vector3.Zero;
                }

                var p = new Vector4(0, 0, 0, 1);
                p = GameDirect.Instance.MainScene.MainCamera.ViewMatrix * WorldModelMatrix * LocalModelMatrix * p;

                return p.Xyz;
            }
        }

        public Matrix4 LocalModelMatrix = Matrix4.Identity;
        public Matrix4 WorldModelMatrix = Matrix4.Identity;

        private Material _materialData;
        public Material MaterialData
        {
            get { return _materialData; }
            set { _materialData = value; }
        }
        
        public Model[] ModelList;
        public delegate void Action();
        protected Dictionary<string, Action> SetUpShaderAction = new Dictionary<string, Action>();

        public Node()
        {
            //localRoation = Quaternion.Identity;
            GameDirect.Instance.OnSatrt += OnStart;

            //testid = testflowid++;
        } 

        public void AddChild(Node child)
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

        public void SetParent(Node target)
        {
            if (target == null) return;

            if (Parent != null)
            {
                Parent._children.Remove(_guid);
                UnregisterCallback(this);
            }

            Parent = target;
            Parent._children.Add(_guid, this);
            this.WorldModelMatrix = Parent.WorldModelMatrix * Parent.LocalModelMatrix;
            RegisterCallback(this);
        }


        public void Rotation(float x, float y, float z, float w)
        {
            var q = Matrix4.CreateFromQuaternion(Algorithm.CreateFromAxisAngle(x, y, z, w * Algorithm.Radin));
            q.Transpose();

            LocalModelMatrix = q * LocalModelMatrix;
            effectChildWorldModelMatrix(q);
        }

        public virtual void OnStart()
        {
            GameDirect.Instance.OnSatrt -= OnStart;
        }
        public virtual void OnUpdate(FrameEventArgs e) { }
        public virtual void OnRelease() { }
        public virtual void OnRender(FrameEventArgs e)
        {
            GL.MatrixMode(MatrixMode.Modelview);
            GL.PushMatrix();
            var vm = GameDirect.Instance.MainScene.MainCamera.ViewMatrix * WorldModelMatrix * LocalModelMatrix;
            vm.Transpose();
            GL.LoadMatrix(ref vm);
        }

        public virtual void OnRenderFinsh(FrameEventArgs e)
        {
            if (MaterialData != null && MaterialData.ShaderProgram != 0)
            {
                GL.UseProgram(0);
            }
            GL.PopMatrix();
        }

        public virtual void OnMouseDown(MouseButtonEventArgs e) { }
        public virtual void OnMouseMove(MouseMoveEventArgs e) { }
        public virtual void OnMouseWheel(MouseWheelEventArgs e) { }


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
            GameDirect.Instance.OnMouseMove += node.OnMouseMove;
            GameDirect.Instance.OnMouseWheel += node.OnMouseWheel;
        }

        public void UnregisterCallback(Node node)
        {
            GameDirect.Instance.OnUpdate -= node.OnUpdate;
            GameDirect.Instance.OnMouseDown -= node.OnMouseDown;
            GameDirect.Instance.OnMouseMove -= node.OnMouseMove;
            GameDirect.Instance.OnMouseWheel -= node.OnMouseWheel;
        }

    }
}
