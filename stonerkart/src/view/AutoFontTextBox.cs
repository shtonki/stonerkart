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

    public class AutoFontTextBox : Panel
    {
        public bool drag = false;
        public bool enab = false;
        private int m_opacity = 100;

        private int alpha;
        public AutoFontTextBox()
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            SetStyle(ControlStyles.Opaque, true);
            this.BackColor = Color.Transparent;
            TextChanged += (sender, args) => txt = Text;
        }

        public int Opacity
        {
            get
            {
                if (m_opacity > 100)
                {
                    m_opacity = 100;
                }
                else if (m_opacity < 1)
                {
                    m_opacity = 1;
                }
                return this.m_opacity;
            }
            set
            {
                this.m_opacity = value;
                if (this.Parent != null)
                {
                    Parent.Invalidate(this.Bounds, true);
                }
            }
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle = cp.ExStyle | 0x20;
                return cp;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Rectangle bounds = new Rectangle(0, 0, this.Width - 1, this.Height - 1);

            Color frmColor = this.Parent.BackColor;
            Brush bckColor = default(Brush);

            alpha = (m_opacity * 255) / 100;

            if (drag)
            {
                Color dragBckColor = default(Color);

                if (BackColor != Color.Transparent)
                {
                    int Rb = BackColor.R * alpha / 255 + frmColor.R * (255 - alpha) / 255;
                    int Gb = BackColor.G * alpha / 255 + frmColor.G * (255 - alpha) / 255;
                    int Bb = BackColor.B * alpha / 255 + frmColor.B * (255 - alpha) / 255;
                    dragBckColor = Color.FromArgb(Rb, Gb, Bb);
                }
                else
                {
                    dragBckColor = frmColor;
                }

                alpha = 255;
                bckColor = new SolidBrush(Color.FromArgb(alpha, dragBckColor));
            }
            else
            {
                bckColor = new SolidBrush(Color.FromArgb(alpha, this.BackColor));
            }

            if (this.BackColor != Color.Transparent | drag)
            {
                g.FillRectangle(bckColor, bounds);
            }

            g.DrawString(txt, f,Brushes.Black, new Rectangle(new Point(), Size), new StringFormat(StringFormatFlags.FitBlackBox));
            
            
            bckColor.Dispose();
            g.Dispose();
            //base.OnPaint(e);
        }

        protected override void OnBackColorChanged(EventArgs e)
        {
            if (this.Parent != null)
            {
                Parent.Invalidate(this.Bounds, true);
            }
            base.OnBackColorChanged(e);
        }

        protected override void OnParentBackColorChanged(EventArgs e)
        {
            this.Invalidate();
            base.OnParentBackColorChanged(e);
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
