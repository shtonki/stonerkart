using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stonerkart
{
    class DeckEditorScreen : Screen
    {
        private CardView[] cards;
        private Square cardSquare;

        private CardList deck;
        private PileView deckView;

        private const int deckViewHeight = 800;
        private const int deckViewWidth = 300;
        private const int deckViewX = 1000;
        private const int deckViewY = 100;
        private const int deckViewMaxPadding = 50;

        private const int cardRows = 3;
        private const int cardCols = 4;
        private const int cardCount = cardRows * cardCols;
        private const int cardSquareWidth = 800;
        private const int cardPadding = 20;

        public DeckEditorScreen()
        {
            deckView = new PileView();
            deckView.setSize(deckViewWidth, deckViewHeight);
            addElement(deckView);
            deckView.Columns = 1;
            deckView.maxPadding = deckViewMaxPadding;
            deckView.setLocation(deckViewX, deckViewY);
            deckView.Backimege = new MemeImege(Textures.buttonbg2);
            deckView.clicked += a =>
            {
                CardView v = deckView.viewAtClick(a);
                if (v != null)
                {
                    removeCardFromDeck(v.card);
                }
            };

            deck = new CardList();
            deck.addObserver(deckView);

            cardSquare = new Square();
            cardSquare.Backcolor = Color.FloralWhite;
            addElement(cardSquare);

            populate(new[]
            {
                CardTemplate.Zap,
                CardTemplate.Zap,
                CardTemplate.Zap,
                CardTemplate.Zap,
                CardTemplate.Zap,
                CardTemplate.Zap,
                CardTemplate.Zap,
                CardTemplate.Zap,
                CardTemplate.Zap,
                CardTemplate.Zap,
                CardTemplate.Zap,
                CardTemplate.Zap,
            });
        }

        private void populate(CardTemplate[] templates)
        {
            if (templates.Length > cardCount) throw new Exception();

            int cvHeight = (cardSquareWidth - cardPadding) / cardRows - cardPadding;
            int cvWidth = CardView.widthFromHeight(cvHeight);
            int cardSquareHeight = cardRows * (cvHeight + cardPadding) + cardPadding;
            cardSquare.clearChildren();
            cardSquare.setSize(cardSquareWidth, cardSquareHeight);

            cards = new CardView[cardRows * cardCols];
            for (int i = 0; i < cardRows; i++)
            {
                for (int j = 0; j < cardCols; j++)
                {
                    int c = i * cardCols + j;
                    if (c > templates.Length) break;

                    CardTemplate ct = templates[c];

                    CardView cv = cards[i * cardCols + j] = new CardView(new Card(templates[c]));
                    cv.Height = cvHeight;
                    cv.X = cardPadding + j * (cardPadding + cvWidth);
                    cv.Y = cardPadding + i * (cardPadding + cvHeight);
                    cardSquare.addChild(cv);
                    cv.clicked += a => addCardToDeck(ct);
                }
            }
        }

        private void addCardToDeck(CardTemplate t)
        {
            deck.addTop(new Card(t));
        }

        private void removeCardFromDeck(Card c)
        {
            deck.remove(c);
        }
    }
}
