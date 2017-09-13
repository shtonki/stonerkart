using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stonerkart
{
    class Imege
    {
        public Textures texture { get; }
        public Box crop { get; }
        public Color brushColor { get; set; }

        public int Width => (int)Math.Round(TextureLoader.sizes[texture].Width*crop.w);
        public int Height => (int)Math.Round(TextureLoader.sizes[texture].Height*crop.h);

        public int Alpha
        {
            get { return brushColor.A; }
            set { brushColor = Color.FromArgb(value, brushColor.R, brushColor.G, brushColor.B); }
        }

        public Imege(Textures texture) : this(texture, new Box(0.0, 0.0, 1.0, 1.0))
        {
        }

        public Imege(Textures texture, Color brushColor) : this(texture)
        {
            this.brushColor = brushColor;
        }

        public Imege(Textures texture, Box crop) : this(texture, crop, Color.White)
        {
        }

        public Imege(Textures texture, Box crop, Color brushColor)
        {
            this.texture = texture;
            this.crop = crop;
            this.brushColor = brushColor;
        }
    }
}
