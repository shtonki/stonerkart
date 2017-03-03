using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace stonerkart.src.view
{
    class BugReportPanel : UserControl
    {
        public RichTextBox bugText;
        public Button submitButton;
        public Button cancelButton;

        public BugReportPanel()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.bugText = new System.Windows.Forms.RichTextBox();
            this.submitButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // bugText
            // 
            this.bugText.Location = new System.Drawing.Point(14, 13);
            this.bugText.Name = "bugText";
            this.bugText.Size = new System.Drawing.Size(398, 288);
            this.bugText.TabIndex = 0;
            this.bugText.Text = "";
            // 
            // submitButton
            // 
            this.submitButton.Location = new System.Drawing.Point(40, 320);
            this.submitButton.Name = "submitButton";
            this.submitButton.Size = new System.Drawing.Size(75, 23);
            this.submitButton.TabIndex = 1;
            this.submitButton.Text = "Submit";
            this.submitButton.UseVisualStyleBackColor = true;
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(317, 320);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 2;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // BugReportPanel
            // 
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.submitButton);
            this.Controls.Add(this.bugText);
            this.Name = "BugReportPanel";
            this.Size = new System.Drawing.Size(430, 364);
            this.ResumeLayout(false);

        }
    }
}
