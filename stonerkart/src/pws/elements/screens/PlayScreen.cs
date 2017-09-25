using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stonerkart
{
    class PlayScreen : Screen
    {
        public PlayScreen() : base()
        {
            var backButton = new Button(120, 40);
            addElement(backButton);
            backButton.Backimege = new MemeImege(Textures.buttonbg2, 43985);
            backButton.Border = new AnimatedBorder(Textures.border0, 4);
            backButton.X = 20;
            backButton.Y = 20;
            backButton.Text = "Back";
            backButton.clicked += a => GUI.transitionToScreen(GUI.mainMenuScreen);

            Button botgame = new Button(30, 30);
            addElement(botgame);
            botgame.setLocation(400, 400);
            botgame.Text = "botgame";
            botgame.clicked += a =>
            {
                GameRules grs = new GameRules(MapConfiguration.JasinHex);
                var gsi = new GameSetupInfo(grs, -1, 420, true, new[] { Controller.user.Name, "Skurk" });
                Controller.StartGame(gsi);
            };
        }

        protected override IEnumerable<MenuEntry> generateMenuEntries()
        {
            return new MenuEntry[] { };
        }
    }
}
