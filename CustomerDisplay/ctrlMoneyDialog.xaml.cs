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

namespace CustomerDisplay
{
    /// <summary>
    /// Interaction logic for ctrlMoneyDialog.xaml
    /// </summary>
    public partial class ctrlMoneyDialog : UserControl
    {
        public ctrlMoneyDialog()
        {
            InitializeComponent();
        }
        public void SetTotal(int Summ)
        {
            this.Dispatcher.Invoke((Action)(() =>
                {
            MonTotal.SetMoney(Summ);

                }));
        }
        public void SetChange(int Summ)
        {
            this.Dispatcher.Invoke((Action)(() =>
                {
            MonChange.SetMoney(Summ);
                    }));
        }
        public void SetDeposit(int Summ)
        {
            this.Dispatcher.Invoke((Action)(() =>
                {
            MonDeposit.SetMoney(Summ);
                    }));
        }
        public void SetAlredy(int Summ)
        {
            this.Dispatcher.Invoke((Action)(() =>
                {
            MonAlready.SetMoney(Summ);
                    }));
        }
        public void SetStatus(string StatusMsg)
        {
            this.Dispatcher.Invoke((Action)(() =>
                {
                    lblStatus.Text = StatusMsg;
                }));


            
        }
    }
}
