using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stonerkart
{
    class Deck
    {
        public CardTemplate heroic;
        public CardTemplate[] deck;

        public Deck(CardTemplate heroic, CardTemplate[] deck)
        {
            this.heroic = heroic;
            this.deck = deck;
        }
    }
}
