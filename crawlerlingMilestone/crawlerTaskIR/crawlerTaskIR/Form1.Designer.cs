namespace crawlerTaskIR
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.crawl = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // crawl
            // 
            this.crawl.BackColor = System.Drawing.Color.Snow;
            this.crawl.Cursor = System.Windows.Forms.Cursors.Hand;
            this.crawl.Font = new System.Drawing.Font("Candara", 12F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.crawl.ForeColor = System.Drawing.Color.Black;
            this.crawl.Location = new System.Drawing.Point(335, 192);
            this.crawl.Name = "crawl";
            this.crawl.Size = new System.Drawing.Size(187, 58);
            this.crawl.TabIndex = 0;
            this.crawl.Text = "Crawl";
            this.crawl.UseVisualStyleBackColor = false;
            this.crawl.Click += new System.EventHandler(this.crawl_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.RoyalBlue;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(882, 477);
            this.Controls.Add(this.crawl);
            this.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "Web Crawler ";
            this.TransparencyKey = System.Drawing.Color.White;
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button crawl;
    }
}

