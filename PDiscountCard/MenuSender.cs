using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace PDiscountCard
{
    static internal  class MenuSender
    {
        
        static internal void MenuSenderInit()
        {
            SendMenuThread.Priority = ThreadPriority.Lowest;
            SendMenuThread.Start();
        }

        static Thread SendMenuThread = new Thread(SendMenu);
        static private void SendMenu()
        {
            try
            {
                Utils.ToCardLog("Запуск потока отправки меню");
                int PriceCange = Utils.GetActualPriceChangeNum();

                if (!iniFile.ExternalInterfaceEnable )
                {
                    Thread.Sleep(60000);
                }
                //Thread.Sleep(AlohaTSClass.GetTermNum() * 10000 * 60);
                //if (AlohaTSClass.ImMaster())
                
                {
                    if (!iniFile.ExternalInterfaceEnable)
                    {
                        try
                        {
                            StopListService.Service1 s1 = new StopListService.Service1();
                            bool b = s1.MenuModifEnabled(AlohainiFile.DepNum);

                            if (!b)
                            {
                                Utils.ToCardLog("С этого подразделения меню уже было недавно отправленно. Выхожу.");
                                return;
                            }
                        }
                        catch (Exception e)
                        {
                            Utils.ToCardLog("Error MenuModifEnabled " + e.Message);
                        }
                    }
                    Utils.ToCardLog("Отправляю текущее меню");
                    try
                    {
                      //  int PriceCange = Utils.GetActualPriceChangeNum();
                        if (PriceCange > -1)
                        {
                            Utils.ToCardLog("PriceChange= " + PriceCange.ToString());
                            int DepNum = iniFile.MenuSenderNum;
                            if (DepNum == 0)
                            {
                                DepNum = AlohainiFile.DepNum;
                            }

                            StopListService.AlohaMnu CurentMnu = AlohaTSClass.GetMenu(PriceCange,DepNum );
                            Utils.ToCardLog(" текущее меню");

                            StopListService.Service1 s1 = new StopListService.Service1();
                            s1.SeтMnuToServer(CurentMnu);
                            s1.Dispose();
                            Utils.ToCardLog("Отправил текущее меню" );
                          
                        }
                        else
                        {
                            Utils.ToCardLog("Не смог определить актуальный PriceChange. Меню не будет отправленно.");
                        }
                    }
                    catch (Exception e)
                    {
                        Utils.ToCardLog("[Error] Ошибка отправления меню на сервер " + e.Message);
                    }

                    try
                    {
                        StopListService.Service1 s1 = new StopListService.Service1();

                        List<StopListService.DishN> d = AlohaTSClass.GetAllItms();
                        s1.SetDishList(d.ToArray());
                        s1.Dispose();
                    }
                    catch (Exception e)
                    {
                        Utils.ToCardLog("[Error] Ошибка отправления списка блюд на сервер " + e.Message);
                    }

                    Utils.ToCardLog("Отправил текущее меню");


                }
            }
            catch(Exception e)

            {
                Utils.ToCardLog("Error  SendMenu " + e.Message);
            }
        }
        
    }
}
