using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stonerkart
{
    class PostGameScreen : Screen
    {
        public PostGameScreen(Game game, GameEndStruct gameEndStruct) : base(new Imege(Textures.background3))
        {
            Button exit = new Button(150, 100);
            exit.Backcolor = Color.FloralWhite;
            addElement(exit);
            exit.moveTo(MoveTo.Center, 30);
            exit.Border = new AnimatedBorder(Textures.border0, 4);
            exit.clicked += a => GUI.transitionToScreen(GUI.mainMenuScreen);
            exit.Text = "Exit";

            Square bread = new Square(400, 300);
            addElement(bread);
            bread.TextLayout = new MultiLineFitLayout(20);
            bread.moveTo(MoveTo.Center, 200);
            bread.Backimege = new MemeImege(Textures.buttonbg2);
            bread.textColor = Color.MintCream;
            bread.Text = String.Format("{0} won by {1}!", gameEndStruct.winningPlayer, gameEndStruct.reason);
            bread.TextPaddingTop = bread.TextPaddingLeft = 5;
        }

        protected override IEnumerable<MenuEntry> generateMenuEntries()
        {
            return new MenuEntry[] {};
        }
    }
}
