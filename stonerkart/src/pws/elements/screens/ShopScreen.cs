using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stonerkart
{
    class ProductPackPanel : Square, Observer<UserChanged>
    {
        private static IEnumerable<Packs> displayedPacks { get; }= Enum.GetValues(typeof(Packs)).Cast<Packs>();
        private Packs[] DisplayedPacks = displayedPacks.ToArray();

        private PackPanel[] PackPanels;

        private const int packWidth = 250;
        private const int packPadding = 25;

        public ProductPackPanel(int width, int height) : base(width, height)
        {
            PackPanels = new PackPanel[DisplayedPacks.Length];

            for (int i = 0; i < DisplayedPacks.Length; i++)
            {
                if (i >= DisplayedPacks.Length) break;
                Packs pack = DisplayedPacks[i];
                var pp = PackPanels[i] = new PackPanel(packWidth, Height, new Pack(DisplayedPacks[i]));
                addChild(pp);
                pp.X = packPadding + i * (packWidth + packPadding);
            }
        }

        public void notify(object o, UserChanged t)
        {
            User user = (User)o;
            ReduceResult<Packs> rr = user.ProductCollection.Where(p => p is Pack).Cast<Pack>().Select(p => p.Template).Reduce();

            foreach (var packpanel in PackPanels)
            {
                packpanel.OwnedCount = rr[packpanel.Pack.Template];
            }
        }
    }

    class ShopScreen : Screen
    {
        private ProductPackPanel packsSquare { get; }
        private Button backButton { get; }

        private const int packSquareWidth = 1200;
        private const int packSquareHeight = 600;


        public ShopScreen() : base(new Imege(Textures.background0))
        {
            packsSquare = new ProductPackPanel(packSquareWidth, packSquareHeight);
            addElement(packsSquare);
            packsSquare.X = 200;
            packsSquare.Y = 200;
        }

        public void ripPack(IEnumerable<CardTemplate> ripped)
        {
            var ripper = new RipperPanel(600, 600, ripped);
            //addElement(ripper);
            freeze(ripper);
        }

        public void couple(User user)
        {
            user.AddObserver(packsSquare);
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
            viewedlist.AddObserver(viewed);
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

        public Pack Pack { get; }

        public int OwnedCount
        {
            get { return ownedCount; }
            set
            {
                ownedCount = value;
                ownedCounter.Text = ownedCount.ToString();
            }
        }

        private int ownedCount { get; set; }



        public PackPanel(int width, int height, Pack pack) : base(width, height)
        {
            this.Pack = pack;
            ownedCount = 0;
            //Backimege = new MemeImege(Textures.buttonbg0, 23579485);

            int paddings = 10;
            Color backer = Color.Silver;
            Color backer2 = Color.DarkGray;

            int priceHeight = (int)Math.Round(height/10.0);
            int priceWidth = (int)Math.Round(width/1.5);

            pricePanel = new ShekelPanel(priceWidth, priceHeight);
            addChild(pricePanel);
            pricePanel.setPrice(pack.Price);
            pricePanel.Backcolor = backer;

            buyButton = new Button(width - priceWidth, priceHeight);
            addChild(buyButton);
            buyButton.Border = new SolidBorder(4, Color.Black);
            buyButton.Text = "Buy";
            buyButton.Backcolor = backer2;
            buyButton.X = pricePanel.Width;
            buyButton.clicked += a =>
            {
                Controller.makePurchase(pack);
            };
            
            productImege = new Square(width, height - priceHeight*2 - paddings*2);
            addChild(productImege);
            productImege.Y = pricePanel.Height + paddings;
            productImege.Backimege = new Imege(TextureLoader.packDisplayImage(pack.Template));

            ownedCounter = new Square(width - priceWidth, priceHeight);
            addChild(ownedCounter);
            ownedCounter.Backcolor = backer;
            ownedCounter.Text = OwnedCount.ToString();
            ownedCounter.Y = height - pricePanel.Height;

            ripem = new Button(priceWidth, priceHeight);
            addChild(ripem);
            ripem.Backcolor = backer2;
            ripem.Border = new SolidBorder(4, Color.Black);
            ripem.Text = "Rip One";
            ripem.clicked += a =>
            {
                if (Controller.ripPack(pack.Template))
                {
                    OwnedCount--;
                    ownedCounter.Text = OwnedCount.ToString();
                }
            };
            ripem.Y = height - pricePanel.Height;
            ripem.X = ownedCounter.Width;
        }
    }
}
