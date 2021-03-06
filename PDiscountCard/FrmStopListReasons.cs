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
    public partial class FrmStopListReasons : Form
    {
        public FrmStopListReasons()
        {
            InitializeComponent();
            
        }


        int MaxPages
        {
            get
            {
             return   DishList.Count / RowsInLabel+1;
            }
        }

        private int RowsInLabel
        {
            get

            {
                CtrStopListReasonChooser Ch = new CtrStopListReasonChooser();

               return  panel9.Height / Ch.Height-1;
            }
        }
        List<StopListDishReason> DishList = new List<StopListDishReason>();
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

            List<int> StopListSubMnuNotShower = iniFile.StopListSubMnuNotShower;
            if (StopListSubMnuNotShower != null)
            {
                foreach (int SmnuNum in iniFile.StopListSubMnuNotShower)
                {
                    Tmp.AddRange(AlohaTSClass.DishInSMnu(SmnuNum));
                }
            }
            return Tmp;
        }

        List<StopListService.StopListReasonT> ReasonTypes = new List<StopListService.StopListReasonT>();
        public void SetDishList(List<StopListDish> mDishList)
        {
            DishList.Clear();
            /*
            foreach (StopListDish StL in mDishList)
            {
                //DishList.
                if (GetNotShowerList().Contains(StL.BarCode)) continue;
                
                DishList.Add(StL.Name);
            }
            */
            //DishList = mDishList.OrderBy(a=>a.Name).ToList();

            string Err = "";
            StopListService.Service1 s1 = new StopListService.Service1();

            StopListService.StopListReasonEvent[] Cr = s1.GetCurentResons(AlohainiFile.DepNum, out Err);

            List<StopListService.StopListReasonEvent> CurRes = s1.GetCurentResons(AlohainiFile.DepNum, out Err).ToList();

            ReasonTypes = s1.GetResonTypes().ToList();

            foreach (StopListDish D in mDishList)
            {
                StopListDishReason d =  new StopListDishReason (mDishList.Where(b => b.BarCode == D.BarCode).First());
                if (CurRes.Select(a =>a.BarCode).Contains(D.BarCode))
                {
                    
                    d.Reason = (int)CurRes.Where(a => a.BarCode == D.BarCode).First().TypeNum;
                    
                }

                if (d.mCtrStopListReasonChooser == null)
                {
                    d.mCtrStopListReasonChooser = new CtrStopListReasonChooser(d, ReasonTypes);
                }
                d.mCtrStopListReasonChooser.Dock = DockStyle.Top;

                DishList.Add(d);
            }
            DishList = DishList.OrderBy(a => a.Name).ToList();

         // DishList.Sort();

            
            
            PageNum = 1;
            //UpdateScreen();


        }

        int LineCount = 13;

        
        private void UpdateScreen()
        {
            
            panel9.Controls.Clear();


            //for (int i = (PageNum-1)*RowsInLabel; i< Math.Min(DishList.Count,PageNum*RowsInLabel);i++)
            for (int i = Math.Min(DishList.Count, PageNum * RowsInLabel)-1; i>(PageNum - 1) * RowsInLabel-1;  i--)


            //foreach (StopListDish StL in DishList)
            {
                StopListDishReason StL = DishList[i];
                panel9.Controls.Add(StL.mCtrStopListReasonChooser);
                
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
            UpdateReasons();
            this.Close();
        }

        private void UpdateReasons()
        { 
            StopListService.Service1 s1 = new StopListService.Service1();
            List<StopListService.StopListReasonEvent> SLS = new List<StopListService.StopListReasonEvent> ();
            string Err = "";

            foreach(StopListDishReason Ch in DishList)
                //foreach (CtrStopListReasonChooser Ch in StopListStrls)
            {
                
                StopListService.StopListReasonEvent Se = new StopListService.StopListReasonEvent ();
                Se.BarCode = Ch.BarCode;
                Se.Dep = AlohainiFile.DepNum;
                //Se.DepName = AlohainiFile.d
                Se.RDateTime = DateTime.Now;
                Se.TypeNum = Ch.mCtrStopListReasonChooser.ReasonId;
                SLS .Add(Se);
                
            }
            s1.AddStopListReasons(SLS.ToArray(), out Err);
            if (Err != "")
            {
                AlohaTSClass.ShowMessage(Err);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //UpdateReasons();
            //AlohaTSClass.PrintStopList(DishList);
        }
    }
}
