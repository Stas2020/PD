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
using System.Threading;
using System.Windows.Threading;


namespace PDiscountCard.FayRetail
{
    /// <summary>
    /// Interaction logic for ctrlApplyFayRetailCard.xaml
    /// </summary>
    public partial class ctrlApplyFayRetailCard : UserControl
    {
        private System.Timers.Timer T1 = new System.Timers.Timer();
        private FayRetailCheckInfo CurentCheck = null;
        private string CurentCashier = "";
        
        private double MaxBonusSumm = 0;

        public ctrlApplyFayRetailCard()
        {
            InitializeComponent();
            digitKeyboard.OnOk += new ctrlDigitalKeyBoard.OkEventHandler(digitKeyboard_OnOk);
            digitKeyboard.OnCancel += new ctrlDigitalKeyBoard.OkEventHandler(digitKeyboard_OnCancel);
            System.Windows.Forms.InputLanguage.CurrentInputLanguage = System.Windows.Forms.InputLanguage.FromCulture(new System.Globalization.CultureInfo("en-US"));
            
            T1.Elapsed += new System.Timers.ElapsedEventHandler(T1_Elapsed);
            T1.Interval = 400;
            SetVisState(0);
            tbCode.Focus();
        }

        public void SettbCodeFocus()
        {
            tbCode.Focus();
        }

        void digitKeyboard_OnCancel(object sender, double Value)
        {
            SetVisState(1);
               
        }

        void digitKeyboard_OnOk(object sender, double Value)
        {
            Utils.ToCardLog("digitKeyboard_OnOk Value = " + Value.ToString() + " MaxBonusSumm " + MaxBonusSumm.ToString());
            if (!((Value > 0 && (Value <= MaxBonusSumm))))
            {
                tbInfoMsg.Text = "Некорректная сумма";
            }
            else
            {
                ApplyDiscount(Value);
            }
        }


        private void ApplyDiscount(double Amount)
        {
            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                (ThreadStart)delegate()
                {
                    string DiscErr = "";
                    int CompId = AlohaTSClass.ApplyComp(iniFile.FayRetailDiscountId, out DiscErr,Amount);
                    if (CompId == 0)
                    {
                        tbCode.Focus();
                        tbError.Text = "Ошибка применения скидки на чек " + DiscErr;
                        SetVisState(0);
                    }
                    else
                    {
                        
                        CurentCheck = FayRetail.FayRetailMain.GetCurentFRCheck();
                        string CardNum = tbCode.Text;
                        string ErrMsg = "";
                        bool Res = FayRetailClient.ApplyFayRetPaymentToCheck(CardNum, CurentCheck, CurentCashier, Amount, out ErrMsg);
                        Utils.ToCardLog("ApplyDiscount FayRetailClient.ApplyFayRetPaymentToCheck end Res: " + Res.ToString());

                        if (!Res)
                        {
                            tbCode.Focus();
                            tbError.Text = ErrMsg;
                            AlohaTSClass.DeleteComp(CompId);
                            SetVisState(0);
                        }
                        else
                        {
                            FayRetailMain.AddCheckToAddBonus(CurentCheck.ChequeNumber, CardNum);
                            tbFinalMsg.Text = "Успешно. " + ErrMsg;
                            SetVisState(3);
                            }
                        

                    }
                }
            );
        }

        void SetVisState(int State)
        {
            StSweepCard.Visibility = System.Windows.Visibility.Hidden;
            StCardInfo.Visibility = System.Windows.Visibility.Hidden;
            grDigitalKeys.Visibility = System.Windows.Visibility.Hidden;
            grFinalMsg.Visibility = System.Windows.Visibility.Hidden;

            switch (State)
            {
                case 0:
                    {
                        StSweepCard.Visibility = System.Windows.Visibility.Visible;
                        tbCode.Focus();
                        break;
                    }
                case 1:
                    {
                        StCardInfo.Visibility = System.Windows.Visibility.Visible;
                        break;
                    }
                case 2:
                    {
                        grDigitalKeys.Visibility = System.Windows.Visibility.Visible;
                        break;
                    }
                case 3:
                    {
                        grFinalMsg.Visibility = System.Windows.Visibility.Visible;
                        break;
                    }
                default:
                    break;
            }
        }

        private Window OwnerWnd;
        internal void SetOwnerWnd(Window _OwnerWnd)
        {
            OwnerWnd = _OwnerWnd;
        }

        public void SetCurCheck(FayRetailCheckInfo curentCheck, string cashier)
        {
            CurentCheck = curentCheck;
            CurentCashier = cashier;
        }



        private void CardRead()
        {
            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                    (ThreadStart)delegate()
                    {
                        string CardNum = tbCode.Text;
                        string ErrMsg = "";

                        BalanceResponse BalResp = FayRetailClient.GetBalance(CardNum, CurentCashier, out ErrMsg);
                        if (BalResp == null)
                        {
                            tbCode.Focus();
                            tbError.Text = ErrMsg;
                        }
                        else
                        {


                            CalculateResponse Resp = FayRetailClient.ApplyCardToCheck(CardNum, CurentCheck, CurentCashier, out ErrMsg);


                            if (Resp == null)
                            {
                                tbCode.Focus();
                                tbError.Text = ErrMsg;
                            }
                            else
                            {
                                guestName.Text = BalResp.Card.Holder; //Resp.Card.Holder;
                                if ((CurentCheck.Items != null) && (CurentCheck.Items.Count() > 0))
                                {
                                    tbAllChkSumm.Text = CurentCheck.Items.Sum(a => a.Amount).ToString("C2");
                                }
                                else
                                {
                                    tbAllChkSumm.Text = "0";
                                }
                                tbBalance.Text = BalResp.BonusAmount.ToString("C2");
                                tbBalanceAvaible.Text = Resp.AvailableAmount.ToString("C2");
                                MaxBonusSumm = Math.Min(CurentCheck.TotalSumm - 1, Math.Floor(Resp.AvailableAmount * 100) / 100);
                                SetVisState(1);

                            }
                        }
                    }
                );
        }

        void T1_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (!T1.Enabled) return;
            if ((DateTime.Now - LastKeyPress).TotalMilliseconds > 500)
            {
                T1.Stop();
                CardRead();
            }
        }




        private void tbCode_TextChanged(object sender, TextChangedEventArgs e)
        {
            string c = tbCode.Text;
        }

        DateTime LastKeyPress = DateTime.Now;
        private void tbCode_KeyUp(object sender, KeyEventArgs e)
        {

            tbError.Text = "Думаю...";
            LastKeyPress = DateTime.Now;
            T1.Start();
        }

        private void tbCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (!T1.Enabled)
            {
                tbCode.Text = "";
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            
            tbError.Text = "";
            tbCode.Text = "";
            tbCode.Focus();
            OwnerWnd.Close();
        }

        private void btnAddBonus_Click(object sender, RoutedEventArgs e)
        {
            AddBonusToCard();
        }

        private void AddBonusToCard()
        {
            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
               (ThreadStart)delegate()
               {
                   string CardNum = tbCode.Text;
                   FayRetailMain.AddCheckToAddBonus(CurentCheck.ChequeNumber, CardNum);
                   tbFinalMsg.Text = "Успешно. "; 
                   SetVisState(3);
                   
                   /*
                   string ErrMsg = "";
                   bool Res = FayRetailClient.AddBonustoCard(CardNum, CurentCheck, CurentCashier, out ErrMsg);

                   if (!Res)
                   {
                       tbCode.Focus();
                       tbError.Text = ErrMsg;
                       SetVisState(0);
                   }
                   else
                   {
                       FayRetailMain.AddCheckToAddBonus(CurentCheck.ChequeNumber, CardNum);
                       tbFinalMsg.Text = "Успешно. " +Environment.NewLine+ ErrMsg;
                       SetVisState(3);
                   }
                    * */
               }
           );


        }

        private void btnDiscount_Click(object sender, RoutedEventArgs e)
        {
            tbMaxSumm.Text = MaxBonusSumm.ToString("c2");
            digitKeyboard.CurentText = MaxBonusSumm.ToString("0.00");
            digitKeyboard.NextTouchClear = true;
            SetVisState(2);

        }

        private void btnTest_Click(object sender, RoutedEventArgs e)
        {
            //tbCode.Text = "2611000037125";
            tbCode.Text = "2611000000105";
            CardRead();
        }
    }
}
