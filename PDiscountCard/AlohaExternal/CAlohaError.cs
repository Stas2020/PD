using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDiscountCard
{
    public enum AlohaErrEnum
    {
        ErrCOM_NoError = 0x00,
        ErrCOM_NoOneLoggedIn = 0x07,
        ErrCOM_EmpAlreadyClockedIn = 0x08,
        ErrCOM_EmpToManyShiftsToday = 0x09,
        ErrCOM_EmpInvalidJobcode = 0x0A,
        ErrCOM_UnknownEmpClockInError = 0x0B,
        ErrCOM_EmpCannotWorkAnotherShift = 0x6C,
        ErrCOM_InvalidEmpPassword = 0x04,
        ErrCOM_CouldNotFindEmployeeFromId = 0x03,
        ErrCOM_InvalidMagCard = 0x05,
        ErrCOM_EmpDrawerNotConfirmed = 0x2C,
        ErrCOM_UnavailableItem = 0x26,
        ErrCOM_EmpLoggedOnOtherTerm = 0x06,
        ErrCOM_SomeoneAlreadyLoggedIn = 0x02,
        ErrCOM_EmpIsMagcardOnly = 0x63,
        ErrCOM_UnauthorizedCOMTerm = 0x01,
        ErrCOM_SetExpiredPasswordRequired = 0x76,
        ErrCOM_SetClearedPasswordRequired = 0x77,
        ErrCOM_SetUninitPasswordRequird = 0x78,
        ErrCOM_FOHCOM_ServerException = 0x4F,

        ErrCOM_TableNotFound = 0x31,
        ErrCOM_TableInUse = 0x32,
        ErrCOM_TableActiveOnOtherTerm = 0x20,
        ErrCOM_InvalidTable = 0x12,
        ErrCOM_TableIsClosed = 0x14,
        ErrCOM_InvalidCheck = 0x16,
        ErrCOM_CheckIsClosed = 0x1C,
        ErrCOM_InvalidItem = 0x21,
        ErrCOM_InsufficientAccessLevel = 0x80,
        ErrCOM_InvalidTender = 0x1B,
        ErrCOM_InvalidModCode = 0x27,
        ErrCOM_ModReqsNotMet = 0x29,

        ErrCOM_Unknown = 0xFF
    }
    public class CAlohaErrors 
    {
        public CAlohaErrors(string errStr)
        {


            SetVal(GetAlohaErrorVal(errStr));
        }

        private static Dictionary<AlohaErrEnum, string> Errors = new Dictionary<AlohaErrEnum, string>()
        {
         {AlohaErrEnum.ErrCOM_NoError, "Нет ошибок"},
        {AlohaErrEnum.ErrCOM_NoOneLoggedIn, "Не выполнен вход"},
            {AlohaErrEnum.ErrCOM_CouldNotFindEmployeeFromId, "Код официанта отсутствует в системе"},
            {AlohaErrEnum.ErrCOM_EmpAlreadyClockedIn, "Смена уже открыта"},
            {AlohaErrEnum.ErrCOM_EmpCannotWorkAnotherShift, "Сотрудник работает с другой очередью"},
            {AlohaErrEnum.ErrCOM_EmpDrawerNotConfirmed, "Это водитель"},
            {AlohaErrEnum.ErrCOM_EmpInvalidJobcode, "Неверный код должности"},
            {AlohaErrEnum.ErrCOM_EmpIsMagcardOnly, "Вход только по магнитной карте"},
            {AlohaErrEnum.ErrCOM_EmpLoggedOnOtherTerm, "Официант выполнил вход на другом терминале"},
            {AlohaErrEnum.ErrCOM_EmpToManyShiftsToday, "Слишком много очередей"},
            {AlohaErrEnum.ErrCOM_FOHCOM_ServerException, "Ошибка com-сервера"},
            {AlohaErrEnum.ErrCOM_InvalidMagCard, "Некорректная карта"},
            {AlohaErrEnum.ErrCOM_SetClearedPasswordRequired, "Неверный пароль"},
            {AlohaErrEnum.ErrCOM_SetExpiredPasswordRequired, "Неверный пароль"},
            {AlohaErrEnum.ErrCOM_SetUninitPasswordRequird, "Неверный пароль"},
            {AlohaErrEnum.ErrCOM_SomeoneAlreadyLoggedIn, "Кто то уже зарегистрировался на терминале"},
            {AlohaErrEnum.ErrCOM_UnauthorizedCOMTerm, "Терминал не определен"},
            {AlohaErrEnum.ErrCOM_UnknownEmpClockInError, "Неизвестная ошибка начала смены"},
            {AlohaErrEnum.ErrCOM_Unknown, "Неизвестная ошибка"},
            {AlohaErrEnum.ErrCOM_TableNotFound, "Неверный номер стола"},
            {AlohaErrEnum.ErrCOM_TableInUse, "Стол используется другим официантом"},
            {AlohaErrEnum.ErrCOM_TableActiveOnOtherTerm, "Стол открыт на другом терминале"},
            {AlohaErrEnum.ErrCOM_InvalidTable, "Этот стол занят другим официантом"},
            {AlohaErrEnum.ErrCOM_TableIsClosed, "Стол уже закрыт"},
            {AlohaErrEnum.ErrCOM_InvalidCheck, "Некорретный чек"},
            {AlohaErrEnum.ErrCOM_CheckIsClosed, "Чек уже закрыт"},
            {AlohaErrEnum.ErrCOM_UnavailableItem, "Блюда нет в наличии"},
            {AlohaErrEnum.ErrCOM_InvalidItem, "Блюда нет в базе"},
            {AlohaErrEnum.ErrCOM_InvalidTender, "Вид оплаты отсутствует в базе"},
            {AlohaErrEnum.ErrCOM_InvalidModCode, "Неверный код модификатора"},
            {AlohaErrEnum.ErrCOM_ModReqsNotMet, "Блюдо должно иметь обязательные модификаторы"},
            {AlohaErrEnum.ErrCOM_InsufficientAccessLevel, "У пользователя недостаточно прав для выполнения данной операции"},


        };

        public string InnerInfo = "";
        private string _Valstr = "";
        private AlohaErrEnum _Val;
        public AlohaErrEnum Val
        {
            get
            {
                return _Val;
            }
        }

        public void SetVal(string str)
        {
            _Val = GetAlohaErrorVal(str);
            _Valstr = str;

        }
        public static AlohaErrEnum GetAlohaErrorVal(String ErrorMessage)
        {
            AlohaErrEnum V = AlohaErrEnum.ErrCOM_Unknown;
            
            string s = ErrorMessage.Substring(ErrorMessage.Length - 2, 2);
            try
            {
                foreach (AlohaErrEnum i in Errors.Keys)
                {

                    if ((int)i == Convert.ToInt16(s, 16))
                    {
                        V = i;
                        return V;
                    }
                }
            }
            catch
            { }
            return V;
        }


        public void SetVal(AlohaErrEnum val)
        {
           _Val = val;
        }
        public string ValStr
        {
            get
            {

                if (_Val == AlohaErrEnum.ErrCOM_Unknown)
                {
                    return _Valstr;
                }
                else
                {
                    string str = "";
                    Errors.TryGetValue(_Val,out str);
                    return str;
                }
            }
        }


    }
}
