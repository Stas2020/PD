using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Windows;
using System.Net;

namespace PDTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //FCCIntegration.MainClass.Init();
          //  MessageBox.Show(FCCIntegration.MainClass.GetStatus());
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //FCCIntegration.MainClass.StartCashinOperation();
            PDiscountCard.FCC.SetBillTest();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            PDiscountCard.FCC.MainClass2_OnOutCasseta(456300, true);

        }

        private void button4_Click(object sender, EventArgs e)
        {
            PDiscountCard.FCC.CancelBill();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            
            //FCCIntegration.MainClass.Init();
            //FCCIntegration.MainClass.StartChangeMoney(110000);
            PDiscountCard.MainClass.InitData();
            //PDiscountCard.AlohaTSClass.InitAlohaCom();
            PDiscountCard.AlohaEventVoids.CloseByWaiter();
             
        }

        private void button6_Click(object sender, EventArgs e)
        {
            //FCCIntegration.MainClass.Reset();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            //FCCIntegration.MainClass.ChangeCancel();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            PDiscountCard.FCC.ShowAdmin();
            
        }

        private void button9_Click(object sender, EventArgs e)
        {
            PDiscountCard.FCC.ShowCassirFrm();
        }

      
        private void button14_Click(object sender, EventArgs e)
        {
            OrderToAlohaSrv.MainClass.Init(64788,"S2010");

        }

        private void button15_Click(object sender, EventArgs e)
        {
            //PDiscountCard.MainClass.InitData();
            PDiscountCard.FCC.Init();
            //PDiscountCard.CCustomerDisplay.Init();

            //PDiscountCard.Shtrih2.CreateShtrih();
            //FCCIntegration.MainClass2.InitDevice();
        }

        private void button16_Click(object sender, EventArgs e)
        {
            /*
            int VisitCount=0;
            int DayCount=0;
            int VisitTotal=0;
            int DayTotal;
            int Gold = 0;

            int k = PDiscountCard.ToBase.DoVizit2("PRE", "000364", 999, "10006",
                     1, 6000, DateTime.Now.AddHours(25) ,false, out  VisitCount, out DayCount, out  VisitTotal, out DayTotal, out Gold, 2);
             * */

            PDiscountCard.AlohaEventVoids.ShowFrmCashIn();
            
        }

        private void button17_Click(object sender, EventArgs e)
        {
            PDiscountCard.FCC.MainClass2_OnOutCasseta(100, true);
        }

        private void button18_Click(object sender, EventArgs e)
        {
            FCCIntegration.MainClass2.RunWriteMins();
        }

        private void button19_Click(object sender, EventArgs e)
        {
            FCCIntegration.MainClass2.StartCashOut();
        }

        private void button20_Click(object sender, EventArgs e)
        {
            FCCIntegration.MainClass2.ShowRazmenWnd();
        }

        private void button21_Click(object sender, EventArgs e)
        {
            
            
        
           // PDiscountCard.DualConnector.DualConnectorMain.TestSale(40000112,10);

        }

        void DC_OnExchangeComplited(object sender, int Result, string ResultDescr, string Reciept)
        {
            MessageBox.Show(ResultDescr + " Reciept: " +Environment.NewLine+ Reciept);
        }

        void DC_OnExchange(object sender, int Code, string CodeDescr, int status)
        {
            
        }

        private void button22_Click(object sender, EventArgs e)
        {
         //   PDiscountCard.DualConnector.DualConnectorMain.Init();
          //  PDiscountCard.DualConnector.DualConnectorMain.ShortReport();
        }

        private void button23_Click(object sender, EventArgs e)
        {
            //PDiscountCard.DualConnector.DualConnectorMain.Init();
            //PDiscountCard.DualConnector.DualConnectorMain.LongReport(40000112);
        }

        private void button24_Click(object sender, EventArgs e)
        {
            //PDiscountCard.DualConnector.DualConnectorMain.Init();
        }

        private void button25_Click(object sender, EventArgs e)
        {
            try
            {
            //PDiscountCard.DualConnector.DualConnectorMain.Init();
            }
            catch
            {}
           // PDiscountCard.DualConnector.DualConnectorMain.Void(null,"");
        }

        private void button26_Click(object sender, EventArgs e)
        {
            try
            {
                //PDiscountCard.DualConnector.DualConnectorMain.Init();
            }
            catch
            { }
           // PDiscountCard.DualConnector.DualConnectorMain.TestCancel(40000112, 0);
        }

        private void button27_Click(object sender, EventArgs e)
        {
            PDiscountCard.Spool.SpoolToHamster.GenerateHamster();
        }

        private void button28_Click(object sender, EventArgs e)
        {
            PDiscountCard.AlohaTSClass.InitAlohaCom();
            bool res = PDiscountCard.AlohaTSClass.GetPriceStartDateIsActual(1);
            MessageBox.Show(res.ToString());
        }

        private void button29_Click(object sender, EventArgs e)
        {
            string Err = "";
            //PDiscountCard.CreditCardAlohaIntegration.Init(out Err);
            PDiscountCard.CreditCardAlohaIntegration.ShowCassirMnu();

        }

        private void button30_Click(object sender, EventArgs e)
        {
            PDiscountCard.ArcusSlips As = PDiscountCard.Arcus2DataFromXML.ReadArcusSlips();
            MessageBox.Show(As.Slips.Sum(s=>s.Sum).ToString());
        }

        private void button31_Click(object sender, EventArgs e)
        {
            PDiscountCard.RemoteCommands.RemoteSender.SendCheckOnClose(111, 20, 1111,0);
        }

        private void button32_Click(object sender, EventArgs e)
        {
            PDiscountCard.SQL.ToSql.CheckBase();
        }

        private void button33_Click(object sender, EventArgs e)
        {
            string ChecksPath = @"C:\aloha\check\discount\tmp\check";
            PDiscountCard.AllChecks myAllChecks = new PDiscountCard.AllChecks();
            myAllChecks = PDiscountCard.CloseCheck.ReadAllChecks(ChecksPath + @"\hamster.xml");
            MessageBox.Show("Нал " + myAllChecks.GetShtrihBalanse(true).Nal + " Возвр Нал " + myAllChecks.GetShtrihBalanse(true).VozvrNal);
            MessageBox.Show("Card " + myAllChecks.GetShtrihBalanse(true).Card + " Возвр card " + myAllChecks.GetShtrihBalanse(true).VozvrCard);
            //MessageBox.Show(myAllChecks.Checks.Where(a=>!a.IsNal).Sum(a=>a.Summ).ToString()); 
            //MessageBox.Show(myAllChecks.Checks.Where(a => !a.IsNal && a.Waiter==9267).Sum(a => a.Summ).ToString()); 

        }

        private void button34_Click(object sender, EventArgs e)
        {
            PDiscountCard.Spool.SpoolToHamster.RegenOT();
        }

        private void button35_Click(object sender, EventArgs e)
        {
            PDiscountCard.West.WestMain.ShowSaleReport(0);
        }

        private void button36_Click(object sender, EventArgs e)
        {
            FiskalDrivers.FiskalDriver.CreateFiskalDriver(1);
            FiskalDrivers.FiskalDriver.Connect(2);

        }

        private void button37_Click(object sender, EventArgs e)
        {
            FiskalDrivers.FiskalDriver.PrintString("");

        }

        private FiskalDrivers.FiskalCheck GetChk()
        {
            FiskalDrivers.FiskalCheck fc = new FiskalDrivers.FiskalCheck();
            FiskalDrivers.FiskalDish fd = new FiskalDrivers.FiskalDish()
            {
                Name = "Бл1",
                Price = 12.34,
                Quantity = 1


            };
            fc.Dishes.Add(fd);
            FiskalDrivers.FiskalDish fd2 = new FiskalDrivers.FiskalDish()
            {
                Name = "Бл2",
                Price = 150,
                Quantity = 5


            };
            FiskalDrivers.FiskalDish fd3 = new FiskalDrivers.FiskalDish()
            {
                Name = "Бл3",
                Price = 750,
                Quantity = 0.25


            };

            fc.Dishes.Add(fd2);
            fc.Dishes.Add(fd3);
            fc.Summ = 1000;
            return fc;
        }
        private void button38_Click(object sender, EventArgs e)
        {


            FiskalDrivers.FiskalDriver.CloseCheck(GetChk());
        }

        private void button39_Click(object sender, EventArgs e)
        {
            FiskalDrivers.FiskalCheck Chk = GetChk();
            Chk.IsVoid = true;
            FiskalDrivers.FiskalDriver.CloseCheck(Chk);
        }

        private void button40_Click(object sender, EventArgs e)
        {
            /*
            FiskalDrivers.FiskalDriver.CreateFiskalDriver(1);
            FiskalDrivers.FiskalDriver.Connect(2);
            PDiscountCard.GallerySrvs g = new PDiscountCard.GallerySrvs();
            g.StartOrderLisenter();
             * */
        }

        private void button41_Click(object sender, EventArgs e)
        {
            PDiscountCard.ArcusSlips As = PDiscountCard.Arcus2DataFromXML.ReadArcusSlips();
            string ChecksPath = @"C:\aloha\check\discount\tmp\check";
            PDiscountCard.AllChecks myAllChecks = new PDiscountCard.AllChecks();
            myAllChecks = PDiscountCard.CloseCheck.ReadAllChecks(ChecksPath + @"\hamster.xml");
            
            foreach (PDiscountCard.ArcusSlip item in As.Slips)
            {
                if (myAllChecks.Checks.Where(a => a.Summ == item.Sum/100).Count() == 0)
                {
                    MessageBox.Show(item.RRN+Environment.NewLine+item.Sum); 
                }
            }

            //MessageBox.Show(As.Slips.Sum(s => s.Sum).ToString());
            //MessageBox.Show(myAllChecks.Checks.Where(a=>!a.IsNal).Sum(a=>a.Summ).ToString()); 
            //MessageBox.Show(myAllChecks.Checks.Where(a => !a.IsNal && a.Waiter == 9267).Sum(a => a.Summ).ToString()); 
        
        }

        private void button42_Click(object sender, EventArgs e)
        {
            /*
            /*
            PDiscountCard.Shtrih2.CreateShtrih();
            PDiscountCard.Shtrih2.Timeout = 3000;
           bool res =   PDiscountCard.Shtrih2.Conn();
           PDiscountCard.Shtrih2.SetExchangeParam();
           PDiscountCard.Shtrih2.GetExchangeParam();
            MessageBox.Show(PDiscountCard.Shtrih2.Timeout.ToString()+" "+res.ToString());
             * */
        }

        private void button43_Click(object sender, EventArgs e)
        {
            PDiscountCard.AlohaTSClass.InitAlohaCom();
            long ChN =PDiscountCard.AlohaTSClass.GetCurentCheckId();
            string Err = "";
         //   PDiscountCard.AlohaTSClass.ApplySVCardPayment((int)ChN, 300, 19, textBox1.Text, textBox2.Text, out Err);
            if (Err!="")
            {
                MessageBox.Show(Err);
            }
        }

        private void button44_Click(object sender, EventArgs e)
        {
            PDiscountCard.AlohaTSClass.InitAlohaCom();
            PDiscountCard.AlohaExternal.UniversalHost.Open();
        }

        private void button45_Click(object sender, EventArgs e)
        {
            string ChecksPath = @"C:\aloha\check\discount\tmp\check";
            PDiscountCard.AllChecks myAllChecks = new PDiscountCard.AllChecks();
            myAllChecks = PDiscountCard.CloseCheck.ReadAllChecks(ChecksPath + @"\hamster.xml");
             //PDiscountCard.AllChecks myAllChecks2 = PDiscountCard.CloseCheck.ReadAllChecks(ChecksPath + @"\hamster2.xml");
            //myAllChecks.AddRange(myAllChecks2.Checks);
            MessageBox.Show(myAllChecks.GetShtrihBalanse(true).Nal.ToString());
            /*
            using(StreamWriter sw = new StreamWriter (ChecksPath +@"\1.txt"))
            {
                foreach (PDiscountCard.Check chk in myAllChecks.Checks.OrderBy(a=>a.Summ))
                {
                    if (chk.Summ!=0)
                    {
                    sw.WriteLine(chk.Summ);
                    }
                }
            }
             * */


        }

        private void button46_Click(object sender, EventArgs e)
        {
            //PDiscountCard.SV.SVClass.StartSVPayment(123);
        }

        private void button47_Click(object sender, EventArgs e)
        {
            //PDiscountCard.SV.SVClass.StartSVSale();
        }

        private void button48_Click(object sender, EventArgs e)
        {
            PDiscountCard.FDegustations Deg= new PDiscountCard.FDegustations();
            Deg.ShowDialog();

        }

        private void button49_Click(object sender, EventArgs e)
            
        {
            //PDiscountCard.FRSClientApp.FRSClient.Init();
            PDiscountCard.FRSClientApp.FRSClient.SendTestChk();
        }

        private void button50_Click(object sender, EventArgs e)
        {
            //PDiscountCard.FRSClientApp.PrintOnWinPrinter.PrintCheck();
        }

        private void button51_Click(object sender, EventArgs e)
        {
            PDiscountCard.EGAIS.EGAISCodeReader.Read();
            //PDiscountCard.EGAIS.EGAISCodeReader.GetEgaisListFrom1C();
            
        }

        private void button52_Click(object sender, EventArgs e)
        {
            PDiscountCard.Shtrih2.CreateShtrih();
            PDiscountCard.AlohaEventVoids.ZReport();
            PDiscountCard.Utils.SaveLog();
        }

        private void button53_Click(object sender, EventArgs e)
        {
            PDiscountCard.Scale.Scale2.GetItmWeight(1000, "Булка");
        }

        private void button54_Click(object sender, EventArgs e)
        {
            PDiscountCard.FRSClientApp.FRSClient.ZReport(new DateTime(2018,1,1));
        }

        private void button55_Click(object sender, EventArgs e)
        {
            //PDiscountCard.FRSClientApp.FRSClient.Init();
            PDiscountCard.FRSClientApp.FRSClient.PrintFCheckShowWnd();



        }

        private void button56_Click(object sender, EventArgs e)
        {
            PDiscountCard.Spool.SpoolToHamster.ConvertLLOTGluk();
        }

        private void button57_Click(object sender, EventArgs e)
        {

            PDiscountCard.Spool.SpoolToHamster.GetSpoolSumm(new DateTime(2018, 07, 04));
        }

        private void button58_Click(object sender, EventArgs e)
        {
            PDiscountCard.FRSClientApp.FRSClient.XReport();
        }

        private void button59_Click(object sender, EventArgs e)
        {
            PDiscountCard.Hamster.HamsterWorker.SendOldHamster();
        }

        private void button60_Click(object sender, EventArgs e)
        {
            FrmFayRetailTest FT = new FrmFayRetailTest();
            FT.Show();
        }

        private void button61_Click(object sender, EventArgs e)
        {
            PDiscountCard.EGAIS.EGAISCodeReader.GetEgaisAlreadyScanFrom1C("22N00002V8O6B47ZV1U839570919001000990824868FZPTPIYNVPI6KNL8R0BM1F93V", 380);
        }

        private void button10_Click(object sender, EventArgs e)
        {

            PrtCheckLocal();
            /*
            List <int> Nums = new List<int> (){
            180029735,
180029868,
180030223,
180030296,
180030396,
180030564,
180030797,
180031326,
180031401

            };
            foreach(int n in Nums)
            {
                PrtCheck(n);
            }
*/
        }

        private void PrtCheckLocal()
        {
            /*
            string Address = @"http://vfiliasesb0:2580/process/Ges3ServicesUTF8Proc";
            System.ServiceModel.EndpointAddress remoteAddress = new System.ServiceModel.EndpointAddress(Address);
            System.ServiceModel.Channels.Binding binding = new System.ServiceModel.BasicHttpBinding();
            ((System.ServiceModel.BasicHttpBinding)binding).MaxReceivedMessageSize = 1024 * 1024;
            SrvGes.Ges3ServicesUTF8ObjClient Cl = new SrvGes.Ges3ServicesUTF8ObjClient(binding, remoteAddress); ;
            */

            DateTime Startdt = new DateTime(2018, 01, 02);
            DateTime dt = new DateTime(2018, 09, 20); ;
            int? CodePost = 0;
            int? CodePodr = 0;
            
            /*
            SrvGes.get_invoice_by_code_T_goodsRow[] GoodsOfInvoice = new SrvGes.get_invoice_by_code_T_goodsRow[0];
            string res = Cl.get_invoice_by_code(InvNum, out dt, out CodePost, out CodePodr, out GoodsOfInvoice);
            SrvGes.get_GestoriGoods_T_GestoriGoodsRow[] Goods = new SrvGes.get_GestoriGoods_T_GestoriGoodsRow[0];
            string res2 = Cl.get_GestoriGoods(CodePodr, out Goods);
            */
            
            dt = dt.AddHours((new Random()).Next(9, 20));
            dt = dt.AddMinutes((new Random()).Next(0, 59));
            PDiscountCard.FRSSrv.AddCheckResponce Resp = new PDiscountCard.FRSSrv.AddCheckResponce();
            Resp.Attrs = new PDiscountCard.FRSSrv.FRAttributes();
            Resp.Attrs.FNNumber = "25765186321863";
            Resp.Attrs.INN = "712306990379";
            Resp.Attrs.Klishe = (new List<string> { "ООО ТРИНИТИ", "г. Москва, ул. Лесная, д.15" }).ToArray();
            Resp.Attrs.RN = "00044887033128";
            Resp.Attrs.Smena = (dt - Startdt).Days;
            Resp.Attrs.TaxId = 1;
            Resp.Attrs.TaxName = "А: СУММА НДС 18%";
            Resp.Attrs.TaxPercent = 1800;
            Resp.Attrs.TaxSystem = "ОСН";
            Resp.Attrs.ZN = "00307402685302";
            Resp.Attrs.PaymentNames = new Dictionary<int, string>();
            Resp.Attrs.PaymentNames.Add(1, "Наличные");

            Resp.Result = true;
            Resp.Check = new PDiscountCard.FRSSrv.FiskalCheck();
            Resp.Check.FROutData = new PDiscountCard.FRSSrv.FCheckOutData();
            Resp.Check.FROutData.SmallNum = (new Random()).Next(250);
            Resp.Check.FROutData.FD = Resp.Attrs.Smena * 250 + Resp.Check.FROutData.SmallNum;
            Resp.Check.FROutData.FN = "9283440300141038";
            Resp.Check.FROutData.FP = (new Random()).Next(99999).ToString("00000") + (new Random()).Next(99999).ToString("00000");
            Resp.Check.FROutData.OperationType = 0;
            Resp.Check.FROutData.Smena = Resp.Attrs.Smena;
            Resp.Check.SystemDate = dt;

            List<PDiscountCard.FRSSrv.FiskalDish> DL = new List<PDiscountCard.FRSSrv.FiskalDish>();
            
            DL.Add(new PDiscountCard.FRSSrv.FiskalDish()
            {
                Name = "Аматоре Бьянко Верона, 0.75 л",
                Price = 414,
                Count = 18
            });
            DL.Add(new PDiscountCard.FRSSrv.FiskalDish()
            {
                Name = " Ромио Пино Гриджо Фриули, 0.75 л",
                Price = 534,
                Count = 6
            });
            DL.Add(new PDiscountCard.FRSSrv.FiskalDish()
            {
                Name = "Ромио Кьянти, 0.75 л",
                Price = 444,
                Count = 18
            });

            DL.Add(new PDiscountCard.FRSSrv.FiskalDish()
            {
                Name = "Кава Кастель Льорд, 0.75 л",
                Price = 444,
                Count = 12
            });
            double Summ = (double)DL.Sum(a=>a.Count*a.Price);
            /*
            foreach (SrvGes.get_invoice_by_code_T_goodsRow G in GoodsOfInvoice)
            {
             //   string GName = Goods.Where(a => a.bar_cod == G.cod_good).First().good_name;
                PDiscountCard.FRSSrv.FiskalDish Fd = new PDiscountCard.FRSSrv.FiskalDish();
                Fd.Name = "Аматоре Бьянко Верона";
                Fd.Price = 414;
                Fd.Count = 18;
                DL.Add(Fd);
                Summ += (double)Fd.Price * (double)Fd.Count;
                
            }
             * */
            Resp.Check.Dishes = DL.ToArray();
            Resp.Check.FROutData.FNSumm = (decimal)Summ;
            Resp.Check.FROutData.SysDt = dt;
            Resp.Check.FROutData.QRAsStr = GetQRstr(Resp.Check.FROutData);
            List<PDiscountCard.FRSSrv.FiskalPayment> LP = new List<PDiscountCard.FRSSrv.FiskalPayment>();
            LP.Add(
            new PDiscountCard.FRSSrv.FiskalPayment()
            {
                Id = 1,
                Summ = (decimal)Summ
            });

            //   List<Resp.Check.Payments>  LP = new List<Resp.Check.Payments> ();

            Resp.Check.Payments = LP.ToArray();
            //Resp.Check.FROutData.FNSumm="4444";

            PDiscountCard.FRSClientApp.PrintOnWinPrinter.PrintDoc2(new PDiscountCard.FRSClientApp.PrintDocArgs() { FStrs = PDiscountCard.FRSClientApp.FiscalCheckCreator.GetFisckalCheckVisual(Resp), QRAsStr = Resp.Check.FROutData.QRAsStr });
        }

        private void PrtCheck(int InvNum )
        {
            string Address = @"http://vfiliasesb0:2580/process/Ges3ServicesUTF8Proc";
            System.ServiceModel.EndpointAddress remoteAddress = new System.ServiceModel.EndpointAddress(Address);
            System.ServiceModel.Channels.Binding binding = new System.ServiceModel.BasicHttpBinding();
            ((System.ServiceModel.BasicHttpBinding)binding).MaxReceivedMessageSize = 1024 * 1024;
            SrvGes.Ges3ServicesUTF8ObjClient Cl = new SrvGes.Ges3ServicesUTF8ObjClient (binding, remoteAddress);;
            
            
            DateTime Startdt =new DateTime (2018,01,02);
            DateTime? dt ;
            int? CodePost=0;
            int? CodePodr=0;
            SrvGes.get_invoice_by_code_T_goodsRow[] GoodsOfInvoice = new  SrvGes.get_invoice_by_code_T_goodsRow[0];
            string res =  Cl.get_invoice_by_code(InvNum, out dt, out CodePost, out CodePodr, out GoodsOfInvoice);
            SrvGes.get_GestoriGoods_T_GestoriGoodsRow[]  Goods = new  SrvGes.get_GestoriGoods_T_GestoriGoodsRow[0];
            string res2= Cl.get_GestoriGoods(CodePodr, out Goods);
            dt = dt.Value.AddHours((new Random()).Next(9, 20));
            dt = dt.Value.AddMinutes((new Random()).Next(0, 59));
            PDiscountCard.FRSSrv.AddCheckResponce Resp = new PDiscountCard.FRSSrv.AddCheckResponce();
            Resp.Attrs = new PDiscountCard.FRSSrv.FRAttributes();
            Resp.Attrs.FNNumber = "25765186321863";
            Resp.Attrs.INN = "712306990379";
            Resp.Attrs.Klishe = (new List<string>{"ООО ТРИНИТИ", "г. Москва, ул. Лесная, д.15"}).ToArray();
            Resp.Attrs.RN = "00044887033128";
            Resp.Attrs.Smena = (dt.Value - Startdt).Days;
            Resp.Attrs.TaxId = 1;
            Resp.Attrs.TaxName = "А: СУММА НДС 18%";
            Resp.Attrs.TaxPercent = 1800;
            Resp.Attrs.TaxSystem = "ОСН";
            Resp.Attrs.ZN = "00307402685302";
            Resp.Attrs.PaymentNames = new Dictionary<int, string>();
            Resp.Attrs.PaymentNames.Add(1, "Наличные");

            Resp.Result = true;
                Resp.Check=new PDiscountCard.FRSSrv.FiskalCheck ();
                Resp.Check.FROutData = new PDiscountCard.FRSSrv.FCheckOutData();
                Resp.Check.FROutData.SmallNum = (new Random()).Next(250);
            Resp.Check.FROutData.FD = Resp.Attrs.Smena*250+ Resp.Check.FROutData.SmallNum ;
            Resp.Check.FROutData.FN="9283440300141038";
            Resp.Check.FROutData.FP = (new Random()).Next(99999).ToString("00000") + (new Random()).Next(99999).ToString("00000");
            Resp.Check.FROutData.OperationType = 0;
            Resp.Check.FROutData.Smena = Resp.Attrs.Smena;
            Resp.Check.SystemDate = dt.Value;
            
            List<PDiscountCard.FRSSrv.FiskalDish> DL = new List<PDiscountCard.FRSSrv.FiskalDish> ();
            double Summ=0;
            

            foreach (SrvGes.get_invoice_by_code_T_goodsRow G in GoodsOfInvoice)
            {
                string GName = Goods.Where(a => a.bar_cod == G.cod_good).First().good_name;
                PDiscountCard.FRSSrv.FiskalDish Fd = new PDiscountCard.FRSSrv.FiskalDish ();
                Fd.Name = GName;
                Fd.Price = G.price.Value;
                Fd.Count = G.quantity.Value;
                DL.Add(Fd);
                Summ += (double)Fd.Price*(double)Fd.Count;
                Resp.Check.Dishes = DL.ToArray();
            }
            Resp.Check.FROutData.FNSumm = (decimal)Summ;
            Resp.Check.FROutData.SysDt = dt.Value;
            Resp.Check.FROutData.QRAsStr = GetQRstr(Resp.Check.FROutData);
            List<PDiscountCard.FRSSrv.FiskalPayment> LP = new List<PDiscountCard.FRSSrv.FiskalPayment> ();
            LP.Add(
            new  PDiscountCard.FRSSrv.FiskalPayment()
            {
             Id=1,
             Summ = (decimal)Summ
            });

        //   List<Resp.Check.Payments>  LP = new List<Resp.Check.Payments> ();

            Resp.Check.Payments = LP.ToArray();
            //Resp.Check.FROutData.FNSumm="4444";

            PDiscountCard.FRSClientApp.PrintOnWinPrinter.PrintDoc2(new PDiscountCard.FRSClientApp.PrintDocArgs() { FStrs = PDiscountCard.FRSClientApp.FiscalCheckCreator.GetFisckalCheckVisual(Resp), QRAsStr = Resp.Check.FROutData.QRAsStr });
        }


        public string GetQRstr(PDiscountCard.FRSSrv.FCheckOutData Resp)
        {
           return  String.Format("t={0}&s={1}&fn={2}&i={3}&fp={4}&n={5}",
                        Resp.SysDt.ToString("yyyyMMddTHHmmss"),
                        Resp.FNSumm.ToString("0.00").Replace(",", "."),
                        Resp.FN,
                        Resp.FD,
                        Resp.FP,
                        Resp.OperationType
                        );
        }

        private void button11_Click(object sender, EventArgs e)
        {
            string err = "";

            PDiscountCard.Check chk = new PDiscountCard.Check();
            chk.Waiter = 1111;
            chk.Dishez = new List<PDiscountCard.Dish>()
            {
                new  PDiscountCard.Dish()
                {
                    LongName ="Цыпленок жареный",
                    BarCode = 842128,

                },
                new  PDiscountCard.Dish()
                {
                    LongName ="Тар тар тунец Тар тар тунец Тар тар тунец",
                    BarCode = 835024,

                },
                new  PDiscountCard.Dish()
                {
                    LongName ="Лечо",
                    BarCode = 855017,

                }


            };
            PDiscountCard.PrintOrder.PrintOrder.Instanse.PrintToGoOrderLabels(chk,out err);
        }

        private void button12_Click(object sender, EventArgs e)
        {
            /*
            string str="";
            PDiscountCard.CreditCardAlohaIntegration.Init(PDiscountCard.CreditCardTerminalType.Arcus2, out str);
            PDiscountCard.CreditCardAlohaIntegration.
            PDiscountCard.CreditCardIntegration.Arcus3Wrapper wrp = new PDiscountCard.CreditCardIntegration.Arcus3Wrapper();
            wrp.CallArcusOper(PDiscountCard.CreditCardIntegration.Arcus3Wrapper.ArcusOp.Pay, new PDiscountCard.CreditCardIntegration.ArcusRequest(), new PDiscountCard.CreditCardIntegration.ArcusResponse());
             * */
        }

        private void button13_Click(object sender, EventArgs e)
        {
            
            var gd = new PDiscountCard.RemoteSrvs.GesData();
            string err="";
          var res =  gd.GetItemDescription(new List<int>() { 841810 }, out err);
          var res2 = gd.GetItemExp2(new List<int>() { 841539 }, out err);
          

            
          //  PDiscountCard.PrintOrder.PrintOrder.Instanse.CutPrint(@"TSC TC200");


        }

        private void button62_Click(object sender, EventArgs e)
        {

            int cat = PDiscountCard.AlohaTSClass.GetCatByItem(3147429);
        }
    }

    
}
