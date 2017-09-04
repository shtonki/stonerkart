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

            int width = 500;
            int height = 250;
            int buttonheight = 80;
            int buttonwidth = width/options.Length;

            Square allofit = new Square(width, height);
            allofit.Backimege = new MemeImege(Textures.buttonbg0);

            Square text = new Square(width, height - buttonheight);
            text.TextLayout = new MultiLineFitLayout(50);
            text.Text = question;
            allofit.addChild(text);

            PublicSaxophone sax = new PublicSaxophone(o => true);

            for (int i = 0; i < options.Length; i++)
            {
                int i1 = i;
                Button b = new Button(buttonwidth, buttonheight);
                allofit.addChild(b);
                b.Text = options[i].ToString();
                b.X = i*buttonwidth;
                b.Y = height - buttonheight;
                b.clicked += a => sax.answer(options[i1]);
                b.Backcolor = Color.Silver;
                b.Border = new SolidBorder(4, Color.Black);
            }

            Winduh w = new Winduh(allofit);
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
        }
    }
}
