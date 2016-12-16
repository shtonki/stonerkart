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

        public bool vertical { get; set; }

        public CardsPanel()
        {
            cardViews = new List<CardView>();
            BackColor = Color.Navy;
            DoubleBuffered = true;
            Resize += (_, __) => layoutCards();
            MouseMove += xd;
            //Capture = true;

        }

        private void clicked(CardView c)
        {
            foreach (var cb in clickedCallbacks) cb(c);
        }

        private void entered(CardView c)
        {
            foreach (var cb in mouseEnteredCallbacks) cb(c);
        }

        private CardView frontCard;

        private void xd(object a, MouseEventArgs e)
        {
            if (!vertical) return;

            Point v = this.PointToClient(Control.MousePosition);
            int cardIndexUnderMouse = v.Y / actualPad;
            CardView cardUnderMouse;

            if (cardIndexUnderMouse >= cardViews.Count)
            {
                cardUnderMouse = null;
            }
            else
            {
                cardUnderMouse = cardViews[cardIndexUnderMouse];
            }

            if (frontCard == cardUnderMouse) return;

            foreach (CardView cv in cardViews)
            {
                cv.BringToFront();
            }

            frontCard = cardUnderMouse;
            frontCard?.BringToFront();
        }

        public void setPile(Pile p)
        {
            p.addObserver(this);
            foreach (var v in p) addCardView(v);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            if (!(ClientRectangle.Contains(PointToClient(Control.MousePosition))))
            {
                foreach (CardView cv in cardViews)
                {
                    cv.BringToFront();
                }
            }
        }

        private void addCardView(Card c)
        {
            CardView cv = new CardView(c);
            c.addObserver(cv);
            cardViews.Add(cv);
            cv.MouseDown += (_, __) => clicked(cv);
            cv.MouseEnter += (_, __) => entered(cv);
            cv.MouseMove += xd;
            cv.MouseLeave += (a, b) => OnMouseLeave(b);
            
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


        private const float HtoWratio = 1.57f;
        private const int minPad = 20;
        private int actualPad;

        private void layoutVertical()
        {
            this.memeout(() =>
            {
                actualPad = minPad;
                int cards = cardViews.Count;
                if (cards == 0) return;
                int cardWidth = Size.Width;
                int cardHeight = (int)(HtoWratio * cardWidth);

                if (cardHeight + actualPad * cards > Size.Height)
                {
                    cardHeight = Size.Height - actualPad * cards;
                }

                for (int i = 0; i < cards; i++)
                {
                    cardViews[i].SetBounds(0, i* actualPad, cardWidth, cardHeight);
                    cardViews[i].BringToFront();
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
