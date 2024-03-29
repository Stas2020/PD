﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace PDiscountCard
{


    public enum TenderType
    { None = 0, Cash = 1, CreditCard = 20, Predoplata = 30, Credit = 40, GloryCash = 25 }

    public class CEmpl
    {
        public int Id = 0;
        public string Name = "";
        public CEmpl()
        { }
        public override string ToString()
        {
            return Name;
        }
    }


    public class AlohaComp
    {
        public int Id { set; get; }
        public decimal Amount { set; get; }
        public String Name { set; get; }
        public String Description { set; get; }
        public int CompType { set; get; }
    }

    public class AlohaTender
    {
        public const int CashTenderId = 1;
        public const int CardTenderId = 20;
        public static List<int> PredoplataTenderIds = new List<int> { 21, 30 };
        //public const int PredoplataTenderId = 30;
        public static List<int> AlohaBallsTenderIds = new List<int> { 25, 26};
        public const int CreditTenderIdOut = 25;
        public const int CreditTenderIdIn = 26;

        public string Name = "";

        public int AlohaTenderId = 0;
        private int tenderId;
        public int TenderId
        {
            get
            {
                if (AlohaTenderId == 0)
                {
                    if (Name == "РУБЛИ")
                    {
                        return 1;
                    }
                    else if (Name == "Кред.карта")
                    {
                        return 20;
                    }
                    else
                    {
                        return 0;
                    }
                }

                if (AlohaBallsTenderIds.Contains(AlohaTenderId))
                {
                    return CreditTenderIdOut;
                }
                return AlohaTenderId;
            }
            set
            {
                tenderId = value;
            }
        }
        public int AuthId = 0;
        public double Summ = 0;
        public double SummWithOverpayment = 0;
        public string Ident = "";
        public double NR = 0;
        public string GCTYPE = "";
        public double GCAMOUNT = 0;
        public double GCREDEEM = 0;

        public string CardPrefix
        {
            get
            {
                if (Ident.Length <= 5) return "";
                try { return Ident.Trim().Substring(0, 5); }
                catch { return ""; }
            }
        }
        public string CardNumber
        {
            get
            {
                try {
                    long CRes = 0;
                    if (Int64.TryParse(Ident.Trim().Substring(5), out CRes))
                    {
                        return Ident.Trim().Substring(5);
                    }
                    else
                    {
                        return "0";
                    }
                
                }
                catch { return ""; }
            }
        }

    }

    public class Dish:ICloneable
    {
        public Guid Id= Guid.NewGuid();
        public bool QtyType = false;
        public bool IsOrdered = false;
        /// <summary>
        /// код айтема из Алохи.он же Id в dbf
        /// </summary>
        public int BarCode = 0;
        /// <summary>
        /// Добавка к цене блюда в целях округления копейки
        /// </summary>
        public decimal Delta = 0;
        public decimal Count = 0;       
        internal decimal DispPrice = 0;
        /// <summary>
        /// Original price
        /// </summary>
        public decimal OPrice = 0;
        /// <summary>
        /// Original price for one
        /// </summary>
        public double OPriceone = 0;
        /// <summary>
        /// Цена с учетом скидки
        /// </summary>
        /// 
        public decimal Price = 0;
        /// <summary>
        /// Цена с учетом скидки за единицу
        /// </summary>
        public double Priceone = 0;
        // public decimal PriceWithDiscount = 0;
        
        public string Name = "";
        public string LongName = "";
        public string CHITNAME = "";
        internal bool Selected = false;
        internal int Level = 0;
        /// <summary>
        /// количество
        /// </summary>
        public decimal QUANTITY = 0;
        /// <summary>
        /// количество для весовых
        /// </summary>
        public decimal QtyQUANTITY = 1;
        internal object LinkedObject = null;

        public decimal DISP_PRICE = 0;

        public string CardPrefix = "";
        public string CardNumber = "";
        /// <summary>
        /// EntryId
        /// </summary>
        public int AlohaNum = 0;

        public decimal ServiceChargeSumm = 0;

        internal List<Dish> CurentModificators = new List<Dish>();


        public Dish Clone()
        {
            return (Dish)this.MemberwiseClone();
        }


        object ICloneable.Clone()
        {
            return this.MemberwiseClone();
            //throw new NotImplementedException();
        }
    }


    [Serializable]
    public class Check:ICloneable
    {
        public Check()
        {

        }

        //private List<int> PresentCardBCs = new List<int>() { 999903, 999905, 999910 };

        internal Check(int _AlohaCheckNum)
        {
            AlohaCheckNum = _AlohaCheckNum;
            //  BusinessDate = GetBusinessDate;
        }

        public List<AlohaTender> Tenders = new List<AlohaTender>();

        //public  TenderType Tender=  TenderType.None;
        //public int TenderId = 0;
        internal string TableDescription = "";
        //internal int CardPaymentId = 0;
        public string DiscountCard = "";
        public int PredcheckCount = 0;
        public List<Dish> Dishez = new List<Dish>();
        internal int NumberInTable = 0;
        public int DiscountMGR_NUMBER = 0;
        public int DegustationMGR_NUMBER = 0;
        public string LoyaltyCard = "";
        public decimal LoyaltyBonus = 0;
        public bool IsClosed = false;
        public string ASVCard = "";
        public List<string> ASVCards = new List<string>();
        public Guid GuidId;

        public  List<string> FrStringsAfter = new List<string>();
        public  List<string> FrStringsBefore = new List<string>();

        public decimal ASVCardBalance = 0;


        public string PMSGuestName = "";
        public string ServiceChargeName = "";
        public decimal ServiceChargeSumm = 0;


        internal bool HasFiskalPayment()
        {
            return Tenders.Where(a => !AlohaTender.PredoplataTenderIds.Contains(a.TenderId)).Count() > 0;
            //return Tenders.Where(a => a.TenderId != AlohaTender.PredoplataTenderId).Count() > 0;
        }

        private DateTime AlohaTime(long T)
        {
            try
            {
                DateTime dt1 = new DateTime(1899, 12, 31);
                DateTime Tmp = new DateTime(dt1.Ticks + T * 10000000, DateTimeKind.Utc);
                //Tmp.AddHours(4);
                return Tmp.ToLocalTime();
            }
            catch
            {
                return DateTime.Now;
            }
        }

        internal bool HasNonCashPayment()
        {
            return Tenders.Where(a => a.TenderId != 1).Count() > 0;
        }

        internal bool HasCreditPayment()
        {
            return CreditPayments().Count > 0;
        }

        internal List<AlohaTender> CreditPayments()
        {
            return Tenders.Where(a => AlohaTender.AlohaBallsTenderIds.Contains(a.TenderId) && a.AuthId == 2).ToList();
        }

        public decimal CashSumm2
        {
            get
            {
                return (decimal)Tenders.Where(a => a.TenderId == 1).Sum(a => a.Summ);
            }
        }
        public decimal CashSummWithOverpayment2
        {
            get
            {
                return (decimal)Tenders.Where(a => a.TenderId == 1).Sum(a => a.SummWithOverpayment);
            }
        }
        public decimal CardSumm2
        {
            get
            {
                return (decimal)Tenders.Where(a => a.TenderId == 20).Sum(a => a.Summ);
            }
        }

        public decimal CreditSumm2 { set; get; }


        private static bool CompareDishez(Dish D1, Dish D2)
        {
            //Utils.ToCardLog("CompareDishez " + D1.LongName + " " + D2.LongName);
            /*
            if (AlohaTSClass.DishIsQty(D2.BarCode ))
            {
                return false;
            }
             * */
            return ((D1.BarCode == D2.BarCode) && (D1.Price == D2.Price) && (D1.DISP_PRICE == D2.DISP_PRICE));
        }

        internal bool HasUnorderedItems
        {
            get
            {
                foreach (Dish d in Dishez)
                {
                    if (!d.IsOrdered)
                    {
                        return true;
                    }
                }
                return false;
            }
        }


        internal List<Dish> OrderedDishez
        {
            get
            {
                List<Dish> Tmp = new List<Dish>();
                foreach (Dish D in Dishez)
                {
                    if (D.IsOrdered)
                    {
                        Tmp.Add(D);
                    }
                }
                return Tmp;

            }
        }

        //ConSolidateDishez
        //internal void SetConSolidateDishez()

        internal List<Dish> ConSolidateDishez
        {
            
            get
            {
                List<Dish> Tmp = new List<Dish>();

                if (Dishez == null)
                {
                    return null;
                }

                
                List<Dish> CopyDishez = new List<Dish>();

                foreach (Dish D in Dishez)
                {
                    CopyDishez.Add(D.Clone());
                }

                foreach (Dish D in CopyDishez)
                {
                    if (!AlohaTSClass.DishIsQty(D.BarCode))
                    {

                        if (Tmp.Find(

                                delegate(Dish Dd)
                                {
                                    return CompareDishez(Dd, D);

                                }
                            ) == null)
                        {

                            int count = 1;


                            count = Dishez.FindAll(
                            delegate(Dish Dd)
                            {
                                return CompareDishez(Dd, D);
                            }
                        ).Count;

                            D.ServiceChargeSumm = Dishez.FindAll(
                            delegate(Dish Dd)
                            {
                                return CompareDishez(Dd, D);
                            }
                        ).Sum(a => a.ServiceChargeSumm);

                            // D.QUANTITY = count;
                            D.Count = count;
                            Tmp.Add(D);
                        }

                    }
                    else
                    {
                        D.Count = 1;
                        Tmp.Add(D);
                    }


                }
                return Tmp;
            }

        }

        internal List<Dish> ConSolidateSpoolDishez
        {
            get
            {
                List<Dish> Tmp2 = new List<Dish>();
                foreach (Dish d in ConSolidateDishez)
                {
                    if (d.BarCode == 999901) continue;

                    if (d.BarCode >=iniFile.SpoolMaxDish) continue;
                    Tmp2.Add(d);
                }
                return Tmp2;

            }
        }


        //internal List<Dish> ConSolidateSpoolDishez=null ;
        /*
        {
            get {
                if (ConSolidateDishez == null)
                {
                    return null;
                }
                List<Dish> Tmp = new List<Dish>();
                foreach (Dish d in ConSolidateDishez)
                {
                    if (d.BarCode == 999901) continue;
                    if (d.BarCode >= 933000) continue;
                    Tmp.Add(d); 
                }
                return Tmp;
            }
        }

        */


        //internal List<Dish> ConSolidateDishez = null;

        public decimal Summ;
        public decimal Oplata
        {
            get
            {
                return Convert.ToDecimal(Tenders.Where(a => a.AuthId == 2).Sum(a => a.Summ));
            }
        }

        public List<AlohaComp> Comps = new List<AlohaComp>();

        public decimal Comp_
        {
            get
            {
                decimal result = 0;
                foreach (var comps_ in Comps)
                {
                    result += comps_.Amount;
                }

                return result;
            }

        }
        public decimal Comp = 0;

        internal DateTime SystemDateOfClose2 = new DateTime();
        internal long _CloseTime;
        internal long CloseTime
        {
            set
            {
                _CloseTime = value;
                SystemDateOfClose2 = AlohaTime(_CloseTime);
                SystemDate = AlohaTime(_CloseTime);
            }
            get
            {
                return _CloseTime;
            }
        }

        internal long _OpenTime;
        internal long OpenTime
        {
            set
            {
                _OpenTime = value;
                SystemDateOfOpen = AlohaTime(_OpenTime);

            }
            get
            {
                return _OpenTime;
            }
        }
        public int OpenTimem = 0;
        public int RealOpenTimem = 1;

        public double CheckTimeLong
        {
            set
            {
                _CheckTimeLong = value;
            }
            get
            {
                if (_CheckTimeLong == 0)
                {
                    return (SystemDate - SystemDateOfOpen).TotalMinutes;
                }
                return _CheckTimeLong;
            }
        }
        double _CheckTimeLong = 0;

        public double CheckTimeLongSec
        {
            set
            {
                _CheckTimeLongSec = value;
            }
            get
            {
                if (_CheckTimeLongSec == 0)
                {
                    return (SystemDate - SystemDateOfOpen).TotalSeconds;
                }
                return _CheckTimeLongSec;
            }
        }
        double _CheckTimeLongSec = 0;


        public int CompId = 0;
        public string CompName = "";
        public string CompDescription = "";
        //public bool IsNal;
        public bool Vozvr;
        public int Waiter;
        public int Cassir;
        public int TerminalId;
        public int TableNumber;
        public string TableName;
        public int TableId;
        public int Guests;
        public Int32 KkmShiftNumber;
        public Int32 KkmNum;
        public Int32 EKLZNumInt
        {
            get
            {
                try
                {
                    if (EKLZNum == "") return 0;
                    Int64 Tmp = Convert.ToInt64(EKLZNum);
                    while (Tmp > Int32.MaxValue)
                    {
                        Tmp = Tmp / 10;
                    }
                    return Convert.ToInt32(Tmp);
                }
                catch
                {
                    return 0;
                }
            }
        }
        public string EKLZNum = "0";



        public string FiskalFileName;


        internal string CheckNum
        {
            get
            {


                ulong Ch1 = (ulong)AlohaCheckNum >> 20;
                ulong Ch2 = (ulong)AlohaCheckNum & 0xFFFFF;
                return Ch1.ToString() + BusinessDate.ToString("ddMMyy") + Ch2.ToString("0000");

            }
        }



        internal static string GetLongCheckNumber(int AlohaNum)
        {

            ulong Ch1 = (ulong)AlohaNum >> 20;
            ulong Ch2 = (ulong)AlohaNum & 0xFFFFF;

            return Ch1.ToString() + AlohainiFile.BDate.ToString("ddMMyy") + Ch2.ToString("0000");
        }


        private int SAlohaCheckNum;
        internal List<Check> ChecksOnTable = new List<Check>();
        //string _OpenTimeStr = "";
        public DateTime SystemDateOfOpen = DateTime.Now;

        internal List<int> PaymentsIds = new List<int>();

        public DateTime SystemDate = DateTime.Now;

        public DateTime BusinessDate = AlohainiFile.BDate;

        public DateTime PedcheckTime = new DateTime(2000, 1, 1);


        public int AlohaCheckNum = 0;


        // public string CheckShortNum = "";


        internal static string GetCheckShortNum(int Id)
        {

            ulong Ch1 = (ulong)Id >> 20;
            ulong Ch2 = (ulong)Id & 0xFFFFF;
            return Ch1.ToString() + Ch2.ToString("0000");

        }

        public string CheckShortNum
        {
            get
            {
                ulong Ch1 = (ulong)AlohaCheckNum >> 20;
                ulong Ch2 = (ulong)AlohaCheckNum & 0xFFFFF;
                return Ch1.ToString() + Ch2.ToString("0000");
            }
        }

        //internal List<AlohaClientCard> 

        public List<AlohaClientCard> AlohaClientCardListCertifDisk = new List<AlohaClientCard>();
        

        internal List<AlohaClientCard> AlohaClientCardList
        {
            get
            {
                List<AlohaClientCard> Tmp = new List<AlohaClientCard>();

                foreach (AlohaTender tndr in Tenders.Where(a => AlohaTender.AlohaBallsTenderIds.Contains(a.TenderId)))
                {
                    AlohaClientCard CC = new AlohaClientCard()
                    {
                        TypeId = "03",
                        Number = tndr.CardNumber,
                        Prefix = tndr.CardPrefix,
                        Payment = Convert.ToInt32(tndr.Summ * 100),
                        BonusRemove = Convert.ToInt32(tndr.Summ * 100),
                    };
                    Tmp.Add(CC);
                }
                int Num = 0;

                //TODO: Продажа подарочных карт
               
                foreach (Dish d in Dishez.Where(a => Loyalty.LoyaltyBasik.PresentCardBCs.Contains(a.BarCode)))
                {
                    AlohaClientCard CC = new AlohaClientCard()
                    {
                        TypeId = "02",
                        Number = d.CardNumber,
                        Prefix = d.CardPrefix,

                        CardPrice = Convert.ToInt32(d.Price * 100)
                    };
                    Tmp.Add(CC);
                }


                if (AlohaClientCardListCertifDisk != null && AlohaClientCardListCertifDisk.Count > 0)
                {
                    List<AlohaClientCard> Tmp2 = AlohaClientCardListCertifDisk.Where(_card => Tmp.FirstOrDefault(_exists =>
                                _exists.TypeId == _card.TypeId &&
                                _exists.Number == _card.Number &&
                                _exists.Prefix == _card.Prefix &&
                                _exists.Payment == _card.Payment &&
                                _exists.BonusRemove == _card.BonusRemove
                    ) == null).ToList();
                    Tmp.AddRange(Tmp2);
                    Utils.ToCardLog("Addd to AlohaClientCardList from AlohaClientCardListCertifDisk ");
                }
                return Tmp;
            }
        }



        public object Clone()
        {

            Check ChkNew = (Check)this.MemberwiseClone();
            ChkNew.Dishez = new List<Dish> ();
            foreach (Dish d in ChkNew.Dishez)
            {
                ChkNew.Dishez.Add(d.Clone());
            }
            return ChkNew;
            
        }
    }

    public class AlohaClientCard
    {
        public string Prefix { set; get; }
        public string Number { set; get; }
        public string TypeId { set; get; }
        public Int32 BonusAdd { set; get; }
        public Int32 BonusRemove { set; get; }
        public Int32 Discount { set; get; }
        public Int32 Payment { set; get; }
        public Int32 CardPrice { set; get; }
    }

    public class StopListDish
    {
        public string Name;
        public int Count;
        public int BarCode;
        public int Reason { get; set; }
    }
    public class StopListDishReason : StopListDish
    {

        public StopListDishReason(StopListDish b)
        {
            Name = b.Name;
            Count = b.Count;

            BarCode = b.BarCode;
            Reason = b.Reason;
        }

        public CtrStopListReasonChooser mCtrStopListReasonChooser = null;


    }
    public class OrderDish
    {
        public OrderDish()
        { }
        public int Id = 0;
        public int barccode = 0;
        public int Vrouting = 0;
        public int CheckId = 0;
    }
}
