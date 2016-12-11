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

        public ManaPool manaPool;

        public bool isHero => this == game.hero;

        public Player(Game g)
        {
            game = g;

            deck = new Pile(new Location(this, PileLocation.Deck));
            field = new Pile(new Location(this, PileLocation.Field));
            hand = new Pile(new Location(this, PileLocation.Hand));
            

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

        public int[] stuntMana()
        {
            int[] r = manaPool.max;
            int[] s = r.Select(v => v == 6 ? 6 : v + 1).ToArray();
            manaPool.max = s;
            notify(new PlayerChangedArgs(this));
            return r;
        }

        public void unstuntMana(int[] r)
        {
            manaPool.max = r;
        }

        public void gainMana(ManaColour c)
        {
            manaPool.max[(int)c]++;
            manaPool.current[(int)c]++;
            notify(new PlayerChangedArgs(this));
        }

        public void payMana(int[] iz)
        {
            manaPool.subtract(iz);
            notify(new PlayerChangedArgs(this));
        }
    }

    class ManaPool
    {
        public int[] max = new int[6];
        public int[] current = new int[6];

        public void reset()
        {
            for (int i = 0; i < 6; i++)
            {
                current[i] = max[i];
            }
        }

        public void subtract(int[] costs)
        {
            for (int i = 0; i < 6; i++)
            {
                current[i] -= costs[i];
            }
        }
    }
}
