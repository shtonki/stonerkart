using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stonerkart
{
    class Card : Observable<CardChangedMessage>, Stuff
    {
        public string name { get; }
        public Image image { get; }
        public Pile pile { get; private set; }
        public Tile tile { get; set; }

        public Tile path;

        public int power;
        public int toughness;
        public int baseMovement;
        public int movement;

        public Card(CardTemplate ct)
        {
            name = ct.ToString();
            power = toughness = 1;
            baseMovement = 2;

            switch (ct)
            {
                case CardTemplate.Hero:
                {
                    image = Properties.Resources.pepeman;
                    baseMovement = 1;
                } break;

                case CardTemplate.Jordan:
                {
                    image = Properties.Resources.jordanno;
                    baseMovement = 2;
                } break;
            }
        }

        public void moveTo(Pile p)
        {
            pile?.remove(this);
            pile = p;
            p.add(this);
        }

        public void walkTo(Tile t)
        {
            
        }
    }

    enum CardTemplate
    {
        Hero,
        Jordan,
    }

    enum CardType
    {
        Hero,
        Creature,
    }
}
