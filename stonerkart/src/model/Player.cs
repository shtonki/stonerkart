using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stonerkart
{
    class Player : Observable<PlayerChangedArgs>, Observer<PileChangedMessage>, Targetable
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

            deck.addObserver(this);
            field.addObserver(this);
            hand.addObserver(this);
            graveyard.addObserver(this);
            displaced.addObserver(this);

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

        public int stateCtr()
        {
            return 0;
        }

        #region mana voodoo
        public void resetMana()
        {
            manaPool.reset();
            notify(new PlayerChangedArgs(this));
        }

        public void gainMana(ManaColour c)
        {
            manaPool.gainMana(c);

            notify(new PlayerChangedArgs(this));
        }

        public void payMana(ManaSet iz)
        {
            manaPool.subtractCurrent(iz);
            notify(new PlayerChangedArgs(this));
        }

        public void gainBonusMana(ManaColour c)
        {
            manaPool.addBonusMana(c);
            notify(new PlayerChangedArgs(this));
        }

        public void clearBonusMana()
        {
            manaPool.resetBonus();
            notify(new PlayerChangedArgs(this));
        }

        public bool stunthack;
        public ManaSet stunthackset;
        public void stuntMana()
        {
            stunthack = true;
            notify(new PlayerChangedArgs(this));
        }

        public void unstuntMana()
        {
            stunthack = false;
            stunthackset = null;
            notify(new PlayerChangedArgs(this));
        }

        public void stuntLoss(ManaSet l)
        {
            stunthackset = l;
            notify(new PlayerChangedArgs(this));
        }

        #endregion

        public void notify(object o, PileChangedMessage t)
        {
            Pile p = (Pile)o;
            notify(new PlayerChangedArgs(p.location.pile));
        }
    }
}
