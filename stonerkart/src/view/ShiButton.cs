using System;
using System.Windows.Forms;

namespace stonerkart
{
    public class Shibbutton : Button, Clickable
    {
        private object hackEx() { return Text; }
        private Func<object> f;

        public Shibbutton()
        {
            f = hackEx;
            Click += (sender, args) => Controller.clicked(this);
        }

        public void setStuffer(Func<object> f)
        {
            this.f = f;
        }

        public Stuff getStuff()
        {
            return new ShibbuttonStuff(Text);
        }
    }

    class ShibbuttonStuff : Stuff
    {
        public string text { get; }

        public ShibbuttonStuff(string text)
        {
            this.text = text;
        }
    }
}