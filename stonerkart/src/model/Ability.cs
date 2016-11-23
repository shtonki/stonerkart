using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stonerkart
{
    class Ability
    {
        public PileLocation usableIn;
        public Effect[] effects;
        public int castRange;

        public Ability(PileLocation usableIn, int castRange, params Effect[] es)
        {
            this.usableIn = usableIn;
            this.effects = es;
            this.castRange = castRange;
        }
    }
    
}
