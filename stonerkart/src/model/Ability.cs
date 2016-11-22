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

        public Ability(PileLocation usableIn, params Effect[] es)
        {
            this.usableIn = usableIn;
            this.effects = es;
        }
    }
    
}
