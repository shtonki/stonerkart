using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stonerkart
{
    class MemeImege : Imege
    {
        private static Random rand = new Random(420);

        public MemeImege(Textures texture, int seed = -1) : base(texture, boxify(seed))
        {
        }

        private static Box boxify(int seed)
        {
            Random r = rand;
            if (seed != -1)
            {
                r = new Random(seed);
            }

            var x = r.NextDouble() * 0.7;
            var y = r.NextDouble() * 0.7;
            return new Box(x, y, 0.3, 0.3);
        }

    }
}
