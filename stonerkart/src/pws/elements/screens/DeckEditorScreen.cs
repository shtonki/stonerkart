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
        private const int CARD_VIEW_WIDTH = Frame.BACKSCREENWIDTH / 8;
        private const int CARD_VIEW_WIDTHd2 = CARD_VIEW_WIDTH / 2;

        public DeckEditorScreen() : base(new Imege(Textures.artAbolish))
        {
            PileView pileView = setupPileView();
            CardList cardList = setupCardList(pileView);
            CardView[] cardViews = setupCardViews(cardList);
            DeckContraints deckConstraints = new DeckContraints(Format.Standard);
            setupMouseListeners(pileView, cardList, cardViews, deckConstraints);
        }

        #region setups
        private void setupMouseListeners(PileView pv, CardList cl, CardView[] cvs, DeckContraints dcs)
        {
            pv.mouseDown += (a) =>
            {
                var cardView = pv.viewAt(a.X - pv.AbsoluteX, a.Y - pv.AbsoluteY);
                if(cardView != null) cl.remove(cardView.card);
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
            pv.Height = (int)(Frame.BACKSCREENHEIGHT*0.8);
            pv.Width = CARD_VIEW_WIDTH;
            pv.setLocation(Frame.BACKSCREENWIDTH - pv.Width, 0);
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
            if(dcs.willBeLegal(CardTemplate.Shibby_sShtank, cardList.Select(c => c.template).ToArray(), ct))
            {
                cardList.addTop(new Card(ct));
            }
        }
    }
}