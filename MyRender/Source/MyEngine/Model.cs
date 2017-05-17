using MyRender.Debug;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;

namespace MyRender.MyEngine
{
    class Model
    {
        public string guid;
        public Vector3[] Vertices;
        public Vector3[] Normals;
        public Vector2[] Texcoords;
        public int[] Element;
        public int VBO = 0; // vertex array
        public int EBO = 0; // elementes array
        public int TBO = 0; // texture coord
        public int NBO = 0; // normal array

        public void Release()
        {
            if (VBO != 0) GL.DeleteBuffer(VBO);
            if (EBO != 0) GL.DeleteBuffer(EBO);
            if (TBO != 0) GL.DeleteBuffer(TBO);
            if (NBO != 0) GL.DeleteBuffer(NBO);
        }


    }
}
