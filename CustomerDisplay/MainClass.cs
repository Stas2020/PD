using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;


namespace CustomerDisplay
{
    public static class MainClass
    {
        private static frmFCCUserDialog Mfrm ;//= new frmFCCUserDialog();

        public static ctrlMoneyDialog mctrlMoneyDialog
        {
            get
            {
                if (Mfrm == null)
                {
                    return null;
                
                }
                else
                {
                return Mfrm.mMoneyDialog;
                }
            }
        }
        public static void Init()
        {
            Mfrm = new frmFCCUserDialog();
            try
            {
                Screen[] sc;
                sc = Screen.AllScreens;
             
                Mfrm.Left = sc[1].Bounds.Left;
                Mfrm.Top = sc[1].Bounds.Top;
            }
            catch
            {
            
            }
            //Mfrm.Location = sc[1].Bounds.Location;
            Mfrm.Show();
        }

        public static void SetPictureState(int i)
        {
            Mfrm.SetPictureState(i);
        }

        public static void SetCheck(CustomerCheck Chk)
        {
            Mfrm.SetDishez(Chk);
            Mfrm.ShowPaiment(true);
        }

        private static int mTotal = 0;
        //private static int mTotal = 0;

        public static void EndPayment()
        {
            Mfrm.StartEndPayment();   
        }

        public static void SetTotal(int Summ)
        {
            mTotal = Summ;
            mctrlMoneyDialog.SetTotal(Summ);
            mctrlMoneyDialog.SetChange(0);
            mctrlMoneyDialog.SetAlredy(0);
        }
        public static void SetChange(int Summ)
        {
            
            mctrlMoneyDialog.SetChange(Summ);
        }
        public static void SetDeposit(int Summ)
        {
            if (mTotal > Summ)
            {
                mctrlMoneyDialog.SetAlredy(mTotal - Summ);
            }
            else
            {
                mctrlMoneyDialog.SetAlredy(0);
                mctrlMoneyDialog.SetChange(Summ-mTotal);
            }
            mctrlMoneyDialog.SetDeposit(Summ);
        }
        public static void SetAlredy(int Summ)
        {
            mctrlMoneyDialog.SetAlredy(Summ);
        }
        public static void SetStatus(string StatusMsg)
        {
            mctrlMoneyDialog.SetStatus(StatusMsg);
        }


        public static void ChangeCanceled(int Change)
        {
            if (Change == 0)
            {
                mctrlMoneyDialog.SetStatus("Оплата отменена");
                
            }
            else
            {
                mctrlMoneyDialog.SetStatus("Обратитесь к кассиру для возврата денежных средств.");
            }
            Mfrm.ClearCheck();
            Mfrm.ClearMoney();
            SetPictureState(0);

        }

        /*
        public static void ShowMessage(string Msg)
        { 
        Mfrm.se
        }
         * */
    }
}
