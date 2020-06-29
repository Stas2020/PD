using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PDiscountCard
{
    public partial class CopySlipFrm : EnterNumberFrm 
    {
        internal int SlipNumber = 0;
        public CopySlipFrm()

        {
            InitializeComponent();
            label1.Text = "Введите номер слипа"; 
             
        }
        public  bool Arcus = false;
        //public int Num = 0;
        override protected   void OkEvent(int Num)
        {
            SlipNumber = Num;
            string s = "";
            if (Arcus)
            {
                ArcusAlohaIntegrator.GetSipCopy(Num); 

            }
            else
            {
                TrPosXAlohaIntegrator.GetJRNCheck(Num, false);
            }

            if (iniFile.Arcus2Enabled)
            { 
                
            }

            this.Hide();
        }
    }

}
