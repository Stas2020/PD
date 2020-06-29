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
    /// Interaction logic for frmMessage.xaml
    /// </summary>
    public partial class frmHandsPayment : Window
    {
        public frmHandsPayment()
        {
            InitializeComponent();
            
        }

        public int ReqType = 0;


        private void btnNal_Click(object sender, RoutedEventArgs e)
        {
            ReqType = 1;
            this.Close();
        }

        private void btnCard_Click(object sender, RoutedEventArgs e)
        {
            ReqType = 2;
            this.Close();
        }

        

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
