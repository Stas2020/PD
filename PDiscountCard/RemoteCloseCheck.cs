using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Linq;


namespace PDiscountCard
{
    

    static class RemoteCloseCheck
    {
        static internal Thread ShtrihWorkThread;
        
        static private List<CRemoteCloseChk> CloseingChecksQuere=new List<CRemoteCloseChk> ();
        //static private List<CRemoteCloseChk> ErrorChecksQuere;



        internal static void Init()
        {
            ShtrihWorkThread = new Thread(CloseingChecksQuereRun);
            ShtrihWorkThread.Name = "Remote Checks";
            ShtrihWorkThread.Priority = ThreadPriority.Lowest;
            ShtrihWorkThread.Start();
            Utils.ToCardLog("Запустил поток закрытия удаленных чеков" );
        }
        
        
        internal static void RemoteDeleteCheck(int ChkId)
        {
            int TermNum = iniFile.RemoteOrderTerminalNumber;
            AlohaTSClass.LogOut(TermNum);
            AlohaTSClass.LogIn(TermNum, iniFile.RemoteOrderWaterNumber);
            AlohaTSClass.DeleteAllItemsOnCurentCheckandClose2(TermNum, iniFile.RemoteOrderWaterNumber, ChkId);
            AlohaTSClass.LogOut(TermNum);
        }


        internal static void AddRemoteChkToQuereLocal(int ChkId, int PaymentId, int Empl, decimal PayMentSumm)
        {

            Utils.ToCardLog("Добавление чека в очередь закрытия текущего терминала" + ChkId);
            int Count = 0;
            while ((Interlocked.CompareExchange(ref TimerSyncPoint, 1, 0) != 0) && (Count < 5000))
            {
                Thread.CurrentThread.Join(50);
                // Utils.ToCardLog("GetCashReg lock" + Count);
                Count += 50;
            }

            CRemoteCloseChk Chk = new CRemoteCloseChk()
            {
                Id = ChkId,
                PaymentId = PaymentId,
                Empl = Empl,
                PaymentSumm = PayMentSumm
            };
            CloseingChecksQuere.Add(Chk);
            Interlocked.CompareExchange(ref TimerSyncPoint, 0, 1);
            CloseingChecksQuereWH.Set();
            Utils.ToCardLog("Добавил чек в очередь закрытия" + ChkId);
        }

        

        internal static void AddRemoteChkToQuere(int ChkId, int PaymentId,int Empl, decimal PayMentSumm)
        {
            if (iniFile.RemoteCloseCheckEnabled)
            {
                Utils.ToCardLog(String.Format("Добавление чека {0} в очередь закрытия терминала {1}",iniFile.RemoteCloseCheckTerminal, ChkId));

                PDiscountCard.RemoteCommands.RemoteSender.SendCheckOnClose(ChkId, PaymentId,Empl, PayMentSumm);    
            }

            else
            {
                AddRemoteChkToQuereLocal(ChkId, PaymentId, Empl, PayMentSumm);

            }
        }

        private static bool mExitRemoteCloseCheckThread = false;

        internal static void ExitRemoteCloseCheckThread()
        {
            mExitRemoteCloseCheckThread = true;
        }
        private static void CloseAlohaTable(CRemoteCloseChk RemChk)
        {
            int TermNum = iniFile.RemoteCloseTerminal;
            try
            {
                Utils.ToCardLog("Отправляю чек из очереди на закрытие" + RemChk.Id);
                
                AlohaTSClass.LogOut(TermNum);
                AlohaTSClass.LogIn(TermNum, iniFile.RemoteCloseEmployee);



                Check Chk = AlohaTSClass.GetCheckById(RemChk.Id);
                if (Chk == null)
                {
                    RemChk.ErrorCount++;
                    return;
                }
                /*
                if ((Chk.Oplata != 0) && (Chk.Oplata != Chk.Summ))
                {
                    AlohaTSClass.DeletePayments(Chk, TermNum);
                }
                 * */
                //if ((Chk.Oplata == 0))
               // {
                  //  if (RemChk.PaymentSumm == 0)
                    //{
                      //  RemChk.PaymentSumm = Chk.Summ;
                    //}
                    AlohaTSClass.ApplyPayment(TermNum, RemChk.Id, (double)RemChk.PaymentSumm, RemChk.PaymentId);
                //}
                AlohaTSClass.CloseCheck(TermNum, RemChk.Id);
                if (!AlohaTSClass.CheckIsClosed(RemChk.Id))
                {
                    RemChk.ErrorCount++;
                }
                else
                {
                    RemChk.ErrorCount = 0;
                    AlohaTSClass.LogOut(TermNum);
                    AlohaTSClass.LogIn(TermNum, RemChk.Empl);
                    AlohaTSClass.CloseTable(TermNum, Chk.TableId);
                }
                AlohaTSClass.LogOut(TermNum);
            }
            catch
            {
                AlohaTSClass.LogOut(TermNum);
                RemChk.ErrorCount++;
            }
        }


        public static void CloseAlohaTableLocalCurentUser(int ChkId, int PaymentId, decimal Summ)
        {
            int TermNum = AlohaTSClass.GetTermNum();
            try
            {
                Check Chk = AlohaTSClass.GetCheckById(ChkId);
                Utils.ToCardLog("[CloseAlohaTableLocalCurentUser] Пытаюсь закрть чек локально" + Chk.AlohaCheckNum + " Терминал " + TermNum);
                AlohaTSClass.ApplyPayment(TermNum, Chk.AlohaCheckNum, (double)Summ/100, PaymentId);
                AlohaTSClass.CloseCheck(TermNum, Chk.AlohaCheckNum);
                if (!AlohaTSClass.CheckIsClosed(Chk.AlohaCheckNum))
                {
                    //Сообщение
                }
                else
                {
                    Utils.ToCardLog("[CloseAlohaTableLocalCurentUser] Чек закрыл" + Chk.AlohaCheckNum);
                    if (AlohaTSClass.IsAlohaTS())
                    {
                        AlohaTSClass.CloseTable(TermNum, Chk.TableId);
                        Utils.ToCardLog("[CloseAlohaTableLocalCurentUser] Стол закрыл" + Chk.AlohaCheckNum);

                        //AlohaTSClass.LogOut();
                        //AlohaTSClass.LogIn(AlohaTSClass.GetTermNum(), 1354);
                        AlohaTSClass.RefreshCheckDisplay();
                    }
                    
                }
            }
            catch(Exception e)
            {

                Utils.ToCardLog("Error CloseAlohaTableLocalCurentUser" + e.Message);
            }
        }

        static private int TimerSyncPoint = 0;



        static EventWaitHandle CloseingChecksQuereWH = new AutoResetEvent(false);
        private static int ErrorTry = 0;
        private static void CloseingChecksQuereRun()
        {
            while (!mExitRemoteCloseCheckThread)
            {
                try
                {
                    TimerSyncPoint = 0;
                    if (CloseingChecksQuere.Where(a => a.ErrorCount == 0).Count() == 0)
                    {
                        CloseingChecksQuereWH.WaitOne(3000);
                    }
                     int Count = 0;
                    while ((Interlocked.CompareExchange(ref TimerSyncPoint, 1, 0) != 0) && (Count < 5000))
                    {
                        Thread.CurrentThread.Join(50);
                        Count += 50;
                    }
                    if (CloseingChecksQuere.Where(a => a.ErrorCount == 0).Count() > 0)
                    {
                            foreach (CRemoteCloseChk Chk in CloseingChecksQuere.Where(a => a.ErrorCount == 0))
                            {
                                CloseAlohaTable(Chk);
                            }
                            CloseingChecksQuere.RemoveAll(a => a.ErrorCount == 0);
                    }

                    if (CloseingChecksQuere.Where(a => a.ErrorCount > 0).Count() > 0)
                    {
                            foreach (CRemoteCloseChk Chk in CloseingChecksQuere.Where(a => a.ErrorCount > 0))
                            { 
                                CloseAlohaTable(Chk);
                            }
                            CloseingChecksQuere.RemoveAll(a => (a.ErrorCount == 0 || a.ErrorCount >10));
                            ErrorTry = 0;
                        
                    }
                    Interlocked.CompareExchange(ref TimerSyncPoint, 0, 1);

                }
                catch
                {
                    Interlocked.CompareExchange(ref TimerSyncPoint, 0, 1);
                }
            }
            CloseingChecksQuereWH.Close();
        }
    }
    public class CRemoteCloseChk
    {
        public CRemoteCloseChk()
        { }
        public int Id{set;get;}
        public int PaymentId { set; get; }
        public int Empl { set; get; }
        public decimal PaymentSumm { set; get; }
        public int ErrorCount=0;
    
    }
}
