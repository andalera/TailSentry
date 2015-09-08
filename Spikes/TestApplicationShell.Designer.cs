namespace Spikes
{
    partial class TestApplicationShell
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.LogFileField = new System.Windows.Forms.TextBox();
            this.BrowseForFileLink = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 36);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "File To Monitor";
            // 
            // LogFileField
            // 
            this.LogFileField.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.LogFileField.Location = new System.Drawing.Point(95, 29);
            this.LogFileField.Name = "LogFileField";
            this.LogFileField.Size = new System.Drawing.Size(494, 20);
            this.LogFileField.TabIndex = 2;
            // 
            // BrowseForFileLink
            // 
            this.BrowseForFileLink.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.BrowseForFileLink.AutoSize = true;
            this.BrowseForFileLink.Location = new System.Drawing.Point(595, 36);
            this.BrowseForFileLink.Name = "BrowseForFileLink";
            this.BrowseForFileLink.Size = new System.Drawing.Size(51, 13);
            this.BrowseForFileLink.TabIndex = 3;
            this.BrowseForFileLink.TabStop = true;
            this.BrowseForFileLink.Text = "Browse...";
            // 
            // ApplicationShell
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(669, 587);
            this.Controls.Add(this.BrowseForFileLink);
            this.Controls.Add(this.LogFileField);
            this.Controls.Add(this.label1);
            this.Name = "ApplicationShell";
            this.Text = " Tail Sentry";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox LogFileField;
        private System.Windows.Forms.LinkLabel BrowseForFileLink;
    }
}