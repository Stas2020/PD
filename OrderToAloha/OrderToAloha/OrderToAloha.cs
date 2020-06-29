using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OrderToAloha
{
    public class OrderToAloha
    {   
        RemoteLisenter RL ;

        public OrderToAloha()
        {
            
            
                RL = new RemoteLisenter(innerPort);
                RL.ResponseEvent += new RemoteLisenter.ResponseEventHandler(RL_ResponseEvent);
            
        }
        public OrderToAloha(int Port)
        {
            if (Port > 0)
            {
                RL = new RemoteLisenter(Port);
                RL.ResponseEvent += new RemoteLisenter.ResponseEventHandler(RL_ResponseEvent);
            }
        }

        void RL_ResponseEvent(DataReciver.AlohaResponse e)
        {
            RaiseResponseEvent(e);
        }

        int innerPort = 64789;
        int outerPort = 64788;

        public int InnerPort
        {
            set
            {
                innerPort = value;
            }
            get
            {
                return innerPort;
            }
        }


        

        public delegate void ResponseEventHandler(object sender, DataReciver.AlohaResponse e);

        
        public  event ResponseEventHandler ResponseEvent;

        protected virtual void RaiseResponseEvent(DataReciver.AlohaResponse e)
        {
            
            if (ResponseEvent != null)
                ResponseEvent(this,e);
        }

      
        public  void CloseConnection()
        {
            RL.Stop();
        }

        public  void SendOrderToAloha(DataReciver.SendOrderToAlohaRequest args)
        {
            DataReciver.STCommand SC = new DataReciver.STCommand()
            {
                Ansver = false,
                CommandType = DataReciver.STCommandType.AddOrder,
                Id = new Guid(),
                Sender = Environment.MachineName,
                SenderPort = innerPort,
                sendOrderToAlohaRequest = args
            };
            if (args.port == 0)
            {
                args.port = outerPort;
            }
            RemoteConnection.SendData(args.RemoteCompName, args.port, SC);
        }


        public  void DeleteOrder(DataReciver.DeleteOrderRequest args)
        {
            DataReciver.STCommand SC = new DataReciver.STCommand()
            {
                Ansver = false,
                CommandType = DataReciver.STCommandType.DeleteOrder,
                Id = new Guid(),
                Sender = Environment.MachineName,
                SenderPort = innerPort,
                deleteOrderRequest = args
            };
            if (args.port == 0)
            {
                args.port = outerPort;
            }
            RemoteConnection.SendData(args.RemoteCompName, args.port, SC);
            
            
            //  return AlohaTS.DeleteOrder(OrderId);
        }

/*
        public  int CloseOrder(int OrderId, int PaymentId)
        {
            return AlohaTS.CloseOrder(OrderId, PaymentId);
        }

        */


        public  List<ItemExt> GetAllItms()
        {

            return null; //AlohaTS.GetAllItms();

        }

    }


    [Serializable]
    public class Item
    {

        public Item()
        {
        }
        private int _barCode;
        public int BarCode
        {
            set
            {
                if (value > 100000)
                {
                    _barCode = value % 100000;
                    SourceBase = (value - _barCode) / 100000;
                }
                else
                {
                    _barCode = value;
                    SourceBase = 0;
                }
            }
            get
            {
                return _barCode;
            }
        }
        public int SourceBase { set; get; }
        public decimal Price { set; get; }
        public double Count { set; get; }
        private string name = "";
        public string Name {
            set
            {
                name = value;
            }
            get
            {
                return name ;
            }
        }
        public  int EntryId { set; get; }
        public  int AlohaBarCode
        {
            get
            {
                return SourceBase * 100000 + BarCode;

            }
        }
    }
    [Serializable]
    public class ItemExt : Item
    {
        public string LongName { set; get; }
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
                return (int)(Ch1 * 10000 + Ch2);
            }
        }

        public List<Item> items = new List<Item>();
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
        None, Disc5, Disc10
    }
}
