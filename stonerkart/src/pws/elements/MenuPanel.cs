using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stonerkart
{
    class MenuPanel : Square
    {
        private int buttonHeightCap = 50;
        private int buttonPadding = 10;

        private Square menu;
        private GuiElement goodstuff;

        public MenuPanel(int width, int height) : base(width, height)
        {
            menu = new Square(Width, Height);
            addChild(menu);
            menu.Backcolor = Color.Black;
            menu.Border = new AnimatedBorder(Textures.border0, 4, 0.0002);
        }

        public void setEntries(IEnumerable<MenuEntry> entries)
        {
            menu.clearChildren();

            int h = (menu.Height - buttonPadding)/entries.Count() - buttonPadding;
            if (h > buttonHeightCap) h = buttonHeightCap;

            int c = 0;
            foreach (var entry in entries)
            {
                Button btn = new Button(Width - buttonPadding*2, h);
                btn.Backcolor = Color.White;
                btn.Text = entry.ButtonText;
                menu.addChild(btn);
                btn.Y = buttonPadding + c++*(buttonPadding + h);
                btn.X = buttonPadding;
                btn.clicked += a =>
                {
                    toEntry(entry);
                };
                entry.doneWithMyBusiness -= doneWithBusiness; //ugly but is the only way i find to ensure that there's only one call to doneWithBusiness when it's ogre
                entry.doneWithMyBusiness += doneWithBusiness;
            }
        }

        private void toEntry(MenuEntry entry)
        {
            if (goodstuff != null) throw new Exception();
            menu.Visible = false;
            goodstuff = entry.GoodStuff;
            addChild(goodstuff);
            setSize(goodstuff.Width, goodstuff.Height);
            moveTo(MoveTo.Center, MoveTo.Center);
        }

        private void doneWithBusiness()
        {
            parent.Visible = false; //hide the menu overlay
            menu.Visible = true;    //show the meny buttons
            removeChild(goodstuff);
            goodstuff = null;
            setSize(menu.Width, menu.Height);
            moveTo(MoveTo.Center, MoveTo.Center);
        }
    }

    internal delegate void MenuEntryDone();

    interface MenuEntry
    {
        string ButtonText { get; }
        GuiElement GoodStuff { get; }
        event MenuEntryDone doneWithMyBusiness;
    }

    class QuitEntry : MenuEntry
    {
        private UserPromptPanel goodstuff;

        public QuitEntry()
        {
            goodstuff = new UserPromptPanel(400, 200, 80, "Really quit Stonerkart?", new [] {ButtonOption.Yes, ButtonOption.No, });
            goodstuff.buttons[0].clicked += a => Controller.quit();
            goodstuff.buttons[1].clicked += a => doneWithMyBusiness?.Invoke();

        }

        public string ButtonText
        {
            get { return "Quit"; }
        }

        public GuiElement GoodStuff
        {
            get { return goodstuff; }
        }
        
        public event MenuEntryDone doneWithMyBusiness;
    }

    class ConcedeEntry : MenuEntry
    {
        private UserPromptPanel goodstuff;

        public ConcedeEntry(int gameid)
        {
            goodstuff = new UserPromptPanel(400, 200, 80, "Really concede?", new[] { ButtonOption.Yes, ButtonOption.No, });
            goodstuff.buttons[0].clicked += a => 
            {

                throw new NotImplementedException("if you weren't expecting too see this you might be in some trouble son");
                /*
                if (gameid == -1)
                {
                    Network.handle(new Message("", Message.MessageType.ENDGAME, new EndGameMessageBody(-1, new GameEndStruct("villain", GameEndStateReason.Surrender))));
                }
                else
                {
                    Network.surrender(gameid, GameEndStateReason.Surrender);
                }
                doneWithMyBusiness?.Invoke();
                */
            };
            goodstuff.buttons[1].clicked += a => doneWithMyBusiness?.Invoke();
        }

        public string ButtonText
        {
            get { return "Concede"; }
        }

        public GuiElement GoodStuff
        {
            get { return goodstuff; }
        }

        public event MenuEntryDone doneWithMyBusiness;
    }
}
