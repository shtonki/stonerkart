﻿using System;
using System.Collections.Generic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace stonerkart
{

    class DeckEditorScreen : Screen
    {
        #region constants

        private const int CARD_VIEW_PANEL_NUMBER_OF_ROWS = 3;

        private const int NR_OF_CARD_VIEWS = 10;
        private const int FRAME_WIDTH = 500;
        private const int FRAME_HEIGHT = 700;
        private const int PILE_VIEW_X = 0;//Frame.BACKSCREENWIDTH - CARD_VIEW_WIDTH;
        private const int PILE_VIEW_Y = 0;//DECK_NAME_BOX_HEIGHT + SAVE_BUTTON_HEIGHT;
        private const int PILE_VIEW_WIDTH = CARD_VIEW_PANEL_WIDTH + PREVIOUS_PAGE_BUTTON_WIDTH;
        private const int PILE_VIEW_HEIGHT = CARD_VIEW_HEIGHT;//(int)(Frame.BACKSCREENHEIGHT * 0.8);
        private const int CARD_VIEW_PANEL_X = 0;
        private const int CARD_VIEW_PANEL_Y = Frame.AVAILABLEHEIGHT-CARD_VIEW_PANEL_HEIGHT;
        private const int CARD_VIEW_X = CARD_VIEW_PANEL_X;
        private const int CARD_VIEW_Y = CARD_VIEW_PANEL_Y;
        private const int CARD_VIEW_STRIDE_X = 250;
        private const int CARD_VIEW_STRIDE_Y = 350;
        private const int CARD_VIEW_WIDTH = Frame.BACKSCREENWIDTH / 8;
        private const int CARD_VIEW_HEIGHT = CARD_VIEW_WIDTH * FRAME_HEIGHT / FRAME_WIDTH;
        private const int CARD_VIEW_WIDTHd2 = CARD_VIEW_WIDTH / 2;
        private const int CARD_VIEW_PANEL_WIDTH = 4*(CARD_VIEW_STRIDE_X-CARD_VIEW_WIDTH) + 2*PREVIOUS_PAGE_BUTTON_WIDTH + 4*CARD_VIEW_WIDTH;
        private const int CARD_VIEW_PANEL_HEIGHT = PREVIOUS_PAGE_BUTTON_HEIGHT;

        private const int DECK_NAME_BOX_WIDTH = CARD_VIEW_WIDTH;
        private const int DECK_NAME_BOX_HEIGHT = (int)(Frame.BACKSCREENHEIGHT * 0.05);
        private const int SAVE_BUTTON_HEIGHT = DECK_NAME_BOX_HEIGHT / 2;
        private const int SAVE_BUTTON_WIDTH = PILE_VIEW_WIDTH / 2;
        private const int SAVE_BUTTON_X = PILE_VIEW_X;
        private const int SAVE_BUTTON_Y = DECK_NAME_BOX_HEIGHT;
        private const int PREVIOUS_PAGE_BUTTON_WIDTH = Frame.BACKSCREENWIDTH / 16;
        private const int PREVIOUS_PAGE_BUTTON_HEIGHT = CARD_VIEW_WIDTH * FRAME_HEIGHT / FRAME_WIDTH + CARD_VIEW_STRIDE_Y; //erf
        private const int PREVIOUS_PAGE_BUTTON_X = CARD_VIEW_X - CARD_VIEW_WIDTHd2;
        private const int PREVIOUS_PAGE_BUTTON_Y = CARD_VIEW_Y;
        private const int NEXT_PAGE_BUTTON_X = CARD_VIEW_X + CARD_VIEW_WIDTH * 5 + (CARD_VIEW_STRIDE_X - CARD_VIEW_WIDTH) * 4;//erf erf
        private const int NEXT_PAGE_BUTTON_Y = CARD_VIEW_Y;
        private const int NEXT_PAGE_BUTTON_WIDTH = PREVIOUS_PAGE_BUTTON_WIDTH;
        private const int NEXT_PAGE_BUTTON_HEIGHT = PREVIOUS_PAGE_BUTTON_HEIGHT;

        private readonly static System.Drawing.Color CARD_PANEL_BACKCOLOR = System.Drawing.Color.FromKnownColor(System.Drawing.KnownColor.Silver);
        private readonly static System.Drawing.Color PAGE_BUTTON_BACKCOLOR = System.Drawing.Color.FromKnownColor(System.Drawing.KnownColor.SaddleBrown);

        #endregion

        private CardList cardList;
        private Card hero;
        private CardView hoverView;
        private PileView pileView;
        private DeckContraints deckConstraints;
        private CardView[] cardViews;
        private List<Card> allCardsEver;
        private List<Card> allHeroes;
        private List<Card> filteredCards;
        private Func<Card, bool> currentFilter;
        private Square cardViewPanel;
        
        private int currentPageNr = 0;

        //todo fix hero saving/loading/ui
        public DeckEditorScreen() : base(new Imege(Textures.artCallToArms))
        {
            

            setupCards();
            setupCardViewsFirstTime();
            
            
            

            pileView = setupPileView();
            cardList = setupCardList(pileView);
            deckConstraints = new DeckContraints(Format.Standard);


            List<Button> buttons = setupButtons();
        }

        private List<Card> filter(Func<Card, bool> filter)
        {
            List<Card> fCards = new List<Card>();
            foreach(var c in allCardsEver)
            {
                if(filter(c) == true)
                {
                    fCards.Add(c);
                }
            }
            return fCards;
        }


        private void addFilter(Func<Card, bool> f)
        {
            currentFilter = c => currentFilter(c) && f(c);
        }

        #region setups

        private void setupCards()
        {

            allCardsEver = new List<Card>();
            var cardTemplates = Enum.GetValues(typeof(CardTemplate)).Cast<CardTemplate>().ToList();
            allCardsEver = cardTemplates.Select(c => new Card(c)).Where(c => c.isToken == false).ToList();

            ///allHeroes = new List<Card>();
            //allHeroes = allCardsEver.Where(c => c.isHeroic).ToList();
            //foreach (var a in allHeroes) System.Console.WriteLine(a);

            currentFilter = new Func<Card, bool>(c => true);
            filteredCards = filter(currentFilter);
        }

        private List<Button> setupButtons()
        {
            List<Button> bs = new List<Button>();

            InputBox deckNameBox = new InputBox(DECK_NAME_BOX_WIDTH, DECK_NAME_BOX_HEIGHT);
            deckNameBox.setLocation(PILE_VIEW_X, 0);
            deckNameBox.setText("My Deck");
            deckNameBox.clicked += (_) =>
            {
                deckNameBox.setText("");
            };
            
            Button saveButton = new Button();
            saveButton.setLocation(SAVE_BUTTON_X, SAVE_BUTTON_Y);
            saveButton.Backcolor = System.Drawing.Color.Red;
            saveButton.Text = "Save";
            saveButton.clicked += (_) => 
            {
                DeckController.saveDeck(new Deck(CardTemplate.Bhewas, cardList.Select(c => c.template).ToArray()), deckNameBox.Text);
            };

            Button loadButton = new Button();
            loadButton.clicked += (_) =>
            {
                Deck d = DeckController.loadDeck("erf");
                cardList.clear();
                foreach (var t in d.templates)
                {
                    cardList.addTop(new Card(t));
                }
            };
            loadButton.Text = "Load";
            loadButton.setLocation(SAVE_BUTTON_X + SAVE_BUTTON_WIDTH, SAVE_BUTTON_Y);
            loadButton.Backcolor = System.Drawing.Color.Red;





            Button previousPageButton = new Button();
            previousPageButton.setLocation(PREVIOUS_PAGE_BUTTON_X, PREVIOUS_PAGE_BUTTON_Y);
            previousPageButton.Width = PREVIOUS_PAGE_BUTTON_WIDTH;
            previousPageButton.Height = PREVIOUS_PAGE_BUTTON_HEIGHT;
            previousPageButton.Backcolor = PAGE_BUTTON_BACKCOLOR;
            previousPageButton.clicked += (_) => 
            {
                if (currentPageNr > 0) currentPageNr--;
                setupCardViews();
            };

            Button nextPageButton = new Button();
            nextPageButton.setLocation(NEXT_PAGE_BUTTON_X, NEXT_PAGE_BUTTON_Y);
            nextPageButton.Width = PREVIOUS_PAGE_BUTTON_WIDTH;
            nextPageButton.Height = PREVIOUS_PAGE_BUTTON_HEIGHT;
            nextPageButton.Backcolor = PAGE_BUTTON_BACKCOLOR;
            nextPageButton.clicked += (_) =>
            {
                if (currentPageNr * NR_OF_CARD_VIEWS + NR_OF_CARD_VIEWS < filteredCards.Count) currentPageNr++;
                setupCardViews();
            };


            addElement(nextPageButton);
            addElement(previousPageButton);
            addElement(deckNameBox);
            addElement(loadButton);
            addElement(saveButton);

            return bs;
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
                if (cardView != null) cardList.remove(cardView.card);
            };
            addElement(pv);

            return pv;
        }


        private void setupCardViewsFirstTime()
        {
            cardViewPanel = new Square();
            cardViewPanel.setSize(CARD_VIEW_PANEL_WIDTH, CARD_VIEW_PANEL_HEIGHT);
            cardViewPanel.setLocation(CARD_VIEW_PANEL_X, CARD_VIEW_PANEL_Y);
            cardViewPanel.Backcolor = CARD_PANEL_BACKCOLOR;

            //setupHeroCardViews();
            setupHoverCardView();
            cardViews = new CardView[NR_OF_CARD_VIEWS];
            
            filteredCards = allCardsEver.Where(currentFilter).ToList();

            Card[] cardsToShow = new Card[NR_OF_CARD_VIEWS];

            for(int i = 0; i < NR_OF_CARD_VIEWS; i++)
            {
                int index = i + NR_OF_CARD_VIEWS * currentPageNr;
                if (index < filteredCards.Count)
                    cardsToShow[i] = filteredCards.ElementAt(index);
            }

            layoutCardViews(cardsToShow);
            addElement(cardViewPanel);
        }

        private void setupHoverCardView()
        {
            hoverView = new CardView(new Card(CardTemplate.Abolish));
            hoverView.Width = NEXT_PAGE_BUTTON_X - FRAME_WIDTH; 
            hoverView.X = PILE_VIEW_WIDTH;
            hoverView.Y = NEXT_PAGE_BUTTON_Y;
            addElement(hoverView);
        }

        private void setupHoverCardView(Card c)
        {
            hoverView = new CardView(new Card(c.template));
            hoverView.Width = NEXT_PAGE_BUTTON_X - FRAME_WIDTH;
            hoverView.X = PILE_VIEW_WIDTH;
            hoverView.Y = NEXT_PAGE_BUTTON_Y;
            addElement(hoverView);
        }

        private void setupCardViews()
        {
            cardViewPanel.clearChildren();

            filteredCards = allCardsEver.Where(currentFilter).ToList();

            Card[] cardsToShow = new Card[NR_OF_CARD_VIEWS];
            for (int i = 0; i < NR_OF_CARD_VIEWS; i++)
            {
                int index = i + NR_OF_CARD_VIEWS * currentPageNr;
                if (index < filteredCards.Count)
                    cardsToShow[i] = filteredCards.ElementAt(index);
            }
            layoutCardViews(cardsToShow);
        }


        private void setupHeroCardViews()
        {
            foreach(var h in allHeroes)
            {
                System.Console.WriteLine("ERF)");
                CardView heroCardView = new CardView(h);
                heroCardView.setLocation(50, 50);
                heroCardView.Width = 400; addElement(heroCardView);
            }
            
            
        }
        [STAThread]
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
                    if (i == NR_OF_CARD_VIEWS / 2)
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
                        addToDeck(cardViews[i1].card.template);
                    };
                    cardViews[i].mouseEnter += (__) =>
                    {
                        //wait for seba to fix mouse enter
                        hoverView.parent.removeChild(hoverView); //erf
                        setupHoverCardView(cardViews[i1].card);
                    };
                }
            }
        }

        #endregion

        private void addToDeck(CardTemplate ct)
        {
            if (deckConstraints.willBeLegal(CardTemplate.Shibby_sShtank, cardList.Select(c => c.template).ToArray(), ct))
            {
                cardList.addTop(new Card(ct));
            }
            else
            {
                //setImage
            }
        }
    }
}