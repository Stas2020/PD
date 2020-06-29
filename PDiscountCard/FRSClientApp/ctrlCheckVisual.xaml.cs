using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PDiscountCard.FRSClientApp
{
    /// <summary>
    /// Логика взаимодействия для ctrlCheckVisual.xaml
    /// </summary>
    public partial class ctrlCheckVisual : UserControl
    {
        public ctrlCheckVisual()
        {
            InitializeComponent();
        }
        public void SetImg(ImageSource source)
        {
            
            QrImg.Source = source;
            QrImg.UpdateLayout();
            UpdateLayout();
        }

        public void SetLba(string txt)
        {
            lbl.Text = txt;
            UpdateLayout();
        }


        public void CreateCheck(List<FiscalCheckVisualString> Strs, BitmapImage QRImage)
        {
            StPMain.Children.Clear();
            foreach (FiscalCheckVisualString s in Strs)
            {
                StPMain.Children.Add(CreateGridForFiskalString(s));
            }
            //Height = Strs.Count * 16 + 140;
            Height = Strs.Count * 20 + 140;
            if (QRImage != null)
            {
                QrImg.Source = QRImage;
            }
        }

        private Grid CreateGridForFiskalString(FiscalCheckVisualString Str)
        {
            int StandartFont = 16;
            int BigFont = 27;
            int Font = (Str.bigFont)? BigFont: StandartFont;
            if (Str.bigFont)
            {
                int d = 0;
            }

            Grid Gr = new Grid()
            {
                Margin = new Thickness(0, -3, -3, 0),
                
                LayoutTransform = new System.Windows.Media.ScaleTransform(0.74, 1.0)
              //  LayoutTransform = new System.Windows.Media.ScaleTransform(0.5, 1.0)
            };
            /*
            Utils.ToCardLog("Fonts");
            foreach (FontFamily fontFamily in Fonts.GetFontFamilies(new Uri("pack://application:,,,/"), "/Resources/"))
            {
                foreach(string s in fontFamily.FamilyNames.Values)
                {
                    Utils.ToCardLog("F: " + s); 
                }
            }
            Utils.ToCardLog("End Fonts");
            */
            TextBox TbLeft = new TextBox()
            {
                BorderThickness=new Thickness (0),
                FontSize = Font,
                //FontFamily = new FontFamily("Calibri"),
                FontFamily = new FontFamily("Consolas"),
                //FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "../Resources/#Consolas"),
              //  FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "/Resources/#Teminus (TTF)for Windows"),
                HorizontalAlignment= System.Windows.HorizontalAlignment.Left,
                FontWeight = System.Windows.FontWeights.DemiBold,
                //FontStretch = FontStretches.UltraCondensed,
                Text = Str.strLeft,
                Padding = new Thickness(0, -3, -3, 0),
            };
            TextBox TbRight = new TextBox()
            {
                BorderThickness = new Thickness(0),
                FontSize = Font,
                FontFamily = new FontFamily("Consolas"),
              //  FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "../Resources/#Consolas"),
                HorizontalAlignment = System.Windows.HorizontalAlignment.Right,
                FontWeight = System.Windows.FontWeights.DemiBold,
                //FontStretch = FontStretches.UltraCondensed,
                Text = Str.strRight   ,
                Padding = new Thickness(0, -3, -3, 0),
            };
            Gr.Children.Add(TbLeft);
            Gr.Children.Add(TbRight);
            return Gr;
        }

    }
    



    
}
