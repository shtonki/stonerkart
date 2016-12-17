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
        public CardType cardType { get; }
        public Rarity rarity { get; }
        public string breadText => breadTextEx();

        public Location location => locationEx();

        public ActivatedAbility castAbility { get; }
        public int castRange => castAbility.castRange;
        public ManaSet castManaCost => castAbility.cost.getSubCost<ManaCost>().cost;

        public string typeText => typeTextEx();

        public IEnumerable<Ability> abilities => activatedAbilities;
        private List<ActivatedAbility> activatedAbilities = new List<ActivatedAbility>();
        private List<TriggeredAbility> triggeredAbilities = new List<TriggeredAbility>();
        public ActivatedAbility[] usableHere => activatedAbilities.Where(a => a.activeIn == location.pile).Cast<ActivatedAbility>().ToArray();
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

        public void exhaust()
        {
            movement.modify(-movement, ModifiableSchmoo.intAdd, ModifiableSchmoo.startOfOwnersTurn(this));
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
                l = triggeredAbilities.Cast<Ability>();
            }
            else if (c == _ACTIVATED)
            {
                l = activatedAbilities.Cast<Ability>();
            }
            else throw new Exception();

            return l.ToArray()[i];
        }

        public TriggeredAbility[] abilitiesTriggeredBy(GameEvent e)
        {
            return triggeredAbilities.Where(a => a.triggeredBy(e)).ToArray();
        }

        public void remodify(GameEvent e)
        {
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
    }

    enum CardTemplate
    {
        Belwas,
        Zap,
        Kappa,
        Cantrip,
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
}
