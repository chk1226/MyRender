using MyRender.MyEngine;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System.Runtime.InteropServices;

namespace MyRender.Game
{
    class Skybox : Node
    {
        private readonly string skyboxGUID = "skybox";

        public override void OnStart()
        {
            base.OnStart();

            //set model
            ModelList = new Model[1];
            var modelData = Resource.Instance.GetModel(skyboxGUID);
            if (modelData == null)
            {
                modelData = Model.CreateCubeData();
                modelData.guid = skyboxGUID;
               
  
                Resource.Instance.AddModel(modelData);
            }
            ModelList[0] = modelData;

            // generate render object
            Render render = Render.CreateRender(Resource.Instance.CreateSkyboxM(), delegate (Render r) {
                var m = r.MaterialData;

                if (m.ShaderProgram != 0)
                {
                    GL.UseProgram(m.ShaderProgram);

                    m.UniformCubemapTexture("cubemapTexture", TextureUnit.Texture0, Material.TextureType.Cubemap, 0);

                }
            },
            this,
            modelData,
            Render.Skybox);
            render.EnableCubemap();
            render.EnableDepthFunc(DepthFunction.Lequal);
            render.PassRender = PassRender;
            RenderList.Add(render);

        }
        
    }

}