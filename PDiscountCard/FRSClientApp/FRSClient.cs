using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using PDiscountCard.FRSSrv;

namespace PDiscountCard.FRSClientApp
{
    public static class FRSClient
    {

        public static FRSSrv.RemoteDataClient GetFRSClient()
        {
            try
            {
                Utils.ToCardLog("Init FClient ");
                
                string CloudAddress = iniFile.FRSSRVPath.Trim();
                if (CloudAddress == "")
                { 
                    string myHost = System.Net.Dns.GetHostName();

                    string CloudIP = "";
                    
                    foreach (IPAddress Addr in Dns.GetHostEntry(myHost).AddressList)
                    {
                        if (Addr.ToString().Length > 8)
                        {
                            if (Addr.ToString().Substring(0, 7) == "192.168")
                            {
                                CloudIP = "192.168." + Addr.GetAddressBytes()[2].ToString() + ".47";
                            }
                        }
                    }
                    CloudAddress = String.Format(@"http://{0}:3838/FRSService/RemoteData", CloudIP);
                }
                Utils.ToCardLog("FClient Address: " +CloudAddress);

                System.ServiceModel.Channels.Binding binding = new System.ServiceModel.BasicHttpBinding();
                ((System.ServiceModel.BasicHttpBinding)binding).MaxReceivedMessageSize = 1024 * 1024;
                //System.ServiceModel.EndpointAddress remoteAddress = new System.ServiceModel.EndpointAddress(@"http://192.168.77.7:3838/FRSService/RemoteData");
                //System.ServiceModel.EndpointAddress remoteAddress = new System.ServiceModel.EndpointAddress(iniFile.FRSSRVPath.Trim());
                System.ServiceModel.EndpointAddress remoteAddress = new System.ServiceModel.EndpointAddress(CloudAddress);
                FRSSrv.RemoteDataClient FClient = new FRSSrv.RemoteDataClient(binding, remoteAddress);
                FClient.InnerChannel.OperationTimeout = new TimeSpan(0, 5, 0);
                return FClient;
            }
            catch (Exception e)
            {
                Utils.ToCardLog("Error Init FClient " + e.Message);
                return null;
            }
        }
        public static void UpdateItems()
        {
            FRSSrv.RemoteDataClient FClient = GetFRSClient();
            if (FClient != null)
            {
                try
                {
                    var tExDishez = AlohaTSClass.GetExDishez();
                    Utils.ToCardLog("ExDishez: " + String.Join(",", tExDishez));


                    FClient.InitItems(tExDishez.ToArray(), AlohaTSClass.GetExDisc().ToArray());
                    FClient.Close();
                    Utils.ToLog("UpdateItems ok ");
                }
                catch(Exception e )
                {
                    Utils.ToLog("Error UpdateItems: " + e.Message);
                }
            }
        }


        public static void ZReport(DateTime BD)
        {
            Utils.ToCardLog("FClient.ZReport" );
            FRSSrv.RemoteDataClient FClient = GetFRSClient();
            //DateTime BD = AlohainiFile.BDate; 

            if (FClient != null)
            {
                FRSSrv.ZReportResponce Res = null;
                try
                {
                    Res = FClient.ZReport(AlohainiFile.DepNum, AlohaTSClass.GetTermNum(), BD);
                }
                catch (Exception e)
                {
                    Utils.ToCardLog("Error  FClient.ZReport" + e.Message);
                    ZReportAskSaver.SaveZRepFRSAsk(BD);
                }
                if (Res != null)
                {
                    if (Res.Result)
                    {

                        if (iniFile.XFromGes)
                        {
                            decimal cash = 0;
                            decimal card = 0;
                            GesData.GetGesData(AlohainiFile.BDate, AlohainiFile.DepNum, out cash, out card);
                            decimal VCash = 0;
                            try { VCash = Res.OutData.VoidPayments.Where(a => a.ExternalId == 1 && a.Term == 0).First().Summ; }
                            catch { }
                            try { Res.OutData.Payments.Where(a => a.ExternalId == 1 && a.Term == 0).First().Summ = cash + VCash; }
                            catch { }
                        }


                        //if (iniFile.FRSPrintCheck)
                        {
                            if (Res.OutData.Payments.Count() > 0)
                            {
                                Utils.ToCardLog("FClient.ZReport print");
                                PrintOnWinPrinter.PrintDoc2(FiscalCheckCreator.GetEndOfSmenaVisual(Res));

                                try {
                                    PrintOnWinPrinter.PrintDoc2(FiscalCheckCreator.GetIMReportVisual(true));
                                } catch { }
                            
                            }
                            else
                            {
                                PrintOnWinPrinter.PrintDoc2(FiscalCheckCreator.GetZReportVisual(Res));
                            }
                        }
                    }
                }
                FClient.Close();
            }
            else
            {
                Utils.ToCardLog("FClient == null");
                ZReportAskSaver.SaveZRepFRSAsk(BD);
            }
        }


        public static void XReport()
        {
            FRSSrv.RemoteDataClient FClient = GetFRSClient();
            if (FClient != null)
            {
                FRSSrv.XReportResponce res = FClient.XReport(AlohainiFile.BDate, AlohainiFile.DepNum, AlohaTSClass.GetTermNum());
                if (res.Result)
                {
                    if (iniFile.XFromGes)
                    {
                        decimal cash = 0;
                        decimal card = 0;
                        GesData.GetGesData(AlohainiFile.BDate, AlohainiFile.DepNum, out cash, out card);
                        decimal VCash = 0;
                        try { VCash = res.VoidPayments.Where(a => a.ExternalId == 1 && a.Term==0).First().Summ; }
                        catch { }
                        try { res.Payments.Where(a => a.ExternalId == 1 && a.Term == 0).First().Summ = cash + VCash; }
                        catch { }
                    }
                    //if (iniFile.FRSPrintCheck)
                    {
                        PrintOnWinPrinter.PrintDoc2(FiscalCheckCreator.GetXReportVisual(res));
                    }
                }
                FClient.Close();
            }

        }


        public static void IMReport()
        {
            try
            {
                PrintOnWinPrinter.PrintDoc2(FiscalCheckCreator.GetIMReportVisual());
            }
            catch 
            {
            }

        }




        public static FRSSrv.AddCheckResponce SendAlohaChk(Check Chk)
        {
            return SendChk(GetFiskalCheckFromAlohaChk(Chk));
        }

        private static FRSSrv.FiskalCheck GetFiskalCheckFromAlohaChk(Check Chk)
        {
            FRSSrv.FiskalCheck Tmp = new FiskalCheck()
            {
                Id = Guid.NewGuid(),
                BusinessDate = Chk.BusinessDate,
                SystemDate = Chk.SystemDate,
                Dep = AlohainiFile.DepNum,
                Term = AlohaTSClass.GetTermNum(),
                ExtNum = Chk.AlohaCheckNum,
                ExtNumData = Chk.CheckShortNum.ToString(),
                ReturnSale = Chk.Vozvr,
                TableNum = Chk.TableNumber,
                Waiter = Chk.Waiter,
                Cassir = Chk.Cassir
            };
            List<FRSSrv.FiskalDish> Fdd = new List<FRSSrv.FiskalDish>();
            //foreach (Dish D in Chk.Dishez)
            foreach (Dish D in Chk.ConSolidateDishez)
            //foreach (Dish D in Chk.ConSolidateSpoolDishez)
            {
                if (D.BarCode == 999901) { continue; }

                FRSSrv.FiskalDish Fd = new FiskalDish()
                {
                    Count = (decimal)(D.QUANTITY * D.Count),
                    Name = D.CHITNAME + " " + D.CardPrefix + D.CardNumber,
                    Price = Math.Round(Math.Abs((decimal)D.Price) + Math.Abs((decimal)D.ServiceChargeSumm) / ((decimal)(D.QUANTITY * D.Count)), 2),
                    Barcode = D.BarCode
                };
                
                if (iniFile.FRNoTax) { Fd.Tax = 0; } else { Fd.Tax = 1; }
                Fdd.Add(Fd);
            }
            Tmp.Dishes = Fdd.ToArray();
            List<FRSSrv.FiskalPayment> Fpp = new List<FRSSrv.FiskalPayment>();

            FRSSrv.FiskalPayment Fp1 = new FiskalPayment()
            {
                Id = 1,
                Summ = Chk.CashSummWithOverpayment2
            };

            foreach (AlohaTender tdr in Chk.Tenders)
            {
                if (tdr.AlohaTenderId == 1)
                {
                    Fpp.Add(new FiskalPayment() { Id = 1, Summ = (decimal)Math.Abs(tdr.SummWithOverpayment), ExternalId = tdr.AlohaTenderId });
                }
                else if (tdr.AlohaTenderId == 20)
                {
                    Fpp.Add(new FiskalPayment() { Id = 4, Summ = (decimal)Math.Abs(tdr.Summ), ExternalId = tdr.AlohaTenderId });
                }
                else if (tdr.AlohaTenderId == 15)
                {
                    Fpp.Add(new FiskalPayment() { Id = 4, Summ = (decimal)Math.Abs(tdr.Summ), ExternalId = tdr.AlohaTenderId });
                }
                else if (AlohaTender.AlohaBallsTenderIds.Contains(tdr.AlohaTenderId)) //25,26
                {
                    if (tdr.AuthId == 2)
                    {
                        Fpp.Add(new FiskalPayment() { Id = 2, Summ = (decimal)Math.Abs(tdr.Summ), ExternalId = tdr.AlohaTenderId });
                    }
                }
                else
                {
                    Fpp.Add(new FiskalPayment() { Id = 0, Summ = (decimal)Math.Abs(tdr.Summ), ExternalId = tdr.AlohaTenderId });
                }

            }

            /*
            foreach (AlohaTender P in Chk.Tenders)
            {
                FRSSrv.FiskalPayment Fp = new FiskalPayment() { 
                Id = P.AlohaTenderId,
                Summ = (decimal)P.SummWithOverpayment
                };
                Fpp.Add(Fp);
            }
             * */

            Tmp.Payments = Fpp.ToArray();

            List<FRSSrv.FiskalDiscount> FDiscs = new List<FRSSrv.FiskalDiscount>();
            if (Math.Abs(Chk.Comp) > 0)
            {
                
                FRSSrv.FiskalDiscount Fds = new FiskalDiscount()
                {
                    Discount = true,

                };
                Fds.Name = "Комплексная скидка";
                if (Chk.CompId == AlohaTSClass.BonusCompId) { Fds.Name = "Оплата баллами"; }
                Fds.Summ = Chk.Comp;
                FDiscs.Add(Fds);
                
            }
            if (Chk.Dishez.Where(a => a.BarCode == 999901).Count() > 0)
            {
                Dish d = Chk.Dishez.Where(a => a.BarCode == 999901).First();
                
                FRSSrv.FiskalDiscount Fds = new FiskalDiscount()
                {
                   Discount = true,
                };
                Fds.Name ="Скидка "+ d.LongName;
                Fds.Summ = d.Price;

                //if (Chk.CompId == AlohaTSClass.BonusCompId) { Fds.Name = "Оплата баллами"; }
                ///Fds.Summ = Chk.Comp;
                FDiscs.Add(Fds);

            }
            Tmp.Discounts = FDiscs.ToArray();

            Tmp.StringsForPrintBefore = Chk.FrStringsBefore.ToArray();
            Tmp.StringsForPrintAfter = Chk.FrStringsAfter.ToArray();

            if (Chk.Tenders.Count > 0)
            {
                decimal DishSumm = Math.Abs(Tmp.Dishes.Sum(a => a.Price * a.Count));
                decimal PSumm = Math.Abs((decimal)Chk.Tenders.Sum(a => a.Summ));
                try
                {
                    if (Math.Abs(PSumm - DishSumm) >= 0.01m)
                    {
                        Utils.ToCardLog("GetFiskalCheckFromAlohaChk diff summ change " + (PSumm - DishSumm).ToString());
                        FiskalDish d = Tmp.Dishes.Where(a => Math.Abs(a.Price) > 0.01m && a.Count == Tmp.Dishes.Min(b => b.Count)).First();
                        Utils.ToCardLog("GetFiskalCheckFromAlohaChk diff summ change old Pr" + d.Price);
                        d.Price = d.Price + (PSumm - DishSumm) / d.Count;
                        Utils.ToCardLog("GetFiskalCheckFromAlohaChk diff summ change new Pr" + d.Price);
                    }
                }
                catch (Exception e)
                {
                    Utils.ToCardLog("Error GetFiskalCheckFromAlohaChk diff summ change " + e.Message);
                }
            }
            
            return Tmp;
        }

        private static FRSSrv.AddCheckResponce SendChk(FRSSrv.FiskalCheck Chk)
        {
            try
            {
                long Mbefore = GC.GetTotalMemory(false);
                Utils.ToCardLog("SendChk memory " + Mbefore);

                FRSSrv.AddCheckResponce Resp = new AddCheckResponce();
                FRSSrv.RemoteDataClient FClient = GetFRSClient();
                if (FClient != null)
                {

                    FRSSrv.AddCheckRequest Request = new FRSSrv.AddCheckRequest()
                    {
                        Id = new Guid(),
                        Check = Chk
                    };

                    Resp = FClient.AddCheck(Request);

                    if (Resp.Check.Sucсess)
                    {
                        Utils.ToCardLog("SendChk ExtNum " + Chk.ExtNum + " ClosedSucsees");
                        
                        if ((iniFile.FRSPrintCheck) && (!Resp.Check.WrongChk))
                        {
                            PrintOnWinPrinter.PrintDoc2(new PrintDocArgs() { FStrs = FiscalCheckCreator.GetFisckalCheckVisual(Resp), QRAsStr = Resp.Check.FROutData.QRAsStr });
                        }
                         
                    }
                    FClient.InnerChannel.Close();
                    FClient.InnerChannel.Dispose();
                    FClient.Close();
                    long MAfter = GC.GetTotalMemory(false);
                    Utils.ToCardLog("SendChk memory " + MAfter);
                }
                return Resp;
            }
            catch (Exception e)
            {
                Utils.ToCardLog("Error SendChk " + e.Message);
                return null;
            }




            //System.Windows.Forms.MessageBox.Show(Resp.Check.FD+" "+Resp.Check.FP);
        }

        public static void ReprintFChk(Guid CheckId)
        {
            try
            {
                FRSSrv.RemoteDataClient FClient = GetFRSClient();
                if (FClient != null)
                {
                    FRSSrv.AddCheckResponce Resp = FClient.PrintCheck(CheckId);

                    if (Resp.Check.Sucсess)
                    {
                        Utils.ToCardLog("ReprintFChk ExtNum " + Resp.Check.ExtNum + " ReprintFChk");
                        //  if (iniFile.FRSPrintCheck)
                        {
                                PrintOnWinPrinter.PrintDoc2(new PrintDocArgs() { FStrs = FiscalCheckCreator.GetFisckalCheckVisual(Resp), QRAsStr = Resp.Check.FROutData.QRAsStr });
                        }
                    }
                    try
                    {
                        if (Resp.ParentCheck != null)
                        {
                            if (Resp.ParentCheck.Sucсess)
                            {
                                Utils.ToCardLog("ReprintFChk Parent ExtNum " + Resp.Check.ExtNum + " ReprintFChk");
                                //  if (iniFile.FRSPrintCheck)
                                {
                                    PrintOnWinPrinter.PrintDoc2(new PrintDocArgs() { FStrs = FiscalCheckCreator.GetFisckalCheckVisual(Resp, true), QRAsStr = Resp.ParentCheck.FROutData.QRAsStr });
                                }
                            }
                        }
                    }
                    catch(Exception ee)
                    {
                        Utils.ToCardLog("Error SendChk ParentCheck" + ee.Message);
                    }
                    FClient.Close();
                }
            }
            catch (Exception e)
            {
                Utils.ToCardLog("Error SendChk " + e.Message);
            }
        }


        private static List<FRSSrv.FiskalCheck> GetCurentChecks()
        {
            try
            {
                Utils.ToCardLog("FRS GetCurentChecks");
                List<FRSSrv.FiskalCheck> Tmp = new List<FiskalCheck>();
                FRSSrv.RemoteDataClient FClient = GetFRSClient();
                if (FClient != null)
                {

                    int WCode = (AlohaTSClass.IsManager(AlohaTSClass.CurentWaiter) || (!AlohaTSClass.IsTableServise()) ? 0 : AlohaTSClass.CurentWaiter);
                    Tmp = FClient.GetLastChecks(DateTime.Now, AlohainiFile.DepNum, WCode).ToList();
                    FClient.Close();
                    
                    Utils.ToCardLog("Get "+Tmp.Count);
                }
                return Tmp;
            }
            catch(Exception e)
            {
                Utils.ToCardLog("[Error] FRS GetCurentChecks " + e.Message);
                return new List<FiskalCheck>();
            }
        }

        public static void PrintFCheckShowWnd()
        {

            ctrlShowOChecks ctrlSelectChk = new ctrlShowOChecks();
            ctrlSelectChk.InitData(GetCurentChecks());
            PDSystem.AlertModalWindow wnd = PDSystem.ModalWindowsForegraund.GetModalWindow(ctrlSelectChk);
            ctrlSelectChk.SetOwnerWnd(wnd);
            ctrlSelectChk.Height = 700;
            ctrlSelectChk.Width = 950;
            //wnd.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            //wnd.WindowState = System.Windows.WindowState.Maximized;
            wnd.ShowDialog();
        }

        public static void SendTestChk()
        {
            FiskalDish fd = new FiskalDish()
            {
                Count = 1.32m,
                Name = "Тест блюдо 1",
                Price = 25.35m
            };
            FiskalDish fd2 = new FiskalDish()
            {
                Count = 10m,
                Name = "Тест блюдо 2",
                Price = 20m
            };
            List<FiskalDish> Fdd = new List<FiskalDish>();
            Fdd.Add(fd);
            Fdd.Add(fd2);

            for (int i = 0; i < 25; i++)
            {
                FiskalDish fd3 = new FiskalDish()
                {
                    Count = 1m,
                    Name = "Тест блюдо 2",
                    Price = 20m
                };
                Fdd.Add(fd3);
            }

            FiskalCheck FCh = new FiskalCheck();
            FCh.BusinessDate = AlohainiFile.BDate;
            FCh.SystemDate = DateTime.Now;
            FCh.Id = Guid.NewGuid();
            FCh.Dishes = Fdd.ToArray();
            FCh.Dep = AlohainiFile.DepNum;
            FCh.Term = 0;
            Random R = new Random();
            FCh.ExtNumData = R.Next(4000000).ToString();

            FCh.ExtNum = R.Next(4000000);
            /*
            FiskalPayment Fp = new FiskalPayment()
            {
                Id = 1,
                Summ = 1000
            };
             * */
            FiskalPayment Fp2 = new FiskalPayment()
            {
                Id = 1,
                Summ = 223.46m + 1000m
            };
            FiskalPayment Fp3 = new FiskalPayment()
            {
                Id = 4,
                Summ = 10m
            };
            List<FiskalPayment> Fpp = new List<FiskalPayment>();
            Fpp.Add(Fp2);
            Fpp.Add(Fp3);
            FCh.Payments = Fpp.ToArray();

            SendChk(FCh);

        }

    }
}
