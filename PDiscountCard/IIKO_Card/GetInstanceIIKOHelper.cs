using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDiscountCard.IIKO_Card
{
    public class GetInstanceIIKOHelper
    {
        public static ICardHelper GetInstance()
        {
            //return new IIKO_CardHelper();
            return new S2010_CardHelper();
        }
    }
}
