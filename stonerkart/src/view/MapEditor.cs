using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace stonerkart
{
    class MapEditor : StickyPanel
    {

        public MapEditor()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // MapEditor
            // 
            this.Name = "MapEditor";
            this.Size = new System.Drawing.Size(868, 731);
            this.ResumeLayout(false);

        }
    }
}
