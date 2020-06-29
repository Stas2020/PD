using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace FiskalDrivers
{
    static class Utils
    {

        public static void ToLog(string Mess)
        {
            string DirPath = @"C:\aloha\check\Discount\FRLog";
            try
            {

                if (!Directory.Exists(DirPath))
                {
                    Directory.CreateDirectory(DirPath);
                }
                using (StreamWriter sw = new StreamWriter(DirPath+@"\FRLog"+DateTime.Now.ToString("ddMMyy")+".txt", true))
                {
                    sw.WriteLine(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss ") + Mess);
                }

            }
            catch
            {

            }
        }
        public static void ToLog(string Mess,int LogLevel)
        {

        }
       
    }
}
