using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stonerkart
{
    class MainMenuScreen : Screen
    {
        public MainMenuScreen()
        {
            InputBox box = new InputBox(500, 200);
            addElement(box);
            Button b = new Button(300, 300);
            addElement(b);
            b.Backcolor = Color.DodgerBlue;
            b.setLocation(500, 500);
            b.clicked += a => Network.challenge(box.Text);
        }
    }
}
