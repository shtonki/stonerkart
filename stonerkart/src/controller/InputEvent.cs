using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stonerkart
{
    class InputEvent
    {
        public Clickable source { get; }
        public Stuff stuff { get; }

        public InputEvent(Clickable source, Stuff stuff)
        {
            this.source = source;
            this.stuff = stuff;
        }
    }

    public interface Stuff
    {
        
    }

    class InputEventFilter
    {
        private Func<Clickable, object, bool> f;

        public InputEventFilter(Func<Clickable, object, bool> f)
        {
            this.f = f;
        }

        public bool filter(InputEvent e)
        {
            return f(e.source, e.stuff);
        }
    }
}
