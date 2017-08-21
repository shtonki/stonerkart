using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stonerkart
{
    class Imege
    {
        public Imege(Textures texture) : this(texture, new Box(0.0, 0.0, 1.0, 1.0))
        {
        }

        public Imege(Textures texture, Box crop)
        {
            this.texture = texture;
            this.crop = crop;
        }

        public Textures texture { get; }
        public Box crop { get; }
    }
}
