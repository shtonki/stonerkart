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

        private List<Card> cardList;

        public int Count => cardList.Count;

        public Pile(Location location)
        {
            cardList = new List<Card>();
            this.location = location;
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

        public Card removeTop()
        {
            int c = cardList.Count;
            Card r = cardList[c - 1];
            cardList.RemoveAt(c - 1);
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
        Hand,
        Field,
        Stack,
        Deck,
        Graveyard,
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
        public readonly Card card;

        public PileChangedMessage(bool added, Card card)
        {
            this.added = added;
            this.card = card;
        }
    }

}
