using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stonerkart
{
    class GamePromptPanel : Square
    {
        private Square promptText { get; }
        private Button[] buttons { get; }
        private Button[] manaButtons { get; }

        public GamePromptPanel(int width, int height) : base(width, height)
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

            manaButtons = new Button[manaButtonCount];
            for (int i = 0; i < manaButtonCount; i++)
            {
                Button b = new Button(0, 0);
                b.Backimege = new Imege(TextureLoader.orbTexture(G.orbOrder[i]));
                manaButtons[i] = b;
                addChild(b);
            }

            layoutStuff();
        }

        private const int buttonCount = 4;
        private const int buttonColumns = 2;
        private const int manaButtonCount = 7;


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
                    b.Visible = false;
                }
                row++;
            }

            int manaButtonPadding = padding/2;
            int manaButtonWidth = (Width - padding*2 - manaButtonCount*manaButtonPadding + manaButtonPadding)/manaButtonCount;

            for (int i = 0; i < manaButtonCount; i++)
            {
                Button b = manaButtons[i];
                b.X = padding + i*(manaButtonWidth + manaButtonPadding);
                b.Y = buttonOrigY;
                b.setSize(manaButtonWidth, manaButtonWidth);
            }
        }

        public void sub(PublicSaxophone sax)
        {
            foreach (var b in buttons)
            {
                sax.sub(b, g => Enum.Parse(typeof(ButtonOption), g.Text));
            }

            for (int i = 0; i < manaButtons.Length; i++)
            {
                int i0 = i;
                sax.sub(manaButtons[i], g => G.orbOrder[i0]);
            }
        }

        private void hideButtons()
        {
            foreach (var b in buttons) b.Visible = false;
            foreach (var b in manaButtons) b.Visible = false;
        }

        public void prompt(string text)
        {
            hideButtons();
            promptText.Text = text;
        }

        public void promptButtons(params ButtonOption[] labels)
        {
            hideButtons();

            for (int i = 0; i < labels.Length; i++)
            {
                Button b = buttons[i];
                var l = labels[i];
                if (l == ButtonOption.NOTHING) continue;
                b.Text = l.ToString();
                b.Visible = true;
            }
        }

        public void promptManaChoice(bool hideRegularButtons, params ManaColour[] colours)
        {
            if (hideRegularButtons) hideButtons();

            foreach (var c in colours)
            {
                int ix = G.orbOrder.IndexOf(c);
                Button b = manaButtons[ix];
                b.Visible = true;
            }
        }

        public void clear()
        {
            prompt("");
        }
    }
}
