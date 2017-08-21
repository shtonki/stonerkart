using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;

using OpenTK.Graphics.OpenGL;
using PixelFormat = OpenTK.Graphics.OpenGL.PixelFormat;
using stonerkart.Properties;

namespace stonerkart
{
    public enum Textures
    {
        A,

        fontovich,

        bg3,

        buttonbg2,

        border0,

        table0,

        cardframegrey,

        orbchaos,
    };

    public static class TextureLoader
    {
        public static Dictionary<Textures, Image> images { get; }


        static TextureLoader()
        {
            images = new Dictionary<Textures, Image>();

            images[Textures.A] = Resources.artMoratianBattleStandard;
            images[Textures.fontovich] = Resources.font0;
            images[Textures.bg3] = Resources.background3;
            images[Textures.buttonbg2] = Resources.buttonbg2;
            images[Textures.border0] = Resources.border0;

            images[Textures.table0] = Resources.table0;

            images[Textures.cardframegrey] = Resources.whiteframe4;

            images[Textures.orbchaos] = Resources.orbChaos;
        }

        public static Size sizeOf(Textures t)
        {
            return images[t].Size;
        }
    }
}
