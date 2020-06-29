using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;

namespace PDiscountCard
{
    class SBRFCreditCardConnector:ICreditCardConnector
    {

        public SBRFCreditCardConnector()
        {
            try
            {
                Utils.ToCardLog("SBRFCreditCardConnector.Init() ");
                SBRFSRV.Server SBSrv = new SBRFSRV.Server();
                SBSrv.Clear();
                Utils.ToCardLog("SBRFCreditCardConnector.Init() End");
                Inited = true;
            }
            catch(Exception e)
            {
                Utils.ToCardLog("[Error] SBRFCreditCardConnector.Init() " + e.Message);
            }
        }

        public bool OperInProcess{set;get;}
        public bool Inited{set;get;}
        string resOper = "";
        string RespCode = "00";
        string Receipt = "";
        CreditCardOperationType mOperType;
        bool Sinc = false;
        Int32 Amount;
        private void RunOper()
        {
            try
            {
                Utils.ToCardLog("Запуск RunOper OperType=" + mOperType.ToString() + " Amount= " + Amount.ToString());
                OperInProcess = true;
                SBRFSRV.Server SBSrv = new SBRFSRV.Server();
                SBSrv.Clear();
                Int32 res = -1;
                if (mOperType==CreditCardOperationType.Payment)
                {
                    SBSrv.SParam("Amount", Math.Abs(Amount).ToString());
                    res = SBSrv.NFun(4000);
                }
                else if (mOperType == CreditCardOperationType.VoidPayment)
                {
                    SBSrv.SParam("Amount", Math.Abs(Amount).ToString());
                    res = SBSrv.NFun(4002);
                }
                else if (mOperType == CreditCardOperationType.Sverka)
                {
                    res = SBSrv.NFun(6000);
                }
                else if (mOperType == CreditCardOperationType.XReport)
                {
                    res = SBSrv.NFun(6002);
                }
                else if (mOperType == CreditCardOperationType.LongReport)
                {
                    res = SBSrv.NFun(7000);
                }
                else if (mOperType == CreditCardOperationType.LastChk)
                {
                 //   SBSrv.SParam("PayInfo", "3");
                    res = SBSrv.NFun(7001);
                }
                Receipt = SBSrv.GParamString("Cheque");
                if (Receipt == null) Receipt = "";
                Receipt = AddCutToReceipt(Receipt);
                if (Receipt != "")
                {
                    CreditCardAlohaIntegration.CreditCardOperationComplited(mOperType, false, res == 0, res.ToString(), Receipt);
                }
                else

                {
                    CreditCardAlohaIntegration.CreditCardOperationComplited(mOperType, true , res == 0, "Код ошибки "+res.ToString(), Receipt);
                
                }
                    SBSrv.Clear();
                OperInProcess = false;
                Utils.ToCardLog("Отработал RunOper OperType=" + mOperType.ToString() + " Amount= " + Amount.ToString());
            }
            catch (Exception e)
            {
                OperInProcess = false;
                resOper = "Ошибка программы. " + e.Message;
                RespCode = "-1";
                CreditCardAlohaIntegration.CreditCardOperationComplited(mOperType, true, false, resOper, Receipt);
                Utils.ToCardLog("[Error] Запускa RunOper " + e.Message);
            }
        }


        private string AddCutToReceipt(string Receipt)
        {
            if (Receipt.Length > 110)
            {
                string End = Receipt.Substring(Receipt.Length - 100);
                string Start = Receipt.Substring(0, Receipt.Length - 100);
                Start = Start.Replace("\r\n  \r\n  \r\n  \r\n  ", "\r\n  \r\n  \r\n  \r\n  \r\n  0xDA");
                Receipt = Start + End;
            }
 
                   
           Receipt = Receipt.Replace("Комиссия за операцию - 0 руб.", "Комиссия за операцию - 0 руб. \r\n");
           return Receipt;
        }

        private void RunOperationAsincComplitedVoid(string res, string resStr, string Response, string Receipt)
        {
            bool ShowError = false;
            
            if (res == "999")
            {
                resStr += Environment.NewLine + "Нет связи с терминалом пластиковых карт.";
                resStr += Environment.NewLine + "Проверьте соединение.";
                ShowError = true;
            }

            CreditCardAlohaIntegration.CreditCardOperationComplited(mOperType, ShowError, res == "000", resStr, Receipt);
        }

        public void RunPaymentAsinc(decimal Summ, Check AlohaChk)
        {
            Amount = (int)Summ;
            mOperType = CreditCardOperationType.Payment;
            if (!OperInProcess)
            {
                Utils.ToCardLog("SBRF Запускаю поток для  RunOper");
                OperInProcess = true;
                Thread TrPosXThread = new Thread(RunOper);
                TrPosXThread.Name = "Поток для RunOper";
                TrPosXThread.Start();
            }
            else
            {
                RunOperationAsincComplitedVoid("-2", "Поток для  RunOper уже запущен. Выхожу. ", "", "");
                Utils.ToCardLog("SBRF Поток для  RunOper уже запущен. Выхожу.");
            }
        }

        public void RunVozvrAsinc(decimal Summ)
        {
            Amount = (int)Summ;
            mOperType = CreditCardOperationType.VoidPayment;
            if (!OperInProcess)
            {
                Utils.ToCardLog("SBRF Запускаю поток для  RunOper");
                OperInProcess = true;
                Thread TrPosXThread = new Thread(RunOper);
                TrPosXThread.Name = "Поток для RunOper";
                TrPosXThread.Start();
            }
            else
            {
                RunOperationAsincComplitedVoid("-2", "Поток для  RunOper уже запущен. Выхожу. ", "", "");
                Utils.ToCardLog("SBRF Поток для  RunOper уже запущен. Выхожу.");
            }
        }

        public void RunCassirMenuAsinc()
        {
            throw new NotImplementedException();
        }

        public void RunXRepAsinc()
        {
            Amount = 0;
            mOperType = CreditCardOperationType.XReport;
            if (!OperInProcess)
            {
                Utils.ToCardLog("SBRF Запускаю поток для  RunOper");
                OperInProcess = true;
                Thread TrPosXThread = new Thread(RunOper);
                TrPosXThread.Name = "Поток для RunOper";
                TrPosXThread.Start();
            }
            else
            {
                RunOperationAsincComplitedVoid("-2", "Поток для  RunOper уже запущен. Выхожу. ", "", "");
                Utils.ToCardLog("Arcus Поток для  RunOper уже запущен. Выхожу.");
            }
        }

        public string RunSVERKARepSinc(out string mReciept, out string mRes)
        {
            Amount = 0;
            mOperType = CreditCardOperationType.Sverka;
            Sinc = true;
            RunOper();
            Sinc = false;
            mReciept = Receipt;
            mRes = resOper;
            return RespCode;
        }


        public void RunDetaleRepAsinc()
        {
            Amount = 0;
            mOperType = CreditCardOperationType.LongReport;
            if (!OperInProcess)
            {
                Utils.ToCardLog("v Запускаю поток для  RunOper");
                OperInProcess = true;
                Thread TrPosXThread = new Thread(RunOper);
                TrPosXThread.Name = "Поток для RunOper";
                TrPosXThread.Start();
            }
            else
            {
                RunOperationAsincComplitedVoid("-2", "Поток для  RunOper уже запущен. Выхожу. ", "", "");
                Utils.ToCardLog("SBRF Поток для  RunOper уже запущен. Выхожу.");
            }
        }


        public void RunLastChkAsinc()
        {
            Amount = 0;
            mOperType = CreditCardOperationType.LastChk;
            if (!OperInProcess)
            {
                Utils.ToCardLog("SBRF Запускаю поток для  RunOper");
                OperInProcess = true;
                Thread TrPosXThread = new Thread(RunOper);
                TrPosXThread.Name = "Поток для RunOper";
                TrPosXThread.Start();
            }
            else
            {
                RunOperationAsincComplitedVoid("-2", "Поток для  RunOper уже запущен. Выхожу. ", "", "");
                Utils.ToCardLog("SBRF Поток для  RunOper уже запущен. Выхожу.");
            }
        }


        public void TestPinPad()
        {
            OperInProcess = true;
            SBRFSRV.Server SBSrv = new SBRFSRV.Server();
            SBSrv.Clear();
            int res = SBSrv.NFun(13);
            string ResStr = "";
            if (res == 0)
            {
                ResStr = "Пинпад готов к работе";
            }
            else
            {
                ResStr = "Ошибка соединения с пинпадом. Номер ошибки "+res.ToString();
            }
            OperInProcess = false;
            Receipt = "";
            CreditCardAlohaIntegration.CreditCardOperationComplited(CreditCardOperationType.TestPinPad, true, res == 0, ResStr, Receipt);
        }
    }
}
