using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace PDiscountCard.FRSClientApp
{
   public static class ZReportAskSaver
    {

       public static void SaveZRepFRSAsk(DateTime BD)
       { 
           try
           {

               Utils.ToCardLog("SaveZRepFRSAsk");
               using (StreamWriter sw = new StreamWriter(PDiscountCard.CloseCheck.ChecksPath + @"\Zrep" + BD.ToString("ddMMyyyy") + ".zrep"))
               {
                   sw.WriteLine("ok");
               }
               //File.Create(PDiscountCard.CloseCheck.ChecksPath+@"\Zrep"+BD.ToString("ddMMyyyy")+".zrep");

           }
           catch(Exception e)
           {
               Utils.ToCardLog("Error SaveZRepFRSAsk " +e.Message);
           }
       }
    }

    
}
