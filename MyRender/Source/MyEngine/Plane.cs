using OpenTK;using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics;

namespace MyRender.MyEngine
{
    class Plane : Node
    {
        private Vector2 rect;
        private int sliceX;
        private int sliceY;

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

                createPlaneData(modelData);
                // gen vertex buffer
                modelData.GenVerticesBuffer();

                // gen texture cood buffer
                modelData.GenTexcoordsBuffer();

                // gen normal texture cood buffer
                modelData.GenNormalBuffer();

                Resource.Instance.AddModel(modelData);
            }
            ModelList[0] = modelData;

            // generate render object
            Render render = Render.CreateRender(Resource.Instance.CreatePlaneM(), delegate (Render r) {
                var m = r.MaterialData;

                if (m.ShaderProgram != 0)
                {
                    GL.UseProgram(m.ShaderProgram);

                    m.UniformTexture("TEX_COLOR", TextureUnit.Texture0, Material.TextureType.Color, 0);
                    m.UniformTexture("SHADOWMAP", TextureUnit.Texture1, GameDirect.Instance.DepthBuffer.DB_Texture, 1);
                    var view_mat = GameDirect.Instance.MainScene.MainCamera.ViewMatrix;
                    m.UniformMatrix4("VIEW_MAT", ref view_mat, true);
                    Light light;
                    if(GameDirect.Instance.MainScene.SceneLight.TryGetTarget(out light))
                    {
                        var mvp = light.LightProjectView() * WorldModelMatrix * LocalModelMatrix;
                        m.UniformMatrix4("LIGHT_PVM", ref mvp, true);
                    }

                }
            },
            this,
            modelData,
            Render.Normal);

            RenderList.Add(render);
        }


        //public override void OnRender(FrameEventArgs e)
        //{
        //    base.OnRender(e);
        
        //    if (MaterialData == null) return;
        //    if (SetUpShaderAction.ContainsKey(Resource.MPlaneGUID)) SetUpShaderAction[Resource.MPlaneGUID]();

        //    // bind vertex buffer 
        //    GL.BindBuffer(BufferTarget.ArrayBuffer, ModelList[0].VBO);
        //    GL.EnableClientState(ArrayCap.VertexArray);
        //    GL.VertexPointer(3, VertexPointerType.Float, 0, 0);

        //    // bind normal buffer
        //    GL.BindBuffer(BufferTarget.ArrayBuffer, ModelList[0].NBO);
        //    GL.EnableClientState(ArrayCap.NormalArray);
        //    GL.NormalPointer(NormalPointerType.Float, 0, 0);

        //    // bind texture coord buffer
        //    GL.BindBuffer(BufferTarget.ArrayBuffer, ModelList[0].TBO);
        //    GL.EnableClientState(ArrayCap.TextureCoordArray);
        //    GL.TexCoordPointer(2, TexCoordPointerType.Float, 0, 0);

        //    //GL.BindTexture(TextureTarget.Texture2D, MaterialData.TextureArray[Material.TextureType.Normal]);
        //    GameDirect.Instance.DrawCall(ModelList[0].DrawType, ModelList[0].Vertices.Length);

        //    GL.DisableClientState(ArrayCap.VertexArray);
        //    GL.DisableClientState(ArrayCap.NormalArray);
        //    GL.DisableClientState(ArrayCap.TextureCoordArray);

        //    GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        //    GL.BindTexture(TextureTarget.Texture2D, 0);

        //}

        private void createPlaneData(Model model)
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

                    model.Vertices[i * (sliceX * 4) + j * 4] = new Vector3(current.X, 0, current.Y + offset.Y);
                    model.Vertices[i * (sliceX * 4) + j * 4 + 1] = new Vector3(current.X, 0, current.Y);
                    model.Vertices[i * (sliceX * 4) + j * 4 + 2] = new Vector3(current.X + offset.X, 0, current.Y);
                    model.Vertices[i * (sliceX * 4) + j * 4 + 3] = new Vector3(current.X + offset.X, 0, current.Y + offset.Y);

                    model.Normals[i * (sliceX * 4) + j * 4] = Vector3.UnitY;
                    model.Normals[i * (sliceX * 4) + j * 4 + 1] = Vector3.UnitY;
                    model.Normals[i * (sliceX * 4) + j * 4 + 2] = Vector3.UnitY;
                    model.Normals[i * (sliceX * 4) + j * 4 + 3] = Vector3.UnitY;

                    model.Texcoords[i * (sliceX * 4) + j * 4] = new Vector2(0, 1);
                    model.Texcoords[i * (sliceX * 4) + j * 4 + 1] = new Vector2(0, 0);
                    model.Texcoords[i * (sliceX * 4) + j * 4 + 2] = new Vector2(1, 0);
                    model.Texcoords[i * (sliceX * 4) + j * 4 + 3] = new Vector2(1, 1);


                }

            }


        }

    }
}
