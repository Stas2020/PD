using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDiscountCard.IIKO_Card
{
    public abstract class ICardHelper
    {
        abstract public bool SendToIikoCard(GiftCard card);
        abstract public GiftCard GetCard(String card_code);
        abstract public bool ReturnToCard(String card_code, decimal sum, int depNum);
        abstract public bool PayFromCard(String card_code, decimal sum, int depNum);
        abstract public bool SetCardActiveStatus(String card_code, bool activeStatus);

    }
}
