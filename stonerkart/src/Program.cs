
using System;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Serialization;
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace stonerkart
{
    class Program
    {
        public static bool design = false;

        

        static void Main(string[] args)
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");

            AppDomain.CurrentDomain.UnhandledException += CrashHandler;

            Controller.launchGame();
        }

        public static void CrashHandler(object o, UnhandledExceptionEventArgs args)
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
            if (Network.Connected)
            {
                var trackernr = Network.SendCrashReport((Exception)args.ExceptionObject);
                Console.WriteLine(trackernr);
            }

            Logger.WriteLine(args.ExceptionObject.ToString());

            Controller.quit(ExitStatus.Fucked);
        }
    }
}
