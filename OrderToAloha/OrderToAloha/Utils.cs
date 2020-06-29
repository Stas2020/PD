using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace OrderToAloha
{
    static class Utils
    {
        internal static void ToLog(string Str)
        {
            try
            { 
                //if (!Directory.Exists( @""))
                using (StreamWriter sw = new StreamWriter(@"OrderToAlohaLog.txt",true))
                {
                    sw.WriteLine(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss ") + Str);
                }
            
            }
            catch
            { 
            
            }
        
        
        }

    }
}
