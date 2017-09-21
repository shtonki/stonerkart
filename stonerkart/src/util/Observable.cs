using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stonerkart
{
    public interface Observer<T>
    {
        void notify(object o, T t);
    }
    

    public abstract class Observable<T>
    {
        private List<WeakReference<Observer<T>>> observers = new List<WeakReference<Observer<T>>>();

        public void addObserver(Observer<T> o)
        {
            foreach (var v in observers)
            {
                Observer<T> w = null;
                v.TryGetTarget(out w);
                if (w != null && w == o) return;
            }
            observers.Add(new WeakReference<Observer<T>>(o));

            o.notify(this, default(T));
        }

        public void removeObserver(Observer<T> o)
        {
            WeakReference<Observer<T>> rm = null;
            foreach (var v in observers)
            {
                Observer<T> w = null;
                v.TryGetTarget(out w);
                if (w != null) rm = v;
            }

            if (rm == null) throw new Exception();

            observers.Remove(rm);
        }

        public bool tryUnsubscribe(Observer<T> o)
        {
            WeakReference<Observer<T>> a = null;
            foreach (var v in observers)
            {
                Observer<T> w = null;
                v.TryGetTarget(out w);
                if (w != null && w == o) a = v;
            }
            if (a == null)
            {
                return false;
            }
            observers.Remove(a);
            return true;
        }

        protected void notify(T t)
        {
            foreach (var v in observers)
            {
                Observer<T> w = null;
                v.TryGetTarget(out w);
                w?.notify(this, t);
            }
        }
    }
}
