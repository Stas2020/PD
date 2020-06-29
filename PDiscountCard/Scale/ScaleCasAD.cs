using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDiscountCard
{
    static public  class ScaleCasAD
    {
      //  static CasAD_AP_DB_EMLib.ScaleClass Sc;
        static  public void  Connect()
        {
        //    Sc = new CasAD_AP_DB_EMLib.ScaleClass();
         //   Sc.Connect(iniFile.ScalePort, 0); 
        }
        static public void DisConnect()
        {
            
           // Sc.Disconnect();
        }
        static public bool Stable()
        {
        //    Sc.Update();
        //    Utils.ToCardLog("ScaleCasAD.Stable() =" + Sc.Stable.ToString());
        //    return (Sc.Stable == 1);
            return false;
        }
        static public int GetWeight()
        {
            try
            {
                //Sc.Update();
                //double W = Sc.Weight;
                //return (int)(W*1000);
                return -1;
            }
            catch(Exception e)
            {
                return -1;
            }
        }


    }
}
