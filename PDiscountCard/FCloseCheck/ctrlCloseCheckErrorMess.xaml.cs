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

namespace PDiscountCard
{
    /// <summary>
    /// Interaction logic for ctrlCloseCheckErrorMess.xaml
    /// </summary>
    public partial class ctrlCloseCheckErrorMess : UserControl
    {
        public ctrlCloseCheckErrorMess()
        {
            InitializeComponent();
        }
        private Window OwnerWnd;
        internal void SetOwnerWnd(Window _OwnerWnd)
        {
            OwnerWnd = _OwnerWnd;
        }

        internal void SetTxt(string txt)
        {
            TbMessage.Text = txt;   
        }
        internal int Result = 0;
        private void BtContinuePrint_Click(object sender, RoutedEventArgs e)
        {
            Result = 1;
            OwnerWnd.Close();
        }

        

        private void BtCancelPrint_Click(object sender, RoutedEventArgs e)
        {
            Result = 2;
            OwnerWnd.Close();
        }
    }
}
