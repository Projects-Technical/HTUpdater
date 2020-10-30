namespace HTUpdater
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.pbdownload = new System.Windows.Forms.ProgressBar();
            this.lblFilename = new System.Windows.Forms.Label();
            this.lblprogress = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // pbdownload
            // 
            this.pbdownload.Location = new System.Drawing.Point(12, 39);
            this.pbdownload.Name = "pbdownload";
            this.pbdownload.Size = new System.Drawing.Size(279, 23);
            this.pbdownload.TabIndex = 0;
            // 
            // lblFilename
            // 
            this.lblFilename.AutoSize = true;
            this.lblFilename.ForeColor = System.Drawing.Color.White;
            this.lblFilename.Location = new System.Drawing.Point(13, 10);
            this.lblFilename.Name = "lblFilename";
            this.lblFilename.Size = new System.Drawing.Size(0, 15);
            this.lblFilename.TabIndex = 1;
            // 
            // lblprogress
            // 
            this.lblprogress.AutoSize = true;
            this.lblprogress.ForeColor = System.Drawing.Color.White;
            this.lblprogress.Location = new System.Drawing.Point(13, 74);
            this.lblprogress.Name = "lblprogress";
            this.lblprogress.Size = new System.Drawing.Size(0, 15);
            this.lblprogress.TabIndex = 1;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(303, 102);
            this.Controls.Add(this.lblprogress);
            this.Controls.Add(this.lblFilename);
            this.Controls.Add(this.pbdownload);
            this.ForeColor = System.Drawing.Color.White;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximumSize = new System.Drawing.Size(319, 141);
            this.MinimumSize = new System.Drawing.Size(319, 141);
            this.Name = "Form1";
            this.Text = "HT Auto Updater";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar pbdownload;
        private System.Windows.Forms.Label lblFilename;
        private System.Windows.Forms.Label lblprogress;
    }
}

