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
        private Button button1;
        private bool stunt;

        private Tuple<PileLocation, Button>[] hack;
        private Player hackp;

        public PlayerPanel()
        {
            InitializeComponent();
            hack = new[] { new Tuple<PileLocation, Button>(PileLocation.Graveyard, button1), };

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
            else
            {
                manaPanel1.setPool(t.player.manaPool, t.player.stunthack, t.player.stunthackset);
            }
        }
        
        private void InitializeComponent()
        {
            this.manaPanel1 = new stonerkart.ManaPanel();
            this.button1 = new System.Windows.Forms.Button();
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
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(430, 47);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(114, 112);
            this.button1.TabIndex = 1;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // PlayerPanel
            // 
            this.Controls.Add(this.button1);
            this.Controls.Add(this.manaPanel1);
            this.Name = "PlayerPanel";
            this.Size = new System.Drawing.Size(598, 372);
            this.ResumeLayout(false);

        }

        private void koen(PileLocation pl)
        {
            Controller.toggleShowPile(hackp, pl);
        }
    }

    class PlayerChangedArgs
    {
        public readonly Player player;
        public readonly bool? active;

        public PlayerChangedArgs(bool active)
        {
            this.active = active;
        }

        public PlayerChangedArgs(Player player)
        {
            this.player = player;
        }
    }
}
