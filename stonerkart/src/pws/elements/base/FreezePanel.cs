using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stonerkart
{
    class FreezePanel : Square
    {
        public FreezePanel(GuiElement content) : base(Frame.AVAILABLEWIDTH, Frame.AVAILABLEHEIGHT)
        {
            Square background = new Square(Width, Height);
            addChild(background);
            background.Backcolor = Color.FromArgb(100, 50, 50, 250);

            addChild(content);
            content.moveTo(MoveTo.Center, MoveTo.Center);
        }
    }
}
