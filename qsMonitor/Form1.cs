using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Linq.Expressions;

namespace qsMonitor
{
    public partial class Form1 : Form
    {
        PerformanceCounter qsCPUCounter = new PerformanceCounter("Processor Information", "% Processor Time", "_Total");
        PerformanceCounter qsMemCounter = new PerformanceCounter("Memory", "Available MBytes");
        PerformanceCounter qsSystemCounter = new PerformanceCounter("System", "System Up Time");
        TimeSpan initialTime = DateTime.Now.TimeOfDay;
        
        public Form1()
        {
            InitializeComponent();
            populateTaskList();
        }

        private void sendToTray()
        {
            notifyIcon1.ShowBalloonTip(1000, "Still here!", "Your notifications will appear down here.", ToolTipIcon.Info);
            this.ShowInTaskbar = false;
            notifyIcon1.Visible = true;
        }

        private void alertUser(string title, string message)
        {
            TimeSpan timeDiff = DateTime.Now.TimeOfDay - initialTime;
            if(timeDiff.Minutes > 15)
            {
                notifyIcon1.ShowBalloonTip(1000, title, message, ToolTipIcon.Warning);
                initialTime = DateTime.Now.TimeOfDay;
            }
        }

        private void populateTaskList()
        {
            List<string> procList = new List<string>();
            procList.Add("Chrome");
            procList.Add("Teams");
            procList.Add("devenv");

            foreach (string proc in procList)
            {
                taskList.Items.Add(proc);
            }
        }

        private void killProcess(string procToKill)
        {
            if (string.IsNullOrEmpty(procToKill))
            {
                MessageBox.Show("Please select a process to terminate.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                Process[] killThisProc = Process.GetProcessesByName(procToKill);

                if (killThisProc.Length == 0)
                {
                    MessageBox.Show($"Process '{procToKill}' not found.", "Process Not Found", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                foreach (Process process in killThisProc)
                {
                    process.Kill();
                }
                
                MessageBox.Show($"Process '{procToKill}' terminated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to terminate process: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void getProcDetail(string procToKill)
        {
            if (string.IsNullOrEmpty(procToKill))
            {
                randValueLabel.Text = "No process selected";
                return;
            }

            try
            {
                PerformanceCounter PC = new PerformanceCounter();
                PC.CategoryName = "Process";
                PC.CounterName = "Working Set - Private";
                PC.InstanceName = procToKill;
                
                // Get memory in bytes, convert to MB
                float memMB = PC.NextValue() / (1024f * 1024f);

                Process[] processes = Process.GetProcessesByName(procToKill);

                if (processes.Length > 0)
                {
                    randValueLabel.Text = $"{memMB:F2} MB";
                }
                else
                {
                    randValueLabel.Text = "Process not found";
                }
            }
            catch (Exception ex)
            {
                randValueLabel.Text = $"Error: {ex.Message}";
            }
        }


        private void timer1_Tick(object sender, EventArgs e)
        {
            updateCpuVal();
            availMemValueLabel.Text = (int)qsMemCounter.NextValue() + "MB";
            updateSysTimeVal();
        }

        private void updateCpuVal()
        {
            int cpuValue = (int)qsCPUCounter.NextValue();
            cpuValueLabel.Text = cpuValue + "%";

            switch (cpuValue)
            {
                case int n when (n >= 80):
                    cpuValueLabel.BackColor = Color.Red;
                    alertUser("CPU", "CPU usage is set to: " + cpuValue);
                    break;
                case int n when (n >= 50):
                    cpuValueLabel.BackColor = Color.Orange;
                    alertUser("CPU", "CPU usage is set to: " + cpuValue);
                    break;
                case int n when (n >= 25):
                    cpuValueLabel.BackColor = Color.Yellow;
                    break;
                default:
                    cpuValueLabel.BackColor = Color.Empty;
                    break;
            }
        }

        private void updateSysTimeVal()
        {
            int sysTimeValue = (int)qsSystemCounter.NextValue() / 60 / 60;
            sysUpTimeValueLabel.Text = sysTimeValue + " Hours";

            switch (sysTimeValue)
            {
                case int n when (n >= 24):
                    sysUpTimeValueLabel.BackColor = Color.Red;
                    sysUpTimeValueLabel.Text = sysUpTimeValueLabel.Text + " -- ** Restart Required **";
                    alertUser("Time To Restart", "Your computer has been on for:" + sysTimeValue + " hours.  Consider restarting it.");
                    break;
                case int n when (n >= 12):
                    sysUpTimeValueLabel.BackColor = Color.Orange;
                    sysUpTimeValueLabel.Text = sysUpTimeValueLabel.Text + " -- ** Restart Required **";
                    alertUser("Time To Restart", "Your computer has been on for:" + sysTimeValue + " hours.  Consider restarting it.");
                    break;
                case int n when (n >= 6):
                    sysUpTimeValueLabel.BackColor = Color.Yellow;
                    break;
                default:
                    sysUpTimeValueLabel.BackColor = Color.Empty;
                    break;
            }
        }

        private void taskList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (taskList.SelectedItem != null)
            {
                getProcDetail(taskList.SelectedItem.ToString());
            }
        }

        private void killButton_Click(object sender, EventArgs e)
        {
            if (taskList.SelectedItem != null)
            {
                killProcess(taskList.SelectedItem.ToString());
            }
            else
            {
                MessageBox.Show("Please select a process from the list.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // Form Size Changed
        private void Form1_Load(object sender, EventArgs e)
        {
            //this.BackColor = Color.DarkGreen;
            //this.TransparencyKey = Color.Blue;
            this.Opacity = 0.8;
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
        }

        private void showToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Form1_Move(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                sendToTray();
            }
        }

        private void rebootBtn_Click(object sender, EventArgs e)
        {
            ConfirmationWindow confWin = new ConfirmationWindow();
            //confWin.ShowDialog();
            confWin.Show();
        }
    }
}
