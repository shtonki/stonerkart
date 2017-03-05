using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stonerkart
{
    abstract class Ability
    {
        public Card card;
        public PileLocation activeIn;
        public Foo effects;
        public Foo cost;
        public int castRange;
        public string description;

        public bool isCastAbility => card.castAbility == this;

        public Ability(Card card, PileLocation activeIn, Foo effects, int castRange, Foo cost, string description)
        {
            this.activeIn = activeIn;
            this.effects = effects;
            this.castRange = castRange;
            this.cost = cost;
            this.description = description;
            this.card = card;
        }

        public Card createDummy()
        {
            return card.createDummy(this);
        }
    }

    class ActivatedAbility : Ability
    {
        public bool isInstant => castSpeed == CastSpeed.Instant;

        private CastSpeed castSpeed;

        public ActivatedAbility(Card card, PileLocation activeIn, int castRange, Foo cost, CastSpeed castSpeed, string description, params Effect[] effects) : base(card, activeIn, new Foo(effects), castRange, cost, description)
        {
            this.castSpeed = castSpeed;
        }
    }

    class TriggeredAbility : Ability
    {
        public enum Timing { Pre, Post };

        public Timing timing;
        private GameEventFilter filter;

        public TriggeredAbility(Card card, PileLocation activeIn, Effect[] effects, int castRange, Foo cost, GameEventFilter trigger, Timing timing, string description) : base(card, activeIn, new Foo(effects), castRange, cost, description)
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
