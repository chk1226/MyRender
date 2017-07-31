using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace MyRender.MyEngine
{
    class Joint
    {
        public string sid; 
        public string id; // id
        public List<Joint> children = new List<Joint>();

        public Matrix4 animatedTransform = Matrix4.Identity;
        public Matrix4 localBindTransform = Matrix4.Identity;
        public Matrix4 inverseBindTransform = Matrix4.Identity;

        // for debug
        //public Matrix4 debugInvBindTransform = Matrix4.Identity;
        //public Matrix4 aaa = Matrix4.Identity;


        //public void CalcInverseBindTransform(Matrix4 parentBindTransform)
        //{
        //    var bindTransform = parentBindTransform * localBindTransform;
        //    inverseBindTransform = Matrix4.Invert(bindTransform);

        //    foreach (var child in children)
        //    {
        //        child.CalcInverseBindTransform(bindTransform);
        //    }
        //}

    }


}
