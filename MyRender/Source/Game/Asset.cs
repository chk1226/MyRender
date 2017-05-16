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
        public static readonly string IBricks = @"Asset\Image\bricks_red.jpg";
        public static readonly string MBricksGUID = "Bricks";

        public Material CreateBricksM()
        {
            string guid = MBricksGUID;

            var m = GetMaterial(guid);
            if(m == null)
            {
                m = new Material();
                m.guid = MBricksGUID;
                m.TextureFileName = IBricks;
                m.TextureID = GetTextureID(m.TextureFileName);
                AddMaterial(m);
            }

            return m;
        }
    }
}
