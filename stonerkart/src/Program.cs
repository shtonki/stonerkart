﻿
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
        public static bool design;

        static void Main(string[] args)
        {
            //System.AppDomain.CurrentDomain.UnhandledException += G.clapTrap;
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
            Controller.startup();
        }
    }
}
