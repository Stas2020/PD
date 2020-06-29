using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDiscountCard.SV
{
   public static class SVClass
    {

       public const Int32 BonusPaymentId = 26;
       public static System.Timers.Timer T1 = new System.Timers.Timer();
       public static System.Timers.Timer PaymenetTimer = new System.Timers.Timer();
       private static decimal LastSumm = 0;
       public static void StartSVPayment(decimal Summ,int CheckNum ,CardEntryParams _CurentCardEntryParams)
       {
           try
           {
               LastSumm = Summ;
               CurentCardEntryParams = _CurentCardEntryParams;
               MainClass.AddRegCardSubscr(SVSistem.Main.GetCardFromMagReader);
               SVSistem.Main.StartPayment(Summ, CheckNum,EndSVPayment);
           }
           catch(Exception e)
           {
               Utils.ToCardLog("[Error] StartSVPayment" + e.Message);
           }
       }


       public static void EndSVPayment(bool res, string CardNum, double Summ)
       {
           try
           {
               MainClass.RemoveRegCardSubscr(SVSistem.Main.GetCardFromMagReader);
               if (!res)
               {
                   AlohaTSClass.DeletePayment(CurentCardEntryParams.CheckId, CurentCardEntryParams.EntryId);
               }
               else
               {
                   if ((decimal)Summ != LastSumm)
                   {
                       AlohaTSClass.DeletePayment(CurentCardEntryParams.CheckId, CurentCardEntryParams.EntryId);
                       string ErrStr="";
                       AlohaTSClass.ApplySVCardPayment(CurentCardEntryParams.CheckId, (decimal)Summ, CardNum, out ErrStr);
                   }
                   
                  
                   
               }
           }
           catch (Exception e)
           {
               Utils.ToCardLog("[Error] EndSVSale" + e.Message);
           }
       }

      

       public static void StartSVSale(decimal Summ,int CheckNum, CardEntryParams _CurentCardEntryParams)
       {
           try
           {
               CurentCardEntryParams = _CurentCardEntryParams;

               MainClass.AddRegCardSubscr(SVSistem.Main.GetCardFromMagReader);
               SVSistem.Main.StartSale(Summ,CheckNum, EndSVSale);
        
           }
           catch(Exception e)
           {
               Utils.ToCardLog("[Error] StartSVSale" + e.Message);
           }
       }

       public static void EndSVSale(bool res, string CardNum, double Summ)
       {
           try
           {
               MainClass.RemoveRegCardSubscr(SVSistem.Main.GetCardFromMagReader);
               if (!res)
               {
                   Utils.ToCardLog("Продажа карты неуспешна. Удаляю блюдо");
                   AlohaTSClass.DeleteCardEntry(CurentCardEntryParams.CheckId, CurentCardEntryParams.EntryId);
                   /*
                   CurentTryDeleteCount = 0;
                   T1 = new System.Timers.Timer();
                   T1.Elapsed += new System.Timers.ElapsedEventHandler(T1_Elapsed);
                   T1.Interval = 1000;
                   T1.Start();
                    * */
               }
               else
               {
                   Utils.ToCardLog("Продажа карты успешна. Добавляю атрибут " + CardNum);
                   AlohaTSClass.SetSVCardSaleAttr(CurentCardEntryParams.CheckId, CardNum);
               }
           }
           catch (Exception e)
           {
               Utils.ToCardLog("[Error] EndSVSale" + e.Message);
           }
       }

       static void T1_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
       {
           try
           {
               if (T1.Enabled)
               {
                   T1.Stop();
                   if (!AlohaTSClass.DeleteCardEntry(CurentCardEntryParams.CheckId, CurentCardEntryParams.EntryId))
                   {
                       if (CurentTryDeleteCount <= MaxTryDeleteCount)
                       {
                           CurentTryDeleteCount++;
                           T1.Start();
                       }
                   }
                   
               }
           }
           catch(Exception ee)
           {
               Utils.ToCardLog("[Error] Таймера удаления блюда "+ ee.Message);
           }
       }

       private static CardEntryParams CurentCardEntryParams;
       private static int CurentTryDeleteCount = 0;
       const int MaxTryDeleteCount = 10;
       
    }
    public class CardEntryParams
    {
        public int TableId ;
        public int CheckId;
             public int EntryId;
    
    }
}
