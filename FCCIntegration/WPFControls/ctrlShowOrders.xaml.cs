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

namespace FCCIntegration.WPFControls
{
    /// <summary>
    /// Interaction logic for ctrlShowOrders.xaml
    /// </summary>
    public partial class ctrlShowOrders : UserControl
    {
        public ctrlShowOrders()
        {
            InitializeComponent();
            dataGrid1.IsReadOnly = true;
            dataGrid1.SelectionMode = DataGridSelectionMode.Single;
            dataGrid1.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
        }

        List<OrderData> CurentOrderData;
        public void InitData(List<OrderData> Ord)
        {
           
                CurentOrderData  = Ord;
                SetPage();
        }

        private void SetPage()
        {
            dataGrid1.ItemsSource = CurentOrderData.Where(a => (a.dt > CurentDT) && (a.dt < CurentDT.AddDays(1)));  
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = System.Windows.Visibility.Hidden;
        }

        private void dataGrid1_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            try
            {
                OrderData OD = (OrderData)e.Row.DataContext;
                if (OD.Command == MoneyChangeCommands.EndPayment)
                {
                    e.Row.Background = new SolidColorBrush(Colors.LightGreen);
                }
                else if ((OD.Command == MoneyChangeCommands.CancelPayment)||(OD.Command == MoneyChangeCommands.CasseteRemoved)||(OD.Command == MoneyChangeCommands.CoinMixerRemoved))
                {
                    e.Row.Background = new SolidColorBrush(Colors.LightPink);
                }
            }
            catch
            { }

        }

        private void btnDown_Click(object sender, RoutedEventArgs e)
        {
            ScrollViewer scrollview = FindVisualChild<ScrollViewer>(dataGrid1);
            scrollview.ScrollToVerticalOffset(scrollview.VerticalOffset+1);
        }

        private void btnUp_Click(object sender, RoutedEventArgs e)
        {
            ScrollViewer scrollview = FindVisualChild<ScrollViewer>(dataGrid1);
            scrollview.ScrollToVerticalOffset(scrollview.VerticalOffset - 1);
        }


        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj)
       where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }

        public static childItem FindVisualChild<childItem>(DependencyObject obj)
            where childItem : DependencyObject
        {
            foreach (childItem child in FindVisualChildren<childItem>(obj))
            {
                return child;
            }

            return null;
        }

        DateTime curentDt = DateTime.Now.Date;
        DateTime CurentDT
        {
            set {
                curentDt = value;
                lblDt.Content = value.ToString("dd.MM");
                if (value.Date >= DateTime.Now.Date)
                {
                    btnRight.IsEnabled = false;
                }
                else
                {
                    btnRight.IsEnabled = true;
                }
                SetPage();
            }
            get
            {
                return curentDt;
            }
        }

        private void btnRight_Click(object sender, RoutedEventArgs e)
        {
            CurentDT = CurentDT.AddDays(1);
        }

        private void btnLeft_Click(object sender, RoutedEventArgs e)
        {
            CurentDT = CurentDT.AddDays(-1);
        }
       
    }
   
}
