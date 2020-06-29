using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace PDiscountCard
{
    static class OrderDishTime
    {
        static internal Thread OrderDishTimeThread;

        internal static void Init()
        {
            OrderDishTimeThread = new Thread(OrderDishTimeQuereRun);
            OrderDishTimeThread.Name = "Order Dish Time";
            OrderDishTimeThread.Priority = ThreadPriority.Lowest;
            OrderDishTimeThread.Start();
            Utils.ToCardLog("Запустил поток Order Dish Time");
        }


        static List<OrderedChk> OrderDishTimeQuere = new List<OrderedChk>();

        static private int TimerSyncPoint = 0;
        static EventWaitHandle OrderDishTimeQuereWH = new AutoResetEvent(false);
        internal static void AddOrderDishTimeToQuere(Check Chk)
        {
            int Count = 0;

            OrderedChk OCh = new OrderedChk()
            {
                NonOrderedDishes = new List<OrderedDish>(),
                OrderedDishes = new List<OrderedDish> (),
                CheckId = Chk.AlohaCheckNum,
                OrderDt = DateTime.Now
            };
            

            foreach (Dish D in Chk.Dishez)
            {
                if (!D.IsOrdered)
                {
                    OCh.NonOrderedDishes.Add(new OrderedDish()
                    {
                        BarCode = D.BarCode,
                        EntryId = D.AlohaNum

                    });
                }

            }

            while ((Interlocked.CompareExchange(ref TimerSyncPoint, 1, 0) != 0) && (Count < 5000))
            {
                Thread.CurrentThread.Join(50);
                // Utils.ToCardLog("GetCashReg lock" + Count);
                Count += 50;
            }

            OrderDishTimeQuere.Add(OCh);

            Interlocked.CompareExchange(ref TimerSyncPoint, 0, 1);
            OrderDishTimeQuereWH.Set();

        }

        private static bool mExitOrderDishTimeThread = false;

        internal static void ExitOrderDishTimeThread()
        {
            mExitOrderDishTimeThread = true;
        }

        private static void OrderDishTimeQuereRun()
        {
            while (!mExitOrderDishTimeThread)
            {
                try
                {
                    TimerSyncPoint = 0;
                    
                    //OrderDishTimeQuereWH.WaitOne(10000);


                    Thread.Sleep(5000);
                    


                    int Count = 0;
                    while ((Interlocked.CompareExchange(ref TimerSyncPoint, 1, 0) != 0) && (Count < 5000))
                    {
                        Thread.CurrentThread.Join(50);
                        // Utils.ToCardLog("GetCashReg lock" + Count);
                        Count += 50;
                    }

                    if (OrderDishTimeQuere.Where(a => a.OrderDt.AddSeconds(7) < DateTime.Now).Count() > 0)
                    {
                        foreach (OrderedChk OCh in OrderDishTimeQuere.Where(a => a.OrderDt.AddSeconds(7) < DateTime.Now))
                        {
                            Check NewChk = AlohaTSClass.GetCheckById(OCh.CheckId);
                            
                            foreach (Dish d in NewChk.Dishez.Where(a => a.IsOrdered))
                            {
                                if (OCh.NonOrderedDishes.Where(a => a.EntryId == d.AlohaNum).Count() > 0)
                                {
                                    OCh.OrderedDishes.Add(OCh.NonOrderedDishes.Where(a => a.EntryId == d.AlohaNum).First());
                                }
                            }
                            if (OCh.OrderedDishes.Count > 0)
                            {
                                //SQL.ToSql.UpdateOrderTime(OCh);
                            }
                            OCh.NeedDelete = true;
                        }

                        OrderDishTimeQuere.RemoveAll(a => a.NeedDelete);
                        
                    }
                    
                   


                    Interlocked.CompareExchange(ref TimerSyncPoint, 0, 1);

                }
                catch
                {
                    Interlocked.CompareExchange(ref TimerSyncPoint, 0, 1);
                }
            }
            OrderDishTimeQuereWH.Close();
        }

    }


    internal class OrderedChk
    {
        internal int CheckId { set; get; }
        internal DateTime OrderDt  { set; get; }
        internal List<OrderedDish> OrderedDishes { set; get; }
        internal List<OrderedDish> NonOrderedDishes { set; get; }
        internal bool NeedDelete = false;

    }
    internal class OrderedDish
    {
        internal Guid Id { set; get; }
        internal int BarCode { set; get; }
        internal int EntryId { set; get; }



    }
}
