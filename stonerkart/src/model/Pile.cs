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
        public readonly Location location;

        private List<Card> cardList = new List<Card>();

        public int Count => cardList.Count;

        public Card this[int i]
        {
            get { return cardList[i]; }
            //set { InnerList[i] = value; }
        }

        public Pile()
        {
        }

        public Pile(Location location)
        {
            this.location = location;
        }

        public int indexOf(Card c)
        {
            for (int i = 0; i < cardList.Count; i++)
            {
                if (cardList[i] == c) return i;
            }
            return -1;
        }

        public void addTop(Card c)
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

        public void addRange(IEnumerable<Card> cs)
        {
            PileChangedMessage m = new PileChangedMessage(true, cs.ToArray());
            foreach (Card c in cs)
            {
                cardList.Add(c);
            }
            notify(m);
        }

        public Card peek()
        {
            int c = cardList.Count;
            Card r = cardList[c - 1];
            return r;
        }

        public Card peekTop()
        {
            return cardList[cardList.Count - 1];
        }

        public void remove(Card c)
        {
            if (!cardList.Contains(c)) throw new Exception();
            cardList.Remove(c);
            notify(new PileChangedMessage(false, c));
        }

        public Card removeTop()
        {
            Card r = cardList[cardList.Count - 1];
            cardList.RemoveAt(cardList.Count - 1);
            notify(new PileChangedMessage(false, r));
            return r;
        }

        public void clear()
        {
            PileChangedMessage m = new PileChangedMessage(false, cardList.ToArray());
            cardList.Clear();
            notify(m);
        }

        public void shuffle(Random rng)
        {
            int n = cardList.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                Card c = cardList[k];
                cardList[k] = cardList[n];
                cardList[n] = c;
            }
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

    enum PileLocation
    {
        Nowhere,
        Hand,
        Field,
        Stack,
        Deck,
        Graveyard,
        Displaced,
    }

    struct Location
    {
        public readonly PileLocation pile;
        public readonly Player player;

        public Location(Player player, PileLocation pile)
        {
            this.pile = pile;
            this.player = player;
        }
    }

    struct PileChangedMessage
    {
        public readonly bool added;
        public readonly Card[] cards;

        public PileChangedMessage(bool added, Card card) : this(added, new[] { card})
        {
        }

        public PileChangedMessage(bool added, Card[] cards) : this()
        {
            this.added = added;
            this.cards = cards;
        }

    }

}
