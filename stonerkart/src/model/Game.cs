using System.Threading;

namespace stonerkart
{
    class Game
    {
        public readonly Map map;
        public readonly Player hero;
        public readonly Player villain;
        public readonly Pile test = new Pile();
        public Game(Map map)
        {
            this.map = map;
        }

        public void startGame()
        {
            Thread t = new Thread(loopEx);
            t.Start();
        }

        private void loopEx()
        {
            while (true)
            {
                gameLoop();
            }
        }

        private void gameLoop()
        {
            var v = Controller.waitFor(new ClickableFilter(typeof(Shibbutton), null));
            test.add(new Card());
            Controller.redraw();
        }
    }
}
