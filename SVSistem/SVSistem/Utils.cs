using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SVSistem
{
    class Utils
    {
        static string LogFilesPath = @"C:\Aloha\check\Discount\logs\";
        static string LogName = "SVLog";
        public static void ToLog(string msg)
        {
            string FullFileName = LogFilesPath + LogName + "_"  + DateTime.Now.ToString("ddMMyy") + ".log";

            if (!(File.Exists(FullFileName)))
            {

                FileStream FS = File.Create(FullFileName);
                FS.Close();
            }

            StreamWriter SW = new StreamWriter(FullFileName, true);

            SW.WriteLine(msg);

            SW.Close();
        }

    }
}
