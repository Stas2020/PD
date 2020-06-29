using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Threading;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.RegularExpressions;

namespace SVSistem
{
    /// <summary>
    /// Interaction logic for WndCardPayment.xaml
    /// </summary>
    public partial class WndCardPayment : Window
    {
        public static readonly DependencyProperty CardTextProperty =
           DependencyProperty.Register("CardText", typeof(string),
           typeof(WndCardPayment), new UIPropertyMetadata(""));

        const int MaxCardLenght = 14;
        public string CardText
        {
            get { return (string)GetValue(CardTextProperty); }
            set {

                string out_string = Regex.Replace(value, @"[^\d]+", "");
                Utils.ToLog("Set CardText =" + out_string);
                if (out_string.Length > MaxCardLenght) return;
                SetValue(CardTextProperty, out_string); 
            }
        }


        public bool SendCardNumber(string bstrTrack1Info, string bstrTrack2Info, string bstrTrack3Info)
        {
            Utils.ToLog("SendCardNumber");
            try
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                    (Action)delegate()
                    {
                        CardText = bstrTrack1Info + bstrTrack2Info + bstrTrack3Info;
                    });
                
            }
            catch(Exception e)
            {
                Utils.ToLog("Error SendCardNumber "+e.Message);
            }
            return false;
        }

        private string CardPrefix
        {
            get
            {
                if (CardText.Length <= 5)
                {
                    return CardText;
                }
                else
                {
                    return CardText.Substring(0, 5);
                }
            }
        }
        private string CardNumber
        {
            get
            {
                if (CardText.Length <= 5)
                {
                    return "";
                }
                else
                {
                    return CardText.Substring(5);
                }
            }
        }

        private void SetWndType(int _Type)
        {
            if (_Type == 0)
            {
                TbCaption.Text = "Оплата бонусной картой";
                StSumm.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                TbCaption.Text = "Продажа бонусной карты";
                StSumm.Visibility = System.Windows.Visibility.Collapsed;
            }
            WndType = _Type;
        }
        private int WndType = 0;

        //static decimal def = 0;
        public static readonly DependencyProperty SummProperty =
           DependencyProperty.Register("Summ", typeof(decimal),
           typeof(WndCardPayment), new UIPropertyMetadata(0m));

        public decimal Summ
        {
            get { return Convert.ToDecimal(GetValue(SummProperty)); }
            set { SetValue(SummProperty, value); }
        }

        public WndCardPayment()
        {
            InitializeComponent();
        }
        private int CheckNum =0;
        public void AskPayment(decimal _Summ, int _Type, int _Check)
        {
            SetWndType(_Type);
            Summ = _Summ;
            CheckNum = _Check;
        }

      


        private void ctrlNumPad_SendKeyEvent(object sender, char C)
        {
            CardText += C;

        }

        private void ctrlNumPad_SendClearEvent(object sender)
        {
            CardText = "";
        }

        private void ctrlNumPad_SendOkEvent(object sender)
        {
            if (WndType == 0)
            {
                PayBySVCard();          
            }
            else if (WndType == 1)
            {
                AddSVCard();
            }

        }

        

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void SetError(string mess)
        {
            TbError.Text = mess;
            BrdErrorMessage.Visibility = System.Windows.Visibility.Visible;
        }

        private void PayBySVCard()
        {
            try
            {
                if (!((CardPrefix == "26603" && Summ == 3000) || (CardPrefix == "26605" && Summ == 5000) || (CardPrefix == "26610" && Summ == 10000)))
                {
                    SetError("Префикс карты неверен либо не совпадает с суммой продажи.");
                }
                else
                {
                    StopListSrv.SVSrv Cl = new StopListSrv.SVSrv();

                    StopListSrv.GetBalanceResult BalRes = Cl.GetBalance(CardPrefix, CardNumber);
                    if (BalRes.Sucsess)
                    {
                        if (BalRes.Balance < Summ)
                        {
                            SetError("Максимально возможная сумма снятия " + BalRes.Balance.ToString("0.00") + " р.");
                            Summ = BalRes.Balance;
                        }
                        StopListSrv.RespResult resp = Cl.AddRedemption(CardPrefix, CardNumber, Summ, Main.DepNum, CheckNum, Main.TermNum, false);
                        if (resp.Sucsess)
                        {
                            WndSucseess = true;
                            this.Close();
                        }
                        else
                        {
                            if (resp.ErrorCode == 1)
                            {

                            }
                            SetError(resp.ErrorMessage);
                        }
                    }
                    else
                    {

                        SetError(BalRes.ErrorMessage);
                    }

                }

            }
            catch (Exception e)
            {
                SetError("Ошибка оплаты картой. " + e.Message);
            }

        }

        private void AddSVCard()
        {
            try
            {
                if (!((CardPrefix == "26603" && Summ == 3000) || (CardPrefix == "26605" && Summ == 5000) || (CardPrefix == "26610" && Summ == 10000)))
                {
                    SetError("Префикс карты неверен либо не совпадает с суммой продажи.");
                }
                else
                {
                    StopListSrv.SVSrv Cl = new StopListSrv.SVSrv();
                    StopListSrv.RespResult resp = Cl.AddCard(CardPrefix, CardNumber, Summ, Main.DepNum, false);
                    if (resp.Sucsess)
                    {
                        WndSucseess = true;
                        this.Close();
                    }
                    else
                    {
                        SetError(resp.ErrorMessage);
                    }
                }
                
            }
            catch(Exception e)
            {
                SetError("Ошибка добавления карты. "+e.Message);
            }


        }

        public bool WndSucseess = false;

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            BrdErrorMessage.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void mWndCardPayment_Closed(object sender, EventArgs e)
        {
            Main.EndSale();
        }
    }
}
