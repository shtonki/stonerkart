using System.Drawing;
using System.Windows.Forms;

namespace stonerkart
{
    class CardView : UserControl
    {
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var g = e.Graphics;

            int W = Size.Width;
            int H = Size.Height;

            g.FillRectangle(Brushes.Chocolate, 0, 0, W, H);


        }
    }
}
