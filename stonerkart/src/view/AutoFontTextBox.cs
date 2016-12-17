using System;
using System.Drawing;
using System.Windows.Forms;

namespace stonerkart
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.Drawing;
    using System.Windows.Forms;
    using System.Windows.Forms.Design;

    enum Justify
    {
        Left,
        Right,
    }

    class AutoFontTextBox : TransparentPanel
    {

        public Justify justifyText { get; set; }

        public AutoFontTextBox()
        {
            justifyText = Justify.Left;
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            SetStyle(ControlStyles.Opaque, true);
            this.BackColor = Color.Transparent;
            TextChanged += (sender, args) => txt = Text;
            Enabled = false;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;

            Rectangle r = generateRectangle();
            g.DrawString(txt, f,Brushes.Black, r, new StringFormat(StringFormatFlags.FitBlackBox));
        }

        private Rectangle generateRectangle()
        {
            int m;

            switch (justifyText)
            {
                case Justify.Left:
                {
                    m = 0;
                } break;

                case Justify.Right:
                {
                    m = textMargin;
                } break;

                default:
                    throw new Exception();
            }

            return new Rectangle(m, 0, Size.Width, Size.Height);
        }

        private Font f = new Font("Ariel Black", 10);
        private string txt = ":^)";

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            txt = Text;
            fix();
        }

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            fix();
        }

        private void fix()
        {
            this.memeout(fixEx);
        }

        private int textMargin;
        private void fixEx()
        {
            Graphics g = Graphics.FromImage(Properties.Resources.orbDeath); //todo hack
            Font testFont = null;
            for (int AdjustedSize = 20; AdjustedSize >= 4; AdjustedSize--)
            {
                testFont = new Font(f.Name, AdjustedSize, f.Style);

                int df, ff;
                var d = g.MeasureString(txt, testFont, Size,
                    new StringFormat(StringFormatFlags.LineLimit), out df, out ff);
                if (Size.Width > d.Width && Size.Height > d.Height && df == txt.Length)
                {
                    textMargin = (int)(Size.Width - d.Width);
                    break;
                }
            }

            f = testFont;
        }
    }
}
