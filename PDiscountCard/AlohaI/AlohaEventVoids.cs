using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.ComponentModel;
using System.IO;
using System.Linq;
using PDiscountCard.IIKO_Card;

namespace PDiscountCard
{
    static public class AlohaEventVoids
    {
        internal static void TestPrint()
        {
            ShtrihCommandBlock Sb = new ShtrihCommandBlock();
            for (int i = 0; i < 200; i++)
            {
                Sb.PrintString(i.ToString());
            }
            Sb.CommandBlockToQwery();

        }


        internal static void PrintCurentPrecheckOnFR()
        {
            try
            {
                Utils.ToCardLog("PrintCurentPrecheckOnFR " );
                int chId = (int)AlohaTSClass.GetCurentCheckId();
                string s = AlohaTSClass.GetFRPredcheck(chId);
                ToShtrih.PrintCardCheck(s,false);
            }
            catch(Exception e)
            {
                Utils.ToCardLog("Error PrintCurentPrecheckOnFR " +e.Message);
            }
        }


        internal static void WriteTimeOfSendInThread(int TableId, int CheckId, int QueueId, int EmployeeId, int ModeId, List<int> CurentItms, List<Dish> OrderredItms, int EmployeeOwner)
        {
            try
            {
                /*   
             
                   Utils.ToCardLog("WriteTimeOfSend dd");
                   foreach (Dish d in Chk.Dishez)
                   {
                       Utils.ToCardLog(d.AlohaNum.ToString());
                   }
                   */

                if (OrderredItms.Count > CurentItms.Count)
                {
                    Utils.ToCardLog("Not First Order CheckId = " + CheckId + ". Orderred Items: ");
                    foreach (Dish OrdItm in OrderredItms)
                    {
                        Utils.ToCardLog(String.Format("EntryId {0} , BarCode {1}", OrdItm.AlohaNum, OrdItm.BarCode));
                    }
                    //int Count = OrderredItms.Count - CurentItms.Count;
                    List<int> AlreadyOrderInSQL = SQL.ToSql.GetOrderTimeEntry(); //Уже в базе
                    OrderredItms.RemoveAll(a => AlreadyOrderInSQL.Contains(a.AlohaNum));
                    Utils.ToCardLog("Not First Order. Orderred Items After Remove: ");
                    foreach (Dish OrdItm in OrderredItms)
                    {
                        Utils.ToCardLog(String.Format("EntryId {0} , BarCode {1}", OrdItm.AlohaNum, OrdItm.BarCode));
                    }

                }


                List<OrderedDish> Tmp = new List<OrderedDish>();
                foreach (int Bc in CurentItms)
                {
                    OrderedDish Od = new OrderedDish()
                    {
                        BarCode = Bc
                    };
                    try
                    {
                        Dish OrderD = OrderredItms.Where(a => a.BarCode == Od.BarCode).Last();
                        Od.EntryId = OrderD.AlohaNum;
                        OrderredItms.Remove(OrderD);
                        Tmp.Add(Od);
                    }
                    catch
                    {

                    }


                }

                //List<int> Itms = AlohaTSClass.GetCurrentItemsPerebor();
                SQL.ToSql.InsertOrderTime(Tmp, CheckId, TableId, DateTime.Now, AlohainiFile.BDate, QueueId, EmployeeId, ModeId, EmployeeOwner);
                //OrderDishTime.AddOrderDishTimeToQuere(AlohaTSClass.GetCheckById(CheckId));
            }
            catch (Exception e)
            {
                Utils.ToCardLog("[Error] WriteTimeOfSend" + e.Message);
            }
        }



        internal static void WriteTimeOfSend(int TableId, int CheckId, int QueueId, int EmployeeId, int ModeId)
        {
            if (!iniFile.SQLDisabled)
            {
                Check Chk = AlohaTSClass.GetCheckById(CheckId);
                List<int> CurentItms = AlohaTSClass.GetCurrentItems(TableId); // Заказанные сейчас без Entity. Это делаем в потоке события т.к. здесь запрос текущего состояния
                List<Dish> OrderredItms = Chk.Dishez.Where(a => a.IsOrdered).ToList();

                //Уезжаем в другой поток, чтобы не тормозила из за SQL
                System.Threading.Thread MyThread =
                     new System.Threading.Thread(delegate() { WriteTimeOfSendInThread(TableId, CheckId, QueueId, EmployeeId, ModeId, CurentItms, OrderredItms, Chk.Waiter); });
                MyThread.Start();
            }


        }

        internal static void ShowfrmCard()
        {

            if (MainClass.CurentfrmCardMoover != null)
            {
                MainClass.CurentfrmCardMoover = null;
            }
            MainClass.CurentfrmCardMoover = new frmCardMoover();
            MainClass.CurentfrmCardMooverEnable = true;
            MainClass.CurentfrmCardMoover.Show();
        }

        internal static void HidefrmCard()
        {

            if (MainClass.CurentfrmCardMoover != null)
            {
                MainClass.CurentfrmCardMoover.Close();
            }
            MainClass.CurentfrmCardMooverEnable = false;
            MainClass.CurentfrmCardMoover = null;
        }

        internal static void AddScaleDish2(string Arg)
        {
            int BarCode = Convert.ToInt32(Arg.Substring(5));
            string DName = AlohaTSClass.GetDishName(BarCode);
            Scale.Scale2.GetItmWeight(BarCode, DName);
        }
        /*
        internal static void AddScaleDish(string Arg)
        {
            int BarCode = Convert.ToInt32(Arg.Substring(5));

            int W = PScale.ScaleWeight();
            PScale.DisConnect();

            Utils.ToCardLog("Вес " + W.ToString() + " Баркод " + BarCode.ToString());

            while (W < 20)
            {
                if (W == -1)
                {
                    return;
                }
                ScaleFrm Sf = new ScaleFrm();
                Sf.SetMsg("Поставьте товар на весы и нажмите Ок");
                Sf.SetPan1();
                Sf.ShowDialog();
                if (Sf.Cancel)
                {
                    return;
                }

                Sf.Dispose();
                W = PScale.ScaleWeight();
                PScale.DisConnect();
            }

            if ((W - iniFile.ScaleTareWeight) < 0)
            {
                ScaleFrm Sf = new ScaleFrm();
                Sf.SetMsg("Малый вес тары. Вес тары в настройках " + iniFile.ScaleTareWeight + "гр");
                Sf.SetPan1();
                Sf.ShowDialog();
                return;
            }

            AlohaTSClass.AddScaleDish(BarCode, W - iniFile.ScaleTareWeight);
        }
        */

        private static bool AlcCheck(Check Chk)
        {
            Utils.ToCardLog("AlcCheck ");
            foreach (AlohaTender Tnd in Chk.CreditPayments())
            {
                Utils.ToCardLog(String.Format("Tnd {0}, Prefix {1}", Tnd.TenderId, Tnd.CardPrefix));
            }


            if (Loyalty.LoyaltyBasik.GetAirBonusTenders(Chk).Count > 0)
            {

                Utils.ToCardLog("AlcCheck contain bonus payment ");
                if (Chk.Dishez.Select(a => a.BarCode).Intersect(AlohaTSClass.AlcList).Count() > 0)
                {
                    Utils.ToCardLog("AlcCheck Contains Alc");
                    string s = "Чек оплаченный баллами не может содержать" + Environment.NewLine;
                    s += "алкогольные напитки" + Environment.NewLine;
                    s += "Удалите оплату и перенесите алкогольные напитки" + Environment.NewLine;
                    s += "на другой чек" + Environment.NewLine;

                    MessageForm mfrm = new MessageForm(s);
                    mfrm.OnlyOk();
                    mfrm.ShowDialog();
                    return false;
                }


            }
            return true;
        }

        public static void CloseCheck()
        {
            Utils.ToCardLog("CloseCheckFromActiv");

            AlohaTSClass.CheckWindow();
            Check Chk = AlohaTSClass.GetCheckById((int)AlohaTSClass.AlohaCurentState.CheckId);
            /*
            if (!AlcCheck(Chk))
            {
                return;
            }
            */
            if (Chk.HasUnorderedItems)
            {
                if (iniFile.AutoOrderBeforeWaiterClose)
                {
                    AlohaTSClass.OrderAllDishez(AlohaTSClass.AlohaCurentState.TerminalId, (int)AlohaTSClass.AlohaCurentState.CheckId, (int)AlohaTSClass.AlohaCurentState.TableId);
                }
                else
                {
                    frmAllertMessage Mf = new frmAllertMessage("В чеке есть незаказаные блюда. ");
                    Utils.ToLog("В чеке есть незаказаные блюда. Выхожу");
                    Mf.ShowDialog();
                    return;
                }
            }
            AlohaTSClass.CloseCheck(AlohaTSClass.GetTermNum(), Chk.AlohaCheckNum);


        }


        private static int GetTrueBitCount(int a)
        {
            byte n = 0;
            while (a != 0)
            {
                a = a & (a - 1);
                n++;
            }
            return (int)n;
        }
        /*
        public static void CloseByWaiter(int TenderType=0)
        {
            Utils.ToCardLog("CloseByWaiter TenderType = " + TenderType.ToString());



            AlohaTSClass.CheckWindow();
            Check Chk = AlohaTSClass.GetCheckById((int)AlohaTSClass.AlohaCurentState.CheckId);


            //Это запрет на алкочеки
            if (!iniFile.FRModeDisabled)
            {
                if (!AlcCheck(Chk))
                {
                    return;
                }

            }

            if (Chk.HasUnorderedItems)
            {
                if (iniFile.AutoOrderBeforeWaiterClose)
                {
                    
                   bool OrderRes =  AlohaTSClass.OrderAllDishez(AlohaTSClass.AlohaCurentState.TerminalId, (int)AlohaTSClass.AlohaCurentState.CheckId, (int)AlohaTSClass.AlohaCurentState.TableId);
                   if (!OrderRes)
                   {
                       frmAllertMessage Mf = new frmAllertMessage("Не могу заказать блюда. Попробуйте еще раз. Либо закажите самостоятельно. Если ошибка будет повторяться свяжитесь со службой техподдержки для перезагрузки Алохи.");
                       Utils.ToLog("Не могу заказать блюда. Выхожу");
                       Mf.ShowDialog();
                       return;
                   }
                }
                else
                {
                    frmAllertMessage Mf = new frmAllertMessage("В чеке есть незаказаные блюда. ");
                    Utils.ToLog("В чеке есть незаказаные блюда. Выхожу");
                    Mf.ShowDialog();
                    return;
                }
            }
            int PaymentsCount = 0;
            if (TenderType == 0)
            {
                PaymentsCount =
                    Convert.ToByte(iniFile.FCCEnable) +
                    Convert.ToByte((iniFile.InPasEnabled) || (CreditCardAlohaIntegration.CreditCardConnectorEnabled)) * 2 +
                    Convert.ToByte(iniFile.CreditCloseByWiterEnabled && (Chk.TableNumber > 190)) * 4;

                Utils.ToCardLog("PaymentsCount= " + PaymentsCount.ToString());
                //BitArray Ba = new BitArray(new byte[] { PaymentsCount });


                if (PaymentsCount == 0)
                {
                    frmAllertMessage Mf = new frmAllertMessage("Нет подключенных модулей оплаты");
                    Mf.ShowDialog();
                    return;
                }

                if (GetTrueBitCount(PaymentsCount) > 1)
                {
                    Utils.ToCardLog("GetTrueBitCount(PaymentsCount) > 1");
                    PDiscountCard.AlohaI.WndPaymentSelect WndSelect = new AlohaI.WndPaymentSelect();
                    WndSelect.SetBtnsVis(PaymentsCount);
                    WndSelect.WaiterName = AlohaTSClass.GetCurentWaterName();
                    Utils.ToCardLog("Before WndSelect.ShowDialog();");
                    WndSelect.ShowDialog();
                    Utils.ToCardLog("After WndSelect.ShowDialog();");
                    PaymentsCount = WndSelect.Result;
                    
                }
            }
            else
            {
                PaymentsCount = TenderType;
            }

            Utils.ToCardLog("PaymentsCount after select = " + PaymentsCount.ToString());
            if (PaymentsCount == 1)
            {
                FCC.SetBill();
            }
            else if (PaymentsCount == 2)
            {
                if (iniFile.InPasEnabled)
                {
                    DualConnector.DualConnectorMain.Sale();
                }
                //   else if (iniFile.Arcus2Enabled)
                else if (CreditCardAlohaIntegration.CreditCardConnectorEnabled)
                {
                    string err = "";

                    CreditCardAlohaIntegration.RunOper(Chk);
                    //DualConnector.DualConnectorMain.Sale();
                }
            }
            else if (PaymentsCount == 4)
            {
                AlohaTSClass.LogOut();
                RemoteCloseCheck.AddRemoteChkToQuere(Chk.AlohaCheckNum, 30, Chk.Waiter, Chk.Summ);
            }

            Utils.ToCardLog("Exit from CloseByWaiter TenderType = " + TenderType.ToString());
        }
        */

        /*
        static Thread CloseByWaiterThread;
        public static void CloseByWaiter(int TenderType = 0)
        {
            try
            {
                Utils.ToCardLog("CloseByWaiter TenderType = " + TenderType.ToString());
                CloseByWaiterThread = new Thread(new ParameterizedThreadStart(CloseByWaiterAsinc));
                CloseByWaiterThread.Start(TenderType);
                Utils.ToCardLog("CloseByWaiter end TenderType = " + TenderType.ToString());
                GC.Collect();
            }
            catch (Exception e)
                { 
            Utils.ToCardLog("Error CloseByWaiter " +e.Message);
            }

        }
        */

        //private static void CloseByWaiter(object  obTenderType)
            public static void CloseByWaiter( int obTenderType=0)
        {
            

            int TenderType = Convert.ToInt32(obTenderType);
            Utils.ToCardLog("CloseByWaiterAsinc TenderType = " + TenderType.ToString());
            AlohaTSClass.CheckWindow();
            Check Chk = AlohaTSClass.GetCheckById((int)AlohaTSClass.AlohaCurentState.CheckId);


            //Это запрет на алкочеки
            if (!iniFile.FRModeDisabled)
            {
                if (!AlcCheck(Chk))
                {
                    return;
                }

            }

            if (Chk.HasUnorderedItems)
            {
                if (iniFile.AutoOrderBeforeWaiterClose)
                {

                    bool OrderRes = AlohaTSClass.OrderAllDishez(AlohaTSClass.AlohaCurentState.TerminalId, (int)AlohaTSClass.AlohaCurentState.CheckId, (int)AlohaTSClass.AlohaCurentState.TableId);
                    if (!OrderRes)
                    {
                        frmAllertMessage Mf = new frmAllertMessage("Не могу заказать блюда. Попробуйте еще раз. Либо закажите самостоятельно. Если ошибка будет повторяться свяжитесь со службой техподдержки для перезагрузки Алохи.");
                        Utils.ToLog("Не могу заказать блюда. Выхожу");
                        Mf.ShowDialog();
                        return;
                    }
                }
                else
                {
                    frmAllertMessage Mf = new frmAllertMessage("В чеке есть незаказаные блюда. ");
                    Utils.ToLog("В чеке есть незаказаные блюда. Выхожу");
                    Mf.ShowDialog();
                    return;
                }
            }


            Utils.ToCardLog("Before AskPaymentsCount " );
            AskPaymentsCount(Chk, TenderType, DoPaymentsOper);


           
            
        }

        static private  void AskPaymentsCount(Check Chk,  int TenderType, Action<int,Check> callback)
        {
            Utils.ToCardLog("AskPaymentsCount TenderType= " + TenderType.ToString());
            int PaymentsCount=0;
            if (TenderType == 0)
            {
                PaymentsCount =
                    Convert.ToByte(iniFile.FCCEnable) +
                    Convert.ToByte((iniFile.InPasEnabled) || (CreditCardAlohaIntegration.CreditCardConnectorEnabled)) * 2 +
                    Convert.ToByte(iniFile.CreditCloseByWiterEnabled && (Chk.TableNumber >= 163)) * 4;

                Utils.ToCardLog("AskPaymentsCount PaymentsCount= " + PaymentsCount.ToString());
                //BitArray Ba = new BitArray(new byte[] { PaymentsCount });


                if (PaymentsCount == 0)
                {
                    frmAllertMessage Mf = new frmAllertMessage("Нет подключенных модулей оплаты");
                    Mf.ShowDialog();
                    return;
                }

                if ((GetTrueBitCount(PaymentsCount) > 1) || (PaymentsCount==4))
                {
                    Utils.ToCardLog("GetTrueBitCount(PaymentsCount) > 1");
                    PDiscountCard.AlohaI.WndPaymentSelect WndSelect = new AlohaI.WndPaymentSelect();
                    WndSelect.Chk = Chk;
                    WndSelect.Callback = callback;
                    WndSelect.Closed += new EventHandler(WndSelect_Closed);
                    WndSelect.SetBtnsVis(PaymentsCount);
                    WndSelect.WaiterName = AlohaTSClass.GetCurentWaterName();
                    Utils.ToCardLog("Before WndSelect.Show();");
                    WndSelect.Show();

                    PaymentsCount = WndSelect.Result;

                }
                else
                {
                    
                    callback(PaymentsCount, Chk);
                }
            }
            else
            {
                PaymentsCount = TenderType;
                callback(PaymentsCount, Chk);
            }

            
        }

        static void WndSelect_Closed(object sender, EventArgs e)
        {
            Utils.ToCardLog("Before WndSelect_Closed();");
            var WndSelect = (AlohaI.WndPaymentSelect)sender;
            WndSelect.DoCallBack();
        }

       // delegate void DoPaymentsOperDelegate(int PaymentsCount, Check Chk);

        static private void DoPaymentsOper(int PaymentsCount, Check Chk)
        {
            Utils.ToCardLog("DoPaymentsOper TenderType = " + PaymentsCount.ToString());
            if (PaymentsCount == 1)
            {
                FCC.SetBill();
            }
            else if (PaymentsCount == 2)
            {
                /*
                if (iniFile.InPasEnabled)
                {
                    DualConnector.DualConnectorMain.Sale();
                }
                 * */
                //   else if (iniFile.Arcus2Enabled)
                if (CreditCardAlohaIntegration.CreditCardConnectorEnabled)
                {
                    string err = "";

                    CreditCardAlohaIntegration.RunOper(Chk);
                    //DualConnector.DualConnectorMain.Sale();
                }
            }
            else if (PaymentsCount == 4)
            {
                AlohaTSClass.LogOut();
                RemoteCloseCheck.AddRemoteChkToQuere(Chk.AlohaCheckNum, 30, Chk.Waiter, Chk.Summ);
            }
            GC.Collect();
            Utils.ToCardLog("Exit from DoPaymentsOper TenderType = " + PaymentsCount.ToString());
            GC.Collect();
        }

        internal static void SetVip()
        {

            AlohaTSClass.SetVipToCurrentCheck();
        }

        public static void ShowFrmCashIn()
        {
            if (!iniFile.CloseCheck)
            {
                frmAllertMessage fa = new frmAllertMessage("К данному термналу не подключен фискальный регистратор.");
                fa.ShowDialog();
                return;
            }
            FrmCashIn frmCashIn = new FrmCashIn();
            frmCashIn.ShowDialog();
        }
        internal static void ShowfrmModifItem()
        {
            Check Ch = null;
            int SelCount = 0;
            Dish D = AlohaTSClass.GetSelectedDish(out Ch, out SelCount);

            if (SelCount == 0)
            {
                AlohaTSClass.ShowMessage("Выделите блюда для модифицирования");
                return;
            }
            if (SelCount > 1)
            {
                AlohaTSClass.ShowMessage("Выделенно более одного блюда");
                return;
            }
            if (D.IsOrdered)
            {
                AlohaTSClass.ShowMessage("Нельзя модифицировать заказанные бюда");
                return;
            }


            if (AlohaTSClass.SelectedDishOnOtherTable(Ch))
            {
                AlohaTSClass.ShowMessage("Присутствуют выделенные блюда на другом чеке");
                return;
            }

            RusMessage.FrmRussMessage fRM = new RusMessage.FrmRussMessage(D.LongName);



            fRM.ShowDialog();

            if ((fRM.AddModif) && (fRM.ModifTxt.Length > 0))
            {
                List<string> tmp = new List<string>();

                foreach (string str in fRM.ModifTxt.Split((Environment.NewLine.ToCharArray())))
                {
                    if (str.Replace(" ", "") == "")
                    {
                        continue;
                    }
                    tmp.Add(str);


                }
                //   tmp.Reverse();
                AlohaTSClass.AddRussMessage2(D, Ch, tmp);
                /*
                foreach (string str in tmp)
                {


                    AlohaTSClass.AddRussMessage(D, Ch, str);
                }
                 * */
            }


        }


        internal static void TestPrintWithPause()
        {
            Utils.ToCardLog("TestPrint();");
            TestPrint();
            Utils.ToCardLog("After TestPrint();");
            //Thread.Sleep(3000);
            do
            {
                Utils.ToCardLog("Thread.CurrentThread.Join(3000);");
                Thread.CurrentThread.Join(3000);
            }
            while (MainClass.FiskalPrinterIsPrinting);

            Utils.ToCardLog("RunFiskalChanger();");
            RunFiskalChanger();

        }

        internal static void DeleteItemsAndCloseCheck()
        {


            AlohaTSClass.DeleteAllItemsOnCurentCheckandClose();


        }


        internal static void XReportHamster()
        {
            var checks = PDiscountCard.CloseCheck.ReadAllChecks();
            AlohaTSClass.PrintXReport(checks);

        }

        internal static void IMReport()
        {

         
                FRSClientApp.FRSClient.IMReport();
         
        }

            internal static void XReport()
        {

            if (iniFile.FRSEnabled)
            {
                FRSClientApp.FRSClient.XReport();
            }
            else
            {
                if (iniFile.FiskalDriverNonShtrih)
                {
                    /*
                    if (iniFile.FiskalDriverNonShtrihAlohaReport)
                    {

                        var checks =  PDiscountCard.CloseCheck.ReadAllChecks();
                        AlohaTSClass.PrintXReport(checks);

                        //PDiscountCard.FRSClientApp.PrintOnWinPrinter.PrintDoc2(PDiscountCard.FRSClientApp.FiscalCheckCreator.GetXReportVisual(res));
                    }
                    else
                    {
                     * */
                        FiskalDrivers.FiskalDriver.PrintXReport();
                    //}
                }
                else
                {
                    if (PDiscountCard.CloseCheck.CheckUnClosedChecks())
                    {
                        Utils.ToCardLog("Shtrih2.XReport();");
                        Shtrih2.XReport();
                    }
                }
            }
        }
        public  static ZReportData ZReport()
        {
            ZReportData Data = new ZReportData();

            if (iniFile.FiskalDriverNonShtrih)
            {
                FiskalDrivers.FiskalDriver.PrintZReport();
            }
            else
            {
                if (PDiscountCard.CloseCheck.CheckUnClosedChecks())
                {
                    Data = Shtrih2.GetPreZReportData();
                    Shtrih2.ZReport();
                    Data.DtZRep = DateTime.Now;
                    if (iniFile.SpoolEnabled)
                    {
                        Spool.SpoolCreator.AddZReportToSpool(Data);
                    }
                }

            }
            return Data;
        }




        internal static void ApplyPayment(double summ)
        {
            if (AlohaTSClass.CheckWindow())
            {
                AlohaTSClass.ApplyPayment(AlohaTSClass.AlohaCurentState.TerminalId, (int)AlohaTSClass.AlohaCurentState.CheckId, (int)AlohaTSClass.AlohaCurentState.TableId, summ, 1);

            }
        }

        internal static void OrderItems()
        {
            if (AlohaTSClass.CheckWindow())
            {
                AlohaTSClass.OrderAllDishez(AlohaTSClass.AlohaCurentState.TerminalId, (int)AlohaTSClass.AlohaCurentState.CheckId, (int)AlohaTSClass.AlohaCurentState.TableId);

            }
        }

        internal static void TryAddDiscount(int ModeId, int CheckId, int EmployeeId)
        {
            if (iniFile.TakeOutEnabled)
            {
                try
                {

                    if ((iniFile.TakeOutOrderId1 == ModeId) || (iniFile.TakeOutOrderId2 == ModeId))
                    {
                        AlohaTSClass.SetWaterToCurentCheck(EmployeeId, CheckId);
                        Utils.ToLog("Пытаюсь наложить скидку на заказ с собой");

                        Check Ch = AlohaTSClass.GetCheckById(CheckId);
                        //string LoyaltyNum = AlohaTSClass.GetTakeOutAttr(Ch.TableId);

                        string LoyaltyName = Ch.TableName;

                        Utils.ToCardLog("LoyaltyName = " + LoyaltyName);

                        if (LoyaltyName != "")
                        {
                            if (TakeOut.TOLoaltyConnect.ApplyDisc(Ch.TableId, LoyaltyName))
                            {
                                /*
                                Utils.ToCardLog("LoyaltyNum = " + LoyaltyNum);
                                AlohaTSClass.CheckWindow();
                                MainClass.AssignMember(LoyaltyNum, LoyaltyNum);
                                 * */
                                return;
                            }
                        }


                        /*
                        if (AlohaTSClass.CheckWindow())
                        {
                            if (AlohaTSClass.AlohaCurentState.CompIsAppled)
                            {
                                Utils.ToLog("Скидка уже применена");
                                return;


                            }
                        }
                       */
                        if (AlohaTSClass.GetCheckDiscountById(CheckId))
                        {
                            Utils.ToLog("Скидка уже применена");
                            return;
                        }
                        string outmsg = "";
                        int i = AlohaTSClass.ApplyCompByCheckId(iniFile.TakeOutDiscountId, EmployeeId, CheckId, out outmsg);
                        if (i > 0)
                        {
                            Utils.ToLog("Cкидка на заказ с собой применена");

                        }
                        AlohaTSClass.RefreshCheckDisplay();
                    }
                }
                catch (Exception e)
                {

                }
            }
        }

        private static void RunFiskalChangerCollect()
        {
            if (File.Exists(@"C:\Aloha\Check\FiskalChanger.exe"))
            {
                Utils.ToLog("Запустил FiskalChanger.exe Collect");

                Process.Start(@"C:\Aloha\Check\FiskalChanger.exe", "Collect");
            }
            else
            {
                Utils.ToLog(@"Не нашел файл C:\Aloha\Check\FiskalChanger.exe");
            }
        }

        static DateTime LasrTrySendZReportDt = DateTime.Now;
        static string ZReportDir = @"C:\Aloha\Check\FiskalLog\Tmp";
        static bool FindZReport = false;
        internal static void TrySendZReport()
        {
            if ((DateTime.Now - LasrTrySendZReportDt).TotalMinutes > 10)
            {
                LasrTrySendZReportDt = DateTime.Now;
                try
                {
                    DirectoryInfo Di = new DirectoryInfo(ZReportDir);

                    if (Di.Exists)
                    {

                        if (Di.GetFiles("*.xml").Length > 0)
                        {
                            if (FindZReport)
                            {
                                RunFiskalChangerCollect();
                                FindZReport = false;
                            }
                            else
                            {
                                FindZReport = true;
                            }
                        }
                        else
                        {
                            FindZReport = false;
                        }

                    }
                }
                catch
                {

                }
            }
        }

        /*
        private static void SendJms(object ChkOb)
        {
            try
            {

                Utils.ToLog("Отправляю Jms: ");

                Check Ch = (Check)ChkOb;



                Spool.SpoolToGesWebSrv SpooToJms = new PDiscountCard.Spool.SpoolToGesWebSrv(Ch);

                string XmlTxt = SpooToJms.GetXmlStr();

                JmsSender Js = new JmsSender();

                Js.CreateJmsSessionAndSendMsg(XmlTxt);
                Utils.ToLog("Отправил Jms ");
            }
            catch (Exception e)
            {
                Utils.ToLog("[Error] SendJms " + e.Message);
            }
        }
        */
        internal static void AddWaiterToCheck()
        {
            AddWaterFrm WaterFrm = new AddWaterFrm();
            WaterFrm.TopMost = true;
            WaterFrm.ShowDialog();

        }

        internal static int ApplyBonusPaymentByCodeCheckId = 0;
        internal static void ApplyBonusPayment(int ManagerId, int EmployeeId, int QueueId, int TableId, int CheckId, int TenderId, int PaymentId)
        {

            if ((TenderId == AlohaTender.CreditTenderIdIn) && (EmployeeId != iniFile.RemoteCloseEmployee))
            {
                Utils.ToCardLog("Добавлена оплата бонусами. ");
                if (CheckId != ApplyBonusPaymentByCodeCheckId)
                {
                    PDiscountCard.SV.CardEntryParams mCardEntryParams = new SV.CardEntryParams()
                    {
                        CheckId = CheckId,
                        EntryId = PaymentId,
                        TableId = TableId
                    };
                    double Summ = AlohaTSClass.GetPaymentSumm(PaymentId);
                    if (Summ == 0) return;
                    SV.SVClass.StartSVPayment((decimal)Summ, CheckId, mCardEntryParams);
                }
                else
                {
                    ApplyBonusPaymentByCodeCheckId = 0;
                }
            }
        }

        internal static void ApplyCardPayment(int ManagerId, int EmployeeId, int QueueId, int TableId, int CheckId, int TenderId, int PaymentId)
        {

            if ((TenderId == 20) && (EmployeeId != iniFile.RemoteCloseEmployee))
            {

                if (MainClass.ComApplyPayment)
                {
                    Utils.ToLog("MainClass.ComApplyPayment return");
                    MainClass.ComApplyPayment = false;
                    return;
                }
                if ((!iniFile.TRPOSXEnables) && (!iniFile.ArcusEnabled))
                {
                    return;
                }


                Utils.ToLog("Создаю форму вопроса безналичной оплаты");

                FTermonalSelect M3 = new FTermonalSelect();
                M3.ShowDialog();
                if (M3.Cancel)
                {
                    M3.Dispose();
                    return;
                }
                M3.Dispose();

                if (MainClass.IsWiFi == 2)
                {
                    return;
                }


                if (MainClass.PlasticTransactionInProcess)
                {
                    MF2 MesFrm = new MF2("Предыдущая транзакция еще не завершена.");
                    MesFrm.button1.Visible = false;
                    MesFrm.button3.Visible = false;
                    MesFrm.button2.Text = "Ок";
                    MesFrm.ShowDialog();
                }
                else
                {
                    MainClass.PlasticTransactionInProcess = true;
                    // string Resp = "";
                    // string Rcp = "";
                    // string RespMes = "";
                    FiskInfo fi = PDiscountCard.CloseCheck.ReadFiskInfo();
                    bool AllChecksOnTable = false;

                    {

                        //  ToShtrih.GetPrinterStatus();
                        //Check Ch = AlohaTSClass.GetCheckById(CheckId);

                        Utils.ToLog("Безналичная оплата. Читаю параметры чека № " + CheckId + " Попытка 1", 2);
                        Check Ch = AlohaTSClass.GetCheckById(CheckId);
                        if (Ch == null)
                        {
                            Thread.Sleep(500);
                            Utils.ToLog("Безналичная оплата. Читаю параметры чека № " + CheckId + " Попытка 2", 2);
                            AlohaTSClass.InitAlohaCom();
                            Ch = AlohaTSClass.GetCheckById(CheckId);
                            if (Ch == null)
                            {
                                Thread.Sleep(500);
                                Utils.ToLog("Безналичная оплата. Читаю параметры чека № " + CheckId + " Попытка 3", 2);
                                AlohaTSClass.InitAlohaCom();
                                Ch = AlohaTSClass.GetCheckById(CheckId);
                                if (Ch == null)
                                {
                                    Thread.Sleep(500);
                                    Utils.ToLog("Безналичная оплата. Читаю параметры чека № " + CheckId + " Попытка 4", 2);
                                    AlohaTSClass.InitAlohaCom();
                                    Ch = AlohaTSClass.GetCheckById(CheckId);
                                }
                            }
                        }
                        if (Ch == null)
                        {
                            AlohaTSClass.ShowMessage("Не удалось прочитать параметры чека ");
                            Utils.ToLog("Безналичная оплата. Не удалось прочитать параметры чека № " + CheckId, 2);
                            MainClass.PlasticTransactionInProcess = false;
                            return;
                        }

                        if (Ch.ChecksOnTable.Count > 1)
                        {
                            FTwoChecks FT = new FTwoChecks();
                            FT.Init(Ch.ChecksOnTable);
                            FT.ShowDialog();
                            if (FT.Cancel)
                            {
                                FT.Dispose();
                                MainClass.PlasticTransactionInProcess = false;
                                return;
                            }

                            if (FT.Result == 1)
                            {
                                AllChecksOnTable = true;
                            }
                            FT.Dispose();
                        }


                        int LastTr = fi.CardTransID;


                        try
                        {
                            if (iniFile.ArcusEnabled)
                            {
                                Utils.ToCardLog("Arcus. Инициалазирую оплату ");
                                ArcusAlohaIntegrator.RunOper(Ch, LastTr, AllChecksOnTable, fi, PaymentId);
                            }
                            else
                            {
                                Utils.ToCardLog("TrPosX. Инициалазирую оплату ");
                                TrPosXAlohaIntegrator.RunOper(Ch, LastTr, AllChecksOnTable, fi, PaymentId);
                            }
                        }
                        catch (Exception e)
                        {
                            Utils.ToCardLog("[Error]. Инициалазации оплаты " + e.Message);
                        }

                    }


                }
            }
        }

        internal static void AddItem(int EmployeeId, int QueueId, int TableId, int CheckId, int EntryId)
        {
            if (iniFile.CustomerDisplayEnabled)
            {
                RemoteCommands.CustomerDisplayEventSender.AddSendChkToCustDispToQueue(CheckId, false);
            }
            DisplayBoardClass.AddDishEvent(CheckId, EntryId);
            SendToVideo.AddItem(CheckId, EntryId);
            CheckAddBonusCard(EmployeeId, QueueId, TableId, CheckId, EntryId);

        }

        internal static void CheckAddBonusCard(int EmployeeId, int QueueId, int TableId, int CheckId, int EntryId)
        {
            if (iniFile.MySVEnabled)
            {
                double Pr = 0;
                int BarCode = AlohaTSClass.GetEntryBarCodeAndPrice(EntryId, out Pr);
                if (Loyalty.LoyaltyBasik.SVMyPresentCardBCs.Contains(BarCode))
                {
                    Utils.ToCardLog("Добавлена моя подарочная карта. Баркод " + BarCode);
                    PDiscountCard.SV.CardEntryParams mCardEntryParams = new SV.CardEntryParams()
                    {
                        CheckId = CheckId,
                        EntryId = EntryId,
                        TableId = TableId
                    };

                    SV.SVClass.StartSVSale((decimal)Pr, CheckId, mCardEntryParams);
                    /*

                    Thread myThread = new Thread(new ThreadStart(
                   (Action)delegate()
               {
                  

                   SV.SVClass.StartSVSale((decimal)Pr, mCardEntryParams);
               })

                   );
                    myThread.SetApartmentState(ApartmentState.STA);
                    myThread.Start();

                    */




                }
            }
        }

        private static void CheckAddBonusCardOtherThread(int TableId, int CheckId, int EntryId)
        {


        }
        internal static void DeleteItems(int ManagerId, int EmployeeId, int QueueId, int TableId, int CheckId, int ReasonId)
        {
            String card_code = AlohaTSClass.GetCheckAttr(CheckId, "gift_card");
            Utils.ToLog("Удалили товар из чека, card_code:" + card_code);
            
            if (card_code != null)
            {
                String payment_id_str = AlohaTSClass.GetCheckAttr(CheckId, "payment_id");
                int payment_id;
                int.TryParse(payment_id_str, out payment_id);
                if (AlohaTSClass.DeletePayment(CheckId, payment_id))
                {
                    AlohaTSClass.ShowMessage("Удалили товар! Средства вернутся на падарочную карту.");
                }                               
            }
        }


        internal static void DeletePayment(int ManagerId, int EmployeeId, int QueueId, int TableId, int CheckId, int TenderId, int PaymentId)
        {
            Utils.ToLog("Хотят удалить Оплату подарочной картой, TableId:" + TableId.ToString());
            if (TenderId == 25)
            {
                String card_code = AlohaTSClass.GetCheckAttr(CheckId, "gift_card");
                if (card_code != null)
                {

                    decimal sum_;
                    String pay_sum_str = AlohaTSClass.GetCheckAttr(CheckId, "pay_sum");
                    decimal.TryParse(pay_sum_str, out sum_);

                    ICardHelper card_helper = GetInstanceIIKOHelper.GetInstance();
                                      
                    if (card_helper.ReturnToCard(card_code, sum_, iniFile.SpoolDepNum))
                    {
                        Utils.ToLog("Вернул деньги на падарочную карту, card_code:" + card_code + " сумма:" + sum_.ToString());
                        AlohaTSClass.SetCheckAttr(CheckId, "gift_card", "");                        
                    }
                    else
                    {
                        Utils.ToLog("НЕ вернул деньги на падарочную карту, card_code:" + card_code + " сумма:" + sum_.ToString());
                    }

                }
            }
        }

        internal static void DeleteComp(int ManagerId, int EmployeeId, int QueueId, int TableId, int CheckId, int CompTypeId, int CompId)
        {

        }

        internal static void CloseCheck(int EmployeeId, int QueueId, int TableId, int CheckId)
        {
            try
            {
                Utils.ToLog("Закрытие чека. Читаю настройки PDiscount.ini", 2);
                if (iniFile.CloseCheck)
                {
                    Utils.ToLog("(iniFile.Read(Options, CloseCheck) == TRUE) . Читаю параметры чека № " + CheckId, 5);
                    Check Ch = AlohaTSClass.GetCheckById(CheckId);
                    if (Ch == null)
                    {

                        Utils.ToLog("Ch == null. Выхожу");
                        return;
                    }
                    if (Ch.Dishez.Count == 0)
                    {
                        Utils.ToLog("Ch.Dishez.Count == 0. Выхожу");
                        return;
                    }

                    Ch.SystemDate = DateTime.Now;
                    
                    PDiscountCard.CloseCheck.mCloseCheck(Ch);
                    Utils.ToLog("Чек закрыл. Выхожу", 2);



                }

            }
            catch (Exception e)
            {
                Utils.ToLog(e.Message);
            }
        }
        internal static void ShowReportSale()
        {

            AlohaTSClass.PrintSoldReport(AlohaTSClass.GetItemSaleCount());

        }



        internal static void ShowTotalSumm()
        {
            string Name = "";
            int Id = 0;
            string str = "Итого: " + AlohaTSClass.GetCurentCheckSumm().ToString("0.00") + " руб." + Environment.NewLine;
            if (AlohaTSClass.CheckWindow())
            {
                if (AlohaTSClass.AlohaCurentState.CompIsAppled)
                {
                    str += AlohaTSClass.GetCompName(AlohaTSClass.AlohaCurentState.CompId) + Environment.NewLine;
                    str += "Скидка: " + AlohaTSClass.AlohaCurentState.CompSumm.ToString("0.00") + " руб." + Environment.NewLine;

                    if ((AlohaTSClass.AlohaCurentState.CompId > 10) && (AlohaTSClass.AlohaCurentState.CompId < 25))
                    {
                        if (AlohaTSClass.GetManagerDiscountAttr((int)AlohaTSClass.AlohaCurentState.CheckId, out Name, out Id))
                        {
                            if (Id > 0)
                            {
                                str += Id.ToString() + " ";
                            }
                            str += Name;
                        }
                    }

                }
                int P = 0;
                int V = 0;

                string PreCard = AlohaTSClass.GetDiscountAttr((int)AlohaTSClass.AlohaCurentState.CheckId, out P, out V);
                if (PreCard.Length > 4)
                {
                    if (PreCard.Substring(0, 3).ToUpper() == "PRE")
                    {
                        str += "Проведена карта друга " + PreCard + Environment.NewLine;
                        str += "Отсталось посещений " + V.ToString() + " Осталось дней: " + P.ToString();
                    }
                }
            }
            AlohaTSClass.ShowMessage(str);

        }


        internal static void AddReassonToDeleteItems(int ManagerId, int EmployeeId, int QueueId, int TableId, int CheckId, int ReasonId)
        {
            AlohaTSClass.AddReassonToDeleteItems(CheckId, ReasonId);


        }


        internal static void EOD()
        {
            try
            {

                Utils.ToLog("Закрытие дня");

                //RunExport();

                try
                {
                    if ((iniFile.FRSEnabled)&&(iniFile.FRSMaster))
                    {
                        try
                        {
                            FRSClientApp.FRSClient.ZReport(AlohainiFile.BDate);
                        }
                        catch { }
                        AlohaEventVoids.IMReport();
                    }

                    if (iniFile.TRPOSXEnables)
                    {
                        if (!TrPosXAlohaIntegrator.Sverka())
                        {
                            FiskInfo fi = PDiscountCard.CloseCheck.ReadFiskInfo();
                            fi.NeedSverka = true;
                            PDiscountCard.CloseCheck.WriteFiskInfo(fi);
                        }
                        else
                        {
                            PDiscountCard.CloseCheck.ZeroPlastNum();
                            FiskInfo fi = PDiscountCard.CloseCheck.ReadFiskInfo();
                            fi.NeedSverka = false;
                            PDiscountCard.CloseCheck.WriteFiskInfo(fi);
                        }
                    }
                    if (iniFile.ArcusEnabled)
                    {
                        if (!ArcusAlohaIntegrator.Sverka())
                        {
                            FiskInfo fi = PDiscountCard.CloseCheck.ReadFiskInfo();
                            fi.NeedSverka = true;
                            PDiscountCard.CloseCheck.WriteFiskInfo(fi);
                        }
                        else
                        {
                            PDiscountCard.CloseCheck.ZeroPlastNum();
                            FiskInfo fi = PDiscountCard.CloseCheck.ReadFiskInfo();
                            fi.NeedSverka = false;
                            PDiscountCard.CloseCheck.WriteFiskInfo(fi);
                        }

                    }
                    /*
                    if (iniFile.InPasEnabled)
                    {
                        DualConnector.DualConnectorMain.Sverka();
                    }
                     * */
                    if (CreditCardAlohaIntegration.CreditCardConnectorEnabled)
                    {
                        CreditCardAlohaIntegration.RunSverka();
                    }
                }
                catch (Exception ee)
                {
                    Utils.ToLog(ee.Message);
                }

                

                
                do
                {
                    Thread.CurrentThread.Join(3000);
                }


                
                while (MainClass.FiskalPrinterIsPrinting);

                string Status = "";

                if (!iniFile.FRSEnabled)
                {
                    Utils.ToLog("!iniFile.FRSEnabled");
                    if (iniFile.FiskalDriverNonShtrih)
                    {
                        Utils.ToLog("iniFile.FiskalDriverNonShtrih");
                        FiskalDrivers.FiskalDriver.PrintZReport();
                        Hamster.HamsterWorker.MoveHamster();

                    }
                    else
                    {
                        if (!Shtrih2.ClosedSmenaInternal(out Status))
                        {
                            //Shtrih2.ZReport();
                            ZReport();
                        }
                        else
                        {
                            Utils.ToLog("Принтер с закрытой сменой. Status :" + Status);
                        }
                    }
                    
                    /*
                    if (iniFile.FiskalDriverNonShtrihAlohaReport)
                    { 
                        
                    }
                     * */

                }
                    /*
                else
                {
                    if (!Shtrih2.ClosedSmenaInternal(out Status))
                    {

                        RunFiskalChanger();
                    }
                    else
                    {
                        Utils.ToLog("Принтер с закрытой сменой. Status :" + Status);
                    }
                }
                */
                Shtrih2.ExitFiskalThread();





            }
            catch (Exception e)
            {
                Utils.ToLog(e.Message);
            }
        }
        private static void RunFiskalChanger()
        {
            if (File.Exists(@"C:\Aloha\Check\FiskalChanger.exe"))
            {
                Utils.ToLog("Запустил FiskalChanger.exe -HideClose");


                if (iniFile.FCCZRepAutoNullNal)
                {

                    Process.Start(@"C:\Aloha\Check\FiskalChanger.exe", "-HideClose");
                }
                else
                {
                    Process.Start(@"C:\Aloha\Check\FiskalChanger.exe", "-HideClose  NotAutoNullNal");
                }
            }
            else
            {
                Utils.ToLog(@"Не нашел файл C:\Aloha\Check\FiskalChanger.exe");
            }
        }

        private static void RunExport()
        {
            if (File.Exists(@"C:\Aloha\KExport\run.bat"))
            {
                Utils.ToLog("Запустил Export");

                Process.Start(@"C:\Aloha\KExport\run.bat");
            }
            else
            {
                Utils.ToLog(@"Не нашел файл C:\Aloha\Check\FiskalChanger.exe");
            }
        }
        internal static void Degustations()
        {
            AlohaTSClass.CheckWindow();

           /*
                bool OrderRes = AlohaTSClass.OrderAllDishez(AlohaTSClass.AlohaCurentState.TerminalId, (int)AlohaTSClass.AlohaCurentState.CheckId, (int)AlohaTSClass.AlohaCurentState.TableId);
                if (!OrderRes)
                {
                    frmAllertMessage Mf = new frmAllertMessage("Не могу заказать блюда. Попробуйте еще раз. Либо закажите самостоятельно. Если ошибка будет повторяться свяжитесь со службой техподдержки для перезагрузки Алохи.");
                    Utils.ToLog("Не могу заказать блюда. Выхожу");
                    Mf.ShowDialog();
                    return;
                }
           */
            if (AlohaTSClass.AlohaCurentState.CompIsAppled)
            {
                AlohaTSClass.ShowMessage("На данный чек уже наложена скидка." + Environment.NewLine + "Для наложения дегустации удалите текущую скидку");
            }
            else
            {
                FDegustations mFrm = new FDegustations();
                mFrm.ShowDialog();
            }
        }

        internal static void ShowStopList()
        {
            List<StopListDish> tmp = AlohaTSClass.GetStopListForShow();

            FrmStopList FStl = new FrmStopList();
            //FrmStopListReasons FStl = new FrmStopListReasons();
            FStl.SetDishList(tmp);
            /*
        foreach (StopListDish SLD in tmp)
        {
            s += (SLD.Name + Environment.NewLine[1]);
        }
        AlohaTSClass.ShowMessage(s);
             * */

            FStl.Show();
        }


        internal static void ShowStopListReason()
        {
            List<StopListDish> tmp = AlohaTSClass.GetStopListForShow();

            //FrmStopList FStl = new FrmStopList();
            FrmStopListReasons FStl = new FrmStopListReasons();
            FStl.SetDishList(tmp);
            /*
        foreach (StopListDish SLD in tmp)
        {
            s += (SLD.Name + Environment.NewLine[1]);
        }
        AlohaTSClass.ShowMessage(s);
             * */

            FStl.Show();
        }

    }
}
