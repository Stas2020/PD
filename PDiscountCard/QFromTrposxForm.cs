using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PDiscountCard
{
    public partial class QFromTrposxForm : MF3
    {
        public QFromTrposxForm(string Caption, string Message)
        {
            InitializeComponent();
            label1.Text = Caption ;
            label2.Text = Message;
            button1.Click += new EventHandler(button1_Click);
            button2.Click += new EventHandler(button2_Click);          
           
        }
        internal bool Answ;

        void button2_Click(object sender, EventArgs e)
        {
            Answ = false;
            this.Hide(); 
        }

        void button1_Click(object sender, EventArgs e)
        {
            Answ = true;
            this.Hide(); 
        }

    }
}
