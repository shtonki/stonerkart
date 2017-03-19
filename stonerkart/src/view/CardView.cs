using System;
using System.Drawing;
using System.Windows.Forms;

namespace stonerkart
{
    

    class CardView : StickyPanel, Clickable, Observer<CardChangedMessage>
    {
        private PictureBox art;
        private AutoFontTextBox breadText;
        private AutoFontTextBox nameBox;
        private AutoFontTextBox toughnessBox;
        private AutoFontTextBox powerBox;
        private AutoFontTextBox cardTypeText;
        private AutoFontTextBox castRangeSlashMovementBox;
        private AutoFontTextBox autoFontTextBox1;
        private SetArtPanel setArtPanel1;
        private Panel topBorderPanel;
        private Panel botBorderPanel;
        private Panel lftBorderPanel;
        private PictureBox frameImage;
        private Panel rgtBorderPanel;
        private ManaCostPanel manaCostPanel1;

        public bool showBase { get; set; }

        public Card card { get; private set; }
        
        public CardView() : this(CardTemplate.Belwas)
        { 
        }

        public CardView(CardTemplate ct) : this (new Card(ct))
        {
        }

        public CardView(Card c)
        {
            InitializeComponent();

            powerBox.justifyText = Justify.Right;

            card = c;
            sickmemes();
            glowColour();
        }

        public void setCard(CardTemplate ct)
        {
            card = new Card(ct, null);
            sickmemes();
        }

        public void setCard(Card c)
        {
            card.tryUnsubscribe(this);
            card = c;
            card.addObserver(this);
            sickmemes();
        }

        private void sickmemes()
        {
            Card c = card;
            if (showBase) card = Card.fromTemplate(card.template);

            this.memeout(() =>
            {
                if (card.colours.Count == 1)
                {
                    frameImage.Image = ImageLoader.frameImage(card.colours[0]);
                }
                else
                {
                    frameImage.Image = ImageLoader.frameImage(ManaColour.Multi);
                }
                art.Image = ImageLoader.artImage(card.template);

                nameBox.Text = card.name;
                setArtPanel1.setThings(card.set, card.rarity);

                if (card.isDummy)
                {
                    breadText.Text = card.dummiedAbility.description;
                }
                else
                {
                    breadText.Text = card.breadText;
                    manaCostPanel1.setCost(card.castManaCost);
                    cardTypeText.Text = card.typeText;
                    if (card.race.HasValue)
                    {
                        cardTypeText.Text = card.typeText + " -- " + card.race.Value + " " + (card.subtype?.ToString() ?? "");
                    }
                }

                if (card.isDummy)
                {
                    powerBox.Text = toughnessBox.Text =
                        castRangeSlashMovementBox.Text = autoFontTextBox1.Text = "";
                    breadText.Text = card.dummiedAbility.description;
                }
                else if (card.hasPT)
                {
                    powerBox.Text = card.power.ToString();
                    toughnessBox.Text = card.toughness.ToString();
                    castRangeSlashMovementBox.Text = card.movement.ToString();
                    autoFontTextBox1.Text = "/";
                }
                else if (card.cardType == CardType.Relic)
                {
                    castRangeSlashMovementBox.Text = "";
                    powerBox.Text = "";
                    toughnessBox.Text = "";
                    autoFontTextBox1.Text = "";
                }
                else if (card.cardType == CardType.Interrupt || card.cardType == CardType.Channel)
                {
                    castRangeSlashMovementBox.Text = (card.castRange > 0) ? card.castRange.ToString() : "";
                    powerBox.Text = "";
                    toughnessBox.Text = "";
                    autoFontTextBox1.Text = "";
                }
                else throw new Exception();
                Invalidate();
            });
            card = c;
        }

        public void glowColour(Color? c = null)
        {
            Panel[] ps = new[]
            {
                topBorderPanel,
                botBorderPanel,
                lftBorderPanel,
                rgtBorderPanel,
            };

            foreach (Panel p in ps)
            {
                p.memeout(() =>
                {
                    if (c.HasValue)
                    {
                        p.Visible = true;
                        p.BackColor = c.Value;
                    }
                    else
                    {
                        p.Visible = false;
                    }
                });
            }
        }


        public void notify(object o, CardChangedMessage t)
        {
            sickmemes();
        }

        public Stuff getStuff()
        {
            return card;
        }

        private void InitializeComponent()
        {
            this.art = new System.Windows.Forms.PictureBox();
            this.breadText = new stonerkart.AutoFontTextBox();
            this.nameBox = new stonerkart.AutoFontTextBox();
            this.toughnessBox = new stonerkart.AutoFontTextBox();
            this.powerBox = new stonerkart.AutoFontTextBox();
            this.cardTypeText = new stonerkart.AutoFontTextBox();
            this.castRangeSlashMovementBox = new stonerkart.AutoFontTextBox();
            this.manaCostPanel1 = new stonerkart.ManaCostPanel();
            this.autoFontTextBox1 = new stonerkart.AutoFontTextBox();
            this.setArtPanel1 = new stonerkart.SetArtPanel();
            this.topBorderPanel = new System.Windows.Forms.Panel();
            this.botBorderPanel = new System.Windows.Forms.Panel();
            this.lftBorderPanel = new System.Windows.Forms.Panel();
            this.frameImage = new System.Windows.Forms.PictureBox();
            this.rgtBorderPanel = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.art)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.frameImage)).BeginInit();
            this.SuspendLayout();
            // 
            // art
            // 
            this.art.Enabled = false;
            this.art.Image = global::stonerkart.Properties.Resources.artBelwas;
            this.art.InitialImage = null;
            this.art.Location = new System.Drawing.Point(46, 64);
            this.art.Name = "art";
            this.art.Size = new System.Drawing.Size(390, 300);
            this.art.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.art.TabIndex = 0;
            this.art.TabStop = false;
            // 
            // breadText
            // 
            this.breadText.BackColor = System.Drawing.Color.Transparent;
            this.breadText.Enabled = false;
            this.breadText.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F);
            this.breadText.justifyText = stonerkart.Justify.Left;
            this.breadText.Location = new System.Drawing.Point(41, 394);
            this.breadText.Name = "breadText";
            this.breadText.Opacity = 100;
            this.breadText.Size = new System.Drawing.Size(400, 150);
            this.breadText.TabIndex = 1;
            // 
            // nameBox
            // 
            this.nameBox.BackColor = System.Drawing.Color.Transparent;
            this.nameBox.Enabled = false;
            this.nameBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F);
            this.nameBox.justifyText = stonerkart.Justify.Left;
            this.nameBox.Location = new System.Drawing.Point(31, 15);
            this.nameBox.Name = "nameBox";
            this.nameBox.Opacity = 100;
            this.nameBox.Size = new System.Drawing.Size(242, 43);
            this.nameBox.TabIndex = 2;
            // 
            // toughnessBox
            // 
            this.toughnessBox.BackColor = System.Drawing.Color.Transparent;
            this.toughnessBox.Enabled = false;
            this.toughnessBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F);
            this.toughnessBox.justifyText = stonerkart.Justify.Left;
            this.toughnessBox.Location = new System.Drawing.Point(405, 545);
            this.toughnessBox.Name = "toughnessBox";
            this.toughnessBox.Opacity = 100;
            this.toughnessBox.Size = new System.Drawing.Size(53, 57);
            this.toughnessBox.TabIndex = 3;
            // 
            // powerBox
            // 
            this.powerBox.BackColor = System.Drawing.Color.Transparent;
            this.powerBox.Enabled = false;
            this.powerBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F);
            this.powerBox.justifyText = stonerkart.Justify.Left;
            this.powerBox.Location = new System.Drawing.Point(332, 545);
            this.powerBox.Name = "powerBox";
            this.powerBox.Opacity = 100;
            this.powerBox.Size = new System.Drawing.Size(49, 57);
            this.powerBox.TabIndex = 4;
            // 
            // cardTypeText
            // 
            this.cardTypeText.BackColor = System.Drawing.Color.Transparent;
            this.cardTypeText.Enabled = false;
            this.cardTypeText.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F);
            this.cardTypeText.justifyText = stonerkart.Justify.Left;
            this.cardTypeText.Location = new System.Drawing.Point(46, 370);
            this.cardTypeText.Name = "cardTypeText";
            this.cardTypeText.Opacity = 100;
            this.cardTypeText.Size = new System.Drawing.Size(364, 28);
            this.cardTypeText.TabIndex = 5;
            // 
            // castRangeSlashMovementBox
            // 
            this.castRangeSlashMovementBox.BackColor = System.Drawing.Color.Transparent;
            this.castRangeSlashMovementBox.Enabled = false;
            this.castRangeSlashMovementBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F);
            this.castRangeSlashMovementBox.justifyText = stonerkart.Justify.Left;
            this.castRangeSlashMovementBox.Location = new System.Drawing.Point(31, 545);
            this.castRangeSlashMovementBox.Name = "castRangeSlashMovementBox";
            this.castRangeSlashMovementBox.Opacity = 100;
            this.castRangeSlashMovementBox.Size = new System.Drawing.Size(53, 57);
            this.castRangeSlashMovementBox.TabIndex = 5;
            // 
            // manaCostPanel1
            // 
            this.manaCostPanel1.BackColor = System.Drawing.Color.Transparent;
            this.manaCostPanel1.Location = new System.Drawing.Point(280, 15);
            this.manaCostPanel1.Name = "manaCostPanel1";
            this.manaCostPanel1.Opacity = 100;
            this.manaCostPanel1.Size = new System.Drawing.Size(161, 43);
            this.manaCostPanel1.TabIndex = 6;
            // 
            // autoFontTextBox1
            // 
            this.autoFontTextBox1.BackColor = System.Drawing.Color.Transparent;
            this.autoFontTextBox1.Enabled = false;
            this.autoFontTextBox1.justifyText = stonerkart.Justify.Left;
            this.autoFontTextBox1.Location = new System.Drawing.Point(382, 545);
            this.autoFontTextBox1.Name = "autoFontTextBox1";
            this.autoFontTextBox1.Opacity = 100;
            this.autoFontTextBox1.Size = new System.Drawing.Size(24, 57);
            this.autoFontTextBox1.TabIndex = 7;
            // 
            // setArtPanel1
            // 
            this.setArtPanel1.BackColor = System.Drawing.Color.Transparent;
            this.setArtPanel1.Location = new System.Drawing.Point(411, 370);
            this.setArtPanel1.Name = "setArtPanel1";
            this.setArtPanel1.Opacity = 100;
            this.setArtPanel1.Size = new System.Drawing.Size(25, 25);
            this.setArtPanel1.TabIndex = 8;
            // 
            // topBorderPanel
            // 
            this.topBorderPanel.Location = new System.Drawing.Point(0, 0);
            this.topBorderPanel.Name = "topBorderPanel";
            this.topBorderPanel.Size = new System.Drawing.Size(478, 8);
            this.topBorderPanel.TabIndex = 9;
            // 
            // botBorderPanel
            // 
            this.botBorderPanel.Location = new System.Drawing.Point(0, 611);
            this.botBorderPanel.Name = "botBorderPanel";
            this.botBorderPanel.Size = new System.Drawing.Size(478, 8);
            this.botBorderPanel.TabIndex = 10;
            // 
            // lftBorderPanel
            // 
            this.lftBorderPanel.Location = new System.Drawing.Point(0, 0);
            this.lftBorderPanel.Name = "lftBorderPanel";
            this.lftBorderPanel.Size = new System.Drawing.Size(8, 618);
            this.lftBorderPanel.TabIndex = 10;
            // 
            // frameImage
            // 
            this.frameImage.Enabled = false;
            this.frameImage.Image = global::stonerkart.Properties.Resources.lifeFrame;
            this.frameImage.InitialImage = null;
            this.frameImage.Location = new System.Drawing.Point(0, 0);
            this.frameImage.Name = "frameImage";
            this.frameImage.Size = new System.Drawing.Size(478, 618);
            this.frameImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.frameImage.TabIndex = 5;
            this.frameImage.TabStop = false;
            // 
            // rgtBorderPanel
            // 
            this.rgtBorderPanel.Location = new System.Drawing.Point(471, 0);
            this.rgtBorderPanel.Name = "rgtBorderPanel";
            this.rgtBorderPanel.Size = new System.Drawing.Size(8, 618);
            this.rgtBorderPanel.TabIndex = 11;
            // 
            // CardView
            // 
            this.Controls.Add(this.rgtBorderPanel);
            this.Controls.Add(this.lftBorderPanel);
            this.Controls.Add(this.botBorderPanel);
            this.Controls.Add(this.topBorderPanel);
            this.Controls.Add(this.setArtPanel1);
            this.Controls.Add(this.autoFontTextBox1);
            this.Controls.Add(this.manaCostPanel1);
            this.Controls.Add(this.castRangeSlashMovementBox);
            this.Controls.Add(this.cardTypeText);
            this.Controls.Add(this.powerBox);
            this.Controls.Add(this.toughnessBox);
            this.Controls.Add(this.nameBox);
            this.Controls.Add(this.breadText);
            this.Controls.Add(this.art);
            this.Controls.Add(this.frameImage);
            this.Name = "CardView";
            this.Size = new System.Drawing.Size(478, 618);
            ((System.ComponentModel.ISupportInitialize)(this.art)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.frameImage)).EndInit();
            this.ResumeLayout(false);

        }
    }

    struct CardChangedMessage
    {
        public Modifiable modified { get; }

        public CardChangedMessage(Modifiable modified)
        {
            this.modified = modified;
        }
    }

    class SetArtPanel : TransparentPanel
    {
        private CardSet set;
        private Rarity rarity;

        public SetArtPanel() : this(CardSet.FirstEdition, Rarity.Common)
        {
        }

        public SetArtPanel(CardSet set, Rarity rarity)
        {
            setThings(set, rarity);
        }

        public void setThings(CardSet set, Rarity rarity)
        {
            this.set = set;
            this.rarity = rarity;
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;

            int h = Size.Height;
            int w = Size.Width;
            int dh = h;
            int dw = w;

            Image b = ImageLoader.cardSetImage(set, rarity);
            Image i = G.ResizeImage(b, dw, dh);
            g.DrawImage(i, 0, 0);
        }
    }
}
