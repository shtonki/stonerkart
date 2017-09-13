using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stonerkart
{
    class Line : GuiElement
    {

        private int xorg;
        private int yorg;
        private int xend;
        private int yend;
        private Color color;
        private int thickness;

        public Line(int xorg, int yorg, int xend, int yend, Color color, int thickness) : base(0, 0)
        {
            this.xorg = xorg;
            this.yorg = yorg;
            this.xend = xend;
            this.yend = yend;
            this.color = color;
            this.thickness = thickness;
        }

        protected override void draw(DrawerMaym dm)
        {
            dm.drawLine(xorg, yorg, xend, yend, color, thickness);
        }
    }
}
