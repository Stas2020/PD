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

namespace PDiscountCard.DualConnectorIntegration
{
    /// <summary>
    /// Interaction logic for frmInpasVoid.xaml
    /// </summary>
    public partial class frmInpasVoid : Window
    {
        public frmInpasVoid()
        {
            InitializeComponent();
            ctrlDigitalKeyBoard1.OkEnable = false;
            ctrlDigitalKeyBoard1.CancelEnable = false;
        }
        public int Res = 0;
        public string RRN = "";
        private void btnVoid_Click(object sender, RoutedEventArgs e)
        {
            //DualConnector.DualConnectorMain.Void();
            RRN =  ctrlDigitalKeyBoard1.CurentText;
            Res = 1;
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            //DualConnector.DualConnectorMain.Cancel();
            RRN =  ctrlDigitalKeyBoard1.CurentText;
            Res = 2;
            this.Close();
        }
    }
}
