using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stonerkart
{
    static class Logger
    {
        private static StringBuilder log = new StringBuilder();
        private const string dumplogFileName = "dumpkart";

        public static void WriteLine(string s, params object[] args)
        {
            string logstring = String.Format(s, args);
            log.AppendLine(logstring);
        }

        public static void ShoutLine(string s, params object[] args)
        {
            Console.WriteLine(s, args);
            WriteLine(s, args);
        }

        public static void DumpLog()
        {
            File.WriteAllText(dumplogFileName, log.ToString());
        }
    }

}
