using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDiscountCard.CreditCardIntegration
{
    class Arcus3CreditCardConnector : ICreditCardConnector
    {
         public Arcus3CreditCardConnector()
        {
            try
            {
                Utils.ToCardLog("ArcusClass3.Init() ");
                /*
                ChequeFilePath = iniFile.ArcusChequeFilesPath;
                CodeFilePath = iniFile.ArcusCodeFilePath;
                SAPacketObj Request = new SAPacketObj();
                SAPacketObj Response = new SAPacketObj();
                PCPOSTConnectorObj Conn = new PCPOSTConnectorObj();
                 * */




                Utils.ToCardLog("ArcusClass3.Init() End");
                Inited = true;
            }
            catch (Exception e)
            {
                Utils.ToCardLog("[Error] ArcusClass3.Init() " + e.Message);
            }
        }

         bool operInProcess = false;
        public bool OperInProcess
        {
            get
            {
                return operInProcess;
            }
            set
            {
                operInProcess = value;
            }
        }

        bool inited=false;
        
        public bool Inited
        {
            get { return inited; }
            set { inited = value; }
        }

        public void RunPaymentAsinc(decimal Summ, Check AlohaChk)
        {
            throw new NotImplementedException();
        }

        public void RunVozvrAsinc(decimal Summ)
        {
            throw new NotImplementedException();
        }

        public void RunCassirMenuAsinc()
        {
            throw new NotImplementedException();
        }

        public void RunXRepAsinc()
        {
            throw new NotImplementedException();
        }

        public void RunDetaleRepAsinc()
        {
            throw new NotImplementedException();
        }

        public void RunLastChkAsinc()
        {
            throw new NotImplementedException();
        }

        public void TestPinPad()
        {
            throw new NotImplementedException();
        }

        public string RunSVERKARepSinc(out string mReciept, out string mRes)
        {
            throw new NotImplementedException();
        }
    }
}
