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

        public TargetVector fillCast(HackStruct hs)
        {
            return ts.fillCast(hs);
        }

        public TargetVector fillResolve(HackStruct hs, TargetVector cached)
        {
            return ts.fillResolve(hs, cached);
        }

        public IEnumerable<GameEvent> resolve(HackStruct hs, TargetVector cached)
        {
            var rows = rowsFromSets(cached.targetSets);
            return doer.act(hs, rows);
        }

        private TargetRow[] rowsFromSets(TargetSet[] ts)
        {
            if (straightRows) return straightRowsx(ts);
            if (ts.Length == 1) return rowsFrom1(ts);
            if (ts.Length == 2) return rowsFrom2(ts);
            if (ts.Length == 3) return rowsFrom3(ts);
            throw new Exception();
        }

        private TargetRow[] straightRowsx(TargetSet[] ts)
        {
            if (ts.Length == 0) throw new Exception();
            int l = ts[0].targets.Length;
            if (ts.Any(s => s.targets.Length != l)) throw new Exception();

            var rt = new TargetRow[l];

            for (int i = 0; i < l; i++)
            {
                int i1 = i;
                rt[i] = new TargetRow(ts.Select(s => s.targets[i1]));
            }

            return rt;
        }

        private TargetRow[] rowsFrom1(TargetSet[] ts)
        {
            List<TargetRow> rt = new List<TargetRow>();
            foreach (var a in ts[0].targets)
            {
                rt.Add(new TargetRow(a));
            }
            return rt.ToArray();
        }

        private TargetRow[] rowsFrom2(TargetSet[] ts)
        {
            List<TargetRow> rt = new List<TargetRow>();
            foreach (var a in ts[0].targets)
            {
                foreach (var b in ts[1].targets)
                {
                    rt.Add(new TargetRow(a, b));
                }
            }
            return rt.ToArray();
        }

        private TargetRow[] rowsFrom3(TargetSet[] ts)
        {
            List<TargetRow> rt = new List<TargetRow>();
            foreach (var a in ts[0].targets)
            {
                foreach (var b in ts[1].targets)
                {
                    foreach (var c in ts[2].targets)
                    {
                        rt.Add(new TargetRow(a, b, c));
                    }
                }
            }
            return rt.ToArray();
        }


        public static Effect summonTokensEffect(params CardTemplate[] templates)
        {
            
            return new Effect(new TargetRuleSet(
                new CreateTokenRule(new PlayerResolveRule(PlayerResolveRule.Rule.ResolveController),
                    templates),
                new ChooseRule<Tile>(
                    ChooseRule<Tile>.ChooseAt.Resolve,
                    t => t.summonable,
                    templates.Length, 
                    false)),
                new SummonToTileDoer(),
                true
                );
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

    class ApplyStacksDoer : SimpleDoer
    {
        private StacksWords stacks;
        private int count;

        public ApplyStacksDoer(StacksWords stacks, int count) : base(typeof(Card))
        {
            this.stacks = stacks;
            this.count = count;
        }


        protected override GameEvent[] simpleAct(HackStruct dkt, TargetRow row)
        {
            Card card = (Card)row[0];
            ApplyStacksEvent e = new ApplyStacksEvent(card, stacks, count);
            return new GameEvent[] {e};
        }
    }

    class ShuffleDoer : SimpleDoer
    {
        public ShuffleDoer() : base(typeof(Player))
        {
        }

        protected override GameEvent[] simpleAct(HackStruct dkt, TargetRow row)
        {
            Player p = (Player)row[0];
            return new GameEvent[] { new ShuffleDeckEvent(p), };
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

    class RaceChangeDoer : SimpleDoer
    {
        public Race race;
        public RaceChangeDoer(Race race) : base(typeof(Card))
        {
            this.race = race;
        }

        protected override GameEvent[] simpleAct(HackStruct dkt, TargetRow row)
        {
            Card card = (Card)row[0];
            return new GameEvent[] { new RaceModifyEvent(card, new RaceModifierStruct(race, new StaticGameEventFilter(() => false))) };
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
        private bool dontExhaust; //whats wrong with private bool exhaust

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

    class PingDoer : SimpleDoer
    {
        public int damage;

        /// <summary>
        /// Card source, Card victim
        /// </summary>
        /// <param name="damage"></param>
        public PingDoer(int damage) : base(typeof(Card), typeof(Card))
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
