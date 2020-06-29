
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;

using ARCCOMLib;

namespace PDiscountCard
{

    internal class Arcus2CreditCardConnector : ICreditCardConnector
    {
        string ChequeFilePath = "";
        string CodeFilePath = "";

        public Arcus2CreditCardConnector()
        {
            try
            {
                Utils.ToCardLog("ArcusClass.Init() ");
                ChequeFilePath = iniFile.ArcusChequeFilesPath;
                CodeFilePath = iniFile.ArcusCodeFilePath;
                SAPacketObj Request = new SAPacketObj();
                SAPacketObj Response = new SAPacketObj();
                PCPOSTConnectorObj Conn = new PCPOSTConnectorObj();
                Utils.ToCardLog("ArcusClass.Init() End");
                Inited = true;
            }
            catch (Exception e)
            {
                Utils.ToCardLog("[Error] ArcusClass.Init() " + e.Message);
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

        private void RunOper()
        {
            try
            {
                Utils.ToCardLog("Запуск RunOper OperType=" + OperType.ToString() + " Amount= " + Amount.ToString());
                OperInProcess = true;
                SAPacketObj Request = new SAPacketObj();
                SAPacketObj Response = new SAPacketObj();
                PCPOSTConnectorObj Conn = new PCPOSTConnectorObj();
                Request.Amount = (Amount).ToString();
                Request.OperationCode = OperType;
                Request.CurrencyCode = "643";
                Request.DateTimeHost = DateTime.Now.ToString("yyyyMMddHHmmss");
                
                //Conn.InitResources();
                Conn.Exchange(ref  Request, ref  Response, 10);

                RespCode = Response.ResponseCodeHost.Trim();

                Utils.ToCardLog(String.Format("[RunOper] Операция выполнена. Результат: {0}, TrxIDCRM {1}, TrxID {2}, ReferenceNumber {3}", RespCode, Response.TrxIDCRM, Response.TrxID, Response.ReferenceNumber));
                try
                {
                    Utils.ToCardLog(String.Format("[RunOper] Операция выполнена. Входящий запрос: {0}, TrxIDCRM {1}, TrxID {2}, ReferenceNumber {3}", RespCode, Request.TrxIDCRM, Request.TrxID, Request.ReferenceNumber));
                }
                catch
                { 
                }
                Receipt = ReadChequeFile();
                if (((RespCode == "00") || (RespCode == "000")) && (!Receipt.ToUpper().Contains("НЕ ОПЛАЧИВАТЬ")))
                {
                    if ((OperType == 1) || (OperType == 4))
                    {
                        string Tmp = Receipt;
                        if (!iniFile.Arcus2PrintOneSlip)
                        {
                            Tmp += Convert.ToChar(31) + Environment.NewLine;
                            Tmp += Receipt;
                            Receipt = Tmp;
                        }


                        try
                        {
                            Utils.ToCardLog("[RunOper] Добавляю Слип в Файл.");

                            ArcusSlips AS = Arcus2DataFromXML.ReadArcusSlips();
                            DateTime HDT = DateTime.Now;
                            try
                            {
                                HDT = Convert.ToDateTime(Response.DateTimeCRM);
                            }
                            catch
                            { }
                            ArcusSlip S = new ArcusSlip()
                            {
                                HostDt = HDT,
                                Sum = Convert.ToDecimal(Response.Amount),
                                Void = (Response.OperationCode == 4),
                                Num = Response.TrxIDCRM,
                                RRN = Response.ReferenceNumber,
                                AlohaCheckId = AlohaCheckId,
                                AlohaCheckShortNum = AlohaCheckShortNum
                            };
                            string[] Str = Receipt.Split(char.ConvertFromUtf32(10)[0]);

                            foreach (string str in Str)
                            {
                                Utils.ToCardLog(str);
                                S.Slip.Add(str);
                            }

                            decimal SummFromTxt = GetSummFromSlip(S.Slip);

                            if ((SummFromTxt != 0) && (S.Sum != SummFromTxt * 100))
                            {
                                Utils.ToCardLog("Error Разные суммы в слипе " + SummFromTxt.ToString() + " и ответе от терминала. " + S.Sum.ToString());
                                S.Sum = SummFromTxt * 100;
                            }

                            AS.Slips.Add(S);

                            Arcus2DataFromXML.WriteArcusSlips(AS);

                            Utils.ToCardLog("[RunOper] Добавлил Слип в Файл.");
                        }
                        catch (Exception e)
                        {
                            Utils.ToCardLog("[Error] Ошибка добавления слипа в файл." + e.Message);
                        }


                    }
                }
                else
                {
                    try
                    {
                        EventSenderClass.SendAlohaAsincEvent(StopListService.AlohaEventType.ErrorCreditCardterminal, "", AlohaTSClass.AlohaCurentState.WaterId,
                           AlohaTSClass.GetJobCode(AlohaTSClass.AlohaCurentState.WaterId),
                           "",
                         Convert.ToInt32(RespCode),
                           (int)AlohaTSClass.AlohaCurentState.TableId,
                           (int)AlohaTSClass.AlohaCurentState.CheckId);
                    }
                    catch
                    { }


                }
                resOper = GetCodeDescr(RespCode);
                if (!Sinc)
                {
                    RunOperationAsincComplitedVoid(OperType, RespCode, resOper, "", Receipt);
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
                    RunOperationAsincComplitedVoid(OperType, RespCode, resOper, "", "");
                }
                Utils.ToCardLog("[Error] Запускa RunOper " + e.Message);
            }
        }




        string resOper = "";

        string ReadChequeFile()
        {
            string Tmp = "";
            using (StreamReader SW = new StreamReader(ChequeFilePath, Encoding.GetEncoding(1251)))
            {
                while (!SW.EndOfStream)
                {
                    Tmp += SW.ReadLine() + Environment.NewLine;
                }
                SW.Close();
            }
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
        int OperType;
        //internal bool OperInProcess = false;
        Thread TrPosXThread;
        public void RunPaymentAsinc(decimal _Amount, Check AlohaChk)
        {
            Amount = _Amount;
            AlohaCheckShortNum = AlohaChk.CheckShortNum;
            AlohaCheckId = AlohaChk.AlohaCheckNum;
            OperType = 1;
            
            if (!OperInProcess)
            {

                Utils.ToCardLog("Arcus2 Запускаю поток для  RunOper");
                mOperType = CreditCardOperationType.Payment;
                OperInProcess = true;
                TrPosXThread = new Thread(RunOper);
                TrPosXThread.IsBackground = true;
                
                TrPosXThread.Priority = ThreadPriority.Lowest;
                TrPosXThread.Name = "Thread RunOper";
                TrPosXThread.Start();

            }
            else
            {
                RunOperationAsincComplitedVoid(0, "-2", "Поток для  RunOper уже запущен. Выхожу. ", "", "");
                Utils.ToCardLog("Arcus Поток для  RunOper уже запущен. Выхожу.");
            }
        }
        public void RunVozvrAsinc(decimal _Amount)
        {
            Amount = Math.Abs(_Amount);
            OperType = 4;
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
                RunOperationAsincComplitedVoid(0, "-2", "Поток для  RunOper уже запущен. Выхожу. ", "", "");
                Utils.ToCardLog("Arcus Поток для  RunOper уже запущен. Выхожу.");
            }
        }


        public void RunCassirMenuAsinc()
        {
            Amount = 0;
            OperType = 13;
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
            string s = Arcus2DataFromXML.PrintShortReport();
            CreditCardAlohaIntegration.CreditCardOperationComplited(CreditCardOperationType.XReport, false, true , "", s);
        }

        public void RunSVERKARepAsinc()
        {
            Amount = 0;
            OperType = 7;
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
            OperType = 7;
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
        private void RunOperationAsincComplitedVoid(int OperType, string res, string resStr, string Response, string Receipt)
        {
            bool ShowError = false;
            
            if (OperType==6)
            {
                mOperType = CreditCardOperationType.XReport;
            }
            else if (OperType==7)
            {
                mOperType = CreditCardOperationType.Sverka;
            }
            if (res == "999")
            {
                resStr += Environment.NewLine + "Нет связи с терминалом пластиковых карт.";
                resStr += Environment.NewLine + "Проверьте соединение.";
                ShowError = true;
            }

            CreditCardAlohaIntegration.CreditCardOperationComplited(mOperType, ShowError, res == "000", resStr,  Receipt);
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
           OperType = 6;
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
           throw new NotImplementedException();
       }


       public void TestPinPad()
       {
           throw new NotImplementedException();
       }
    }

}
