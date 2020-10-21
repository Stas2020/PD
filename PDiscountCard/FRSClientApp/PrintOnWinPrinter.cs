using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Printing;
//using System.Drawing;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;

namespace PDiscountCard.FRSClientApp
{
    static public class PrintOnWinPrinter
    {
        static public void PrintDoc2(List<FiscalCheckVisualString> FStrs)
        {
            PrintDoc2(new PrintDocArgs(){FStrs=FStrs});
        }

        static public void PrintDoc2(PrintDocArgs Args)
        {
            PrintDoc2(Args,0);
        }

        static public void PrintDoc2(PrintDocArgs Args, int TryCount)
        {
            int W = 268; //268
            double H = 5000;
            ctrlCheckVisual vis = new ctrlCheckVisual();
            if (Args.FStrs != null)
            {
                BitmapImage QrImg = FiscalCheckCreator.CreateQRBitmap(Args.QRAsStr, 130, 130);
                vis.CreateCheck(Args.FStrs, QrImg);
                vis.Visibility = Visibility.Visible;
                // string PrName = @"Predchek4";
                string PrName = iniFile.FRSPrinterName;
                try
                {
                    Utils.ToCardLog("PrintDoc PrName " + PrName);
                    PrintDialog Pd = new PrintDialog();
                    Pd.PageRangeSelection = PageRangeSelection.AllPages;
                    PrintServer Ps = new PrintServer();
                    PrintQueue PQ = new PrintQueue(Ps, PrName);
                    Pd.PrintQueue = PQ;
                   // Pd.ShowDialog();
                    PrintTicket Pt = Pd.PrintTicket;
                    Pt.PageMediaSize = new PageMediaSize(W, 11349);
                    Pt.PageBorderless = PageBorderless.Borderless;
                    Pt.PageResolution = new PageResolution(203, 203);
                    Pt.PageScalingFactor = 1;
                    Pt.TrueTypeFontMode = TrueTypeFontMode.DownloadAsRasterFont;
                    
                    Size pageSize = new Size(W-10, Pd.PrintableAreaHeight);
                    //pageSize = new Size(W, H);
                    ((UserControl)vis).Measure(pageSize);
                    ((UserControl)vis).Arrange(new Rect(0, 0, W - 10, ((UserControl)vis).Height));
                    //((UserControl)vis).Arrange(new Rect(0, 0, W, H));
                        if (iniFile.FRSSaveCheckToImg2)
                        {
                            SaveCheckVisualToFile(vis, W, Convert.ToInt32(((UserControl)vis).Height));
                        }
                    Pd.PrintVisual(vis, "Hello");
                    Ps.Dispose();
                    Pd.PrintQueue.Dispose();
                    
                    Pd = null;
                    QrImg = null;
                }
                catch (Exception e)
                {
                    Utils.ToCardLog("PrintDoc Error " + e.Message);
                    if (TryCount < 5)
                    {
                        Utils.ToCardLog("Try again " + TryCount); 
                        System.Threading.Thread.Sleep(300);
                        GC.Collect();
                        PrintDoc2(Args, TryCount + 1);
                    }
                }
            }
            vis = null;
            GC.Collect();
        }

        internal static void DeleteAllImgs()
        {
            try
            {
                if (Directory.Exists(DPath))
                {

                    foreach (string fi in Directory.GetFiles(DPath))
                    {
                        try
                        {
                            File.Delete(fi);
                        }
                        catch
                        { }
                    }
                }
            }
            catch
            { }
        }

        static string DPath = @"C:\Aloha\check\discount\tmp\imgs\";
        private static void SaveCheckVisualToFile(Visual Chkvis, int W,int H)
        {
            try
            {

                
                if (!Directory.Exists(DPath))
                {
                    Directory.CreateDirectory(DPath);
                }
        
                RenderTargetBitmap rtb = new RenderTargetBitmap(W,H, 96, 96, PixelFormats.Pbgra32);
                rtb.Render(Chkvis);
                // Encoding the RenderBitmapTarget as a PNG file.
                PngBitmapEncoder png = new PngBitmapEncoder();
                png.Frames.Add(BitmapFrame.Create(rtb));
                using (Stream stm = File.Create(DPath + @"\" + Guid.NewGuid().ToString() + ".png"))
                {
                    png.Save(stm);
                }
                rtb = null;
                png = null;
            }
            catch
            { }
        }
    }
    public class PrintDocArgs
    {
        public PrintDocArgs()
        { }
       public  List<FiscalCheckVisualString> FStrs {set; get;}
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
