using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PDTest
{
    public partial class FrmFayRetailTest : Form
    {
        public FrmFayRetailTest()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //PDiscountCard.FayRetail.FayRetailClient.ApplyCardToCheck()
            string Status = "";
            System.Net.HttpStatusCode StatusCode = System.Net.HttpStatusCode.OK;
            string resp= PDiscountCard.FayRetail.FayRetailClient.SendDataToSrv(textBox1.Text, out Status, out StatusCode);
            textBox2.Text = "Status: " + Status + Environment.NewLine + resp;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string s = PDiscountCard.FayRetail.FayRetailTest.GetTest1XmlString();
            textBox1.Text = s;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string s = PDiscountCard.FayRetail.FayRetailTest.Test2();
            textBox2.Text = s;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string s = PDiscountCard.FayRetail.FayRetailTest.TestBalance();
            textBox2.Text = s;
        }

        private void button5_Click(object sender, EventArgs e)
        {

            PDiscountCard.FayRetail.XMLSerializer.ResponseDeSerializer(textBox1.Text);

        }

        private void button6_Click(object sender, EventArgs e)
        {
            PDiscountCard.FayRetail.FayRetailTest.TestWnd();
            decimal val = 1253.36m;
                MessageBox.Show(val.ToString("C2"));

        }

        private void button7_Click(object sender, EventArgs e)
        {
            string s = PDiscountCard.FayRetail.FayRetailTest.TestAddBonus(textBox3.Text);
            textBox2.Text = s;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            string s = PDiscountCard.FayRetail.FayRetailTest.TestConfirmPurchase(textBox3.Text);
            textBox2.Text += s;
        }

        private void button9_Click(object sender, EventArgs e)
        {
            string s = PDiscountCard.FayRetail.FayRetailTest.TestPayment(textBox3.Text, Convert.ToDouble(textBox4.Text));
            textBox2.Text = s;
        }

        private void button10_Click(object sender, EventArgs e)
        {
            List<string> s = PDiscountCard.FayRetail.FayRetailTest.TestClose();
            textBox2.Text += "TestClose " + Environment.NewLine;
                foreach(string ss in s)
                {
                textBox2.Text += ss+Environment.NewLine;
                }
        }

        private void button11_Click(object sender, EventArgs e)
        {

        }
    }
}
