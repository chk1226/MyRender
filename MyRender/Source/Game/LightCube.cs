using OpenTK;
using OpenTK.Graphics.OpenGL;
using MyRender.MyEngine;
using System;
namespace MyRender.Game
{
    class LightCube : Node
    {
        private readonly string cubeGUID = "ColorCube";

        public Vector3 Color = Vector3.One;

        private readonly float constant = 1.0f;
        private readonly float linear = 0.7f;
        private readonly float quadratic = 1.8f;
        private readonly float imax = 5;
        private float lightMax;
        private float radius;

        public Vector4 GetLightInfo()
        {
            var regV = GameDirect.Instance.MainScene.MainCamera.ViewMatrix * WorldModelMatrix * LocalModelMatrix * Algorithm.ZeroVector;
            regV.W = radius;
            return regV;
        }

        public Vector4 GetAttenuationInfo()
        {
            return new Vector4(constant, linear, quadratic, imax);
        }

        public override void OnStart()
        {
            base.OnStart();

            ModelList = new Model[1];

            var modelData = Resource.Instance.GetModel(cubeGUID);
            if (modelData == null)
            {
                modelData = Model.CreateCubeData();
                modelData.guid = cubeGUID;
             
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

                Resource.Instance.AddModel(modelData);
            }
            ModelList[0] = modelData;

            // generate material object
            var material = Resource.Instance.GetMaterial(cubeGUID);
            if (material == null)
            {
                material = new Material();
                material.guid = cubeGUID;
                material.ShaderProgram = Resource.Instance.GetShader(Resource.SMRT_PNC);

                Resource.Instance.AddMaterial(material);
            }


            Render render = Render.CreateRender(material, delegate (Render r) {
                var m = r.MaterialData;

                if (m.ShaderProgram != 0)
                {
                    GL.UseProgram(m.ShaderProgram);

                    m.Uniform3("Color", Color.X, Color.Y, Color.Z);

                }
            },
            this,
            modelData,
            Render.Normal);
            render.PassRender = true;
            RenderList.Add(render);

            GameDirect.Instance.MainScene.SceneLightCube.Add(new System.WeakReference<LightCube>(this));

        }


        public void LightCaculation()
        {
            lightMax = Math.Max(Math.Max(Color.X, Color.Y), Color.Z);
            radius = (-linear + (float)Math.Sqrt(linear * linear - 4 * quadratic * (constant - (256.0 / imax) * lightMax)))
            / (2 * quadratic);
        }

    }
}
