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
    /// Interaction logic for FrmCashIn.xaml
    /// </summary>
    public partial class FrmCashIn : Window
    {
        public FrmCashIn()
        {
            InitializeComponent();
            ctrlDigitalKeyBoard1.OnOk += new ctrlDigitalKeyBoard.OkEventHandler(ctrlDigitalKeyBoard1_OnOk);
            ctrlDigitalKeyBoard1.OnCancel += new ctrlDigitalKeyBoard.OkEventHandler(ctrlDigitalKeyBoard1_OnCancel);
        }

        void ctrlDigitalKeyBoard1_OnCancel(object sender, double Value)
        {
            
            this.Close();
        }

        void ctrlDigitalKeyBoard1_OnOk(object sender, double Value)
        {
            ToShtrih.CashIncome(Convert.ToDecimal(Value));
            FCC.WriteIncomeInfoToFccMoneyLog(Convert.ToDecimal(Value));
            this.Close();
        }
    }
}
