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
        public Foo effects;
        public Foo cost;
        public int castRange;
        public string description;

        public Ability(PileLocation activeIn, Foo effects, int castRange, Foo cost, string description)
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


        public ActivatedAbility(PileLocation activeIn, int castRange, Foo cost, CastSpeed castSpeed, string description, params Effect[] effects) : base(activeIn, new Foo(effects), castRange, cost, description)
        {
            this.castSpeed = castSpeed;
        }
    }

    class TriggeredAbility : Ability
    {
        public enum Timing { Pre, Post };

        public Timing timing;
        private GameEventFilter filter;
        private Card card;

        public TriggeredAbility(Card card, PileLocation activeIn, Effect[] effects, int castRange, Foo cost, GameEventFilter trigger, Timing timing, string description) : base(activeIn, new Foo(effects), castRange, cost, description)
        {
            this.card = card;
            filter = trigger;
            this.timing = timing;
        }

        public bool triggeredBy(GameEvent e)
        {
            return card.location.pile == activeIn && filter.filter(e);
        }
    }
    
}
