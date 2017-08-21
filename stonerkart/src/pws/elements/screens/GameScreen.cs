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
            //
                CardView cv = new CardView(new Card(CardTemplate.Seraph));
                addElement(cv);
            /*/
            pw = new PileView();
            addElement(pw);
            pw.setSize(800, 300);
            //*/
        }
    }
}
