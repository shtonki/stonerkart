using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stonerkart
{
    class Player : Observable<PlayerChangedArgs>, Targetable
    {
        public Card heroCard { get; private set; }
        public readonly Game game;

        public Pile deck { get; }
        public Pile field { get; }
        public Pile hand { get; }
        public Pile graveyard { get; }

        public Pile displaced { get; }

        public ManaPool manaPool { get; private set; }

        public bool isHero => this == game.hero;

        public Player(Game g)
        {
            game = g;

            deck = new Pile(new Location(this, PileLocation.Deck));
            field = new Pile(new Location(this, PileLocation.Field));
            hand = new Pile(new Location(this, PileLocation.Hand));
            graveyard = new Pile(new Location(this, PileLocation.Graveyard));
            displaced = new Pile(new Location(this, PileLocation.Displaced));

            manaPool = new ManaPool();
        }

        public void setHeroCard(Card hc)
        {
            if (heroCard != null) throw new Exception();
            heroCard = hc;
        }

        public void loadDeck(IEnumerable<Card> cards)
        {
            foreach (Card c in cards)
            {
                deck.addTop(c);
            }
        }

        public Pile pileFrom(PileLocation l)
        {
            switch (l)
            {
                case PileLocation.Deck:
                    return deck;

                case PileLocation.Hand:
                    return hand;

                case PileLocation.Displaced:
                    return displaced;

                case PileLocation.Field:
                    return field;

                case PileLocation.Graveyard:
                    return graveyard;

                default:
                    throw new Exception();
            }
        }

        public void resetMana()
        {
            manaPool.reset();
            notify(new PlayerChangedArgs(this));
        }

        private ManaPool original;
        public void stuntMana(ManaPool newPool)
        {
            fixOriginal();

            manaPool = newPool;

            notify(new PlayerChangedArgs(this));
        }

        public void stuntCurrentDiff(ManaColour mc, int d)
        {
            fixOriginal();

            manaPool.current[mc] += d;

            notify(new PlayerChangedArgs(this));
        }

        public void stuntCurrentLoss(ManaSet set)
        {
            fixOriginal();

            manaPool.subtractCurrent(set);

            notify(new PlayerChangedArgs(this));
        }

        public void unstuntMana()
        {
            if (original == null) throw new Exception();
            manaPool = original;
            original = null;

            notify(new PlayerChangedArgs(this));
        }

        private void fixOriginal()
        {
            if (original == null)
            {
                original = manaPool.clone();
                manaPool = manaPool.clone();
            }
        }

        public void gainMana(ManaColour c)
        {
            if (original != null) throw new Exception();

            manaPool.max[c]++;
            manaPool.current[c]++;

            notify(new PlayerChangedArgs(this));
        }

        public void payMana(ManaSet iz)
        {
            if (original != null) throw new Exception();

            manaPool.subtractCurrent(iz);
            notify(new PlayerChangedArgs(this));
        }

        public void setActive(bool b)
        {
            notify(new PlayerChangedArgs(b));
        }
    }
}
