using System;
using System.Collections.Generic;
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
            manaPanel1.setLightUp(t.player.manaPool);
        }

        private void InitializeComponent()
        {
            this.manaPanel1 = new stonerkart.ManaPanel();
            this.SuspendLayout();
            // 
            // manaPanel1
            // 
            this.manaPanel1.BackColor = System.Drawing.Color.DarkGray;
            this.manaPanel1.Location = new System.Drawing.Point(4, 4);
            this.manaPanel1.Name = "manaPanel1";
            this.manaPanel1.Size = new System.Drawing.Size(346, 308);
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

    struct PlayerChangedArgs
    {
        public readonly Player player;

        public PlayerChangedArgs(Player player)
        {
            this.player = player;
        }
    }
}
