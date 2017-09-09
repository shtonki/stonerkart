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
            var ripper = new RipperOverlay(Frame.BACKSCREENWIDTH, Frame.AVAILABLEHEIGHT, ripped);
            addElement(ripper);
        }

        public void populate(IEnumerable<Packs> ownedPacks)
        {
            packsSquare.clearChildren();

            for (int i = 0; i < packSquareWidth/(packWidth); i++)
            {
                if (i >= displayedPacks.Length) break;
                Packs pack = displayedPacks[i];
                var pp = new PackProductPanel(packWidth, packSquareHeight, new ProductUnion(displayedPacks[i]), ownedPacks.Count(p => p == pack));
                packsSquare.addChild(pp);
                pp.X = packPadding + i*(packWidth + packPadding);
            }

        }
    }

    class RipperOverlay : Square
    {
        private Square goodstuff;

        private PileView viewed;
        private CardList viewedlist = new CardList();

        private const int goodstuffHeight = 600;
        private const int goodstuffWidth = 600;
        private int cardWidth = CardView.widthFromHeight(goodstuffHeight);


        public RipperOverlay(int width, int height, IEnumerable<CardTemplate> ripped) : base(width, height)
        {
            Backcolor = Color.FromArgb(150, 150, 150, 150);

            var xd = new Square(goodstuffWidth + 10, goodstuffHeight + 10);
            addChild(xd);
            xd.moveTo(MoveTo.Center, MoveTo.Center);
            xd.Border = new AnimatedBorder(Textures.border0, 5);

            goodstuff = new Square(goodstuffWidth, goodstuffHeight);
            addChild(goodstuff);
            goodstuff.moveTo(MoveTo.Center, MoveTo.Center);
            goodstuff.Backimege = new MemeImege(Textures.buttonbg0, 4357987);

            viewed = new PileView();
            viewedlist.addObserver(viewed);
            goodstuff.addChild(viewed);
            viewed.Width = goodstuffWidth - cardWidth;
            viewed.Height = goodstuffHeight;
            viewed.Columns = 1;
            viewed.maxPadding = 35;
            viewed.X = cardWidth;

            Button done = new Button(cardWidth, 100);
            goodstuff.addChild(done);
            done.Backimege = new MemeImege(Textures.buttonbg2, 53896735);
            done.Y = goodstuffHeight / 3;
            done.Text = "Rip'd";
            done.clicked += a => screen.removeElement(this);

            foreach (var rip in ripped.Reverse())
            {
                Card c = new Card(rip);
                CardView cv = new CardView(c);
                goodstuff.addChild(cv);
                cv.Height = goodstuffHeight;
                cv.clicked += a =>
                {
                    viewedlist.addTop(c);
                    goodstuff.removeChild(cv);
                };
            }

        }
    }

    class PackProductPanel : Square
    {
        private Square productImege { get; }
        private PricePanel pricePanel { get; }
        private Button buyButton { get; }
        private Square ownedCounter { get; }
        private Button ripem { get; }

        private int ownedCount;

        public PackProductPanel(int width, int height, ProductUnion product, int initialCount) : base(width, height)
        {
            if (!product.isPack) throw new Exception();
            ownedCount = initialCount;
            //Backimege = new MemeImege(Textures.buttonbg0, 23579485);

            int paddings = 10;
            Color backer = Color.Silver;
            Color backer2 = Color.DarkGray;

            int priceHeight = (int)Math.Round(height/10.0);
            int priceWidth = (int)Math.Round(width/1.5);

            pricePanel = new PricePanel(priceWidth, priceHeight);
            addChild(pricePanel);
            pricePanel.setPrice(product.price);
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
            productImege.Backimege = new Imege(TextureLoader.packDisplayImage(product.Pack));

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
                if (Controller.ripPack(product.Pack))
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
