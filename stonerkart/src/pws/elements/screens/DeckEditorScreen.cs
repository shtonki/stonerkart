using System;
using System.Collections.Generic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stonerkart
{

    class DeckEditorScreen : Screen
    {
        private const int NR_OF_CARD_VIEWS = 10;
        private const int PILE_VIEW_X = Frame.BACKSCREENWIDTH - CARD_VIEW_WIDTH;
        private const int PILE_VIEW_Y = DECK_NAME_BOX_HEIGHT + SAVE_BUTTON_HEIGHT;
        private const int PILE_VIEW_WIDTH = CARD_VIEW_WIDTH;
        private const int PILE_VIEW_HEIGHT = (int)(Frame.BACKSCREENHEIGHT * 0.8);
        private const int CARD_VIEW_WIDTH = Frame.BACKSCREENWIDTH / 8;
        private const int CARD_VIEW_WIDTHd2 = CARD_VIEW_WIDTH / 2;
        private const int DECK_NAME_BOX_WIDTH = CARD_VIEW_WIDTH;
        private const int DECK_NAME_BOX_HEIGHT = (int)(Frame.BACKSCREENHEIGHT * 0.05);
        private const int SAVE_BUTTON_HEIGHT = DECK_NAME_BOX_HEIGHT / 2;
        private const int SAVE_BUTTON_WIDTH = PILE_VIEW_WIDTH / 2;
        private const int SAVE_BUTTON_X = PILE_VIEW_X;
        private const int SAVE_BUTTON_Y = DECK_NAME_BOX_HEIGHT;


        public DeckEditorScreen() : base(new Imege(Textures.artAbolish))
        {
            PileView pileView = setupPileView();
            CardList cardList = setupCardList(pileView);
            CardView[] cardViews = setupCardViews(cardList);
            DeckContraints deckConstraints = new DeckContraints(Format.Standard);
            setupMouseListeners(pileView, cardList, cardViews, deckConstraints);
            List<Button> buttons = setupButtons();
        }

        #region setups

        private List<Button> setupButtons()
        {
            List<Button> bs = new List<Button>();
            InputBox deckNameBox = new InputBox(DECK_NAME_BOX_WIDTH, DECK_NAME_BOX_HEIGHT);
            deckNameBox.setLocation(PILE_VIEW_X, 0);
            addElement(deckNameBox);
            Button saveButton = new Button();
            saveButton.setLocation(SAVE_BUTTON_X, SAVE_BUTTON_Y);
            saveButton.Backimege = new Imege(Textures.artAlterTime);
            Button loadButton = new Button();
            loadButton.setLocation(SAVE_BUTTON_X + SAVE_BUTTON_WIDTH, SAVE_BUTTON_Y);
            loadButton.Backimege = new Imege(Textures.artAlterTime);
            addElement(loadButton);
            
            addElement(saveButton);


            return bs;
        }

        private void setupMouseListeners(PileView pv, CardList cl, CardView[] cvs, DeckContraints dcs)
        {
            pv.mouseDown += (a) =>
            {
                var cardView = pv.viewAtClick(a);
                if (cardView != null) cl.remove(cardView.card);
            };

            for (int i = 0; i < NR_OF_CARD_VIEWS; i++)
            {
                int i1 = i;
                cvs[i].clicked += (__) =>
                {
                    addToDeck(cvs[i1].card.template, cl, dcs);
                };
            }
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
            pv.Height = (int)(PILE_VIEW_HEIGHT);
            pv.Width = PILE_VIEW_WIDTH;
            pv.setLocation(PILE_VIEW_X, PILE_VIEW_Y);
            pv.Columns = 1;
            addElement(pv);

            return pv;
        }

        private CardView[] setupCardViews(CardList cl)
        {
            CardView[] cvs = new CardView[NR_OF_CARD_VIEWS];
            //int spacing = 20;
            const int originX = 000;
            const int originY = 300;
            const int strideX = 250;
            const int strideY = 350;
            int x = originX;
            int y = originY;
            for (int i = 0; i < NR_OF_CARD_VIEWS; i++)
            {
                int i1 = i;
                cvs[i] = new CardView(new Card((CardTemplate)i));
                cvs[i].Width = CARD_VIEW_WIDTH;

                if (i == NR_OF_CARD_VIEWS / 2)
                {
                    y += strideY;
                    x = originX;
                }
                x += strideX;
                cvs[i].setLocation(x, y);

                addElement(cvs[i]);
            }
            return cvs;
        }
        #endregion

        private void addToDeck(CardTemplate ct, CardList cardList, DeckContraints dcs)
        {
            if (dcs.willBeLegal(CardTemplate.Shibby_sShtank, cardList.Select(c => c.template).ToArray(), ct))
            {
                cardList.addTop(new Card(ct));
            }
        }
    }
}