using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace stonerkart
{

    class DeckEditorPanel : StickyPanel, Screen
    {
        public List<CardTemplate> ownedCards = new List<CardTemplate>() {CardTemplate.Daring_sPoppy, CardTemplate.Stark_sLily}; 
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
        private Button button6;
        private Button button7;
        private DeckContraints currentConstraint = new DeckContraints(Format.Standard);
        private Button button8;
        private Label numberOfCardsLabel;
        private const int cardsPerPage = 10;

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
                numberOfCardsLabel.Text = deck.Count.ToString();
            });

            filterCards(x => true);
            drawCards();
            button3_Click(null, null);
            
            heroic = CardTemplate.Shibby_sShtank;
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

            if (!currentConstraint.willBeLegal(CardTemplate.Bhewas, deck.Select(c => c.template).ToArray(), ct))
                return;

            nicememe();

            deck.addTop(new Card(ct));
            numberOfCardsLabel.Text = deck.Count.ToString();
        }

        private void mouseEntered(CardTemplate ct)
        {

        }

        private void filterCards(Func<CardTemplate, bool> filter)
        {
            var x = Enum.GetValues(typeof(CardTemplate)).Cast<CardTemplate>().Where(filter).ToList();
            var xx = x.Memesort(srt);
            filteredCards = xx.ToArray();

            page = 0;
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

            if (c1.convertedManaCost > c2.convertedManaCost) return 1;
            if (c1.convertedManaCost < c2.convertedManaCost) return -1;

            return String.Compare(c1.name, c2.name);
        }

        private void drawCards()
        {
            int offset = page * cardsPerPage;
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
                    if (ownedCards.Contains((CardTemplate)shownCards[i]))
                    {
                        cardViews[i].glowColour(Color.Green);
                    }
                    else
                    {
                        cardViews[i].glowColour(Color.DarkRed);
                    }
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

        private void loadDeck(Deck d)
        {
            deck = new Pile();
            deck.addRange(d.templates.Select(t => new Card(t)));
            deckName.Text = d.name;
            heroic = d.hero;
            deckPanel.vertical = true;
            deckPanel.setPile(deck);
            deck.addTop(deck.removeTop());

            numberOfCardsLabel.Text = deck.Count.ToString();
        }

        private void turnPage(bool left)
        {
            page += left ? -1 : 1;
            if (page * cardsPerPage > filteredCards.Length) page--;
            if (page < 0) page = 0;
            drawCards();
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
                                         manaPanel.Width / jasinhackxd, manaPanel.Height);
            }
        }

        public IEnumerable<MenuItem> getMenuPanel()
        {
            return new List<MenuItem>();
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
            this.button6 = new System.Windows.Forms.Button();
            this.button7 = new System.Windows.Forms.Button();
            this.button8 = new System.Windows.Forms.Button();
            this.numberOfCardsLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // cardView1
            // 
            this.cardView1.BackColor = System.Drawing.Color.DarkViolet;
            this.cardView1.Location = new System.Drawing.Point(69, 311);
            this.cardView1.Name = "cardView1";
            this.cardView1.showBase = false;
            this.cardView1.Size = new System.Drawing.Size(129, 166);
            this.cardView1.TabIndex = 0;
            // 
            // cardView2
            // 
            this.cardView2.BackColor = System.Drawing.Color.DarkViolet;
            this.cardView2.Location = new System.Drawing.Point(204, 311);
            this.cardView2.Name = "cardView2";
            this.cardView2.showBase = false;
            this.cardView2.Size = new System.Drawing.Size(129, 166);
            this.cardView2.TabIndex = 7;
            // 
            // cardView3
            // 
            this.cardView3.BackColor = System.Drawing.Color.DarkViolet;
            this.cardView3.Location = new System.Drawing.Point(339, 311);
            this.cardView3.Name = "cardView3";
            this.cardView3.showBase = false;
            this.cardView3.Size = new System.Drawing.Size(129, 166);
            this.cardView3.TabIndex = 7;
            // 
            // cardView5
            // 
            this.cardView5.BackColor = System.Drawing.Color.DarkViolet;
            this.cardView5.Location = new System.Drawing.Point(609, 311);
            this.cardView5.Name = "cardView5";
            this.cardView5.showBase = false;
            this.cardView5.Size = new System.Drawing.Size(129, 166);
            this.cardView5.TabIndex = 7;
            // 
            // cardView6
            // 
            this.cardView6.BackColor = System.Drawing.Color.DarkViolet;
            this.cardView6.Location = new System.Drawing.Point(69, 479);
            this.cardView6.Name = "cardView6";
            this.cardView6.showBase = false;
            this.cardView6.Size = new System.Drawing.Size(129, 166);
            this.cardView6.TabIndex = 9;
            // 
            // cardView8
            // 
            this.cardView8.BackColor = System.Drawing.Color.DarkViolet;
            this.cardView8.Location = new System.Drawing.Point(339, 479);
            this.cardView8.Name = "cardView8";
            this.cardView8.showBase = false;
            this.cardView8.Size = new System.Drawing.Size(129, 166);
            this.cardView8.TabIndex = 11;
            // 
            // cardView9
            // 
            this.cardView9.BackColor = System.Drawing.Color.DarkViolet;
            this.cardView9.Location = new System.Drawing.Point(474, 479);
            this.cardView9.Name = "cardView9";
            this.cardView9.showBase = false;
            this.cardView9.Size = new System.Drawing.Size(129, 166);
            this.cardView9.TabIndex = 12;
            // 
            // cardView10
            // 
            this.cardView10.BackColor = System.Drawing.Color.DarkViolet;
            this.cardView10.Location = new System.Drawing.Point(609, 479);
            this.cardView10.Name = "cardView10";
            this.cardView10.showBase = false;
            this.cardView10.Size = new System.Drawing.Size(129, 166);
            this.cardView10.TabIndex = 8;
            // 
            // searchBox
            // 
            this.searchBox.Location = new System.Drawing.Point(69, 207);
            this.searchBox.Name = "searchBox";
            this.searchBox.Size = new System.Drawing.Size(273, 20);
            this.searchBox.TabIndex = 13;
            this.searchBox.TextChanged += new System.EventHandler(this.newSearch);
            // 
            // manaPanel
            // 
            this.manaPanel.Location = new System.Drawing.Point(69, 131);
            this.manaPanel.Name = "manaPanel";
            this.manaPanel.Size = new System.Drawing.Size(449, 70);
            this.manaPanel.TabIndex = 14;
            // 
            // cardView4
            // 
            this.cardView4.BackColor = System.Drawing.Color.DarkViolet;
            this.cardView4.Location = new System.Drawing.Point(474, 311);
            this.cardView4.Name = "cardView4";
            this.cardView4.showBase = false;
            this.cardView4.Size = new System.Drawing.Size(129, 166);
            this.cardView4.TabIndex = 7;
            // 
            // cardView7
            // 
            this.cardView7.BackColor = System.Drawing.Color.DarkViolet;
            this.cardView7.Location = new System.Drawing.Point(204, 479);
            this.cardView7.Name = "cardView7";
            this.cardView7.showBase = false;
            this.cardView7.Size = new System.Drawing.Size(129, 166);
            this.cardView7.TabIndex = 10;
            // 
            // deckPanel
            // 
            this.deckPanel.BackColor = System.Drawing.Color.Navy;
            this.deckPanel.comp = null;
            this.deckPanel.Location = new System.Drawing.Point(809, 58);
            this.deckPanel.Name = "deckPanel";
            this.deckPanel.Size = new System.Drawing.Size(208, 583);
            this.deckPanel.TabIndex = 14;
            this.deckPanel.vertical = false;
            // 
            // heroicCardView
            // 
            this.heroicCardView.BackColor = System.Drawing.Color.DarkViolet;
            this.heroicCardView.Location = new System.Drawing.Point(593, 0);
            this.heroicCardView.Name = "heroicCardView";
            this.heroicCardView.showBase = false;
            this.heroicCardView.Size = new System.Drawing.Size(204, 201);
            this.heroicCardView.TabIndex = 15;
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.SystemColors.ButtonShadow;
            this.button1.Location = new System.Drawing.Point(809, 29);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(107, 23);
            this.button1.TabIndex = 16;
            this.button1.Text = "Save";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // deckName
            // 
            this.deckName.Location = new System.Drawing.Point(809, 3);
            this.deckName.Name = "deckName";
            this.deckName.Size = new System.Drawing.Size(176, 20);
            this.deckName.TabIndex = 17;
            this.deckName.Text = "nigra";
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.SystemColors.ButtonShadow;
            this.button2.Location = new System.Drawing.Point(922, 30);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(95, 23);
            this.button2.TabIndex = 18;
            this.button2.Text = "Load";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(145, 20);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 19;
            this.button3.Text = "Standard";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(145, 50);
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
            this.formatTextBox.Location = new System.Drawing.Point(257, 6);
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
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(744, 311);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(53, 338);
            this.button6.TabIndex = 24;
            this.button6.Text = "Next Page";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // button7
            // 
            this.button7.Location = new System.Drawing.Point(7, 307);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(53, 338);
            this.button7.TabIndex = 25;
            this.button7.Text = "Previous Page";
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.button7_Click);
            // 
            // button8
            // 
            this.button8.Location = new System.Drawing.Point(996, 1);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(21, 22);
            this.button8.TabIndex = 26;
            this.button8.Text = "button8";
            this.button8.UseVisualStyleBackColor = true;
            this.button8.Click += new System.EventHandler(this.button8_Click);
            // 
            // numberOfCardsLabel
            // 
            this.numberOfCardsLabel.AutoSize = true;
            this.numberOfCardsLabel.Location = new System.Drawing.Point(257, 25);
            this.numberOfCardsLabel.Name = "numberOfCardsLabel";
            this.numberOfCardsLabel.Size = new System.Drawing.Size(13, 13);
            this.numberOfCardsLabel.TabIndex = 27;
            this.numberOfCardsLabel.Text = "0";
            // 
            // DeckEditorPanel
            // 
            this.BackColor = System.Drawing.Color.Aqua;
            this.Controls.Add(this.numberOfCardsLabel);
            this.Controls.Add(this.button8);
            this.Controls.Add(this.button7);
            this.Controls.Add(this.button6);
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
            this.Controls.Add(this.cardView4);
            this.Controls.Add(this.cardView3);
            this.Controls.Add(this.cardView10);
            this.Controls.Add(this.cardView9);
            this.Controls.Add(this.cardView1);
            this.Controls.Add(this.cardView2);
            this.Controls.Add(this.searchBox);
            this.Controls.Add(this.cardView6);
            this.Controls.Add(this.cardView5);
            this.Controls.Add(this.cardView7);
            this.Controls.Add(this.cardView8);
            this.Name = "DeckEditorPanel";
            this.Size = new System.Drawing.Size(1020, 652);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private ManaColour? hack;
        private void newManaFilter(ManaButton mb)
        {
            ManaColour colour = mb.orb;

            if (hack.HasValue && hack.Value == colour)
            {
                hack = null;
                filterCards(_ => true);
                return;
            }

            hack = colour;

            Func<CardTemplate, bool> f = ct => (Card.fromTemplate(ct).colours.Contains(colour));
            Func<CardTemplate, bool> f2 = ct => (Card.fromTemplate(ct).colours.Count > 1);

            filterCards(colour == ManaColour.Multi ? f2 : f);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Deck d = new Deck(heroic, deck.Select(c => c.template).ToArray());
            DeckController.saveDeck(d, deckName.Text);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DeckController.chooseDeck(loadDeck);
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
                formatTextBox.BackColor = currentConstraint.testLegal(CardTemplate.Bhewas, deck.Select(c => c.template).ToArray()) ? Color.ForestGreen : Color.IndianRed;
            });
        }

        private void button5_Click(object sender, EventArgs e)
        {
            ScreenController.transitionToMainMenu();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            turnPage(false);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            turnPage(true);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("public static CardTemplate[] xd = new []{");

            foreach (Card c in deck)
            {
                sb.Append("CardTemplate.");
                sb.Append(c.template);
                sb.AppendLine(",");
            }
            sb.Append("};");
            File.WriteAllText("dmp.txt", sb.ToString());
        }

    }
}