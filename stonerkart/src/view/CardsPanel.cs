using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace stonerkart
{
    class CardsPanel : UserControl, Observer<PileChangedMessage>
    {
        private List<CardView> cardViews;
        public CardsPanel()
        {
            cardViews = new List<CardView>();
            BackColor = Color.Navy;
            DoubleBuffered = true;
            Resize += (_, __) => layoutCards(); //todo unhack
        }

        public void setPile(Pile p)
        {
            p.addObserver(this);
            foreach (var v in p.cards) addCardView(v);
        }

        private void addCardView(Card c)
        {
            CardView cv = new CardView(c);
            cardViews.Add(cv);
            this.memeout(() => Controls.Add(cv), true);
        }

        private void removeCardView(Card c)
        {
            CardView cv = new CardView(c);
            cardViews.Remove(cv);
            this.memeout(() => Controls.Remove(cv));
        }

        private void layoutCards()
        {
            this.memeout(() =>
            {
                int cards = cardViews.Count;
                if (cards == 0) return;
                int cardWidth = Math.Min(Size.Width/5, (Size.Width - 0)/cards);
                int cardHeight = Size.Height - 0;

                for (int i = 0; i < cards; i++)
                {
                    cardViews[i].SetBounds(i*cardWidth, 0, cardWidth, cardHeight);
                }
            });
        }

        public void notify(PileChangedMessage t)
        {
            if (t.added)
            {
                addCardView(t.card);
            }
            else
            {
                removeCardView(t.card);
            }
            layoutCards();
        }
    }
}
