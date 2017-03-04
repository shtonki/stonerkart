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
        public Tile tile { get; set; }
        public Player owner { get; }
        public Player controller { get; }
        public CardTemplate template { get; }
        public bool isDummy { get; private set; }
        public CardType cardType { get; }
        public Rarity rarity { get; }
        public CardSet set { get; }
        public Race? race { get; }
        public Subtype? subtype { get; }
        public int convertedManaCost => castManaCost.colours.Count();

        public bool isExhausted => Movement == 0;
        public bool canRetaliate => !isExhausted && !isHeroic;

        public string breadText => breadTextEx();

        public Location location => locationEx();

        public ActivatedAbility castAbility { get; }
        public int castRange => castAbility.castRange;
        public ManaSet castManaCost { get; }

        public string typeText => typeTextEx();

        public Card dummyFor;

        public IEnumerable<ActivatedAbility> activatedAbilities
            => abilities.Where(a => a is ActivatedAbility).Cast<ActivatedAbility>();

        public IEnumerable<TriggeredAbility> triggeredAbilities
            => abilities.Where(a => a is TriggeredAbility).Cast<TriggeredAbility>();

        private List<Ability> abilities = new List<Ability>();
        public ActivatedAbility[] usableHere => activatedAbilities.Where(a => a.activeIn == location.pile).ToArray();

        public List<ManaColour> colours => coloursEx();

        public bool isHeroic { get; }

        public bool hasPT => cardType == CardType.Creature;

        public int power => Power;
        public int toughness => Toughness - damageTaken;
        public int movement => Movement - fatigue;

        private Modifiable Power;
        private Modifiable Toughness;
        private Modifiable Movement;

        private int damageTaken;
        private int fatigue;

        private ManaColour? forceColour;
        private Modifiable[] modifiables;

        private GameEventHandlerBuckets eventHandler;

        public bool canAttack(Card defender)
        {
            if (defender.cardType != CardType.Creature) return false;

            return true;
        }

        public bool canDamage(Card defender)
        {
            return true;
        }

        public bool canTarget(Card target)
        {
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

        public void exhaust(int steps = -1)
        {
            fatigue += steps < 0 ? Movement : steps;
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
            if (hs.Count == 0) return new List<ManaColour>(new[] {ManaColour.Colourless});

            return hs.ToList();
        }

        public bool isCastAbility(Ability a)
        {
            return castAbility == a;
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
            shitterhack.handle(e);
            if (!(e is CardedEvent && ((CardedEvent)e).getCard() == this)) return;

            eventHandler.handle(e);

            foreach (Modifiable modifiable in modifiables)
            {
                modifiable.check(e);
            }
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
            string s = isHeroic ? "Heroic " : "";
            s += cardType.ToString();

            return s;
        }

        private string breadTextEx()
        {
            StringBuilder sb = new StringBuilder();
            foreach (Ability a in abilities)
            {
                sb.Append(a.description);
                sb.Append("\r\n");
            }
            return sb.ToString();
        }

        public Card clone()
        {
            Card r = new Card(template, owner);
            r.dummyFor = this;
            r.isDummy = true;
            return r;
        }

        private Location locationEx()
        {
            if (pile == null) throw new Exception();
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

        private static Card[] flyweight =
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
                        a =>
                        {
                            int v = 2;
                            return a.step == Steps.Replenish && a.activePlayer == controller;
                        }), e => fatigue = 0);


            r.add(new TypedGameEventHandler<ApplyModifierEvent>(e =>
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
                moveTo(e.card.controller.field);
                exhaust();
            }));

            r.add(new TypedGameEventHandler<MoveEvent>(e =>
            {
                Path path = e.path;
                Tile destination = path.to;
                moveTo(destination);
                exhaust(path.attacking ? movement : path.length);
            }));

            r.add(new TypedGameEventHandler<DamageEvent>(e =>
            {
                if (e.source.canDamage(e.target))
                e.target.damage(e.amount);
            }));

            r.add(new TypedGameEventHandler<MoveToPileEvent>(e =>
            {
                if (e.nullTile && tile != null)
                {
                    tile.removeCard();
                    tile = null;
                }
                moveTo(e.to);
            }));
            
            return r;
        }
    }

    enum ModifiableStats { Power, Toughness, Movement };


    enum CardTemplate
    {
        missingno,
        One_sWith_sNature,
        Graverobber_sSyrdin,
        Alter_sFate,
        Goblin_sGrenade,
        Cleansing_sFire,
        Belwas,
        Zap,
        Kappa,
        Cantrip,
        Temple_sHealer,
        Yung_sLich,
        Nature_sHeroman,
        Risen_sAbberation,
        Shibby_sShtank,
        Unmake,
        Rockhand_sOgre,
        Bear_sCavalary,
        Illegal_sGoblin_sLaboratory,
        Teleport
    }

    internal enum CardType
    {
        Creature,
        Instant,
        Sorcery,
        Relic
    }

    internal enum Rarity
    {
        Common,
        Uncommon,
        Rare,
        Legendary
    }

    internal enum CastSpeed
    {
        Instant,
        Slow
    }

    internal enum CardSet
    {
        FirstEdition
    }

    internal enum Race
    {
        Human,
        Undead,
        Lizard,
        Goblin
    }

    internal enum Subtype
    {
        Warrior,
        Wizard,
        Cleric
    }
}