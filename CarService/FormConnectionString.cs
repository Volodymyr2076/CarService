using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CarService
{
    public partial class FormConnectionString : Form
    {
        public FormConnectionString()
        {
            InitializeComponent();
        }

        private void buttonChangeConnectionString_Click(object sender, EventArgs e)
        {
            Form1.ConnectionString = textBoxConnectionString.Text;
            Close();
        }

        private void FormConnectionString_Load(object sender, EventArgs e)
        {
            textBoxConnectionString.Text = Form1.ConnectionString;
            textBoxConnectionString.Focus();
        }
    }
}
