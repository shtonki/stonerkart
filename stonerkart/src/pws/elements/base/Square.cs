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

        private Square tintPanel;
        private Color tintColor = Color.Transparent;

        public virtual void setSize(int newWidth, int newHeight, TextLayout layout = null)
        {
            resizeEventStruct args = new resizeEventStruct(newWidth, newHeight, width, height);
            base.Height = newHeight;
            base.Width = newWidth;
            if (layout != null) textLayout = layout;
            onResize(args);
        }

        public override int Height
        {
            get { return base.Height; }
            set { setSize(Width, value); }
        }

        public override int Width
        {
            get { return base.Width; }
            set { setSize(value, Height); }
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

        public virtual string Text
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

        public Color TintColor
        {
            get { return tintColor; }
            set
            {
                tintColor = value;
                redoTint();
            }
        }

        public Square() : this(100, 100)
        {
        }

        public Square(int width, int height) : this(0, 0, width, height, Color.Transparent)
        {
        }

        public Square(int x, int y, int width, int height, Color backgroundColor) : base(x, y, width, height)
        {
            Backcolor = backgroundColor;
        }

        public override void onResize(resizeEventStruct args)
        {
            base.onResize(args);
            redoTint();
            layoutText();
        }

        private void redoTint()
        {
            if (tintPanel == null)
            {
                if (tintColor == Color.Transparent) return;
                tintPanel = new Square(0, 0, Width, Height, tintColor);
                tintPanel.Hoverable = false;
                addChild(tintPanel);
            }
            else if (tintColor == Color.Transparent)
            {
                if (tintPanel == null) return;
                removeChild(tintPanel);
                tintPanel = null;
            }
            else
            {
                tintPanel.setSize(Width, Height);
                tintPanel.Backcolor = tintColor;
            }
        }

        private void layoutText()
        {
            lock (this)
            {
                laidText = TextLayout.Layout(
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
            dm.fillRectange(Backcolor, 0, 0, width, height);
            if (Backimege != null)
            {
                dm.drawImege(Backimege, 0, 0, Width, Height);
            }

            laidText?.draw(dm, TextPaddingLeft + border.thickness*2, TextPaddingTop + border.thickness*2, Width, textColor);
            Border?.draw(dm, 0, 0, Width, Height);
        }
    }
}
