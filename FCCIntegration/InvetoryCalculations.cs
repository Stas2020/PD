using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FCCIntegration
{
   public static class InvetoryCalculations
    {
      static List<MyDenomination> MyDenominations = new List<MyDenomination>();
       internal static void UpdateMyDenominations()
       {
           try
           {
               CFCCApi FCCApi = new CFCCApi();

               FCCSrv2.CashUnitsType[] Cash = FCCApi.UpdateInventory();
               MyDenominations.Clear();
              List < FCCSrv2.CashUnitType> MyCash = Cash.Where(a => a.devid == "1").FirstOrDefault().CashUnit.Where(a => a.unitno == "4043" || a.unitno == "4044" || a.unitno == "4045").ToList();
               MyCash.AddRange(Cash.Where(a => a.devid == "2").FirstOrDefault().CashUnit.Where(a => int.Parse(a.unitno) > 4042 && int.Parse(a.unitno) < 4056).ToList());
               
               if (MyCash != null)
               {

                   foreach (FCCSrv2.CashUnitType Dt in MyCash)
                   {
                       MyDenomination Md = MyDenominations.Where(a => a.fv.ToString() == Dt.Denomination[0].fv).FirstOrDefault();
                       if (Md == null)
                       {
                           Md = new MyDenomination() { 
                           DevId = int.Parse(Dt.Denomination[0].devid),
                           fv = int.Parse(Dt.Denomination[0].fv),
                           };
                           MyDenominations.Add(Md);
                           Md.Pieces += int.Parse(Dt.Denomination[0].Piece);
                       }
                    }
                   

                   MyDenominations.Sort(delegate(MyDenomination x, MyDenomination y)
        {
            if (x.fv>y.fv) return -1;
            else if (x.fv<y.fv) return 1;
            else return 0;
        });

               }
              
            
           }
           catch
           {
              
           }

       
       }
        

       public static FCCSrv2.DenominationType[] GetDtsBySum(int Summ)
       {
            List<FCCSrv2.DenominationType> Tmp = new List<FCCSrv2.DenominationType>();
     
            for(int i=0;i<MyDenominations.Count;i++)
            {
                
                int PCount = Math.Min(MyDenominations[i].Pieces,Summ/MyDenominations[i].fv);
                if (PCount > 0)
                {
                    FCCSrv2.DenominationType Dt = new FCCSrv2.DenominationType();
                    Dt.cc = "RUB";
                    Dt.devid = MyDenominations[i].DevId.ToString();
                    Dt.fv = MyDenominations[i].fv.ToString();
                    Dt.Status = "2";
                    Dt.rev = "0";
                    Dt.Piece = PCount.ToString();
                    Summ = Summ - PCount * MyDenominations[i].fv;
                    Tmp.Add(Dt);
                }
                if (Summ <= 0)
                {
                    break;
                }
            }


           return Tmp.ToArray();
       
       }
     
    }


   public class MyDenomination
   {
       public int fv { set; get; }
       public int DevId { set; get; }
       public int Pieces { set; get; }
   }
}
