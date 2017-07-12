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

        public static readonly string IBricks = @"Asset\Image\DiagonalHerringbone-ColorMap.bmp";
        public static readonly string IBricksNormal = @"Asset\Image\DiagonalHerringbone-NormalMap.bmp";
        public static readonly string IRider = @"Asset\model\Hunter.png";

        public static readonly string MRider = @"..\..\Asset\model\Rider.DAE";


        public static readonly string MBricksGUID = "Bricks";
        public static readonly string MRiderGUID = "Rider";

        public Material CreateBricksM()
        {
            string guid = MBricksGUID;

            var m = GetMaterial(guid);
            if(m == null)
            {
                m = new Material();
                m.guid = MBricksGUID;
                m.TextureFileName = IBricks;
                m.NormalTextureFileName = IBricksNormal;
                m.TextureID = GetTextureID(m.TextureFileName);
                m.NormalTextureID = GetTextureID(m.NormalTextureFileName);
                //m.ShaderProgram = GetShader(SBlinPhong);
                m.ShaderProgram = GetShader(SNormalmap);

                AddMaterial(m);
            }

            return m;
        }

        public Material CreateDaeM()
        {
            var m = GetMaterial(MRiderGUID);
            if (m == null)
            {
                m = new Material();
                m.guid = MRiderGUID;
                m.TextureFileName = IRider;
                m.TextureID = GetTextureID(m.TextureFileName);
                m.ShaderProgram = GetShader(SBlinPhong);

                AddMaterial(m);
            }

            return m;
        }

    }
}
