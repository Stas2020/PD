using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using System.IO;


using ZXing;
using ZXing.Common;
using ZXing.QrCode;


namespace PDiscountCard.FRSClientApp
{
    public  static class FiscalCheckCreator
    {
        static FRSSrv.FRAttributes OldFRAttributes = new FRSSrv.FRAttributes();
        public  static List<FiscalCheckVisualString> GetFisckalCheckVisual(FRSSrv.AddCheckResponce CheckParametrs,bool SecondChk=false)
        {
            Utils.ToCardLog("GetFisckalCheckVisual ExtNum: "+ CheckParametrs.Check.ExtNum);
           // QrImg = null;
            try
            {
               
                FRSSrv.FiskalCheck FCh = CheckParametrs.Check;
                /*
                if (SecondChk)
                {
                    FCh = CheckParametrs.ParentCheck;
                }
                 * */
                List<FiscalCheckVisualString> strs = new List<FiscalCheckVisualString>();
                if (CheckParametrs != null)
                {
                    if (CheckParametrs.Attrs != null)
                    {
                        OldFRAttributes = CheckParametrs.Attrs;
                    }
                    foreach (string s in OldFRAttributes.Klishe)
                    {
                        strs.Add(new FiscalCheckVisualString(s, ""));

                    }
                    strs.Add(new FiscalCheckVisualString("РН ККТ: " + OldFRAttributes.RN.ToString(), FCh.FROutData.SysDt.ToString("dd.MM.yy HH:mm")));
                    strs.Add(new FiscalCheckVisualString("ЗН ККТ: " + OldFRAttributes.ZN.ToString(), String.Format("СМЕНА:{0} ЧЕК:{1}", OldFRAttributes.Smena, FCh.FROutData.SmallNum)));
                    if (FCh.ReturnSale) { strs.Add(new FiscalCheckVisualString("КАССОВЫЙ ЧЕК/ВОЗВРАТ ПРИХОДА", "")); }
                    else { strs.Add(new FiscalCheckVisualString("КАССОВЫЙ ЧЕК/ПРИХОД", "")); }
                    strs.Add(new FiscalCheckVisualString(String.Format("ИНН: {0}", OldFRAttributes.INN), String.Format("ФН:{0} ", OldFRAttributes.FNNumber)));
                    strs.Add(new FiscalCheckVisualString(String.Format("Кассир:{0}", OldFRAttributes.Cassir), String.Format("#{0} ", FCh.FROutData.bigNum)));
                    strs.Add(new FiscalCheckVisualString(String.Format("Сайт ФНС:"), String.Format("www.nalog.ru")));
                    if (FCh.StringsForPrintBefore != null)
                    {
                        foreach (string s in FCh.StringsForPrintBefore)
                        {
                            strs.Add(new FiscalCheckVisualString(s, ""));
                        }
                    }

                    if (FCh.Dishes != null)
                    {
                        foreach (FRSSrv.FiskalDish Fd in FCh.Dishes)
                        {
                            strs.Add(new FiscalCheckVisualString(Fd.Name, ""));
                            strs.Add(new FiscalCheckVisualString("", String.Format("{0} X {1}", Fd.Count.ToString(), Fd.Price.ToString("0.00")).Replace(",", ".")));
                            strs.Add(new FiscalCheckVisualString("", String.Format("≡{0}_A", (Fd.Count * Fd.Price).ToString("0.00")).Replace(",", ".")));
                        }
                    }
                    if (FCh.Discounts != null)
                    {
                        foreach (FRSSrv.FiskalDiscount fffd in FCh.Discounts)
                        {
                            strs.Add(new FiscalCheckVisualString(fffd.Name, fffd.Summ.ToString("0.00").Replace(",", ".")));
                        }
                    }
                    if (FCh.StringsForPrintAfter != null)
                    {
                        foreach (string s in FCh.StringsForPrintAfter)
                        {
                            strs.Add(new FiscalCheckVisualString(s, ""));
                        }
                    }
                    strs.Add(new FiscalCheckVisualString("ИТОГ:", String.Format("≡{0}", FCh.FROutData.FNSumm.ToString("0.00")).Replace(",", "."), true));
                    strs.AddRange(GetCommonPaymentsStrings(CheckParametrs, 0));
                    strs.Add(new FiscalCheckVisualString("ПОЛУЧЕНО:", ""));
                    strs.AddRange(GetPaymentsStrings(CheckParametrs,1));
                    if (FCh.FROutData.Change > 0)
                    {
                        strs.Add(new FiscalCheckVisualString("СДАЧА:", String.Format("≡{0}", FCh.FROutData.Change.ToString("0.00")).Replace(",", ".")));
                    }
                    /*
                    if (iniFile.FRNoTax)
                    {
                        strs.Add(new FiscalCheckVisualString("Г: СУММА БЕЗ НАЛОГА", String.Format("≡{0}", FCh.FROutData.FNSumm.ToString("0.00")).Replace(",", ".")));
                    }
                    else
                    {
                        strs.Add(new FiscalCheckVisualString("А: СУММА НДС 18%", String.Format("≡{0}", (FCh.FROutData.FNSumm * 0.152548m).ToString("0.00")).Replace(",", ".")));
                    }
                    */

                    decimal nal = (OldFRAttributes.TaxPercent / 10000) / (1 + OldFRAttributes.TaxPercent / 10000);
                    if (OldFRAttributes.TaxPercent == 0)
                    {
                        nal = 1;
                    }
                    strs.Add(new FiscalCheckVisualString(OldFRAttributes.TaxName, String.Format("≡{0}", (FCh.FROutData.FNSumm * nal).ToString("0.00")).Replace(",", ".")));

                    strs.Add(new FiscalCheckVisualString("СНО: " + OldFRAttributes.TaxSystem, String.Format("ФД: {0} ФП: {1}", FCh.FROutData.FD, FCh.FROutData.FP)));
                   // QrImg = CreateQRBitmap(FCh.FROutData.QRAsStr, 130, 130);

                }

                return strs;
            }
            catch(Exception e)
            {
                Utils.ToCardLog("GetFisckalCheckVisual error  ExtNum: " + CheckParametrs.Check.ExtNum +" " +e.Message);
            
            }
            return null;



        }

        internal static List<FiscalCheckVisualString> GetZReportVisual(FRSSrv.ZReportResponce ZRepData)
        {
            if (ZRepData == null) { return null ;}
            if (ZRepData.OutData==null) { return null; }
            FRSSrv.ZReportComamnd Data = ZRepData.OutData;
            List<FiscalCheckVisualString> strs = new List<FiscalCheckVisualString>();
            try
            {

                foreach (string s in Data.FRAttributesData.Klishe)
                {
                    strs.Add(new FiscalCheckVisualString(s));
                }

                strs.Add(new FiscalCheckVisualString("РН ККТ: " + Data.FRAttributesData.RN.ToString(), Data.FROutData.SysDt.ToString("dd.MM.yy HH:mm")));
                strs.Add(new FiscalCheckVisualString("ЗН ККТ: " + Data.FRAttributesData.ZN.ToString(), String.Format("СМЕНА:{0} ", Data.FROutData.Smena)));
                strs.Add(new FiscalCheckVisualString("ЗАКРЫТИЕ СМЕНЫ", ""));
                //if (FCh.ReturnSale) { strs.Add(new FiscalCheckVisualString("КАССОВЫЙ ЧЕК/ВОЗВРАТ ПРИХОДА", "")); }
                //else { strs.Add(new FiscalCheckVisualString("КАССОВЫЙ ЧЕК/ПРИХОД", "")); }
                strs.Add(new FiscalCheckVisualString(String.Format("ИНН: {0}", Data.FRAttributesData.INN), String.Format("ФН:{0} ", Data.FRAttributesData.FNNumber)));
                strs.Add(new FiscalCheckVisualString(String.Format("Администратор"), String.Format("#{0} ", Data.FROutData.bigNum)));
                strs.Add(new FiscalCheckVisualString(String.Format("Сайт ФНС:"), String.Format("www.nalog.ru")));
                strs.Add(new FiscalCheckVisualString(String.Format("ЧЕКОВ ЗА СМЕНУ:"), String.Format("{0}",Data.RegisterData.Where(a=>a.Id<5).Sum(a=>a.SmenaCount).ToString())));
                strs.Add(new FiscalCheckVisualString(String.Format("ФД ЗА СМЕНУ:"), String.Format("{0}", (Data.RegisterData.Where(a => a.Id < 5).Sum(a => a.SmenaCount)+2).ToString())));
                strs.Add(new FiscalCheckVisualString(String.Format("НЕПЕРЕДАННЫХ ФД:"), String.Format("0")));
                strs.Add(new FiscalCheckVisualString(String.Format("ФД НЕ ПЕРЕДАНЫ С:"), String.Format("00.00.00")));
                strs.Add(new FiscalCheckVisualString(String.Format("ПЕРВЫЙ НЕПЕРЕДАННЫЙ ФД:"), String.Format("0")));

                foreach (FRSSrv.ZRepPositionData PD in Data.RegisterData)
                {
                    if  ((PD.Id == 11) || (PD.Id == 19))
                    {
                        strs.Add(new FiscalCheckVisualString(String.Format(PD.Name), String.Format("≡{0}", PD.SmenaSumm.ToString("0.00")).Replace(",", ".")));
                    }
                    else if (PD.Id < 20)
                    {
                        strs.Add(new FiscalCheckVisualString(String.Format(PD.Name), String.Format(PD.AllCount.ToString("0000"))));
                    }
                    else
                    {
                        strs.Add(new FiscalCheckVisualString(String.Format(PD.Name), String.Format("")));
                    }
                    

                    if (PD.Id == 1 || PD.Id == 2)
                    {
                        strs.Add(new FiscalCheckVisualString(String.Format(PD.SmenaCount.ToString("0000")), String.Format("≡{0}", PD.SmenaSumm.ToString("0.00")).Replace(",", "."), true));
                    }
                    else if (PD.Id < 10)
                    {
                        strs.Add(new FiscalCheckVisualString(String.Format(PD.SmenaCount.ToString("0000")), String.Format("≡{0}", PD.SmenaSumm.ToString("0.00")).Replace(",", ".")));
                    }

                    if (PD.PaymentsData != null)
                    {
                        foreach (FRSSrv.ZRepPaymentPositionData PDD in PD.PaymentsData)
                        {
                            if (PD.Id < 6)
                            {
                                if (PDD.SmenaSumm != 0)
                                {
                                    strs.Add(new FiscalCheckVisualString(String.Format(" " + PDD.Name), String.Format("≡{0}", PDD.SmenaSumm.ToString("0.00")).Replace(",", ".")));
                                }
                            }
                            else if (PD.Id == 10)
                            {
                                strs.Add(new FiscalCheckVisualString(String.Format(PDD.SmenaCount.ToString("0000") + " " + PDD.Name), String.Format("≡{0}", PDD.SmenaSumm.ToString("0.00")).Replace(",", ".")));
                            }
                            else if (PD.Id >= 20)
                            {
                                strs.Add(new FiscalCheckVisualString(String.Format(" " + PDD.Name), String.Format("≡{0}", PDD.SmenaSumm.ToString("0.00")).Replace(",", ".")));
                            }
                        }
                    }
                    
                }
                strs.Add(new FiscalCheckVisualString(String.Format("ФД: {0} ",Data.FROutData.FD), String.Format("ФП: {0}",  Data.FROutData.FP)));
                strs.Add(new FiscalCheckVisualString(String.Format("СМЕНА ЗАКРЫТА"), String.Format("")));
            }
            catch(Exception e)
            {
                Utils.ToCardLog("Error GetZReportVisual " +e.Message);
            
            }
            return strs;
        }

        internal static List<FiscalCheckVisualString> GetXReportVisualPaymentsTerm(FRSSrv.XRepFiskalPayment[] Payments, FRSSrv.XRepFiskalPayment[] VoidPayments)
        {
            List<FiscalCheckVisualString> Res = new List<FiscalCheckVisualString>();
            Res.Add(new FiscalCheckVisualString("Чеков за смену: ", (Payments.Sum(a => a.Count) + VoidPayments.Sum(a => a.Count)).ToString()));
       //     Res.Add(new FiscalCheckVisualString("   "));
         //   Res.Add(new FiscalCheckVisualString("Чеков прихода: ", ""));
            Res.Add(new FiscalCheckVisualString(Payments.Where(a => a.Id > 0).Sum(a => a.Count).ToString(), String.Format("≡{0}", Payments.Where(a => a.Id > 0).Sum(a => a.Summ).ToString("0.00").Replace(",", ".")), true));
            foreach (FRSSrv.XRepFiskalPayment Fp in Payments.Where(a => a.Id > 0))
            {
                if (String.IsNullOrWhiteSpace(Fp.Name))
                {
                    Fp.Name = AlohaTSClass.GetTenderName(Fp.ExternalId);
                }
                Res.Add(new FiscalCheckVisualString(Fp.Name, String.Format("≡{0}", Fp.Summ.ToString("0.00").Replace(",", "."))));
            }
            if (Payments.Where(a => a.Id == 0).Count() > 0)
            {
                Res.Add(new FiscalCheckVisualString("--------------------------"));
                foreach (FRSSrv.XRepFiskalPayment Fp in Payments.Where(a => a.Id == 0))
                {
                    if (String.IsNullOrWhiteSpace(Fp.Name))
                    {
                        Fp.Name = AlohaTSClass.GetTenderName(Fp.ExternalId);
                    }
                    Res.Add(new FiscalCheckVisualString(Fp.Name, String.Format("≡{0}", Fp.Summ.ToString("0.00").Replace(",", "."))));
                }
                Res.Add(new FiscalCheckVisualString("--------------------------"));
            }

            Res.Add(new FiscalCheckVisualString("Чеков возврата прихода: ", ""));
            Res.Add(new FiscalCheckVisualString(VoidPayments.Where(a=>a.Id!=-10 && a.Id != -11).Sum(a => a.Count).ToString(), String.Format("≡{0}", VoidPayments.Sum(a => a.Summ).ToString("0.00").Replace(",", "."))));
            foreach (FRSSrv.XRepFiskalPayment Fp in VoidPayments.Where(a => a.Id != -10 && a.Id != -11))
            {
                if (String.IsNullOrWhiteSpace(Fp.Name))
                {
                    Fp.Name = AlohaTSClass.GetTenderName(Fp.ExternalId);
                }
                Res.Add(new FiscalCheckVisualString(Fp.Name, String.Format("≡{0}", Fp.Summ.ToString("0.00").Replace(",", "."))));
            }
            decimal Nal = 0;
            if (Payments.Where(a => a.Id == 1).Count() > 0)
            {
                Nal = Payments.Where(a => a.Id == 1).Sum(b => b.Summ) - VoidPayments.Where(a => a.Id == 1).Sum(b => b.Summ);
            }

            decimal CashFlow = 0;

            CashFlow = Payments.Where(a => a.Id > 0).Sum(b => b.Summ) - VoidPayments.Where(a => a.Id > 0).Sum(b => b.Summ);

            Res.Add(new FiscalCheckVisualString("Нал. в кассе", String.Format("≡{0}", Nal.ToString("0.00").Replace(",", "."))));
            Res.Add(new FiscalCheckVisualString("Выручка", String.Format("≡{0}", CashFlow.ToString("0.00").Replace(",", "."))));
            Res.Add(new FiscalCheckVisualString("   "));
            Res.Add(new FiscalCheckVisualString("   "));


            try
            {
                if (Payments.Any(a => a.Id < 0))
                {
                    Res.Add(new FiscalCheckVisualString("Интернет магазин"));
                    Res.Add(new FiscalCheckVisualString("--------------------------"));
                    var ImP = Payments.FirstOrDefault(a => a.Id == -10);
                    var ImPV = VoidPayments.FirstOrDefault(a => a.Id == -10);
                    var ImS = ImP.Summ - ImPV?.Summ;
                    Res.Add(new FiscalCheckVisualString(ImP.Name, String.Format("≡{0}", ImS.GetValueOrDefault().ToString("0.00").Replace(",", "."))));
                   var  ImP2 = Payments.FirstOrDefault(a => a.Id == -11);
                    Res.Add(new FiscalCheckVisualString(ImP2.Name, String.Format("≡{0}", ImP2.Summ.ToString("0.00").Replace(",", "."))));
                    if (Math.Abs(ImS.GetValueOrDefault() - ImP2.Summ)>1)
                    {
                        Res.Add(new FiscalCheckVisualString("Сумма не равна!!!", (ImS.GetValueOrDefault() - ImP2.Summ).ToString()));
                    }
                    Res.Add(new FiscalCheckVisualString("--------------------------"));
                    Res.Add(new FiscalCheckVisualString("   "));
                    Res.Add(new FiscalCheckVisualString("   "));

                }
            }
            catch(Exception e)
            {
                Utils.ToCardLog("GetXReportVisualPaymentsTerm error" + e.Message);
            }


            return Res;
        }


        internal static List<FiscalCheckVisualString> GetIMReportVisual(bool Z=false)
        {
            List<FiscalCheckVisualString> Res = new List<FiscalCheckVisualString>();
            Res.Add(new FiscalCheckVisualString("   "));
            Res.Add(new FiscalCheckVisualString("   "));
            Res.Add(new FiscalCheckVisualString("   "));
            Res.Add(new FiscalCheckVisualString("   "));
            Res.Add(new FiscalCheckVisualString(AlohainiFile.UNITNAME));
            Res.Add(new FiscalCheckVisualString(AlohainiFile.ADDRESS1));
            Res.Add(new FiscalCheckVisualString(AlohainiFile.ADDRESS2));
            Res.Add(new FiscalCheckVisualString("Выручка интернет магазин"));
            Res.Add(new FiscalCheckVisualString("Смена  ", DateTime.Now.ToString("dd.MM.yy HH:mm:ss")));
            Res.Add(new FiscalCheckVisualString("ДБ: " + AlohainiFile.BDate.ToString("dd.MM.yy")));

            if (Z)
            {
                Res.Add(new FiscalCheckVisualString("День закрыт", "ДБ: " +AlohainiFile.BDate.ToString("dd.MM.yy")));
            }
            Res.Add(new FiscalCheckVisualString("   "));
            Res.Add(new FiscalCheckVisualString("   "));
            /*
            Res.Add(new FiscalCheckVisualString("--------------------------"));


            var dHCDHConnect = new DHConnect();
            var dhP = dHCDHConnect.GetHubXReport(AlohainiFile.BDate, AlohainiFile.DepNum, out int dhCount);
            
            var AlohaImSumm = AlohaTSClass.GetIMChecksSumm();

            //var ImP = Payments.FirstOrDefault(a => a.Id == -10);
            //var ImPV = VoidPayments.FirstOrDefault(a => a.Id == -10);

            //var ImS = ImP.Summ - ImPV?.Summ;
            Res.Add(new FiscalCheckVisualString("Алоха ИМ", String.Format("≡{0}", AlohaImSumm.ToString("0.00").Replace(",", "."))));
            //var ImP2 = Payments.FirstOrDefault(a => a.Id == -11);
            Res.Add(new FiscalCheckVisualString("Эквайринг ИМ", String.Format("≡{0}", dhP.ToString("0.00").Replace(",", "."))));
            if (Math.Abs(AlohaImSumm- dhP) > 1)
            {
                Res.Add(new FiscalCheckVisualString("Сумма не равна!!!", (AlohaImSumm - dhP).ToString()));
            }
            Res.Add(new FiscalCheckVisualString("--------------------------"));
            */
            Res.Add(new FiscalCheckVisualString("   "));
            Res.Add(new FiscalCheckVisualString("   "));



            return Res;
        }



            internal static List<FiscalCheckVisualString> GetXReportVisual(FRSSrv.XReportResponce XRepData)
        {
            if (XRepData == null) { XRepData = new FRSSrv.XReportResponce(); }
            List<FiscalCheckVisualString> Res = new List<FiscalCheckVisualString>();
            Res.Add(new FiscalCheckVisualString("   "));
            Res.Add(new FiscalCheckVisualString("   "));
            Res.Add(new FiscalCheckVisualString("   "));
            Res.Add(new FiscalCheckVisualString("   "));
            Res.Add(new FiscalCheckVisualString(AlohainiFile.UNITNAME));
            Res.Add(new FiscalCheckVisualString(AlohainiFile.ADDRESS1));
            Res.Add(new FiscalCheckVisualString(AlohainiFile.ADDRESS2));
            Res.Add(new FiscalCheckVisualString("Смена  " , DateTime.Now.ToString("dd.MM.yy HH:mm:ss")));
            Res.Add(new FiscalCheckVisualString("Отчет без гашения","ДБ: "+ XRepData.BUsinessDate.ToString("dd.MM.yy")));

            Res.Add(new FiscalCheckVisualString("   "));
            Res.Add(new FiscalCheckVisualString("   "));
            if (XRepData.Payments == null) { XRepData.Payments = (new List<FRSSrv.XRepFiskalPayment>()).ToArray(); }

            if (XRepData.Payments.Where(a=>a.Term>0).Select(a => a.Term).Distinct().Count() > 1)
            {

                foreach (int Trm in XRepData.Payments.Where(a => a.Term > 0).Select(a => a.Term).Distinct().OrderBy(b => b))
                {
                    Res.Add(new FiscalCheckVisualString("   "));
                    Res.Add(new FiscalCheckVisualString("   "));

                    Res.Add(new FiscalCheckVisualString("Касса №" + Trm.ToString()));
                    Res.Add(new FiscalCheckVisualString("   "));

                    Res.AddRange(GetXReportVisualPaymentsTerm(XRepData.Payments.Where(a => a.Term == Trm).ToArray(), XRepData.VoidPayments.Where(a => a.Term == Trm).ToArray()));
                   
                }
                Res.Add(new FiscalCheckVisualString("   "));
                Res.Add(new FiscalCheckVisualString("По всем кассам"));
            }

            Res.AddRange(GetXReportVisualPaymentsTerm(XRepData.Payments.Where(a => a.Term == 0).ToArray(), XRepData.VoidPayments.Where(a => a.Term == 0).ToArray()));
                Res.Add(new FiscalCheckVisualString("   "));
                Res.Add(new FiscalCheckVisualString("   "));
            return Res;
        }

        internal static List<FiscalCheckVisualString> GetEndOfSmenaVisual(FRSSrv.ZReportResponce ZRepData)
        {
            if (ZRepData == null) { ZRepData = new FRSSrv.ZReportResponce(); }
            FRSSrv.XReportComamnd XRepData = ZRepData.OutData;
            
            List<FiscalCheckVisualString> Res = new List<FiscalCheckVisualString>();
            Res.Add(new FiscalCheckVisualString("   "));
            Res.Add(new FiscalCheckVisualString("   "));
            Res.Add(new FiscalCheckVisualString("   "));
            Res.Add(new FiscalCheckVisualString("   "));
            Res.Add(new FiscalCheckVisualString(AlohainiFile.UNITNAME));
            Res.Add(new FiscalCheckVisualString(AlohainiFile.ADDRESS1));
            Res.Add(new FiscalCheckVisualString(AlohainiFile.ADDRESS2));
            Res.Add(new FiscalCheckVisualString("Дата: ", DateTime.Now.ToString("dd.MM.yy HH:mm:ss")));
            Res.Add(new FiscalCheckVisualString("Отчет закрытия смены", "ДБ: " +ZRepData.BUsinessDate.ToString("dd.MM.yy")));
            if (XRepData.Payments == null) { XRepData.Payments = (new List<FRSSrv.XRepFiskalPayment>()).ToArray(); }

            Res.AddRange(GetXReportVisualPaymentsTerm(XRepData.Payments.Where(a => a.Term == 0).ToArray(), XRepData.VoidPayments.Where(a => a.Term == 0).ToArray()));

            //Res.Add(new FiscalCheckVisualString("Чеков за смену: ", XRepData.Payments.Sum(a => a.Count) + XRepData.VoidPayments.Sum(a => a.Count).ToString()));
            //Res.Add(new FiscalCheckVisualString("   "));
            //Res.Add(new FiscalCheckVisualString("Чеков прихода: ", ""));
            //Res.Add(new FiscalCheckVisualString(XRepData.Payments.Where(a => a.Id > 0).Sum(a => a.Count).ToString(), String.Format("≡{0}".Replace(",", "."), XRepData.Payments.Where(a => a.Id > 0).Sum(a => a.Summ).ToString("0.00")), true));
            //foreach (FRSSrv.XRepFiskalPayment Fp in XRepData.Payments.Where(a => a.Id > 0))
            //{
            //    if (String.IsNullOrWhiteSpace(Fp.Name))
            //    {
            //        Fp.Name = AlohaTSClass.GetTenderName(Fp.ExternalId);
            //    }
            //    Res.Add(new FiscalCheckVisualString(Fp.Name, String.Format("≡{0}".Replace(",", "."), Fp.Summ.ToString("0.00"))));
            //}
            //if (XRepData.Payments.Where(a => a.Id == 0).Count() > 0)
            //{
            //    Res.Add(new FiscalCheckVisualString("--------------------------"));
            //    foreach (FRSSrv.XRepFiskalPayment Fp in XRepData.Payments.Where(a => a.Id == 0))
            //    {
            //        if (String.IsNullOrWhiteSpace(Fp.Name))
            //        {
            //            Fp.Name = AlohaTSClass.GetTenderName(Fp.ExternalId);
            //        }
            //        Res.Add(new FiscalCheckVisualString(Fp.Name, String.Format("≡{0}".Replace(",", "."), Fp.Summ.ToString("0.00"))));
            //    }
            //    Res.Add(new FiscalCheckVisualString("--------------------------"));
            //}

            //Res.Add(new FiscalCheckVisualString("Чеков возврата прихода: ", ""));
            //Res.Add(new FiscalCheckVisualString(XRepData.VoidPayments.Sum(a => a.Count).ToString(), String.Format("≡{0}".Replace(",", "."), XRepData.VoidPayments.Sum(a => a.Summ).ToString("0.00"))));
            //foreach (FRSSrv.XRepFiskalPayment Fp in XRepData.VoidPayments)
            //{
            //    if (String.IsNullOrWhiteSpace(Fp.Name))
            //    {
            //        Fp.Name = AlohaTSClass.GetTenderName(Fp.ExternalId);
            //    }
            //    Res.Add(new FiscalCheckVisualString(Fp.Name, String.Format("≡{0}".Replace(",", "."), Fp.Summ.ToString("0.00"))));
            //}
            //decimal Nal = 0;
            //if (XRepData.Payments.Where(a => a.Id == 1).Count() > 0)
            //{
            //    Nal = XRepData.Payments.Where(a => a.Id == 1).Sum(b => b.Summ) - XRepData.VoidPayments.Where(a => a.Id == 1).Sum(b => b.Summ);
            //}

            //decimal CashFlow = 0;

            //CashFlow = XRepData.Payments.Where(a => a.Id > 0).Sum(b => b.Summ) - XRepData.VoidPayments.Where(a => a.Id > 0).Sum(b => b.Summ);

            //Res.Add(new FiscalCheckVisualString("Нал. в кассе", String.Format("≡{0}".Replace(",", "."), Nal.ToString("0.00"))));
            //Res.Add(new FiscalCheckVisualString("Выручка", String.Format("≡{0}".Replace(",", "."), CashFlow.ToString("0.00"))));
            //Res.Add(new FiscalCheckVisualString("   "));
            Res.Add(new FiscalCheckVisualString("Смена закрыта",true,true));
            Res.Add(new FiscalCheckVisualString("   "));
            Res.Add(new FiscalCheckVisualString("   "));
            Res.Add(new FiscalCheckVisualString("   "));
            return Res;



        }



        private static List<FiscalCheckVisualString> GetCommonPaymentsStrings(FRSSrv.AddCheckResponce CheckParametrs,int t)
        {
            FRSSrv.FiskalCheck FCh = CheckParametrs.Check;
            
            List<FiscalCheckVisualString> strs = new List<FiscalCheckVisualString>();
            if (FCh.Payments == null || FCh.Payments.Count() == 0) return strs;
            if (FCh.Payments.Any(a => a.ExternalId == 1))
            {
                strs.Add(new FiscalCheckVisualString(" НАЛИЧНЫМИ", "≡" + FCh.Payments.Where(a => a.ExternalId == 1).Sum(a=>a.Summ).ToString("0.00").Replace(",", ".")));
            }
            if (FCh.Payments.Any(a => a.ExternalId != 1))
            {
                strs.Add(new FiscalCheckVisualString(" БЕЗНАЛИЧНЫМИ", "≡" + FCh.Payments.Where(a => a.ExternalId != 1).Sum(a => a.Summ).ToString("0.00").Replace(",", ".")));
            }

           
            return strs;
        }

        private static List<FiscalCheckVisualString> GetPaymentsStrings(FRSSrv.AddCheckResponce CheckParametrs, int t)
        {
            FRSSrv.FiskalCheck FCh = CheckParametrs.Check;

            List<FiscalCheckVisualString> strs = new List<FiscalCheckVisualString>();
           
            foreach (FRSSrv.FiskalPayment Fpp in FCh.Payments)
            {
                if (Fpp.Summ == 0) continue;
                if (t == 0)
                {
                    strs.Add(new FiscalCheckVisualString(" " + OldFRAttributes.PaymentNames[Fpp.Id], "≡" + Math.Min(Fpp.Summ, CheckParametrs.Check.FROutData.FNSumm).ToString("0.00").Replace(",", ".")));
                }
                else
                {
                    strs.Add(new FiscalCheckVisualString(" " + OldFRAttributes.PaymentNames[Fpp.Id], "≡" + Fpp.Summ.ToString("0.00").Replace(",", ".")));
                }
            }
           
            return strs;
        }
        
        public static BitmapImage CreateQRBitmap(string s, int width,int height)
        {
            Utils.ToCardLog("CreateQRBitmap s: ");
            if (String.IsNullOrEmpty(s) || String.IsNullOrWhiteSpace(s))
            {
                Utils.ToCardLog("CreateQRBitmap QRString is Empty: ");
                return null;
            }
            BitmapImage bitmapImage = new BitmapImage();
            try
            {
                QRCodeWriter qrEncode = new QRCodeWriter();
                Dictionary<EncodeHintType, object> hints = new Dictionary<EncodeHintType, object>();    //для колекции поведений
                BitMatrix qrMatrix = qrEncode.encode(   //создание матрицы QR
                   s,                 //кодируемая строка
                   BarcodeFormat.QR_CODE,  //формат кода, т.к. используется QRCodeWriter применяется QR_CODE
                   width,                    //ширина
                   height,                    //высота
                   hints);
                BarcodeWriter qrWrite = new BarcodeWriter();    //класс для кодирования QR в растровом файле
                System.Drawing.Bitmap qrImage = qrWrite.Write(qrMatrix);
                using (MemoryStream memory = new MemoryStream())
                {
                    qrImage.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
                    memory.Position = 0;
                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = memory;
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.EndInit();
                    
                }
                qrImage.Dispose();
                qrWrite = null;
                qrEncode = null;
            }
            catch(Exception e)
            {
                Utils.ToCardLog("Error CreateQRBitmap " + e.Message);
            }
            return bitmapImage;
        }
    }
    
}
