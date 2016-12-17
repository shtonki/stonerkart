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
        public bool isInstant => castSpeed == CastSpeed.Instant;
        public CastSpeed castSpeed;


        public ActivatedAbility(PileLocation activeIn, int castRange, Cost cost, CastSpeed castSpeed, params Effect[] effects) : base(activeIn, effects, castRange, cost)
        {
            this.castSpeed = castSpeed;
        }
    }

    class TriggeredAbility : Ability
    {
        public TriggeredAbility(PileLocation activeIn, Effect[] effects, int castRange, Cost cost) : base(activeIn, effects, castRange, cost)
        {
        }
    }
    
}
