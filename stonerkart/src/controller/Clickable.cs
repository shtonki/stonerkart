using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stonerkart
{
    public interface Clickable
    {
        object getStuff();
    }

    class ClickableFilter
    {
        private List<Type> clickableType;
        private List<Type> stuffType;
        private Func<object, bool> f;

        public ClickableFilter()
        {
        }

        public ClickableFilter(Type clickableType, Type stuffType)
        {
            if (clickableType != null) this.clickableType = new List<Type>(new Type[] {clickableType});
            if (stuffType != null)     this.stuffType = new List<Type>(new Type[] {stuffType});
        }

        public ClickableFilter(IEnumerable<Type> clickableTypes, IEnumerable<Type> stuffTypes)
        {
            this.clickableType = new List<Type>(clickableTypes);
            this.stuffType = new List<Type>(stuffTypes);
        }

        public bool filter(Clickable c)
        {
            return
                (
                    (clickableType == null || clickableType.Any(x => x == c.GetType()))
                    &&
                    (stuffType == null || stuffType.Any(x => x == c.getStuff().GetType()))
                    &&
                    (f == null || f(c.getStuff()))
                );
        }
    }
}
