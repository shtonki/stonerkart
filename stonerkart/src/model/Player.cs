using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stonerkart
{
    class Player
    {
        public readonly Card heroCard;
        public readonly Game game;

        public readonly Pile deck;
        public readonly Pile field;
        public readonly Pile hand;

        public bool isHero => this == game.hero;

        public Player(Game g, CardTemplate hc)
        {
            game = g;
            deck = new Pile();
            field = new Pile();
            hand = new Pile();

            heroCard = new Card(hc, this);
            field.add(heroCard);

            deck.add(new Card(CardTemplate.Jordan, this));
            deck.add(new Card(CardTemplate.Jordan, this));
            deck.add(new Card(CardTemplate.Jordan, this));
            deck.add(new Card(CardTemplate.Jordan, this));
            deck.add(new Card(CardTemplate.Jordan, this));
            deck.add(new Card(CardTemplate.Jordan, this));
            deck.add(new Card(CardTemplate.Jordan, this));
            deck.add(new Card(CardTemplate.Jordan, this));
            deck.add(new Card(CardTemplate.Jordan, this));
            deck.add(new Card(CardTemplate.Jordan, this));
            deck.add(new Card(CardTemplate.Jordan, this));
            deck.add(new Card(CardTemplate.Jordan, this));
        }
    }
}
