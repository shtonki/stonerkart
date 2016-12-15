using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace stonerkart
{
    internal class CardsPanel : UserControl, Observer<PileChangedMessage>
    {
        private List<CardView> cardViews;
        public List<Action<Clickable>> clickedCallbacks { get; } = new List<Action<Clickable>>();
        public List<Action<Clickable>> mouseEnteredCallbacks { get; } = new List<Action<Clickable>>();
        public bool vertical;

        public CardsPanel()
        {
            cardViews = new List<CardView>();
            BackColor = Color.Navy;
            DoubleBuffered = true;
            Resize += (_, __) => layoutCards();
        }

        private void clicked(CardView c)
        {
            foreach (var cb in clickedCallbacks) cb(c);
        }

        private void entered(CardView c)
        {
            foreach (var cb in mouseEnteredCallbacks) cb(c);
        }

        public void setPile(Pile p)
        {
            p.addObserver(this);
            foreach (var v in p) addCardView(v);
        }

        private void addCardView(Card c)
        {
            CardView cv = new CardView(c);
            c.addObserver(cv);
            cardViews.Add(cv);
            cv.MouseDown += (_, __) => clicked(cv);
            cv.MouseEnter += (_, __) => entered(cv);
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
                    cardViews.RemoveAt(i); //todo i'm pretty sure this straight up breaks
                    break;
                }
            }
            this.memeout(() => Controls.Remove(cv));
        }

        private void layoutCards()
        {
            if (vertical)
            {
                layoutVertical();
            }
            else
            {
                layoutHorizontal();
            }
        }

        //todo unhack these

        private void layoutVertical()
        {
            this.memeout(() =>
            {
                int cards = cardViews.Count;
                if (cards == 0) return;
                int cardHeight = Size.Height/cards;
                int cardWidth = Size.Width;

                for (int i = 0; i < cards; i++)
                {
                    cardViews[i].SetBounds(0, i*cardHeight, cardWidth, cardHeight);
                }
            });
        }

        private void layoutHorizontal()
        {
            this.memeout(() =>
            {
                int cards = cardViews.Count;
                if (cards == 0) return;
                int cardHeight = Size.Height - 0;
                int cardWidth = Math.Min((int)(cardHeight * 0.773f), (Size.Width - 0) / cards);

                for (int i = 0; i < cards; i++)
                {
                    cardViews[i].SetBounds(i * cardWidth, 0, cardWidth, cardHeight);
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
