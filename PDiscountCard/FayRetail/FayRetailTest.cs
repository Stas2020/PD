using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDiscountCard.FayRetail
{
    public static class FayRetailTest
    {
        //static string CardTrack = "2611000037125";
        static string CardTrack = "2611000000105";
        static string Cashier = "1001";
        public static void Test1()
        {
            
         //   FayRetailClient.ApplyCardToCheck(CardTrack, GetTestChequeLines(), "1001");
        }


        public static void TestWnd()
        {
        
            FayRetailCheckInfo CheckInfo = new FayRetailCheckInfo()
            {
                Items = GetTestChequeLines(),
                ChequeNumber = "10002",
                ChequeDate = DateTime.Now
            };
            FayRetailMain.ShowWndApplyCard(CheckInfo);
        }
        public static string TestBalance()
        {
            RequestData BalanceData = FayRetailClient.GetBalanceRequestData(CardTrack, Cashier);
            string BalanceRequestStr = XMLSerializer.RequestSerializer(BalanceData);
            string BalAnsw = "";
            System.Net.HttpStatusCode StatusCode = System.Net.HttpStatusCode.OK;
            string res = FayRetailClient.SendDataToSrv(BalanceRequestStr, out BalAnsw, out StatusCode);

            return BalAnsw + Environment.NewLine + res;
        }


        public static void  TestResponse()
        {


        }


        public static string TestConfirmPurchase(string PurchaseID)
        {
            System.Net.HttpStatusCode StatusCode = System.Net.HttpStatusCode.OK;

            RequestData PurcData = FayRetailClient.GetConfirmPurchaseRequestData(Cashier, PurchaseID);
            string PurcRequestStr = XMLSerializer.RequestSerializer(PurcData);
            string PurcAnsw = "";
            string PurcAnsw2 = FayRetailClient.SendDataToSrv(PurcRequestStr, out PurcAnsw, out StatusCode);

            string Res = "Purc:" + Environment.NewLine +
                           PurcRequestStr + Environment.NewLine + Environment.NewLine +
                           "PurcResp:" + Environment.NewLine +
                           PurcAnsw + Environment.NewLine + PurcAnsw2 + Environment.NewLine;


            return Res;
        }


        public static string TestAddBonus(string PurchaseID)
        {
            FayRetailCheckInfo CheckInfo = new FayRetailCheckInfo()
            {
                Items = GetTestChequeLines(),
                ChequeNumber = "10002",
                ChequeDate = DateTime.Now
            };

            RequestData Data = FayRetailClient.GetCalculateRequestData(CardTrack, CheckInfo, Cashier);
            string CalcRequestStr = XMLSerializer.RequestSerializer(Data);
            string CalcAnsw = "";
            System.Net.HttpStatusCode StatusCode = System.Net.HttpStatusCode.OK;
            string CalcAnsw2 = FayRetailClient.SendDataToSrv(CalcRequestStr, out CalcAnsw, out StatusCode);

            RequestData DiscData = FayRetailClient.GetDiscountRequestData(CardTrack, CheckInfo, Cashier);
            string DiscRequestStr = XMLSerializer.RequestSerializer(DiscData);
            string DiscAnsw = "";
            string DiscAnsw2 = FayRetailClient.SendDataToSrv(DiscRequestStr, out DiscAnsw, out StatusCode);
            string DiscRespMessage = "";
            if (StatusCode == System.Net.HttpStatusCode.OK)
            {
                ResponseData Aswer = XMLSerializer.ResponseDeSerializer(DiscAnsw2);
                if (Aswer.ErrorCode == 0)
                {
                    if ((Aswer.Discounts != null) && (Aswer.Discounts.Count > 0))
                    {
                        DiscRespMessage = Aswer.Discounts[0].ChequeMessageDecript;
                    }
                    else
                    {
                        DiscRespMessage = "Пустой ответ от сервера.";
                    }

                }
                else
                {
                    DiscRespMessage = "Ошибка в  ответе от сервера: " + Aswer.ErrorMessage;
                }
               
            }


            /*
            RequestData PurcData = FayRetailClient.GetConfirmPurchaseRequestData(Cashier, PurchaseID);
            string PurcRequestStr = XMLSerializer.RequestSerializer(PurcData);
            string PurcAnsw = "";
            string PurcAnsw2 = FayRetailClient.SendDataToSrv(PurcRequestStr, out PurcAnsw, out StatusCode);
            */
            string Resstr = "Calc:" + Environment.NewLine +
                            CalcRequestStr + Environment.NewLine +
                            "CalcResp:" + Environment.NewLine +
                            CalcAnsw + Environment.NewLine + CalcAnsw2 + Environment.NewLine +
                            "Disc:" + Environment.NewLine +
                            DiscRequestStr + Environment.NewLine + Environment.NewLine +
                            "DiscResp:" + Environment.NewLine +
                            DiscAnsw + Environment.NewLine + DiscAnsw2 + Environment.NewLine +
                            "DiscRespMessage:" + Environment.NewLine +
                            DiscRespMessage + Environment.NewLine;
                            /*
                            "Purc:" + Environment.NewLine +
                            PurcRequestStr + Environment.NewLine + Environment.NewLine +
                            "PurcResp:" + Environment.NewLine +
                            PurcAnsw + Environment.NewLine + PurcAnsw2 + Environment.NewLine;
                             * */
            return Resstr;
        }

        public static List<string> TestClose()
        {
            FayRetailMain.AddCheckToAddBonus("10005", CardTrack);

            Check Chk = new Check();
            Chk.AlohaCheckNum = 10005;
            Chk.Dishez = new List<Dish>();
            Dish d1 = new Dish()
            {
                BarCode = 100,
                Price = 123.5m,
                Count = 1,
                LongName = "Test D1",
                QtyQUANTITY=1,
                QUANTITY=1
            };
            Chk.Dishez.Add(d1);
            Dish d2 = new Dish()
            {
                BarCode = 200,
                Price = 150.5m,
                Count = 1,
                LongName = "Test D2",
                QtyQUANTITY = 1,
                QUANTITY = 1
            };
            Chk.Dishez.Add(d2);
            List<string> res = FayRetailMain.CloseCheck(Chk);
            return res;
        }

        public static string TestPayment(string PurchaseID, double PAmount)
        {
            FayRetailCheckInfo CheckInfo = new FayRetailCheckInfo()
            {
                Items = GetTestChequeLines(),
                ChequeNumber = "10005",
                ChequeDate = DateTime.Now
            };

            RequestData Data = FayRetailClient.GetCalculateRequestData(CardTrack, CheckInfo, Cashier);
            string CalcRequestStr = XMLSerializer.RequestSerializer(Data);
            string CalcAnsw = "";
            System.Net.HttpStatusCode StatusCode = System.Net.HttpStatusCode.OK;
            string CalcAnsw2 = FayRetailClient.SendDataToSrv(CalcRequestStr, out CalcAnsw, out StatusCode);

            RequestData PData = FayRetailClient.GetPaymentRequestData(CardTrack, CheckInfo, Cashier, PAmount);
            string PRequestStr = XMLSerializer.RequestSerializer(PData);
            string PAnsw = "";
            string PAnsw2 = FayRetailClient.SendDataToSrv(PRequestStr, out PAnsw, out StatusCode);
            string PRespMessage = "";
            if (StatusCode == System.Net.HttpStatusCode.OK)
            {
                ResponseData Aswer = XMLSerializer.ResponseDeSerializer(PAnsw2);
                if (Aswer.ErrorCode == 0)
                {
                    if ((Aswer.Payments != null) && (Aswer.Payments.Count > 0))
                    {
                        PRespMessage = Aswer.Payments[0].ChequeMessageDecript;
                    }
                    else
                    {
                        PRespMessage = "Пустой ответ от сервера.";
                    }

                }
                else
                {
                    PRespMessage = "Ошибка в  ответе от сервера: " + Aswer.ErrorMessage;
                }

            }
            CheckInfo.Pays = new List<Pay>();
            //CheckInfo.Pays.Clear();

            CheckInfo.Pays.Add(new Pay()
            {
                Amount = PAmount.ToString(),
                Type = "FAYRETAIL"
            });
            CheckInfo.Pays.Add(new Pay()
            {
                Amount = (CheckInfo.TotalSumm - PAmount).ToString(),
                Type = "Cash"
            }
                );  
            RequestData DiscData = FayRetailClient.GetDiscountRequestData(CardTrack, CheckInfo, Cashier);
            string DiscRequestStr = XMLSerializer.RequestSerializer(DiscData);
            string DiscAnsw = "";
            string DiscAnsw2 = FayRetailClient.SendDataToSrv(DiscRequestStr, out DiscAnsw, out StatusCode);
            string DiscRespMessage = "";
            if (StatusCode == System.Net.HttpStatusCode.OK)
            {
                ResponseData Aswer = XMLSerializer.ResponseDeSerializer(DiscAnsw2);
                if (Aswer.ErrorCode == 0)
                {
                    if ((Aswer.Discounts != null) && (Aswer.Discounts.Count > 0))
                    {
                        DiscRespMessage = Aswer.Discounts[0].ChequeMessageDecript;
                    }
                    else
                    {
                        DiscRespMessage = "Пустой ответ от сервера.";
                    }

                }
                else
                {
                    DiscRespMessage = "Ошибка в  ответе от сервера: " + Aswer.ErrorMessage;
                }

            }
           
            /*
            RequestData PurcData = FayRetailClient.GetConfirmPurchaseRequestData(Cashier, PurchaseID);
            string PurcRequestStr = XMLSerializer.RequestSerializer(PurcData);
            string PurcAnsw = "";
            string PurcAnsw2 = FayRetailClient.SendDataToSrv(PurcRequestStr, out PurcAnsw, out StatusCode);
            */
            string Resstr = "Calc:" + Environment.NewLine +
                            CalcRequestStr + Environment.NewLine +
                            "CalcResp:" + Environment.NewLine +
                            CalcAnsw + Environment.NewLine + CalcAnsw2 + Environment.NewLine +
                            "Disc:" + Environment.NewLine +
                           PRequestStr + Environment.NewLine + Environment.NewLine +
                            "DiscResp:" + Environment.NewLine +
                            PAnsw + Environment.NewLine + PAnsw2 + Environment.NewLine +
                            "DiscRespMessage:" + Environment.NewLine +
                            PRespMessage + Environment.NewLine+
                             "Disc:" + Environment.NewLine +
                            DiscRequestStr + Environment.NewLine + Environment.NewLine +
                            "DiscResp:" + Environment.NewLine +
                            DiscAnsw + Environment.NewLine + DiscAnsw2 + Environment.NewLine +
                            "DiscRespMessage:" + Environment.NewLine +
                            DiscRespMessage + Environment.NewLine;
            /*
            "Purc:" + Environment.NewLine +
            PurcRequestStr + Environment.NewLine + Environment.NewLine +
            "PurcResp:" + Environment.NewLine +
            PurcAnsw + Environment.NewLine + PurcAnsw2 + Environment.NewLine;
             * */
            return Resstr;
        }


        public static string  Test2()
        {
            string PurchaseID = Guid.NewGuid().ToString().Replace("-", "");
            
            FayRetailCheckInfo CheckInfo = new FayRetailCheckInfo()
            {
                Items = GetTestChequeLines(),
                ChequeNumber = "10002",
                ChequeDate = DateTime.Now
            };
            Pay P1 = new Pay()
            {
                Amount = (150.5 + 123.5).ToString(),
                Type = "Card"
            };
            CheckInfo.Pays = new List<Pay>();
            CheckInfo.Pays.Add(P1);


            RequestData Data = FayRetailClient.GetCalculateRequestData(CardTrack, CheckInfo, Cashier);
            string CalcRequestStr = XMLSerializer.RequestSerializer(Data);
            string CalcAnsw="";
            System.Net.HttpStatusCode StatusCode = System.Net.HttpStatusCode.OK;
            string CalcAnsw2 = FayRetailClient.SendDataToSrv(CalcRequestStr, out CalcAnsw, out StatusCode);

            
            RequestData DiscData = FayRetailClient.GetDiscountRequestData(CardTrack, CheckInfo, Cashier);
            string DiscRequestStr = XMLSerializer.RequestSerializer(DiscData);
            string DiscAnsw = "";
            string DiscAnsw2 = FayRetailClient.SendDataToSrv(DiscRequestStr, out DiscAnsw, out StatusCode);

            RequestData PurcData = FayRetailClient.GetConfirmPurchaseRequestData(Cashier, PurchaseID);
            string PurcRequestStr = XMLSerializer.RequestSerializer(PurcData);
            string PurcAnsw = "";
            string PurcAnsw2 = FayRetailClient.SendDataToSrv(PurcRequestStr, out PurcAnsw, out StatusCode);

            string Resstr = "Calc:" + Environment.NewLine +
                            CalcRequestStr + Environment.NewLine +
                            "CalcResp:" + Environment.NewLine +
                            CalcAnsw + Environment.NewLine + CalcAnsw2+ Environment.NewLine +
                            "Disc:" + Environment.NewLine +
                            DiscRequestStr + Environment.NewLine + Environment.NewLine +
                            "DiscResp:" + Environment.NewLine +
                            DiscAnsw + Environment.NewLine +DiscAnsw2+ Environment.NewLine +
                            "Purc:" + Environment.NewLine +
                            PurcRequestStr + Environment.NewLine +Environment.NewLine +
                            "PurcResp:" + Environment.NewLine +
                            PurcAnsw + Environment.NewLine + PurcAnsw2 + Environment.NewLine;
            return Resstr;
        }

        public static string  GetTest1XmlString()
        {


            string PurchaseID = Guid.NewGuid().ToString().Replace("-", "");
            FayRetailCheckInfo CheckInfo = new FayRetailCheckInfo ()
            {
            Items = GetTestChequeLines(),
            ChequeNumber ="10002",
            ChequeDate = DateTime.Now
            };
            RequestData Data = FayRetailClient.GetCalculateRequestData(CardTrack, CheckInfo, "1001");
            
            return XMLSerializer.RequestSerializer(Data);


        }


        public static List<ChequeLine> GetTestChequeLines()
        {
            List<ChequeLine> Tmp = new List<ChequeLine>();
            ChequeLine L1 = new ChequeLine()
            {
                PosID = 1,
                Amount = 123.5,
                Barcode = "100",
                Name = "Test D1",
                Quantity = 1,
                Price = 123.5
            };
            ChequeLine L2 = new ChequeLine()
            {
                PosID = 2,
                Amount = 150.5,
                Barcode = "200",
                Name = "Test D2",
                Quantity = 1,
                Price = 150.5
            };
            Tmp.Add(L1);
            Tmp.Add(L2);

            

            return Tmp;
        }
    }
}
