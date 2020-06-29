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

namespace PDiscountCard.FRSClientApp
{
    /// <summary>
    /// Interaction logic for ctrlShowOrders.xaml
    /// </summary>
    public partial class ctrlShowOChecks : UserControl
    {
        public ctrlShowOChecks()
        {
            InitializeComponent();
            dataGrid1.IsReadOnly = true;
            dataGrid1.SelectionMode = DataGridSelectionMode.Single;
            dataGrid1.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
        }

        private Window OwnerWnd;
        internal void SetOwnerWnd(Window _OwnerWnd)
        {
            OwnerWnd = _OwnerWnd;
        }


        List<FRSSrv.FiskalCheck> CurentOrderData;
        public void InitData(List<FRSSrv.FiskalCheck> Ord)
        {
            
                CurentOrderData  = Ord;
                SetPage();
        }
        
        private void SetPage()
        {
            
            dataGrid1.ItemsSource = CurentOrderData;
            dataGrid1.SelectedIndex = 0;
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            OwnerWnd.Close();
        }

        private void dataGrid1_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            //Change BackColor
            /* 
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
             * */

        }

        private void btnDown_Click(object sender, RoutedEventArgs e)
        {
            ScrollViewer scrollview = FindVisualChild<ScrollViewer>(dataGrid1);
            scrollview.ScrollToVerticalOffset(scrollview.VerticalOffset+1);
            if (dataGrid1.SelectedIndex < dataGrid1.Items.Count-1)
            {
                dataGrid1.SelectedIndex++;
            }
            dataGrid1.Focus();


        }

        private void btnUp_Click(object sender, RoutedEventArgs e)
        {
            ScrollViewer scrollview = FindVisualChild<ScrollViewer>(dataGrid1);
            scrollview.ScrollToVerticalOffset(scrollview.VerticalOffset - 1);
            if (dataGrid1.SelectedIndex > 0)
            {
                dataGrid1.SelectedIndex--;
            }
            dataGrid1.Focus();
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

      
        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid1.SelectedItem != null)
            {
                FRSClientApp.FRSClient.ReprintFChk(((FRSSrv.FiskalCheck)dataGrid1.SelectedItem).Id);
            }
            OwnerWnd.Close();
        }
       
    }
   
}
