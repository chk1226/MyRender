using OpenTK;
using System.Collections.Generic;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

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


        public void UniformCubemapTexture(string variableName, TextureUnit texId, Material.TextureType bindType, int value)
        {
            var variable = GL.GetUniformLocation(ShaderProgram, variableName);
            GL.ActiveTexture(texId);
            GL.BindTexture(TextureTarget.TextureCubeMap, TextureArray[bindType]);
            GL.Uniform1(variable, value);
        }

        public void UniformTexture(string variableName, TextureUnit texId, Material.TextureType bindType, int value)
        {
            var variable = GL.GetUniformLocation(ShaderProgram, variableName);
            GL.ActiveTexture(texId);
            GL.BindTexture(TextureTarget.Texture2D, TextureArray[bindType]);
            GL.Uniform1(variable, value);
        }

        public void Uniform4(string variableName, float x, float y, float z, float w)
        {
            var variable = GL.GetUniformLocation(ShaderProgram, variableName);
            GL.Uniform4(variable, x, y, z, w);
        }

        public void UniformMatrix4(string variableName, ref Matrix4 mat, bool transpose)
        {
            var variable = GL.GetUniformLocation(ShaderProgram, variableName);
            GL.UniformMatrix4(variable, transpose, ref mat);
        }

        public void Release()
        {
            foreach (var tex in TextureArray)
            {
                GL.DeleteTexture(tex.Value);
            }
            TextureArray.Clear();

            GL.DeleteProgram(ShaderProgram);


        }


    }
}
