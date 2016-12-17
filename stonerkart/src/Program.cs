
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;

namespace stonerkart
{
    class Program
    {
        static void Main(string[] args)
        {
            System.AppDomain.CurrentDomain.UnhandledException += G.clapTrap;
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
            Controller.startup();
        }
    }
}
