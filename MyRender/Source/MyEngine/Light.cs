using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace MyRender.MyEngine
{
    class Light : Node
    {
        public Vector4 Ambient;
        public Vector4 Specular;
        public Vector4 Diffuse;

        public override void OnStart()
        {
            base.OnStart();

            Ambient = new Vector4(0.2f, 0.2f, 0.2f, 1.0f);
            Specular = new Vector4(0.8f, 0.8f, 0.8f, 1.0f);
            Diffuse = new Vector4(0.4f, 0.4f, 0.4f, 1.0f);
            LocalPosition = new Vector3(0, 1000, 0);
            GL.Material(MaterialFace.Front, MaterialParameter.Shininess, 64);

            GL.Light(LightName.Light0, LightParameter.Position, new Vector4(LocalPosition, 1));
            GL.Light(LightName.Light0, LightParameter.Ambient, Ambient);
            GL.Light(LightName.Light0, LightParameter.Diffuse, Diffuse);
            GL.Light(LightName.Light0, LightParameter.Specular, Specular);
        }


    }
}
