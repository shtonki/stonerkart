using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stonerkart
{
    class Player
    {
        public readonly Pile deck;
        public readonly Pile field;
        public readonly Pile hand;

        public Player()
        {
            deck = new Pile();
            field = new Pile();
            hand = new Pile();

            deck.add(new Card());
            deck.add(new Card());
            deck.add(new Card());
            deck.add(new Card());
            deck.add(new Card());
            deck.add(new Card());
            deck.add(new Card());
            deck.add(new Card());
            deck.add(new Card());
            deck.add(new Card());
            deck.add(new Card());
            deck.add(new Card());
            deck.add(new Card());
        }
    }
}
