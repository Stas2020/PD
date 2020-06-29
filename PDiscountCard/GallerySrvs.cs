using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace PDiscountCard
{
    /*
    public class GallerySrvs
    {

        Thread GalleryFikalWorkThread;
        public  void StartOrderLisenter()
        {
            if (iniFile.GalleryOrderEnabled)
            {
                try
                {
                    Utils.ToLog("[initData]Инициализация галлереи");

                    OrderToAlohaSrv.MainClass.Init(64788,iniFile.GallerySqlServerName);
                }
                catch (Exception e)
                {
                    Utils.ToLog("Error GallerySrvs.StartOrderLisenter" + e.Message);
                }
                Utils.ToLog("[initData] Инициализация галлереи iniFile.GalleryCloseCheckEnabled =" + iniFile.GalleryCloseCheckEnabled);
                if (iniFile.GalleryCloseCheckEnabled)
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
            foreach (OrderToAlohaSrv.OrderItem oi in ord.OrderItems.Where(a=>!a.Deleted.Value && a.Price>0 && !a.Alk))
            { 
                FiskalDrivers.FiskalDish fd = new FiskalDrivers.FiskalDish ()
                {
                    Name = OrderToAlohaSrv.ToSql.SQLGetDishName(oi.Barcode.Value),
                    Price = (double)oi.Price.Value,
                    Quantity = oi.Quantity.Value
                };
                Tmp.Dishes.Add(fd);
            }

            Tmp.Discount = (double)ord.ComplexDisc + (double)ord.StDiscount;
            if ((Tmp.Discount > 0)&&(ord.Margin>0))
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
                    Utils.ToLog("OrderToAlohaSrv.Order Or = OrderToAlohaSrv.ToSql.GetNonCloseCheck();" );
                    if (!(Or == null))
                    {
                        FiskalDrivers.FiskalCheck Fchk = GetFiskalCheckFromOrder(Or);

                        if ((Fchk.Payments.Where(a => a.PaymentType > 0).Count() == 0) || Fchk.Dishes.Count()==0)
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
                catch(Exception e)
                {
                    Utils.ToLog("Error CheckCloseQuere " + e.Message);
                }

                try
                {
                    OrderToAlohaSrv.Order Or = OrderToAlohaSrv.ToSql.GetNeedPrecheck();
                    Utils.ToLog("OrderToAlohaSrv.Order Or = OrderToAlohaSrv.ToSql.GetNonCloseCheck();");
                    if (!(Or == null))
                    {
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
     * */
}
