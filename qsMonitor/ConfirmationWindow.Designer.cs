namespace qsMonitor
{
    partial class ConfirmationWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfirmationWindow));
            this.label1 = new System.Windows.Forms.Label();
            this.yesBtn = new System.Windows.Forms.Button();
            this.noBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Showcard Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(211, 111);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(665, 30);
            this.label1.TabIndex = 0;
            this.label1.Text = "Are you sure you want to reboot your computer?";
            // 
            // yesBtn
            // 
            this.yesBtn.BackColor = System.Drawing.Color.IndianRed;
            this.yesBtn.Font = new System.Drawing.Font("Bernard MT Condensed", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.yesBtn.Location = new System.Drawing.Point(379, 225);
            this.yesBtn.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.yesBtn.Name = "yesBtn";
            this.yesBtn.Size = new System.Drawing.Size(92, 42);
            this.yesBtn.TabIndex = 1;
            this.yesBtn.Text = "Yes";
            this.yesBtn.UseVisualStyleBackColor = false;
            this.yesBtn.Click += new System.EventHandler(this.yesBtn_Click);
            // 
            // noBtn
            // 
            this.noBtn.BackColor = System.Drawing.Color.ForestGreen;
            this.noBtn.Font = new System.Drawing.Font("Bernard MT Condensed", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.noBtn.Location = new System.Drawing.Point(620, 225);
            this.noBtn.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.noBtn.Name = "noBtn";
            this.noBtn.Size = new System.Drawing.Size(92, 42);
            this.noBtn.TabIndex = 2;
            this.noBtn.Text = "No";
            this.noBtn.UseVisualStyleBackColor = false;
            this.noBtn.Click += new System.EventHandler(this.noBtn_Click);
            // 
            // ConfirmationWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.SteelBlue;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.ClientSize = new System.Drawing.Size(1058, 484);
            this.Controls.Add(this.noBtn);
            this.Controls.Add(this.yesBtn);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Bernard MT Condensed", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximumSize = new System.Drawing.Size(1080, 540);
            this.MinimumSize = new System.Drawing.Size(1080, 540);
            this.Name = "ConfirmationWindow";
            this.Text = "ConfirmationWindow";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button yesBtn;
        private System.Windows.Forms.Button noBtn;
    }
}