using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;


namespace PDiscountCard
{
    public static class Arcus2DataFromXML
    {

        public  static string PrintShortReport()
        {
            Utils.ToCardLog("Печать краткого отчета");
            ArcusSlips AS = ArcusAlohaIntegrator.ReadArcusSlips();
            decimal Card = 0;
            int CardCount = 0;
            decimal Vozvr = 0;
            int VozvrCount = 0;


            foreach (ArcusSlip s in AS.Slips)
            {
                if (!s.Void)
                {
                    Card += s.Sum / 100;
                    CardCount++;
                }
                else
                {
                    Vozvr += s.Sum / 100;
                    VozvrCount++;
                }
            }
            List<string> ReportStrings = new List<string>();

            ReportStrings.Add("ОБЩИЙ ОТЧЕТ &&  " + DateTime.Now.ToString("dd/MM/yyyy"));
            ReportStrings.Add("ПО ЭМИТЕНТАМ &&  " + DateTime.Now.ToString("HH:mm"));
            ReportStrings.Add("   ");
            ReportStrings.Add("ОПЛАТ && " + CardCount + "   " + "RUR " + Card.ToString("0.00"));
            ReportStrings.Add("   ");
            ReportStrings.Add("ВОЗВРАТОВ && " + VozvrCount + "   " + "RUR " + Vozvr.ToString("0.00"));
            ReportStrings.Add("   ");
            ReportStrings.Add("ИТОГО && " + (CardCount - VozvrCount) + "   " + "RUR " + (Card - Vozvr).ToString("0.00"));
            ReportStrings.Add("   ");
            ReportStrings.Add("   ");
            string OutStr = "";
            foreach (String s in ReportStrings)
            {
                OutStr += s + " " + char.ConvertFromUtf32(10)[0];
            }
            Utils.ToCardLog("ReportStrings" + OutStr + char.ConvertFromUtf32(31)[0]);
            //ToShtrih.Init();
            //ToShtrih.Conn();
            return (OutStr + char.ConvertFromUtf32(31)[0]);


        }
        internal static void SlipFileMove()
        {
            try
            {
                string SlipFilePath = iniFile.ArcusFilesPath + "slips.xml";
                string NewSlipFilePathFolderPath = Utils.FilesPath + @"ArcusSlips\";
                string NewSlipFileName = DateTime.Now.ToString("ddMMyyHHmmss") + "slips.xml";
                if (File.Exists(SlipFilePath))
                {
                    if (!Directory.Exists(NewSlipFilePathFolderPath))
                    {
                        Directory.CreateDirectory(NewSlipFilePathFolderPath);
                    }
                    File.Move(SlipFilePath, NewSlipFilePathFolderPath + NewSlipFileName);
                    Utils.ToLog("Файл Slips.xml перенесен.");
                }

            }
            catch (Exception e)
            {
                Utils.ToLog("[Error]Файл Slips.xml не перенесен. " + e.Message);
            }
        }
        public static void GetSipCopy(int Num)
        {
            ArcusSlip S = GetSlip(Num);
            if (S == null)
            {
                //RunOperationAsincComplitedVoid("0", "Неверный номер слипа", "", "");
                CreditCardAlohaIntegration.ShowMsg("Неверный номер слипа");
                //PrintSlip(strslip);
            }
            else
            {
                string strslip = "Копия " + Environment.NewLine;
                foreach (string s in S.Slip)
                {
                    if (s == "")
                    {
                        strslip += " " + Environment.NewLine;
                    }
                    else
                    {
                        strslip += s + Environment.NewLine;
                    }
                }

                //RunOperationAsincComplitedVoid("0", "Успешно", "", strslip);
                PrintSlip(strslip);
            }


        }


        static private void PrintSlip(string Slip)
        {

            if (iniFile.CreditCardSlipPrintPreCheck)
            {
                try
                {
                    // string[] stringSeparators = new string[] { "\n\r", "\n\n", Environment.NewLine};

                    string[] stringSeparators = new string[] { "\n" };

                    string sres = Slip.Replace("\r", "");

                    AlohaTSClass.PrintCardSlip(sres.Split(stringSeparators, StringSplitOptions.None).ToList());
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


        /*
        static private void PrintSlip(string Slip)
        {




            Slip += Convert.ToChar(31);
            //ToShtrih.Conn();
            ToShtrih.PrintCardCheck(Slip);
        }
        */
        private static ArcusSlip GetSlip(int Num)
        {
            ArcusSlips AS = ReadArcusSlips();
            foreach (ArcusSlip S in AS.Slips)
            {
                if (S.Num == Num)
                {
                    return S;
                }
            }
            return null;
        }

        public  static ArcusSlips ReadArcusSlips()
        {
            string FileName = iniFile.ArcusFilesPath + "Slips.xml";
            if (!File.Exists(FileName))
            {
                Utils.ToCardLog("Отсутствует файл слипов. " + FileName);
                return new ArcusSlips();

            }
            XmlReader XR = new XmlTextReader(FileName);
            try
            {
                //ToLog("[ReadVisitsFromFile] Читаю из файла " + Fi.FullName + "информацию о карте");

                XmlSerializer XS = new XmlSerializer(typeof(ArcusSlips));
                //XmlSerializer XS = new XmlSerializer(typeof(CardMooverInfo));
                ArcusSlips CMI = (ArcusSlips)XS.Deserialize(XR);
                XR.Close();
                return CMI;
            }
            catch
            {
                XR.Close();
                return new ArcusSlips();
            }
        }

        internal static void WriteArcusSlips(ArcusSlips AllSlips)
        {
            try
            {
                if (!Directory.Exists(iniFile.ArcusFilesPath))
                {
                    Directory.CreateDirectory(iniFile.ArcusFilesPath);
                }
                XmlWriter XWriter = new XmlTextWriter(iniFile.ArcusFilesPath + @"\Slips.xml", System.Text.Encoding.UTF8);

                XmlSerializer XS = new XmlSerializer(typeof(ArcusSlips));

                XS.Serialize(XWriter, AllSlips);
                XWriter.Close();
            }
            catch
            {
            }
        }
    }
}
