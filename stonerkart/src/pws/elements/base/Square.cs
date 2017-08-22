using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stonerkart
{
    class Square : GuiElement
    {
        private int textPaddingLeft = 0;
        private int textPaddingTop = 0;
        private int textPaddingRight = 0;
        private int textPaddingBottom = 0;

        private Border border = new SolidBorder(0, Color.Fuchsia);

        private Color bclr = Color.Transparent;
        private Imege backimege;

        private string text = "";
        private FontFamille fontFamily = FontFamille.font1;
        private TextLayout textLayout = new SingleLineFitLayout();
        public Color textColor { get; set; } = Color.Black;

        public virtual void setSize(int width, int height, TextLayout layout = null)
        {
            base.Height = height;
            base.Width = width;
            if (layout != null) textLayout = layout;
            onResize();
        }

        public override int Height
        {
            get { return base.Height; }
            set
            {
                base.Height = value;
                onResize();
            }
        }

        public override int Width
        {
            get { return base.Width; }
            set
            {
                base.Width = value;
                onResize();
            }
        }

        public Color Backcolor
        {
            get { return bclr; }
            set { bclr = value; }
        }

        public Imege Backimege
        {
            get { return backimege; }
            set { backimege = value; }
        }

        public string Text
        {
            get { return text; }
            set
            {
                text = value;
                layoutText();
            }
        }

        public FontFamille FontFamily
        {
            get { return fontFamily; }
            set { fontFamily = value; }
        }


        public TextLayout TextLayout
        {
            get { return textLayout; }
            set
            {
                textLayout = value;
                layoutText();
            }
        }

        public virtual int TextPaddingLeft
        {
            get { return textPaddingLeft; }
            set { textPaddingLeft = value; }
        }

        public virtual int TextPaddingTop
        {
            get { return textPaddingTop; }
            set { textPaddingTop = value; }
        }

        public virtual int TextPaddingRight
        {
            get { return textPaddingRight; }
            set { textPaddingRight = value; }
        }

        public virtual int TextPaddingBottom
        {
            get { return textPaddingBottom; }
            set { textPaddingBottom = value; }
        }

        public virtual Border Border
        {
            get { return border; }
            set
            {
                border = value;
                layoutText();
            }
        }

        public Square() : base(0, 0, 100, 100)
        {
        }

        public Square(int x, int y, int width, int height) : base(x, y, width, height)
        {
        }

        public Square(int width, int height) : base(0, 0, width, height)
        {
        }

        public Square(int x, int y, int width, int height, Color backgroundColor) : base(x, y, width, height)
        {
            Backcolor = backgroundColor;
        }

        public Square(int width, int height, Color backgroundColor) : base(0, 0, width, height)
        {
            Backcolor = backgroundColor;
        }

        public Square(int width, int height, string text) : base(0, 0, width, height)
        {
            Text = text;
        }

        public override void onResize(resizeEventStruct args)
        {
            base.onResize(args);
            layoutText();
        }

        private void onResize()
        {
            layoutText();
        }

        private void layoutText()
        {
            lock (this)
            {
                laidText = TextLayout.layout(
                    Text, 
                    Width - TextPaddingLeft - TextPaddingRight - border.thickness * 4,
                    Height - TextPaddingTop - TextPaddingBottom - border.thickness * 4, 
                    FontFamily
                    );
            }
        }

        public LaidText laidText;

        protected override void draw(DrawerMaym dm)
        {
            if (Backimege == null)
            {
                dm.fillRectange(Backcolor, 0, 0, width, height);
            }
            else
            {
                dm.drawImege(Backimege, 0, 0, Width, Height);
            }

            laidText?.draw(dm, TextPaddingLeft + border.thickness*2, TextPaddingTop + border.thickness*2, Width, textColor);
            Border?.draw(dm, 0, 0, Width, Height);
        }
    }
}
