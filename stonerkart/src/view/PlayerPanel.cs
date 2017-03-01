using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace stonerkart
{
    class PlayerPanel : StickyPanel, Observer<PlayerChangedArgs>
    {
        public ManaPanel manaPanel1;
        private Button graveyardButton;
        private Button exileButton;
        private Button handButton;
        private Button deckButton;
        private bool stunt;

        private Tuple<PileLocation, Button>[] hack;

        private Player hackp;

        public PlayerPanel()
        {
            InitializeComponent();
            hack = new[]
            {
                new Tuple<PileLocation, Button>(PileLocation.Graveyard, graveyardButton),
                new Tuple<PileLocation, Button>(PileLocation.Hand, handButton),
                new Tuple<PileLocation, Button>(PileLocation.Displaced, exileButton),
                new Tuple<PileLocation, Button>(PileLocation.Deck, deckButton),
            };

            foreach (Tuple<PileLocation, Button> t in hack)
            {
                var t1 = t;
                t.Item2.MouseClick += (_, __) => koen(t1.Item1);
            }
        }

        public void notify(object o, PlayerChangedArgs t)
        {
            hackp = (Player)o;
            if (t.active.HasValue)
            {
                Color c = t.active.Value ? StepPanel.on : StepPanel.off;
                BackColor = c;
            }
            if (t.pileChanged.HasValue)
            {
                foreach (Tuple<PileLocation, Button> tpl in hack)
                {
                    Pile p = hackp.pileFrom(tpl.Item1);
                    Button b = tpl.Item2;
                    b.memeout(() =>
                    {
                        b.Text = p.Count.ToString();
                    });
                }
                Invalidate();
            }
            else
            {
                manaPanel1.setPool(hackp.manaPool, hackp.stunthack, hackp.stunthackset);
            }
        }
        
        private void InitializeComponent()
        {
            this.manaPanel1 = new stonerkart.ManaPanel();
            this.graveyardButton = new System.Windows.Forms.Button();
            this.exileButton = new System.Windows.Forms.Button();
            this.handButton = new System.Windows.Forms.Button();
            this.deckButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // manaPanel1
            // 
            this.manaPanel1.BackColor = System.Drawing.Color.DarkGray;
            this.manaPanel1.Location = new System.Drawing.Point(24, 15);
            this.manaPanel1.Name = "manaPanel1";
            this.manaPanel1.Size = new System.Drawing.Size(359, 337);
            this.manaPanel1.TabIndex = 0;
            // 
            // graveyardButton
            // 
            this.graveyardButton.Location = new System.Drawing.Point(401, 273);
            this.graveyardButton.Name = "graveyardButton";
            this.graveyardButton.Size = new System.Drawing.Size(82, 79);
            this.graveyardButton.TabIndex = 1;
            this.graveyardButton.Text = "button1";
            this.graveyardButton.UseVisualStyleBackColor = true;
            // 
            // exileButton
            // 
            this.exileButton.Location = new System.Drawing.Point(489, 273);
            this.exileButton.Name = "exileButton";
            this.exileButton.Size = new System.Drawing.Size(82, 79);
            this.exileButton.TabIndex = 2;
            this.exileButton.Text = "button2";
            this.exileButton.UseVisualStyleBackColor = true;
            // 
            // handbutton
            // 
            this.handButton.Location = new System.Drawing.Point(401, 188);
            this.handButton.Name = "handButton";
            this.handButton.Size = new System.Drawing.Size(82, 79);
            this.handButton.TabIndex = 3;
            this.handButton.Text = "button3";
            this.handButton.UseVisualStyleBackColor = true;
            // 
            // deckButton
            // 
            this.deckButton.Location = new System.Drawing.Point(489, 188);
            this.deckButton.Name = "deckButton";
            this.deckButton.Size = new System.Drawing.Size(82, 79);
            this.deckButton.TabIndex = 4;
            this.deckButton.Text = "button4";
            this.deckButton.UseVisualStyleBackColor = true;
            // 
            // PlayerPanel
            // 
            this.Controls.Add(this.deckButton);
            this.Controls.Add(this.handButton);
            this.Controls.Add(this.exileButton);
            this.Controls.Add(this.graveyardButton);
            this.Controls.Add(this.manaPanel1);
            this.Name = "PlayerPanel";
            this.Size = new System.Drawing.Size(598, 372);
            this.ResumeLayout(false);

        }

        private void koen(PileLocation pl)
        {
            if (hackp == null) return;
            Controller.toggleShowPile(hackp, pl);
        }
    }

    class PlayerChangedArgs
    {
        public readonly Player player;
        public readonly bool? active;
        public readonly PileLocation? pileChanged;

        public PlayerChangedArgs(bool active)
        {
            this.active = active;
        }

        public PlayerChangedArgs(Player player)
        {
            this.player = player;
        }

        public PlayerChangedArgs(PileLocation pileChanged)
        {
            this.pileChanged = pileChanged;
        }
    }
}
