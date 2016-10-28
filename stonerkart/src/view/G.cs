using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stonerkart
{
    static class G
    {
        public static Map map = new Map(2, 2, false, false);

        public static bool valid;

        public static void load(Map m)
        {
            if (valid) throw new Exception();
            map = m;

            valid = true;
        }

        public static void unload()
        {
            valid = false;
        }
    }
}
