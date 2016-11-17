using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stonerkart
{
    class Pile : Observable<PileChangedMessage>, IEnumerable<Card>
    {
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
            else
            {
                throw new Exception();
            }

        }

        public Card draw()
        {
            int c = cardList.Count;
            if (c == 0) throw new Exception();
            Card r = cardList[c - 1];
            cardList.RemoveAt(c - 1);
            return r;
        }

        public void remove(Card c)
        {
            if (!cardList.Contains(c)) throw new Exception();
            cardList.Remove(c);
            notify(new PileChangedMessage(false, c));
        }

        public IEnumerator<Card> GetEnumerator()
        {
            return cardList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
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
