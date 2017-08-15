using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics;
using System;

namespace MyRender.MyEngine
{
    class Plane : Node
    {
        private Vector2 rect;
        private int sliceX;
        private int sliceY;
        private FrameBuffer useFrame;

        public Plane(float width, float height, uint sliceX, uint sliceY)
        {
            rect = new Vector2(width, height);
            this.sliceX = (int)sliceX;
            this.sliceY = (int)sliceY;
            ModelList = new Model[1];

            var modelData = Resource.Instance.GetModel(GUID);
            if (modelData == null)
            {
                modelData = new Model();
                modelData.DrawType = PrimitiveType.Quads;

                modelData.guid = GUID;

                CreatePlaneData(modelData, rect, this.sliceX, this.sliceY);
                // gen vertex buffer
                modelData.GenVerticesBuffer();

                // gen texture cood buffer
                modelData.GenTexcoordsBuffer();

                // gen normal texture cood buffer
                modelData.GenNormalBuffer();

                Resource.Instance.AddModel(modelData);
            }
            ModelList[0] = modelData;
        }

        public void SetFrameBuffer(FrameBuffer use)
        {
            useFrame = use;
        }

        public override void OnStart()
        {
            base.OnStart();
            var modelData = ModelList[0];
 
            Render render = Render.CreateRender(Resource.Instance.CreatePlaneM(), delegate (Render r) {
                var m = r.MaterialData;

                if (m.ShaderProgram != 0)
                {
                    GL.UseProgram(m.ShaderProgram);

                    m.UniformTexture("TEX_COLOR", TextureUnit.Texture0, Material.TextureType.Color, 0);

                    Light light;
                    if (GameDirect.Instance.MainScene.SceneLight.TryGetTarget(out light))
                    {
                        var dir = light.GetDirectVector();
                        m.Uniform3("DIR_LIGHT", dir.X, dir.Y, dir.Z);
                        if (light.EnableSadowmap)
                        {
                            if (useFrame != null) m.UniformTexture("SHADOWMAP", TextureUnit.Texture1, useFrame.CB_Texture, 1);
                            //HACK
                            m.UniformTexture("SSAO", TextureUnit.Texture2, Resource.Instance.GetFrameBuffer(FrameBuffer.Type.SSAOFrame).CB_Texture, 2);
                            var bmvp = light.LightBiasProjectView() * WorldModelMatrix * LocalModelMatrix;
                            m.UniformMatrix4("LIGHT_BPVM", ref bmvp, true);
                        }
                    }

                }
            },
            this,
            modelData,
            Render.Normal);

            RenderList.Add(render);
        }

        static public void CreatePlaneData(Model model, Vector2 rect, int sliceX, int sliceY)
        {
            if(model == null)
            {
                return;
            }

            Vector2 offset = new Vector2(rect.X / sliceX, rect.Y / sliceY);
            int total = sliceX * sliceY;
            model.Vertices = new Vector3[total * 4];
            model.Normals = new Vector3[total * 4];
            model.Texcoords = new Vector2[total * 4];

            Vector2 current = Vector2.Zero;
            for(int i = 0; i < sliceY; i++)
            {
                current.X = 0;
                current.Y = i * offset.Y;
                for(int j = 0; j < sliceX; j++)
                {
                    current.X = j * offset.X;

                    model.Vertices[i * (sliceX * 4) + j * 4] = new Vector3(current.X, 0, current.Y);
                    model.Vertices[i * (sliceX * 4) + j * 4 + 1] = new Vector3(current.X, 0, current.Y + offset.Y);
                    model.Vertices[i * (sliceX * 4) + j * 4 + 2] = new Vector3(current.X + offset.X, 0, current.Y + offset.Y);
                    model.Vertices[i * (sliceX * 4) + j * 4 + 3] = new Vector3(current.X + offset.X, 0, current.Y);

                    model.Normals[i * (sliceX * 4) + j * 4] = Vector3.UnitY;
                    model.Normals[i * (sliceX * 4) + j * 4 + 1] = Vector3.UnitY;
                    model.Normals[i * (sliceX * 4) + j * 4 + 2] = Vector3.UnitY;
                    model.Normals[i * (sliceX * 4) + j * 4 + 3] = Vector3.UnitY;

                    model.Texcoords[i * (sliceX * 4) + j * 4] = new Vector2(0, 0);
                    model.Texcoords[i * (sliceX * 4) + j * 4 + 1] = new Vector2(0, 1);
                    model.Texcoords[i * (sliceX * 4) + j * 4 + 2] = new Vector2(1, 1);
                    model.Texcoords[i * (sliceX * 4) + j * 4 + 3] = new Vector2(1, 0);


                }

            }


        }

    }
}
