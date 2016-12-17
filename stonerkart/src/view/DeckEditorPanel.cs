using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace stonerkart.src.view
{

    class DeckEditorPanel : StickyPanel, Screen
    {
        private CardView cardView1;
        private CardView cardView2;
        private CardView cardView3;
        private CardView cardView5;
        private CardView cardView6;
        private CardView cardView8;
        private CardView cardView9;
        private CardView cardView10;
        private TextBox searchBox;
        private CardsPanel cardsPanel1;
        private Panel manaPanel;
        private CardView[] cardViews;
        private ManaButton[] manaButtons;
        private CardView cardView4;
        private CardView cardView7;
        private List<Card> allCards;
        private List<CardTemplate> myCurrentDeck;

        private CardTemplate[] filteredCards;
        private CardTemplate?[] shownCards;
        private int page;
        private Pile deck;

        public DeckEditorPanel()
        {
            InitializeComponent();
            myCurrentDeck = new List<CardTemplate>();
            var cards = Enum.GetValues(typeof(CardTemplate)).Cast<CardTemplate>();
            allCards = new List<Card>();
            foreach (var c in cards)
            {
                allCards.Add(new Card(c, null));
            }


            manaButtons = new ManaButton[6];
            for (int i = 0; i < 6; i++)
            {
                manaButtons[i] = new ManaButton((ManaColour)i, 0);
                manaPanel.Controls.Add(manaButtons[i]);
                manaButtons[i].setVisibility(ManaButton.Visibility.Full);
                var i1 = i;
                manaButtons[i].Click += (sender, args) => newManaFilter(manaButtons[i1]);
            }

            //Paint += (sender, args) => onResize();
            Resize += (sender, args) => onResize();


            cardViews = new CardView[]
            {
                cardView1,
                cardView2,
                cardView3,
                cardView4,
                cardView5,
                cardView6,
                cardView7,
                cardView8,
                cardView9,
                cardView10,
            };
            foreach (var c in cardViews)
            {
                var c1 = c;
                c.Click += (sender, args) =>
                {
                    myCurrentDeck.Add(c1.card.template);
                };
            }

            drawCards(x => true);
            onResize();

            page = 0;
            deck = new Pile();
            cardsPanel1.setPile(deck);
            cardsPanel1.vertical = true;
            shownCards = new CardTemplate?[cardViews.Length];

            for (int i = 0; i < cardViews.Length; i++)
            {
                int i1 = i;
                cardViews[i].Click += (_, __) =>
                {
                    if (shownCards[i1].HasValue)
                    {
                        cardClicked(shownCards[i1].Value);
                    }
                };
            }

            filterCards(x => true);
            drawCards();
        }

        private void cardClicked(CardTemplate ct)
        {
            int cc = deck.Count(c => c.template == ct);
            if (cc >= 3) return;
            deck.addTop(new Card(ct));
        }

        private void filterCards(Func<CardTemplate, bool> filter)
        {
            filteredCards = Enum.GetValues(typeof(CardTemplate)).Cast<CardTemplate>().Where(filter).ToArray();
            drawCards();
        }

        private void drawCards()
        {
            int offset = 0;
            for (int i = 0; i < shownCards.Length; i++)
            {
                int ix = offset + i;
                CardTemplate? t = filteredCards.Length > ix ? filteredCards[ix] : (CardTemplate?)null;
                shownCards[i] = t;
            }

            for (int i = 0; i < cardViews.Length; i++)
            {
                if (shownCards[i].HasValue)
                {
                    cardViews[i].setCard(shownCards[i].Value);
                    cardViews[i].Visible = true;
                }
                else
                {
                    cardViews[i].Visible = false;
                }
            }
        }

        private void newSearch(object sender, EventArgs e)
        {
            filterCards(x => x.ToString().ToLower().Contains(searchBox.Text));
        }
        
        private void onResize()
        {
            for (int i = 0; i < 6; i++)
            {
                manaButtons[i].SetBounds(i * manaPanel.Width / 6, 0, manaPanel.Width / 6, manaPanel.Height);
            }
        }

        private void InitializeComponent()
        {
            this.cardView1 = new stonerkart.CardView();
            this.cardView2 = new stonerkart.CardView();
            this.cardView3 = new stonerkart.CardView();
            this.cardView5 = new stonerkart.CardView();
            this.cardView6 = new stonerkart.CardView();
            this.cardView8 = new stonerkart.CardView();
            this.cardView9 = new stonerkart.CardView();
            this.cardView10 = new stonerkart.CardView();
            this.searchBox = new System.Windows.Forms.TextBox();
            this.manaPanel = new System.Windows.Forms.Panel();
            this.cardView4 = new stonerkart.CardView();
            this.cardView7 = new stonerkart.CardView();
            this.cardsPanel1 = new CardsPanel();
            this.SuspendLayout();

            //
            // cardsPanel1
            //
            this.cardsPanel1.BackColor = System.Drawing.Color.Navy;
            this.cardsPanel1.Location = new System.Drawing.Point(870, 3);
            this.cardsPanel1.Name = "cardsPanel1";
            this.cardsPanel1.Size = new System.Drawing.Size(147, 646);
            this.cardsPanel1.TabIndex = 14;
            this.cardsPanel1.vertical = false;

            // 
            // cardView1
            // 
            this.cardView1.BackColor = System.Drawing.Color.DarkViolet;
            this.cardView1.Location = new System.Drawing.Point(3, 105);
            this.cardView1.Name = "cardView1";
            this.cardView1.Size = new System.Drawing.Size(161, 193);
            this.cardView1.TabIndex = 0;
            // 
            // cardView2
            // 
            this.cardView2.BackColor = System.Drawing.Color.DarkViolet;
            this.cardView2.Location = new System.Drawing.Point(170, 105);
            this.cardView2.Name = "cardView2";
            this.cardView2.Size = new System.Drawing.Size(161, 193);
            this.cardView2.TabIndex = 7;
            // 
            // cardView3
            // 
            this.cardView3.BackColor = System.Drawing.Color.DarkViolet;
            this.cardView3.Location = new System.Drawing.Point(337, 105);
            this.cardView3.Name = "cardView3";
            this.cardView3.Size = new System.Drawing.Size(161, 193);
            this.cardView3.TabIndex = 7;
            // 
            // cardView5
            // 
            this.cardView5.BackColor = System.Drawing.Color.DarkViolet;
            this.cardView5.Location = new System.Drawing.Point(504, 105);
            this.cardView5.Name = "cardView5";
            this.cardView5.Size = new System.Drawing.Size(161, 193);
            this.cardView5.TabIndex = 7;
            // 
            // cardView6
            // 
            this.cardView6.BackColor = System.Drawing.Color.DarkViolet;
            this.cardView6.Location = new System.Drawing.Point(504, 304);
            this.cardView6.Name = "cardView6";
            this.cardView6.Size = new System.Drawing.Size(161, 193);
            this.cardView6.TabIndex = 9;
            // 
            // cardView8
            // 
            this.cardView8.BackColor = System.Drawing.Color.DarkViolet;
            this.cardView8.Location = new System.Drawing.Point(337, 304);
            this.cardView8.Name = "cardView8";
            this.cardView8.Size = new System.Drawing.Size(161, 193);
            this.cardView8.TabIndex = 11;
            // 
            // cardView9
            // 
            this.cardView9.BackColor = System.Drawing.Color.DarkViolet;
            this.cardView9.Location = new System.Drawing.Point(170, 304);
            this.cardView9.Name = "cardView9";
            this.cardView9.Size = new System.Drawing.Size(161, 193);
            this.cardView9.TabIndex = 12;
            // 
            // cardView10
            // 
            this.cardView10.BackColor = System.Drawing.Color.DarkViolet;
            this.cardView10.Location = new System.Drawing.Point(3, 304);
            this.cardView10.Name = "cardView10";
            this.cardView10.Size = new System.Drawing.Size(161, 193);
            this.cardView10.TabIndex = 8;
            // 
            // searchBox
            // 
            this.searchBox.Location = new System.Drawing.Point(0, 79);
            this.searchBox.Name = "searchBox";
            this.searchBox.Size = new System.Drawing.Size(328, 20);
            this.searchBox.TabIndex = 13;
            this.searchBox.TextChanged += new System.EventHandler(this.newSearch);
            // 
            // manaPanel
            // 
            this.manaPanel.Location = new System.Drawing.Point(3, 3);
            this.manaPanel.Name = "manaPanel";
            this.manaPanel.Size = new System.Drawing.Size(328, 70);
            this.manaPanel.TabIndex = 14;
            // 
            // cardView4
            // 
            this.cardView4.BackColor = System.Drawing.Color.DarkViolet;
            this.cardView4.Location = new System.Drawing.Point(671, 105);
            this.cardView4.Name = "cardView4";
            this.cardView4.Size = new System.Drawing.Size(161, 193);
            this.cardView4.TabIndex = 7;
            // 
            // cardView7
            // 
            this.cardView7.BackColor = System.Drawing.Color.DarkViolet;
            this.cardView7.Location = new System.Drawing.Point(671, 304);
            this.cardView7.Name = "cardView7";
            this.cardView7.Size = new System.Drawing.Size(161, 193);
            this.cardView7.TabIndex = 10;
            // 
            // DeckEditorPanel
            // 
            this.BackColor = System.Drawing.Color.Aqua;
            this.Controls.Add(this.cardsPanel1);
            this.Controls.Add(this.manaPanel);
            this.Controls.Add(this.searchBox);
            this.Controls.Add(this.cardView6);
            this.Controls.Add(this.cardView5);
            this.Controls.Add(this.cardView7);
            this.Controls.Add(this.cardView4);
            this.Controls.Add(this.cardView8);
            this.Controls.Add(this.cardView9);
            this.Controls.Add(this.cardView3);
            this.Controls.Add(this.cardView10);
            this.Controls.Add(this.cardView2);
            this.Controls.Add(this.cardView1);
            this.Name = "DeckEditorPanel";
            this.Size = new System.Drawing.Size(1020, 652);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void drawCards(Func<CardTemplate, bool> filter)
        {
            var cards = Enum.GetValues(typeof(CardTemplate)).Cast<CardTemplate>().Where(filter);

            for (int i = 0; i < cardViews.Length; i++)
            {
                if (i < cards.Count())
                {
                    cardViews[i].setCard(cards.ElementAt(i));
                    cardViews[i].Visible = true;
                }
                else
                    cardViews[i].Visible = false;
            }
        }

        private void newManaFilter(ManaButton mb)
        {

        }
    }
}
