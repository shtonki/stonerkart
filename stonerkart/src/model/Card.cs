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
        public int convertedManaCost => castManaCost.orbs.Count();

        public string breadText => breadTextEx();

        public Location location => locationEx();

        public ActivatedAbility castAbility { get; }
        public int castRange => castAbility.castRange;
        public ManaSet castManaCost => castAbility.cost.getSubCost<ManaCost>().cost;

        public string typeText => typeTextEx();

        public IEnumerable<Ability> abilities => activatedAbilities.Cast<Ability>().Concat(triggeredAbilities);
        private List<ActivatedAbility> activatedAbilities = new List<ActivatedAbility>();
        private List<TriggeredAbility> triggeredAbilities = new List<TriggeredAbility>();
        public ActivatedAbility[] usableHere => activatedAbilities.Where(a => a.activeIn == location.pile).ToArray();
        /// <summary>
        /// Returns a list containing the unique colours of the card. If the card has no mana cost it returns an 
        /// array containing only ManaColour.Colourless.
        /// </summary>
        public List<ManaColour> colours => coloursEx();


        public bool isHeroic { get; }

        public bool hasPT => cardType == CardType.Creature;
        public Modifiable<int> power { get; }
        public Modifiable<int> toughness { get; }
        public Modifiable<int> movement { get; }

        private ManaColour? forceColour;
        private Modifiable[] modifiables;

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
            int v = steps < 0 ? movement : steps;
            movement.modify(-v, ModifiableSchmoo.intAdd, ModifiableSchmoo.startOfOwnersTurn(this));
        }

        private List<ManaColour> coloursEx()
        {
            if (forceColour.HasValue) return new List<ManaColour>(new ManaColour[] { forceColour.Value });
            HashSet<ManaColour> hs = new HashSet<ManaColour>(castManaCost.orbs.Where(x => x != ManaColour.Colourless));
            if (hs.Count == 0) return new List<ManaColour>(new ManaColour[] {ManaColour.Colourless,});
            
            return hs.ToList();
        }


        private const int _ACTIVATED = 0x100;
        private const int _TRIGGERED = 0x101;
        public int abilityOrd(Ability a)
        {
            int c, i;
            if (a is ActivatedAbility)
            {
                ActivatedAbility ab = (ActivatedAbility)a;
                i = activatedAbilities.IndexOf(ab);
                if (i == -1) throw new Exception();
                c = _ACTIVATED;
            }
            else if (a is TriggeredAbility)
            {
                TriggeredAbility ab = (TriggeredAbility)a;
                i = triggeredAbilities.IndexOf(ab);
                if (i == -1) throw new Exception();
                c = _ACTIVATED;
            }
            else
            {
                throw new Exception();
            }
            return c | i;
        }

        public Ability abilityFromOrd(int v)
        {
            int c, i;

            i = v & 0x00FF;
            c = v & 0xFF00;

            IEnumerable<Ability> l;
            if (c == _TRIGGERED)
            {
                return triggeredAbilities[i];
            }
            else if (c == _ACTIVATED)
            {
                return activatedAbilities[i];
            }
            else throw new Exception();

        }

        public IEnumerable<TriggeredAbility> abilitiesTriggeredBy(GameEvent e)
        {
            return triggeredAbilities.Where(a => a.triggeredBy(e));
        }

        public void remodify(GameEvent e)
        {
            if (template == CardTemplate.Shibby_Shtank && e is EndOfStepEvent)
            {
                EndOfStepEvent x = (EndOfStepEvent)e;
                int i = 0;
            }
            foreach (Modifiable modifiable in modifiables)
            {
                modifiable.check(e);
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

        private Location locationEx()
        {
            if (pile == null) throw new Exception();
            return pile.location;
        }

        public void notify(int t)
        {
            notify(new CardChangedMessage());
        }

        public override string ToString()
        {
            return name;
        }

        private static Card[] flyweight = Enum.GetValues(typeof (CardTemplate)).Cast<CardTemplate>().Select(ct => (Card)null).ToArray();
        public static Card fromTemplate(CardTemplate ct)
        {
            int ix = (int)ct;
            if (flyweight[ix] == null)
            {
                Card c = new Card(ct);
                flyweight[ix] = c;
            }
            return flyweight[ix];
        }
    }

    enum CardTemplate
    {
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
        Frothing_Goblin,
        Bear_Cavalary,
    }

    enum CardType
    {
        Creature,
        Instant,
        Sorcery,
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
