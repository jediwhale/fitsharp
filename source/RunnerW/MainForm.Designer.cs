namespace fitSharp.RunnerW
{
    partial class MainForm
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
            this.progressText = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // progressText
            // 
            this.progressText.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.progressText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.progressText.Location = new System.Drawing.Point(0, 0);
            this.progressText.Multiline = true;
            this.progressText.Name = "progressText";
            this.progressText.ReadOnly = true;
            this.progressText.Size = new System.Drawing.Size(292, 273);
            this.progressText.TabIndex = 1;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 273);
            this.Controls.Add(this.progressText);
            this.Name = "MainForm";
            this.Text = "Runner";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Activated += new System.EventHandler(this.MainForm_Activated);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox progressText;

    }
}

