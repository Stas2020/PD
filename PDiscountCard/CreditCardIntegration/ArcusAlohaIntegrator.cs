using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Linq;


namespace PDiscountCard
{
    static  class ArcusAlohaIntegrator
    {
        internal static bool Init(out string Ex)
        {
            Ex = "";
            try
            {
                string OutStr = "";
         
                ArcusClass.Init(); 
                ArcusClass.RunOperationAsincComplited += new ArcusClass.RunOperationAsincDelegate(ArcusClass_RunOperationAsincComplited);
                    return true;
         
            }
            catch (Exception e)
            {
                Ex = e.Message;
                return false;
            }
        }
        
        delegate void ShowWindowDelegate(TrPosXClass.ScreenParamsOut Params);
        private struct AddResponseParams
        {
            public string  res;
            public string resStr;
            public string Response;
            public string Receipt;
        }

        delegate void AddResponseToTextBoxDelegate(AddResponseParams Params);
        static private void AddResponseToTextBox(AddResponseParams Params)
        {
            // TrPosXMessageForm.ShowDialog();
            AddResponseParams Arp = (AddResponseParams)Params;

            TrPosXMessageForm.UpdateMess(Arp.resStr + Environment.NewLine);
            if (Arp.Receipt != "")
            {
                Button1State = 1;

                TrPosXMessageForm.button1.Text = "Распечатать слип";
                TrPosXMessageForm.button1.Visible = true;
            }
            else
            {
                Button1State = 2;
                TrPosXMessageForm.AddMess("Нет данных для печати" + Environment.NewLine);
                TrPosXMessageForm.button1.Text = "Закрыть";
                TrPosXMessageForm.button1.Visible = true;
            }
        }
        static  int Button1State = 0;
 
        static AddResponseParams Arp;

        static void ArcusClass_RunOperationAsincComplited(string  res, string resStr, string Response, string Receipt)
        {

            if (TrPosXMessageForm != null)
            {
                //AddResponseParams Arp = new AddResponseParams();
                if (res == "999")
                {
                    resStr += Environment.NewLine + "Нет связи с терминалом пластиковых карт.";
                    resStr += Environment.NewLine + "Проверьте соединение.";
                }
                

                Arp.Receipt = Receipt;
                Arp.res = res;
                Arp.Response = Response;
                Arp.resStr = resStr;

                

                object[] Params = new object[1];
                Params[0] = Arp;

                
                
              TrPosXMessageForm.BeginInvoke(new AddResponseToTextBoxDelegate(AddResponseToTextBox), Params);
                /*
                    if (GetParam("MessageID", Arp.Response) == "PUR")
                    {
                        if (GetParam("Approve", Arp.Response) == "Y")
                        {
                            CloseCheck.InkreasePlastNum();
                        }
                    }
                
                */



            }
        }


        static FTrposxRunComplited TrPosXMessageForm;
        internal static bool RunOper(Check Ch, int LastTr, bool AllChecks, FiskInfo mfi, int PaymentId)
        {
            string inStr = "";
            decimal Sum = 0;
            if (AllChecks)
            {
                foreach (Check Ch2 in Ch.ChecksOnTable)
                {
                    if (Ch2.AlohaCheckNum != Ch.AlohaCheckNum)
                    {
                        AlohaTSClass.ApplyCardPayment(Ch2.AlohaCheckNum, Ch2.Summ);
                        AlohaTSClass.SetPaymentAttr(Ch2.AlohaCheckNum, LastTr);
                    }
                    Sum += Ch2.Summ;
                }
            }
            else
            {
                Sum = Ch.Summ;
            }
            Utils.ToCardLog("Arcus Транзакция старт " + inStr);
            try
            {
                if (TrPosXMessageForm != null)
                {
                    TrPosXMessageForm.Close();
                }
            }
            catch (Exception e)
            {
                Utils.ToCardLog("[Error] При попытке закрыть старое окно. " + e.Message);
                    try
                {
                TrPosXMessageForm.Dispose();
                }
                catch
                {}
            }
            
            TrPosXMessageForm = new FTrposxRunComplited();
            //Button1State = 0;
            TrPosXMessageForm.button2.Text = "Свернуть";
            TrPosXMessageForm.button1.Visible = false;
            TrPosXMessageForm.button2.Visible = true;
            TrPosXMessageForm.button3.Visible = false;
            TrPosXMessageForm.Init("Операция на терминале пластиковых карт.", "");
            TrPosXMessageForm.button1.Click += new EventHandler(button1_Click);
            TrPosXMessageForm.button2.Click += new EventHandler(button2_Click);

            //TrPosXClass.RunOperationAsinc(inStr);
            if (!Ch.Vozvr)
            {

                ArcusClass.RunPaymentAsinc(Sum); 
            }
            else
            {
                ArcusClass.RunVozvrAsinc(Sum);
            }
            
            TrPosXMessageForm.TopMost = true;
            TrPosXMessageForm.Show();

            return true;
        }
        static void button2_Click(object sender, EventArgs e)
        {
            //TrPosXMessageForm.ToFone();
            ((FTrposxRunComplited)((System.Windows.Forms.Button)sender).Tag).ToFone();
        }
        /*
        static private void PrintSlip(string Slip)
        {
            Slip += Convert.ToChar(31);
            if (iniFile.RemoteCloseCheckEnabled)
            { 
                bool Ok=true;
                RemoteCommands.RemoteSender.SendPrintSlipTCP(Slip, out Ok);
            }
            else
            {
                //ToShtrih.Conn();
                ToShtrih.PrintCardCheck(Slip);
            }
        }
        */

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



        static void button1_Click(object sender, EventArgs e)
        {
            Utils.ToLog("Нажал на кнопку "+ Button1State.ToString());
            if (Button1State == 0)
            {
                //TrPosXMessageForm.Close();
            }
            else if (Button1State == 1)
            {
                PrintSlip(Arp.Receipt);
                
                //TrPosXMessageForm.Close();
            }
            else if (Button1State == 2)
            {
                //TrPosXMessageForm.Close();

            }
            else if (Button1State == 3)
            {
                //TrPosXMessageForm.Close();
                
            }

            try
            {
                ((FTrposxRunComplited)((System.Windows.Forms.Button)sender).Tag).Close();
                MainClass.PlasticTransactionInProcess = false;
            }
            catch
            { }
        }
        static public void SverkaWithQ()
        {
            QSverkaForm F1 = new QSverkaForm();
            F1.ShowDialog();
            if (!F1.IsGood)
            {
                return;
            }
            else
            {
                if (Sverka())
                {
                    PDiscountCard.CloseCheck.ZeroPlastNum();
                    FiskInfo fi = PDiscountCard.CloseCheck.ReadFiskInfo();
                    fi.NeedSverka = false;
                    PDiscountCard.CloseCheck.WriteFiskInfo(fi);
                }

            }

        }
        static public bool Sverka()
        {
            Utils.ToLog("Запуск сверки для Аркуса");
            /*
            
             * */
            string Rec="";
            string ResOper = "";
            string  i = ArcusClass.RunSVERKARepSinc(out Rec, out ResOper);
            PrintSlip(Rec);
            int resWin = 0;
            while ((i != "00") && (i != "000") && (resWin == 0))
            {
                Button1State = 3;
                TrPosXMessageForm = new FTrposxRunComplited();
                TrPosXMessageForm.button1.Visible = true ;
                TrPosXMessageForm.button2.Visible = false;
                TrPosXMessageForm.button3.Visible = true ;
                TrPosXMessageForm.Init("Печать сверки с терминала пластиковых карт.", "Неудачный результат сверки. " + Environment.NewLine + ResOper);
                TrPosXMessageForm.button1.Text = "Повторить сверку";
                TrPosXMessageForm.button1.Click += new EventHandler(button1_Click);
                TrPosXMessageForm.button3.Text = "Закрыть";
                TrPosXMessageForm.button3.Click += new EventHandler(button3_Click);
                
                TrPosXMessageForm.TopMost = true;
                TrPosXMessageForm.ShowDialog();
                resWin = Convert.ToInt32(TrPosXMessageForm.Cancel);
                Button1State = 1;
                if (resWin==0)
                {
                    i = ArcusClass.RunSVERKARepSinc(out Rec, out ResOper);
                    PrintSlip(Rec);
                }
            }
            return ((i == "00") || (i == "000"));
        }

        static void button3_Click(object sender, EventArgs e)
        {
            try
            {
                ((FTrposxRunComplited)((System.Windows.Forms.Button)sender).Tag).Cancel =true;
                ((FTrposxRunComplited)((System.Windows.Forms.Button)sender).Tag).Close();
            }
            catch
            { }
        }

        static public bool XReport()
        {
            if (MainClass.PlastikActivateResult.Result )
            {
                TrPosXMessageForm = new FTrposxRunComplited();
                TrPosXMessageForm.button1.Visible = false;
                TrPosXMessageForm.button2.Visible = false;
                TrPosXMessageForm.button3.Visible = false;
                TrPosXMessageForm.Init("Печать отчета с терминала пластиковых карт.", "");
                TrPosXMessageForm.button1.Click += new EventHandler(button1_Click);
                TrPosXMessageForm.button2.Click += new EventHandler(button2_Click);
                TrPosXMessageForm.TopMost = true;
                TrPosXMessageForm.Show();
                ArcusClass.RunXRepAsinc();
                return true;
            }
            else
            {
                AlohaTSClass.ShowMessage(MainClass.PlastikActivateResult.Comment );
                return false;
            }
            
        }

        public static void GetSipCopy()
        {
            CopySlipFrm CSF = new CopySlipFrm();
            CSF.Arcus = true;
            CSF.ShowDialog();

        }
        public static void GetSipCopy(int Num)
        {


            TrPosXMessageForm = new FTrposxRunComplited();
            TrPosXMessageForm.button1.Visible = false;
            TrPosXMessageForm.button2.Visible = false;
            TrPosXMessageForm.button3.Visible = false;
            TrPosXMessageForm.Init("Печать копии слипа с терминала пластиковых карт.", "");
            TrPosXMessageForm.button1.Click += new EventHandler(button1_Click);
            TrPosXMessageForm.button2.Click += new EventHandler(button2_Click);
            TrPosXMessageForm.TopMost = true;
            TrPosXMessageForm.Show();
            ArcusClass.GetSipCopy(Num);
            

        }

        static  private bool IsVisa(List<string> Slip)
        {
            foreach (string s in Slip)
            {
                if (s.Contains("Карта:VISA"))
                {
                    return true;
                }
            }
            return false;
        }

        static internal void PrintShortReport()
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
                    Card += s.Sum/100;
                    CardCount++;
                }
                else
                {
                    Vozvr += s.Sum/100;
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
                OutStr += s + " "+char.ConvertFromUtf32(10)[0];
            }
            Utils.ToCardLog("ReportStrings" + OutStr + char.ConvertFromUtf32(31)[0]);
            //ToShtrih.Init();
            //ToShtrih.Conn();
            PrintSlip(OutStr + char.ConvertFromUtf32(31)[0]);
            //ToShtrih.PrintCardCheck(OutStr + char.ConvertFromUtf32(31)[0]);
            

        }

        static internal ArcusSlips ReadArcusSlips()
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
        static internal void WriteArcusSlips(ArcusSlips AllSlips)
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
    public class ArcusSlip
    {
        public ArcusSlip()
        { 
        }
        public bool Void = false;
        public decimal Sum = 0;
        public DateTime HostDt;
        public int Num=0;
        public string RRN = "";
        public List<string> Slip = new List<string>();
        public string AlohaCheckShortNum { set; get; }
        public int AlohaCheckId { set; get; }

    }
    public class ArcusSlips
    {
        public ArcusSlips()
        {
        }

        public List<ArcusSlip> Slips = new List<ArcusSlip>();

    }

}
