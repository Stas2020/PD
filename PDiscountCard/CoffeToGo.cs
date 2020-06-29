using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDiscountCard
{
    
    public  class CoffeToGo
    {
        internal RemoteOrderSrv.Service1 s2010Serv;
        public void Init()
        {
            s2010Serv = new RemoteOrderSrv.Service1() ;
            s2010Serv.GetNewOrdersCompleted += new RemoteOrderSrv.GetNewOrdersCompletedEventHandler(GetNewOrdersCompleted); 

        
        }

        
        

        internal bool RemoteOrderInProcess = false;
        internal static bool inOrders(int Id)
        {
            foreach (RemoteOrderSrv.OrderInfoForAloha oi in AlohaTSClass.RemoteOrders)
            {
                //Utils.ToLog("Проверяю наличие в базе заказов " + oi.OrderID);
                if (oi.ID == Id)
                {

                    return true;
                }
            }
            return false;
        }

        internal  bool inOrders2(string Id)
        {
            foreach (OrderInfo oi in AlohaTSClass.RemoteOrders2)
            {
                //Utils.ToLog("Проверяю наличие в базе заказов " + oi.OrderID);
                if (oi.OrderID == Id)
                {

                    return true;
                }
            }
            return false;
        }

        internal int AddRemoteOrderFromTCPCommand(RemoteOrderSrv.OrderInfoForAloha OI, out string ExeptionMessage, out int CheckId)
        {
            CheckId = 0;
            try
            {
                ExeptionMessage = "";
                Utils.ToCardLog("AddRemoteOrderFromTCPCommand");
                       int res = 0;
                        int i = 0;
                        do
                        {
                            res = AlohaTSClass.OrderDishes(OI,true, out CheckId);
                            i++;
                        } while (!((res > 0) || (i > 5)));
                        
                return res;
            }
            catch (Exception e)
            {
                Utils.ToCardLog("[Error] OrderAllItems " + e.Message);
                ExeptionMessage = e.Message;
                return -1;
            }
        }


        internal  void ProcessRemoteOrder(OrderInfo Oi)
        {
            if (AlohaTSClass.GetEmptyTable() > -1)
            {

                if (!inOrders2(Oi.OrderID))
                {
                    AlohaTSClass.RemoteOrders2.Add(Oi);
                    AlohaTSClass.OrderDishes2(Oi);

                }
            }
            else
            {
                Utils.ToLog("Все столы заняты. Отправил заказ в очередь.");
                AlohaTSClass.NotOrderRemoteOrders.Add(Oi);
            }
        }

        internal  bool ProcessNonOrderRemoteOrder(OrderInfo Oi)
        {
            if (AlohaTSClass.GetEmptyTable() > -1)
            {
                Utils.ToLog("Стол свободен. Делаю заказ из очереди");
                if (!inOrders2(Oi.OrderID))
                {
                    AlohaTSClass.RemoteOrders2.Add(Oi);
                    return AlohaTSClass.OrderDishes2(Oi);

                }
                else
                {
                    Utils.ToLog("Заказ в списке уже заказаных");
                }
                return true;
            }
            Utils.ToLog("Столы еще заняты");
            return false;

        }


        



        internal void GetNewOrdersCompleted(object sender, RemoteOrderSrv.GetNewOrdersCompletedEventArgs  e)
        {
            int CheckId = 0;
            RemoteOrderInProcess = true;
            try
            {
                RemoteOrderSrv.OrderInfoForAloha[] Ro = e.Result;
                if (Ro != null)
                {
                    //Utils.ToLog("Получил заказ асинхронно в количестве " + e.Result.Length + "  " + AlohainiFile.DepNum + " заказ: " + e.Result[0].ID + " " + e.Result[0].OrderID);
                    if (AlohaTSClass.GetEmptyTable() > -1)
                    {
                        foreach (RemoteOrderSrv.OrderInfoForAloha Oi in Ro)
                        {
                            if (!inOrders(Oi.ID))
                            {
                                Utils.ToLog("Обрабатываю заказ  " + Oi.ID + " " + Oi.OrderID);
                                AlohaTSClass.RemoteOrders.Add(Oi);

                                int res = 0;
                                int i = 0;
                                do
                                {
                                    Utils.ToLog("Вызов OrderDishes №" + i);
                                    res = AlohaTSClass.OrderDishes(Oi, false, out CheckId);
                                    Utils.ToLog("Результат Вызова " + res);

                                    i++;
                                } while (!((res == 0) || (i > 5)));
                                if (res != 0)
                                {
                                    AlohaTSClass.RemoteOrders.Remove(Oi);
                                }
                            }
                            else
                            {
                                Utils.ToLog("Заказ + " + Oi.ID + " в обработке");
                            }
                        }
                    }

                }
            }
            catch (Exception ee)
            {
                //   Utils.ToLog("Error s2010Serv_GetNewOrdersCompleted " + ee.Message + " INNER: " + ee.InnerException.Message);
                Utils.ToLog("Error s2010Serv_GetNewOrdersCompleted " + ee.Message);
                if (ee.InnerException != null)
                {
                    Utils.ToLog("INNER: " + ee.InnerException.Message);
                }
            }
            finally
            {
                RemoteOrderInProcess = false;
            }



        }
    }
}
