using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stonerkart
{
    class GameScreen : Screen
    {
        public PileView pw;

        public GameScreen() : base(new Imege(Textures.table0))
        {
            /*/
            int xp = 0;
            for (int i = 0; i < 5; i++)
            {
                CardView cv = new CardView(new Card(CardTemplate.Seraph));
                cv.Height = 100 + i*100;
                cv.X = xp;
                xp += cv.Width;
                addElement(cv);
            }
            /*/
            pw = new PileView();
            addElement(pw);
            pw.setSize(800, 300);
            //*/
        }
    }
}
