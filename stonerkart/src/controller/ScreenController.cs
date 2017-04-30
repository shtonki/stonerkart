using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stonerkart
{
    static class ScreenController
    {
        public static Game transitionToGamePanel(NewGameStruct ngs, bool local)
        {
            Game g = new Game(ngs, local);
            GamePanel gp = new GamePanel(g);
            GameController gc = new GameController(g, gp);
            g.gameController = gc;
            UIController.gameFrame.transitionTo(gp);
            g.startGame();

            return g;
        }

        public static void transitionToMapEditor()
        {
            UIController.gameFrame.transitionTo(new MapEditor());
        }

        public static void transitionToLoginScreen()
        {
            UIController.gameFrame.transitionTo(new LoginScreen());
        }

        public static void transitionToMainMenu()
        {
            UIController.gameFrame.transitionTo(new MainMenuPanel());
        }

        public static void transitionToDeckEditor()
        {
            UIController.gameFrame.transitionTo(new DeckEditorPanel());
        }

        public static void transitionToRankedScreen()
        {
            UIController.gameFrame.transitionTo(new RankedScreen());
        }

        public static void transtitionToPostGameScreen(Game g)
        {
            UIController.gameFrame.transitionTo(new PostGameScreen());
        }
    }
}
