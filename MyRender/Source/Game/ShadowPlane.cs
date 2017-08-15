using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics;
using System;
using MyRender.MyEngine;

namespace MyRender.Game
{
    class ShadowPlane : Plane 
    {
        private FrameBuffer useFrame;

        public ShadowPlane(float width, float height) : base(width, height, 1, 1)
        {
            LocalPosition = new Vector3(-width / 2, 0, -height / 2);
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
                            var bmvp = light.LightBiasProjectView() * WorldModelMatrix * LocalModelMatrix;
                            m.UniformMatrix4("LIGHT_BPVM", ref bmvp, true);
                        }

                        var frame = Resource.Instance.GetFrameBuffer(FrameBuffer.Type.GaussianRYFrame);
                        if (frame != null)
                        {
                            m.UniformTexture("SSAO", TextureUnit.Texture2, frame.CB_Texture, 2);
                        }
                    }

                }
            },
            this,
            modelData,
            Render.Normal);

            RenderList.Add(render);
        }

        public void SetFrameBuffer(FrameBuffer use)
        {
            useFrame = use;
        }
    }
}
