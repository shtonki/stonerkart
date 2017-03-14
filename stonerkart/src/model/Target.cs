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
            if (straightRows) return straightRowsEx();
            int l = columns.Aggregate(1, (current, c) => current*c.targets.Length);
            if (l == 0) return new TargetRow[] {};

            int[] ms = columns.Select(c => c.targets.Length).ToArray();
            int[] cs = columns.Select(c => 0).ToArray();

            TargetRow[] r = new TargetRow[l];
            int rc = 0;
            while (true)
            {
                Targetable[] tr = new Targetable[columns.Length];
                for (int i = 0; i < columns.Length; i++)
                {
                    tr[i] = columns[i].targets[cs[i]];
                }
                r[rc++] = new TargetRow(tr);

                cs[columns.Length - 1]++;

                for (int i = columns.Length - 1; i > 0; i--)
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
    }

    class SelectCardRule : TargetRule
    {
        private TargetRule pg;
        private PileLocation l;
        private Func<Card, bool> filter;

        public SelectCardRule(PileLocation l, TargetRule pg, Func<Card, bool> filter) : base(typeof(Card))
        {
            this.pg = pg;
            this.l = l;
            this.filter = filter;
        }

        public SelectCardRule(PileLocation location, TargetRule playerGenerator) : this(location, playerGenerator, c => true)
        {
            pg = playerGenerator;
            l = location;
        }

        public override TargetColumn? fillCastTargets(HackStruct f)
        {
            return pg.fillCastTargets(f);
        }

        public override TargetColumn? fillResolveTargets(HackStruct hs, TargetColumn c)
        {
            TargetColumn? t = pg.fillResolveTargets(hs, c);
            if (!t.HasValue) return null;
            TargetColumn r = t.Value;
            if (r.targets.Length != 1) throw new Exception();
            Player p = (Player)r.targets[0];
            while (true)
            {
                Card crd = hs.selectCardSynchronized(p.pileFrom(l), filter);
                if (crd == null) return null;
                return new TargetColumn(crd);
            }
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
            return hs.previousColumn;
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
                        if (v is ShibbuttonStuff)
                        {
                            ShibbuttonStuff b = (ShibbuttonStuff)v;
                            if (b.option == ButtonOption.Cancel)
                            {
                                p.unstuntMana();
                                return null;
                            }
                        }
                    }
                    p.unstuntMana();
                }
                else
                {
                    cost = hs.castingPlayer.manaPool.current;
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
    }

    abstract class PryRule : TargetRule
    {
        private bool pryAtResolveTime;
        private int count;
        private bool allowDuplicates;
        private TargetRule playerGenerator;

        public PryRule(Type targetType, TargetRule playerGenerator, bool pryAtResolveTime, int count, bool allowDuplicates) : base(targetType)
        {
            this.pryAtResolveTime = pryAtResolveTime;
            this.count = count;
            this.allowDuplicates = allowDuplicates;
            this.playerGenerator = playerGenerator;
        }

        private TargetColumn? playerCache;
        public override TargetColumn? fillCastTargets(HackStruct hs)
        {
            playerCache = playerGenerator.fillCastTargets(hs);
            return pryAtResolveTime ? new TargetColumn(new Targetable[0]) : loopEx(hs);
        }

        public override TargetColumn? fillResolveTargets(HackStruct hs, TargetColumn c)
        {
            playerCache = playerGenerator.fillResolveTargets(hs, playerCache.Value);
            return !pryAtResolveTime ? c : loopEx(hs);
        }

        private TargetColumn? loopEx(HackStruct hs)
        {
            var pc = playerCache.Value.targets;
            if (pc.Length != 1 || !(pc[0] is Player)) throw new Exception();
            Player p = (Player)pc[0];

            List<Targetable> ts = new List<Targetable>();
            hs.highlight(hs.tilesInRange.Where(t => pry(t) != null), Color.OrangeRed);
            hs.setPrompt("Click on a tile", pryAtResolveTime ? ButtonOption.NOTHING : ButtonOption.Cancel);
            while (ts.Count < count)
            {
                Stuff v = hs.getStuff();

                if (v is ShibbuttonStuff)
                {
                    var b = (ShibbuttonStuff)v;
                    if (b.option == ButtonOption.Cancel)
                    {
                        ts.Clear();
                        break;
                    }
                }

                if (!(v is Tile)) continue;

                Tile t = (Tile)v;

                Targetable target = pry(t);
                
                if (target != null && hs.tilesInRange.Contains(t))
                {
                    if (allowDuplicates || !ts.Contains(target))
                    {
                        ts.Add(target);
                        hs.highlight(new Tile[] { t }, Color.ForestGreen);
                    }
                    else
                    {
                        hs.highlight(new Tile[] { t }, Color.OrangeRed);
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
            return new TargetColumn(hs.tilesInRange.Where(t => pry(t) != null));
        }

        protected abstract Targetable pry(Tile t);
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

        protected override Targetable pry(Tile t)
        {
            Card card = t.card;
            return card != null && fltr(card) ? card : null;
        }
    }

    class PryPlayerRule : PryRule
    {
        private Func<Player, bool> fltr;
        

        public PryPlayerRule(Func<Player, bool> filter, TargetRule playerGenerator, bool flip = false, int count = 1, bool allowDuplicates = true) : base(typeof(Card), playerGenerator, flip, count, allowDuplicates)
        {
            this.fltr = filter;
        }

        protected override Targetable pry(Tile t)
        {
            Card card = t.card;
            return card != null && card.isHeroic && fltr(card.owner) ? card.owner : null;
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
            
            VillainHeroes,
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

        public static TargetColumn empty => new TargetColumn();

        public TargetColumn(params Targetable[] targets)
        {
            this.targets = targets;
        }

        public TargetColumn(IEnumerable<Targetable> ts) : this(ts.ToArray())
        {
            
        }
    }
    
    
    interface Targetable
    {
        
    }
}
