namespace Spikes
{
    partial class TestForm
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
            this.TextBox1 = new System.Windows.Forms.RichTextBox();
            this.LongRunnerButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // TextBox1
            // 
            this.TextBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.TextBox1.Location = new System.Drawing.Point(12, 12);
            this.TextBox1.Name = "TextBox1";
            this.TextBox1.Size = new System.Drawing.Size(503, 465);
            this.TextBox1.TabIndex = 0;
            this.TextBox1.Text = "";
            // 
            // LongRunnerButton
            // 
            this.LongRunnerButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.LongRunnerButton.Location = new System.Drawing.Point(12, 500);
            this.LongRunnerButton.Name = "LongRunnerButton";
            this.LongRunnerButton.Size = new System.Drawing.Size(150, 23);
            this.LongRunnerButton.TabIndex = 1;
            this.LongRunnerButton.Text = "Start Long Running Code";
            this.LongRunnerButton.UseVisualStyleBackColor = true;
            // 
            // TestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(527, 539);
            this.Controls.Add(this.LongRunnerButton);
            this.Controls.Add(this.TextBox1);
            this.Name = "TestForm";
            this.Text = "TestForm";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox TextBox1;
        public System.Windows.Forms.Button LongRunnerButton;
    }
}