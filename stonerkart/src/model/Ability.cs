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
        public CastSpeed castSpeed;
        public bool isInstant => castSpeed == CastSpeed.Instant;

        public Ability(PileLocation activeIn, Effect[] effects, int castRange, Cost cost, CastSpeed castSpeed)
        {
            this.activeIn = activeIn;
            this.effects = effects;
            this.castRange = castRange;
            this.cost = cost;
            this.castSpeed = castSpeed;
        }
    }

    class ActivatedAbility : Ability
    {
        public ActivatedAbility(PileLocation activeIn, int castRange, Cost cost, CastSpeed castSpeed, params Effect[] effects) : base(activeIn, effects, castRange, cost, castSpeed)
        {
        }
    }
    
}
