using System;
using System.Windows.Forms;

namespace stonerkart
{
    class Shibbutton : Button, Clickable
    {
        public ShibbuttonStuff stuff { get; set; }
        //private object hackEx() { return Text; }
        private Func<object> f;

        public Shibbutton() : this(ButtonOption.Nigra)
        {
            
        }

        public Shibbutton(ButtonOption bo)
        {
            setOption(bo);
        }

        public void setOption(ButtonOption bo)
        {
            stuff = new ShibbuttonStuff(bo);
            Text = stuff.option.ToString();
        }

        public Stuff getStuff()
        {
            return stuff;
        }
    }

    enum ButtonOption
    {
        Nigra,
        OK,
        Cancel,
    }

    class ShibbuttonStuff : Stuff
    {
        public ButtonOption option { get; }

        public ShibbuttonStuff(ButtonOption option)
        {
            this.option = option;
        }
    }
}