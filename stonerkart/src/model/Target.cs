using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace stonerkart
{
    class TargetSet
    {
        public object[] targets { get; }
        public bool Cancelled { get; private set; }
        public bool Fizzled { get; private set; }

        protected TargetSet(bool cancelled, bool fizzled)
        {
            Cancelled = cancelled;
            Fizzled = fizzled;
        }

        public TargetSet(IEnumerable<object> ts)
        {
            targets = ts.ToArray();
            Fizzled = false;
            Cancelled = false;
        }

        public static TargetSet CreateCancelled()
        {
            return new TargetSet(true, false);
        }

        public static TargetSet CreateFizzled()
        {
            return new TargetSet(false, true);
        }

        public static TargetSet CreateEmpty()
        {
            return new TargetSet(new object[0]);
        }
    }
    struct TargetVector
    {
        public TargetSet[] targetSets { get; }

        public bool Cancelled => targetSets.Any(ts => ts.Cancelled);
        public bool Fizzled => targetSets.Any(ts => ts.Fizzled);

        public TargetVector(IEnumerable<TargetSet> targetSets)
        {
            this.targetSets = targetSets.ToArray();
        }

        public static TargetVector CreateCancelled()
        {
            return new TargetVector(new []{ TargetSet.CreateCancelled() });
        }
        public static TargetVector CreateFizzled()
        {
            return new TargetVector(new[] { TargetSet.CreateFizzled() });
        }
    }
    struct TargetMatrix
    {
        public TargetVector[] targetVectors { get; }

        public bool Cancelled => targetVectors.Any(tv => tv.Cancelled);
        public bool Fizzled => targetVectors.Any(tv => tv.Fizzled);

        public TargetMatrix(TargetVector[] targetVectors)
        {
            this.targetVectors = targetVectors;
            if (Cancelled || Fizzled) throw new Exception();
        }

        private TargetMatrix(TargetVector vector)
        {
            targetVectors = new[] {vector};
        }

        public static TargetMatrix CreateCancelled()
        {
            return new TargetMatrix(TargetVector.CreateCancelled());
        }

        public static TargetMatrix CreateFizzled()
        {
            return new TargetMatrix(TargetVector.CreateFizzled());
        }
    }
    class TargetRow
    {
        private object[] targets;

        public object this[int index]
        {
            get { return targets[index]; }
        }

        public TargetRow(params object[] targets)
        {
            this.targets = targets;
        }

        public TargetRow(IEnumerable<object> ts) : this(ts.ToArray())
        {
            
        }
    }

    abstract class ChooseRule<T> : Generator<T>
    {
        protected bool atResolveTime { get; }
        protected Generator<Player> playergenerator { get; }
        protected int count { get; }
        protected Func<T, bool> filter { get; }

        /// <summary>
        /// Does what you'd expect. On null inputs it sets corresponding field to a default value decided by me.
        /// </summary>
        /// <param name="atResolveTime"></param>
        /// <param name="playergenerator"></param>
        /// <param name="count"></param>
        /// <param name="filter"></param>
        public ChooseRule(bool? atResolveTime, Generator<Player> playergenerator, int? count, Func<T, bool> filter)
        {
            this.atResolveTime = atResolveTime ?? false;
            this.playergenerator = playergenerator ?? new ResolvePlayerRule(ResolvePlayerRule.Rule.ResolveController);
            this.count = count ?? 1;
            this.filter = filter ?? (t => true);
        }

        protected override TargetSet GenerateCastTS(HackStruct hs)
        {
            var cache = playergenerator.GenerateCast(hs);
            var choosers = cache.targets.Cast<Player>();
            return atResolveTime ? cache : Generate(choosers, hs);
        }

        protected override TargetSet GenerateResolveTS(HackStruct hs, TargetSet cache)
        {
            var choosersset = playergenerator.GenerateResolve(hs, cache);
            var choosers = choosersset.targets.Cast<Player>();
            return atResolveTime ? Generate(choosers, hs) : cache;
        }

        private TargetSet Generate(IEnumerable<Player> choosers, HackStruct hs)
        {
            List<T> tlist = new List<T>();
            foreach (var chooser in choosers)
            {
                var generated = Generate(chooser, hs);
                if (generated.Fizzled || generated.Cancelled) return generated;
                tlist.AddRange(generated.targets.Cast<T>());
            }
            return new TargetSet(tlist.Cast<object>());
        }

        protected abstract TargetSet Generate(Player chooser, HackStruct hs);
    }

    abstract class ChooseHex<T> : ChooseRule<T>
    {
        public ChooseHex(bool? atResolveTime, Generator<Player> playergenerator, int? count, Func<T, bool> filter) : base(atResolveTime, playergenerator, count, filter)
        {
        }

        protected override TargetSet Generate(Player chooser, HackStruct hs)
        {
            if (count != 1) throw new Exception();
            var v = hs.game.chooseTileSynced(chooser, t => TileFilter(t) && hs.tilesInRange.Contains(t), "choose a tile boy", ButtonOption.Cancel);
            if (v == null) return TargetSet.CreateCancelled();
            return new TargetSet(new object[]{ TileTransform(v)});
        }

        protected abstract bool TileFilter(Tile t);
        protected abstract T TileTransform(Tile t);
    }
    class ChooseHexCard : ChooseHex<Card>
    {
        public ChooseHexCard() : base(null, null, null, null)
        {
            
        }

        public ChooseHexCard(
            Func<Card, bool> filter
            ) : base(null, null, null, filter)
        {
        }

        protected override bool TileFilter(Tile t)
        {
            if (t.card == null) return false;
            return filter(t.card);
        }

        protected override Card TileTransform(Tile t)
        {
            return t.card;
        }
    }
    class ChooseHexTile : ChooseHex<Tile>
    {
        public ChooseHexTile() : base(null, null, null, null)
        {
            
        }

        public ChooseHexTile(
            Func<Tile, bool> filter
            ) : base(null, null, null, filter)
        {
        }

        public ChooseHexTile(
            bool atResolveTime,
            Func<Tile, bool> filter
            ) : base(atResolveTime, null, null, filter)
        {
        }

        public ChooseHexTile(
            bool atResolveTime,
            Func<Tile, bool> filter,
            int count
            ) : base(atResolveTime, null, count, filter)
        {
        }

        protected override bool TileFilter(Tile t)
        {
            return filter(t);
        }

        protected override Tile TileTransform(Tile t)
        {
            return t;
        }

    }
    class ChooseHexPlayer : ChooseRule<Player>
    {
        public ChooseHexPlayer() : base(null, null, null, null)
        {
        }

        protected override TargetSet Generate(Player chooser, HackStruct hs)
        {
            throw new NotImplementedException();
        }
    }
    class ChooseAnyCard : ChooseRule<Card>
    {
        public ChooseAnyCard(Func<Card, bool> filter) : base(null, null, null, filter)
        {
        }

        protected override TargetSet Generate(Player chooser, HackStruct hs)
        {
            throw new NotImplementedException();
        }
    }

    class ChooseCardsFromCards : ChooseRule<Card>
    {
        public enum Mode { PlayerLooksAtPlayer, ResolverLooksAtPlayer, }
        private Mode mode { get; }
        private Generator<Card> generator { get; }

        public ChooseCardsFromCards(
            Generator<Card> generator,
            Mode mode,
            bool? atResolveTime,
            Generator<Player> playerGenerator,
            int? count, 
            Func<Card, bool> filter
            ) : base(atResolveTime, playerGenerator, count, filter)
        {
            this.generator = generator;
            this.mode = mode;
        }

        protected override TargetSet Generate(Player chooser, HackStruct hs)
        {
            throw new NotImplementedException();
        }
    }
    class ChooseCardsFromPile : ChooseCardsFromCards
    {

        public ChooseCardsFromPile(Generator<Player> playergen, PileLocation pile, bool? atResolveTime, int? count, Mode mode) 
            : base(new PlayersCardsRule(playergen, pile), mode, atResolveTime, null, count, null)
        {
        }

        public ChooseCardsFromPile(Generator<Player> playergen, PileLocation pile, bool? atResolveTime, int? count, Func<Card, bool> filter, Mode mode)
            : base(new PlayersCardsRule(playergen, pile), mode, atResolveTime, null, count, filter)
        {
        }

    }

    class AllCardsRule : Generator<Card>
    {
        private Func<Card, bool> filter;

        public AllCardsRule(Func<Card, bool> filter)
        {
            this.filter = filter;
        }

        protected override TargetSet GenerateCastTS(HackStruct hs)
        {
            throw new NotImplementedException();
        }

        protected override TargetSet GenerateResolveTS(HackStruct hs, TargetSet cache)
        {
            return GenerateCastTS(hs);
        }
    }
    class AoERule : Generator<Card>
    {
        private Generator<Tile> tilegen { get; } = new ChooseHexTile();
        private int radius { get; }

        public AoERule(int radius)
        {
            this.radius = radius;
        }

        public AoERule(Generator<Tile> tilegen, int radius) 
        {
            this.tilegen = tilegen;
            this.radius = radius;
        }

        protected override TargetSet GenerateCastTS(HackStruct hs)
        {
            throw new NotImplementedException();
        }

        protected override TargetSet GenerateResolveTS(HackStruct hs, TargetSet cache)
        {
            throw new NotImplementedException();
        }
    }

    class CreateTokens : Generator<Card>
    {
        private CardTemplate[] templates { get; }

        public CreateTokens(params CardTemplate[] templates)
        {
            this.templates = templates;
        }

        protected override TargetSet GenerateCastTS(HackStruct hs)
        {
            throw new NotImplementedException();
        }

        protected override TargetSet GenerateResolveTS(HackStruct hs, TargetSet cache)
        {
            throw new NotImplementedException();
        }
    }

    class PlayersCardsRule : Generator<Card>
    {
        Generator<Player> ownergen { get; }
        private Func<Player, IEnumerable<Card>> transform { get; }

        public PlayersCardsRule(Generator<Player> ownergen, PileLocation loc) : this(ownergen, p => p.pileFrom(loc))
        {
            
        }

        public PlayersCardsRule(Generator<Player> ownergen, Func<Player, IEnumerable<Card>> transform)
        {
            this.ownergen = ownergen;
            this.transform = transform;
        }

        protected override TargetSet GenerateCastTS(HackStruct hs)
        {
            throw new NotImplementedException();
        }

        protected override TargetSet GenerateResolveTS(HackStruct hs, TargetSet cache)
        {
            throw new NotImplementedException();
        }
    }

    class ResolvePlayerRule : Generator<Player>
    {
        public enum Rule { ResolveController, AllPlayers };

        public Rule rule { get; }

        public ResolvePlayerRule(Rule rule)
        {
            this.rule = rule;
        }

        protected override TargetSet GenerateCastTS(HackStruct hs)
        {
            switch (rule)
            {
                case Rule.ResolveController: return new TargetSet(new [] { hs.resolveController});
            }

            throw new Exception();
        }

        protected override TargetSet GenerateResolveTS(HackStruct hs, TargetSet cache)
        {
            return GenerateCastTS(hs);
        }
    }
    class ResolveCardRule : Generator<Card>
    {
        public enum Rule { ResolveCard, ResolveControllerCard, VillansCard,}

        private Rule rule { get; }
        private Func<Card, bool> filter { get; } = c => true;


        public ResolveCardRule(Rule rule)
        {
            this.rule = rule;
        }

        public ResolveCardRule(Rule rule, Func<Card, bool> filter)
        {
            this.rule = rule;
            this.filter = filter;
        }


        protected override TargetSet GenerateCastTS(HackStruct hs)
        {
            Card rtval;
            switch (rule)
            {
                case Rule.ResolveCard: rtval = hs.resolveCard; break;
                case Rule.ResolveControllerCard: rtval = hs.resolveCard.Controller.heroCard; break;
                case Rule.VillansCard: rtval = hs.gameState.villain.heroCard; break;
                default: throw new Exception();
            }
            if (!filter(rtval)) return TargetSet.CreateFizzled();
            else return new TargetSet(new [] { rtval});
        }

        protected override TargetSet GenerateResolveTS(HackStruct hs, TargetSet cache)
        {
            return GenerateCastTS(hs);
        }
    }

    class TriggeredRule<Trigger, Target> : Generator<Target> where Trigger : GameEvent
    {
        private Func<Trigger, Target> transform;

        public TriggeredRule(Func<Trigger, Target> transform)
        {
            this.transform = transform;
        }

        protected override TargetSet GenerateCastTS(HackStruct hs)
        {
            throw new NotImplementedException();
        }

        protected override TargetSet GenerateResolveTS(HackStruct hs, TargetSet cache)
        {
            throw new NotImplementedException();
        }
    }

    class ModifyRule<F, T> : Generator<T>
    {
        private Func<F, T> transform { get; }

        public ModifyRule(int effect, int column, Func<F, T> transform)
        {
            this.transform = transform;
        }

        protected override TargetSet GenerateCastTS(HackStruct hs)
        {
            throw new NotImplementedException();
        }

        protected override TargetSet GenerateResolveTS(HackStruct hs, TargetSet cache)
        {
            throw new NotImplementedException();
        }
    }

    class OptionRule<T> : Generator<T>
    {
        private string promptstring { get; }
        private Generator<T> basegenerator { get; }

        public OptionRule(string promptstring, Generator<T> basegenerator)
        {
            this.promptstring = promptstring;
            this.basegenerator = basegenerator;
        }

        protected override TargetSet GenerateCastTS(HackStruct hs)
        {
            throw new NotImplementedException();
        }

        protected override TargetSet GenerateResolveTS(HackStruct hs, TargetSet cache)
        {
            throw new NotImplementedException();
        }
    }
}
