using OpenTK;
using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;

namespace MyRender.MyEngine
{
    class Render
    {
        public static readonly int Prerender = 1500;
        public static readonly int PrePostrender = 1300;
        public static readonly int Skybox = 1250;
        public static readonly int Normal = 1000;
        public static readonly int UI = 750;
        public static readonly int Postrender = 500;
        public static int DrawcallCount = 0;

        public int Priority;
        public Render ReplaceRender;
        public Action<Render> ShaderAction;
        public Material MaterialData;
        public Model ModelData;

        public WeakReference<Node> WNode;
        private List<VertexAttribute> vertexAttribute = new List<VertexAttribute>();
        private bool enableBlend = false;
        private bool enableCubemap = false;
        private bool enableDepthFunc = false;
        private BlendingFactorSrc sfactor;
        private BlendingFactorDest dfactor;
        private DepthFunction depthFunc;


        private Render() { }

        public static Render CreateRender(Material material, Action<Render> shaderAction, Node node, Model model, int priority)
        {
            var render = new Render();

            if (material == null) return null;
            if (node == null) return null;

            render.MaterialData = material;
            render.ShaderAction = shaderAction;
            render.Priority = priority;
            render.ModelData = model;
            render.WNode = new WeakReference<Node>(node);

            return render;
        }


        public void AddVertexAttribute(string name, int bufferID, int size, bool normalize)
        {
            var va = new VertexAttribute();
            va.name = name;
            va.bufferID = bufferID;
            va.size = size;
            va.normalize = normalize;
            va.shader = MaterialData.ShaderProgram;

            vertexAttribute.Add(va);
        }

        public void EnableBlend(BlendingFactorSrc s, BlendingFactorDest d)
        {
            enableBlend = true;
            sfactor = s;
            dfactor = d;
        }

        public void EnableCubemap()
        {
            enableCubemap = true;
        }

        public void EnableDepthFunc(DepthFunction f)
        {
            enableDepthFunc = true;
            depthFunc = f;
        }

        public void OnRenderBegin(FrameEventArgs e)
        {
            Node node;
            if(WNode.TryGetTarget(out node))
            {
                node.OnRenderBegin(e);
            }
        }

        public void OnRender(FrameEventArgs e)
        {
            if (ModelData == null) return;

            if(ReplaceRender != null)
            {
                ReplaceRender.ShaderAction?.Invoke(ReplaceRender);
            }
            else
            {
                ShaderAction?.Invoke(this);
            }

            if(enableDepthFunc)
            {
                GL.DepthFunc(depthFunc);
            }

            if(enableBlend)
            {
                GL.BlendFunc(sfactor, dfactor);
                GL.Enable(EnableCap.Blend);
            }

            // bind vertex buffer 
            if (ModelData.VBO != 0)
            {
                GL.BindBuffer(BufferTarget.ArrayBuffer, ModelData.VBO);
                GL.EnableClientState(ArrayCap.VertexArray);
                GL.VertexPointer(3, VertexPointerType.Float, 0, 0);
            }

            // bind normal buffer
            if(ModelData.NBO != 0)
            {
                GL.BindBuffer(BufferTarget.ArrayBuffer, ModelData.NBO);
                GL.EnableClientState(ArrayCap.NormalArray);
                GL.NormalPointer(NormalPointerType.Float, 0, 0);
            }

            // bind texture coord buffer
            if(ModelData.TBO != 0)
            {
                GL.BindBuffer(BufferTarget.ArrayBuffer, ModelData.TBO);
                GL.EnableClientState(ArrayCap.TextureCoordArray);
                GL.TexCoordPointer(2, TexCoordPointerType.Float, 0, 0);
            }

            // bind Vertex Attrib Array
            foreach (var va in vertexAttribute)
            {
                va.BindVertexAttribute();
            }

            DrawCall(ModelData.DrawType, ModelData.Vertices.Length);

            if(ModelData.VBO != 0)
            {
                GL.DisableClientState(ArrayCap.VertexArray);
            }

            if(ModelData.NBO != 0)
            {
                GL.DisableClientState(ArrayCap.NormalArray);
            }

            if(ModelData.TBO != 0)
            {
                GL.DisableClientState(ArrayCap.TextureCoordArray);
            }

            //Disable Vertex AttribArray
            foreach (var va in vertexAttribute)
            {
                GL.DisableVertexAttribArray(va.LocationID);
            }

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            if(enableCubemap)
            {
                GL.BindTexture(TextureTarget.TextureCubeMap, 0);
            }
            GL.BindTexture(TextureTarget.Texture2D, 0);
            
            if (ReplaceRender != null)
            {
                ReplaceRender = null;
            }

            if (enableBlend)
            {
                GL.Disable(EnableCap.Blend);
            }

            if(enableDepthFunc)
            {
                GL.DepthFunc(DepthFunction.Less);
            }

            GL.UseProgram(0);
        }

        public void OnRenderFinsh(FrameEventArgs e)
        {
            Node node;
            if (WNode.TryGetTarget(out node))
            {
                node.OnRenderFinsh(e);
            }
        }

        public void DrawCall(PrimitiveType type, int length)
        {
            GL.DrawArrays(type, 0, length);
            DrawcallCount++;
        }

        public void Release()
        {
            vertexAttribute.Clear();
            MaterialData = null;
            ModelData = null;
        }


        public class VertexAttribute
        {
            public string name;
            public int bufferID;
            public int size;
            public bool normalize;
            public int shader;
            public int LocationID;

            public void BindVertexAttribute()
            {
                LocationID = GL.GetAttribLocation(shader, name);
                GL.EnableVertexAttribArray(LocationID);
                GL.BindBuffer(BufferTarget.ArrayBuffer, bufferID);
                GL.VertexAttribPointer(LocationID, size, VertexAttribPointerType.Float, normalize, 0, 0);
            }
        }
    }
}
