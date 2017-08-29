using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stonerkart
{
    class PlayerPanel : Square, Observer<ManaPoolChanged>, Observer<PileChangedMessage>
    {
        private Square manaSquare;
        private Square[][] manaPoolOrbs;

        private Square[] locationButtons;
        private Square buttonsSquare;

        public Square handButton => locationButtons[0];
        public Square graveyardButton => locationButtons[1];
        public Square deckButton => locationButtons[2];
        public Square displacedButton => locationButtons[3];

        private PileLocation[] locations = new[]
        {PileLocation.Hand, PileLocation.Graveyard, PileLocation.Deck, PileLocation.Displaced,};

        private Textures[] locationTextures = new[]
        {Textures.handButton, Textures.graveyardButton, Textures.deckButton, Textures.displaceButton, };

        private const int rows = 6;
        private const int columns = 6;


        public PlayerPanel(int width, int height) : base(width, height)
        {
            Backcolor = Color.FromArgb(120, 20, 20, 20);
            manaSquare = new Square();
            manaSquare.Backcolor = Color.Silver;
            addChild(manaSquare);

            manaPoolOrbs = new Square[columns][];
            for (int i = 0; i < columns; i++)
            {
                ManaColour c = G.orbOrder[i];
                manaPoolOrbs[i] = new Square[rows];

                for (int j = 0; j < rows; j++)
                {
                    Square s = new Square();
                    s.Backimege = new Imege(TextureLoader.orbTexture(c));
                    s.Backimege.Alpha = 0;
                    manaPoolOrbs[i][j] = s;
                    manaSquare.addChild(s);
                }
            }
            
            buttonsSquare = new Square();
            addChild(buttonsSquare);
            locationButtons = new Square[locations.Length];
            for (int i = 0; i < locationButtons.Length; i++)
            {
                Square s = locationButtons[i] = new Square();
                s.Backimege = new Imege(locationTextures[i]);
                buttonsSquare.addChild(s);
            }

            layoutstuff();
        }

        private void layoutstuff()
        {
            int manaSquareSize = Width;
            manaSquare.Height = manaSquareSize;
            manaSquare.Width = manaSquareSize;
            manaSquare.moveTo(MoveTo.Center, Height - manaSquareSize);

            int buttonWidth = (int)Math.Round(((double)manaSquare.Width) / columns);
            int buttonHeight = (int)Math.Round(((double)manaSquare.Height) / rows);

            for (int i = 0; i < columns; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    Square s = manaPoolOrbs[i][j];
                    s.setSize(buttonWidth, buttonHeight);
                    s.setLocation(i*buttonWidth, j*buttonHeight);
                }
            }

            int locationButtonPadding = 10;
            int availableHeight = Height - manaSquareSize;
            int locationButtonSize = (availableHeight - locationButtonPadding*3)/2;

            int ctr = 0;
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    var btn = locationButtons[ctr++];
                    btn.Border = new SolidBorder(2, Color.AliceBlue);
                    btn.Text = "0";
                    btn.setSize(locationButtonSize, locationButtonSize);
                    btn.setLocation(locationButtonPadding + i*(locationButtonSize + locationButtonPadding),
                        locationButtonPadding + j*(locationButtonSize + locationButtonPadding));
                }
            }
        }

        public void notify(object o, ManaPoolChanged t)
        {
            ManaPool pool = t.manaPool;
            for (int i = 0; i < columns - 1; i++)
            {
                var cur = pool.currentMana(G.orbOrder[i]);
                var max = pool.maxMana(G.orbOrder[i]);
                for (int j = 0; j < rows; j++)
                {
                    Square s = manaPoolOrbs[i][columns - j - 1];
                    int alpha = 0;
                    if (j < max) alpha = 100;
                    if (j < cur) alpha = 255;
                    s.Backimege.Alpha = alpha;
                }
            }
        }

        public void notify(object o, PileChangedMessage t)
        {
            Pile pile = (Pile)o;
            var location = pile.location.pile;
            int ix = 0;
            for (; ix < locations.Length; ix++)
            {
                if (locations[ix] == location) break;
            }
            locationButtons[ix].Text = pile.Count.ToString();
        }
    }
}
