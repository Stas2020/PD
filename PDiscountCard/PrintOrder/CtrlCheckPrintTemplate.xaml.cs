using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PDiscountCard.PrintOrder
{
    /// <summary>
    /// Interaction logic for CtrlCheckPrintTemplate.xaml
    /// </summary>
    public partial class CtrlCheckPrintTemplate : UserControl
    {
        public CtrlCheckPrintTemplate()
        {
            InitializeComponent();
            //tbSostavGrid.Height = 0;
        }
        int descrScale = 0;
        
        public void CreateCheck(List<FiscalCheckVisualString> Strs, string descr)
        {
            tbSostavGrid.Height = 0;
            if (descr != null && descr.Length != 0)
            {
                descrScale = 2;
                
                tbSostavGrid.Height = 40;
                if (descr.Length > 120)
                {
                    tbSostavGrid.Height = 40;
                    descrScale = 2;
                }
                tbSostav.Text = "Состав: "+descr;
            }
            StPMain.Children.Clear();
            QrImg.Height = 0;
            foreach (FiscalCheckVisualString s in Strs)
            {
                StPMain.Children.Add(CreateGridForString(s,descrScale:descrScale));
            }
            //Height = Strs.Count * 20 + 140;
            //200
            //100
            Height = 150;
          
        }


        public void CreateCheck(BitmapImage QRImage)
        {
            StPMain.Children.Clear();
            StPMain.Children.Add(CreateGridForString( new FiscalCheckVisualString(String.Format("Оставьте, пожалуйста, отзыв")),false));
            //StPMain.Children.Add(CreateGridForString( new FiscalCheckVisualString(String.Format("наведя камеру на QR код"))));
            tbSostavGrid.Height = 0;
            if (QRImage != null)
            {
                QrImg.Source = QRImage;
            }
            Height = 150;

        }

        private Grid CreateGridForString(FiscalCheckVisualString Str,bool centered =false,int descrScale=0)
        {
            int StandartFont = 16;
            int BigFont = 24;
            if (descrScale == 1)
            {
                StandartFont = 14;
                BigFont = 18;
            }
            if (descrScale == 2)
            {
                StandartFont = 13;
                BigFont = 18;
            }

            int Font = (Str.bigFont) ? BigFont : StandartFont;
            if (Str.bigFont)
            {
                int d = 0;
            }

            Grid Gr = new Grid()
            {
                Margin = new Thickness(15, -3, -3, 0),
                HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                //Это ширина ленты!!!
                Width=240,  //290
                LayoutTransform = new System.Windows.Media.ScaleTransform(0.71, 1.0),
            };


            if (centered) Gr.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            TextBlock TbLeft = new TextBlock()
            {
               
                FontSize = Font,
                //FontFamily = new FontFamily("Calibri"),
                FontFamily = new FontFamily("Consolas"),
                HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                TextAlignment = System.Windows.TextAlignment.Left,
                FontWeight = System.Windows.FontWeights.DemiBold,
                Text = Str.strLeft,
                Padding = new Thickness(0, 0, 0, 0),
                TextWrapping = TextWrapping.NoWrap
            };
            if (centered) TbLeft.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            TextBlock TbRight = new TextBlock()
            {
                
                FontSize = Font,
                FontFamily = new FontFamily("Consolas"),
                HorizontalAlignment = System.Windows.HorizontalAlignment.Right,
                TextAlignment = System.Windows.TextAlignment.Right,
                FontWeight = System.Windows.FontWeights.DemiBold,
                Text = Str.strRight,
                Padding = new Thickness(0, 0, 0, 0),
                TextWrapping = TextWrapping.Wrap
            };
            Gr.Children.Add(TbLeft);
            Gr.Children.Add(TbRight);
            return Gr;
        }
    }
}
