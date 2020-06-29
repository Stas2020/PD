using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDiscountCard
{
    public interface ICreditCardConnector
    {
       
        bool OperInProcess { set; get; }
        bool Inited { set; get; }
        void RunPaymentAsinc(decimal Summ, Check AlohaChk);
        void RunVozvrAsinc(decimal Summ);
        void RunCassirMenuAsinc();
        void RunXRepAsinc();
        void RunDetaleRepAsinc();
        void RunLastChkAsinc();
        void TestPinPad();
        string  RunSVERKARepSinc(out string mReciept, out  string mRes);
    }
    public interface ICreditCardConnector2 : ICreditCardConnector
    {
        void RunGetSlipCopyAsinc();
    }

    

}
