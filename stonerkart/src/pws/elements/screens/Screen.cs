using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stonerkart
{
    abstract class Screen
    {
        public Imege background { get; }

        static Screen()
        {
            menu = new Square(Frame.BACKSCREENWIDTH, 50);
            menu.Backimege = new MemeImege(Textures.buttonbg2, 1);
            menu.Y = Frame.BACKSCREENHEIGHT - menu.Height;
        }

        public Screen()
        {
        }

        public Screen(Imege background)
        {
            this.background = background;
        }

        public List<GuiElement> elements { get; } = new List<GuiElement>();

        public IEnumerable<GuiElement> Elements => elements.Concat(new [] { menu } );

        public static Square menu;

        public void addElement(GuiElement element)
        {
            elements.Add(element);
        }
    }
}
