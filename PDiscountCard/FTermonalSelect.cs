using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PDiscountCard
{
    public partial class FTermonalSelect : MF3
    {
        public FTermonalSelect()
        {
            InitializeComponent();
            button1.Click +=new EventHandler(button1_Click);
            button2.Click += new EventHandler(button2_Click); 
            button3.Click +=new EventHandler(button3_Click);

        }

        public bool Answer = true;


        private void button1_Click(object sender, EventArgs e)
        {
            MainClass.IsWiFi = 2;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            MainClass.IsWiFi = 1;
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
             Cancel = true;
            this.Close(); 

        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            Cancel = true;
            this.Close(); 
        }
    }
}
