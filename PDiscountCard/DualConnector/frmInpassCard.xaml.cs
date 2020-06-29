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

namespace PDiscountCard.DualConnector
{
    /// <summary>
    /// Interaction logic for frmInpassCard.xaml
    /// </summary>
    public partial class frmInpassCard : Window
    {
        public frmInpassCard()
        {
            InitializeComponent();
        }

        private void btnShortRep_Click(object sender, RoutedEventArgs e)
        {
            DualConnector.DualConnectorMain.ShortReport();
            this.Close();
        }

        private void btnLongRep_Click(object sender, RoutedEventArgs e)
        {
            DualConnector.DualConnectorMain.LongReport();
            this.Close();
        }

        

        private void btnSverka_Click(object sender, RoutedEventArgs e)
        {
            DualConnector.DualConnectorMain.Sverka();
            this.Close();
        }

        private void btnVoid_Click(object sender, RoutedEventArgs e)
        {
            DualConnector.DualConnectorMain.Void(null,"");
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DualConnector.DualConnectorMain.Cancel(null,"");
            this.Close();
        }

        private void btnCopySlip_Click(object sender, RoutedEventArgs e)
        {
            DualConnector.DualConnectorMain.GetCopySlip();
            this.Close();
        }

        private void btnCopyLastSlip_Click(object sender, RoutedEventArgs e)
        {
            DualConnector.DualConnectorMain.GetCopyLastSlip();
            this.Close();
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnCheckLink_Click(object sender, RoutedEventArgs e)
        {
            DualConnector.DualConnectorMain.CheckLink();
            this.Close();
        }
    }
}
