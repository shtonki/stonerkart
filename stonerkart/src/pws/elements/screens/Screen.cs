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
        protected abstract IEnumerable<MenuEntry> generateMenuEntries();
        public IEnumerable<MenuEntry> menuEntries => generateMenuEntries();


        public Imege background { get; }

        public Screen()
        {
        }

        public Screen(Imege background) : this()
        {
            this.background = background;
        }

        private List<GuiElement> elements { get; } = new List<GuiElement>();

        public IEnumerable<GuiElement> Elements => elements;
        
        public void addElement(GuiElement element)
        {
            lock (elements)
            {
                elements.Add(element);
            }
            element.Screen = this;
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

        private bool frozen => freezePanel != null;
        private FreezePanel freezePanel;
        public void freeze(GuiElement content)
        {
            if (frozen) throw new Exception();
            freezePanel = new FreezePanel(content);
            addElement(freezePanel);
        }

        public void unfreeze()
        {
            if (!frozen) throw new Exception();
            removeElement(freezePanel);
            freezePanel = null;
        }
    }
}
