using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataReciver;

namespace PDiscountCard.RemoteCommands
{
    public static class RemoteSender
    {

        
        public static  void OrderItems(List<int> EntIds, int CheckId, int TableId, int TermId)
        {
            /*
            PDiscountCommand Comm = new PDiscountCommand();
            Comm.CommandType = PDiscountCommandType.OrderItems;
            Comm.EntIds = EntIds;
            Comm.CheckId = CheckId;
            Comm.TableId = TableId;
            Comm.Sender = Environment.MachineName;
            Comm.Ansver = false; 
            RemoteConnection.SendData(AlohainiFile.TermStr + TermId.ToString(), 9267, Comm, true);
            */

        }

        public static void SendCheckOnClose(int CheckId,int PId,int EId,decimal PaymentSumm)
        {
            PDiscountCommand Comm = new PDiscountCommand();
            Comm.CommandType = PDiscountCommandType.CloseCheck;
            Comm.CheckId = CheckId;
            Comm.PaymentId = PId;
            Comm.EmployeeId = EId;
            Comm.PaymentSumm = PaymentSumm;

            Comm.Sender = Environment.MachineName;
            Comm.SenderPort = iniFile.RemoteLisenterPort;
            Comm.Ansver = false;
            RemoteConnection.SendData(AlohainiFile.TermStr + iniFile.RemoteCloseCheckTerminal.ToString(), iniFile.RemoteLisenterPort, Comm, true);
        }


        public static void SendPrintSlipTCP(string Slip, out bool Sucseess)
        {
            DataReciver.PDiscountCommand Comm = new DataReciver.PDiscountCommand();
            Comm.CommandType = DataReciver.PDiscountCommandType.PrintSlip;

            Comm.Sender = Environment.MachineName;
            Comm.Ansver = false;
            Comm.Slip = Slip;
            Comm.Id = Guid.NewGuid();

            //   List<DataReciver.Item> mBasket = GetConvertBasketToAloha();
            //   Comm.OrderBody = mBasket;

        //    logger.Log(LogLevel.Debug, "Отправляю TCP пакет PrintSlip машине " + Setting.CloseTerminalName + " порт:" + Setting.OutPort);
            //Sucseess = RemoteConnection.SendData(Setting.CloseTerminalName, Setting.OutPort, Comm, true);
         Sucseess=   RemoteConnection.SendData(AlohainiFile.TermStr + iniFile.RemoteCloseCheckTerminal.ToString(), iniFile.RemoteLisenterPort, Comm, true);
        //    logger.Log(LogLevel.Debug, "Попытка отправления PrintSlip завершена " + Setting.CloseTerminalName + " порт:" + Setting.OutPort);
            //return Comm;
        }


    }
}
