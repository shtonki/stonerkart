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
        private static Frame frame;

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

        public static void setScreen(Screen s)
        {
            frame.setScreen(s);
        }
    }
}
