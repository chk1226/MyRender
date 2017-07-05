using MyRender.Debug;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;

namespace MyRender.MyEngine
{
    class Model
    {
        public PrimitiveType DrawType;
        public string guid;
        public Vector3[] Vertices;
        public Vector3[] Normals;
        public Vector2[] Texcoords;
        public Vector3[] Tangent;
        //public int[] Element;
        public int VBO = 0; // vertex array
        public int EBO = 0; // elementes array
        public int TBO = 0; // texture coord
        public int NBO = 0; // normal array
        public int TangentBuffer = 0; 

        public void Release()
        {
            if (VBO != 0) GL.DeleteBuffer(VBO);
            if (EBO != 0) GL.DeleteBuffer(EBO);
            if (TBO != 0) GL.DeleteBuffer(TBO);
            if (NBO != 0) GL.DeleteBuffer(NBO);
            if (TangentBuffer != 0) GL.DeleteBuffer(TangentBuffer);

        }

        public void ComputeTangentBasis()
        {
            if( Vertices == null || Texcoords == null || 
                this.Vertices.Length != Texcoords.Length )
            {
                Log.Print("[ComputeTangentBasis] Tangent Gen Fail");
                return;
            }

            Tangent = new Vector3[Vertices.Length];
            int offset = 0;
            switch (DrawType)
            {
                case PrimitiveType.Quads:
                    offset = 4;
                    break;
                case PrimitiveType.Triangles:
                    offset = 3;
                    break;

                default:
                    Log.Print("[ComputeTangentBasis] Tangent Gen Fail");
                    return;
            }

            try
            {
                for (int i = 0; i < Vertices.Length; i += offset)
                {
                    var edge1 = Vertices[i + 1] - Vertices[i];
                    var edge2 = Vertices[i + 2] - Vertices[i];

                    var dUV1 = Texcoords[i + 1] - Texcoords[i];
                    var dUV2 = Texcoords[i + 2] - Texcoords[i];

                    float f = 1.0f / (dUV1.X * dUV2.Y - dUV2.X * dUV1.Y);

                    Tangent[i].X = f * (dUV2.Y * edge1.X - dUV1.Y * edge2.X);
                    Tangent[i].Y = f * (dUV2.Y * edge1.Y - dUV1.Y * edge2.Y);
                    Tangent[i].Z = f * (dUV2.Y * edge1.Z - dUV1.Y * edge2.Z);

                    for (int k = 1; k < offset; k++)
                    {
                        Tangent[i + k] = Tangent[i];
                    }

                }
            }
            catch (Exception e)
            {
                Log.Print("[ComputeTangentBasis][Exception] " + e.Message);
                return;
            }


        }

    }
}
