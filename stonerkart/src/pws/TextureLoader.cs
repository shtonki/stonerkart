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
        B,

        memex,

        fontovich,

        bg0,
        bg1,
        bg2,
        bg3,

        buttonbg0,
        buttonbg1,
        buttonbg2,

        border0,
        border1,
        border2,

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
            images[Textures.B] = Resources.AlterTime;
            images[Textures.fontovich] = Resources.font0;
            images[Textures.memex] = Resources.memex;
            images[Textures.bg0] = Resources.background0;
            images[Textures.bg1] = Resources.background1;
            images[Textures.bg2] = Resources.background2;
            images[Textures.bg3] = Resources.background3;
            images[Textures.buttonbg0] = Resources.buttonbg0;
            images[Textures.buttonbg1] = Resources.buttonbg1;
            images[Textures.buttonbg2] = Resources.buttonbg2;
            images[Textures.border0] = Resources.border0;
            images[Textures.border1] = Resources.border1;
            images[Textures.border2] = Resources.border2;

            images[Textures.table0] = Resources.table0;

            images[Textures.cardframegrey] = Resources.whiteframe4;

            images[Textures.orbchaos] = Resources.chaos;
        }

        public static Size sizeOf(Textures t)
        {
            return images[t].Size;
        }
    }
}
