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

    class AutoFontTextBox : TransparentPanel
    {
        public AutoFontTextBox()
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            SetStyle(ControlStyles.Opaque, true);
            this.BackColor = Color.Transparent;
            TextChanged += (sender, args) => txt = Text;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.DrawString(txt, f,Brushes.Black, new Rectangle(new Point(), Size), new StringFormat(StringFormatFlags.FitBlackBox));
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

        private void fixEx()
        {
            Graphics g = Graphics.FromImage(Properties.Resources.jordanno); //todo hack
            Font testFont = null;
            for (int AdjustedSize = 20; AdjustedSize >= 4; AdjustedSize--)
            {
                testFont = new Font(f.Name, AdjustedSize, f.Style);

                int df, ff;
                var d = g.MeasureString(txt, testFont, Size,
                    new StringFormat(StringFormatFlags.LineLimit), out df, out ff);
                if (Size.Width > d.Width && Size.Height > d.Height && df == txt.Length)
                {
                    break;
                }
            }

            f = testFont;
        }
    }
}
