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
    public partial class frmLockScreeen : Window
    {
        System.Timers.Timer TClose;
        DateTime StartTime = DateTime.Now;
        int SecToClose = 100;
        public frmLockScreeen()
        {
            InitializeComponent();
            Topmost = true;
            TClose = new System.Timers.Timer();
            TClose.Interval = 1000;
            TClose.Elapsed += new System.Timers.ElapsedEventHandler(TClose_Elapsed);
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
           
            base.OnClosing(e);
        }

        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);
            try
            {
                StartTime = DateTime.Now;
                TClose.Start();
            }
            catch
            { }
        }
        public void mClose()
        { this.Dispatcher.BeginInvoke(((Action)(() =>
                {
            try
            {
                Utils.ToCardLog(String.Format("frmLockScreeen mClose" ));
                TClose.Stop();
                TClose.Enabled = false;
                this.Hide();
                this.Close();
            }
            catch(Exception e)
            {
                Utils.ToCardLog(String.Format("Error HideLockScreen " + e.Message));
            }
            
                })));
        }

        void TClose_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (!TClose.Enabled)
            {
                return;
            }
            try
            {
                this.Dispatcher.BeginInvoke(((Action)(() =>
                {
                    try
                    {
                        if (SecToClose - (DateTime.Now - StartTime).TotalSeconds < 0)
                        {
                            this.Close();
                        }
                        else
                        {
                            TbTime.Text = (SecToClose - (DateTime.Now - StartTime).TotalSeconds).ToString("0");
                        }
                    }
                    catch
                    { }

                })));
            }
            catch
            { }
        }


    }
}
