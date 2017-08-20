using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics;
using System;
using MyRender.MyEngine;
using MyRender.Debug;

namespace MyRender.Game
{
    class WaterPlane : Plane
    {
        public WeakReference<PreRender> ReflectionNode;

        private readonly float waveSpeed = 0.008f;
        private float moveFactor = 0;

        public WaterPlane(float width, float height, float waterHeight, uint slicex, uint slicey)
            :base(width, height, slicex, slicey)
        {
            LocalPosition = new Vector3(-width / 2, waterHeight, -height / 2);
        }

        public override void OnStart()
        {
            base.OnStart();

            Material material = new Material();
            material.ShaderProgram = Resource.Instance.GetShader(Resource.SWater);
            Render render = Render.CreateRender(material, delegate (Render r) {
                var m = r.MaterialData;

                if (m.ShaderProgram != 0)
                {
                    GL.UseProgram(m.ShaderProgram);

                    m.UniformTexture("REFLECTION", TextureUnit.Texture0, Resource.Instance.GetFrameBuffer(FrameBuffer.Type.ReflectionFrame).CB_Texture, 0);
                    m.UniformTexture("REFRACTION", TextureUnit.Texture1, Resource.Instance.GetFrameBuffer(FrameBuffer.Type.RefractionFrame).CB_Texture, 1);
                    m.UniformTexture("DUDVMAP", TextureUnit.Texture2, Resource.Instance.GetTextureID(Resource.IDudvmap), 2);
                    m.UniformTexture("NORMAL_TEX_COLOR", TextureUnit.Texture3, Resource.Instance.GetTextureID(Resource.IWaterNormalmap), 3);

                    m.Uniform1("MoveFactor", moveFactor);
                    var camerPos = GameDirect.Instance.MainScene.MainCamera.eye;
                    m.Uniform3("CameraPos", camerPos.X, camerPos.Y, camerPos.Z);
                    var modelMatrix = WorldModelMatrix * LocalModelMatrix;
                    m.UniformMatrix4("ModelMatrix", ref modelMatrix, true);

                    PreRender reflection;
                    if(ReflectionNode.TryGetTarget(out reflection))
                    {
                        var rmvp = reflection.ReflectionProjectMatrix * reflection.ReflectionViewMatrix * WorldModelMatrix * LocalModelMatrix;
                        m.UniformMatrix4("REFLECTION_PVM", ref rmvp, true);
                    }

                    Light light;
                    if (GameDirect.Instance.MainScene.SceneLight.TryGetTarget(out light))
                    {
                        var pos = light.LocalPosition;
                        m.Uniform3("LightPos", pos.X, pos.Y, pos.Z);
                    }
                }
            },
            this,
            ModelList[0],
            Render.Normal);
            render.PassPreRender = true;

            RenderList.Add(render);
        }

        public override void OnUpdate(FrameEventArgs e)
        {
            base.OnUpdate(e);
            moveFactor += waveSpeed * (float)e.Time;
        }

    }
}
