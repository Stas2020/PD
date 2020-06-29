using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Xml.Linq;


namespace PDiscountCard.FayRetail
{
    
        [XmlRoot(ElementName = "XMLResponse")]
        public class ResponseData
        {
            public string Version = "3.2";
            public int ErrorCode { set; get; }
        public string ErrorMessage { set; get; }

            public List<CalculateResponse> Calculates { set; get; }
            public List<BalanceResponse> Balances { set; get; }
            public List<DiscountResponse> Discounts { set; get; }
            public List<PaymentResponse> Payments { set; get; }
        }
    

         public class CommandResponse : CommandRequest
        {
            public CommandResponse() {}
            public int ErrorCode { set; get; }
        }

        public class CalculateResponse : CommandResponse
        {
            public CalculateResponse() { }
            [XmlAttribute]
            public string PurchaseID { set; get; }

            [XmlAttribute]
            public double AvailableAmount { set; get; }
            [XmlAttribute]
            public double AvailableBonusAmount { set; get; }
            [XmlAttribute]
            public double Balance { set; get; }

            [XmlElement]
            public Cheque Cheque { set; get; }
        }

        public class BalanceResponse : CommandResponse
        {
            public BalanceResponse() { }
            public Card Card { set; get; }
            public double BonusAmount { set; get; }
            public string Currency { set; get; }
            public string Description { set; get; }

        }


        public class ConfirmPurchaseResponse : CommandResponse
        {
            public ConfirmPurchaseResponse() { }
            [XmlAttribute]
            public string PurchaseID { set; get; }
        }

        public class DiscountResponse : CommandResponse
        {
            public DiscountResponse() { }
            [XmlAttribute]
            public string PurchaseID { set; get; }
            [XmlElement]
            public String  ChequeMessage { set; get; }
            [XmlIgnore]
            public string ChequeMessageDecript
            {
                get
                {
                    if (ChequeMessage == null)
                    {
                        return null;
                    }

                    byte[] textAsBytes = System.Convert.FromBase64String(ChequeMessage);
                    return  Encoding.UTF8.GetString(textAsBytes);
                }
            }


            [XmlElement]
            public Cheque Cheque { set; get; }
            //[XmlElement]
            public List<Pay> Pays { set; get; }
        }

        public class PaymentResponse : CommandResponse
        {
            public PaymentResponse() { }
            [XmlAttribute]
            public string PurchaseID { set; get; }
            [XmlAttribute]
            public string TransactionID { set; get; }
            [XmlAttribute]
            public double Amount { set; get; }
            [XmlElement]
            public Cheque Cheque { set; get; }
            [XmlElement]
            public String ChequeMessage { set; get; }
            public string ChequeMessageDecript
            {
                get
                {
                    if (ChequeMessage == null)
                    {
                        return null;
                    }

                    byte[] textAsBytes = System.Convert.FromBase64String(ChequeMessage);
                    return Encoding.UTF8.GetString(textAsBytes);
                }
            }

        }
}

