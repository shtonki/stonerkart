using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stonerkart
{
    class  FontFamille
    {
        private static Dictionary<string, Imege> hacktionary;

        static FontFamille()
        {
            hacktionary = new Dictionary<string, Imege>();

            hacktionary[G.newlineGlyph] = new Imege(Textures.orbColourless, new Box(0, 0, 0, 0));

            foreach (var clr in new[] { ManaColour.Chaos, ManaColour.Death, ManaColour.Life, ManaColour.Might, ManaColour.Nature, ManaColour.Order,})
            {
                hacktionary[G.colouredGlyph(clr)] = new Imege(TextureLoader.orbTexture(clr));
            }

            for (int i = 0; i <= 12; i++)
            {
                hacktionary[G.colourlessGlyph(i)] = new Imege(TextureLoader.colourlessTexture(i));
            }
        }

        public int Height => TextureLoader.sizes[fontImage].Height;
        public int Width => TextureLoader.sizes[fontImage].Width;

        public Textures fontImage { get; }
        private Dictionary<string, Imege> characters { get; }

        public Imege this[string glyph]
        {
            get
            {
                if (glyph.Length == 1)
                {
                    return characters[glyph];
                }
                else
                {
                    return hacktionary[glyph];
                }
            }
        }

        public int widthOf(string glyph)
        {
            if (characters.ContainsKey(glyph))
            {
                return characters[glyph].Width;
            }
            else
            {
                return Height;
            }
        }

        public FontFamille(Textures fontImage, string fontstring)
        {
            this.fontImage = fontImage;

            characters = new Dictionary<string, Imege>();
            //characters[G.newlineGlyph] = new glyphxd(0, 0);

            var fontImageWidth = (double)TextureLoader.sizes[fontImage].Width;
            var fontImageHeight = (double)TextureLoader.sizes[fontImage].Width;

            var lines = fontstring.Split("\n\r".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            foreach (string line in lines)
            {
                string[] ss = line.Split();
                if (ss[0].Length != 1) throw new Exception();
                string glyph = ss[0];
                int startx = Int32.Parse(ss[1]);
                int width = Int32.Parse(ss[2]);

                Box b = new Box(startx/fontImageWidth, 0, width/fontImageWidth, 1);

                characters[glyph] = new Imege(fontImage, b);
            }

            var spacewidth = 24/(fontImageWidth);
            characters[" "] = new Imege(fontImage, new Box(1/spacewidth, 0, spacewidth, 1));
        }

        public static FontFamille font1 = new FontFamille(Textures.font0, Properties.Resources.font1);
    }
}
