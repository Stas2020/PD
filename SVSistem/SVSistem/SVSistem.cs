using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SVSistem
{
    public static class Main
    {
        public static double Summ = 0;
        public static void StartPayment(decimal DefaultSumm,int CheckNum, EndSaleDelegate callback)
        {
            
            CurentEndSaleCallBack = callback;
            WndCardPayment Wndp = new WndCardPayment();
            Wndp.AskPayment(DefaultSumm, CheckNum, 0);
            Wndp.Show();
        }

        public delegate bool RegCardSubScriberdelegate(string bstrTrack1Info, string bstrTrack2Info, string bstrTrack3Info);
        public delegate void RegDel(RegCardSubScriberdelegate v);

        public static bool GetCardFromMagReader(string bstrTrack1Info, string bstrTrack2Info, string bstrTrack3Info)
        {
            Utils.ToLog("GetCardFromMagReader");
            if (Wndp != null)
            {
               return Wndp.SendCardNumber(bstrTrack1Info, bstrTrack2Info, bstrTrack3Info);
            }
            return true;
        }

        static WndCardPayment Wndp = new WndCardPayment();
        public delegate void EndSaleDelegate(bool res, string Cardnumber, double Summ);
        private static EndSaleDelegate CurentEndSaleCallBack;

        public static void StartSale(decimal Balance,int CheckNum, EndSaleDelegate callback)
        {
            CurentEndSaleCallBack = callback;
            Wndp = new WndCardPayment();
            Wndp.AskPayment(Balance,1, CheckNum);
            Wndp.Show();

            
        }

        public static void EndSale()
        {

            string  CarNum = Wndp.CardText;
            double Summ = (double)Wndp.Summ;
            bool res = Wndp.WndSucseess;
            Wndp = null;
            CurentEndSaleCallBack(res, CarNum,Summ);
            
        }

        public static void AddCard(string CardPrefix,string CardNum, double Balance, int CheckNum)
        {
            
            
                StopListSrv.SVSrv Cl = new StopListSrv.SVSrv();
                StopListSrv.RespResult resp = Cl.AddCard(CardPrefix, CardNum, (decimal)Balance, Main.DepNum, true);
            
        }

        public static void Init(int _DepNum, int _TermNum)
        {
            DepNum = _DepNum;
            TermNum = _TermNum;
            
        }
        internal static  int DepNum { set; get; }
        internal static int TermNum { set; get; }
        

    }
}
