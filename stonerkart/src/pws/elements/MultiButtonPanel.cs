using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stonerkart
{
    class MultiButtonPanel : Square
    {
        private List<Button> buttons { get; } = new List<Button>();

        public void AddButton(Button b)
        {
            
        }

        public void RemoveButton(Button b)
        {
            
        }

        private void LayoutButtons()
        {
            int buttonHeight = Height;
            int buttonWidth = Width/buttons.Count;
            for (int i = 0; i < buttons.Count; i++)
            {
                var b = buttons[i];
                b.Width = buttonWidth;
                b.Height = buttonHeight;
                b.setLocation(i*buttonWidth, 0);
            }
        }
    }
}
