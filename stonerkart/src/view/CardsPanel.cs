using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace stonerkart
{
    class CardsPanel : UserControl
    {
        private List<CardView> cardViews;
        public CardsPanel()
        {
            cardViews = new List<CardView>();

            addCardView(new CardView());
            addCardView(new CardView());
            addCardView(new CardView());
            
            BackColor = Color.Navy;
            DoubleBuffered = true;
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            layoutCards();
        }

        private void addCardView(CardView cv)
        {
            cardViews.Add(cv);
            Controls.Add(cv);
            layoutCards();
        }

        private void layoutCards()
        {
            int cards = cardViews.Count;
            int cardWidth = (Size.Width - 0)/cards;
            int cardHeight = Size.Height - 0;

            for (int i = 0; i < cards; i++)
            {
                cardViews[i].SetBounds(i*cardWidth, 0, cardWidth, cardHeight);
            }
        }
    }
}
