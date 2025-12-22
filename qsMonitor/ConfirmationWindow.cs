using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace qsMonitor
{
    public partial class ConfirmationWindow : Form
    {
        public ConfirmationWindow()
        {
            InitializeComponent();
        }

        private void noBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void yesBtn_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("shutdown", "/r /t 0");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to restart computer: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
