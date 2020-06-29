using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDiscountCard
{
    public class ShtrihReports
    {
        /*
        public void PrintOutKassetaReport(double Summ)
        {
            string CurentDate = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            ShtrihCommandBlock ShtrihCommandBlockPrint = new ShtrihCommandBlock();
            ShtrihCommandBlockPrint.PrintString(CurentDate);
            ShtrihCommandBlockPrint.PrintString("ОТЧЕТ ИЗЪЯТИЯ КАССЕТЫ ");
            ShtrihCommandBlockPrint.PrintString("------------------------");
            ShtrihCommandBlockPrint.PrintString("  ");
            ShtrihCommandBlockPrint.PrintWideString("СУММА ИЗЪЯТИЯ &&" + ToShtrihFormat(Summ));
            ShtrihCommandBlockPrint.PrintString("  ");
            ShtrihCommandBlockPrint.PrintString("------------------------");
            ShtrihCommandBlockPrint.FeedDocument();
            ShtrihCommandBlockPrint.CommandBlockToQwery();
        }

        static private string ToShtrihFormat(double d)
        {
            string Eq2 = "&&" + "=";
            return Eq2 + d.ToString("0.00").Replace(",", ".");
        }
         * */
    }
}
