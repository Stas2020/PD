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

namespace PDiscountCard.Scale
{
    /// <summary>
    /// Interaction logic for ctrlScaleMessage.xaml
    /// </summary>
    public partial class ctrlScaleMessage : UserControl
    {
        public ctrlScaleMessage()
        {
            InitializeComponent();
        }

        public void SetOwner(Window _Owner)
        {
            myOwner = _Owner;
        }
        private Window myOwner;

        internal void SetDish(int BarCode, string DishName)
        {
            CurentBarcode = BarCode;
            try
            {
                this.Dispatcher.BeginInvoke(((Action)(() =>
                {
                    try
                    {
                        LblItmName.Content = DishName;
               
                    }
                    catch
                    { }
                })));
            }
            catch
            { }
        }

        internal void SetError(string ErrMsg)
        {
            try
            {
                this.Dispatcher.BeginInvoke(((Action)(() =>
                {
                    try
                    {
                        LblError.Visibility = System.Windows.Visibility.Visible;
                        LblError.Content = ErrMsg;
                    }
                    catch
                    { }
                })));
            }
            catch
            { }
        }
        internal void SetWeight(int W, bool Stable)
        {
            try
            {
                this.Dispatcher.BeginInvoke(((Action)(() =>
                {
                    try
                    {
                        CurentStable = Stable;
                        CurentWeight = W;

                        LblError.Visibility = System.Windows.Visibility.Hidden;
                        LblError.Content = "";
                        LblWeight.Content = W.ToString() +" г";
                        if (Stable)
                        {
                            LblWeight.Foreground = new SolidColorBrush(Colors.Blue);
                            LblWeightNonStable.Visibility = System.Windows.Visibility.Hidden;
                        }
                        else
                        {
                            LblWeight.Foreground = new SolidColorBrush(Colors.LightBlue);
                        }
                    }
                    catch
                    { }
                })));
            }
            catch
            { }
        }

        static bool CurentStable = false;
        static int CurentWeight = 0;
        static int CurentBarcode = 0;


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!CurentStable)
            {
                LblWeightNonStable.Content = "Вес не стабилен";
                LblWeightNonStable.Visibility = System.Windows.Visibility.Visible;
                return;
            }
            if (CurentWeight <=0)
            {
                LblWeightNonStable.Content = "Недопустимый вес";
                LblWeightNonStable.Visibility = System.Windows.Visibility.Visible;
                return;
            }
            AlohaTSClass.AddScaleDish(CurentBarcode, CurentWeight);
            myOwner.Close();            
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            myOwner.Close();            
        }
    }
}
