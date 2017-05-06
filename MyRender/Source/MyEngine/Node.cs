using OpenTK;
using System;
using System.Collections.Generic;

namespace MyRender.MyEngine
{
    class Node
    {
        //public static int test = 0;
        //public int regTest;

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
        //public Quaternion localRoation;

        public Node()
        {
            //localRoation = Quaternion.Identity;
            OnStart();
            //regTest = test++;
        }
        

        public void AddChild(Node child)
        {
            if (child == null) return;

            if (child.Parent != null)
            {
                child.Parent._children.Remove(child._guid);
                GameDirect.Instance.OnUpdate -= child.OnUpdate;

            }

            child.Parent = this;
            _children.Add(child._guid, child);

            GameDirect.Instance.OnUpdate += child.OnUpdate;
        }

        public void SetParent(Node target)
        {
            if (target == null) return;

            if (Parent != null)
            {
                Parent._children.Remove(_guid);
                GameDirect.Instance.OnUpdate -= this.OnUpdate;

            }

            Parent = target;
            Parent._children.Add(_guid, this);
            GameDirect.Instance.OnUpdate += this.OnUpdate;

        }


        public virtual void Rotation(Quaternion q) { }
        public virtual void LocalPosition(Vector3 pos) { }

        public virtual void OnStart() { }
        public virtual void OnUpdate(FrameEventArgs e) { }
        public virtual void OnRender(FrameEventArgs e) { }



    }
}
