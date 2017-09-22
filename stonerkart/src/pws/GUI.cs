using System;
using System.Collections.Generic;
using System.Drawing;
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
        public static PlayScreen playScreen { get; } = new PlayScreen();

        public static ButtonOption promptUser(string question, params ButtonOption[] options)
        {
            if (options.Length == 0) throw new Exception();

            PublicSaxophone sax = new PublicSaxophone(o => true);
            UserPromptPanel userPromptPanel = new UserPromptPanel(500, 250, 80, question, options, sax);

            Winduh w = new Winduh(userPromptPanel);
            frame.activeScreen.addWinduh(w);
            var v = (ButtonOption)sax.call();

            w.close();
            return v;
        }

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
            frame.menuPanel.setEntries(frame.DefaultMenuEntries.Concat(s.menuEntries));
        }
    }
}
