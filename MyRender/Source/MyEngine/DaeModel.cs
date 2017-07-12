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
        //private readonly string daeGuid = "dae";

        public override void OnStart()
        {
            base.OnStart();

        }

        public bool Loader(string path)
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
                modelData.DrawType = PrimitiveType.Triangles;

                //get geometries data
                
                //vertex
                float[] pos = l_g.Geometry[i].Mesh.Source[0].Float_Array.Value();

                //normal
                float[] nor = l_g.Geometry[i].Mesh.Source[1].Float_Array.Value();

                //texture coordinate
                float[] tex = l_g.Geometry[i].Mesh.Source[2].Float_Array.Value();

                //index
                var mesh_num = l_g.Geometry[i].Mesh.Triangles[0].Count;
                int[] index = l_g.Geometry[i].Mesh.Triangles[0].P.Value();

                modelData.Vertices = new Vector3[mesh_num * 3];
                modelData.Normals = new Vector3[mesh_num * 3];
                modelData.Texcoords = new Vector2[mesh_num * 3];

                int reg;
                for (int num_2 = 0; num_2 < mesh_num * 3; num_2++)
                {
                    reg = index[num_2 * 3];
                    modelData.Vertices[num_2] =
                        new Vector3(pos[reg * 3], pos[reg * 3 + 1], pos[reg * 3 + 2]);

                    reg = index[num_2 * 3 + 1];
                    modelData.Normals[num_2] =
                        new Vector3(nor[reg * 3], nor[reg * 3 + 1], nor[reg * 3 + 2]);

                    reg = index[num_2 * 3 + 2];
                    modelData.Texcoords[num_2] =
                        new Vector2(tex[reg * 3], tex[reg * 3 + 1]);

                }

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

                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);


                Resource.Instance.AddModel(modelData);
                ModelList[i] = modelData;
            }


            MaterialData = Resource.Instance.CreateDaeM();

            return true;
        }


        public override void SetUpShader()
        {
            base.SetUpShader();

            if (MaterialData != null && MaterialData.ShaderProgram != 0)
            {
                var variable = GL.GetUniformLocation(MaterialData.ShaderProgram, "TEX_COLOR");
                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindTexture(TextureTarget.Texture2D, MaterialData.TextureID);
                GL.Uniform1(variable, 0);

                //GL.BindTexture(TextureTarget.Texture2D, 0);
            }
        }

        public override void OnRender(FrameEventArgs e)
        {
            base.OnRender(e);
            GL.Color4(Color4.White);  //byte型で指定

            foreach (var model in ModelList)
            {
                // bind vertex buffer 
                GL.BindBuffer(BufferTarget.ArrayBuffer, model.VBO);
                GL.EnableClientState(ArrayCap.VertexArray);
                GL.VertexPointer(3, VertexPointerType.Float, 0, 0);

                // bind normal buffer
                GL.BindBuffer(BufferTarget.ArrayBuffer, model.NBO);
                GL.EnableClientState(ArrayCap.NormalArray);
                GL.NormalPointer(NormalPointerType.Float, 0, 0);

                // bind texture coord buffer
                GL.BindBuffer(BufferTarget.ArrayBuffer, model.TBO);
                GL.EnableClientState(ArrayCap.TextureCoordArray);
                GL.TexCoordPointer(2, TexCoordPointerType.Float, 0, 0);

                // tangent buffer
                //var tangent = GL.GetAttribLocation(MaterialData.ShaderProgram, "tangent");
                //GL.EnableVertexAttribArray(tangent);
                //GL.BindBuffer(BufferTarget.ArrayBuffer, ModelList[0].TangentBuffer);
                //GL.VertexAttribPointer(tangent, 3, VertexAttribPointerType.Float, false, 0, 0);


                //GL.BindTexture(TextureTarget.Texture2D, MaterialData.TextureID);
                GL.DrawArrays(model.DrawType, 0, model.Vertices.Length);

                GL.DisableClientState(ArrayCap.VertexArray);
                GL.DisableClientState(ArrayCap.NormalArray);
                GL.DisableClientState(ArrayCap.TextureCoordArray);

                //GL.DisableVertexAttribArray(tangent);


                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
                GL.BindTexture(TextureTarget.Texture2D, 0);

            }


        }
    }
}
