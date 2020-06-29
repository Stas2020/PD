using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text;
using System.Threading;
using System.Linq.Expressions;
using System.IO;
using AlohaGalClient.DbSrv;

namespace AlohaGalClient
{
    public static class MainClass
    {
        public static GallerySrvs sv = new GallerySrvs();
        public static AlohaFlySrvs AlFl = new AlohaFlySrvs();
        public static void Init()
        {
            try
            {
                FiskalDrivers.FiskalDriver.CreateFiskalDriver(1);
                Utils.ToLog("FiskalDrivers.FiskalDriver.CreateFiskalDriver(1) ok ");
                FiskalDrivers.FiskalDriver.Connect(1);
                Utils.ToLog("FiskalDrivers.FiskalDriver.Connect(1) ok ");
                /*
                sv = new GallerySrvs();
                sv.StartOrderLisenter();
                */

                AlFl.StartOrderLisenter();
            }

            catch (Exception e)
            {
                Utils.ToLog("Ошибка при активации драйвера фискального принтера " + e.Message);
            }

        }
        public static void Close()
        {
            AlFl.Stop();
        }

        public static void ZReport()
        {
            FiskalDrivers.FiskalDriver.PrintZReport();
 

        }

        public static void XReport()
        {
            FiskalDrivers.FiskalDriver.PrintXReport();
        }

        public static  void ReturnSale(long checkId)
        {
            var q = new DBQuery();
            var Or = q.GetCheckById(checkId);
            var ch = AlohaFlySrvs.GetFiskalCheckFromOrder(Or);
            ch.IsVoid = true;
            FiskalDrivers.FiskalDriver.CloseCheck(ch);
 
        }

        public static void ReturnSaleToGo(long checkId)
        {
            var q = new DBQuery();
            var Or = q.GetCheckByIdToGo(checkId);
            var ch = AlohaFlySrvs.GetFiskalCheckFromOrder(Or);
            ch.IsVoid = true;
            FiskalDrivers.FiskalDriver.CloseCheck(ch);

        }
    }


    static class Utils
    {
        static EventWaitHandle EditCheckWaitHandle = new AutoResetEvent(true);
        public static void ToLog(string Str)
        {
            EditCheckWaitHandle.WaitOne();
            try
            {
                //if (!Directory.Exists( @""))
                using (StreamWriter sw = new StreamWriter(@"C:\aloha\check\OrderToAlohaLog.txt", true))
                {
                    sw.WriteLine(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss ") + " [" + Thread.CurrentThread.ManagedThreadId + "] " + Str);
                }

            }
            catch
            {

            }
            EditCheckWaitHandle.Set();

        }

    }

    public class AlohaFlySrvs
    {
        Thread AlohaFlyFiskalWorkThread;


        public void StartOrderLisenter()
        {
            try
            {
                Utils.ToLog("[initData]Инициализация AlohaFlySrvs");
                AlohaFlyFiskalWorkThread = new Thread(CheckCloseQuere);
                AlohaFlyFiskalWorkThread.Start();
            }
            catch (Exception e)
            {
                Utils.ToLog("[initData]Error Инициализация AlohaFlySrvs " + e.Message);
            }
        }


        internal void Stop()
        {
            mCheckCloseThread = true;
        }


        public  static FiskalDrivers.FiskalCheck GetFiskalCheckFromOrder(OrderFlight ord)
        {
            FiskalDrivers.FiskalCheck Tmp = new FiskalDrivers.FiskalCheck();
            Tmp.CheckNum = ord.Id.ToString();
            Tmp.TimeofOpen = ord.DeliveryDate;
            Tmp.Cassir = ord.AirCompany.Name;
            Tmp.Waiter = ord.FlightNumber;
            Tmp.FlyCheck = true;
            foreach (var oi in ord.DishPackages.Where(a => a.TotalPrice > 0 && !a.Dish.IsAlcohol))
            {
                FiskalDrivers.FiskalDish fd = new FiskalDrivers.FiskalDish()
                {
                    Name = oi.DishName,
                    Price = (double)oi.TotalPrice,
                    Quantity = (double)oi.Amount
                };
                Tmp.Dishes.Add(fd);
            }
            foreach (var oi in ord.DishPackages.Where(a => a.TotalPrice > 0 && a.Dish.IsAlcohol))
            {
                FiskalDrivers.FiskalDish fd = new FiskalDrivers.FiskalDish()
                {
                    Name = "Открытый напиток",
                    Price = (double)oi.TotalPrice,
                    Quantity = (double)oi.Amount
                };
                Tmp.Dishes.Add(fd);
            }
            double DSumm = (double)ord.DishPackages.Sum(a => a.TotalPrice * a.Amount);
            double ExCh = DSumm * (double)ord.ExtraCharge/100;
            Tmp.Discount = (double)ord.DiscountSumm;
            if ((Tmp.Discount > 0) && (ExCh > 0))
            {
                if (Tmp.Discount > ExCh)
                {
                    Tmp.Discount -= ExCh;
                }
                else
                {

                    Tmp.Charge = ExCh - Tmp.Discount;
                    Tmp.Discount = 0;
                }
            }
            else if (ExCh > 0)
            {
                Tmp.Charge = ExCh;
            }

            double DSummTotal = DSumm - Tmp.Discount + Tmp.Charge;
            var op = ord.AirCompany.PaymentType;
            //OrderToAlohaSrv.OrderPayment op = ord.OrderPayments;
            FiskalDrivers.FiskalPayment fp = new FiskalDrivers.FiskalPayment()
            {
                Name = op.Name,
                Summ = DSummTotal,
                PaymentType = (int)op.FiskalId
                //  NonFiskalPayment = op.TNDR.
            };
            if (fp.PaymentType==0)
            {
                fp.Name = "Наличные";
            }
            Tmp.Payments.Add(fp);

            Tmp.Summ = DSummTotal;
            return Tmp;
        }


        public static FiskalDrivers.FiskalCheck GetFiskalCheckFromOrder(OrderToGo ord)
        {
            FiskalDrivers.FiskalCheck Tmp = new FiskalDrivers.FiskalCheck();
            Tmp.CheckNum = ord.Id.ToString();
            Tmp.TimeofOpen = ord.DeliveryDate;
            /*
            Tmp.Cassir = ord.AirCompany.Name;
            Tmp.Waiter = ord.FlightNumber;
             * */
            Tmp.FlyCheck = false;
            double tmpSumm = 0;
            foreach (var oi in ord.DishPackages.Where(a => a.TotalPrice > 0 && !a.Dish.IsAlcohol))
            {
                FiskalDrivers.FiskalDish fd = new FiskalDrivers.FiskalDish()
                {
                    Name = oi.DishName,
                    Price = (double)oi.TotalPrice * (1 - (double)ord.DiscountPercent / 100),
                    Quantity = (double)oi.Amount
                };
                tmpSumm += fd.Price * fd.Quantity;
                Tmp.Dishes.Add(fd);
            }

            if (ord.DiscountPercent > 0)
            {
                Tmp.Discount = (double)(ord.DishPackages.Sum(a => a.TotalPrice * a.Amount) * (ord.DiscountPercent / 100));
                Tmp.DiscountName = "Скидка ";
            }
            var diff = tmpSumm - ((double)ord.DishPackages.Sum(a => a.TotalPrice * a.Amount) - Tmp.Discount);
            if (Math.Abs(diff) > 0.001)
            {
                if (Tmp.Dishes.Any(a => a.Price * a.Quantity > diff))
                {
                    var d = Tmp.Dishes.Where(a => a.Price * a.Quantity > diff).FirstOrDefault();
                    d.Price = d.Price - diff / d.Quantity;
                }
            }


            if (ord.DeliveryPrice > 0)
            {
                FiskalDrivers.FiskalDish fd = new FiskalDrivers.FiskalDish()
                {
                    Name = "Доставка",
                    Price = (double)ord.DeliveryPrice,
                    Quantity = 1
                };
                Tmp.Dishes.Add(fd);
            }

           // double DSumm = (double)ord.DishPackages.Sum(a => a.TotalPrice * a.Amount) + (double)ord.DeliveryPrice;
            double DSumm = (double)Tmp.Dishes.Sum(a => a.Price * a.Quantity);

            /*
            double ExCh = 0;
            Tmp.Discount =  0;
            if ((Tmp.Discount > 0) && (ExCh > 0))
            {
                if (Tmp.Discount > ExCh)
                {
                    Tmp.Discount -= ExCh;
                }
                else
                {

                    Tmp.Charge = ExCh - Tmp.Discount;
                    Tmp.Discount = 0;
                }
            }
            else if (ExCh > 0)
            {
                Tmp.Charge = ExCh;
            }
            */

            //double DSummTotal = DSumm - Tmp.Discount + Tmp.Charge;
            var op = ord.PaymentType;
            //OrderToAlohaSrv.OrderPayment op = ord.OrderPayments;
            FiskalDrivers.FiskalPayment fp = new FiskalDrivers.FiskalPayment()
            {
                Name = op.Name,
                Summ = DSumm,
                PaymentType = (int)op.FiskalId
                //  NonFiskalPayment = op.TNDR.
            };
            Tmp.Payments.Add(fp);

            Tmp.Summ = DSumm;
            return Tmp;
        }

        static bool mCheckCloseThread = false;
        static void CheckCloseQuere()
        {

            Utils.ToLog("CheckCloseQuere mCheckCloseThread=" + mCheckCloseThread);

            //DateTime sDt = DateTime.Now.AddMonths(-1);
            //DateTime eDt = DateTime.Now.AddMonths(1);
            while (!mCheckCloseThread)
            {
                try
                {
                    var q = new DBQuery();
                    var ToFlyChecks = q.GetNonClosingCheck();

                    if (!(ToFlyChecks == null))
                    {
                        foreach (var ch in ToFlyChecks)
                        {
                            Utils.ToLog("Get FCheck");
                            FiskalDrivers.FiskalCheck Fchk = GetFiskalCheckFromOrder(ch);

                            if (Fchk.Dishes.Count() == 0)
                            {
                                if (!q.UpdateClosingCheck(ch, false, true))
                                {
                                    mCheckCloseThread = true;
                                }
                                continue;
                            }
                            if (ch.NeedPrintFR)
                            {
                                if (Fchk.Payments.Where(a => a.PaymentType > 0).Count() == 0)
                                {
                                    if (!q.UpdateClosingCheck(ch, true, false))
                                    {
                                        mCheckCloseThread = true;
                                    }
                                }
                                else
                                {
                                    if (!q.UpdateClosingCheck(ch, true, false))
                                    {
                                        mCheckCloseThread = true;
                                    }
                                    else
                                    {
                                        bool res = FiskalDrivers.FiskalDriver.CloseCheck(Fchk);
                                    }

                                }
                            }
                        }
                    }

                    var ToGoChecks = q.GetNonToGoClosingCheck();

                    if (!(ToGoChecks == null))
                    {
                        foreach (var ch in ToGoChecks)
                        {
                            Utils.ToLog("Get FCheckToGo");
                            FiskalDrivers.FiskalCheck Fchk = GetFiskalCheckFromOrder(ch);

                            if (Fchk.Dishes.Count() == 0)
                            {
                                Utils.ToLog("Fchk.Dishes.Count() == 0");
                                if (!q.UpdateClosingCheck(ch, false, true))
                                {
                                    mCheckCloseThread = true;
                                }
                                continue;
                            }
                            if (ch.NeedPrintFR)
                            {
                                Utils.ToLog("ch.NeedPrintFR");
                                /*
                                if (Fchk.Payments.Where(a => a.PaymentType > 0).Count() == 0)
                                {
                                    Utils.ToLog("Fchk.Payments.Where(a => a.PaymentType > 0).Count() == 0");
                                    if (!q.UpdateClosingCheck(ch, true, false))
                                    {
                                        mCheckCloseThread = true;
                                    }
                                }
                                else
                                 * */
                                {
                                    if (!q.UpdateClosingCheck(ch, true, false))
                                    {
                                        mCheckCloseThread = true;
                                    }
                                    else
                                    {
                                        Utils.ToLog("Print");
                                        bool res = FiskalDrivers.FiskalDriver.CloseCheck(Fchk);
                                    }

                                }
                            }


                        }
                        /*
                        else if (Or.NeedPrintPrecheck)
                        {
                            Utils.ToLog("NeedPrintPrecheck");
                            bool res = FiskalDrivers.FiskalDriver.PrintPreCheck(Fchk);
                            if (res)
                            {
                                if (!q.UpdateClosingCheck(Or, false, true))
                                {
                                    mCheckCloseThread = true;
                                }
                            }
                        }
                             * */
                    }
                    Thread.Sleep(2000);
                }
                catch (Exception e)
                {
                    Utils.ToLog("Error CheckCloseQuere " + e.Message);
                }
                /*
                try
                {
                    OrderToAlohaSrv.Order Or = OrderToAlohaSrv.ToSql.GetNeedPrecheck();
                    //  Utils.ToLog("OrderToAlohaSrv.Order Or = OrderToAlohaSrv.ToSql.GetNonCloseCheck();");
                    if (!(Or == null))
                    {
                        Utils.ToLog("Get PreCheck");
                        FiskalDrivers.FiskalCheck Fchk = GetFiskalCheckFromOrder(Or);

                        bool res = FiskalDrivers.FiskalDriver.PrintPreCheck(Fchk);
                        if (res)
                        {
                            OrderToAlohaSrv.ToSql.UpdatePrintPreCheck(Or.StNum.Value);
                        }
                    }
                    Thread.Sleep(2000);
                }
                catch (Exception e)
                {
                    Utils.ToLog("Error CheckCloseQuere " + e.Message);
                }
                */

            }

        }

    }

    public class GallerySrvs
    {

        Thread GalleryFikalWorkThread;
        public void StartOrderLisenter()
        {
            string SrvName = @"localhost\sqlexpress";
            if (true)
            {
                try
                {
                    Utils.ToLog("[initData]Инициализация галлереи");
                    //string SrvName = @"Data Source=localhost\sqlexpress;Initial Catalog=AlohaGallery;User ID=AlohaOper;Password=Qdrjnty^pp1223";

                    OrderToAlohaSrv.MainClass.Init(64788, SrvName);
                }
                catch (Exception e)
                {
                    Utils.ToLog("Error GallerySrvs.StartOrderLisenter" + e.Message);
                }
                Utils.ToLog("[initData] Инициализация галлереи iniFile.GalleryCloseCheckEnabled =" + SrvName);
                if (true)
                {
                    GalleryFikalWorkThread = new Thread(CheckCloseQuere);
                    GalleryFikalWorkThread.Start();
                }
            }
        }


        internal void Stop()
        {
            mCheckCloseThread = true;
        }


        static FiskalDrivers.FiskalCheck GetFiskalCheckFromOrder(OrderToAlohaSrv.Order ord)
        {
            FiskalDrivers.FiskalCheck Tmp = new FiskalDrivers.FiskalCheck();
            Tmp.CheckNum = ord.StNum.ToString();
            Tmp.TimeofOpen = ord.TimeofShipping.Value;
            Tmp.Cassir = ord.CompanyName;
            Tmp.Waiter = ord.BortName;
            Tmp.FlyCheck = true;
            foreach (OrderToAlohaSrv.OrderItem oi in ord.OrderItems.Where(a => !a.Deleted.Value && a.Price > 0 && !a.Alk))
            {
                FiskalDrivers.FiskalDish fd = new FiskalDrivers.FiskalDish()
                {
                    Name = OrderToAlohaSrv.ToSql.SQLGetDishName(oi.Barcode.Value),
                    Price = (double)oi.Price.Value,
                    Quantity = oi.Quantity.Value
                };
                Tmp.Dishes.Add(fd);
            }

            Tmp.Discount = (double)ord.ComplexDisc + (double)ord.StDiscount;
            if ((Tmp.Discount > 0) && (ord.Margin > 0))
            {
                if (Tmp.Discount > (double)ord.Margin)
                {
                    Tmp.Discount -= (double)ord.Margin;
                }
                else
                {

                    Tmp.Charge = (double)ord.Margin - Tmp.Discount;
                    Tmp.Discount = 0;
                }
            }
            else if (ord.Margin > 0)
            {
                Tmp.Charge = (double)ord.Margin;
            }

            foreach (OrderToAlohaSrv.OrderPayment op in ord.OrderPayments)
            {
                //OrderToAlohaSrv.OrderPayment op = ord.OrderPayments;
                FiskalDrivers.FiskalPayment fp = new FiskalDrivers.FiskalPayment()
                {
                    Name = op.TNDR.Name,
                    Summ = (double)op.Summ.Value,
                    PaymentType = op.TNDR.FisckalId.Value,
                    //  NonFiskalPayment = op.TNDR.
                };
                Tmp.Payments.Add(fp);
            }
            Tmp.Summ = (double)(ord.Summ - ord.StDiscount - ord.ComplexDisc + ord.Margin);
            return Tmp;

        }

        static bool mCheckCloseThread = false;
        static void CheckCloseQuere()
        {
            Utils.ToLog("CheckCloseQuere mCheckCloseThread=" + mCheckCloseThread);
            while (!mCheckCloseThread)
            {
                try
                {
                    OrderToAlohaSrv.Order Or = OrderToAlohaSrv.ToSql.GetNonCloseCheck();

                    if (!(Or == null))
                    {
                        Utils.ToLog("Get FCheck");
                        FiskalDrivers.FiskalCheck Fchk = GetFiskalCheckFromOrder(Or);

                        if ((Fchk.Payments.Where(a => a.PaymentType > 0).Count() == 0) || Fchk.Dishes.Count() == 0)
                        {
                            OrderToAlohaSrv.ToSql.UpdateCloseCheck(Or.StNum.Value);
                            continue;
                        }
                        bool res = FiskalDrivers.FiskalDriver.CloseCheck(Fchk);
                        if (res)
                        {
                            OrderToAlohaSrv.ToSql.UpdateCloseCheck(Or.StNum.Value);
                        }
                    }
                    Thread.Sleep(2000);
                }
                catch (Exception e)
                {
                    Utils.ToLog("Error CheckCloseQuere " + e.Message);
                }

                try
                {
                    OrderToAlohaSrv.Order Or = OrderToAlohaSrv.ToSql.GetNeedPrecheck();
                    //  Utils.ToLog("OrderToAlohaSrv.Order Or = OrderToAlohaSrv.ToSql.GetNonCloseCheck();");
                    if (!(Or == null))
                    {
                        Utils.ToLog("Get PreCheck");
                        FiskalDrivers.FiskalCheck Fchk = GetFiskalCheckFromOrder(Or);

                        bool res = FiskalDrivers.FiskalDriver.PrintPreCheck(Fchk);
                        if (res)
                        {
                            OrderToAlohaSrv.ToSql.UpdatePrintPreCheck(Or.StNum.Value);
                        }
                    }
                    Thread.Sleep(2000);
                }
                catch (Exception e)
                {
                    Utils.ToLog("Error CheckCloseQuere " + e.Message);
                }


            }

        }
    }
}
