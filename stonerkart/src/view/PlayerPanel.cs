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

        public PlayerPanel()
        {
            InitializeComponent();
        }

        public void notify(PlayerChangedArgs t)
        {
            if (t.active.HasValue)
            {
                Color c = t.active.Value ? StepPanel.on : StepPanel.off;
                BackColor = c;
                //manaPanel1.BackColor = c;
            }
            else
            {
                manaPanel1.setLightUp(t.player.manaPool);
            }
        }

        private void InitializeComponent()
        {
            this.manaPanel1 = new stonerkart.ManaPanel();
            this.SuspendLayout();
            // 
            // manaPanel1
            // 
            this.manaPanel1.BackColor = System.Drawing.Color.DarkGray;
            this.manaPanel1.Location = new System.Drawing.Point(15, 14);
            this.manaPanel1.Name = "manaPanel1";
            this.manaPanel1.Size = new System.Drawing.Size(263, 287);
            this.manaPanel1.TabIndex = 0;
            // 
            // PlayerPanel
            // 
            this.Controls.Add(this.manaPanel1);
            this.Name = "PlayerPanel";
            this.Size = new System.Drawing.Size(354, 316);
            this.ResumeLayout(false);

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
