using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDiscountCard
{
    public class InpasChk
    {
        public int Ammount { set; get; }
        public int TableId { set; get; }
        public bool isVoid { set; get; }
        public bool AllInTable { set; get; }
        public string Num { set; get; }
        public int AlohaId { set; get; }
        public int CurrentEmpl { set; get; }

        public List<InpasChk> AllChks = new List<InpasChk>();


        public InpasChk(Check Chk, bool mAllInTable, int Empl)
        {
            CurrentEmpl = Empl;
            if (mAllInTable)
            {
                isVoid = false;
                AllInTable = true;
                Ammount = (int)((Chk.ChecksOnTable.Sum(a => a.Summ) * 100) - (Chk.ChecksOnTable.Sum(a => a.Oplata) * 100));
                TableId = Chk.TableId;
                foreach (Check Chk2 in Chk.ChecksOnTable)
                {
                    AllChks.Add(new InpasChk(Chk2, false, Empl));

                }
            }
            else
            {
                AllInTable = false;
                Ammount = (int)((Chk.Summ * 100) - (Chk.Oplata * 100));
                TableId = Chk.TableId;
                isVoid = Chk.Vozvr;
                AlohaId = Chk.AlohaCheckNum;
                Num = Chk.CheckShortNum;



            }
        }
    }
}
