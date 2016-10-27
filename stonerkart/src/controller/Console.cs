using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stonerkart
{
    static class Console
    {
        public static void print(params object[] o)
        {
            printEx(false, o);
        }

        public static void printLine(params object[] o)
        {
            printEx(true, o);
        }

        public static void printEx(bool newLine, params object[] o)
        {
            if (o == null || o.Length == 0)
            {
                throw new Exception();
            }
            StringBuilder sb = new StringBuilder();
            foreach (object s in o)
            {
                sb.Append(s.ToString());
                sb.Append(" ");
            }

            if (newLine) sb.Append(Environment.NewLine);

            Controller.print(sb.ToString());
        }
    }
}
