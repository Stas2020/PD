using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PDiscountCard
{
    public partial class QSverkaForm : MF3
    {
        public QSverkaForm()
        {
            InitializeComponent();
            button1.Text = "Ok";
            button2.Visible = false;
            label1.Text = "Выполнить сверку?";
            button1.Click += new EventHandler(button1_Click);
        }
        public bool IsGood = false;
        void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "6")
            {
                IsGood = true;
            
            this.Close();
            }
        }

    }
}
