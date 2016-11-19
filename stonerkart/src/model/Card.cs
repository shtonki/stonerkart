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
        public List<Tile> path { get; set; }

        public readonly Modifiable<int> power;
        public readonly Modifiable<int> toughness;
        public readonly Modifiable<int> movement;

        private Modifiable[] ms;

        public Card(CardTemplate ct)
        {
            name = ct.ToString();

            int basePower;
            int baseToughness;
            int baseMovement;

            #region oophell
            switch (ct)
            {
                case CardTemplate.Hero:
                {
                    image = Properties.Resources.pepeman;
                    baseMovement = 1;
                    basePower = 1;
                    baseToughness = 10;
                } break;

                case CardTemplate.Jordan:
                {
                    image = Properties.Resources.jordanno;
                    baseMovement = 2;
                    basePower = 1;
                    baseToughness = 2;
                } break;

                default:
                    throw new Exception();
            }

            #endregion

            power = new Modifiable<int>(basePower);
            toughness = new Modifiable<int>(baseToughness);
            movement = new Modifiable<int>(baseMovement);

            ms = new Modifiable[]
            {
                power,
                toughness,
                movement,
            };
        }

        public void moveTo(Pile p)
        {
            pile?.remove(this);
            pile = p;
            p.add(this);
        }

        public void reherp(GameEvent e)
        {
            foreach (var v in ms)
            {
                v.check(e);
            }
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
