
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;
using ARCCOMLib; 

namespace PDiscountCard
{
  
    static internal class ArcusClass
    {
        static string ChequeFilePath = "";
        static string CodeFilePath = "";
        static public  void Init()
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
            }
            catch(Exception e)
            {
                Utils.ToCardLog("[Error] ArcusClass.Init() "+ e.Message); 
            }
        }

        
        static string  RespCode = "00";
        static string Receipt = "";
     


        static private string ReplaceDemicalSeparator(string s)
        {
            double  d = 1.1;
            Char  DemSep = d.ToString()[1];
            return s.Replace("."[0], DemSep).Replace(","[0], DemSep);
        
        }

        static private decimal GetSummFromSlip(List<string> Slip)
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

        static private  void RunOper()
        {
            try
            {
                Utils.ToCardLog("Запуск RunOper OperType=" + OperType.ToString() + " Amount= "+Amount.ToString ()); 
                OperInProcess = true;
                SAPacketObj Request = new SAPacketObj();
                SAPacketObj Response = new SAPacketObj();
                PCPOSTConnectorObj Conn = new PCPOSTConnectorObj();
                Request.Amount = (Amount * 100).ToString();
                Request.OperationCode = OperType;
                Request.CurrencyCode = "643";
                Request.DateTimeHost = DateTime.Now.ToString("yyyyMMddHHmmss");
                
                //Conn.InitResources();
                Conn.Exchange(ref  Request, ref  Response, 10);
                
                RespCode = Response.ResponseCodeHost.Trim ();
                
                Utils.ToCardLog("[RunOper] Операция выполнена. Результат: " + RespCode);
                Receipt = ReadChequeFile();
                if ((RespCode == "00") || (RespCode == "000"))
                {
                    if ((OperType == 1) || (OperType == 4))
                    {
                        string Tmp = Receipt;
                        Tmp += Convert.ToChar(31) + Environment.NewLine;
                        Tmp += Receipt;
                        Receipt = Tmp;

                        try
                        {
                            Utils.ToCardLog("[RunOper] Добавляю Слип в Файл.");

                            ArcusSlips AS = ArcusAlohaIntegrator.ReadArcusSlips();
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
                                Num = Response.TrxIDCRM

                            };
                           
                            string[] Str = Receipt.Split(char.ConvertFromUtf32(10)[0]);

                            foreach (string str in Str)
                            {
                               
                                Utils.ToCardLog(str);
                                S.Slip.Add(str);
                            }

                            decimal SummFromTxt = GetSummFromSlip(S.Slip);

                            if ((SummFromTxt != 0) && (S.Sum != SummFromTxt*100))
                            {
                                Utils.ToCardLog("Error Разные суммы в слипе " + SummFromTxt.ToString() + " и ответе от терминала. " + S.Sum.ToString ());
                                S.Sum = SummFromTxt*100;
                            }

                            AS.Slips.Add(S);

                            ArcusAlohaIntegrator.WriteArcusSlips(AS);

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
                resOper =GetCodeDescr(RespCode);
                if (!Sinc)
                {
                    RunOperationAsincComplitedVoid(RespCode, resOper, "", Receipt);
                }
                OperInProcess = false;
                Utils.ToCardLog("Отработал RunOper OperType=" + OperType.ToString() + " Amount= " + Amount.ToString()); 
            }
            catch(Exception e)
            {
                resOper = "Ошибка программы. "+e.Message ;
                RespCode = "-1";
                if (!Sinc)
                {
                    RunOperationAsincComplitedVoid(RespCode, resOper, "", "");
                }
                Utils.ToCardLog("[Error] Запускa RunOper " + e.Message ); 
            }
        }

        static string resOper = "";
       
        static string  ReadChequeFile()
        {
            string Tmp = "";
            using (StreamReader SW = new StreamReader(ChequeFilePath, Encoding.GetEncoding(1251)))
            {
                while (!SW.EndOfStream)
                {
                    Tmp += SW.ReadLine()+Environment.NewLine;
                }
                SW.Close(); 
            }
            return Tmp;
        
        }
        
      public   static string GetCodeDescr(string  Code)
        {
            string Tmp = "";
            using (StreamReader SW = new StreamReader(CodeFilePath, Encoding.GetEncoding (1251)))
            {
                while (!SW.EndOfStream)
                {
                    Tmp = SW.ReadLine();
                    try
                    { 
                        int i = Convert.ToInt32(Tmp.Substring (0,3));
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

        static  decimal Amount;
        static int OperType;
        static  bool OperInProcess = false;
        public static void RunPaymentAsinc(decimal _Amount)
        {
            Amount = _Amount;
            OperType = 1;
            if (!OperInProcess)
            {
                Utils.ToCardLog("Arcus Запускаю поток для  RunOper");
                OperInProcess = true;
                Thread TrPosXThread = new Thread(RunOper);
                TrPosXThread.Name = "Поток для RunOper";
                TrPosXThread.Start();
                
            }
            else
            {
                RunOperationAsincComplitedVoid("-2", "Поток для  RunOper уже запущен. Выхожу. " , "", "");
                Utils.ToCardLog("Arcus Поток для  RunOper уже запущен. Выхожу.");
            }
        }
        public static void RunVozvrAsinc(decimal _Amount)
        {
            Amount = Math.Abs (_Amount);
            OperType = 4;
            if (!OperInProcess)
            {
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
        public static void RunXRepAsinc()
        {
            Amount = 0;
            OperType = 6;
            if (!OperInProcess)
            {
                OperInProcess = true;
                Thread TrPosXThread = new Thread(RunOper);
                TrPosXThread.Name = "Поток для RunOper";
                TrPosXThread.Start();
            }
        }

        public static void RunSVERKARepAsinc()
        {
            Amount = 0;
            OperType = 7;
            if (!OperInProcess)
            {
                OperInProcess = true;
                Thread TrPosXThread = new Thread(RunOper);
                TrPosXThread.Name = "Поток для RunOper";
                TrPosXThread.Start();
                
            }
        }
       static bool Sinc = false;
       public static string  RunSVERKARepSinc(out string mReciept, out string mRes)
        {
            Amount = 0;
            OperType = 7;
            Sinc = true;
            RunOper();
            Sinc = false;
            mReciept = Receipt;
            mRes = resOper;
            if ((RespCode == "00") || (RespCode == "000"))
            {
                SlipFileMove();
            }
            return RespCode;
        }

        public static void GetSipCopy(int Num)
        {
            ArcusSlip S =  GetSlip(Num);
            if (S == null)
            {
                RunOperationAsincComplitedVoid("0", "Неверный номер слипа", "", "");
            }
            else
            {
                string strslip = "Копия " + Environment.NewLine;
                 foreach (string s in S.Slip)
                {
                    if (s == "")
                    {
                        strslip += " " + Environment.NewLine;
                    }
                    else
                    {
                        strslip += s + Environment.NewLine;
                    }
                }

                RunOperationAsincComplitedVoid("0", "Успешно", "", strslip);
            }


        }

      static  private ArcusSlip GetSlip(int Num)
        {
            ArcusSlips AS = ArcusAlohaIntegrator.ReadArcusSlips();
            foreach (ArcusSlip S in AS.Slips )
            {
                if (S.Num == Num)
                {
                    return S;
                }
            }
            return null;
        }

        public delegate void RunOperationAsincDelegate(string  res, string resStr, string Response, string Receipt);

        public static event RunOperationAsincDelegate RunOperationAsincComplited;

        static private void RunOperationAsincComplitedVoid(string  res, string resStr, string Response, string Receipt)
        {

            if (RunOperationAsincComplited != null)
            {
                RunOperationAsincComplited(res, resStr, Response, Receipt);
            }
        }

        

        static internal void SlipFileMove()
        {
            try
            {
                string SlipFilePath = iniFile.ArcusFilesPath + "slips.xml";
                string NewSlipFilePathFolderPath = Utils.FilesPath + @"ArcusSlips\";
                string NewSlipFileName = DateTime.Now.ToString("ddMMyyHHmmss") + "slips.xml";
                if (File.Exists(SlipFilePath))
                {
                    if (!Directory.Exists(NewSlipFilePathFolderPath))
                    {
                        Directory.CreateDirectory(NewSlipFilePathFolderPath);
                    }
                    File.Move(SlipFilePath, NewSlipFilePathFolderPath + NewSlipFileName);
                    Utils.ToLog("Файл Slips.xml перенесен.");
                }

            }
            catch (Exception e)
            {
                Utils.ToLog("[Error]Файл Slips.xml не перенесен. "+ e.Message);
            }
        }

            }

}
