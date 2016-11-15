using System;
using System.Drawing;
using System.Windows.Forms;

namespace stonerkart
{
    public class AutoFontTextBox : Label
    {
        private Font f = new Font("Ariel Black", 10);
        private string txt = "1 2 3 4 5 6 7 8 9 10";
        public AutoFontTextBox()
        {
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
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
                testFont = new Font(Font.Name, AdjustedSize, Font.Style);

                int df, ff;
                var d = g.MeasureString(Text, testFont, Size, 
                    new StringFormat(StringFormatFlags.LineLimit), out df, out ff);
                if (Size.Height > d.Height && df == Text.Length)
                {
                    break;
                }
            }

            Font = testFont;
        }
    }
}