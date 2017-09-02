using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/*
namespace stonerkart
{
    /// <summary>
    /// Mostly defunct afaik
    /// </summary>
    static class Console
    {
        /// <summary>
        /// Prints each element in the given array the Console intersperced with a ' '.
        /// </summary>
        /// <param text="os">List of objects to print</param>
        public static void Write(params object[] os)
        {
            printEx(false, os);
        }

        /// <summary>
        /// Prints each element in the given array the Console intersperced with a ' ' and terminating with a new line.
        /// </summary>
        /// <param text="os">List of objects to print</param>
        public static void WriteLine(params object[] os)
        {
            printEx(true, os);
        }

        /// <summary>
        /// Used internally to format each input and then print it to the console.
        /// </summary>
        /// <param text="newLine">Whether to append a new line at the end of the string.</param>
        /// <param text="os">List of objects to print</param>
        private static void printEx(bool newLine, params object[] os)
        {
            if (os == null || os.Length == 0)
            {
                throw new Exception();
            }
            StringBuilder sb = new StringBuilder();
            foreach (object s in os)
            {
                sb.Append(s.ToString());
                sb.Append(" ");
            }

            if (newLine) sb.Append(Environment.NewLine);

            throw new NotImplementedException();
        }
    }
}
*/