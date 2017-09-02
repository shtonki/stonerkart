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
        private Square textBox;
        private int textMargin;

        public override string Text
        {
            get { return textBox.Text; }
            set { textBox.Text = value; }
        }

        public InputBox(int width, int height) : base(width, height)
        {
            textBox = new Square(width, height);
            addChild(textBox);
            textBox.TextLayout = new SingleLineLayout();
            textBox.Hoverable = false;
            //textBox.Backcolor = Color.Yellow;

            Selectable = true;
            Backcolor = Color.FloralWhite;
        }

        public void setText(string text)
        {
            textBox.Text = text;
        }
        

        public override Border Border
        {
            get { return textBox.Border; }
            set { textBox.Border = value; }
        }

        public override int TextPaddingLeft
        {
            get { return textBox.TextPaddingLeft; }
            set { textBox.TextPaddingLeft = value; }
        }

        public override int TextPaddingTop
        {
            get { return textBox.TextPaddingTop; }
            set { textBox.TextPaddingTop = value; }
        }

        public override void onResize(resizeEventStruct args)
        {
            base.onResize(args);
            textBox.setSize(args.newWidth, args.newHeight);
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
                if (textBox.Text.Length > 0) textBox.Text = textBox.Text.Substring(0, textBox.Text.Length - 1);
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
                textBox.Text = textBox.Text + c.Value;
                caretBlinkCounter = 0;
            }


            int textWidth = textBox.laidText.characters.Sum(cl => cl.width);
            if (textWidth > Width)
            {
                textShift = Width - textWidth - 8;
            }
            else
            {
                textShift = 0;
            }

            caretPosition = textShift + textWidth;
            textBox.TextPaddingLeft = textMargin + textShift;
        }

        private int textShift;
        private int caretBlinkCounter;

        protected override void draw(DrawerMaym dm)
        {
            base.draw(dm);

            caretBlinkCounter++;
            if (caretBlinkCounter > 80) caretBlinkCounter = 0;

            if (Focused && caretBlinkCounter < 40)
            {
                dm.fillRectange(Color.Black, caretPosition + textBox.X, 5, 5, Height - 10);
            }
        }
    }
}
