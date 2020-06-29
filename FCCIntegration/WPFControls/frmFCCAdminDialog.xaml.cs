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
using System.Windows.Threading;

namespace FCCIntegration.WPFControls
{
    /// <summary>
    /// Interaction logic for frmFCCAdminDialog.xaml
    /// </summary>
    public partial class frmFCCAdminDialog : Window
    {
        //private Tim


        
        public  event MainClass2.HideFrmEventHandler OnHideAdminfrm;


        public frmFCCAdminDialog()
        {
            //   Topmost = true;
            InitializeComponent();
            State = State_Main;
            CFCCApi.OnReplenishCountChange += new EventHandleClass.ReplenishCountChangeEventHandler(FCCApi_OnReplenishCountChange);
            CFCCApi.OnSetStatus += new EventHandleClass.SetStatusEventHandler(FCCApi_OnSetStatus);

            pnlInkassMenu.btnInkasCancel.Click += new RoutedEventHandler(btnInkasCancel_Click);

            pnlInkassMenu.rbFromBarabans.Checked += new RoutedEventHandler(rbFromBarabans_Checked);
            pnlInkassMenu.rbAll.Checked += new RoutedEventHandler(rbAll_Checked);
            pnlInkassMenu.rbKassetOnly.Checked += new RoutedEventHandler(rbKassetOnly_Checked);

            pnlInkassMenu.btnInkasVigr.Click += new RoutedEventHandler(btnInkasVigr_Click);
            pnlInkassMenu.btnUnlock.Click += new RoutedEventHandler(btnUnlock_Click);


            MainClass2.OnOutCasseta += new MainClass2.OnOutCassetaEventHandler(MainClass2_OnOutCasseta);
            MainClass2.OnInsertCasseta += new MainClass2.OnOutCassetaEventHandler(MainClass2_OnInsertCasseta);


            dT.Interval = new TimeSpan(0, 0, 0, 0, 500);
            dT.Tick += new EventHandler(dT_Tick);
            dT.Start();


            HideProcessMessage();
            //Init();
        }

        string CurrentMess = "";
        DispatcherTimer dT = new DispatcherTimer();
        private int AnimCount = 0;
        void dT_Tick(object sender, EventArgs e)
        {
            try
            {
                if (ProcessMessageShown)
                {
                    if (!dT.IsEnabled) { return; }
                    string Dob = "".PadRight(AnimCount, "."[0]);

                    MsgLbl.Text = CurrentMess + Dob;
                }



            }
            catch
            { }

            AnimCount++;
            if (AnimCount == 4)
            {
                AnimCount = 0;
            }
        }

        void MainClass2_OnInsertCasseta(int Summ, bool Casseta)
        {
            if (Casseta)
            {

                GhangeConnectionState(ConnectonState_CassetIneserted);
            }
            else
            {

                GhangeConnectionState(ConnectonState_CassetIneserted);
            }
        }

        void MainClass2_OnOutCasseta(int Summ, bool Casseta)
        {
            if (Casseta)
            {
                NeedInventoryUpdate = false;
                GhangeConnectionState(ConnectonState_CassetRemoved);
            }
            else
            {
                NeedInventoryUpdate = false;
                GhangeConnectionState(ConnectonState_CassetRemoved);
            }
        }



        internal int AddBillCurentSumm { set; get; }

        internal int AddCointCurentSumm { set; get; }


      


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
                /*
            else if (status == FCCApi.STATUS_CODE_WAITINGINVENTORY)
            {
                NeedInventoryUpdate = true;

            }
                 * */
                else if (status == CFCCApi.STATUS_CODE_DEPOSIT_WAIT)
                {
                    
                    HideProcessMessage();

                }
                else if (status == CFCCApi.STATUS_CODE_IDLE)
                {
                    if (NeedInventoryUpdate)
                    {
                        NeedInventoryUpdate = false;
                        CFCCApi FCCApi = new CFCCApi();
                        FCCApi.UpdateInventoryAsync(UpdateInventory);
                    }
                    if (NeedStatusRequest)
                    {
                        FCCApi = new CFCCApi();
                        int StatusRes = FCCApi.GetStatusAsync(InitDeviceRevisionResponse);
                    }
                    //GhangeSubstate(GlorySubState_Ok, 0);
                }
                else if (status == CFCCApi.STATUS_CODE_UNLOCKING)
                {
                    GhangeSubstate(GlorySubState_Unlocking, 0);
                }


            }

            else
            {
                if (EventName == "eventCassetteInventoryOnRemoval")
                {

                    //FCCApi
                }


                else if (EventName == "eventCassetteInserted")
                //else if (EventName == "eventCassetteInventoryOnInsertion")
                {

                    //FCCApi
                }
            }

        }));
        }


        
        void FCCApi_OnReplenishCountChange(object sender, FCCSrv2.CashType[] Ct)
        {

            if (CurrentConnectonState == ConnectonState_StartRepl)
            {
                UpdateReplenishAdd(Ct);
            }
        }

        CFCCApi FCCApi = new CFCCApi();
        public void Init()
        {

            GhangeConnectionState(ConnectonState_GetStatus);
            int StatusRes = FCCApi.GetStatusAsync(InitDeviceRevisionResponse);

        }


        private void InitDeviceRevisionResponse(int ResId, int StatusId, int RBWState, int RCWState, string StatusStr)
        {
            try
            {
                Utils.ToLog("InitDeviceRevisionResponse");

                if (ResId != CFCCApi.FCC_SUCCESS)
                {
                    GhangeConnectionState(ConnectonState_DevNotConnect);
                }
                else
                {
                    if (StatusId == CFCCApi.STATUS_CODE_IDLE)
                    {
                        Utils.ToLog("InitDeviceRevisionResponse GhangeConnectionState");
                        GhangeSubstate(GlorySubState_Ok, 0);
                        GhangeConnectionState(ConnectonState_Idle, StatusStr, Colors.Black);

                    }



                    else if (StatusId == CFCCApi.STATUS_CODE_INIT)
                    {
                        if ((RBWState < 1001) && (RCWState < 1001))
                        {
                            GhangeSubstate(GlorySubState_Init, 0);
                            
                        }
                        else
                        {
                            GhangeSubstate(GlorySubState_InitError, 0);
                            if (RBWState > 1000)
                            {
                                SetStatus("Инициализация. Ошибка устройств: " + CFCCApi.GetDiviceStateByCode(RBWState, 1));
                            }
                            else if (RCWState > 1000)
                            {
                                SetStatus("Инициализация. Ошибка устройств: " + CFCCApi.GetDiviceStateByCode(RBWState, 2));
                            }


                        }

                        
                    }

                    else if (StatusId == CFCCApi.STATUS_CODE_DEPOSIT_WAIT)
                    {
                        if (RBWState == 2700) //Ожидает окончания внесения размена
                        {
                            GhangeConnectionState(ConnectonState_StartRepl);
                        }
                        else
                        {
                            GhangeConnectionState(ConnectonState_WaitingInsertionNonRepl, "Устройство приема наличных средств находится в состоянии ожидания внесения денежных сумм.", Colors.Red);
                        }
                    }
                    else if (StatusId == CFCCApi.STATUS_CODE_DEPOSITREMOVWAIT)
                    {
                        GhangeSubstate(GlorySubState_WaitingRemovalReject, 0);
                    }
                    else if (StatusId == CFCCApi.STATUS_CODE_DISPENSEREMOVWAIT)
                    {
                        GhangeSubstate(GlorySubState_WaitingRemovalCashout, StatusId);
                    }
                    else if (StatusId == CFCCApi.STATUS_CODE_UNLOCKING)
                    {
                        cassetaLockState = false;
                        pnlInkassMenu.SetBtnUnlockTxt("Заблокировать кассету");

                        AllDoButtonsDisable();
                        pnlInkassMenu.SetBtnUnlockEnable(true);

                        GhangeSubstate(GlorySubState_Unlocking, 0);
                    }

                    else
                    {
                        GhangeSubstate(GlorySubState_NonDescript, StatusId);
                    }
                    if ((StatusId != CFCCApi.STATUS_CODE_WAITINGINVENTORY) && (StatusId != CFCCApi.STATUS_CODE_INIT))
                    {
                        CFCCApi FCCApi = new CFCCApi();

                        FCCApi.UpdateInventoryAsync(UpdateInventory);
                    }

                }


            }
            catch (Exception e)
            {
                Utils.ToLog("InitDeviceRevisionResponse Error " + e.Message);
            }

        }


        internal void SetStatus(string StatusStr)
        {
            SetStatus(StatusStr, Colors.Black);
        }

        internal void SetStatus(string StatusStr, Color color)
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                lblStatus.Content = StatusStr;
                lblStatus.Foreground = new SolidColorBrush(color);
            }));


        }


        internal void ClearReplenishAdd()
        {

            AddBillCurentSumm = 0;
            AddCointCurentSumm = 0;
            this.Dispatcher.Invoke((Action)(() =>
           {
               Barabans.ClearBillAdd();
               Barabans.ClearCoinAdd();
               ReplTotal.SetMoney(0);
           }));

        }

        internal void UpdateReplenishAdd(FCCSrv2.CashType[] Cash)
        {
            if ((Cash == null) || (Cash[0] == null))
            {
                return;
            }

            this.Dispatcher.Invoke((Action)(() =>
           {
               if (Cash != null)
               {
                   int BillSumm = 0;
                   int CoinSumm = 0;

                   int BillBarabansSumm = 0;
                   int CoinBarabansSumm = 0;
                   //Барабаны
                   FCCSrv2.CashType MyCash = Cash.Where(a => a.type == "4").FirstOrDefault();
                   if (MyCash != null)
                   {
                       int CassetaAdd = 0;
                       int MonetaAdd = 0;

                       //MyCash
                       foreach (FCCSrv2.DenominationType Dt in MyCash.Denomination)
                       {
                           if (Dt == null) continue;
                           if (Dt.devid == "1")
                           {
                               
                               //if (Dt.)
                               Barabans.SetBillAdd(int.Parse(Dt.fv), int.Parse(Dt.Piece), int.Parse(Dt.Status));
                               BillBarabansSumm += int.Parse(Dt.fv) * int.Parse(Dt.Piece);

                           }
                           else if (Dt.devid == "2")
                           {
                               Barabans.SetCoinAdd(int.Parse(Dt.fv), int.Parse(Dt.Piece), int.Parse(Dt.Status));
                               CoinBarabansSumm += int.Parse(Dt.fv) * int.Parse(Dt.Piece);

                           }
                       }


                       if (BillBarabansSumm == 0)
                       {
                           BillBarabansSumm = AddBillCurentSumm;
                       }
                       else
                       {
                           Barabans.SetBillAdd(0, BillBarabansSumm, 0);
                           AddBillCurentSumm = BillBarabansSumm;
                       }

                       if (CoinBarabansSumm == 0)
                       {
                           CoinBarabansSumm = AddCointCurentSumm;
                       }
                       else
                       {
                           AddCointCurentSumm = CoinBarabansSumm;
                       }

                   }
                   ReplTotal.SetMoney(BillBarabansSumm + CoinBarabansSumm);
               }
           }));
        }



        private void UpdateInventory(FCCSrv2.CashUnitsType[] Cash, int ResultId, string ResSt)
        {
            // if (ResultId != FCCApi.FCC_SUCCESS)
            //{

            if (Cash != null)
            {
                int BillSumm = 0;
                int BillCassetSumm = 0;
                int BillCassetCountSumm = 0;
                int CoinSumm = 0;

                int CoinCassetSumm = 0;
                int CoinCassetCountSumm = 0;
                //Банкноты
                FCCSrv2.CashUnitsType MyCash = Cash.Where(a => a.devid == "1").FirstOrDefault();
                if (MyCash != null)
                {

                    foreach (FCCSrv2.CashUnitType Dt in MyCash.CashUnit.Where(a => a.unitno == "4043" || a.unitno == "4044" || a.unitno == "4045"))
                    {
                        FCCSrv2.DenominationType Den = Dt.Denomination[0];
                        Barabans.SetBillValue(int.Parse(Den.fv), int.Parse(Den.Piece), int.Parse(Den.Status), int.Parse(Dt.max), int.Parse(Dt.unitno), Den);
                        BillSumm += int.Parse(Den.fv) * int.Parse(Den.Piece);
                    }
                    FCCSrv2.CashUnitType DtCas = MyCash.CashUnit.Where(a => a.unitno == "4059").FirstOrDefault();

                    foreach (FCCSrv2.DenominationType Den in DtCas.Denomination)
                    {
                        BillCassetSumm += int.Parse(Den.fv) * int.Parse(Den.Piece);
                        BillCassetCountSumm += int.Parse(Den.Piece);

                    }
                    Barabans.SetBillValue(BillCassetCountSumm, BillCassetSumm, 0, int.Parse(DtCas.max), int.Parse(DtCas.unitno), null);
                    BillSumm += BillCassetSumm;
                }

                CurrentCassetaSumm = BillCassetSumm;
                FCCApi.CurrentCassetaSumm = BillCassetSumm;
                //Монеты
                MyCash = Cash.Where(a => a.devid == "2").FirstOrDefault();
                if (MyCash != null)
                {

                    foreach (FCCSrv2.CashUnitType Dt in MyCash.CashUnit.Where(a => int.Parse(a.unitno) > 4042 && int.Parse(a.unitno) < 4056))
                    {
                        FCCSrv2.DenominationType Den = Dt.Denomination[0];
                        Barabans.SetCoinValue(int.Parse(Den.fv), int.Parse(Den.Piece), int.Parse(Den.Status), int.Parse(Dt.max), int.Parse(Dt.unitno), Den);
                        CoinSumm += int.Parse(Den.fv) * int.Parse(Den.Piece);
                    }
                    FCCSrv2.CashUnitType DtCas = MyCash.CashUnit.Where(a => a.unitno == "4084").FirstOrDefault();

                    foreach (FCCSrv2.DenominationType Den in DtCas.Denomination)
                    {
                        CoinCassetSumm += int.Parse(Den.fv) * int.Parse(Den.Piece);
                        CoinCassetCountSumm += int.Parse(Den.Piece);
                    }
                    Barabans.SetCoinValue(CoinCassetCountSumm, CoinCassetSumm, 0, int.Parse(DtCas.max), int.Parse(DtCas.unitno), null);
                    CoinSumm += CoinCassetSumm;

                }
                CurrentCoinCassetaSumm = CoinCassetSumm;
                FCCApi.CurrentCoinCassetaSumm = CoinCassetSumm;
                SetSumm(BillSumm + CoinSumm);
            }
            else
            {
                GhangeConnectionState(ConnectonState_UpdateInventoryError, String.Format("Ошибка запроса состояния. Код ошибки  {0}. Сообщение {1} ", ResultId, ResSt), Colors.Red);

            }

            AllDoButtonsEnable();
            HideProcessMessage();


            /*
            if (CurrentConnectonState == ConnectonState_CassetIneserted)
            {
                if (CurrentCassetaSumm > 0)
                {
                    MainClass2.NonEmptyCasset = true;
                }
            }
            */
        }
        private int CurrentCassetaSumm = 0;
        private int CurrentCoinCassetaSumm = 0;

        public void SetSumm(int summ)
        {
            this.Dispatcher.Invoke((Action)(() =>
                      {
                          MonTotal.SetMoney(summ);
                      }));
        }


        private int total = 0;

        public int Total
        {
            set
            {
                total = value;
                MonTotal.SetMoney(value);
            }
            get
            {
                return total;
            }
        }


        private const int State_Main = 0;
        private const int State_Repl = 1;


        private int state = 0;
        public int State
        {
            get
            {
                return state;
            }
            set
            {
                state = value;
                switch (state)
                {
                    case State_Main:
                        pnlBtnsRepl.Visibility = System.Windows.Visibility.Hidden;
                        pnlBtnsMain.Visibility = System.Windows.Visibility.Visible;
                        break;
                    case State_Repl:
                        pnlBtnsRepl.Visibility = System.Windows.Visibility.Visible;
                        pnlBtnsMain.Visibility = System.Windows.Visibility.Hidden;

                        break;

                    default:
                        break;
                }

            }
        }

        private const int ConnectonState_Idle = 0;
        private const int ConnectonState_GetStatus = 1;
        private const int ConnectonState_DevNotConnect = 2;
        private const int ConnectonState_UpdateInventoryError = 3;
        private const int ConnectonState_StartReplError = 4;
        private const int ConnectonState_WaitingInsertionNonRepl = 5;
        private const int ConnectonState_StartRepl = 6;
        private const int ConnectonState_EndReplError = 7;
        private const int ConnectonState_CancelReplError = 8;
        private const int ConnectonState_CancelReplSuccess = 9;
        private const int ConnectonState_EndReplSuccess = 10;

        private const int ConnectonState_StartInkass = 11;
        private const int ConnectonState_CancelInkass = 12;
        private const int ConnectonState_StartInkassFromBarabans = 13;
        private const int ConnectonState_EndInkassFromBarabans = 14;
        private const int ConnectonState_EndInkassFromBarabansError = 15;
        //private const int ConnectonState_EndInkassFromBarabansError = 15;

        private const int ConnectonState_CassetRemoved = 17;
        private const int ConnectonState_CassetIneserted = 18;



        private void AllDoButtonsDisable()
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                btnRepl.IsEnabled = false;
                btnInkas.IsEnabled = false;
               // btnReset.IsEnabled = false;
                btnReplEnd.IsEnabled = false;
                btnReplCancel.IsEnabled = false;
                pnlInkassMenu.AllBtnsEnableState(false);
            }));
        }
        private void AllDoButtonsEnable()
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                btnRepl.IsEnabled = true;
                btnInkas.IsEnabled = true;
              //  btnReset.IsEnabled = false;
                btnReset.IsEnabled = true;
                btnReplEnd.IsEnabled = true;
                btnReplCancel.IsEnabled = true;
                pnlInkassMenu.AllBtnsEnableState(true);
            }));
        }

        private bool cassetaLockState = true;

        internal bool CassetaLockState
        {
            set
            {
                CFCCApi FCCApi = new CFCCApi();
                cassetaLockState = value;
                if (value)
                {
                    FCCApi.Lock();

                    pnlInkassMenu.SetBtnUnlockTxt("Разблокировать кассету");
                    AllDoButtonsEnable();
                    SetStatus("Кассета заблокирована");



                }
                else
                {
                    FCCApi.Unlock(pnlInkassMenu.ChBBill.IsChecked.Value, pnlInkassMenu.ChBCoins.IsChecked.Value);
                    pnlInkassMenu.SetBtnUnlockTxt("Заблокировать кассету");
                    AllDoButtonsDisable();
                    pnlInkassMenu.SetBtnUnlockEnable(true);
                }
            }
            get
            {
                return cassetaLockState;
            }
        }



        private const int GlorySubState_Ok = 0;
        private const int GlorySubState_WaitingRemovalReject = 1;
        private const int GlorySubState_WaitingRemovalCashout = 2;
        private const int GlorySubState_Unlocking = 3;
        private const int GlorySubState_InitError = 4;
        private const int GlorySubState_Init = 5;
        private const int GlorySubState_NonDescript = 99;

        private int CurrentGlorySubstate = 0;


        private bool NeedStatusRequest = false;

        private void GhangeSubstate(int Substate, int DescrId)
        {
            NeedStatusRequest = (Substate != GlorySubState_Ok);


            switch (Substate)
            {
                case GlorySubState_Ok:
                    if (CurrentGlorySubstate != Substate)
                    {
                        AllDoButtonsEnable();
                        cassetaLockState = true;
                        pnlInkassMenu.SetBtnUnlockTxt("Разблокировать кассету");
                        GhangeConnectionState(CurrentConnectonState);
                    }
                    break;

                case GlorySubState_InitError:
                    AllDoButtonsDisable();
                    this.Dispatcher.Invoke((Action)(() =>
                    {
                        btnReset.IsEnabled = true;
                    }));
                    break;
                case GlorySubState_Init:
                    AllDoButtonsDisable();
                    SetStatus("Инициализация. Обновите состояние");
                    break;

                case GlorySubState_WaitingRemovalReject:

                    SetStatus("Удалите банкноты из лотка для сдачи", Colors.Red);
                    break;
                case GlorySubState_WaitingRemovalCashout:
                    AllDoButtonsDisable();
                    SetStatus("Удалите непринятые банкноты из лотка для сдачи", Colors.Red);
                    break;
                case GlorySubState_Unlocking:
                    AllDoButtonsDisable();
                    pnlInkassMenu.SetBtnUnlockEnable(true);
                    MenuState = 2;
                    SetStatus("Дверца разблокирована", Colors.Red);
                    break;
                case GlorySubState_NonDescript:
                    //AllDoButtonsDisable();
                    SetStatus(String.Format("Устройство находится в состоянии {0} : {1},", DescrId, FCCApi.GetStatusStringRus(DescrId)), Colors.Red);
                    break;
                default:
                    break;
            }
            CurrentGlorySubstate = Substate;
        }




        private int connectionState = -1;

        private int CurrentConnectonState = 0;

        private void GhangeConnectionState(int value)
        {

            GhangeConnectionState(value, "", Colors.Black);

        }


        private int menuState = 0;
        public int MenuState
        {
            set
            {
                menuState = value;

                switch (value)
                {
                    case 0:
                        pnlInkassMenu.Visibility = System.Windows.Visibility.Hidden;
                        pnlBtnsRepl.Visibility = System.Windows.Visibility.Hidden;
                        pnlBtnsMain.Visibility = System.Windows.Visibility.Visible;
                        break;
                    case 1:
                        pnlInkassMenu.Visibility = System.Windows.Visibility.Hidden;
                        pnlBtnsRepl.Visibility = System.Windows.Visibility.Visible;
                        pnlBtnsMain.Visibility = System.Windows.Visibility.Hidden;
                        break;
                    case 2:
                        pnlInkassMenu.Visibility = System.Windows.Visibility.Visible;
                        pnlBtnsRepl.Visibility = System.Windows.Visibility.Hidden;
                        pnlBtnsMain.Visibility = System.Windows.Visibility.Hidden;
                        break;
                    default:
                        break;
                }
            }

            get
            {

                return menuState;
            }
        }








        private void GhangeConnectionState(int value, string Status, Color color)
        {
            CurrentConnectonState = value;
            this.Dispatcher.Invoke((Action)(() =>
          {



              /*
              if ((value != ConnectonState_StartInkass) && (value != ConnectonState_StartInkassFromBarabans) && (value != ConnectonState_StartInkassFromBarabans))
              {
                  pnlBtnsMain.Visibility = System.Windows.Visibility.Visible;
                  pnlInkassMenu.Visibility = System.Windows.Visibility.Hidden;
              }
              */

              connectionState = value;
              switch (connectionState)
              {
                  case ConnectonState_Idle:
                      AllDoButtonsEnable();
                      SetStatus(Status, color);
                      break;
                  case ConnectonState_GetStatus:
                      AllDoButtonsDisable();
                      SetStatus("Получение данных от устройства.");
                      break;
                  case ConnectonState_DevNotConnect:
                      AllDoButtonsDisable();
                      SetStatus("Нет связи с устройством приема наличных средств." + Environment.NewLine +
                          "Убедитесь, что устройство подключено и нажмите кнопку Обновить состояние.", Colors.Red);
                      Barabans.SetNotFindStatus();
                      break;
                  case ConnectonState_UpdateInventoryError:
                      AllDoButtonsDisable();
                      SetStatus(Status, color);
                      break;
                  case ConnectonState_StartReplError:
                      //AllDoButtonsDisable();
                      //SetStatus(Status, color);
                      SetStatus(Status, color);
                      needInventoryUpdate = true;
                      ClearReplenishAdd();
                      MenuState = 0;
                      break;
                  case ConnectonState_WaitingInsertionNonRepl:
                      state = 0;
                      btnRepl.IsEnabled = false;
                      btnInkas.IsEnabled = false;
                      btnReset.IsEnabled = true;
                      SetStatus(Status, color);
                      break;
                  case ConnectonState_StartRepl:
                      /*
                      if (MainClass2.NonEmptyCasset)
                      {
                          SetStatus("Вы вставили не пустую кассету. Поменяйте кассету.", Colors.Red);

                      }
                      else
                       * */
                      {
                          pnlBtnsRepl.Visibility = System.Windows.Visibility.Visible;
                          pnlBtnsMain.Visibility = System.Windows.Visibility.Hidden;
                          btnReplEnd.IsEnabled = true;
                          btnReplCancel.IsEnabled = true;
                          SetStatus("Внесите размен", Colors.Black);
                      }
                      break;
                  case ConnectonState_EndReplError:
                      AllDoButtonsDisable();
                      SetStatus(Status, color);
                      MenuState = 0;
                      break;
                  case ConnectonState_CancelReplError:
                      AllDoButtonsDisable();
                      SetStatus(Status, color);
                      MenuState = 0;
                      break;
                  case ConnectonState_CancelReplSuccess:

                      NeedInventoryUpdate = true;
                      AllDoButtonsEnable();
                      SetStatus("Внесение размена отменено", Colors.Black);
                      ClearReplenishAdd();
                      MenuState = 0;
                      break;
                  case ConnectonState_EndReplSuccess:

                      NeedInventoryUpdate = true;
                      AllDoButtonsEnable();
                      SetStatus("Внесение завершено успешно", Colors.Black);
                      ClearReplenishAdd();
                      MenuState = 0;
                      break;

                  case ConnectonState_StartInkass:
                      MenuState = 2;
                      SetStatus("Выберите режим инкасации", Colors.Black);
                      break;
                  case ConnectonState_CancelInkass:
                      ClearReplenishAdd();
                      Barabans.BtnsCountVisible = false;
                      MenuState = 0;
                      break;

                  case ConnectonState_StartInkassFromBarabans:
                      if (MainClass2.NonEmptyCasset)
                      {
                          SetStatus("Вы вставили не пустую кассету. Поменяйте кассету.", Colors.Red);

                      }
                      else
                      {

                          SetStatus("Внесение денег из барабанов в кассету", Colors.Black);
                          pnlInkassMenu.IsEnabled = false;
                          Barabans.BtnsCountVisible = false;
                      }
                      break;

                  case ConnectonState_EndInkassFromBarabans:

                      SetStatus("Внесение денег из барабанов в кассету окончено", Colors.Black);

                      ClearReplenishAdd();
                      pnlInkassMenu.IsEnabled = true;

                      NeedInventoryUpdate = true;
                      break;

                  case ConnectonState_EndInkassFromBarabansError:
                      SetStatus("Внесение денег из барабанов в кассету окончено с ошибкой", Colors.Black);
                      pnlInkassMenu.IsEnabled = true;
                      NeedInventoryUpdate = true;
                      ClearReplenishAdd();
                      break;
                  case ConnectonState_CassetRemoved:
                      SetStatus("Кассета удалена", Colors.Red);
                      pnlInkassMenu.AllBtnsEnableState(false);

                      break;
                  case ConnectonState_CassetIneserted:
                      SetStatus("Кассета вставлена", Colors.Red);
                      pnlInkassMenu.AllBtnsEnableState(true);

                      NeedInventoryUpdate = true;
                      break;
                  default:
                      break;


              }
          }));
        }



        private void StartInkassFromBaraban()
        {


        }

        private bool needInventoryUpdate = false;
        internal bool NeedInventoryUpdate
        {
            set
            {
                needInventoryUpdate = value;
                if (value)
                {
                    AllDoButtonsDisable();
                    CurrentMess = "Пересчитываю денежные средства";
                }
                else
                {
                    //AllDoButtonsEnable();
                    //HideProcessMessage();
                }
            }
            get
            {
                return needInventoryUpdate;
            }
        }

        private void StartRepl()
        {
            MenuState = 1;
            CFCCApi FCCApi = new CFCCApi();
            FCCApi.StartReplenishmentAsync(StartReplenishmentRes);
        }

        private void StartReplenishmentRes(int ResId, string ResStr)
        {
            if (ResId == CFCCApi.FCC_SUCCESS)
            {
                //Replenish=true
                GhangeConnectionState(ConnectonState_StartRepl);
            }
            else
            {
                GhangeConnectionState(ConnectonState_StartReplError, ResStr, Colors.Black);

            }
        }

        private void EndRepl()
        {
            AllDoButtonsDisable();
            ShowProcessMessage("Завершаю внесение размена.");
            CFCCApi FCCApi = new CFCCApi();
            FCCApi.EndReplenishment(EndReplenishmentRes);
        }
        private void EndReplenishmentRes(int ResId, string ResStr,int Summ)
        {
            //  CurrentMesagefrm.Close();
            if (ResId == CFCCApi.FCC_SUCCESS)
            {
                MainClass2.RaiseOnReplenish(Summ);
                Utils.ToMoneyCountLog(MoneyChangeCommands.ReplenishEnd, Summ);
                GhangeConnectionState(ConnectonState_EndReplSuccess);
            }
            else
            {
                GhangeConnectionState(ConnectonState_EndReplError, ResStr, Colors.Black);
            }
        }

        //frmMessageAsync CurrentMesagefrm;


        bool ProcessMessageShown = false;

        private void ShowProcessMessage(string Message)
        {
            this.Dispatcher.Invoke((Action)(() =>
                      {
                          CurrentMess = Message;
                          MsgLbl.Text = CurrentMess;
                          MsgGrid.Visibility = System.Windows.Visibility.Visible;
                          ProcessMessageShown = true;
                      }));
        }

        private void HideProcessMessage()
        {
            this.Dispatcher.Invoke((Action)(() =>
                      {
                          MsgGrid.Visibility = System.Windows.Visibility.Hidden;
                          ProcessMessageShown = false;
                      }));
        }

        private void CancelRepl()
        {
            AllDoButtonsDisable();
            ShowProcessMessage("Отменяю внесение размена. Дождитесь выдачи денежных средств и заберите их из лотка.");
            CFCCApi FCCApi = new CFCCApi();
            FCCApi.CancelReplenishment(CancelReplenishmentRes);
        }
        private void CancelReplenishmentRes(int ResId, string ResStr)
        {
            //CurrentMesagefrm.HideFrm();
            if (ResId == CFCCApi.FCC_SUCCESS)
            {
                GhangeConnectionState(ConnectonState_CancelReplSuccess);

            }
            else
            {
                GhangeConnectionState(ConnectonState_CancelReplError, ResStr, Colors.Black);

            }
        }

        private void StartInkass()
        {
            GhangeConnectionState(ConnectonState_StartInkass);
        }
        private void CancelInkass()
        {
            GhangeConnectionState(ConnectonState_CancelInkass);
        }


        private void InkassVigr()
        {


            ShowProcessMessage("Произвожу выгрузку из барабанов в кассету ");
            GhangeConnectionState(ConnectonState_StartInkassFromBarabans);

            FCCSrv2.DenominationType[] Tmp = Barabans.GetBarabansAdd(pnlInkassMenu.rbAll.IsChecked.Value, pnlInkassMenu.ChBBill.IsChecked.Value, pnlInkassMenu.ChBCoins.IsChecked.Value);
            FCCApi = new CFCCApi();
            FCCApi.CollectAsync(Tmp, InkassVigrRes);

        }

        /*
        private bool NeedUnlock = false;
        private void BeforeInkassUnlock()
        {



            FCCApi = new CFCCApi();
            NeedUnlock = true;
            FCCApi.UpdateInventoryAsync(UpdateInventory);

            //FCCApi.Unlock(pnlInkassMenu.ChBBill.IsChecked.Value, pnlInkassMenu.ChBCoins.IsChecked.Value);

        }
         * */
        private void InkassUnlock()
        {

            CassetaLockState = !CassetaLockState;


        }

        private void InkassVigrRes(int ResId, string ResStr)
        {
            //  CurrentMesagefrm.Close();
            if (ResId == CFCCApi.FCC_SUCCESS)
            {
                GhangeConnectionState(ConnectonState_EndInkassFromBarabans);
            }
            else
            {
                GhangeConnectionState(ConnectonState_EndInkassFromBarabansError, ResStr, Colors.Black);
            }
        }

        private void btnRepl_Click(object sender, RoutedEventArgs e)
        {
            StartRepl();

        }

        private void btnReplEnd_Click(object sender, RoutedEventArgs e)
        {
            EndRepl();
        }

        private void btnReplCancel_Click(object sender, RoutedEventArgs e)
        {
            CancelRepl();
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            Init();
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            MainClass2.Reset();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            if (connectionState == ConnectonState_StartRepl)
            {
                EndRepl();
            }
            this.Hide();
            if (OnHideAdminfrm != null)
            {
                OnHideAdminfrm(this);
            }
        }

        private void btnInkas_Click(object sender, RoutedEventArgs e)
        {

            StartInkass();
        }
        void btnInkasCancel_Click(object sender, RoutedEventArgs e)
        {
            CancelInkass();
        }

        void rbKassetOnly_Checked(object sender, RoutedEventArgs e)
        {
            Barabans.BtnsCountVisible = false;
        }

        void rbAll_Checked(object sender, RoutedEventArgs e)
        {
            Barabans.BtnsCountVisible = false;
        }

        void rbFromBarabans_Checked(object sender, RoutedEventArgs e)
        {
            Barabans.BtnsCountVisible = true;
        }
        void btnInkasVigr_Click(object sender, RoutedEventArgs e)
        {
            InkassVigr();
        }

        void btnUnlock_Click(object sender, RoutedEventArgs e)
        {
            InkassUnlock();
        }
        
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Init();     
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            //Init(); 
        }

        private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
  
            {
                Init();
            }
        }


        internal void ShowHistory(List<string> res)
        {
            ctrlShowOrders1.InitData(Utils.GetOrders(res));
            ctrlShowOrders1.Visibility = System.Windows.Visibility.Visible;
        
        }

        private void btnReport_Click(object sender, RoutedEventArgs e)
        {
            //ctrlShowOrders1.InitData(Utils.GetOrders());
            //ctrlShowOrders1.Visibility = System.Windows.Visibility.Visible;
            MainClass2.RaiseOnGetMoneyLog(DateTime.Now.AddDays(-3), DateTime.Now.AddDays(1));

        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            ShowCassetaStruct();
        }

        private void ShowCassetaStruct()
        {
          FCCSrv2.CashUnitsType[]  Cash = FCCApi.UpdateInventory();
          FCCSrv2.CashUnitsType MyCash = Cash.Where(a => a.devid == "1").FirstOrDefault();
          if (MyCash != null)
          {

            
              FCCSrv2.CashUnitType DtCas = MyCash.CashUnit.Where(a => a.unitno == "4059").FirstOrDefault();
              string res = "";
              foreach (FCCSrv2.DenominationType Den in DtCas.Denomination)
              {
                  res += (int.Parse(Den.fv) / 100).ToString() + "р. х " + int.Parse(Den.Piece) + "шт"+Environment.NewLine;
                
              }
              ShowProcessMessage(res);
                  
          }
          
        }
    }
}
