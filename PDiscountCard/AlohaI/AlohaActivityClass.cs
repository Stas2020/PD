using System;
using System.Collections.Generic;
using System.Text;
using AlohaFOHLib;
using Interop.INTERCEPTACTIVITYLib;
using System.Runtime.InteropServices;
using System.Threading;

using System.Diagnostics;
using System.ComponentModel;
using System.IO;



namespace PDiscountCard
{
   
    [ComVisible(true)]
    public class AlohaActivityClass : IInterceptMagcard //, IAlohaActivityClass
    {

        #region IInterceptMagcard Members

        public int InterceptMagcard(string bstrAccountNumber, string bstrCustomerName, string bstrExpirationDate, string bstrTrack1Info, string bstrTrack2Info, string bstrTrack3Info, string bstrRawMagcardData)
        {
            if (!iniFile.CardIntercepterEnabled)
            {
                return 0;
            }
            return MainClass.RegCard(bstrAccountNumber, bstrCustomerName, bstrExpirationDate, bstrTrack1Info, bstrTrack2Info, bstrTrack3Info, bstrRawMagcardData);
        }

        #endregion

          
    }
   
      }
