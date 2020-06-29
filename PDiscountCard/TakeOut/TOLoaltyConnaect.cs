using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDiscountCard.TakeOut
{
    static class TOLoaltyConnect
    {
        public static bool ApplyDisc(int Table, string Name)
        {
            Utils.ToCardLog("ApplyDisc Table: " + Table.ToString() + " name " + Name);
            string Disc = TOBaseConnect.GetLoyaltyNumber(Name);
        
            if (Disc!="")
            {
                Utils.ToCardLog ("ApplyDisc к мемберу "+ Name +"  привязана карта "+ Disc);

                //AlohaTSClass.SetTakeOutAttr(Table, Name);
                /*
                AlohaTSClass.CheckWindow();
                
                */
                MainClass.AssignMember(Disc, Disc);
                return true;






            }
            return false;
        
        }


    }
}
