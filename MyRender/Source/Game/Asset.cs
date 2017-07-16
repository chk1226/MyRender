using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyRender.MyEngine
{
    partial class Resource
    {
        public static readonly string SBlinPhong = @"Source\Shader\BlinPhong.glsl";
        public static readonly string SNormalmap = @"Source\Shader\NormalMap.glsl";
        public static readonly string SRobotNormalmap = @"Source\Shader\robot_NormalMap.glsl";


        public static readonly string IBricks = @"Asset\Image\DiagonalHerringbone-ColorMap.bmp";
        public static readonly string IBricksNormal = @"Asset\Image\DiagonalHerringbone-NormalMap.bmp";

        public static readonly string IRobotColor = @"Asset\model\robot\id01_color.png";
        public static readonly string IRobotColor2 = @"Asset\model\robot\id02_color.png";
        public static readonly string IRobotNormal = @"Asset\model\robot\id01_normal.png";
        public static readonly string IRobotNormal2 = @"Asset\model\robot\id02_normal.png";
        public static readonly string IRobotGlow = @"Asset\model\robot\id01_glow.png";
        public static readonly string IRobotGlow2 = @"Asset\model\robot\id02_glow.png";
        public static readonly string IRobotSpecular = @"Asset\model\robot\id01_specular.png";
        public static readonly string IRobotSpecular2 = @"Asset\model\robot\id02_specular.png";
        public static readonly string MRobot = @"..\..\Asset\model\robot\robot_plus.dae";

        public static readonly string MBricksGUID = "Bricks";
        public static readonly string MRobotGUID = "Robot";

        public Material CreateBricksM()
        {
            string guid = MBricksGUID;

            var m = GetMaterial(guid);
            if(m == null)
            {
                m = new Material();
                m.guid = MBricksGUID;
                m.TextureArray.Add(Material.TextureType.Color, GetTextureID(IBricks));
                m.TextureArray.Add(Material.TextureType.Normal, GetTextureID(IBricksNormal));
                //m.ShaderProgram = GetShader(SBlinPhong);
                m.ShaderProgram = GetShader(SNormalmap);

                AddMaterial(m);
            }

            return m;
        }

        public Material CreateRobotM()
        {
            var m = GetMaterial(MRobotGUID);
            if (m == null)
            {
                m = new Material();
                m.guid = MRobotGUID;
                m.TextureArray.Add(Material.TextureType.Color, GetTextureID(IRobotColor));
                m.TextureArray.Add(Material.TextureType.Color_02, GetTextureID(IRobotColor2));
                m.TextureArray.Add(Material.TextureType.Normal, GetTextureID(IRobotNormal));
                m.TextureArray.Add(Material.TextureType.Normal_02, GetTextureID(IRobotNormal2));
                m.TextureArray.Add(Material.TextureType.Glow, GetTextureID(IRobotGlow));
                m.TextureArray.Add(Material.TextureType.Glow2, GetTextureID(IRobotGlow2));
                m.TextureArray.Add(Material.TextureType.Specular, GetTextureID(IRobotSpecular));
                m.TextureArray.Add(Material.TextureType.Specular2, GetTextureID(IRobotSpecular2));

                m.ShaderProgram = GetShader(SRobotNormalmap);

                AddMaterial(m);
            }

            return m;
        }

    }
}
