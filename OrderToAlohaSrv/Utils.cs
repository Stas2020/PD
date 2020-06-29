using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;



namespace OrderToAlohaSrv
{
    static class Utils
    {
        static EventWaitHandle EditCheckWaitHandle = new AutoResetEvent(true);
        public static void ToLog(string Str)
        {
            EditCheckWaitHandle.WaitOne();
            try
            { 
                //if (!Directory.Exists( @""))
                using (StreamWriter sw = new StreamWriter(@"C:\aloha\check\OrderToAlohaLog.txt",true))
                {
                    sw.WriteLine(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss ") + " [" + Thread.CurrentThread.ManagedThreadId + "] " + Str);
                }
            
            }
            catch
            { 
            
            }
            EditCheckWaitHandle.Set();
        
        }

    }
}
