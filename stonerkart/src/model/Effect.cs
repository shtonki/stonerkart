using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stonerkart
{
    interface Generator
    {
        TargetSet GenerateCast(HackStruct hs);
        TargetSet GenerateResolve(HackStruct hs, TargetSet cache);
    }


    internal abstract class Generator<T> : Generator
    {
        protected Func<T, bool> filter { get; }

        

        public Generator(Func<T, bool> filter)
        {
            this.filter = filter;
        }

        public Generator() : this(t => true)
        {
        }

        public TargetSet Generate(HackStruct hs)
        {
            var cache = GenerateCast(hs);
            if (cache.Fucked) return cache;
            return GenerateResolve(hs, cache);
        }

        public TargetSet GenerateCast(HackStruct hs)
        {
            return GenerateCastx(hs);
        }
        public TargetSet GenerateResolve(HackStruct hs, TargetSet cache)
        {
            return GenerateResolvex(hs, cache);
        }


        public abstract TargetSet UnfilteredCandidates(HackStruct hs);
        protected abstract TargetSet GenerateCastx(HackStruct hs);
        protected abstract TargetSet GenerateResolvex(HackStruct hs, TargetSet cache);

        /*
        public abstract IEnumerable<T> Candidates(Player chooser, HackStruct hs);
        public IEnumerable<T> FilteredCandidates(Player chooser, HackStruct hs) => Candidates(chooser, hs).Where(filter);
        */

    }

    class StaticGenerator<T> : Generator<T>
    {
        private T[] ts;

        public StaticGenerator(Func<T, bool> filter, T[] ts) : base(filter)
        {
            this.ts = ts;
        }

        public StaticGenerator(params T[] ts) : this(t => true, ts)
        {
            this.ts = ts;
        }

        public static implicit operator StaticGenerator<T>(T t)  // explicit byte to digit conversion operator
        {
            return new StaticGenerator<T>(t);
        }

        public override TargetSet UnfilteredCandidates(HackStruct hs)
        {
            return new TargetSet(ts.Cast<object>());
        }

        protected override TargetSet GenerateCastx(HackStruct hs)
        {
            return new TargetSet(ts.Cast<object>());
        }

        protected override TargetSet GenerateResolvex(HackStruct hs, TargetSet cache)
        {
            return GenerateCast(hs);
        }
    }

    class TargetAdapter
    {
        private Generator[] generators { get; }

        public TargetAdapter(Generator[] generators)
        {
            this.generators = generators;
        }

        public TargetVector GenerateCast(HackStruct hs)
        {
            return new TargetVector(generators.Select(generator => generator.GenerateCast(hs)));
        }

        public TargetVector GenerateResolve(HackStruct hs, TargetVector cache)
        {
            return new TargetVector(generators.Select((gen, i) => gen.GenerateResolve(hs, cache.targetSets[i])));
        }

        public IEnumerable<TargetRow> GenerateRows(TargetVector vector)
        {
            if (vector.targetSets.Length == 0) throw new Exception();
            if (vector.targetSets.Length == 1) return rowsfrom1(vector);
            if (vector.targetSets.Length == 2) return rowsfrom2(vector);
            throw new NotImplementedException("if you weren't expecting too see this you might be in some trouble son");
        }

        private IEnumerable<TargetRow> rowsfrom1(TargetVector vec)
        {
            foreach (var v1 in vec.targetSets[0].targets)
            {
                yield return new TargetRow(v1);
            }
        }

        private IEnumerable<TargetRow> rowsfrom2(TargetVector vec)
        {
            foreach (var v1 in vec.targetSets[0].targets)
            {
                foreach (var v2 in vec.targetSets[1].targets)
                {
                    yield return new TargetRow(v1, v2);
                }
            }
        }
        /*
        protected IEnumerable<Tuple<T1, T2>> rowsfrom2<T1, T2>(HackStruct hs)
        {
            List<Tuple<T1, T2>> rtval = new List<Tuple<T1, T2>>();


            var vs1 = generators[0].Generate(hs).Cast<T1>();
            var vs2 = generators[1].Generate(hs).Cast<T2>();

            foreach (var v1 in vs1)
            {
                foreach (var v2 in vs2)
                {
                    rtval.Add(new Tuple<T1, T2>(v1, v2));
                }
            }

            return rtval;
        }
        */
    }

    abstract class Effect
    {
        private TargetAdapter ta { get; }

        public Effect(params Generator[] generators)
        {
            ta = new TargetAdapter(generators);
        }

        public abstract IEnumerable<GameEvent> Do(HackStruct hs, TargetRow row);

        public TargetVector fillCast(HackStruct hs)
        {
            return ta.GenerateCast(hs);
        }

        public TargetVector fillResolve(HackStruct hs, TargetVector cache)
        {
            return ta.GenerateResolve(hs, cache);
        }

        public IEnumerable<GameEvent> resolve(HackStruct hs, TargetVector cache)
        {
            var rows = ta.GenerateRows(cache);
            return rows.SelectMany(row => Do(hs, row));
        }
    }


    /*
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
    */

    class DisplayEffect : Effect
    {
        public DisplayEffect(Generator<Card> cardgen, Generator<Player> viewergen) : base(cardgen, viewergen)
        {

        }

        public override IEnumerable<GameEvent> Do(HackStruct hs, TargetRow row)
        {
            throw new NotImplementedException();
        }
    }

    class ApplyStacksEffect : Effect
    {
        public ApplyStacksEffect(Generator<Card> cardgen, Generator<Counter> stackgen) : base(cardgen, stackgen)
        {

        }

        public override IEnumerable<GameEvent> Do(HackStruct hs, TargetRow row)
        {
            Card card = (Card)row[0];
            Counter counter = (Counter)row[1];
            return new GameEvent[]
            {
                new ApplyStacksEvent(card, counter, 1),
            };
        }
    }

    class ShuffleEffect : Effect
    {
        public ShuffleEffect(Generator<Player> playergen) : base(playergen)
        {
        }

        public override IEnumerable<GameEvent> Do(HackStruct hs, TargetRow row)
        {
            Player shuffler = (Player)row[0];
            return new GameEvent[]
            {
                new ShuffleDeckEvent(shuffler), 
            };
        }
    }

    class FatigueEffect : Effect
    {
        private Func<Card, int> fatigueCalculator;

        public FatigueEffect(Generator<Card> cardgen, Func<Card, int> fatigueCalculator) : base(cardgen)
        {
            this.fatigueCalculator = fatigueCalculator;
        }

        public override IEnumerable<GameEvent> Do(HackStruct hs, TargetRow row)
        {
            Card c = (Card)row[0];
            int v = fatigueCalculator(c);
            return new GameEvent[] {new FatigueEvent(c, v)};
        }
        
    }


    class ModifyEffect : Effect
    {
        public ModifierStruct modifier;

        public ModifyEffect(Func<int, int> f, GameEventFilter until, Generator<Card> cardgen, Generator<ModifiableStats> statsgen) : base(cardgen, statsgen)
        {
            modifier = new ModifierStruct(f, until);
        }

        public override IEnumerable<GameEvent> Do(HackStruct hs, TargetRow row)
        {
            Card card = (Card)row[0];
            ModifiableStats stats = (ModifiableStats)row[1];

            return new GameEvent[]
            {
                new ModifyEvent(card, stats, modifier), 
            };
        }
    }

    class GainBonusManaEffect : Effect
    {
        public GainBonusManaEffect(Generator<Player> playergen, Generator<ManaColour> orbgen) : base(playergen, orbgen)
        {
        }

        public override IEnumerable<GameEvent> Do(HackStruct hs, TargetRow row)
        {
            throw new NotImplementedException("if you weren't expecting too see this you might be in some trouble son");
            //it's a manacolour not a manaorb now and it's going to break
            Player player = (Player)row[0];
            ManaOrb orb = (ManaOrb)row[1];
            return new GameEvent[] { new GainBonusManaEvent(player, orb) };
        }
    }

    class RaceChangeEffect : Effect
    {
        public Race race;

        public RaceChangeEffect(Race race, Generator<Card> cardgen) : base(cardgen)
        {
            this.race = race;
        }

        public override IEnumerable<GameEvent> Do(HackStruct hs, TargetRow row)
        {
            Card card = (Card)row[0];
            return new GameEvent[] { new RaceModifyEvent(card, new RaceModifierStruct(race, new StaticGameEventFilter(() => false))) };
        }
    }

    class PayManaEffect : Effect
    {
        public PayManaEffect(Generator<Player> playergen, Generator<ManaSet> costgen) : base(playergen, costgen)
        {

        }

        public override IEnumerable<GameEvent> Do(HackStruct hs, TargetRow row)
        {
            Player player = (Player)row[0];
            ManaSet cost = (ManaSet)row[1];
            return new GameEvent[]
            {
                new PayManaEvent(player, cost), 
            };
        }
    }

    class DrawCardsEffect : Effect
    {
        public int cards;

        public DrawCardsEffect(int cards, Generator<Player> playergen) : base(playergen)
        {
            this.cards = cards;
        }

        public override IEnumerable<GameEvent> Do(HackStruct hs, TargetRow row)
        {
            Player player = (Player)row[0];
            return new GameEvent[] {new DrawEvent(player, cards)};
        }
    }
    
    class MoveToTileEffect : Effect
    {

        public MoveToTileEffect(Generator<Card> cardgen, Generator<Tile> tilegen) : base(cardgen, tilegen)
        {
        }

        public override IEnumerable<GameEvent> Do(HackStruct hs, TargetRow row)
        {
            Card moved = (Card)row[0];
            Tile moveTo = (Tile)row[1];
            return new GameEvent[] {new PlaceOnTileEvent(moved, moveTo, true)};
        }
    }

    class SummonToTileEffect : Effect
    {
        //todo 280917 merge this class with MoveToTileDoer
        public SummonToTileEffect(Generator<Card> cardgen, Generator<Tile> tilegen) : base(cardgen, tilegen)
        {
        }

        public override IEnumerable<GameEvent> Do(HackStruct hs, TargetRow row)
        {
            Card moved = (Card)row[0];
            Tile moveTo = (Tile)row[1];
            return new GameEvent[] { new MoveToPileEvent(moved, moved.Controller.field), new PlaceOnTileEvent(moved, moveTo, false)  };
        }
    }

    class PingEffect : Effect
    {
        public int damage;

        /// <summary>
        /// Card source, Card victim
        /// </summary>
        /// <param name="damage"></param>
        public PingEffect(int damage, Generator<Card> sourcegen, Generator<Card> targetgen) : base(sourcegen, targetgen)
        {
            this.damage = damage;
        }

        public override IEnumerable<GameEvent> Do(HackStruct hs, TargetRow row)
        {
            Card damager = (Card)row[0];
            Card damaged = (Card)row[1];
            return new GameEvent[] {new DamageEvent(damager, damaged, damage)};
        }
    }

    class MoveToPileEffect : Effect
    {
        public PileLocation pileLocation;

        public MoveToPileEffect(PileLocation pileLocation, Generator<Card> cardgen) : base(cardgen)
        {
            this.pileLocation = pileLocation;
        }

        public override IEnumerable<GameEvent> Do(HackStruct hs, TargetRow row)
        {
            Card c = (Card)row[0];
            return new GameEvent[] { new MoveToPileEvent(c, c.owner.pileFrom(pileLocation)) };
        }
    }
}
