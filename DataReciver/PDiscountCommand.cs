using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataReciver
{
    [Serializable]
    public class PDiscountCommand
    {

        public PDiscountCommand()
        {
            Id = new Guid();
        }
        public Guid Id { set; get; }

        public PDiscountCommandType CommandType { set; get; }
        public int TableId { set; get; }
        public int CheckId = 0;
        public int PaymentId { set; get; }
        public decimal PaymentSumm { set; get; }
        public int EmployeeId { set; get; }
        public List<int> EntIds { set; get; }
        public bool Result { set; get; }
        public int ResultId { set; get; }
        public string Sender { set; get; }
        public int SenderPort { set; get; }
        public bool Ansver { set; get; }
        public string ExeptionMessage = "";

        public List<Item> OrderBody = new List<Item>();

        public bool CheckNeedClose =false;
        public string Slip { set; get; }
    }


    [Serializable]
    public class DBData
    {
        public DBData()
        { }
        public bool CheckClosed { set; get; }
        public List<DBItem> DBItems = new List<DBItem>();
        public List<DBDiscount> DBDiscounts = new List<DBDiscount>();
        public List<DBPayment> DBPayments = new List<DBPayment>();
        public decimal Total { set; get; }
        //public List<Item> Mods = new List<Item>();
    }

    [Serializable]
    public class DBItem
    {
        public DBItem()
        { }
        public string Name { set; get; }
        public decimal Price { set; get; }
        public int Level { set; get; }
        //public List<Item> Mods = new List<Item>();
    }
    [Serializable]
    public class DBDiscount
    {
        public DBDiscount()
        { }
        public string Name { set; get; }
        public decimal Summ { set; get; }
        //public List<Item> Mods = new List<Item>();
    }

    [Serializable]
    public class DBPayment
    {
        public DBPayment()
        { }
        public string Name { set; get; }
        public decimal Summ { set; get; }
        //public List<Item> Mods = new List<Item>();
    }

    [Serializable]
    public class Item
    {
        public Item()
        {}
        public int Barcode {set; get ;}
        public List <Item>  Mods = new List<Item>();

    }
    [Serializable]
    public enum PDiscountCommandType
    {
        OrderItems, OrderAllItems, AddRemoteOrder, GetStopList, CloseCheck, DeleteOrder, PrintSlip
    }


    [Serializable]
    public class STItem
    {
        public STItem()
        { }
        public int Barcode { set; get; }
        public int SourceBase { set; get; }
        public decimal Price { set; get; }
        public int Count { set; get; }
    }

    [Serializable]
    public class STCommand
    {

        public STCommand()
        {
            Id = new Guid();
        }
        public Guid Id { set; get; }

        public STCommandType CommandType { set; get; }
        /*
        public int OrderId { set; get; }
        public List<STItem> Items { set; get; }
        public int CompanyId { set; get; }
        public string CompanyName { set; get; }
        public string BortName { set; get; }
        public int DiscountId { set; get; }
        public int Margin { set; get; }
        public DateTime TimeOfShipping { set; get; }
        public int AlohaCheckId { set; get; }
        public int AlohaTableNum { set; get; }
        */

        public SendOrderToAlohaRequest sendOrderToAlohaRequest { set; get; }
        public DeleteOrderRequest deleteOrderRequest { set; get; }
        public bool Result { set; get; }
        public int ResultId { set; get; }
        public string Sender { set; get; }
        public int SenderPort { set; get; }
        public bool Ansver { set; get; }
        public string ExeptionMessage = "";


        
      //  public List<Item> OrderBody = new List<Item>();

    }
    [Serializable]
    public enum STCommandType
    {
        AddOrder, DeleteOrder
    }

    [Serializable]
    public class SendOrderToAlohaRequest : OrderRequest
    {
        public SendOrderToAlohaRequest()
        { }

        
        public int OrderId { set; get; }
        public List<Item> Items { set; get; }
        public int CompanyId { set; get; }
        public string CompanyName { set; get; }
        public string BortName { set; get; }
        public int DiscountId { set; get; }
        public int Margin { set; get; }
        public DateTime TimeOfShipping { set; get; }
        //public out int AlohaCheckId, out int AlohaTableNum
    }

    [Serializable]
    public class DeleteOrderRequest : OrderRequest
    {
        public DeleteOrderRequest()
        { }

        
        public int OrderId { set; get; }
        
    }
    [Serializable]
    public class OrderRequest
    {
        public OrderRequest()
        { }

        public string RemoteCompName { set; get; }
        public int port { set; get; }
        
        //public out int AlohaCheckId, out int AlohaTableNum
    }
}

