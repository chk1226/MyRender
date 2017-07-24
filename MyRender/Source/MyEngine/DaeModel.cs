using grendgine_collada;
using MyRender.Debug;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;

namespace MyRender.MyEngine
{
    class DaeModel : Node
    {
        //protected class SkinLoaderData
        //{
        //    public string ControllerID;
        //    public string[] JointsList;
        //    public float[] WeightList;
        //    public int[] EffectiveJointCount;

        //}
        protected AnimationModel Animation;

        protected class MeshSkinData
        {
            public VertexSkinData[] VertexData;
            public string[] Joints;
        }

        protected struct VertexSkinData
        {
            public Vector4 Jointsindex;
            public Vector4 Weights;
        }


        public virtual bool Loader(string path)
        {
            var dae = Grendgine_Collada.Grendgine_Load_File(path);
            if(dae == null)
            {
                Log.Print("[DaeModel.Loader] model load fail");
                return false;
            }

            MeshSkinData[] meshSkinData = null;
            if (dae.Library_Controllers != null)
            {
                meshSkinData = skinLoader(dae.Library_Controllers);
            }

            Grendgine_Collada_Library_Geometries l_g = dae.Library_Geometries;
            ModelList = new Model[l_g.Geometry.Length];
            
            geometriesLoader(l_g, path, meshSkinData);

            if (dae.Library_Animations != null &&
                dae.Library_Visual_Scene != null)
            {
                skeletonLoader(dae.Library_Visual_Scene, meshSkinData);
                //animationLoader(dae.Library_Animations);
            }

            return true;
        }

        private void skeletonLoader(Grendgine_Collada_Library_Visual_Scenes l_s, MeshSkinData[] meshSkin)
        {
            var result = loadJointData(l_s.Visual_Scene[0].Node[1]);
            result.CalcInverseBindTransform(Matrix4.Identity);
            Animation = new AnimationModel();
            Animation.JointHierarchy.Add(result);

            // create joint table
            foreach(var mesh in meshSkin)
            {
                Animation.CreateHashJoint(mesh.Joints);
            }
        }

        private Joint loadJointData(Grendgine_Collada_Node data)
        {
            Joint joint = new Joint();

            joint.sid = data.sID;
            joint.name = data.ID;

            var localBind = Matrix4.Identity;
            //matrix
            if (data.Matrix != null)
            {
                var value = data.Matrix[0].Value();
                Matrix4 mat = new Matrix4();
                mat.Row0 = new Vector4(value[0], value[1], value[2], value[3]);
                mat.Row1 = new Vector4(value[4], value[5], value[6], value[7]);
                mat.Row2 = new Vector4(value[8], value[9], value[10], value[11]);
                mat.Row3 = new Vector4(value[12], value[13], value[14], value[15]);
                mat.Transpose();
                localBind = mat * localBind;
            }
            else
            {
                if (data.Rotate != null)
                {
                    for (int i = data.Rotate.Length - 1; i >= 0; i--)
                    {
                        var value = data.Rotate[i].Value();
                        var q = Algorithm.CreateFromAxisAngle(value[0], value[1], value[2], value[3] * Algorithm.Radin);
                        var mat = Matrix4.CreateFromQuaternion(q);
                        mat.Transpose();

                        localBind = mat * localBind;

                        if(data.Rotate[i].sID == "ScaleAxis")
                        {
                            // scale
                            if(data.Scale != null)
                            {
                                var scaleValue = data.Scale[0].Value();
                                mat = Matrix4.CreateScale(scaleValue[0], scaleValue[1], scaleValue[2]);
                                mat.Transpose();

                                localBind = mat * localBind;
                            }

                        }
                    }
                }

                if (data.Translate != null)
                {
                    var value = data.Translate[0].Value();
                    var mat = Matrix4.CreateTranslation(value[0], value[1], value[2]);
                    mat.Transpose();
                    localBind = mat * localBind;
                }

            }

            joint.localBindTransform = localBind;

            // child
            if(data.node != null)
            {
                foreach(var child in data.node)
                {
                    joint.children.Add(loadJointData(child));
                }

            }

            return joint;        
        }

        private MeshSkinData[] skinLoader(Grendgine_Collada_Library_Controllers l_c)
        {
            MeshSkinData[] meshSkinData = new MeshSkinData[l_c.Controller.Length];

            for (int i = 0; i < l_c.Controller.Length; i++)
            {
                //var model = ModelList[i];
                var mesh = new MeshSkinData();
                var skin = l_c.Controller[i].Skin;

                // joint
                mesh.Joints = skin.Source[0].Name_Array.Value();
                // weight
                var weightList = skin.Source[2].Float_Array.Value();
                // effective joint count, vecor <=> joint|weight
                var effectiveJointCount = skin.Vertex_Weights.VCount.Value();
                // effective joint value
                var effectiveValue = skin.Vertex_Weights.V.Value();

                mesh.VertexData = new VertexSkinData[skin.Vertex_Weights.Count];
        
                int index = 0;
                int stride = 2;

                for(int j = 0; j < skin.Vertex_Weights.Count; j++)
                {
                    var effectNum = effectiveJointCount[j];
                    
                    var joint = new int[] { 0, 0, 0, 0};
                    var weight = new float[] { 0f, 0f, 0f, 0f};
                    bool over = false;
                    for (int k = 0; k < effectNum; k++)
                    {
                        if (k > joint.Length)
                        {
                            over = true;
                            break;
                        }
                        joint[k] = effectiveValue[index + k * stride];
                        weight[k] = weightList[effectiveValue[index + k * stride + 1]];
                    }

                    if(over)
                    {
                        float total = 0;
                        foreach(var w in weight)
                        {
                            total += w;
                        }
                        for (int l = 0; l < weight.Length; l++)
                        {
                            weight[l] /= total;
                        }
                    }

                    index += stride * effectNum;

                    mesh.VertexData[j].Jointsindex = new Vector4(joint[0], joint[1], joint[2], joint[3]);
                    mesh.VertexData[j].Weights = new Vector4(weight[0], weight[1], weight[2], weight[3]);

                }

                meshSkinData[i] = mesh;
            }

            return meshSkinData;
        }

        private void animationLoader(Grendgine_Collada_Library_Animations l_ani)
        {

            // get time
            var times = l_ani.Animation[0].Source[0].Float_Array.Value();
            var during = times[times.Length - 1];

            // create keyframe and set time stamp value
            KeyFrame[] keyframe = new KeyFrame[times.Length];
            for (int i = 0; i < times.Length; i++)
            {
                keyframe[i] = new KeyFrame();
                keyframe[i].timeStamp = times[i];
            }
           
            foreach (var jointNode in l_ani.Animation)
            {
                var jointName = jointNode.Channel[0].Target.Split('/')[0];
                var output = jointNode.Source[1];
                if (!output.ID.Contains("output"))
                {
                    Log.Assert("[animationLoader] output of source not found");
                }

                Matrix4 mat = Matrix4.Identity;
                Vector3 scale = Vector3.One;
                var floatArray = output.Float_Array.Value();
                var output_stride = output.Technique_Common.Accessor.Stride;

                for (int index = 0; index < floatArray.Length / output_stride; index++)
                {

                    if (output_stride == 16)
                    {
                        mat.Row0 = new Vector4(floatArray[index * output_stride], floatArray[index * output_stride + 1], floatArray[index * output_stride + 2], floatArray[index * output_stride + 3]);
                        mat.Row1 = new Vector4(floatArray[index * output_stride + 4], floatArray[index * output_stride + 5], floatArray[index * output_stride + 6], floatArray[index * output_stride + 7]);
                        mat.Row2 = new Vector4(floatArray[index * output_stride + 8], floatArray[index * output_stride + 9], floatArray[index * output_stride + 10], floatArray[index * output_stride + 11]);
                        mat.Row3 = new Vector4(floatArray[index * output_stride + 12], floatArray[index * output_stride + 13], floatArray[index * output_stride + 14], floatArray[index * output_stride + 15]);
                        // TODO do Transpose?
                        mat.Transpose();
                        scale = Vector3.One;
                    }
                    else if (output_stride == 3)
                    {
                        scale.X = floatArray[index * output_stride];
                        scale.Y = floatArray[index * output_stride + 1];
                        scale.Z = floatArray[index * output_stride + 2];
                        mat = Matrix4.Identity;
                    }
                    else
                    {
                        Log.Assert("[animationLoader] unknow output");
                    }

                    var jointTransform = new JointTransform();
                    jointTransform.position = new Vector3(mat.M31, mat.M32, mat.M33);
                    jointTransform.rotation = Quaternion.FromMatrix(new Matrix3(mat));
                    jointTransform.scale = scale;

                    keyframe[index].pose.Add(jointName, jointTransform);
                }
            }

            var animation = new Animation();
            animation.length = during;
            animation.keyFrames = keyframe;
        }

        private void geometriesLoader(Grendgine_Collada_Library_Geometries l_g, string path, MeshSkinData[] meshSkinData)
        {
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
                else if (l_g.Geometry[i].Mesh.Polylist != null)
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

                if(meshSkinData != null)
                {
                    modelData.JointsIndex = new Vector4[mesh_num * mesh_vertex_num];
                    modelData.Weights = new Vector4[mesh_num * mesh_vertex_num];
                }

                int reg;
                for (int num_2 = 0; num_2 < mesh_num * mesh_vertex_num; num_2++)
                {
                    reg = index[num_2 * stride];
                    modelData.Vertices[num_2] =
                        new Vector3(pos[reg * 3], pos[reg * 3 + 1], pos[reg * 3 + 2]);

                    if(meshSkinData != null)
                    {
                        modelData.JointsIndex[num_2] = meshSkinData[i].VertexData[reg].Jointsindex;
                        modelData.Weights[num_2] = meshSkinData[i].VertexData[reg].Weights;
                    }

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

                if(meshSkinData != null)
                {
                    // gen jointIndices
                    modelData.JointBuffer = GL.GenBuffer();
                    GL.BindBuffer(BufferTarget.ArrayBuffer, modelData.JointBuffer);
                    size = modelData.JointsIndex.Length * Marshal.SizeOf(default(Vector4));
                    GL.BufferData(BufferTarget.ArrayBuffer, size, modelData.JointsIndex, BufferUsageHint.StaticDraw);

                    // gen weight
                    modelData.WeightBuffer = GL.GenBuffer();
                    GL.BindBuffer(BufferTarget.ArrayBuffer, modelData.WeightBuffer);
                    size = modelData.Weights.Length * Marshal.SizeOf(default(Vector4));
                    GL.BufferData(BufferTarget.ArrayBuffer, size, modelData.Weights, BufferUsageHint.StaticDraw);
                }

                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

                Resource.Instance.AddModel(modelData);
                ModelList[i] = modelData;
            }

        }

    }
}
