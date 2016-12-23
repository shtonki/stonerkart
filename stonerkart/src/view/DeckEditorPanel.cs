using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace stonerkart
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
        private CardsPanel deckPanel;
        private Panel manaPanel;
        private CardView[] cardViews;
        private ManaButton[] manaButtons;
        private CardView cardView4;
        private CardView cardView7;
        private CardView heroicCardView;

        private CardTemplate[] filteredCards;
        private CardTemplate?[] shownCards;
        private int page;

        private Pile deck;
        private CardTemplate _heroic;
        private Button button1;
        private TextBox deckName;
        private Button button2;
        private Button button3;
        private Button button4;
        private Label formatTextBox;
        private Button button5;
        private DeckContraints currentConstraint = new DeckContraints(Format.Standard);

        public DeckEditorPanel()
        {
            InitializeComponent();
            deckPanel.comp = srt;
            manaButtons = new ManaButton[Enum.GetValues(typeof(ManaColour)).Length];
            for (int i = 0; i < manaButtons.Length; i++)
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

            onResize();

            page = 0;
            deck = new Pile();
            deckPanel.setPile(deck);
            deckPanel.vertical = true;
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
                cardViews[i].MouseEnter += (_, __) =>
                {
                    if (shownCards[i1].HasValue)
                    {
                        mouseEntered(shownCards[i1].Value);
                    }
                };
            }

            deckPanel.clickedCallbacks.Add(clickable =>
            {
                Card c = (Card)clickable.getStuff();
                deck.remove(c);
            });

            filterCards(x => true);
            drawCards();
            button3_Click(null, null);
        }

        private CardTemplate heroic
        {
            get { return _heroic; }
            set
            {
                _heroic = value;
                heroicCardView.setCard(value);
            }
        }

        private void cardClicked(CardTemplate ct)
        {
            if (Card.fromTemplate(ct).isHeroic)
                heroic = ct;

            if (!currentConstraint.willBeLegal(CardTemplate.Belwas, deck.Select(c => c.template).ToArray(), ct))
                return;

            nicememe();

            deck.addTop(new Card(ct));
        }

        private void mouseEntered(CardTemplate ct)
        {

        }

        private void filterCards(Func<CardTemplate, bool> filter)
        {
            var x = Enum.GetValues(typeof(CardTemplate)).Cast<CardTemplate>().Where(filter).ToList();
            x.Sort(srt);
            filteredCards = x.ToArray();

            drawCards();
        }

        private int srt(CardTemplate t1, CardTemplate t2)
        {
            Card c1 = Card.fromTemplate(t1);
            Card c2 = Card.fromTemplate(t2);

            if (c1.colours.Count > c2.colours.Count) return 1;
            if (c1.colours.Count < c2.colours.Count) return -1;

            if ((int)c1.colours[0] > (int)c2.colours[0]) return 1;
            if ((int)c1.colours[0] < (int)c2.colours[0]) return -1;

            if (c1.convertedManaCost > c2.convertedManaCost) return -1;
            if (c1.convertedManaCost < c2.convertedManaCost) return 1;

            return String.Compare(c1.name, c2.name);
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

        private void loadDeck(string deckName)
        {
            deck.clear();
            Deck v = Controller.loadDeck(deckName);
            deck.addRange(v.templates.Select(t => new Card(t)));
            heroic = v.hero;
        }

        private int srt(CardView v1, CardView v2)
        {
            Card c1 = v1.card;
            Card c2 = v2.card;

            if (c1.convertedManaCost > c2.convertedManaCost) return 1;
            if (c2.convertedManaCost > c1.convertedManaCost) return -1;

            return String.Compare(c1.name, c2.name);
        }

        private void onResize()
        {
            int jasinhackxd = manaButtons.Length;
            for (int i = 0; i < jasinhackxd; i++)
            {
                manaButtons[i].SetBounds(i * manaPanel.Width / jasinhackxd, 0, 
                                         manaPanel.Width / jasinhackxd    , manaPanel.Height);
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
            this.deckPanel = new stonerkart.CardsPanel();
            this.heroicCardView = new stonerkart.CardView();
            this.button1 = new System.Windows.Forms.Button();
            this.deckName = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.formatTextBox = new System.Windows.Forms.Label();
            this.button5 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // cardView1
            // 
            this.cardView1.BackColor = System.Drawing.Color.DarkViolet;
            this.cardView1.Location = new System.Drawing.Point(83, 249);
            this.cardView1.Name = "cardView1";
            this.cardView1.Size = new System.Drawing.Size(133, 193);
            this.cardView1.TabIndex = 0;
            // 
            // cardView2
            // 
            this.cardView2.BackColor = System.Drawing.Color.DarkViolet;
            this.cardView2.Location = new System.Drawing.Point(222, 249);
            this.cardView2.Name = "cardView2";
            this.cardView2.Size = new System.Drawing.Size(133, 193);
            this.cardView2.TabIndex = 7;
            // 
            // cardView3
            // 
            this.cardView3.BackColor = System.Drawing.Color.DarkViolet;
            this.cardView3.Location = new System.Drawing.Point(361, 249);
            this.cardView3.Name = "cardView3";
            this.cardView3.Size = new System.Drawing.Size(133, 193);
            this.cardView3.TabIndex = 7;
            // 
            // cardView5
            // 
            this.cardView5.BackColor = System.Drawing.Color.DarkViolet;
            this.cardView5.Location = new System.Drawing.Point(500, 249);
            this.cardView5.Name = "cardView5";
            this.cardView5.Size = new System.Drawing.Size(133, 193);
            this.cardView5.TabIndex = 7;
            // 
            // cardView6
            // 
            this.cardView6.BackColor = System.Drawing.Color.DarkViolet;
            this.cardView6.Location = new System.Drawing.Point(83, 448);
            this.cardView6.Name = "cardView6";
            this.cardView6.Size = new System.Drawing.Size(133, 193);
            this.cardView6.TabIndex = 9;
            // 
            // cardView8
            // 
            this.cardView8.BackColor = System.Drawing.Color.DarkViolet;
            this.cardView8.Location = new System.Drawing.Point(361, 448);
            this.cardView8.Name = "cardView8";
            this.cardView8.Size = new System.Drawing.Size(133, 193);
            this.cardView8.TabIndex = 11;
            // 
            // cardView9
            // 
            this.cardView9.BackColor = System.Drawing.Color.DarkViolet;
            this.cardView9.Location = new System.Drawing.Point(500, 448);
            this.cardView9.Name = "cardView9";
            this.cardView9.Size = new System.Drawing.Size(133, 193);
            this.cardView9.TabIndex = 12;
            // 
            // cardView10
            // 
            this.cardView10.BackColor = System.Drawing.Color.DarkViolet;
            this.cardView10.Location = new System.Drawing.Point(639, 448);
            this.cardView10.Name = "cardView10";
            this.cardView10.Size = new System.Drawing.Size(133, 193);
            this.cardView10.TabIndex = 8;
            // 
            // searchBox
            // 
            this.searchBox.Location = new System.Drawing.Point(0, 79);
            this.searchBox.Name = "searchBox";
            this.searchBox.Size = new System.Drawing.Size(167, 20);
            this.searchBox.TabIndex = 13;
            this.searchBox.TextChanged += new System.EventHandler(this.newSearch);
            // 
            // manaPanel
            // 
            this.manaPanel.Location = new System.Drawing.Point(83, 3);
            this.manaPanel.Name = "manaPanel";
            this.manaPanel.Size = new System.Drawing.Size(508, 70);
            this.manaPanel.TabIndex = 14;
            // 
            // cardView4
            // 
            this.cardView4.BackColor = System.Drawing.Color.DarkViolet;
            this.cardView4.Location = new System.Drawing.Point(639, 249);
            this.cardView4.Name = "cardView4";
            this.cardView4.Size = new System.Drawing.Size(133, 193);
            this.cardView4.TabIndex = 7;
            // 
            // cardView7
            // 
            this.cardView7.BackColor = System.Drawing.Color.DarkViolet;
            this.cardView7.Location = new System.Drawing.Point(222, 448);
            this.cardView7.Name = "cardView7";
            this.cardView7.Size = new System.Drawing.Size(133, 193);
            this.cardView7.TabIndex = 10;
            // 
            // deckPanel
            // 
            this.deckPanel.BackColor = System.Drawing.Color.Navy;
            this.deckPanel.comp = null;
            this.deckPanel.Location = new System.Drawing.Point(870, 58);
            this.deckPanel.Name = "deckPanel";
            this.deckPanel.Size = new System.Drawing.Size(147, 583);
            this.deckPanel.TabIndex = 14;
            this.deckPanel.vertical = false;
            // 
            // heroicCardView
            // 
            this.heroicCardView.BackColor = System.Drawing.Color.DarkViolet;
            this.heroicCardView.Location = new System.Drawing.Point(707, 3);
            this.heroicCardView.Name = "heroicCardView";
            this.heroicCardView.Size = new System.Drawing.Size(157, 235);
            this.heroicCardView.TabIndex = 15;
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.SystemColors.ButtonShadow;
            this.button1.Location = new System.Drawing.Point(870, 29);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(66, 23);
            this.button1.TabIndex = 16;
            this.button1.Text = "Save";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // deckName
            // 
            this.deckName.Location = new System.Drawing.Point(870, 3);
            this.deckName.Name = "deckName";
            this.deckName.Size = new System.Drawing.Size(147, 20);
            this.deckName.TabIndex = 17;
            this.deckName.Text = "nigra";
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.SystemColors.ButtonShadow;
            this.button2.Location = new System.Drawing.Point(955, 29);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(62, 23);
            this.button2.TabIndex = 18;
            this.button2.Text = "Load";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(616, 29);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 19;
            this.button3.Text = "Standard";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(616, 58);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(75, 23);
            this.button4.TabIndex = 21;
            this.button4.Text = "Test";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // formatTextBox
            // 
            this.formatTextBox.AutoSize = true;
            this.formatTextBox.BackColor = System.Drawing.Color.Gray;
            this.formatTextBox.Location = new System.Drawing.Point(636, 6);
            this.formatTextBox.Name = "formatTextBox";
            this.formatTextBox.Size = new System.Drawing.Size(55, 13);
            this.formatTextBox.TabIndex = 22;
            this.formatTextBox.Text = "nicememe";
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(4, -1);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(73, 74);
            this.button5.TabIndex = 23;
            this.button5.Text = "Back";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // DeckEditorPanel
            // 
            this.BackColor = System.Drawing.Color.Aqua;
            this.Controls.Add(this.button5);
            this.Controls.Add(this.formatTextBox);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.deckName);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.heroicCardView);
            this.Controls.Add(this.deckPanel);
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

        private void button1_Click(object sender, EventArgs e)
        {
            Deck d = new Deck(heroic, deck.Select(c => c.template).ToArray());
            Controller.saveDeck(d, deckName.Text);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Controller.chooseDeck(loadDeck);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            nicememe(Format.Standard);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            nicememe(Format.Test);
        }

        private void nicememe(Format f)
        {
            formatTextBox.memeout(() =>
            {
                formatTextBox.Text = f.ToString();
                formatTextBox.Refresh();
                currentConstraint = new DeckContraints(f);
            });
            nicememe();
        }

        private void nicememe()
        {

            formatTextBox.memeout(() =>
            {
                formatTextBox.BackColor = currentConstraint.testLegal(CardTemplate.Belwas, deck.Select(c => c.template).ToArray()) ? Color.ForestGreen : Color.IndianRed;
            });
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Controller.transitionToMainMenu();
        }
    }
}
