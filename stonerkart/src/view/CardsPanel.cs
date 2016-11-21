using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace stonerkart
{
    class CardsPanel : UserControl, Observer<PileChangedMessage>
    {
        private List<CardView> cardViews;
        public readonly List<Action<Clickable>> callbacks = new List<Action<Clickable>>();

        public CardsPanel()
        {
            cardViews = new List<CardView>();
            BackColor = Color.Navy;
            DoubleBuffered = true;
            Resize += (_, __) => layoutCards();
        }

        private void clicked(CardView c)
        {
            foreach (var cb in callbacks) cb(c);
        }

        public void setPile(Pile p)
        {
            p.addObserver(this);
            foreach (var v in p) addCardView(v);
        }

        private void addCardView(Card c)
        {
            CardView cv = new CardView(c);
            cardViews.Add(cv);
            cv.MouseDown += (_, __) => clicked(cv);
            this.memeout(() => Controls.Add(cv));
        }

        private void removeCardView(Card c)
        {
            CardView cv = null;
            for (int i = 0; i < cardViews.Count; i++)
            {
                if (cardViews[i].card == c)
                {
                    cv = cardViews[i];
                    cardViews.RemoveAt(i);  //todo i'm pretty sure this straight up breaks
                    break;
                }
            }
            this.memeout(() => Controls.Remove(cv));
        }

        private void layoutCards()
        {
            this.memeout(() =>
            {
                int cards = cardViews.Count;
                if (cards == 0) return;
                int cardHeight = Size.Height - 0;
                int cardWidth = Math.Min((int)(cardHeight*0.773f), (Size.Width - 0)/cards);

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
