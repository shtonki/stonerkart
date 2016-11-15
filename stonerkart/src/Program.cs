
using System.Globalization;
using System.Threading;

namespace stonerkart
{
    class Program
    {
        static void Main(string[] args)
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
            Controller.startup();
        }
    }
}
