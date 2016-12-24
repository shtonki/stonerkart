
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace stonerkart
{
    class Program
    {
        static void Main(string[] args)
        {
            var v = G.range(4, 7);

            //System.AppDomain.CurrentDomain.UnhandledException += G.clapTrap;
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
            Controller.startup();
        }
    }
}
