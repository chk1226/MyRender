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
        public string name;
        public List<Joint> children = new List<Joint>();

        public Matrix4 animatedTransform;
        public Matrix4 localBindTransform;
        public Matrix4 inverseBindTransform;

        public void CalcInverseBindTransform(Matrix4 parentBindTransform)
        {
            var bindTransform = localBindTransform * parentBindTransform;
            inverseBindTransform = Matrix4.Invert(bindTransform);
            foreach ( var child in children)
            {
                child.CalcInverseBindTransform(bindTransform);
            }
        }

    }


}
