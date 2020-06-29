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
    /// Interaction logic for frmRazmen.xaml
    /// </summary>
    public partial class frmRazmen : Window
    {
        public frmRazmen()
        {
            InitializeComponent();
            FCCApi = new CFCCApi();
            CFCCApi.OnSetStatus += new EventHandleClass.SetStatusEventHandler(FCCApi_OnSetStatus);
            CFCCApi.OnReplenishCountChange += new EventHandleClass.ReplenishCountChangeEventHandler(CFCCApi_OnReplenishCountChange);
            CFCCApi.OnCashOutComplited += new CFCCApi.OnCashOutComplitedHandler(CFCCApi_OnCashOutComplited);

        }


        
        public event MainClass2.HideFrmEventHandler OnHidefrm;
      

        private bool NeedStartCashOut = false;
        private bool NeedUpdateDenom = false;


        void FCCApi_OnSetStatus(object sender, bool StatusChange, int status, int DevId, string EventName)
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                if (StatusChange)
                {

                    if (status == CFCCApi.STATUS_CODE_DISPENSEREMOVWAIT)
                    {

                        ShowProcessMessage("Заберите деньги из лотка.");

                    }
                    else if (status == CFCCApi.STATUS_CODE_DEPOSITREMOVWAIT)
                    {
                        ShowProcessMessage("Заберите непринятые купюры.");

                    }
                    else if (status == CFCCApi.STATUS_CODE_DEPOSIT_WAIT)
                    {

                        HideProcessMessage();

                    }
                    else if (status == CFCCApi.STATUS_CODE_IDLE)
                    {
                        if ( State == State_CashOutInProcess)
                        {
                            State = State_normal;
                            
                        }

                        if (NeedUpdateDenom)
                        {
                            NeedUpdateDenom = false;
                           // ShowProcessMessage("Пересчет наличности.");
                            InvetoryCalculations.UpdateMyDenominations();
                            NeedStartCashOut = false;
                            StartCashOut();
                           // return;
                        }
                        else if (NeedStartCashOut)
                        {
                            NeedStartCashOut = false;
                          //  ShowProcessMessage("Выдача денег.");
                            StartCashOut();
                        }
                        else
                        {
                            HideProcessMessage();
                        }
                      
                    }
                    


                }

             

            }));
        }

        void CFCCApi_OnCashOutComplited(FCCSrv2.CashoutOperationCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                if (e.Result.result == CFCCApi.FCC_SUCCESS.ToString())
                {
                    MonDeposit.SetMoney(GetSumm(e.Result.Cash));
                    Utils.ToMoneyCountLog(MoneyChangeCommands.Razmen, AddSumm, GetSumm(e.Result.Cash),0,0,0);

                }
                else
                {
                    ShowEror("Ошибка завершения операции выдачи. Код " + e.Result);
                }
            }
            else
            {
                ShowEror("Ошибка завершения операции выдачи. " + e.Error.Message);
            }
        }

        void CFCCApi_OnReplenishCountChange(object sender, FCCSrv2.CashType[] Ct)
        {
            UpdateReplenishAdd(Ct);
        }

        const int State_normal = 0;
        const int State_replenishInProcess = 1;
        const int State_replenishEnd = 2;
        const int State_CashOutInProcess = 3;

        private int state = 0;
        internal int State
        {
            set
            {
                this.Dispatcher.Invoke((Action)(() =>
        {
            state = value;
            switch (value)
            {
                case State_normal:
                    btnRepl.IsEnabled = true;
                    btnRepl.Content = "Внесение";
                    btnExit.IsEnabled = true;
                    break;
                case State_replenishInProcess:
                    AddSumm = 0;
                    MonDeposit.SetMoney(0);
                    btnRepl.IsEnabled = true;
                    lblStatus.Text = "Внесите денежные средства";
                    btnRepl.Content = "Закончить внесение";
                    btnExit.IsEnabled = false;
                    break;
                case State_replenishEnd:
                    //btnRepl.IsEnabled = true;
                    NeedUpdateDenom = true;
                    NeedStartCashOut = true;
                    btnRepl.Content = "Закончить внесение";
                    break;
                case State_CashOutInProcess:
                    //btnRepl.IsEnabled = true;

                    //btnRepl.Content = "Закончить внесение";
                    //lblStatus.Text = "Выдача ltyt";
                    break;

                default:
                    break;
            }
        }));
            }
            get
            {
                return state;
            }
        }

        private bool NeedEnableBtn = false;
        private void StartCashOut()
        {
            FCCSrv2.DenominationType[] Dt = InvetoryCalculations.GetDtsBySum(AddSumm);

            string res = FCCApi.CashOut(Dt);
            
            if (res == "")
            {
                State = State_CashOutInProcess;
                ShowProcessMessage("Выдача денежных средств..");
            }
            else
            {
                ShowEror("Ошибка старта выдачи денег код: " + res);
            }
        }

        private void StartReplenishmentRes(int ResId, string ResStr)
        {
            if (ResId == CFCCApi.FCC_SUCCESS)
            {
                State = State_replenishInProcess;
            }
            else
            {
                btnRepl.IsEnabled = false;
                FCCApi.EndReplenishment(EndReplenishmentRes);
                ShowEror(ResStr);

            }
        }
        int addSumm = 0;
        int AddSumm
        {
            set
            {
                addSumm = value;
                this.Dispatcher.Invoke((Action)(() =>
           {
               MonTotal.SetMoney(addSumm);
           }));
            }
            get
            {
                return addSumm;

            }
        }

        private int GetSumm(FCCSrv2.CashType MyCash)
        {


            int TmmAddSumm = 0;

            if (MyCash != null)
            {



                foreach (FCCSrv2.DenominationType Dt in MyCash.Denomination)
                {


                    if (Dt == null) continue;
                    TmmAddSumm += int.Parse(Dt.fv) * int.Parse(Dt.Piece);

                }

            }
            return TmmAddSumm;


        }
        int AddSummBill = 0;
        int AddSummCoin = 0;
        internal void UpdateReplenishAdd(FCCSrv2.CashType[] Cash)
        {
            if ((Cash == null) || (Cash[0] == null))
            {
                return;
            }


            if (Cash != null)
            {
                FCCSrv2.CashType MyCash = Cash.Where(a => a.type == "4").FirstOrDefault();
                if ((MyCash != null)&&(MyCash.Denomination[0]!=null))
                {
                    if (MyCash.Denomination[0].devid == "1")
                    {
                        AddSummBill = GetSumm(MyCash);
                    }
                    else
                    {
                        AddSummCoin = GetSumm(MyCash);
                    }
                        AddSumm = AddSummBill+AddSummCoin;
                }
            }

        }



        private void EndReplenishmentRes(int ResId, string ResStr,int Summ)
        {
            //  CurrentMesagefrm.Close();
            if (ResId == CFCCApi.FCC_SUCCESS)
            {


                State = State_replenishEnd;
            }
            else
            {
                ShowEror(ResStr);
            }
        }

        private void ShowEror(string ResStr)
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                MsgGrid.Visibility = System.Windows.Visibility.Visible;
                MsgLbl.Text = ResStr;
            }));

        }
        private void ShowProcessMessage(string ResStr)
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                MsgGrid.Visibility = System.Windows.Visibility.Visible;
                MsgLbl.Text = ResStr;
            }));

        }

        private void HideProcessMessage()
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                MsgGrid.Visibility = System.Windows.Visibility.Hidden;
               
            }));
        }

        
        CFCCApi FCCApi;
        private void btnRepl_Click(object sender, RoutedEventArgs e)
        {
            if (State == State_normal)
            {
                AddSumm = 0;
                AddSummCoin = 0;
                AddSummBill = 0;
                btnRepl.IsEnabled = false;
                FCCApi.StartReplenishmentAsync(StartReplenishmentRes);

            }
            else if (State == State_replenishInProcess)
            {
                ShowProcessMessage("Считаю размен...");
                btnRepl.IsEnabled = false;
                FCCApi.EndReplenishment(EndReplenishmentRes);


            }
        }




        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            if (OnHidefrm != null)
            {
                OnHidefrm(this);
            }
        }

       
    }
}
