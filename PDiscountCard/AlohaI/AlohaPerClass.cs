using System;
using System.Collections.Generic;
using System.Text;
using AlohaFOHLib;

namespace PDiscountCard
{
    class AlohaPerClass: IInterceptAlohaPeripherals 
    {

        #region IInterceptAlohaPeripherals Members

        public void RegisterBarcodeInterceptor(IInterceptBarcode pIInterceptBarcode)
        {
            throw new NotImplementedException();
        }

        public void RegisterMagcardInterceptor(IInterceptMagcard pIInterceptMagcard)
        {
           // throw new NotImplementedException();
            int i = 1;
            
        }

        public void ReleaseBarcodeInterceptor(IInterceptBarcode pIInterceptBarcode)
        {
            throw new NotImplementedException();
        }

        public void ReleaseMagcardInterceptor(IInterceptMagcard pIInterceptMagcard)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
