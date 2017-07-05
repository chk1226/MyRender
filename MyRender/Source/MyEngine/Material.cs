using OpenTK;

namespace MyRender.MyEngine
{
    class Material
    {
        public string guid;
        public string TextureFileName;
        public string NormalTextureFileName;
        public int TextureID = 0;
        public int NormalTextureID = 0;
        public int ShaderProgram = 0;
    }
}
