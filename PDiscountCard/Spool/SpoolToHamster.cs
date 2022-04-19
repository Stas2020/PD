using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;



namespace PDiscountCard.Spool
{
    public class SpoolToHamster
    {

        static string ChecksPath = @"C:\aloha\check\discount\tmp\check";

        static public void GenerateHamster()
        {
            WriteAllChecks(GetHamsterForSpool());
        }


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


        public static void GetSpoolSumm(DateTime BD)
        {
            decimal Nal = 0;
            decimal Plast = 0;
            decimal Other = 0;
            decimal VPlast = 0;
            decimal VNal = 0;
            decimal VOther = 0;
            string SpoolDirPath = @"C:\Aloha\check\Arhiv";
            DirectoryInfo SpoolDir = new DirectoryInfo(SpoolDirPath);
        //    foreach (DirectoryInfo Di in SpoolDir.GetDirectories())
            {
                foreach (FileInfo Fi in SpoolDir.GetFiles())
                {
                    try
                    {
                        StreamReader SW = new StreamReader(Fi.FullName);
                        Check Ch = new Check();
                        DateTime LastBD = new DateTime();
                        while (!SW.EndOfStream)
                        {
                            string s = SW.ReadLine();
                            if (s.Length < 2)
                            {
                                continue;
                            }
                            if (s.Substring(0, 2) == "01")
                            {
                                s = s.Substring(2, s.Length - 2);
                                s = s.Substring(12, s.Length - 12);
                                Ch = new Check();

                                Ch.Dishez = new List<Dish>();
                                if (s.Substring(0, 6) != "      ")
                                {
                                    Ch.DiscountMGR_NUMBER = Convert.ToInt32(s.Substring(0, 6));
                                }
                                s = s.Substring(6, s.Length - 6);
                                if (s.Substring(0, 6) != "      ")
                                {
                                    Ch.CompId = Convert.ToInt32(s.Substring(0, 6));
                                }
                                s = s.Substring(6, s.Length - 6);

                                int dd = Convert.ToInt32(s.Substring(0, 2));
                                int mm = Convert.ToInt32(s.Substring(2, 2));
                                int yy = Convert.ToInt32(s.Substring(4, 2));
                                //Chk.SystemDate = new Chk.SystemDate();

                                s = s.Substring(6, s.Length - 6);

                                int waiter = Convert.ToInt32(s.Substring(0, 4));

                                if (Ch.CompId >= 10)
                                {
                                    Ch.DegustationMGR_NUMBER = waiter;

                                }
                                else
                                {
                                    Ch.Waiter = waiter;
                                }
                                s = s.Substring(4, s.Length - 4);

                                s = s.Substring(3, s.Length - 3);

                                Ch.Cassir = Convert.ToInt32(s.Substring(0, 4));
                                s = s.Substring(4, s.Length - 4);

                                s = s.Substring(2, s.Length - 2);

                                int vozvr = Convert.ToInt32(s.Substring(0, 2));

                                Ch.Vozvr = (vozvr == 1);
                                s = s.Substring(2, s.Length - 2);

                                int hh = Convert.ToInt32(s.Substring(0, 2));
                                int minute = Convert.ToInt32(s.Substring(3, 2));
                                s = s.Substring(5, s.Length - 5);
                                s = s.Substring(29, s.Length - 29);
                                int dd2 = Convert.ToInt32(s.Substring(0, 2));
                                int mm2 = Convert.ToInt32(s.Substring(2, 2));
                                int yy2 = Convert.ToInt32(s.Substring(4, 2))+2000;
                                LastBD = new DateTime(yy2, mm2, dd2);


                            }
                            else if (s.Substring(0, 2) == "03")
                            {
                                s = s.Substring(19, s.Length - 19);

                                Ch.Summ = Convert.ToDecimal(s.Substring(0, 12)) / 100;
                            }
                            else if (s.Substring(0, 2) == "04")
                            {
                                if (Ch == null)
                                {
                                    continue;
                                }
                                if (LastBD != BD)
                                {
                                    continue;
                                }
                                s = s.Substring(2, s.Length - 2);
                                int Pt = Convert.ToInt32(s.Substring(0, 2));
                                s = s.Substring(2, s.Length - 2);
                                
                                decimal Summ = Convert.ToDecimal(s.Substring (0,28))/100m;
                                if (Pt == 1)
                                {
                                    if (Summ > 0)
                                    {
                                        Nal += Summ;
                                    }
                                    else
                                    {
                                        VNal += Summ;
                                    }
                                }
                                else if (Pt == 20)
                                {
                                    if (Summ > 0)
                                    {
                                        Plast  += Summ;
                                    }
                                    else
                                    {
                                        VPlast += Summ;
                                    }
                                }
                                else 
                                {
                                    if (Summ > 0)
                                    {
                                        Other += Summ;
                                    }
                                    else
                                    {
                                        VOther += Summ;
                                    }
                                }
                                
                            //    Tmp.Checks.Add(Ch);
                                Ch = null;

                            }
                        }
                        SW.Close();
                    }
                    catch (Exception e)
                    {
                        System.Windows.Forms.MessageBox.Show("Error read ll " + e.Message);
                    }

                }
            }
            string Str = String.Format("Нал: {0}", Nal) + Environment.NewLine;
            Str += String.Format("ВНал: {0}", VNal) + Environment.NewLine;
            Str += String.Format("Пласт: {0}", Plast) + Environment.NewLine;
            Str += String.Format("ВПласт: {0}", VPlast) + Environment.NewLine;
            Str += String.Format("Other: {0}", Other) + Environment.NewLine;
            Str += String.Format("VOther: {0}", VOther) + Environment.NewLine;

            System.Windows.Forms.MessageBox.Show(Str);
        }


         public static AllChecks GetHamsterForSpool()
        {
            AllChecks Tmp = new AllChecks();
            StreamReader SW = new StreamReader(iniFile.SpoolPath);
            Check Ch = new Check();
            while (!SW.EndOfStream)
            {
                string s = SW.ReadLine();
                if (s.Length < 2)
                {
                    continue;
                }
                if (s.Substring(0, 2) == "01")
                {
                    s = s.Substring(2, s.Length - 2);
                    s = s.Substring(12, s.Length - 12);
                    Ch = new Check();
                    Ch.Dishez = new List<Dish>();
                    if (s.Substring(0, 6) != "      ")
                    {
                        Ch.DiscountMGR_NUMBER = Convert.ToInt32(s.Substring(0, 6));
                    }
                    s = s.Substring(6, s.Length - 6);
                    if (s.Substring(0, 6) != "      ")
                    {
                        Ch.CompId = Convert.ToInt32(s.Substring(0, 6));
                    }
                    s = s.Substring(6, s.Length - 6);

                    int dd = Convert.ToInt32(s.Substring(0, 2));
                    int mm = Convert.ToInt32(s.Substring(2, 2));
                    int yy = Convert.ToInt32(s.Substring(4, 2));
                    //Chk.SystemDate = new Chk.SystemDate();

                    s = s.Substring(6, s.Length - 6);

                    int waiter = Convert.ToInt32(s.Substring(0, 4));

                    if (Ch.CompId >= 10)
                    {
                        Ch.DegustationMGR_NUMBER = waiter;

                    }
                    else
                    {
                        Ch.Waiter = waiter;
                    }
                    s = s.Substring(4, s.Length - 4);

                    s = s.Substring(3, s.Length - 3);

                    Ch.Cassir = Convert.ToInt32(s.Substring(0, 4));
                    s = s.Substring(4, s.Length - 4);

                    s = s.Substring(2, s.Length - 2);

                    int vozvr = Convert.ToInt32(s.Substring(0, 2));

                    Ch.Vozvr = (vozvr == 1);
                    s = s.Substring(2, s.Length - 2);

                    int hh = Convert.ToInt32(s.Substring(0, 2));
                    int minute = Convert.ToInt32(s.Substring(3, 2));
                    s = s.Substring(5, s.Length - 5);
                }
                else if (s.Substring(0, 2) == "03")
                {
                    s = s.Substring(19, s.Length - 19);

                    Ch.Summ = Convert.ToDecimal(s.Substring(0, 12)) / 100;
                }
                else if (s.Substring(0, 2) == "04")
                {
                    if (Ch == null)
                    {
                        continue;
                    }
                    s = s.Substring(2, s.Length - 2);
                    int Pt = Convert.ToInt32(s.Substring(0, 2));
                    /*
                    if (Pt == 1)
                    {
                        Ch.Tender = TenderType.Cash;
                        Ch.IsNal = true;

                    }
                    else if (Pt == 20)
                    {
                        Ch.Tender = TenderType.CreditCard;
                        Ch.IsNal = false;
                    }
                     * */
                    Tmp.Checks.Add(Ch);
                    Ch = null;

                }
            }

            SW.Close();
            return Tmp;

        }

        public static void ConvertLLOTGluk()
        {
            string Dpath = @"C:\lls\in\";
            string DOutPath = @"C:\lls\out\";


            DirectoryInfo di = new DirectoryInfo(Dpath);
            Dictionary<int, int> Prices = new Dictionary<int, int>();
            foreach (DirectoryInfo Di2 in di.GetDirectories())
            {
                DirectoryInfo diOut1 = new DirectoryInfo(DOutPath + Di2.Name);
                if (!diOut1.Exists)
                {
                    diOut1.Create();

                }
                foreach (DirectoryInfo Di3 in Di2.GetDirectories())
                {
                    DirectoryInfo diOut = new DirectoryInfo(diOut1 + @"\" + Di3.Name);
                    if (!diOut.Exists)
                    {
                        diOut.Create();
                    }

                    foreach (FileInfo fi in Di3.GetFiles())
                    {
                        using (StreamReader Sr = new StreamReader(fi.FullName))
                        {
                            using (StreamWriter Sw = new StreamWriter(diOut.FullName + @"\" + fi.Name))
                            {
                                while (!Sr.EndOfStream)
                                {
                                    try
                                    {
                                        string s = Sr.ReadLine();
                                        string OutS = s;
                                        if (s.Substring(0, 2) == "12")
                                        {
                                            int Bc = Convert.ToInt32(s.Substring(14, 6));
                                            int Pr = Convert.ToInt32(s.Substring(53, 8));
                                            int PrOut = 0;
                                            if (Prices.TryGetValue(Bc, out PrOut))
                                            {
                                                Prices[Bc] = Pr;
                                            }
                                            else
                                            {
                                                Prices.Add(Bc, Pr);
                                            }
                                        }
                                        if (s.Substring(0, 2) == "44")
                                        {
                                            int Bc = Convert.ToInt32(s.Substring(41, 6));
                                            int PrOut = 0;
                                            if (!Prices.TryGetValue(Bc, out PrOut))
                                            {
                                                PrOut = 100;
                                            }
                                            OutS = s.Substring(0, 47) + PrOut.ToString("000000000000") + s.Substring(59);

                                        }
                                        Sw.WriteLine(OutS);
                                    }
                                    catch { }
                                }
                            }
                        }
                    }
                }
            }
        }
        public static void RegenOT()
        {
            string SpoolsDir = @"C:\aloha\check\allSpools";
            DirectoryInfo di = new DirectoryInfo(SpoolsDir);
            //string OutSpoolName = @"C:\aloha\check\OutOtSpool";
            string OutSpoolName = @"C:\boffice\Term2\IN\ll010416.3";
            StreamWriter sw = new StreamWriter(OutSpoolName);
            DateTime CurentOpenTime = new DateTime(2016, 03, 01);


            string CoffeeBars = "821114,1811,821397,1898,1449,823025,1658,822206,3,822207,822091,822180,1560,1896,822209,822194,1474,825030,825005,1897,822065,822136,821228,821400,1739,823017,1439,822173,822211,1667,338,822224,1473,822223,822221,822204,1366,822215,38,822212,824190,1472,822183,821282,821395,822225,822208,821170,821398,822110,821351,822090,822214,822228,822213,352,822062,1491,821207,1440,821375,822088,822146,822171,822167,17,1401,822178,822196,18,822131,696,821213,1562,822095,1758,823019,823020,1479,822130,1438,822222,821200,337,1418,1411,1809,821206,1481,822210,1659,822218,822226,1660,347,6,822044,822177,1661,822170,823018,694,822179,822220,822195,1400,822217,1419,1911,823024,822227,114,1563,822229,824186,822094,2,13,822087,1757,1556,821146,1666,1945,822219,822216,821399,24,1738,825007,1480,822153,822145,14,822164,25,1810,823023";
            List<int> CoffeeBarsLst = CoffeeBars.Split(","[0]).Select(a => int.Parse(a)).ToList();
            int CofeeCount = 0;

            List<string> CurenD = new List<string>();

            int StartNetNum = 3;


            for (int NetNum = StartNetNum; NetNum < 11; NetNum++)
            {
                string NetPath = @"\\neglinnaya" + NetNum + @"\BOOTDRV\check\ARHIV";

                di = new DirectoryInfo(NetPath);
                if (!di.Exists)
                {
                    continue;
                }
                foreach (FileInfo fi in di.GetFiles())
                {
                    if (fi.CreationTime < new DateTime(2016, 03, 01))
                    {
                        continue;
                    }
                    using (StreamReader sr = new StreamReader(fi.FullName))
                    {
                        while (!sr.EndOfStream)
                        {
                            string s = sr.ReadLine();
                            if (s.Length < 2)
                            {
                                continue;
                            }

                            if (s.Substring(0, 2) == "01")
                            {
                                String dtstr = s.Substring(113, 11);
                                //   System.Windows.Forms.MessageBox.Show(dtstr);
                                CurentOpenTime = new DateTime(2000 + int.Parse(dtstr.Substring(4, 2)), int.Parse(dtstr.Substring(2, 2))
                                , int.Parse(dtstr.Substring(0, 2)), int.Parse(dtstr.Substring(6, 2)), int.Parse(dtstr.Substring(9, 2)), 0);
                                //System.Windows.Forms.MessageBox.Show(CurentOpenTime.ToString());

                                CurenD = new List<string>();
                            }
                            else if (s.Substring(0, 2) == "44")
                            {
                                String dtstr = s.Substring(2, 13);
                                DateTime SelfCurentTime = new DateTime(int.Parse(dtstr.Substring(4, 4)), int.Parse(dtstr.Substring(2, 2))
                                , int.Parse(dtstr.Substring(0, 2)), int.Parse(dtstr.Substring(8, 2)), int.Parse(dtstr.Substring(11, 2)), 0);

                                string NewS = "";
                                if (SelfCurentTime < CurentOpenTime)
                                {
                                    NewS = "44" + CurentOpenTime.ToString("ddMMyyyyHH:mm") + s.Substring(15, s.Length - 15);
                                }
                                else
                                {
                                    NewS = "44" + SelfCurentTime.ToString("ddMMyyyyHH:mm") + s.Substring(15, s.Length - 15);
                                    //NewS = s;
                                }



                                CurenD.Add(NewS);
                                // sw.WriteLine(NewS);
                            }
                            else if (s.Substring(0, 2) == "03")
                            {
                                int NewPr = 1;
                                try
                                {
                                    long Price = long.Parse(s.Substring(24, 7));
                                    if (Price == 0)
                                    {
                                        NewPr = 0;
                                    }
                                    else if (Price < 0)
                                    {
                                        NewPr = -1;
                                    }

                                }
                                catch (Exception ee)
                                {
                                    Utils.ToLog(ee.Message);
                                }

                                foreach (string ss in CurenD)
                                {
                                    string NewS = ss;
                                    if (NewPr == 0)
                                    {
                                        NewS = ss.Substring(0, 52) + "      0" + ss.Substring(59, ss.Length - 59);
                                    }
                                    else if (NewPr == -1)
                                    {
                                        //long oldPr = long.Parse(ss.Substring(0, 52) )
                                    }

                                    int Bc = int.Parse(ss.Substring(41, 6));
                                    if (CoffeeBarsLst.Contains(Bc) && (NewPr == 1))
                                    {
                                        CofeeCount++;
                                    }

                                    sw.WriteLine(NewS);
                                }
                                CurenD.Clear();
                            }
                        }
                    }
                }
            }
            sw.Close();
        }

    }




}
