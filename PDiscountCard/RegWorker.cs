using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32; 

namespace PDiscountCard
{
    static  class RegWorker
    {
        static  RegistryKey readKey=null ;


        internal static  bool RegKeyExist()
        {
            readKey = Registry.CurrentUser.OpenSubKey("software\\HamSoft");
            return true;
        }
        internal static int GetRegPercent()
        {
            if (readKey == null)
            {
                return -1;
            }
            int Val = (int)readKey.GetValue("Percent", -1);
            return Val;
        }
        internal static int GetRegModType()
        {
            if (readKey == null)
            {
                return -1;
            }
            int Val = (int)readKey.GetValue("ModType", -1);
            return Val;
        }


    }
}
