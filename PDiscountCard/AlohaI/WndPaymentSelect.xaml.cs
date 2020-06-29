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
using System.Threading;
using System.Windows.Threading;
using System.Windows.Interop;

namespace PDiscountCard.AlohaI
{
    /// <summary>
    /// Interaction logic for WndPaymentSelect.xaml
    /// </summary>
    public partial class WndPaymentSelect : Window
    {
        System.Timers.Timer T1 = new System.Timers.Timer();
        AutoResetEvent waitHandler;
        public WndPaymentSelect()
        {
            InitializeComponent();
            GrdPayments.Visibility = System.Windows.Visibility.Visible;
            GrdCreditAsk.Visibility = System.Windows.Visibility.Hidden;

            this.Closing += new System.ComponentModel.CancelEventHandler(WndPaymentSelect_Closing);

            T1.Interval = 1000;
            T1.Elapsed += new System.Timers.ElapsedEventHandler(T1_Elapsed);
            T1.Start();
            waitHandler = new AutoResetEvent(true);
        }
        public Action<int, Check> Callback;
        public Check Chk;

        public void DoCallBack()
        {
            Callback(Result, Chk);
        }

        bool _shown;

        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);
            Utils.ToCardLog("WndPaymentSelect OnContentRendered " + this.Visibility.ToString());

            if (_shown)
                return;

            _shown = true;


        }

        /*
        public new bool? ShowDialog()
        {
            
            

            return base.ShowDialog(); 
        }
        */



        void WndPaymentSelect_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Utils.ToCardLog("WndPaymentSelect_Closing ");
            T1.Stop();
            T1.Dispose();
        }

       
       

        void T1_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {

            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                 (ThreadStart)delegate()
                 {
                     try
                     {
                         if (T1 != null && T1.Enabled)
                         {

                             Utils.ToCardLog(String.Format("Window WndPaymentSelect  is shown T {0}; L {1}; W{2}; H {3}", Top, Left, Width, Height));
                             IntPtr windowHandle = new WindowInteropHelper(this).Handle;
                             WinApi.ShowTopmost(windowHandle, (int)Top, (int)Left, (int)Width, (int)Height);
                         }
                     }
                     catch (Exception ee)
                     {
                         Utils.ToCardLog("Window WndPaymentSelect  is shown error " + ee.Message);
                     }

                 }
             );

        }
        public int Result = 0;
        public string WaiterName = "";



        public void SetBtnsVis(int State)
        {
            BtnGlory.Visibility = (((byte)State & 0x01) == 1 ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden);
            BtnPlast.Visibility = (((byte)State & 0x02) == 0x02 ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden);
            BtnCred.Visibility = (((byte)State & 0x04) == 0x04 ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden);
        }




        private void BtnGlory_Click(object sender, RoutedEventArgs e)
        {
            Result = 1;
            this.Close();
        }

        private void BtnPlast_Click(object sender, RoutedEventArgs e)
        {
            Result = 2;
            this.Close();
        }

        private void BtnCred_Click(object sender, RoutedEventArgs e)
        {
            ShowCreditAsk();
        }
        private void ShowCreditAsk()
        {
            TbCreditAsk.Text = "Я, " + WaiterName + ", находясь в здравом уме и трезвой памяти, сознательно и добровольно объявляю о своем желании закрыть этот чек на предоплату!";
            GrdCreditAsk.Visibility = System.Windows.Visibility.Visible;
            GrdPayments.Visibility = System.Windows.Visibility.Hidden;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnCredYes_Click(object sender, RoutedEventArgs e)
        {
            Result = 4;
            this.Close();
        }

        private void BtnCredNo_Click(object sender, RoutedEventArgs e)
        {
            GrdCreditAsk.Visibility = System.Windows.Visibility.Hidden;
            GrdPayments.Visibility = System.Windows.Visibility.Visible;
        }
    }
}
