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


namespace PDiscountCard.EGAIS
{
    /// <summary>
    /// Interaction logic for wndEGAIS.xaml
    /// </summary>
    partial class wndEGAIS : Window
    {
        private System.Timers.Timer T1 = new System.Timers.Timer();
        public wndEGAIS()
        {
            InitializeComponent();
            System.Windows.Forms.InputLanguage.CurrentInputLanguage = System.Windows.Forms.InputLanguage.FromCulture(new System.Globalization.CultureInfo("en-US"));
            tbCode.Focus();
            T1.Elapsed += new System.Timers.ElapsedEventHandler(T1_Elapsed);
            T1.Interval = 400;
            btnSend.IsEnabled = false;
        }

        void T1_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (!T1.Enabled) return;
            if ((DateTime.Now - LastKeyPress).TotalMilliseconds > 500)
            {
                T1.Stop();

                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                (ThreadStart)delegate()
                {
                    string EgaisCode = "";
                    bool res = EGAISCodeReader.GetEgaisCode(tbCode.Text, out EgaisCode);
                    bool AlreadyScan = EGAISCodeReader.GetEgaisAlreadyScanFrom1C(tbCode.Text);
                    if (EgaisCode != "")
                    {
                        tbName.Text = EgaisCode;
                        if (AlreadyScan)
                        {
                            tbName.Text += Environment.NewLine + "Отсканировано ранее!!!";
                        }
                        else
                        {
                            btnSend.IsEnabled = res;
                        }
                    }
                }
            );
            }
        }
            

        private void tbCode_TextChanged(object sender, TextChangedEventArgs e)
        {
            string c = tbCode.Text;
        }

        DateTime LastKeyPress = DateTime.Now;
        private void tbCode_KeyUp(object sender, KeyEventArgs e)
        {
        tbName.Text = "Думаю...";
            LastKeyPress = DateTime.Now;
            btnSend.IsEnabled = false;
            T1.Start();



        }

        private void tbCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (!T1.Enabled)
            {
                tbCode.Text = "";
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            tbName.Text = "";
            tbCode.Text = "";
            tbCode.Focus();
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            if (btnSend.IsEnabled)
            {
                btnSend.IsEnabled = false;
                if (EGAISCodeReader.SendEgaisTo1C(tbCode.Text))
                {
                    tbName.Text = tbName.Text + Environment.NewLine + "Успешно отправлено";
             
                }

                tbCode.Focus();
            }
        }

      
        private void btnBeer_Click(object sender, RoutedEventArgs e)
        {
            
            EGAISSrv2.Element[] Data = EGAISCodeReader.GetEgaisListFrom1C();
            if ((Data!=null) && (Data.Count()>0))
            {
                ctrlBeer.Init(Data, this);
                StAlc.Visibility = System.Windows.Visibility.Hidden;
                GrBeer.Visibility = System.Windows.Visibility.Visible;
            }
              else
            {
                tbName.Text = "Для вашего подразделения нет остатков пива";
            }
           
            tbCode.Focus();

        }

        internal void BeerExit()
        {
            StAlc.Visibility = System.Windows.Visibility.Visible;
            GrBeer.Visibility = System.Windows.Visibility.Hidden;
            tbCode.Focus();
        }

        internal void SendBeer(EGAISSrv2.Element SelectedBeer)
        {
            bool res = EGAISCodeReader.SendBeerEgaisTo1C(SelectedBeer.AlcoCode);
            StAlc.Visibility = System.Windows.Visibility.Visible;
            GrBeer.Visibility = System.Windows.Visibility.Hidden;

            if (res)
            {
                tbName.Text = SelectedBeer.AlcoName + " успешно отправленно";
            }
            else
            {
                tbName.Text = "Ошибка отправки!!!";
                tbName.Background = new SolidColorBrush(Colors.Red);
            }
            tbCode.Focus();
        }

    }
}
