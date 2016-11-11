using System.Collections.Generic;
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
        }

        private void addCardView(CardView cv)
        {
            cardViews.Add(cv);
            Controls.Add(cv);
        }
    }
}
