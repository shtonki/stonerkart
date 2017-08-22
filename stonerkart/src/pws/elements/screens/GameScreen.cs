using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stonerkart
{
    class GameScreen : Screen
    {
        public PileView pw1;

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
            //
            
            pw1 = new PileView();
            addElement(pw1);
            pw1.setSize(1700, 300);
            pw1.Y = 300;
            /*/

            int leftPanelWidth = 500;
            int leftPanelHeight = Frame.AVAILABLEHEIGHT;


            Square leftPanel = new Square(leftPanelWidth, leftPanelHeight);
            leftPanel.Backimege = new MemeImege(Textures.buttonbg0);

            var p = new PromptPanel(leftPanelWidth, 500);
            leftPanel.addChild(p);

            addElement(leftPanel);
        }
    }
}
