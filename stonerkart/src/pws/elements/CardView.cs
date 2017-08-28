using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stonerkart
{
    class CardView : Square, Observer<CardChangedMessage>
    {
        public const int framewidth = 500;
        public const int frameheight = 700;

        public Card card { get; }

        public CardView(Card c) : base(framewidth, frameheight)
        {
            card = c;

            topbutton = new Square(0, 0);
            topbutton.clicked += args =>
            {
                this.onClick(args);
            };
            addChild(topbutton);

            namebox = new Square();
            namebox.TextLayout = new SingleLineFitLayout(Justify.Left);
            addChild(namebox);

            breadbox = new Square();
            addChild(breadbox);

            movementbox = new Square();
            movementbox.TextLayout = new SingleLineFitLayout();
            addChild(movementbox);

            ptbox = new Square();
            ptbox.TextLayout = new SingleLineFitLayout();
            addChild(ptbox);

            typebox = new Square();
            typebox.TextLayout = new SingleLineFitLayout(Justify.Middle);
            addChild(typebox);

            artbox = new Square();
            addChild(artbox);

            orbbox = new Square();
            addChild(orbbox);


            //layoutStuff();
            populate(c);
            Height = frameheight;
        }

        #region yup

        private Square topbutton;

        private  Square namebox;
        private const int nameboxOrigX = 26;
        private const int nameboxOrigY = 17;
        private const int nameboxOrigW = 246;
        private const int nameboxOrigH = 40;
                 
        private  Square breadbox;
        private const int breadboxOrigX = 44;
        private const int breadboxOrigY = 402;
        private const int breadboxOrigW = 408;
        private const int breadboxOrigH = 236;
                 
        private  Square movementbox;
        private const int movementboxOrigX = 32;
        private const int movementboxOrigY = 646;
        private const int movementboxOrigW = 40;
        private const int movementboxOrigH = 48;
                 
        private  Square ptbox;
        private const int ptboxOrigX = 398;
        private const int ptboxOrigY = movementboxOrigY;
        private const int ptboxOrigW = 86;
        private const int ptboxOrigH = 48;
                 
        private  Square typebox;
        private const int typeboxOrigX = 134;
        private const int typeboxOrigY = movementboxOrigY;
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

        #endregion

        private IEnumerable<ManaOrb> orbs;

        public void notify(object o, CardChangedMessage t)
        {
            populate((Card)o);
            layoutStuff();
        }

        private void populate(Card c)
        {
            if (card != c) throw new Exception();

            namebox.Text = c.name;
            breadbox.Text = c.breadText;
            movementbox.Text = c.movement.ToString();
            ptbox.Text = c.power + "/" + c.toughness;
            typebox.Text = c.typeText;
            artbox.Backimege = new Imege(TextureLoader.cardArt(c.template));

            orbs = c.castManaCost.orbs;

            Color frameColor;

            if (c.colours.Count > 1)
            {
                frameColor = Color.Gold;
            }
            else
            {
                var v = c.colours[0];

                switch (v)
                {
                    case ManaColour.Chaos: frameColor = Color.DarkRed; break;
                    case ManaColour.Death: frameColor = Color.FromArgb(60, 60, 60); break;
                    case ManaColour.Life: frameColor = Color.White; break;
                    case ManaColour.Might: frameColor = Color.Purple; break;
                    case ManaColour.Nature: frameColor = Color.LightGreen; break;
                    case ManaColour.Order: frameColor = Color.LightSkyBlue; break;
                    case ManaColour.Colourless: frameColor = Color.Silver; break;
                    default: throw new Exception();
                }
            }

            frameImage.brushColor = frameColor;
        }

        public void layoutStuff()
        {
            topbutton.setSize(width, height);

            var scale = ((double)(height))/frameheight;

            var colours = orbs.Where(o => o.colour != ManaColour.Colourless).ToArray();
            var colourlessCount = orbs.Count() - colours.Count();

            var orbcount = colours.Count() + (colourlessCount > 0 ? 1 : 0);

            int pad = 1;

            orbbox.clearChildren();

            var orbsize = (int)Math.Round((scale * orbboxOrigH));

            orbbox.X = (int)Math.Round((scale * orbboxOrigR)) - orbcount * orbsize;
            orbbox.Y = (int)Math.Round((scale * orbboxOrigY));
            orbbox.setSize(
                orbcount * (orbsize + pad),
                orbsize
                );

            int i;
            for (i = 0; i < colours.Count(); i++)
            {
                var o = colours[i];
                Square orbsquare = new Square(orbsize, orbsize);
                orbbox.addChild(orbsquare);
                orbsquare.X = i * (orbsize + pad);
                orbsquare.Backimege = new Imege(TextureLoader.orbTexture(o.colour));
            }

            if (colourlessCount > 0)
            {
                Square orbsquare = new Square(orbsize, orbsize);
                orbbox.addChild(orbsquare);
                orbsquare.X = i * (orbsize + pad);
                orbsquare.Backimege = new Imege(TextureLoader.colourlessTexture(colourlessCount));
            }

            namebox.X = (int)Math.Round((scale * nameboxOrigX));
            namebox.Y = (int)Math.Floor((scale * nameboxOrigY));
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

        public static int heightFromWidth(int width)
        {
            return width * frameheight / framewidth;
        }

        public static int widthFromHeight(int height)
        {
            return height * framewidth / frameheight;
        }

        private const double imgxo = 26.0/framewidth;
        private const double imgxw = 96.0/framewidth;
        private Imege frameImage = new Imege(Textures.cardframegrey);

        protected override void draw(DrawerMaym dm)
        {
            dm.drawImege(frameImage, 0, 0, width, height);
        }
    }
}
