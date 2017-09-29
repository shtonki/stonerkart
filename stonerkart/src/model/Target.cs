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
    struct TargetSet
    {
        public object[] targets { get; }
        public bool Cancelled { get; private set; }
        public bool Fizzled { get; private set; }


        public TargetSet(params object[] targets)
        {
            this.targets = targets;
            Cancelled = false;
            Fizzled = false;
        }

        public TargetSet(IEnumerable<object> ts) : this(ts.ToArray())
        {

        }

        public static TargetSet CreateCancelled()
        {
            var rt = new TargetSet(null);
            rt.Cancelled = true;
            return rt;
        }

        public static TargetSet CreateFizzled()
        {
            var rt = new TargetSet(null);
            rt.Fizzled = true;
            return rt;
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
            throw new NotImplementedException();
        }

        protected override TargetSet GenerateResolveTS(HackStruct hs, TargetSet cache)
        {
            throw new NotImplementedException();
        }
    }

    class ChooseHexCard : ChooseRule<Card>
    {
        public ChooseHexCard(
            Func<Card, bool> filter
            ) : base(null, null, null, filter)
        {
        }
    }

    class ChooseHexTile : ChooseRule<Tile>
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
    }

    class ChooseHexPlayer : ChooseRule<Player>
    {
        public ChooseHexPlayer() : base(null, null, null, null)
        {
        }
    }

    class ChooseAnyCard : ChooseRule<Card>
    {
        public ChooseAnyCard(Func<Card, bool> filter) : base(null, null, null, filter)
        {
        }
    }

    public enum ChooseMode { PlayerLooksAtPlayer, ResolverLooksAtPlayer, }

    class ChooseCardsFromCards : ChooseRule<Card>
    {
        private ChooseMode mode { get; }

        public ChooseCardsFromCards(
            bool? atResolveTime,
            int? count, 
            ChooseMode mode
            ) : base(atResolveTime, null, count, null)
        {
            this.mode = mode;
        }
    }

    class ChooseCardsFromPile : ChooseRule<Card>
    {
        private ChooseMode mode { get; }
        private PileLocation location { get; }

        public ChooseCardsFromPile(
            ChooseMode mode,
            PileLocation location,
            Func<Card, bool> filter
            ) : base(null, null, null, filter)
        {
            this.mode = mode;
            this.location = location;
        }

        public ChooseCardsFromPile(
            ChooseMode mode, 
            PileLocation location, 
            bool? atResolveTime, 
            Generator<Player> playergenerator, 
            Func<Card, bool> filter
            ) : base(atResolveTime, playergenerator, null, filter)
        {
            this.mode = mode;
            this.location = location;
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

    class ResolvePlayerRule : Generator<Player>
    {
        public enum Rule { ResolveController };

        public Rule rule { get; }

        public ResolvePlayerRule(Rule rule)
        {
            this.rule = rule;
        }

        protected override TargetSet GenerateCastTS(HackStruct hs)
        {
            switch (rule)
            {
                case Rule.ResolveController: return new TargetSet(hs.resolveController);
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
            throw new NotImplementedException("if you weren't expecting too see this you might be in some trouble son");
        }

        protected override TargetSet GenerateResolveTS(HackStruct hs, TargetSet cache)
        {
            return GenerateCastTS(hs);
        }
    }

    class TriggeredRule<Trigger, Target> : Generator<Target>
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
}
