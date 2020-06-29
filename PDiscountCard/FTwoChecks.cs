using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PDiscountCard
{
    public partial class FTwoChecks : MF3
    {
        public FTwoChecks()
        {
            InitializeComponent();
        }

        public int Result = 0;

        private string Getword(int a)
        {
            string n ="чек";
            switch (a.ToString().Substring(a.ToString().Length -1,1))
            {
                case "1":
                    return n;
                case "2":
                    return n + "а"; 
                    case "3":
                    return n + "а"; ;
                    case "4":
                    return n + "а"; ;
                default:
                    return n + "ов";
                    
            }
        }

        

        public  void Init(List<Check> Checks)
        {
            SetLabHeight(Checks.Count+2);
            label1.Text = "На столе " + Checks.Count + " незакрытых " + Getword(Checks.Count) + ": " + Environment.NewLine;
            decimal summ=0;
                foreach (Check Ch in Checks)
                {
                    label1.Text+= Ch.CheckShortNum  +":  " + Ch.Summ +"руб."+Environment.NewLine;
                    summ += Ch.Summ;
                }
                label1.Text += "Итого: "+summ +"руб."+Environment.NewLine;
            button1.Text = "Объединить сумму";
            button2.Text = "Только текущий чек";
            button1.Click += new EventHandler(button1_Click);
            button2.Click += new EventHandler(button2_Click); 
        }

        void button2_Click(object sender, EventArgs e)
        {
            Result = 0;
            this.Close(); 
        }

        void button1_Click(object sender, EventArgs e)
        {
            Result = 1;
            this.Close(); 
        }
    }
}
