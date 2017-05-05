using OpenTK;


namespace MyRender.MyEngine
{
    class Node
    {
        //public Quaternion localRoation;

        //public Node()
        //{
        //    localRoation = Quaternion.Identity;
        //}

        public virtual void Rotation(Quaternion q) { }
        public virtual void LocalPosition(Vector3 pos) { }
        //public virtual void Rotation(Vector3 eulerAngles) { }


        public virtual void OnUpdate(FrameEventArgs e) { }
        public virtual void OnRender(FrameEventArgs e) { }

    }
}
