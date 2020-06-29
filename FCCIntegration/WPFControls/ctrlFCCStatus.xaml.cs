using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FCCIntegration.WPFControls
{
    /// <summary>
    /// Interaction logic for ctrlFCCStatus.xaml
    /// </summary>
    public partial class ctrlFCCStatus : UserControl
    {
        Dictionary<int,ctrlBaraban > CoinBarabans = new Dictionary<int,ctrlBaraban> ();
        Dictionary<int, ctrlBaraban> BillBarabans = new Dictionary<int, ctrlBaraban>();
        public ctrlFCCStatus()
        {
            InitializeComponent();
            CoinBarabans.Add(4043, CoinStack1);
            CoinBarabans.Add(4044, CoinStack2);
            CoinBarabans.Add(4045, CoinStack3);
            CoinBarabans.Add(4046, CoinStack4);
            CoinBarabans.Add(4047, CoinStack5);
            CoinBarabans.Add(4048, CoinStack6);
            CoinBarabans.Add(4054, CoinStack7);
            CoinBarabans.Add(4055, CoinStack8);
            CoinBarabans.Add(4084, BarCoinIzl);


            BarBillKass.OneDenom = false;
            BarCoinIzl.OneDenom = false;
            foreach (ctrlBaraban cc in CoinBarabans.Values)
            {
                cc.MoneyType = 1;
            }

            BillBarabans.Add(4043, BarBill50);
            BillBarabans.Add(4044, BarBill100);
            BillBarabans.Add(4045, BarBill500);
            BillBarabans.Add(4059, BarBillKass);

        }


        public FCCSrv2.DenominationType[] GetBarabansAdd(bool All, bool Bills, bool Coins)
        {
            List<FCCSrv2.DenominationType> Tmp = new List<FCCSrv2.DenominationType>();
            if ((!All) || (Coins))
            {
                foreach (int k in CoinBarabans.Keys)
                {
                    if (k != 4084)
                    {
                        FCCSrv2.DenominationType dt = CoinBarabans[k].DenomType;
                        if (All)
                        {
                            dt.Piece = CoinBarabans[k].MoneyCount.ToString();
                        }
                        else
                        {
                            dt.Piece = ((int)(Math.Abs(CoinBarabans[k].AddMoneyValue) / CoinBarabans[k].DenomValue)).ToString();
                        }
                        dt.Status = "0";
                        if (dt.Piece != "0")
                        {
                            Tmp.Add(dt);
                        }
                    }

                }
            }
            if ((!All) || (Bills))
            {
                
                foreach (int k in BillBarabans.Keys)
                {
                    if (k != 4059)
                    {
                        FCCSrv2.DenominationType dt = BillBarabans[k].DenomType;
                        if (All)
                        {
                            dt.Piece = BillBarabans[k].MoneyCount.ToString();
                        }
                        else
                        {
                            dt.Piece = ((int)(Math.Abs(BillBarabans[k].AddMoneyValue) / BillBarabans[k].DenomValue)).ToString();
                        }
                        if (dt.Piece != "0")
                        {
                            Tmp.Add(dt);
                        }
                    }
                }
            }
            return Tmp.ToArray() ;
        
        }

        private bool btnsCountVisible=false;
        public bool BtnsCountVisible
        {
            set
            {
                btnsCountVisible = value;

                foreach(int k in CoinBarabans.Keys)
                {
                    if (k != 4084)
                    {
                      CoinBarabans[k].BtnsVisible = value;
                      
                    }
                }
                foreach (int k in BillBarabans.Keys)
                {
                    if (k != 4059)
                    {
                        BillBarabans[k].BtnsVisible = value;
                      
                    }
                }

            }
            get
            {
                return btnsCountVisible;
            }

        }

        public void SetNotFindStatus()
        {
            foreach (ctrlBaraban b in CoinBarabans.Values)
            {
                b.MoneyValue = -1;
            }
            BarBillKass.MoneyValue = -1;

            BarBill50.MoneyValue = -1;
            BarBill500.MoneyValue = -1;
            BarBill100.MoneyValue = -1;
        }

        public void SetBillValue(int Denom, int val, int State, int Maxm, int StackVal,FCCSrv2.DenominationType Den)
        {
            this.Dispatcher.Invoke((Action)(() =>
            {


                if (StackVal == 4059)
                {
                    BillBarabans[StackVal].DenomType = Den;
                    //CoinBarabans[StackVal].DenomValue = Denom;

                    BillBarabans[StackVal].MoneyCount = Denom;
                    BillBarabans[StackVal].MoneyValue = val;
                    BillBarabans[StackVal].MaxMoneyCount = Maxm;
                    // CoinBarabans[StackVal].MoneyState = State;
                }
                else
                {

                    BillBarabans[StackVal].DenomType = Den;
                    BillBarabans[StackVal].DenomValue = Denom;
                    BillBarabans[StackVal].MoneyValue = val * Denom;
                    BillBarabans[StackVal].MaxMoneyCount = Maxm;
                    BillBarabans[StackVal].MoneyState = State;
                }





                /*
            switch (Denom)
            {
                case 0:
                    BarBillKass.MoneyCount = State;
                    BarBillKass.MoneyValue = Count;
                    //BarBillKass.MoneyState = State;
                    //BarBillKass.MoneyState = State;
                    break;
                case 5000:
                    BarBill50.DenomType = Den;
                    BarBill50.MoneyValue = 0;
                    BarBill50.MoneyValue = Count * 5000;
                    BarBill50.MoneyState = State;
                    break;
                case 10000:
                    BarBill100.DenomType = Den;
                    BarBill100.MoneyValue = Count * 10000;
                    BarBill100.MoneyState = State;
                    break;
                case 50000:
                    BarBill500.DenomType = Den;
                    BarBill500.MoneyValue = Count * 50000;
                    BarBill500.MoneyState = State;
                    break;
                default:
                    break;
                






            }
                 *  * */
            }));

        }
        public void SetCoinValue(int Denom, int val, int State,int Maxm,int StackVal,FCCSrv2.DenominationType Den)
        {
             this.Dispatcher.Invoke((Action)(() =>
            {

                if (StackVal == 4084)
                {
                    CoinBarabans[StackVal].DenomType = Den;
                    //CoinBarabans[StackVal].DenomValue = Denom;

                    CoinBarabans[StackVal].MoneyCount = Denom;
                    CoinBarabans[StackVal].MoneyValue = val;
                    CoinBarabans[StackVal].MaxMoneyCount = Maxm;
                   // CoinBarabans[StackVal].MoneyState = State;
                }
                else
                {

                    CoinBarabans[StackVal].DenomType = Den;
                    CoinBarabans[StackVal].DenomValue = Denom;
                    CoinBarabans[StackVal].MoneyValue = val * Denom;
                    CoinBarabans[StackVal].MaxMoneyCount = Maxm;
                    CoinBarabans[StackVal].MoneyState = State;
                }



                /*
            switch (Denom)
            {
                case 0:
                    BarCoinIzl.MoneyValue = val;
                    BarCoinIzl.MoneyState = State;
                    break;
                case 50:
                    BarCoin05.MoneyValue = val * 50;
                    BarCoin05.MoneyState = State;
                    break;
                case 100:
                    BarCoin1.MoneyValue = val * 100;
                    BarCoin1.MoneyState = State;
                    break;
                case 200:
                    BarCoin2.MoneyValue = val * 200;
                    BarCoin2.MoneyState = State;
                    break;

                case 500:
                    BarCoin5.MoneyValue = val * 500;
                    BarCoin5.MoneyState = State;
                    break;
                case 1000:
                    BarCoin10.MoneyValue = val * 1000;
                    BarCoin10.MoneyState = State;
                    break;
                default:
                    break;
            }
                 * */
            }));

        }

        public void ClearBillAdd()
        {
            /*
            BarBillKass.AddMoneyValue = 0;

            BarBill50.AddMoneyValue = 0;

            BarBill100.AddMoneyValue = 0;

            BarBill500.AddMoneyValue = 0;

             * */
            foreach (ctrlBaraban bar in BillBarabans.Values)
            {
                bar.AddMoneyValue = 0;
            }

        }
        public void ClearCoinAdd()
        {
            BarCoinIzl.AddMoneyValue = 0;
          
            foreach (ctrlBaraban bar in CoinBarabans.Values)
            {
                bar.AddMoneyValue = 0;
            }
        }
        public void SetBillAdd(int Denom, int Count, int State)
        {

            this.Dispatcher.Invoke((Action)(() =>
            {
                if (BillBarabans.Values.Where(a => a.DenomValue == Denom).Count() > 0)
                {
                    BillBarabans.Values.Where(a => a.DenomValue == Denom).FirstOrDefault().AddMoneyValue = Count * Denom;
                }
                /*
              if (Denom==0)
              {
                  BillBarabans[4059].AddMoneyValue = Count;
              }
                */
                /*
                switch (Denom)
                {
                    case 0:
                        BarBillKass.AddMoneyValue = Count;
                        //  BarBillKass.MoneyState = State;
                        break;
                    case 5000:
                        BarBill50.AddMoneyValue = Count * 5000;
                        // BarBill50.MoneyState = State;
                        break;
                    case 10000:
                        BarBill100.AddMoneyValue = Count * 10000;
                        //  BarBill100.MoneyState = State;
                        break;
                    case 50000:
                        BarBill500.AddMoneyValue = Count * 50000;
                        //  BarBill500.MoneyState = State;
                        break;
                    default:
                        BarBillKass.AddMoneyValue = Count*Denom;
                        break;
                }
                 * */
            }));

        }
        public void SetCoinAdd(int Denom, int Count, int State)
        {

            this.Dispatcher.Invoke((Action)(() =>
            {
                CoinBarabans.Values.Where(a=>a.DenomValue==Denom).FirstOrDefault().AddMoneyValue = Count*Denom;
                



                /*
                switch (Denom)
                {

                case 0:
                    BarCoinIzl.AddMoneyValue = Count;
                    break;

                case 50:
                    BarCoin05.AddMoneyValue = Count * 50;
                    break;

                case 100:
                    BarCoin1.AddMoneyValue = Count * 100;
                    break;

                case 200:
                    BarCoin2.AddMoneyValue = Count * 200;
                    break;

                case 500:
                    BarCoin5.AddMoneyValue = Count * 500;
                    break;
                case 1000:
                    BarCoin10.AddMoneyValue = Count * 1000;
                    break;

                default:
                    break;
                }
                 * */
            }));

        }
    }
}
