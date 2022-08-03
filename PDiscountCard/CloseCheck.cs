using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Threading;
using System.Linq;

namespace PDiscountCard
{
    static public class CloseCheck
    {

        static internal Thread CloseCheckThread;
        static internal Thread CloseCheckFRSThread;
        static internal Thread JMSSendThread;

        static bool mExitCloseCheckThread = false;
        static bool mExitJMSSendThread = false;

        internal static void ExitCloseCheckThread()
        {
            mExitCloseCheckThread = true;
        }
        internal static void ExitJMSSendThread()
        {
            mExitJMSSendThread = true;
        }

        static internal void StartCloseCheckQuere()
        {
            if ((!iniFile.HamsterDisabled) && (!iniFile.FRSEnabled))
            {
                try
                {
                    Utils.ToLog("Десиарилизация чекового файла. Начало", 6);
                    myAllChecks = ReadAllChecks(ChecksPath + @"\hamster.xml");
                    Utils.ToLog("Десиарилизация чекового файла. Конец", 6);
                }
                catch
                {
                    Utils.ToLog("[Error] Ошибка Десиарилизации чекового файла.", 1);
                }
            }
            CloseCheckThread = new Thread(CloseCheckQuere);
            CloseCheckThread.Start();

            CloseCheckFRSThread = new Thread(CloseCheckFRSQuere);
            CloseCheckFRSThread.Start();

            

            
        }

        static internal void StartJMSSEndQuere()
        {
            JMSSendThread = new Thread(JMSSendQuere);
            JMSSendThread.Start();
        }

        static List<Check> JMSSendQuereChecks = new List<Check>();


        static internal bool CheckUnClosedChecks()
        {
            DirectoryInfo Di = new DirectoryInfo(ChecksPath);
            if (!Di.Exists) Di.Create();
            if (Di.GetFiles("*.1").Count() == 0)
            {
                return true;
            }

            Utils.ToCardLog("CheckUnClosedChecks");
            frmAllertMessage Mf = new frmAllertMessage("Существуют незакрытые чеки. Будет произведена попытка закрытия чеков. После этого снова запустите отчет.");
            Mf.ShowDialog();


            if (!Shtrih2.QuereIsEmpty())
            {
                frmAllertMessage Mf2 = new frmAllertMessage("Очередь чеков содержит незакрытые чеки. Это говорит о неполадках ФР. Убедитесь в исправности ФР и перезапустите отчет.");
                Mf2.ShowDialog();
                return false;
            }

            foreach (FileInfo fi in Di.GetFiles("*.1"))
            {
                Utils.ToCardLog("CheckUnClosedChecks " + fi.Name);
                try
                {
                    Check Ch = ReadCheckFromTmp(fi.FullName);
                    Ch.FiskalFileName = fi.FullName;
                    ToShtrih.CloseCheck2(Ch);
                }
                catch (Exception e)
                {
                    Utils.ToCardLog("[Error] CheckUnClosedChecks " + e.Message);
                }
            }
            return false;
        }


        static void CloseCheckFRSQuere()
        {
            try
            {
                DirectoryInfo Di = new DirectoryInfo(ChecksPath);
                if (!Di.Exists) Di.Create();

                while (!mExitCloseCheckThread)
                {
                    try
                    {
                        CreateCloseCheckFileEventWaitHandle.WaitOne();
                        try
                        {
                            foreach (FileInfo fi in Di.GetFiles("*.0"))
                            {
                                try
                                {
                                    Utils.ToCardLog("Очередь на фискальник. Файл " + fi.Name);
                                    String State1FileName = fi.FullName.Substring(0, fi.FullName.Length - 1) + "1";
                                    String State2FileName = fi.FullName.Substring(0, fi.FullName.Length - 1) + "2";
                                    Check Ch = ReadCheckFromTmp(fi.FullName);

                                    if (iniFile.FRSEnabled)
                                    {
                                        try
                                        {
                                            Utils.ToCardLog("FRSEnabled");
                                            if (Ch.GuidId == null) { Ch.GuidId = Guid.NewGuid(); }

                                            String State5FileName = fi.FullName.Substring(0, fi.FullName.Length - 2) + Ch.GuidId.ToString() + ".5"; //FRS
                                            Utils.ToCardLog("State5FileName: " + State5FileName);
                                            if (File.Exists(State5FileName))
                                            {
                                                Utils.ToCardLog("Exist. Delete: ");
                                                File.Delete(State5FileName);
                                            }
                                            fi.CopyTo(State5FileName);

                                            try
                                            {
                                                Utils.ToCardLog("fi.MoveTo(State2FileName): " + State2FileName);
                                                fi.MoveTo(State2FileName);
                                            }
                                            catch (Exception e)
                                            {
                                                Utils.ToCardLog("Error fi.MoveTo(State2FileName):  " + e.Message);
                                            }
                                            continue;
                                        }
                                        catch (Exception e)
                                        {
                                            Utils.ToCardLog("Error FRSEnabled " + e.Message);
                                        }
                                        try
                                        {
                                            Utils.ToCardLog("Переименовываю в  " + State2FileName);
                                            fi.MoveTo(State2FileName); //Нужен грамотный lock
                                        }
                                        catch (Exception ee)
                                        {
                                            Utils.ToCardLog("Error Переименовываю в  " + State2FileName + " " + ee.Message);
                                        }
                                    }
                                }
                                catch
                                { }
                            }
                        }
                        catch
                        { }
                        CreateCloseCheckFileEventWaitHandle.Set();
                    }
                    catch
                    { }

                    Thread.Sleep(1000);
                }
            }
            catch
            { }
        }


      internal static EventWaitHandle CreateCloseCheckFileEventWaitHandle = new AutoResetEvent(true);
      internal static EventWaitHandle WriteCheck2FileEventWaitHandle = new AutoResetEvent(true);

        static void CloseCheckQuere()
        {
            try
            {
                DirectoryInfo Di = new DirectoryInfo(ChecksPath);
                if (!Di.Exists) Di.Create();

                while (!mExitCloseCheckThread)
                {
                    try
                    {
                      //  CreateCloseCheckFileEventWaitHandle.WaitOne();
                        try
                        {
                            foreach (FileInfo fi in Di.GetFiles("*.0"))
                            {
                                try
                                {
                                    Utils.ToCardLog("Очередь на фискальник. Файл " + fi.Name);
                                    String State1FileName = fi.FullName.Substring(0, fi.FullName.Length - 1) + "1";
                                    String State2FileName = fi.FullName.Substring(0, fi.FullName.Length - 1) + "2";
                                    Check Ch = ReadCheckFromTmp(fi.FullName);

                                    if (iniFile.FRSEnabled)
                                    {
                                        continue;
                                        /*
                                        try
                                        {
                                            Utils.ToCardLog("FRSEnabled");
                                            if (Ch.GuidId == null) { Ch.GuidId = Guid.NewGuid(); }

                                            String State5FileName = fi.FullName.Substring(0, fi.FullName.Length - 2) + Ch.GuidId.ToString() + ".5"; //FRS
                                            Utils.ToCardLog("State5FileName: " + State5FileName);
                                            if (File.Exists(State5FileName))
                                            {
                                                Utils.ToCardLog("Exist. Delete: ");
                                                File.Delete(State5FileName);
                                            }
                                            fi.CopyTo(State5FileName);

                                            try
                                            {
                                                Utils.ToCardLog("fi.MoveTo(State2FileName): " + State2FileName);
                                                fi.MoveTo(State2FileName);
                                            }
                                            catch (Exception e)
                                            {
                                                Utils.ToCardLog("Error fi.MoveTo(State2FileName):  " + e.Message);
                                            }
                                            continue;
                                        }
                                        catch (Exception e)
                                        {
                                            Utils.ToCardLog("Error FRSEnabled " + e.Message);
                                        }
                                        try
                                        {
                                            Utils.ToCardLog("Переименовываю в  " + State2FileName);
                                            fi.MoveTo(State2FileName); //Нужен грамотный lock
                                        }
                                        catch (Exception ee)
                                        {
                                            Utils.ToCardLog("Error Переименовываю в  " + State2FileName + " " + ee.Message);
                                        }
                                        */
                                    }
                                    else
                                    {

                                        if (File.Exists(State1FileName))
                                        {
                                            Utils.ToCardLog("Существует " + State1FileName + " переношу в папку багов");
                                            if (!Directory.Exists(BugChecksPath))
                                            {
                                                Directory.CreateDirectory(BugChecksPath);
                                            }
                                            fi.MoveTo(BugChecksPath + fi.Name);
                                            continue;
                                        }

                                        Utils.ToCardLog("Переименовываю в  " + State1FileName);
                                        //Зачем это нужно ?
                                        fi.MoveTo(State1FileName); //Нужен грамотный lock
                                    }

                                    if (Ch != null)
                                    {
                                        if (myAllChecks.Checks.Where(a => a.CheckNum == Ch.CheckNum && a.Summ == Ch.Summ && a.SystemDateOfOpen == Ch.SystemDateOfOpen).Count() > 0)
                                        {
                                            Utils.ToCardLog("Чек присутствует в myAllChecks " + Ch.CheckNum);
                                            fi.Delete();
                                            continue;
                                        }


                                        if (Shtrih2.QuereContainsChk(Ch))
                                        {
                                            Utils.ToCardLog("Чек уже присутствует в очереди закрытия " + Ch.CheckNum);
                                            continue;
                                        }


                                        if ((Ch.HasFiskalPayment() && Ch.Summ != 0) && (!iniFile.NOLocalFR) && (!iniFile.FRSEnabled))
                                        {
                                            Utils.ToCardLog("Отправляю в фискальную очередь  ");
                                            if (!iniFile.FiskalDriverNonShtrih)
                                            {
                                                Ch.FiskalFileName = State1FileName;
                                                ToShtrih.CloseCheck2(Ch);
                                                fi.MoveTo(State2FileName);

                                            }
                                            else
                                            {
                                                Utils.ToCardLog("iniFile.FiskalDriverNonShtrih");
                                                if (FiskalDrivers.FiskalDriver.CloseCheck(Utils.GetFiskalCheck(Ch)))
                                                {
                                                    fi.MoveTo(State2FileName);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            Utils.ToCardLog("Нет фискальных платежей либо локального ФР");
                                            fi.MoveTo(State2FileName);
                                        }
                                    }

                                }
                                catch (Exception e)
                                {
                                    Utils.ToCardLog("[Error] Очередь на фискальник. Файл " + fi.Name + " Err " + e.Message);
                                }
                            }
                        }
                        catch
                        { }
                      //  CreateCloseCheckFileEventWaitHandle.Set();

                        WriteCheck2FileEventWaitHandle.WaitOne();
                        try
                        {

                            foreach (FileInfo fi in Di.GetFiles("*.2"))
                            {
                                try
                                {
                                    Utils.ToCardLog("*.2 Find " + fi.FullName);
                                    String State3FileName = fi.FullName.Substring(0, fi.FullName.Length - 1) + "3";
                                    Check Ch = ReadCheckFromTmp(fi.FullName);
                                    if (File.Exists(Ch.FiskalFileName)) { File.Delete(Ch.FiskalFileName); }

                                    if (Ch != null)
                                    {
                                        if (iniFile.SpoolEnabled)
                                        {
                                            //if (Ch.ConSolidateSpoolDishez.Count > 0)
                                            {
                                                Spool.SpoolCreator.AddToSpoolFile(Ch);
                                            }
                                        }
                                        if (iniFile.JMSEnable)
                                        {
                                            //if (Ch.ConSolidateSpoolDishez.Count > 0)
                                            {
                                                JMSSendQuereChecks.Add(Ch);
                                                //Thread th = new Thread(SendJms);
                                                //th.Start(Ch);
                                            }

                                        }

                                        if (iniFile.AlohaFlyExportEnabled)
                                        {
                                            AlohaFlyExport.AlohaFlyExportHelper exp = new AlohaFlyExport.AlohaFlyExportHelper();
                                            exp.SendOrderFlightAsync(Ch);
                                        }


                                    }
                                    if (iniFile.FRSEnabled)
                                    {
                                        String State6FileName = fi.FullName.Substring(0, fi.FullName.Length - 2) + Ch.GuidId.ToString() + ".6"; //FRS
                                        fi.MoveTo(State6FileName);
                                    }
                                    else
                                    {
                                        fi.MoveTo(State3FileName);
                                    }
                                }
                                catch (Exception e)
                                {
                                    Utils.ToCardLog("Error2  *.2 Find " + fi.FullName);
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Utils.ToCardLog("Error *.2 Find " + e.Message);
                        }
                        WriteCheck2FileEventWaitHandle.Set();

                        foreach (FileInfo fi in Di.GetFiles("*.3"))
                        {
                            //if ()

                            if (!iniFile.HamsterDisabled)
                            {
                                Check Ch = ReadCheckFromTmp(fi.FullName);
                                try
                                {
                                    AllChecks myAllChecks2 = ReadAllChecks(ChecksPath + @"\hamster.xml");
                                    if ((myAllChecks2.Checks.Count == 0) && File.Exists(ChecksPath + @"\hamster.xml"))
                                    {
                                        File.Copy(ChecksPath + @"\hamster.xml", ChecksPath + @"\hamster" + Guid.NewGuid() + ".xml");
                                    }
                                    myAllChecks2.Checks.Add(Ch);
                                    WriteAllChecks(myAllChecks2);
                                    fi.Delete();
                                }
                                catch (Exception ee)
                                {
                                    Utils.ToCardLog("[Error] CloseCheckQuere state 3 " + ee.Message);
                                }
                                myAllChecks.AddChk(Ch);
                            }
                            else
                            {
                                fi.Delete();
                            }
                        }

                        foreach (FileInfo fi in Di.GetFiles("*.6"))
                        {
                            try
                            {
                                //Check Ch = ReadCheckFromTmp(fi.FullName);
                                Hamster.HamsterWorker.SendHamsterChk(fi.FullName);

                            }
                            catch (Exception ee)
                            {
                                Utils.ToCardLog("[Error] CloseCheckQuere state 6 " + ee.Message);
                            }

                        }


                    }
                    catch (Exception e)
                    {
                        Utils.ToCardLog("[Error] CloseCheckQuere " + e.Message);
                    }
                    Thread.Sleep(200);
                }
            }
            catch (Exception ee)
            {
                Utils.ToCardLog("[Error] CloseCheckQuere2 " + ee.Message);
            }
        }

        static void JMSSendQuere()
        {

            while (!mExitJMSSendThread)
            {
                if (JMSSendQuereChecks.Count == 0)
                {
                    //TimerSyncPoint = 0;
                    Thread.Sleep(1000);
                    continue;
                }
                SendJms(JMSSendQuereChecks[0]);
                JMSSendQuereChecks.Remove(JMSSendQuereChecks[0]);

            }
        }

        private static void SendJms(Check ChkOb)
        {
            try
            {
                Utils.ToLog("Отправляю Jms: ");
                Check Ch = (Check)ChkOb;
                Spool.SpoolToGesWebSrv SpooToJms = new PDiscountCard.Spool.SpoolToGesWebSrv(Ch);

                string XmlTxt = SpooToJms.GetXmlStr2();

                JmsSender Js = new JmsSender();

                Js.CreateJmsSessionAndSendMsg(XmlTxt);
                Utils.ToLog("Отправил Jms ");

                if (false) //раскоментируем, когда Волков починит свою глюкалку
                {
                    Utils.ToLog("Отправляю Jms ВП: ");
                    string VPStr = SpooToJms.GetDishOrderTimeXMLList2(Ch);
                    Utils.ToLog(VPStr);
                    Js = new JmsSender();
                    Js.CreateJmsSessionAndSendMsg(VPStr);
                    Utils.ToLog(VPStr);
                    Utils.ToLog("Отправил Jms ВП");
                }
            }
            catch (Exception e)
            {
                Utils.ToLog("[Error] SendJms " + e.Message);
            }
        }


        static List<int> SuperCardsZAV = new List<int>() { 11691, 11598 };

        static string FileName = @"C:\Aloha\check\Fisk.xml";
        public static string ChecksPath = @"C:\aloha\check\discount\tmp\check";
        static public string BugChecksPath = @"C:\aloha\check\discount\tmp\check\bugs\";

        static public AllChecks myAllChecks = new AllChecks();



        static internal bool GetCheckColor(Check Ch)
        {


            FiskInfo Fi = ReadFiskInfo();
            if (!Fi.IsMod)
            {
                return true;
            }

            if (Ch.Vozvr)
            {
                return true;
            }
            /*
            if (Ch.Tender!=   TenderType.Cash)
            {
                return true;
            }
            */
            if (Ch.HasNonCashPayment())
            {
                return true;
            }

            if ((Ch.TableNumber >= 200) && (Ch.TableNumber <= 240))
            {
                return true;// FoodFox
            }


            //Shtrih.RegisterNumber = 241;
            // Shtrih.GetCashReg();


            decimal CurentShtrihCash = 5000;
            if (!iniFile.FiskalDriverNonShtrih)
            {
                try
                {
                    Utils.ToLog("CurentShtrihCash S");
                    CurentShtrihCash = Shtrih2.GetCashReg(241);
                    Utils.ToLog("CurentShtrihCash E");

                }
                catch (Exception e)
                {
                    Utils.ToCardLog("[Error] CurentShtrihCash " + e.Message);
                }
            }


            if (CurentShtrihCash < 5000)
            {
                return true;
            }
            foreach (Dish D in Ch.Dishez)
            {
                if (AlohaTSClass.DishIsTort(D.BarCode))
                {
                    return true;
                }
            }

            /*
            if (Ch.CompId != 0)
            {
                return true;
            }
            */
            if ((Ch.CompId == 8) || (Ch.CompId == 53))
            {
                return true;
            }


            try
            {
                if (AlohaTSClass.GetVip(Ch.AlohaCheckNum))
                {
                    Utils.ToCardLog("VIP");
                    return true;
                }
            }
            catch
            {

            }

            try
            {
                int P = 0;
                int V = 0;
                string DCard = AlohaTSClass.GetDiscountAttr((int)Ch.AlohaCheckNum, out P, out V);
                if (DCard.Length > 4)
                {
                    if (DCard.Substring(0, 3).ToUpper() == "ZAV")
                    {
                        try
                        {
                            string n = DCard.Substring(3);
                            int num = Convert.ToInt32(n);
                            if (SuperCardsZAV.Contains(num))
                            {
                                Utils.ToCardLog("card");
                                return true;
                            }
                        }
                        catch
                        {

                        }
                    }
                }
            }
            catch
            { }

            if (VerifyCheckPercent())
            {
                Utils.ToLog("Hamster");
                return true;
            }
            /*
            if (RegWorker.GetRegModType() == 3) //Азарика
            {
                Utils.ToCardLog("ModType3");
                if ((DateTime.Now.Hour >= 8) && (DateTime.Now.Hour < 22))

                //&& (DateTime.Now.DayOfWeek != DayOfWeek.Saturday) && (DateTime.Now.DayOfWeek != DayOfWeek.Sunday))
                {
                    int P = 0;
                    int V = 0;
                    string DCard = AlohaTSClass.GetDiscountAttr((int)Ch.AlohaCheckNum, out P, out V);
                    if (DCard.Length > 4)
                    {
                        if ((DCard.Substring(0, 3).ToUpper() == "ZAV") || (DCard.Substring(0, 3).ToUpper() == "VIP"))
                        {
                            return false;
                        }
                        else if (Ch.CompId == 2)
                        {
                            return false;
                        }
                        else
                        {
                            return true;
                        }

                    }
                    else if (Ch.CompId == 2)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }

            */


            if (RegWorker.GetRegModType() == 2)
            {

                if ((DateTime.Now.Hour >= 8) && (DateTime.Now.Hour < 22))

                //&& (DateTime.Now.DayOfWeek != DayOfWeek.Saturday) && (DateTime.Now.DayOfWeek != DayOfWeek.Sunday))
                {
                    Utils.ToCardLog("ModType2");
                    int P = 0;
                    int V = 0;
                    string DCard = AlohaTSClass.GetDiscountAttr((int)Ch.AlohaCheckNum, out P, out V);
                    if (DCard.Length > 4)
                    {
                        if ((DCard.Substring(0, 3).ToUpper() == "ZAV") || (DCard.Substring(0, 3).ToUpper() == "VIP"))
                        {
                            return false;
                        }
                        else if (Ch.CompId == 2)
                        {
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                    else if (Ch.CompId == 2)
                    {
                        return false;
                    }
                    else if (Ch.Summ > 2500)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }

                }
            }
            //else
            {


                if ((Ch.CompId == 6) || (Ch.CompId == 54)) //с собой
                {
                    return false;
                }

                DateTime dt2 = DateTime.Now;
                //  dt2 = AlohaTime(Ch.OpenTimeStr);
                if (dt2 > DateTime.Now)
                {
                    dt2.AddDays(-1);
                }


                if ((Ch.CheckTimeLong < Fi.MaxLong) && (Ch.CheckTimeLong > Fi.MinLong))
                {
                    if ((Ch.Summ < Fi.BigSum) && (Ch.Summ > Fi.LowSum))
                    {
                        //if (Ch.CompId == 0)
                        {
                            Random R = new Random();
                            if (R.Next(1, 100) > Fi.HiPercent)
                            {

                                return true;
                            }
                        }
                    }
                }

            }

            return false;
        }


        static internal bool VerifyCheckPercent()
        {

            Utils.ToLog("VerifyCheckPercent");
            if (RegWorker.GetRegPercent() == -1)
            {
                Utils.ToLog("RegWorker.GetRegPercent() ==-1 ");
                return false;
            }


            decimal CPerc = 0;
            if (myAllChecks.GetShtrihBalanse(true).AllSumm == 0)
            {
                CPerc = 100;
            }
            else
            {

                CPerc = (myAllChecks.GetShtrihBalanse(false).AllSumm * 100) / myAllChecks.GetShtrihBalanse(true).AllSumm;
            }

            return (CPerc < RegWorker.GetRegPercent() - 1);
        }

        static internal void mCloseCheck(Check Ch)
        {


            if (!iniFile.FRModeDisabled)
            {
                bool Color = false;
                if ((Ch.Summ != 0))
                {
                    Color = GetCheckColor(Ch);
                }
               
                if (Color)
                {
                    Ch.OpenTimem = 1; 
                }
                else
                {
                    Ch.OpenTimem = 0;
                }
               
            }
            //  Ch.SystemDate = DateTime.Now.AddMonths(1);
            Ch.SystemDate = DateTime.Now;

            Ch.CheckTimeLong = (Ch.SystemDate - Ch.SystemDateOfOpen).TotalMinutes;

            if (Ch.HasCreditPayment())
            {
                Loyalty.LoyaltyBasik Lb = new Loyalty.LoyaltyBasik();
                Lb.PrintAndSendCreditPaymentsTobase(Ch);
            }

            foreach (AlohaClientCard CC in Ch.AlohaClientCardList)
            {
                if (Loyalty.LoyaltyBasik.PresentCardPrefix.Contains(CC.Prefix) && CC.TypeId == "02")
                {
                    try
                    {
                        SVSistem.Main.AddCard(CC.Prefix, CC.Number, ((double)CC.CardPrice / 100d), Ch.AlohaCheckNum);
                    }
                    catch (Exception e)
                    {
                        Utils.ToCardLog("[Error] " + e.Message);
                    }
                }
            }

            Ch.GuidId = Guid.NewGuid();
            Ch.FrStringsBefore.Clear();
            Ch.FrStringsBefore.Add("ЧЕК " + Ch.CheckShortNum + "   (" + AlohainiFile.DepNum.ToString() + ")" + "   Стол" + Ch.TableNumber);


            Ch.FrStringsAfter.Clear();
            if (Ch.LoyaltyCard != "")
            {
                Ch.FrStringsAfter.Add("     ");
                Ch.FrStringsAfter.Add("   Программа лояльности КОФЕМАНИЯ АЭРО");
                Ch.FrStringsAfter.Add("   Начисление баллов");
                Ch.FrStringsAfter.Add("   Карта " + Ch.LoyaltyCard);
                Ch.FrStringsAfter.Add("   Начислено " + Ch.LoyaltyBonus.ToString("0.00") + " баллов");
            }



            foreach (AlohaTender AT in Ch.CreditPayments())
            {

                Ch.FrStringsAfter.Add("    ");
                Ch.FrStringsAfter.Add("   Списание средств");
                Ch.FrStringsAfter.Add("   Карта " + AT.Ident);
                Ch.FrStringsAfter.Add("   Списано " + AT.Summ.ToString("0.00"));


                DateTime dt = DateTime.Now;
                string Err = "";
                decimal Bal = Loyalty.LoyaltyBasik.GetASVCardBalance(AT.Ident, out dt, out Err);
                if (Bal != -1)
                {
                    Ch.FrStringsAfter.Add("   Текущий баланс " + Bal.ToString("0.00"));
                    Ch.FrStringsAfter.Add("   Срок действия карты " + dt.ToString("dd.MM.yyyy"));
                }
                Ch.FrStringsAfter.Add("    ");

            }
            if (iniFile.FayRetailEnabled)
            {
                try
                {
                    List<string> Strs = FayRetail.FayRetailMain.CloseCheck(Ch);
                    if (Strs != null)
                    {
                        foreach (string ss in Strs)
                        {
                            Utils.ToCardLog("Add FayRetStr " + ss);
                            Ch.FrStringsAfter.Add(ss);
                        }
                    }
                }
                catch { }
                //Ch.FrStringsAfter.AddRange(Strs);


            }

                       
            


                CreateCloseCheckFileEventWaitHandle.WaitOne();
                try
                {
                    Utils.ToLog("Записываю чек в файл временный.", 6);
                    WriteCheck(Ch, 0);
                }
                catch
                {
                    Utils.ToLog("[Error] Ошибка записи чека в файл временный.", 1);
                }
                CreateCloseCheckFileEventWaitHandle.Set();
            
        }

        /*
        static private void PrinterStatusVerification()
        {
            try
            {
                if (!ToShtrih.PrinterColorIsGray())
                {
                    //Андрюша раздолбай
                    //Надо ему об этом сообщить
                    EventSenderClass.SendAsincEvent(PDiscountCard.StopListService.RemoteEventType.AndrFoool, "");
                }
                else
                {
                    //  EventSenderClass.SendAsincEvent(PDiscountCard.s2010StopList.RemoteEventType.CheckClosed, "Good");
                }
            }
            catch
            { }
        }
        */
        /*
        static internal void CheckIsClosed(int TableId)
        {
            try
            {
                if (iniFile.Read("Options", "CloseCheck") == "TRUE")
                {
                    Utils.ToLog("CheckIsClosed TableId=" + TableId.ToString(), 6);
                    Check Ch = ReadCheckFromTmp();
                    if (Ch == null)
                    {
                        Utils.ToLog("Ch == null", 6);
                        return;
                    }
                    if (Ch.TableId == TableId)
                    {
                        Utils.ToLog("Ch.TableId == TableId", 6);
                        File.Delete(ChecksPath + @"\ch.xml");
                        Utils.ToLog("Файл удален", 6);
                    }
                }
            }
            catch (Exception e)
            {
                Utils.ToLog("Error CheckIsClosed " + e.Message, 6);

            }
        }
        */
        static internal void WriteAllChecks(AllChecks AllCh)
        {
            try
            {
                if (!Directory.Exists(ChecksPath))
                {
                    Directory.CreateDirectory(ChecksPath);
                }
                XmlWriter XWriter = new XmlTextWriter(ChecksPath + @"\hamster.xml", System.Text.Encoding.UTF8);
                //CardMooverInfoSerializer.CardMooverInfoSerializer.CardMooverInfoSerializer.CardMooverInfoSerializer.XmlSerializer XS = new XmlSerializer(typeof(CardMooverInfo));
                XmlSerializer XS = new XmlSerializer(typeof(AllChecks));
                //FiskInfo CDI = new FiskInfo();
                //CDI.FiskNum = CurentNum;И
                XS.Serialize(XWriter, AllCh);
                XWriter.Close();
            }
            catch
            {
            }
        }

        static public AllChecks ReadAllChecks()
        {
            return ReadAllChecks(ChecksPath + @"\hamster.xml");
        }

        static public AllChecks ReadAllChecks(string FileName)
        {

            if (!File.Exists(FileName))
            {
                return new AllChecks();

            }
            XmlReader XR = new XmlTextReader(FileName);
            try
            {
                //ToLog("[ReadVisitsFromFile] Читаю из файла " + Fi.FullName + "информацию о карте");

                XmlSerializer XS = new XmlSerializer(typeof(AllChecks));
                //XmlSerializer XS = new XmlSerializer(typeof(CardMooverInfo));
                AllChecks CMI = (AllChecks)XS.Deserialize(XR);
                XR.Close();

                return CMI;
            }
            catch
            {
                XR.Close();
                return new AllChecks();
            }
        }


        static internal void DeleteCheckFile()
        {
            try
            {
                if (Directory.Exists(ChecksPath))
                {
                    File.Delete(ChecksPath + @"\ch.xml");
                }
            }
            catch
            { }
        }

        static internal void WriteCheck(Check Ch, int State)
        {
            if (State == 2) { WriteCheck2FileEventWaitHandle.WaitOne(); }
            try
            {
                if (!Directory.Exists(ChecksPath))
                {
                    Directory.CreateDirectory(ChecksPath);
                }
                String Fname = ChecksPath + @"\ch" + Ch.CheckNum + ".xml" + "." + State.ToString();

                try
                {
                    if (File.Exists(Fname))
                    {
                        File.Copy(Fname, Fname + "_old", true);
                        File.Delete(Fname);
                    }
                }
                catch (Exception e)
                {
                    Utils.ToLog("Error WriteCheck old file" + e.Message);
                }

                SerializeCheck(Fname, Ch);
                XmlWriter XWriter = new XmlTextWriter(Fname, System.Text.Encoding.UTF8);
                XmlSerializer XS = new XmlSerializer(typeof(Check));
                XS.Serialize(XWriter, Ch);
                XWriter.Close();
            }
            catch (Exception ee)
            {
                Utils.ToLog("Error WriteCheck" + ee.Message);
            }
            if (State == 2) { WriteCheck2FileEventWaitHandle.Set(); }
        }


        public static void SerializeCheck(string path, Check chk)
        {
            XmlWriter XWriter = new XmlTextWriter(path, System.Text.Encoding.UTF8);
            XmlSerializer XS = new XmlSerializer(typeof(Check));
            XS.Serialize(XWriter, chk);
            XWriter.Close();


        }

        static internal Check ReadCheckFromTmp(string FileName)
        {

            if (!File.Exists(FileName))
            {
                return null;

            }

            int TryCount = 0;
            while (TryCount < 5)
            {
                TryCount++;
                XmlReader XR = new XmlTextReader(FileName);
                try
                {
                    XmlSerializer XS = new XmlSerializer(typeof(Check));
                    Check CMI = (Check)XS.Deserialize(XR);

                    XR.Close();
                    return CMI;
                }
                catch
                {
                    XR.Close();

                }

                Thread.Sleep(500);
            }
            return null;
        }


        static internal void InkreasePlastNum()
        {
            FiskInfo Fi = ReadFiskInfo();
            Fi.CardTransID++;
            WriteFiskInfo(Fi);
        }
        static internal void ZeroPlastNum()
        {
            FiskInfo Fi = ReadFiskInfo();
            Fi.CardTransID = 0;
            WriteFiskInfo(Fi);
        }

        static internal void WriteFiskInfo(FiskInfo Fi)
        {

            try
            {
                XmlWriter XWriter = new XmlTextWriter(FileName, System.Text.Encoding.UTF8);

                //CardMooverInfoSerializer.CardMooverInfoSerializer.CardMooverInfoSerializer.CardMooverInfoSerializer.XmlSerializer XS = new XmlSerializer(typeof(CardMooverInfo));
                XmlSerializer XS = new XmlSerializer(typeof(FiskInfo));
                //FiskInfo CDI = new FiskInfo();
                //CDI.FiskNum = CurentNum;И

                XS.Serialize(XWriter, Fi);
                XWriter.Close();



            }
            catch
            { }
        }
        static internal FiskInfo ReadFiskInfo()
        {
            if (!File.Exists(FileName))
            {
                return new FiskInfo
                {
                    ZIsPrinting = false,
                    BoundRate = 6,
                    FiskNum = "0",
                    PortNum = 1
                };
            }
            XmlReader XR = new XmlTextReader(FileName);
            try
            {
                //ToLog("[ReadVisitsFromFile] Читаю из файла " + Fi.FullName + "информацию о карте");

                XmlSerializer XS = new XmlSerializer(typeof(FiskInfo));
                //XmlSerializer XS = new XmlSerializer(typeof(CardMooverInfo));
                FiskInfo CMI = (FiskInfo)XS.Deserialize(XR);
                XR.Close();
                return
                CMI;
            }
            catch
            {
                XR.Close();
                return new FiskInfo
                {
                    ZIsPrinting = false,
                    BoundRate = 6,
                    FiskNum = "0",
                    PortNum = 1
                };
            }
        }
    }
    public class FiskInfo
    {

        public FiskInfo()
        {
        }
        [XmlElement]
        public DateTime DateOfLastWrite;
        [XmlElement]
        public string FiskNum;
        [XmlElement]
        public bool ZIsPrinting = false;
        [XmlElement]
        public int PortNum;
        [XmlElement]
        public int BoundRate;
        [XmlElement]
        public bool IsMod = false;
        [XmlElement]
        public string TimeOfBorder = (new DateTime(2000, 1, 1, 15, 35, 11)).ToString("HH:mm:ss");
        [XmlElement]
        public int LowPercent = 70;
        [XmlElement]
        public int HiPercent = 30;
        [XmlElement]
        public bool VisibleReport = false;
        [XmlElement]
        public int BigSum = 2000;
        [XmlElement]
        public int LowSum = 0;
        [XmlElement]
        public int MaxLong = 30;
        [XmlElement]
        public int MinLong = 0;
        [XmlElement]
        public int CardTransID = 0;
        [XmlElement]
        public bool NeedSverka = false;
        [XmlElement]
        public DateTime DateOfStart = DateTime.Now;
        [XmlElement]
        public bool NewMod = true;

    }

    public class AllChecks
    {
        public AllChecks()
        {

        }
        public List<Check> Checks = new List<Check>();

        public void AddRange(List<Check> Chks)
        {
            Checks.AddRange(Chks);
        }

        public void AddChk(Check Chk)
        {
            if (Chk != null)
            {
                try
                {
                    Checks.Add(Chk);
                }
                catch (Exception e)
                {
                    Utils.ToCardLog("Error AddChk to AllChecks Number " + Chk.AlohaCheckNum + " Summ " + Chk.Summ + " " + e.Message);
                }
            }
        }

        public Balanse GetShtrihBalanse(bool AllChk)
        {
            Balanse Tmp = new Balanse();
            foreach (Check Ch in Checks)
            {

                if (!AllChk)
                {
                    if (Ch.OpenTimem == 0)
                    {
                        continue;
                    }
                }
                foreach (AlohaTender AT in Ch.Tenders)
                {
                    if ((AT.TenderId == AlohaTender.CardTenderId) && (!Ch.Vozvr))
                    {
                        Tmp.Card += (double)Ch.Summ;
                        Tmp.CountCard += 1;
                    }
                    else if ((AT.TenderId == AlohaTender.CashTenderId) && (!Ch.Vozvr))
                    {
                        Tmp.Nal += (double)Ch.Summ;
                        Tmp.CountNal += 1;
                    }
                    else if ((AT.TenderId == AlohaTender.CashTenderId) && (Ch.Vozvr))
                    {
                        Tmp.VozvrNal -= (double)Ch.Summ;
                        Tmp.CountVozvrNal += 1;
                    }
                    else if ((AT.TenderId == AlohaTender.CardTenderId) && (Ch.Vozvr))
                    {
                        Tmp.VozvrCard -= (double)Ch.Summ;
                        Tmp.CountVozvrCard += 1;
                    }
                }
            }
            return Tmp;

        }
        /*
        internal Balanse GetBalanse()
        {
            Balanse Tmp = new Balanse();
            foreach (Check Ch in Checks)
            {
                if ((!Ch.IsNal) && (!Ch.Vozvr))
                {
                    Tmp.Card += (double)Ch.Summ;
                    Tmp.CountCard += 1;
                }
                else if ((Ch.IsNal) && (!Ch.Vozvr))
                {
                    Tmp.Nal += (double)Ch.Summ;
                    Tmp.CountNal += 1;
                }
                else if ((Ch.IsNal) && (Ch.Vozvr))
                {
                    Tmp.VozvrNal -= (double)Ch.Summ;
                    Tmp.CountVozvrNal += 1;
                }
                else if ((!Ch.IsNal) && (Ch.Vozvr))
                {
                    Tmp.VozvrCard -= (double)Ch.Summ;
                    Tmp.CountVozvrCard += 1;
                }
            }
            return Tmp;
        }
         * */

    }

}
