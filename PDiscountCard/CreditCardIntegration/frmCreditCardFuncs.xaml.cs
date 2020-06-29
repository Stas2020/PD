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
    /// Interaction logic for frmInpassCard.xaml
    /// </summary>
    public partial class frmCreditCardFuncs : Window
    {
        public frmCreditCardFuncs()
        {
            InitializeComponent();
            if (iniFile.Arcus4Enabled)
            {
                btnCasirMenu.Visibility = System.Windows.Visibility.Hidden;
                btnCopyLastSlip.Visibility = System.Windows.Visibility.Visible;
                btnCopySlip.Visibility = System.Windows.Visibility.Visible;
            }

            if (iniFile.Arcus4Enabled &&  iniFile.Arcus4ShowShortReportBtn)
            {
                btnShortRep.Visibility = System.Windows.Visibility.Visible;
                
            }

            if (iniFile.Arcus2ShowShortReportBtn)
            {
                btnShortRep.Visibility = System.Windows.Visibility.Visible;
            }
            if (iniFile.SBCreditCardEnabled)
            {
                btnShortRep.Visibility = System.Windows.Visibility.Visible;
                btnCasirMenu.Visibility = System.Windows.Visibility.Hidden;
                btnCopyLastSlip.Visibility = System.Windows.Visibility.Visible;
                btnCheckLink.Visibility = System.Windows.Visibility.Visible;
            }

            if (iniFile.VerifoneEnabled)
            {
                btnCasirMenu.Visibility = System.Windows.Visibility.Hidden;
                btnCopyLastSlip.Visibility = System.Windows.Visibility.Visible;
                btnCopySlip.Visibility = System.Windows.Visibility.Visible;
            }

        }

        private void btnShortRep_Click(object sender, RoutedEventArgs e)
        {
            //DualConnector.DualConnectorMain.ShortReport();
            CreditCardAlohaIntegration.PrintShortReport();
            this.Close();
        }

        private void btnLongRep_Click(object sender, RoutedEventArgs e)
        {
            CreditCardAlohaIntegration.PrintLongReport();
            this.Close();
        }

        

        private void btnSverka_Click(object sender, RoutedEventArgs e)
        {
            //DualConnector.DualConnectorMain.Sverka();
            CreditCardAlohaIntegration.RunSverka();
            this.Close();
        }

        private void btnVoid_Click(object sender, RoutedEventArgs e)
        {
            //DualConnector.DualConnectorMain.Void(null,"");
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            //DualConnector.DualConnectorMain.Cancel(null,"");
            this.Close();
        }

        private void btnCopySlip_Click(object sender, RoutedEventArgs e)
        {
           // DualConnector.DualConnectorMain.GetCopySlip();


            CreditCardAlohaIntegration.GetSipCopy();
            this.Close();
        }

        private void btnCopyLastSlip_Click(object sender, RoutedEventArgs e)
        {
            //DualConnector.DualConnectorMain.GetCopyLastSlip();
            CreditCardAlohaIntegration.GetLastSlipCopy();
            this.Close();
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnCheckLink_Click(object sender, RoutedEventArgs e)
        {
            //DualConnector.DualConnectorMain.CheckLink();
            CreditCardAlohaIntegration.TestPinPad();
            this.Close();
        }

        private void btnCasirMenu_Click(object sender, RoutedEventArgs e)
        {
            CreditCardAlohaIntegration.ShowCassirMnu();
            this.Close();
        }
    }
}
