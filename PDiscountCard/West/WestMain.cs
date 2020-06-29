using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Runtime.InteropServices;

namespace PDiscountCard.West
{
    public static class WestMain
    {
        [DllImport("westreport.dll", CharSet = CharSet.Unicode)]
        static extern void Show(IntPtr param, String Fs);

        [DllImport("westreport.dll", CharSet = CharSet.Unicode)]
        static extern void Pager(IntPtr param, String ServerName);

        

        public static void ShowPagerDialog()
        {
            //Уезжаем в другой поток, чтобы не глючило
            System.Threading.Thread MyThread =
                 new System.Threading.Thread(delegate() { mShowPagerDialog(); });
            MyThread.Start();

        }
        public static void mShowPagerDialog()
        {
            //Уезжаем в другой поток, чтобы не глючило
            AlohaTSClass.CheckWindow();
            int EmpNum = AlohaTSClass.AlohaCurentState.EmployeeNumberCode;
            Pager((IntPtr)EmpNum, Utils.GetFileServerName());

        }

        public static void ShowSaleReport(int Param)
        {
            //Уезжаем в другой поток, чтобы не глючило
            System.Threading.Thread MyThread =
                 new System.Threading.Thread(delegate() { mShowSaleReport(Param); });
            MyThread.Start();

        }
        public static void mShowSaleReport(int Param)
        {
            try
            {
                Utils.ToCardLog("ShowSaleReport ");
                //Show((IntPtr)Param,Utils.GetFileServerName);
               // Show((IntPtr)Param, "PiskovStend1".ToArray());
                //Show((IntPtr)Param, "PiskovStend1");
                Show((IntPtr)Param,Utils.GetFileServerName());
              
            }
            catch(Exception e)
            {
                Utils.ToCardLog("Error WestMain.ShowSaleReport " + e.Message);
            }
        
        }
    }
}
