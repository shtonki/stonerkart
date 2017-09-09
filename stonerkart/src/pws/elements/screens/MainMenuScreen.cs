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
        private const int buttonWidth = 420;
        private const int buttonHeight = 70;
        private const int buttonPadding = 30;

        private const int squareWidth = 550;
        private const int squareX = 500;
        private const int squareY = 200;



        public MainMenuScreen() : base(new Imege(Textures.background3))
        {
            int buttons = 3;

            int squareheight = buttonPadding + buttons*(buttonPadding + buttonHeight);
            Square buttonsquare = new Square(squareWidth, squareheight);
            addElement(buttonsquare);
            buttonsquare.Backimege = new MemeImege(Textures.buttonbg0, 435908354);
            buttonsquare.Backimege.Alpha = 150;
            buttonsquare.setLocation(squareX, squareY);


            Button playbutton = new Button(buttonWidth, buttonHeight);
            buttonsquare.addChild(playbutton);
            playbutton.Backimege = new MemeImege(Textures.buttonbg2, 423530793);
            playbutton.moveTo(MoveTo.Center, buttonPadding + 0*(buttonPadding + buttonHeight));
            playbutton.Text = "Play";
            playbutton.clicked += a => GUI.transitionToScreen(GUI.playScreen);

            Button shopbutton = new Button(buttonWidth, buttonHeight);
            buttonsquare.addChild(shopbutton);
            shopbutton.Backimege = new MemeImege(Textures.buttonbg2, 423580793);
            shopbutton.moveTo(MoveTo.Center, buttonPadding + 1*(buttonPadding + buttonHeight));
            shopbutton.Text = "Shop";
            shopbutton.clicked += a => GUI.transitionToScreen(GUI.shopScreen);

            Button deckeditorbutton = new Button(buttonWidth, buttonHeight);
            buttonsquare.addChild(deckeditorbutton);
            deckeditorbutton.Backimege = new MemeImege(Textures.buttonbg2, 423580793);
            deckeditorbutton.moveTo(MoveTo.Center, buttonPadding + 2 * (buttonPadding + buttonHeight));
            deckeditorbutton.Text = "Deck Editor";
            deckeditorbutton.clicked += a => GUI.transitionToScreen(GUI.deckeditorScreen);
        }
    }
}
