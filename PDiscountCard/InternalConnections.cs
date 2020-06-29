using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace PDiscountCard
{
   public  class InternalConnections
    {
      static  internal int CheckId;
      static internal int TermId;
      static internal int EmplId;
      static internal int TableId;

       public InternalConnections()
       {
           try
           {
               Utils.ToLog("InternalConnections() ");
               AlohaTSClass.InitAlohaCom();
               //MainClass.InitAssignLoyaltyTimer();
               MainClass.WHThreadLoyaltyCard = new AutoResetEvent(false);
               MainClass.ThreadLoyaltyCard = new System.Threading.Thread(MainClass.AssignLoyaltiStateThread);
               MainClass.ThreadLoyaltyCard.IsBackground = true;
               MainClass.ThreadLoyaltyCard.Start();
               ToBase.FirstInit();
               AlohaTSClass.SendMessageEvent += new AlohaTSClass.SendMessageEventHandler(AlohaTSClass_SendMessageEvent);
           }
           catch(Exception e)
           {
               Utils.ToLog("[Error] InternalConnections() " + e.Message);
           }
       }

       public static void PrindChkToPrinter(int ChId,int Printer,int Empl)
       {
           try
           {
               Utils.ToLog("PrindChkToPrinter() ");
               AlohaTSClass.InitAlohaCom();
               if (AlohaTSClass.ChkCanPrint(ChId, Empl))
               {
                   AlohaTSClass.PrintPredCheckByXml(ChId, Printer);
                   AlohaTSClass.SetPrintChkAttr(ChId, 1);
                   //AlohaTSClass.PrintPredCheck(ChId, true);
               }
               else
               {
                   throw new Exception("Чек уже напечатан");
               }
           }
           catch (Exception e)
           {
               Utils.ToLog("[Error] PrindChkToPrinter() " + e.Message);
           }
       }

       public static bool ChkAlreadyPrented(int ChId, int Empl)
       {
           try
           {
               AlohaTSClass.InitAlohaCom();
               return !AlohaTSClass.ChkCanPrint(ChId, Empl);
           }
           catch (Exception e)
           {
               Utils.ToLog("[Error] ChkAlreadyPrented() " + e.Message);
               return false;
           }
       }

       public static string GetChkToPrinterXml(int ChId,int Empl)
       {
           try
           {
               AlohaTSClass.InitAlohaCom();
               return AlohaTSClass.GetPrintPredcheckById(ChId, 0, false, null, "", false);
           }
           catch (Exception e)
           {
               Utils.ToLog("[Error] GetChkToPrinterXml() " + e.Message);
               return "";
           }
       }

       public static void SetAlreadyPrindChk(int ChId)
       {
           try
           {
               AlohaTSClass.InitAlohaCom();
               AlohaTSClass.SetPrintChkAttr(ChId, 1);
           }
           catch (Exception e)
           {
               Utils.ToLog("[Error] SetAlreadyPrindChk() " + e.Message);
               
           }
       }


       void AlohaTSClass_SendMessageEvent(string Mess)
       {
           SendMessageEventToUser(Mess);
       }

       public int RegCardInternal(string Track1, string Track2, string Track3, int mCheckId, int mTermId, int mEmplId, int mTableId, out string ResMsg)
       {
           ResMsg = "";
           try
           {
              
               CheckId = mCheckId;
               TermId = mTermId;
               EmplId = mEmplId;
               TableId = mTableId;
               AlohaTSClass.GetCurentStateFromArgs = true;
               return MainClass.RegCard(Track1, Track2, Track3);
           }
           catch (Exception e)
           {
               Utils.ToLog("[Error] RegCardInternal() " + e.Message);
               ResMsg = e.Message;
               return -1;
           }

       }

       public delegate void SendMessageEventHandler(string Mess);
       public event SendMessageEventHandler SendMessageEvent;


       private void SendMessageEventToUser(string Message)
       {
           if (SendMessageEvent != null)
           {
               SendMessageEvent(Message);
           }
       }
    }
}
