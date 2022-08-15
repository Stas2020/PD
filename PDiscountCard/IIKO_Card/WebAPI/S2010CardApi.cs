using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;
using Newtonsoft.Json;
using System.IO;

namespace PDiscountCard.IIKO_Card.WebAPI
{
    class S2010CardApi : WebAPIBase
    {
        public S2010CardApi(string _login, string _pass, string _urlAPI, string _baseAdress)
        {
            authorized = false;
            token = null;

            login = _login;
            pass = _pass;

            urlAPI = _urlAPI;

            client.BaseAddress = new Uri(_baseAdress);

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            System.Net.ServicePointManager.ServerCertificateValidationCallback = (senderX, certificate, chain, sslPolicyErrors) => { return true; };
        }

        public override bool Login(out string _errorMessage)
        {
            _errorMessage = null;
            try
            {
                string path = urlAPI + string.Format("Auth/GetUser?Login={0}&Password={1}", login, pass);
                HttpResponseMessage response = client.GetAsync(path).Result;

                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    _errorMessage = string.Format("HTTP Error: {0} {1}", Convert.ToInt32(response.StatusCode), response.ReasonPhrase);
                    AuthorizedStateCheck();
                    return false;
                }

                if (response != null)
                {
                    try
                    {
                        HttpResponseHeaders headers = response.Headers;

                        string[] cookiesArr = headers.GetValues("Set-Cookie").ToArray();

                        if (cookiesArr.Count(f => f.Contains("id=")) > 0)
                            if (cookiesArr.First(f => f.Contains("id=")).Length > cookiesArr.First(f => f.Contains("id=")).IndexOf("="))
                            {
                                token = cookiesArr.First(f => f.Contains("id=")).Substring(cookiesArr.First(f => f.Contains("id=")).IndexOf('=') + 1);
                                //cookies = new CookieCollection();
                                //cookies.Add(new Cookie("id", token) { Domain = cookieDomain });
                            }
                        authorized = true;

                    }
                    catch (Exception ex)
                    {
                        _errorMessage = ex.Message;
                        AuthorizedStateCheck();
                        return false;
                    }
                }
                else
                {
                    _errorMessage = "Авторизация не завершена. Не получен ответ от сервера";
                    AuthorizedStateCheck();
                    return false;
                }

                AuthorizedStateCheck();
                return true;
            }
            catch (Exception ex)
            {
                _errorMessage = ex.Message;
                AuthorizedStateCheck();
                return false;
            }
        }
        public override bool Logout(out string _errorMessage)
        {
            _errorMessage = null;
            try
            {
                string path = urlAPI + string.Format("Auth/GetLogoutUser/{0}", login);
                HttpResponseMessage response = client.GetAsync(path).Result;

                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    _errorMessage = string.Format("HTTP Error: {0} {1}", Convert.ToInt32(response.StatusCode), response.ReasonPhrase);
                    AuthorizedStateCheck();
                    return false;
                }

                authorized = false;
                token = null;

                AuthorizedStateCheck();
                return true;
            }
            catch (Exception ex)
            {
                _errorMessage = ex.Message;
                AuthorizedStateCheck();
                return false;
            }
        }
        //public async Task<List<S2010User>> GetUsers()
        //{
        //    string path = $"https://s2010/complaints/api/info/GetCheckListReportStats";
        //    List<S2010User> s2010Users = null;
        //    HttpResponseMessage response = await client.GetAsync(path);
        //    if (response.IsSuccessStatusCode)
        //    {
        //        s2010Users = await response.Content.ReadAsAsync<List<S2010User>>();
        //    }
        //    return s2010Users;
        //}
        public List<GiftCard> GetGiftAllCards(DateTime _minDate, out string _errorMessage)
        {
            List<GiftCard> result = null;
            string serialized = GetString($"GiftCard/GetAllCards?MinDate={_minDate:yyyy-MM-dd}", out _errorMessage);
            if (_errorMessage == null)
                result = JsonConvert.DeserializeObject<List<GiftCard>>(serialized);
            return result;
        }
        public GiftCard GetGiftCardByNumber(string _cardCode, out string _errorMessage)
        {
            GiftCard result = null;
            string serialized = GetString($"GiftCard/GetGiftCard?CardCode={_cardCode}", out _errorMessage);
            if (_errorMessage == null)
                result = JsonConvert.DeserializeObject<GiftCard>(serialized);
            return result;
        }
        public GiftCard CreateOrUpdateGiftCard(GiftCard _giftCard, out string _errorMessage)
        {
            GiftCard result = null;
            string serialized = GetString("GiftCard/AddGiftCard", out _errorMessage, HttpMethod.Post,
                new GiftCardForRequest()
                {
                    card_code_ = _giftCard.CardCode,
                    dt_create_ = $"{_giftCard.DTCreate:yyyy-MM-dd}",
                    balance_ = _giftCard.Balance,
                    num_shop_ = _giftCard.NumShop,
                    active_ = _giftCard.Active
                });
            if (_errorMessage == null)
                result = JsonConvert.DeserializeObject<GiftCard>(serialized);
            return result;
        }
        public bool DeleteGiftCard(string _cardCode, out string _errorMessage)
        {
            string serialized = GetString("GiftCard/DeleteGiftCard", out _errorMessage, HttpMethod.Post, _cardCode);
            return (_errorMessage == null && JsonConvert.DeserializeObject<string>(serialized) == _cardCode);
        }
        public bool ActivateGiftCard(string _cardCode, out string _errorMessage)
        {
            string serialized = GetString("GiftCard/ActivateGiftCard", out _errorMessage, HttpMethod.Post, _cardCode);
            return (_errorMessage == null && JsonConvert.DeserializeObject<string>(serialized) == _cardCode);
        }
        public bool DeactivateGiftCard(string _cardCode, out string _errorMessage)
        {
            string serialized = GetString("GiftCard/DeactivateGiftCard", out _errorMessage, HttpMethod.Post, _cardCode);
            return (_errorMessage == null && JsonConvert.DeserializeObject<string>(serialized) == _cardCode);
        }
        public bool PayFromGiftCard(string CardCode, decimal Sum, int Dep, out string _errorMessage)
        {
            GiftCardBalanceRequest resultObj = null;
            string serialized = GetString("GiftCard/PayFromGiftCard", out _errorMessage, HttpMethod.Post, new GiftCardBalanceRequest() { CardCode = CardCode, Sum = Sum, Dep = Dep });
            if (_errorMessage == null)
                resultObj = JsonConvert.DeserializeObject<GiftCardBalanceRequest>(serialized);
            return (_errorMessage == null && resultObj != null);
        }
        public bool ReturnToGiftCard(string CardCode, decimal Sum, int Dep, out string _errorMessage)
        {
            GiftCardBalanceRequest resultObj = null;
            string serialized = GetString("GiftCard/ReturnToGiftCard", out _errorMessage, HttpMethod.Post, new GiftCardBalanceRequest() { CardCode = CardCode, Sum = Sum, Dep = Dep });
            if (_errorMessage == null)
                resultObj = JsonConvert.DeserializeObject<GiftCardBalanceRequest>(serialized);
            return (_errorMessage == null && resultObj != null);
        }















        class GiftCardBalanceRequest
        {
            public string CardCode;
            public decimal Sum;
            public int Dep;
        }
        public class GiftCardForRequest
        {
            public GiftCardForRequest()
            {
            }
            public string card_code_;
            //public DateTime dt_create_;
            public string dt_create_;
            public int num_shop_;
            public double balance_;
            public bool active_;
        }
    }
}
