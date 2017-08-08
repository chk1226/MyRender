using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace MyRender.MyEngine
{
    class Light : Node
    {
        /// <summary>
        /// Ambient default value is Vector4(0.4f, 0.4f, 0.4f, 1.0f)
        /// </summary>
        public Vector4 Ambient = new Vector4(0.4f, 0.4f, 0.4f, 1.0f);
        /// <summary>
        /// Specular default value is Vector4(0.9f, 0.9f, 0.9f, 1.0f)
        /// </summary>
        public Vector4 Specular = new Vector4(0.9f, 0.9f, 0.9f, 1.0f);
        /// <summary>
        /// Diffuse default value is Vector4(0.3f, 0.3f, 0.3f, 1.0f)
        /// </summary>
        public Vector4 Diffuse = new Vector4(0.3f, 0.3f, 0.3f, 1.0f);

        public override void OnStart()
        {
            base.OnStart();

            LocalPosition = new Vector3(0, 1000, 0);

            // Shininess value is problem, sometime shader can't get value
            //GL.Material(MaterialFace.Front, MaterialParameter.Shininess, 64);

            SetupLight();
        }

        public void SetupLight()
        {
            GL.Light(LightName.Light0, LightParameter.Position, new Vector4(LocalPosition, 1));
            GL.Light(LightName.Light0, LightParameter.Ambient, Ambient);
            GL.Light(LightName.Light0, LightParameter.Diffuse, Diffuse);
            GL.Light(LightName.Light0, LightParameter.Specular, Specular);
        }

    }
}
