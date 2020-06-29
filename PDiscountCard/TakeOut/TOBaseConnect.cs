using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDiscountCard.TakeOut
{
    static class TOBaseConnect
    {
        static public string GetLoyaltyNumber(string Name)
        {
           // string TakeOutBaseConnectString = @"Data Source=avrora1\sqlexpress;Initial Catalog=AlohaToGo;Persist Security Info=True;User ID=PDiscount;Password=PDiscount";
            
            
            string TakeOutBaseConnectString = @"Data Source="+Utils.GetFileServerName() +@"\sqlexpress;Initial Catalog=AlohaToGo;Persist Security Info=True;User ID=sa;Password=";
            
            try
            {
                Utils.ToCardLog("GetLoyaltyNumber Name = " + Name);
                //Name = Name.Replace(",", "");
                TOServerBaseDataContext TakeOutBase = new TOServerBaseDataContext(TakeOutBaseConnectString);
                IEnumerable<string> Num = null ;
                try
                {
                    Num = from a in TakeOutBase.Guests where (a.FirstName + " " + a.LastName).Substring(0, Math.Min((a.FirstName + " " + a.LastName).Length, Name.Length)) == Name select a.EFrequencyMemberID;
                }
                catch
                { 
                
                }
                if (Num != null)
                {
                    Utils.ToCardLog("GetLoyaltyNumber return " + Num.ToList()[0]);
                    
                    return Num.ToList ()[0].Trim();
                }
                else
                {
                    Utils.ToCardLog("GetLoyaltyNumber return null" );
                    return "";
                }
            
            }
            catch(Exception e)
            {
                Utils.ToCardLog("[Error] GetLoyaltyNumber " + e.Message);
                return "";
            }

        }

    }
}
