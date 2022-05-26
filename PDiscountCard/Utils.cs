using System;
using System.Collections.Generic;

using System.Text;
using System.IO;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;
using System.Diagnostics;
using System.Linq;
using FCCIntegration.FCCSrv2;

namespace PDiscountCard
{
    static public  class Utils
    {

        static internal string FilesPath = @"C:\Aloha\Check\Discount\";
        static internal string LogFilesPath = @"C:\Aloha\Check\Discount\Logs\";
        static internal string VizitsFilePath = @"C:\Aloha\Check\Discount\Tmp\";
        static internal string TmpSpoolFilePath = @"C:\Aloha\Check\Discount\Tmp\Spool\";
        static internal string Ininame = "PDiscount.ini";
        static internal string DownTimeIniFile = @"C:\Aloha\AlohaTs\DownTime.ini";
        static string LogName = "PDiscount.log";
        static string VizitsFileName = "PDiscountVizits.txt";



        static Dictionary<int, List<string>> LogList = new Dictionary<int, List<string>>();
        static Dictionary<int, List<string>> LogList2 = new Dictionary<int, List<string>>();


        //static internal int LogDeleteDays = 3;


        internal static Version GetWinVersion()
        {
            return System.Environment.OSVersion.Version;
             
        }

        internal static FiskalDrivers.FiskalCheck GetFiskalCheck(Check chk)
        {
            Utils.ToLog("GetFiskalCheck ", 6);
            FiskalDrivers.FiskalCheck Tmp = new FiskalDrivers.FiskalCheck ()
            {
            Charge = 0,
            Discount = Math.Abs((double)chk.Comp),
            Summ = Math.Abs((double)chk.Oplata),
            CheckNum = chk.CheckShortNum,
            TableName =chk.TableName,
            TimeofOpen =chk.SystemDateOfOpen,
            Cassir = AlohaTSClass.GetWaterName(chk.Cassir),
            Waiter = AlohaTSClass.GetWaterName(chk.Waiter),
           IsVoid = chk.Vozvr

            };

            foreach (AlohaTender Tndr in chk.Tenders)
            {
                Utils.ToLog("GetFiskalCheck Altender:" + Tndr.TenderId + " ", 6);
                FiskalDrivers.FiskalPayment p = new FiskalDrivers.FiskalPayment()
                {
                    Name = Tndr.Name,
                    Summ = Math.Abs(Tndr.Summ)
                };
                
                if (iniFile.FiskalDriverCashPayments.Contains(Tndr.TenderId))
                {
                    Utils.ToLog("GetFiskalCheck  p.PaymentType = 0;" , 6);
                    p.PaymentType = 0;
                }
                else if (iniFile.FiskalDriverCreditPayments.Contains(Tndr.TenderId))
                {
                    Utils.ToLog("GetFiskalCheck  p.PaymentType = 1;", 6);
                    p.PaymentType = 1;
                }
                else
                {
                    Utils.ToLog("GetFiskalCheck  p.PaymentType = -1;", 6);
                    p.PaymentType = -1;
                }
                Tmp.Payments.Add(p);
            }


            foreach (Dish d in chk.ConSolidateDishez)
            {
                FiskalDrivers.FiskalDish fd = new FiskalDrivers.FiskalDish()
                {
                    Discount = 0,
                    Name = d.LongName,
                    Price =Math.Abs((double)d.Priceone),

                    
                };

                if (AlohaTSClass.GalAlco.Contains(d.BarCode))
                {
                    fd.Name = "Открытый напиток"; // Это для галеры шереметьево
                }

                    fd.Quantity = (double)d.Count * (double)d.QtyQUANTITY;
                    Tmp.Dishes.Add(fd);
            }

            if (chk.Comp != 0)
            {
                Utils.ToLog("GetFiskalCheck Скидка:  " + Tmp.Discount.ToString());
                Tmp.Discount = (double)chk.Comp;
                Tmp.DiscountName = chk.CompName;
                
            }
                Tmp.Charge = Tmp.Summ - Tmp.Dishes.Sum(a => a.AllSumm);

          

            if (Tmp.Charge > 0)
            {
                Utils.ToLog("Charge: " + Tmp.Charge.ToString());
            }

            return Tmp;
        }

        internal static void DeleteOldLogs()
        {
            try
            {
                DirectoryInfo di = new DirectoryInfo (LogFilesPath);
                Utils.ToCardLog("Удаление старых логов");
                foreach (FileInfo fi in di.GetFiles())
                {
                    try
                    {
                        if (fi.CreationTime < DateTime.Now.AddDays(iniFile.LogDeleteDays * (-1)))
                        {
                            fi.Delete();
                        }
                    }
                    catch
                    { }
                }
            }

            catch
            { 
            
            }
        }


        internal static string GetFileServerName()
        {
            string MName = Environment.MachineName;
            //   string MName = "stend20";
            string Sname = "";

            if ((MName[MName.Length - 2] >= "0"[0]) && (MName[MName.Length - 2] <= "9"[0]))
            {
                Sname = MName.Substring(0, MName.Length - 2);
            }
            else
            {
                Sname = MName.Substring(0, MName.Length - 1);
            }
            Sname += "1";
            return Sname;
        }





        static internal int GetActualPriceChangeNum()
        {
            try
            {
                //string EventsCfgPath = @"\\"+GetFileServerName() +@"\bootdrv\alohats\data\EVENTS.cfg";
                string EventsCfgPath = @"C:\aloha\alohats\data\EVENTS.cfg";
                using (StreamReader SR = new StreamReader(EventsCfgPath))
                {
                    while (!SR.EndOfStream)
                    {
                        try
                        {
                            string[] s = SR.ReadLine().Split(" "[0]);
                            //Utils.ToCardLog("[GetActualPriceChangeNum] " + s[0]);
                            if (s.Length == 4)
                            {

                                if ((s[1] == "SETPRICECHANGE") && (s[3] == "1" || s[3] == "0") && s[0] == "00:00")
                                {
                                    
                                    if (AlohaTSClass.GetPriceStartDateIsActual(int.Parse(s[2])))
                                    {
                                        Utils.ToCardLog("[GetActualPriceChangeNum] " + s[2]);
                                        return Convert.ToInt32(s[2]);
                                    }

                                }
                            }
                        }
                        catch
                        { }
                    }
                }
                return 0;
            }
            catch(Exception e )
            {
                Utils.ToCardLog("Error GetActualPriceChangeNum " + e.Message);
                return -1;
            }
        }


        static internal int GetDiscountNumByCardSeries(int Series)
        {
            switch (Series)
            {
                case 80828:
                    //20%
                    return 2;
                case 80827:
                    //50%
                    return 8;
                case 90658:
                    //15%
                    return 1;
                case 80826:
                    //20%
                    return 4;
                case 80830:
                    //20 с собой
                    return 6;
                default:
                    return -1;
            }
        
        }

        static void mToLog(string Message)
        {
            DateTime DT1 = DateTime.Now;

            int ThId = Thread.CurrentThread.ManagedThreadId;

            string MStr = DT1.ToString("dd/MM/yy HH:mm:ss:") + DT1.Millisecond.ToString() + " " + ThId.ToString() + " " + Message;

            ToDirectLog(MStr);

            Debug.WriteLine(MStr, "ToLog");


            GetMyList(ThId).Add(MStr);
        }

        private static void ToDirectLog(string str)
        {
            string FullFileName = LogFilesPath + LogName + "_" + Thread.CurrentThread.ManagedThreadId + "_Thread" + DateTime.Now.ToString("ddMMyy") + ".log";

            if (!Directory.Exists(LogFilesPath))
            {
                Directory.CreateDirectory(LogFilesPath);
            }

            if (!(File.Exists(FullFileName)))
            {

                FileStream FS = File.Create(FullFileName);
                FS.Close();
            }

            StreamWriter SW = new StreamWriter(FullFileName, true);
         
            SW.WriteLine(str);
            
            SW.Close();
        }

        private static List<string> GetMyList(int DevId)
        {
            List<string> tmp = new List<string>();

            if (!(LogList.TryGetValue(DevId, out tmp)))
            {
                LogList.Add(DevId, new List<string>());
                LogList.TryGetValue(DevId, out tmp);

            }
            return tmp;
        }


        public  static void SaveLog()
        {
            if (LogList.Count == 0)
            {
                return;
            }

            if (!(Directory.Exists(FilesPath)))
            {

                Directory.CreateDirectory(FilesPath);
            }

            LogList2.Clear();
            lock (LogList)
            {
                foreach (int k in LogList.Keys)
                {
                    List<string> tmp = new List<string>();
                    LogList2.Add(k, tmp);
                    foreach (string Mess in GetMyList(k))
                    {
                        tmp.Add(Mess);
                    }
                }
                LogList.Clear();
            }

            foreach (int k in LogList2.Keys)
            {
                List<string> tmp = new List<string>();
                LogList2.TryGetValue(k, out tmp);
                string FullFileName = LogFilesPath + LogName + "_" + DateTime.Now.ToString("ddMMyy") + ".log";

                if (!(File.Exists(FullFileName)))
                {

                    FileStream FS = File.Create(FullFileName);
                    FS.Close();
                }

                StreamWriter SW = new StreamWriter(FullFileName, true);


                foreach (string Mess in tmp)
                {
                    SW.WriteLine(Mess);
                }
                SW.Close();
            }

            LogList2.Clear();

        }




        internal static void ToLog(string Message, int Level)
        {
            int CurentLevelLog = 8;
            try
            {
                CurentLevelLog = iniFile.CurentLevelLog;
            }
            catch
            {

            }
            if (Level <= CurentLevelLog)
            {
                ToLog(Message);
            }
        }

        internal static double ConvertToDoubleWithRepDemSep(String Val)
        {
            double k = 1.1;
            char t = k.ToString()[1];
            string s = Val.Replace("."[0], t);
            s = s.Replace(","[0], t);
            return Convert.ToDouble(s);


        }

        public static void ToLog(string Message)
        {
            try
            {
                mToLog(Message);
            }
            catch (Exception e)
            {
                AlohaTSClass.ShowMessage("Ошибка записи в лог-файл " + e.Message);
            }
        }

        internal static void ToCardLog(string Message)
        {
            try
            {
                mToLog("[CardLog] " + Message);
            }
            catch (Exception e)
            {
                AlohaTSClass.ShowMessage("Ошибка записи в лог-файл " + e.Message);
            }
        }

        static public byte[] HexStringToByteArray(string s)
        {
            s = s.Replace(" ", "");
            byte[] buffer = new byte[s.Length / 2];
            for (int i = 0; i < s.Length; i += 2)
                buffer[i / 2] = (byte)Convert.ToByte(s.Substring(i, 2), 16);
            return buffer;
        }

        static public int GetUnTermNum()
        {
            return Convert.ToInt32((AlohainiFile.DepNum.ToString() + GetTermNum().ToString()));
        }

        static public int GetTermNum()
        {
            string MName = Environment.MachineName;
            //   string MName = "stend20";
            string Sname = "";

            if ((MName[MName.Length - 2] >= "0"[0]) && (MName[MName.Length - 2] <= "9"[0]))
            {

                return Convert.ToInt32(MName.Substring(MName.Length - 2, 2));
            }
            else
            {
                return Convert.ToInt32(MName.Substring(MName.Length - 1, 1));
            }
        }


        static internal bool WriteInfoToFile(string Data, string FileName)
        {
            try
            {
                string FilePath = iniFile.Read("Options", "StoreClaimCheckPath");
                if (FilePath[FilePath.Length - 1] != @"\"[0])
                {
                    FilePath += @"\";
                }

                FileName = FilePath + FileName;
                if (File.Exists(FileName)) File.Delete(FileName);
                StreamWriter writer = File.AppendText(FileName);
                writer.WriteLine(Data);
                writer.Close();
                ToLog("[WriteInfoToFile] Информация " + Data + " успешно выгружена в файл " + FileName);
                return true;
            }
            catch (Exception e)
            {
                ToLog("[ERROR] [WriteInfoToFile] Информация " + Data + " успешно не выгружена в файл " + FileName + "по причине: " + e.Message);
                return false;
            }
        }

        static internal void WriteVisitToFile2(CardMooverInfo CDI)
        {
            string FileName = DateTime.Now.ToString("HHmmss_ddMMyy") + ".xml";
            try
            {
                ToLog("Выгружаю в файл информацию о карте");
                DirectoryInfo di = new DirectoryInfo(VizitsFilePath);
                if (!di.Exists)
                {
                    di.Create();
                }

                XmlWriter XWriter = new XmlTextWriter(di + FileName, System.Text.Encoding.UTF8);

                //CardMooverInfoSerializer.CardMooverInfoSerializer.CardMooverInfoSerializer.CardMooverInfoSerializer.XmlSerializer XS = new XmlSerializer(typeof(CardMooverInfo));
                //XmlSerializer XS = new CardMooverInfoSerializer();
                XmlSerializer XS = new XmlSerializer(typeof(CardMooverInfo));

                XS.Serialize(XWriter, CDI);
                XWriter.Close();
                ToLog("Выгрузил в файл " + di + FileName + " информацию о карте");
            }
            catch (Exception e)
            {
                ToLog("[ERROR] [WriteVisitToFile2] Ошибка выгрузки информации о карте в файл " + FileName + ": " + e.Message);
            }
        }



        static internal void ReadVisitsFromFile()
        {
            try
            {
                Thread Th = new Thread(ReadVisitsFromFileAsink);
                Th.Start();
            }
            catch (Exception e)
            {
                Utils.ToCardLog("[Error] ReadVisitsFromFile");
            }

        }

        static internal void ReadVisitsFromFileAsink()
        {
            try
            {
                DirectoryInfo di = new DirectoryInfo(VizitsFilePath);
                if (!di.Exists)
                {
                    di.Create();
                    return;
                }

                foreach (FileInfo Fi in di.GetFiles())
                {
                    if (Fi.Name.Contains("pl"))
                    {
                        return;
                    }
                    try
                    {
                        ToLog("[ReadVisitsFromFile] Читаю из файла " + Fi.FullName + "информацию о карте");
                        XmlReader XR = new XmlTextReader(Fi.FullName);
                        //XmlSerializer XS = new CardMooverInfoSerializer();

                        XmlSerializer XS = new XmlSerializer(typeof(CardMooverInfo));
                        CardMooverInfo CMI = (CardMooverInfo)XS.Deserialize(XR);
                        XR.Close();
                        if (CMI.DoVizit() == 1)
                        {
                            Fi.Delete();
                            ToLog("[ReadVisitsFromFile] Прочитал из файла " + Fi.FullName + "информацию о карте и удалил файл");
                        }
                        else
                        {
                            return;
                        }
                    }
                    catch (Exception e)
                    {
                        ToLog("[ERROR] [WriteVisitToFile2] Ошибка чтения информации о карте из файла " + Fi.FullName + ": " + e.Message);
                    }
                }
            }
            catch
            {

            }
        }

        static internal string GetDayWord(int c)
        {
            try
            {
                string ss = c.ToString();
                if (ss.Length > 1)
                {
                    if (ss[ss.Length - 1] == "1"[0])
                    {
                        return "дней";
                    }
                }
                string i = ss[ss.Length].ToString();
                if (i == "1")
                {
                    return "день";
                }
                if (i == "2")
                {
                    return "дня";
                }
                if ((i == "3") || (i == "4") || (i == "2"))
                {
                    return "дня";
                }

            }
            catch
            {

            }
            return "дней";
        }

        static internal bool WriteVisitToFile(string Prefix, string CardNum, int CodSh, string CheckNum, int TermNum,
         DateTime CDT)
        {
            string Data = "";
            string FileName = "";
            try
            {
                FileName = FilesPath + VizitsFileName;
                StreamWriter writer;
                if (!File.Exists(FileName))
                {
                    writer = File.CreateText(FileName);
                }
                else
                {
                    writer = File.AppendText(FileName);
                }
                Data = Prefix + "," + CardNum + "," + CodSh.ToString() + "," + CheckNum + "," + TermNum.ToString() + "," + CDT.ToString();
                writer.WriteLine(Data);
                writer.Close();
                ToLog("[WriteVisitToFile]  Информация о посещении" + Data + " успешно выгружена в файл " + FileName);
                return true;
            }
            catch (Exception e)
            {
                ToLog("[ERROR][WriteVisitToFile] Информация о посещении " + Data + "не выгружена в файл " + FileName + "по причине: " + e.Message);
                return false;
            }
        }

    }

    internal enum CardTypes { Manager, Discount, Friend, SM, NonFind, PodKarta,Sber, NewLoyCard,None }

    internal class MCard
    {
        internal string Num = "-1";
        internal String Prefix = "-1";
        internal bool bad
        {
            get
            {
                if (((Num == "-1") && (Prefix == "-1")) || (Num == "") || (Prefix == ""))
                {
                    return true;
                }
                else
                {
                    try
                    {
                        long i = Convert.ToInt64(Num.Replace("!", ""));
                        if (i > 0)
                        {
                            return false;
                        }
                    }
                    catch
                    {
                        Utils.ToLog("[MCard] Ошибка при чтении номера карты.");
                        return true;
                    }

                    return false;
                }

            }
        }




        CardTypes _mType=CardTypes.None;

        private CardTypes GetmType()
        {
            Utils.ToLog("Prefix.ToUpper() == " + Prefix.ToUpper());
            if (Prefix == "83857")
            {
                return CardTypes.NewLoyCard;
            }


            if ((Prefix.ToUpper() == "20180") ||
                (Prefix.ToUpper() == "20181") || 
                (Prefix.ToUpper() == "20182") ||
                (Prefix.ToUpper() == "20183") ||
                (Prefix.ToUpper() == "20184") ||
                (Prefix.ToUpper() == "20185") ||
                (Prefix.ToUpper() == "20186") ||
                (Prefix.ToUpper() == "20187") ||
                (Prefix.ToUpper() == "20188") ||
                (Prefix.ToUpper() == "20189"))
            {
                return CardTypes.Sber;
            }

            if (Prefix.ToUpper() == "80827")
            {
                return CardTypes.Discount;
            }

            if (Prefix.ToUpper() == "83858")
            {
                return CardTypes.Discount;
            }


            if ((Prefix.ToUpper() == "000") || (Prefix.ToUpper() == "99999"))
            {
                return CardTypes.Manager;
            }
            //if (Prefix.ToUpper() == iniFile.Read("Discounts", "ClaimPrefix") || Prefix.ToUpper() == "906")
            if (Prefix.ToUpper() == iniFile.Read("Discounts", "ClaimPrefix"))
            {
                return CardTypes.Friend;
            }
            else if (iniFile.Read("Discounts", Prefix.ToUpper()) != null)
            {
                return CardTypes.Discount;
            }
            else if (Prefix == "80830")
            {
                return CardTypes.Discount;
            }

            else if (iniFile.Read("PrivilegedKey", Prefix + Num) != null)
            {
                return CardTypes.Discount;
            }
            else if (Prefix.ToUpper() == "26610" || Prefix.ToUpper() == "26605" || Prefix.ToUpper() == "26603"
                || Prefix.ToUpper() == "266"
                || Prefix.ToUpper() == "267" || Prefix.ToUpper() == "26720" || Prefix.ToUpper() == "26750" || Prefix.ToUpper() == "77577")
            {
                Utils.ToLog("Pod cart");
                return CardTypes.PodKarta;
            }
            else if (Prefix.ToUpper() == "SM")
            {
                Utils.ToLog("Prefix SM");
                return CardTypes.SM;
            }
            Utils.ToLog("Prefix NonFind");
            return CardTypes.NonFind;
        }

        internal CardTypes mType
        {
            get
            {
                if (_mType == CardTypes.None)
                {
                    _mType = GetmType();
                }
                return _mType;
            }
        }

        internal int DiscountType
        {
            get
            {
                try
                {

                    if (mType == CardTypes.Sber)
                    {
                        return 9;
                    }
                    if (mType == CardTypes.Discount)
                    {
                        int k = 0;
                        try
                        {
                            if ((Prefix == "80827")||(Prefix == "83858"))
                            {
                                Utils.ToLog("Супервип. return 8");
                                //string s = iniFile.Read("PrivilegedKey", Prefix + Num);
                                return 8;
                                
                            }

                            if (Prefix == "VIP")
                            {
                                if ((Convert.ToInt32(Num) == 108) || (Convert.ToInt32(Num) == 110) || (Convert.ToInt32(Num) == 111) || (Convert.ToInt32(Num) == 112))
                                {
                                    //Это для мэра сочи и его мудаков
                                    Utils.ToLog("Вип карта 3: "  + (Prefix + Num).ToString());
                                    return 3;
                                }
                                
                            }
                        }
                        catch { }


                        try
                        {
                            k = Convert.ToInt32(iniFile.Read("PrivilegedKey", Prefix + Num));
                            //Utils.ToLog("Привелигированая карта: тип:" + k + " карта:" + (Prefix + Num).ToString());
                        }
                        catch
                        {
                            k = 0;
                        }

                        if (k > 0)
                        {
                            /*
                            if (k == 8)
                            {
                                Utils.ToLog("k == 8 Меняю префикс на 80827:" );
                                Prefix = "80827";
                            }
                             * */
                            return k;
                        }

                        else if (iniFile.Read("Discounts", Prefix) != null)
                        {
                            return Convert.ToInt32(iniFile.Read("Discounts", Prefix.ToUpper()));
                        }
                        return -1;
                    }
                    else
                    {
                        return -1;
                    }
                }
                catch
                {
                    return -1;
                }
            }
        }

        internal string PurgeNum(string Or)
        {
            string s = Or.Replace(";", "");
            s = s.Replace("%", "");
            s = s.Replace("?", "");
            return s;
        }

        internal MCard(string Track1, string Track2)
        {
            try
            {
                Utils.ToCardLog("Track1 " + Track1);
                Utils.ToCardLog("Track2 " + Track2);
                Track1 = PurgeNum(Track1);
                Track2 = PurgeNum(Track2);


                // если вся информация записана на первой дорожке карты
                if (Track2.Length == 0 && Track1.Length > 3)
                {
                    Utils.ToLog("вся информация записана на первой дорожке карты");
                    Track2 = Track1.Substring(3);
                    Track1 = Track1.Substring(0, 3);
                    
                }
                
                // если вся информация записана на второй дорожке карты
                if (Track1.Length == 0 && Track2.Length > 3)
                {
                    Utils.ToLog("вся информация записана на второй дорожке карты");
                    if ((Track2.Substring(0, 5) == "20180") || 
                        (Track2.Substring(0, 5) == "20181") ||
                        (Track2.Substring(0, 5) == "20182") ||
                        (Track2.Substring(0, 5) == "20183") ||
                        (Track2.Substring(0, 5) == "20184") ||
                        (Track2.Substring(0, 5) == "20185") ||
                        (Track2.Substring(0, 5) == "20186") ||
                        (Track2.Substring(0, 5) == "20187") ||
                        (Track2.Substring(0, 5) == "20188") ||
                        (Track2.Substring(0, 5) == "20189") )
                    {
                        Track1 = Track2.Substring(0, 5);
                        Track2 = Track2.Substring(5);
                        Utils.ToLog("Это карта сбер ");
                        Num = Track2;
                        Prefix = Track1;
                        return;
                    }

                    if ((Track2.Substring(0, 5) == "83857"))
                    {
                        Track1 = Track2.Substring(0, 5);
                        Track2 = Track2.Substring(5);
                        return;
                    }

                    if ((Track2.Substring(0, 5) == "83858")) //зелёные карты
                    {
                        Track1 = Track2.Substring(0, 5);
                        Track2 = Track2.Substring(5);
                        Num = Track2;
                        Prefix = Track1;
                        Utils.ToLog($"Зелёные карты Prefix: {Track1} Number {Track2}");
                        return;
                    }   

                    if ((Track2.Substring(0,5) == "80830")||
                        (Track2.Substring(0, 5) == "86738")||
                        (Track2.Substring(0, 5) == "80827") ||
                        (Track2.Substring(0, 5) == "90658") ||
                        (iniFile.Read("Discounts", Track2.Substring(0, 5).ToUpper()) != null))
                    {
                        Track1 = Track2.Substring(0, 5);
                        Track2 = Track2.Substring(5);
                    }
                    
                    else
                    {
                        Track1 = Track2.Substring(0, 3);
                        Track2 = Track2.Substring(3);
                    }
                }
                Num = Track2;
                Prefix = Track1;

                if (Prefix == "90658")
                {
                    Prefix = "ZAV";
                 
                }

                if (Prefix == "86738")
                {
                    Prefix = "PRE";
                    Num = Num.Substring(2);
                }

                if ((Prefix == "ZAV") && (AlohainiFile.DepNum == 371) && ((Num == "000000012456") || (Num == "000000012457")))
                {
                    Prefix = "VIP";
                    Utils.ToLog("Это карта, которая на новой площади работает как вип. Меняю префикс " );
                }
                if ((Prefix == "ZAV") && (AlohainiFile.DepNum == 295) && ((Num == "000000008056")))
                {
                    Prefix = "VIP";
                    Utils.ToLog("Это карта, которая на комсомолке работает как вип. Меняю префикс ");
                }


                if ((Prefix == "VIP") && !(AlohainiFile.DepNum == 205) && ((Num == "000000004715")))
                {
                    Prefix = "BAD";
                    Utils.ToLog("Это карта, которая Должна работать только на авроре. Меняю префикс ");
                }

                if ((Prefix == "VIP") && !(AlohainiFile.DepNum == 180) && ((Num == "000000003086") || (Num == "000000003087") || (Num == "000000003088") || (Num == "000000003089") ))
                {
                    Prefix = "BAD";
                    Utils.ToLog("Это карта, которая Должна работать только на Осенней. Меняю префикс ");
                }

                if ((Prefix == "VIP") && !((AlohainiFile.DepNum == 180) || (AlohainiFile.DepNum == 370)) && ((Num == "000000003084") || (Num == "000000003085")))
                {
                    Prefix = "BAD";
                    Utils.ToLog("Это карта, которая Должна работать только на Осенней и Кутузовской. Меняю префикс ");
                }


                if ((Prefix == "VIP") && !(AlohainiFile.DepNum == 310) && ((Num == "000000004716")))
                {
                    Prefix = "BAD";
                    Utils.ToLog("Это карта, которая Должна работать только на внуково. Меняю префикс ");
                }

                if (Prefix == "906") 
                {
                    long s = Convert.ToInt64(Num);
                    if (s >= 58000020500 && s <= 58000021500)
                    {
                        Prefix = "PRE";
                        Num = Num.Substring(4);
                        Utils.ToLog("Это костыль для префикса 906. Будет карта друга. Меняю префикс ");
                    }
                }


                if ((Prefix == "ZAV") || (Prefix == "90658"))
                {
                    long s = Convert.ToInt64(Num);
                    if (s >= 000020500 && s <= 000021500)
                    {
                        Prefix = "PRE";
                        Num = Num.Substring(2);
                        Utils.ToLog("Это костыль для префикса 906. Будет карта друга. Меняю префикс ");
                    }
                }

                try
                {
                    if ((Prefix == "PRE") && ((Convert.ToInt64(Num) == 4370) || (Convert.ToInt64(Num) == 4371) || (Convert.ToInt64(Num) == 4372)))
                    {
                        Utils.ToLog("Это прикол от АЛ. Будет 20");
                        Prefix = "VIP";
                    }
                }
                catch
                { }

            }
            catch (Exception e)
            {
                Utils.ToLog("[ERROR] [MCard] Неудачное распознание карты" + Track1 + "  " + Track2 + " " + e.Message);
            }
        }
    }




    internal class AbsiniFile
    {
        FileInfo file = null;
        // FileInfo file1 = null;

        internal AbsiniFile(string Path)
        {

            file = new FileInfo(Path);
            if (!file.Exists)
            {
                Utils.ToLog("[AbsiniFile] Не могу найти файл конфигурации " + Path);
            }
        }


        public string Read(string Section, string Param)
        {
            string line, result = null;

            bool SectionFound = (Section == null);

            if (!file.Exists)
                return null;

            // StreamReader reader = new StreamReader(file.FullName);
            StreamReader reader = new StreamReader(file.FullName, System.Text.Encoding.GetEncoding("Windows-1251"));

            while ((line = reader.ReadLine()) != null)
            {
                line = line.Trim();

                if (line.StartsWith("#"))
                    continue;

                if (!SectionFound)
                {
                    if (string.Compare(line, "[" + Section + "]", true) == 0)
                    {
                        SectionFound = true;
                        continue;
                    }
                }
                else
                {
                    if (line.StartsWith("[") && line.EndsWith("]"))
                    {
                        SectionFound = false;
                        continue;
                    }

                    if ((line.IndexOf("=") > 0) &&
                        (line.ToUpper().Split(("=").ToCharArray(), 2)[0].Trim() == Param.ToUpper().Trim()))
                    {
                        result = (line.Split(("=").ToCharArray(), 2)[1]).Trim();
                        result = result.ToUpper();
                        break;
                    }
                }
            }

            reader.Close();

            // reader.Dispose();

            return result;
        }






        public string Read(string Section, string Param, string Default)
        {
            string value = Read(Section, Param);

            return (value == null) ? Default : value;
        }

        public bool  Read(string Section, string Param, bool Default)
        {
            string value = Read(Section, Param);
            if (value == null)
            {
                return Default;
            }
            else if ((value.ToUpper() == "TRUE") || (value.ToUpper() == "1"))
            {
                return true;
            }
            else if ((value.ToUpper() == "FALSE") || (value.ToUpper() == "0"))
            { 
            return false ;
            }


            return Default;
        }


        public int Read(string Section, string Param, int Default)
        {
            int value;

            try
            {
                value = Convert.ToInt32(Read(Section, Param, Default.ToString()));
            }
            catch
            {
                value = Default;
            }
            return value;
        }
    }

    static internal class AlohainiFile
    {

        static AbsiniFile AbsIni;


        static AlohainiFile()
        {

            string MName = Environment.MachineName;
            //   string MName = "stend20";
            string Sname = "";

            if ((MName[MName.Length - 2] >= "0"[0]) && (MName[MName.Length - 2] <= "9"[0]))
            {
                Sname = MName.Substring(0, MName.Length - 2);
            }
            else
            {
                Sname = MName.Substring(0, MName.Length - 1);
            }
            Sname += "1";




            if (File.Exists(@"\\" + Sname + @"\Bootdrv\AlohaTS\Data\Aloha.ini"))
            {
                AbsIni = new AbsiniFile(@"\\" + Sname + @"\Bootdrv\AlohaTS\Data\Aloha.ini");
            }
            else
            {
                AbsIni = new AbsiniFile(@"C:\Aloha\AlohaTS\Data\Aloha.ini");
            }




            ReadBD();
        }


        
        static internal string TermStr
        {
            get
            {
                string MName = Environment.MachineName;
                //   string MName = "stend20";
                string Sname = "";

                if ((MName[MName.Length - 2] >= "0"[0]) && (MName[MName.Length - 2] <= "9"[0]))
                {
                    Sname = MName.Substring(0, MName.Length - 2);
                }
                else
                {
                    Sname = MName.Substring(0, MName.Length - 1);
                }
                return Sname;
            }
        }

        static DateTime _BDate;
        static internal DateTime BDate
        {
            get
            {
                return _BDate;
            }
        }

        static int _DepNum;
        static internal int DepNum
        {
            get
            {
                return _DepNum;
            }
        }
        static string _UNITNAME;
        static internal string UNITNAME
        {
            get
            {
                return _UNITNAME;
            }
        }
        static string _ADDRESS1;
        static internal string ADDRESS1
        {
            get
            {
                return _ADDRESS1;
            }
        }
        static string _ADDRESS2;
        static internal string ADDRESS2
        {
            get
            {
                return _ADDRESS2;
            }
        }



        static void ReadBD()
        {
            string s = AbsIni.Read("IBERTECH", "DOB");
            _BDate = new DateTime(Convert.ToInt32(s.Substring(6, 4)), Convert.ToInt32(s.Substring(0, 2)), Convert.ToInt32(s.Substring(3, 2)), 0, 0, 0);
            _DepNum = Convert.ToInt32(AbsIni.Read("IBERTECH", "UNITNUMBER"));
            _UNITNAME = AbsIni.Read("IBERTECH", "UNITNAME");
            _ADDRESS1 = AbsIni.Read("IBERTECH", "ADDRESS1");
            _ADDRESS2 = AbsIni.Read("IBERTECH", "ADDRESS2");
        }
    }

    static public  class iniFile
    {
        static AbsiniFile AbsIni;


        static iniFile()
        {

            AbsIni = new AbsiniFile(Utils.FilesPath + Utils.Ininame);
        }


        internal static bool CloseCheck
        {
            get
            {
                return  AbsIni.Read("Options", "CloseCheck", true);
                

            }
        }



        

        internal static string Read(string Section, string Param)
        {
            return AbsIni.Read(Section, Param);
        }

        internal static bool StopListOff
        {
            get
            {
                int k = AbsIni.Read("Options", "StopListOff", 0);
                return (k == 1);

            }
        }
        /*
        internal static bool NoFiskalChanger
        {
            get
            {
                int k = AbsIni.Read("Options", "NoFiskalChanger", 0);
                return (k == 1);

            }
        }
        */
        internal static bool RemoteOrder
        {
            get
            {
                int k = AbsIni.Read("Options", "RemoteOrder", 0);
                return (k == 1);

            }
        }

        internal static bool WinHook
        {
            get
            {
                int k = AbsIni.Read("Options", "WinHook", 0);
                return (k == 1);

            }
        }

        internal static bool NoCut
        {
            get
            {
                int k = AbsIni.Read("Options", "NoCut", 0);
                return (k == 1);

            }
        }
        internal static bool CardEmulate
        {
            get
            {
                int k = AbsIni.Read("Options", "CardEmulate", 0);
                return (k == 1);

            }
        }

        internal static bool NOLocalFR
        {
            get
            {
                int k = AbsIni.Read("Options", "NOLocalFR", 1);
                return (k == 1);

            }
        }

        internal static bool USBFR
        {
            get
            {
                int k = AbsIni.Read("Options", "USBFR", 0);
                return (k == 1);

            }
        }
        internal static int USBFRPort
        {
            get
            {
                int k = AbsIni.Read("Options", "USBFRPort", 0);
                return k;

            }
        }
        internal static int USBFRBaudRate
        {
            get
            {
                int k = AbsIni.Read("Options", "USBFRBaudRate", 0);
                return k;

            }
        }

        internal static bool CardIntercepterEnabled
        {
            get
            {
                int k = AbsIni.Read("Options", "CardIntercepterEnabled", 1);
                return (k == 1);

            }
        }


        internal static int RemoteOrderWaterNumber
        {
            get
            {
                int k = AbsIni.Read("Options", "RemoteOrderWaterNumber", 1111);
                return k;

            }
        }

        internal static int LogDeleteDays
        {
            get
            {
                int k = AbsIni.Read("Options", "LogDeleteDays", 5);
                return k;

            }
        }

        

        internal static int FRConnectionType
        {
            get
            {
                int k = AbsIni.Read("Options", "FRConnectionType", 0);
                return k;
            }
        }
        internal static int FRTCPPort
        {
            get
            {
                int k = AbsIni.Read("Options", "FRTCPPort", 7778);
                return k;
            }
        }
        internal static int TCPConnectionTimeout
        {
            get
            {
                int k = AbsIni.Read("Options", "TCPConnectionTimeout", 3000);
                return k;
            }
        }
        internal static string  FRComputerName
        {
            get
            {
                string k = AbsIni.Read("Options", "FRComputerName", "192.168.137.111");
                return k;
            }
        }

        internal static int FRTimeout
        {
            get
            {
                int k = AbsIni.Read("Options", "FRTimeout", 150);
                return k;
            }
        }

        internal static bool FRModeDisabled
        {
            get
            {
                int k = AbsIni.Read("Options", "FRModeDisabled", 0);
                return (k == 1);
            }
        }
        internal static bool FRDiscountMode
        {
            get
            {
                int k = AbsIni.Read("Options", "FRDiscountMode", 1);
                return (k == 1);
            }
        }
        internal static bool FRNoTax
        {
            get
            {
                int k = AbsIni.Read("Options", "FRNoTax", 0);
                return (k == 1);
            }
        }
        internal static bool FRPriceFromDisplay
        {
            get
            {
                int k = AbsIni.Read("Options", "FRPriceFromDisplay", 0);
                return (k == 1);
            }
        }

        internal static bool HamsterDisabled
        {
            get
            {
                int k = AbsIni.Read("Options", "HamsterDisabled", 0);
                return (k == 1);
            }
        }


        internal static int CurentLevelLog
        {
            get
            {
                int k = AbsIni.Read("Options", "CurentLevelLog", 8);
                return k;

            }
        }

        internal static int HotelBreakfastDiscId
        {
            get
            {
                int k = AbsIni.Read("HotelBreakfast", "HotelBreakfastDiscId", 10);
                return k;

            }
        }
        internal static int HotelBreakfastBarCode
        {
            get
            {
                int k = AbsIni.Read("HotelBreakfast", "HotelBreakfastBarCode", 2600);
                return k;

            }
        }
        internal static int HotelBreakfastBarCodeSmall
        {
            get
            {
                int k = AbsIni.Read("HotelBreakfast", "HotelBreakfastBarCodeSmall", 2700);
                return k;

            }
        }
        internal static int HotelBreakfastPaymentId
        {
            get
            {
                int k = AbsIni.Read("HotelBreakfast", "HotelBreakfastPaymentId", 22);
                return k;

            }
        }

        internal static bool RemoteLisenterDisabled
        {
            get
            {
                bool k = Convert.ToBoolean(AbsIni.Read("Options", "RemoteLisenterDisabled", 0));
                return k;

            }
        }

        internal static bool FriendCardsDisabled
        {
            get
            {
                bool k = Convert.ToBoolean(AbsIni.Read("Options", "FriendCardsDisabled", 0));
                return k;

            }
        }

        internal static bool SQLCheckDisabled
        {
            get
            {
                bool k = Convert.ToBoolean(AbsIni.Read("Options", "SQLCheckDisabled", 0));
                return k;

            }
        }

        internal static bool SQLDisabled
        {
            get
            {
                bool k = Convert.ToBoolean(AbsIni.Read("Options", "SQLDisabled", 0));
                return k;

            }
        }


        internal static bool AsincSenderEventDisabled
        {
            get
            {
                bool k = Convert.ToBoolean(  AbsIni.Read("Options", "AsincSenderEventDisabled", 0));
                return k;

            }
        }
        internal static bool MenuSenderDisabled
        {
            get
            {
                bool k = Convert.ToBoolean(AbsIni.Read("Options", "MenuSenderDisabled", 0));
                return k;

            }
        }

        internal static bool MenuSenderNeedSend
        {
            get
            {
                bool k = Convert.ToBoolean(AbsIni.Read("Options", "MenuSenderNeedSend", 0));
                return k;

            }
        }
        

        internal static int MenuSenderNum
        {
            get
            {
                int k = Convert.ToInt32(AbsIni.Read("Options", "MenuSenderNum", 0));
                return k;

            }
        }

        internal static int RemoteOrderTime
        {
            get
            {
                int k = AbsIni.Read("Options", "RemoteOrderTime", 5);
                return k;

            }
        }

        internal static int DishPrintName
        {
            get
            {
                int k = AbsIni.Read("Options", "DishPrintName", 0);
                return k;
            }
        }


        internal static int ShtrihFRType
        {
            get
            {
                //2-для штрих-м
                int k = AbsIni.Read("Options", "ShtrihFRType", 0);
                return k;
            }
        }


        internal static int RemoteOrderTerminalNumber
        {
            get
            {
                int k = AbsIni.Read("Options", "RemoteOrderTerminalNumber", 1);
                return k;

            }
        }


        internal static List<int> DefaultTableForBartender
        {
            get
            {
                try
                {
                    List<int> Tmp = new List<int>();

                    string k = AbsIni.Read("Options", "DefaultTableForBartender", "5");
                    foreach (string s in k.Split(","[0]))
                    {
                        Tmp.Add(Convert.ToInt32(s));
                    }
                    return Tmp;
                }
                catch (Exception e)
                {
                    // Utils.ToLog("Ошибка при чтении номеров столов для удаленного заказа " + e.Message);
                    return null;
                }

            }
        }
        /*
        internal static int DefaultTableForBartender
        {
            get
            {
                int k = AbsIni.Read("Options", "DefaultTableForBartender", 5);
                return k;

            }
        }
         * */
        internal static bool TRPOSXEnables
        {
            get
            {
                int k = AbsIni.Read("Options", "TRPOSX", 0);
                return Convert.ToBoolean(k);

            }
        }
        internal static bool PrintTableDesc
        {
            get
            {
                int k = AbsIni.Read("Options", "PrintTableDesc", 0);
                return Convert.ToBoolean(k);

            }
        }
        internal static bool PrintEnglishItemName
        {
            get
            {
                int k = AbsIni.Read("Options", "PrintEnglishItemName", 0);
                return Convert.ToBoolean(k);

            }
        }
        internal static bool PrintPrecheckOnFR
        {
            get
            {
                int k = AbsIni.Read("Options", "PrintPrecheckOnFR", 0);
                return Convert.ToBoolean(k);

            }
        }
        internal static bool DisableNonWaiterOborot
        {
            get
            {
                int k = AbsIni.Read("Options", "DisableNonWaiterOborot", 0);
                return Convert.ToBoolean(k);
            }
        }
        internal static bool PrintFriendInfoEnabled
        {
            get
            {
                int k = AbsIni.Read("Options", "PrintFriendInfoEnabled", 1);
                return Convert.ToBoolean(k);
            }
        }

        internal static bool MySVEnabled
        {
            get
            {
                int k = AbsIni.Read("Options", "MySVEnabled", 1);
                return Convert.ToBoolean(k);
            }
        }

        internal static bool CreditCloseByWiterEnabled
        {
            get
            {
                int k = AbsIni.Read("Options", "CreditCloseByWiterEnabled", 0);
                return Convert.ToBoolean(k);
            }
        }



        public static bool AlohaFlyExportEnabled
        {
            get
            {
                int k = AbsIni.Read("AlohaFlyExport", "AlohaFlyExportEnabled", 0);
                return Convert.ToBoolean(k);
            }
        }

        public  static string AlohaFlyExportConnectionString
        {
            get
            {
                string k = AbsIni.Read("AlohaFlyExport", "AlohaFlyExportConnectionString", @"https://18.216.147.247/AlohaServiceStaging/AlohaService.svc");
                return k;
            }
        }

        internal static int AlohaFlyExportUserId
        {
            get
            {
                int k = AbsIni.Read("FayRetail", "AlohaFlyExportUserId", 10);
                return k;
            }
        }

        internal static int AlohaFlyExportDefaultAirId
        {
            get
            {
                int k = AbsIni.Read("FayRetail", "AlohaFlyExportDefaultAirId", 32);
                return k;
            }
        }

        
        internal static int AlohaFlyExportPlaceId
        {
            get
            {
                int k = AbsIni.Read("FayRetail", "AlohaFlyExportPlaceId", 1);
                return k;
            }
        }

        internal static int AlohaFlyExportFlightNumberDishId
        {
            get
            {
                int k = AbsIni.Read("FayRetail", "AlohaFlyExportFlightNumberDishId", 999900);
                return k;
            }
        }

        internal static bool FayRetailEnabled
        {
            get
            {
                int k = AbsIni.Read("FayRetail", "FayRetailEnabled", 1);
                return Convert.ToBoolean(k);
            }
        }

        internal static string FayRetailDeviceLogicalID
        {
            get
            {
                string k = AbsIni.Read("FayRetail", "DeviceLogicalID", "T000001");
                return k;
            }
        }

        internal static int FayRetailDiscountId
        {
            get
            {
                int k = AbsIni.Read("FayRetail", "FayRetailDiscountId", 3);
                return k;
            }
        }

        internal static string FayRetailLogin
        {
            get
            {
                string k = AbsIni.Read("FayRetail", "FayRetailLogin", "T000001");
                return k;
            }
        }
        internal static string FayRetailPass
        {
            get
            {
                string k = AbsIni.Read("FayRetail", "FayRetailPass", "654321");
                return k;
            }
        }
        internal static string FayRetailServer
        {
            get
            {
                string k = AbsIni.Read("FayRetail", "FayRetailServer", "http://10.10.0.242:8080/service/exchange/fay_retail");
                return k;
            }
        }

        internal static bool InPasEnabled
        {
            get
            {
                int k = AbsIni.Read("INPAS", "InPasEnabled", 0);
                return Convert.ToBoolean(k);
            }
        }
        internal static string InPasTerminalNum
        {
            get
            {
                string k = AbsIni.Read("INPAS", "InPasTerminalNum", "0");
                return k;
            }
        }
        internal static string CreditTerminalNum
        {
            get
            {
                string k = AbsIni.Read("Options", "CreditTerminalNum", "0");
                return k;
            }
        }

        internal static int CreditTerminalTimeout
        {
            get
            {
                int k = AbsIni.Read("Options", "CreditTerminalTimeout", 60);
                return k;
            }
        }

        internal static bool ExternalInterfaceEnable
        {
            get
            {
                int k = AbsIni.Read("ExternalInterface", "ExternalInterfaceEnable", 0);
                return Convert.ToBoolean(k);
            }
        }


        internal static bool ExternalInterfaceSendItemWhenAdd
        {
            get
            {
                int k = AbsIni.Read("ExternalInterface", "ExternalInterfaceSendItemWhenAdd", 0);
                return Convert.ToBoolean(k);
            }
        }
        internal static int ExternalInterfacePort
        {
            get
            {
                int k = AbsIni.Read("ExternalInterface", "ExternalInterfacePort", 8000);
                return k;
            }
        }
        internal static int ExternalInterfaceJSONPort
        {
            get
            {
                int k = AbsIni.Read("ExternalInterface", "ExternalInterfaceJSONPort", 8001);
                return k;
            }
        }
        internal static string  ExternalInterfaceIp
        {
            get
            {
                string k = AbsIni.Read("ExternalInterface", "ExternalInterfaceIp", "localhost");
                return k;
            }
        }
        internal static int ExternalInterfaceEmployee
        {
            get
            {
                int k = AbsIni.Read("ExternalInterface", "ExternalInterfaceEmployee", 9268);
                return k;
            }
        }


        internal static int ExternalInterfaceManager
        {
            get
            {
                int k = AbsIni.Read("ExternalInterface", "ExternalInterfaceManager", 99922);
                return k;
            }
        }


        internal static int ExternalInterfaceTerminal
        {
            get
            {
                int k = AbsIni.Read("ExternalInterface", "ExternalInterfaceTerminal", AlohaTSClass.GetTermNum());
                return k;
            }
        }

        internal static int ExternalInterfaceRange1BarCode
        {
            get
            {
                int k = AbsIni.Read("ExternalInterface", "ExternalInterfaceRange1BarCode", 999950);
                return k;
            }
        }
        internal static int ExternalInterfaceRange1OrderMode
        {
            get
            {
                int k = AbsIni.Read("ExternalInterface", "ExternalInterfaceRange1OrderMode", 10);
                return k;
            }
        }

        internal static string ExternalInterfaceRange1Name
        {
            get
            {
               string k = AbsIni.Read("ExternalInterface", "ExternalInterfaceRange1Name", "Room Service");
                return k;
            }
        }

        internal static List<int> AddOrderFromExtRange1
        {
            get
            {
                try
                {
                    List<int> Tmp = new List<int>();

                    string k = AbsIni.Read("ExternalInterface", "AddOrderFromExtRange1", "");
                    foreach (string s in k.Split(","[0]))
                    {
                        Tmp.Add(Convert.ToInt32(s));
                    }
                    return Tmp;
                }
                catch (Exception e)
                {
                    // Utils.ToLog("Ошибка при чтении номеров столов для удаленного заказа " + e.Message);
                    return null;
                }

            }
        }

        internal static bool FRSEnabled
        {
            get
            {
                int k = AbsIni.Read("FRS", "FRSEnabled", 1);
                return Convert.ToBoolean(k);
            }
        }

        internal static bool FRSPrintCheck
        {
            get
            {
                int k = AbsIni.Read("FRS", "FRSPrintCheck2", 0);
                return Convert.ToBoolean(k);
            }
        }

        internal static bool XFromGes
        {
            get
            {
                int k = AbsIni.Read("FRS", "XFromGes", 0);
                return Convert.ToBoolean(k);
            }
        }

        internal static bool FRSSaveCheckToImg
        {
            get
            {
                int k = AbsIni.Read("FRS", "FRSSaveCheckToImg", 0);
                return Convert.ToBoolean(k);
            }
        }
        internal static bool FRSMaster
        {
            get
            {
                int k = AbsIni.Read("FRS", "FRSMaster", 0);
                return Convert.ToBoolean(k);
            }
        }
        internal static bool FRSSaveCheckToImg2
        {
            get
            {
                int k = AbsIni.Read("FRS", "FRSSaveCheckToImg2", 0);
                return Convert.ToBoolean(k);
            }
        }
        internal static string FRSSRVPath
        {
            get
            {
                //string k = AbsIni.Read("FRS", "FRSSRVPath", String.Format (@"http://{0}cloud:3838/FRSService/RemoteData",AlohainiFile.TermStr));
                string k = AbsIni.Read("FRS", "FRSSRVPath", "");
                return k;
            }
        }

        internal static string FRSPrinterName
        {
            get
            {
                string k = AbsIni.Read("FRS", "FRSPrinterName", AlohaTSClass.GetCurentPrtName());
                return k;
            }
        }


        internal static string LabelPrinterName
        {
            get
            {
                string k = AbsIni.Read("Options", "LabelPrinterName", "LabelPrinter");
                return k;
            }
        }


        internal static bool ArcusEnabled
        {
            get
            {
                int k = AbsIni.Read("Options", "Arcus", 0);
                return Convert.ToBoolean(k);
            }
        }
        internal static bool Arcus2Enabled
        {
            get
            {
                int k = AbsIni.Read("Options", "Arcus2", 0);
                return Convert.ToBoolean(k);
            }
        }
        internal static bool Arcus4Enabled
        {
            get
            {
                int k = AbsIni.Read("Options", "Arcus4", 0);
                return Convert.ToBoolean(k);
            }
        }
        internal static bool VerifoneEnabled
        {
            get
            {
                int k = AbsIni.Read("Options", "Verifone", 0);
                return Convert.ToBoolean(k);
            }
        }
        internal static bool Arcus2PlasticMomentClose
        {
            get
            {
                int k = AbsIni.Read("Options", "Arcus2PlasticMomentClose", 0);
                return Convert.ToBoolean(k);
            }
        }
        internal static bool Arcus2PlasticLocalClose
        {
            get
            {
                int k = AbsIni.Read("Options", "Arcus2PlasticLocalClose", 0);
                return Convert.ToBoolean(k);
            }
        }
        internal static bool PlasticLocalClose
        {
            get
            {
                int k = AbsIni.Read("Options", "PlasticLocalClose", 0);
                return Convert.ToBoolean(k);
            }
        }


        internal static bool Arcus2PrintSlip
        {
            get
            {
                int k = AbsIni.Read("Options", "Arcus2PrintSlip", 0);
                return Convert.ToBoolean(k);
            }
        }
        internal static bool PlastikPrintSlip
        {
            get
            {
                int k = AbsIni.Read("Options", "PlastikPrintSlip", 0);
                return Convert.ToBoolean(k);
            }
        }
        internal static bool Arcus2PrintOneSlip
        {
            get
            {
                int k = AbsIni.Read("Options", "Arcus2PrintOneSlip", 0);
                return Convert.ToBoolean(k);
            }
        }
        internal static bool Arcus2ShowShortReportBtn
        {
            get
            {
                int k = AbsIni.Read("Options", "Arcus2ShowShortReportBtn", 0);
                return Convert.ToBoolean(k);
            }
        }

        internal static bool Arcus4ShowShortReportBtn
        {
            get
            {
                int k = AbsIni.Read("Options", "Arcus4ShowShortReportBtn", 1);
                return Convert.ToBoolean(k);
            }
        }
        internal static bool VerifoneShowShortReportBtn
        {
            get
            {
                int k = AbsIni.Read("Options", "VerifoneShowShortReportBtn", 1);
                return Convert.ToBoolean(k);
            }
        }
        internal static bool CreditCardEmulatorEnabled
        {
            get
            {
                int k = AbsIni.Read("CreditCardEmulator", "CreditCardEmulatorEnabled", 0);
                return Convert.ToBoolean(k);
            }
        }

        internal static bool SBCreditCardEnabled
        {
            get
            {
                int k = AbsIni.Read("Options", "SBCreditCardEnabled", 0);
                return Convert.ToBoolean(k);
            }
        }
        internal static bool CreditCardSlipPrintPreCheck
        {
            get
            {
                int k = AbsIni.Read("Options", "CreditCardSlipPrintPreCheck", 0);
                return Convert.ToBoolean(k);
            }
        }
        internal static bool CreditCardAutoOpenCheck
        {
            get
            {
                int k = AbsIni.Read("Options", "CreditCardAutoOpenCheck", 1);
                return Convert.ToBoolean(k);
            }
        }
        internal static string ArcusFilesPath
        {
            get
            {
                string k = AbsIni.Read("Options", "ArcusFilesPath", @"C:\Aloha\Check\Arcus2\");
                return k;
            }
        }

        internal static string ArcusChequeFilesPath
        {
            get
            {
                string k = AbsIni.Read("Options", "ArcusChequeFilesPath", @"C:\Aloha\Check\Arcus2\cheq.out");
                return k;
            }
        }

        

        internal static string ArcusCodeFilePath
        {
            get
            {
                string k = AbsIni.Read("Options", "ArcusCodeFilePath", @"C:\Aloha\Check\Arcus2\INI\rc_res.ini");
                return k;
            }
        }


        internal static bool Rounded
        {
            get
            {
                bool k = Convert.ToBoolean(AbsIni.Read("Options", "Rounded", 0));
                return k;
            }
        }


        


        internal static bool DisplayBoardEnabled
        {
            get
            {
                int k = AbsIni.Read("DisplayBoard", "DBEnabled", 0);
                return Convert.ToBoolean(k);
            }
        }
        internal static int DisplayBoardPort
        {
            get
            {
                return AbsIni.Read("DisplayBoard", "DBPort", 1);
            }
        }
        internal static int DisplayBoardCodePage
        {
            get
            {
                return AbsIni.Read("DisplayBoard", "DBCodePage", 866);
            }
        }
        internal static int DisplayBoardSendCodePage
        {
            get
            {
                return AbsIni.Read("DisplayBoard", "DBSendCodePage", 11);
            }
        }
        internal static string DisplayBoardMessageStr1
        {
            get
            {
                return AbsIni.Read("DisplayBoard", "DBMessageStr1", "");
            }
        }
        internal static string DisplayBoardMessageStr2
        {
            get
            {
                return AbsIni.Read("DisplayBoard", "DBMessageStr2", "");
            }
        }
        internal static int DisplayBoardPortBaudRate
        {
            get
            {
                return AbsIni.Read("DisplayBoard", "DBPortBaudRate", 38400);
            }
        }
        internal static int DisplayBoardScreenLenght
        {
            get
            {
                return AbsIni.Read("DisplayBoard", "DBScreenLenght", 20);
            }
        }

        internal static int DisplayBoardType
        {
            get
            {
                return AbsIni.Read("DisplayBoard", "DBType", 1);
            }
        }

        internal static bool FiskalDriverNonShtrih
        {
            get
            {
                int k = AbsIni.Read("FiskalDriver", "FiskalDriverNonShtrih", 0);
                return Convert.ToBoolean(k);
            }
        }
        /*
        internal static bool FiskalDriverNonShtrihAlohaReport
        {
            get
            {
                int k = AbsIni.Read("FiskalDriver", "FiskalDriverNonShtrihAlohaReport", 0);
                return Convert.ToBoolean(k);
            }
        }
         * */
        internal static int FiskalDriverType
        {
            get
            {
                // 1 - Атолл
                return AbsIni.Read("FiskalDriver", "FType", 1);
            }
        }
        internal static int FiskalDriverLUNumber
        {
            get
            {
                // 1 - Атолл
                return AbsIni.Read("FiskalDriver", "LUNumber", 1);
            }
        }

        internal static List<int> FiskalDriverNonSendPayments
        {
            get
            {
                try
                {
                    List<int> Tmp = new List<int>();

                    string k = AbsIni.Read("FiskalDriver", "NonSendPayments", "");
                    foreach (string s in k.Split(","[0]))
                    {
                        Tmp.Add(Convert.ToInt32(s));
                    }
                    return Tmp;
                }
                catch (Exception e)
                {
                   // Utils.ToLog("Ошибка при чтении номеров столов для удаленного заказа " + e.Message);
                    return null;
                }

            }
        }




        internal static List<int> FiskalDriverCashPayments
        {
            get
            {
                try
                {
                    List<int> Tmp = new List<int>();

                    string k = AbsIni.Read("FiskalDriver", "FiskalDriverCashPayments", "");
                    foreach (string s in k.Split(","[0]))
                    {
                        Tmp.Add(Convert.ToInt32(s));
                    }
                    return Tmp;
                }
                catch (Exception e)
                {
                    // Utils.ToLog("Ошибка при чтении номеров столов для удаленного заказа " + e.Message);
                    return new List<int>();
                }

            }
        }
        internal static List<int> FiskalDriverCreditPayments
        {
            get
            {
                try
                {
                    List<int> Tmp = new List<int>();

                    string k = AbsIni.Read("FiskalDriver", "FiskalDriverCreditPayments", "");
                    foreach (string s in k.Split(","[0]))
                    {
                        Tmp.Add(Convert.ToInt32(s));
                    }
                    return Tmp;
                }
                catch (Exception e)
                {
                    // Utils.ToLog("Ошибка при чтении номеров столов для удаленного заказа " + e.Message);
                    return new List<int>();
                }

            }
        }


        /*
        internal static bool RemoteEventEnabled
        {
            get
            {
                int k = AbsIni.Read("RemoteEvent", "RemoteEventEnabled", 0);
                return Convert.ToBoolean(k);
            }
        }
        */


        internal static List<int> RemoteOrderTablesList
        {
            get
            {
                try
                {
                    List<int> Tmp = new List<int>();

                    string k = AbsIni.Read("Options", "RemoteOrderTablesList", "");
                    foreach (string s in k.Split(","[0]))
                    {
                        Tmp.Add(Convert.ToInt32(s));
                    }
                    return Tmp;
                }
                catch (Exception e)
                {
                    Utils.ToLog("Ошибка при чтении номеров столов для удаленного заказа " + e.Message);
                    return null;
                }

            }
        }


        


        internal static List<int> StopListSubMnuNotShower
        {
            get
            {
                try
                {
                    List<int> Tmp = new List<int>();

                    string k = AbsIni.Read("Options", "StopListSubMnuNotShower", "");
                    
                    foreach (string s in k.Split(","[0]))
                    {
                        Tmp.Add(Convert.ToInt32(s));
                    }
                    return Tmp;
                }
                catch (Exception e)
                {
                    Utils.ToLog("[Error] StopListSubMnuNotShower" + e.Message);
                    return null;
                }

            }
        }

        internal static bool RemoteCloseCheckEnabled
        {
            get
            {
                int k = AbsIni.Read("RemoteClose", "RemoteCloseCheckEnabled", 0);
                return Convert.ToBoolean(k);
            }
        }
        internal static int RemoteCloseCheckTerminal
        {
            get
            {
                int k = AbsIni.Read("RemoteClose", "RemoteCloseCheckTerminal", 0);
                return k;
            }
        }


        internal static bool TakeOutEnabled
        {
            get
            {
                int k = AbsIni.Read("TakeOut", "TakeOutEnabled", 0);
                return Convert.ToBoolean(k);
            }
        }

        internal static int TakeOutOrderId1
        {
            get
            {
                int k = AbsIni.Read("TakeOut", "TakeOutOrderId1", -1);
                return Convert.ToInt32(k);
            }
        }
        internal static int TakeOutOrderId2
        {
            get
            {
                int k = AbsIni.Read("TakeOut", "TakeOutOrderId2", -1);
                return Convert.ToInt32(k);
            }
        }

        internal static int TakeOutDiscountId
        {
            get
            {
                int k = AbsIni.Read("TakeOut", "TakeOutDiscountId", 0);
                return Convert.ToInt32(k);
            }
        }

        internal static bool DishConsolidate
        {
            get
            {
                int k = AbsIni.Read("Options", "DishConsolidate", 1);
                return Convert.ToBoolean(k);
            }
        }

        internal static bool InterceptPrint
        {
            get
            {
                int k = AbsIni.Read("Options", "InterceptPrint", 1);
                return Convert.ToBoolean(k);
            }
        }

        internal static int DifferentPredcheckSaleGroupe
        {
            get
            {
                string k = AbsIni.Read("Options", "DifferentPredcheckSaleGroupe", "0");
                //string k = AbsIni.Read("JMS", "Host", @"vfiliasesb0:2506");
                return Convert.ToInt32(k);
            }
        }

        internal static string SpoolPath
        {
            get
            {
                return AbsIni.Read("Spool", "SpoolPath", @"C:\aloha\check\spool" + @"\pl.chk");
            }
        }
        internal static string SpoolDir2
        {
            get
            {
                return AbsIni.Read("Spool", "SpoolPath", @"C:\aloha\check\spool2\" );
            }
        }
        internal static bool SpoolEnabled
        {
            get
            {
                int k = AbsIni.Read("Spool", "SpoolEnabled", 1);
                return Convert.ToBoolean(k);
            }
        }

        internal static bool SpoolTenderConvert
        {
            get
            {
                int k = AbsIni.Read("Spool", "SpoolTenderConvert", 0);
                return Convert.ToBoolean(k);
            }
        }


        internal static int SpoolTenderCash
        {
            get
            {
                int k = AbsIni.Read("Spool", "SpoolTenderCash", 1);
                return Convert.ToInt32(k);
            }
        }

        internal static int SpoolTenderCard
        {
            get
            {
                int k = AbsIni.Read("Spool", "SpoolTenderCard", 20);
                return Convert.ToInt32(k);
            }
        }

        internal static bool SpoolMultiFilesEnabled
        {
            get
            {
                int k = AbsIni.Read("Spool", "SpoolMultiFilesEnabled", 0);
                return Convert.ToBoolean(k);
            }
        }
        internal static bool SpoolOneFileEnabled
        {
            get
            {
                int k = AbsIni.Read("Spool", "SpoolOneFileEnabled", 1);
                return Convert.ToBoolean(k);
            }
        }
        internal static bool SpoolOrderTimeDisable
        {
            get
            {
                int k = AbsIni.Read("Spool", "SpoolOrderTimeDisable", 0);
                return Convert.ToBoolean(k);
            }
        }

        internal static int SpoolDepNum
        {
            get
            {
                int k = AbsIni.Read("Spool", "SpoolDepNum", AlohainiFile.DepNum);
                return k;
            }
        }

        internal static int SpoolMaxDish
        {
            get
            {
                int k = AbsIni.Read("Spool", "SpoolMaxDish", 933000);
                return k;
            }
        }

        internal static int SpoolKassaNum
        {
            get
            {
                int k = AbsIni.Read("Spool", "SpoolKassaNum", -1);
                return k;
            }
        }
        internal static bool SpoolPrintDiscName
        {
            get
            {
                bool k = AbsIni.Read("Spool", "SpoolPrintDiscName", true);
                return k;
            }
        }

        internal static bool  AutoOrderBeforeWaiterClose
        {
            get
            {
                int k = AbsIni.Read("Options", "AutoOrderBeforeWaiterClose", 0);
                return Convert.ToBoolean(k);
            }
        }


        internal static int RemoteCloseEmployee
        {
            get
            {
                int k = AbsIni.Read("Options", "RemoteCloseEmployee", 9269);
                return Convert.ToInt32(k);
            }
        }
        internal static int RemoteCloseTerminal
        {
            get
            {
                int k = AbsIni.Read("Options", "RemoteCloseTerminal", 14);
                return Convert.ToInt32(k);
            }
        }



        internal static bool FCCEnable
        {
            get
            {
                int k = AbsIni.Read("FCC", "FCCEnable", 0);
                return Convert.ToBoolean(k);
            }
        }
        internal static bool FCCCustumerDisplayEnable
        {
            get
            {
                int k = AbsIni.Read("FCC", "FCCCustumerDisplayEnable", 0);
                return Convert.ToBoolean(k);
            }
        }

        internal static bool FCCSync
        {
            get
            {
                int k = AbsIni.Read("FCC", "FCCSync", 1);
                return Convert.ToBoolean(k);
            }
        }

        internal static int FCCCash
        {
            get
            {
                int k = AbsIni.Read("FCC", "FCCCash", 1);
                return Convert.ToInt32(k);
            }
        }
        internal static int FCCGloryCash
        {
            get
            {
                int k = AbsIni.Read("FCC", "FCCGloryCash", 1);
                return Convert.ToInt32(k);
            }
        }
        internal static int FCCEmployee
        {
            get
            {
                int k = AbsIni.Read("FCC", "FCCEmployee", 9268);
                return Convert.ToInt32(k);
            }
        }
        internal static int FCCTerminal
        {
            get
            {
                int k = AbsIni.Read("FCC", "FCCTerminal", 14);
                return Convert.ToInt32(k);
            }
        }

        internal static int FCCLockDish
        {
            get
            {
                int k = AbsIni.Read("FCC", "FCCLockDish", 999001);
                return Convert.ToInt32(k);
            }
        }

        internal static bool FCCZZRepCassetRemove
        {
            get
            {
                int k = AbsIni.Read("FCC", "FCCZZRepCassetRemove", 0);
                return Convert.ToBoolean(k);
            }
        }
        internal static bool FCCNoChangeCashIncome
        {
            get
            {
                int k = AbsIni.Read("FCC", "FCCNoChangeCashIncome", 0);
                return Convert.ToBoolean(k);
            }
        }


        


        internal static bool FCCZRepAutoNullNal
        {
            get
            {
                int k = AbsIni.Read("FCC", "FCCZRepAutoNullNal", 1);
                return Convert.ToBoolean(k);
            }
        }




        internal static bool JMSEnable
        {
            get
            {
                int k = AbsIni.Read("JMS", "JMSEnable", 1);
                return Convert.ToBoolean(k);
            }
        }


        internal static string JMSHost
        {
            get
            {
                string k = AbsIni.Read("JMS", "Host", @"vfiliasesb0");
                //string k = AbsIni.Read("JMS", "Host", @"vfiliasesb0:2506");
                return k;
            }
        }
        internal static int JMSPort
        {
            get
            {
                string k = AbsIni.Read("JMS", "Port", "2506");
                //string k = AbsIni.Read("JMS", "Host", @"vfiliasesb0:2506");
                return Convert.ToInt32(k);
            }
        }
        internal static string JMSLogin
        {
            get
            {
                string k = AbsIni.Read("JMS", "Login", @"Administrator");
                return k;
            }
        }
        internal static string JMSPassword
        {
            get
            {
                string k = AbsIni.Read("JMS", "Password", @"Administrator");
                return k;
            }
        }
        internal static string JMSQueue
        {
            get
            {
                string k = AbsIni.Read("JMS", "Queue", @"GestoriReceiver.Entry");
                return k;
            }
        }
        internal static string JMSService
        {
            get
            {
                string k = AbsIni.Read("JMS", "Service", @"AlohaReceipts");
                return k;
            }
        }

        internal static bool SendToVideoEnable
        {
            get
            {
                int k = AbsIni.Read("SendToVideo", "SendToVideoEnable", 0);
                return Convert.ToBoolean(k);

            }
        }



        internal static bool SendToVideoTCP
        {
            get
            {
                int k = AbsIni.Read("SendToVideo", "SendToVideoTCP", 0);
                return Convert.ToBoolean(k);

            }
        }


        internal static bool SendToVideoEnglish
        {
            get
            {
                int k = AbsIni.Read("SendToVideo", "SendToVideoEnglish", 0);
                return Convert.ToBoolean(k);

            }
        }

        internal static string SendToVideoIP
        {
            get
            {
                string k = AbsIni.Read("SendToVideo", "SendToVideoIP", "192.168.3.37");
                return k;
            }
        }
        internal static int SendToVideoPort
        {
            get
            {
                int k = AbsIni.Read  ("SendToVideo", "SendToVideoPort", 3000);
                return k;
            }
        }
        internal static bool NoReklama
        {
            get
            {
                int k = AbsIni.Read("Options", "NoReklama", 0);
                return Convert.ToBoolean(k);

            }
        }

        internal static bool NonAskRemoteSettings
        {
            get
            {
                int k = AbsIni.Read("Options", "NonAskRemoteSettings", 0);
                return Convert.ToBoolean(k);

            }
        }

        internal static int AkcTest
        {
            get
            {
                int k = AbsIni.Read("Options", "AkcTest", 0);
                return Convert.ToInt32(k);

            }
        }

        internal static bool AskGuestCountOnPreCheck
        {
            get
            {
                bool k = AbsIni.Read("Options", "AskGuestCountOnPreCheck", true);
                return k;

            }
        }
       


        internal static bool CustomerDisplayEnabled
        {
            get
            {
                //2 - Cas AD
                int k = AbsIni.Read("CustomerDisplay", "CustomerDisplayEnabled", 0);
                return Convert.ToBoolean(k);
            }
        }
        internal static int RemoteLisenterCustDispPort
        {
            get
            {
                int k = AbsIni.Read("CustomerDisplay", "CustDispPort", 25252);
                return k;
            }
        }

        internal static string RemoteLisenterCustDispPC
        {
            get
            {
                string k = AbsIni.Read("CustomerDisplay", "CustDispPC", "localhost");
                return k;
            }
        }



        
        internal static int RemoteLisenterPort
        {
            get
            {
                int k = AbsIni.Read("RemoteLisenter", "Port", 9267);
                return k;
            }
        }

       
        internal static int RemoteSenderPort
        {
            get
            {
                int k = AbsIni.Read("RemoteLisenter", "RemoteSenderPort", 9268);
                return k;
            }
        }

        internal static int ScalePort
        {
            get
            {
                int k = AbsIni.Read("Scale", "Port", 2);
                return k;
            }
        }

        internal static int ScaleBaudRate
        {
            get
            {
                int k = AbsIni.Read("Scale", "BaudRate", 9600);
                return k;
            }
        }
        internal static int ScaleTareWeight
        {
            get
            {
                int k = AbsIni.Read("Scale", "TareWeight", 0);
                return k;
            }
        }
        internal static int ScaleType
        {
            get
            {
                
                //2 - Cas ED
                int k = AbsIni.Read("Scale", "Type", 2);
                return k;
            }
        }

        internal static bool GalleryOrderEnabled
        {
            get
            {
                //2 - Cas AD
                int k = AbsIni.Read("Gallery", "GalleryOrderEnabled", 0);
                return Convert.ToBoolean(k);
            }
        }
        internal static bool GalleryCloseCheckEnabled
        {
            get
            {
                //2 - Cas AD
                int k = AbsIni.Read("Gallery", "GalleryCloseCheckEnabled", 0);
                return Convert.ToBoolean(k);
            }
        }
        internal static string GallerySqlServerName
        {
            get
            {
                //2 - Cas AD
                string k = AbsIni.Read("Gallery", "SqlServerName", "xxx");
                return k;
            }
        }


    }

    static internal class DownTimeiniFile
    {
        static AbsiniFile AbsIni;

        static internal bool FileIsPresent
        {
            get
            {
                return File.Exists(Utils.DownTimeIniFile);
            }
        }

        static DownTimeiniFile()
        {


        }


        internal static int GetMaster()
        {
            try
            {
                if (FileIsPresent)
                {
                    AbsIni = new AbsiniFile(Utils.DownTimeIniFile);
                    return AbsIni.Read("Ibertech", "LastMasterID", -1);
                }
                else
                {
                    return -1;
                }
            }
            catch
            {

                return -1;
            }
        }
    }

}

