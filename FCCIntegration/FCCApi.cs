using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FCCIntegration
{
    public class CFCCApi
    {

        // STATUS_REQ_TYPE
        public const int FCC_REQ_GETST_WITHOUT_CASH = 0;
        public const int FCC_REQ_GETST_WITH_CASH = 1;


        // FCC_STATUS_CODE
        public const int FCC_SUCCESS = 0;
        public const int FCC_CANCEL = 1;
        public const int FCC_SHORTAGE_CANCEL = 9;
        public const int FCC_SHORTAGE = 10;
        public const int FCC_EXCLUSIVE_ERROR = 11;
        public const int FCC_INVENTORY_ERROR = 12;
        public const int FCC_VERIFICATION_ERROR = 32;
        public const int FCC_ILLEGAL_DENOMI_ERROR = 33;
        public const int FCC_SHORTAGE_STK_ERROR = 34;
        public const int FCC_INTERNAL_ERROR = 99;
        public const int FCC_MACHINE_ERROR = 100;


        // STATUS_CODE  
        public const int STATUS_CODE_INIT = 0;
        public const int STATUS_CODE_IDLE = 1;
        public const int STATUS_CODE_CHANGE = 2;
        public const int STATUS_CODE_DEPOSIT_WAIT = 3;
        public const int STATUS_CODE_DEPOSIT_COUNTING = 4;
        public const int STATUS_CODE_DISPENSE = 5;
        public const int STATUS_CODE_DEPOSITREMOVWAIT = 6;
        public const int STATUS_CODE_DISPENSEREMOVWAIT = 7;
        public const int STATUS_CODE_RESET = 8;
        public const int STATUS_CODE_DEPOSITCANCEL = 9;
        public const int STATUS_CODE_CALCAMOUNT = 10;
        public const int STATUS_CODE_CASHINCANCEL = 11;
        public const int STATUS_CODE_COLLECT = 12;
        public const int STATUS_CODE_ERROR = 13;
        public const int STATUS_CODE_UPFARMA = 14;
        public const int STATUS_CODE_READLOG = 15;
        public const int STATUS_CODE_WAITREPLENISH = 16;
        public const int STATUS_CODE_CALCREPLENISH = 17;
        public const int STATUS_CODE_UNLOCKING = 18;
        public const int STATUS_CODE_WAITINGINVENTORY = 19;
        public const int STATUS_CODE_FIXEDDEPOSITAMOUNT = 20;

        FCCSrv2.BrueBoxService clsBrueBoxService;
        static EventHandleClass mEventHandleClass;

        public static void InitDevice()
        {
            mEventHandleClass = new EventHandleClass();
            mEventHandleClass.OnSetDeposit += new EventHandleClass.ChangeMoneyEventHandler(mEventHandleClass_OnSetDeposit);
            mEventHandleClass.OnSetStatus += new EventHandleClass.SetStatusEventHandler(mEventHandleClass_OnSetStatus);
            mEventHandleClass.OnReplenishCountChange += new EventHandleClass.ReplenishCountChangeEventHandler(mEventHandleClass_OnReplenishCountChange);
            mEventHandleClass.OnSendError += new EventHandleClass.SendErrorEventHandler(mEventHandleClass_OnSendError);


        }

        static void mEventHandleClass_OnSendError(object sender, string Message, string Url)
        {
            if (OnSendError != null)
            {
                OnSendError(sender, Message, Url);
            }
        }

        public CFCCApi()
        {

            try
            {
                string Url = MainClass2.Setting.FCCIp;
                //Utils.ToLog("Подключаюсь к серверу FCC по адресу " + Url);
                clsBrueBoxService = new FCCSrv2.BrueBoxService(Url);
                clsBrueBoxService.GetStatusCompleted += new FCCSrv2.GetStatusCompletedEventHandler(clsBrueBoxService_GetStatusCompleted);
                clsBrueBoxService.StartReplenishmentFromEntranceOperationCompleted += new FCCSrv2.StartReplenishmentFromEntranceOperationCompletedEventHandler(clsBrueBoxService_StartReplenishmentFromEntranceOperationCompleted);
                clsBrueBoxService.EndReplenishmentFromEntranceOperationCompleted += new FCCSrv2.EndReplenishmentFromEntranceOperationCompletedEventHandler(clsBrueBoxService_EndReplenishmentFromEntranceOperationCompleted);
                clsBrueBoxService.ReplenishmentFromEntranceCancelOperationCompleted += new FCCSrv2.ReplenishmentFromEntranceCancelOperationCompletedEventHandler(clsBrueBoxService_ReplenishmentFromEntranceCancelOperationCompleted);
                clsBrueBoxService.InventoryOperationCompleted += new FCCSrv2.InventoryOperationCompletedEventHandler(clsBrueBoxService_InventoryOperationCompleted);
                clsBrueBoxService.CollectOperationCompleted += new FCCSrv2.CollectOperationCompletedEventHandler(clsBrueBoxService_CollectOperationCompleted);
                clsBrueBoxService.ChangeOperationCompleted += new FCCSrv2.ChangeOperationCompletedEventHandler(clsBrueBoxService_ChangeOperationCompleted);
                clsBrueBoxService.ChangeCancelOperationCompleted += new FCCSrv2.ChangeCancelOperationCompletedEventHandler(clsBrueBoxService_ChangeCancelOperationCompleted);
                clsBrueBoxService.CashoutOperationCompleted += new FCCSrv2.CashoutOperationCompletedEventHandler(clsBrueBoxService_CashoutOperationCompleted);

            

            //    Utils.ToLog("Успешно подключился к серверу FCC");
            }
            catch (Exception e)
            {
                string ErrMess = "Ошибка при подключении к серверу FCC " + e.Message;
                Utils.ToLog(ErrMess);
            }
        }

        void clsBrueBoxService_CashoutOperationCompleted(object sender, FCCSrv2.CashoutOperationCompletedEventArgs e)
        {

            if (OnCashOutComplited != null)
            {
                OnCashOutComplited(e);
            }

            
        }

        public  void Reset()
        {
            FCCSrv2.ResetRequestType objReset = new FCCSrv2.ResetRequestType()

            {
                Id = GetId(),
                SeqNo = GetSequenceNumber()
            };
            FCCSrv2.ResetResponseType objResetResp = new FCCSrv2.ResetResponseType();
            objResetResp = clsBrueBoxService.ResetOperation(objReset);

        }

        internal int GetKassetaSumm()
        {
            // if (ResultId != FCCApi.FCC_SUCCESS)
            //{

            FCCSrv2.CashUnitsType[] Cash = UpdateInventory();
            if (Cash != null)
            {

                int BillCassetSumm = 0;
                //Банкноты
                FCCSrv2.CashUnitsType MyCash = Cash.Where(a => a.devid == "1").FirstOrDefault();
                if (MyCash != null)
                {


                    FCCSrv2.CashUnitType DtCas = MyCash.CashUnit.Where(a => a.unitno == "4059").FirstOrDefault();
                    foreach (FCCSrv2.DenominationType Den in DtCas.Denomination)
                    {
                        BillCassetSumm += int.Parse(Den.fv) * int.Parse(Den.Piece);

                    }

                }

                return BillCassetSumm;
            }
            return -1;
        }

        static void mEventHandleClass_OnSetDeposit(object sender, int summ)
        {
            MainClass2.UpdateDeposit(summ);


        }

        public delegate void OnCashOutComplitedHandler(FCCSrv2.CashoutOperationCompletedEventArgs Summ);
        internal static event OnCashOutComplitedHandler OnCashOutComplited;




        public delegate void OnChangeCancelHandler(int Change);
        internal static event OnChangeCancelHandler OnChangeCancel;

        internal static  event EventHandleClass.SetStatusEventHandler OnSetStatus;

        internal static event EventHandleClass.SendErrorEventHandler OnSendError;


        static void mEventHandleClass_OnSetStatus(object sender, bool StatusChange, int status, int DevId, string EventName)
        {
            if (OnSetStatus != null)
            {
                OnSetStatus(sender,  StatusChange, status, DevId, EventName);
            }
        }

        //public delegate void ChangeMoneyEndEventHandler(FCCCheck Chk);
        internal static   event EventHandleClass.ReplenishCountChangeEventHandler OnReplenishCountChange;

        static internal  void mEventHandleClass_OnReplenishCountChange(object sender, FCCSrv2.CashType[] Ct)
        {
            if (OnReplenishCountChange != null)
            {
                OnReplenishCountChange(sender, Ct);
            }
        }

         void clsBrueBoxService_GetStatusCompleted(object sender, FCCSrv2.GetStatusCompletedEventArgs e)
        {
            Utils.ToLog("Ответ на асинхронный запрос статуса Glory");

            int res = 0;
            int Code = 0;
            int RBWState = 0;
            int RCWState = 0;
            string Status = "";

            if (e.Error == null)
            {

                FCCSrv2.StatusResponseType objStatusResponse = e.Result;
                res = int.Parse(objStatusResponse.result);
                if (int.Parse(objStatusResponse.result) == FCC_SUCCESS)
                {
                    Code = int.Parse(objStatusResponse.Status.Code);
                    Status = GetStatusStringRus(Code);
                    RBWState = int.Parse(objStatusResponse.Status.DevStatus.Where(a=>a.devid=="1").FirstOrDefault().st);
                    RCWState = int.Parse(objStatusResponse.Status.DevStatus.Where(a => a.devid == "2").FirstOrDefault().st);

                    Utils.ToLog(String.Format("objStatusResponse.result = {0};  objStatusResponse.Status.Code = {1}; StatusRus = {2} ", objStatusResponse.result, objStatusResponse.Status.Code, Status));
                }
                else
                {

                    Status = "Ошибка сервера FCC. Код ошибки " + objStatusResponse.result;
                    Utils.ToLog("Ошибка сервера FCC. Код ошибки " + objStatusResponse.result);
                }


            }
            else
            {
                res = -1;
                Status = "Ошибка соединения с устройством приема наличных средств. ";
                Utils.ToLog("Ошибка соединения с устройством приема наличных средств.  " + e.Error.Message);
            }

            CurrentGetStatusCallBack(res, Code,RBWState ,RCWState , Status);
        }


         DeviceRevisionResponseEventHandler CurrentGetStatusCallBack;
        public delegate void DeviceRevisionResponseEventHandler(int ResId, int StatusId,int RBWState,int RCWState, string StatusStr);

         string CurrentGetStatusAsyncID;

        public  int GetStatusAsync(DeviceRevisionResponseEventHandler CallBack)
        {
            CurrentGetStatusCallBack = CallBack;
            Utils.ToLog("Асинхронный запрос статуса Glory");
            int res = 0;
            string Status = "";
            bool WithCash = false;
            FCCSrv2.StatusRequestType objStatusRequest = new FCCSrv2.StatusRequestType();
            objStatusRequest.Id = GetId();
            CurrentGetStatusAsyncID = objStatusRequest.Id;
            objStatusRequest.SeqNo = GetSequenceNumber();
            objStatusRequest.Option = new FCCSrv2.StatusOptionType();
            if (WithCash)
            {
                objStatusRequest.Option.type = FCC_REQ_GETST_WITH_CASH.ToString();
            }
            else
            {
                objStatusRequest.Option.type = FCC_REQ_GETST_WITHOUT_CASH.ToString();
            }

            try
            {
                clsBrueBoxService.GetStatusAsync(objStatusRequest);
            }
            catch (Exception e)
            {
                Utils.ToLog("Ошибка асинхронного запроса статуса " + e.Message);
                Status = "Ошибка запроса статуса Glory. " + e.Message;
                res = -1;
            }
            return res;
        }




        public  int GetStatus(out string Status)
        {
            Utils.ToLog("Запрос статуса");
            int res = 0;
            Status = "";
            bool WithCash = false;
            FCCSrv2.StatusRequestType objStatusRequest = new FCCSrv2.StatusRequestType();
            objStatusRequest.Id = GetId();
            objStatusRequest.SeqNo = GetSequenceNumber();
            objStatusRequest.Option = new FCCSrv2.StatusOptionType();
            if (WithCash)
            {
                objStatusRequest.Option.type = FCC_REQ_GETST_WITH_CASH.ToString();
            }
            else
            {
                objStatusRequest.Option.type = FCC_REQ_GETST_WITHOUT_CASH.ToString();
            }
            try
            {
                FCCSrv2.StatusResponseType objStatusResponse = clsBrueBoxService.GetStatus(objStatusRequest);
                if (int.Parse(objStatusResponse.result) == FCC_SUCCESS)
                {
                    res = int.Parse(objStatusResponse.Status.Code);
                    Status = GetStatusStringRus(res) +"Код: "+res.ToString();
                    int RBWState = int.Parse(objStatusResponse.Status.DevStatus.Where(a => a.devid == "1").FirstOrDefault().st);
                    int RCWState = int.Parse(objStatusResponse.Status.DevStatus.Where(a => a.devid == "2").FirstOrDefault().st);
                    Status += Environment.NewLine + GetDiviceStateByCode(RBWState, 1);
                    Status += Environment.NewLine + GetDiviceStateByCode(RBWState, 2);
                    

                }
                else
                {
                    res = int.Parse(objStatusResponse.result);
                    Status = "Ошибка соединения с сервером FCC";
                }

                Utils.ToLog(String.Format("objStatusResponse.result = {0};  objStatusResponse.Status.Code = {1}; StatusRus = {2} ", objStatusResponse.result, objStatusResponse.Status.Code, Status));

            }
            catch (Exception e)
            {
                Utils.ToLog("Ошибка запроса статуса " + e.Message);
                Status = "Ошибка запроса статуса Glory. " + e.Message;
                res = -1;
            }
            return res;
        }
        private  string GetId()
        {
            return Guid.NewGuid().ToString();
        }

         int seqNumber;
        private  String GetSequenceNumber()
        {
            String seqnum = DateTime.Today.ToString("yyyyMMdd");
            seqnum += seqNumber.ToString("000#");
            return seqnum;
        }
        public  String GetStatusStringRus(int st)
        {
            string[] strRet = new string[] { 
                "Инициализация",
                "Устройство готово к работе",
                "",
                "Внесите денежные средства",
                "Считаю...",
                "Выдача сдачи",
                "Пожалуйста, заберите непринятые купюры",
                "Пожалуйста, возьмите сдачу",
                "RESET",
                "DEPOSIT CANCEL",
                "CALCULATING AMOUNT",
                "CASH IN CANCEL",
                "COLLECTING",
                "Ошибка устройства",
                "DOWNLOADING FARMWARE",
                "READING LOGS",
                "Ожидаю внесения",
                "CALCULATING REPLENISH AMOUNT",
                "Дверца разблокирована",
                
                "Ожидайте. Проверка оборудования..",
                "FIXED DEPOSIT AMOUNT",
                "FIXED DISPENSE AMOUNT"
            };

            if (strRet.Length <= st)
            {
                return "Unknown";
            }
            return strRet[st];
        }

        public bool SmallChange(out List<int> NewMinfvs, out List<int> NEfvs)
        {
            Dictionary<int, int> fvsCount = new Dictionary<int, int>();
            //Utils.ToLog("SmallChange");
            List<int> Minfvs = new List<int>();
            NEfvs = new List<int>();
            NewMinfvs = new List<int>();
           
            try
            {
                FCCSrv2.CashUnitsType[] Cash = UpdateInventory();
                FCCSrv2.CashUnitsType MyCash = Cash.Where(a => a.devid == "1").FirstOrDefault();
                if (MyCash != null)
                {
                    
                    foreach (FCCSrv2.CashUnitType Dt in MyCash.CashUnit.Where(a => a.unitno == "4043" || a.unitno == "4044" || a.unitno == "4045"))
                    {
                        int fv = int.Parse(Dt.Denomination[0].fv);
                        int Count = MyCash.CashUnit.Where(a => ((a.unitno == "4043" || a.unitno == "4044" || a.unitno == "4045") && (a.Denomination[0].fv == fv.ToString()))).Sum(b => int.Parse(b.Denomination[0].Piece));
                        bool NotNe = MyCash.CashUnit.Where(a => ((a.unitno == "4043" || a.unitno == "4044" || a.unitno == "4045") && (a.Denomination[0].fv == fv.ToString())&&(int.Parse(a.ne)<int.Parse(a.Denomination[0].Piece)))).Count()>0;
                        if (!NotNe)
                        {
                            if (!NEfvs.Contains(fv))
                            {
                                NEfvs.Add(fv);
                            }
                        }
                        int MCount =0;
                        if (!fvsCount.TryGetValue(fv, out MCount))
                        {
                            fvsCount.Add(fv, Count);
                        }
                        else
                        {
                            fvsCount[fv] += Count;
                        }

                        int NeedCount = 0;
                        if (MainClass2.Setting.BarabanMins.Where(a => a.fv == fv).Count() >0)
                        {
                            NeedCount = MainClass2.Setting.BarabanMins.Where(a => a.fv == fv).First().MinCount;
                        }
                        if (NeedCount > Count)
                        {
                            if (!Minfvs.Contains(fv))
                            {
                                Minfvs.Add(fv);
                            }
                            //return true;
                        }
                       
                    }
                }
                MyCash = Cash.Where(a => a.devid == "2").FirstOrDefault();
                if (MyCash != null)
                {

                    foreach (FCCSrv2.CashUnitType Dt in MyCash.CashUnit.Where(a => int.Parse(a.unitno) > 4042 && int.Parse(a.unitno) < 4056))
                    {
                        int fv = int.Parse(Dt.Denomination[0].fv);
                        int Count = MyCash.CashUnit.Where(a => ((int.Parse(a.unitno) > 4042 && int.Parse(a.unitno) < 4056) && (a.Denomination[0].fv == fv.ToString()))).Sum(b => int.Parse(b.Denomination[0].Piece));
                        bool NotNe = MyCash.CashUnit.Where(a => ((int.Parse(a.unitno) > 4042 && int.Parse(a.unitno) < 4056) && (a.Denomination[0].fv == fv.ToString()) && (int.Parse(a.ne) < int.Parse(a.Denomination[0].Piece)))).Count() > 0;
                        if (!NotNe)
                        {
                            if (!NEfvs.Contains(fv))
                            {
                                NEfvs.Add(fv);
                            }
                        }
                        int MCount = 0;
                        if (!fvsCount.TryGetValue(fv, out MCount))
                        {
                            fvsCount.Add(fv, Count);
                        }
                        else
                        {
                            fvsCount[fv] += Count;
                        }
                        int NeedCount = 0;
                        if (MainClass2.Setting.BarabanMins.Where(a => a.fv == fv).Count()>0)
                        {
                            NeedCount = MainClass2.Setting.BarabanMins.Where(a => a.fv == fv).First().MinCount;
                        }
                        if (NeedCount > Count)
                        {
                            if (!Minfvs.Contains(fv))
                            {
                                Minfvs.Add(fv);
                            }
                        }
                    }
                }

             
                foreach (int fv in Minfvs)
                {
                    int BeforeFvCount = 0;
                    int BeforeBeforeFvCount = 0;
                    int NeedCount = 0;
                    int BeforeFv= fvsCount.Keys.Where(a=>a<fv).DefaultIfEmpty().Max();
                    if (BeforeFv!=0)
                    {
                        BeforeFvCount = fvsCount[BeforeFv];
                    }
                    int BeforeBeforeFv = fvsCount.Keys.Where(a => a < BeforeFv).DefaultIfEmpty().Max();
                    if (BeforeBeforeFv != 0)
                    {
                        BeforeBeforeFvCount = fvsCount[BeforeBeforeFv];
                    }

                    if (MainClass2.Setting.BarabanMins.Where(a => a.fv == fv).Count() > 0)
                    {
                        //if (MainClass2.Setting.BarabanMins.Where(a => a.fv == fv).Count() > 0)
                        //{
                            NeedCount = MainClass2.Setting.BarabanMins.Where(a => a.fv == fv).First().MinCount;
                        //}
                    }
                    if (fvsCount[fv] + BeforeFvCount * ((double)BeforeFv / (double)fv) + BeforeBeforeFvCount * ((double)BeforeBeforeFv / (double)fv) < NeedCount)
                    {

                        NewMinfvs.Add(fv);   
                    }
                
                }

                return NewMinfvs.Count > 0;
            }
            catch(Exception e)
            {
                Utils.ToLog("Error SmallChange " + e.Message);
                return false;
            }


        }

         UpdateInventoryResponseEventHandler CurrentUpdateInventoryResponseCallBack;
        public delegate void UpdateInventoryResponseEventHandler(FCCSrv2.CashUnitsType[] res, int ResultId, string ResStr);
        public  void UpdateInventoryAsync(UpdateInventoryResponseEventHandler callback)
        {
            Utils.ToLogFCCApiLog("UpdateInventoryAsync");
            CurrentUpdateInventoryResponseCallBack = callback;
            FCCSrv2.InventoryRequestType objStatusRequest = new FCCSrv2.InventoryRequestType();
            objStatusRequest.Id = GetId();
            objStatusRequest.SeqNo = GetSequenceNumber();
            objStatusRequest.Option = new FCCSrv2.InventoryOptionType();
            objStatusRequest.Option.type = "0";
            clsBrueBoxService.InventoryOperationAsync(objStatusRequest);
        }

        static int LastSumm = 0;

        private void UpDateLastSumm(FCCSrv2.CashUnitsType[] Bars)
        {
            try
            {
                Utils.ToLogFCCApiLog("UpDateLastSumm");
                int NLastSumm = 0;
                foreach (FCCSrv2.CashUnitsType CUT in Bars)
                {
                    foreach (FCCSrv2.CashUnitType CU in CUT.CashUnit)
                    {
                        foreach (FCCSrv2.DenominationType dt in CU.Denomination)
                        {
                            NLastSumm += Convert.ToInt32(dt.cc) * Convert.ToInt32(dt.fv);
                        }
                    }
                }
                LastSumm = NLastSumm;
                Utils.ToLogFCCApiLog("UpDateLastSumm", true, 0, "", "");
            }
            catch (Exception e)
            {
                Utils.ToLogFCCApiLog("UpDateLastSumm", true, 0, "", e.Message);
            }
        }
        public  FCCSrv2.CashUnitsType[] UpdateInventory()
        {
            Utils.ToLogFCCApiLog("UpdateInventory");
            try
            {
                FCCSrv2.InventoryRequestType objStatusRequest = new FCCSrv2.InventoryRequestType();
                objStatusRequest.Id = GetId();
                objStatusRequest.SeqNo = GetSequenceNumber();
                objStatusRequest.Option = new FCCSrv2.InventoryOptionType();
                objStatusRequest.Option.type = "0";
                FCCSrv2.InventoryResponseType objStatusResponse = clsBrueBoxService.InventoryOperation(objStatusRequest);
                if (int.Parse(objStatusResponse.result)==FCC_SUCCESS)
                {
                    UpDateLastSumm(objStatusResponse.CashUnits);
                    return objStatusResponse.CashUnits;
                }
                return null;

            }
            catch(Exception e)
            {
                return null;
            }
        }



         private string TransactionResultStr(int ResId)
        {
            switch (ResId)
            {
                case 0:
                    return "success";
                    break;

                case 3:
                    return "occupied by other";
                    break;
                case 11:
                    return "exlusive error";
                    break;


                default:
                    return "";
                    break;
            }
        }

         void clsBrueBoxService_InventoryOperationCompleted(object sender, FCCSrv2.InventoryOperationCompletedEventArgs e)
        {
            Utils.ToLogFCCApiLog("InventoryOperationCompleted");

            if (e.Error == null)
            {
                Utils.ToLogFCCApiLog("InventoryOperationCompleted", true, int.Parse(e.Result.result), TransactionResultStr(int.Parse(e.Result.result)),"");
                if (int.Parse(e.Result.result) == FCC_SUCCESS)
                {
                    CurrentUpdateInventoryResponseCallBack(e.Result.CashUnits, int.Parse(e.Result.result), "");
                }
                else
                {
                    CurrentUpdateInventoryResponseCallBack(null, int.Parse(e.Result.result), "");
                }
            }
            else
            {
                Utils.ToLogFCCApiLog("InventoryOperationCompleted",true , 0, "", e.Error.Message);
                Utils.ToLog("InventoryOperationCompleted Error  " + e.Error.Message);
                CurrentUpdateInventoryResponseCallBack(null, -1, e.Error.Message);
            }
        }


         bool replenishment;
        public  bool Replenishment
        {
            set
            {
                replenishment = value;
            }
            get
            {
                return replenishment;
            }
        }

         StartReplenishmentResponseEventHandler StartReplenishmentResponseCallBack;
         StartReplenishmentResponseEventHandler CancelReplenishmentResponseCallBack;
         EndReplenishmentResponseEventHandler EndReplenishmentResponseCallBack;
        public delegate void StartReplenishmentResponseEventHandler(int ResultId, string ResStr);
        public delegate void EndReplenishmentResponseEventHandler(int ResultId, string ResStr,int Summ);
        public  void StartReplenishmentAsync(StartReplenishmentResponseEventHandler callBack)
        {
            
            Utils.ToLogFCCApiLog("StartReplenishment");
            StartReplenishmentResponseCallBack = callBack;
            FCCSrv2.StartReplenishmentFromEntranceRequestType objStartRepFERequest = new FCCSrv2.StartReplenishmentFromEntranceRequestType();
            FCCSrv2.StartReplenishmentFromEntranceResponseType objStartRepFEResponse = new FCCSrv2.StartReplenishmentFromEntranceResponseType();
            objStartRepFERequest.Id = GetId(); ;
            objStartRepFERequest.SeqNo = GetSequenceNumber();
            
            clsBrueBoxService.StartReplenishmentFromEntranceOperationAsync(objStartRepFERequest);

                Replenishment = true;
            
        }


         void clsBrueBoxService_StartReplenishmentFromEntranceOperationCompleted(object sender, FCCSrv2.StartReplenishmentFromEntranceOperationCompletedEventArgs e)
        {
            Utils.ToLogFCCApiLog("StartReplenishmentFromEntranceOperationCompleted");

            if (e.Error == null)
            {
                Utils.ToLogFCCApiLog("StartReplenishmentFromEntranceOperationCompleted", true, int.Parse(e.Result.result), TransactionResultStr(int.Parse(e.Result.result)), "");
                if (int.Parse(e.Result.result) == FCC_SUCCESS)
                {
                    StartReplenishmentResponseCallBack(int.Parse(e.Result.result), "");
                }
                else
                {
                    string Mess = "Код ошибки " + e.Result.result+Environment.NewLine;

                    string MessTmp = "";
                    GetStatus(out MessTmp);
                    Replenishment = false; 
                    StartReplenishmentResponseCallBack(int.Parse(e.Result.result), Mess+MessTmp);
                }
            }
            else
            {
                Utils.ToLogFCCApiLog("StartReplenishmentFromEntranceOperationCompleted", true, 0, "", e.Error.Message);
                Utils.ToLog("StartReplenishmentFromEntranceOperationCompleted Error  " + e.Error.Message);
                StartReplenishmentResponseCallBack(-1, e.Error.Message);
            }

        }




        public  void CancelReplenishment(StartReplenishmentResponseEventHandler callBack)
        {
            Utils.ToLogFCCApiLog("CancelReplenishment");
            CancelReplenishmentResponseCallBack = callBack;
            FCCSrv2.ReplenishmentFromEntranceCancelRequestType objCancelRepFERequest = new FCCSrv2.ReplenishmentFromEntranceCancelRequestType();
            FCCSrv2.ReplenishmentFromEntranceCancelResponseType objCancelRepFEResponse = new FCCSrv2.ReplenishmentFromEntranceCancelResponseType();
            objCancelRepFERequest.Id = GetId();
            objCancelRepFERequest.SeqNo = GetSequenceNumber();
            try
            {
                clsBrueBoxService.ReplenishmentFromEntranceCancelOperationAsync(objCancelRepFERequest);

                Replenishment = false;
            }
            catch (Exception ex)
            {

            }
        }

        public  void EndReplenishment(EndReplenishmentResponseEventHandler callBack)
        {
            Utils.ToLogFCCApiLog("EndReplenishment");
            EndReplenishmentResponseCallBack = callBack;
            FCCSrv2.EndReplenishmentFromEntranceRequestType objEndRepFERequest = new FCCSrv2.EndReplenishmentFromEntranceRequestType();
            FCCSrv2.EndReplenishmentFromEntranceResponseType objEndRepFEResponse = new FCCSrv2.EndReplenishmentFromEntranceResponseType();
            objEndRepFERequest.Id = GetId();
            objEndRepFERequest.SeqNo = GetSequenceNumber();
            try
            {
                clsBrueBoxService.EndReplenishmentFromEntranceOperationAsync(objEndRepFERequest);

                Replenishment = false;
            }
            catch (Exception ex)
            {

            }
        }



        internal int CurrentCassetaSumm = 0;
        internal int CurrentCoinCassetaSumm = 0;



         void clsBrueBoxService_ReplenishmentFromEntranceCancelOperationCompleted(object sender, FCCSrv2.ReplenishmentFromEntranceCancelOperationCompletedEventArgs e)
        {
            Utils.ToLogFCCApiLog("ReplenishmentFromEntranceCancelOperationCompleted");

            if (e.Error == null)
            {
                Utils.ToLogFCCApiLog("ReplenishmentFromEntranceCancelOperationCompleted", true, int.Parse(e.Result.result), TransactionResultStr(int.Parse(e.Result.result)), "");
                if (int.Parse(e.Result.result) == FCC_SUCCESS)
                {
                    int Summ = 0;
                    foreach(FCCSrv2.CashType Ct in e.Result.Cash)
                    {
                        if (Ct.Denomination != null)
                        {
                            Summ += int.Parse(Ct.Denomination[0].fv) * int.Parse(Ct.Denomination[0].Piece);
                        }
                    
                    }
                    //MainClass2.RaiseOnReplenish(Summ);
                    Utils.ToMoneyCountLog(MoneyChangeCommands.ReplenishCancel, Summ);
                    CancelReplenishmentResponseCallBack(int.Parse(e.Result.result), "");
                }
                else
                {
                    CancelReplenishmentResponseCallBack(int.Parse(e.Result.result), "");
                }
            }
            else
            {
                Utils.ToLogFCCApiLog("ReplenishmentFromEntranceCancelOperationCompleted", true, 0, "", e.Error.Message);
                Utils.ToLog("ReplenishmentFromEntranceCancelOperationCompleted Error  " + e.Error.Message);
                CancelReplenishmentResponseCallBack(-1, e.Error.Message);
            }
        }

         void clsBrueBoxService_EndReplenishmentFromEntranceOperationCompleted(object sender, FCCSrv2.EndReplenishmentFromEntranceOperationCompletedEventArgs e)
        {
            Utils.ToLogFCCApiLog("EndReplenishmentFromEntranceOperationCompleted");

            if (e.Error == null)
            {
                Utils.ToLogFCCApiLog("EndReplenishmentFromEntranceOperationCompleted", true, int.Parse(e.Result.result), TransactionResultStr(int.Parse(e.Result.result)), "");
                if (int.Parse(e.Result.result) == FCC_SUCCESS)
                {
                    int Summ = 0;
                    if (e.Result.Cash.Denomination != null)
                    {
                        foreach (FCCSrv2.DenominationType Ct in e.Result.Cash.Denomination)
                        //FCCSrv2.CashType Ct = e.Result.Cash;
                        {


                            Summ += int.Parse(Ct.fv) * int.Parse(Ct.Piece);


                        }
                    }
                  //  MainClass2.RaiseOnReplenish(Summ);
                    Utils.ToLog("End Replenish Summ+"+ Summ.ToString());
             
                    EndReplenishmentResponseCallBack(int.Parse(e.Result.result), "",Summ);
                }
                else
                {
                    EndReplenishmentResponseCallBack(int.Parse(e.Result.result), "",0);
                }
            }
            else
            {
                Utils.ToLogFCCApiLog("EndReplenishmentFromEntranceOperationCompleted", true, 0, "", e.Error.Message);
                Utils.ToLog("EndReplenishmentFromEntranceOperationCompleted Error  " + e.Error.Message);
                EndReplenishmentResponseCallBack(-1, e.Error.Message,0);
            }
        }

        // COLLECT_REQ_MIX_TYPE
        public const int FCC_REQ_COLLECTION_MIX_OFF = 0;
        public const int FCC_REQ_COLLECTION_MIX_ON = 1;
        // COLLECT_REQ_CASH_TYPE
        public const int FCC_REQ_COLLECTION_CASH_INFO = 5;

        


         StartReplenishmentResponseEventHandler CollectAsyncResponseCallBack;

        public  void CollectAsync(FCCSrv2.DenominationType[] Cash, StartReplenishmentResponseEventHandler callback)
        {
            CollectAsyncResponseCallBack = callback;
            FCCSrv2.CollectRequestType objCollect = new FCCSrv2.CollectRequestType();
            objCollect.Id = GetId();
            objCollect.SeqNo = GetSequenceNumber();
            objCollect.Option = new FCCSrv2.CollectOptionType();
            objCollect.Option.type = "0";

            objCollect.RequireVerification = new FCCSrv2.RequireVerificationType();
            objCollect.RequireVerification.type = "0";
            objCollect.Mix = new FCCSrv2.CollectOptionType();
            objCollect.Mix.type = FCC_REQ_COLLECTION_MIX_ON.ToString();
            objCollect.Cash = new FCCSrv2.CashType();
            objCollect.Cash.type = FCC_REQ_COLLECTION_CASH_INFO.ToString();
         //   objCollect.Cash.type = "5";
          //  objCollect.Cash.Denomination = new FCCSrv2.DenominationType[11];
            //FCCSrv2.CashUnitsType[] CU = UpdateInventory();
            //FCCSrv2.CashUnitsType MyCash = CU.Where(a => a.devid == "1").FirstOrDefault();
            objCollect.Cash.Denomination = Cash;
            /*
            objCollect.Partial = new FCCSrv2.CollectPartialType();
            objCollect.Partial.type = "1";
            */
            /*
            if (MyCash != null)
            {
                FCCSrv2.CashUnitType Dt = MyCash.CashUnit.Where(a => a.unitno == "4045").FirstOrDefault();
                objCollect.Cash.Denomination[0] = Dt.Denomination[0];
                Dt = MyCash.CashUnit.Where(a => a.unitno == "4044").FirstOrDefault();
                objCollect.Cash.Denomination[1] = Dt.Denomination[0];
                Dt = MyCash.CashUnit.Where(a => a.unitno == "4043").FirstOrDefault();
                objCollect.Cash.Denomination[2] = Dt.Denomination[0];
            }
             * */
            clsBrueBoxService.CollectOperationAsync(objCollect);

        }

         void clsBrueBoxService_CollectOperationCompleted(object sender, FCCSrv2.CollectOperationCompletedEventArgs e)
        {
            Utils.ToLogFCCApiLog("CollectOperationCompleted");

            if (e.Error == null)
            {
                Utils.ToLogFCCApiLog("CollectOperationCompleted", true, int.Parse(e.Result.result), TransactionResultStr(int.Parse(e.Result.result)), "");
                if (int.Parse(e.Result.result) == FCC_SUCCESS)
                {
                    CollectAsyncResponseCallBack(int.Parse(e.Result.result), "");
                }
                else
                {
                    CollectAsyncResponseCallBack(int.Parse(e.Result.result), "");
                }
            }
            else
            {
                Utils.ToLogFCCApiLog("CollectOperationCompleted", true, 0, "", e.Error.Message);
                Utils.ToLog("CollectOperationCompleted Error  " + e.Error.Message);
                CollectAsyncResponseCallBack(-1, e.Error.Message);
            }
        }

        public  void Unlock(bool Kasseta, bool Mixer)
        {
            try
            {
                if (Kasseta)
                {
                    FCCSrv2.UnLockUnitRequestType objUnlock = new FCCSrv2.UnLockUnitRequestType();
                    objUnlock.Id = GetId();
                    objUnlock.SeqNo = GetSequenceNumber();
                    objUnlock.Option = new FCCSrv2.UnLockUnitOptionType();
                    objUnlock.Option.type = "1";
                    clsBrueBoxService.UnLockUnitOperation(objUnlock);
                }
                if (Mixer)
                {
                    FCCSrv2.UnLockUnitRequestType objUnlock = new FCCSrv2.UnLockUnitRequestType();
                    objUnlock.Id = GetId();
                    objUnlock.SeqNo = GetSequenceNumber();
                    objUnlock.Option = new FCCSrv2.UnLockUnitOptionType();
                    objUnlock.Option.type = "2";
                    clsBrueBoxService.UnLockUnitOperation(objUnlock);
                }
            }
            catch
            { }

        }
        public void Lock()
        {
            try
            {
                Utils.ToLogFCCApiLog("Lock");
                FCCSrv2.LockUnitRequestType objLock = new FCCSrv2.LockUnitRequestType();
                objLock.Id = GetId();
                objLock.SeqNo = GetSequenceNumber();
                objLock.Option = new FCCSrv2.LockUnitOptionType();
                objLock.Option.type = "1";
                clsBrueBoxService.LockUnitOperation(objLock);
                objLock = new FCCSrv2.LockUnitRequestType();
                objLock.Id = GetId();
                objLock.SeqNo = GetSequenceNumber();
                objLock.Option = new FCCSrv2.LockUnitOptionType();
                objLock.Option.type = "2";
                clsBrueBoxService.LockUnitOperation(objLock);
            }
            catch
            { }
        }

        private static int LastChangeSumm = 0;
        public  string StartChangeAsync(int Summ)
        {
            Utils.ToLogFCCApiLog("StartChangeAsync");
            LastChangeSumm = Summ;
            FCCSrv2.ChangeRequestType objChangeRequestType =
            new FCCSrv2.ChangeRequestType();
            objChangeRequestType.Id = GetId();
            objChangeRequestType.SeqNo = GetSequenceNumber();
            //set total amount
            objChangeRequestType.Amount = Summ.ToString();
            //Invoke asynchronous change request
            try
            {
                clsBrueBoxService.Timeout = 6000000; //My.Settings.SoapRequestTimeout
                clsBrueBoxService.ChangeOperationAsync(objChangeRequestType);
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
            return "";
        }

         void clsBrueBoxService_ChangeOperationCompleted(object sender, FCCSrv2.ChangeOperationCompletedEventArgs arg)
        {
            Utils.ToLogFCCApiLog("ChangeOperationCompleted");

            if (arg.Error == null)
            {
                Utils.ToLogFCCApiLog("ChangeOperationCompleted", true, int.Parse(arg.Result.result), TransactionResultStr(int.Parse(arg.Result.result)), "");
                long insertedAmount = 0;
                long dispensedAmount = 0;
                string insertedDenom = getDetailDenom(arg.Result.Cash[0].Denomination, ref insertedAmount, arg.Result.ManualDeposit);
                string dispensedDenom = getDetailDenom(arg.Result.Cash[1].Denomination, ref dispensedAmount, "0");
                long changeAmount = insertedAmount - int.Parse(arg.Result.Amount) - dispensedAmount;

                if (int.Parse(arg.Result.result) == FCC_SUCCESS)
                {
                    MainClass2.RaiseOnEndChange(0, (int)insertedAmount, (int)dispensedAmount);
                }
                else if ((int.Parse(arg.Result.result) == 10) || (int.Parse(arg.Result.result) == 12) || (int.Parse(arg.Result.result) == 100))
                {
                    MainClass2.RaiseOnEndChange((int)changeAmount, (int)insertedAmount, (int)dispensedAmount);
                }
                else if ((int.Parse(arg.Result.result) == 1) || (int.Parse(arg.Result.result) == 9)) 
                {
                    MainClass2.RaiseOnCancelChange((int)insertedAmount - (int)dispensedAmount, (int)insertedAmount, (int)dispensedAmount);
                }
                else if ((int.Parse(arg.Result.result) == 11) || (int.Parse(arg.Result.result) == 100)) //Error
                {
                    MainClass2.RaiseOnCancelChange((int)insertedAmount, (int)dispensedAmount, "Код ошибки " + arg.Result.result);
                }
            }
            else
            {
                Utils.ToLogFCCApiLog("ChangeOperationCompleted", true, 0, "", arg.Error.Message);
                Utils.ToLog("ChangeOperationCompleted Error  " + arg.Error.Message);
                CorrectErrorChangeOperationCompleted();

            }
        }


         private void CorrectErrorChangeOperationCompleted()
         {
             Utils.ToLogFCCApiLog("CorrectErrorChangeOperationCompleted");
             int OldSumm = LastSumm;
             UpdateInventory();
             if (LastSumm + LastChangeSumm <= OldSumm)
             {
                 Utils.ToLogFCCApiLog("CorrectErrorChangeOperationCompleted Summ ok");
                 MainClass2.RaiseOnEndChange(0, LastChangeSumm, LastChangeSumm);
             }
             else
             {
                 Utils.ToLogFCCApiLog("CorrectErrorChangeOperationCompleted Not enought money");
                 MainClass2.RaiseOnCancelChange(LastSumm - OldSumm, LastSumm - OldSumm, 0);
             }
         }

        private  string getDetailDenom(FCCSrv2.DenominationType[] dnm, ref long amount, String manual)
        {
            int i = 0;
            string retStr = "";
            if (dnm != null || int.Parse(manual) > 0)
            {
                if (dnm != null)
                {
                    for (i = 0; i < dnm.Length; i++)
                    {
                        amount += int.Parse(dnm[i].fv) * int.Parse(dnm[i].Piece);
                        /*
                    retStr += "\n " + (String.Format("{0:f2}", double.Parse(dnm[i].fv)
                    / 100) + Euro " + dnm[i].Piece + " Pieces, ");
                         */
                    }
                }
                // Manual Input
                if (int.Parse(manual) > 0)
                {
                    amount += int.Parse(manual);
                    retStr += "\n " + "Manual Input " + String.Format("{0:f2}",
                    double.Parse(manual) / 100) + "Euro, ";
                }
            }
            else
            {
                retStr = "Nothing";
            }
            return retStr;
        }

         public void ChangeCancel()
        {
            try
            {
                Utils.ToLogFCCApiLog("ChangeCancel");
                FCCSrv2.ChangeCancelRequestType objChangeCancelRequest = new FCCSrv2.ChangeCancelRequestType();
                objChangeCancelRequest.Id = GetId();
                objChangeCancelRequest.SeqNo = GetSequenceNumber();
                clsBrueBoxService.ChangeCancelOperationAsync(objChangeCancelRequest);
            }
            catch(Exception e)
            {
                Utils.ToLogFCCApiLog("ChangeCancel", true, 0, "", e.Message);
            }
        }

         void clsBrueBoxService_ChangeCancelOperationCompleted(object sender, FCCSrv2.ChangeCancelOperationCompletedEventArgs e)
         {
             Utils.ToLogFCCApiLog("ChangeCancelOperationCompleted");
             if (e.Error == null)
             {
                 Utils.ToLogFCCApiLog("ChangeCancelOperationCompleted", true, int.Parse(e.Result.result), TransactionResultStr(int.Parse(e.Result.result)), "");
                 /*
                 if (int.Parse(e.Result.result) == FCC_SUCCESS)
                 {

                     if (OnChangeCancel != null)
                     {
                         
                         OnChangeCancel(int.Parse(e.Result.result), GetStatusStringRus(int.Parse(e.Result.result)));
                     }
                 }
                 else
                 {
                     if (OnChangeCancel != null)
                     {
                         OnChangeCancel(int.Parse(e.Result.result), GetStatusStringRus(int.Parse(e.Result.result)));
                     }
                 }
                  * */
             }
             else
             {
                 Utils.ToLogFCCApiLog("ChangeCancelOperationCompleted", true, 0, "", e.Error.Message);
                 Utils.ToLog("ChangeCancelOperationCompleted Error  " + e.Error.Message);
                 /*
                 if (OnChangeCancel != null)
                 {
                     OnChangeCancel(-1, e.Error.Message);
                 }
                  * */
                 //CollectAsyncResponseCallBack(-1, e.Error.Message);
             }
         }


         public  string StartCashInAsinc()
         {
             FCCSrv2.StartCashinRequestType objChangeRequestType = new FCCSrv2.StartCashinRequestType();
             objChangeRequestType.Id = GetId();
             objChangeRequestType.SeqNo = GetSequenceNumber();
             //set total amount
            
             //Invoke asynchronous change request
             try
             {
                 clsBrueBoxService.Timeout = 6000000; //My.Settings.SoapRequestTimeout
                 clsBrueBoxService.StartCashinOperationAsync(objChangeRequestType);
             }
             catch (Exception ex)
             {
                 return ex.Message.ToString();
             }
             return "";
         
         }


         public string CashOut( FCCSrv2.DenominationType[] CashDenom)
         {
             FCCSrv2.CashoutRequestType objChangeRequestType = new FCCSrv2.CashoutRequestType();
             objChangeRequestType.Id = GetId();
             objChangeRequestType.SeqNo = GetSequenceNumber();
             //set total amount
             objChangeRequestType.Cash = new FCCSrv2.CashType();
             objChangeRequestType.Cash.type = "0";
             objChangeRequestType.Cash.Denomination = CashDenom;
             //Invoke asynchronous change request
             try
             {
                 clsBrueBoxService.Timeout = 6000000; //My.Settings.SoapRequestTimeout
              clsBrueBoxService.CashoutOperationAsync(objChangeRequestType);
            

             }
             catch (Exception ex)
             {
                 return ex.Message.ToString();
             }
             return "";

         }


         internal static string GetDiviceStateByCode(int Code, int DevId)
         {
             string DevName;
             if (DevId == 1)
             {
                 DevName = "Купюроприемник";
             }
             else if (DevId == 2)
             {
                 DevName = "Монетоприемник";
             }
             else
             {
                 DevName = "Неизвестное устройство DevId = " + DevId;
             }

             string Status = " в состоянии "+Code.ToString()+": ";

             switch (Code)
             {
                 case 0:
                     Status += " Инициализация (STATE_INITIALIZE)";
                     break;
                 case 1000:
                     Status += " Готово (STATE_IDLE)";
                     break;
                 case 9100:
                     Status += " Занято (STATE_BUSY)";
                     break;
                 case 9300:
                     Status += " (STATE_COM_ERROR)";
                     break;
                 default:
                     
                     break;
             }
             return DevName + Status;
         }
    }
}
