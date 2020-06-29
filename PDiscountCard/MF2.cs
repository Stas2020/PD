using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PDiscountCard
{
    public partial class MF2 : MF3
    {
        public int result=0;
        public MF2(string Mess)
        {
            InitializeComponent();
            label1.Text = Mess;

            button1.Click += new EventHandler(button1_Click);
            button2.Click += new EventHandler(button2_Click);
                
        }

        void button2_Click(object sender, EventArgs e)
        {
            this.Close(); 
        }

        void button1_Click(object sender, EventArgs e)
        {
            result = 1;
            this.Close(); 
        }

        
    }
}
