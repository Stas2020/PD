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
    /// Interaction logic for frmCassir.xaml
    /// </summary>
    public partial class frmCassir : Window
    {
        public frmCassir()
        {
            InitializeComponent();

            Topmost = false;

        }

        public delegate void ChangeCancelEventHandler(object owner);
        public event ChangeCancelEventHandler OnCancelChange;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
          

            if (btnCancelChange.Content.ToString() == "Выход")
            {
                RenameCancelBtn("Отменить оплату");
                this.Hide();
            }

            if (OnCancelChange != null)
            {
                OnCancelChange(this);
            }
            if (MainClass2.IsSync)
            {
              //  this.Hide();

            }
            
        }
        public ctrlMoneyDialog GetMoneyDialog()
        {
            return mMoneyDialog;
        }
        public void Init(FCCCheck Chk, int Summ)
        {
            this.Dispatcher.Invoke((Action)(() =>
                {

                    if (MainClass2.IsSync)
                    {
                        btnExit.Visibility = System.Windows.Visibility.Hidden;
                        btnShowOrders.Visibility = System.Windows.Visibility.Hidden;
                    }
                    if (Chk != null)
                    {

                        ctrlOrder.SetDishez(Chk);
                    }
                    mMoneyDialog.SetTotal(Summ);
                    mMoneyDialog.SetChange(0);
                    SetStatus("Идет процесс оплаты");
                }));
        }
        /*
        public void CloseDisp()
        {
            this.Dispatcher.Invoke((Action)(() =>
                {
                    this.Close();
                }
            ));
        }
         * */
        public void SetStatus(string StatusMsg)
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                lblStatus.Text = StatusMsg;
            }));



        }

        public   void DispHide()
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                this.Hide();
            }));
            
        }

        public void RenameCancelBtn(string BtnName)
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                btnCancelChange.Content = BtnName;
            }));



        }

        public void ClearCheck()
        {
            //SetStatus("Нет заказов для оплаты");
            ClearMoney();
            ctrlOrder.ClearDishez();
        
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

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        public void ChangeCanceled(int Change)
        {

            

            this.Dispatcher.Invoke((Action)(() =>
           {
               if (Change == 0)
               {
                   RenameCancelBtn("Выход"); 
                   SetStatus("Оплата отменена");
                   btnCancelChange.IsEnabled = true;
               }
               else
               {
                   SetStatus(String.Format("Оплата отменена. Необходимо вернуть гостю деньги в размере {0} руб.",Change/100));
               }
               ClearCheck();
               ClearMoney();
           }));
        }
        
        private void btnShowOrders_Click(object sender, RoutedEventArgs e)
        {
            ctrlShowOrders1.InitData(Utils.GetOrdersFromFile());

            ctrlShowOrders1.Visibility = System.Windows.Visibility.Visible;

        }
        
        internal void SetCancelButtonEnabled(bool val)
        {
            this.Dispatcher.Invoke((Action)(() =>
           {
               btnCancelChange.IsEnabled = val;
           }));
        }
    }
}
