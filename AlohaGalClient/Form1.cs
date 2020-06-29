using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AlohaGalClient
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            MainClass.Init();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MainClass.ReturnSale(Convert.ToInt32(textBox1.Text));
        }

        private void button2_Click(object sender, EventArgs e)
        {
            MainClass.ZReport();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            MainClass.XReport();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            MainClass.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            MainClass.ReturnSaleToGo(Convert.ToInt32(textBox1.Text));
            
        }
    }
}
