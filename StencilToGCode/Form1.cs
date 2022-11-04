using StencilToGCode.UserControls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StencilToGCode
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Add MainControl user control
            MainControl mainControl = new MainControl();
            mainControl.Dock = DockStyle.Fill;
            this.Controls.Add(mainControl);
        }
    }
}
