using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OpenTK.Input;

namespace stonerkart
{
    class PublicSaxophone
    {
        private Func<object, bool> filter;
        private ManualResetEventSlim re;
        private object saved;

        public PublicSaxophone(Func<object, bool> filter)
        {
            this.filter = filter;
            re = new ManualResetEventSlim();
        }

        public object call()
        {
            re.Wait();
            return saved;
        }

        public void answer(object o)
        {
            if (filter(o))
            {
                saved = o;
                re.Set();
                unsubAll();
            }
        }

        public void sub<T>(T g, Func<T, object> f) where T : GuiElement
        {
            GuiElement.mouseClickEventHandler v = a => answer(f(g));
            g.clicked += v;
            fs.Add(new Tuple<GuiElement, GuiElement.mouseClickEventHandler>(g, v));
        }

        public void sub<T>(T g, Func<MouseButtonEventArgs, T, object> f) where T : GuiElement
        {
            GuiElement.mouseClickEventHandler v = a => answer(f(a, g));
            g.clicked += v;
            fs.Add(new Tuple<GuiElement, GuiElement.mouseClickEventHandler>(g, v));
        }

        private List<Tuple<GuiElement, GuiElement.mouseClickEventHandler>> fs = new List<Tuple<GuiElement, GuiElement.mouseClickEventHandler>>();

        private void unsubAll()
        {
            foreach (var t in fs)
            {
                t.Item1.clicked -= t.Item2;
            }
        }
    }
}
