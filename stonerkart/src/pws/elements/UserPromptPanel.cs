using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stonerkart
{
    class UserPromptPanel : Square
    {
        public Button[] buttons { get; }

        public UserPromptPanel(int width, int height, int buttonheight, string question, ButtonOption[] options, PublicSaxophone sax = null) : base(width, height)
        {
            int buttonwidth = width / options.Length;

            Backimege = new MemeImege(Textures.buttonbg0);

            Square text = new Square(width, height - buttonheight);
            text.TextLayout = new MultiLineFitLayout(50);
            text.Text = question;
            addChild(text);

            buttons = new Button[options.Length];

            for (int i = 0; i < options.Length; i++)
            {
                int i1 = i;
                Button b = buttons[i] = new Button(buttonwidth, buttonheight);
                addChild(b);
                b.Text = options[i].ToString();
                b.X = i * buttonwidth;
                b.Y = height - buttonheight;
                if (sax != null) b.clicked += a => sax.answer(options[i1]);
                b.Backcolor = Color.Silver;
                b.Border = new SolidBorder(4, Color.Black);
            }
        }
    }
}
