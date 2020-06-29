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
    /// Interaction logic for frmMessageAsinc.xaml
    /// </summary>
    public partial class frmMessageAsync : Window
    {
        System.Timers.Timer T1 = new System.Timers.Timer();

        private bool CloseDisabled = true;

        public frmMessageAsync(string Mess)
            : this(Mess, 0)
        {
        }


        public frmMessageAsync(string Mess, int delay)
        {
            InitializeComponent();

            this.Closing += new System.ComponentModel.CancelEventHandler(frmMessageAsync_Closing);
            T1.Interval = 500;
            T1.Elapsed += new System.Timers.ElapsedEventHandler(T1_Elapsed);
            T1.Start();
            CurrentMess = Mess;
            LblMsg.Text = Mess;
            this.Topmost = true;

            if (delay == 0)
            {

                this.Show();
            }
            else
            {
                _delayEnabled = true;
            }
            _delay = delay;
        }

        void frmMessageAsync_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            T1.Stop();
            
        }

        internal void SetMessage(string Mess)
        {
            this.Dispatcher.Invoke((Action)(() =>
                       {
                           try
                           {
                               CurrentMess = Mess;
                               this.Show();
                           }
                           catch
                           { }
                       }));
        }

        private int _delay = 0;
        private bool _delayEnabled = false;

        string CurrentMess = "";

        int AnimCount = 0;
        void T1_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                if (_delayEnabled)
                {
                    _delay -= (int)T1.Interval;
                    if (_delay <= 0)
                    {
                        this.Dispatcher.Invoke((Action)(() =>
                        {
                            this.Show();
                        }));
                        _delayEnabled = false;
                    }
                }
                if (!T1.Enabled) { return; }
                string Dob = "".PadRight(AnimCount, "."[0]);
                this.Dispatcher.Invoke((Action)(() =>
                    {
                        LblMsg.Text = CurrentMess + Dob;
                    }
                        ));
            }
            catch
            { }
            AnimCount++;
            if (AnimCount == 4)
            {
                AnimCount = 0;
            }
        }

        public void StopWait(string Mess)
        {
            this.Dispatcher.Invoke((Action)(() =>
                    {
                        CloseDisabled = false;
                        this.Show();
                        T1.Stop();
                        CurrentMess = Mess;
                        LblMsg.Text = Mess;
                    }));
        }


        public void HideFrm()
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                T1.Stop();
                CloseDisabled = false;
                this.Hide();
                
                //CurrentMess = Mess;
                //LblMsg.Text = Mess;
            }));
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            if (CloseDisabled)
            {
                _delayEnabled = false;
                this.Hide();
            }
            else
            {
                this.Close();
            }
        }

    }
}
