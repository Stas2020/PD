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

namespace PDiscountCard.EGAIS
{
    /// <summary>
    /// Interaction logic for ctrlEGAISBeer.xaml
    /// </summary>
    public partial class ctrlEGAISBeer : UserControl
    {
        public ctrlEGAISBeer()
        {
            InitializeComponent();
        }
        wndEGAIS Ownerwnd;
        public void Init(EGAISSrv2.Element[] Data,wndEGAIS _Ownerwnd )
        {
            Ownerwnd = _Ownerwnd;
            dataGrid1.ItemsSource = Data;
            dataGrid1.SelectedIndex = 0;
            btnPrint.IsEnabled = true;
        }

        private void btnDown_Click(object sender, RoutedEventArgs e)
        {
            ScrollViewer scrollview = FindVisualChild<ScrollViewer>(dataGrid1);
            scrollview.ScrollToVerticalOffset(scrollview.VerticalOffset + 1);
            if (dataGrid1.SelectedIndex < dataGrid1.Items.Count - 1)
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
        public static childItem FindVisualChild<childItem>(DependencyObject obj)
          where childItem : DependencyObject
        {
            foreach (childItem child in FindVisualChildren<childItem>(obj))
            {
                return child;
            }

            return null;
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
        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            if (btnPrint.IsEnabled)
            {
                btnPrint.IsEnabled = false;
                if ((Ownerwnd != null) && (dataGrid1.SelectedItem != null))
                {
                    Ownerwnd.SendBeer((EGAISSrv2.Element)dataGrid1.SelectedItem);
                }
            }
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Ownerwnd.BeerExit();
        }
    }
}
