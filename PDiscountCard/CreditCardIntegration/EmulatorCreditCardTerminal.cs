using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace PDiscountCard
{
    class EmulatorCreditCardTerminal : ICreditCardConnector
    {
        public EmulatorCreditCardTerminal()
        {
            Inited = true;
        }
        bool _OperInProcess = false;
        public bool OperInProcess
        {
            get
            {
                return _OperInProcess;
            }
            set
            {
                _OperInProcess = value;
            }
        }

        public void RunPaymentAsinc(decimal Summ, Check AlohaChk)
        {
            
            //RunOper();
            
            Thread TrPosXThread = new Thread(RunOper);
            TrPosXThread.Name = "Поток для RunOper";
            TrPosXThread.Start();
        }


        public void RunOper()
        {
            Thread.Sleep(3000);

            CreditCardAlohaIntegration.CreditCardOperationComplited(CreditCardOperationType.Payment,
                false, true, "Успешно", "Это тело слипа");
        }

        public void RunVozvrAsinc(decimal Summ)
        {
            Utils.ToCardLog("Emulator RunVozvrAsinc");
            CreditCardAlohaIntegration.CreditCardOperationComplited(CreditCardOperationType.Payment,
                false, true, "Успешно", "Это тело слипа");
        }

        public void RunCassirMenuAsinc()
        {
            //throw new NotImplementedException();
        }

        public void RunXRepAsinc()
        {
            CreditCardAlohaIntegration.CreditCardOperationComplited(CreditCardOperationType.Payment,
                false, true, "Успешно", "Это тело Х-отчета");
        }

        public string RunSVERKARepSinc(out string mReciept, out string mRes)
        {
            mRes="000";
            mReciept = "Сверка успешна";
            CreditCardAlohaIntegration.CreditCardOperationComplited(CreditCardOperationType.Payment,
                false, true, "Успешно", "Это тело сверки");
            return "";
        }


        bool _Inited = false;
        public bool Inited
        {
            get
            {
                return _Inited;
            }
            set
            {
                _Inited = value;
            }
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
    }
}
