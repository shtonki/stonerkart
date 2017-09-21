using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stonerkart
{
    class ShopScreen : Screen
    {
        private Square packsSquare { get; }
        private Button backButton { get; }

        private const int packSquareWidth = 1200;
        private const int packSquareHeight = 600;
        private const int packWidth = 250;
        private const int packPadding = 25;

        private Packs[] displayedPacks = Enum.GetValues(typeof (Packs)).Cast<Packs>().ToArray();

        public ShopScreen() : base(new Imege(Textures.background0))
        {
            packsSquare = new Square(packSquareWidth, packSquareHeight);
            addElement(packsSquare);
            packsSquare.X = 200;
            packsSquare.Y = 200;

            backButton = new Button(120, 40);
            addElement(backButton);
            backButton.Backimege = new MemeImege(Textures.buttonbg2, 43985);
            backButton.Border = new AnimatedBorder(Textures.border0, 4);
            backButton.X = 20;
            backButton.Y = 20;
            backButton.Text = "Back";
            backButton.clicked += a => GUI.transitionToScreen(GUI.mainMenuScreen);
        }

        public void ripPack(IEnumerable<CardTemplate> ripped)
        {
            var ripper = new RipperPanel(600, 600, ripped);
            //addElement(ripper);
            freeze(ripper);
        }

        public void populatex(IEnumerable<Product> ownedProducts)
        {
            var ownedPacks = ownedProducts.Where(p => p is Pack).Cast<Pack>().ToArray();

            packsSquare.clearChildren();

            for (int i = 0; i < packSquareWidth/(packWidth); i++)
            {
                if (i >= displayedPacks.Length) break;
                Packs pack = displayedPacks[i];
                var pp = new PackPanel(packWidth, packSquareHeight, new Pack(displayedPacks[i]), ownedPacks.Count(p => p.pack == pack));
                packsSquare.addChild(pp);
                pp.X = packPadding + i*(packWidth + packPadding);
            }

        }

        protected override IEnumerable<MenuEntry> generateMenuEntries()
        {
            return new MenuEntry[] {};
        }
    }

    class RipperPanel : Square
    {
        private PileView viewed;
        private CardList viewedlist = new CardList();


        public RipperPanel(int width, int height, IEnumerable<CardTemplate> ripped) : base(width, height)
        {
            int cardWidth = CardView.widthFromHeight(Height);

            Backimege = new MemeImege(Textures.buttonbg0, 4357987);

            viewed = new PileView();
            viewedlist.addObserver(viewed);
            addChild(viewed);
            viewed.Width = Width - cardWidth;
            viewed.Height = Height;
            viewed.Columns = 1;
            viewed.maxPadding = 35;
            viewed.X = cardWidth;

            Button done = new Button(cardWidth, 100);
            addChild(done);
            done.Backimege = new MemeImege(Textures.buttonbg2, 53896735);
            done.Y = Height / 3;
            done.Text = "Rip'd";
            done.clicked += a => Screen.unfreeze();

            foreach (var rip in ripped.Reverse())
            {
                Card c = new Card(rip);
                CardView cv = new CardView(c);
                addChild(cv);
                cv.Height = Height;
                cv.clicked += a =>
                {
                    viewedlist.addTop(c);
                    removeChild(cv);
                };
            }

        }
    }

    class PackPanel : Square
    {
        private Square productImege { get; }
        private ShekelPanel pricePanel { get; }
        private Button buyButton { get; }
        private Square ownedCounter { get; }
        private Button ripem { get; }

        private int ownedCount;

        public PackPanel(int width, int height, Pack product, int initialCount) : base(width, height)
        {
            ownedCount = initialCount;
            //Backimege = new MemeImege(Textures.buttonbg0, 23579485);

            int paddings = 10;
            Color backer = Color.Silver;
            Color backer2 = Color.DarkGray;

            int priceHeight = (int)Math.Round(height/10.0);
            int priceWidth = (int)Math.Round(width/1.5);

            pricePanel = new ShekelPanel(priceWidth, priceHeight);
            addChild(pricePanel);
            pricePanel.setPrice(product.Price);
            pricePanel.Backcolor = backer;

            buyButton = new Button(width - priceWidth, priceHeight);
            addChild(buyButton);
            buyButton.Border = new SolidBorder(4, Color.Black);
            buyButton.Text = "Buy";
            buyButton.Backcolor = backer2;
            buyButton.X = pricePanel.Width;
            buyButton.clicked += a =>
            {
                if (Controller.makePurchase(product))
                {
                    ownedCount++;
                    ownedCounter.Text = ownedCount.ToString();
                }
            };
            
            productImege = new Square(width, height - priceHeight*2 - paddings*2);
            addChild(productImege);
            productImege.Y = pricePanel.Height + paddings;
            productImege.Backimege = new Imege(TextureLoader.packDisplayImage(product.pack));

            ownedCounter = new Square(width - priceWidth, priceHeight);
            addChild(ownedCounter);
            ownedCounter.Backcolor = backer;
            ownedCounter.Text = ownedCount.ToString();
            ownedCounter.Y = height - pricePanel.Height;

            ripem = new Button(priceWidth, priceHeight);
            addChild(ripem);
            ripem.Backcolor = backer2;
            ripem.Border = new SolidBorder(4, Color.Black);
            ripem.Text = "Rip One";
            ripem.clicked += a =>
            {
                if (Controller.ripPack(product.pack))
                {
                    ownedCount--;
                    ownedCounter.Text = ownedCount.ToString();
                }
            };
            ripem.Y = height - pricePanel.Height;
            ripem.X = ownedCounter.Width;
        }
    }
}
