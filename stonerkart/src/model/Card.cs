using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public IEnumerable<ActivatedAbility> activatedAbilities => abilities.Where(a => a is ActivatedAbility).Cast<ActivatedAbility>();
        public IEnumerable<TriggeredAbility> triggeredAbilities => abilities.Where(a => a is TriggeredAbility).Cast<TriggeredAbility>();
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


        public bool canAttack(Card defender)
        {
            if (defender.cardType != CardType.Creature) return false;

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

        public void dealDamage(int d)
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
            if (forceColour.HasValue) return new List<ManaColour>(new ManaColour[] { forceColour.Value });
            HashSet<ManaColour> hs = new HashSet<ManaColour>(castManaCost.colours.Where(x => x != ManaColour.Colourless));
            if (hs.Count == 0) return new List<ManaColour>(new ManaColour[] {ManaColour.Colourless,});
            
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
            GameEventFilter sickhack = new TypedGameEventFilter<StartOfStepEvent>(a => a.step == Steps.Replenish && a.activePlayer == controller);
            if (sickhack.filter(e)) fatigue = 0;

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

        private static Card[] flyweight = Enum.GetValues(typeof (CardTemplate)).Cast<CardTemplate>().Select(ct => new Card(ct)).ToArray();
        public static Card fromTemplate(CardTemplate ct)
        {
            return flyweight[(int)ct];
        }
    }

    enum CardTemplate
    {
        missingno,
        One_With_Nature,
        Graverobber_Syrdin,
        Alter_Fate,
        Goblin_Grenade,
        Cleansing_Fire,
        Belwas,
        Zap,
        Kappa,
        Cantrip,
        Temple_Healer,
        Yung_Lich,
        Nature_Heroman,
        Risen_Abberation,
        Shibby_Shtank,
        Unmake,
        Rockhand_Ogre,
        Bear_Cavalary,
        Illegal_Goblin_Laboratory,
        Teleport,
    }

    enum CardType
    {
        Creature,
        Instant,
        Sorcery,
        Relic,
    }

    enum Rarity
    {
        Common,
        Uncommon,
        Rare,
        Legendary,
    }

    enum CastSpeed
    {
        Instant,
        Slow,
    }

    enum CardSet
    {
        FirstEdition,
    }

    enum Race
    {
        Human,
        Undead,
        Lizard,
        Goblin,
    }

    enum Subtype
    {
        Warrior,
        Wizard,
        Cleric,
    }
}
