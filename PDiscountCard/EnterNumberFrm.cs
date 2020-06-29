using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PDiscountCard
{
    public partial class EnterNumberFrm : Form
    {
        
        public EnterNumberFrm()
        {
            InitializeComponent();
            unitKeyboard1.NumberEnterEvent += new UnitKeyboard.NumberEnterDelegate(unitKeyboard1_NumberEnterEvent); 
          }

        void unitKeyboard1_NumberEnterEvent(object sender, int Num)
        {
            OkEvent(Num);
        }


        private void button1_Click(object sender, EventArgs e)
        {
            
            this.Hide(); 
        }
        virtual protected  void OkEvent(int Num)
        { 
        
        }

        public bool Cancel = false;
        private void button2_Click(object sender, EventArgs e)
        {
            Cancel = true;
            this.Hide(); 
        }
    }
}
