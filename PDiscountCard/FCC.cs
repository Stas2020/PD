using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.ComponentModel;
using System.IO;




namespace PDiscountCard
{
    public static class FCC
    {
        private static bool IsInited = false;
        public static void Init()
        {
            IsInited = true;

            FCCIntegration.MainClass2.InitDevice();
            FCCIntegration.MainClass2.SetSync(iniFile.FCCSync);
            FCCIntegration.MainClass2.OnSendMoneyLog += new FCCIntegration.MainClass2.SendMoneyLogEventHandler(MainClass2_OnSendMoneyLog);
            FCCIntegration.MainClass2.OnGetMoneyLog += new FCCIntegration.MainClass2.GetMoneyLogEventHandler(MainClass2_OnGetMoneyLog);

            FCCIntegration.MainClass2.OnStartChange += new FCCIntegration.MainClass2.StartChangeEventHandler(MainClass2_OnStartChange);
            FCCIntegration.MainClass2.OnEndChange += new FCCIntegration.MainClass2.EndChangeEventHandler(MainClass2_OnEndChange);
            FCCIntegration.MainClass2.OnErrorChange += new FCCIntegration.MainClass2.ErrorChangeEventHandler(MainClass2_OnErrorChange);
            FCCIntegration.MainClass2.OnUpdateDeposit += new FCCIntegration.MainClass2.UpdateDepositEventHandler(MainClass2_OnUpdateDeposit);
            FCCIntegration.MainClass2.OnWaitForRemoval += new FCCIntegration.MainClass2.WaitForRemovalEventHandler(MainClass2_OnWaitForRemoval);
            FCCIntegration.MainClass2.OnWaitForRemovalReject += new FCCIntegration.MainClass2.WaitForRemovalRejectEventHandler(MainClass2_OnWaitForRemovalReject);
            FCCIntegration.MainClass2.OnReplenish += new FCCIntegration.MainClass2.ReplenishEventHandler(MainClass2_OnReplenish);
            FCCIntegration.MainClass2.OnCancelChange += new FCCIntegration.MainClass2.StartChangeEventHandler(MainClass2_OnCancelChange);
            FCCIntegration.MainClass2.OnOutCasseta += new FCCIntegration.MainClass2.OnOutCassetaEventHandler(MainClass2_OnOutCasseta);
            FCCIntegration.MainClass2.OnFixedDeposit += new FCCIntegration.MainClass2.WaitForRemovalEventHandler(MainClass2_OnFixedDeposit);




            FCCIntegration.MainClass2.OnHideAdminfrm += new FCCIntegration.MainClass2.HideFrmEventHandler(MainClass2_OnHideAdminfrm);
            FCCIntegration.MainClass2.OnHideRazmenfrm += new FCCIntegration.MainClass2.HideFrmEventHandler(MainClass2_OnHideRazmenfrm);

        }

        static void MainClass2_OnGetMoneyLog(DateTime StartDt, DateTime StopDt)
        {
            StopListService.Service1 s1 = new StopListService.Service1();
            FCCIntegration.MainClass2.GetMoneyLog(s1.GetGloryLog(AlohainiFile.DepNum, AlohaTSClass.GetTermNum(), StartDt, StopDt).ToList());
        }




        static void MainClass2_OnSendMoneyLog(string Mess)
        {
            new System.Threading.Thread(delegate()
            {
                try
                {
                    StopListService.Service1 s1 = new StopListService.Service1();
                    s1.SendGloryLog(AlohainiFile.DepNum, AlohaTSClass.GetTermNum(), Mess);
                }
                catch
                {
                }

            }).Start();


        }

        static void MainClass2_OnFixedDeposit(int Deposit, FCCIntegration.FCCCheck Chk)
        {
            if (Chk.RoundedAmount <= Deposit)
            {
                CloseAlohaTable(Math.Max(Chk.Ammount, Deposit), Chk);
            }
        }

        static void MainClass2_OnHideRazmenfrm(object sender)
        {
            AlohaTSClass.LogOut();
        }

        static void MainClass2_OnHideAdminfrm(object sender)
        {

            AlohaTSClass.LogOut();

        }

        static void MainClass2_OnErrorChange(int Change, FCCIntegration.FCCCheck Chk, string ErrorMsg)
        {
            if (!iniFile.FCCSync)
            {
                AlohaTSClass.UnLockTable(iniFile.FCCTerminal, iniFile.FCCEmployee);
            }
            CCustomerDisplay.EndChange(Change);

            MessageForm Mf = new MessageForm("Ошибка оборудования." + Environment.NewLine + ErrorMsg);
            Mf.button1.Visible = false;
            Mf.button3.Visible = false;
            Mf.button2.Text = "Ок";
            Mf.ShowDialog();
        }

        static public void MainClass2_OnOutCasseta(int Summ, bool Casseta)
        {
            //ToShtrih.PrintOutKassetaReport(Summ, Casseta);

            if (iniFile.FCCZZRepCassetRemove)
            {
                if (Casseta)
                {
                    Utils.ToLog("CurentShtrihCash OutCasseta S");
                    decimal CurentShtrihCash = Shtrih2.GetCashReg(241);
                    Utils.ToLog("CurentShtrihCash OutCasseta E");
                    ToShtrih.ZReportWithCashIncome(CurentShtrihCash - Summ / 100);
                }
                else
                {
                    ToShtrih.CashOutCome(Summ / 100);
                    ToShtrih.PrintOutKassetaReport(Summ / 100, Casseta);
                }
            }
            else
            {
                ToShtrih.PrintOutKassetaReport(Summ / 100, Casseta);
            }
        }

        internal static void WriteIncomeInfoToFccMoneyLog(decimal Summ)
        {
            FCCIntegration.MainClass2.WriteIncomeInfoToFccMoneyLog(Summ);
        }

        static void MainClass2_OnCancelChange(FCCIntegration.FCCCheck Chk, int Change)
        {
            if (!iniFile.FCCSync)
            {
                AlohaTSClass.UnLockTable(iniFile.FCCTerminal, iniFile.FCCEmployee);
            }
            CCustomerDisplay.CancelChange(Change);
        }

        static void MainClass2_OnReplenish(int Summ)
        {
            if (Summ > 0)
            {
                ToShtrih.CashIncome(Summ / 100);
            }
        }

        static void MainClass2_OnWaitForRemovalReject()
        {
            CCustomerDisplay.WaitForRemovalReject();
        }

        static void MainClass2_OnWaitForRemoval(int Change, FCCIntegration.FCCCheck Chk)
        {


            CCustomerDisplay.WaitForRemoval();
        }

        static void MainClass2_OnEndChange(int Change, FCCIntegration.FCCCheck Chk)
        {
            CCustomerDisplay.EndChange(Change);
            try
            {
                if (iniFile.FCCNoChangeCashIncome)
                {
                    if (Change > 0)
                    {
                        ToShtrih.CashIncome(Change / 100);
                    }
                }

            }
            catch (Exception e)
            {
                Utils.ToLog("FCC.MainClass2_OnEndChange Error " + e.Message);
            }
        }

        private static void CloseAlohaTable(int Deposit, FCCIntegration.FCCCheck Chk)
        {
            if (AlohaTSClass.IsAlohaTS())
            {
                AlohaTSClass.LogOut(AlohaTSClass.GetTermNum());
                AlohaTSClass.RefreshCheckDisplay();

                if (Chk.AllChkOnTable)
                {
                    int CurId = 1;
                    foreach (FCCIntegration.FCCDish ChekinTable in Chk.Dishes)
                    {


                        decimal oplata = ChekinTable.Price;
                        if (CurId == Chk.Dishes.Count)
                        {
                            oplata += ChekinTable.Price + (Deposit - Chk.Ammount);
                        }
                        CurId++;
                        RemoteCloseCheck.AddRemoteChkToQuere(ChekinTable.AlohaCheckNum, 1, CurrentEmpl, (decimal)((double)oplata / (double)100));

                    }
                }
                else
                {
                    RemoteCloseCheck.AddRemoteChkToQuere(Chk.AlohId, 1, CurrentEmpl, (decimal)((Deposit) / (double)100));

                }
            }
            else
            {
                RemoteCloseCheck.CloseAlohaTableLocalCurentUser(Chk.AlohId, 1, (decimal)(Deposit));
                if (iniFile.CreditCardAutoOpenCheck)
                {
                    AlohaTSClass.OpenEmptyCheck();
                }
            }

        }

        private static int CurrentEmpl = 0;
        public static void ShowAdmin()
        {
            if (IsInited)
            {
                /*
                AlohaTSClass.CheckWindow();
                CurrentEmpl = AlohaTSClass.AlohaCurentState.EmployeeNumberCode;
                Utils.ToLog("CurrentEmpl = " + CurrentEmpl.ToString());
                 * */
                FCCIntegration.MainClass2.ShowAdminfrm();

            }
            else
            {
                frmAllertMessage fa = new frmAllertMessage("К данному термналу не подключено устройство приема денежных средств.");
                fa.ShowDialog();
            }
        }

        public static void ShowRazmen()
        {
            if (IsInited)
            {

                FCCIntegration.MainClass2.ShowRazmenWnd();
            }
            else
            {
                frmAllertMessage fa = new frmAllertMessage("К данному термналу не подключено устройство приема денежных средств.");
                fa.ShowDialog();
            }
        }



        public static void ShowCassirFrm()
        {
            if (!IsInited)
            {
                frmAllertMessage fa = new frmAllertMessage("К данному термналу не подключено устройство приема денежных средств.");
                fa.ShowDialog();
                return;
            }
            FCCIntegration.MainClass2.ShowCassirFrm();



        }
        /*
        internal static void SetBillWithHands()
        {

            Utils.ToLog("SetBillWithHands ");
            AlohaTSClass.CheckWindow();
            string str = "";
            Check Chk = AlohaTSClass.GetCheckById((int)AlohaTSClass.AlohaCurentState.CheckId);
            AlohaTSClass.OrderAllItems(Chk.AlohaCheckNum, Chk.TableId, out str);
            AlohaTSClass.ApplyPaymentAndClose(Chk.AlohaCheckNum, Chk.Summ, iniFile.FCCCash);


            //FCCIntegration.MainClass.ShowWithHandsFrm(GetFCCCheckfromAloha(Chk));
        }
        */
        public static bool InpectSmallChange(bool ShowSuccess)
        {
            Utils.ToLog("InpectSmallChange");
            if (!IsInited)
            {
                frmAllertMessage fa = new frmAllertMessage("К данному термналу не подключено устройство приема денежных средств.");
                fa.ShowDialog();
                return false;
            }
            List<int> Minfvs = new List<int>();
            List<int> NEfvs = new List<int>();
            bool SmallChange = FCCIntegration.MainClass2.SmallChange(out Minfvs, out NEfvs);
            if (SmallChange)
            {
                string SmallChangeMess = "В устройстве отсутствует необходимое для сдачи количество банкнот (монет) номиналом " + Environment.NewLine;
                foreach (int fv in Minfvs)
                {
                    SmallChangeMess += (fv / 100).ToString() + "р " + Environment.NewLine;
                }
                frmAllertMessage Mf = new frmAllertMessage(SmallChangeMess);
                Mf.ShowDialog();
                return false;
            }
            if (NEfvs.Count > 0)
            {
                string SmallChangeMess = "Количество банкнот (монет) номиналом: " + Environment.NewLine;
                foreach (int fv in NEfvs)
                {
                    SmallChangeMess += (fv / 100).ToString() + "р " + Environment.NewLine;
                }
                frmAllertMessage Mf = new frmAllertMessage(SmallChangeMess + " близко к завершению");
                Mf.ShowDialog();

            }
            else
            {
                if (ShowSuccess)
                {
                    frmAllertMessage Mf = new frmAllertMessage("В устройстве достаточно сдачи");
                    Mf.ShowDialog();
                }
            }
            return true;
        }

        internal static void SetBill()
        {
            Utils.ToLog("SetBill");
            if (!IsInited)
            {
                frmAllertMessage fa = new frmAllertMessage("К данному термналу не подключено устройство приема денежных средств.");
                fa.ShowDialog();
                return;
            }
            AlohaTSClass.CheckWindow();

            Utils.ToLog("SetBill Curent Check = " + AlohaTSClass.AlohaCurentState.CheckId);


            if (iniFile.FCCSync)
            {
                // FCCIntegration.MainClass2.CancelChangeMoney();
                //Utils.ToLog("SetBill");
                //FCCIntegration.MainClass2.ShowCassirFrm();
            }
            else
            {
                if (FCCIntegration.MainClass2.ChangeProcess)
                {
                    if (FCCIntegration.MainClass2.CurrentCheck.AlohId != (int)AlohaTSClass.AlohaCurentState.CheckId)
                    {
                        MessageForm Mf = new MessageForm("Устройство приема наличных средств находится в состоянии ожидания оплаты. " +
                            Environment.NewLine + "Завершите или отмените текущую операцию и затем приступайте к новой."
                            );
                        Mf.button1.Visible = false;
                        Mf.button3.Visible = false;
                        Mf.button2.Text = "Ок";
                        Mf.ShowDialog();
                        return;
                    }
                    else
                    {
                        FCCIntegration.MainClass2.ShowCassirFrm();
                        return;
                    }
                }
            }


            if (!InpectSmallChange(false))
            {
                Utils.ToLog("Мало сдачи. Выхожу");
                return;
            }



            Check Chk = AlohaTSClass.GetCheckById((int)AlohaTSClass.AlohaCurentState.CheckId);
            if (iniFile.FCCSync)
            {
                if (Chk.HasUnorderedItems)
                {
                    frmAllertMessage Mf = new frmAllertMessage("В чеке есть незаказаные блюда. ");
                    Utils.ToLog("В чеке есть незаказаные блюда. Выхожу");
                    Mf.ShowDialog();
                    return;
                }
            }
            if (Chk.Summ == 0)
            {
                frmAllertMessage Mf = new frmAllertMessage("Чек нулевой");
                Mf.ShowDialog();
                return;
            }
            else if (Chk.Oplata >= Chk.Summ)
            {
                frmAllertMessage Mf = new frmAllertMessage("Чек уже оплачен");
                Mf.ShowDialog();
                return;
            }

            else if (Chk.Vozvr)
            {
                frmAllertMessage Mf = new frmAllertMessage("Нельзя оплачивать возвратные чеки");
                Mf.ShowDialog();
                return;
            }
            else
            {
                bool AllCheck = false;
                if (Chk.ChecksOnTable.Count > 1)
                {
                    MessageForm ManyChecksMsg = new MessageForm("Закрыть все чеки на столе?");
                    ManyChecksMsg.button1.Text = "Да";
                    ManyChecksMsg.button2.Text = "Только текущий";
                    // ManyChecksMsg.button2.Visible = false;
                    ManyChecksMsg.ShowDialog();

                    if (ManyChecksMsg.Result == 1)
                    {
                        //   Summ = Chk.ChecksOnTable.Sum(a => a.Summ);

                        AllCheck = true;
                        Utils.ToLog("Отправляю на оплату все чеки на столе." + GetFCCCheckfromAloha(Chk, AllCheck).Ammount);

                    }
                }

                string Status;
                Utils.ToLog("Отправляю на оплату. Сумма" + GetFCCCheckfromAloha(Chk, AllCheck).Ammount);
                if (FCCIntegration.MainClass2.StartChangeMoney(GetFCCCheckfromAloha(Chk, AllCheck), out Status))
                {
                    string str = "";
                    CurrentEmpl = AlohaTSClass.AlohaCurentState.EmployeeNumberCode;
                    AlohaTSClass.OrderAllItems(Chk.AlohaCheckNum, Chk.TableId, out str);
                    if (!iniFile.FCCSync)
                    {
                        AlohaTSClass.OpenEmptyCheck();
                        AlohaTSClass.LogIn(iniFile.FCCTerminal, iniFile.FCCEmployee);
                        AlohaTSClass.LockTable(iniFile.FCCLockDish, iniFile.FCCTerminal, Chk.AlohaCheckNum);
                    }



                    CCustomerDisplay.SetCheck(Chk);
                }
                else
                {
                    MessageForm Mf = new MessageForm("Устройство приема наличных средств занято другой транзакцией.");
                    Mf.button1.Visible = false;
                    Mf.button3.Visible = false;
                    Mf.button2.Text = "Ок";
                    Mf.ShowDialog();
                }


            }



        }
        static void MainClass2_OnStartChange(FCCIntegration.FCCCheck Chk, int Change)
        {
            CCustomerDisplay.StartChange(Chk.Ammount);
        }


        static void MainClass2_OnUpdateDeposit(int Summ)
        {
            CCustomerDisplay.UpDateDeposit(Summ);
        }
        public static void CancelBill()
        {
            Utils.ToLog("CancelBill");
            AlohaTSClass.UnLockTable(iniFile.FCCTerminal, iniFile.FCCEmployee);
            FCCIntegration.MainClass2.CancelChangeMoney();
        }
        public static void SetBillTest()
        {
            if (!InpectSmallChange(false))
            {
                return;
            }


            FCCIntegration.FCCCheck Chk = new FCCIntegration.FCCCheck()
            {
                AlohId = 1,
                Ammount = 33000
            };

            //FCCIntegration.MainClass2.StartChangeMoney(Chk);
            string Status;
            if (!FCCIntegration.MainClass2.StartChangeMoney(Chk, out Status))
            {
                MessageForm Mf = new MessageForm("Ошибка оборудования." + Environment.NewLine + Status);
                Mf.button1.Visible = false;
                Mf.button3.Visible = false;
                Mf.button2.Text = "Ок";
                Mf.ShowDialog();
            }

        }
        /*
        static void MainClass_OnChangeMoneyEnd(FCCIntegration.FCCCheck Chk)
        {
            Check Chk2 = AlohaTSClass.GetCheckById((int)Chk.AlohId);

            AlohaTSClass.LogIn(Chk2.Waiter);
            AlohaTSClass.ApplyPayment(AlohaTSClass.GetTermNum(), Chk.AlohId, Chk.Ammount/100, 11);
            //AlohaTSClass.OrderAllDishez(AlohaTSClass.GetTermNum(), Chk.AlohId, Chk2.TableId);
            AlohaTSClass.CloseCheck(AlohaTSClass.GetTermNum(), Chk.AlohId);
        }
        */
        private static FCCIntegration.FCCCheck GetFCCCheckfromAloha(Check Chk, bool AllChecksOnTable)
        {
            FCCIntegration.FCCCheck FCCCh = new FCCIntegration.FCCCheck();
            if (!AllChecksOnTable)
            {

                FCCCh.Ammount = (int)((Chk.Summ - Chk.Oplata) * 100);
                FCCCh.AlohId = (int)(Chk.AlohaCheckNum);
                FCCCh.AlohNumber = Convert.ToInt32(Chk.CheckShortNum);
                FCCCh.TableId = Chk.TableId;
                foreach (Dish D in Chk.Dishez)
                {
                    FCCCh.Dishes.Add(
                        new FCCIntegration.FCCDish()
                        {
                            Name = D.LongName,
                            Price = D.Price * 100,
                        }
                        );
                }
            }
            else
            {
                FCCCh.AllChkOnTable = true;
                FCCCh.Ammount = (int)(Chk.ChecksOnTable.Sum(a => a.Summ - Chk.Oplata) * 100);
                FCCCh.AlohId = (int)(Chk.AlohaCheckNum);
                FCCCh.AlohNumber = Convert.ToInt32(Chk.CheckShortNum);
                FCCCh.TableId = Chk.TableId;
                foreach (Check Chk2 in Chk.ChecksOnTable)
                {
                    FCCCh.Dishes.Add(
                        new FCCIntegration.FCCDish()
                        {
                            Name = "Чек " + Chk2.CheckShortNum,
                            Price = (Chk2.Summ - Chk.Oplata) * 100,
                            AlohaCheckNum = (int)Chk2.AlohaCheckNum
                        }
                        );
                }
            }
            return FCCCh;
        }
        /*
        static internal void UpdateChk(Check Chk)
        {
            FCCIntegration.MainClass.UpdateGuestScreen(GetFCCCheckfromAloha(Chk));
        }
        */
    }
}
