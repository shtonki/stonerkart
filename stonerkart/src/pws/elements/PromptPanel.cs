using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stonerkart
{
    class PromptPanel : Square
    {

        public Square promptText;
        public Button[] buttons;

        public PromptPanel(int width, int height) : base(width, height)
        {
            Backcolor = Color.Chocolate;

            promptText = new Square();
            promptText.Backcolor = Color.AliceBlue;
            addChild(promptText);
            promptText.Text = "Your prompt goes here.";

            buttons = new Button[buttonCount];
            for (int i = 0; i < buttonCount; i++)
            {
                Button b = new Button(0, 0);
                buttons[i] = b;
                addChild(b);
            }

            layoutStuff();
        }

        private const int buttonCount = 4;
        private const int buttonColumns = 2;


        private void layoutStuff()
        {
            int padding = Width/15;

            int ptWidth = Width - padding*2;
            int ptHeight = Height/2 - padding*2;
            promptText.setSize(ptWidth, ptHeight, new MultiLineFitLayout(ptHeight/4));
            promptText.moveTo(MoveTo.Center, padding);

            int buttonWidth = (Width - padding*2);
        }
    }
}
