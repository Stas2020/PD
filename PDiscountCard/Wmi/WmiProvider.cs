using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management;
using System.Threading;

namespace PDiscountCard.Wmi
{
    public static  class WmiProvider
    {
        static public void RestartRedirectSrv(string CompName, string Login, string Password, string Params)
        {
            try
            {
                Utils.ToCardLog("RestartRedirectSrv CompName: " + CompName.ToString());          
                ConnectionOptions connection = new ConnectionOptions();
                connection.Username = Login;
                connection.Password = Password;
                connection.Authority = "ntlmdomain:";

                ManagementScope scope = new ManagementScope(
                    "\\\\" + CompName + "\\root\\CIMV2", connection);
                scope.Connect();

                SelectQuery query = new SelectQuery("select * from Win32_Service ");

                ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);
                ManagementObjectCollection queryCollection = searcher.Get();
                foreach (ManagementObject m in queryCollection)
                {
                    //System.Windows.Forms.MessageBox.Show(m.GetPropertyValue("Name").ToString().Trim() );
                    if (m.GetPropertyValue("Name").ToString().Trim() == "Aloha Enterprise Redirector Service")
                    {
                        //   m.InvokeMethod ("StopService")
                        ManagementBaseObject outParams = m.InvokeMethod("StopService", null, null);
                        // Console.WriteLine("Out parameters:");
                        // Console.WriteLine("ProcessId: " + outParams["ProcessId"]);
                        //  Console.WriteLine("ReturnValue: " + outParams["ReturnValue"]);
                        Utils.ToLog  ("Стоп: " + outParams["ReturnValue"].ToString());

                        Thread.Sleep(2000); 

                        ManagementBaseObject outParams2 = m.InvokeMethod("StartService", null, null);
                        // Console.WriteLine("Out parameters:");
                        // Console.WriteLine("ProcessId: " + outParams["ProcessId"]);
                        //  Console.WriteLine("ReturnValue: " + outParams["ReturnValue"]);
                        Utils.ToLog("Стaрт: " + outParams2["ReturnValue"].ToString());

                    }
                }




            }
            catch (ManagementException err)
            {
                Utils.ToLog("An error occurred while trying to execute the WMI method: " + err.Message);
                // MessageBox.Show("An error occurred while trying to execute the WMI method: " + err.Message);
            }
            catch (System.UnauthorizedAccessException unauthorizedErr)
            {
                Utils.ToLog("Connection error (user name or password might be incorrect): " + unauthorizedErr.Message);
                //MessageBox.Show("Connection error (user name or password might be incorrect): " + unauthorizedErr.Message);
            }
        }


    }
}
