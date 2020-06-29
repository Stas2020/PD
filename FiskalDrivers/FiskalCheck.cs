using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FiskalDrivers
{
   public class FiskalCheck
    {
        public  List<FiskalDish> Dishes = new List<FiskalDish>();
        public List<FiskalPayment> Payments = new List<FiskalPayment>();
        //public int PaymentType { set; get; }
        public double Summ { set; get; }
        public double Charge { set; get; }
       public double Discount { set; get; }
       public string DiscountName { set; get; }
       
       public bool IsVoid = false;
       public string  CheckNum { set; get; }
       public string TableName { set; get; }
       public string Cassir { set; get; }
       public string Waiter { set; get; }
       public bool FlyCheck = false;
      // public string PaymentName { set; get; }

       public string GetDiscountString(int CheckWidth)
       {
           try
           {
               return DiscountName + new string(" "[0], CheckWidth - DiscountName.Length - Discount.ToString("0.00").Length) + Discount.ToString("0.00");
           }
           catch
           {
               return "";
           }

       }

       public DateTime TimeofClose
       {
           get
           {
               return DateTime.Now;
           }
       }
       public DateTime TimeofOpen { set; get; }


       public List<string> CaptionInfoStrings
       {
           get
           {
               List<String> Tmp = new List<string>();
               Tmp.Add(String.Format("Чек # {0} Стол # {1}", CheckNum, TableName));
               Tmp.Add(String.Format(TimeofOpen.ToString("dd-MM-yyyy")+" Открыт "+TimeofOpen.ToString("HH:mm")));
               Tmp.Add(String.Format(TimeofClose.ToString("dd-MM-yyyy") + " Закрыт " + TimeofClose.ToString("HH:mm")));
               if (Cassir != "")
               {
                   Tmp.Add(String.Format("Кассир: {0} ", Cassir));
               }
               if (Waiter != "")
               {
                   Tmp.Add(String.Format("Официант: {0} ", Waiter));
               }
               return Tmp;
           }
       }
       

       public bool HasNotNalPayment
       {
           get
           {
               return Payments.Where(a => a.PaymentType >= 0).Count() > 0;
           }
       }
      
    }


   public class FiskalPayment
   {
       public string Name { set; get; }
       public int PaymentType { set; get; }
       public double Summ { set; get; }
       public bool NonFiskalPayment = false;

       public string GetPaymentNameString(int CheckWidth)
       {
           try
           {
               return Name + new string(" "[0], CheckWidth - Name.Length - Summ.ToString("0.00").Length) + Summ.ToString("0.00");
           }
           catch
           {
               return "";
           }

       }

       
   }
   public class FiskalDish
    {
        public string Name { set; get; }
        public double Price { set; get; }
        public double Quantity { set; get; }
        public double Discount { set; get; }
        public double AllSumm
        {
            get
            {
                return Quantity * Price;
            }
        }
        private int MaxNameLenght = 25;
        private int MaxNameAndQLenght = 30;
        public string GetNameAndCountString()
        {
            try
            {
                string N = Name;
                if (N.Length > MaxNameLenght)
                {
                    N = N.Substring(0,MaxNameLenght);
                }

                return Name + new string(" "[0], MaxNameAndQLenght - N.Length - Quantity.ToString().Length) + Quantity.ToString();
            }
            catch
            {
                return "";
            }

        }
    }
}
