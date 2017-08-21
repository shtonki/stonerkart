using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Input;

namespace stonerkart
{
    class InputBox : Square
    {
        private int caretPosition;

        public InputBox(int width, int height) : base(width, height)
        {
            TextLayout = new SingleLineLayout();
            selectable = true;
            Backcolor = Color.FloralWhite;
        }

        public override void onKeyDown(KeyboardKeyEventArgs args)
        {
            base.onKeyDown(args);

            char? c = null;

            if (args.Key == Key.Space)
            {
                c = ' ';
            }
            else if (args.Key == Key.BackSpace)
            {
                if (Text.Length > 0) Text = Text.Substring(0, Text.Length - 1);
            }
            else
            {
                var v = args.Key.ToString();
                if (v.Length == 1)
                {
                    c = v[0];
                    if (!args.Shift)
                    {
                        c = Char.ToLowerInvariant(c.Value);
                    }
                }
            }

            if (c.HasValue)
            {
                Text = Text + c.Value;
                caretBlinkCounter = 0;
            }


            int textWidth = laidText.xs.Sum(cl => cl.width);
            if (textWidth > Width)
            {
                textPaddingX = Width - textWidth - 8;
            }
            else
            {
                textPaddingX = 0;
            }

            caretPosition = textPaddingX + textWidth;
        }

        private int caretBlinkCounter;

        public override void draw(DrawerMaym dm)
        {
            base.draw(dm);

            caretBlinkCounter++;
            if (caretBlinkCounter > 80) caretBlinkCounter = 0;

            if (focused && caretBlinkCounter < 40)
            {
                dm.fillRectange(Color.Black, caretPosition, 5, 7, Height - 10);
            }
        }
    }
}
