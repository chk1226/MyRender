using OpenTK;
using System.Collections.Generic;

namespace MyRender.MyEngine
{
    class Material
    {
        public enum TextureType
        {
            Color,
            Color_02,
            Normal,
            Normal_02,
            Glow,
            Glow2,
            Specular,
            Specular2,
            Cubemap
        }
        
        public string guid;
        public Dictionary<TextureType, int> TextureArray = new Dictionary<TextureType, int>();
        public int ShaderProgram = 0;
    }
}
