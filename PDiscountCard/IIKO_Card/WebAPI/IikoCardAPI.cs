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
using System.Xml;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.ComponentModel;
using Newtonsoft.Json.Converters;

namespace PDiscountCard.IIKO_Card
{
    /*
     * 
     * 
     * 
     *                                                  ************************************************************************************************
     * 
     *                                                       ЕДИНЫЙ КЛАСС ДЛЯ РАБОТЫ С iikoAPI, ПРАВИТЬ/ДОПОЛНЯТЬ ЛУЧШЕ НЕ ЗДЕСЬ, А ЦЕНТРАЛИЗОВАННО
     *                                                       
     *                                                                               Базовые функции для работы с iikoApi
     * 
     *                                                  ************************************************************************************************
     * 
     * 
     * 
    */
    class IikoCardApi : WebAPIBase
    {
        bool isHttps = false;
        public string DepRMS = null;

        public IikoCardApi(string _login, string _pass, string _urlAPI, string _baseAdress, string _DepRMS = null)
        {
            isHttps = _urlAPI.IndexOf("https:\\\\") != -1;

            authorized = false;
            token = null;

            login = _login;
            pass = _pass;

            urlAPI = _urlAPI;

            DepRMS = _DepRMS;

            client.BaseAddress = new Uri(_baseAdress);// @"http://coffemania.iiko.it:8080/");

            if (isHttps)
                System.Net.ServicePointManager.ServerCertificateValidationCallback = (senderX, certificate, chain, sslPolicyErrors) => { return true; };
        }

        // Бывает, что не логинится в iikoCard буз причины
        int maxTryLoginCount = 10;
        int delayBadRequestMS = 500;







        public override bool Login(out string _errorMessage)
        {
            _errorMessage = null;
            try
            {
                string path = urlAPI + string.Format("auth/access_token?user_id={0}&user_secret={1}", login, pass);
                //string path = urlAPI + string.Format("auth/biz_access_token?user_ext_id={0}", login, pass);
                HttpResponseMessage response = null;
                int loginCount = 0;
                bool doTry = true;
                while (doTry)
                {
                    response = client.GetAsync(path).Result;
                    loginCount++;
                    if (loginCount >= maxTryLoginCount || response.StatusCode != System.Net.HttpStatusCode.BadRequest)
                        doTry = false;
                    if (doTry)
                        System.Threading.Thread.Sleep(delayBadRequestMS);
                }

                if (response == null)
                {
                    _errorMessage = string.Format("HTTP Error");
                    AuthorizedStateCheck();
                    return false;
                }

                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    _errorMessage = string.Format("HTTP Error: {0} {1}", Convert.ToInt32(response.StatusCode), response.ReasonPhrase);
                    AuthorizedStateCheck();
                    return false;
                }

                if (response != null)
                {
                    using (StreamReader stream = new StreamReader(response.Content.ReadAsStreamAsync().Result))
                        token = stream.ReadToEnd();
                    token = token.Replace("\"", "");

                    var answerControl = "TOKEN_IS_OK";
                    string pathTest = urlAPI + string.Format("auth/echo?msg={0}&access_token={1}", answerControl, token);
                    //string pathTest = urlAPI + string.Format("organization/list?access_token={1}", "Test", token);

                    HttpResponseMessage responseTest = client.GetAsync(pathTest).Result;

                    if (responseTest.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        _errorMessage = string.Format("HTTP Error: {0} {1}", Convert.ToInt32(responseTest.StatusCode), responseTest.ReasonPhrase);
                        AuthorizedStateCheck();
                        return false;
                    }

                    if (responseTest != null)
                    {
                        string answer = null;
                        using (StreamReader stream = new StreamReader(responseTest.Content.ReadAsStreamAsync().Result))
                            answer = stream.ReadToEnd();
                        answer = answer.Replace("\"", "");

                        if (answer != answerControl)
                        {
                            _errorMessage = "Авторизация не завершена. Неправильный токен";
                            token = null;
                            AuthorizedStateCheck();
                            return false;
                        }
                    }
                    else
                    {
                        _errorMessage = "Авторизация не завершена. Не получен ответ от сервера при проверке токена";
                        token = null;
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
                string path = urlAPI + string.Format("logout");
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

        public List<IikoCard.OrganizationInfo> GetOrganizationInfos(out string _errorMessage)
        {
            List<IikoCard.OrganizationInfo> resultList = null;// new List<IikoNomenclature>();
            string serialized = GetString($"organization/list?access_token={token}", out _errorMessage);
            if (_errorMessage == null)
                resultList = JsonConvert.DeserializeObject<List<IikoCard.OrganizationInfo>>(serialized);
            return resultList;
        }

        public List<IikoCard.CorporateNutritionInfo> GetActivePrograms(string _organizationId, out string _errorMessage)
        {
            List<IikoCard.CorporateNutritionInfo> resultList = null;// new List<IikoNomenclature>();
            string serialized = GetString($"organization/{_organizationId}/corporate_nutritions?access_token={token}", out _errorMessage);
            if (_errorMessage == null)
                resultList = JsonConvert.DeserializeObject<List<IikoCard.CorporateNutritionInfo>>(serialized);
            return resultList;
        }
        public List<IikoCard.ExtendedCorporateNutritionInfo> GetActiveProgramsByOrganizationOrNetwork(string _organizationId, string _networkId, out string _errorMessage)
        {
            List<IikoCard.ExtendedCorporateNutritionInfo> resultList = null;// new List<IikoNomenclature>();
            string serialized = GetString($"organization/programs?access_token={token}&" + (_organizationId != null ? $"organization={_organizationId}" : $"network={_networkId}"), out _errorMessage);
            if (_errorMessage == null)
                resultList = JsonConvert.DeserializeObject<List<IikoCard.ExtendedCorporateNutritionInfo>>(serialized);
            return resultList;
        }

        public List<IikoCard.GuestBalance> GetBalancesByGuestsAndWallets_NOT_WORKING(string _organizationId, string _walletId, string[] _guestIds, out string _errorMessage)
        {
            List<IikoCard.GuestBalance> resultList = null;// new List<IikoNomenclature>();
            string serialized = GetString($"customers/get_balances_by_guests_and_wallet?access_token={token}&organizationId={_organizationId}&walletId={_walletId}", out _errorMessage,
                HttpMethod.Post, new { guestIds = _guestIds });
            if (_errorMessage == null)
                resultList = JsonConvert.DeserializeObject<List<IikoCard.GuestBalance>>(serialized);
            return resultList;
        }

        public bool GuestBalancePlus(IikoCard.ApiChangeBalanceRequest _request, out string _errorMessage)
        {
            string serialized = GetString($"customers/refill_balance?access_token={token}", out _errorMessage, HttpMethod.Post, _request);
            return _errorMessage == null;
        }
        public bool GuestBalanceMinus(IikoCard.ApiChangeBalanceRequest _request, out string _errorMessage)
        {
            string serialized = GetString($"customers/withdraw_balance?access_token={token}", out _errorMessage, HttpMethod.Post, _request);
            return _errorMessage == null;
        }

        public IikoCard.OrganizationGuestInfo GetGuestByPhone(string _organizationId, string _userPhone, out string _errorMessage)
        {
            IikoCard.OrganizationGuestInfo resultList = null;// new List<IikoNomenclature>();
            string serialized = GetString($"customers/get_customer_by_phone?access_token={token}&organization={_organizationId}&phone={_userPhone}", out _errorMessage);
            if (_errorMessage == null)
                resultList = JsonConvert.DeserializeObject<IikoCard.OrganizationGuestInfo>(serialized);
            return resultList;
        }
        public IikoCard.OrganizationGuestInfo GetGuestById(string _organizationId, string _id, out string _errorMessage)
        {
            IikoCard.OrganizationGuestInfo resultList = null;// new List<IikoNomenclature>();
            string serialized = GetString($"customers/get_customer_by_id?access_token={token}&organization={_organizationId}&id={_id}", out _errorMessage);
            if (_errorMessage == null)
                resultList = JsonConvert.DeserializeObject<IikoCard.OrganizationGuestInfo>(serialized);
            return resultList;
        }
        public IikoCard.OrganizationGuestInfo GetGuestByCardNumber(string _organizationId, string _cardNumber, out string _errorMessage)
        {
            IikoCard.OrganizationGuestInfo resultList = null;// new List<IikoNomenclature>();
            string serialized = GetString($"customers/get_customer_by_card?access_token={token}&organization={_organizationId}&card={_cardNumber}", out _errorMessage);
            if (_errorMessage == null)
                resultList = JsonConvert.DeserializeObject<IikoCard.OrganizationGuestInfo>(serialized);
            return resultList;
        }
        public List<IikoCard.ShortGuestInfo> GetGuestsShortInfoForPeriod(string _organizationId, DateTime _dateFrom, DateTime _dateTo, out string _errorMessage)
        {
            List<IikoCard.ShortGuestInfo> resultList = null;// new List<IikoNomenclature>();
            string serialized = GetString($"customers/get_customers_by_organization_and_by_period?access_token={token}&organization={_organizationId}&dateFrom={_dateFrom:yyyy-MM-dd}&dateTo={_dateTo:yyyy-MM-dd}", out _errorMessage);
            if (_errorMessage == null)
                resultList = JsonConvert.DeserializeObject<List<IikoCard.ShortGuestInfo>>(serialized);
            return resultList;
        }
        public string CreateOrUpdateGuest(string _organizationId, IikoCard.CustomerForImport _customer, out string _errorMessage)
        {
            //List<IikoCard.ShortGuestInfo> resultList = null;// new List<IikoNomenclature>();
            string serialized = GetString($"customers/create_or_update?access_token={token}&organization={_organizationId}", out _errorMessage,
                HttpMethod.Post, new { customer = _customer }
                );
            if (_errorMessage == null)
                return serialized;
            else
                return null;
        }
        public bool CreateGuestCard(string _organizationId, string _customerId, IikoCard.AddMagnetCardRequest _request, out string _errorMessage)
        {
            string serialized = GetString($"customers/{_customerId}/add_card?access_token={token}&organization={_organizationId}", out _errorMessage,
                HttpMethod.Post, _request);
            return (_errorMessage == null);
        }
        public bool DeleteGuestCard(string _organizationId, string _customerId, string _cardTrack, out string _errorMessage)
        {
            string serialized = GetString($"customers/{_customerId}/delete_card?access_token={token}&organization={_organizationId}&card_track={_cardTrack}", out _errorMessage,
                HttpMethod.Post, null);
            return (_errorMessage == null);
        }


    }

    namespace IikoCard
    {
        public class AddMagnetCardRequest
        {
            public string cardTrack;
            public string cardNumber;
        }
        public class WalletInfo
        {
            public string id;
            public string name;
            public string type;//1=Bonus,2=IikoCard=real,3=IikoCardInteger=int
            public string programType;//Money=1,Bonus=2,Product=3
        }
        public class Guest
        {
            public string id;
            public string name;
            public string lastName;
            public string birthday;
        }

        public class ApiChangeBalanceRequest
        {
            public string customerId;
            public string organizationId;
            public string walletId;
            public decimal sum;
            public string comment;
        }

        //public enum ConsentStatus { Undefined = 1, HasConsent = 1, ConsentDeprecated = 2 }
        [XmlRoot(ElementName = "CustomerForImport", Namespace = "http://tempuri.org/")]
        public class CustomerForImport
        {
            public string id;
            public string name;
            public string phone;
            public string magnetCardTrack;
            public string magnetCardNumber;
            public string birthday;
            public string email;
            public string middleName;
            public string surName;
            public string sex;
            public bool? shouldReceivePromoActionsInfo;
            public string referrerId;
            public string userData;
            public int consentStatus;

            //public ConsentStatus consentStatus;
        }
        [XmlRoot(ElementName = "customer", Namespace = "http://tempuri.org/")]
        public class CreateOrUpdateCustomer
        {
            public CustomerForImport customer;
        }

        public class GuestUserData
        {
            public int depNum;
            public string dateStart;
            public decimal sumStart;
        }

        public class OrganizationGuestInfo
        {
            public bool anonymized;
            public string id;
            public string name;
            public string phone;
            public string cultureName;
            public string birthday;
            public string email;
            public string surname;
            public string sex;
            public CustomerPhone[] additionalPhones;
            public GuestCardInfo[] cards;
            public GuestCategoryInfo[] categories;
            public UserWalletInfo[] walletBalances;
            public string middleName;
            public string referrerId;
            public string userData;
            public string comment;
            public int consentStatus;
            public decimal iikoCardOrdersSum;
            public bool isBlocked;
            public bool isDeleted;
            public object personalDataConsentFrom;
            public object personalDataConsentTo;
            public object personalDataProcessingFrom;
            public object personalDataProcessingTo;
            public object rank;
            public bool shouldReceiveLoyaltyInfo;
            public bool shouldReceiveOrderStatusInfo;
            public bool shouldReceivePromoActionsInfo;
        }

        public class CustomerPhone
        {
            public string phone;
        }
        public class GuestCardInfo
        {
            public string Id;
            public string Track;
            public string Number;
            public bool IsActivated;

            public string NetworkId;
            public string OrganizationId;
            public string OrganizationName;
            public string ValidToDate;
        }
        public class GuestCategoryInfo
        {
            public string id;
            public string name;
            public bool isActive;
            public bool isDefaultForNewGuests;
        }
        public class UserWalletInfo
        {
            public WalletInfo wallet;
            public decimal balance;
        }

        public enum BizOrganizationType { Unknowm = 0, Restaurant = 1, Shop = 2, TravelAgency = 3, BeautySalon = 4 }
        public class ContactInfo
        {
            public string phone { get; set; }
            public string location { get; set; }
            public string email { get; set; }
        }
        public class OrganizationInfo
        {
            public string id { get; set; }
            public string name { get; set; }
            public string description { get; set; }
            public string logo { get; set; }
            public ContactInfo contact { get; set; }
            public string homePage { get; set; }
            public string address { get; set; }
            public bool isActive { get; set; }
            public decimal? longitude { get; set; }
            public decimal? latitude { get; set; }
            public string networkId { get; set; }
            public string logoImage { get; set; }
            public string phone { get; set; }
            public string website { get; set; }
            public string averageCheque { get; set; }
            public string workTime { get; set; }
            public BizOrganizationType bizOrganizationType { get; set; }
            public string currencyIsoName { get; set; }
        }

        public class CorporateNutritionInfo
        {
            public string id;
            public string name;
            public string description;
            public string serviceFrom;
            public string serviceTo;
            public WalletInfo[] wallets;
        }
        public class ExtendedCorporateNutritionInfo
        {
            public string id;
            public string name;
            public string description;
            public string serviceFrom;
            public string serviceTo;
            public string walletId;
            public string organizationId;
            public string networkId;
            public bool notifyAboutBalanceChanges;
            public int programType;//0-money, 1-bonus, 2-product, 3-discount WO account
            public bool isActive;
            public MarketingCampaignInfo[] marketingCampaigns;
            public string[] appliedOrganizations;
        }

        public class MarketingCampaignInfo
        {
            public string id;
            public string name;
            public string description;
            public string programId;
            public string periodFrom;
            public string periodTo;
            public string organizationId;
            public string networkId;
            public bool isActive;
            public MarketingCampaignActionConditionBindingInfo[] orderActionConditionBindings;
            public MarketingCampaignActionConditionBindingInfo[] periodicActionConditionBindings;
            public MarketingCampaignActionConditionBindingInfo[] overdraftActionConditionBindings;
            public bool canHoldMoney;
        }

        public class MarketingCampaignActionConditionBindingInfo
        {
            public string id;
            public bool stopFurtherExecution;
            public MarketingCampaignActionInfo[] actions;
            public MarketingCampaignConditionInfo[] conditions;
        }

        public class MarketingCampaignActionInfo
        {
            public string id;
            public string settings;
            public string typeName;
            public string checkSum;
        }
        public class MarketingCampaignConditionInfo
        {
            public string id;
            public string settings;
            public string typeName;
            public string checkSum;
        }

        public class GuestBalance
        {
            public string guestId;
            public decimal balance;
        }
        public class GuestWalletRequest
        {
            public string[] guestIds;
        }

        public class ShortGuestInfo
        {
            public string id;
            public string name;
            public string phone;
            public string birthday;
            public string email;
            public string surname;
            public string sex;
            public CustomerPhone[] additionalPhones;
            public string comment;
            public string whenCreated;
            public string lastVisitDate;
        }
    }

}
