using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stonerkart
{
    abstract class Ability
    {
        private Foo effects;
        private Foo cost;
        public Card card { get; protected set; }
        public PileLocation activeIn { get; }
        public int castRange { get; }
        public string description { get; }

        public bool isCastAbility => card.isCastAbility(this);

        public Ability(Card card, PileLocation activeIn, Foo effects, int castRange, Foo cost, string description)
        {
            this.activeIn = activeIn;
            this.effects = effects;
            this.castRange = castRange;
            this.cost = cost;
            this.description = description;
            this.card = card;
        }
        /*
        public IEnumerable<GameEvent> payCosts(HackStruct hs)
        {
            var costCache = cost.fillCast(hs);
            if (costCache == null) return null;
            return cost.resolve(hs, costCache);
        }
        */

        public TargetMatrix targetCosts(HackStruct hs)
        {
            var castTargets = cost.fillCast(hs);
            if (castTargets.Cancelled || castTargets.Fizzled) return castTargets;
            var resolveTargets = cost.fillResolve(hs, castTargets);
            return resolveTargets;
        }

        public virtual IEnumerable<GameEvent> payCosts(HackStruct hs, TargetMatrix cache)
        {
            return cost.resolve(hs, cache);
        }

        public TargetMatrix targetCast(HackStruct hs)
        {
            return effects.fillCast(hs);
        }

        public TargetMatrix targetResolve(HackStruct hs, TargetMatrix cached)
        {
            TargetMatrix tm;
            while (true)
            {
                tm = effects.fillResolve(hs, cached);

                if (tm.Cancelled)
                {
                    continue;
                }

                return tm;
            }
        }

        public virtual IEnumerable<GameEvent> resolve(HackStruct hs, TargetMatrix cache)
        {
            return effects.resolve(hs, cache);
        }

        
        public Card createDummy()
        {
            return card.createDummy(this);
        }
    }


    class ActivatedAbility : Ability
    {
        public bool isInstant => castSpeed == CastSpeed.Interrupt;

        private CastSpeed castSpeed;

        public ActivatedAbility(Card card, PileLocation activeIn, int castRange, Foo cost, CastSpeed castSpeed, string description, params Effect[] effects) : base(card, activeIn, new Foo(effects), castRange, cost, description)
        {
            this.castSpeed = castSpeed;
        }
    }

    class TriggeredAbility : Ability
    {
        public enum Timing { Pre, Post };

        public Timing timing { get; }
        public bool isOptional { get; }

        private GameEventFilter filter;

        public TriggeredAbility(Card card, PileLocation activeIn, Effect[] effects, int castRange, Foo cost, GameEventFilter trigger, bool isOptional, Timing timing, string description) : base(card, activeIn, new Foo(effects), castRange, cost, description)
        {
            this.card = card;
            filter = trigger;
            this.timing = timing;
            this.isOptional = isOptional;
        }

        public override IEnumerable<GameEvent> resolve(HackStruct hs, TargetMatrix cached)
        {
            if (isOptional)
            {
                ButtonOption opt = hs.game.chooseButtonSynced(hs.resolveController,
                    "Do you want to use this ability?",
                    "Opponent is deciding whether to use optional ability.",
                    ButtonOption.Yes, ButtonOption.No);

                if (opt == ButtonOption.No) return new GameEvent[] {};
            }
            return base.resolve(hs, cached);
        }

        public bool triggeredBy(GameEvent e)
        {
            return card.location.pile == activeIn && filter.filter(e);
        }
    }
    
}
