using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stonerkart
{
    class ToggleButton : Button
    {
        private bool toggled;

        public bool Toggled
        {
            get { return toggled; }
            set
            {
                toggled = value;
                setGreying();
            }
        }

        private Square greySquare;


        public ToggleButton(int width, int height) : base(width, height)
        {
            clicked += a => Toggled = !Toggled;
            greySquare = new Square(width, height);
            greySquare.Hoverable = false;
            addChild(greySquare);
            Toggled = true;
        }

        private void setGreying()
        {
            textColor = Toggled ? Color.Black : Color.Gray;

            if (Backimege == null) return;
            Backimege.Alpha = Toggled ? 255 : 150;
        }
    }
}
