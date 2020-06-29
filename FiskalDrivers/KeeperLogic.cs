using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FiskalDrivers
{
    public static class KeeperLogic
    {
        public static bool GetColor(FiskalCheck Chk)
        {
            return Chk.HasNotNalPayment;
        }
    }
}
