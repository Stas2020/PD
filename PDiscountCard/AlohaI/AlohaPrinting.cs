using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using AlohaFOHLib;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace PDiscountCard.InterceptPrinting
{

    [Guid("ED72405B-94A6-4F71-9A80-D6D16C611D9F")]
    [ComVisible(true)]
    public class PDiscountPrintingIntercept : IInterceptAlohaPrinting
    {
        static PDiscountPrintingIntercept()
        { 
        }

        public PDiscountPrintingIntercept ()
        {}

        [MethodImpl(MethodImplOptions.NoInlining)]
        public string PrintXML(int FOHDocumentType, string xmlIn)
        {
            
            Utils.ToLog("PrintXML in: ");
            Utils.ToLog(xmlIn);
            Utils.ToLog("PrintXML in end: ");
            
            if ((FOHDocumentType == 2) || (FOHDocumentType == 3))
            {
                    AlohaPrintUtils.SendEventPrintPredCheck(xmlIn);
                //return xmlIn;
            }

            if (!iniFile.InterceptPrint)
            {
                return xmlIn;
            }

            if ((FOHDocumentType == 2) || (FOHDocumentType == 3))
            {
                
                string xmlOut = AlohaPrintUtils.PrintCheckByMe(xmlIn);
            
            
            
                Utils.ToLog("PrintXML out: ");
                Utils.ToLog(xmlOut);
                Utils.ToLog("PrintXML out end: ");
            
                return xmlOut;
                //return xmlIn;
            }
            else
            {
                Utils.ToLog("XML out the same ");
                return xmlIn;
            }
        }
        #region Component Category Registration
        [ComRegisterFunction()]
        public static void Reg(String regKey)
        {
            Microsoft.Win32.Registry.ClassesRoot.CreateSubKey(regKey.Substring(18) + "\\Implemented Categories\\" + "{D0579B21-8915-4851-B99F-798F51E2A3BB}");
        }

        [ComUnregisterFunction()]
        public static void Unreg(String regKey)
        {
            try
            {
                Microsoft.Win32.Registry.ClassesRoot.DeleteSubKey(regKey.Substring(18) + "\\Implemented Categories\\" + "{D0579B21-8915-4851-B99F-798F51E2A3BB}");
            }
            catch (Exception)
            {
                // this can happen if the subkey is not present...
                // as in the case where unreg is called multiple times
            }
        }
        #endregion
    
    }

    

    static public class AlohaPrintUtils
    {
        static private List<int> PrintedChecks = new List<int>();
        static private string  GetInnerString(string xmlIn)
        {
            try
            {
                int StartPos = xmlIn.IndexOf(@"<STOPJOURNAL />");
                if (StartPos==-1)
                {
                    StartPos = xmlIn.IndexOf(@"<STOPJOURNAL/>");
                }
                int StopPos = xmlIn.IndexOf(@"<STARTJOURNAL />");
                int STARTJOURNALL = 16;
                if (StopPos == -1)
                {
                    StopPos = xmlIn.IndexOf(@"<STARTJOURNAL/>");
                    STARTJOURNALL = 15;
                }
                string StrOut = xmlIn.Substring(StartPos, StopPos - StartPos + STARTJOURNALL);
                return StrOut;
            }
            catch
            {
                return "";
            }
 
        }
        public static string  PrintCheckByMe(string xmlIn)
        {
            double AllSumm = 0;
            XmlDocument Xd = new XmlDocument();
            Xd.LoadXml(xmlIn);
            XmlNodeList Xn = Xd.GetElementsByTagName("CHECKID");
            string Num = Xn[0].FirstChild.Value;
            XmlNodeList Xp = Xd.GetElementsByTagName("PRINTER");
            string PrNum = Xp[0].FirstChild.Value;
            int CheckId = Convert.ToInt32(Num);
            string innerString = GetInnerString(xmlIn);
            bool Closed = xmlIn.Contains("Check Closed") || xmlIn.Contains("Чек закрыт");

            
            
            if (!PrintedChecks.Contains(CheckId))
            {
                if ((!Closed))
                {
                    GuestCount.GuestCount.SetGuestCount(CheckId);
                }

                PrintedChecks.Clear();
                List<int> AlreadyPrintedCheck = new List<int>();
                AlohaTSClass.CheckWindow(); 
                List<int> Chks = AlohaTSClass.GetChecksIdInThisTable(CheckId, out AlreadyPrintedCheck);
                if (Chks.Count > 1)
                {
                    MessageForm ManyChecksMsg = new MessageForm("Печатать все чеки на столе?");
                    ManyChecksMsg.button1.Text = "Да";
                    ManyChecksMsg.button2.Text = "Нет";
                   // ManyChecksMsg.button2.Visible = false;
                    ManyChecksMsg.ShowDialog();

                    if (ManyChecksMsg.Result == 1)
                    {
                        if (AlreadyPrintedCheck.Count > 0)
                        {
                            string Mess = "";
                            foreach(int nn in AlreadyPrintedCheck )
                            {
                                Mess += "Предчек #" +AlohaTSClass.GetCheckById (nn).CheckShortNum   +" уже напечатан. " +Environment.NewLine; 
                            }
                            AlohaTSClass.ShowMessage(Mess); 
                        }
                        foreach (int ChId in Chks)
                        {
                            PrintedChecks.Add(ChId);
                            AlohaTSClass.PrintPredCheckByXml(ChId, Convert.ToInt32(PrNum));
                            try
                            {
                                AlohaTSClass.PrintPredCheck(ChId,true); //Это чтобы чек считался распечатанным в алохе.
                            }
                            catch
                            { }
                        }
                        AlohaTSClass.PrintAllPredchecksById(CheckId, Convert.ToInt32 (PrNum), Chks); 
                        return "";
                    }
                }
                else
                {
                    PrintedChecks.Remove(CheckId);
                }


            }
            else
            {
                PrintedChecks.Remove(CheckId);
                return "";
            }
            
       
            return AlohaTSClass.GetPrintPredcheckById(CheckId, Convert.ToInt32(PrNum ),false ,null,innerString ,Closed);
                
        }

       

        public static void  SendEventPrintPredCheck(string xmlIn)
        {
            XmlDocument Xd = new XmlDocument();
            Xd.LoadXml(xmlIn);
            XmlNodeList Xn = Xd.GetElementsByTagName("CHECKID");
            string Num = Xn[0].FirstChild.Value;
            XmlNodeList Xp = Xd.GetElementsByTagName("PRINTER");
            EventSenderClass.SendAlohaAsincEvent(PDiscountCard.StopListService.AlohaEventType.PrintPredCheck ,"",0,0,"",0,0,Convert.ToInt32(Num));
            //return AlohaTSClass.GetPrintPredcheckById(Convert.ToInt32(Num), Convert.ToInt32(PrNum));

        }
    }

}
