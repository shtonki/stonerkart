using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stonerkart
{
    class Pile : Observable<PileChangedMessage>
    {
        public IEnumerable<Card> cards => cardList;

        private List<Card> cardList;

        public Pile()
        {
            cardList = new List<Card>();
        }

        public void add(Card c)
        {
            if (!cardList.Contains(c))
            {
                cardList.Add(c);
                notify(new PileChangedMessage(true, c));
            }

        }

        public bool remove(Card c)
        {
            if (!cardList.Contains(c)) return false;
            cardList.Remove(c);
            notify(new PileChangedMessage(false, c));
            return true;
        }
    }

    struct PileChangedMessage
    {
        public readonly bool added;
        public readonly Card card;

        public PileChangedMessage(bool added, Card card)
        {
            this.added = added;
            this.card = card;
        }
    }

}
