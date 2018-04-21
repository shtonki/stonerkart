using System.Collections.Generic;

namespace stonerkart
{
    abstract class Ability
    {
        private Foo Cost;
        private Foo Effects;
        public Card Card { get; protected set; }
        public PileLocation ActiveIn { get; }
        public int CastRange { get; }
        public string Description { get; }
        public bool IsCastAbility => Card.IsCastAbility(this);

        public Ability(Card card, PileLocation activeIn, Foo effects, int castRange, Foo cost, string description)
        {
            ActiveIn = activeIn;
            Effects = effects;
            CastRange = castRange;
            Cost = cost;
            Description = description;
            Card = card;
        }

        public TargetMatrix AquireCostTargets(HackStruct hs)
        {
            var castTargets = Cost.fillCast(hs);
            if (castTargets.Cancelled || castTargets.Fizzled) return castTargets;
            var resolveTargets = Cost.fillResolve(hs, castTargets);
            return resolveTargets;
        }

        public virtual IEnumerable<GameEvent> PayCosts(HackStruct hs, TargetMatrix cache)
        {
            return Cost.resolve(hs, cache);
        }

        public TargetMatrix AquireCastTargets(HackStruct hs)
        {
            return Effects.fillCast(hs);
        }

        public TargetMatrix AquireResolveTargets(HackStruct hs, TargetMatrix cached)
        {
            TargetMatrix tm;
            while (true)
            {
                tm = Effects.fillResolve(hs, cached);

                if (tm.Cancelled)
                {
                    continue;
                }

                return tm;
            }
        }

        public virtual IEnumerable<GameEvent> Resolve(HackStruct hs, TargetMatrix cache)
        {
            return Effects.resolve(hs, cache);
        }

        public Card CreateDummy()
        {
            return Card.createDummy(this);
        }
    }


    class ActivatedAbility : Ability
    {
        private CastSpeed castSpeed;
        public bool IsInstant => castSpeed == CastSpeed.Interrupt;

        public ActivatedAbility(Card card, PileLocation activeIn, int castRange, Foo effect, Foo cost, CastSpeed castSpeed, string description) : base(card, activeIn, effect, castRange, cost, description)
        {
            this.castSpeed = castSpeed;
        }
    }

    class TriggeredAbility : Ability
    {
        public enum Timing { Pre, Post };

        private GameEventFilter filter;
        public Timing timing { get; }
        public bool isOptional { get; }

        public TriggeredAbility(Card card, PileLocation activeIn, Foo effects, int castRange, Foo cost, GameEventFilter trigger, bool isOptional, Timing timing, string description) : base(card, activeIn, effects, castRange, cost, description)
        {
            Card = card;
            filter = trigger;
            this.timing = timing;
            this.isOptional = isOptional;
        }

        public override IEnumerable<GameEvent> Resolve(HackStruct hs, TargetMatrix cached)
        {
            if (isOptional)
            {
                ButtonOption opt = hs.game.chooseButtonSynced(hs.resolveController,
                    "Do you want to use this ability?",
                    ButtonOption.Yes, ButtonOption.No);

                if (opt == ButtonOption.No) return new GameEvent[] {};
            }
            return base.Resolve(hs, cached);
        }

        public bool triggeredBy(GameEvent e)
        {
            return Card.location.pile == ActiveIn && filter.filter(e);
        }
    }
    
}
