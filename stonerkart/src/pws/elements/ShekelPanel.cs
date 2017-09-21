using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stonerkart
{
    class ShekelPanel : Square
    {
        private Square shekelsCount { get; }

        public ShekelPanel(int width, int height) : base(width, height)
        {
            var shekelIcon = new Square(height, height);
            addChild(shekelIcon);
            shekelIcon.Backimege = new Imege(Textures.iconShekel);

            shekelsCount = new Square(width - shekelIcon.Width - 20, Height);
            addChild(shekelsCount);
            shekelsCount.X = width - shekelsCount.Width - 10;
            shekelsCount.TextLayout = new SingleLineFitLayout(Justify.Left);
        }

        public void setPrice(int price)
        {
            shekelsCount.Text = G.shekelsToString(price);
        }
    }
}
