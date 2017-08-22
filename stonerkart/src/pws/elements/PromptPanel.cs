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
            //Backcolor = Color.Chocolate;
            promptText = new Square();
            promptText.Backcolor = Color.FromArgb(100, 120,135,171);
            promptText.Border = new AnimatedBorder(Textures.border0, 4, 0.0002, 0.3);
            addChild(promptText);
            promptText.Text = "Your prompt goes here.";

            buttons = new Button[buttonCount];
            for (int i = 0; i < buttonCount; i++)
            {
                Button b = new Button(0, 0);
                b.Backimege = new MemeImege(Textures.buttonbg2);
                b.Border = new SolidBorder(2, Color.Black);
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

            int rows = (int)Math.Ceiling((decimal)(buttonCount)/buttonColumns);
            int buttonOrigY = promptText.Bottom + padding;
            int buttonWidth = (Width - padding*(buttonColumns+1)) / buttonColumns;
            int buttonHeight = (Height - buttonOrigY - padding*(rows))/rows;

            int row = 0;
            for (int i = 0; i < buttonCount;)
            {
                for (int j = 0; j < buttonColumns; j++)
                {
                    if (i >= buttonCount) continue;
                    Button b = buttons[i++];
                    b.Backcolor = Color.Aqua;
                    b.setSize(buttonWidth, buttonHeight);
                    b.setLocation(
                        padding + j*(buttonWidth + padding), 
                        buttonOrigY + row*(buttonHeight + padding)
                        );
                }
                row++;
            }
        }
    }
}
