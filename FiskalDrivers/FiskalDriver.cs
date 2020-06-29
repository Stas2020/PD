using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FiskalDrivers
{
    public static class FiskalDriver
    {
        static IFiskalDriver _driver;
        public static bool CreateFiskalDriver(int FDType)
        {
            Utils.ToLog("CreateFiskalDriver" + FDType);
            try
            {
                if (FDType == 1)
                {
                    _driver = new AtollDrviver();
                    Utils.ToLog("Активирую драйвер Биксилона");
                }
                else
                {
                    Utils.ToLog("Не найден тип драйвера " + FDType);
                }

                return _driver.Create();
            }
            catch
            {
                return false;
            }
        }

        public static void Connect(int LUNum)
        {
            _driver.Connect(LUNum);
        }

        public static bool CloseCheck(FiskalCheck FCh)
        {
            if (KeeperLogic.GetColor(FCh))
            {
               return  _driver.CloseCheck(FCh);
            }
            return true ;
        }

        public static bool PrintPreCheck(FiskalCheck FCh)
        {
            
            return _driver.PrintPreCheck(FCh);
            
            
        }

        public static void PrintString(string Str)
        {
            _driver.PrintString(Str);
        }

        public static void PrintXReport()
        {
            _driver.PrintXReport();
        }
        public static void PrintZReport()
        {
            _driver.PrintZReport();
        }


    }
    interface IFiskalDriver
    {
        bool CloseCheck(FiskalCheck FCh);
        bool PrintPreCheck(FiskalCheck FCh);

        bool Create();

        bool Connect(int LUNumber);

        void PrintString(string Str);
        void PrintXReport();
        void PrintZReport();
        

    }
}
