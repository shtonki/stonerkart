using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stonerkart
{
    class ManaOrb : Stuff
    {
        public ManaColour colour { get; }

        public ManaOrb(ManaColour colour)
        {
            this.colour = colour;
        }
    }

    enum ManaColour
    {
        Might, 
        Life,
        Death,
        Nature,
        Order,
        Chaos
    }
}
