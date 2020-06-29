using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.Xml;
using System.Xml.Serialization;



namespace FCCIntegration
{
    public static class MainClass2
    {

        private static WPFControls.frmCassir CurentOrderfrm; //= new WPFControls.frmCassir();


        public delegate void HideFrmEventHandler(object sender);
        public static event HideFrmEventHandler OnHideAdminfrm;
        public static event HideFrmEventHandler OnHideRazmenfrm;

        public delegate void ReplenishEventHandler(int Summ);
        public static event ReplenishEventHandler OnReplenish;
        public delegate void OnOutCassetaEventHandler(int Summ, bool Casseta);
        public static event OnOutCassetaEventHandler OnOutCasseta;
        public static event OnOutCassetaEventHandler OnInsertCasseta;

        public delegate void StartChangeEventHandler(FCCCheck Chk,int Change);
        public static event StartChangeEventHandler OnStartChange;


        public static event StartChangeEventHandler OnCancelChange;

        public delegate void UpdateDepositEventHandler(int Summ);
        public static event UpdateDepositEventHandler OnUpdateDeposit;

        public delegate void WaitForRemovalEventHandler(int Change, FCCIntegration.FCCCheck Chk);
        public static event WaitForRemovalEventHandler OnWaitForRemoval;
        public static event WaitForRemovalEventHandler OnFixedDeposit;




        public delegate void WaitForRemovalRejectEventHandler();
        public static event WaitForRemovalRejectEventHandler OnWaitForRemovalReject;


        public delegate void ErrorChangeEventHandler(int Change, FCCCheck Chk, string ErrorMsg);
        public static event ErrorChangeEventHandler OnErrorChange;

        public delegate void EndChangeEventHandler(int Change, FCCCheck Chk);
        public static event EndChangeEventHandler OnEndChange;


        public delegate void SendMoneyLogEventHandler(string Mess);
        public static event SendMoneyLogEventHandler OnSendMoneyLog;

        //GetMoneyLogCallBackEventHandler CurrentUpdateInventoryResponseCallBack;
        public delegate void GetMoneyLogCallBackEventHandler(List<string> Res);
        public delegate void GetMoneyLogEventHandler(DateTime StartDt, DateTime StopDt);
        public static event GetMoneyLogEventHandler OnGetMoneyLog;


        internal static void RaiseOnGetMoneyLog(DateTime StartDt, DateTime StopDt)
        {
            if (OnGetMoneyLog != null)
            {
                OnGetMoneyLog(StartDt, StopDt);
            }
        }

        public static void GetMoneyLog(List<string> res)
        {
            myAdminForm.ShowHistory( res);
            
        }

        internal static void RaiseOnSendMoneyLog(string Mess)
        {
            if (OnSendMoneyLog != null)
            {
                OnSendMoneyLog(Mess);
            }
        }


        internal static void RaiseOnReplenish(int Summ)
        {
            if (OnReplenish != null)
            {
                OnReplenish(Summ);
            }
        }


        internal static void RaiseOnChange()
        {

          

            CurentOrderfrm.Init(CurrentCheck, CurrentCheck.RoundedAmount);
            CurrentTotal = CurrentCheck.RoundedAmount;
            if (OnStartChange != null)
            {
                OnStartChange(CurrentCheck,0);
            }
        }

       

        internal static void RaiseOnEndChange(int HandChange, int Summ,int Change)
        {

            if (ChangeProcess)
            {

                if (!FixedDepositRaised)
                {
                    FixedDepositRaised = true;
                    if (OnFixedDeposit != null)
                    {
                        OnFixedDeposit(Summ, CurrentCheck);
                    }
                }


                if (HandChange != 0)
                {
                    Utils.ToMoneyCountLog(MoneyChangeCommands.EndPayment, Summ, Change, HandChange, CurrentCheck.Ammount, CurrentCheck.AlohNumber);
                    CurentOrderfrm.SetStatus(String.Format("В устройстве не хватило сдачи. Необходимо отдать гостю сдачу в размере {0} руб", (HandChange / 100).ToString("0.00")));
                    if (IsSync)
                    {
                        CurentOrderfrm.RenameCancelBtn("Выход"); 
                    }
                }
                else
                {
                    Utils.ToMoneyCountLog(MoneyChangeCommands.EndPayment, Summ, Change,HandChange, CurrentCheck.Ammount, CurrentCheck.AlohNumber);
                    CurentOrderfrm.SetStatus("Чек оплачен успешно.");
                    CurentOrderfrm.DispHide();

                }
                if (OnEndChange != null)
                {
                    OnEndChange(HandChange, CurrentCheck);
                }
                ChangeProcess = false;
            }
        }

        internal static void RaiseWaitForRemovalChange(int Change)
        {

            if (ChangeProcess)
            {
                CurentOrderfrm.SetStatus(String.Format("Необходимо забрать сдачу"));
                if (OnWaitForRemoval != null)
                {
                    OnWaitForRemoval(Change, CurrentCheck);
                }

            }
        }


        private static bool FixedDepositRaised = true;
        internal static void RaiseFixedDeposit(int Deposit)
        {

            if (ChangeProcess)
            {
                if (!CheckCanceled)
                {
                    //CurentOrderfrm.SetCancelButtonEnabled(false);
                    if (!FixedDepositRaised)
                    {
                        FixedDepositRaised = true;
                        if (OnFixedDeposit != null)
                        {
                            OnFixedDeposit(Deposit, CurrentCheck);
                        }
                    }
                }
                
            }
        }
        internal static void RaiseWaitForRemovalRejectChange()
        {

            if (ChangeProcess)
            {
                CurentOrderfrm.SetStatus(String.Format("Необходимо забрать непринятые купюры"));
                if (OnWaitForRemovalReject != null)
                {
                    OnWaitForRemovalReject();
                }

            }
        }

        public static void StartCashOut()
        {
            FCCSrv2.DenominationType[] Dt = new FCCSrv2.DenominationType[1];
            Dt[0] = new FCCSrv2.DenominationType();
            Dt[0].devid = "1";
            Dt[0].fv = "100000";
            Dt[0].cc = "RUB";
            Dt[0].Piece = "2";
            Dt[0].rev = "0";
            Dt[0].Status = "2";
            CFCCApi FCC = new CFCCApi();
            FCC.CashOut(Dt);
        }

        static public SettingInfo Setting;




       // CFCCApi FCCApiEvents = new CFCCApi();
        //CFCCApi  FCCApi = new CFCCApi();
        public static void InitDevice()
        {
            
            Utils.LogDirCreate();
            Setting = ReadSettings();
            CFCCApi.InitDevice();
            CFCCApi.OnSetStatus += new EventHandleClass.SetStatusEventHandler(FCCApi_OnSetStatus);
            CFCCApi.OnSendError += new EventHandleClass.SendErrorEventHandler(CFCCApi_OnSendError);
            CurentOrderfrm = new WPFControls.frmCassir();
            CurentOrderfrm.OnCancelChange += new WPFControls.frmCassir.ChangeCancelEventHandler(CurentOrderfrm_OnCancelChange);
            myAdminForm = new WPFControls.frmFCCAdminDialog     ();
            myAdminForm.OnHideAdminfrm += new HideFrmEventHandler(myAdminForm_OnHideAdminfrm);

            //WriteSettings(Setting);
          //  CFCCApi.InitDevice();
            DeviceRevisionAsinc();



        }

     
        static void myAdminForm_OnHideAdminfrm(object sender)
        {

           
            if (OnHideAdminfrm != null)
            {
                OnHideAdminfrm(sender);
            }
        }

        public static void SetSync(bool value)
        {
            IsSync = value;
        }

        static internal bool IsSync = true;

        static void CFCCApi_OnSendError(object sender, string Message, string Url)
        {
            CurentOrderfrm.Dispatcher.Invoke((Action)(() =>
                     {
                         WPFControls.frmShowError Errfrm = new WPFControls.frmShowError(Message, Url);
                         Errfrm.ShowDialog();
                     }
          ));
        }
        
        internal static void RaiseOnCancelChange(int Inserted, int Despens, string ErrorMsg)
        {
            ChangeProcess = false;
            if (OnErrorChange != null)
            {
                string Mess = "Ошибка выполнения команды StartChange " + ErrorMsg + Environment.NewLine;
                CFCCApi FCCApi = new CFCCApi();
                string DevStatus="";
                FCCApi.GetStatus(out DevStatus);
                Mess += DevStatus;

                Utils.ToLog("Event OnCancelChange");
                int Change = Math.Max(0, Inserted-Despens);

                OnErrorChange(Change, CurrentCheck, Mess);
                

            }

        }

        internal static void RaiseOnCancelChange(int HandChange, int Summ,int Change)
        //internal static void CFCCApi_OnChangeCancel(int Change)
        {
            ChangeProcess = false;
            

            if (HandChange < 0)
            {
                HandChange = 0;
            }

            CurentOrderfrm.ChangeCanceled(HandChange);
            if (CurrentCheck != null)
            {
                Utils.ToMoneyCountLog(MoneyChangeCommands.CancelPayment, Summ, Change, HandChange, CurrentCheck.Ammount, CurrentCheck.AlohNumber);
                if (OnCancelChange != null)
                {
                    Utils.ToLog("Event OnCancelChange");
                    OnCancelChange(CurrentCheck, HandChange);
                }
            }
            else
            {
                Utils.ToMoneyCountLog(MoneyChangeCommands.CancelPayment, 0);
            }
            CheckCanceled = false;
            
            //CurentOrderfrm.
        }

        static void CurentOrderfrm_OnCancelChange(object owner)
        {
            //CheckNeedCanceled = true;
            NeedCancelChangeMoney();
            //CancelChangeMoney();
        }

        static private int LastCI10StatusChange = 1;
        static private int LastStatus=0;
        static void FCCApi_OnSetStatus(object sender, bool StatusChange, int status, int DevId, string EventName)
        {

            LastStatus = status;
            if (StatusChange)
            {
                LastCI10StatusChange = status;
                if (status == CFCCApi.STATUS_CODE_CHANGE)
                {
                    if (ChangeProcess)
                    {
                        RaiseOnChange();
                    }
                }
                else if (status == CFCCApi.STATUS_CODE_IDLE)
                {
                    ChangeProcess = false;
                    
                }

                else if (status == CFCCApi.STATUS_CODE_DISPENSEREMOVWAIT)
                {
                    if (ChangeProcess)
                    {
                        RaiseWaitForRemovalChange(DevId); //В данном случае DevId кол-во сдачи.
                    }
                }
                else if (status == CFCCApi.STATUS_CODE_DEPOSITREMOVWAIT)
                {
                    if (ChangeProcess)
                    {
                        RaiseWaitForRemovalRejectChange();
                    }
                }
                else if (status == CFCCApi.STATUS_CODE_FIXEDDEPOSITAMOUNT)
                {
                    if (ChangeProcess)
                    {
                        RaiseFixedDeposit(DevId); //В данном случае DevId кол-во принятых денег.
                    }
                }
                else if (status == CFCCApi.STATUS_CODE_DEPOSIT_WAIT)
                {
                    if (ChangeProcess)
                    {
                        if (CheckNeedCanceled)
                        {
                            CheckNeedCanceled = false;
                            CancelChangeMoney();
                        }
                    }
                }


            }

            else
            {
                if (EventName == "eventCassetteInventoryOnRemoval")
                {
                    if (DevId == 1)
                    {
                        Utils.ToMoneyCountLog(MoneyChangeCommands.CasseteRemoved, status);
                    }
                    else
                    {
                        Utils.ToMoneyCountLog(MoneyChangeCommands.CoinMixerRemoved, status);
                    }
                        if (OnOutCasseta != null)
                        {


                            OnOutCasseta(status, (DevId == 1));
                        }
                }


                else if (EventName == "eventCassetteInserted")
                //else if (EventName == "eventCassetteInventoryOnInsertion")
                {

                    if (DevId == 1)
                    {
                        Utils.ToMoneyCountLog(MoneyChangeCommands.CasseteInserted, status);
                    }
                    else
                    {
                        Utils.ToMoneyCountLog(MoneyChangeCommands.CoinMixerInserted, status);
                    }

                    if (OnInsertCasseta != null)
                    {
                        OnInsertCasseta(status, (DevId == 1));
                    }
                }
            }

        }



         public static void WriteIncomeInfoToFccMoneyLog(decimal Summ)
        {
            Utils.ToMoneyCountLog(MoneyChangeCommands.CashIncome, (int)(Summ*100));
        }


        static WPFControls.frmMessageAsync CurrentfrmMessAsync;

        private static int CurrentTotal = 0;
        static internal void UpdateDeposit(int Summ)
        {
            if (ChangeProcess)
            {
                //CurentOrderfrm.GetMoneyDialog().SetDeposit(Summ);
                if (CurrentTotal > Summ)
                {
                    CurentOrderfrm.GetMoneyDialog().SetAlredy(CurrentTotal - Summ);
                }
                else
                {
                    CurentOrderfrm.GetMoneyDialog().SetAlredy(0);
                    CurentOrderfrm.GetMoneyDialog().SetChange(Summ - CurrentTotal);
                }
                CurentOrderfrm.GetMoneyDialog().SetDeposit(Summ);

            }
            if (OnUpdateDeposit != null)
            {
                OnUpdateDeposit(Summ);
            }
        }

        static CFCCApi FCCApi;// = new CFCCApi();
        /*
        private static int  DeviceRevision(out string Status)
        {

            FCCApi = new CFCCApi();
            int res = FCCApi.GetStatusAsync(DeviceRevisionResponse);

            if (res == 0)
            {
                if (CurrentfrmMessAsync != null)
                {
                    CurrentfrmMessAsync.Hide();

                }

                CurrentfrmMessAsync = new WPFControls.frmMessageAsync("Соединение с устройством приема наличных средств.", 1000);
                //CurrentfrmMessAsync.Topmost = true;
                // CurrentfrmMessAsync.Show();
            }
            else
            {
                WPFControls.frmMessage frmMsg = new WPFControls.frmMessage("Ошибка соединения с устройством приема денежных средств.");
                frmMsg.ShowDialog();
            }


        }
        */

        private static void DeviceRevisionAsinc()
        {

            FCCApi = new CFCCApi();
            int res = FCCApi.GetStatusAsync(DeviceRevisionResponse);
            
            if (res == 0)
            {
                if (CurrentfrmMessAsync != null)
                {
                    CurrentfrmMessAsync.Hide();

                }

                CurrentfrmMessAsync = new WPFControls.frmMessageAsync("Соединение с устройством приема наличных средств.", 1000);
                //CurrentfrmMessAsync.Topmost = true;
                // CurrentfrmMessAsync.Show();
            }
            else
            {
                WPFControls.frmMessage frmMsg = new WPFControls.frmMessage("Ошибка соединения с устройством приема денежных средств.");
                frmMsg.ShowDialog();
            }


        }



        private static void DeviceRevisionResponse(int ResId, int StatusId, int RBWState, int RCWState, string StatusStr)
        {
            try
            {

                if (ResId != CFCCApi.FCC_SUCCESS)
                {
                    CurrentfrmMessAsync.StopWait(StatusStr);
                }
                else
                {
                    //CFCCApi FApi = new CFCCApi();

                    //EmptyCassetRevision();

                    LastCI10StatusChange = StatusId;

                    if (StatusId == CFCCApi.STATUS_CODE_IDLE)
                    {
                        CurrentfrmMessAsync.HideFrm();
                    }
                    else if (StatusId == CFCCApi.STATUS_CODE_INIT)
                    {
                        if ((RBWState < 1001) && (RCWState < 1001))
                        {
                     

                        }
                        else
                        {
                            
                            if (RBWState > 1000)
                            {
                                CurrentfrmMessAsync.StopWait("Инициализация. Ошибка устройств: " + CFCCApi.GetDiviceStateByCode(RBWState, 1));
                            }
                            else if (RCWState > 1000)
                            {
                                CurrentfrmMessAsync.StopWait("Инициализация. Ошибка устройств: " + CFCCApi.GetDiviceStateByCode(RBWState, 2));
                            }


                        }


                    }
                    else if (StatusId == CFCCApi.STATUS_CODE_DEPOSIT_WAIT)
                    {
                        ChangeProcess = true;

                        if (RBWState == 2700)
                        {
                            CurrentfrmMessAsync.StopWait("Устройство приема наличных средств находится в состоянии ожидания внесения размена. Перейдите на экран администратора и продолжите операцию");
                        }
                        else
                        {
                            CurrentfrmMessAsync.StopWait("Устройство приема наличных средств находится в состоянии ожидания внесения денежных сумм.");
                        }
                    }
                    else
                    {
                        CurrentfrmMessAsync.StopWait(StatusStr);
                    }
                }
            }
            catch
            { }

        }
        /*
        static internal void EmptyCassetRevision()
        {
            try
            {
                if (Utils.GetOrders().First().Command == MoneyChangeCommands.CasseteInserted)
                {
                    CFCCApi FApi = new CFCCApi();
                    if (FApi.GetKassetaSumm() > 0)
                    {
                        NonEmptyCasset = true;
                    }
                }
            }
            catch
            {

            }
        
        }
        */
        static WPFControls.frmFCCAdminDialog myAdminForm;
        public static void ShowAdminfrm()
        {
            if (myAdminForm == null)
            {
                myAdminForm = new WPFControls.frmFCCAdminDialog();
                myAdminForm.OnHideAdminfrm+=new HideFrmEventHandler(myAdminForm_OnHideAdminfrm);
            }
            myAdminForm.Hide();
            myAdminForm.Show();
        }

        private static bool nonEmptyCasset=false;
        public static bool NonEmptyCasset
        {
            set {
                nonEmptyCasset = value;
            }
            get {

                return nonEmptyCasset;
            }
        }

        public static FCCCheck CurrentCheck;
        private static decimal Total;

        private static bool changeProcess = false;
        public static bool ChangeProcess
        {
            set
            {
                if (CurentOrderfrm != null)
                {
                   // CurentOrderfrm.SetCancelButtonEnabled(value);
                }
                changeProcess = value;

            }
            get
            {
                return changeProcess;
            }
        }

        static WPFControls.frmRazmen Rzm ;
        public static void ShowRazmenWnd()
        {
            try
            {
                Rzm.Hide();
                Rzm.Show();
            }
            catch
            {
                Rzm = new WPFControls.frmRazmen();
                Rzm.OnHidefrm += new HideFrmEventHandler(Rzm_OnHidefrm);
                Rzm.Show();
            }

        }

        static void Rzm_OnHidefrm(object sender)
        {
            if (OnHideRazmenfrm != null)
            {
                OnHideRazmenfrm(sender);
            }
        }


        public static bool SmallChange(out List<int> Minfvs, out List<int>NEfvs)
        {


            return FCCApi.SmallChange(out  Minfvs, out NEfvs);
        }

        public static bool StartChangeMoney(FCCCheck Chk, out string Status)
        {
            CFCCApi FCCApi = new CFCCApi();
            Status = FCCApi.GetStatusStringRus(LastCI10StatusChange);
            if (LastCI10StatusChange != 1)
            {
                return false;
            }

            CurentOrderfrm.RenameCancelBtn("Отменить оплату"); 
            CheckNeedCanceled = false;
            CheckCanceled = false;
            
            //string StatusStr = "";
            //int StatusId = FCCApi.GetStatus(out StatusStr);
            //Status = StatusStr;

            CurentOrderfrm.SetCancelButtonEnabled(true);
            /*
            if ((StatusId != 1) && (StatusId != 18) && (StatusId != 3) && (StatusId != 4))
            {
                return false;
            }
            */
            CurrentCheck = Chk;

        

            Total = Chk.RoundedAmount;
            Utils.ToMoneyCountLog(MoneyChangeCommands.StartPayment, (int)Total, 0, 0,0, CurrentCheck.AlohNumber);
            ChangeProcess = true;
            FixedDepositRaised = false;
            FCCApi.StartChangeAsync((int)(Total));
            
            if (IsSync)
            {
                ShowCassirFrm();
            }
            return true;
        }
        private static bool CheckNeedCanceled = false;
        private static bool CheckCanceled = false;
        public static void CancelChangeMoney()
        {
            CheckCanceled = true;
            //ChangeProcess = false;
            Utils.ToLog("Init CancelChangeMoney");
            CFCCApi FCCApi = new CFCCApi();
            FCCApi.ChangeCancel();
        }


        private static void NeedCancelChangeMoney()
        {
            Utils.ToLog("NeedCancelChangeMoney");
            if (LastStatus == 3)
            {
                CancelChangeMoney();
            }
            else
            {
                CheckNeedCanceled = true;
            }
        }

        public static void ShowCassirFrm()
        {
            //CurentOrderfrm.Topmost = true;

            try
            {
                CurentOrderfrm.Hide();
                CurentOrderfrm.Show();
            }
            catch
            {
                CurentOrderfrm = new WPFControls.frmCassir();
                CurentOrderfrm.OnCancelChange += new WPFControls.frmCassir.ChangeCancelEventHandler(CurentOrderfrm_OnCancelChange);

            }
            
        }

        static string SettingPath = @"C:\aloha\check\fcc\data\Setting.xml";
        static private SettingInfo ReadSettings()
        {
            Utils.ToLog("Читаю настройки");
            XmlReader XR = new XmlTextReader(SettingPath);
            try
            {

                XmlSerializer XS = new XmlSerializer(typeof(SettingInfo));
                //XmlSerializer XS = new XmlSerializer(typeof(CardMooverInfo));
                SettingInfo CMI = (SettingInfo)XS.Deserialize(XR);
                XR.Close();
                Utils.ToLog("Настройки успешно прочитаны");
                return CMI;

            }
            catch (Exception e)
            {
                Utils.ToLog("Ошибка при чтении настроек " + e.Message);
                XR.Close();
                return new SettingInfo();

            }
        }
        static private void WriteSettings(SettingInfo Set)
        {
            Utils.ToLog("Пишу настройки");
            //XmlWriter XR = new XmlTextWriter(SettingPath,);
            try
            {
                XmlWriter XWriter = new XmlTextWriter(SettingPath, System.Text.Encoding.UTF8);
                XmlSerializer XS = new XmlSerializer(typeof(SettingInfo));
                XS.Serialize(XWriter, Set);
                XWriter.Close();
            }
            catch (Exception e)
            {

            }
        }
        static public void Reset()
        { 
            CFCCApi FCCApi = new CFCCApi();
            FCCApi.Reset();
        
        }

        static public void RunWriteMins()
        {
            FCCApi.UpdateInventoryAsync(WriteMins);
        }


        static public void WriteMins(FCCSrv2.CashUnitsType[] Cash, int ResultId, string ResSt)
        {

            SettingInfo Set = Setting;

            List<int> res = new List<int>();
            List<BarabanMin> res2 = new List<BarabanMin>();
             FCCSrv2.CashUnitsType MyCash = Cash.Where(a => a.devid == "1").FirstOrDefault();
             if (MyCash != null)
             {

                 foreach (FCCSrv2.CashUnitType Dt in MyCash.CashUnit.Where(a => a.unitno == "4043" || a.unitno == "4044" || a.unitno == "4045"))
                 {
                     int fv = int.Parse(Dt.Denomination[0].fv);
                     if (!res.Contains(fv))
                     {
                         res.Add(fv);
                         res2.Add(new BarabanMin { fv = fv, MinCount = 0 });
                     }
                 }
             }
            MyCash = Cash.Where(a => a.devid == "2").FirstOrDefault();
            if (MyCash != null)
            {

                foreach (FCCSrv2.CashUnitType Dt in MyCash.CashUnit.Where(a => int.Parse(a.unitno) > 4042 && int.Parse(a.unitno) < 4056))
                {
                    int fv = int.Parse(Dt.Denomination[0].fv);
                    if (!res.Contains(fv))
                    {
                        res.Add(fv);
                        res2.Add(new BarabanMin { fv = fv, MinCount = 0 });
                    }
                }
            }
            Set.BarabanMins = res2;
            WriteSettings(Set);
        }
}
}
