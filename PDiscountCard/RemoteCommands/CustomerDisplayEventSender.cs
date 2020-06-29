using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace PDiscountCard.RemoteCommands
{
  static  class CustomerDisplayEventSender
    {

        static internal void CustomerDisplayEventSenderInit()
        {
            CustomerDisplayEventSenderThread = new Thread(SendChkToCustDispQueue);
            CustomerDisplayEventSenderThread.Start();
        }

        static internal void CustomerDisplayEventSenderStop()
        {
            ExitCustomerDisplayEventSenderThread = true;
        }
        static Thread CustomerDisplayEventSenderThread;
        static bool ExitCustomerDisplayEventSenderThread = false;

        static internal void AddSendChkToCustDispToQueue(int ChkNum, bool ShowTotal)
        {
            AlohaChkNeedToUpdate = ChkNum;
            mSendTotal = ShowTotal;
        }

        static internal void AddSendCloseChkToCustDisp(int CheckId)
        {
            AlohaChkNeedToUpdate = CheckId;
            mSendTotal = true;
            mCloseCheck = true;
        }

        static internal void AddSendCurentChkToCustDispToQueue(bool ShowTotal)
        {

            AlohaChkNeedToUpdate = LastAlohaChkNeedToUpdate;
            mSendTotal = ShowTotal;
        }


        static int LastAlohaChkNeedToUpdate = 0;
       static int AlohaChkNeedToUpdate = 0;
        static bool mSendTotal = false;
        static bool mCloseCheck = false;



        static private void SendChkToCustDispQueue()
        {
            while (!ExitCustomerDisplayEventSenderThread)
            {
                /*
                if (CloseCheck)
                {
                    DataReciver.DBData SendData = new DataReciver.DBData();
                    SendData.CheckClosed = true;
                    PDiscountCard.RemoteCommands.RemoteConnection.SendData(iniFile.RemoteLisenterCustDispPC, iniFile.RemoteLisenterCustDispPort, SendData, true);
                    CloseCheck = false;
                }
                */
                if (AlohaChkNeedToUpdate==0)
                {
                    //TimerSyncPoint = 0;
                    Thread.Sleep(500);
                    continue;
                }
                try
                {
                    LastAlohaChkNeedToUpdate = AlohaChkNeedToUpdate;
                    AlohaChkNeedToUpdate = 0;
                    bool LastmSendTotal = mSendTotal;
                    bool LastmCloseCheck = mCloseCheck;
                    mSendTotal = false;
                    mCloseCheck = false;
                    SendRefreshCheck(LastAlohaChkNeedToUpdate, LastmSendTotal, LastmCloseCheck);
                }
                catch(Exception e)
                {
                    Utils.ToCardLog("[Error] SendChkToCustDispQueue" + e.Message);
                }
                
                
                
            }
        }

        static void SendRefreshCheck(int CheckId, bool SendTotal, bool CloseCheck)
        {
            //Check Ch = AlohaTSClass.GetCheckById(CheckId);
            Check Ch = AlohaTSClass.GetCheckByIdShort(CheckId);
            SendRefreshCheck(Ch, SendTotal, CloseCheck);
        }

        static void SendRefreshCheck(Check Ch, bool SendTotal, bool CloseCheck)
        {
            DataReciver.DBData SendData = new DataReciver.DBData();
            SendData.CheckClosed = CloseCheck;
            foreach (Dish d in Ch.Dishez)
            {
                DataReciver.DBItem it = new DataReciver.DBItem()
                {
                    Name = d.LongName,
                    Price = d.Price,
                    Level =d.Level
                };
              
                SendData.DBItems.Add(it);
            }

            foreach (AlohaTender P in Ch.Tenders)
            {
                DataReciver.DBPayment DBP = new DataReciver.DBPayment()
                {
                    Name = P.Name,
                    Summ = (decimal)P.SummWithOverpayment
                };
                SendData.DBPayments.Add(DBP);
            }

            if (Ch.CompId != 0)
            {
                DataReciver.DBDiscount Disc = new DataReciver.DBDiscount()
                {
                    Name = Ch.CompName,
                    Summ = Ch.Comp
                };
                SendData.DBDiscounts.Add(Disc);
            }

            if (SendTotal)
            {
                SendData.Total = Ch.Summ;
            }

            PDiscountCard.RemoteCommands.RemoteConnection.SendData(iniFile.RemoteLisenterCustDispPC, iniFile.RemoteLisenterCustDispPort, SendData, true);
        }

    }
}
