﻿using System;
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
            menu = new Square(Frame.BACKSCREENWIDTH, Frame.MENUHEIGHT);
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
            lock (elements)
            {
                elements.Add(element);
            }
            element.screen = this;
        }

        public void removeElement(GuiElement element)
        {
            lock (elements)
            {
                elements.Remove(element);
            }
        }

        public void addWinduh(Winduh winduh)
        {
            addElement(winduh);
            winduh.moveTo(MoveTo.Center, MoveTo.Center);
        }
    }
}
