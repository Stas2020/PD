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
        public bool SendToIikoCard(GiftCard card)
        {
            var cardNumber = card.CardCode;
            var depNum = card.NumShop;
            var dateStart = $"{card.DTCreate:yyyy-MM-dd}";
            var sum = card.Balance;
            var active = card.Active;

            Utils.ToLog($"Создания карты {cardNumber} в iikoCard");

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
                bool balanceUpdated = iikoCardApi.GuestBalancePlus(new IikoCard.ApiChangeBalanceRequest()
                {
                    customerId = newGuestId,
                    organizationId = orgId,
                    walletId = walletId,
                    sum = (decimal)sum,
                    comment = $"{depNum}"
                }, out string errorMessageBalancePlus);
                if (!balanceUpdated)
                {
                    Utils.ToLog($"Не удалось зачислить гостю с номером карты {cardNumber} в {depNum} от {dateStart} бонусы в размере {sum}. Сообщение: {errorMessageBalancePlus}");
                    return false;
                }
            }
            else
            {
                Utils.ToLog($"Не удалось создать госта с номером карты {cardNumber} в {depNum} от {dateStart}. Сообщение: {errorMessageGuestCreate}");
                return false;
            }
            return true;
        }

        public bool SetCardActiveStatus(String card_code, bool acviteStatus)
        {
            Utils.ToLog($"Установка статуса карты {card_code} active={acviteStatus}");

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
                        Utils.ToLog($"Невозможно распознать дату выпуска карты {card_code} в iikoCard. UserData={guest.userData}");
                        //return null;
                    }
                    depNum = userData.depNum;
                    if (userData.active != null)
                        active = (bool)userData.active;
                    sum = userData.sumStart;
                }
                catch (Exception ex)
                {
                    Utils.ToLog($"Невозможно распознать объект userData карты {card_code} в iikoCard. UserData={guest.userData} Сообщение: {ex.Message}");
                    //return null;
                }

                var newGuestId = iikoCardApi.CreateOrUpdateGuest(orgId, new IikoCard.CustomerForImport()
                {
                    id = guest.id,
                    userData = JsonConvert.SerializeObject(new IikoCard.GuestUserData() { depNum = depNum, dateStart = $"{dateStart:yyyy-MM-dd}", sumStart = (decimal)sum, active = acviteStatus })
                }, out string errorMessageGuestCreate);

                if (newGuestId != null)
                {
                    Utils.ToLog($"{(acviteStatus ? "Активирована" : "Деактивирована")} карта {card_code}");
                    return true;
                }
                else
                {
                    Utils.ToLog($"Не удалось {(acviteStatus ? "активировать" : "деактивировать")} карту {card_code}. Сообщение: {errorMessageGuestCreate}");
                    return false;
                }

            }
            else
            {
                Utils.ToLog($"Карта {card_code} не найдена в iikoCard. Сообщение: {errorGuest} ");
                return false;
            }


        }

        public GiftCard GetCard(String card_code)
        {
            Utils.ToLog($"Запрос баланса карты {card_code} в iikoCard");

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
                        Utils.ToLog($"Невозможно распознать дату выпуска карты {card_code} в iikoCard. UserData={guest.userData}");
                        //return null;
                    }
                    depNum = userData.depNum;
                    if (userData.active != null)
                        active = (bool)userData.active;
                }
                catch(Exception ex)
                {
                    Utils.ToLog($"Невозможно распознать объект userData карты {card_code} в iikoCard. UserData={guest.userData}. Сообщение: {ex.Message}");
                    //return null;
                }
            }
            else
            {
                Utils.ToLog($"Карта {card_code} не найдена в iikoCard. Сообщение: {errorGuest} ");
                return null;
            }

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
            string actName = isReturn ? "Возврат" : "Списание";
            Utils.ToLog($"{actName} с карты {card_code} в iikoCard в размере {sum}");

            IikoCardApi iikoCardApi = TryInitIikoCard(out string networkId, out string orgId, out string walletId);

            if (iikoCardApi == null)
                return false;

            var guest = iikoCardApi.GetGuestByCardNumber(orgId, card_code, out string errorGuest);
            if (guest != null)
            {
                var balance = guest.walletBalances.Where(_wb => _wb.wallet.id == walletId).Sum(_wb => _wb.balance);
                if (balance < sum && !isReturn)
                {
                    Utils.ToLog($"Остаток на карте {card_code} (balance бонусов) менее списываевой суммы в {sum}");
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
                        Utils.ToLog($"{actName} {sum} бонусов с карты {card_code}");
                        return true;
                    }
                    else
                    {
                        Utils.ToLog($"{actName} не удалось. {sum} бонусов с карты {card_code}. Сообщение: {errorTrans}");
                        return false;
                    }
                }
            }
            else
            {
                Utils.ToLog($"Карта {card_code} не найдена. Сообщение: {errorGuest}");
                return false;
            }
        }





        string iikoCardLogin = "apiCoffeemania";
        string iikoCardPWD = "Shmanager2022";
        string iikoCardURI = "https://iiko.biz:9900/api/0/";
        string iikoCardDomain = "https://iiko.biz";
        private IikoCardApi TryInitIikoCard(out string networkId, out string orgId, out string walletId)
        {
            networkId = null;
            orgId = null;
            walletId = null;

            IikoCardApi iikoCardApi = new IikoCardApi(iikoCardLogin, iikoCardPWD, iikoCardURI, iikoCardDomain);
            bool loginOk = iikoCardApi.Login(out string errorMessageLogin);
            if (!loginOk)
            {
                // Не удалось авторизоваться в iikoCard errorMessageLogin
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
                return null;
            }

            var orgCoffeemania = organizationsAll.FirstOrDefault(_org => _org.name.ToUpper() == "COFFEEMANIA");
            if (orgCoffeemania == null)
            {
                // Не удалось найти организацию Coffeemania в iikoCard
                return null;
            }

            networkId = orgCoffeemania.networkId;
            orgId = orgCoffeemania.id;

            var programsAll = iikoCardApi.GetActiveProgramsByOrganizationOrNetwork(null, networkId, out string errorMessageProgs);
            if (programsAll == null)
            {
                // Не удалось получить список программ в iikoCard errorMessageProgs
                return null;
            }
            var progAlohaSV = programsAll.FirstOrDefault(_pr => Convert.ToString(_pr.description).ToUpper().IndexOf("ALOHA") != -1
                                                             || Convert.ToString(_pr.name).ToUpper().IndexOf("ALOHA") != -1);
            if (progAlohaSV == null)
            {
                // Не удалось найти программу Подарочные карты Aloha Stored Values в iikoCard
                return null;
            }

            walletId = progAlohaSV.walletId;

            return iikoCardApi;
        }
    }
}
