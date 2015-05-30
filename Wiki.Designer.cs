namespace ShinKen
{
    partial class Wiki
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
                //this.Hide();
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
            this.webBrowser1 = new System.Windows.Forms.WebBrowser();
            this.WikiLink = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // webBrowser1
            // 
            this.webBrowser1.Location = new System.Drawing.Point(-1, 1);
            this.webBrowser1.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser1.Name = "webBrowser1";
            this.webBrowser1.ScriptErrorsSuppressed = true;
            this.webBrowser1.Size = new System.Drawing.Size(949, 640);
            this.webBrowser1.TabIndex = 0;
            this.webBrowser1.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.webBrowser1_DocumentCompleted);
            // 
            // WikiLink
            // 
            this.WikiLink.BackColor = System.Drawing.Color.Transparent;
            this.WikiLink.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.WikiLink.Location = new System.Drawing.Point(-1, 1);
            this.WikiLink.Name = "WikiLink";
            this.WikiLink.Size = new System.Drawing.Size(67, 34);
            this.WikiLink.TabIndex = 1;
            this.WikiLink.Text = "Wiki";
            this.WikiLink.UseVisualStyleBackColor = false;
            this.WikiLink.Click += new System.EventHandler(this.Wiki_Click);
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.Transparent;
            this.button1.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Location = new System.Drawing.Point(-1, 34);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(67, 34);
            this.button1.TabIndex = 2;
            this.button1.Text = "攻略速報";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.Button2_Click);
            // 
            // Wiki
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(949, 640);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.WikiLink);
            this.Controls.Add(this.webBrowser1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "Wiki";
            this.Text = "Wiki";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Wiki_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Wiki_FormClosed);
            this.Load += new System.EventHandler(this.Wiki_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Wiki_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Wiki_KeyUp);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.WebBrowser webBrowser1;
        private System.Windows.Forms.Button WikiLink;
        private System.Windows.Forms.Button button1;
    }
}