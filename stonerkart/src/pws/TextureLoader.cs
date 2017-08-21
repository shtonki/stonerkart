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

        orbChaos,
        orbNature,
        orbOrder,
        orbLife,
        orbDeath,
        orbColourless,
        orbMight,
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

            images[Textures.orbChaos] = Resources.orbChaos;
            images[Textures.orbNature] = Resources.orbNature;
            images[Textures.orbOrder] = Resources.orbOrder;
            images[Textures.orbLife] = Resources.orbLife;
            images[Textures.orbDeath] = Resources.orbDeath;
            images[Textures.orbColourless] = Resources.orbColourless;
            images[Textures.orbMight] = Resources.orbMight;
        }

        public static Textures orbTexture(ManaColour mc)
        {
            switch (mc)
            {
                case ManaColour.Chaos: return Textures.orbChaos;
                case ManaColour.Colourless: return Textures.orbColourless;
                case ManaColour.Death: return Textures.orbDeath;
                case ManaColour.Life: return Textures.orbLife;
                case ManaColour.Might: return Textures.orbMight;
                case ManaColour.Nature: return Textures.orbNature;
                case ManaColour.Order: return Textures.orbOrder;
                default:
                    throw new Exception();
            }
        }

        public static Size sizeOf(Textures t)
        {
            return images[t].Size;
        }
    }
}
