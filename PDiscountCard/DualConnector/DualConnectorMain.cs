using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DualConnector;

namespace PDiscountCard.DualConnector
{

    public static class DualConnectorMain
    {
        public delegate void OnExchangeComplitedFromApiEventHendler(int Code, string CodeDescr, ISAPacket response);
        public delegate void OnExchangeComplitedEventHendler(object sender, int Result, string ResultDescr, string Reciept);
        //public event OnExchangeComplitedEventHendler OnExchangeComplited;
        static Dictionary<string, DualConnectorTransaction> Terminals = new Dictionary<string, DualConnectorTransaction>();
        //DualConnectorApi DApi = new DualConnectorApi ();
        public static void Init()
        {
            TermNum = iniFile.InPasTerminalNum;
            Terminals.Add(TermNum, new DualConnectorTransaction(TermNum));
        }

        public static void InPasFuncsfrm()
        {
            frmInpassCard mfrmInpass = new frmInpassCard();
            mfrmInpass.ShowDialog();

        }

      


        private static void ShowMsg(string Mess)
        {
            frmAllertMessage Mf = new frmAllertMessage(Mess);
            Mf.ShowDialog();
        }



        static string TermNum = "40000112";
        public static void Sale()
        {
            AlohaTSClass.CheckWindow();
            Check Chk = AlohaTSClass.GetCheckById((int)AlohaTSClass.AlohaCurentState.CheckId);


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
        
            if (AllCheck)
            {
                Utils.ToLog("Отправляю на пластиковую оплату все чеки на столе." + inpasChk.Ammount);
            }

            AlohaTSClass.LogOut(AlohaTSClass.GetTermNum());
            AlohaTSClass.RefreshCheckDisplay();


            if (!inpasChk.isVoid)
            {
                Utils.ToCardLog("Старт пластиковой оплаты Чек " + inpasChk.Num);
                RunOper(1, 0, 0, TermNum, inpasChk, 0);
            }
            else
            {
                //RunOper(1, 0, 0, TermNum, AlohaTSClass.GetCheckById((int)AlohaTSClass.AlohaCurentState.CheckId), 0);
                frmInpasVoid fv = new frmInpasVoid();
                fv.ShowDialog();
                if (fv.Res == 1)
                {
                    Utils.ToCardLog("Старт возврата пластиковой оплаты Чек " + Chk.AlohaCheckNum);
                    DualConnector.DualConnectorMain.Void(inpasChk, fv.RRN);

                }
                else if (fv.Res == 2)
                {
                    Utils.ToCardLog("Старт отмены пластиковой оплаты Чек " + Chk.AlohaCheckNum);
                    DualConnector.DualConnectorMain.Cancel(inpasChk, fv.RRN);
                }
            }
        }
        /*
        private static void Sale(int terminal)
        {
            RunOper(1, 0, 0, terminal, inpasChk, 0);
        }
        */
        public static void TestSale(string terminal, int Summ)
        {
            RunOper(1, 0, 0, terminal, null, Summ);
        }

        public static void TestCancel(string terminal, int Summ)
        {
            RunOper(4, 0, 22, TermNum, null, 270, "85666667775");
        }
        public static void ShortReport()
        {
            RunOper(63, 0, 20, TermNum, null, 0);
        }
        private static void ShortReport(string terminal)
        {
            RunOper(63, 0, 20, terminal, null, 0);
        }
        public static void LongReport()
        {
            RunOper(63, 0, 21, TermNum, null, 0);
        }
        public static void LongReport(string terminal)
        {
            RunOper(63, 0, 21, terminal, null, 0);
        }

        public static void Sverka()
        {
            RunOper(59, 0, 0, TermNum, null, 0);
        }

        public static void GetCopyLastSlip()
        {
            RunOper(63, 1, 22, TermNum, null, 0);
        }
        public static void GetCopySlip()
        {
            RunOper(63, 0, 22, TermNum, null, 0);
        }

        public static void Void(InpasChk Chk, string RRN)
        {
            RunOper(29, 0, 22, TermNum, Chk, 0, RRN);
        }
        public static void Cancel(InpasChk Chk, string RRN)
        {
            RunOper(4, 0, 22, TermNum, Chk, 0, RRN);
        }


        public static void CheckLink()
        {
            RunOper(26, 0, 0, TermNum, null, 0);
        }

        private static int RunOper(int OperId, int CommandMode, int CommandMode2, string terminal, InpasChk Chk, int Summ)
        {
            return RunOper(OperId, CommandMode, CommandMode2, terminal, Chk, Summ, "");
        }

        private static int RunOper(int OperId, int CommandMode, int CommandMode2, string terminal, InpasChk Chk, int Summ, string RRN)
        {
            DualConnectorTransaction Dt = new DualConnectorTransaction("0");
            if (Terminals.TryGetValue(terminal, out Dt))
            {
                if (Dt.TransactionInProcess)
                {
                    ShowMsg("На терминале выполняется транзакция");
                    return -1;
                }
                else
                {
                    //AlohaTSClass.CheckWindow();
                    Dt.RunOper(OperId, CommandMode, CommandMode2, Summ, Chk, RRN);
                }
            }
            else
            {
                ShowMsg("нет такого терминала");
                return -2;
            }
            return 0;
        }

      

        public static void TransactionComplited(int TransCode, int Status, InpasChk  PayMentCheck, int Code, string CodeDescrstring, string Reciept)
        {





            if (Reciept != null)
            {
                if (Reciept != "")
                {
                    //System.Windows.Forms.MessageBox.Show(Reciept);

                    //  AlohaTSClass.PrintCardSlip
                    try
                    {
                        // string[] stringSeparators = new string[] { "\n\r", "\n\n", Environment.NewLine};

                        string[] stringSeparators = new string[] { "\n" };

                        string sres = Reciept.Replace("\r", "");

                        AlohaTSClass.PrintCardSlip(sres.Split(stringSeparators, StringSplitOptions.None).ToList());
                    }
                    catch (Exception e)
                    {
                        Utils.ToCardLog("Ошибка печати слипа " + e.Message);
                    }
                }
            }
            if (Code == 0)
            {
                if (Status == 1)
                {
                    if (TransCode == 1)
                    {


                        if (PayMentCheck.AllInTable)
                        {
                            foreach (InpasChk Chk in PayMentCheck.AllChks)
                            {
                                RemoteCloseCheck.AddRemoteChkToQuere(Chk.AlohaId, 20, Chk.CurrentEmpl,0);
                            }
                        }
                        else
                        {
                            RemoteCloseCheck.AddRemoteChkToQuere(PayMentCheck.AlohaId, 20, PayMentCheck.CurrentEmpl,0);
                        }
                    }
                }
            }
        }


     

        /*
        private void ExchangeComplited(int Code, string CodeDescr, ISAPacket response)
        {
            if (OnExchangeComplited != null)
            {
                OnExchangeComplited(this, Code, CodeDescr, response.ReceiptData);
            }
        
        }
         * */
    }

    public class InpasChk
    {
        public int Ammount { set; get; }
        public int TableId { set; get; }
        public bool isVoid { set; get; }
        public bool AllInTable { set; get; }
        public string  Num { set; get; }
        public int AlohaId { set; get; }
        public int CurrentEmpl { set; get; }

        public List<InpasChk> AllChks = new List<InpasChk>();


        public InpasChk(Check Chk, bool mAllInTable, int Empl)
        {
            CurrentEmpl = Empl;
            if (mAllInTable)
            {
                isVoid = false;
                AllInTable = true;
                Ammount = (int)((Chk.ChecksOnTable.Sum(a => a.Summ) * 100) - (Chk.ChecksOnTable.Sum(a => a.Oplata) * 100));
                TableId = Chk.TableId;
                foreach (Check Chk2 in Chk.ChecksOnTable)
                {
                    AllChks.Add(new InpasChk(Chk2, false, Empl));
                    
                }
            }
            else
            {
                AllInTable = false;
                Ammount = (int)((Chk.Summ * 100)-(Chk.Oplata * 100));
                TableId = Chk.TableId;
                isVoid = Chk.Vozvr;
                AlohaId =Chk.AlohaCheckNum;
                Num = Chk.CheckShortNum;



            }
        }
    }

    public class DualConnectorTransaction
    {
        InpasChk currentCheck;

        public string terminalId = "";

        public DualConnectorTransaction(string Terminal)
        {
            terminalId = Terminal;
        }

        DualConnectorApi DApi = new DualConnectorApi();
        bool transactionInProcess = false;
        public bool TransactionInProcess
        {
            set
            {
                transactionInProcess = value;
                if (value)
                {
                    StartTransactionTime = DateTime.Now;
                }
            }
            get
            {

                if (transactionInProcess)
                {
                    if ((DateTime.Now - StartTransactionTime).TotalMinutes > 2)
                    {
                        transactionInProcess = false;
                    }
                }
                return transactionInProcess;
            }
        }
        private DateTime StartTransactionTime = DateTime.Now;
        public int RunOper(int OperId, int CommandMode, int CommandMode2, int Amount, InpasChk CurrentCheck, string RRN)
        {

            TransactionInProcess = true;
            if (CurrentCheck != null)
            {
                currentCheck = CurrentCheck;
                Amount = Math.Abs((int)(currentCheck.Ammount));
            }
            string resStr = "";
            int res = DApi.Exchange(OperId, CommandMode, CommandMode2, Amount, terminalId, RRN, ExchangeComplited, out resStr);
            if (res != 0)
            {
                TransactionInProcess = false;
                string Mess = "Ошибка старта операции" + OperId + " на терминале пластиковых карт." + Environment.NewLine + resStr;
                Mess += "Код ошибки: " + res;
                Mess += "Описание ошибки: " + resStr;
                Utils.ToCardLog(Mess);
                frmAllertMessage Mf = new frmAllertMessage(Mess);
                Mf.ShowDialog();
            }
            return res;
        }



        private void ExchangeComplited(int Code, string CodeDescr, ISAPacket response)
        {
            /*
            if (OnExchangeComplited != null)
            {
                OnExchangeComplited(this, Code, CodeDescr, response.ReceiptData);
            }
             * */
            string PrintSlipData = "";

            Utils.ToCardLog(string.Format("ExchangeComplited Code={0} ",Code));
            if (Code != 0)
            {
                

                string Mess = "";   // "Ошибка проведения операции на терминале пластиковых карт." + Environment.NewLine + response.TextResponse;
                Mess += " Ошибка проведения операции" + Environment.NewLine + " на терминале пластиковых карт." + Environment.NewLine;
                Mess += Environment.NewLine;
                Mess += "Терминал: " + terminalId + Environment.NewLine;
                Mess += Environment.NewLine;
                Mess += " Код ошибки: " + Code + Environment.NewLine;
                Mess += " Описание ошибки: " + Environment.NewLine + CodeDescr + Environment.NewLine;
                Mess += Environment.NewLine;
                if (currentCheck != null)
                {
                    foreach (InpasChk Chk2 in currentCheck.AllChks)
                    {
                        Mess += " Чек в Алохе " + Chk2.Num + Environment.NewLine;
                    }
                    Mess += " Сумма " + (double)currentCheck.Ammount/100D;
                }
                Utils.ToCardLog(Mess);
                frmAllertMessage Mf = new frmAllertMessage(Mess);
                Mf.ShowDialog();
                /*
                PrintSlipData = "Ошибка проведения операции" + Environment.NewLine + " на терминале пластиковых карт.";
                PrintSlipData += "Код ошибки: " + response.Status;
                PrintSlipData += "Описание ошибки: " + response.Status;
                 
                if (currentCheck != null)
                {
                    PrintSlipData += "Чек в Алохе " + currentCheck.AlohaCheckNum;
                }
                 * */
                PrintSlipData += Environment.NewLine;
                PrintSlipData += Environment.NewLine;
                PrintSlipData += Environment.NewLine;
                PrintSlipData += Mess;
                PrintSlipData += Environment.NewLine;
                PrintSlipData += Environment.NewLine;
                PrintSlipData += Environment.NewLine;
                PrintSlipData += "===========================================";

            }
            else
            {
                Utils.ToCardLog(string.Format("ExchangeComplited response.Status ={0} ", response.Status));
                if ((response.Status != 1) && (response.Status != 16))
                {


                    string Mess = "Ошибка результата операции на терминале пластиковых карт." + Environment.NewLine + response.TextResponse + Environment.NewLine;
                    Mess += Environment.NewLine;
                    Mess += "Терминал: " + terminalId + Environment.NewLine;
                    Mess += Environment.NewLine;
                    Mess += "Ошибка проведения операции " + Environment.NewLine + " на терминале пластиковых карт." + Environment.NewLine;
                    Mess += Environment.NewLine;
                    Mess += "  Код операции: " + response.OperationCode + Environment.NewLine;
                    Mess += "  Код ответа: " + response.Status + Environment.NewLine;
                    Mess += "  Дополнительные данные ответа: " + Environment.NewLine + response.TextResponse + Environment.NewLine;
                    Mess += Environment.NewLine;

                    if (currentCheck != null)
                    {
                        foreach (InpasChk Chk2 in currentCheck.AllChks)
                        {
                            Mess += " Чек в Алохе " + Chk2.Num + Environment.NewLine;
                        }
                        Mess += "; Сумма " + (double)currentCheck.Ammount / 100D;

                    }
                    Utils.ToCardLog(Mess);
                    frmAllertMessage Mf = new frmAllertMessage(Mess);
                    Mf.ShowDialog();

                    PrintSlipData += Environment.NewLine;
                    PrintSlipData += Environment.NewLine;
                    PrintSlipData += Environment.NewLine;
                    PrintSlipData += Mess;
                    PrintSlipData += Environment.NewLine;
                    if (response.ReceiptData != null)
                    {
                        PrintSlipData += response.ReceiptData;
                    }
                    PrintSlipData += Environment.NewLine;
                    PrintSlipData += Environment.NewLine;
                    PrintSlipData += "===========================================";

                }
                else
                {
                    if (response.OperationCode == 26)
                    {

                        string Mess = "Проверка соединения завершена успешно." + Environment.NewLine;
                        Mess += Environment.NewLine;
                        Mess += "Терминал: " + terminalId + Environment.NewLine;
                        Mess += Environment.NewLine;
                        Utils.ToCardLog(Mess);
                        frmAllertMessage Mf = new frmAllertMessage(Mess);
                        Mf.ShowDialog();

                    }
                    else
                    {
                        /*
                        StringConverter Sc = new StringConverter();

                        byte[] btOut = new byte[64000];
                        int btOutLenght = 64000;
                        int l = Sc.Get1251Bytes(response.ReceiptData, btOut, btOutLenght);
                        btOut = new byte[l];
                        btOutLenght = l;
                        l = Sc.Get1251Bytes(response.ReceiptData, btOut, btOutLenght);
                        PrintSlipData = Encoding.GetEncoding(1251).GetString(btOut);
                        //    PrintSlipData =  Encoding.GetEncoding(866).GetString(Encoding.GetEncoding(1251).GetBytes(response.ReceiptData));
                         * */
                        if (currentCheck != null)
                        {
                            foreach (InpasChk Chk2 in currentCheck.AllChks)
                            {
                                PrintSlipData += "Чек в Алохе " + Chk2.Num + Environment.NewLine;
                            }
                        }

                        if ((response.ReceiptData == null) || (response.ReceiptData.Length < 30))
                        {
                            if (response.Status == 1)
                            {
                                PrintSlipData += "Операция выполнена успешно" + Environment.NewLine;
                            }
                            else
                            {
                                PrintSlipData += "Операция отклонна" + Environment.NewLine;
                            }
                            PrintSlipData += "Нет данных для печати" +Environment.NewLine;
                            string Mess = Environment.NewLine;
                            Mess += "Терминал: " + terminalId + Environment.NewLine;
                            Mess += Environment.NewLine;
                            Mess += "Ошибка проведения операции " + Environment.NewLine + " на терминале пластиковых карт." + Environment.NewLine;
                            Mess += Environment.NewLine;
                            Mess += "  Код операции: " + response.OperationCode + Environment.NewLine;
                            Mess += "  Код ответа: " + response.Status + Environment.NewLine;
                            Mess += "  Дополнительные данные ответа: " + Environment.NewLine + response.TextResponse + Environment.NewLine;
                            Mess += Environment.NewLine;

                            if (currentCheck != null)
                            {
                                foreach (InpasChk Chk2 in currentCheck.AllChks)
                                {
                                    Mess += " Чек в Алохе " + Chk2.Num + Environment.NewLine;
                                }
                                Mess += "; Сумма " + (double)currentCheck.Ammount / 100D;

                            }
                            PrintSlipData += Mess;
                            PrintSlipData += response.ReceiptData;
                        }
                        else
                        {

                            PrintSlipData += response.ReceiptData;
                        }
                    }
                }
            }
            TransactionInProcess = false;

            DualConnectorMain.TransactionComplited(response.OperationCode,response.Status, currentCheck, Code, CodeDescr, PrintSlipData);


        }
    }
}
