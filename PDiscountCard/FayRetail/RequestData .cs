using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Xml.Linq;


namespace PDiscountCard.FayRetail
{
   [XmlRoot(ElementName = "XMLRequest")]
    public  class RequestData
    {
         public  string Version = "3.2";
         public List<CalculateRequest> Calculates { set; get; }
         public List<BalanceRequest> Balances { set; get; }
         public List<DiscountRequest> Discounts { set; get; }
         public List<PaymentRequest> Payments { set; get; }
         public List<ConfirmPurchaseRequest> ConfirmPurchases { set; get; }


    }
   


    public class Command
    {
        public Command() { }
        [XmlAttribute]
        public string DeviceLogicalID { set; get; }
        [XmlAttribute]
        public int ElementID { set; get; }
    }

    public class CommandRequest : Command, ICommandRequest
    {
        public CommandRequest() { }
        [XmlAttribute]
        public string OperationID { set; get; }
        [XmlIgnore]
        public DateTime OperationDate { set; get; }
        [XmlAttribute(AttributeName="OperationDate")]
        public string OperationDateString
        {
            get { return this.OperationDate.ToString("yyyy-MM-ddTHH:mm:ss.fff"); }
            set { this.OperationDate = DateTime.Parse(value); }
        }
        [XmlAttribute]
        public string Cashier { set; get; }


          [XmlElement]  
        public Card Card { set; get; }
        
    }


    public interface ICommandRequest 
    {
        string OperationID { set; get; }
        DateTime OperationDate { set; get; }
        Card Card { set; get; }
        string DeviceLogicalID { set; get; }
        int ElementID { set; get; }
        string Cashier { set; get; }
        
    }

   

    public  class Card
    {
        public Card(){}
         [XmlAttribute]
        public String Number {set;get;}
         [XmlAttribute]
        public String Track2 {set;get;}
         [XmlAttribute]
        public String MaskedNumber { set; get; }
         [XmlAttribute]
        public String Holder { set; get; }
         [XmlAttribute]
        public String CardType { set; get; }
    }


    public class CalculateRequest:CommandRequest
    {
        public CalculateRequest() { }
        [XmlAttribute]
        public string PurchaseID { set; get; }

        [XmlElement]  
        public Cheque Cheque { set; get; }
    }

    public class PaymentRequest : CommandRequest
    {
        public PaymentRequest() { }
        [XmlAttribute]
        public string PurchaseID { set; get; }
        [XmlAttribute]
        public double Amount { set; get; }
        
        [XmlElement]
        public Cheque Cheque { set; get; }
    }

    public class DiscountRequest : CommandRequest
    {
        public DiscountRequest() { }
        [XmlAttribute]
        public string PurchaseID { set; get; }

        [XmlElement]
        public Cheque Cheque { set; get; }
        //[XmlElement]
        public List<Pay> Pays { set; get; }
    }

    public class ConfirmPurchaseRequest  : CommandRequest
    {
        public ConfirmPurchaseRequest() { }
        [XmlAttribute]
        public string PurchaseID { set; get; }
    }


    public class Pay
    {
        public Pay() { }
        [XmlAttribute]
        public string Type { set; get; }
        [XmlAttribute]
        public string Amount { set; get; }
    }
    
    public class Cheque
    {
        public Cheque() { }
        public Cheque(FayRetailCheckInfo CheckInfo) 
        { 
            Items = CheckInfo.Items;
            ChequeNumber = CheckInfo.ChequeNumber;
            ChequeDate = CheckInfo.ChequeDate;
        }

         [XmlElement(ElementName = "ChequeLine")]
        public List<ChequeLine> Items { set; get; }
        [XmlAttribute]
        public string  ChequeNumber { set; get; }
        [XmlIgnore]
        public DateTime ChequeDate { set; get; }
        
        [XmlAttribute(AttributeName="ChequeDate")]
        public string ChequeDateString
        {
            get { return this.ChequeDate.ToString("yyyy-MM-ddTHH:mm:ss.fff"); }
            set { this.ChequeDate = DateTime.Parse(value); }
        }

           
    }

    public class ChequeLine
    {
        public ChequeLine() { }
        [XmlAttribute]
        public int PosID { set; get; }
        [XmlAttribute]
        public double Amount { set; get; }
        [XmlAttribute]
        public int GoodsId { set; get; }
        [XmlAttribute]
        public string Message { set; get; }
        [XmlAttribute]
        public double Discount { set; get; }
        [XmlAttribute]
        public double Quantity { set; get; }
        [XmlAttribute]
        public double Price { set; get; }
        [XmlAttribute]
        public double PayAmount { set; get; }
        [XmlAttribute]
        public string  Barcode { set; get; }
        [XmlAttribute]
        public string Name { set; get; }
      

    }
    public class BalanceRequest : CommandRequest
    {
        public BalanceRequest() { }
        

    }
    

}
