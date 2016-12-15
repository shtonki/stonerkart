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

        public readonly Pile deck;
        public readonly Pile field;
        public readonly Pile hand;
        public readonly Pile graveyard;

        public ManaPool manaPool;

        public bool isHero => this == game.hero;

        public Player(Game g)
        {
            game = g;

            deck = new Pile(new Location(this, PileLocation.Deck));
            field = new Pile(new Location(this, PileLocation.Field));
            hand = new Pile(new Location(this, PileLocation.Hand));
            graveyard = new Pile(new Location(this, PileLocation.Hand));


            manaPool = new ManaPool();
        }

        public void setHeroCard(Card hc)
        {
            if (heroCard != null) throw new Exception();
            heroCard = hc;
            field.addTop(heroCard);
        }

        public void loadDeck(IEnumerable<Card> cards)
        {
            foreach (Card c in cards)
            {
                deck.addTop(c);
            }
        }

        public void resetMana()
        {
            manaPool.reset();
            notify(new PlayerChangedArgs(this));
        }

        public ManaSet stuntMana()
        {
            ManaSet r = manaPool.max;
            int[] s = r.Select(v => v == 6 ? 6 : v + 1).ToArray();
            manaPool.max = new ManaSet(s);
            notify(new PlayerChangedArgs(this));
            return r;
        }

        public void unstuntMana(ManaSet r)
        {
            manaPool.max = r;
        }

        public void gainMana(ManaColour c)
        {
            manaPool.max[c]++;
            manaPool.current[c]++;
            notify(new PlayerChangedArgs(this));
        }

        public void payMana(ManaSet iz)
        {
            manaPool.subtract(iz);
            notify(new PlayerChangedArgs(this));
        }
    }
}
