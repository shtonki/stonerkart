using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stonerkart
{
    abstract class Ability
    {
        public PileLocation activeIn;
        public Effect[] effects;
        public int castRange;
        public Cost cost;

        public Ability(PileLocation activeIn, Effect[] effects, int castRange, Cost cost)
        {
            this.activeIn = activeIn;
            this.effects = effects;
            this.castRange = castRange;
            this.cost = cost;
        }
    }

    class ActivatedAbility : Ability
    {
        public ActivatedAbility(PileLocation activeIn, int castRange, Cost cost, params Effect[] effects) : base(activeIn, effects, castRange, cost)
        {
        }
    }
    
}
