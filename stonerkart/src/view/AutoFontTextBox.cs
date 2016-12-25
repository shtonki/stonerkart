using System;
using System.Drawing;
using System.Threading;
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
        private int textMargin;
        private Font f = null;



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

        private string fontName = "Comic Sans MS";
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

        public void setFont(string name)
        {
            fontName = name;
            fixEx();
            Refresh();
        }

        private int inc = 0;

        private ManualResetEventSlim parallelHackWaiter = new ManualResetEventSlim(false);
        private Font parallelHackValue = null;

        private Font[] fonts = new Font[100];

        private Font loadFont(int size)
        {
            if (fonts[size] == null)
            {
                fonts[size] = new Font("Lucid Sans Unicode", size, FontStyle.Bold);
            }
            return fonts[size];
        }

        private void parallelHack(object o)
        {
            int AdjustedSize = (int)o;
            Panel p = new Panel();
            using (Graphics g = Graphics.FromHwnd(IntPtr.Zero))
            {
                var testFont = loadFont(AdjustedSize);

                int df, ff;
                var d = g.MeasureString(txt, testFont, Size,
                    new StringFormat(StringFormatFlags.LineLimit), out df, out ff);
                if (Size.Width > d.Width && Size.Height > d.Height && df == txt.Length)
                {
                    textMargin = (int)(Size.Width - d.Width);
                    parallelHackValue = testFont;
                    parallelHackWaiter.Set();
                }
            }
        }

        private void fixEx()
        {
            using (Graphics g = Graphics.FromHwnd(IntPtr.Zero))
            {
                for (int AdjustedSize = 33; AdjustedSize >= 3; AdjustedSize -= 3)
                {
                    var testFont = loadFont(AdjustedSize);

                    int df, ff;
                    var d = g.MeasureString(txt, testFont, Size,
                        new StringFormat(StringFormatFlags.LineLimit), out df, out ff);
                    if (Size.Width > d.Width && Size.Height > d.Height && df == txt.Length)
                    {
                        f = testFont;
                        break;
                    }

                    /*
                    while (parallelHackValue == null)
                    {
                        ThreadPool.QueueUserWorkItem(new WaitCallback(parallelHack), 
                            AdjustedSize);
                    }

                    parallelHackWaiter.Wait();
                    f = parallelHackValue;
                    */
                }
            }
        }
    }
}
