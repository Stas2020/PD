using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
namespace PDiscountCard
{
    static public class CreditCardAlohaIntegration
    {
        static ICreditCardConnector CreditCardConnector;
        static frmLockScreeen LockScreeen = null;


        static public bool CreditCardConnectorEnabled
        {
            get
            {
                if (CreditCardConnector != null)
                {
                    return CreditCardConnector.Inited;
                }
                else
                {
                    return false;
                }

            }
        }

        static void HideLockScreen()
        {

            if (LockScreeen != null)
            {
                Utils.ToCardLog(String.Format("HideLockScreen"));
                try
                {


                    try
                    {

                        LockScreeen.mClose();
                    }
                    catch (Exception e)
                    {
                        Utils.ToCardLog(String.Format("Error HideLockScreen " + e.Message));
                    }


                    /*
                    LockScreeen.Dispatcher.BeginInvoke(((Action)(() =>
                    {
                        try
                        {
                            Utils.ToCardLog(String.Format("LockScreeen.Dispatcher.BeginInvoke"));
                            LockScreeen.Close();
                        }
                        catch (Exception e)
                        {
                            Utils.ToCardLog(String.Format("Error LockScreeen.Dispatcher.BeginInvoke " +e.Message));
                        }
                    })));
                     * */
                }
                catch (Exception)
                {

                }
                LockScreeen = null;
            }
        }

        static void ShowLockScreen()
        {
            Utils.ToCardLog(String.Format("ShowLockScreen"));
            HideLockScreen();
            try
            {

                LockScreeen = new frmLockScreeen();
                LockScreeen.Show();
            }
            catch (Exception e)
            {
                Utils.ToCardLog(String.Format("[Error] ShowLockScreen " + e.Message));
            }
        }

        public static bool Init(CreditCardTerminalType TerminalType, out string Ex)
        {
            Ex = "";
            try
            {
                string OutStr = "";
                switch (TerminalType)
                {
                    case CreditCardTerminalType.Arcus2:
                        CreditCardConnector = new Arcus2CreditCardConnector();
                        
                        break;
                    case CreditCardTerminalType.Arcus4:
                        
                        CreditCardConnector = new CreditCardIntegration.Arcus4CreditCardConnector();
                        break;
                    case CreditCardTerminalType.Inpas:
                        CreditCardConnector = new CreditCardIntegration.InpasCreditCardConnector();
                        break;
                    case CreditCardTerminalType.Sber:
                        CreditCardConnector = new SBRFCreditCardConnector();
                        break;
                    case CreditCardTerminalType.Emulator:
                        CreditCardConnector = new EmulatorCreditCardTerminal();

                        break;
                    default:
                        break;
                }


                return CreditCardConnector.Inited;
                //Arcus2CreditCardConnector.RunOperationAsincComplited += new Arcus2CreditCardConnector.RunOperationAsincDelegate(ArcusClass_RunOperationAsincComplited);
            }
            catch (Exception e)
            {
                CreditCardConnector = null;
                Ex = e.Message;
                return false;
            }
        }


        public static void ShowFuncsfrm()
        {
            if (CreditCardConnectorEnabled)
            {
                frmCreditCardFuncs mfrmArcus2 = new frmCreditCardFuncs();
                mfrmArcus2.ShowDialog();
            }
            else
            {
                ShowMsg("Нет подключенных модулей оплаты");
            }
        }




        static InpasChk currentCheck;
        static public void CreditCardOperationComplited(CreditCardOperationType OperType, bool ShowError, bool res, string resStr, string Receipt, string bins="", string RRN="")
        {
            Utils.ToCardLog(String.Format("CreditCardOperationComplited Receipt: {0}, res: {1},  resStr: {2}", Receipt, res, resStr));
            try
            {

                if (ShowError)
                {
                    HideLockScreen();
                    ShowMsg(resStr);
                }

                if (res && OperType == CreditCardOperationType.Payment)//Добавление бинов и RRN в атрибуты чека
                {
                    AlohaTSClass.SetPaymentOperAttr(currentCheck.AlohaId, bins, RRN);

                }


                        if ((!String.IsNullOrWhiteSpace(Receipt)) && (Receipt.Trim() != ""))
                {
                    if ((iniFile.Arcus2PrintSlip) || ((iniFile.PlastikPrintSlip)))
                    {
                 
                        PrintSlip(Receipt);
                    }
                }

                if (OperType == CreditCardOperationType.Payment || OperType == CreditCardOperationType.VoidPayment)
                {
                    if (res)
                    {
                        if (currentCheck.AllInTable)
                        {
                            foreach (InpasChk Chk in currentCheck.AllChks)
                            {
                                if ((iniFile.Arcus2PlasticLocalClose) || ((iniFile.PlasticLocalClose)))
                                {
                                    RemoteCloseCheck.CloseAlohaTableLocalCurentUser(Chk.AlohaId, 20, (decimal)Chk.Ammount);

                                }
                                else
                                {
                                    RemoteCloseCheck.AddRemoteChkToQuere(Chk.AlohaId, 20, Chk.CurrentEmpl, (decimal)Chk.Ammount / 100);
                                }
                            }
                            if ((iniFile.Arcus2PlasticLocalClose) || ((iniFile.PlasticLocalClose)))
                            {
                                AlohaTSClass.OpenEmptyCheck();
                            }

                        }
                        else
                        {
                            if ((iniFile.Arcus2PlasticLocalClose) || ((iniFile.PlasticLocalClose)))
                            {
                                RemoteCloseCheck.CloseAlohaTableLocalCurentUser(currentCheck.AlohaId, 20, Sum);
                                if (iniFile.CreditCardAutoOpenCheck)
                                {
                                    AlohaTSClass.OpenEmptyCheck();
                                }
                            }
                            else
                            {
                                RemoteCloseCheck.AddRemoteChkToQuere(currentCheck.AlohaId, 20, currentCheck.CurrentEmpl, Sum / 100);
                            }
                        }
                    }
                }
                HideLockScreen();
            }
            catch(Exception e)
            {
                Utils.ToCardLog("Error CreditCardOperationComplited " + e.Message);
            }
        }




        static private void PrintSlip(string Slip)
        {

            if (iniFile.CreditCardSlipPrintPreCheck)
            {
                Utils.ToLog("PrintSlip CreditCardSlipPrintPreCheck ");
                try
                {
                    // string[] stringSeparators = new string[] { "\n\r", "\n\n", Environment.NewLine};

                    string[] stringSeparators = new string[] { "\n" };
                    string sres = Slip.Replace("\r", "");
                    Convert.ToChar(31);
                    List<string> LSlip = sres.Split(stringSeparators, StringSplitOptions.None).ToList();
                    List<string> LSlip1 = new List<string>();
                    foreach (string str in LSlip)
                    {
                        if (str.Contains(Convert.ToChar(31)) && str.Length<3)
                        {
                            Utils.ToLog("PrintSlip Split and send");
                            if (LSlip1.Count() > 0)
                            {
                                AlohaTSClass.PrintCardSlip(LSlip1);
                                LSlip1.Clear();
                            }
                        }
                        else
                        {
                        LSlip1.Add(str);
                        }
                    }
                    if (LSlip1.Count() > 0)
                    {
                        AlohaTSClass.PrintCardSlip(LSlip1);
                        LSlip1.Clear();
                    }


                    //AlohaTSClass.PrintCardSlip(sres.Split(stringSeparators, StringSplitOptions.None).ToList());
                }
                catch (Exception e)
                {
                    Utils.ToCardLog("Ошибка печати слипа " + e.Message);
                }
            }
            else
            {
                Slip += Convert.ToChar(31);
                ToShtrih.PrintCardCheck(Slip);
            }
        }

        internal static void ShowMsg(string Mess)
        {
            Utils.ToCardLog("Показал сообщение " + Mess);
            frmAllertMessage Mf = new frmAllertMessage(Mess);
            Mf.ShowDialog();
        }
        private static decimal Sum = 0;

        internal static bool RunOper()
        {
            Check Chk = AlohaTSClass.GetCheckById((int)AlohaTSClass.AlohaCurentState.CheckId);
            return RunOper(Chk);
        }
        internal static bool RunOper(Check Chk)
        {
            if (CreditCardConnector.OperInProcess)
            {
                ShowMsg("На терминале пластиковых карт выполняется операция. Если вы уверены, что это не так, перезагрузите алоху.");
                return false;
            }
            if ((iniFile.Arcus2PlasticLocalClose) || ((iniFile.PlasticLocalClose)))
            {
                ShowLockScreen();
            }

            AlohaTSClass.CheckWindow();


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
                    //Utils.ToLog("Отправляю на оплату все чеки на столе." + GetFCCCheckfromAloha(Chk, AllCheck).Ammount);
                    AllCheck = true;
                }
            }
            InpasChk inpasChk = new InpasChk(Chk, AllCheck, AlohaTSClass.AlohaCurentState.EmployeeNumberCode);
            currentCheck = inpasChk;
            string inStr = "";

            Sum = inpasChk.Ammount;




            Utils.ToCardLog("Кредитный терминал транзакция старт Sum" + Sum);
            if (!inpasChk.isVoid)
            {
                CreditCardConnector.RunPaymentAsinc(Sum,Chk);
            }
            else
            {
                CreditCardConnector.RunVozvrAsinc(Sum);
            }
            if ((!iniFile.Arcus2PlasticLocalClose) && ((!iniFile.PlasticLocalClose)))
            {
                AlohaTSClass.LogOut(AlohaTSClass.GetTermNum());
                AlohaTSClass.RefreshCheckDisplay();
            }
            return true;
        }


        internal static void TestPinPad()
        {
            CreditCardConnector.TestPinPad();
        }

        internal static void PrintShortReport()
        {
            CreditCardConnector.RunXRepAsinc();

        }


        public static void ShowCassirMnu()
        {
            CreditCardConnector.RunCassirMenuAsinc();
        }



        public static void GetSipCopy()
        {
            if (CreditCardConnector is ICreditCardConnector2)
            {
                ((ICreditCardConnector2)CreditCardConnector).RunGetSlipCopyAsinc();
            }
            else
            {
                CopySlipFrm CSF = new CopySlipFrm();
                CSF.ShowDialog();
                Arcus2DataFromXML.GetSipCopy(CSF.SlipNumber);
            }
        }
        public static void GetLastSlipCopy()
        {
            CreditCardConnector.RunLastChkAsinc();
        }
        internal static void PrintLongReport()
        {
            CreditCardConnector.RunDetaleRepAsinc();

        }
        internal static void RunSverka()
        {
            string mReciept = "";
            string mRes = "";
            CreditCardConnector.RunSVERKARepSinc(out mReciept, out mRes);
            Utils.ToCardLog("RunSverka Complited mReciept " + mReciept + "  mRes " + mRes);
            PrintSlip(mReciept);
        }
    }
    public enum CreditCardTerminalType
    {
        Arcus2,
        Inpas,
        Sber,
        Emulator,
        Arcus4,
    }
    public enum CreditCardOperationType
    {
        Payment,
        VoidPayment,
        Sverka,
        XReport,
        LongReport,
        LastChk,
        TestPinPad,
        CassirMenu
    }

}
