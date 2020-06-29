using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDiscountCard
{
    static public class Akciya
    {

        //static public DateTime StartDtOfAkciya = new DateTime(2017, 04, 21);
        static public DateTime StartDtOfAkciya = new DateTime(2017, 04, 01);
        static public DateTime EndDtOfAkciya = new DateTime(2017, 05, 1);
        static public List<int> DepsAkcii = new List<int>() { 260,998,999};
        static public int HourOfStartAkcii = 18;
        static public int HourOfStopAkcii = 4;

        static public int BigSumm = 0;

        public static bool InAkc(decimal Summ)
        {

            /*
            HourOfStartAkcii -= iniFile.AkcTest;

            return (DepsAkcii.Contains(AlohainiFile.DepNum) && ((DateTime.Now.Hour >= HourOfStartAkcii) || (DateTime.Now.Hour < HourOfStopAkcii)) && (Summ > BigSumm)
                && (DateTime.Now > StartDtOfAkciya) && (DateTime.Now < EndDtOfAkciya));
            */

            return (DepsAkcii.Contains(AlohainiFile.DepNum) && (Summ > BigSumm) && (DateTime.Now > StartDtOfAkciya) && (DateTime.Now < EndDtOfAkciya));

        }



    }
}
