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
        public string description;

        public Ability(PileLocation activeIn, Effect[] effects, int castRange, Cost cost, string description)
        {
            this.activeIn = activeIn;
            this.effects = effects;
            this.castRange = castRange;
            this.cost = cost;
            this.description = description;
        }
    }

    class ActivatedAbility : Ability
    {
        public bool isInstant => castSpeed == CastSpeed.Instant;

        private CastSpeed castSpeed;


        public ActivatedAbility(PileLocation activeIn, int castRange, Cost cost, CastSpeed castSpeed, string description, params Effect[] effects) : base(activeIn, effects, castRange, cost, description)
        {
            this.castSpeed = castSpeed;
        }
    }

    class TriggeredAbility : Ability
    {
        public enum Timing { Pre, Post };

        private Card card;
        private GameEventFilter filter;
        public Timing timing;

        public TriggeredAbility(Card card, PileLocation activeIn, Effect[] effects, int castRange, Cost cost, GameEventFilter trigger, Timing timing, string description) : base(activeIn, effects, castRange, cost, description)
        {
            this.card = card;
            filter = trigger;
            this.timing = timing;
        }

        public bool triggeredBy(GameEvent e)
        {
            if (card.location.pile != activeIn) return false;
            return filter.filter(e);
        }
    }
    
}
