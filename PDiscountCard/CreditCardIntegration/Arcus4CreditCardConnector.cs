﻿
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;



namespace PDiscountCard.CreditCardIntegration
{

    internal class Arcus4CreditCardConnector : ICreditCardConnector, ICreditCardConnector2
    {
        
        
        string ChequeFilePath = "";
        string CodeFilePath = "";

        public Arcus4CreditCardConnector()
        {
            try
            {
                Utils.ToCardLog("ArcusClass4.Init() ");
                ChequeFilePath = iniFile.ArcusChequeFilesPath;
                CodeFilePath = iniFile.ArcusCodeFilePath;
                
                Utils.ToCardLog("ArcusClass.Init() End");
                Inited = true;
            }
            catch (Exception e)
            {
                Utils.ToCardLog("[Error] ArcusClass4.Init() " + e.Message);
            }
        }

        string RespCode = "00";
        string Receipt = "";


        private string ReplaceDemicalSeparator(string s)
        {
            double d = 1.1;
            Char DemSep = d.ToString()[1];
            return s.Replace("."[0], DemSep).Replace(","[0], DemSep);

        }

        private decimal GetSummFromSlip(List<string> Slip)
        {
            foreach (string str in Slip)
            {
                if (str.Contains("СУММА:"))
                {
                    try
                    {
                        string SummStr = str.Substring(7).TrimStart(" "[0]);
                        int stop = SummStr.IndexOf(" "[0]);
                        decimal Summ = Convert.ToDecimal(ReplaceDemicalSeparator(SummStr.Substring(0, stop + 1)));
                        return Summ;
                    }
                    catch
                    { }
                }

            }
            return 0;
        }
        Arcus3Wrapper.ArcusOp OperType;
        private void RunOper()
        {
            try
            {
                Utils.ToCardLog("Запуск RunOper OperType=" + OperType.ToString() + " Amount= " + Amount.ToString());
                OperInProcess = true;
                var arc = new Arcus3Wrapper();
                var request = new ArcusRequest()
                {
                    Amount = (int)Amount,
                    OriginalDate = DateTime.Now,
                    Currency = "643",

                };
                var response = new ArcusResponse();


                Utils.ToCardLog(String.Format("[RunOper] Запрос операции. Входящий запрос: " + request.ToString()));
                int res = arc.CallArcusOper(OperType, request, response);
                
                
       
                
                RespCode = response.Response_code.Trim();

                Utils.ToCardLog(String.Format("[RunOper] Операция выполнена res: " + res + "; Результат:  " + response.ToString()));
                
                if (response!=null )


                Receipt = ReadChequeFile();
               
                resOper = GetCodeDescr(RespCode);
                Utils.ToCardLog(String.Format("[RunOper] Sinc: " + Sinc + " RespCode: " + RespCode));
                


                    if (!Sinc)
                {
                    RunOperationAsincComplitedVoid(OperType, RespCode.Trim(), resOper,  Receipt, response.Pan, response.Rnn);
                }
                OperInProcess = false;
                Utils.ToCardLog("Отработал RunOper OperType=" + OperType.ToString() + " Amount= " + Amount.ToString());
            }
            catch (Exception e)
            {
                resOper = "Ошибка программы. " + e.Message;
                RespCode = "-1";
                if (!Sinc)
                {
                    RunOperationAsincComplitedVoid(OperType, RespCode, resOper,  "");
                }
                Utils.ToCardLog("[Error] Запускa RunOper " + e.Message);
            }
        }




        string resOper = "";

        string ReadChequeFile()
        {
            Utils.ToCardLog("ReadChequeFile path " + ChequeFilePath);
            string Tmp = "";
            try
            {
                using (StreamReader SW = new StreamReader(ChequeFilePath, Encoding.GetEncoding(1251)))
                {
                    while (!SW.EndOfStream)
                    {
                        Tmp += SW.ReadLine() + Environment.NewLine;
                    }
                    SW.Close();
                }
            }
            catch(Exception e)
            {
                Utils.ToCardLog("[Error] ReadChequeFile "+e.Message);
            }
            Utils.ToCardLog("ReadChequeFile complited "+Environment.NewLine + Tmp);
            return Tmp;
        }

        public string GetCodeDescr(string Code)
        {
            string Tmp = "";
            using (StreamReader SW = new StreamReader(CodeFilePath, Encoding.GetEncoding(1251)))
            {
                while (!SW.EndOfStream)
                {
                    Tmp = SW.ReadLine();
                    try
                    {
                        int i = Convert.ToInt32(Tmp.Substring(0, 3));
                        if (i == Convert.ToInt32(Code))
                        {
                            return Tmp.Substring(4, Tmp.Length - 4);
                        }

                    }
                    catch
                    {

                    }
                }
                SW.Close();
            }
            return "Нет кода ответа";
        }

        decimal Amount;
        string AlohaCheckShortNum = "";
        int AlohaCheckId = 0;
        
        //internal bool OperInProcess = false;
        public void RunPaymentAsinc(decimal _Amount, Check AlohaChk)
        {
            Amount = _Amount;
            AlohaCheckShortNum = AlohaChk.CheckShortNum;
            AlohaCheckId = AlohaChk.AlohaCheckNum;
            OperType = Arcus3Wrapper.ArcusOp.Pay;
            
            if (!OperInProcess)
            {

                Utils.ToCardLog("Arcus4 Запускаю поток для  RunOper");
                mOperType = CreditCardOperationType.Payment;
                OperInProcess = true;
                Thread TrPosXThread = new Thread(RunOper);
                TrPosXThread.Name = "Поток для RunOper";
                TrPosXThread.Start();

            }
            else
            {
                RunOperationAsincComplitedVoid(0, "-2", "Поток для  RunOper уже запущен. Выхожу. ",  "");
                Utils.ToCardLog("Arcus Поток для  RunOper уже запущен. Выхожу.");
            }
        }
        public void RunVozvrAsinc(decimal _Amount)
        {
            Amount = Math.Abs(_Amount);
            OperType = Arcus3Wrapper.ArcusOp.Refund;
            if (!OperInProcess)
            {
                mOperType = CreditCardOperationType.VoidPayment;
                OperInProcess = true;
                Thread TrPosXThread = new Thread(RunOper);
                TrPosXThread.Name = "Поток для RunOper";
                TrPosXThread.Start();
            }
            else
            {
                RunOperationAsincComplitedVoid(0, "-2", "Поток для  RunOper уже запущен. Выхожу. ",  "");
                Utils.ToCardLog("Arcus Поток для  RunOper уже запущен. Выхожу.");
            }
        }


        public void RunCassirMenuAsinc()
        {
            return;
            Amount = 0;
            OperType = Arcus3Wrapper.ArcusOp.XReportShort;
            if (!OperInProcess)
            {
                OperInProcess = true;
                mOperType = CreditCardOperationType.CassirMenu;

                Thread TrPosXThread = new Thread(RunOper);
                TrPosXThread.Name = "Поток для RunOper";
                TrPosXThread.Start();
            }
        }

        public void RunXRepAsinc()
        {
            /*
            string s = Arcus2DataFromXML.PrintShortReport();
            CreditCardAlohaIntegration.CreditCardOperationComplited(CreditCardOperationType.XReport, false, true , "", s);
             * */


            Amount = 0;
            OperType = Arcus3Wrapper.ArcusOp.XReportShort;
            if (!OperInProcess)
            {
                OperInProcess = true;
                mOperType = CreditCardOperationType.XReport;
                Thread TrPosXThread = new Thread(RunOper);
                TrPosXThread.Name = "Поток для RunOper";
                TrPosXThread.Start();
            }

        }

        public void RunSVERKARepAsinc()
        {
            Amount = 0;
            OperType = Arcus3Wrapper.ArcusOp.ZReport;
            if (!OperInProcess)
            {
                mOperType = CreditCardOperationType.Sverka;
                OperInProcess = true;
                Thread TrPosXThread = new Thread(RunOper);
                TrPosXThread.Name = "Поток для RunOper";
                TrPosXThread.Start();

            }
        }
        bool Sinc = false;
        public string RunSVERKARepSinc(out string mReciept, out string mRes)
        {
            Amount = 0;
            OperType = Arcus3Wrapper.ArcusOp.ZReport;
            mOperType = CreditCardOperationType.Sverka;
            Sinc = true;
            RunOper();
            Sinc = false;
            mReciept = Receipt;
            mRes = resOper;
            if ((RespCode == "00") || (RespCode == "000"))
            {
                Arcus2DataFromXML.SlipFileMove();
            }
            return RespCode;
        }



        //  public delegate void RunOperationAsincDelegate(int OperType, string res, string resStr, string Response, string Receipt);

        //public event RunOperationAsincDelegate RunOperationAsincComplited;

        CreditCardOperationType mOperType;
        private void RunOperationAsincComplitedVoid(Arcus3Wrapper.ArcusOp OperType, string respCode, string resStr, string receipt, string bins = "", string RRN = "")
        {
            Utils.ToCardLog(String.Format("[RunOper] RunOperationAsincComplitedVoid " + OperType.ToString() + "; RespCode=" + respCode));

            int respCodeInt = 0;
            var tryParseRes =  int.TryParse(respCode, out respCodeInt);
            Utils.ToCardLog("tryParseRes " + tryParseRes + " respCodeInt: " + respCodeInt);

            bool ShowError = false;
            
            if (OperType==Arcus3Wrapper.ArcusOp.XReportShort)
            {
                mOperType = CreditCardOperationType.XReport;
            }
            else if (OperType == Arcus3Wrapper.ArcusOp.ZReport)
            {
                mOperType = CreditCardOperationType.Sverka;
            }
            else if (OperType == Arcus3Wrapper.ArcusOp.XReportFull)
            {
                mOperType = CreditCardOperationType.LongReport;
            }
            else if (OperType == Arcus3Wrapper.ArcusOp.Pay)
            {
                mOperType = CreditCardOperationType.Payment;
            }
            else if (OperType == Arcus3Wrapper.ArcusOp.Cancel)
            {
                mOperType = CreditCardOperationType.VoidPayment;
            }
            else if (OperType == Arcus3Wrapper.ArcusOp.Refund)
            {
                mOperType = CreditCardOperationType.VoidPayment;
            }
            bool res = false;

            if(iniFile.Arcus4ParseReceiptAutoCancel && receipt != null)
            {
                var cancelMarkers = new List<string>() { "ОТВЕТ НЕ ПОЛУЧЕН", "ОПЕРАЦИЯ АВТОМАТИЧЕСКИ", "ОТМЕНЕНА" };
                if (cancelMarkers.TrueForAll(_marker => receipt.ToUpper().IndexOf(_marker) != -1))
                {
                    Utils.ToCardLog("Arcus4ParseReceiptAutoCancel Выявлена автоотмена по парсингу чека. Стол не закрыт.");
                    res = false;
                    respCode = "999";
                }
            }

            if (respCode == "999")
            {
                resStr += Environment.NewLine + "Нет связи с терминалом пластиковых карт.";
                resStr += Environment.NewLine + "Проверьте соединение.";
                ShowError = true;
            }
            else
            {
                res = (respCodeInt==0) && (!receipt.ToUpper().Contains("НЕ ОПЛАЧИВАТЬ"));
            }
            CreditCardAlohaIntegration.CreditCardOperationComplited(mOperType, ShowError, res, resStr,  receipt,bins ,RRN );
        }

        bool _OperInProcess;
       public  bool OperInProcess
        {
            get
            {
                return _OperInProcess;
            }
            set
            {
                _OperInProcess = value;
            }
        }



       bool _Inited = false;
       public bool Inited
       {
           get
           {
               return _Inited;
           }
           set
           {
               _Inited = value;
           }
       }


       public void RunDetaleRepAsinc()
       {
           Amount = 0;
           OperType = Arcus3Wrapper.ArcusOp.XReportFull;
           if (!OperInProcess)
           {
               OperInProcess = true;
               mOperType = CreditCardOperationType.LongReport;
               Thread TrPosXThread = new Thread(RunOper);
               TrPosXThread.Name = "Поток для RunOper";
               TrPosXThread.Start();
           }

       }


       public void RunLastChkAsinc()
       {
           Amount = 0;
           OperType = Arcus3Wrapper.ArcusOp.LastSlipCopy;
           if (!OperInProcess)
           {
               mOperType = CreditCardOperationType.Sverka;
               OperInProcess = true;
               Thread TrPosXThread = new Thread(RunOper);
               TrPosXThread.Name = "Поток для RunOper";
               TrPosXThread.Start();

           }
       }


       public void TestPinPad()
       {
           throw new NotImplementedException();
       }


       public void RunGetSlipCopyAsinc()
       {
           Amount = 0;
           OperType = Arcus3Wrapper.ArcusOp.SlipCopy;
           if (!OperInProcess)
           {
               mOperType = CreditCardOperationType.Sverka;
               OperInProcess = true;
               Thread TrPosXThread = new Thread(RunOper);
               TrPosXThread.Name = "Поток для RunOper";
               TrPosXThread.Start();

           }
       }
    }


}
