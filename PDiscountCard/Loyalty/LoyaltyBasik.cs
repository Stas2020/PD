using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDiscountCard.Loyalty
{
    class LoyaltyBasik
    {
       static public List<string> BonusPrefix = new List<string> { "11115", "26530" };
       static public List<int> PresentCardBCs = new List<int> { 999903, 999905, 999910, 999803, 999805, 999810 };
       static public List<int> SVMyPresentCardBCs = new List<int> { 999803, 999805, 999810 };
       static public List<string> PresentCardPrefix = new List<string> { "11116", "11117", "11118", "26603", "26605", "26610" };
       static string PresCard = "Подарочная карта"; 
       static public Dictionary<int, string> PaymentCardDescr = new Dictionary<int, string> { 
           {11116, PresCard},
           {11117, PresCard},
           {11118, PresCard},
           {26603, PresCard},
           {26605, PresCard},
           {26610, PresCard}
       };

       static internal void InsertASVCardInfo(Check chk)
       {
           if (chk.ASVCards.Count > 0)
           {
               foreach (string Card in chk.ASVCards)
               {
                   PDiscountCard.AlohaTSClass.SetSVCardSaleAttr(chk.AlohaCheckNum, Card);
               }
           }
           if (chk.Dishez.Select(a => a.BarCode).Intersect(PresentCardBCs).Count() > 0)
           {
               chk.ASVCards.Clear();
               string mCard = "";
               bool res = true;
               int Num = 0;
               do
               {
                   res = PDiscountCard.AlohaTSClass.GetSVCardSaleAttr(chk.AlohaCheckNum, Num, out mCard);
                   if (res)
                   {
                       chk.ASVCards.Add(mCard);
                   }
                   else
                   {
                       break;
                   }
                   Num++;
               } while (res);
           }
           int Num2 = 0;
           foreach (Dish d in chk.Dishez.Where(a => Loyalty.LoyaltyBasik.PresentCardBCs.Contains(a.BarCode)))
           {
               if (chk.ASVCards.Count >= Num2 + 1)
               {
                   string Card = chk.ASVCards[Num2];
                   if (Card.Length > 5)
                   {
                       d.CardPrefix = Card.Trim().Substring(0, 5);
                       d.CardNumber = Card.Trim().Substring(5);
                       
                   }
               }
               Num2++;
           }

       }

       static public List<AlohaTender> GetAirBonusTenders(Check Chk)
       {
           return Chk.CreditPayments().Where(a => BonusPrefix.Contains(a.CardPrefix.ToString())).ToList();
       }

       static internal Check CheckConvertForBOnusCard(Check InCheck)
       {
           Utils.ToLog(String.Format("CheckConvertForBOnusCard"));
           Check OutCheck = InCheck;
           if (OutCheck.LoyaltyCard != "")
           {
               OutCheck.LoyaltyBonus = OutCheck.Summ * (decimal)0.1;
           }
           if (OutCheck.CreditPayments().Where(a => BonusPrefix.Contains(a.CardPrefix.ToString())).Count() > 0)
           {
               decimal BonusSumm =Convert.ToDecimal(OutCheck.CreditPayments().Where(a => BonusPrefix.Contains(a.CardPrefix.ToString())).Sum(a=>a.Summ));
               Utils.ToLog(String.Format("CheckConvertForBOnusCard BonusPrefix Payment  find summ {0}",BonusSumm));
               OutCheck.Comp += BonusSumm;
               //OutCheck.ASVCard = 
               /*
               if (OutCheck.Oplata <= (decimal)GetAirBonusTenders(OutCheck).Sum(a => a.Summ))
               {
                   OutCheck.Oplata = 1;
                   OutCheck.Comp += (decimal)OutCheck.Summ - 1;
                   OutCheck.IsNal = true;
                   OutCheck.TenderId = 1;
               }
               else
               {
                   OutCheck.Oplata -= (decimal)GetAirBonusTenders(OutCheck).Sum(a => a.Summ);
                   OutCheck.Comp += (decimal)GetAirBonusTenders(OutCheck).Sum(a => a.Summ);
               }
               OutCheck.CompId = AlohaTSClass.BonusCompId;
               if (OutCheck.TenderId == AlohaTSClass.BonusPaymentId)
               {
                   if (OutCheck.Tenders.Where(a => a.TenderId != AlohaTSClass.BonusPaymentId).Count() > 0)
                   {
                       OutCheck.TenderId = GetAirBonusTenders(OutCheck).First().TenderId;
                   }
                   else
                   {
                       OutCheck.TenderId = 1;
                   }
                   if (OutCheck.TenderId == 1)
                   {
                       OutCheck.IsNal = true;
                   }
               }
      
               Utils.ToLog(String.Format("CheckConvertForBOnusCard out Summ {0}, Tender {1}, TenderId {2}, Comp {3}", OutCheck.Summ, OutCheck.Oplata, OutCheck.TenderId, OutCheck.Comp));
                * */
           }
           OutCheck.CreditSumm2 = (decimal)OutCheck.CreditPayments().Where(a => !BonusPrefix.Contains(a.CardPrefix.ToString())).Sum(a => a.Summ);
           return OutCheck;

       }


       public void PrintAndSendCreditPaymentsTobase(Check Chk)
       {
           DateTime dt = DateTime.Now;
           foreach (AlohaTender AT in Chk.CreditPayments().Where(a => PresentCardPrefix.Contains(a.CardPrefix.ToString())))
           {
             string  slip=   PrintSlip(AT, dt, Convert.ToInt32(Chk.CheckShortNum));
             AddCardPaymentToBase(AT, slip, dt, Convert.ToInt32(Chk.CheckShortNum));
           }   
       }

       public static void PrintLongSlipReport()
       {
           ZRSrv.Service1 S1 = new ZRSrv.Service1();
           List<ZRSrv.PaymentCard> Tmp= S1.GetPaymentCardInfoReport(Utils.GetUnTermNum(),AlohainiFile.BDate).ToList();
           List<string> Slip = new List<string>();
         //  Slip.Add("  ");
         //  Slip.Add(AlohainiFile.UNITNAME);
         //  Slip.Add(AlohainiFile.ADDRESS1);
         //  Slip.Add(AlohainiFile.ADDRESS2);
         //  Slip.Add("  ");
           Slip.Add("Касса: " + Utils.GetUnTermNum());
           String CardName = "";
           Slip.Add("Дата: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
           Slip.Add("  ");

           foreach (ZRSrv.PaymentCard PC in Tmp)
           {
               Slip.Add(PrintLine );
               Slip.Add("  ");
               Slip.Add("Карта " + PC.CardPrefix + PC.CardNumber.ToString().PadLeft(9,'0'));
               Slip.Add("Сумма: " + PC.Summ.Value.ToString("0.00"));
               Slip.Add("Дата: " + PC.SystemDate.Value.ToString("dd/MM/yyyy HH:mm:ss"));
               Slip.Add("  ");
           }
           Slip.Add(PrintLine);
           Slip.Add("  ");
           Slip.Add("Итого карт: "+Tmp.Count );
           Slip.Add("Сумма: " + Tmp.Sum(a=>a.Summ));
           ToShtrih.PrintCardCheck(Slip,"Отчет по оплате под. картами");
       }


       private static string PrintLine
       {
           get
           {
               return  new string('-',Shtrih2.CharCountWidth);
           }
       }
       private string PrintSlip(AlohaTender AT, DateTime dt, int CheckNum)
       {
           List<string> Slip = new List<string>();
         //  Slip.Add("  ");
         //  Slip.Add(AlohainiFile.UNITNAME);
         //  Slip.Add(AlohainiFile.ADDRESS1);
         //  Slip.Add(AlohainiFile.ADDRESS2);
           Slip.Add("  ");
           Slip.Add("Касса: &&" + Utils.GetUnTermNum());
           Slip.Add("Чек: &&" + CheckNum.ToString());
           String CardName = "";
           PaymentCardDescr.TryGetValue(Convert.ToInt32(AT.CardPrefix), out CardName);
           Slip.Add("Оплата &&" + CardName);
           Slip.Add("  ");
           Slip.Add("Карта &&" + AT.CardPrefix + AT.CardNumber.ToString().PadLeft(9, '0'));
           Slip.Add("Сумма: &&" + AT.Summ.ToString("0.00"));
           Slip.Add("Дата: &&" + dt.ToString("dd/MM/yyyy HH:mm:ss"));
           Slip.Add("  ");
           Slip.Add("  ");
           Slip.Add("Подпись клиента: " );
           Slip.Add("  ");
           Slip.Add("  ");
           Slip.Add("__________________________________");
           Slip.Add("  ");
           ToShtrih.PrintCardCheck(Slip,"Оплата подарочной картой");

           String OutSlip="";
           foreach (string s in Slip)
           {
                OutSlip+= s+Environment.NewLine;
           }
           return OutSlip;
       }


       private void AddCardPaymentToBase(AlohaTender AT, string slip,DateTime dt,  int CheckNum)
       {
           ZRSrv.PaymentCard Card = new ZRSrv.PaymentCard()
           {
               BusinessDate = AlohainiFile.BDate,
               CardNumber = Convert.ToInt32(AT.CardNumber),
               CardPrefix = Convert.ToInt32(AT.CardPrefix),
               CheckNumber = CheckNum,
               Dep = AlohainiFile.DepNum,
               Slip = slip,
               Summ = (decimal?)AT.Summ,
               SystemDate = dt,
               UnDep = Utils.GetUnTermNum()

           };
           ZRSrv.Service1 S1 = new ZRSrv.Service1();
           S1.AddPaymentCardInfo(Card);
       }


       public static decimal GetASVCardBalance(string CardNunber, out DateTime dt, out string Error)
       {
           Error = "";
           dt = DateTime.Now;
           Utils.ToCardLog("GetASVCardBalance " + CardNunber);
           try
           {
               ASVWebSrv.StoredValuePublicWSImplService mlws = new ASVWebSrv.StoredValuePublicWSImplService();
               ASVWebSrv.GetCardBalanceRequest req = new ASVWebSrv.GetCardBalanceRequest()
               {
                   cardNumber = CardNunber,
                   company = "tat02",
                   storeID = AlohainiFile.DepNum,
                   requestID = Guid.NewGuid().ToString(),
                   user = "ASVWebService",
                   password = "Fil123fil123"


               };
               ASVWebSrv.GetCardBalanceResponse resp = mlws.GetCardBalance(req);
               if (resp.responseCode == 0)
               {
                   dt = resp.expirationDate;
                   return resp.balance;
               }
               else
               {
                   Error = resp.responseCodeName;
                   return -1;
               }
           }
           catch(Exception e)
           {
               Utils.ToCardLog("Error GetASVCardBalance " + e.Message);
               Error = Error + Environment.NewLine + e.Message;
               return -1;
           }
       }

    }
}
