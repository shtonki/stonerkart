#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

#endregion

namespace stonerkart
{
    partial class Card : Observable<CardChangedMessage>, Stuff, Observer<int>
    {

        public bool IsCreature => cardType == CardType.Creature;
        public bool IsHeroic => isHeroic;
        public bool IsExhausted => movement == 0;
        public bool IsDamaged => damageTaken > 0;
        public bool Is(Counter sw) => StacksCount(sw) > 0;
        public bool Is(ManaColour mc) => colours.Contains(mc);
        public bool Is(CardType ct) => cardType == ct;
        public bool HasKeyword(KeywordAbility kwa) => keywordAbilities.Contains(kwa);
        public bool canExhaust => fatigue == 0 && movement > 0;



        public string name { get; }
        public Pile pile { get; private set; }

        public Tile tile
        {
            get { return Tile; } 
            set
            {
                Tile = value;
                if (Tile != null)
                {
                    lastSeenAt = Tile;
                }
            }
        }
        private Tile Tile;
        public Tile lastSeenAt { get; private set; }
        public int moveCount { get; private set; }
        public Player owner { get; }
        public Player Controller { get; }
        public CardTemplate template { get; }
        public bool isDummy { get; private set; }
        public bool isToken { get; private set; }
        public CardType cardType { get; }
        public Rarity rarity { get; set; }
        public CardSet set { get; }
        public Race race => ForceRace.HasValue ? ForceRace.Value : baseRace;
        private Race baseRace { get; }
        private Race? ForceRace => getForceRace();
        private Modifiable forceRace { get; set; }

        private Race? getForceRace()
        {
            if (forceRace.value >= 0) return (Race)forceRace.value;
            return null;
        }

        public Subtype? subtype { get; }
        public int convertedManaCost => castManaCost.colours.Count();

        public bool canRetaliate => !IsHeroic;

        public string breadText => breadTextEx();
        public string flavourText { get; }

        public Location location => locationEx();

        public Path combatPath { get; set; }

        public ActivatedAbility castAbility { get; }
        private List<ActivatedAbility> alternateCasts = new List<ActivatedAbility>();
        public int castRange => castAbility.castRange;
        public ManaSet castManaCost { get; }

        public string typeText => typeTextEx();

        public Card dummyFor => dummiedAbility.card;
        public Ability dummiedAbility { get; private set; }

        private List<ActivatedAbility> activatedAbilities = new List<ActivatedAbility>();
        private List<TriggeredAbility> triggeredAbilities = new List<TriggeredAbility>();
        public IEnumerable<Ability> abilities => activatedAbilities.Cast<Ability>().Concat(triggeredAbilities);
        //private List<Ability> abilities { get; }= new List<Ability>();
        private IEnumerable<ActivatedAbility> usable => usableEx();
        public IEnumerable<ActivatedAbility> usableHere => usable.Where(a => a.activeIn == location.pile);

        public List<ManaColour> colours => coloursEx();

        public List<Aura> auras { get; } = new List<Aura>();

        private bool isHeroic { get; }

        public bool hasPT => cardType == CardType.Creature;
        public bool hasMovenemt => cardType == CardType.Creature || cardType == CardType.Relic;

        public int power => Power;
        public int toughness => Toughness - damageTaken;
        public int movement => movementEx();

        private Modifiable Power { get; }
        private Modifiable Toughness { get; }
        private Modifiable Movement { get; }

        private int damageTaken;
        public int fatigue { get; private set; }

        private ManaColour? forceColour { get; }
        private Modifiable[] modifiables;

        private List<KeywordAbility> keywordAbilities;

        private CounterCounter counterCounter { get; } = new CounterCounter();

        public int StacksCount(Counter sw) => counterCounter[sw];


        private GameEventHandlerBuckets eventHandler;

        public bool isColour(ManaColour c)
        {
            return colours.Contains(c);
        }

        public int combatDamageTo(Card defender)
        {
            int d = power;
            if (defender.IsHeroic && HasKeyword(KeywordAbility.Kingslayer)) d = d*2;
            return d;
        }

        public bool canMove => cardType == CardType.Creature && movement > 0;

        public bool canAttack(Card defender)
        {
            if (defender.cardType != CardType.Creature) return false;
            if (defender.HasKeyword(KeywordAbility.Flying) &&
                !(this.HasKeyword(KeywordAbility.Flying) || this.HasKeyword(KeywordAbility.Wingclipper))) return false;
            return true;
        }

        public bool canDamage(Card defender)
        {
            return true;
        }

        public bool canTarget(Card target)
        {
            if (target.HasKeyword(KeywordAbility.Elusion)) return false;
            return true;
        }


        public void moveTo(Pile p)
        {
            pile?.remove(this);
            pile = p;
            p.addTop(this);
        }

        public void moveTo(Tile t)
        {
            tile?.removeCard();
            tile = t;
            t.place(this);
        }

        public void exhaust()
        {
            exhaust(movement);
        }

        public void exhaust(int steps)
        {
            fatigue += steps;
        }

        public void damage(int d)
        {
            damageTaken = Math.Max(damageTaken + d, 0);
            notify(new CardChangedMessage(Toughness));
        }

        /// <summary>
        /// Returns a list containing the unique colours of the card. If the card has no mana cost it returns an 
        /// array containing only ManaColour.Colourless.
        /// </summary>
        private List<ManaColour> coloursEx()
        {
            if (forceColour.HasValue) return new List<ManaColour>(new[] {forceColour.Value});
            HashSet<ManaColour> hs = new HashSet<ManaColour>(castManaCost.colours.Where(x => x != ManaColour.Colourless));
            if (hs.Count == 0) return new List<ManaColour>(new ManaColour[] { ManaColour.Colourless });

            return hs.ToList();
        }
        private int movementEx()
        {
            var val = Movement.value;

            val -= StacksCount(Counter.Crippled);
            val = Math.Max(val, 1);

            if (Is(Counter.Stunned)) val = 0;

            val -= fatigue;

            val = Math.Max(val, 0);

            return val;
        }

        private IEnumerable<ActivatedAbility> usableEx()
        {
            if (Is(Counter.Stunned)) return new ActivatedAbility[0];

            return activatedAbilities;
        }


        public bool isCastAbility(Ability a)
        {
            return castAbility == a || alternateCasts.Contains(a);
        }

        public IEnumerable<TriggeredAbility> abilitiesTriggeredBy(GameEvent e)
        {
            return triggeredAbilities.Where(a => a.triggeredBy(e));
        }

        public void handleEvent(GameEvent e)
        {
            foreach (Modifiable modifiable in modifiables)
            {
                modifiable.check(e);
            }

            shitterhack.handle(e);

            if (!(e is CardedEvent && ((CardedEvent)e).getCard() == this)) return;
            eventHandler.handle(e);
            notify(this, 0);

        }

        public int stateCtr()
        {
            return moveCount;
        }

        public void updateState()
        {
            foreach (Modifiable modifiable in modifiables)
            {
                modifiable.recount();
            }
        }

        private string typeTextEx()
        {
            if (cardType == CardType.Creature)
            {
                string s = IsHeroic ? "Heroic " : "";
                s += race.ToString();
                if (subtype.HasValue) s += ' ' + subtype.Value.ToString();
                return s;
            }
            else
            {
                return cardType.ToString();
            }
        }

        private string breadTextEx()
        {
            if (isDummy)
            {
                return dummiedAbility.description;
            }

            StringBuilder sb = new StringBuilder();

            foreach (Ability a in abilities)
            {
                string s = a.description;
                if (s.Length > 0)
                {
                    sb.Append(a.description);
                    sb.Append(G.newlineGlyph);
                }
            }

            foreach (var a in keywordAbilities)
            {
                sb.Append(a);
                sb.Append(" -- (");
                sb.Append(kaExplainer(a));
                sb.Append(')');
                sb.Append(G.newlineGlyph);

            }

            foreach (var a in auras)
            {
                sb.Append(a.description);
                sb.Append(G.newlineGlyph);

            }

            var counts = counterCounter.Counts;
            for (int i = 0; i < counts.Length; i++)
            {
                if (counts[i] > 0)
                {
                    Counter sw = (Counter)i;
                    sb.Append(sw + ": " + counts[i]);
                    sb.Append(G.newlineGlyph);
                }
            }

            return sb.ToString();
        }

        private static string kaExplainer(KeywordAbility ab)
        {
            switch (ab)
            {
                case KeywordAbility.Elusion:
                {
                    return "Cannot be targeted.";
                }

                case KeywordAbility.Fervor:
                {
                    return "Enters the battlefield with full movement.";
                }

                case KeywordAbility.Kingslayer:
                {
                    return "Deals double damage to heroic creatures";
                }

                case KeywordAbility.Ambush:
                {
                    return "When attacking this creature deals damage prior to the defender and is not retaliated against if it kills the defender.";
                }

                case KeywordAbility.Flying:
                {
                    return "Can only be attacked by creatures with Flying";
                } 

                case KeywordAbility.Wingclipper:
                {
                    return "Can attack creatures with Flying.";
                }

                case KeywordAbility.Reinforcement:
                {
                    return "May be cast whenever you may cast an interrupt.";
                }
            }

            return "";
        }

        public Card createDummy(Ability a)
        {
            Card r = new Card(template, owner);
            r.isDummy = true;
            r.dummiedAbility = a;
            r.rarity = Rarity.None;
            return r;
        }

        private Location locationEx()
        {
            if (pile == null) return new Location(owner, PileLocation.Nowhere);
            return pile.location;
        }

        public void notify(object o, int t)
        {
            notify(new CardChangedMessage());
        }

        public override string ToString()
        {
            return name;
        }

        public static Card[] flyweight { get; } =
            Enum.GetValues(typeof (CardTemplate)).Cast<CardTemplate>().Select(ct => new Card(ct)).ToArray();

        public static Card fromTemplate(CardTemplate ct)
        {
            return flyweight[(int)ct];
        }

        private TypedGameEventHandler<StartOfStepEvent> shitterhack;

        private GameEventHandlerBuckets generateDefaultEventHandlers()
        {
            GameEventHandlerBuckets r = new GameEventHandlerBuckets();

            shitterhack =
                new TypedGameEventHandler<StartOfStepEvent>(
                    new TypedGameEventFilter<StartOfStepEvent>(
                        a => a.step == Steps.Replenish && a.activePlayer == Controller),
                    e =>
                    {
                        fatigue = 0;
                        counterCounter.TickDown();
                    });

            r.add(new TypedGameEventHandler<ApplyStacksEvent>(e =>
            {
                e.card.counterCounter[e.stack] += e.count;
            }));

            r.add(new TypedGameEventHandler<RaceModifyEvent>(e =>
            {
                e.card.forceRace.modify(new ModifierStruct(Set((int)e.race), e.filter));
            }));

            r.add(new TypedGameEventHandler<ModifyEvent>(e =>
            {
                Modifiable m;
                switch (e.stat)
                {
                    case ModifiableStats.Movement: m = Movement; break;
                    case ModifiableStats.Toughness: m = Toughness; break;
                    case ModifiableStats.Power: m = Power; break;

                    default: throw new Exception();
                }

                m.modify(e.modifier);
            }));

            r.add(new TypedGameEventHandler<PlaceOnTileEvent>(e =>
            {
                if (e.tile.card != null) throw new Exception();

                moveTo(e.tile);
                if (!e.dontExhaust && (cardType == CardType.Creature && !HasKeyword(KeywordAbility.Fervor))) exhaust();
            }));

            r.add(new TypedGameEventHandler<FatigueEvent>(e =>
            {
                exhaust(e.amount);
            }));

            r.add(new TypedGameEventHandler<MoveEvent>(e =>
            {
                Path path = e.path;
                Tile destination = path.last;
                moveTo(destination);
            }));

            r.add(new TypedGameEventHandler<DamageEvent>(e =>
            {
                if (e.source.canDamage(e.target))
                e.target.damage(e.amount);
            }));

            r.add(new TypedGameEventHandler<MoveToPileEvent>(e =>
            {
                moveCount++;
                if (e.nullTile && tile != null)
                {
                    tile.removeCard();
                    tile = null;
                }
                if (e.to.location.pile != PileLocation.Field)
                {
                    if (isToken)
                    {
                        pile.remove(this);
                        pile = null;
                        return;
                    }
                    fatigue = 0;
                    damageTaken = 0;
                }
                moveTo(e.to);
            }));
            
            return r;
        }

        //can't see me
        public TriggerGlueHack tghack { get; set; }


        #region legallambdas

        private static Func<int, int> Add(int val) => a => a + val;
        private static Func<int, int> Set(int val) => a => val;

        private static GameEventFilter Forever => new StaticGameEventFilter(() => false);
        private static GameEventFilter EndOfTurn => new TypedGameEventFilter<StartOfStepEvent>(e => e.step == Steps.Replenish);
        private static GameEventFilter StartOfHeros(Steps step) => new TypedGameEventFilter<StartOfStepEvent>(e => e.activePlayer.IsHero && e.step == step);


        private static StaticGenerator<T> sg<T>(params T[] ts)
        {
            return new StaticGenerator<T>(ts);
        }
        private static Generator<Player> ResolveController => new ResolvePlayerRule(ResolvePlayerRule.Rule.ResolveController);
        private static Generator<Card> ResolveCard => new ResolveCardRule(ResolveCardRule.Rule.ResolveCard);
        private static Generator<Card> ResolveControllerCard => new ResolveCardRule(ResolveCardRule.Rule.ResolveControllerCard);
        private static Generator<Card> VillainsCard => new ResolveCardRule(ResolveCardRule.Rule.VillansCard);

        private static Effect ResolverPaysManaEffect(params ManaColour[] colours)
        {
            return ResolverPaysManaEffect(new ManaSet(colours));
        }
        private static Effect ResolverPaysManaEffect(ManaSet set)
        {
            return new PayManaEffect(ResolveController, sg(set));
        }

        private static Effect SummonTokensEffect(Generator<Player> playersgen, params CardTemplate[] cts)
        {
            return new SummonToTileEffect(new CreateTokens(cts), new ChooseHexTile(true, t => t.Summonable, cts.Length));
        }
        private static Effect ExhaustThis => new FatigueEffect(new ResolveCardRule(ResolveCardRule.Rule.ResolveCard, c => c.canExhaust), c => c.Movement);
        private Effect SacThis => new MoveToPileEffect(PileLocation.Graveyard, new ResolveCardRule(ResolveCardRule.Rule.ResolveCard, c => c.location.pile == PileLocation.Field));

        private void AddDiesLambda(string description, Foo effect, int range = -1, bool optional = false)
        {
            AddTriggeredAbility(
                description,
                effect,
                new Foo(),
                new TypedGameEventFilter<MoveToPileEvent>(
                    moveEvent =>
                        moveEvent.card == this && moveEvent.to.location.pile == PileLocation.Graveyard &&
                        location.pile == PileLocation.Field),
                range,
                PileLocation.Field,
                optional);
        }
        private void AddEtBLambda(string description, Foo effect, int range = -1, bool optional = false)
        {
            AddTriggeredAbility(
                description,
                effect,
                new Foo(),
                new TypedGameEventFilter<MoveToPileEvent>(moveEvent => moveEvent.card == this && location.pile == PileLocation.Field),
                range,
                PileLocation.Field,
                optional,
                TriggeredAbility.Timing.Post
                );
        }
        private void AddDeathtouchLambda()
        {
            AddTriggeredAbility(
                        "Whenever this creature deals damage to a non-heroic creature destroy it.",
                        new Foo(new MoveToPileEffect(PileLocation.Graveyard, new TriggeredRule<DamageEvent, Card>(de => de.target))),
                        new Foo(),
                        new TypedGameEventFilter<DamageEvent>(de => de.source == this && !de.target.isHeroic),
                        0,
                        PileLocation.Field,
                        false
);
        }
        #endregion
    }

    class Aura
    {
        public string description { get; }
        public ModifiableStats stat { get; }
        public Func<Card, bool> filter { get; }
        public PileLocation activeIn { get; }

        private Func<int, int> fn;

        private static GameEventFilter clearaura => new TypedGameEventFilter<ClearAurasEvent>();

        public ModifierStruct modifer => new ModifierStruct(fn, clearaura);

        public Aura(string description, Func<int, int> fn, ModifiableStats stat, Func<Card, bool> filter, PileLocation activeIn)
        {
            this.fn = fn;
            this.description = description;
            this.stat = stat;
            this.filter = filter;
            this.activeIn = activeIn;
        }
    }

    enum ModifiableStats { Power, Toughness, Movement, Race };


    public enum CardTemplate
    {
        Confuse,
        Big_sMonkey,
    	Great_sWhite_sBuffalo,
    	Alter_sFate,
        Count_sFera_sII,
        Spiderling,
        Arachosa,
        Paralyzing_sSpider,
        Seblastian,
        Warp,
        Hosro,
        Jabroncho,
        Iradj,
        Makaroni,
        Archfather,
        Hungry_sFelhound,
        Vincennes,
        Ilatian_sGhoul,
        Commander_sSparryz,
        Flamekindler,
        Moratian_sBattle_sStandard,
        Seraph,
        Chromatic_sUnicorn,
        Enraged_sDragon,
        Haunted_sChapel,
        Unyeilding_sStalwart,
        Bubastis,
        Morenian_sMedic,
        Famished_sTarantula,
        Vibrant_sZinnia,
        Primal_sChopter,
        Stark_sLily,
        Serene_sDandelion,
        Daring_sPoppy,
        Mysterious_sLilac,
        Solemn_sLotus,
        Resounding_sBlast,
        Feral_sImp,
        Shotty_sContruct,
        Houndmaster,
        Marilith,
        Seething_sRage,
        Ilas_sBargain,
        Rapture,
        Suspicious_sVortex,
        Ancient_sDruid,
        Raise_sDead,
        Deep_sFry,
        Abolish,
        Chains_sof_sVirtue,
        Chains_sof_sSin,
        Rider_sof_sDeath,
        Rider_sof_sWar,
        Rider_sof_sPestilence,
        Rider_sof_sFamine,
        Magma_sVents,
        Gotterdammerung,
        Overgrow,
        Gleeful_sDuty,
        Counterspell,
        Invigorate,
        Ilatian_sHaunter,
        Frenzied_sPirhana,
        Ilas_sGravekeeper,
        Kraken,
        Prince_sIla,
        Wilt,
        Huntress_sOf_sNibemem,
        Baby_sDragon,
        Fresh_sFox,
        Survival_sInstincts,
        Damage_sWard,
        One_sWith_sNature,
        Graverobber_sSyrdin,
        Sinister_sPact,
        Goblin_sGrenade,
        Cleansing_sFire,
        Bhewas,
        Zap,
        Kappa,
        Cantrip,
        Temple_sHealer,
        Yung_sLich,
        Chieftain_sZ_aloot_aboks,
        Risen_sAbberation,
        Shibby_sShtank,
        Unmake,
        Rockhand_sEchion,
        Primordial_sChimera,
        Illegal_sGoblin_sLaboratory,
        Teleport,
        Squire,
        Spirit,
        Gryphon,
        Wolf,
        Zombie,
        Rock,
        Call_sTo_sArms,
        Sanguine_sArtisan,
        Shimmering_sKoi,
        Decaying_sHorror,
        Relentless_sConsriptor,
        Terminate,
        Spirit_sof_sSalvation,
        Benedictor,
        Pyrostorm,
        Elven_sCultivator,
        Faceless_sSphinx,
        Cerberus,
        Heroic_sMight,
        Taouy_sRuins,
        Shibby_as_sSaboteur,
        Brute_sForce,
        Scroll_sof_sEarth,
        Maleficent_sSpirit,
        Thirstclaw,
        Flameheart_sPhoenix,
        Lord_sPlombie,
        Gryphon_sRider,
        Ravaging_sGreed,
        Water_sBolt,
        Lone_sRanger,
        AOE_sEXHAUST,
        Charging_sBull,
        Ent
    }


    internal enum CardType
    {
        Creature,
        Interrupt,
        Channel,
        Relic
    }

    public enum Rarity
    {
        None,
        Common,
        Uncommon,
        Rare,
        Legendary
    }

    internal enum CastSpeed
    {
        Interrupt,
        Channel
    }

    public enum CardSet
    {
        FirstEdition
    }

    internal enum Race
    {
        Elemental,
        Elf,
        Angel,
        Demon,
        Mecha,
        Human,
        Undead,
        Zombie,
        Goblin,
        Giant,
        Beast,
        Dragon,
        Vampire,
        Spirit,
    }

    internal enum Subtype
    {
        Hunter,
        Guardian,
        Warrior,
        Wizard,
        Cleric,
        Rogue,
    }

    enum KeywordAbility
    {
        Fervor,
        Elusion,
        Kingslayer,
        Ambush,
        Flying,
        Wingclipper,
        Reinforcement,
    }

    struct CardChangedMessage
    {
        public Modifiable modified { get; }

        public CardChangedMessage(Modifiable modified)
        {
            this.modified = modified;
        }
    }
}