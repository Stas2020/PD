using System.Windows.Forms;
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime;
using System.Reflection;
using System.Threading;
using AlohaFOHLib;

using System.IO;

namespace PDiscountCard
{
    public static class MainClass
    {

        static bool Isinited = false;
        //static bool CheckCanClose = true ;

        static public int IsWiFi = 1;

        static internal DateTime TimeTic = DateTime.Now;


        static internal bool PlasticTransactionInProcess = false;
        static internal bool EodStop = false;
        static internal bool RemoteOrderEnable = false;
        static internal bool TrPosxEnable = false;

        static internal bool IamIsMaster = false;
        static internal bool ComApplyPayment = false;
        static internal CoffeToGo CTG;
        static internal int CurentMaster = -1;


        static internal int KKMNumber = 0;
        static internal int CurentManagerCard = -1;
        static internal int DeletedPayment = -1;
        static internal int DeletedPaymentCheck = -1;
        static internal int DeletedPaymentTry = 0;
        static internal DateTime DeletedPaymentDateTime;
        static internal ActivateResult PlastikActivateResult = new ActivateResult();
        static internal DateTime TimeTicForOrder = DateTime.Now;

        static internal bool FiskalPrinterIsPrinting = false;

        // static SetWinHook Sh = new SetWinHook();





        //static mSonic ms;

        static internal Thread ThreadDeleteLogs;

        static public Thread MainThread;
        static public SetWinHook mSetWinHook;

        static internal Thread ThreadLoyaltyCard;
        static internal EventWaitHandle WHThreadLoyaltyCard = new AutoResetEvent(false);


        static internal Thread ThreadStopList;
        static internal EventWaitHandle WHThreadThreadStopList = new AutoResetEvent(false);

        static private System.Timers.Timer LogTimer = new System.Timers.Timer(5000);
        //static private System.Timers.Timer AssignLoyaltyTimer = new System.Timers.Timer(1000);

        /*
        static internal void InitAssignLoyaltyTimer()
        {
            AssignLoyaltyTimer.Elapsed += new System.Timers.ElapsedEventHandler(AssignLoyaltyTimer_Elapsed);
            AssignLoyaltyTimer.Stop();
            Utils.ToLog("InitAssignLoyaltyTimer");
        }
        */





        static internal bool mStopAllThreads = false;
        static internal void StopAllThreads()
        {
            PDSystem.ModalWindowsForegraund.StopWndForegroundLoop();

            PDiscountCard.FRSClientApp.FRSQueue.StopFRSQueueThread();

            CloseCheck.ExitCloseCheckThread();
            CloseCheck.ExitJMSSendThread();
            Shtrih2.ExitFiskalThread();

            RemoteCommands.CustomerDisplayEventSender.CustomerDisplayEventSenderStop();
            mStopAllThreads = true;
            WHThreadThreadStopList.Set();
            


        }

        internal static void StopListSender()
        {
            //Thread.Sleep(300000); 
            while (!mStopAllThreads)
            {
                WHThreadThreadStopList.WaitOne();
                if (mStopAllThreads) return;
                try
                {
                    Utils.ToLog("Формирую стоп лист!");
                    long[] s = AlohaTSClass.GetStopList().ToArray();
                    Utils.ToLog("Соединяюсь с сервером  стоп листа!");
                    StopListService.Service1 s1 = new PDiscountCard.StopListService.Service1();
                    Utils.ToLog("Отправляю стоп лист!");
                    s1.GetEventAsync(AlohainiFile.DepNum, s);
                    s1.Dispose();
                    Utils.ToLog("Отправил стоп лист!");
                }
                catch (Exception e)
                {
                    Utils.ToLog("GetStopList Exception: " + e.Message);
                }

            }

        }
        

        static AlohaFlyExport.AlohaFlyExportHelper exp;
        static public void InitData()
        {
            //AppDomain.CurrentDomain.AppendPrivatePath(@"C:\Aloha\check\DualConnector");
            

            MainThread = Thread.CurrentThread;
            LogTimer.Elapsed += new System.Timers.ElapsedEventHandler(LogTimer_Elapsed);
            LogTimer.Start();
            //InitAssignLoyaltyTimer();


            ThreadDeleteLogs = new Thread(Utils.DeleteOldLogs);
            ThreadDeleteLogs.IsBackground = true;
            ThreadDeleteLogs.Start();


            ThreadLoyaltyCard = new Thread(AssignLoyaltiStateThread);
            ThreadLoyaltyCard.IsBackground = true;
            ThreadLoyaltyCard.Start();

            ThreadStopList = new Thread(StopListSender);
            ThreadStopList.IsBackground = true;
            ThreadStopList.Priority = ThreadPriority.Lowest;
            ThreadStopList.Start();

// ThreadLoyaltyCard.Start();



            if (!Isinited)
            {
                Isinited = true;
                try
                {
                    RegWorker.RegKeyExist();
                }
                catch
                { }
                Utils.ToLog("Начало инициализации Version: " + Assembly.GetExecutingAssembly().GetName().Version.ToString());


                Utils.ToLog("AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += CurrentDomain_ReflectionOnlyAssemblyResolve; нах");
                /*
            AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += CurrentDomain_ReflectionOnlyAssemblyResolve;
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_ReflectionOnlyAssemblyResolve;
                */


                AlohaTSClass.InitAlohaCom();


                if (!iniFile.NonAskRemoteSettings)
                {
                    Config.ConfigSettings.SetSettings();
                }


                try
                {
                    Utils.ToCardLog("Инициализация SV");
                    SVSistem.Main.Init(AlohainiFile.DepNum, AlohaTSClass.GetTermNum());
                }
                catch(Exception e)
                {
                    Utils.ToCardLog("Error Игициализации SV "+e.Message);
                }

                if (iniFile.CloseCheck)
                {
                    Utils.ToLog("iniFile.NOLocalFR " + iniFile.NOLocalFR.ToString());
                    Utils.ToLog("iniFile.FiskalDriverNonShtrih " + iniFile.FiskalDriverNonShtrih.ToString());
                    if (!iniFile.NOLocalFR)
                    {
                        if (!iniFile.FiskalDriverNonShtrih)
                        {
                            try
                            {
                                Utils.ToLog("Активирую драйвер Штриха");
                                Shtrih2.CreateShtrih();
                            }
                            catch (Exception e)
                            {
                                Utils.ToLog("Ошибка при активации драйвера Штриха " + e.Message);
                            }

                        }
                        else
                        {


                            try
                            {
                                FiskalDrivers.FiskalDriver.CreateFiskalDriver(iniFile.FiskalDriverType);
                                FiskalDrivers.FiskalDriver.Connect(iniFile.FiskalDriverLUNumber);

                            }

                            catch (Exception e)
                            {
                                Utils.ToLog("Ошибка при активации драйвера фискального принтера " + e.Message);
                            }

                        }
                    }
                }


              

                Hamster.HamsterWorker.SendOldHamster();

                RemoteCloseCheck.Init();


                Utils.ReadVisitsFromFile();  

                if (iniFile.CustomerDisplayEnabled)
                {
                    Utils.ToLog("Активирую CustomerDisplayEventSender");
                    RemoteCommands.CustomerDisplayEventSender.CustomerDisplayEventSenderInit();
                }


                if (!iniFile.RemoteLisenterDisabled)
                {
                    try
                    {
                        Utils.ToLog("Активирую RemoteLisenter на порту 9267");
                        PDiscountCard.RemoteCommands.RemoteLisenter.Init();
                    }
                    catch (Exception e)
                    {
                        Utils.ToLog("Ошибка при активации RemoteLisenter на порту 9267 " + e.Message);
                    }
                }
                /*
                if (iniFile.FCCEnable)
                {
                    FCC.ShowGuestScreen();
                }
                */
                if (iniFile.JMSEnable)
                {
                    try
                    {
                        Utils.ToLog("Проверка недоотправленных спулов");
                        JmsSender Js = new JmsSender();
                        Js.TrySendOldJmsAsync();
                    }
                    catch (Exception e)
                    {
                        Utils.ToLog("Ошибка проверки недоотправленных спулов " + e.Message);

                    }
                }

                if (iniFile.ExternalInterfaceEnable)
                {
                    PDiscountCard.AlohaExternal.UniversalHost.Open();
                }

                if (iniFile.FRSEnabled)
                {
                    PDiscountCard.FRSClientApp.FRSQueue.StartFRSQueueThread();
                }


                if (iniFile.CloseCheck || iniFile.JMSEnable || iniFile.SpoolEnabled)
                { 
                    CloseCheck.StartJMSSEndQuere();
                    //if (!iniFile.NOLocalFR)
                    {
                        CloseCheck.StartCloseCheckQuere();
                    }
                }

                /*
                if (iniFile.XFromGes)
                {
                    decimal cash = 0;
                    decimal card = 0;
                    FRSClientApp.GesData.GetGesData(AlohainiFile.BDate, AlohainiFile.DepNum, out cash, out card);
                    
                }
                */
                FRSClientApp.PrintOnWinPrinter.DeleteAllImgs();


                try
                {
                    if (!iniFile.AsincSenderEventDisabled) 
                    {
                        Utils.ToLog("Активирую ловца событий");
                        EventSenderClass.Init();
                    }
                    /*
                    if (iniFile.InPasEnabled)
                    {
                        Utils.ToLog("Активирую модуль приема пластиковых карт Inpas");
                        DualConnector.DualConnectorMain.Init();
                    }
                    */

                    RemoteOrderEnable = iniFile.RemoteOrder;
                    CTG = new CoffeToGo();
                    if (RemoteOrderEnable)
                    {
                        Utils.ToLog("Активирую режим приема удаленных заказов с Туманного Альбиона");
                        try
                        {
  

                            //CTG.Init();
                            //s2010Serv. += new PDiscountCard.RemoteOrderSrv.GetNewOrdersCompletedEventHandler(s2010Serv_GetNewOrdersCompleted);
                            Utils.ToLog("Активирован режим приема удаленных заказов с Туманного Альбиона");
                        }
                        catch (Exception e)
                        {
                            Utils.ToLog("Ошибка Активировации режима приема удаленных заказов с Туманного Альбиона " + e.Message);
                        }
                    }

                   
                    try
                    {
                        //  InterceptAlohaPrintingCreator.CreateInterface();



                        if (!iniFile.WinHook)
                        {
                            Utils.ToLog("Активирую перехватчик карт Aloha InterceptAlohaPeripheralsClass " + typeof(InterceptAlohaPeripheralsClass).GUID);
                            AlohaActivityClass AAC = new AlohaActivityClass();

                            IInterceptAlohaPeripherals m_AlohaPeripherals;

                            WinApi.IClassFactory2 icf2 = WinApi.CoGetClassObject(
            typeof(InterceptAlohaPeripheralsClass).GUID,
            WinApi.CLSCTX.CLSCTX_ALL, new System.IntPtr(),
            typeof(WinApi.IClassFactory2).GUID) as WinApi.IClassFactory2;



                            // WinApi.CoGetClassObject (InterceptAlohaPeripheralsClass ) 
                            string Lic = "1T1X1W05575U1U0@4H2>185J4I5=5H4G2X4W0U3R0Y491D4R4<42051S5T5O3Z5A241O1:5<";
                        //    string Lic = "4G0<5C5J3S2Q232B1T3N2P4O2W3;21353?0T054>4;2I3M3K3G4T3E1M452=510O4N5F3Y1T";
                            m_AlohaPeripherals = icf2.CreateInstanceLic(
            null, null, typeof(IInterceptAlohaPeripherals).GUID, Lic) as IInterceptAlohaPeripherals;

                            m_AlohaPeripherals.RegisterMagcardInterceptor(AAC);

                            Utils.ToLog("Активировал перехватчик карт Aloha");
                        }
                        else
                        {
                            mSetWinHook = new SetWinHook();

                        }
                    }
                    catch (Exception e)
                    {
                        Utils.ToLog(e.Message);
                    }


                    if (iniFile.AlohaFlyExportEnabled)
                    {
                        exp = new AlohaFlyExport.AlohaFlyExportHelper();
                        exp.SendOrderFlightAsync(null);
                    }
                    
                    if (!iniFile.FriendCardsDisabled)
                    {
                        Utils.ToLog("Активирую веб-сервис карт гостя");
                        ToBase.FirstInit();
                    }

                    if ((iniFile.TRPOSXEnables) && (iniFile.ArcusEnabled))
                    {
                        Utils.ToLog("Не могу подключить оба модуля пластиковых карт");
                    }
                    else
                    {

                        if (iniFile.TRPOSXEnables)
                        {
                            Utils.ToLog("Активирую модуль приема оплаты пластиковых карт");

                            string s = "";
                            if (TrPosXAlohaIntegrator.Init(out s))
                            {
                                PlastikActivateResult.Result = true;
                                TrPosxEnable = true;
                                //ToShtrih.Init();
                                Utils.ToLog("Активировал модуль приема оплаты пластиковых карт TrPosX");
                            }
                            else
                            {
                                PlastikActivateResult.Result = false;
                                PlastikActivateResult.Comment = s;

                                Utils.ToLog("Ошибка модуля приема оплаты пластиковых карт TrPosX " + s);
                            }
                        }

                        if (iniFile.ArcusEnabled)
                        {
                            string s = "";
                            if (ArcusAlohaIntegrator.Init(out s))
                            {
                                //ToShtrih.Init();
                                PlastikActivateResult.Result = true;
                                Utils.ToLog("Активировал модуль приема оплаты пластиковых карт Arcus");
                            }
                            else
                            {
                                PlastikActivateResult.Result = false;
                                PlastikActivateResult.Comment = "Ошибка модуля приема оплаты пластиковых карт  Arcus" + s;

                                Utils.ToLog("Ошибка модуля приема оплаты пластиковых карт  Arcus " + s);
                            }


                        }


                    }


                    if (iniFile.SBCreditCardEnabled)
                    {
                        string s = "";
                        if (CreditCardAlohaIntegration.Init(CreditCardTerminalType.Sber, out s))
                        {
                            Utils.ToLog("Активировал сб терминал приема оплаты пластиковых карт");
                        }
                        else
                        {
                            Utils.ToLog("Ошибка сб терминала приема оплаты пластиковых карт  " + s);
                        }


                    }


                    if (iniFile.CreditCardEmulatorEnabled)
                    {
                        string s = "";
                        if (CreditCardAlohaIntegration.Init(CreditCardTerminalType.Emulator, out s))
                        {
                            Utils.ToLog("Активировал эмулятор приема оплаты пластиковых карт");
                        }
                        else
                        {
                            Utils.ToLog("Ошибка эмулятора приема оплаты пластиковых карт  " + s);
                        }


                    }


                    if (iniFile.Arcus2Enabled)
                    {
                        string s = "";
                        if (   CreditCardAlohaIntegration.Init(CreditCardTerminalType.Arcus2, out s))
                        {
                            Utils.ToLog("Активировал модуль приема оплаты пластиковых карт Arcus2");
                        }
                        else
                        {
                            Utils.ToLog("Ошибка модуля приема оплаты пластиковых карт  Arcus2 " + s);
                        }


                    }

                    if (iniFile.Arcus4Enabled)
                    {
                        string s = "";
                        if (CreditCardAlohaIntegration.Init(CreditCardTerminalType.Arcus4, out s))
                        {
                            Utils.ToLog("Активировал модуль приема оплаты пластиковых карт Arcus4");
                        }
                        else
                        {
                            Utils.ToLog("Ошибка модуля приема оплаты пластиковых карт  Arcus4 " + s);
                        }


                    }


                    if (iniFile.VerifoneEnabled)
                    {
                        string s = "";
                        if (CreditCardAlohaIntegration.Init(CreditCardTerminalType.Inpas, out s))
                        {
                            Utils.ToLog("Активировал модуль приема оплаты пластиковых карт Verifone");
                        }
                        else
                        {
                            Utils.ToLog("Ошибка модуля приема оплаты пластиковых карт  Verifone " + s);
                        }


                    }


                    //AlohaTSClass.SendCodiroffkaToDisplayBoard();

                    DisplayBoardClass.InitDisplayBoard();


                    if (iniFile.GalleryOrderEnabled)
                    {
                        
                        // GallerySrvs gs = new GallerySrvs();
                        //gs.StartOrderLisenter();

                    }

                    if (iniFile.FCCEnable)
                    {
                        try
                        {
                            Utils.ToLog("[initData]Инициализация FCC");
                            FCC.Init();

                            if (iniFile.FCCCustumerDisplayEnable)
                            {
                                Utils.ToLog("[initData]CCustomerDisplay");

                                PDiscountCard.CCustomerDisplay.Init();
                            }
                        }
                        catch (Exception e)
                        {
                            Utils.ToLog("[initData] Ошибка Инициализации FCC " + e.Message);
                        }

                    }

                    Version ver = Utils.GetWinVersion();
                    Utils.ToLog(String.Format("WinVersion Major: {0}; Minor {1} ", ver.Major, ver.Minor));
                    Utils.ToLog("No send menu, because sending throw codepage");
                    /*
                    if ((ver.Major > 5) || (iniFile.MenuSenderNeedSend))
                    {
                        if ((!iniFile.MenuSenderDisabled))
                        {
                            MenuSender.MenuSenderInit();
                        }
                    }
                    else {
                        
                        Utils.ToLog(String.Format("WinVersion <6. No MenuSend "));
                    }
                    */

                    if (!iniFile.SQLCheckDisabled)
                    {
                        SQL.ToSql.CheckBase();

                        OrderDishTime.Init();
                    }



                    //Spool.FromTranslog.GenSpoolFromTr();


                    // ArcusClass.AddSlip();
                    Utils.ToLog("[initData]Успешное окончание инициализации");
                    /*
                    if (DownTimeiniFile.GetMaster() == Utils.GetTermNum())
                    {
                        Utils.ToCardLog("LogInAllClockedInEmployees ");
                        AlohaTSClass.LogInAllClockedInEmployees();
                    }
                    */

                }
                catch (Exception e)
                {
                    Utils.ToLog(e.Message);
                    Utils.ToLog("[ERROR] [InitData]Ошибка инициализации");
                }
            }
        }

        static Assembly CurrentDomain_ReflectionOnlyAssemblyResolve(object sender, ResolveEventArgs args)
        {
            Utils.ToLog("CurrentDomain_ReflectionOnlyAssemblyResolve Name: " + args.Name + "  RequestingAssembly: " + args.RequestingAssembly);
            
            try
            {
                var ass = Assembly.LoadFrom(@"C:\Aloha\check\DualConnector\DualConnector.dll");
                

               return   ass;
            }
            catch(Exception e)
            {
                Utils.ToLog("[ERROR] Assembly.LoadFrom "+e.Message);
                return null;
            }
        }



        static int GetAssingMemberStateErrorCount = 0;
        static internal bool AssignLoyaltiStateInProcess = false;
        static internal bool AssignLoyaltiStateTimerEnabled = false;

        //        static internal bool  LoyaltiThreadEnable = false;

        internal static void AssignLoyaltiStateThread()
        {
            while (!mStopAllThreads)
            {
                WHThreadLoyaltyCard.WaitOne();
                while (AssignLoyaltiStateTimerEnabled)
                {
                    Thread.Sleep(1000);
                    AssignLoyaltiState();
                }
            }
        }

        private static void AssignLoyaltiState()
        {
            if (AssignLoyaltiStateInProcess)
            {
                return;
            }
            if (!AssignLoyaltiStateTimerEnabled)
            {
                return;
            }
            AssignLoyaltiStateInProcess = true;

            //AlohaTSClass.ShowMessage("AssignLoyaltiState: GetAssingMemberStateErrorCount: " + GetAssingMemberStateErrorCount);
            AlohaTSClass.CheckWindow();
            Utils.ToCardLog("Таймер проверки пременения скидки " + GetAssingMemberStateErrorCount);
            if (AlohaTSClass.AlohaCurentState.CompIsAppled)
            {
                AlohaTSClass.ShowMessage("Скидка применена");
                EventSenderClass.SendAlohaAsincEvent(StopListService.AlohaEventType.LoyaltyCardOk, "", AlohaTSClass.AlohaCurentState.WaterId,
                    AlohaTSClass.GetJobCode(AlohaTSClass.AlohaCurentState.WaterId),
                    CurentCadrNumber,
                    GetAssingMemberStateErrorCount,
                    (int)AlohaTSClass.AlohaCurentState.TableId,
                    (int)AlohaTSClass.AlohaCurentState.CheckId);
                AssignLoyaltiStateTimerEnabled = false;
                //WHThreadLoyaltyCard.;
                if (!AlohaTSClass.GetCurentStateFromArgs)
                {
                    CurentfrmCardMoover.SetOk();
                }
                return;
            }
            else
            {
                GetAssingMemberStateErrorCount++;
                if (GetAssingMemberStateErrorCount > 1)
                {

                    AlohaTSClass.ShowMessage("Ожидание ответа от сервера..." + (GetAssingMemberStateErrorCount - 1).ToString() +
                        " сек. " + Environment.NewLine +
                        "Макcимальное время ожидания: 15 сек."
                        );
                }
            }





            if (GetAssingMemberStateErrorCount == 3)
            {

                int i = AlohaTSClass.GetAssingMemberState(AlohaTSClass.AlohaCurentState.TerminalId);
                Utils.ToCardLog("GetAssingMemberState: " + i.ToString());
                if (i == 3)
                {

                    int LocalDiscNum = -1;
                    try
                    {
                        LocalDiscNum = Utils.GetDiscountNumByCardSeries(Convert.ToInt32(CurentCadrNumber.Substring(0, 5)));
                    }
                    catch (Exception)
                    {

                    }
                    if (LocalDiscNum == -1)
                    {
                        Utils.ToCardLog("Не смог определить номер локальной скидки для карты " + CurentCadrNumber);
                    }

                    AlohaTSClass.DeleteLoyaltyMember(AlohaTSClass.AlohaCurentState.WaterId, (int)AlohaTSClass.AlohaCurentState.TableId, (int)AlohaTSClass.AlohaCurentState.CheckId);
                    if (LocalDiscNum == 6)
                    {
                        string OutMsg = "";
                        int res = AlohaTSClass.ApplyCompByCheckId(LocalDiscNum, AlohaTSClass.AlohaCurentState.WaterId, (int)AlohaTSClass.AlohaCurentState.CheckId, out OutMsg);

                        if (res > 0)
                        {
                            AlohaTSClass.ShowMessage("Скидка применена.");
                            AssignLoyaltiStateTimerEnabled = false;
                            AssignLoyaltiStateInProcess = false;
                            if (!AlohaTSClass.GetCurentStateFromArgs)
                            {
                                CurentfrmCardMoover.SetOk();
                            }
                            return;
                        }

                    }

                    AlohaTSClass.ShowMessage("Данная карта заблокирована либо использована на другом чеке . " + Environment.NewLine + "Скидка не может быть применена.");
                    EventSenderClass.SendAlohaAsincEvent(StopListService.AlohaEventType.LoyaltyOtherError, "", AlohaTSClass.AlohaCurentState.WaterId,
                    AlohaTSClass.GetJobCode(AlohaTSClass.AlohaCurentState.WaterId),
                    CurentCadrNumber,
                    1,
                    (int)AlohaTSClass.AlohaCurentState.TableId,
                    (int)AlohaTSClass.AlohaCurentState.CheckId);

                    AssignLoyaltiStateTimerEnabled = false;
                    AssignLoyaltiStateInProcess = false;
                    if (!AlohaTSClass.GetCurentStateFromArgs)
                    {
                        CurentfrmCardMoover.EnableButton();
                    }
                }
            }


            /*
            if (GetAssingMemberStateErrorCount == 5)
            {
                // AlohaTSClass.ShowMessage("Скидка не может быть применена. Попробуйте еще раз.");
                Utils.ToLog("GetAssingMemberStateErrorCount =5");
                EventSenderClass.SendAlohaAsincEvent(StopListService.AlohaEventType.LoyaltyTimeOutError, "", AlohaTSClass.AlohaCurentState.WaterId,
                    AlohaTSClass.GetJobCode(AlohaTSClass.AlohaCurentState.WaterId),
                    CurentCadrNumber,
                    0,
                    (int)AlohaTSClass.AlohaCurentState.TableId,
                    (int)AlohaTSClass.AlohaCurentState.CheckId);

                // Wmi.WmiProvider.RestartRedirectSrv(AlohainiFile.TermStr + "1", "manager", "manager", ""); 

                Utils.ToLog("AlohaTSClass.DeleteLoyaltyMember");
                AlohaTSClass.DeleteLoyaltyMember(AlohaTSClass.AlohaCurentState.WaterId, (int)AlohaTSClass.AlohaCurentState.TableId, (int)AlohaTSClass.AlohaCurentState.CheckId);
                Utils.ToLog("AlohaTSClass.DeleteLoyaltyMember ok");
                //return;
            }
             * 
             **/

            /*
            if (GetAssingMemberStateErrorCount == 6)
            {
                string ErrMsg = "";
                Utils.ToLog("Провожу карту еще раз");
                if (AlohaTSClass.AssingMember(AlohaTSClass.GetTermNum(),
                     AlohaTSClass.AlohaCurentState.WaterId,
                      (int)AlohaTSClass.AlohaCurentState.TableId,
                     (int)AlohaTSClass.AlohaCurentState.CheckId, CurentCadrNumber, CurentCadrNumber, -1, -1, -1,
                     out ErrMsg))
                {
                    AlohaTSClass.ShowMessage("Запрос отправлен на сервер повторно.");
                }
                else
                {
                    Utils.ToLog("Неудачно " + ErrMsg);
                    if (!AlohaTSClass.GetCurentStateFromArgs)
                    {
                        CurentfrmCardMoover.EnableButton();
                    }
                }


            }
             * 
             **/

            if (GetAssingMemberStateErrorCount == 16)
            {

                int i = AlohaTSClass.GetAssingMemberState(AlohaTSClass.AlohaCurentState.TerminalId);
                Utils.ToCardLog("GetAssingMemberState: " + i.ToString());
                AlohaTSClass.DeleteLoyaltyMember(AlohaTSClass.AlohaCurentState.WaterId, (int)AlohaTSClass.AlohaCurentState.TableId, (int)AlohaTSClass.AlohaCurentState.CheckId);
                if ((i == 5) || (i == 2))
                {
                    AlohaTSClass.ShowMessage("Отсутствуе соединение с сервером карт лояльности либо в чеке отутствуют товары к которым можно применить данную скидку." + Environment.NewLine + "Применяю локальную скидку");

                    int LocalDiscNum = -1;
                    try
                    {
                        LocalDiscNum = Utils.GetDiscountNumByCardSeries(Convert.ToInt32(CurentCadrNumber.Substring(0, 5)));
                    }
                    catch (Exception)
                    {

                    }
                    if (LocalDiscNum == -1)
                    {
                        Utils.ToCardLog("Не смог определить номер локальной скидки для карты " + CurentCadrNumber);
                    }
                    else
                    {
                        string OutMsg = "";
                        AlohaTSClass.ApplyCompByCheckId(LocalDiscNum, AlohaTSClass.AlohaCurentState.WaterId, (int)AlohaTSClass.AlohaCurentState.CheckId, out OutMsg);
                    }

                }
                else
                {
                    AlohaTSClass.ShowMessage("Скидка не может быть применена. Попробуйте еще раз.");
                }
                EventSenderClass.SendAlohaAsincEvent(StopListService.AlohaEventType.LoyaltyTimeOutError, "", AlohaTSClass.AlohaCurentState.WaterId,
                    AlohaTSClass.GetJobCode(AlohaTSClass.AlohaCurentState.WaterId),
                    CurentCadrNumber,
                   -1,
                    (int)AlohaTSClass.AlohaCurentState.TableId,
                    (int)AlohaTSClass.AlohaCurentState.CheckId);

                AssignLoyaltiStateTimerEnabled = false;
                //AssignLoyaltyTimer.Stop();
                AssignLoyaltiStateInProcess = false;
                //CurentfrmCardMoover.EnableButton();
                if (!AlohaTSClass.GetCurentStateFromArgs)
                {
                    CurentfrmCardMoover.SetOk();
                }
            }
            AssignLoyaltiStateInProcess = false;
        }
        static void LogTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Utils.SaveLog();
        }


        static internal void SendDishList()
        {
            try
            {
                StopListService.Service1 ss1 = new PDiscountCard.StopListService.Service1();



                //                StopListService.DishN[] Dn = new PDiscountCard.StopListService.DishN[1];
                //              ss1.SetDishListCompleted += new PDiscountCard.StopListService.SetDishListCompletedEventHandler(ss1_SetDishListCompleted); 
                //            Dn[0]= AlohaTSClass.GetListOfDish().ToArray()[2] ;
                ss1.SetDishListAsync(AlohaTSClass.GetListOfDish().ToArray());

                //    ss1.SetDishList( Dn);
            }
            catch
            {

            }


        }

        static void ss1_SetDishListCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            string k = e.Error.Message;
            int i = 1;
        }



        static private MCard CurentCard;


        static internal void SendCard(List<int> bstrTrack1InfoL,
                                    List<int> bstrTrack2InfoL)
        {

            string bstrTrack1Info = "";
            string bstrTrack2Info = "";
            foreach (int i in bstrTrack1InfoL)
            {
                bstrTrack1Info += ((Keys)i).ToString();
            }
            foreach (int i in bstrTrack2InfoL)
            {


                bstrTrack2Info += ((Keys)i).ToString().Replace("D", "");

            }
            Utils.ToLog("Первая дорожка: " + bstrTrack1Info);
            Utils.ToLog("Вторая дорожка: " + bstrTrack2Info);

            RegCard("", "", "", bstrTrack1Info, bstrTrack2Info, "", "");
        }




        static internal void PrintFriendInfo(string Info)
        {

            List<string> tmp = new List<string>();


            tmp.Add(Info);


            AlohaTSClass.PrintFriendINfo(tmp);
        }

        static public int CurentGloryClosedCheck = 0;

        static internal void PrintFriendInfo(int Code)
        {
            List<string> tmp = new List<string>();

            if (Code == -3)
            {

                tmp.Add("Ваша следующая регистрация происходит");
                tmp.Add("через 4 часа после последнего визита.");
            }
            if (Code == -1)
            {

                tmp.Add("К сожалению мы не можем");
                tmp.Add("отобразить количество посещений");
                tmp.Add("Данное посещение");
                tmp.Add("будет обязательно зарегистрировано");
            }

            AlohaTSClass.PrintFriendINfo(tmp);
        }

        static internal void PrintFriendInfo(int Count, int Days, bool Gold, int TableNum, string Pref_Num, int VisCount)
        {
            List<string> tmp = new List<string>();



            tmp.Add("Стол: " + TableNum);
            tmp.Add("Карта: " + Pref_Num);
            tmp.Add("На Вашу карту «Друг Кофемании» ");
            if (VisCount == 1)
            {
                tmp.Add("начислен 1 визит!");
            }
            else

            {
                tmp.Add("начислено 2 визита!");
                tmp.Add("Участвуйте в фестивале МатчаФест");
                tmp.Add("21 – 30 апреля и получайте двойные");
                tmp.Add("визиты при покупке напитков с матча!");
            }
            
            

            
            

            tmp.Add("Осталось " + Count.ToString() + " посещений за " + Days.ToString() + " " + Utils.GetDayWord(Days));
            /*
            if (Gold)
            {
                tmp.Add("Воспользуйтесь Вашей картой ");
                tmp.Add("Gold MasterCard® и выше ");
                tmp.Add("и получите еще один визит");
                tmp.Add("на карту «Друг Кофемании»!");
            }
             * */
            AlohaTSClass.PrintFriendINfo(tmp);
        }


        static string OldPrefix = "";
        static DateTime OldPrefixTime;

        static internal string CurentCadrNumber = "";

        static internal bool AssignMemberEx(string TrInfo, string memberId, int CheckId, int TermId, int EmplId, int TableId, out string ResMsg)
        {
            ResMsg = "";
            // Utils.ToLog("Провожу карту loyalty Ex " + TrInfo + memberId);
            //if (AlohaTSClass.CheckWindow())

            if (AlohaTSClass.GetCheckById(CheckId).HasUnorderedItems)
            {
                ResMsg = "В чеке есть незаказаные блюда. Невозможно применить скидку.";
                return false;
            }
            /*
            if (AlohaTSClass.AlohaCurentState.CompIsAppled)
            {
                ResMsg ="Скидка уже применена. Невозможно применить скидку еще раз.";
                return false ;
            }
            */

            string ErrMsg = "";
            // Utils.ToLog("TableId " + TableId.ToString());



            if (AlohaTSClass.AssingMember(TermId,
                 EmplId,
                 TableId,
                 CheckId, memberId, TrInfo, -1, -1, -1,
                 out ErrMsg))
            {
                if (!AlohaTSClass.GetCurentStateFromArgs)
                {
                    CurentfrmCardMoover.DisableButton();
                }

                CurentCadrNumber = memberId;
                GetAssingMemberStateErrorCount = 0;
                AssignLoyaltiStateInProcess = false;
                AssignLoyaltiStateTimerEnabled = true;

                //AlohaTSClass.ShowMessage("Обращение к серверу...0");
                //  AssignLoyaltiState();

                WHThreadLoyaltyCard.Set();
                //AssignLoyaltyTimer.Start();

                return true;
            }
            else
            {
                ResMsg = "Ошибка применения карты. " + ErrMsg;
                return false;

            }

        }

        static internal void AssignMember(string TrInfo, string memberId)
        {
            Utils.ToLog("Провожу карту loyalty " + TrInfo);
            //if (AlohaTSClass.CheckWindow())
            {
                Utils.ToLog("TableId " + AlohaTSClass.AlohaCurentState.TableId.ToString());
                string ResMsg = "";
                if (!AssignMemberEx(TrInfo, memberId, (int)AlohaTSClass.AlohaCurentState.CheckId, (int)AlohaTSClass.AlohaCurentState.TerminalId, AlohaTSClass.AlohaCurentState.EmployeeNumberCode, (int)AlohaTSClass.AlohaCurentState.TableId, out ResMsg))
                {
                    AlohaTSClass.ShowMessage(ResMsg);
                }
                /*
                if (AlohaTSClass.GetCheckById((int)AlohaTSClass.AlohaCurentState.CheckId).HasUnorderedItems)
                {
                    AlohaTSClass.ShowMessage("В чеке есть незаказаные блюда. Невозможно применить скидку.");
                    return;
                }

                if (AlohaTSClass.AlohaCurentState.CompIsAppled)
                {
                    AlohaTSClass.ShowMessage("Скидка уже применена. Невозможно применить скидку еще раз.");
                    return;
                }


                string ErrMsg = "";
                if (AlohaTSClass.AssingMember(AlohaTSClass.AlohaCurentState.TerminalId,
                     AlohaTSClass.AlohaCurentState.EmployeeNumberCode,
                     (int)AlohaTSClass.AlohaCurentState.TableId,
                     (int)AlohaTSClass.AlohaCurentState.CheckId, memberId, TrInfo, -1, -1, -1,
                     out ErrMsg))
                {
                    GetAssingMemberStateErrorCount = 0;
                    AssignLoyaltyTimer.Start();
                }
                else
                {
                    AlohaTSClass.ShowMessage("Ошибка применения карты. ");

                }
                 * */
            }
        }



        static internal int RegCard(string bstrAccountNumber, string bstrCustomerName,
                                    string bstrExpirationDate, string bstrTrack1Info,
                                    string bstrTrack2Info, string bstrTrack3Info, string bstrRawMagcardData)
        {
            return RegCard(bstrTrack1Info, bstrTrack2Info, bstrTrack3Info);
        }

        /*
        static internal void TryVizitOnGold(int ChkId)
        {
            int P = 0;
            int V = 0;
            string GoldAttr = AlohaTSClass.GetGoldDiscountAttr(ChkId, out P, out V);
            decimal summ =  (decimal)AlohaTSClass.GetCheckSum(ChkId);

            Utils.ToCardLog("GoldAttr =" + GoldAttr);
            if (GoldAttr != "")
            {
                int VisitCount = -10;
                int DayCount = -10;
                int VisitTotal = -10;
                int DayTotal = -10;
                int Gold = 0;
                string Prefix = GoldAttr.Substring(0, 3);
                string Num = GoldAttr.Substring(4, GoldAttr.Length - 4);
                Utils.ToCardLog("Prefix =" + Prefix + "Num =" + Num);
                AlohaTSClass.CheckWindow();
                int k = ToBase.DoVizit2(Prefix, Num, AlohainiFile.DepNum, AlohaTSClass.AlohaCurentState.CheckNum,
                           AlohaTSClass.AlohaCurentState.TerminalId, summ, DateTime.Now, out  VisitCount, out DayCount, out  VisitTotal, out DayTotal, out Gold, 1);
                if (k == -1)
                {
                    // AlohaTSClass.ShowMessage("Нет связи с базой данных. Посещение зафиксировано.");

                    PrintFriendInfo(-1);


                }
                if (VisitCount == 0)
                {

                    if (V - 1 > 0)
                    {
                        //   AlohaTSClass.ShowMessage("Осталось посещений: " + (V - 1).ToString() + " Осталось дней: " + P.ToString());
                        PrintFriendInfo(V - 1, P, false, AlohaTSClass.GetCheckById(Convert.ToInt32(AlohaTSClass.AlohaCurentState.CheckId)).TableNumber, Prefix + " " + Num);
                    }
                    else
                    {
                        // AlohaTSClass.ShowMessage("Совершенно необходимое количество посещений. Можно выдавать карту.");
                        PrintFriendInfo("Совершенно необходимое количество посещений." + Environment.NewLine + " Можно выдавать карту.");
                    }
                    return;
                }

                if (VisitCount == 11)
                {
                    Utils.ToCardLog("Не удалось наложить воторой визит по карте Gold. Количество визитов по данному чеку: " + VisitCount);
                }


            }
        }
        */


        public delegate bool RegCardSubScriberdelegate(string bstrTrack1Info, string bstrTrack2Info, string bstrTrack3Info);
        static internal void AddRegCardSubscr(RegCardSubScriberdelegate Subscr)
        {
            Utils.ToCardLog("Добавил подписчика проводки карт");
            RegCardSubScribers.Add(Subscr);
        }
        static internal void RemoveRegCardSubscr(RegCardSubScriberdelegate Subscr)
        {
            Utils.ToCardLog("Удалил подписчика проводки карт");
            RegCardSubScribers.Remove(Subscr);
        }

        static List<RegCardSubScriberdelegate> RegCardSubScribers = new List<RegCardSubScriberdelegate>();

        static internal frmCardMoover CurentfrmCardMoover = null;
        static internal bool CurentfrmCardMooverEnable = false;

        

        static internal int RegCard(string bstrTrack1Info,string bstrTrack2Info, string bstrTrack3Info)
        {
            Utils.ToCardLog("Проведена карта "+bstrTrack1Info+bstrTrack2Info);
            foreach (RegCardSubScriberdelegate d in RegCardSubScribers)
            {
                Utils.ToCardLog("Отправляю магридер во внешнюю процедуру");
              bool res= d(bstrTrack1Info,bstrTrack2Info, bstrTrack3Info);
              if (!res)
              {
                  return 1;
              }
            }


            try
            {
                
                if (AssignLoyaltiStateTimerEnabled)
                {
                    Utils.ToCardLog("Не закончена предыдущая транзакция. Выхожу");
                    return 1;
                }
                
                CurentCard = new MCard(bstrTrack1Info, bstrTrack2Info);

                
                if (CurentCard.bad)
                {
                    AlohaTSClass.ShowMessage("Не могу прочитать карту. Проведите еще раз.");
                    Utils.ToLog("[RegCard] плохая карта " + bstrTrack1Info + " " + bstrTrack2Info);
                    OldPrefixTime = DateTime.Now;
                    OldPrefix = bstrTrack1Info;
                    return 0;
                }

                
                if (OldPrefix != "")
                {
                    Utils.ToLog("OldPrefix");
                    if ((DateTime.Now - OldPrefixTime).Seconds < 2)
                    {
                        Utils.ToLog("OldPrefix");
                        CurentCard = new MCard(OldPrefix, bstrTrack1Info);

                    }
                }
                OldPrefix = "";
               
                if (CurentCard.mType == CardTypes.PodKarta)
                {
                    Utils.ToCardLog("Подарочная карта");
                    return 0;
                }

                if (CurentCard.mType == CardTypes.Manager)
                {
                    Utils.ToCardLog("Карта менеджера");
                    return 0;
                }

                AlohaTSClass.CheckWindow(); // Оставить для менеджеров

                // Utils.ToCardLog("Должность: " + AlohaTSClass.GetJobCode(AlohaTSClass.AlohaCurentState.WaterId).ToString ());
                //Utils.ToCardLog("AlohaTSClass.GetJobCode");
                if (//(AlohaTSClass.GetJobCode(AlohaTSClass.AlohaCurentState.WaterId) != 10) &&
                    //(AlohaTSClass.GetJobCode(AlohaTSClass.AlohaCurentState.WaterId) != 40) &&
                    (!AlohaTSClass.GetCurentStateFromArgs)
                    )
                {
                    if (CurentfrmCardMooverEnable == false)
                    {
                        AlohaTSClass.ShowMessage("Для проведения карты нажмите на кнопку Карта");
                        return 1;
                    }
                }


                //                Utils.ToCardLog("((iniFile.CardEmulate) && (!iniFile.WinHook))");

                Utils.ToCardLog(CurentCard.mType.ToString());
                if (CurentCard.mType == CardTypes.SM)
                {
                    Utils.ToCardLog("Вызов системы. Не поддерживаю");
                    try
                    {
                        /*
                        AlohaEventVoids.HidefrmCard();
                        PDiscountCard.Sistema.SysReq SR = new PDiscountCard.Sistema.SysReq();
                        PDiscountCard.Sistema.UserResp UR = SR.CardRequest("SM", CurentCard.Num);
                        PDiscountCard.Sistema.frmEatingSelect f = new PDiscountCard.Sistema.frmEatingSelect();
                        f.SetCard(UR);
                        f.ShowDialog();
                         * */
                    }
                    catch (Exception e)
                    {
                        Utils.ToCardLog("Вызов системы Error " + e.Message);
                    }
                    return 1;


                }

                /*
                if (CurentCard.mType != CardTypes.Friend)
                {
                    if ((iniFile.CardEmulate) && (!iniFile.WinHook))
                    {
                        CardEmulate cc = new CardEmulate();
                        if (cc.EmulateLoyaltyCard(CurentCard.Prefix, CurentCard.Num))
                        {
                            AlohaTSClass.SetDiscountAttr((int)AlohaTSClass.AlohaCurentState.CheckId, CurentCard.Prefix + " " + CurentCard.Num, false, 0, 0);
                            //Utils.ToCardLog("End");
                            return 1;
                        }
                    }
                }
                 * */
                int VisitCount = -10;
                int DayCount = -10;
                int VisitTotal = -10;
                int DayTotal = -10;
                int Gold = 0;
                Check Ch = AlohaTSClass.GetCheckById((int)AlohaTSClass.AlohaCurentState.CheckId);
                if (CurentCard.mType == CardTypes.Friend)
                {
                    Utils.ToLog("[RegCard] Это карта друга");
                    Utils.ToLog("[RegCard] Терминал: " + AlohaTSClass.AlohaCurentState.TerminalId +
                        "Официант: " + AlohaTSClass.AlohaCurentState.WaterId +
                        "Чек: " + AlohaTSClass.AlohaCurentState.CheckId + "(" + AlohaTSClass.AlohaCurentState.CheckNum + ")");

                   
                    if (Ch.OrderedDishez.Count == 0)
                    {
                        if (AlohaTSClass.IsTableServise())
                        {
                            AlohaTSClass.ShowMessage("Нельзя применить посещение на чек с незаказанными блюдами");
                            return 1;
                        }
                        else
                        {
                            string Outstr = "";
                            AlohaTSClass.OrderAllItems((int)AlohaTSClass.AlohaCurentState.CheckId, (int)AlohaTSClass.AlohaCurentState.TableId, out Outstr);
                        }
                    }
                    if (AlohaTSClass.GetDiscountAttr((int)AlohaTSClass.AlohaCurentState.CheckId) != "")
                    {
                        AlohaTSClass.ShowMessage("Нельзя применить посещение на чек с зарегистрированным посещением. Совсем нельзя. ");
                        return 1;
                    }
                    if (Ch.Summ < 150)
                    {
                        AlohaTSClass.ShowMessage("Нельзя применить посещение на чек с суммой менее 150 руб.");
                        return 1;
                    }

                    if (Ch.Comps.Count > 0)
                    {
                        foreach (var d in Ch.Comps)
                        {
                            if (d.CompType != 6)
                            {
                                AlohaTSClass.ShowMessage("Нельзя применить посещение на чек со скидкой.");
                                return 1;
                            }
                        }
                    }

                    int k = 0;


                    int VisCount = 1;

               

                        int compId = 0;
                        MB.MBClient mbClient = new MB.MBClient();
                        if (mbClient.UsingMB())
                        {
                            k = mbClient.GetFrendConvertCodeCardProcessing(Ch, CurentCard.Prefix, CurentCard.Num, out  VisitCount, out DayCount, out  VisitTotal, out DayTotal, out compId);
                            if (k == -1)
                            {
                                //Это чтобы карта записалась в файл, если MB недоступен.
                                k = ToBase.DoVizit2(CurentCard.Prefix, CurentCard.Num.ToString(), AlohainiFile.DepNum, AlohaTSClass.AlohaCurentState.CheckNum,
                             AlohaTSClass.AlohaCurentState.TerminalId, Ch.Summ, DateTime.Now, out  VisitCount, out DayCount, out  VisitTotal, out DayTotal, out Gold, 0);
                            }

                        }
                        else
                        {
                            k = ToBase.DoVizit2(CurentCard.Prefix, CurentCard.Num.ToString(), AlohainiFile.DepNum, AlohaTSClass.AlohaCurentState.CheckNum,
                             AlohaTSClass.AlohaCurentState.TerminalId, Ch.Summ, DateTime.Now, out  VisitCount, out DayCount, out  VisitTotal, out DayTotal, out Gold, 0);
                        }
                    //}


                    if (k == -1)
                    {
                        AlohaTSClass.ShowMessage("Нет связи с базой данных. Посещение зафиксировано.");

                        PrintFriendInfo(-1);

                        return 1;
                    }

                    if (VisitCount == -5)
                    {
                        AlohaTSClass.ShowMessage("Карта не зарегистрирована.");
                        PrintFriendInfo("Карта не зарегистрирована.");
                        return 1;
                    }
                    if (VisitCount == -1)
                    {
                        AlohaTSClass.ShowMessage("Карта заблокирована.");
                        PrintFriendInfo("Карта заблокирована.");
                        return 1;
                    }
                    if (VisitCount == -4)
                    {
                        AlohaTSClass.ShowMessage("Визит по этому чеку уже зарегистрирован.");
                        PrintFriendInfo("Визит по этому чеку уже зарегистрирован.");
                        return 1;
                    }

                    if (VisitCount == -3)
                    {
                        AlohaTSClass.ShowMessage("Не прошло необходимое время с момента последнего визита.");
                        PrintFriendInfo(-3);
                        return 1;
                    }
                    if (VisitCount == -2)
                    {
                        string Mess = "Истекло время отведенное на совершения посещений." + Environment.NewLine
                                + " Посещения обнулены. Осталось посещений: " + (VisitTotal).ToString() + " Осталось дней: " + DayCount.ToString();
                        AlohaTSClass.ShowMessage(Mess);
                        PrintFriendInfo(Mess);
                        CurentfrmCardMoover.SetOk();
                        return 1;
                    }

                    if (VisitCount == 1)
                    {
                        AlohaTSClass.ShowMessage("Совершенно необходимое количество посещений. Можно выдавать карту.");
                        PrintFriendInfo("Совершенно необходимое количество посещений." + Environment.NewLine + " Можно выдавать карту.");
                        CurentfrmCardMoover.SetOk();
                        return 1;
                    }
                    bool gold = Convert.ToBoolean(Gold);
                    if (VisitCount > 1)
                    {
                        try
                        {
                            AlohaTSClass.SetDiscountAttr((int)AlohaTSClass.AlohaCurentState.CheckId, CurentCard.Prefix + " " + CurentCard.Num, gold, VisitCount, DayCount);
                            Utils.ToCardLog("Наложил аттрибут: " + AlohaTSClass.DiscountAttrName + " значение: " + CurentCard.Prefix + " " + CurentCard.Num + " на чек " + AlohaTSClass.AlohaCurentState.CheckNum);

                        }
                        catch (Exception e)
                        {
                            Utils.ToCardLog("[ERROR]не удалось наложить аттрибут " + e.Message);
                        }
                        PrintFriendInfo((VisitCount), DayCount, gold, AlohaTSClass.GetCheckById(Convert.ToInt32(AlohaTSClass.AlohaCurentState.CheckId)).TableNumber, CurentCard.Prefix + " " + CurentCard.Num, VisCount);
                        AlohaTSClass.ShowMessage("Осталось посещений: " + (VisitCount).ToString() + " Осталось дней: " + DayCount.ToString());
                        CurentfrmCardMoover.SetOk();
                        return 1;
                    }
                }
                else if (CurentCard.mType == CardTypes.Sber)
                {
                    if (AlohaTSClass.CheckWindow())
                    {
                        if (AlohaTSClass.AlohaCurentState.CompIsAppled)
                        {
                            AlohaTSClass.ShowMessage("Скидка уже применена");
                            return 1;
                        }
                        if (CurentCard.DiscountType != 6)
                        {
                            if (AlohaTSClass.GetDiscountAttr((int)AlohaTSClass.AlohaCurentState.CheckId) != "")
                            {
                                AlohaTSClass.ShowMessage("Нельзя применить скидку на чек с зарегистрированным посещением. Совсем нельзя. ");
                                return 1;
                            }
                        }




                        try
                        {
                            int k = 0;
                            int compId=0;
                            MB.MBClient mbClient = new MB.MBClient();

                            k = mbClient.GetFrendConvertCodeCardProcessing(Ch, CurentCard.Prefix, CurentCard.Num, out  VisitCount, out DayCount, out  VisitTotal, out DayTotal,out compId);




                        }
                        catch { }



                        string Emess = "";
                        //int CompRes = AlohaTSClass.ApplyComp(CurentCard.DiscountType, out Emess);
                        int CompRes = AlohaTSClass.ApplyCompManagerOverride(CurentCard.DiscountType, out Emess);

                        Utils.ToLog("[RegCard] Результат успешного наложения скидки: " + CompRes.ToString());
                        if (CompRes == 0)
                        {

                            AlohaTSClass.ShowMessage("Cкидка не применена.");
                            AlohaTSClass.WriteToDebout("Cкидка не применена  тип: " + CurentCard.DiscountType + ", чек: " + AlohaTSClass.AlohaCurentState.CheckNum + " Причина: " + Emess);
                        }
                        else
                        {
                            //  AlohaTSClass.SetDiscountAttr((int)AlohaTSClass.AlohaCurentState.CheckId, CurentCard.Prefix + " " + CurentCard.Num, false, 0, 0);
                            AlohaTSClass.WriteToDebout("Применена скидка тип: " + CurentCard.DiscountType + ", чек: " + AlohaTSClass.AlohaCurentState.CheckNum);
                            AlohaTSClass.ShowMessage("Применена скидка.");
                        }
                        CurentfrmCardMoover.SetOk();
                        return 1;

                    }
                }
                else
                    if (CurentCard.mType == CardTypes.Discount)
                    {
                        Utils.ToLog("[RegCard] Это скидочная карта");
                        
                        if (CurentCard.DiscountType == -1)
                        {
                            AlohaTSClass.ShowMessage("Карта не активна для данного подразделения");
                            return 1;

                        }
                        
                        if (AlohaTSClass.CheckWindow())
                        {
                            if (AlohaTSClass.AlohaCurentState.CompIsAppled)
                            {
                                AlohaTSClass.ShowMessage("Скидка уже применена");
                                return 1;
                            }
                            if (CurentCard.DiscountType != 6)
                            {
                                if (AlohaTSClass.GetDiscountAttr((int)AlohaTSClass.AlohaCurentState.CheckId) != "")
                                {
                                    AlohaTSClass.ShowMessage("Нельзя применить скидку на чек с зарегистрированным посещением. Совсем нельзя. ");
                                    return 1;
                                }
                            }

                            try
                            {
                                int k = 0;
                                int compId=0;
                                MB.MBClient mbClient = new MB.MBClient();
                                if (mbClient.UsingMB())
                                {
                                    k = mbClient.GetFrendConvertCodeCardProcessing(Ch, CurentCard.Prefix, CurentCard.Num, out  VisitCount, out DayCount, out  VisitTotal, out DayTotal,out compId);
                                }
                                else
                                {

                                    k = ToBase.DoVizit(CurentCard.Prefix, CurentCard.Num, AlohainiFile.DepNum, AlohaTSClass.AlohaCurentState.CheckNum,
                       AlohaTSClass.AlohaCurentState.TerminalId, DateTime.Now, out  VisitCount, out DayCount, out  VisitTotal, out DayTotal);
                                    Utils.ToLog("[RegCard] Отправил в базу информацию о скидке. Результат: " + k.ToString() + "Ответ базы: " + VisitCount.ToString());
                                }

                                try
                                {
                                if (CurentCard.Prefix == "80827")
                                {
                                    Utils.ToLog("[RegCard]  Распознал  супервип");
                                    if (k == -1)
                                    {
                                        AlohaTSClass.ShowMessage("Нет связи с базой данных. Рекомендую применить скидку");
                                        compId = 8;
                                    }
                                    else
                                    {
                                        if (VisitCount == -1)
                                        {
                                            AlohaTSClass.ShowMessage("Карта не активна для данного подразделения. Скидки не будет.");
                                            return 1;
                                        }
                                        else if (VisitCount == -5)
                                        {
                                            AlohaTSClass.ShowMessage("Карта заблокирована для данного подразделения. Скидки не будет.");
                                            return 1;
                                        }
                                        else
                                        {
                                            Utils.ToCardLog("[RegCard]  Положительный ответ от сервера. Будет применена скидка");
                                        }
                                    }

                                    string Emess2 = "";
                                    int CompRes2 = AlohaTSClass.ApplyCompManagerOverride(compId, out Emess2);
                                    Utils.ToLog("[RegCard] Результат успешного наложения скидки: " + CompRes2.ToString());

                                    if (CompRes2 == 0)
                                    {

                                        AlohaTSClass.ShowMessage("Cкидка не применена.");
                                        AlohaTSClass.WriteToDebout("Cкидка не применена  тип: " + CurentCard.DiscountType + ", чек: " + AlohaTSClass.AlohaCurentState.CheckNum + " Причина: " + Emess2);
                                    }
                                    else
                                    {
                                        //  AlohaTSClass.SetDiscountAttr((int)AlohaTSClass.AlohaCurentState.CheckId, CurentCard.Prefix + " " + CurentCard.Num, false, 0, 0);
                                        AlohaTSClass.WriteToDebout("Применена скидка тип: " + CurentCard.DiscountType + ", чек: " + AlohaTSClass.AlohaCurentState.CheckNum);
                                        AlohaTSClass.ShowMessage("Применена скидка.");
                                    }
                                    CurentfrmCardMoover.SetOk();
                                    return 1;


                                }
                                else if (CurentCard.DiscountType != 8 && CurentCard.Prefix == "VIP" && Convert.ToInt64(CurentCard.Num) < 2500)
                                {
                                    Utils.ToLog("Вип карта первой сотни. n=" + CurentCard.Num + ". Надо проверить на неотключенность");
                                    if (k == -1)
                                    {
                                        AlohaTSClass.ShowMessage("Нет связи с базой данных. Скидки не будет.");
                                        return 1;
                                    }

                                    if (VisitCount < 0)
                                    {
                                        AlohaTSClass.ShowMessage("Карта заблокирована либо не активна для данного подразделения. Скидки не будет.");
                                        return 1;
                                    }
                                }


                                else if ((CurentCard.DiscountType == 1)|| (CurentCard.DiscountType == 4))
                                {
                                    if (VisitCount == -7)
                                    {
                                        AlohaTSClass.ShowMessage("Не прошло необходимое время с момента последнего визита.");
                                        
                                        return 1;
                                    }
                                }
                                /* Вернуть, когда появится инструмент добавления карт
                            else if(CurentCard.DiscountType != 8 )
                            {
                                if (VisitCount == -5)
                                {
                                    AlohaTSClass.ShowMessage("Карта не зарегистрирована.");

                                    return 1;
                                }
                                if (VisitCount == -1)
                                {
                                    AlohaTSClass.ShowMessage("Карта заблокирована.");

                                    return 1;
                                }
                            }
                                 * */
                            }
                                catch (Exception e)
                                {
                                    Utils.ToLog("Error проверки первой сотни. " + e.Message);
                                }


                            }
                            catch (Exception e)
                            {
                                Utils.ToLog("[ERROR] [RegCard] Ошибка Отправки в базу информацию о скидке. Результат: " + e.Message);
                            }

                            string Emess = "";
                            //int CompRes = AlohaTSClass.ApplyComp(CurentCard.DiscountType, out Emess);

                            int CompRes = AlohaTSClass.ApplyCompManagerOverride(CurentCard.DiscountType, out Emess);
                            Utils.ToLog("[RegCard] Результат успешного наложения скидки: " + CompRes.ToString());
                            if (CompRes == 0)
                            {

                                AlohaTSClass.ShowMessage("Cкидка не применена.");
                                AlohaTSClass.WriteToDebout("Cкидка не применена  тип: " + CurentCard.DiscountType + ", чек: " + AlohaTSClass.AlohaCurentState.CheckNum + " Причина: " + Emess);
                            }
                            else
                            {
                                //  AlohaTSClass.SetDiscountAttr((int)AlohaTSClass.AlohaCurentState.CheckId, CurentCard.Prefix + " " + CurentCard.Num, false, 0, 0);
                                AlohaTSClass.WriteToDebout("Применена скидка тип: " + CurentCard.DiscountType + ", чек: " + AlohaTSClass.AlohaCurentState.CheckNum);
                                AlohaTSClass.ShowMessage("Применена скидка.");
                            }
                            CurentfrmCardMoover.SetOk();
                            return 1;
                        }
                        else
                        {
                            AlohaTSClass.ShowMessage("Не удалось прочесть параметры Aloha");
                        }
                    }
                Utils.ToLog("[RegCard] Не знаю такую карту.");
                try
                {
                    CurentManagerCard = Convert.ToInt32(CurentCard.Num);
                }
                catch (Exception e)
                {
                    Utils.ToLog("Не удалось перевести в int: " + CurentCard.Num);
                }
                AlohaTSClass.ShowMessageInternal("Неизвестный формат карты");
                return 0;
            }
            catch (Exception e)
            {
                Utils.ToLog(e.Message);
                return 0;
            }


        }


        public class ActivateResult
        {
            public bool Result = false;
            public string Comment = "";
        }







        /*
        static  public  void ResetCardHandler(Object stateInfo)
        {
            try
            {
                ActivityCard vActivityCard = (ActivityCard)stateInfo;
                if (vActivityCard == null)
                    return;

                vActivityCard.m_AlohaPeripherals.ReleaseMagcardInterceptor(vActivityCard);
                vActivityCard.m_AlohaPeripherals.RegisterMagcardInterceptor(vActivityCard);
                Utils.ToLog("Успешное создание vActivityCard");
            }
            catch (Exception E)
            {

                Utils.ToLog("Ошибка при создании vActivityCard");

            }
        }


        */

    }


}

