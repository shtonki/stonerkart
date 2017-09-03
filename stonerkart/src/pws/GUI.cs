using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace stonerkart
{
    static class GUI
    {
        public static Frame frame { get; private set; }

        public static LoginScreen loginScreen { get; } = new LoginScreen();
        public static MainMenuScreen mainMenuScreen { get; } = new MainMenuScreen();
        public static DeckEditorScreen deckeditorScreen { get; } = new DeckEditorScreen();
        public static ShopScreen shopScreen { get; } = new ShopScreen();

        public static void launch()
        {
            ManualResetEventSlim loadre = new ManualResetEventSlim();
            Thread t = new Thread(launchEx);
            t.Start(loadre);
            loadre.Wait();
        }

        private static void launchEx(object o)
        {
            ManualResetEventSlim loadre = (ManualResetEventSlim)o;
            frame = new Frame(1280, 720, loadre, Program.design);
            frame.Run(100, 0);
        }

        public static void transitionToScreen(Screen s)
        {
            frame.setScreen(s);
        }
    }
}
