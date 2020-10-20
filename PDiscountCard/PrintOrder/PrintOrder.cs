using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
using System.Threading;
using System.Collections.Concurrent;

using System.Web;
using System.Runtime.InteropServices;

namespace PDiscountCard.PrintOrder
{
  public  class PrintOrder
    {
        private PrintOrder()
        {

        }

        static PrintOrder printOrder;
        public static PrintOrder Instanse
        {
            get
            {
                if (printOrder == null)
                {
                    printOrder = new PrintOrder();
                }
                return printOrder;
            }
        }

        public static ConcurrentDictionary<int, DateTime> LastPrintDic = new ConcurrentDictionary<int, DateTime>();
        
        public bool PrintToGoOrderLabels(Check chk,out string err)
        {
            err = "";
            DateTime dt;
            if (LastPrintDic.TryGetValue(chk.AlohaCheckNum, out dt))
            {
                if ((DateTime.Now - dt).TotalSeconds < 20)
                {
                    LastPrintDic.TryUpdate(chk.AlohaCheckNum, DateTime.Now, dt);
                    return true;
                }
                else
                {
                    LastPrintDic.TryUpdate(chk.AlohaCheckNum, DateTime.Now, dt);
                }

            }
            else
            {
                LastPrintDic.TryAdd(chk.AlohaCheckNum, DateTime.Now);
            }
            //t.SetApartmentState(ApartmentState.STA);

            
            try
            {
                
            Utils.ToCardLog("PrintToGoOrderLabels ");
            if (chk == null) return false;
            if (chk.Dishez == null) return false;

            string wname = AlohaTSClass.GetWaterName(chk.Waiter);

            if (wname == "") wname = "Иван Петров";
            List<int> itmsBc= chk.Dishez.Where(a=>a.BarCode<933300).Select(b=>b.BarCode).ToList();
                var gd = new RemoteSrvs.GesData();
            var expTimes =  gd.GetItemExp2(itmsBc,out err);
            var descr = gd.GetItemDescription(itmsBc, out err);


            bool res = true;
            if (!PrintQRLabel(wname, chk.CheckShortNum))
            {
                err += "Ошибка печати на принтере Qr code " + Environment.NewLine;
                res = false;
            }

                var butterList = new List<int> { 864007, 864008, 864009, 864010 };
                //  foreach (Dish itm in chk.Dishez.Where(a=>a.Level==0 && a.Price>0))
                foreach (Dish itm in chk.Dishez.Where(a => a.BarCode<933000 && !butterList.Contains(a.BarCode)))
                {
                var expT = new Tuple<int,string>(24,"");
                string dDescr = "";
                
                
                if (expTimes != null)
                {
                    expTimes.TryGetValue(itm.BarCode, out expT);
                }
                if (descr != null)
                {
                    descr.TryGetValue(itm.BarCode, out dDescr);
                    if (dDescr != null && dDescr != "")
                    {
                        Utils.ToCardLog("PrintToGoDishLabel dESCR: "+dDescr);
                    }
                }
             //   if (expT.Item1 == 0) expT.Item1  = 24;
                if (!PrintToGoDishLabel(itm, wname, expT.Item1, chk.TableNumber, dDescr, expT.Item2))
                {
                    err += "Ошибка печати на принтере блюда "+itm.LongName + Environment.NewLine;
                    res = false;
                }
                

            }
                
                //Приборы
                if (chk.Dishez.Any(a => a.BarCode == 977222))
                {
                    if (!PrintFreeStringLabel("Приборы"))
                    {
                        err += "Ошибка печати на принтере блюда Приборы" ;
                        res = false;
                    }
                }

                //Хлеб

                
                if (chk.Dishez.Any(a => butterList.Contains(a.BarCode)))
                {
                    if (!PrintFreeStringLabel("Хлеб"))
                    {
                        err += "Ошибка печати на принтере блюда Хлеб";
                        res = false;
                    }

                }


                CutPrint(iniFile.LabelPrinterName);
            return res;
            }
            catch(Exception e)
            {
                Utils.ToCardLog("Error PrintToGoOrderLabels " + e.Message);
                err+= "Ошибка печати этикеток "+e.Message + Environment.NewLine;
                return false;
            }
        }

        private bool PrintQRLabel(string EmpName,string ordernum)
        {
            string EncEmpl = HttpUtility.UrlEncode(EmpName);
            string QRStr = @"http://saycoffeemania.ru/?DepId=" + AlohainiFile.DepNum + "&Emp=" + EncEmpl + "_" + ordernum;
            //string QRStr = @"http://saycoffeemania.ru/?DepId=" + "104" + "&Emp=" + EncEmpl;
            Utils.ToCardLog("PrintQRLabel " + QRStr);
            var QrImg = FRSClientApp.FiscalCheckCreator.CreateQRBitmap(QRStr, 115, 115);
            //BitmapEncoder encoder = new BmpBitmapEncoder();
            //encoder.Frames.Add(BitmapFrame.Create(QrImg));

            var vis = new CtrlCheckPrintTemplate();
            vis.CreateCheck(QrImg);

            return PrintDoc3(vis, 0, iniFile.LabelPrinterName, 270, 200); //200
        }




        private bool PrintFreeStringLabel(string descr)
        {
            
            var toGoStrs = new List<FiscalCheckVisualString>() {
                new FiscalCheckVisualString(" ",true,true ),
                new FiscalCheckVisualString(" ",true,true ),
             new FiscalCheckVisualString(descr,false,true )
            

            };

            var vis = new CtrlCheckPrintTemplate();
            
            vis.CreateCheck(toGoStrs, "");

            return PrintDoc3(vis, 0, iniFile.LabelPrinterName, 270, 190); //200

        }

        private bool PrintToGoDishLabel(Dish itm, string WaterName, int expirationHours, int chId, string descr,string saveDescr)
        {
            string expirationName = "час";
            string saveDescrDef = "+2°C - +6°C";

            int deltah = expirationHours <= 24 ? 0 : -4;
            if (saveDescr.Trim() != "")
            {
                saveDescrDef = saveDescr;
            }
                DateTime dtStart = DateTime.Now.AddHours(deltah);
            DateTime dtStop = dtStart.AddHours(expirationHours);

            

            if (expirationHours >= 48)
            {
                expirationName = "суток";
                expirationHours = expirationHours / 24;
            }          

            var toGoStrs = new List<FiscalCheckVisualString>() { 
             new FiscalCheckVisualString(String.Format("Чек {0}",chId))
            ,new FiscalCheckVisualString(String.Format("Упаковал: {0}",WaterName))

            ,new FiscalCheckVisualString(String.Format(itm.LongName),false,true)
            ,new FiscalCheckVisualString(String.Format("Произведено: {0} ",dtStart.ToString("dd.MM.yy HH:mm")))
            ,new FiscalCheckVisualString($"Срок хранения ({expirationName}): {expirationHours} " )
            ,new FiscalCheckVisualString($"Усл. хранения: {saveDescrDef}" )
            ,new FiscalCheckVisualString(String.Format("Использовать до: {0} ",dtStop.ToString("dd.MM.yy HH:mm")))
            };

            var vis = new CtrlCheckPrintTemplate();
            vis.CreateCheck(toGoStrs,descr );

           return  PrintDoc3(vis, 0, iniFile.LabelPrinterName, 270, 190); //200

        }



        //[DllImport(@"c:\aloha\alohats\bin\westreport.dll", CharSet = CharSet.Unicode)]
        [DllImport(@"westreport.dll", CharSet = CharSet.Unicode)]
        static extern void Cut(String Fs);


        public void CutPrint(string PrName)
        {
            Utils.ToCardLog("CutPrint PrName " + PrName);
            //RawPrinterHelper.SendStringToPrinter(PrName, "CUT");
            Cut(PrName);
            /*
            openport(PrName);
            sendcommand("CUT");
            closeport();
            */
        }

        bool PrintDoc3(UserControl vis, int TryCount, string PrName, int W, int H)
        {
            //   int W = 268;
            // double H = 5000;
            //ctrlCheckVisual vis = new ctrlCheckVisual();
            //string PrName = iniFile.FRSPrinterName;
            //string PrName = iniFile.FRSPrinterName;
            try
            {
                Utils.ToCardLog("PrintDoc3 PrName " + PrName);
                PrintDialog Pd = new PrintDialog();
                Pd.PageRangeSelection = PageRangeSelection.AllPages;
                PrintServer Ps = new PrintServer();
                PrintQueue PQ = new PrintQueue(Ps, PrName);
                Pd.PrintQueue = PQ;
             
                PrintTicket Pt = Pd.PrintTicket;
              
                Pt.PageMediaSize = new PageMediaSize(W, H);
                Pt.PageBorderless = PageBorderless.Borderless;
                Pt.PageResolution = new PageResolution(203, 203);
                Pt.PageScalingFactor = 1;
                Pt.TrueTypeFontMode = TrueTypeFontMode.DownloadAsRasterFont;

                Size pageSize = new Size(W - 10, Pd.PrintableAreaHeight);
               
                ((UserControl)vis).Measure(pageSize);
                ((UserControl)vis).Arrange(new Rect(0, 0, W - 10, ((UserControl)vis).Height));

                
                Pd.PrintVisual(vis, "Hello");
                Ps.Dispose();
                Pd.PrintQueue.Dispose();

                Pd = null;
                return true;

            }
            catch (Exception e)
            {
                Utils.ToCardLog("PrintDoc3 Error " + e.Message);
                if (TryCount < 5)
                {
                    Utils.ToCardLog("PrintDoc3  Try again " + TryCount);
                    System.Threading.Thread.Sleep(300);
                    GC.Collect();
                  return PrintDoc3(vis, TryCount + 1, PrName, W, H);
                }
                return false;
            }

            //vis = null;
            GC.Collect();


        }
    }
    public class PrintDocArgs
    {
        public PrintDocArgs()
        { }
        public List<FiscalCheckVisualString> FStrs { set; get; }
        public string QRAsStr { set; get; }

    }


    public class FiscalCheckVisualString
    {
        public FiscalCheckVisualString(string StringForPrint, bool Middle = true, bool BigFont = false)
        {
            strLeft = StringForPrint;
            strRight = "";
            Middle = true;
            bigFont = BigFont;
        }
        public FiscalCheckVisualString(string Left, string Right, bool BigFont = false)
        {
            strLeft = Left;
            strRight = Right;
            bigFont = BigFont;
            Middle = false;
        }
       
        public string strLeft { set; get; }
        public string strRight { set; get; }
        public bool bigFont { set; get; }
        public bool Middle { set; get; }

    }
}
