using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PDiscountCard
{
    public partial class FrmStopList : Form
    {
        public FrmStopList()
        {
            InitializeComponent();
            
        }


        int MaxPages
        {
            get
            {
             return   DishList.Count / RowsInLabel;
            }
        }

        private int RowsInLabel
        {
            get

            {
               return  label2.Height / label2.Font.Height-1;
            }
        }
        List<string> DishList = new List<string>();
        int _PageNum = 1;
        int PageNum
        {
            set
            {
                _PageNum = value;
                UpdateScreen();
                button1.Visible = (value>1);

                button2.Visible = (value < MaxPages);
                
            }
            get
            {
                return _PageNum;
            }
        }


        private List<int> GetNotShowerList()
        {
            List<int> Tmp = new List<int>();

            try
            {
                List<int> StopListSubMnuNotShower = iniFile.StopListSubMnuNotShower;
                if (StopListSubMnuNotShower != null)
                {
                    foreach (int SmnuNum in iniFile.StopListSubMnuNotShower)
                    {
                        Tmp.AddRange(AlohaTSClass.DishInSMnu(SmnuNum));
                    }
                }
            }
            catch
            { 
            
            }
            return Tmp;
        }

        public void SetDishList(List<StopListDish> mDishList)
        {
            DishList.Clear();
            foreach (StopListDish StL in mDishList)
            {
                //DishList.
                if (GetNotShowerList().Contains(StL.BarCode)) continue;
                
                DishList.Add(StL.Name);
            }

            DishList.Sort();
            
            PageNum = 1;
            UpdateScreen();
        }

        private void UpdateScreen()
        { 
            label2.Text="";
            label3.Text="";

            for(int i=(PageNum-1)*RowsInLabel*2; i< Math.Min(DishList.Count,PageNum*RowsInLabel*2);i++)
            {
                if (i - (PageNum - 1) * RowsInLabel * 2 <  RowsInLabel)
                {
                    label2.Text += DishList[i] + Environment.NewLine;
                }
                else
                {
                    label3.Text += DishList[i] + Environment.NewLine;
                }
            }
        
        }

        private void button1_Click(object sender, EventArgs e)
        {
            PageNum--;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            PageNum++;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            AlohaTSClass.PrintStopList(DishList);
        }
    }
}
