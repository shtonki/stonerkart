using System;
using System.Collections.Generic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace stonerkart
{
    class DeckEditorScreen : Screen
    {
        #region constants

        private const int NR_OF_CARDS_PER_ROW = 8;
        private const int NR_OF_CARDS_PER_COLLUMN = 3;
        private const int NR_OF_CARD_VIEWS = NR_OF_CARDS_PER_ROW * NR_OF_CARDS_PER_COLLUMN;
        private const int DECK_SCREEN_ITEMS_PER_ROW = 2; //load button and save button
        private const int CARD_VIEW_WIDTH = Frame.BACKSCREENWIDTH / 12;
        private const int CARD_VIEW_HEIGHT = CARD_VIEW_WIDTH * 700 / 500;
        private const int FRAME_WIDTH = Frame.BACKSCREENWIDTH;
        private const int FRAME_HEIGHT = Frame.AVAILABLEHEIGHT;
        private const int HERO_VIEW_WIDTH = PILE_VIEW_HEIGHT * 500 / 700;


        private const int STATS_CARDCOUNTER_HEIGHT = 90;
        private const int STATS_CARDCOUNTER_WIDTH = 90;
        private const int STATS_MAX_BAR_HEIGHT = 256;
        private const int STATS_BAR_WIDTH = 32;
        private const int STATS_BAR_SPACING = 16;
        private const int STATS_TEXT_SPACING = 32;
        private const int STATS_MANA_BUTTON_HEIGHT = 32;
        private const int DECK_SCREEN_WIDTH = DECK_SCREEN_ITEMS_PER_ROW * BUTTON_WIDTH;
        private const int HERO_VIEW_X = PILE_VIEW_WIDTH;
        private const int HERO_VIEW_Y = 0;
        private const int HOVER_VIEW_WIDTH = CARD_VIEW_WIDTH * NR_OF_CARDS_PER_COLLUMN;// + CARD_VIEW_PANEL_WIDTH > FRAME_WIDTH ? FRAME_WIDTH - CARD_VIEW_PANEL_WIDTH - DOWN_PAGE_BUTTON_WIDTH : CARD_VIEW_PANEL_WIDTH * NR_OF_CARDS_PER_COLLUMN;
        private const int HOVER_VIEW_X = FRAME_WIDTH - HOVER_VIEW_WIDTH;
        private const int HOVER_VIEW_Y = CARD_VIEW_PANEL_Y;
        private const int PILE_VIEW_X = 0;
        private const int PILE_VIEW_Y = 0;
        private const int PILE_VIEW_WIDTH = CARD_VIEW_PANEL_WIDTH;//-HERO_VIEW_WIDTH;
        private const int PILE_VIEW_HEIGHT = FRAME_HEIGHT - CARD_VIEW_PANEL_HEIGHT - MANA_BUTTON_HEIGHT <= 0 ? CARD_VIEW_PANEL_HEIGHT : FRAME_HEIGHT - CARD_VIEW_PANEL_HEIGHT - MANA_BUTTON_HEIGHT;//FRAME_HEIGHT - CARD_VIEW_PANEL_HEIGHT <= 0 ? CARD_VIEW_PANEL_HEIGHT : FRAME_HEIGHT - CARD_VIEW_PANEL_HEIGHT;
        private const int CARD_VIEW_PANEL_X = 0;
        private const int CARD_VIEW_PANEL_Y = FRAME_HEIGHT - CARD_VIEW_PANEL_HEIGHT;
        private const int CARD_VIEW_PANEL_WIDTH = NR_OF_CARDS_PER_ROW * CARD_VIEW_STRIDE_X;
        private const int CARD_VIEW_PANEL_HEIGHT = NR_OF_CARDS_PER_COLLUMN * CARD_VIEW_HEIGHT;
        private const int CARD_VIEW_X = CARD_VIEW_PANEL_X;
        private const int CARD_VIEW_Y = CARD_VIEW_PANEL_Y;
        private const int CARD_VIEW_STRIDE_X = CARD_VIEW_WIDTH + 0;
        private const int CARD_VIEW_STRIDE_Y = CARD_VIEW_HEIGHT + 0;
        private const int INPUT_BOX_X = MANA_BUTTON_X + MANA_BUTTON_WIDTH * NR_OF_MANA_BUTTONS;
        private const int INPUT_BOX_Y = MANA_BUTTON_Y;
        private const int INPUT_BOX_WIDTH = MANA_BUTTON_WIDTH * NR_OF_MANA_BUTTONS;
        private const int INPUT_BOX_HEIGHT = MANA_BUTTON_HEIGHT;
        private const int NR_OF_MANA_BUTTONS = 7;
        private const int MANA_BUTTON_X = 0;//CARD_VIEW_PANEL_WIDTH + HERO_VIEW_WIDTH;
        private const int MANA_BUTTON_Y = CARD_VIEW_PANEL_Y - MANA_BUTTON_WIDTH;
        private const int MANA_BUTTON_WIDTH = CARD_VIEW_HEIGHT/4;//(FRAME_WIDTH - PILE_VIEW_WIDTH - HERO_VIEW_WIDTH)/NR_OF_MANA_BUTTONS;
        private const int MANA_BUTTON_HEIGHT = MANA_BUTTON_WIDTH;

        //private const int CARD_VIEW_PANEL_HEIGHT = CARD_VIEW_PANEL_NUMBER_OF_ROWS * CARD_VIEW_HEIGHT;
        private const int DECK_NAME_BOX_X = CARD_VIEW_PANEL_WIDTH + HERO_VIEW_WIDTH;
        private const int DECK_NAME_BOX_Y = 0;
        private const int DECK_NAME_BOX_WIDTH = FRAME_WIDTH - PILE_VIEW_WIDTH - HERO_VIEW_WIDTH;
        private const int DECK_NAME_BOX_HEIGHT = (int)(Frame.BACKSCREENHEIGHT * 0.05);
        private const int BUTTON_HEIGHT = DECK_NAME_BOX_HEIGHT / 2;
        private const int BUTTON_WIDTH = DECK_NAME_BOX_WIDTH / 2;
        private const int SAVE_BUTTON_X = LOAD_BUTTON_X + BUTTON_WIDTH;
        private const int SAVE_LOAD_BUTTON_Y = DECK_NAME_BOX_HEIGHT;
        private const int LOAD_BUTTON_X = DECK_NAME_BOX_X;


        private const int UP_PAGE_BUTTON_WIDTH = DOWN_PAGE_BUTTON_WIDTH;
        private const int UP_PAGE_BUTTON_HEIGHT = DOWN_PAGE_BUTTON_HEIGHT;
        private const int UP_PAGE_BUTTON_X = CARD_VIEW_PANEL_WIDTH;
        private const int UP_PAGE_BUTTON_Y = CARD_VIEW_PANEL_Y;
        private const int DOWN_PAGE_BUTTON_X = CARD_VIEW_PANEL_WIDTH;
        private const int DOWN_PAGE_BUTTON_Y = CARD_VIEW_PANEL_Y + UP_PAGE_BUTTON_HEIGHT;
        private const int DOWN_PAGE_BUTTON_WIDTH = FRAME_WIDTH - CARD_VIEW_PANEL_WIDTH - HOVER_VIEW_WIDTH;
        private const int DOWN_PAGE_BUTTON_HEIGHT = CARD_VIEW_PANEL_HEIGHT / 2;



        private readonly static System.Drawing.Color CARD_PANEL_BACKCOLOR = System.Drawing.Color.FromKnownColor(System.Drawing.KnownColor.Silver);
        private readonly static System.Drawing.Color PAGE_BUTTON_BACKCOLOR = System.Drawing.Color.FromKnownColor(System.Drawing.KnownColor.SaddleBrown);

        #endregion

        private CardList cardList;
        private Card currentHero;
        private CardView hoverView;
        private PileView pileView;
        private DeckContraints deckConstraints;
        private CardView[] cardViews;
        private List<Card> allCardsEver;
        private List<Card> filteredCards;
        private CardView heroCardView;
        private Square cardViewPanel;
        private Square cardCounter;
        private int currentPageNr = 0;
        private ToggleButton[] manaButtons { get; set; }
        private string searchString;
        private Square barChartPanel;


        //todo fix hero and search and deckinfo
        public DeckEditorScreen() : base(new Imege(Textures.artCallToArms))
        {
            searchString = "";
            setupButtons();
            initStatsThingy();
            setupCards();
            initCardViews(); //fix duplicate code
            pileView = setupPileView();
            cardList = setupCardList(pileView);
            deckConstraints = new DeckContraints(Format.Standard);

            
        }

        private void initStatsThingy()
        {
            barChartPanel = new Square(NR_OF_MANA_BUTTONS * (STATS_BAR_WIDTH + STATS_BAR_SPACING), STATS_MAX_BAR_HEIGHT + STATS_TEXT_SPACING);
            barChartPanel.setLocation(FRAME_WIDTH - barChartPanel.Width, SAVE_LOAD_BUTTON_Y + BUTTON_HEIGHT);
            barChartPanel.Backcolor = System.Drawing.Color.CadetBlue;
            elements.Add(barChartPanel);

            for (int i = 0; i < NR_OF_MANA_BUTTONS; i++)
            {
                int barsX = i * (STATS_BAR_WIDTH + STATS_BAR_SPACING);
                int barsY = barChartPanel.Height - STATS_MANA_BUTTON_HEIGHT - STATS_BAR_SPACING;
                Button mb = new Button(STATS_BAR_WIDTH, STATS_BAR_WIDTH);
                mb.Backimege = new Imege(TextureLoader.orbTexture((ManaColour)i));
                mb.setLocation(barsX, barsY);
                barChartPanel.addChild(mb);

                Button tb = new Button(STATS_MANA_BUTTON_HEIGHT, STATS_MANA_BUTTON_HEIGHT);
                tb.Text = "0";
                tb.setLocation(barsX, barsY - STATS_TEXT_SPACING);
                barChartPanel.addChild(tb);
            }


            cardCounter = new Square(STATS_CARDCOUNTER_WIDTH, STATS_CARDCOUNTER_HEIGHT);
            addElement(cardCounter);
            cardCounter.X = barChartPanel.X - STATS_CARDCOUNTER_WIDTH;
            cardCounter.Y = barChartPanel.Y;
            cardCounter.Text = "0";
        }

        private void setupStatsThingy()
        {
            barChartPanel.clearChildren();
            //Todo remove children but now i need to go work afk
            //todo bolshevik revolution
            int[] nrOfCardsOfEachMana = new int[NR_OF_MANA_BUTTONS];
            foreach(var c in cardList)
            {
                foreach (var clr in c.colours)
                {
                    nrOfCardsOfEachMana[(int)clr]++;
                }
            }

            Square[] bars = new Square[NR_OF_MANA_BUTTONS];
            for(int i = 0; i < NR_OF_MANA_BUTTONS; i++)
            {
                int barHeight = 
                    cardList.Count == 0 ? 
                    0 : 
                    -((STATS_MAX_BAR_HEIGHT-STATS_TEXT_SPACING-STATS_MANA_BUTTON_HEIGHT) * nrOfCardsOfEachMana[i]) / nrOfCardsOfEachMana.Sum();// + STATS_TEXT_SPACING + STATS_MANA_BUTTON_HEIGHT;
                bars[i] = new Square(i * (STATS_BAR_WIDTH + STATS_BAR_SPACING), barChartPanel.Height-STATS_MANA_BUTTON_HEIGHT-STATS_BAR_SPACING, STATS_BAR_WIDTH, barHeight, System.Drawing.Color.Black);
                barChartPanel.addChild(bars[i]);

                Button mb = new Button(STATS_BAR_WIDTH, STATS_BAR_WIDTH);
                mb.Backimege = new Imege(TextureLoader.orbTexture((ManaColour)i));
                mb.setLocation(bars[i].X, bars[i].Y);
                barChartPanel.addChild(mb);

                Button tb = new Button(STATS_MANA_BUTTON_HEIGHT, STATS_MANA_BUTTON_HEIGHT);
                tb.Text = nrOfCardsOfEachMana[i].ToString();
                tb.setLocation(bars[i].X, bars[i].Y + bars[i].Height - STATS_TEXT_SPACING);
                barChartPanel.addChild(tb);
            }

            cardCounter.Text = cardList.Count.ToString();
            cardCounter.textColor = cardList.Count >= deckConstraints.cardMin ? Color.Green : Color.Red;
        }

        private void refilter()
        {
            filteredCards = allCardsEver.Where(filterx).ToList();
        }

        private bool filterx(Card c)
        {
            bool colortest =
                (c.castManaCost[ManaColour.Chaos] > 0 && manaButtons[0].Toggled) ||
                (c.castManaCost[ManaColour.Death] > 0 && manaButtons[1].Toggled) ||
                (c.castManaCost[ManaColour.Might] > 0 && manaButtons[2].Toggled) ||
                (c.castManaCost[ManaColour.Order] > 0 && manaButtons[3].Toggled) ||
                (c.castManaCost[ManaColour.Life] > 0 && manaButtons[4].Toggled) ||
                (c.castManaCost[ManaColour.Nature] > 0 && manaButtons[5].Toggled) ||
                (c.castManaCost[ManaColour.Colourless] == c.convertedManaCost && manaButtons[6].Toggled) ||
                (c.isHeroic);

            bool tokentest = c.isToken == false;

            return colortest && tokentest;
        }

        #region setups

        private void setupCards()
        {
            allCardsEver = Card.flyweight.ToList();
            allCardsEver.Sort((c1, c2) =>
            {
                var colourcountdiff = Math.Min(2, c1.colours.Count) - Math.Min(2, c2.colours.Count);
                if (colourcountdiff != 0) return colourcountdiff;

                if (c1.colours.Count < 2)
                {
                    var colourdiff = c1.colours[0] - c2.colours[0];
                    if (colourdiff != 0) return colourdiff;
                }

                var cmcdiff = c1.convertedManaCost - c2.convertedManaCost;
                return cmcdiff;
            });
        }

        private List<Button> setupButtons()
        {
            #region input
            List<Button> bs = new List<Button>();
            InputBox deckNameBox = new InputBox(DECK_NAME_BOX_WIDTH, DECK_NAME_BOX_HEIGHT);
            deckNameBox.setLocation(DECK_NAME_BOX_X, DECK_NAME_BOX_Y);
            deckNameBox.setText("My Deck");
            deckNameBox.clicked += (_) =>
            {
                deckNameBox.setText("");
            };
            addElement(deckNameBox);
            


            InputBox searchBox = new InputBox(INPUT_BOX_WIDTH, INPUT_BOX_HEIGHT);
            searchBox.setLocation(INPUT_BOX_X, INPUT_BOX_Y);
            searchBox.setText("Search");
            searchBox.clicked += (_) =>
            {
                searchBox.setText("");
            };
            searchBox.keyDown += (_) =>
            {
                searchString = searchBox.Text;
                //erf = new Func<Card, bool>(c => c.ToString().StartsWith(searchBox.Text));
                setupCardViews();
            };
            addElement(searchBox);
            #endregion
            #region save
            Button saveButton = new Button(BUTTON_WIDTH, BUTTON_HEIGHT);
            saveButton.setLocation(SAVE_BUTTON_X, SAVE_LOAD_BUTTON_Y);
            saveButton.Backcolor = System.Drawing.Color.Red;
            saveButton.Text = "Save";
            saveButton.clicked += (_) =>
            {
                DeckController.saveDeck(new Deck(currentHero.template, cardList.Select(c => c.template).ToArray()), deckNameBox.Text);
            };
            addElement(saveButton);
            #endregion
            #region load
            Button loadButton = new Button(BUTTON_WIDTH, BUTTON_HEIGHT);
            loadButton.clicked += (_) =>
            {
                new Thread(() =>
                {
                    Deck d = DeckController.chooseDeck();
                    loadDeck(d);
                }).Start();
            };    
            loadButton.Text = "Load";
            loadButton.setLocation(LOAD_BUTTON_X, SAVE_LOAD_BUTTON_Y);
            loadButton.Backcolor = System.Drawing.Color.Red;
            addElement(loadButton);
            #endregion
            #region page buttons
            Button previousPageButton = new Button(BUTTON_WIDTH, BUTTON_HEIGHT);
            previousPageButton.setLocation(UP_PAGE_BUTTON_X, UP_PAGE_BUTTON_Y);
            previousPageButton.Width = UP_PAGE_BUTTON_WIDTH;
            previousPageButton.Height = UP_PAGE_BUTTON_HEIGHT;
            previousPageButton.Backcolor = PAGE_BUTTON_BACKCOLOR;
            previousPageButton.clicked += (_) =>
            {
                if (currentPageNr > 0) currentPageNr--;
                setupCardViews();
            };

            Button nextPageButton = new Button(BUTTON_WIDTH, BUTTON_HEIGHT);
            nextPageButton.setLocation(DOWN_PAGE_BUTTON_X, DOWN_PAGE_BUTTON_Y);
            nextPageButton.Width = UP_PAGE_BUTTON_WIDTH;
            nextPageButton.Height = UP_PAGE_BUTTON_HEIGHT;
            nextPageButton.Backcolor = PAGE_BUTTON_BACKCOLOR;
            nextPageButton.clicked += (_) =>
            {
                if (currentPageNr * NR_OF_CARD_VIEWS + NR_OF_CARD_VIEWS < filteredCards.Count) currentPageNr++;
                setupCardViews();
            };
            addElement(nextPageButton);
            addElement(previousPageButton);
            #endregion
            #region mana buttons

            manaButtons = new ToggleButton[Enum.GetValues(typeof(ManaColour)).Length];
            int mx = MANA_BUTTON_X;
            int my = MANA_BUTTON_Y;
            for (int i = 0; i < manaButtons.Length-1; i++)
            {
                int ii = i;
                manaButtons[i] = new ToggleButton(MANA_BUTTON_WIDTH, MANA_BUTTON_WIDTH);
                manaButtons[i].setLocation(mx, my);
                manaButtons[i].Backimege = new Imege(TextureLoader.orbTexture(G.orbOrder[i]));
                mx += MANA_BUTTON_WIDTH;
                if (mx > FRAME_WIDTH)
                {
                    mx = MANA_BUTTON_X;
                    my -= MANA_BUTTON_WIDTH;
                }
                addElement(manaButtons[i]);
                manaButtons[i].clicked += (args) =>
                {
                    currentPageNr = 0;
                    setupCardViews();
                };
            }

            #endregion
            #region back
            Button back = new Button(100, 40);
            back.Text = "Back";
            back.Backcolor = Color.Silver;;
            back.Border = new SolidBorder(4, Color.Black);
            back.clicked += a => GUI.transitionToScreen(GUI.mainMenuScreen);
            addElement(back);
            back.setLocation(1350, 310);
            #endregion
            return bs;
        }

        private void loadDeck(Deck deck)
        {
            cardList.clear();
            var cards = deck.templates.Select(t => new Card(t)).ToArray();
            cardList.addRange(cards);
        }

        private CardList setupCardList(PileView pv)
        {
            CardList cardList = new CardList();
            cardList.addObserver(pv);
            return cardList;
        }

        private PileView setupPileView()
        {
            PileView pv = new PileView();
            pv.Backimege = new Imege(Textures.artDeath);
            pv.Width = (int)(Frame.BACKSCREENWIDTH * 0.80);
            pv.Height = Frame.BACKSCREENHEIGHT / 4;
            pv.Height = (int)(PILE_VIEW_HEIGHT);
            pv.Width = PILE_VIEW_WIDTH;
            pv.setLocation(PILE_VIEW_X, PILE_VIEW_Y);
            //pv.Columns = 1;
            pv.mouseDown += (a) =>
            {
                var cardView = pv.viewAtClick(a);
                if (cardView != null) removeFromDeck(cardView.card);
            };
            pv.mouseMove += (__) =>
            {
                var cardView = pv.viewAtClick(__);
                if(hoverView != null && cardView != null) setupHoverCardView(cardView.card);
            };

            addElement(pv);

            return pv;
        }
        private void setupCardViews()
        {
            cardViewPanel.clearChildren();
            refilter();
            Card[] cardsToShow = getCardsFromPage();
            layoutCardViews(cardsToShow);

            //is this erf?
            while (cardsToShow.Count() < 1)
            {
                currentPageNr--;
                setupCardViews();
            }
        }

        private Card[] getCardsFromPage()
        {
            Card[] cardsToShow = new Card[NR_OF_CARD_VIEWS];
            for (int i = 0; i < NR_OF_CARD_VIEWS; i++)
            {
                int index = i + NR_OF_CARD_VIEWS * currentPageNr;
                if (index < filteredCards.Count)
                    cardsToShow[i] = filteredCards.ElementAt(index);
            }
            return cardsToShow;
        }

        private void initCardViews()
        {
            setupCardViewPanel();
            setupHeroCardView(new Card(CardTemplate.Bhewas));
            
            cardViews = new CardView[NR_OF_CARD_VIEWS];

            refilter();
            Card[] cardsToShow = getCardsFromPage();
            
            layoutCardViews(cardsToShow);
            addElement(cardViewPanel);
            setupHoverCardView();
        }

        private void setupCardViewPanel()
        {
            cardViewPanel = new Square();
            cardViewPanel.setSize(CARD_VIEW_PANEL_WIDTH, CARD_VIEW_PANEL_HEIGHT);
            cardViewPanel.setLocation(CARD_VIEW_PANEL_X, CARD_VIEW_PANEL_Y);
            cardViewPanel.Backcolor = CARD_PANEL_BACKCOLOR;
        }

        private void setupHoverCardView()
        {
            hoverView = new CardView(new Card(CardTemplate.Abolish));
            hoverView.Width = HOVER_VIEW_WIDTH;
            hoverView.X = HOVER_VIEW_X;
            hoverView.Y = HOVER_VIEW_Y;
            addElement(hoverView);

            if (cardViews == null) throw new Exception("you must setup hovercardview after cardviews");
            foreach(CardView cv in cardViews)
            {
                if (cv != null)
                {
                    cv.mouseEnter += (__) =>
                    {
                        setupHoverCardView(cv.card);
                    };
                }
            }
            heroCardView.mouseEnter += (__) =>
            {
                setupHoverCardView(heroCardView.card);
            }; 
        }

        private void setupHoverCardView(Card c)
        {
            removeElement(hoverView);
            hoverView = new CardView(new Card(c.template));
            //hoverView.Width = NEXT_PAGE_BUTTON_X - FRAME_WIDTH;
            hoverView.X = HOVER_VIEW_X;
            hoverView.Y = HOVER_VIEW_Y;
            addElement(hoverView);
        }

       

        private void setupHeroCardView(Card c)
        {
            heroCardView = new CardView(c);
            heroCardView.setLocation(HERO_VIEW_X, HERO_VIEW_Y);
            heroCardView.Width = HERO_VIEW_WIDTH;
            heroCardView.Backimege = new Imege(TextureLoader.cardArt(c.template));
            currentHero = c;
            addElement(heroCardView);
        }

        private void layoutCardViews(Card[] cards)
        {
            int x = 0;
            int y = 0;
            for (int i = 0; i < cards.Length; i++)
            {

                if (cards[i] != null) //should work without this but it doesnt (: problem for later  :)))) (:
                {
                    cardViews[i] = new CardView(cards[i]);

                    cardViews[i].Width = CARD_VIEW_WIDTH;

                    if (x >= cardViewPanel.Width)
                    {
                        y += CARD_VIEW_STRIDE_Y;
                        x = 0;
                    }
                    cardViews[i].setLocation(x, y);
                    x += CARD_VIEW_STRIDE_X;
                    cardViewPanel.addChild(cardViews[i]);

                    int i1 = i;
                    cardViews[i].clicked += (__) =>
                    {
                        if (cardViews[i1].card.isHeroic)
                        {
                            setupHeroCardView(cardViews[i1].card);
                        }
                        else
                        {
                            addToDeck(cardViews[i1].card.template);
                        }            
                    };
                    
                    cardViews[i].mouseEnter += (__) =>
                    {
                        setupHoverCardView(cardViews[i1].card);
                    };
                }
            }
        }

        #endregion

        private void addToDeck(CardTemplate ct)
        {
            if (deckConstraints.willBeLegal(currentHero.template, cardList.Select(c => c.template).ToArray(), ct))
            {
                cardList.addTop(new Card(ct));
                setupStatsThingy();
            }
        }

        private void removeFromDeck(Card card)
        {
            cardList.remove(card);
            setupStatsThingy();
        }

        
    }
}