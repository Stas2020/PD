using Newtonsoft.Json;
using PDiscountCard.IIKO_Card.WebAPI;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDiscountCard.IIKO_Card
{

    // Здесь есть метод удаления карты


    public class S2010_CardHelper  : ICardHelper
    {
        static string IikoCardFlag = "[S2010Card] ";
        override public bool SendToIikoCard(GiftCard card)
        {
            DateTime tm = DateTime.Now;

            var cardNumber = card.CardCode;
            var depNum = card.NumShop;
            var dateStart = $"{card.DTCreate:yyyy-MM-dd}";
            var sum = card.Balance;
            var active = card.Active;

            Utils.ToLog($"{IikoCardFlag}Создание карты {cardNumber} в iikoCard");

            S2010CardApi S2010CardApi = new S2010CardApi(s2010CardLogin, s2010CardPWD, s2010CardURI, s2010CardDomain);
            if (S2010CardApi == null)
            {
                Utils.ToLog($"{IikoCardFlag}Не удалось авторизоваться на S2010. {GetTiming(tm)}");
                return false;
            }
            S2010CardApi.Login(out string errorMessageLoginS2010);
            var newCard = S2010CardApi.CreateOrUpdateGiftCard(card,
                    out string errorMessageGuestCreate);
            if (newCard == null)
            {
                Utils.ToLog($"{IikoCardFlag}Не удалось создать карту с номером {cardNumber} от {dateStart} уч.{depNum}. Сообщение: {errorMessageGuestCreate}");
                return false;
            }

            Utils.ToLog($"{IikoCardFlag}Карта {cardNumber} от {dateStart}/{depNum} создана ({(active ? "активна" : "неактивна")}). {GetTiming(tm)}");
            return true;
        }
        public bool DeleteCard(String card_code)
        {
            DateTime tm = DateTime.Now;

            Utils.ToLog($"{IikoCardFlag}Удаление карты {card_code}");

            S2010CardApi S2010CardApi = new S2010CardApi(s2010CardLogin, s2010CardPWD, s2010CardURI, s2010CardDomain);
            if (S2010CardApi == null)
            {
                Utils.ToLog($"{IikoCardFlag}Не удалось авторизоваться на S2010. {GetTiming(tm)}");
                return false;
            }

            if (S2010CardApi.DeleteGiftCard(card_code, out string errorMessageDelete))
            {
                Utils.ToLog($"{IikoCardFlag}Удалена карта {card_code}. {GetTiming(tm)}");
                return true;
            }
            else
            {
                Utils.ToLog($"{IikoCardFlag}Не удалось удалить карту {card_code}. {GetTiming(tm)}. Сообщение: {errorMessageDelete}");
                return false;
            }
        }
        override public bool SetCardActiveStatus(String card_code, bool activeStatus)
        {
            DateTime tm = DateTime.Now;

            Utils.ToLog($"{IikoCardFlag}Установка статуса карты {card_code} active={activeStatus}");

            S2010CardApi S2010CardApi = new S2010CardApi(s2010CardLogin, s2010CardPWD, s2010CardURI, s2010CardDomain);
            if (S2010CardApi == null)
            {
                Utils.ToLog($"{IikoCardFlag}Не удалось авторизоваться на S2010. {GetTiming(tm)}");
                return false;
            }
            S2010CardApi.Login(out string errorMessageLoginS2010);
            string errorMessageActivate;
            var operOk = activeStatus
                ? S2010CardApi.ActivateGiftCard(card_code, out errorMessageActivate)
                : S2010CardApi.DeactivateGiftCard(card_code, out errorMessageActivate);
            if (operOk)
            {
                Utils.ToLog($"{IikoCardFlag}{(activeStatus ? "Активирована" : "Деактивирована")} карта {card_code}. {GetTiming(tm)}");
                return true;
            }
            else
            {
                Utils.ToLog($"{IikoCardFlag}Не удалось {(activeStatus ? "активировать" : "деактивировать")} карту {card_code}. {GetTiming(tm)}. Сообщение: {errorMessageActivate}");
                return false;
            }
        }

        override public GiftCard GetCard(String card_code)
        {
            DateTime tm = DateTime.Now;

            Utils.ToLog($"{IikoCardFlag}Запрос баланса карты {card_code} в iikoCard");


            S2010CardApi S2010CardApi = new S2010CardApi(s2010CardLogin, s2010CardPWD, s2010CardURI, s2010CardDomain);
            if (S2010CardApi == null)
            {
                Utils.ToLog($"{IikoCardFlag}Не удалось авторизоваться на S2010. {GetTiming(tm)}");
                return null;
            }
            S2010CardApi.Login(out string errorMessageLoginS2010);
            var result = S2010CardApi.GetGiftCardByNumber(card_code, out string errorGuest);

            if(result == null)
            {
                Utils.ToLog($"{IikoCardFlag}Карта {card_code} не найдена в iikoCard. {GetTiming(tm)}. Сообщение: {errorGuest} ");
                return null;
            }
            return result;

        }

        override public bool PayFromCard(String card_code, decimal sum, int depNum)
        {
            return ChangeCardBalance(card_code, sum, depNum, false);
        }

        override public bool ReturnToCard(String card_code, decimal sum, int depNum)
        {
            return ChangeCardBalance(card_code, sum, depNum, true);
        }

        private bool ChangeCardBalance(String card_code, decimal sum, int depNum, bool isReturn)
        {
            DateTime tm = DateTime.Now;
            string actName = isReturn ? "Возврат" : "Списание";
            Utils.ToLog($"{IikoCardFlag}{actName} с карты {card_code} в iikoCard в размере {sum}");

            S2010CardApi S2010CardApi = new S2010CardApi(s2010CardLogin, s2010CardPWD, s2010CardURI, s2010CardDomain);
            if (S2010CardApi == null)
            {
                Utils.ToLog($"{IikoCardFlag}Не удалось авторизоваться на S2010. {GetTiming(tm)}");
                return false;
            }
            if (sum < 0)
            {
                Utils.ToLog($"{IikoCardFlag}Ошибка: сумма должна быть положительной. {GetTiming(tm)}");
                return false;
            }
            S2010CardApi.Login(out string errorMessageLoginS2010);
            string errorTrans;
            var transactOk = isReturn
                ? S2010CardApi.ReturnToGiftCard(card_code, sum, depNum, out errorTrans)
                : S2010CardApi.PayFromGiftCard(card_code, sum, depNum, out errorTrans);
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





        string s2010CardLogin = "manager";
        string s2010CardPWD = "bf3c2a80220b6cfec6d3b850f5d1319808e756a4963d9266aa7fb7e71bc48bde";
        string s2010CardURI = @"https://s2010/giftcard/api/";
        string s2010CardDomain = @"https://s2010";
     
        static int timingMax = 2;
        static string GetTiming(DateTime startTime)
        {
            var timing = (Math.Round((DateTime.Now - startTime).TotalSeconds, 3));
            return $"Тайминг:{timing} cек{(timing >= timingMax ? " (!!!)" : "")}";
        }
    }

}
