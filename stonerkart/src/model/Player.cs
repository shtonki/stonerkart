using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stonerkart
{
    class Player : Targetable
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

            deck = new Pile(new Location(this, PileLocation.Deck));
            field = new Pile(new Location(this, PileLocation.Field));
            hand = new Pile(new Location(this, PileLocation.Hand));

            heroCard = new Card(hc, this);
            field.addTop(heroCard);

            deck.addTop(new Card(CardTemplate.Zap, this));
            deck.addTop(new Card(CardTemplate.Zap, this));
            deck.addTop(new Card(CardTemplate.Zap, this));
            deck.addTop(new Card(CardTemplate.Zap, this));
            deck.addTop(new Card(CardTemplate.Zap, this));
            deck.addTop(new Card(CardTemplate.Zap, this));
            deck.addTop(new Card(CardTemplate.Zap, this));
            deck.addTop(new Card(CardTemplate.Zap, this));
            deck.addTop(new Card(CardTemplate.Zap, this));
            deck.addTop(new Card(CardTemplate.Zap, this));
            deck.addTop(new Card(CardTemplate.Zap, this));
            deck.addTop(new Card(CardTemplate.Zap, this));
        }
    }
}
