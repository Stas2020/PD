using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PDiscountCard
{
    public partial class AddWaterFrm : Form
    {
        public AddWaterFrm()
        {
            InitializeComponent();
            unitKeyboard1.NumberEnterEvent += new UnitKeyboard.NumberEnterDelegate(unitKeyboard1_NumberEnterEvent);
        }
        int WNumber = 0;
        void unitKeyboard1_NumberEnterEvent(object sender, int Num)
        {
            
            if (AlohaTSClass.GetWaitersList().Contains(Num))
            {
                label1.Text = AlohaTSClass.GetWaterName(Num) + Environment.NewLine +
                "Назначить чек этому официанту? ";
                panel1.Visible = true;
                WNumber = Num;
            }
            else
            {
                label1.Text = "Неверный номер официанта. " + Environment.NewLine + "Повторите ввод"; 
            }
        }

       

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
            this.Dispose();

        }

        private void BtnYes_Click(object sender, EventArgs e)
        {
            AlohaTSClass.SetWaterToCurentCheck(WNumber); 
            this.Close();
            AlohaTSClass.ShowMessage("Назначил чек официанту " + WNumber);
            this.Dispose();
        }

        private void BtnNo_Click(object sender, EventArgs e)
        {
            label1.Text = "Введите номер официанта";
            panel1.Visible = false; 
        }
    }
}
