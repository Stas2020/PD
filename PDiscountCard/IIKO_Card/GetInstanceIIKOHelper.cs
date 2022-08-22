using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDiscountCard.IIKO_Card
{
    public class GetInstanceIIKOHelper
    {
        private static ICardHelper card_helper = null;
        public static ICardHelper GetInstance()
        {
            if(card_helper == null)
            {
                //card_helper = new IIKO_CardHelper();
                card_helper = new S2010_CardHelper();
                Utils.ToLog("-------------------------Create object CardHelper-------------------------");
            }

            return card_helper;
        }
    }
}
