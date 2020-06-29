using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
namespace OrderToAlohaSrv
{
    public static class SendFromNetToAloha
    {
        public static List<DataReciver.STCommand> OrdersQuery = new List<DataReciver.STCommand>();

        public static void SendOrderToAloha(DataReciver.STCommand req)
        {
            OrdersQuery.Add(req);
            Utils.ToLog("Add req to query " + req.sendOrderToAlohaRequest.OrderId);
           
            foreach (OrderToAloha.Item itm in req.sendOrderToAlohaRequest.Items)
            {
                Utils.ToLog(string.Format("itm: {0}; Name: {1}",itm.BarCode,itm.Name));
            }
        }
        private static bool AddOrdQStop = false;
        public  static void AddOrdQ()
        {
            while (!AddOrdQStop)
            {
                if (OrdersQuery.Count() > 0)
                {
                    Utils.ToLog("get from query");
                    int Chid = 0;
                    int Tid = 0;
                    DataReciver.STCommand InCommand = OrdersQuery[0];
                    DataReciver.SendOrderToAlohaRequest req = InCommand.sendOrderToAlohaRequest;
                    
                    SendOrderToAloha(req.OrderId, req.Items, req.CompanyId, req.CompanyName, req.BortName, req.DiscountId, req.Margin, req.TimeOfShipping, req.FreeDisc, out Chid, out Tid);
                    Thread.Sleep(5000);
                    Utils.ToLog("start RC.SendData " );
                    req.AlohaCheckId = 1;
                    req.AlohaTableNum = 1;
                    InCommand.Result = true;
                    InCommand.ResultId = 1;
                    InCommand.Ansver = true;
                    
                  //  DataTCPSender RC = new DataTCPSender();
                  //  bool r = RC.SendData(InCommand.Sender, InCommand.SenderPort, InCommand);
                //    Utils.ToLog("RC.SendData end" + r.ToString());
                    OrdersQuery.Remove(InCommand);
                    
                }
                else
                {
                    Thread.Sleep(1000);
                }
                
            }
        }


        public static void SendOrderToAloha(int OrderId, List<OrderToAloha.Item>  Items, int CompanyId, string CompanyName, string BortName, int DiscountId, int Margin, DateTime TimeOfShipping, int FreeDisc ,out int AlohaCheckId, out int AlohaTableNum)
        {
            //Сверсекретный код взаимодействия с алохой
            if (FreeDisc == null)
            {
                FreeDisc = 0;
            }
            AlohaCheckId = 0;
            AlohaTableNum = 0;

            var Items2 = Items;
            //AlohaTS.AddOrder(OrderId, Items, CompanyId, CompanyName, BortName, DiscountId, Margin, TimeOfShipping, FreeDisc,out AlohaCheckId, out AlohaTableNum);
            Utils.ToLog("ToSql.AddOrder" + OrderId);
            ToSql.AddOrder(OrderId, Items2, CompanyId, CompanyName, BortName, DiscountId, Margin, TimeOfShipping, FreeDisc, AlohaCheckId, AlohaTableNum); 
        }
        public static int DeleteOrder(int OrderId)
        {
            Utils.ToLog("ToSql.DeleteOrder" + OrderId);
            //return AlohaTS.DeleteOrder(OrderId);
            return 0;
        }

        public static int CloseOrder(int OrderId, int PaymentId)
        {
            Utils.ToLog("ToSql.CloseOrder" + OrderId);
            //return AlohaTS.CloseOrder(OrderId, PaymentId);
            return 0;
        }
        /*
        public static List<OrderToAloha.ItemExt> GetAllItms()
        {

            return AlohaTS.GetAllItms();
        
        }
        */
    }
  



    public class AlohaCheck
    {
        public int STId;
        public int AlohaId;
        public int AlohaNum
        {
            get
            {
                ulong Ch1 = (ulong)AlohaId >> 20;
                ulong Ch2 = (ulong)AlohaId & 0xFFFFF;
                return (int)(Ch1*10000 + Ch2);
            }
        }

        public List<OrderToAloha.Item> items = new List<OrderToAloha.Item>();
        public int DiscTypeId;
        public int DiscId;
        public int TableNumber;

        public List<CPayment> Payments = new List<CPayment>();

        public decimal Summ = 0;
    }

    public class CPayment
    {
        public int PType;
        public decimal Count;
    
    }

    public enum AlohaDiscounts
    {
      None,  Disc5, Disc10
    }
}
