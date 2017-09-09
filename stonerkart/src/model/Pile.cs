using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stonerkart
{
    class Pile : CardList
    {
        public Location location { get; }

        public Pile(Location location)
        {
            this.location = location;
        }
    }

    class CardList : Observable<PileChangedMessage>, IEnumerable<Card>
    {
        private List<Card> cardList = new List<Card>();
        public IEnumerable<Card> cards => cardList;

        public int Count => cardList.Count;

        public Card this[int i]
        {
            get { return cardList[i]; }
            //set { InnerList[i] = value; }
        }

        public CardList()
        {
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
                notify(new PileChangedMessage(PileChangedArg.Add, c));
            }
            else
            {
                throw new Exception();
            }

        }

        public void addRange(IEnumerable<Card> cs)
        {
            PileChangedMessage m = new PileChangedMessage(PileChangedArg.Add, cs);
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
            notify(new PileChangedMessage(PileChangedArg.Remove, c));
        }

        public void removeWhere(Predicate<Card> f)
        {
            cardList.RemoveAll(f);
        }

        public Card removeTop()
        {
            Card r = cardList[cardList.Count - 1];
            cardList.RemoveAt(cardList.Count - 1);
            notify(new PileChangedMessage(PileChangedArg.Remove, r));
            return r;
        }

        public void clear()
        {
            PileChangedMessage m = new PileChangedMessage(PileChangedArg.Remove, cards.ToArray());
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

    class PileChangedMessage
    {
        public PileChangedArg arg;
        public Card[] cards;

        public PileChangedMessage(PileChangedArg arg, IEnumerable<Card> cards)
        {
            this.arg = arg;
            this.cards = cards.ToArray();
        }

        public PileChangedMessage(PileChangedArg arg, Card card) : this (arg, new []{card})
        {
        }
    }

    enum PileChangedArg { Add, Remove, }

}
