using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stonerkart
{
    class CardView : Square
    {
        private const int framewidth = 500;
        private const int frameheight = 700;

        public CardView() : base(framewidth, frameheight)
        {
            namebox = new Square();
            namebox.Text = "Rasputin the Mad";
            namebox.TextLayout = new SingleLineFitLayout();
            addChild(namebox);

            breadbox = new Square();
            breadbox.Text = "Whenever Rasputin the Mad attacks you may draw a card.";
            addChild(breadbox);

            movementbox = new Square();
            movementbox.Text = "3";
            movementbox.TextLayout = new SingleLineFitLayout();
            addChild(movementbox);

            ptbox = new Square();
            ptbox.Text = "3/4";
            ptbox.TextLayout = new SingleLineFitLayout();
            addChild(ptbox);

            typebox = new Square();
            typebox.Text = "Human Mystic";
            typebox.TextLayout = new SingleLineFitLayout(Justify.Middle);
            addChild(typebox);

            artbox = new Square();
            artbox.Backimege = new Imege(Textures.A);
            addChild(artbox);

            orbbox = new Square();
            addChild(orbbox);


            topbutton = new Square(0, 0);
            topbutton.clicked += args =>
            {
                this.onClick(args);
            };
            //addChild(topbutton);

            layoutStuff();
        }

        private Square topbutton;

        private  Square namebox;
        private const int nameboxOrigX = 26;
        private const int nameboxOrigY = 9;
        private const int nameboxOrigW = 246;
        private const int nameboxOrigH = 40;
                 
        private  Square breadbox;
        private const int breadboxOrigX = 44;
        private const int breadboxOrigY = 396;
        private const int breadboxOrigW = 408;
        private const int breadboxOrigH = 236;
                 
        private  Square movementbox;
        private const int movementboxOrigX = 32;
        private const int movementboxOrigY = 636;
        private const int movementboxOrigW = 40;
        private const int movementboxOrigH = 48;
                 
        private  Square ptbox;
        private const int ptboxOrigX = 398;
        private const int ptboxOrigY = movementboxOrigY;
        private const int ptboxOrigW = 86;
        private const int ptboxOrigH = 48;
                 
        private  Square typebox;
        private const int typeboxOrigX = 134;
        private const int typeboxOrigY = 639;
        private const int typeboxOrigW = 236;
        private const int typeboxOrigH = 50;
                 
        private  Square artbox;
        private const int artboxOrigX = 52;
        private const int artboxOrigY = 68;
        private const int artboxOrigW = 398;
        private const int artboxOrigH = 298;
                 
        private  Square orbbox;
        private const int orbboxOrigR = 470;
        private const int orbboxOrigY = 20;
        private const int orbboxOrigW = 398;
        private const int orbboxOrigH = 30;

        public void layoutStuff()
        {
            topbutton.setSize(width, height);

            #region fulhack
            int adj = height < 280 ? 2 : 0;
            int adj1 = height < 380 ? 3 : 0;
            #endregion

            var scale = ((double)(height-adj1))/frameheight;

            int orbcount = 3;
            int pad = 1;

            orbbox.clearChildren();

            var orbsize = (int)Math.Round((scale * orbboxOrigH));

            orbbox.X = (int)Math.Round((scale * orbboxOrigR)) - orbcount * orbsize;
            orbbox.Y = (int)Math.Round((scale * orbboxOrigY));
            orbbox.setSize(
                orbcount * (orbsize + pad),
                orbsize
                );

            for (int i = 0; i < orbcount; i++)
            {
                Square orbsquare = new Square(orbsize, orbsize);
                orbbox.addChild(orbsquare);
                orbsquare.X = i * (orbsize + pad);
                orbsquare.Backimege = new Imege(Textures.orbchaos);
            }


            namebox.X = (int)Math.Round((scale * nameboxOrigX));
            namebox.Y = (int)Math.Floor((scale * nameboxOrigY)) - adj;
            namebox.setSize(
                orbbox.X - namebox.X,
                (int)Math.Round((scale * nameboxOrigH))
                );

            breadbox.X = (int)Math.Round((scale * breadboxOrigX));
            breadbox.Y = (int)Math.Round((scale * breadboxOrigY));
            breadbox.setSize(
                (int)Math.Round((scale * breadboxOrigW)),
                (int)Math.Round((scale * breadboxOrigH)),
                new MultiLineFitLayout(height / 17)
                );

            movementbox.X = (int)Math.Round((scale * movementboxOrigX));
            movementbox.Y = (int)Math.Round((scale * movementboxOrigY));
            movementbox.setSize(
                (int)Math.Round((scale * movementboxOrigW)),
                (int)Math.Round((scale * movementboxOrigH))
                );

            ptbox.X = (int)Math.Round((scale * ptboxOrigX));
            ptbox.Y = (int)Math.Round((scale * ptboxOrigY));
            ptbox.setSize(
                (int)Math.Round((scale * ptboxOrigW)),
                (int)Math.Round((scale * ptboxOrigH))
                );

            typebox.X = (int)Math.Round((scale * typeboxOrigX));
            typebox.Y = (int)Math.Round((scale * typeboxOrigY));
            typebox.setSize(
                (int)Math.Round((scale * typeboxOrigW)),
                (int)Math.Round((scale * typeboxOrigH))
                );

            artbox.X = (int)Math.Round((scale * artboxOrigX));
            artbox.Y = (int)Math.Round((scale * artboxOrigY));
            artbox.setSize(
                (int)Math.Round((scale * artboxOrigW)),
                (int)Math.Round((scale * artboxOrigH))
                );
        }

        public override int Width
        {
            get { return width; }
            set
            {
                width = value;
                height = width * frameheight / framewidth;
                layoutStuff();
            }
        }

        public override void setSize(int width, int height, TextLayout layout = null)
        {
            throw new Exception("Use Height or Width to prevent misshaped cards");
        }

        public override int Height
        {
            get { return height; }
            set
            {
                height = value;
                width = height*framewidth/frameheight;
                layoutStuff();
            }
        }

        private const double imgxo = 26.0/framewidth;
        private const double imgxw = 96.0/framewidth;


        public override void draw(DrawerMaym dm)
        {
            //base.draw(dm);
            dm.drawTexture(Textures.cardframegrey, 0, 0, width, height, null, Color.FromArgb(70, 70, 70));
        }
    }
}
