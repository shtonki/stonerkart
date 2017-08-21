using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stonerkart
{
    class Button : Square
    {
        private static Color pressedColor = Color.FromArgb(50, 50, 50, 50);

        public Button(int width, int height) : base(width, height)
        {
        }

        public override void draw(DrawerMaym dm)
        {
            base.draw(dm);

            if (pressed)
            {
                dm.fillRectange(pressedColor, 0, 0, width, height);
            }
        }
    }

    enum ButtonOption
    {
        NOTHING,

        Nigra,
        Yes,
        No,
        OK,
        Cancel,
        Pass,
    }
}
