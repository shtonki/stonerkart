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

        public TargetMatrix fillCast(HackStruct hs)
        {
            return ts.fillCast(hs);
        }

        public TargetMatrix fillResolve(TargetMatrix tm, HackStruct hs)
        {
            var r = ts.fillResolve(tm, hs);
            return r;
        }

        public bool possibleAsCost(HackStruct hs)
        {
            var tm = ts.possible(hs);
            var rs = tm.generateRows(straightRows);
            var ps = doer.filterCostRows(rs);
            return ps != null;
        }
    }

    abstract class Doer : TypeSigned
    {
        public Doer(params Type[] typeSignatureTypes) : base(typeSignatureTypes)
        {
        }

        public abstract GameEvent[] act(HackStruct dkt, TargetRow[] ts);

        public abstract IEnumerable<TargetRow> filterCostRows(IEnumerable<TargetRow> rs);
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

        public override IEnumerable<TargetRow> filterCostRows(IEnumerable<TargetRow> rs)
        {
            var v = rs.Where(validCostRow).ToArray();
            if (v.Length == 0) return null;
            else return v;
        }

        protected virtual bool validCostRow(TargetRow tr)
        {
            return true;
        }
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

        protected override bool validCostRow(TargetRow tr)
        {
            Card c = (Card)tr[0];
            return exhaust ? c.canExhaust : c.movement >= fatigueBy;
        }
    }

    class ModifyDoer : SimpleDoer
    {
        public ModifiableStats modifiableStats;
        public ModifierStruct modifier;

        public ModifyDoer(ModifiableStats modifiableStats, int value, Func<int, int, int> f, GameEventFilter until) : base(typeof(Card))
        {
            this.modifiableStats = modifiableStats;
            modifier = new ModifierStruct(value, f, until);
        }

        protected override GameEvent[] simpleAct(HackStruct dkt, TargetRow row)
        {
            Card card = (Card)row[0];
            return new GameEvent[] {new ModifyEvent(card, modifiableStats, modifier)};
        }
    }

    class GainBonusManaDoer : SimpleDoer
    {
        public IEnumerable<ManaColour> colour;

        public GainBonusManaDoer(params ManaColour[] colour) : base(typeof(Player))
        {
            this.colour = colour;
        }

        protected override GameEvent[] simpleAct(HackStruct dkt, TargetRow ts)
        {
            Player p = (Player)ts[0];
            return colour.Select(c => new GainBonusManaEvent(p, c)).Cast<GameEvent>().ToArray();
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

        public override IEnumerable<TargetRow> filterCostRows(IEnumerable<TargetRow> rs)
        {
            var ra = rs.ToArray();
            if (ra.Length == 0) return ra;
            Player payer = (Player)ra[0][0];

            var orbs = ra.Select(r => (ManaOrb)r[1]);

            return payer.manaPool.covers(orbs) ? rs : null;
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
