using System;
using System.Collections.Generic;

using System.Text;

namespace PDiscountCard
{
    /*
    static class ToBase:ServiceReference1.WriteVisitDiscountObj  
    {
        static  internal ToBase()
        { }
        ServiceReference1.WriteVisitDiscountObjClient ob = new PDiscountCard.ServiceReference1.WriteVisitDiscountObjClient();
        ServiceReference1.WriteVizitRequest r = new PDiscountCard.ServiceReference1.WriteVizitRequest ();
        ServiceReference1.WriteVizitResponse resp = new PDiscountCard.ServiceReference1.WriteVizitResponse();
        
        internal void DoVizit()
        {
            //ob.WriteVizit( 
        }
    }
     * */

    static public class ToBase 
    {
        static  vfiliasesb0.WriteVisitDiscountService  BaseConnect;
        //static vfiliasesb0. BaseConnect;

        static internal  void Dispose()
        {
            if (BaseConnect == null)
            {
                
            }
        }

        static internal bool FirstInit()
        {
            try
            {

                BaseConnect = new PDiscountCard.vfiliasesb0.WriteVisitDiscountService();
                BaseConnect.Timeout = 10000;
                Utils.ToLog("Активировал веб-сервис карт гостя");
                
                //Utils.ReadVisitsFromFile();  

                return true;
            }
            catch (Exception e)
            {
                Utils.ToLog("[ERROR] [FirstInit] Нет связи с вебсервисом. " + e.Message);
                return false;
            }
        }


        

        static internal int DoVizit(string Prefix, string CardNum, int CodSh, string CheckNum, int TermNum,
            DateTime CDT, out int CountV, out int CountD, out int VisitTotal, out int DayTotal)
        {
            return 
                DoVizit(Prefix, CardNum, CodSh, CheckNum, TermNum, CDT, false, out CountV, out CountD, out VisitTotal, out DayTotal);
        }

      
        static internal int DoVizit2(string Prefix, string CardNum, int CodSh, string CheckNum, int TermNum, decimal Summ,
            DateTime CDT, out int CountV, out int CountD, out int VisitTotal, out int DayTotal, out int Gold, int SecondVizit)
        { 
        return
             DoVizit2(Prefix, CardNum, CodSh, CheckNum, TermNum,  Summ, CDT, false, out CountV, out CountD, out VisitTotal, out DayTotal, out Gold,  SecondVizit);
        }



     



        static public int DoVizit2(string Prefix, string CardNum, int CodSh, string CheckNum, int TermNum,decimal Summ,
            DateTime CDT, bool FromFile, out int CountV, out int CountD, out int VisitTotal, out int DayTotal, out int Gold, int SecondVizit )
        {
            CountV = 0;
            CountD = 0;
            VisitTotal = 50;
            DayTotal = 60;
            Gold = 0;
            int? _CountV = (int?)CountV;
            int? _CountD = (int?)CountD;
            int? _VisitTotal = (int?)VisitTotal;
            int? _DayTotal = (int?)DayTotal;
            int? _Gold = (int?)Gold;
            int Count = 1;
            try
            {
                if (BaseConnect == null)
                {
                    //BaseConnect.
                    FirstInit();
                }
                if (SecondVizit > 0)
                {
                    Count = 2;
                }
                BaseConnect.WriteVizitNew(Prefix, CardNum, CodSh, CheckNum, TermNum, CDT, Summ, Count, out _CountV, out _CountD);
                Utils.ToLog("[DoVizit] Зарегистрировал проводку карты." + Prefix + " " + CardNum + " Количество оставшихся посещений: " + _CountV.ToString() + " Количество оставшихся дней: " + _CountD.ToString() + "Номер чека: " + CheckNum);
                CountV = (int)_CountV;
                CountD = (int)_CountD;
                Gold = (int)_Gold;
                return 1;

            }
            catch (Exception e)
            {
                Utils.ToLog("[ERROR] [DoVizit] Неудачный вызов удаленной процедуры. " + e.Message);
                if (!FromFile)
                {
                    CardMooverInfo CMI = new CardMooverInfo()
                    {
                        CardNum = CardNum,
                        CDT = DateTime.Now,
                        CheckNum = CheckNum,
                        CodSh = CodSh,
                        Prefix = Prefix,
                        TermNum = TermNum,
                        Count =Count,
                        Summ = Summ
                    };
                    Utils.WriteVisitToFile2(CMI);
                }
                return -1;
            }
        }   

        static internal int DoVizit(string Prefix, string CardNum, int CodSh, string CheckNum,int TermNum,
            DateTime CDT, bool FromFile, out int CountV, out int CountD, out int VisitTotal, out int DayTotal)
        {



            CountV = 0;
            CountD = 0;
            VisitTotal = 50;
            DayTotal = 60;
            int? _CountV = (int?)CountV;
            int? _CountD = (int?)CountD;
            int? _VisitTotal = (int?)VisitTotal;
            int? _DayTotal = (int?)DayTotal;
           
            try
            {
                if (BaseConnect == null)
                {
                    //BaseConnect.
                    FirstInit();
                }

                
                BaseConnect.WriteVizit(Prefix, CardNum, CodSh, CheckNum, TermNum,
                CDT, out _CountV, out _CountD);
                Utils.ToLog("[DoVizit] Зарегистрировал проводку карты." + Prefix +" " +CardNum + " Количество оставшихся посещений: " + _CountV.ToString() + " Количество оставшихся дней: " + _CountD.ToString());
                CountV = (int)_CountV;
                CountD = (int)_CountD;
                return 1;

            }
            catch (Exception e)
            {
                Utils.ToLog("[ERROR] [DoVizit] Неудачный вызов удаленной процедуры. " + e.Message);
                if (!FromFile)
                {
                    CardMooverInfo CMI = new CardMooverInfo()
                    {
                        CardNum = CardNum,
                        CDT = DateTime.Now,
                        CheckNum = CheckNum,
                        CodSh = CodSh,
                        Prefix = Prefix,
                        TermNum = TermNum
                    };
                    Utils.WriteVisitToFile2(CMI);
                }
                return -1;
            }
        }   
        static internal int DoVizitAsink(string Prefix, string CardNum, int CodSh, string CheckNum, int TermNum,
         DateTime CDT)
        {

            /*
            try
            {
                BaseConnect = new PDiscountCard.vfiliasesb0.WriteVisitDiscountService();
                Utils.ToLog("[DoVizit] Соединение установленно.");
            }
            catch (Exception e)
            {
                Utils.ToLog("[DoVizit] Нет связи с вебсервисом. " + e.Message);
                return -1;
            }
            */

            try
            {
                if (BaseConnect == null)
                {
                    FirstInit();
                }

                DateTime? _CDT =  (DateTime?)CDT  ;
                BaseConnect.WriteVizitAsync(Prefix, CardNum, CodSh, CheckNum, TermNum,
                CDT);
                Utils.ToLog("[DoVizit] Зарегистрировал проводку карты." + Prefix + CardNum);
            
                return 1;

            }
            catch (Exception e)
            {
                Utils.ToLog("[ERROR] [DoVizit] Неудачный вызов удаленной процедуры. " + e.Message);
                return -1;
            }
        }

    }


}
