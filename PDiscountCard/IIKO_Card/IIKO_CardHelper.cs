using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDiscountCard.IIKO_Card
{
    class IIKO_CardHelper
    {
        static string IikoCardFlag = "[iikoCard] ";
        public bool SendToIikoCard(GiftCard card)
        {
            DateTime tm = DateTime.Now;

            var cardNumber = card.CardCode;
            var depNum = card.NumShop;
            var dateStart = $"{card.DTCreate:yyyy-MM-dd}";
            var sum = card.Balance;
            var active = card.Active;

            Utils.ToLog($"{IikoCardFlag}Создание карты {cardNumber} в iikoCard");

            IikoCardApi iikoCardApi = TryInitIikoCard(out string networkId, out string orgId, out string walletId);

            if (iikoCardApi == null)
                return false;

            var newGuestId = iikoCardApi.CreateOrUpdateGuest(orgId, new IikoCard.CustomerForImport()
            {
                magnetCardNumber = cardNumber,
                magnetCardTrack = cardNumber,
                userData = JsonConvert.SerializeObject(new IikoCard.GuestUserData() { depNum = depNum, dateStart = dateStart, sumStart = (decimal)sum, active = active })
            }, out string errorMessageGuestCreate);
            if (newGuestId != null)
            {
                newGuestId = newGuestId.Replace("\"", "");


                var guest = iikoCardApi.GetGuestById(orgId, newGuestId, out string errorGuestFind);
                if(guest == null)
                {
                    Utils.ToLog($"{IikoCardFlag}Не обновить вновь созданную карту {cardNumber} в {depNum} от {dateStart}. {GetTiming(tm)}. Сообщение: {errorGuestFind}");
                    return false;
                }
                else
                {
                    var balance = guest.walletBalances.Where(_wb => _wb.wallet.id == walletId).Sum(_wb => _wb.balance);

                    Utils.ToLog($"{IikoCardFlag}На карте {cardNumber} уже имелось {balance} бонусов. {GetTiming(tm)}.");

                    double needToPut = sum - (double)balance;
                    //if(needToPut != 0)
                    if (Math.Abs(needToPut) > 0.1)
                    {
                        var balanceRequest = new IikoCard.ApiChangeBalanceRequest()
                        {
                            customerId = newGuestId,
                            organizationId = orgId,
                            walletId = walletId,
                            sum = (decimal)Math.Abs(sum),
                            comment = $"{depNum}"
                        };
                        string errorMessageBalanceChange;
                        bool balanceUpdated = needToPut > 0
                            ? iikoCardApi.GuestBalancePlus(balanceRequest, out errorMessageBalanceChange)
                            : iikoCardApi.GuestBalanceMinus(balanceRequest, out errorMessageBalanceChange);
                        if (!balanceUpdated)
                        {
                            Utils.ToLog($"{IikoCardFlag}Не удалось {(needToPut > 0 ? "зачислить на карту" : "списать с карты")} {cardNumber} в {depNum} от {dateStart} бонусы в размере {sum}. {GetTiming(tm)}. Сообщение: {errorMessageBalanceChange}");
                            return false;
                        }
                    }
                }


                




            }
            else
            {
                Utils.ToLog($"{IikoCardFlag}Не удалось создать карту {cardNumber} в {depNum} от {dateStart}. {GetTiming(tm)}. Сообщение: {errorMessageGuestCreate}");
                return false;
            }
            Utils.ToLog($"{IikoCardFlag}Карта {cardNumber} от {dateStart}/{depNum} создана ({(active ? "активна" : "неактивна")}). {GetTiming(tm)}");
            return true;
        }

        public bool SetCardActiveStatus(String card_code, bool activeStatus)
        {
            DateTime tm = DateTime.Now;

            Utils.ToLog($"{IikoCardFlag}Установка статуса карты {card_code} active={activeStatus}");

            IikoCardApi iikoCardApi = TryInitIikoCard(out string networkId, out string orgId, out string walletId);

            if (iikoCardApi == null)
                return false;



            var guest = iikoCardApi.GetGuestByCardNumber(orgId, card_code, out string errorGuest);
            decimal balance = 0;
            DateTime dateStart = DateTime.Now;
            int depNum = 0;
            decimal sum = 0;
            bool active = true;
            if (guest != null)
            {
                balance = guest.walletBalances.Where(_wb => _wb.wallet.id == walletId).Sum(_wb => _wb.balance);
                try
                {
                    IikoCard.GuestUserData userData = JsonConvert.DeserializeObject<IikoCard.GuestUserData>(guest.userData);
                    if (!DateTime.TryParseExact(userData.dateStart, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateStart))
                    {
                        Utils.ToLog($"{IikoCardFlag}Невозможно распознать дату выпуска карты {card_code} в iikoCard. {GetTiming(tm)}. UserData={guest.userData}");
                        //return null;
                    }
                    depNum = userData.depNum;
                    if (userData.active != null)
                        active = (bool)userData.active;
                    sum = userData.sumStart;
                }
                catch (Exception ex)
                {
                    Utils.ToLog($"{IikoCardFlag}Невозможно распознать объект userData карты {card_code} в iikoCard. {GetTiming(tm)}. UserData={guest.userData} Сообщение: {ex.Message}");
                    //return null;
                }

                var newGuestId = iikoCardApi.CreateOrUpdateGuest(orgId, new IikoCard.CustomerForImport()
                {
                    id = guest.id,
                    userData = JsonConvert.SerializeObject(new IikoCard.GuestUserData() { depNum = depNum, dateStart = $"{dateStart:yyyy-MM-dd}", sumStart = (decimal)sum, active = activeStatus })
                }, out string errorMessageGuestCreate);

                if (newGuestId != null)
                {
                    Utils.ToLog($"{IikoCardFlag}{(activeStatus ? "Активирована" : "Деактивирована")} карта {card_code}. {GetTiming(tm)}");
                    return true;
                }
                else
                {
                    Utils.ToLog($"{IikoCardFlag}Не удалось {(activeStatus ? "активировать" : "деактивировать")} карту {card_code}. {GetTiming(tm)}. Сообщение: {errorMessageGuestCreate}");
                    return false;
                }

            }
            else
            {
                Utils.ToLog($"{IikoCardFlag}Карта {card_code} не найдена в iikoCard. {GetTiming(tm)}. Сообщение: {errorGuest} ");
                return false;
            }


        }

        public GiftCard GetCard(String card_code)
        {
            DateTime tm = DateTime.Now;

            Utils.ToLog($"{IikoCardFlag}Запрос баланса карты {card_code} в iikoCard");

            IikoCardApi iikoCardApi = TryInitIikoCard(out string networkId, out string orgId, out string walletId);

            if (iikoCardApi == null)
                return null;

            var guest = iikoCardApi.GetGuestByCardNumber(orgId, card_code, out string errorGuest);
            decimal balance = 0;
            DateTime date = DateTime.Now;
            int depNum = 0;
            bool active = true;
            if (guest != null)
            {
                balance = guest.walletBalances.Where(_wb => _wb.wallet.id == walletId).Sum(_wb => _wb.balance);
                try
                {
                    IikoCard.GuestUserData userData = JsonConvert.DeserializeObject<IikoCard.GuestUserData>(guest.userData);
                    if (!DateTime.TryParseExact(userData.dateStart, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
                    {
                        Utils.ToLog($"{IikoCardFlag}Невозможно распознать дату выпуска карты {card_code} в iikoCard. {GetTiming(tm)}. UserData={guest.userData}");
                        //return null;
                    }
                    depNum = userData.depNum;
                    if (userData.active != null)
                        active = (bool)userData.active;
                }
                catch(Exception ex)
                {
                    Utils.ToLog($"{IikoCardFlag}Невозможно распознать объект userData карты {card_code} в iikoCard. {GetTiming(tm)}. UserData={guest.userData}. Сообщение: {ex.Message}");
                    //return null;
                }
            }
            else
            {
                Utils.ToLog($"{IikoCardFlag}Карта {card_code} не найдена в iikoCard. {GetTiming(tm)}. Сообщение: {errorGuest} ");
                return null;
            }

            Utils.ToLog($"{IikoCardFlag}Карта {card_code} от {date}/{depNum} найдена ({(active ? "активна" : "неактивна")}), баланс: {balance} . {GetTiming(tm)}");
            return new GiftCard(card_code, date, depNum, (double)balance, active);
        }

        public bool PayFromCard(String card_code, decimal sum, int depNum)
        {
            return ChangeCardBalance(card_code, sum, depNum, false);
        }

        public bool ReturnToCard(String card_code, decimal sum, int depNum)
        {
            return ChangeCardBalance(card_code, sum, depNum, true);
        }





        private bool ChangeCardBalance(String card_code, decimal sum, int depNum, bool isReturn)
        {
            DateTime tm = DateTime.Now;
            string actName = isReturn ? "Возврат" : "Списание";
            Utils.ToLog($"{IikoCardFlag}{actName} с карты {card_code} в iikoCard в размере {sum}");

            IikoCardApi iikoCardApi = TryInitIikoCard(out string networkId, out string orgId, out string walletId);

            if (iikoCardApi == null)
                return false;

            var guest = iikoCardApi.GetGuestByCardNumber(orgId, card_code, out string errorGuest);
            if (guest != null)
            {
                var balance = guest.walletBalances.Where(_wb => _wb.wallet.id == walletId).Sum(_wb => _wb.balance);
                if (balance < sum && !isReturn)
                {
                    Utils.ToLog($"{IikoCardFlag}Остаток на карте {card_code} (balance бонусов) менее списываевой суммы в {sum}. {GetTiming(tm)}");
                    return false;
                }
                else
                {
                    var request = new IikoCard.ApiChangeBalanceRequest()
                    {
                        customerId = guest.id,
                        organizationId = orgId,
                        walletId = walletId,
                        sum = sum,
                        comment = $"{depNum}"
                    };

                    string errorTrans;
                    var transactOk = isReturn
                        ? iikoCardApi.GuestBalancePlus(request, out errorTrans) 
                        : iikoCardApi.GuestBalanceMinus(request, out errorTrans);

                    if (transactOk)
                    {
                        Utils.ToLog($"{IikoCardFlag}{actName} {sum} бонусов с карты {card_code}. {GetTiming(tm)}");
                        return true;
                    }
                    else
                    {
                        Utils.ToLog($"{IikoCardFlag}{actName} не удалось. {sum} бонусов с карты {card_code}. {GetTiming(tm)}. Сообщение: {errorTrans}");
                        return false;
                    }
                }
            }
            else
            {
                Utils.ToLog($"{IikoCardFlag}Карта {card_code} не найдена. Сообщение: {errorGuest}");
                return false;
            }
        }





        string iikoCardLogin = "apiCoffeemania";
        string iikoCardPWD = "Shmanager2022";
        string iikoCardURI = "https://iiko.biz:9900/api/0/";
        string iikoCardDomain = "https://iiko.biz";
        private IikoCardApi TryInitIikoCard(out string networkId, out string orgId, out string walletId)
        {
            DateTime tm = DateTime.Now;

            networkId = null;
            orgId = null;
            walletId = null;

            IikoCardApi iikoCardApi = new IikoCardApi(iikoCardLogin, iikoCardPWD, iikoCardURI, iikoCardDomain);
            bool loginOk = iikoCardApi.Login(out string errorMessageLogin);
            if (!loginOk)
            {
                // Не удалось авторизоваться в iikoCard errorMessageLogin
                Utils.ToLog($"{IikoCardFlag}Не удалось авторизоваться в iikoCard. {GetTiming(tm)}. Сообщение: {errorMessageLogin}");
                return null;
            }

            // Сейчас всё это захардкожено, но дальше находятся рабочие методы по
            // получению этих ГУИДов из iikoCard
            // Прим. Вечером сильно тормозят
            networkId = "03650000-6bec-ac1f-47ae-08da6b3f4a31";
            orgId = "03650000-6bec-ac1f-26e0-08da6b3e22d3";
            walletId = "01330000-6bec-ac1f-38dd-08da753ab95d";
            return iikoCardApi;


            ///////


            var organizationsAll = iikoCardApi.GetOrganizationInfos(out string errorMessageGetOrgs);

            if (organizationsAll == null)
            {
                // Не удалось получить список организаций в iikoCard errorMessageGetOrgs
                Utils.ToLog($"{IikoCardFlag}Не удалось получить список организаций в iikoCard. Сообщение: {errorMessageGetOrgs}");
                return null;
            }

            var orgCoffeemania = organizationsAll.FirstOrDefault(_org => _org.name.ToUpper() == "COFFEEMANIA");
            if (orgCoffeemania == null)
            {
                // Не удалось найти организацию Coffeemania в iikoCard
                Utils.ToLog($"{IikoCardFlag}Не удалось найти организацию Coffeemania");
                return null;
            }

            networkId = orgCoffeemania.networkId;
            orgId = orgCoffeemania.id;

            var programsAll = iikoCardApi.GetActiveProgramsByOrganizationOrNetwork(null, networkId, out string errorMessageProgs);
            if (programsAll == null)
            {
                // Не удалось получить список программ в iikoCard errorMessageProgs
                Utils.ToLog($"{IikoCardFlag}Не удалось получить список программ в iikoCard. Сообщение: {errorMessageProgs}");
                return null;
            }
            var progAlohaSV = programsAll.FirstOrDefault(_pr => Convert.ToString(_pr.description).ToUpper().IndexOf("ALOHA") != -1
                                                             || Convert.ToString(_pr.name).ToUpper().IndexOf("ALOHA") != -1);
            if (progAlohaSV == null)
            {
                // Не удалось найти программу Подарочные карты Aloha Stored Values в iikoCard
                Utils.ToLog($"{IikoCardFlag}Не удалось найти программу Подарочные карты Aloha Stored Values в iikoCard");
                return null;
            }

            walletId = progAlohaSV.walletId;

            return iikoCardApi;
        }
        static int timingMax = 2;
        static string GetTiming(DateTime startTime)
        {
            var timing = (Math.Round((DateTime.Now - startTime).TotalSeconds, 3));
            return $"Тайминг:{timing} cек{(timing >= timingMax ? " (!!!)" : "")}";
        }
    }

}
