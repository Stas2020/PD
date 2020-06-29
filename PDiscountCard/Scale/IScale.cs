using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDiscountCard.Scale
{
    interface IScale
    {
        bool Connect();
        void Disconnect();
        int GetWeight(out double Weight, out bool Stable, out string ErrStr);

    }
}
