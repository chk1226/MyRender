using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics;
using System;
using MyRender.MyEngine;

namespace MyRender.Game
{
    class TerrainPlane : Plane
    {

        public TerrainPlane(float width, float height, uint slicex, uint slicey)
            :base(width, height, slicex, slicey)
        {
            LocalPosition = new Vector3(-width / 2, 0, -height / 2);
        }

        public override void OnStart()
        {
            base.OnStart();

            refreshModelData();

            Material material = new Material();
            material.ShaderProgram = Resource.Instance.GetShader(Resource.STerrian);
            Render render = Render.CreateRender(material, delegate (Render r) {
                var m = r.MaterialData;

                if (m.ShaderProgram != 0)
                {
                    GL.UseProgram(m.ShaderProgram);

                    m.UniformTexture("TEX_COLOR", TextureUnit.Texture0, Resource.Instance.GetTextureID(Resource.ITerrainPlane), 0);
                    m.UniformTexture("TEX2_COLOR", TextureUnit.Texture1, Resource.Instance.GetTextureID(Resource.ITerrain2Plane), 1);

                    if (r.ReplaceRender != null && r.ReplaceRender.Parameter.Count != 0)
                    {
                        var clipPlane = (Vector4)r.ReplaceRender.Parameter[0];
                        if (clipPlane != null)
                        {
                            m.Uniform4("ClipPlane", clipPlane.X, clipPlane.Y, clipPlane.Z, clipPlane.W);
                        }
                    }
                    var modelm = WorldModelMatrix * LocalModelMatrix;
                    m.UniformMatrix4("ModelMatrix", ref modelm, true);

                    Light light;
                    if (GameDirect.Instance.MainScene.SceneLight.TryGetTarget(out light))
                    {
                        var dir = light.GetDirectVector();
                        m.Uniform3("DIR_LIGHT", dir.X, dir.Y, dir.Z);
                    }
                }
            },
           this,
           ModelList[0],
           Render.Normal);

           RenderList.Add(render);
        }

        // reference
        // https://stackoverflow.com/questions/13983189/opengl-how-to-calculate-normals-in-a-terrain-height-grid
        private Vector3 computeNormal(float hL, float hR, float hD, float hU)
        {

            // deduce terrain normal
            var N = new Vector3(hL - hR, 2.0f, hD - hU);
            N.Normalize();
            return N;
        }


        private void refreshModelData()
        {
            if (ModelList == null || ModelList[0] == null)
            {
                return;
            }

            var model = ModelList[0];
                
            Vector2 offset = new Vector2(rect.X / sliceX, rect.Y / sliceY);

            Vector2 current = Vector2.Zero;
            for (int i = 0; i < sliceY; i++)
            {
                current.X = 0;
                current.Y = i * offset.Y;
                for (int j = 0; j < sliceX; j++)
                {
                    current.X = j * offset.X;

                    model.Vertices[i * (sliceX * 4) + j * 4] = new Vector3(current.X, Algorithm.PerlinNoise2D(j, i), current.Y);
                    model.Vertices[i * (sliceX * 4) + j * 4 + 1] = new Vector3(current.X, Algorithm.PerlinNoise2D(j, i+ 1), current.Y + offset.Y);
                    model.Vertices[i * (sliceX * 4) + j * 4 + 2] = new Vector3(current.X + offset.X, Algorithm.PerlinNoise2D(j+1, i+1), current.Y + offset.Y);
                    model.Vertices[i * (sliceX * 4) + j * 4 + 3] = new Vector3(current.X + offset.X, Algorithm.PerlinNoise2D(j+1, i), current.Y);
                    
                }

            }

            model.ReloadVerticesBuffer();

            for (int i = 0; i < sliceY; i++)
            {
                for (int j = 0; j < sliceX; j++)
                {
                    var h1 = model.Vertices[i * (sliceX * 4) + j * 4].Y;
                    var h2 = model.Vertices[i * (sliceX * 4) + j * 4 + 1].Y;
                    var h3 = model.Vertices[i * (sliceX * 4) + j * 4 + 2].Y;
                    var h4 = model.Vertices[i * (sliceX * 4) + j * 4 + 3].Y;

                    model.Normals[i * (sliceX * 4) + j * 4] = computeNormal((j - 1 < 0) ? 0 : model.Vertices[i * (sliceX * 4) + (j - 1) * 4].Y,
                        h4, h2,
                        (i - 1 < 0) ? 0 : model.Vertices[(i - 1) * (sliceX * 4) + j * 4].Y);
                    model.Normals[i * (sliceX * 4) + j * 4 + 1] = computeNormal((j - 1 < 0) ? 0 : model.Vertices[i * (sliceX * 4) + (j - 1) * 4 + 1].Y,
                        h3,
                        (i + 1 >= sliceY) ? 0 : model.Vertices[(i + 1) * (sliceX * 4) + j * 4 + 1].Y,
                        h1);
                    model.Normals[i * (sliceX * 4) + j * 4 + 2] = computeNormal(h2,
                        (j + 1 >= sliceX) ? 0 : model.Vertices[i * (sliceX * 4) + (j + 1) * 4 + 2].Y,
                        (i + 1 >= sliceY) ? 0 : model.Vertices[(i + 1) * (sliceX * 4) + j * 4 + 2].Y,
                        h4);
                    model.Normals[i * (sliceX * 4) + j * 4 + 3] = computeNormal(h1,
                        (j + 1 >= sliceX) ? 0 : model.Vertices[i * (sliceX * 4) + (j + 1) * 4 + 3].Y,
                        h3,
                        (i - 1 < 0) ? 0 : model.Vertices[(i - 1) * (sliceX * 4) + j * 4 + 3].Y);
                }

            }
            model.ReloadNormalBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

        }
    }
}
