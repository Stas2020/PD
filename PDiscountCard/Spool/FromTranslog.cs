using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace PDiscountCard.Spool
{
    static  class FromTranslog
    {
        public  static void GenSpoolFromTr()
        { 
            List<Check> AllChk = AlohaTSClass.GetAllChecks();
            foreach (Check ch in AllChk.OrderBy(a=>a.SystemDate))
            {
                AddToSpoolFile(ch);
            }
        }
        static string P = @"C:\aloha\check\discount\tmp\sp_copy.chk";

        static public void AddToSpoolFile(Check Chk)
        {
            try
            {
                string SpoolString = SpoolCreator.CreateSpoolStrings2(Chk);
                    StreamWriter SW2 = new StreamWriter(P, true, Encoding.GetEncoding(1251));
                    SW2.Write(SpoolString);
                    SW2.Close();
                
            }
            catch (Exception e)
            {
               
            }
        }
    }
}
