using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PDiscountCard
{
    public partial class MessageForm : MF3
    {

        public int Result=0;
        public MessageForm(string mess)
        {
            InitializeComponent();
            label1.Text  = mess;
            button1.Text = "Продолжить печать";
            button2.Text = "Отмена";
            button3.Text = "Напечатать документ заново";
            button1.Click += new EventHandler(button1_Click);
            button2.Click += new EventHandler(button2_Click);
            button3.Click += new EventHandler(button3_Click);

            button3.Visible = false;
            TopMost = true;
           
        }

        void button4_Click(object sender, EventArgs e)
        {
            Result = 4;
            this.Close(); 
        }
        public void OnlyOk()
        {
            button1.Text = "Ok";
            button2.Visible = false;
            button3.Visible = false;
            button1.Left = this.Width - button1.Width / 2;
            
        }

        void button3_Click(object sender, EventArgs e)
        {
            Result = 2;
        }

        void button2_Click(object sender, EventArgs e)
        {
            Result = -1;
            this.Close(); 
        }

        void button1_Click(object sender, EventArgs e)
        {
            Result = 1;
            this.Close(); 
        }

        
        public void SetCpt(string Mess)
        {
            label1.Text = Mess;
        }

    }
}
