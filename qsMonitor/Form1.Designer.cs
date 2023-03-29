namespace qsMonitor
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.cpuLabel = new System.Windows.Forms.Label();
            this.availMemLabel = new System.Windows.Forms.Label();
            this.sysUpTimeLabel = new System.Windows.Forms.Label();
            this.randomVal = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.randValueLabel = new System.Windows.Forms.Label();
            this.sysUpTimeValueLabel = new System.Windows.Forms.Label();
            this.availMemValueLabel = new System.Windows.Forms.Label();
            this.cpuValueLabel = new System.Windows.Forms.Label();
            this.taskList = new System.Windows.Forms.ComboBox();
            this.killButton = new System.Windows.Forms.Button();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.showToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rebootBtn = new System.Windows.Forms.Button();
            this.dangerZoneGB = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.dangerZoneGB.SuspendLayout();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // cpuLabel
            // 
            this.cpuLabel.AutoSize = true;
            this.cpuLabel.Location = new System.Drawing.Point(17, 55);
            this.cpuLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.cpuLabel.Name = "cpuLabel";
            this.cpuLabel.Size = new System.Drawing.Size(43, 24);
            this.cpuLabel.TabIndex = 1;
            this.cpuLabel.Text = "CPU:";
            this.cpuLabel.Click += new System.EventHandler(this.cpuLabel_Click);
            // 
            // availMemLabel
            // 
            this.availMemLabel.AutoSize = true;
            this.availMemLabel.Location = new System.Drawing.Point(17, 117);
            this.availMemLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.availMemLabel.Name = "availMemLabel";
            this.availMemLabel.Size = new System.Drawing.Size(153, 24);
            this.availMemLabel.TabIndex = 2;
            this.availMemLabel.Text = "Available Memory:";
            this.availMemLabel.Click += new System.EventHandler(this.availMemLabel_Click);
            // 
            // sysUpTimeLabel
            // 
            this.sysUpTimeLabel.AutoSize = true;
            this.sysUpTimeLabel.Location = new System.Drawing.Point(17, 179);
            this.sysUpTimeLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.sysUpTimeLabel.Name = "sysUpTimeLabel";
            this.sysUpTimeLabel.Size = new System.Drawing.Size(134, 24);
            this.sysUpTimeLabel.TabIndex = 3;
            this.sysUpTimeLabel.Text = "System Up Time:";
            this.sysUpTimeLabel.Click += new System.EventHandler(this.sysUpTimeLabel_Click);
            // 
            // randomVal
            // 
            this.randomVal.AutoSize = true;
            this.randomVal.Location = new System.Drawing.Point(17, 240);
            this.randomVal.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.randomVal.Name = "randomVal";
            this.randomVal.Size = new System.Drawing.Size(81, 24);
            this.randomVal.TabIndex = 4;
            this.randomVal.Text = "Random:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.randValueLabel);
            this.groupBox1.Controls.Add(this.sysUpTimeValueLabel);
            this.groupBox1.Controls.Add(this.availMemValueLabel);
            this.groupBox1.Controls.Add(this.cpuValueLabel);
            this.groupBox1.Controls.Add(this.cpuLabel);
            this.groupBox1.Controls.Add(this.availMemLabel);
            this.groupBox1.Controls.Add(this.randomVal);
            this.groupBox1.Controls.Add(this.sysUpTimeLabel);
            this.groupBox1.Location = new System.Drawing.Point(14, 19);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.groupBox1.Size = new System.Drawing.Size(522, 317);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Performance";
            // 
            // randValueLabel
            // 
            this.randValueLabel.AutoSize = true;
            this.randValueLabel.Location = new System.Drawing.Point(278, 240);
            this.randValueLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.randValueLabel.Name = "randValueLabel";
            this.randValueLabel.Size = new System.Drawing.Size(133, 24);
            this.randValueLabel.TabIndex = 9;
            this.randValueLabel.Text = "randValueLabel";
            // 
            // sysUpTimeValueLabel
            // 
            this.sysUpTimeValueLabel.AutoSize = true;
            this.sysUpTimeValueLabel.Location = new System.Drawing.Point(278, 179);
            this.sysUpTimeValueLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.sysUpTimeValueLabel.Name = "sysUpTimeValueLabel";
            this.sysUpTimeValueLabel.Size = new System.Drawing.Size(133, 24);
            this.sysUpTimeValueLabel.TabIndex = 8;
            this.sysUpTimeValueLabel.Text = "sysUpTimeValue";
            // 
            // availMemValueLabel
            // 
            this.availMemValueLabel.AutoSize = true;
            this.availMemValueLabel.Location = new System.Drawing.Point(278, 117);
            this.availMemValueLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.availMemValueLabel.Name = "availMemValueLabel";
            this.availMemValueLabel.Size = new System.Drawing.Size(128, 24);
            this.availMemValueLabel.TabIndex = 7;
            this.availMemValueLabel.Text = "availMemValue";
            // 
            // cpuValueLabel
            // 
            this.cpuValueLabel.AutoSize = true;
            this.cpuValueLabel.Location = new System.Drawing.Point(278, 55);
            this.cpuValueLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.cpuValueLabel.Name = "cpuValueLabel";
            this.cpuValueLabel.Size = new System.Drawing.Size(82, 24);
            this.cpuValueLabel.TabIndex = 6;
            this.cpuValueLabel.Text = "cpuValue";
            // 
            // taskList
            // 
            this.taskList.FormattingEnabled = true;
            this.taskList.Location = new System.Drawing.Point(34, 360);
            this.taskList.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.taskList.Name = "taskList";
            this.taskList.Size = new System.Drawing.Size(218, 32);
            this.taskList.TabIndex = 7;
            this.taskList.SelectedIndexChanged += new System.EventHandler(this.taskList_SelectedIndexChanged);
            // 
            // killButton
            // 
            this.killButton.Location = new System.Drawing.Point(259, 360);
            this.killButton.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.killButton.Name = "killButton";
            this.killButton.Size = new System.Drawing.Size(88, 44);
            this.killButton.TabIndex = 8;
            this.killButton.Text = "End Task";
            this.killButton.UseVisualStyleBackColor = true;
            this.killButton.Click += new System.EventHandler(this.killButton_Click);
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.ContextMenuStrip = this.contextMenuStrip1;
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "Settings";
            this.notifyIcon1.Visible = true;
            this.notifyIcon1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseDoubleClick);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(129, 68);
            // 
            // showToolStripMenuItem
            // 
            this.showToolStripMenuItem.Name = "showToolStripMenuItem";
            this.showToolStripMenuItem.Size = new System.Drawing.Size(128, 32);
            this.showToolStripMenuItem.Text = "Show";
            this.showToolStripMenuItem.Click += new System.EventHandler(this.showToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(128, 32);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // rebootBtn
            // 
            this.rebootBtn.BackColor = System.Drawing.Color.Crimson;
            this.rebootBtn.Location = new System.Drawing.Point(33, 55);
            this.rebootBtn.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.rebootBtn.Name = "rebootBtn";
            this.rebootBtn.Size = new System.Drawing.Size(83, 44);
            this.rebootBtn.TabIndex = 9;
            this.rebootBtn.Text = "Reboot";
            this.rebootBtn.UseVisualStyleBackColor = false;
            this.rebootBtn.Click += new System.EventHandler(this.rebootBtn_Click);
            // 
            // dangerZoneGB
            // 
            this.dangerZoneGB.Controls.Add(this.rebootBtn);
            this.dangerZoneGB.Location = new System.Drawing.Point(760, 19);
            this.dangerZoneGB.Name = "dangerZoneGB";
            this.dangerZoneGB.Size = new System.Drawing.Size(439, 141);
            this.dangerZoneGB.TabIndex = 10;
            this.dangerZoneGB.TabStop = false;
            this.dangerZoneGB.Text = "Danger Zone";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.Teal;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.ClientSize = new System.Drawing.Size(1338, 1084);
            this.Controls.Add(this.dangerZoneGB);
            this.Controls.Add(this.killButton);
            this.Controls.Add(this.taskList);
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("Bernard MT Condensed", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.MinimumSize = new System.Drawing.Size(1080, 960);
            this.Name = "Form1";
            this.Text = "QStack System Monitor";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Move += new System.EventHandler(this.Form1_Move);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.contextMenuStrip1.ResumeLayout(false);
            this.dangerZoneGB.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label cpuLabel;
        private System.Windows.Forms.Label availMemLabel;
        private System.Windows.Forms.Label sysUpTimeLabel;
        private System.Windows.Forms.Label randomVal;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label cpuValueLabel;
        private System.Windows.Forms.Label randValueLabel;
        private System.Windows.Forms.Label sysUpTimeValueLabel;
        private System.Windows.Forms.Label availMemValueLabel;
        private System.Windows.Forms.ComboBox taskList;
        private System.Windows.Forms.Button killButton;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem showToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.Button rebootBtn;
        private System.Windows.Forms.GroupBox dangerZoneGB;
    }
}

