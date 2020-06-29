using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Threading;
using System.Windows.Threading;
using System.Windows.Interop;


namespace PDiscountCard.PDSystem
{
    internal class ModalWindowsForegraund
    {
        static List<Window> Wnds = new List<Window>();
        static internal AlertModalWindow GetModalWindow()
        {
            AlertModalWindow wnd = new AlertModalWindow();
            
            wnd.Closing += new System.ComponentModel.CancelEventHandler(wnd_Closing);
            AddWndToNonHide(wnd);
            return wnd;
        }

        static internal AlertModalWindow GetModalWindow(System.Windows.Controls.UserControl ctrl)
        {

            AlertModalWindow wnd = GetModalWindow();
            wnd.SetContent(ctrl);
            return wnd; 
        }

        static void wnd_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ((Window)sender).Closing -= new System.ComponentModel.CancelEventHandler(wnd_Closing);
            CloseAlertModalWindow((Window)sender);
        }

        static void AddWndToNonHide(Window wnd)
        {
            Wnds.Add(wnd);
        }

        static internal void CloseAlertModalWindow(Window wnd)
        {
            try
            {
                lock (Wnds)
                {
                    if (Wnds.Contains(wnd))
                    {
                        Wnds.Remove(wnd);
                    }
                }
            }
            catch { }
            
        }

        static Thread ThreadStopWndForegroundLoop;
        static internal void StartWndForegroundLoop()
        {
            ThreadStopWndForegroundLoop = new Thread(WndForegroundLoop);
            ThreadStopWndForegroundLoop.IsBackground = true;
            ThreadStopWndForegroundLoop.Start();

        }
        static internal void StopWndForegroundLoop()
        {
            WndForegroundLoopExit = true;
        }
        static bool WndForegroundLoopExit = false;
        static void WndForegroundLoop()
        {
            while (!WndForegroundLoopExit)
            {
                lock (Wnds)
                {
                    if (Wnds.Where(a => a.Visibility == Visibility.Visible && a != null).Count() > 0)
                    {
                        Window wnd = Wnds.Where(a => a.Visibility == Visibility.Visible && a != null).Last();

                        wnd.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                  (ThreadStart)delegate()
                  {
                      try
                      {
                          //Utils.ToCardLog(String.Format("Window WndPaymentSelect  is shown T {0}; L {1}; W{2}; H {3}", Top, Left, Width, Height));
                          IntPtr windowHandle = new WindowInteropHelper(wnd).Handle;
                          WinApi.ShowTopmost(windowHandle, (int)wnd.Top, (int)wnd.Left, (int)wnd.Width, (int)wnd.Height);
                      }
                      catch (Exception ee)
                      {
                          Utils.ToCardLog("Window wnd  is shown error " + ee.Message);

                      }

                  }
              );
                    }
                }
                Thread.Sleep(1000);

            }
        }

    }
}
