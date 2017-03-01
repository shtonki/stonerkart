using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stonerkart
{
    /// <summary>
    /// Interface to hack the attribute of being clickable in the interface. 
    /// </summary>
    interface Clickable
    {
        /// <summary>
        /// When this Clickable is clicked in the interface it yields a Stuff.
        /// </summary>
        /// <returns>The Stuff generated from the state/configuration of the Clickable</returns>
         Stuff getStuff();
    }
}
