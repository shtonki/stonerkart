using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stonerkart
{
    class PlayerPanel : Square, Observer<ManaPoolChanged>
    {
        private Square manaSquare;
        private Square[][] manaPoolOrbs;

        private const int rows = 6;
        private const int columns = 6;


        public PlayerPanel(int width, int height) : base(width, height)
        {
            manaSquare = new Square();
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

            layoutstuff();
        }

        private void layoutstuff()
        {
            manaSquare.Width = Width - 20;
            manaSquare.Height = Height - 20;
            manaSquare.X = 10;
            manaSquare.Y = 10;

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
        }

        public void notify(object o, ManaPoolChanged t)
        {
            ManaPool pool = t.manaPool;
            for (int i = 0; i < columns; i++)
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
    }
}
