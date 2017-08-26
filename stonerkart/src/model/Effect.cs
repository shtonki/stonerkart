using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stonerkart
{
    class Effect
    {
        public TargetRuleSet ts;
        public Doer doer;

        public bool straightRows { get; }

        public Effect(TargetRuleSet ts, Doer d, bool straightRows = false)
        {
            if (!ts.matchesTypeSignatureOf(d)) throw new Exception();
            this.ts = ts;
            doer = d;
            this.straightRows = straightRows;
        }

        public Effect(TargetRule r, Doer d) : this(new TargetRuleSet(r), d)
        {
        }

        public TargetSet[] fillCast(HackStruct hs)
        {
            return ts.fillCast(hs);
        }

        public IEnumerable<GameEvent> resolve(HackStruct hs, TargetSet[] cached)
        {
            var filled = ts.fillResolve(hs, cached);
            var rows = rowsFromSet(filled);
            return doer.act(hs, rows);
        }

        private TargetRow[] rowsFromSet(TargetSet[] ts)
        {
            throw new NotImplementedException("if you weren't expecting too see this you might be in some trouble son");
        }


        public static Effect summonTokensEffect(params CardTemplate[] templates)
        {

            throw new NotImplementedException("if you weren't expecting too see this you might be in some trouble son");/*
            return new Effect(new TargetRuleSet(
                new CreateTokenRule(new PlayerResolveRule(PlayerResolveRule.Rule.ResolveController),
                    templates),
                new PryTileRule(t => t.card == null && !t.isEdgy,
                    new PlayerResolveRule(PlayerResolveRule.Rule.ResolveController), true, templates.Length, false)),
                new SummonToTileDoer(),
                true
                );*/
        }

    }

    abstract class Doer : TypeSigned
    {
        public Doer(params Type[] typeSignatureTypes) : base(typeSignatureTypes)
        {
        }

        public abstract GameEvent[] act(HackStruct dkt, TargetRow[] ts);
        
    }

    abstract class SimpleDoer : Doer
    {
        public SimpleDoer(params Type[] typeSignatureTypes) : base(typeSignatureTypes)
        {
        }

        public override GameEvent[] act(HackStruct dkt, TargetRow[] ts)
        {
            List<GameEvent> r = new List<GameEvent>();
            foreach (TargetRow row in ts)
            {
                r.AddRange(simpleAct(dkt, row));
            }
            return r.ToArray();
        }

        protected abstract GameEvent[] simpleAct(HackStruct dkt, TargetRow row);
        
    }

    class FatigueDoer : SimpleDoer
    {
        private int fatigueBy;
        private bool exhaust;
        private bool invigorate;

        /// <summary>
        /// Exhausts if passed paramater is true, else invigorates.
        /// </summary>
        /// <param name="fatigue"></param>
        public FatigueDoer(bool exhaust) : base(typeof(Card))
        {
            if (exhaust)
            {
                this.exhaust = true;
            }
            else
            {
                invigorate = true;
            }
        }

        public FatigueDoer(int fatigueBy) : base(typeof(Card))
        {
            this.fatigueBy = fatigueBy;
        }

        protected override GameEvent[] simpleAct(HackStruct dkt, TargetRow row)
        {
            Card c = (Card)row[0];
            int v;
            if (exhaust)
            {
                v = c.movement;
            }
            else if (invigorate)
            {
                v = -c.fatigue;
            }
            else
            {
                v = fatigueBy;
            }
            return new GameEvent[] {new FatigueEvent(c, v)};
        }
        
    }

    class ModifyDoer : SimpleDoer
    {
        public ModifiableStats[] modifiableStats;
        public ModifierStruct modifier;

        public ModifyDoer(Func<int, int> f, GameEventFilter until, params ModifiableStats[] modifiableStats) : base(typeof(Card))
        {
            this.modifiableStats = modifiableStats;
            modifier = new ModifierStruct(f, until);
        }

        protected override GameEvent[] simpleAct(HackStruct dkt, TargetRow row)
        {
            Card card = (Card)row[0];
            return modifiableStats.Select(s => new ModifyEvent(card, s, modifier)).Cast<GameEvent>().ToArray();
        }
    }

    class ForceStaticModifyDoer : SimpleDoer
    {
        public ModifiableStats modifiableStats;
        private Func<Func<int, int>> f;
        private GameEventFilter until;

        public ForceStaticModifyDoer(ModifiableStats modifiableStats, Func<Func<int, int>> f, GameEventFilter until) : base(typeof(Card))
        {
            this.modifiableStats = modifiableStats;
            this.f = f;
            this.until = until;
        }

        protected override GameEvent[] simpleAct(HackStruct dkt, TargetRow row)
        {
            Card card = (Card)row[0];
            var fn = f();
            return new GameEvent[] { new ModifyEvent(card, modifiableStats, new ModifierStruct(fn, until)) };
        }
    }

    class GainBonusManaDoer : SimpleDoer
    {
        public GainBonusManaDoer() : base(typeof(Player), typeof(ManaOrb))
        {
        }

        protected override GameEvent[] simpleAct(HackStruct dkt, TargetRow ts)
        {
            Player p = (Player)ts[0];
            ManaOrb o = (ManaOrb)ts[1];
            return new GameEvent[] { new GainBonusManaEvent(p, o) };
        }
    }

    class PayManaDoer : Doer
    {
        public PayManaDoer() : base(typeof(Player), typeof(ManaOrb))
        {

        }

        public override GameEvent[] act(HackStruct dkt, TargetRow[] rows)
        {
            if (rows.Length == 0) return new GameEvent[0];
            Player p = (Player)rows[0][0];
            var v = rows.Select(r => ((ManaOrb)r[1]).colour);
            return new GameEvent[] { new PayManaEvent(p, new ManaSet(v)) };
        }
    }

    class DrawCardsDoer : SimpleDoer
    {
        public int cards;

        public DrawCardsDoer(int cards) : base(typeof(Player))
        {
            this.cards = cards;
        }

        protected override GameEvent[] simpleAct(HackStruct dkt, TargetRow row)
        {
            Player player = (Player)row[0];
            return new GameEvent[] {new DrawEvent(player, cards)};
        }
    }
    
    class MoveToTileDoer : SimpleDoer
    {
        private bool dontExhaust;

        public MoveToTileDoer(bool dontExhaust = false) : base(typeof(Card), typeof(Tile))
        {
            this.dontExhaust = dontExhaust;
        }

        protected override GameEvent[] simpleAct(HackStruct dkt, TargetRow row)
        {
            Card moved = (Card)row[0];
            Tile moveTo = (Tile)row[1];
            return new GameEvent[] {new PlaceOnTileEvent(moved, moveTo, dontExhaust)};
        }
    }

    class SummonToTileDoer : SimpleDoer
    {

        public SummonToTileDoer() : base(typeof(Card), typeof(Tile))
        {
        }

        protected override GameEvent[] simpleAct(HackStruct dkt, TargetRow row)
        {
            Card moved = (Card)row[0];
            Tile moveTo = (Tile)row[1];
            return new GameEvent[] { new MoveToPileEvent(moved, moved.controller.field), new PlaceOnTileEvent(moved, moveTo, false)  };
        }
    }

    class ZepperDoer : SimpleDoer
    {
        public int damage;

        /// <summary>
        /// Card source, Card victim
        /// </summary>
        /// <param name="damage"></param>
        public ZepperDoer(int damage) : base(typeof(Card), typeof(Card))
        {
            this.damage = damage;
        }

        protected override GameEvent[] simpleAct(HackStruct dkt, TargetRow row)
        {
            Card damager = (Card)row[0];
            Card damaged = (Card)row[1];
            return new GameEvent[] {new DamageEvent(damager, damaged, damage)};
        }
    }

    class MoveToPileDoer : SimpleDoer
    {
        public PileLocation pileLocation;

        public MoveToPileDoer(PileLocation pileLocation) : base(typeof(Card))
        {
            this.pileLocation = pileLocation;
        }

        protected override GameEvent[] simpleAct(HackStruct dkt, TargetRow row)
        {
            Card c = (Card)row[0];

            var e = new MoveToPileEvent(c, c.owner.pileFrom(pileLocation));
            return c.pile.location.pile == PileLocation.Deck
                ? new GameEvent[] {new ShuffleDeckEvent(c.owner), e}
                : new GameEvent[] {e};

        }
    }
    
}
