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
    internal class TargetSet
    {
        public object[] targets { get; }
        public bool Cancelled { get; private set; }
        public bool Fizzled { get; private set; }
        public bool Fucked => Cancelled || Fizzled;

        protected TargetSet(bool cancelled, bool fizzled)
        {
            Cancelled = cancelled;
            Fizzled = fizzled;
        }

        public TargetSet(IEnumerable<object> ts)
        {
            if (ts.Any(t => t == null)) throw new Exception();
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

        public static TargetSet Singleton(object o)
        {
            return new TargetSet(new[] {o});
        }
    }

    internal struct TargetVector
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
            return new TargetVector(new[] {TargetSet.CreateCancelled()});
        }

        public static TargetVector CreateFizzled()
        {
            return new TargetVector(new[] {TargetSet.CreateFizzled()});
        }
    }

    internal struct TargetMatrix
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

    internal class TargetRow
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

    internal abstract class ResolveOrCastGenerator<T> : Generator<T>
    {
        protected bool generateAtResolveTime { get; }

        public ResolveOrCastGenerator(Func<T, bool> filter, bool generateAtResolveTime) : base(filter)
        {
            this.generateAtResolveTime = generateAtResolveTime;
        }

        public ResolveOrCastGenerator()
        {
        }

        protected abstract TargetSet GenerateCache(HackStruct hs);
        protected abstract TargetSet GenerateRest(HackStruct hs, TargetSet cache);

        protected override TargetSet GenerateCastx(HackStruct hs)
        {
            var cache = GenerateCache(hs);
            if (generateAtResolveTime) return cache;
            else return GenerateRest(hs, cache);
        }

        protected override TargetSet GenerateResolvex(HackStruct hs, TargetSet cache)
        {
            if (generateAtResolveTime) return GenerateRest(hs, cache);
            return cache;
        }
    }

    internal abstract class ChooseRule<T> : ResolveOrCastGenerator<T>
    {
        protected Generator<Player> playergenerator { get; }
        protected int count { get; }

        public ChooseRule(Func<T, bool> filter, Generator<Player> playergenerator, int count, bool generateAtResolveTime)
            : base(filter ?? (t => true), generateAtResolveTime)
        {
            this.playergenerator = playergenerator;
            this.count = count;
        }

        protected override TargetSet GenerateCache(HackStruct hs)
        {
            return playergenerator.GenerateCast(hs);
        }

        protected override TargetSet GenerateRest(HackStruct hs, TargetSet cache)
        {
            var v = playergenerator.GenerateResolve(hs, cache);
            if (v.Fucked) return v;
            var players = v.targets.Cast<Player>();
            if (players.Count() != 1) throw new Exception();

            return Choose(players.ElementAt(0), hs);
        }


        public override TargetSet UnfilteredCandidates(HackStruct hs)
        {
            throw new NotImplementedException();
        }
        protected abstract TargetSet Choose(Player player, HackStruct hs);
    }

    internal abstract class ChooseHex<T> : ChooseRule<T>
    {
        public ChooseHex(bool generateAtResolveTime, Generator<Player> playergenerator, int count, Func<T, bool> filter)
            : base(filter, playergenerator, count, generateAtResolveTime)
        {
        }

        protected override TargetSet Choose(Player player, HackStruct hs)
        {
            if (count != 1) throw new Exception();
            var v = hs.game.chooseTileSynced(player, t => TileFilter(t) && hs.tilesInRange.Contains(t),
                "choose a tile boy", ButtonOption.Cancel);
            if (v == null) return TargetSet.CreateCancelled();
            return TargetSet.Singleton(TileTransform(v));
        }

        protected abstract bool TileFilter(Tile t);
        protected abstract T TileTransform(Tile t);
    }

    internal class ChooseHexCard : ChooseHex<Card>
    {
        public ChooseHexCard() : base(false, Card.ResolveController, 1, f => true)
        {

        }

        public ChooseHexCard(
            Func<Card, bool> filter
            ) : base(false, Card.ResolveController, 1, filter)
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

    internal class ChooseHexTile : ChooseHex<Tile>
    {
        public ChooseHexTile() : base(false, Card.ResolveController, 1, t => true)
        {

        }

        public ChooseHexTile(
            Func<Tile, bool> filter
            ) : base(false, Card.ResolveController, 1, filter)
        {
        }

        public ChooseHexTile(
            bool generateAtResolveTime,
            Func<Tile, bool> filter
            ) : base(generateAtResolveTime, Card.ResolveController, 1, filter)
        {
        }

        public ChooseHexTile(
            bool generateAtResolveTime,
            Func<Tile, bool> filter,
            int count
            ) : base(generateAtResolveTime, null, count, filter)
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

    internal class ChooseHexPlayer : ChooseHex<Player>
    {
        public ChooseHexPlayer() : base(false, Card.ResolveController, 1, t => true)
        {
        }

        protected override bool TileFilter(Tile t)
        {
            if (t.card == null) return false;
            if (!t.card.IsHeroic) return false;
            return filter(t.card.owner);
        }

        protected override Player TileTransform(Tile t)
        {
            return t.card.owner;
        }
    }

    internal class ChooseAnyCard : ChooseRule<Card>
    {
        public ChooseAnyCard(Func<Card, bool> filter) : base(filter, Card.ResolveController, 1, false)
        {
        }

        protected override TargetSet Choose(Player player, HackStruct hs)
        {
            throw new NotImplementedException();
        }
    }

    internal class ChooseCardsFromCards : ChooseRule<Card>
    {
        public enum Mode
        {
            PlayerLooksAtPlayer,
            ResolverLooksAtPlayer,
        }

        private Mode mode { get; }
        private Func<Player, IEnumerable<Card>> transform { get; }

        public ChooseCardsFromCards(Func<Card, bool> filter, Generator<Player> playergenerator, int count, bool generateAtResolveTime, Mode mode, Func<Player, IEnumerable<Card>> transform) : base(filter, playergenerator, count, generateAtResolveTime)
        {
            this.mode = mode;
            this.transform = transform;
        }


        protected override TargetSet Choose(Player player, HackStruct hs)
        {
            var cards = transform(player);
            var chooser = player;

            var v = hs.game.chooseCardsFromCardsSynced(chooser, count, cards, filter, "fuck me xd", ButtonOption.Cancel, "xddd420");
            if (!cards.Any(filter)) return TargetSet.CreateFizzled();
            if (v == null) return TargetSet.CreateCancelled();
            return new TargetSet(v);
            throw new NotImplementedException();
            /*
             * 
        protected override TargetSet Generate(Player chooser, HackStruct hs)
        {
            var cards = generator.Generate<Card>(hs);
            var v = hs.game.chooseCardsFromCardsSynced(
                chooser, count, cards, filter, "xd", ButtonOption.Cancel, "xdd"
                );
            if (v == null) return TargetSet.CreateCancelled();
            return new TargetSet(v);
        }

        public override IEnumerable<Card> Candidates(Player chooser, HackStruct hs)
        {
            return generator.UnfilteredCandidates(chooser, hs);
            var v = generator.GenerateResolve(hs, generator.GenerateCast(hs));
            return v.targets.Cast<Card>();
        }*/
        }
    }

    class ChooseCardsFromPile : ChooseCardsFromCards
    {

        public ChooseCardsFromPile(Generator<Player> playergen, PileLocation pile, bool? atResolveTime, int? count, Mode mode) 
            : this(playergen, pile, atResolveTime, count, null, mode)
        {
        }

        public ChooseCardsFromPile(Generator<Player> playergen, PileLocation pile, bool? atResolveTime, int? count, Func<Card, bool> filter, Mode mode)
            : base(filter, playergen, count.Value, atResolveTime ?? false, mode, p => p.pileFrom(pile))
        {
        }

    }

    class AllCardsRule : Generator<Card>
    {
        public AllCardsRule(Func<Card, bool> filter) : base(filter)
        {
        }

        public override TargetSet UnfilteredCandidates(HackStruct hs)
        {
            return new TargetSet(hs.cards.Where(filter));
        }

        protected override TargetSet GenerateCastx(HackStruct hs)
        {
            return new TargetSet(hs.cards.Where(filter));
        }

        protected override TargetSet GenerateResolvex(HackStruct hs, TargetSet cache)
        {
            return new TargetSet(hs.cards.Where(filter));
        }
    }

    internal enum Timing
    {
        Cast,
        Resolve
    };


    class AoERule : Generator<Card>
    {
        private Generator<Tile> tilegen { get; } = new ChooseHexTile();
        private int radius { get; }

        public Timing timing { get; set; }

        public AoERule(int radius)
        {
            this.radius = radius;
        }

        public AoERule(Generator<Tile> tilegen, int radius) : base(null)
        {
            this.tilegen = tilegen;
            this.radius = radius;
        }

        public override TargetSet UnfilteredCandidates(HackStruct hs)
        {
            throw new NotImplementedException();
        }

        protected override TargetSet GenerateCastx(HackStruct hs)
        {
            throw new NotImplementedException();
        }

        protected override TargetSet GenerateResolvex(HackStruct hs, TargetSet cache)
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

        public override TargetSet UnfilteredCandidates(HackStruct hs)
        {
            throw new NotImplementedException();
        }

        protected override TargetSet GenerateCastx(HackStruct hs)
        {
            throw new NotImplementedException();
        }

        protected override TargetSet GenerateResolvex(HackStruct hs, TargetSet cache)
        {
            throw new NotImplementedException();
        }
    }

    class PlayersCardsRule : ResolveOrCastGenerator<Card>
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

        public override TargetSet UnfilteredCandidates(HackStruct hs)
        {
            return new TargetSet(transform(hs.resolveController));
        }

        protected override TargetSet GenerateCache(HackStruct hs)
        {
            return ownergen.GenerateCast(hs);
        }

        protected override TargetSet GenerateRest(HackStruct hs, TargetSet cache)
        {
            if (generateAtResolveTime)
            {
                var v = ownergen.GenerateResolve(hs, cache);
                var players = v.targets.Cast<Player>();
                if (players.Count() != 1) throw new Exception();

                return new TargetSet(players.SelectMany(p => transform(p)));
            }
            else
            {
                return cache;
            }
        }
    }

    class ResolvePlayerRule : Generator<Player>
    {
        public enum Rule { ResolveController, AllPlayers };

        public ResolvePlayerRule(Rule rule)
        {
            this.rule = rule;
        }

        public Rule rule { get; }

        public override TargetSet UnfilteredCandidates(HackStruct hs)
        {
            return g(hs);
        }

        protected override TargetSet GenerateCastx(HackStruct hs)
        {
            return g(hs);
        }

        protected override TargetSet GenerateResolvex(HackStruct hs, TargetSet cache)
        {
            return g(hs);
        }

        private TargetSet g(HackStruct hs)
        {
            switch (rule)
            {
                case Rule.ResolveController: return TargetSet.Singleton(hs.resolveController);
            }

            throw new Exception();
        }
    }
    class ResolveCardRule : Generator<Card>
    {
        public enum Rule { ResolveCard, ResolveControllerCard, VillansCard,}

        private Rule rule { get; }


        public ResolveCardRule(Rule rule)
        {
            this.rule = rule;
        }

        public ResolveCardRule(Rule rule, Func<Card, bool> filter) : base(filter)
        {
            this.rule = rule;
        }

        public override TargetSet UnfilteredCandidates(HackStruct hs)
        {
            throw new NotImplementedException();
        }

        protected override TargetSet GenerateCastx(HackStruct hs)
        {
            return g(hs);
        }

        protected override TargetSet GenerateResolvex(HackStruct hs, TargetSet cache)
        {
            return g(hs);
        }

        private TargetSet g(HackStruct hs)
        {
            switch (rule)
            {  
                case Rule.ResolveCard:
                    return TargetSet.Singleton(hs.resolveCard);
                case Rule.ResolveControllerCard:
                    return TargetSet.Singleton(hs.resolveCard.Controller.heroCard);
                case Rule.VillansCard:
                    return TargetSet.Singleton(hs.gameState.villain.heroCard);
                default: throw new Exception();
            }
        }

    }

    class TriggeredRule<Trigger, Target> : Generator<Target> where Trigger : GameEvent
    {
        private Func<Trigger, Target> transform;

        public TriggeredRule(Func<Trigger, Target> transform)
        {
            this.transform = transform;
        }

        public override TargetSet UnfilteredCandidates(HackStruct hs)
        {
            throw new NotImplementedException();
        }

        protected override TargetSet GenerateCastx(HackStruct hs)
        {
            throw new NotImplementedException();
        }

        protected override TargetSet GenerateResolvex(HackStruct hs, TargetSet cache)
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

        public override TargetSet UnfilteredCandidates(HackStruct hs)
        {
            throw new NotImplementedException();
        }

        protected override TargetSet GenerateCastx(HackStruct hs)
        {
            throw new NotImplementedException();
        }

        protected override TargetSet GenerateResolvex(HackStruct hs, TargetSet cache)
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

        public override TargetSet UnfilteredCandidates(HackStruct hs)
        {
            throw new NotImplementedException();
        }

        protected override TargetSet GenerateCastx(HackStruct hs)
        {
            throw new NotImplementedException();
        }

        protected override TargetSet GenerateResolvex(HackStruct hs, TargetSet cache)
        {
            throw new NotImplementedException();
        }
    }
}
