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
    partial class Card : Observable<CardChangedMessage>, Stuff, Targetable, Observer<int>
    {
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
        public Player controller { get; }
        public CardTemplate template { get; }
        public bool isDummy { get; private set; }
        public bool isToken { get; private set; }
        public CardType cardType { get; }
        public Rarity rarity { get; set; }
        public CardSet set { get; }
        public Race race => forceRace.HasValue ? forceRace.Value : baseRace;
        private Race baseRace { get; }
        private Race? forceRace { get; set; }

        public Subtype? subtype { get; }
        public int convertedManaCost => castManaCost.colours.Count();

        public bool isExhausted => movement == 0;
        public bool canRetaliate => !isHeroic;

        public string breadText => breadTextEx();

        public Location location => locationEx();

        public Path combatPath { get; set; }

        public ActivatedAbility castAbility { get; }
        private List<ActivatedAbility> alternateCasts = new List<ActivatedAbility>();
        public int castRange => castAbility.castRange;
        public ManaSet castManaCost { get; }

        public string typeText => typeTextEx();

        public Card dummyFor => dummiedAbility.card;
        public Ability dummiedAbility { get; private set; }

        public IEnumerable<ActivatedAbility> activatedAbilities
            => abilities.Where(a => a is ActivatedAbility).Cast<ActivatedAbility>();

        public IEnumerable<TriggeredAbility> triggeredAbilities
            => abilities.Where(a => a is TriggeredAbility).Cast<TriggeredAbility>();

        private List<Ability> abilities = new List<Ability>();
        public ActivatedAbility[] usableHere => activatedAbilities.Where(a => a.activeIn == location.pile).ToArray();

        public List<ManaColour> colours => coloursEx();

        public List<Aura> auras { get; } = new List<Aura>();

        public bool isHeroic { get; }

        public bool hasPT => cardType == CardType.Creature;
        public bool hasMovenemt => cardType == CardType.Creature || cardType == CardType.Relic;

        public int power => Power;
        public int toughness => Toughness - damageTaken;
        public int movement => Movement - fatigue;

        private Modifiable Power;
        private Modifiable Toughness;
        private Modifiable Movement;

        private int damageTaken;
        public int fatigue { get; private set; }

        private ManaColour? forceColour { get; }
        private Modifiable[] modifiables;

        private List<KeywordAbility> keywordAbilities;

        private GameEventHandlerBuckets eventHandler;

        public bool isColour(ManaColour c)
        {
            return colours.Contains(c);
        }

        public int combatDamageTo(Card defender)
        {
            int d = power;
            if (defender.isHeroic && hasAbility(KeywordAbility.Kingslayer)) d = d*2;
            return d;
        }

        public bool canMove => cardType == CardType.Creature && movement > 0;

        public bool canAttack(Card defender)
        {
            if (defender.cardType != CardType.Creature) return false;
            if (defender.hasAbility(KeywordAbility.Flying) &&
                !(this.hasAbility(KeywordAbility.Flying) || this.hasAbility(KeywordAbility.Wingclipper))) return false;
            return true;
        }

        public bool canDamage(Card defender)
        {
            return true;
        }

        public bool canTarget(Card target)
        {
            if (target.hasAbility(KeywordAbility.Elusion)) return false;
            return true;
        }

        public bool canExhaust => fatigue == 0;

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

        public bool hasAbility(KeywordAbility kwa)
        {
            return keywordAbilities.Contains(kwa);
        }

        public bool isCastAbility(Ability a)
        {
            return castAbility == a || alternateCasts.Contains(a);
        }

        public int abilityOrd(Ability a)
        {
            if (abilities.IndexOf(a) < 0)
            {
                int i = 2;
            }
            return abilities.IndexOf(a);
        }

        public Ability abilityFromOrd(int v)
        {
            return abilities[v];
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
                string s = isHeroic ? "Heroic " : "";
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

        private static Card[] flyweight { get; } =
            Enum.GetValues(typeof (CardTemplate)).Cast<CardTemplate>().Select(ct => new Card(ct)).ToArray();

        public static Card fromTemplate(CardTemplate ct)
        {
            return flyweight[(int)ct];
        }

        private TypedGameEventHandler<StartOfStepEvent> shitterhack;

        private GameEventHandlerBuckets generatedlft()
        {
            GameEventHandlerBuckets r = new GameEventHandlerBuckets();

            shitterhack =
                new TypedGameEventHandler<StartOfStepEvent>(
                    new TypedGameEventFilter<StartOfStepEvent>(
                        a => a.step == Steps.Replenish && a.activePlayer == controller
                        ), e => fatigue = 0);


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
                if (!e.dontExhaust && (cardType == CardType.Creature && !hasAbility(KeywordAbility.Fervor))) exhaust();
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

        #region helpers

        #region Foos

        private static Foo emptyFoo => new Foo();

        private static Foo manaCostFoo(params ManaColour[] cs)
        {
            return new Foo(manaCostEffect(cs));
        }
        #endregion

        #region Effects

        public static Effect exhaustThis => new 
            Effect(new TargetRuleSet(new CardResolveRule(CardResolveRule.Rule.ResolveCard, c => c.canExhaust)), 
            new FatigueDoer(true));
        public Effect sacThisLambda => new Effect(new CardResolveRule(CardResolveRule.Rule.ResolveCard), new MoveToPileDoer(PileLocation.Graveyard));
        public Effect sacCostLambda
            =>
                new Effect(
                    new ChooseRule<Card>(
                        c => !c.isHeroic && c.controller == this.controller && c.cardType == CardType.Creature),
                    new MoveToPileDoer(PileLocation.Graveyard));

        public Effect zepLambda(int damage)
        {
            return
                new Effect(new TargetRuleSet(new CardResolveRule(CardResolveRule.Rule.ResolveCard), creature()),
                    new PingDoer(damage));
        }


        public static Effect manaCostEffect(ManaSet castManaCost)
        {
            return new Effect(
                new TargetRuleSet(new PlayerResolveRule(PlayerResolveRule.Rule.ResolveController),
                    new ManaCostRule(castManaCost)), new PayManaDoer());
        }

        public static Effect manaCostEffect(params ManaColour[] ms)
        {
            return manaCostEffect(new ManaSet(ms));
        }


        public Effect zepNonHeroicLambda(int damage)
        {
            return
                new Effect(new TargetRuleSet(new CardResolveRule(CardResolveRule.Rule.ResolveCard), nonheroicCreature()),
                    new PingDoer(damage));
        }

        public static Effect displaceFromGraveyard(Func<Card, bool> filter = null)
        {
            filter = filter ?? (c => true);
            return new Effect(new ChooseRule<Card>(
                new SelectCardRule(PileLocation.Graveyard, SelectCardRule.Mode.PlayerLooksAtPlayer),
                ChooseRule<Card>.ChooseAt.Cast,
                filter),
                new MoveToPileDoer(PileLocation.Displaced));
        }

        public Effect playerSacLambda(TargetRule sacrificer)
        {
            return new Effect(new ChooseRule<Card>(
                new SelectCardRule(PileLocation.Field, SelectCardRule.Mode.PlayerLooksAtPlayer),
                sacrificer,
                ChooseRule<Card>.ChooseAt.Resolve,
                c => c.cardType == CardType.Creature && !c.isHeroic),
                new MoveToPileDoer(PileLocation.Graveyard));
        }

        #endregion

        #region addTriggeredAbility lambdas
        public void etbLambda(String description, Effect e, int range = -1, bool optional = false)
        {
            addTriggeredAbility(
                        description,
                        e,
                        new Foo(),
                        new TypedGameEventFilter<MoveToPileEvent>(
                            moveEvent => moveEvent.card == this && location.pile == PileLocation.Field),
                        range,
                        PileLocation.Field,
                        optional,
                        TriggeredAbility.Timing.Post
                        );
        }

        public void etbLambda(String description, Effect[] es, int range = -1, bool optional = false)
        {
            addTriggeredAbility(
                        description,
                        es,
                        new Foo(),
                        new TypedGameEventFilter<MoveToPileEvent>(
                            moveEvent => moveEvent.card == this && location.pile == PileLocation.Field),
                        range,
                        PileLocation.Field,
                        optional,
                        TriggeredAbility.Timing.Post
                        );
        }

        public void diesLambda(String description, Effect e, int range = -1, bool optional = false)
        {
            addTriggeredAbility(
                description,
                e,
                new Foo(),
                new TypedGameEventFilter<MoveToPileEvent>(
                    moveEvent =>
                        moveEvent.card == this && moveEvent.to.location.pile == PileLocation.Graveyard &&
                        location.pile == PileLocation.Field),
                range,
                PileLocation.Field,
                optional
                );
        }

        public void deathtouchLambda()
        {
            addTriggeredAbility(
                        "Whenever this creature deals damage to a non-heroic creature destroy it.",
                        new TargetRuleSet(new TriggeredTargetRule<DamageEvent, Card>(de => de.target)),
                        new MoveToPileDoer(PileLocation.Graveyard),
                        new Foo(),
                        new TypedGameEventFilter<DamageEvent>(de => de.source == this && !de.target.isHeroic),
                        0,
                        PileLocation.Field,
                        false
                        );
        }

        #endregion

        #region ModifyFunctions

        public static Func<int, int> add(int i)
        {
            return v => v + i;
        }

        public static Func<int, int> setTo(int i)
        {
            return v => i;
        }

        #endregion

        #region TargetRule

        private static TargetRule resolveCard => new CardResolveRule(CardResolveRule.Rule.ResolveCard);
        public static TargetRule player => new ChooseRule<Card>(c => c.isHeroic);
        public static TargetRule relic => new ChooseRule<Card>(c => c.cardType == CardType.Relic);
        private static TargetRule enemyHeroicCreatures => new CardsRule(c => c.isHeroic && !c.owner.isHero);


        public static TargetRule creature(Func<Card, bool> filter = null)
        {
            filter = filter ?? (c => true);
            return new ChooseRule<Card>(c => c.cardType == CardType.Creature && filter(c));
        }

        public static TargetRule nonheroicCreature(Func<Card, bool> filter = null)
        {
            filter = filter ?? (c => true);
            return new ChooseRule<Card>(c => c.cardType == CardType.Creature && !c.isHeroic && filter(c));
        }

        public static TargetRule nonColouredCreature(ManaColour notAllowed)
        {
            return new ChooseRule<Card>(c => c.cardType == CardType.Creature && !c.isColour(notAllowed));
        }

        #endregion

        #region GameEventFilter

        public static GameEventFilter never { get; } = new StaticGameEventFilter(() => false);
        public static GameEventFilter endOfTurn { get; } = new TypedGameEventFilter<EndOfStepEvent>((e) => e.step == Steps.End);
        public static GameEventFilter clearAura { get; } = new TypedGameEventFilter<ClearAurasEvent>();

        public static GameEventFilter thisEnters(Card c, PileLocation pl)
        {
            return new TypedGameEventFilter<MoveToPileEvent>(
                e => e.card == c && e.to.location.pile == pl);
        }

        private GameEventFilter startOfHerosStep(Steps step)
        {
            return new TypedGameEventFilter<StartOfStepEvent>(e => e.step == step && e.activePlayer == controller);
        }

        #endregion

        #endregion
    }

    class Aura
    {
        public string description { get; }
        public ModifiableStats stat { get; }
        public Func<Card, bool> filter { get; }
        public PileLocation activeIn { get; }

        private Func<int, int> fn;

        public ModifierStruct modifer => new ModifierStruct(fn, Card.clearAura);

        public Aura(string description, Func<int, int> fn, ModifiableStats stat, Func<Card, bool> filter, PileLocation activeIn)
        {
            this.fn = fn;
            this.description = description;
            this.stat = stat;
            this.filter = filter;
            this.activeIn = activeIn;
        }
    }

    enum ModifiableStats { Power, Toughness, Movement };


    public enum CardTemplate
    {
        Count_sFera_sII,
        /*Makaroni,
        Spiderling,
        Arachosa,
        Vincennes,
        Ilatian_sGhoul,
        Hungry_sFelhound,
        Archfather,
        Jabroni,
        Iradj,
        Hosro,
        Warp,
        
        Seblastian,
        Paralyzing_sSpider,*/
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
        Alter_sFate,
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
        Lord_sPlevin,
        Gryphon_sRider,
        Ravaging_sGreed,
        Water_sBolt,
        Lone_sRanger,
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