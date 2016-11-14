using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stonerkart
{
    interface Observer<T>
    {
        void notify(T t);
    }

    abstract class Observable<T>
    {
        private List<Observer<T>> observers = new List<Observer<T>>();

        public void addObserver(Observer<T> o)
        {
            if (!observers.Contains(o))
            {
                observers.Add(o);
            }
        }

        public void unsubscribe(Observer<T> o)
        {
            if (observers.Contains(o))
            {
                observers.Remove(o);
            }
        }

        protected void notify(T t)
        {
            foreach (var o in observers)
                o.notify(t);
        }
    }
}
