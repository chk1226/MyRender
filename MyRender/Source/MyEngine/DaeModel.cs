using grendgine_collada;
using MyRender.Debug;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System.Runtime.InteropServices;

namespace MyRender.MyEngine
{
    class DaeModel : Node
    {

        public virtual bool Loader(string path)
        {
            var dae = Grendgine_Collada.Grendgine_Load_File(path);
            if(dae == null)
            {
                Log.Print("[DaeModel.Loader] model load fail");
                return false;
            }

            Grendgine_Collada_Library_Geometries l_g = dae.Library_Geometries;
            ModelList = new Model[l_g.Geometry.Length];

            for (int i = 0; i < l_g.Geometry.Length; i++)
            {
                var modelData = Resource.Instance.GetModel(path + i);
                if (modelData != null)
                {
                    ModelList[i] = modelData;
                    continue;
                }

                modelData = new Model();
                modelData.guid = path + i;

                //get geometries data
            
                //vertex
                float[] pos = l_g.Geometry[i].Mesh.Source[0].Float_Array.Value();

                //normal
                float[] nor = l_g.Geometry[i].Mesh.Source[1].Float_Array.Value();

                //texture coordinate
                float[] tex = l_g.Geometry[i].Mesh.Source[2].Float_Array.Value();
                int tex_stride = l_g.Geometry[i].Mesh.Source[2].Technique_Common.Accessor.Param.Length;

                //index
                int mesh_num = 0;
                int[] index = { };
                int mesh_vertex_num = 0;
                int stride = 0;

                if (l_g.Geometry[i].Mesh.Triangles != null)
                {
                    mesh_num = l_g.Geometry[i].Mesh.Triangles[0].Count;
                    index = l_g.Geometry[i].Mesh.Triangles[0].P.Value();
                    modelData.DrawType = PrimitiveType.Triangles;
                    mesh_vertex_num = 3;
                    stride = l_g.Geometry[i].Mesh.Triangles[0].Input.Length;
                }
                else if(l_g.Geometry[i].Mesh.Polylist != null)
                {
                    mesh_num = l_g.Geometry[i].Mesh.Polylist[0].Count;
                    index = l_g.Geometry[i].Mesh.Polylist[0].P.Value();

                    if (l_g.Geometry[i].Mesh.Polylist[0].VCount != null &&
                        l_g.Geometry[i].Mesh.Polylist[0].VCount.Value()[0] == 4)
                    {
                        modelData.DrawType = PrimitiveType.Quads;
                        mesh_vertex_num = 4;
                    }
                    else
                    {
                        modelData.DrawType = PrimitiveType.Triangles;
                        mesh_vertex_num = 3;
                    }

                    stride = l_g.Geometry[i].Mesh.Polylist[0].Input.Length;

                }
                modelData.id = l_g.Geometry[i].ID;


                modelData.Vertices = new Vector3[mesh_num * mesh_vertex_num];
                modelData.Normals = new Vector3[mesh_num * mesh_vertex_num];
                modelData.Texcoords = new Vector2[mesh_num * mesh_vertex_num];

                int reg;
                for (int num_2 = 0; num_2 < mesh_num * mesh_vertex_num; num_2++)
                {
                    reg = index[num_2 * stride];
                    modelData.Vertices[num_2] =
                        new Vector3(pos[reg * 3], pos[reg * 3 + 1], pos[reg * 3 + 2]);

                    reg = index[num_2 * stride + 1];
                    modelData.Normals[num_2] =
                        new Vector3(nor[reg * 3], nor[reg * 3 + 1], nor[reg * 3 + 2]);

                    reg = index[num_2 * stride + 2];
                    modelData.Texcoords[num_2] =
                        new Vector2(tex[reg * tex_stride], tex[reg * tex_stride + 1]);

                }

                modelData.ComputeTangentBasis();

                // gen vertex buffer
                modelData.VBO = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ArrayBuffer, modelData.VBO);
                int size = modelData.Vertices.Length * Marshal.SizeOf(default(Vector3));
                GL.BufferData(BufferTarget.ArrayBuffer, size, modelData.Vertices, BufferUsageHint.StaticDraw);

                // gen texture cood buffer
                modelData.TBO = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ArrayBuffer, modelData.TBO);
                size = modelData.Texcoords.Length * Marshal.SizeOf(default(Vector2));
                GL.BufferData(BufferTarget.ArrayBuffer, size, modelData.Texcoords, BufferUsageHint.StaticDraw);

                // gen texture cood buffer
                modelData.NBO = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ArrayBuffer, modelData.NBO);
                size = modelData.Normals.Length * Marshal.SizeOf(default(Vector3));
                GL.BufferData(BufferTarget.ArrayBuffer, size, modelData.Normals, BufferUsageHint.StaticDraw);

                // gen tangent buffer 
                modelData.TangentBuffer = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ArrayBuffer, modelData.TangentBuffer);
                size = modelData.Tangent.Length * Marshal.SizeOf(default(Vector3));
                GL.BufferData(BufferTarget.ArrayBuffer, size, modelData.Tangent, BufferUsageHint.StaticDraw);

                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

                Resource.Instance.AddModel(modelData);
                ModelList[i] = modelData;
            }

            return true;
        }

    }
}
