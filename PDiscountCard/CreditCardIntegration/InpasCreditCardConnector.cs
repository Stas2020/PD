using System;
using System.Collections.Generic;
using System.Linq;

using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PDiscountCard.DualConnector;
using DualConnector;

namespace PDiscountCard.CreditCardIntegration
{
    class InpasCreditCardConnector : ICreditCardConnector
    {
        public InpasCreditCardConnector()
        {
            inited = true;
        }



        bool operInProcess=false;
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
            get
            {
                return inited;
            }
            set
            {
                inited = value;
            }
        }
        CreditCardOperationType operType;
        bool showError = false;
        public void RunPaymentAsinc(decimal Summ, Check AlohaChk)
        {
            operType = CreditCardOperationType.Payment;
            RunOper(1,Summ);
        }

        public void RunVozvrAsinc(decimal Summ)
        {
            operType = CreditCardOperationType.VoidPayment;
            frmInpasVoid fv = new frmInpasVoid();
            fv.ShowDialog();
            if (fv.Res == 1)
            {
                
                RunOper(29, Summ,CommandMode2:22, RRN:fv.RRN);

                

            }
            else if (fv.Res == 2)
            {
                
               // RunOper(4, Summ, CommandMode2: 22, RRN: fv.RRN);
                RunOper(4, Summ, RRN: fv.RRN);
                
            }
        }

        public void RunCassirMenuAsinc()
        {
            throw new NotImplementedException();
        }

        public void RunXRepAsinc()
        {
            operType = CreditCardOperationType.XReport;
            RunOper(63, 0, CommandMode2: 20 );
            
        }

        public void RunDetaleRepAsinc()
        {
            operType = CreditCardOperationType.LongReport;
            RunOper(63, 0, CommandMode2: 21 );
        }

        public void RunLastChkAsinc()
        {
            operType = CreditCardOperationType.LastChk;
            RunOper(63, 0, CommandMode:1, CommandMode2: 22);
        }

        public void TestPinPad()
        {
            throw new NotImplementedException();
        }

        public string RunSVERKARepSinc(out string mReciept, out string mRes)
        {
            throw new NotImplementedException();
            //RunOper(59, 0);
        }

        /*
        private void ExchangeComplited(int Code, string CodeDescr, ISAPacket response)
        {

            CreditCardAlohaIntegration.CreditCardOperationComplited(operType, showError, Code == 0, CodeDescr, response.ReceiptData);
            OperInProcess = false;
    }
        */
        private DateTime StartTransactionTime = DateTime.Now;
        public void RunOper(int OperId, decimal amount, int CommandMode=0, int CommandMode2=0, string RRN = "")
        {

            Task t = new Task (()=>{

            DualConnectorApi DApi = new DualConnectorApi();
            OperInProcess = true;
            string resStr = "";
            string statusDescr = "";
            string receipt = "";
                int status =0;
            int amountBase = (int)(amount);
            int res = DApi.Exchange(OperId, CommandMode, CommandMode2, amountBase, iniFile.CreditTerminalNum, RRN, out resStr, out receipt, out status, out statusDescr);

            if (res != 0)
            {
                            
                string Mess = "Ошибка старта операции" + OperId + " на терминале пластиковых карт." + Environment.NewLine + resStr;
                Mess += "Код ошибки: " + res;
                Mess += "Описание ошибки: " + resStr;
                Utils.ToCardLog(Mess);
            }


            CreditCardAlohaIntegration.CreditCardOperationComplited(operType, res!=0, res==0 && status==1, resStr, receipt);

            OperInProcess = false;
            Utils.ToCardLog("OperInProcess = false");           
            

            });
            t.Start();
        }

    }
}
