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
    /// Interaction logic for frmFCCUserDialog.xaml
    /// </summary>
    public partial class frmFCCUserDialog : Window
    {
        public frmFCCUserDialog()
        {
            InitializeComponent();
        
        }
        public ctrlMoneyDialog GetMoneyDialog()
        {
            return mMoneyDialog;
        }
        public void SetBill(int Summ)
        {
            mMoneyDialog.SetTotal(Summ);
        }

        public void SetDishez(FCCCheck Chk)
        {
            if (Chk != null)
            {
                ctrlOrder.SetDishez(Chk);
            }
        }
        public void ClearOrder()
        {
            this.Dispatcher.Invoke((Action)(() =>
                {
                    ctrlOrder.ClearDishez();
                }));
        }
        public void ClearMoney()
        {
            this.Dispatcher.Invoke((Action)(() =>
                {
                    mMoneyDialog.SetTotal(0);
                    mMoneyDialog.SetAlredy(0);
                    mMoneyDialog.SetChange(0);
                    mMoneyDialog.SetDeposit(0);
                }));
            
        }

    }
}
