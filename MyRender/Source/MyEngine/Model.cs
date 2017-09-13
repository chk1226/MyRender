using MyRender.Debug;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
// TODO
// will modify
namespace MyRender.MyEngine
{
    class Model
    {
        public enum BufferType
        {
            Rotation,
            Scale,
            BlendFactor,
            JointsIndex,
            Weights,
            ParticleTextureCoord,
            Vertices,
            Normals,
            Tangent,
            Texcoords
        }

        public class BufferData
        {
            public int BufferID = 0;
            public float[] floatData;
            public Vector4[] vec4Data;
            public Vector3[] vec3Data;
            public Vector2[] vec2Data;
        }

        public PrimitiveType DrawType;
        public string guid;
        public string id;


        private Dictionary<BufferType, BufferData> modelBuffer = new Dictionary<BufferType, BufferData>();
        
        public BufferData GetBufferData(BufferType key)
        {
            if(!modelBuffer.ContainsKey(key))
            {
                return null;
            }

            return modelBuffer[key];
        }

        public void Release()
        {
            foreach (var buf in modelBuffer)
            {
                if(buf.Value.BufferID != 0)
                {
                    GL.DeleteTexture(buf.Value.BufferID);
                }
            }
            modelBuffer.Clear();
        }

        public Vector3[] ComputeTangentBasis()
        {
                 
            if(GetBufferData(BufferType.Vertices) == null ||
                GetBufferData(BufferType.Texcoords) == null)
            {
                Log.Print("[ComputeTangentBasis] Tangent Gen Fail");
                return null;
            }

            var Vertices = GetBufferData(BufferType.Vertices).vec3Data;
            var Texcoords = GetBufferData(BufferType.Texcoords).vec2Data;

            if(Vertices.Length != Texcoords.Length)
            {
                Log.Print("[ComputeTangentBasis] Tangent Gen Fail");
                return null;
            }

            var Tangent = new Vector3[Vertices.Length];
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
                    return null;
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

                return Tangent;
            }
            catch (Exception e)
            {
                Log.Print("[ComputeTangentBasis][Exception] " + e.Message);
                return null;
            }
            
        }

        public void ReloadBufferVec2Data(BufferType key)
        {
            if (!modelBuffer.ContainsKey(key))
            {
                Log.Print("[ReloadBufferFloatData] not contain key : " + key.ToString());
                return;
            }

            var data = modelBuffer[key];
            GL.BindBuffer(BufferTarget.ArrayBuffer, data.BufferID);
            int size = data.vec2Data.Length * Marshal.SizeOf(default(Vector2));
            GL.BufferData(BufferTarget.ArrayBuffer, size, data.vec2Data, BufferUsageHint.StaticDraw);
        }

        public int GenVec2Buffer(BufferType key, Vector2[] data)
        {
            var d = new BufferData();
            d.BufferID = GL.GenBuffer();
            d.vec2Data = data;
            modelBuffer.Add(key, d);
            ReloadBufferVec2Data(key);

            return d.BufferID;
        }

        public void ReloadBufferVec3Data(BufferType key)
        {
            if (!modelBuffer.ContainsKey(key))
            {
                Log.Print("[ReloadBufferFloatData] not contain key : " + key.ToString());
                return;
            }

            var data = modelBuffer[key];
            GL.BindBuffer(BufferTarget.ArrayBuffer, data.BufferID);
            int size = data.vec3Data.Length * Marshal.SizeOf(default(Vector3));
            GL.BufferData(BufferTarget.ArrayBuffer, size, data.vec3Data, BufferUsageHint.StaticDraw);
        }

        public int GenVec3Buffer(BufferType key, Vector3[] data)
        {
            var d = new BufferData();
            d.BufferID = GL.GenBuffer();
            d.vec3Data = data;
            modelBuffer.Add(key, d);
            ReloadBufferVec3Data(key);

            return d.BufferID;
        }

        public void ReloadBufferVec4Data(BufferType key)
        {
            if (!modelBuffer.ContainsKey(key))
            {
                Log.Print("[ReloadBufferFloatData] not contain key : " + key.ToString());
                return;
            }

            var data = modelBuffer[key];
            GL.BindBuffer(BufferTarget.ArrayBuffer, data.BufferID);
            int size = data.vec4Data.Length * Marshal.SizeOf(default(Vector4));
            GL.BufferData(BufferTarget.ArrayBuffer, size, data.vec4Data, BufferUsageHint.StaticDraw);
        }

        public int GenVec4Buffer(BufferType key, Vector4[] data)
        {
            var d = new BufferData();
            d.BufferID = GL.GenBuffer();
            d.vec4Data = data;
            modelBuffer.Add(key, d);
            ReloadBufferVec4Data(key);

            return d.BufferID;
        }

        public void ReloadBufferFloatData(BufferType key)
        {
            if(!modelBuffer.ContainsKey(key))
            {
                Log.Print("[ReloadBufferFloatData] not contain key : " + key.ToString());
                return;
            }

            var data = modelBuffer[key];
            GL.BindBuffer(BufferTarget.ArrayBuffer, data.BufferID);
            int size = data.floatData.Length * Marshal.SizeOf(default(float));
            GL.BufferData(BufferTarget.ArrayBuffer, size, data.floatData, BufferUsageHint.StaticDraw);
        }

        public int GenFloatBuffer(BufferType key, float[] data)
        {
            var d = new BufferData();
            d.BufferID = GL.GenBuffer();
            d.floatData = data;
            modelBuffer.Add(key, d);
            ReloadBufferFloatData(key);

            return d.BufferID;
        }

        public static Model CreateCubeData()
        {
            var modelData = new Model();
            modelData.DrawType = PrimitiveType.Quads;

            modelData.GenVec3Buffer(BufferType.Vertices, new[]
                {
                    // front face
                    new Vector3(-1.0f, -1.0f,  1.0f),
                    new Vector3( 1.0f, -1.0f,  1.0f),
                    new Vector3( 1.0f,  1.0f,  1.0f),
                    new Vector3(-1.0f,  1.0f,  1.0f),
                    // back face
                    new Vector3( -1.0f, -1.0f, -1.0f),
                    new Vector3( 1.0f, -1.0f, -1.0f),
                    new Vector3( 1.0f, 1.0f, -1.0f),
                    new Vector3( -1.0f, 1.0f, -1.0f),
                    // top face
                    new Vector3( -1.0f, 1.0f, 1.0f),
                    new Vector3( 1.0f, 1.0f, 1.0f),
                    new Vector3( 1.0f, 1.0f, -1.0f),
                    new Vector3( -1.0f, 1.0f, -1.0f),
                    // bottom face
                    new Vector3( -1.0f, -1.0f, 1.0f),
                    new Vector3( 1.0f, -1.0f, 1.0f),
                    new Vector3( 1.0f, -1.0f, -1.0f),
                    new Vector3( -1.0f, -1.0f, -1.0f),
                    // right face
                    new Vector3(1.0f, -1.0f, 1.0f),
                    new Vector3( 1.0f, -1.0f, -1.0f),
                    new Vector3( 1.0f,  1.0f, -1.0f),
                    new Vector3(1.0f,  1.0f, 1.0f),
                    // left face
                    new Vector3(-1.0f, -1.0f, 1.0f),
                    new Vector3( -1.0f, -1.0f, -1.0f),
                    new Vector3( -1.0f,  1.0f, -1.0f),
                    new Vector3(-1.0f,  1.0f, 1.0f)
                });

            modelData.GenVec3Buffer(BufferType.Normals, new[]
            {
                    new Vector3( 0.0f, 0.0f, 1.0f),
                    new Vector3( 0.0f, 0.0f, 1.0f),
                    new Vector3( 0.0f, 0.0f, 1.0f),
                    new Vector3( 0.0f, 0.0f, 1.0f),

                    new Vector3( 0.0f, 0.0f, -1.0f),
                    new Vector3( 0.0f, 0.0f, -1.0f),
                    new Vector3( 0.0f, 0.0f, -1.0f),
                    new Vector3( 0.0f, 0.0f, -1.0f),

                    new Vector3( 0.0f, 1.0f, 0.0f),
                    new Vector3( 0.0f, 1.0f, 0.0f),
                    new Vector3( 0.0f, 1.0f, 0.0f),
                    new Vector3( 0.0f, 1.0f, 0.0f),

                    new Vector3( 0.0f, -1.0f, 0.0f),
                    new Vector3( 0.0f, -1.0f, 0.0f),
                    new Vector3( 0.0f, -1.0f, 0.0f),
                    new Vector3( 0.0f, -1.0f, 0.0f),

                    new Vector3( 1.0f, 0.0f, 0.0f),
                    new Vector3( 1.0f, 0.0f, 0.0f),
                    new Vector3( 1.0f, 0.0f, 0.0f),
                    new Vector3( 1.0f, 0.0f, 0.0f),

                    new Vector3( -1.0f, 0.0f, 0.0f),
                    new Vector3( -1.0f, 0.0f, 0.0f),
                    new Vector3( -1.0f, 0.0f, 0.0f),
                    new Vector3( -1.0f, 0.0f, 0.0f),
                });

            modelData.GenVec2Buffer(BufferType.Texcoords, new[]
            {
                    new Vector2( 0.0f, 0.0f),
                    new Vector2( 0.0f, 1.0f),
                    new Vector2( 1.0f, 1.0f),
                    new Vector2( 1.0f, 0.0f),

                    new Vector2( 0.0f, 0.0f),
                    new Vector2( 0.0f, 1.0f),
                    new Vector2( 1.0f, 1.0f),
                    new Vector2( 1.0f, 0.0f),

                    new Vector2( 0.0f, 0.0f),
                    new Vector2( 0.0f, 1.0f),
                    new Vector2( 1.0f, 1.0f),
                    new Vector2( 1.0f, 0.0f),

                    new Vector2( 0.0f, 0.0f),
                    new Vector2( 0.0f, 1.0f),
                    new Vector2( 1.0f, 1.0f),
                    new Vector2( 1.0f, 0.0f),

                    new Vector2( 0.0f, 0.0f),
                    new Vector2( 0.0f, 1.0f),
                    new Vector2( 1.0f, 1.0f),
                    new Vector2( 1.0f, 0.0f),

                    new Vector2( 0.0f, 0.0f),
                    new Vector2( 0.0f, 1.0f),
                    new Vector2( 1.0f, 1.0f),
                    new Vector2( 1.0f, 0.0f),
                });
            return modelData;
        }

        public static Model CreateUIData()
        {
            var modelData = new Model();
            modelData.DrawType = PrimitiveType.Quads;

            modelData.GenVec3Buffer(BufferType.Vertices, new[]
            {
                    new Vector3(0, 1,  0.0f),
                    new Vector3(0, 0,  0.0f),
                    new Vector3(1, 0,  0.0f),
                    new Vector3(1, 1,  0.0f),
            });

            modelData.GenVec2Buffer( BufferType.Texcoords, new[] {
                    new Vector2( 0.0f, 1.0f),
                    new Vector2( 0.0f, 0.0f),
                    new Vector2( 1.0f, 0.0f),
                    new Vector2( 1.0f, 1.0f),
            });

            return modelData;
        }
    }
}
