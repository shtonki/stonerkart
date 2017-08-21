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
        public TargetRule[] rules;

        protected TargetRuleSet(IEnumerable<Type> typeSignatureTypes) : base(typeSignatureTypes)
        {
        }

        public TargetRuleSet(params TargetRule[] rules) : base(rules.Select(r => r.targetType))
        {
            this.rules = rules;
        }

        public virtual TargetMatrix fillCast(HackStruct str)
        {
            TargetColumn[] r = new TargetColumn[rules.Length];

            for (int i = 0; i < rules.Length; i++)
            {
                TargetColumn? v = rules[i].fillCastTargets(str);
                if (!v.HasValue) return null;
                r[i] = v.Value;
            }

            return new TargetMatrix(r);
        }

        public TargetMatrix fillResolve(TargetMatrix tm, HackStruct hs)
        {
            TargetColumn[] r = tm.columns;

            for (int i = 0; i < rules.Length; i++)
            {
                var column = rules[i].fillResolveTargets(hs, r[i]);
                if (!column.HasValue) return null;
                r[i] = column.Value;
            }

            return new TargetMatrix(r);
        }

        public TargetMatrix possible(HackStruct hs)
        {
            TargetColumn[] cs = new TargetColumn[rules.Length];
            for (int i = 0; i < cs.Length; i++)
            {
                cs[i] = rules[i].possible(hs);
            }

            return new TargetMatrix(cs);
        }
    }

    class TargetMatrix
    {
        public TargetColumn[] columns;

        public TargetMatrix(IEnumerable<TargetColumn> columns)
        {
            this.columns = columns.ToArray();
        }

        public TargetRow[] generateRows(bool straightRows)
        {
            TargetColumn[] cls = columns.Select(c => c.valid()).ToArray();
            if (straightRows) return straightRowsEx();
            int l = cls.Aggregate(1, (current, c) => current*c.targets.Length);
            if (l == 0) return new TargetRow[] {};

            int[] ms = cls.Select(c => c.targets.Length).ToArray();
            int[] cs = cls.Select(c => 0).ToArray();

            TargetRow[] r = new TargetRow[l];
            int rc = 0;
            while (true)
            {
                Targetable[] tr = new Targetable[cls.Length];
                for (int i = 0; i < cls.Length; i++)
                {
                    tr[i] = cls[i].targets[cs[i]];
                }
                r[rc++] = new TargetRow(tr);

                cs[cls.Length - 1]++;

                for (int i = cls.Length - 1; i > 0; i--)
                {
                    if (cs[i] == ms[i])
                    {
                        cs[i] = 0;
                        cs[i - 1]++;
                    }
                }

                if (cs[0] == ms[0]) return r;
            }
        }

        public TargetRow[] straightRowsEx()
        {
            if (columns.Select(c => c.targets.Length).Any(l => l != columns[0].targets.Length)) throw new Exception();
            int length = columns[0].targets.Length;
            TargetRow[] rows = new TargetRow[length];
            for (int i = 0; i < length; i++)
            {
                rows[i] = new TargetRow(columns.Select(c => c.targets[i]).ToArray());
            }
            return rows;
        }
    }

    abstract class TargetRule
    {
        public Type targetType { get; set; }

        public TargetRule(Type targetType)
        {
            this.targetType = targetType;
        }

        public abstract TargetColumn? fillCastTargets(HackStruct f);
        public abstract TargetColumn? fillResolveTargets(HackStruct hs, TargetColumn c);

        public abstract TargetColumn possible(HackStruct hs);

        protected const bool IGNORE = true;

        public abstract bool allowEmpty();
    }

    class SelectManaRule : TargetRule
    {
        private TargetRule playerRule;

        public SelectManaRule(TargetRule playerRule) : base(typeof(ManaOrb))
        {
            this.playerRule = playerRule;
        }

        public override TargetColumn? fillCastTargets(HackStruct f)
        {
            return playerRule.fillCastTargets(f);
        }

        public override TargetColumn? fillResolveTargets(HackStruct hs, TargetColumn c)
        {
            var selectingPlayers = playerRule.fillResolveTargets(hs, c).Value;
            return
                new TargetColumn(
                    selectingPlayers.targets.Cast<Player>().Select(p => new ManaOrb(hs.selectColour(p, clr => true))));
        }

        public override TargetColumn possible(HackStruct hs)
        {
            return new TargetColumn(Enum.GetValues(typeof (ManaColour)).Cast<ManaColour>().Select(c => new ManaOrb(c)));
        }

        public override bool allowEmpty()
        {
            return false;
        }
    }

    class StaticManaRule : TargetRule
    {
        private ManaColour[] colours;

        public StaticManaRule(params ManaColour[] colours) : base(typeof(ManaOrb))
        {
            this.colours = colours;
        }

        public override TargetColumn? fillCastTargets(HackStruct f)
        {
            return TargetColumn.empty;
        }

        public override TargetColumn? fillResolveTargets(HackStruct hs, TargetColumn c)
        {
            return new TargetColumn(colours.Select(clr => new ManaOrb(clr)));
        }

        public override TargetColumn possible(HackStruct hs)
        {
            return new TargetColumn(Enum.GetValues(typeof(ManaColour)).Cast<ManaColour>().Select(c => new ManaOrb(c)));
        }

        public override bool allowEmpty()
        {
            return false;
        }
    }

    class TriggeredTargetRule<G, T> : TargetRule where T : Targetable where G : GameEvent
    {
        private Func<G, T> func;

        public TriggeredTargetRule(Func<G, T> func) : base(typeof(T))
        {
            this.func = func;
        }

        public override TargetColumn? fillCastTargets(HackStruct f)
        {
            G g = (G)f.triggeringEvent;
            var t = (Targetable)func(g);
            return new TargetColumn(new []{t});
        }

        public override TargetColumn? fillResolveTargets(HackStruct hs, TargetColumn c)
        {
            return c;
        }

        public override TargetColumn possible(HackStruct hs)
        {
            return TargetColumn.empty;
        }

        public override bool allowEmpty()
        {
            return true;
        }
    }

    class ClickCardRule : TargetRule
    {
        private Func<Card, bool> filter;

        public ClickCardRule(Func<Card, bool> filter) : base(typeof(Card))
        {
            this.filter = filter;
        }

        public override TargetColumn? fillCastTargets(HackStruct f)
        {

            throw new NotImplementedException("if you weren't expecting too see this you might be in some trouble son");/*
            f.setPrompt("Select target.", ButtonOption.Cancel);
            while (true)
            {
                var v = f.getStuff();
                if (v is ShibbuttonStuff)
                {
                    return null;
                }
                if (v is Card)
                {
                    Card c = (Card)v;
                    if (filter(c)) return new TargetColumn(c);
                }
            }
            */
        }

        public override TargetColumn? fillResolveTargets(HackStruct hs, TargetColumn c)
        {
            return c;
        }

        public override TargetColumn possible(HackStruct hs)
        {
            return new TargetColumn(hs.cards.Where(filter));
        }

        public override bool allowEmpty()
        {
            return false;
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

        public override TargetColumn? fillCastTargets(HackStruct f)
        {
            return summonFor.fillCastTargets(f);
        }

        public override TargetColumn? fillResolveTargets(HackStruct hs, TargetColumn c)
        {
            TargetColumn? players = summonFor.fillResolveTargets(hs, c);
            if (!players.HasValue) return null;
            Player[] ps = players.Value.targets.Cast<Player>().ToArray();
            List<Card> ts = new List<Card>(ps.Length);
            foreach (Player p in ps)
            {
                foreach (var t in tokens)
                {
                    ts.Add(hs.createToken(t, p));
                }
            }
            return new TargetColumn(ts);
        }

        public override TargetColumn possible(HackStruct hs)
        {
            return TargetColumn.empty;
        }

        public override bool allowEmpty()
        {
            return true;
        }
    }

    class SelectCardRule : TargetRule
    {
        public enum Mode { Resolver, Reflective}

        private TargetRule pg;
        private PileLocation l;
        private Func<Card, bool> filter;
        private Mode mode;
        public bool cancelable { get; set; }
        public bool breakOnEmpty { get; }
        public bool sync { get; set; } = true;
        public int count;

        public SelectCardRule(TargetRule pileOwnerRule, PileLocation pile, Func<Card, bool> filter, Mode mode) : this(pileOwnerRule, pile, filter, mode, false)
        {
            
        }

        public SelectCardRule(PileLocation pile)
            : this(
                new PlayerResolveRule(PlayerResolveRule.Rule.ResolveController), pile, c => true, Mode.Resolver, false)
        {
            
        }

        public SelectCardRule(TargetRule pileOwnerRule, PileLocation pile, Func<Card, bool> filter)
            : this(pileOwnerRule, pile, filter, Mode.Resolver)
        {
            
        }


        public SelectCardRule(PileLocation pile, Func<Card, bool> filter)
            : this(new PlayerResolveRule(PlayerResolveRule.Rule.ResolveController), pile, filter, Mode.Resolver)
        {

        }

        public SelectCardRule(TargetRule pg, PileLocation l, Func<Card, bool> filter, Mode mode, bool cancelable, int count = 1, bool breakOnEmpty = true) : base(typeof(Card))
        {
            this.pg = pg;
            this.l = l;
            this.filter = filter;
            this.mode = mode;
            this.cancelable = cancelable;
            this.count = count;
            this.breakOnEmpty = breakOnEmpty;
        }

        public override TargetColumn? fillCastTargets(HackStruct f)
        {
            return pg.fillCastTargets(f);
        }

        public override TargetColumn? fillResolveTargets(HackStruct hs, TargetColumn c)
        {
            TargetColumn? t = pg.fillResolveTargets(hs, c);
            if (!t.HasValue) return null;
            List<Card> rt = new List<Card>();
            TargetColumn r = t.Value;
            foreach (var tbl in r.targets)
            {
                Player p = (Player)tbl;
                Card crd;

                Player seer;
                if (mode == Mode.Reflective)
                {
                    seer = p;
                }
                else if (mode == Mode.Resolver)
                {
                    seer = hs.resolveController;
                }
                else throw new Exception();

                for (int i = 0; i < Math.Min(count, p.pileFrom(l).Count); i++)
                {
                    crd = sync ? hs.selectCardHalfSynchronized(p.pileFrom(l), seer, filter, cancelable) : hs.selectCardUnsynchronized(p.pileFrom(l), seer, filter, cancelable);
                    if (crd == null) break; //todo allow canceling or something
                    if (rt.Contains(crd))
                    {
                        i--;
                        continue;
                    }
                    rt.Add(crd);
                }
            }
            return new TargetColumn(rt);
        }

        public override TargetColumn possible(HackStruct hs)
        {
            TargetColumn innerPossible = pg.possible(hs);
            List<Targetable> psbl = new List<Targetable>();

            foreach (var v in innerPossible.targets)
            {
                if (!(v is Player)) throw new Exception();
                Player p = (Player)v;
                psbl.AddRange(p.pileFrom(l));
            }

            return new TargetColumn(psbl);
        }

        public override bool allowEmpty()
        {
            return !breakOnEmpty || cancelable;
        }
    }

    class CastSelectCardRule : TargetRule
    {
        private SelectCardRule r;

        public CastSelectCardRule(SelectCardRule r) : base(typeof(Card))
        {
            this.r = r;
            r.cancelable = true;
            r.sync = false;
        }

        public override TargetColumn? fillCastTargets(HackStruct f)
        {
            var v = r.fillCastTargets(f);
            if (!v.HasValue)
            {
                return null;
            }
            var rt =  r.fillResolveTargets(f, v.Value).Value;
            if (rt.targets.Length == 0) return null;
            return rt;
        }

        public override TargetColumn? fillResolveTargets(HackStruct hs, TargetColumn c)
        {
            return c;
        }

        public override TargetColumn possible(HackStruct hs)
        {
            return r.possible(hs);
        }

        public override bool allowEmpty()
        {
            return r.allowEmpty();
        }
    }

    class CopyPreviousRule<T> : ResolveRule
    {
        private int column;

        public CopyPreviousRule(int column) : base(typeof(T))
        {
            this.column = column;
        }

        public override TargetColumn? fillResolveTargets(HackStruct hs, TargetColumn c)
        {
            return hs.previousTargets.columns[column];
        }

        public override TargetColumn possible(HackStruct hs)
        {
            return TargetColumn.empty;
        }

        public override bool allowEmpty()
        {
            return IGNORE;
        }
    }

    class ModifyPreviousRule<P, T> : ResolveRule where P : Targetable where T : Targetable
    {
        private int column;
        private Func<P, T> fn;

        public ModifyPreviousRule(int column, Func<P, T> f) : base(typeof(T))
        {
            this.column = column;
            this.fn = f;
        }

        public override TargetColumn? fillResolveTargets(HackStruct hs, TargetColumn c)
        {
            return new TargetColumn(hs.previousTargets.columns[column].targets.Cast<P>().Select(p => fn(p)).Cast<Targetable>());
            //return hs.previousTargets.columns[column];
        }

        public override TargetColumn possible(HackStruct hs)
        {
            return TargetColumn.empty;
            //return new TargetColumn(hs.previousColumn.targets.Cast<P>().Select(p => fn(p)).Cast<Targetable>());
            //return hs.previousColumn;
        }

        public override bool allowEmpty()
        {
            return IGNORE;
        }
    }

    class AoeRule : TargetRule
    {
        private PryTileRule ruler;
        private int radius;
        private Func<Card, bool> cardFilter;


        public AoeRule(Func<Tile, bool> filter, int radius, Func<Card, bool> cardFilter) : base(typeof(Card))
        {
            ruler = new PryTileRule(filter, new PlayerResolveRule(PlayerResolveRule.Rule.ResolveController));
            this.radius = radius;
            this.cardFilter = cardFilter;
        }

        public override TargetColumn? fillCastTargets(HackStruct f)
        {
            return ruler.fillCastTargets(f);
        }

        public override TargetColumn? fillResolveTargets(HackStruct hs, TargetColumn c)
        {
            if (c.targets.Length != 1) throw new Exception();
            Tile centre = (Tile)c.targets[0];
            var v = centre.withinDistance(radius).Where(t => t.card != null && cardFilter(t.card)).Select(t => t.card);
            return new TargetColumn(v);
        }

        public override TargetColumn possible(HackStruct hs)
        {
            return ruler.possible(hs);
        }

        public override bool allowEmpty()
        {
            return true;
        }
    }

    class ManaCostRule : TargetRule
    {
        private ManaSet ms;

        public ManaCostRule(ManaSet ms) : base(typeof(ManaOrb))
        {
            this.ms = ms;
        }

        public ManaCostRule(params ManaColour[] cs) : this(new ManaSet(cs))
        {

        }

        public ManaCostRule(int[] cs) : this(new ManaSet(cs))
        {

        }

        public override TargetColumn? fillCastTargets(HackStruct hs)
        {
            Player p = hs.castingPlayer;
            ManaSet cost = ms.clone();
            for (int i = 0; i < ManaSet.size; i++)
            {
                if ((ManaColour)i == ManaColour.Colourless) continue;
                if (p.manaPool.currentMana((ManaColour)i) < cost[i]) return null;
            }

            IEnumerable<ManaColour> poolOrbs = p.manaPool.orbs;
            IEnumerable<ManaColour> costOrbs = cost.orbs.Select(o => o.colour);

            int diff = costOrbs.Count() - poolOrbs.Count();

            if (diff > 0) return null;

            if (cost[ManaColour.Colourless] > 0)
            {
                if (diff < 0) // we have more total mana than the cost
                {
                    hs.setPrompt("Cast using what mana", ButtonOption.Cancel);
                    ManaSet colours = cost.clone();
                    colours[ManaColour.Colourless] = 0;
                    p.stuntLoss(colours);
                    while (cost[ManaColour.Colourless] > 0)
                    {
                        var v = hs.getStuff();
                        if (v is ManaOrb)
                        {
                            ManaOrb orb = (ManaOrb)v;
                            ManaColour colour = orb.colour;
                            if (p.manaPool.currentMana(colour) - cost[colour] > 0)
                            {
                                cost[colour]++;
                                cost[ManaColour.Colourless]--;
                                colours[colour]++;
                                p.stuntLoss(colours);
                            }
                        }
                        throw new NotImplementedException("if you weren't expecting too see this you might be in some trouble son");/*
                        if (v is ShibbuttonStuff)
                        {
                            ShibbuttonStuff b = (ShibbuttonStuff)v;
                            if (b.option == ButtonOption.Cancel)
                            {
                                p.unstuntMana();
                                return null;
                            }
                        }
                        */
                    }
                    p.unstuntMana();
                }
                else
                {
                    cost = new ManaSet(hs.castingPlayer.manaPool.orbs);
                }
            }
            return new TargetColumn(cost.orbs);
        }

        public override TargetColumn? fillResolveTargets(HackStruct hs, TargetColumn c)
        {
            return c;
        }

        public override TargetColumn possible(HackStruct hs)
        {
            return new TargetColumn(ms.orbs);
        }

        public override bool allowEmpty()
        {
            return false;
        }
    }

    abstract class PryRule : TargetRule
    {
        private bool pryAtResolveTime;
        private int count;
        private bool allowDuplicates;
        private TargetRule playerGenerator;

        public PryRule(Type targetType, TargetRule playerGenerator, bool pryAtResolveTime, int count,
            bool allowDuplicates) : base(targetType)
        {
            this.pryAtResolveTime = pryAtResolveTime;
            this.count = count;
            this.allowDuplicates = allowDuplicates;
            this.playerGenerator = playerGenerator;
        }

        public override TargetColumn? fillCastTargets(HackStruct hs)
        {
            return pryAtResolveTime ? playerGenerator.fillCastTargets(hs) : selectTiles(hs);
        }

        public override TargetColumn? fillResolveTargets(HackStruct hs, TargetColumn c)
        {
            if (pryAtResolveTime)
            {
                var v = playerGenerator.fillResolveTargets(hs, c);
                if (!v.HasValue) throw new Exception();
                return selectTiles(hs, v.Value);
            }
            else
            {
                return c;
            }
        }

        private TargetColumn? selectTiles(HackStruct hs, TargetColumn tc)
        {
            var pc = tc.targets;
            if (pc.Length != 1 || !(pc[0] is Player)) throw new Exception();
            Player p = (Player)pc[0];

            //List<Targetable> ts = new List<Targetable>();

            if (p == hs.hero)
            {
                var v = selectTiles(hs);
                if (!v.HasValue) throw new Exception();
                var ts = v.Value.targets;
                hs.sendChoices(ts.Select(t => TtoI(hs, t)).ToArray());
                return v.Value;
            }
            else
            {
                hs.setPrompt("Opponent is making selections.");
                return new TargetColumn(hs.receiveChoices().Select(i => ItoT(hs, i)));
            }
        }

        protected abstract int TtoI(HackStruct hs, Targetable t);
        protected abstract Targetable ItoT(HackStruct hs, int i);


        private TargetColumn? selectTiles(HackStruct hs)
        {
            List<Targetable> ts = new List<Targetable>();
            hs.highlight(hs.tilesInRange.Where(t => pryx(t, hs) != null), Color.OrangeRed);
            hs.setPrompt("Click on a tile", pryAtResolveTime ? ButtonOption.NOTHING : ButtonOption.Cancel);
            while (ts.Count < count)
            {
                Stuff v = hs.getStuff();

                throw new NotImplementedException("if you weren't expecting too see this you might be in some trouble son");/*
                if (v is ShibbuttonStuff)
                {
                    var b = (ShibbuttonStuff)v;
                    if (b.option == ButtonOption.Cancel)
                    {
                        ts.Clear();
                        break;
                    }
                }
                */
                if (!(v is Tile)) continue;

                Tile t = (Tile)v;
                Card c = t.card;
                Targetable target = pryx(t, hs);

                if (target != null && hs.tilesInRange.Contains(t))
                {
                    if (allowDuplicates || !ts.Contains(target))
                    {
                        ts.Add(target);
                        hs.highlight(new Tile[] {t}, Color.ForestGreen);
                    }
                    else
                    {
                        hs.highlight(new Tile[] {t}, Color.OrangeRed);
                        ts.Remove(target);
                    }

                }
            }
            hs.clearHighlights();
            if (ts.Count < count) return null;
            return new TargetColumn(ts);
        }
        
        public override TargetColumn possible(HackStruct hs)
        {
            return new TargetColumn(hs.tilesInRange.Select(t => pry(t)).Where(t => t != null));
        }
        
        protected Targetable pryx(Tile t, HackStruct hs)
        {
            Card c = t.card;
            if (c != null && !hs.resolveAbility.card.canTarget(c)) return null;
            return pry(t);
        }

        protected abstract Targetable pry(Tile t);

        public override bool allowEmpty()
        {
            return false;
        }
    }

    class PryTileRule : PryRule
    {
        private Func<Tile, bool> fltr;

        public PryTileRule(Func<Tile, bool> filter, TargetRule playerGenerator, bool pryAtResolveTime = false, int count = 1, bool allowDuplicates = true) : base(typeof(Tile), playerGenerator, pryAtResolveTime, count, allowDuplicates)
        {
            this.fltr = filter;
        }

        protected override Targetable pry(Tile t)
        {
            return fltr(t) ? t : null;
        }

        protected override int TtoI(HackStruct hs, Targetable t)
        {
            return hs.ordT((Tile)t);
        }

        protected override Targetable ItoT(HackStruct hs, int i)
        {
            return hs.Tord(i);
        }
    }

    class PryCardRule : PryRule
    {
        private Func<Card, bool> fltr;

        public PryCardRule(bool flip = false) : this(c => true, new PlayerResolveRule(PlayerResolveRule.Rule.ResolveController), flip)
        {
            
        }

        public PryCardRule(Func<Card, bool> filter, TargetRule playerGenerator, bool flip = false, int count = 1, bool allowDuplicates = true) : base(typeof(Card), playerGenerator, flip, count, allowDuplicates)
        {
            this.fltr = filter;
        }

        public PryCardRule(Func<Card, bool> filter)
            : this(filter, new PlayerResolveRule(PlayerResolveRule.Rule.ResolveController))
        {
            
        }

        protected override Targetable pry(Tile t)
        {
            Card card = t.card;
            return card != null && fltr(card) ? card : null;
        }

        protected override int TtoI(HackStruct hs, Targetable t)
        {
            return hs.ordC((Card)t);
        }

        protected override Targetable ItoT(HackStruct hs, int i)
        {
            return hs.Cord(i);
        }
    }

    class PryPlayerRule : PryRule
    {
        private Func<Player, bool> fltr;


        public PryPlayerRule() : this(p => true, new PlayerResolveRule(PlayerResolveRule.Rule.ResolveController))
        {
            
        }

        public PryPlayerRule(Func<Player, bool> filter, TargetRule playerGenerator, bool flip = false, int count = 1, bool allowDuplicates = true) : base(typeof(Card), playerGenerator, flip, count, allowDuplicates)
        {
            this.fltr = filter;
        }

        protected override Targetable pry(Tile t)
        {
            Card card = t.card;
            return card != null && card.isHeroic && fltr(card.owner) ? card.owner : null;
        }

        protected override int TtoI(HackStruct hs, Targetable t)
        {
            return hs.ordP((Player)t);
        }

        protected override Targetable ItoT(HackStruct hs, int i)
        {
            return hs.Pord(i);
        }
    }

    class CardResolveRule : ResolveRule
    {
        private Rule rule;

        public CardResolveRule(Rule rule) : base(typeof(Card))
        {
            this.rule = rule;
        }

        public override TargetColumn? fillResolveTargets(HackStruct hs, TargetColumn c)
        {
            if (c.targets.Length != 0) throw new Exception();
            switch (rule)
            {
                case Rule.ResolveCard:
                {
                    return new TargetColumn(hs.resolveCard);
                }

                case Rule.ResolveControllerCard:
                {
                    return new TargetColumn(hs.resolveCard.controller.heroCard);
                }

                case Rule.VillainHeroes:
                {
                    return new TargetColumn(hs.cards.Where(crd => crd.isHeroic && crd.controller != hs.resolveCard.controller));
                }

                case Rule.AllHeroes:
                {
                    return new TargetColumn(hs.cards.Where(crd => crd.isHeroic));
                }

                case Rule.AllFieldCards:
                {
                    return new TargetColumn(hs.cards.Where(crd => crd.location.pile == PileLocation.Field));
                }
            }
            throw new Exception();
        }

        public override TargetColumn possible(HackStruct hs)
        {
            return fillResolveTargets(hs, new TargetColumn(new Targetable[0])).Value;
        }

        public enum Rule
        {
            ResolveControllerCard,
            ResolveCard,
            AllHeroes,
            VillainHeroes,
            AllFieldCards,
        }

        public override bool allowEmpty()
        {
            return false;
        }
    }

    class AllCardsRule : ResolveRule
    {
        private Func<Card, bool> filter;

        public AllCardsRule(Func<Card, bool> filter) : base(typeof(Card))
        {
            this.filter = filter;
        }

        public override TargetColumn? fillResolveTargets(HackStruct hs, TargetColumn c)
        {
            return new TargetColumn(hs.cards.Where(filter));
        }

        public override TargetColumn possible(HackStruct hs)
        {
            return new TargetColumn(hs.cards.Where(filter));
        }

        public override bool allowEmpty()
        {
            return true;
        }
    }

    class PlayerResolveRule : ResolveRule
    {
        private Rule rule;

        public PlayerResolveRule(Rule rule) : base(typeof(Player))
        {
            this.rule = rule;
        }

        public override TargetColumn? fillResolveTargets(HackStruct hs, TargetColumn c)
        {
            if (c.targets.Length != 0) throw new Exception();
            switch (rule)
            {
                case Rule.ResolveController:
                {
                    return new TargetColumn(hs.resolveCard.controller);
                }

                case Rule.AllPlayers:
                {
                    return new TargetColumn(hs.players);
                }
            }
            throw new Exception();
        }

        public override TargetColumn possible(HackStruct hs)
        {
            return fillResolveTargets(hs, new TargetColumn(new Targetable[0])).Value;
        }

        public enum Rule
        {
            ResolveController,
            AllPlayers,
        }
        public override bool allowEmpty()
        {
            return false;
        }
    }

    abstract class ResolveRule : TargetRule
    {
        public ResolveRule(Type t) : base(t)
        {
        }

        public override TargetColumn? fillCastTargets(HackStruct box)
        {
            return new TargetColumn(new Targetable[] {});
        }

    }


    struct TargetRow
    {
        private Targetable[] ts;

        public TargetRow(Targetable[] ts)
        {
            this.ts = ts;
        }

        public Targetable this[int ix] => ts[ix];
    }
    

    struct TargetColumn
    {
        public Targetable[] targets;
        private int[] stateCtrs;

        public static TargetColumn empty => new TargetColumn(new Targetable[0]);

        public TargetColumn(params Targetable[] targets)
        {
            this.targets = targets;
            stateCtrs = targets.Select(t => t.stateCtr()).ToArray();
        }

        public TargetColumn(IEnumerable<Targetable> ts) : this(ts.ToArray())
        {
            
        }

        public TargetColumn valid()
        {
            var ebinLanguage = stateCtrs;
            return new TargetColumn(targets.Where((t, i) => t.stateCtr() == ebinLanguage[i]));
        }
    }
    
    
    interface Targetable
    {
        int stateCtr();
    }
}
