using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Linq;

namespace PDiscountCard
{
    public static class Shtrih2
    {
        static int LastBaudRate = 0;
        static int LastComNumber = 0;

        public static bool Conn()
        {
            var res = ConnEx();
            Utils.ToLog("KKM SerialNumberInt: " + SerialNumberInt);
            MainClass.KKMNumber = SerialNumberInt;
            return res;
        }
        private static bool ConnEx()
        {
           try
            {
                Utils.ToLog("FR Connect ConnectionType: " + iniFile.FRConnectionType);
                switch (iniFile.FRConnectionType)
                {
                    case 0:

                        ConnectionType = 0;

                        Timeout = iniFile.FRTimeout;
                        if (iniFile.USBFR)
                        {
                            Utils.ToLog("USBFR");
                            BaudRate = iniFile.USBFRBaudRate;
                            ComNumber = iniFile.USBFRPort;
                            Utils.ToLog("USBFRBaudRate: " + BaudRate + " USBFRPort: " + ComNumber + " Timeout " + Timeout);
                        }
                        Password = 30;
                        //  SetExchangeParam();
                        Connect();
                        if (ResultCode != 0)
                        {
                            Utils.ToLog("ResCode=" + ResultCodeDescription + " (" + ResultCode.ToString() + ")");
                            if (!iniFile.USBFR)
                            {
                                Utils.ToLog("Ищу принтер");
                                FindDevice();
                                if (ComNumber > 100)
                                {
                                    Utils.ToLog("Нашел принтер скорость: " + BaudRate + " порт: " + ComNumber);
                                    ComNumber = LastComNumber;
                                    BaudRate = LastBaudRate;
                                    Utils.ToLog("Оставил старые параметры скорость: " + BaudRate + " порт: " + ComNumber);
                                }
                                else
                                {
                                    Utils.ToLog("Нашел принтер скорость: " + BaudRate + " порт: " + ComNumber);
                                    LastComNumber = ComNumber;
                                    LastBaudRate = 5;
                                }


                                Connect();
                                if (ResultCode != 0)
                                {
                                    Utils.ToLog("Ошибка соединения с принтером ResCode=" + ResultCodeDescription);
                                    return false;
                                }
                                else
                                {
                                    Utils.ToLog("ResCode=" + ResultCodeDescription);
                                    Utils.ToLog("Соединился с принтером скорость: " + BaudRate + " порт: " + ComNumber);
                                    return true;
                                }
                            }
                            else
                            {
                                Utils.ToLog("Ищу USB принтер");
                                for (int b = 0; b <= 6; b++)
                                {
                                    BaudRate = b;
                                    Connect();
                                    if (ResultCode == 0)
                                    {
                                        Utils.ToLog("Нашел USB принтер скорость " + b);
                                        if (BaudRate != iniFile.USBFRBaudRate)
                                        {
                                            Utils.ToLog("Устанавливаю скорость " + iniFile.USBFRBaudRate);
                                            BaudRate = iniFile.USBFRBaudRate;
                                            SetExchangeParam();
                                            Connect();
                                            if (ResultCode != 0)
                                            {
                                                Utils.ToLog("Не смог установить  скорость " + iniFile.USBFRBaudRate + " возвращаю " + b.ToString() + " ResultCode " + ResultCode.ToString());
                                                BaudRate = b;
                                                SetExchangeParam();
                                                Connect();
                                            }
                                            if (ResultCode == 0)
                                            {
                                                Utils.ToLog("Соединился с принтером скорость: " + BaudRate + " порт: " + ComNumber);
                                                return true;
                                            }
                                        }
                                    }
                                }
                                Utils.ToLog("Не нашел USB принтер порт " + iniFile.USBFRPort);

                                return false;
                            }
                        }
                        else
                        {
                            Utils.ToLog("ResCode=" + ResultCodeDescription);
                            Utils.ToLog("Соединился с принтером скорость: " + BaudRate + " порт: " + ComNumber);
                            return true;
                        }
                        break;

                    case 6:

                        ConnectionType = 6;
                        TCPPort = iniFile.FRTCPPort;
                        TCPConnectionTimeout = iniFile.TCPConnectionTimeout;
                        ComputerName = iniFile.FRComputerName;
                        Utils.ToLog(String.Format("Подключаюсь по TCP. ComputerName: {0};  TCPPort: {1}; TCPConnectionTimeout: {2}", ComputerName, TCPPort, TCPConnectionTimeout));
                        Connect();
                        Disconnect();
                        if (ResultCode == 0)
                        {
                            Utils.ToLog("Соединился с принтером скорость: ");
                            return true;
                        }
                        else
                        {
                            Utils.ToLog(String.Format("Ошибка соединения. ResultCode: {0}; ResultCodeDescription: {1}", ResultCode, ResultCodeDescription));
                            return false;
                        }
                        break;
                    default:
                        Connect();
                        Disconnect();
                        if (ResultCode == 0)
                        {
                            Utils.ToLog("Соединился с принтером c настройками драйвера TCPPort: " + TCPPort + " ComputerName:" + ComputerName + 
                                " ConnectionType: " + ConnectionType+ " скорость: " + BaudRate + " порт: " + ComNumber);
                            return true;
                        }
                        else
                        {
                            Utils.ToLog("ResCode=" + ResultCodeDescription + " (" + ResultCode.ToString() + ")");

                            if (ConnectionType == 0)
                            {
                                Utils.ToLog("Ищу принтер");
                                FindDevice();

                                Connect();
                                Disconnect();
                            }
                        if (ResultCode == 0)
                        {
                            Utils.ToLog("Соединился с принтером c настройками драйвера TCPPort: " + TCPPort + " ComputerName:" + ComputerName +
                                " ConnectionType: " + ConnectionType + " скорость: " + BaudRate + " порт: " + ComNumber);
                            return true;
                        }
                        else
                        {

                            Utils.ToLog(String.Format("Ошибка соединения. ResultCode: {0}; ResultCodeDescription: {1}", ResultCode, ResultCodeDescription));
                            return false;
                        }
                        }
                        break;
                }
                
            }
            catch (Exception e)
            {
                Utils.ToLog("Error Conn " + e.Message);
                return false;
            }
        }


        static object mShtrih;
        //static string DllName = "AddIn.DrvFr3";
        static string DllName = "AddIn.DrvFr";
        //static string DllName = "DrvFrLib";

        static public int CharCountWidth = 36;

        static internal Type _ShtrihType;
        static internal Type ShtrihType
        {
            get
            {
                return _ShtrihType;
            }
        }


        static public bool CreateShtrih()
        {
            if (_ShtrihType != null)
            {
                try
                {
                    Disconnect();

                }
                catch
                { }
            }
            _ShtrihType = Type.GetTypeFromProgID(DllName);
            mShtrih = Activator.CreateInstance(_ShtrihType);
            if (_ShtrihType == null)
            {
                Utils.ToLog("[Error] не смог создать драйвер штриха", 6);
                return false;
            }
            //QuereTimer = new System.Timers.Timer(100);
            TimerSyncPoint = 0;
            CommandQuere = new List<ShtrihCommandBlock>();

            RushCommandQuere = new List<ShtrihCommandBlock>();


            
            ShtrihWorkThread = new Thread(CheckCommandQuere);
            ShtrihWorkThread.SetApartmentState(ApartmentState.STA);
            ShtrihWorkThread.Start();



            return true;
        }

        static private int TimerSyncPoint = 0;

        static private List<ShtrihCommandBlock> CommandQuere;
        static private List<ShtrihCommandBlock> RushCommandQuere;
        //static private List<CProp> CurentProp = new List<CProp>();
        //static System.Timers.Timer QuereTimer;

        static bool CommandRunning = false;

        static internal Thread ShtrihWorkThread;

        static bool mExitFiskalThread = false;
        internal static void ExitFiskalThread()
        {
            Disconnect();
            mExitFiskalThread = true;
        }

        internal static bool QuereIsEmpty()
        {
            if (CommandQuere == null)
            {
                return true;
            }
            return ((CommandQuere.Count == 0) && (RushCommandQuere.Count == 0));
        }

        internal static bool QuereContainsChk(Check Chk)
        {
            if (CommandQuere == null)
            {
                return false;
            }
            return CommandQuere.Where(a=>a.ChkOwner!=null && a.ChkOwner.AlohaCheckNum==Chk.AlohaCheckNum && a.ChkOwner.Summ==Chk.Summ).Count()>0 ;
        }


        static void CheckCommandQuere()
        {
            TimerSyncPoint = 0;
            Conn();
            Disconnect();
            while (!mExitFiskalThread)
            {

                try
                {
                    if ((CommandQuere.Count == 0) && (RushCommandQuere.Count == 0))
                    {
                        TimerSyncPoint = 0;

                        Thread.Sleep(200);
                        continue;
                    }
                    int sync = Interlocked.CompareExchange(ref TimerSyncPoint, 1, 0);
                    GetShortECRStatus();
                    switch (ECRAdvancedMode)
                    {
                        case 0:

                            if (ECRMode == 12)
                            {
                                continue;
                            }
                            //Это для срочного чтения значения из таблицы
                            if ((RushCommandQuere.Count > 0) && (!RushCommandQuere[0].GetValComplited))
                            {
                                try
                                {
                                    List<int> IgnoreErrors = new List<int>();
                                    RunCommandCommandBlock(RushCommandQuere[0], RushCommandQuere[0].GetFirstBlock(), out IgnoreErrors);
                                    /*
                                    if ((ResultCode != 0) && (!IgnoreErrors.Contains(ResultCode)))
                                    {

                                        if (!(ShowMessage(ResultCodeDescription)))
                                        {
                                            RushCommandQuere.Remove(RushCommandQuere[0]);
                                            MainClass.FiskalPrinterIsPrinting = false;
                                        }

                                        //ShowMessage2(ResultCodeDescription);
                                        TimerSyncPoint = 0;
                                        continue;
                                    }


                                    */
                                    //RushCommandQuere.RemoveRange(0,RushCommandQuere[0].GetFirstBlock().Count);
                                    //RushCommandQuere.Remove(RushCommandQuere[0]);
                                    continue;
                                }
                                catch (Exception ee)
                                {
                                    Utils.ToCardLog("[Error] при выполнении команды  фискальником " + ee.Message);
                                   
                                    MainClass.FiskalPrinterIsPrinting = false;
                                    TimerSyncPoint = 0;
                                    //return;
                                    continue;
                                }
                                finally
                                { try { RushCommandQuere.Remove(RushCommandQuere[0]); } catch { } }
                            }

                            if (CommandRunning)
                            {
                                if (CommandQuere[0].RemoveFirstBlock())
                                {
                                    CommandQuere.Remove(CommandQuere[0]);
                                }
                                CommandRunning = false;
                            }
                            if (CommandQuere.Count > 0)
                            {

                                try
                                {

                                    List<int> IgnoreErrors = new List<int>();
                                    if (CommandQuere[0].GetFirstBlock().Where(a => a.Name == "OpenCheck").Count() > 0) //Это прверяем перед открытием чека а не висит ли продажа
                                    {
                                        GetShortECRStatus();
                                        if (ECRMode == 8)
                                        {
                                            Utils.ToLog("It's Open check. Try CancelCheck. ");
                                            CancelCheck();
                                            WaitForPrinting();
                                            
                                        }
                                    }

                                    RunCommandCommandBlock(CommandQuere[0], CommandQuere[0].GetFirstBlock(), out IgnoreErrors); //запускаем команду

                                    if (ResultCode == -1) //Это для если не понятно прошла или нет команда
                                    {
                                        Utils.ToLog("ResultCode: " + ResultCode.ToString() + " try get status", 8);

                                        Thread.Sleep((1000));
                                        GetShortECRStatus();
                                        if (ResultCode == 0)
                                        {
                                            WaitForPrinting();

                                            if (CommandQuere[0].GetLastBlock().Where(a => a.Name == "CloseCheck").Count() > 0)
                                            {
                                                Utils.ToLog("It's Close check block. ");
                                                if (CommandQuere[0].GetFirstBlock().Last().Name == "CloseCheck")
                                                {
                                                    Utils.ToLog(String.Format("Last command is CloseCheck. ECRMode: {0} ({1})",ECRMode,ECRModeDescription));
                                                    if (ECRMode == 8)
                                                    {
                                                        Utils.ToLog("Check is open. Try Close check again");
                                                        _CloseCheck();
                                                        WaitForPrinting();
                                                        if (ResultCode == 0)
                                                        {

                                                            Utils.ToLog("Check is closed. ");
                                                        }
                                                        else if (ResultCode == 80) //Печать предыдущей команды  (такое бывает)
                                                        {
                                                            //ничего не делаем, т.к. выше есть WaitForPrinting();
                                                        }
                                                        else
                                                        {
                                                            Utils.ToLog(String.Format("Check is not closed. ResultCode: {0}, ({1}) ", ResultCode, ResultCodeDescription));
                                                        }
                                                    }
                                                    else
                                                    {
                                                        Utils.ToLog("Last check is closed. ");
                                                    }

                                                }
                                                else
                                                {
                                                    Utils.ToLog("Try CancelCheck. ");
                                                    CancelCheck();
                                                    WaitForPrinting();
                                                    if (ResultCode == 0)
                                                    {
                                                        Utils.ToLog("It's Close check. Try CancelCheck ok. Try RepeatBlock");
                                                        CommandQuere[0].RepeatBlock();
                                                    }
                                                    else if (ResultCode == 80)
                                                    {
                                                        
                                                    }
                                                }
                                            }
                                        }
                                    }


                                    if ((ResultCode != 0) && (!IgnoreErrors.Contains(ResultCode)))
                                    {
                                        if (!(ShowMessage(ResultCodeDescription)))
                                        {
                                            CommandQuere.Remove(CommandQuere[0]);
                                            MainClass.FiskalPrinterIsPrinting = false;
                                        }
                                        TimerSyncPoint = 0;
                                        continue;
                                    }
                                    else
                                    {
                                        try
                                        {
                                            if (CommandQuere[0].GetFirstBlock().Where(a => a.Name == "CloseCheck").Count() > 0)
                                            {
                                                Check chk = (Check)CommandQuere[0].GetFirstBlock().Where(a => a.Name == "CloseCheck").First().tag;
                                                if (!iniFile.FRModeDisabled)
                                                {
                                                    if (chk.OpenTimem == 0)
                                                    {
                                                        if (ReadTableInt(1, 1, 49) == 1)
                                                        {
                                                            chk.RealOpenTimem = 0;
                                                        }
                                                        else
                                                        {
                                                            if (ResultCode == 0)
                                                            {
                                                                //Андрюша раздолбай
                                                                //Надо ему об этом сообщить
                                                                EventSenderClass.SendAlohaAsincEvent(StopListService.AlohaEventType.AndrFoool, "", chk.Cassir, (Int32)SerialNumberInt, "", 0, chk.TableId, chk.AlohaCheckNum);
                                                            }
                                                            else
                                                            {
                                                                //А это не смог узнать значение таблицы
                                                                EventSenderClass.SendAlohaAsincEvent(StopListService.AlohaEventType.AndrFoool2, "", chk.Cassir, (Int32)SerialNumberInt, "", 0, chk.TableId, chk.AlohaCheckNum);
                                                                chk.RealOpenTimem = 0;
                                                            }
                                                        }
                                                    }
                                                    GetEKLZSerialNumber();
                                                    chk.EKLZNum = EKLZNumber;
                                                }
                                                chk.KkmNum = SerialNumberInt;
                                                chk.KkmShiftNumber = SessionNumber + 1;
                                                CloseCheck.WriteCheck(chk, 2); //Чек закрыт на ФР
                                                // chk.EKLZNum = 
                                            }

                                        }
                                        catch
                                        {
                                        }
                                        CommandRunning = true;

                                    }
                                }
                                catch (Exception ee)
                                {
                                    Utils.ToCardLog("[Error] при выполнении команды  фискальником " + ee.Message);
                                    CommandQuere.Remove(CommandQuere[0]);
                                    MainClass.FiskalPrinterIsPrinting = false;
                                    TimerSyncPoint = 0;
                                    //return;
                                    continue;
                                }
                            }

                            else
                            {
                                MainClass.FiskalPrinterIsPrinting = false;
                                Disconnect();
                            }
                            break;
                        case 1:
                            ShowMessage(ECRAdvancedModeDescription);
                            break;
                        case 2:
                            ShowMessage(ECRAdvancedModeDescription);
                            break;
                        case 3:
                            ContinuePrint();
                            break;
                        case 4:
                            //TimerSyncPoint = 0;
                            // QuereTimer.Start();
                            break;
                        case 5:
                            //TimerSyncPoint = 0;
                            //QuereTimer.Start();
                            break;
                        default:
                            break;
                    }

                    TimerSyncPoint = 0;
                }



                catch (Exception e)
                {
                    Utils.ToCardLog("Error CheckCommandQuere " + e.Message);
                }

            }



        }

        internal static bool NeedShowMsg = false;
        internal static string NeedShowMsgMess = "";

        private static void ShowMessage2(string sError)
        {
            NeedShowMsg = true;
            NeedShowMsgMess = sError;
        }
        internal static void ShowMessageExt()
        {
            ShowMessage(NeedShowMsgMess);
        }

        private static bool ShowMessage(string sError)
        {

            string Mess =
                "ResultCode: (" + ResultCode + ") " + ResultCodeDescription + " ECRMode: " + ECRMode + ": " + ECRModeDescription + " ECRAdvancedMode: " + ECRAdvancedModeDescription;
            /*
            MessageForm Mf = new MessageForm(Mess);
            Mf.TopMost = true;
            Utils.ToLog(Mess);
            //Mf.SetCpt(ResultCodeDescription);
            
            Mf.ShowDialog();
             * */

            Disconnect();
            ctrlCloseCheckErrorMess ctrlError = new ctrlCloseCheckErrorMess();
            ctrlError.SetTxt("Ошибка ФР. "+Environment.NewLine+Mess);

            PDSystem.AlertModalWindow ScaleWnd = PDSystem.ModalWindowsForegraund.GetModalWindow(ctrlError);
            ctrlError.SetOwnerWnd(ScaleWnd);
            ctrlError.Width = 800;
            ctrlError.Height = 400;
            ScaleWnd.ShowDialog();

            if (ctrlError.Result == 1)
            {
                Conn();
                //ContinuePrint();
                TimerSyncPoint = 0;
                // QuereTimer.Start();
             //   Mf.Close();
              //  Mf.Dispose();
                return true;
            }
            else if (ctrlError.Result == 2)
            {
                Conn();
                TimerSyncPoint = 0;
                //QuereTimer.Start();
              //  CommandQuere[0].ReprintBlock();
              //  Mf.Close();
              //  Mf.Dispose();
                return false ;
            }
                /*
            else if (Mf.Result == -1)
            {
                Mf.Close();
                Mf.Dispose();
                return false;
            }
            */
            return false;
        }


        private static void RunCommandCommandBlock(ShtrihCommandBlock Owner, List<ShtrihCommand> CBL, out List<int> IgnoreErrors)
        {
            IgnoreErrors = new List<int>();
            foreach (ShtrihCommand CB in CBL)
            {
                if (CB == null) continue;

                if (CB.Type == ShtrihCommandType.ErrorBehavor)
                {
                    if (CB.Value == null)
                    { continue; }
                    IgnoreErrors = (List<int>)CB.Value;

                    Utils.ToLog("IgnoreErrors ", 8);
                    foreach (int i in IgnoreErrors)
                    {
                        Utils.ToLog(i.ToString(), 8);
                    }
                    Utils.ToLog("End IgnoreErrors ", 8);
                }

                else if (CB.Type == ShtrihCommandType.Property)
                {
                    if (CB.Value == null)
                    {
                        CB.Value = "";
                    }
                    Utils.ToLog("Set  Prop " + CB.Name + " Val: " + CB.Value.ToString(), 8);
                    ShtrihType.InvokeMember(CB.Name, BindingFlags.SetProperty, null, mShtrih, new object[] { CB.Value });
                }
                else if (CB.Type == ShtrihCommandType.Void)
                {
                    Utils.ToLog("Run  Comm " + CB.Name, 8);
                    ShtrihType.InvokeMember(CB.Name, BindingFlags.InvokeMethod, null, mShtrih, null);
                    /*
                    if (ResultCode < 0)
                    {
                        Utils.ToLog("ResultCode: " + ResultCode.ToString() + " try get status", 8);
                        Thread.Sleep((1000));
                        GetShortECRStatus();
                    }
                     * */
                }
                else if (CB.Type == ShtrihCommandType.GetVal)
                {

                    Utils.ToLog("GetVal  " + CB.Name, 8);
                    if (ResultCode == 0)
                    {
                            Owner.OutVal = ShtrihType.InvokeMember(CB.Name, BindingFlags.GetProperty, null, mShtrih, null);
                        if (Owner.OutVal != null)
                        {
                            Utils.ToLog("GetVal  return " + Owner.OutVal, 8);
                        }
                    }
                    Owner.GetValComplited = true;

                }
            }
        }
        private static void RunCommand(string sCommand)
        {
            ShtrihType.InvokeMember(sCommand, BindingFlags.InvokeMethod, null, mShtrih, null);
            //Utils.ToLog("Выполнил команду " + sCommand);
        }
        private static void SetProp(CProp Prop)
        {
            object[] ob = new object[1];
            ob[0] = Prop.PropVal;
            ShtrihType.InvokeMember(Prop.PropName, BindingFlags.SetProperty, null, mShtrih, ob);
            //  Utils.ToLog("Установил свойство " + Prop.PropName + " = " + Prop.PropVal);
        }


        internal static List<string> GetCaption()
        {
            List<string> Tmp = new List<string>();


            int MaxCaptionField = 16;
            if (iniFile.ShtrihFRType == 2)
            {
                MaxCaptionField = 14;
            }
            for (int i = 12; i <= MaxCaptionField; i++)
            {
                Tmp.Add(ReadTable(4, 1, i));
            }

            return Tmp;
        }
        /*
        private static void AddProp(string Prop, object val, CCommandQuere Command)
        {
            CProp Cp;
            Cp.PropName = Prop;
            Cp.PropVal = val;
            //CurentProp.Add(Cp);
            Command.PropertysList.Add(Cp);
            Utils.ToLog("Добавил свойство " + Prop + " = " + val);
        }
        */

        internal static void RunBlock(ShtrihCommandBlock Block)
        {

            if (CommandQuere == null)
            {
                Utils.ToLog("Error RunBlock. Очередь фискального регистратора не создана.");
            }
            else
            {
                MainClass.FiskalPrinterIsPrinting = true;
                CommandQuere.Add(Block);
            }
        }
        internal static CCommandQuere CreateCommand(string Command)
        {
            return new CCommandQuere(Command);
        }
        private static void AddCommand(CCommandQuere cCommand, CCommandQuereBlock CommandBlock)
        {
            CommandBlock.CommandList.Add(cCommand);
        }

        internal static decimal GetCashReg(int RegNum)
        {
            int Count = 0;

            //Utils.ToLog("TimerSyncPoint " + TimerSyncPoint);
            while ((Interlocked.CompareExchange(ref TimerSyncPoint, 1, 0) != 0) && (Count < 5000))
            {
                Thread.CurrentThread.Join(50);
                // Utils.ToCardLog("GetCashReg lock" + Count);
                Count += 50;
            }
            Utils.ToLog("GetCashReg S");
            //if (sync == 1) { return; }


            CProp Cp;
            Cp.PropName = "RegisterNumber";
            Cp.PropVal = RegNum;
            //  QuereTimer.Stop();
            SetProp(Cp);
            RunCommand("GetCashReg");
            decimal res = ContentsOfCashRegister;
            //   QuereTimer.Start ();
            TimerSyncPoint = 0;
            return res;
        }

        internal static double GetOperReg(int RegNum)
        {

            int Count = 0;
            while ((Interlocked.CompareExchange(ref TimerSyncPoint, 1, 0) != 0) && (Count < 5000))
            {
                Thread.CurrentThread.Join(50);
                //    Utils.ToCardLog("GetCashReg lock" + Count);
                Count += 50;
            }
            CProp Cp;
            Cp.PropName = "RegisterNumber";
            Cp.PropVal = RegNum;

            SetProp(Cp);
            RunCommand("GetOperationReg");
            double res = (double)ContentsOfOperationRegister;
            TimerSyncPoint = 0;

            return res;

        }

        static internal int ReadTableInt(int TableNumber, int FieldNumber, int RowNumber)
        {
            int Count = 0;
            while ((Interlocked.CompareExchange(ref TimerSyncPoint, 1, 0) != 0) && (Count < 5000))
            {
                Thread.CurrentThread.Join(50);
                // Utils.ToCardLog("GetCashReg lock" + Count);
                Count += 50;
            }
            CProp Cp;
            Cp.PropName = "TableNumber";
            Cp.PropVal = TableNumber;
            SetProp(Cp);
            Cp.PropName = "FieldNumber";
            Cp.PropVal = FieldNumber;
            SetProp(Cp);
            Cp.PropName = "RowNumber";
            Cp.PropVal = RowNumber;
            SetProp(Cp);
            RunCommand("ReadTable");

            int res = ValueOfFieldInteger;
            TimerSyncPoint = 0;
            return res;
        }


        static internal string ReadTable(int TableNumber, int FieldNumber, int RowNumber)
        {

            ShtrihCommandBlock SCB = new ShtrihCommandBlock();
            SCB.ReadTableStr(TableNumber, RowNumber, FieldNumber, null);
            RushCommandQuere.Add(SCB);

            int Count = 0;
            while ((!SCB.GetValComplited) && (Count < 500))
            {
                Thread.CurrentThread.Join(50);
                Count += 50;
            }
            string Val = "";
            if (SCB.OutVal != null)
            {
             Val = (string)SCB.OutVal;
            }
            RushCommandQuere.Remove(SCB);
            return Val;
        }

        static internal bool ClosedSmena(out String status)
        {
            Password = 30;
            GetECRStatus();
            status = Shtrih2.ECRModeDescription;
            return (Shtrih2.ECRMode == 4);

        }
        static internal bool ClosedSmenaInternal(out String status)
        {
            Password = 30;
            GetECRStatus();
            status = Shtrih2.ECRModeDescription;
            bool res = Shtrih2.ECRMode == 4;
            Disconnect();
            return (res);

        }

        internal static void XReport()
        {

            int Count = 0;
            while ((Interlocked.CompareExchange(ref TimerSyncPoint, 1, 0) != 0) && (Count < 5000))
            {
                Thread.CurrentThread.Join(50);
                //    Utils.ToCardLog("GetCashReg lock" + Count);
                Count += 50;
            }
            PrintReportWithoutCleaning();

        }


        internal static ZReportData GetPreZReportData()
        {
            ZReportData Data = new ZReportData();
            int Count = 0;
            while ((Interlocked.CompareExchange(ref TimerSyncPoint, 1, 0) != 0) && (Count < 5000))
            {
                Thread.CurrentThread.Join(50);
                //    Utils.ToCardLog("GetCashReg lock" + Count);
                Count += 50;
            }
            GetECRStatus();
            Data.Kkmnum = SerialNumberInt;
            /*
            Data.CashIncome = GetCashReg(193) - GetCashReg(195) +
                GetCashReg(197) - GetCashReg(199) +
                GetCashReg(201) - GetCashReg(203) +
                GetCashReg(205) - GetCashReg(207);
             * */
            Data.CashIncomeCount = (int)(GetOperReg(144) + GetOperReg(146));
            Data.NewShiftNumber = SessionNumber + 2;

            foreach (KKMSpoolPayment P in Data.SpoolPayments)
            {
                P.PaymentSumm = GetCashReg(P.KKMPaymentId) - GetCashReg(P.KKMPaymentVoidId);
                P.PaymentName = ReadTable(5, 1, P.KKMPaymentNameId);
            }

            return Data;
        }
        
        internal static void ZReport()
        {

            int Count = 0;
            while ((Interlocked.CompareExchange(ref TimerSyncPoint, 1, 0) != 0) && (Count < 5000))
            {
                Thread.CurrentThread.Join(50);
                //    Utils.ToCardLog("GetCashReg lock" + Count);
                Count += 50;
            }
            PrintReportWithCleaning();
        }


        static private void PrintReportWithoutCleaning()
        {

            ShtrihType.InvokeMember("PrintReportWithoutCleaning", BindingFlags.InvokeMethod, null, mShtrih, null);
        }
        static private void PrintReportWithCleaning()
        {

            ShtrihType.InvokeMember("PrintReportWithCleaning", BindingFlags.InvokeMethod, null, mShtrih, null);
        }

        static private void GetECRStatus()
        {

            ShtrihType.InvokeMember("GetECRStatus", BindingFlags.InvokeMethod, null, mShtrih, null);
        }
        static private void GetShortECRStatus()
        {

            ShtrihType.InvokeMember("GetShortECRStatus", BindingFlags.InvokeMethod, null, mShtrih, null);
        }
        static private void ContinuePrint()
        {

            ShtrihType.InvokeMember("ContinuePrint", BindingFlags.InvokeMethod, null, mShtrih, null);
        }
        static private void FindDevice()
        {
            ShtrihType.InvokeMember("FindDevice", BindingFlags.InvokeMethod, null, mShtrih, null);
        }

        static private void GetEKLZSerialNumber()
        {
            ShtrihType.InvokeMember("GetEKLZSerialNumber", BindingFlags.InvokeMethod, null, mShtrih, null);
        }



        static public void GetExchangeParam()
        {

            ShtrihType.InvokeMember("GetExchangeParam", BindingFlags.InvokeMethod, null, mShtrih, null);
        }

        static public void SetExchangeParam()
        {

            ShtrihType.InvokeMember("SetExchangeParam", BindingFlags.InvokeMethod, null, mShtrih, null);
        }



        static internal void WaitForPrinting()
        {
            ShtrihType.InvokeMember("WaitForPrinting", BindingFlags.InvokeMethod, null, mShtrih, null);
        }

        static internal void CancelCheck()
        {
            ShtrihType.InvokeMember("CancelCheck", BindingFlags.InvokeMethod, null, mShtrih, null);
        }

        static internal void _CloseCheck()
        {
            ShtrihType.InvokeMember("CloseCheck", BindingFlags.InvokeMethod, null, mShtrih, null);
        }

        static private void Connect()
        {
            ShtrihType.InvokeMember("Connect", BindingFlags.InvokeMethod, null, mShtrih, null);
        }
        static private void Disconnect()
        {
            ShtrihType.InvokeMember("Disconnect", BindingFlags.InvokeMethod, null, mShtrih, null);
        }

        static private void CashIncome()
        {
            ShtrihType.InvokeMember("CashIncome", BindingFlags.InvokeMethod, null, mShtrih, null);
        }


        static internal Int32 SerialNumberInt
        {
            get
            {
                try
                {
                    return Convert.ToInt32(SerialNumber);
                }
                catch
                {
                    return 0;
                }
            }

        }



        static internal string EKLZNumber
        {
            get
            {
                return (string)ShtrihType.InvokeMember("EKLZNumber", BindingFlags.GetProperty, null, mShtrih, null);
            }
            set
            {
                ShtrihType.InvokeMember("EKLZNumber", BindingFlags.SetProperty, null, mShtrih, new object[] { value });
            }
        }

        static internal string SerialNumber
        {
            get
            {
                return (string)ShtrihType.InvokeMember("SerialNumber", BindingFlags.GetProperty, null, mShtrih, null);
            }
            set
            {
                ShtrihType.InvokeMember("SerialNumber", BindingFlags.SetProperty, null, mShtrih, new object[] { value });
            }
        }
        static internal int SessionNumber
        {
            get
            {
                return (int)ShtrihType.InvokeMember("SessionNumber", BindingFlags.GetProperty, null, mShtrih, null);
            }

            set
            {
                ShtrihType.InvokeMember("SessionNumber", BindingFlags.SetProperty, null, mShtrih, new object[] { value });
            }
        }


        static internal int Password
        {
            get
            {
                return (int)ShtrihType.InvokeMember("Password", BindingFlags.GetProperty, null, mShtrih, null);
            }
            set
            {
                ShtrihType.InvokeMember("Password", BindingFlags.SetProperty, null, mShtrih, new object[] { value });
            }
        }
        static private string ValueOfFieldString
        {
            get
            {
                return (string)ShtrihType.InvokeMember("ValueOfFieldString", BindingFlags.GetProperty, null, mShtrih, null);
            }

        }
        static private int ValueOfFieldInteger
        {
            get
            {
                return (int)ShtrihType.InvokeMember("ValueOfFieldInteger", BindingFlags.GetProperty, null, mShtrih, null);
            }

        }
        static private decimal ContentsOfCashRegister
        {
            get
            {
                return (decimal)ShtrihType.InvokeMember("ContentsOfCashRegister", BindingFlags.GetProperty, null, mShtrih, null);
            }
            set
            {
                ShtrihType.InvokeMember("ContentsOfCashRegister", BindingFlags.SetProperty, null, mShtrih, new object[] { value });
            }
        }

        static private int ContentsOfOperationRegister
        {
            get
            {
                return (int)ShtrihType.InvokeMember("ContentsOfOperationRegister", BindingFlags.GetProperty, null, mShtrih, null);
            }
            set
            {
                ShtrihType.InvokeMember("ContentsOfOperationRegister", BindingFlags.SetProperty, null, mShtrih, new object[] { value });
            }
        }

        static public int Timeout
        {
            get
            {
                return (int)ShtrihType.InvokeMember("Timeout", BindingFlags.GetProperty, null, mShtrih, null);
            }
            set
            {
                ShtrihType.InvokeMember("Timeout", BindingFlags.SetProperty, null, mShtrih, new object[] { value });
            }
        }
        static private int ResultCode
        {
            get
            {
                return (int)ShtrihType.InvokeMember("ResultCode", BindingFlags.GetProperty, null, mShtrih, null);
            }
            set
            {
                ShtrihType.InvokeMember("ResultCode", BindingFlags.SetProperty, null, mShtrih, new object[] { value });
            }
        }
        static private int BaudRate
        {
            get
            {
                return (int)ShtrihType.InvokeMember("BaudRate", BindingFlags.GetProperty, null, mShtrih, null);
            }
            set
            {
                ShtrihType.InvokeMember("BaudRate", BindingFlags.SetProperty, null, mShtrih, new object[] { value });
            }
        }

        static private int TCPPort
        {
            get
            {
                return (int)ShtrihType.InvokeMember("TCPPort", BindingFlags.GetProperty, null, mShtrih, null);
            }
            set
            {
                ShtrihType.InvokeMember("TCPPort", BindingFlags.SetProperty, null, mShtrih, new object[] { value });
            }
        }

        static private int TCPConnectionTimeout
        {
            get
            {
                return (int)ShtrihType.InvokeMember("TCPConnectionTimeout", BindingFlags.GetProperty, null, mShtrih, null);
            }
            set
            {
                ShtrihType.InvokeMember("TCPConnectionTimeout", BindingFlags.SetProperty, null, mShtrih, new object[] { value });
            }
        }

        static private int ConnectionType
        {
            get
            {
                return (int)ShtrihType.InvokeMember("ConnectionType", BindingFlags.GetProperty, null, mShtrih, null);
            }
            set
            {
                ShtrihType.InvokeMember("ConnectionType", BindingFlags.SetProperty, null, mShtrih, new object[] { value });
            }
        }


        static private int ComNumber
        {
            get
            {
                return (int)ShtrihType.InvokeMember("ComNumber", BindingFlags.GetProperty, null, mShtrih, null);
            }
            set
            {

                ShtrihType.InvokeMember("ComNumber", BindingFlags.SetProperty, null, mShtrih, new object[] { value });
            }
        }
        static private int ECRAdvancedMode
        {
            get
            {
                return (int)ShtrihType.InvokeMember("ECRAdvancedMode", BindingFlags.GetProperty, null, mShtrih, null);
            }

        }
        static private int ECRMode
        {
            get
            {
                return (int)ShtrihType.InvokeMember("ECRMode", BindingFlags.GetProperty, null, mShtrih, null);
            }

        }
        static private string ECRAdvancedModeDescription
        {
            get
            {
                return (string)ShtrihType.InvokeMember("ECRAdvancedModeDescription", BindingFlags.GetProperty, null, mShtrih, null);
            }

        }
        static private string ECRModeDescription
        {
            get
            {
                return (string)ShtrihType.InvokeMember("ECRModeDescription", BindingFlags.GetProperty, null, mShtrih, null);
            }

        }
        static private string ResultCodeDescription
        {
            get
            {
                return (string)ShtrihType.InvokeMember("ResultCodeDescription", BindingFlags.GetProperty, null, mShtrih, null);
            }
            set
            {
                ShtrihType.InvokeMember("ResultCodeDescription", BindingFlags.SetProperty, null, mShtrih, new object[] { value });
            }
        }


        static private string ComputerName
        {
            get
            {
                return (string)ShtrihType.InvokeMember("ComputerName", BindingFlags.GetProperty, null, mShtrih, null);
            }
            set
            {
                ShtrihType.InvokeMember("ComputerName", BindingFlags.SetProperty, null, mShtrih, new object[] { value });
            }
        }





    }

    public class ShtrihCommandBlock
    {
        Guid _id;
        public Guid Id
        {
            get
            {
                return _id;
            }
        }


        internal Check ChkOwner = null;


        internal bool GetValComplited = false;
        private object _OutVal = null;
        public object OutVal
        {
            get
            {
                return _OutVal;
            }
            set
            {
                _OutVal = value;
            }
        }

        public ShtrihCommandBlock()
        {
            _id = Guid.NewGuid();
        }

        private List<ShtrihCommand> CommanQuery = new List<ShtrihCommand>();
        private List<ShtrihCommand> PrintedCommanQuery = new List<ShtrihCommand>();

        internal void CommandBlockToQwery()
        {

            Shtrih2.RunBlock(this);
        }

        internal List<ShtrihCommand> GetLastBlock()
        {
            List<ShtrihCommand> Tmp = new List<ShtrihCommand>();
            Tmp.Add(CommanQuery.Last());

            return Tmp;
        }

        internal List<ShtrihCommand> GetFirstBlock()
        {
            List<ShtrihCommand> Tmp = new List<ShtrihCommand>();
            bool FindVoid = false;
            foreach (ShtrihCommand C in CommanQuery)
            {

                if (C.Type == ShtrihCommandType.GetVal)
                {
                    Tmp.Add(C);
                    return Tmp;
                }
                if (FindVoid)
                {
                    return Tmp;
                }
                Tmp.Add(C);
                if (C.Type == ShtrihCommandType.Void)
                {
                    FindVoid = true;
                    //return Tmp;
                }
            }
            return Tmp;
        }

        internal void RepeatBlock()
        {
            List<ShtrihCommand> Tmp = new List<ShtrihCommand>();
            foreach (ShtrihCommand C in CommanQuery)
            {
                Tmp.Add(C);
            }
            CommanQuery.Clear();
            foreach (ShtrihCommand C in PrintedCommanQuery)
            {
                CommanQuery.Add(C);
            }
            foreach (ShtrihCommand C in Tmp)
            {
                CommanQuery.Add(C);

            }
        }

        internal bool RemoveFirstBlock()
        {
            List<ShtrihCommand> Tmp = new List<ShtrihCommand>();
            Tmp = GetFirstBlock();
            foreach (ShtrihCommand C in Tmp)
            {
                PrintedCommanQuery.Add(C);
                CommanQuery.Remove(C);
            }
            if (CommanQuery.Count == 0)
            {
                return true;
            }
            return false;
        }

        private bool CaptionAdded = false;
        internal void ReprintBlock()
        {
            foreach (ShtrihCommand C in CommanQuery)
            {
                PrintedCommanQuery.Add(C);
            }
            CommanQuery.Clear();
            if (!CaptionAdded)
            {
                foreach (String s in Shtrih2.GetCaption())
                {
                    CommanQuery.Add(new ShtrihCommand(ShtrihCommandType.Property, "StringForPrinting", s));
                    CommanQuery.Add(new ShtrihCommand(ShtrihCommandType.Void, "PrintString", 0));
                }
                CaptionAdded = true;
            }
            foreach (ShtrihCommand C in PrintedCommanQuery)
            {
                CommanQuery.Add(C);
            }
            PrintedCommanQuery.Clear();

        }

        internal int StringQuantity
        {
            set
            {

                CommanQuery.Add(new ShtrihCommand(ShtrihCommandType.Property, "StringQuantity", value));

            }
        }





        internal void CutCheck()
        {
            if (!iniFile.NoCut)
            {
                int PrintStringCount = 0;
                foreach (ShtrihCommand Comm in CommanQuery)
                {
                    if ((Comm.Name == "FeedDocument") || (Comm.Name == "PrintWideString") || (Comm.Name == "PrintString") || (Comm.Name == "PrintString") || (Comm.Name == "Sale") || (Comm.Name == "ReturnSale"))
                    {
                        PrintStringCount++;

                    }
                    if ((Comm.Name == "CutCheck") || (Comm.Name == "FinishDocument") || (Comm.Name == "CloseCheck"))
                    {
                        PrintStringCount = 0;
                    }
                }
                if (PrintStringCount > 1)
                {
                    CommanQuery.Add(new ShtrihCommand(ShtrihCommandType.Void, "CutCheck", 0));
                }
                else
                {
                    Utils.ToCardLog("Не смог добавить команду отрезки чека. Размер блока команд: " + CommanQuery.Count);
                    Utils.ToCardLog("-------------------------------");
                    foreach (ShtrihCommand Sc in CommanQuery)
                    {
                        Utils.ToCardLog(Sc.Name);
                    }
                    Utils.ToCardLog("-------------------------------");
                }
            }
        }
        internal void FeedDocument()
        {
            CommanQuery.Add(new ShtrihCommand(ShtrihCommandType.Void, "FeedDocument", 0));
        }

        internal void FinishDocument()
        {
            CommanQuery.Add(new ShtrihCommand(ShtrihCommandType.Void, "FinishDocument", 0));
        }
        internal void OpenCheck(int ChType)
        {
            CommanQuery.Add(new ShtrihCommand(ShtrihCommandType.Property, "CheckType", ChType));
            CommanQuery.Add(new ShtrihCommand(ShtrihCommandType.Void, "OpenCheck", 0));
        }
        internal void PrintDocumentTitle(string DocumentName, int DocumentNumber)
        {
            CommanQuery.Add(new ShtrihCommand(ShtrihCommandType.Property, "DocumentName", DocumentName));
            CommanQuery.Add(new ShtrihCommand(ShtrihCommandType.Property, "DocumentNumber", DocumentNumber));
            CommanQuery.Add(new ShtrihCommand(ShtrihCommandType.Void, "PrintDocumentTitle", 0));
        }
        internal void PrintWideString(string str)
        {
            CommanQuery.Add(new ShtrihCommand(ShtrihCommandType.Property, "StringForPrinting", str));
            CommanQuery.Add(new ShtrihCommand(ShtrihCommandType.Void, "PrintWideString", 0));
        }
        internal void PrintString(string str)
        {
            CommanQuery.Add(new ShtrihCommand(ShtrihCommandType.Property, "StringForPrinting", str));
            CommanQuery.Add(new ShtrihCommand(ShtrihCommandType.Void, "PrintString", 0));
        }
        internal void PrintStringWithFont(string str, int FontType)
        {
            CommanQuery.Add(new ShtrihCommand(ShtrihCommandType.Property, "FontType", FontType));
            CommanQuery.Add(new ShtrihCommand(ShtrihCommandType.Property, "StringForPrinting", str));
            CommanQuery.Add(new ShtrihCommand(ShtrihCommandType.Void, "PrintString", 0));
        }

        internal void Sale(double Quantity, decimal Price, string StringForPrinting, int Tax1, int Department)
        {
            CommanQuery.Add(new ShtrihCommand(ShtrihCommandType.Property, "Quantity", Quantity));
            CommanQuery.Add(new ShtrihCommand(ShtrihCommandType.Property, "Price", Price));
            CommanQuery.Add(new ShtrihCommand(ShtrihCommandType.Property, "StringForPrinting", StringForPrinting));
            CommanQuery.Add(new ShtrihCommand(ShtrihCommandType.Property, "Tax1", Tax1));
            CommanQuery.Add(new ShtrihCommand(ShtrihCommandType.Property, "Department", Department));
            CommanQuery.Add(new ShtrihCommand(ShtrihCommandType.Void, "Sale", 0));
        }


        internal void PrintReportWithCleaning()
        {
            CommanQuery.Add(new ShtrihCommand(ShtrihCommandType.Void, "PrintReportWithCleaning", 0));
        }




        internal void CashOutCome(decimal Summ)
        {

            CommanQuery.Add(new ShtrihCommand(ShtrihCommandType.Property, "Summ1", Summ));
            CommanQuery.Add(new ShtrihCommand(ShtrihCommandType.Void, "CashOutCome", 0));

        }
        internal void CashIncome(decimal Summ)
        {

            CommanQuery.Add(new ShtrihCommand(ShtrihCommandType.Property, "Summ1", Summ));
            CommanQuery.Add(new ShtrihCommand(ShtrihCommandType.Void, "CashIncome", 0));

        }


        internal void ReturnSale(double Quantity, decimal Price, string StringForPrinting, int Tax1, int Department)
        {
            CommanQuery.Add(new ShtrihCommand(ShtrihCommandType.Property, "Quantity", Quantity));
            CommanQuery.Add(new ShtrihCommand(ShtrihCommandType.Property, "Price", Price));
            CommanQuery.Add(new ShtrihCommand(ShtrihCommandType.Property, "StringForPrinting", StringForPrinting));
            CommanQuery.Add(new ShtrihCommand(ShtrihCommandType.Property, "Tax1", Tax1));
            CommanQuery.Add(new ShtrihCommand(ShtrihCommandType.Property, "Department", Department));
            CommanQuery.Add(new ShtrihCommand(ShtrihCommandType.Void, "ReturnSale", 0));
        }
        internal void Discount(decimal Summ1, string StringForPrinting)
        {
            CommanQuery.Add(new ShtrihCommand(ShtrihCommandType.Property, "Summ1", Summ1));

            CommanQuery.Add(new ShtrihCommand(ShtrihCommandType.Property, "StringForPrinting", StringForPrinting));

            CommanQuery.Add(new ShtrihCommand(ShtrihCommandType.Void, "Discount", 0));
        }
        internal void CloseCheck(decimal Summ1, decimal Summ2, decimal Summ3, decimal Summ4, int Tax1, int Tax2, int Tax3, int Tax4, string StringForPrinting, Check chk)
        {
            CommanQuery.Add(new ShtrihCommand(ShtrihCommandType.Property, "Summ1", Summ1));
            CommanQuery.Add(new ShtrihCommand(ShtrihCommandType.Property, "Summ2", Summ2));
            CommanQuery.Add(new ShtrihCommand(ShtrihCommandType.Property, "Summ3", Summ3));
            CommanQuery.Add(new ShtrihCommand(ShtrihCommandType.Property, "Summ4", Summ4));

            CommanQuery.Add(new ShtrihCommand(ShtrihCommandType.Property, "StringForPrinting", StringForPrinting));
            CommanQuery.Add(new ShtrihCommand(ShtrihCommandType.Property, "Tax1", Tax1));
            CommanQuery.Add(new ShtrihCommand(ShtrihCommandType.Property, "Tax2", Tax2));
            CommanQuery.Add(new ShtrihCommand(ShtrihCommandType.Property, "Tax3", Tax3));
            CommanQuery.Add(new ShtrihCommand(ShtrihCommandType.Property, "Tax4", Tax4));


            CommanQuery.Add(new ShtrihCommand(ShtrihCommandType.Void, "CloseCheck", 0, chk));
            //CommanQuery.Add(new ShtrihCommand(ShtrihCommandType.Void, "CloseCheck", 0));
        }


        internal void SetDate(DateTime Dt)
        {
            List<int> IgnoreErrors = new List<int> { 115 }; //Не поддерживается в данном режиме
            if ((Dt.Hour == 23) && (Dt.Minute == 59) && (Dt.Second > 55))
            {
                Dt = new DateTime(
                    Dt.Year,
                    Dt.Month,
                    Dt.Day,
                    Dt.Hour,
                    Dt.Minute,
                    55); //Это чтобы дата не перескакивала;

            }
            CommanQuery.Add(new ShtrihCommand(ShtrihCommandType.ErrorBehavor, "", IgnoreErrors));
            CommanQuery.Add(new ShtrihCommand(ShtrihCommandType.Property, "Date", Dt));
            CommanQuery.Add(new ShtrihCommand(ShtrihCommandType.Property, "Time", Dt));
            CommanQuery.Add(new ShtrihCommand(ShtrihCommandType.Void, "SetDate", 0));
            CommanQuery.Add(new ShtrihCommand(ShtrihCommandType.ErrorBehavor, "", IgnoreErrors));
            CommanQuery.Add(new ShtrihCommand(ShtrihCommandType.Void, "ConfirmDate", 0));
            CommanQuery.Add(new ShtrihCommand(ShtrihCommandType.ErrorBehavor, "", IgnoreErrors));
            CommanQuery.Add(new ShtrihCommand(ShtrihCommandType.Void, "SetTime", 0));

        }


        internal void WriteTableInt(int TableNumber, int RowNumber, int FieldNumber, int ValueOfFieldInteger, List<int> IgnoreErrors)
        {
            CommanQuery.Add(new ShtrihCommand(ShtrihCommandType.ErrorBehavor, "", IgnoreErrors));
            CommanQuery.Add(new ShtrihCommand(ShtrihCommandType.Property, "TableNumber", TableNumber));
            CommanQuery.Add(new ShtrihCommand(ShtrihCommandType.Property, "RowNumber", RowNumber));
            CommanQuery.Add(new ShtrihCommand(ShtrihCommandType.Property, "FieldNumber", FieldNumber));
            CommanQuery.Add(new ShtrihCommand(ShtrihCommandType.Property, "ValueOfFieldInteger", ValueOfFieldInteger));
            CommanQuery.Add(new ShtrihCommand(ShtrihCommandType.Void, "GetFieldStruct", 0));
            CommanQuery.Add(new ShtrihCommand(ShtrihCommandType.ErrorBehavor, "", IgnoreErrors));
            CommanQuery.Add(new ShtrihCommand(ShtrihCommandType.Void, "WriteTable", 0));
        }
        internal void ReadTableInt(int TableNumber, int RowNumber, int FieldNumber, List<int> IgnoreErrors)
        {
            CommanQuery.Add(new ShtrihCommand(ShtrihCommandType.ErrorBehavor, "", IgnoreErrors));
            CommanQuery.Add(new ShtrihCommand(ShtrihCommandType.Property, "TableNumber", TableNumber));
            CommanQuery.Add(new ShtrihCommand(ShtrihCommandType.Property, "RowNumber", RowNumber));
            CommanQuery.Add(new ShtrihCommand(ShtrihCommandType.Property, "FieldNumber", FieldNumber));
            CommanQuery.Add(new ShtrihCommand(ShtrihCommandType.Void, "GetFieldStruct", 0));
            CommanQuery.Add(new ShtrihCommand(ShtrihCommandType.ErrorBehavor, "", IgnoreErrors));
            CommanQuery.Add(new ShtrihCommand(ShtrihCommandType.Void, "ReadTable", 0));
            CommanQuery.Add(new ShtrihCommand(ShtrihCommandType.GetVal, "ValueOfFieldInteger", 0));
        }
        internal void GetFieldStruct(int TableNumber, int RowNumber, int FieldNumber, List<int> IgnoreErrors)
        {
            CommanQuery.Add(new ShtrihCommand(ShtrihCommandType.ErrorBehavor, "", IgnoreErrors));
            CommanQuery.Add(new ShtrihCommand(ShtrihCommandType.Property, "TableNumber", TableNumber));
            CommanQuery.Add(new ShtrihCommand(ShtrihCommandType.Property, "RowNumber", RowNumber));
            CommanQuery.Add(new ShtrihCommand(ShtrihCommandType.Property, "FieldNumber", FieldNumber));
            CommanQuery.Add(new ShtrihCommand(ShtrihCommandType.Void, "GetFieldStruct", 0));
            
        }


        internal void WriteTableStr(int TableNumber, int RowNumber, int FieldNumber, string ValueOfFieldString, List<int> IgnoreErrors)
        {
            CommanQuery.Add(new ShtrihCommand(ShtrihCommandType.ErrorBehavor, "", IgnoreErrors));
            CommanQuery.Add(new ShtrihCommand(ShtrihCommandType.Property, "TableNumber", TableNumber));
            CommanQuery.Add(new ShtrihCommand(ShtrihCommandType.Property, "RowNumber", RowNumber));
            CommanQuery.Add(new ShtrihCommand(ShtrihCommandType.Property, "FieldNumber", FieldNumber));
            CommanQuery.Add(new ShtrihCommand(ShtrihCommandType.Property, "ValueOfFieldString", ValueOfFieldString));
            CommanQuery.Add(new ShtrihCommand(ShtrihCommandType.Void, "GetFieldStruct", 0));
            CommanQuery.Add(new ShtrihCommand(ShtrihCommandType.ErrorBehavor, "", IgnoreErrors));
            CommanQuery.Add(new ShtrihCommand(ShtrihCommandType.Void, "WriteTable", 0));
        }

        internal void ReadTableStr(int TableNumber, int RowNumber, int FieldNumber, List<int> IgnoreErrors)
        {
            CommanQuery.Add(new ShtrihCommand(ShtrihCommandType.ErrorBehavor, "", IgnoreErrors));
            CommanQuery.Add(new ShtrihCommand(ShtrihCommandType.Property, "TableNumber", TableNumber));
            CommanQuery.Add(new ShtrihCommand(ShtrihCommandType.Property, "RowNumber", RowNumber));
            CommanQuery.Add(new ShtrihCommand(ShtrihCommandType.Property, "FieldNumber", FieldNumber));
            CommanQuery.Add(new ShtrihCommand(ShtrihCommandType.Void, "ReadTable", 0));
            CommanQuery.Add(new ShtrihCommand(ShtrihCommandType.GetVal, "ValueOfFieldString", 0));
        }


    }
    public class ShtrihCommand
    {
        public ShtrihCommandType Type;
        public string Name;
        public object Value;
        public object tag;
        public ShtrihCommand(ShtrihCommandType mType, string mName, object mValue)
        {
            Type = mType;
            Name = mName;
            Value = mValue;
        }

        public ShtrihCommand(ShtrihCommandType mType, string mName, object mValue, object mtag)
        {
            Type = mType;
            Name = mName;
            Value = mValue;
            tag = mtag;
        }

    }
    public enum ShtrihCommandType
    {
        Property, Void, ErrorBehavor, GetVal
    }
    public class ZReportData
    {
        public ZReportData()
        {
            SpoolPayments = new List<KKMSpoolPayment> ();
            SpoolPayments.Add(new KKMSpoolPayment() { SpoolPaymentId = 100, KKMPaymentId = 193, KKMPaymentVoidId = 195,KKMPaymentNameId=1 }) ;
            SpoolPayments.Add(new KKMSpoolPayment() { SpoolPaymentId = 102, KKMPaymentId = 197, KKMPaymentVoidId = 199, KKMPaymentNameId = 2 });
            SpoolPayments.Add(new KKMSpoolPayment() { SpoolPaymentId = 103, KKMPaymentId = 201, KKMPaymentVoidId = 203, KKMPaymentNameId = 3 });
            SpoolPayments.Add(new KKMSpoolPayment() { SpoolPaymentId = 104, KKMPaymentId = 205, KKMPaymentVoidId = 207, KKMPaymentNameId = 4 });
        }

        public decimal CashIncome {
            get
            {
                return SpoolPayments.Sum(a => a.TotalSumm);
            }
        }
        public int CashIncomeCount { set; get; }
        
        public int Kkmnum { set; get; }
        public DateTime DtZRep { set; get; }
        public int NewShiftNumber { set; get; }
        public List<KKMSpoolPayment> SpoolPayments { set; get; }
    }
    public class KKMSpoolPayment
    {
        public int SpoolPaymentId { set; get; }
        public int KKMPaymentId { set; get; }
        public int KKMPaymentVoidId { set; get; }
        public int KKMPaymentNameId { set; get; }
        public decimal PaymentVoidSumm { set; get; }
        public decimal PaymentSumm { set; get; }
        public decimal TotalSumm {
            get
            {
                return PaymentSumm - PaymentVoidSumm;
            }
        }

        public String PaymentName { set; get; }
    }

}
