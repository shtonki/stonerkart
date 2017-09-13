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
    abstract class TypeSigned
    {
        protected Type[] typeSignatureTypes;

        public TypeSigned(IEnumerable<Type> typeSignatureTypes) : this(typeSignatureTypes.ToArray())
        {

        }

        public TypeSigned(params Type[] typeSignatureTypes)
        {
            this.typeSignatureTypes = typeSignatureTypes;
        }

        public bool matchesTypeSignatureOf(TypeSigned other)
        {
            return typeSignatureTypes.SequenceEqual(other.typeSignatureTypes);
        }
    }

    class TargetRuleSet : TypeSigned
    {
        private TargetRule[] rules;

        public TargetRuleSet(params TargetRule[] rules) : base(rules.Select(t => t.targetType))
        {
            this.rules = rules;
        }

        public TargetVector fillCast(HackStruct hs)
        {
            var cache = new TargetSet[rules.Length];
            for (int i = 0; i < rules.Length; i++)
            {
                var targetSet = rules[i].fillCastTargets(hs);
                if (targetSet.Cancelled)
                {
                    return TargetVector.CreateCancelled();
                }
                if (targetSet.Fizzled)
                {
                    return TargetVector.CreateFizzled();
                }
                cache[i] = targetSet;
            }
            return new TargetVector(cache);
        }

        public TargetVector fillResolve(HackStruct hs, TargetVector cached)
        {
            if (rules.Length != cached.targetSets.Length) throw new Exception();

            TargetSet[] newSets = new TargetSet[rules.Length];

            for (int i = 0; i < rules.Length; i++)
            {
                var rule = rules[i];
                var cachedTargetSet = cached.targetSets[i];
                var newTargetSet = rule.fillResolveTargets(hs, cachedTargetSet);
                var validated = rule.validate(hs, newTargetSet);

                if (validated.Cancelled)
                {
                    return TargetVector.CreateCancelled();
                }
                if (validated.Fizzled)
                {
                    return TargetVector.CreateFizzled();
                }
                newSets[i] = validated;
            }

            return new TargetVector(newSets);
        }

    }

    struct TargetSet
    {
        private int[] stateCtrs { get; }
        public Targetable[] targets { get; }
        public bool Cancelled { get; private set; }
        public bool Fizzled { get; private set; }


        public TargetSet(params Targetable[] targets)
        {
            this.targets = targets;
            if (targets != null) stateCtrs = targets.Select(t => t.stateCtr()).ToArray();
            else stateCtrs = null;
            Cancelled = false;
            Fizzled = false;
        }

        public TargetSet(IEnumerable<Targetable> ts) : this(ts.ToArray())
        {

        }

        public TargetSet clearInvalids(Func<Targetable, bool> filter)
        {
            if (Cancelled || Fizzled) return this;
            var nicelanguage = stateCtrs;
            var ts = targets.Where((t, i) => filter(t) && t.stateCtr() == nicelanguage[i]);
            return new TargetSet(ts);
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
            return new TargetSet(new Targetable[0]);
        }
    }

    struct TargetVector
    {
        public TargetSet[] targetSets { get; }

        public bool Cancelled => targetSets.Any(ts => ts.Cancelled);
        public bool Fizzled => targetSets.Any(ts => ts.Fizzled);

        public TargetVector(TargetSet[] targetSets)
        {
            this.targetSets = targetSets;
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
        private Targetable[] targets;

        public Targetable this[int index]
        {
            get { return targets[index]; }
        }

        public TargetRow(params Targetable[] targets)
        {
            this.targets = targets;
        }

        public TargetRow(IEnumerable<Targetable> ts) : this(ts.ToArray())
        {
            
        }
    }

    abstract class TargetRule
    {
        public Type targetType { get; protected set; }

        public TargetRule(Type targetType)
        {
            this.targetType = targetType;
        }
        
        public abstract TargetSet fillCastTargets(HackStruct hs);
        public abstract TargetSet fillResolveTargets(HackStruct hs, TargetSet ts);

        public virtual TargetSet validate(HackStruct hs, TargetSet ts)
        {
            var newset = ts.clearInvalids(filterfunc);
            if (newset.Cancelled || newset.Fizzled) return newset;
            if (newset.targets.Length < mincount()) return TargetSet.CreateFizzled();
            return newset;
        }

        protected virtual bool filterfunc(Targetable t) { return true; }
        protected virtual int mincount() { return 0; }
    }

    interface chooserface<T> where T : Targetable
    {
        IEnumerable<T> candidates(HackStruct hs, Player p);
        T pickOne(Player chooser, Func<T, bool> filter, HackStruct hs);
        void pickNone(Player chooser, HackStruct hs);
    }

    class TargetOption : TargetRule
    {
        private TargetRule baserule;

        public TargetOption(TargetRule baserule) : base(baserule.targetType)
        {
            this.baserule = baserule;
        }

        public override TargetSet fillCastTargets(HackStruct hs)
        {
            return baserule.fillCastTargets(hs);
        }

        public override TargetSet fillResolveTargets(HackStruct hs, TargetSet ts)
        {
            var v = baserule.fillResolveTargets(hs, ts);
            var option = hs.game.chooseButtonSynced(hs.resolveController, "placeholder", ButtonOption.Yes, ButtonOption.No);

            return option == ButtonOption.Yes ? v : TargetSet.CreateEmpty();
        }
    }

    class ChooseRule<T> : TargetRule where T : Targetable
    {
        private chooserface<T> chooser = dflt();
        private TargetRule playerRule = new PlayerResolveRule(PlayerResolveRule.Rule.ResolveController);
        private ChooseAt chooseAt = ChooseAt.Cast;
        protected Func<T, bool> filter = t => true;
        private int count = 1;
        private bool allowDuplicates = false;

        private static chooserface<T> dflt()
        {
            Type t = typeof (T);
            if (t == typeof (Card)) return (chooserface<T>)new PryCardRule();
            if (t == typeof (Tile)) return (chooserface<T>)new PryTileRule();
            if (t == typeof (Player)) return (chooserface<T>)new PryPlayerRule();
            if (t == typeof (ManaOrb)) return (chooserface<T>)new ClickManaRule();
            throw new Exception();
        }

        public ChooseRule(chooserface<T> chooser, TargetRule playerRule, ChooseAt chooseAt, Func<T, bool> filter, int count, bool allowDuplicates) : base(typeof(T))
        {
            this.playerRule = playerRule;
            this.count = count;
            this.allowDuplicates = allowDuplicates;
            this.chooseAt = chooseAt;
            this.filter = filter;
            this.chooser = chooser;
        }

        public ChooseRule(Func<T, bool> filter) : base(typeof(T))
        {
            this.filter = filter;
        }

        public ChooseRule(chooserface<T> chooser, TargetRule playerRule, ChooseAt chooseAt, Func<T, bool> filter) : base(typeof(T))
        {
            this.playerRule = playerRule;
            this.chooseAt = chooseAt;
            this.filter = filter;
            this.chooser = chooser;
        }

        public ChooseRule(ChooseAt chooseAt, Func<T, bool> filter, int count, bool allowDuplicates) : base(typeof(T))
        {
            this.chooseAt = chooseAt;
            this.filter = filter;
            this.count = count;
            this.allowDuplicates = allowDuplicates;
        }

        public ChooseRule(ChooseAt chooseAt) : base(typeof(T))
        {
            this.chooseAt = chooseAt;
        }

        public ChooseRule(chooserface<T> chooser, ChooseAt chooseAt, Func<T, bool> filter) : base(typeof(T))
        {
            this.chooser = chooser;
            this.chooseAt = chooseAt;
            this.filter = filter;
        }

        public ChooseRule(chooserface<T> chooser) : base(typeof(T))
        {
            this.chooser = chooser;
        }

        public ChooseRule(ChooseAt chooseAt, Func<T, bool> filter) : base(typeof(T))
        {
            this.filter = filter;
            this.chooseAt = chooseAt;
        }

        public ChooseRule() : base(typeof(T))
        {
        }

        public ChooseRule(chooserface<T> chooser, ChooseAt chooseAt) : base(typeof(T))
        {
            this.chooser = chooser;
            this.chooseAt = chooseAt;
        }


        public override TargetSet fillCastTargets(HackStruct hs)
        {
            var choosers = playerRule.fillCastTargets(hs);
            if (chooseAt == ChooseAt.Cast)
            {
                choosers = playerRule.fillResolveTargets(hs, choosers);
                return choose(choosers.targets.Cast<Player>(), hs);
            }
            else return choosers;
        }

        public override TargetSet fillResolveTargets(HackStruct hs, TargetSet ts)
        {
            if (chooseAt == ChooseAt.Cast) return ts;
            var choosers = playerRule.fillResolveTargets(hs, ts);
            return choose(choosers.targets.Cast<Player>(), hs);
        }

        private TargetSet choose(IEnumerable<Player> players, HackStruct hs)
        {
            List<Targetable> targets = new List<Targetable>();
            foreach (var player in players)
            {
                var ts = playerChooses(player, hs);
                if (ts.Cancelled || ts.Fizzled) return ts;
                targets.AddRange(ts.targets);
            }

            return new TargetSet(targets);
        }

        private TargetSet playerChooses(Player player, HackStruct hs)
        {
            int requiredCandidates = count;

            var v = chooser.candidates(hs, player).Where(filter);

            if (requiredCandidates == 0)
            {
                chooser.pickNone(player, hs);
                return TargetSet.CreateEmpty();
            }

            if (v.Count() < requiredCandidates)
            {
                chooser.pickNone(player, hs);
                return TargetSet.CreateFizzled();
            }

            TargetSet? ts = null;
            List<Targetable> chosenList = new List<Targetable>();
            if (player.isHero) hs.game.highlight(v.Cast<Targetable>(), Color.Green);

            while (chosenList.Count < count)
            {
                var chosen = chooser.pickOne(player, filter, hs);

                if (chosen == null)
                {
                    if (v.Count() == 0)
                    {
                        ts = TargetSet.CreateFizzled();
                        break;
                    }
                    else
                    {
                        ts = TargetSet.CreateCancelled();
                        break;
                    }
                }
                else
                {
                    if (!allowDuplicates && chosenList.Contains(chosen))
                    {

                    }
                    else
                    {
                        chosenList.Add(chosen);
                    }
                }
            }

            if (!ts.HasValue)
            {
                ts = new TargetSet(chosenList);
            }

            hs.game.clearHighlights();

            return ts.Value;
        }

        protected override bool filterfunc(Targetable t)
        {
            return t is T && filter((T)t);
        }

        protected override int mincount()
        {
            return count;
        }

        public enum ChooseAt
        {
            Cast,
            Resolve
        }
    }

    class SelectCardRule : chooserface<Card>
    {
        private Mode mode;
        private PileLocation pile;
        private int count = -1;

        public SelectCardRule(PileLocation pile, Mode mode)
        {
            this.mode = mode;
            this.pile = pile;
        }

        public SelectCardRule(PileLocation pile, Mode mode, int count)
        {
            this.mode = mode;
            this.pile = pile;
            this.count = count;
        }

        public IEnumerable<Card> candidates(HackStruct hs, Player p)
        {
            Player lookedAt;
            switch (mode)
            {
                case Mode.PlayerLooksAtPlayer:
                case Mode.ResolverLooksAtPlayer:
                {
                    lookedAt = p;
                } break;

                case Mode.PlayerLooksAtResolver:
                {
                    lookedAt = hs.resolveController;
                } break;

                default: throw new Exception();
            }

            return count == -1 ? 
                lookedAt.pileFrom(pile) : 
                lookedAt.pileFrom(pile).Take(count);
        }

        private Player chooser(Player player, HackStruct hs)
        {
            switch (mode)
            {
                case Mode.PlayerLooksAtPlayer:
                case Mode.PlayerLooksAtResolver:
                {
                    return player;
                }
                case Mode.ResolverLooksAtPlayer:
                {
                    return hs.resolveController;
                }
                default:
                    throw new Exception();
            }
        }

        public Card pickOne(Player player, Func<Card, bool> filter, HackStruct hs)
        {
            var chsr = chooser(player, hs);
            var cs = candidates(hs, player);
            return hs.game.chooseCardsFromCardsSynced(
                chsr, 
                cs, 
                filter,
                String.Format("Choose target for {0}.", hs.resolveCard),
                ButtonOption.Cancel,
                String.Format("{0}'s {1}", player.name, pile.ToString()));
        }

        public void pickNone(Player player, HackStruct hs)
        {
            var chsr = chooser(player, hs);
            var cs = candidates(hs, player);
            var v = hs.game.chooseCardsFromCardsSynced(
                chsr, 
                cs, 
                _ => false,
                String.Format("Not enough valid targets for {0}.", hs.resolveCard),
                ButtonOption.OK,
                String.Format("{0}'s {1}", player.name, pile.ToString()));
        }

        public enum Mode
        {
            ResolverLooksAtPlayer,
            PlayerLooksAtPlayer,
            PlayerLooksAtResolver,
        }
    }

    class ClickCardRule : chooserface<Card>
    {
        public IEnumerable<Card> candidates(HackStruct hs, Player p)
        {
            return hs.cards;
        }

        public Card pickOne(Player chooser, Func<Card, bool> filter, HackStruct hs)
        {
            return hs.game.chooseCardSynced(
                chooser, 
                filter, 
                "Placeholder xd", 
                ButtonOption.Cancel);
        }

        public void pickNone(Player chooser, HackStruct hs)
        {
            hs.game.chooseButtonSynced(
                chooser,
                String.Format("Not enough valid targets for {0}.", hs.resolveCard),
                ButtonOption.OK);
        }
    }

    class PryCardRule : chooserface<Card>
    {
        public IEnumerable<Card> candidates(HackStruct hs, Player p)
        {
            return hs.tilesInRange.Where(t => t.card != null).Select(t => t.card);
        }

        public Card pickOne(Player chooser, Func<Card, bool> filter, HackStruct hs)
        {
            Tile tl = hs.game.chooseTileSynced(
                chooser,
                t => hs.tilesInRange.Contains(t) && t.card != null && filter(t.card),
                String.Format("Choose target for {0}", hs.resolveCard),
                ButtonOption.Cancel);
            if (tl == null) return null;
            return (tl.card);
        }

        public void pickNone(Player chooser, HackStruct hs)
        {
            hs.game.chooseButtonSynced(
                chooser,
                String.Format("Not enough valid targets for {0}.", hs.resolveCard),
                ButtonOption.OK);
        }
    }

    class PryTileRule : chooserface<Tile>
    {
        public IEnumerable<Tile> candidates(HackStruct hs, Player p)
        {
            return hs.tilesInRange;
        }

        public Tile pickOne(Player chooser, Func<Tile, bool> filter, HackStruct hs)
        {
            return hs.game.chooseTileSynced(
                chooser, 
                t => hs.tilesInRange.Contains(t) && filter(t),
                String.Format("Choose target for {0}", hs.resolveCard),
                ButtonOption.Cancel);
        }

        public void pickNone(Player chooser, HackStruct hs)
        {
            hs.game.chooseButtonSynced(
                chooser,
                String.Format("Not enough valid targets for {0}.", hs.resolveCard),
                ButtonOption.OK);
        }
    }

    class PryPlayerRule : chooserface<Player>
    {
        public IEnumerable<Player> candidates(HackStruct hs, Player p)
        {
            return hs.gameState.players;
        }

        public Player pickOne(Player chooser, Func<Player, bool> filter, HackStruct hs)
        {
            Tile tl = hs.game.chooseTileSynced(
                chooser, 
                t => t.card != null && t.card.isHeroic && filter(t.card.owner),
                String.Format("Choose target for {0}", hs.resolveCard),
                ButtonOption.Cancel);
            if (tl == null) return null;
            return tl.card.owner;
        }
        public void pickNone(Player chooser, HackStruct hs)
        {
            hs.game.chooseButtonSynced(
                chooser,
                String.Format("Not enough valid targets for {0}.", hs.resolveCard),
                ButtonOption.OK);
        }
    }

    class ClickManaRule : chooserface<ManaOrb>
    {
        public IEnumerable<ManaOrb> candidates(HackStruct hs, Player p)
        {
            return G.orbOrder.Select(c => new ManaOrb(c));
        }

        public ManaOrb pickOne(Player chooser, Func<ManaOrb, bool> filter, HackStruct hs)
        {
            var v = hs.game.chooseManaColourSynced(
                chooser, 
                c => filter(new ManaOrb(c)), 
                "Placeholder", 
                ButtonOption.Cancel);
            if (v == null) return null;
            return new ManaOrb(v.Value);
        }

        public void pickNone(Player chooser, HackStruct hs)
        {
            hs.game.chooseButtonSynced(
                chooser,
                String.Format("Not enough valid targets for {0}.", hs.resolveCard),
                ButtonOption.OK);
        }
    }

    class ManaCostRule : TargetRule
    {
        private ManaSet cost;

        public ManaCostRule(ManaSet cost) : base(typeof(ManaOrb))
        {
            this.cost = cost;
        }

        public override TargetSet fillCastTargets(HackStruct hs)
        {
            return meme(hs.castingPlayer, hs);
        }

        public override TargetSet fillResolveTargets(HackStruct hs, TargetSet ts)
        {
            return ts;
        }

        private TargetSet meme(Player p, HackStruct hs)
        {
            var playerMana = p.manaPool;
            var costOrbs = cost.orbs.ToList();
            var poolOrbs = playerMana.orbs.ToList();

            foreach (var orb in costOrbs.Where(c => c.colour != ManaColour.Colourless))
            {
                if (!poolOrbs.Any(o => o.colour == orb.colour)) return TargetSet.CreateFizzled();
                poolOrbs.Remove(poolOrbs.First(o => o.colour == orb.colour));
            }

            var colourlessCost = costOrbs.Where(c => c.colour == ManaColour.Colourless);
            var colouredCost = costOrbs.Where(c => c.colour != ManaColour.Colourless);
            int colourlessToPay = colourlessCost.Count();

            if (colourlessToPay == 0) return new TargetSet(costOrbs);
            if (colourlessToPay == poolOrbs.Count()) return new TargetSet(p.manaPool.orbs);

            var stateOfAffairs = playerMana.clone();
            playerMana.pay(new ManaSet(colouredCost));
            var paid = new List<ManaOrb>();
            int diff;

            while ((diff = colourlessToPay - paid.Count) > 0)
            {
                var chosen = hs.game.chooseManaColourSynced(
                    p, 
                    c => playerMana.orbs.Any(o => o.colour == c),
                    String.Format("Pay {0}", G.colourlessGlyph(diff)), 
                    ButtonOption.Cancel);
                if (chosen == null) break;
                paid.Add(new ManaOrb(chosen.Value));
                playerMana.pay(new ManaSet(chosen.Value));
            }

            p.manaPool.setTo(stateOfAffairs);
            if (paid.Count == colourlessToPay) return new TargetSet(colouredCost.Concat(paid));
            else return TargetSet.CreateCancelled();


            throw new NotImplementedException("if you weren't expecting too see this you might be in some trouble son");/*
            var v = playerMana.orbs.Where(c => c != ManaColour.Colourless).

            IEnumerable<ManaColour> poolOrbs = p.manaPool.orbs;
            IEnumerable<ManaColour> costOrbs = cost.orbs.Select(o => o.colour);

            int diff = costOrbs.Count() - poolOrbs.Count();

            if (diff > 0) return TargetSet.CreateFizzled();

            if (cost[ManaColour.Colourless] > 0)
            {
                if (diff < 0) // we have more total mana than the cost
                {
                    ManaSet colours = cost.clone();
                    colours[ManaColour.Colourless] = 0;
                    p.stuntLoss(colours);
                    while (cost[ManaColour.Colourless] > 0)
                    {
                        var clr = hs.game.chooseManaColourSynced(p, c => true, "1", "2").Value;
                        if (bc.targets.Length == 1)
                        {
                            var orb = (ManaOrb)bc.targets[0];
                            var colour = orb.colour;
                            if (p.manaPool.currentMana(colour) - cost[colour] > 0)
                            {
                                cost[colour]++;
                                cost[ManaColour.Colourless]--;
                                colours[colour]++;
                                p.stuntLoss(colours);
                            }
                        }
                        else
                        {
                                p.unstuntMana();
                                return null;
                        }
                    }
                    p.unstuntMana();
                }
                else
                {
                    cost = new ManaSet(hs.castingPlayer.manaPool.orbs);
                }
            }
            return new TargetSet(cost.orbs);*/
        }
    }

    class StaticManaRule : TargetRule
    {
        private ManaColour[] colours;

        public StaticManaRule(params ManaColour[] colours) : base(typeof(ManaOrb))
        {
            this.colours = colours;
        }

        public override TargetSet fillCastTargets(HackStruct f)
        {
            return TargetSet.CreateEmpty();
        }

        public override TargetSet fillResolveTargets(HackStruct hs, TargetSet c)
        {
            return new TargetSet(colours.Select(clr => new ManaOrb(clr)));
        }
    }

    class TriggeredTargetRule<G, T> : TargetRule where T : Targetable where G : GameEvent
    {
        private Func<G, T> func;

        public TriggeredTargetRule(Func<G, T> func) : base(typeof(T))
        {
            this.func = func;
        }

        public override TargetSet fillCastTargets(HackStruct f)
        {
            G g = (G)f.triggeringEvent;
            var t = (Targetable)func(g);
            return new TargetSet(new []{t});
        }

        public override TargetSet fillResolveTargets(HackStruct hs, TargetSet ts)
        {
            return ts;
        }
        
    }
    
    class CreateTokenRule : TargetRule
    {
        private TargetRule summonFor;
        private CardTemplate[] tokens;

        public CreateTokenRule(TargetRule summonFor, params CardTemplate[] tokens) : base(typeof(Card))
        {
            this.summonFor = summonFor;
            this.tokens = tokens;
        }

        public override TargetSet fillCastTargets(HackStruct f)
        {
            return summonFor.fillCastTargets(f);
        }

        public override TargetSet fillResolveTargets(HackStruct hs, TargetSet c)
        {
            TargetSet players = summonFor.fillResolveTargets(hs, c);
            Player[] ps = players.targets.Cast<Player>().ToArray();
            List<Card> ts = new List<Card>(ps.Length);

            foreach (Player p in ps)
            {
                foreach (var t in tokens)
                {
                    ts.Add(hs.game.createToken(t, p));
                }
            }
            return new TargetSet(ts);
        }
    }

    class ModifyRule<P, T> : TargetRule where P : Targetable where T : Targetable
    {
        private int column;
        private int vector;
        private Func<P, T> fn;

        public ModifyRule(int vector, int column, Func<P, T> f) : base(typeof(T))
        {
            this.column = column;
            this.fn = f;
            this.vector = vector;
        }

        public override TargetSet fillCastTargets(HackStruct hs)
        {
            return TargetSet.CreateEmpty();
        }

        public override TargetSet fillResolveTargets(HackStruct hs, TargetSet c)
        {
            return new TargetSet(hs.previousTargets[vector].targetSets[column].targets.Cast<P>().Select(p => fn(p)).Cast<Targetable>());
        }

    }

    class AoeRule : TargetRule
    {
        private ChooseRule<Tile> ruler;
        private int radius;
        private Func<Card, bool> cardFilter;


        public AoeRule(Func<Tile, bool> filter, int radius, Func<Card, bool> cardFilter) : base(typeof(Card))
        {
            ruler = new ChooseRule<Tile>(filter);
            this.radius = radius;
            this.cardFilter = cardFilter;
        }

        public override TargetSet fillCastTargets(HackStruct f)
        {
            return ruler.fillCastTargets(f);
        }

        public override TargetSet fillResolveTargets(HackStruct hs, TargetSet c)
        {
            c = ruler.fillResolveTargets(hs, c);
            if (c.targets.Length != 1) throw new Exception();
            Tile centre = (Tile)c.targets[0];
            var v = centre.withinDistance(radius).Where(t => t.card != null && cardFilter(t.card)).Select(t => t.card);
            return new TargetSet(v);
        }
        
    }

    class CardResolveRule : TargetRule
    {
        private Rule rule;
        private Func<Card, bool> filter = _ => true;

        public CardResolveRule(Rule rule) : base(typeof(Card))
        {
            this.rule = rule;
        }

        public CardResolveRule(Rule rule, Func<Card, bool> filter) : this(rule)
        {
            this.filter = filter;
        }

        public override TargetSet fillCastTargets(HackStruct hs)
        {
            return TargetSet.CreateEmpty();
        }

        public override TargetSet fillResolveTargets(HackStruct hs, TargetSet c)
        {
            if (c.targets.Length != 0) throw new Exception();
            Card card;
            switch (rule)
            {
                case Rule.ResolveCard:
                {
                    card = hs.resolveCard;
                } break;

                case Rule.ResolveControllerCard:
                {
                    card = hs.resolveCard.controller.heroCard;
                } break;
                default: throw new Exception();
            }

            if (filter(card)) return new TargetSet(card);
            else return TargetSet.CreateFizzled();
        }
        

        public enum Rule
        {
            ResolveControllerCard,
            ResolveCard,
        }
        
    }

    class CardsRule : TargetRule
    {
        private Func<Card, bool> filter;

        public CardsRule(Func<Card, bool> filter) : base(typeof(Card))
        {
            this.filter = filter;
        }

        public override TargetSet fillCastTargets(HackStruct hs)
        {
            return TargetSet.CreateEmpty();
        }

        public override TargetSet fillResolveTargets(HackStruct hs, TargetSet ts)
        {
            return new TargetSet(hs.cards.Where(filter));
        }
    }

    class PlayerResolveRule : TargetRule
    {
        private Rule rule;

        public PlayerResolveRule(Rule rule) : base(typeof(Player))
        {
            this.rule = rule;
        }

        public override TargetSet fillCastTargets(HackStruct hs)
        {
            return TargetSet.CreateEmpty();
        }

        public override TargetSet fillResolveTargets(HackStruct hs, TargetSet c)
        {
            if (c.targets.Length != 0) throw new Exception();
            switch (rule)
            {
                case Rule.ResolveController:
                {
                    return new TargetSet(hs.resolveCard.controller);
                }

                case Rule.AllPlayers:
                {
                    return new TargetSet(hs.players);
                }
            }
            throw new Exception();
        }

        public enum Rule
        {
            ResolveController,
            AllPlayers,
        }

    }


    interface Targetable
    {
        int stateCtr();
    }
}
