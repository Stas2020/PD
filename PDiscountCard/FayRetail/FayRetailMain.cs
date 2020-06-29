using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDiscountCard.FayRetail
{
    public static class FayRetailMain
    {

        public static void ShowWndApplyCardWithCurentCheck()
        {
            try
            {
                Utils.ToCardLog("ShowWndApplyCardWithCurentCheck");
                Check CurChk = AlohaTSClass.GetCheckById((int)AlohaTSClass.GetCurentCheckId());
                FayRetailCheckInfo FRCheckInfo = GetRetailCheckInfobyAlohaChk(CurChk);
                ShowWndApplyCard(FRCheckInfo);
            }
            catch (Exception e)
            {
                Utils.ToCardLog("Error ShowWndApplyCardWithCurentCheck " + e.Message);
            }
        }

        public static Dictionary<int, string> ChecksNeedToAddBonus = new Dictionary<int, string>();

        //public static Dictionary<int, string> ChecksIdPurchaseId = new Dictionary<int, string>();

        public static void AddCheckToAddBonus(string CheckIdStr, string CardId)
        {
            try
            {
                int CheckId = Convert.ToInt32(CheckIdStr);
                Utils.ToCardLog("AddCheckToConfirm CheckId " + CheckId + "CardId: " + CardId);
                string Res = "";
                if (ChecksNeedToAddBonus.TryGetValue(CheckId, out Res))
                {
                    ChecksNeedToAddBonus.Remove(CheckId);
                }
                ChecksNeedToAddBonus.Add(CheckId, CardId);
            }
            catch (Exception e)
            {
                Utils.ToCardLog("AddCheckToAddBonus CheckId Error " + e.Message);
            }
        }

        /*
        public static void AddCheckToConfirm(string CheckIdStr, string PId, string Mess)
        {
            try
            {
                int CheckId = Convert.ToInt32(CheckIdStr);
                Utils.ToCardLog("AddCheckToConfirm CheckId " + CheckId);
                string Res = "";
                if (ChecksNeedToConfirm.TryGetValue(CheckId, out Res))
                {
                    ChecksNeedToConfirm.Remove(CheckId);
                }
                string PId2 = "";
                if (ChecksIdPurchaseId.TryGetValue(CheckId, out PId2))
                {
                    ChecksIdPurchaseId.Remove(CheckId);
                }
                ChecksNeedToConfirm.Add(CheckId, Mess);
                ChecksIdPurchaseId.Add(CheckId, PId);
            }
            catch(Exception e)
            {
                Utils.ToCardLog("AddCheckToConfirm CheckId Error " + e.Message);
            }

        }
        */



        public static List<string> CloseCheck(Check AlCheck)
        {
            try
            {
                Utils.ToCardLog("FayRetail CloseCheck " + AlCheck.AlohaCheckNum);
                string Card = "";


                if (ChecksNeedToAddBonus.TryGetValue(AlCheck.AlohaCheckNum, out Card))
                {
                    Utils.ToCardLog("FayRetail CloseCheck check find Card: " + Card);
                    FayRetailCheckInfo ChInf = GetRetailCheckInfobyAlohaChk(AlCheck);
                    string Msg = "";
                    string MsgConf = "";
                    string Cassier = AlohaTSClass.CurentWaiter.ToString();
                    FayRetailClient.AddBonustoCard(Card, ChInf, Cassier , out Msg);
                    FayRetailClient.SendConfirm(ChInf.PurchaseID, Cassier,out MsgConf);
                    List<string> Res = Msg.Split('\n').ToList();
                    Utils.ToCardLog("FayRetail CloseCheck return: AddBonustoCardMsg: " + Msg + " SendConfirmMsg: " + MsgConf);
                    return Res;
                }

                return null;


                /*
                string Res = "";
                if (ChecksNeedToConfirm.TryGetValue(CheckId, out Res))
                {
                    Utils.ToCardLog("FayRetail CloseCheck Find mess" + Res);
                    string PId = "";
                    if (ChecksIdPurchaseId.TryGetValue(CheckId, out PId))
                    {
                        string ErrMsg = "";
                        FayRetailClient.SendConfirm(PId, AlohaTSClass.CurentWaiter.ToString(), out ErrMsg);
                        ChecksIdPurchaseId.Remove(CheckId);
                    }
                    ChecksNeedToConfirm.Remove(CheckId);
                    return Res;
                }
                 
                return "";
                 * */
            }
            catch (Exception e)
            {
                Utils.ToCardLog("FayRetail CloseCheck Error" + e.Message);
                return null;

            }
        }

        public static FayRetailCheckInfo GetCurentFRCheck()
        {
            Check CurChk = AlohaTSClass.GetCheckById((int)AlohaTSClass.GetCurentCheckId());
            return GetRetailCheckInfobyAlohaChk(CurChk);
        }

        public static void ShowWndApplyCard(FayRetailCheckInfo DefaultCheck)
        {
            ctrlApplyFayRetailCard ctrlSetCard = new ctrlApplyFayRetailCard();
            ctrlSetCard.SetCurCheck(DefaultCheck, AlohaTSClass.CurentWaiter.ToString());

            PDSystem.AlertModalWindow wnd = PDSystem.ModalWindowsForegraund.GetModalWindow(ctrlSetCard);
            //PDSystem.AlertModalWindow wnd = new PDSystem.AlertModalWindow();
            //wnd.SetContent(ctrlSetCard);
            ctrlSetCard.SetOwnerWnd(wnd);
            ctrlSetCard.Height = 700;
            ctrlSetCard.Width = 950;
            //wnd.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            //wnd.WindowState = System.Windows.WindowState.Maximized;
            ctrlSetCard.SettbCodeFocus();
            wnd.ShowDialog();
        }
        /*
         public static void SetDiscount(string CardTrack,  FayRetailCheckInfo Chk, string Cashier, double Amount)
         {
             string res = FayRetailClient.GetPaymentRequestData(CardTrack, Chk, Cashier, Amount);
         }
        */
        private static FayRetailCheckInfo GetRetailCheckInfobyAlohaChk(Check Chk)
        {
            FayRetailCheckInfo FRCheck = new FayRetailCheckInfo();
            FRCheck.Items = new List<ChequeLine>();
            int POsId=0;
            foreach (Dish d in Chk.Dishez)
            {
                POsId++;
                decimal Count = d.Count * d.QUANTITY * d.QtyQUANTITY;
                decimal OSummNetto = (decimal)Math.Round(d.OPriceone * (double)Count, 2, MidpointRounding.ToEven);
                decimal SummNetto = (decimal)Math.Round(d.Priceone * (double)Count, 2, MidpointRounding.ToEven) + d.Delta + d.ServiceChargeSumm;
                
                ChequeLine ChLine = new ChequeLine()
                {
                    PosID =POsId,    
                    Barcode = d.BarCode.ToString(),
                    Price = (double)d.Price,
                    Quantity = (double)Count,
                    Amount = (double)(d.Price * d.Count),
                    Name = d.LongName
                };
                FRCheck.Items.Add(ChLine);
            }


            FRCheck.Pays = new List<Pay>();
            foreach (AlohaTender ATndr in Chk.Tenders)
            {
                //string FRPayTypeName = ATndr.AlohaTenderId == 1 ? "Cash" : "Card";
                Pay P = new Pay()
                {
                    Amount = ATndr.Summ.ToString(),
                    Type = ATndr.AlohaTenderId == 1 ? "Cash" : "Card",
                };
                FRCheck.Pays.Add(P);
            }

            if (Chk.Comps.Where(a => a.Id == iniFile.FayRetailDiscountId).Count() > 0)
            {

                Pay P = new Pay()
                {
                    Amount = Chk.Comps.Where(a => a.Id == iniFile.FayRetailDiscountId).Sum(a => a.Amount).ToString(),
                    Type = "FAYRETAIL",
                };
                FRCheck.Pays.Add(P);

            }


            FRCheck.ChequeDate = DateTime.Now;
            FRCheck.ChequeNumber = Chk.AlohaCheckNum.ToString();
            return FRCheck;

        }
    }
}
