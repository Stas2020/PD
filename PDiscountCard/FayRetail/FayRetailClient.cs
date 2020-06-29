using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Net;
using System.IO;

namespace PDiscountCard.FayRetail
{
   

    public static class FayRetailClient
    {



        public static BalanceResponse GetBalance(string CardTrack, string Cashier, out string ErrMessage)
        {
            ErrMessage = "";
            try
            {
                Utils.ToCardLog("GetBalance CardTrack: " + CardTrack);
                RequestData BalanceRequest = GetBalanceRequestData(CardTrack, Cashier);
                string BalRequestStr = XMLSerializer.RequestSerializer(BalanceRequest);
                string StatusDescription = "";
                System.Net.HttpStatusCode StatusCode = System.Net.HttpStatusCode.OK;
                string CalcAnsw2 = FayRetailClient.SendDataToSrv(BalRequestStr, out StatusDescription, out StatusCode);
                if (StatusCode == HttpStatusCode.OK)
                {
                    ResponseData Aswer = XMLSerializer.ResponseDeSerializer(CalcAnsw2);
                    if (Aswer.ErrorCode == 0)
                    {
                        if ((Aswer.Balances != null) && (Aswer.Balances.Count > 0))
                        {
                            return Aswer.Balances[0];
                        }
                        ErrMessage = "Не могу показать баланс по карте. Пустой ответ от сервера.";
                        return null;
                    }
                    ErrMessage = "Не могу показать баланс по карте. Ошибка в  ответе от сервера: " + Aswer.ErrorMessage;
                    return null;
                }
                ErrMessage = "Не могу показать баланс по карте. Ошибка сервера: " + StatusDescription;
                return null;
            }
            catch (Exception e)
            {
                Utils.ToCardLog("Error ApplyCardToCheck " + e.Message);
                return null;
            }
        }






        public static CalculateResponse ApplyCardToCheck(string CardTrack, FayRetailCheckInfo  CheckInfo, string Cashier,  out string ErrMessage)
        {
                ErrMessage = "";
                try
                {
                Utils.ToCardLog("ApplyCardToCheck CardTrack: " + CardTrack);
                RequestData CalcRequest = GetCalculateRequestData(CardTrack, CheckInfo, Cashier);
                string CalcRequestStr = XMLSerializer.RequestSerializer(CalcRequest);
                string StatusDescription = "";
                System.Net.HttpStatusCode StatusCode = System.Net.HttpStatusCode.OK;
                string CalcAnsw2 = FayRetailClient.SendDataToSrv(CalcRequestStr, out StatusDescription, out StatusCode);
                if (StatusCode == HttpStatusCode.OK)
                {
                    ResponseData Aswer = XMLSerializer.ResponseDeSerializer(CalcAnsw2);
                    if (Aswer.ErrorCode == 0)
                    {
                        if ((Aswer.Calculates != null) && (Aswer.Calculates.Count > 0))
                        {
                            return Aswer.Calculates[0];
                        }
                        ErrMessage = "Не могу показать баланс по карте. Пустой ответ от сервера.";
                        return null;
                    }
                    ErrMessage = "Не могу показать баланс по карте. Ошибка в  ответе от сервера: " + Aswer.ErrorMessage;
                    return null;
                }
                ErrMessage = "Не могу показать баланс по карте. Ошибка сервера: " + StatusDescription;
                return null;
            }
            catch(Exception e)
            {
                Utils.ToCardLog("Error ApplyCardToCheck " + e.Message);
                return null;
            }
        }



        public static bool AddBonustoCard(string CardTrack, FayRetailCheckInfo CheckInfo, string Cashier, out string Message)
        {
                Message = "";
                try
                {
                    Utils.ToCardLog("AddBonustoCard CardTrack: " + CardTrack);
                    RequestData DiscRequest = GetDiscountRequestData(CardTrack, CheckInfo, Cashier);
                    string DiscRequestStr = XMLSerializer.RequestSerializer(DiscRequest);
                    string StatusDescription = "";
                    System.Net.HttpStatusCode StatusCode = System.Net.HttpStatusCode.OK;
                    string CalcAnsw2 = FayRetailClient.SendDataToSrv(DiscRequestStr, out StatusDescription, out StatusCode);
                    if (StatusCode == HttpStatusCode.OK)
                    {
                        ResponseData Aswer = XMLSerializer.ResponseDeSerializer(CalcAnsw2);
                        if (Aswer.ErrorCode == 0)
                        {
                            if ((Aswer.Discounts != null) && (Aswer.Discounts.Count > 0))
                            {
                                Message = Aswer.Discounts[0].ChequeMessageDecript;
                                return true;
                            }
                            Message = "Пустой ответ от сервера.";
                            return false;
                        }
                        Message = "Ошибка в  ответе от сервера: " + Aswer.ErrorMessage;
                        return false;
                    }
                    Message = "Ошибка сервера: " + StatusDescription;
                    return false;
                }
                catch (Exception e)
                {
                    Message = "Error AddBonustoCard " + e.Message;
                    //Utils.ToCardLog("Error AddBonustoCard " + e.Message);
                    return false;
                }
        }


        public static bool ApplyFayRetPaymentToCheck(string CardTrack, FayRetailCheckInfo CheckInfo, string Cashier, double Amount, out string Message)
        {
            Message = "";
            try
            {
                Utils.ToCardLog("ApplyDiscount CardTrack: " + CardTrack);
                RequestData PaymentRequest = GetPaymentRequestData(CardTrack, CheckInfo, Cashier, Amount);
                string PaymentRequestStr = XMLSerializer.RequestSerializer(PaymentRequest);
                string StatusDescription = "";
                System.Net.HttpStatusCode StatusCode = System.Net.HttpStatusCode.OK;
                string CalcAnsw2 = FayRetailClient.SendDataToSrv(PaymentRequestStr, out StatusDescription, out StatusCode);
                Utils.ToCardLog("ApplyDiscount FayRetailClient.SendDataToSrv end: StatusCode: " + StatusCode.ToString());

                
                if (StatusCode == HttpStatusCode.OK)
                {
                    ResponseData Aswer = XMLSerializer.ResponseDeSerializer(CalcAnsw2);
                    if (Aswer.ErrorCode == 0)
                    {
                        if ((Aswer.Payments != null) && (Aswer.Payments.Count > 0))
                        {
                            Message = Aswer.Payments[0].ChequeMessageDecript;
                            return true;
                        }
                        Message = "Пустой ответ от сервера.";
                        return false;
                    }
                    Message = "Ошибка в  ответе от сервера: " + Aswer.ErrorMessage;
                    return false;
                }
                Message = "Ошибка сервера: " + StatusDescription;
                return false;
            }
            catch (Exception e)
            {
                Message = "Error ApplyFayRetPaymentToCheck " + e.Message;
                Utils.ToCardLog("Error ApplyFayRetPaymentToCheck " + e.Message);
                return false;
            }
        }




        public static bool SendConfirm(string PurchaseId, string  Cashier, out string Message)
        {
            Message = "";
            try
            {
                Utils.ToCardLog("SendConfirm PurchaseId: " + PurchaseId);
                RequestData PurchaseRequest = GetConfirmPurchaseRequestData(PurchaseId, Cashier);
                string PurchaseRequestStr = XMLSerializer.RequestSerializer(PurchaseRequest);
                string StatusDescription = "";
                System.Net.HttpStatusCode StatusCode = System.Net.HttpStatusCode.OK;
                string CalcAnsw2 = FayRetailClient.SendDataToSrv(PurchaseRequestStr, out StatusDescription, out StatusCode);
                Utils.ToCardLog("SendConfirm FayRetailClient.SendDataToSrv end: StatusCode: " + StatusCode.ToString());


                if (StatusCode == HttpStatusCode.OK)
                {
                    ResponseData Aswer = XMLSerializer.ResponseDeSerializer(CalcAnsw2);
                    if (Aswer.ErrorCode == 0)
                    {
                       Utils.ToCardLog("SendConfirm Ok");
                            return true;
                       
                    }
                    Message = "Ошибка в  ответе от сервера: " + Aswer.ErrorMessage;
                    return false;
                }
                Message = "Ошибка сервера: " + StatusDescription;
                return false;
            }
            catch (Exception e)
            {
                Message = "Error ApplyFayRetPaymentToCheck " + e.Message;
                Utils.ToCardLog("Error ApplyFayRetPaymentToCheck " + e.Message);
                return false;
            }
        }


        public static RequestData GetCalculateRequestData(string CardTrack, FayRetailCheckInfo CheckInfo, string Cashier)
        {
            RequestData Request = new RequestData();
            Request.Calculates = new List<CalculateRequest>();
            CalculateRequest CalcRequest = new CalculateRequest();
            CalcRequest = (CalculateRequest)GetCommandRequest(CalcRequest, CardTrack, 1, Cashier);
            CalcRequest.Cashier = Cashier;
            CalcRequest.PurchaseID = CheckInfo.PurchaseID;
            CalcRequest.Cheque = new Cheque(CheckInfo);
            Request.Calculates.Add(CalcRequest);
            return Request;
        }

        public static RequestData GetBalanceRequestData(string CardTrack, string Cashier)
        {
            RequestData Request = new RequestData();
            BalanceRequest Balances = new BalanceRequest();
            Balances = (BalanceRequest)GetCommandRequest(Balances, CardTrack, 2, Cashier);
            Request.Balances = new List<BalanceRequest>();
            Request.Balances.Add(Balances);
            return Request;
        }


        public static RequestData GetDiscountRequestData(string CardTrack, FayRetailCheckInfo CheckInfo, string Cashier)
        {
            RequestData Request = new RequestData();
            Request.Discounts = new List<DiscountRequest>();
            DiscountRequest CalcRequest = new DiscountRequest();
            CalcRequest = (DiscountRequest)GetCommandRequest(CalcRequest, CardTrack, 1, Cashier);
            CalcRequest.Cashier = Cashier;
            CalcRequest.PurchaseID = CheckInfo.PurchaseID;
            CalcRequest.Cheque = new Cheque(CheckInfo);
            CalcRequest.Pays = CheckInfo.Pays;
            Request.Discounts.Add(CalcRequest);
           return Request;

        }


        public static RequestData GetPaymentRequestData(string CardTrack, FayRetailCheckInfo CheckInfo, string Cashier, double Amount)
        {
            RequestData Request = new RequestData();
            Request.Payments = new List<PaymentRequest>();
            PaymentRequest CalcRequest = new PaymentRequest();
            CalcRequest = (PaymentRequest)GetCommandRequest(CalcRequest, CardTrack, 1, Cashier);
            CalcRequest.Cashier = Cashier;
            CalcRequest.PurchaseID = CheckInfo.PurchaseID;
            CalcRequest.Cheque = new Cheque(CheckInfo);
            CalcRequest.Amount = Amount;
            Request.Payments.Add(CalcRequest);
            return Request;

        }


        public static RequestData GetConfirmPurchaseRequestData(string Cashier, string PurchaseID)
        {
            RequestData Request = new RequestData();
            Request.ConfirmPurchases = new List<ConfirmPurchaseRequest>();
            ConfirmPurchaseRequest CalcRequest = new ConfirmPurchaseRequest();
            CalcRequest = (ConfirmPurchaseRequest)GetCommandRequest(CalcRequest, "", 1, Cashier);
            CalcRequest.Cashier = Cashier;
            CalcRequest.PurchaseID = PurchaseID;
            Request.ConfirmPurchases.Add(CalcRequest);
            return Request;

        }

        public static ICommandRequest GetCommandRequest(ICommandRequest Comm ,  string CardTrack, int ElId, string Cashier)
        {
            
            
                Comm.DeviceLogicalID = iniFile.FayRetailDeviceLogicalID;
                Comm.OperationDate = DateTime.Now;
                Comm.ElementID = ElId;
                Comm.OperationID = Guid.NewGuid().ToString().Replace("-", "");
                Comm.Cashier = Cashier;
                if (CardTrack != "")
                {
                    Comm.Card = new Card()
                       {
                           Track2 = CardTrack
                       };
                }
            return Comm;
        }

        public static string SendDataToSrv(string XmlRequest, out string StatusDescription, out HttpStatusCode StatusCode)
        {
            StatusDescription = "";
            StatusCode= HttpStatusCode.OK;
            try
            {
                Utils.ToCardLog("FayRetailClient.SendDataToSrv Xml: " + XmlRequest);
                string Login = iniFile.FayRetailLogin;
                string Pass = iniFile.FayRetailPass;
                WebRequest request = WebRequest.Create(iniFile.FayRetailServer);
  
                System.Net.CredentialCache credentialCache = new System.Net.CredentialCache();
                credentialCache.Add(
                    new System.Uri(iniFile.FayRetailServer),
                    "Basic",
                    new System.Net.NetworkCredential(Login, Pass)
                );

                request.Credentials = credentialCache;

                request.Method = "POST";
                byte[] byteArray = Encoding.UTF8.GetBytes(XmlRequest);
                request.ContentType = "text/xml; encoding='utf-8'";
                request.ContentLength = byteArray.Length;
                request.Headers.Add(HttpRequestHeader.ContentEncoding, "UTF-8");
                //request.PreAuthenticate = true;
                Stream dataStream = request.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                
                Utils.ToCardLog("FayRetailClient.SendDataToSrv send xml2 " + iniFile.FayRetailServer);
                WebResponse response = request.GetResponse();
                dataStream.Close();
                StatusDescription = ((HttpWebResponse)response).StatusDescription;
                StatusCode = ((HttpWebResponse)response).StatusCode;
                Utils.ToCardLog(String.Format("FayRetailClient.SendDataToSrv send xml StatusCode: {0}, StatusDescription: {1} ", StatusCode, StatusDescription));
                dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                string responseFromServer = reader.ReadToEnd();
                Utils.ToCardLog("FayRetailClient.SendDataToSrv response " + responseFromServer);
                reader.Close();
                dataStream.Close();
                response.Close();
                return responseFromServer;
            }
            catch (Exception e)
            {
                StatusCode = HttpStatusCode.NoContent;
                StatusDescription = "Error FayRetailClient.SendDataToSrv " + e.Message;
                Utils.ToCardLog("Error FayRetailClient.SendDataToSrv " + e.Message);
                return "";
            }
        }

    }
}
