using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PDiscountCard
{
    public partial class VoidFrm : Form
    {
        private decimal Sum = 0;
        public VoidFrm(decimal _Sum)
        {
            InitializeComponent();
            Sum = _Sum;
            unitKeyboard1.NumberEnterEvent += new UnitKeyboard.NumberEnterDelegate(unitKeyboard1_NumberEnterEvent); 
  
        }

        void unitKeyboard1_NumberEnterEvent(object sender, int Num)
        {
            EnterrNumber(Num);
        }

        internal void EnterrNumber(int Num)
        {
            
            string s= TrPosXAlohaIntegrator.GetJRNCheck(Num);
        }

        internal void EnterrNumber(int Num, decimal sum)
        {
            
            
            if (sum!=Math.Abs(   Sum))
            {
                label2.Text = "Не совпадают сумма чека в Алохе (" + Math.Abs(Sum).ToString() + "руб.)" + Environment.NewLine + " и в терминале (" + sum.ToString() + "руб.)";
                button1.Enabled = false;
            }
            else
            {

                label2.Text = "Чек №" + Num + " на сумму " + sum + "руб." + Environment.NewLine + "Верно?";
                button1.Enabled = true;
                ChNum = Num;
            }
            panel1.Visible = true;
            
        }
        public int ChNum;
        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide(); 
        }

        public bool Cancel = false;
        private void button2_Click(object sender, EventArgs e)
        {
            Cancel = true;
            this.Hide(); 
        }
    }
}
