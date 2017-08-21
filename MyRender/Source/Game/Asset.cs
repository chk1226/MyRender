using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyRender.MyEngine
{
    partial class Resource
    {
        // glsl
        public static readonly string SBlinPhong = @"Source\Shader\BlinPhong.glsl";
        public static readonly string SNormalmap = @"Source\Shader\NormalMap.glsl";
        public static readonly string SRobotNormalmap = @"Source\Shader\RobotNormalMap.glsl";
        public static readonly string SCowboyNormalmap = @"Source\Shader\CowboyNormalMap.glsl";
        public static readonly string SSkybox = @"Source\Shader\Skybox.glsl";
        public static readonly string SUISprite = @"Source\Shader\UISprite.glsl";
        public static readonly string SUIFont = @"Source\Shader\UIFont.glsl";
        public static readonly string SShadowBlinPhong = @"Source\Shader\ShaodwBlinPhong.glsl";
        public static readonly string SVSM = @"Source\Shader\VarianceShadowMap.glsl";
        public static readonly string SGaussianBlur = @"Source\Shader\GaussianBlur.glsl";
        public static readonly string SMRT = @"Source\Shader\MRT.glsl";
        public static readonly string SSSAO = @"Source\Shader\SSAO.glsl";
        public static readonly string STerrian = @"Source\Shader\Terrain.glsl";
        public static readonly string SWater = @"Source\Shader\Water.glsl";
        public static readonly string SBrightFilter = @"Source\Shader\BrightFilter.glsl";
        public static readonly string SCombineBright = @"Source\Shader\CombineBright.glsl";


        // image
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
        public static readonly string ICowboyColor = @"Asset\model\cowboy\diffuse.png";
        public static readonly string ICowboyNormal = @"Asset\model\cowboy\NormalMap.png";
        public static readonly string ISkybox = @"Asset\Image\skybox\lake1_{0}.png";
        public static readonly string IUISprite = @"Asset\Image\sprite.png";
        public static readonly string IUIBlack = @"Asset\Image\black.png";
        public static readonly string IUIWhite = @"Asset\Image\white.png";
        public static readonly string ITTFBitmap = @"Asset\font\Mecha.ttf.png";
        public static readonly string ITerrainPlane = @"Asset\Image\terrain\NatureForests0038_5_S.png";
        public static readonly string ITerrain2Plane = @"Asset\Image\terrain\SoilMud0006_3_S_1.png";
        public static readonly string IDudvmap = @"Asset\Image\dudvmap.png";
        public static readonly string IWaterNormalmap = @"Asset\Image\normalMap.png";

        // xml
        public static readonly string XTTFBitmap = @"Asset\font\Mecha.ttf.xml";

        // model
        public static readonly string MRobot = @"..\..\Asset\model\robot\robot_plus.dae";
        public static readonly string MCowboy = @"..\..\Asset\model\cowboy\model.dae";
        public static readonly string MHouse = @"..\..\Asset\model\house\Small_Building_1.dae";

        // guid
        public static readonly string MBricksGUID = "Bricks";
        public static readonly string MRobotGUID = "Robot";
        public static readonly string MCowboyGUID = "Cowboy";
        public static readonly string MHomeGUID = "Home";
        public static readonly string MSkyboxGUID = "Skybox";
        public static readonly string MPlaneGUID = "Plane";
        //public static readonly string MUISpriteGUID = "UISprite";

        public Material CreatePlaneM()
        {
            string guid = MPlaneGUID;

            var m = GetMaterial(guid);
            if (m == null)
            {
                m = new Material();
                m.guid = guid;
                m.TextureArray.Add(Material.TextureType.Color, GetTextureID(IUIWhite));

                m.ShaderProgram = GetShader(SShadowBlinPhong);

                AddMaterial(m);
            }

            return m;
        }


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
                //m.ShaderProgram = GetShader(SShadowBlinPhong);

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

        public Material CreateCowboyM()
        {
            var m = GetMaterial(MCowboyGUID);
            if (m == null)
            {
                m = new Material();
                m.guid = MCowboyGUID;
                m.TextureArray.Add(Material.TextureType.Color, GetTextureID(ICowboyColor));
                m.TextureArray.Add(Material.TextureType.Normal, GetTextureID(ICowboyNormal));

                m.ShaderProgram = GetShader(SCowboyNormalmap);

                AddMaterial(m);
            }

            return m;
        }

        public Material CreateHomeM()
        {
            var m = GetMaterial(MHomeGUID);
            if (m == null)
            {
                m = new Material();
                m.guid = MHomeGUID;
                m.TextureArray.Add(Material.TextureType.Color, GetTextureID(IUIWhite));

                m.ShaderProgram = GetShader(SShadowBlinPhong);

                AddMaterial(m);
            }

            return m;
        }

        public Material CreateSkyboxM()
        {
            var m = GetMaterial(MSkyboxGUID);
            if (m == null)
            {
                m = new Material();
                m.guid = MSkyboxGUID;
                m.TextureArray.Add(Material.TextureType.Cubemap,
                    GetCubemapTextureID(ISkybox));

                m.ShaderProgram = GetShader(SSkybox);

                AddMaterial(m);
            }

            return m;
        }

        public Material CreateUISpriteM(string textureFile)
        {
            var m = GetMaterial(textureFile);
            if (m == null)
            {
                m = new Material();
                m.guid = textureFile;
                m.TextureArray.Add(Material.TextureType.Color,
                    GetTextureID(textureFile));

                m.ShaderProgram = GetShader(SUISprite);

                AddMaterial(m);
            }

            return m;
        }

        public Material CreateUISpriteM(int textureID, string guid)
        {
            var m = GetMaterial(guid);
            if (m == null)
            {
                m = new Material();
                m.guid = guid;
                m.TextureArray.Add(Material.TextureType.Color,
                    textureID);

                m.ShaderProgram = GetShader(SUISprite);

                AddMaterial(m);
            }

            return m;
        }

        public Material CreateUIFontM(string textureFile)
        {
            var m = GetMaterial(textureFile);
            if (m == null)
            {
                m = new Material();
                m.guid = textureFile;
                m.TextureArray.Add(Material.TextureType.Color,
                    GetTextureID(textureFile, false));

                m.ShaderProgram = GetShader(SUIFont);

                AddMaterial(m);
            }

            return m;
        }

    }
}
