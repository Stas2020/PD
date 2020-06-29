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
    /// Interaction logic for ctrIncass.xaml
    /// </summary>
    public partial class ctrIncass : UserControl
    {
        public ctrIncass()
        {
            InitializeComponent();
        }

        internal void AllBtnsEnableState(bool State)
        {
            btnInkasCancel.IsEnabled = State;
            btnInkasVigr.IsEnabled = State;
            btnUnlock.IsEnabled = State;
        }

        internal void SetBtnUnlockTxt(string txt)
        {
               this.Dispatcher.Invoke((Action)(() =>
                {
            txtUnlock.Text = txt;
                }));
        }

        internal void SetBtnUnlockEnable(bool val)
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                btnUnlock.IsEnabled = val;
                
            }));
        }

        private void btnInkasCancel_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnInkasVigr_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnUnlock_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
