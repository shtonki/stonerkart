using System.Threading;

namespace stonerkart
{
    class Game
    {
        public readonly Map map;
        public readonly Player hero;
        public readonly Player villain;
        public readonly Pile test = new Pile();
        public readonly Pile test2 = new Pile();
        public Game(Map map)
        {
            this.map = map;
        }

        public void startGame()
        {
            Thread t = new Thread(loopEx);
            Console.WriteLine("Game Starting...");
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
            Controller.setPromt("");

            var v = Controller.waitFor(new Retard());
            if (v.source is Shibbutton)
            {
                new Card().moveTo(test);
            }
            else if (v.val is Card)
            {
                Controller.setPromt("Play to what tile?");
                var o = Controller.waitFor(new SourceFilter(typeof (TileView)));
                Tile t = (Tile)o.val;
                Card c = (Card)v.val;
                c.moveTo(test2);
                t.play((Card)v.val);
            }
            Controller.redraw();
        }
    }
}
