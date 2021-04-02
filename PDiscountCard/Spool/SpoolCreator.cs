using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace PDiscountCard.Spool
{
    static class SpoolCreator
    {

        static public void AddToSpoolFile(Check Chk)
        {
            try
            {
                Utils.ToCardLog("[AddToSpoolFile] Start kkm:" + MainClass.KKMNumber.ToString());
                string SpoolString = CreateSpoolStrings2(Chk);
                if (iniFile.SpoolOneFileEnabled)
                {
                    Utils.ToCardLog("SpoolOneFileEnabled");
                    StreamWriter SW = new StreamWriter(iniFile.SpoolPath, true, Encoding.GetEncoding(1251));
                    SW.Write(SpoolString);
                    SW.Close();
                }
                if (iniFile.SpoolMultiFilesEnabled)
                {
                    Utils.ToCardLog("SpoolMultiFilesEnabled");
                    if (!Directory.Exists(iniFile.SpoolDir2))
                    {
                        Directory.CreateDirectory(iniFile.SpoolDir2);
                    }
                    StreamWriter SW2 = new StreamWriter(iniFile.SpoolDir2 + "ll" + Chk.AlohaCheckNum + ".chk", true, Encoding.GetEncoding(1251));
                    SW2.Write(SpoolString);
                    SW2.Close();
                }
                Utils.ToCardLog("[AddToSpoolFile] End");
            }
            catch (Exception e)
            {
                Utils.ToCardLog("[Error AddToSpoolFile] " + e.Message);
            }
        }


        /*
        static public string CreateDeleteItemSpoolStrings(int ManagerId, int EmployeeId, int QueueId, int TableId, int CheckId, int ReasonId)
        {
            string Tmp;

            Tmp = "00"; //тип строки
            Tmp += GetStrWithOffcet(3, ReasonId.ToString() ); //код отмены
            Tmp += GetStrWithOffcet(3, AlohainiFile.DepNum.ToString ()); //код подразделения
            Tmp += GetStrWithOffcet(6, DateTime.Now.ToString("ddMMyy")); //ДАТА
            Tmp += GetStrWithOffcet(5, DateTime.Now.ToString("HH:mm")); //время
            Tmp += GetStrWithOffcet(12, Check.GetLongCheckNumber (CheckId) ); //номер чека в алохе
            Tmp += GetStrWithOffcet(4, EmployeeId.ToString ()); //номер чека в алохе
            Tmp += GetStrWithOffcet(6, ""); //баркод
            Tmp += GetStrWithOffcet(6, ""); //кол-во


        }
        */


        static public string CreateSpoolStrings2(Check Chk)
        {
            string Tmp;

            Tmp = "01"; //тип строки 2
            Tmp += GetStrWithOffcet(12, ""); //номер  чека (CMOS) 14
            Tmp += GetStrWithOffcet(6, (Chk.DiscountMGR_NUMBER == 0) ? "" : Chk.DiscountMGR_NUMBER.ToString()); //код менеджера - скидки 20
            Tmp += GetStrWithOffcet(6, (Chk.CompId == 0) ? "" : Chk.CompId.ToString()); //код клиента - скидки 26
            Tmp += GetStrWithOffcet(6, Chk.SystemDate.ToString("ddMMyy")); //дата чека  32
            if ((Chk.CompId >= 10) && (Chk.Summ == 0))
            {
                //Cod_Manager = Chk.DegustationMGR_NUMBER;
                Tmp += GetStrWithOffcet(4, Chk.DegustationMGR_NUMBER.ToString()); //Код официанта (старый) 36 менеджер дегустации
            }
            else
            {
                Tmp += GetStrWithOffcet(4, Chk.Waiter.ToString()); //Код официанта (старый) 36
            }
            Tmp += GetStrWithOffcet(3, iniFile.SpoolKassaNum != -1 
                ? iniFile.SpoolKassaNum.ToString() 
                : Utils.GetTermNum().ToString()); //Номер кассы  (не использ) 39

            Tmp += GetStrWithOffcet(4, Chk.Cassir.ToString()); //Код кассира 43
            Tmp += GetStrWithOffcet(2, Chk.ConSolidateDishez.Count.ToString()); //Количество  наименований в чеке (не исп) 45
            Tmp += GetStrWithOffcet(2, GetChkTypeForSpoolCaption(Chk)); //Тип чека 47 (не исп)
            Tmp += GetStrWithOffcet(5, Chk.SystemDate.ToString("HH:mm")); //Время чека 52 
            Tmp += GetStrWithOffcet(3, iniFile.SpoolDepNum.ToString()); // Код подразделения 55
            if ((Chk.CompId >= 10) && (Chk.Summ == 0))
            {
                Tmp += GetStrWithOffcet(6, Chk.DegustationMGR_NUMBER.ToString()); //Код официанта (новый) 61
            }
            else
            {
                Tmp += GetStrWithOffcet(6, Chk.Waiter.ToString()); //Код официанта (новый) 61
            }

            Tmp += GetStrWithOffcet(6, ""); //Доп.  Номер  чека (не исп)  67 
            Tmp += GetStrWithOffcet(12, Chk.TableNumber.ToString()); //Код стола 79
            Tmp += GetStrWithOffcet(12, Chk.CheckNum); //Длинный номер чека 91
            Tmp += GetStrWithOffcet(4, Chk.KkmShiftNumber.ToString()); //Номер смены ккм 95
            Tmp += GetStrWithOffcet(1, Convert.ToInt32(Chk.RealOpenTimem).ToString()); //Тип чека 96
            //Tmp += GetStrWithOffcet(6, Chk.KkmNum.ToString()); //Номер ККМ 102
            Tmp += GetStrWithOffcet(6, MainClass.KKMNumber.ToString()); //Номер ККМ 102
            Tmp += GetStrWithOffcet(11, Chk.EKLZNumInt.ToString()); //Номер ЭКЛЗ 113
            Tmp += GetStrWithOffcet(11, Chk.SystemDateOfOpen.ToString("ddMMyyHH:mm")); //Дата и время открытия 124
            Tmp += GetStrWithOffcet(1, Math.Min(9, Chk.PredcheckCount).ToString()); //Кол-во предчеков 125
            Tmp += GetStrWithOffcet(1, "0"); //Х-з че такое.. 126
            Tmp += GetStrWithOffcet(3, "0"); //Х-з че такое.. 129
            if (iniFile.SpoolPrintDiscName)
            {
                Tmp += GetStrWithOffcet(20, GetCompNameSpoolCaption(Chk)); //Имя компенсатора 149
            }
            else
            {
                Tmp += GetStrWithOffcet(20, "");
            }
            Tmp += GetStrWithOffcet(2, Environment.NewLine); //CR/NL

            decimal ChOSumm = 0;
            int IsVosvr = (Chk.Vozvr) ? (-1) : 1;
            Utils.ToCardLog("---------------------------------CreateSpoolStrings2-----------------------------------------");

            foreach (Dish d in Chk.ConSolidateSpoolDishez)
            {
                decimal Count = d.Count * d.QUANTITY * d.QtyQUANTITY;
                decimal CountRound = Math.Ceiling(d.Count * d.QUANTITY * d.QtyQUANTITY);
                decimal OSummNetto = (decimal)Math.Round(d.OPriceone * (double)Count, 2, MidpointRounding.ToEven);
                decimal SummNetto = (decimal)Math.Round(d.Priceone * (double)Count, 2, MidpointRounding.ToEven) + d.Delta + d.ServiceChargeSumm;

                Utils.ToCardLog("SummNetto:" + SummNetto.ToString() + " " + d.LongName + " Priceone: " + d.Priceone.ToString() + " Delta:" + d.Delta.ToString() + "ServiceChargeSumm: " + d.ServiceChargeSumm.ToString());

                ChOSumm += d.OPrice;
                Tmp += GetStrWithOffcet(2, "12"); //тип строки
                Tmp += GetStrWithOffcet(2, "00"); //тип операции
                Tmp += GetStrWithOffcet(16, d.BarCode.ToString()); //код  товара 
                Tmp += GetStrWithOffcet(11, (Math.Abs(d.OPriceone) * 100).ToString("0")); //цена продажи  в валюте кассы
                Tmp += GetStrWithOffcet(8, (CountRound * 100 * IsVosvr).ToString("0")); //Количество*100
                //Tmp += GetStrWithOffcet(11, ((d.Price)  * 100).ToString()); //Величина скидки с учетом знака 
                Tmp += GetStrWithOffcet(11, ""); //Величина скидки с учетом знака 

                Tmp += GetStrWithOffcet(11, (Math.Abs(SummNetto) * IsVosvr * 100).ToString("0")); //нетто сумма по строке
                Tmp += GetStrWithOffcet(3, ""); //код  валюты  цены товара
                Tmp += GetStrWithOffcet(12, (Math.Abs(d.OPriceone) * 100).ToString("0")); //Цена товара в валюте справочника
                Tmp += GetStrWithOffcet(8, (CountRound * 1000 * IsVosvr).ToString("0")); //Количество*1000
                Tmp += GetStrWithOffcet(14, (Math.Abs(OSummNetto) * 100).ToString("0")); //Цена товара в валюте кассы
                Tmp += GetStrWithOffcet(20, d.LongName); //Наименование товара / Группы
                Tmp += GetStrWithOffcet(4, "3"); //Код секции.. ХЗ че такое
                Tmp += GetStrWithOffcet(2, Environment.NewLine); //CR/NL

                //Пишем время заказа блюда

            }
            if (!iniFile.SpoolOrderTimeDisable)
            {
                foreach (Dish d in Chk.Dishez)
                {
                    Tmp += GetDishOrderTimeSpoolString(Chk, d.BarCode, SQL.ToSql.GetOrderTime(d.BarCode, Chk.AlohaCheckNum, d.AlohaNum, Chk.SystemDateOfOpen, Chk.Summ > 0, Chk.Waiter), (int)(Math.Abs(d.Priceone) * 100), d.Id);
                }
            }
            //итог чека
            Tmp += GetStrWithOffcet(2, "03"); //тип строки
            Tmp += GetStrWithOffcet(2, "00"); //тип операции
            Tmp += GetStrWithOffcet(15, ""); //Заполнитель

            Tmp += GetStrWithOffcet(12, (Chk.Summ * 100).ToString("0")); //сумма к оплате в валюте кассы без итоговой скидки(якобы)
            Tmp += GetStrWithOffcet(8, ""); //Заполнитель
            Tmp += GetStrWithOffcet(11, ""); //Заполнитель
            Tmp += GetStrWithOffcet(11, (Chk.Summ * 100).ToString("0")); //сумма к оплате в валюте кассы без итоговой скидки(якобы)
            Tmp += GetStrWithOffcet(2, Environment.NewLine); //CR/NL

            //Оплата

            foreach (AlohaTender AT in Chk.Tenders)
            {
                Tmp += GetStrWithOffcet(2, "04"); //тип строки 4
                if (!iniFile.SpoolTenderConvert)
                {
                    Tmp += GetStrWithOffcet(2, AT.TenderId.ToString("##")); ///Код платежа 4
                }
                else
                {
                    Utils.ToCardLog("SpoolTenderConvert");
                    int tndrid = AT.TenderId;
                    if (tndrid == 1)
                    {
                        tndrid = iniFile.SpoolTenderCash;
                        Utils.ToCardLog("SpoolTenderConvert SpoolTenderCash");
                    }
                    else if (tndrid == 20)
                    {
                        Utils.ToCardLog("SpoolTenderConvert SpoolTenderCard");
                        tndrid = iniFile.SpoolTenderCard;
                    }
                    Tmp += GetStrWithOffcet(2, tndrid.ToString("##")); ///Код платежа 4
                }

                Tmp += GetStrWithOffcet(15, ""); //не исп 19
                Tmp += GetStrWithOffcet(12, (AT.Summ * 100).ToString("0")); //сумма платежа 31
                Tmp += GetStrWithOffcet(9, ""); //не исп 40
                Tmp += GetStrWithOffcet(5, AT.CardPrefix); //префикс карты 45
                Tmp += GetStrWithOffcet(2, ""); //не исп 47
                Tmp += GetStrWithOffcet(9, AT.CardNumber); //номер карты 56
                Tmp += GetStrWithOffcet(2, Environment.NewLine); //CR/NL
            }

            foreach (AlohaClientCard CC in Chk.AlohaClientCardList)
            {
                Tmp += GetStrWithOffcet(2, "25"); //тип строки 2
                Tmp += GetStrWithOffcet(2, CC.TypeId); //Код платежа 4
                Tmp += GetStrWithOffcet(6, ""); //не исп 10
                Tmp += GetStrWithOffcet(5, CC.Prefix); //Префикс 15
                Tmp += GetStrWithOffcet(2, ""); //не исп 17
                Tmp += GetStrWithOffcet(9, CC.Number); //Номер 26
                Tmp += GetStrWithOffcet(10, CC.BonusAdd.ToString()); //Начисленные баллы  36
                Tmp += GetStrWithOffcet(10, CC.BonusRemove.ToString()); //Списанные баллы  46
                Tmp += GetStrWithOffcet(10, CC.Discount.ToString()); //Сумма скидки в копейках  56
                Tmp += GetStrWithOffcet(10, CC.Payment.ToString()); //Сумма оплаты в копейках  66
                Tmp += GetStrWithOffcet(10, CC.CardPrice.ToString()); //Цена продажи карты в копейках   76
                Tmp += GetStrWithOffcet(2, Environment.NewLine); //CR/NL
            }

            Tmp += GetStrWithOffcet(2, Environment.NewLine); //CR/NL
            return Tmp;
        }
        static public void AddZReportToSpool(ZReportData FiskData)
        {
            string SpoolString = CreateZReportStrings(FiskData);
            StreamWriter SW = new StreamWriter(iniFile.SpoolPath, true, Encoding.GetEncoding(1251));
            SW.Write(SpoolString);
            SW.Close();
        }
        static private string CreateZReportStrings(ZReportData FiskData)
        {
            string Tmp = "";

            foreach (KKMSpoolPayment P in FiskData.SpoolPayments)
            {
                Tmp += GetStrWithOffcet(2, "05"); //тип строки 2
                Tmp += GetStrWithOffcet(2, "11"); //тип отчета 4
                Tmp += GetStrWithOffcet(20, P.SpoolPaymentId.ToString()); //Код счетчика 24
                Tmp += GetStrWithOffcet(12, ""); //Кол-во операций 36
                Tmp += GetStrWithOffcet(16, (P.TotalSumm * 100).ToString("0")); //Сумма 52
                Tmp += GetStrWithOffcet(20, P.PaymentName); //Наименование счетчика 52
                Tmp += GetStrWithOffcet(6, FiskData.Kkmnum.ToString()); //Фискальный Номер кассы 52
                Tmp += GetStrWithOffcet(3, Utils.GetTermNum().ToString()); //Номер кассы 52
                Tmp += GetStrWithOffcet(3, iniFile.SpoolDepNum.ToString()); //Номер магазина 52
                Tmp += GetStrWithOffcet(8, FiskData.DtZRep.ToString("yyyyMMdd")); //Дата отчета
                Tmp += GetStrWithOffcet(6, iniFile.SpoolDepNum.ToString()); //Номер магазина расш 52
                Tmp += GetStrWithOffcet(6, FiskData.DtZRep.ToString("HHmmss")); //Время отчета
                Tmp += GetStrWithOffcet(2, Environment.NewLine); //CR/NL
            }


            // Z-отчет
            Tmp += GetStrWithOffcet(2, "05"); //тип строки 2
            Tmp += GetStrWithOffcet(2, "11"); //тип отчета 4
            Tmp += GetStrWithOffcet(20, "902"); //Код счетчика 24
            Tmp += GetStrWithOffcet(12, FiskData.CashIncomeCount.ToString()); //Кол-во операций 36
            Tmp += GetStrWithOffcet(16, (FiskData.CashIncome * 100).ToString("0")); //Сумма 52
            Tmp += GetStrWithOffcet(20, "Выручка"); //Наименование счетчика 52
            Tmp += GetStrWithOffcet(6, FiskData.Kkmnum.ToString()); //Фискальный Номер кассы 52
            Tmp += GetStrWithOffcet(3, Utils.GetTermNum().ToString()); //Номер кассы 52
            Tmp += GetStrWithOffcet(3, iniFile.SpoolDepNum.ToString()); //Номер магазина 52
            Tmp += GetStrWithOffcet(8, FiskData.DtZRep.ToString("yyyyMMdd")); //Дата отчета
            Tmp += GetStrWithOffcet(6, iniFile.SpoolDepNum.ToString()); //Номер магазина расш 52
            Tmp += GetStrWithOffcet(6, FiskData.DtZRep.ToString("HHmmss")); //Время отчета
            Tmp += GetStrWithOffcet(2, Environment.NewLine); //CR/NL

            // Новый итог
            Tmp += GetStrWithOffcet(2, "05"); //тип строки 2
            Tmp += GetStrWithOffcet(2, "11"); //тип отчета 4
            Tmp += GetStrWithOffcet(20, "904"); //Код счетчика 24
            Tmp += GetStrWithOffcet(12, FiskData.NewShiftNumber.ToString()); //Номерновой смены 36
            Tmp += GetStrWithOffcet(16, (FiskData.CashIncome * 100).ToString("0")); //Сумма 52
            Tmp += GetStrWithOffcet(20, "Новый итог"); //Наименование счетчика 52
            Tmp += GetStrWithOffcet(6, FiskData.Kkmnum.ToString()); //Фискальный Номер кассы 52
            Tmp += GetStrWithOffcet(3, Utils.GetTermNum().ToString()); //Номер кассы 52
            Tmp += GetStrWithOffcet(3, iniFile.SpoolDepNum.ToString()); //Номер магазина 52
            Tmp += GetStrWithOffcet(8, FiskData.DtZRep.ToString("yyyyMMdd")); //Дата отчета
            Tmp += GetStrWithOffcet(6, iniFile.SpoolDepNum.ToString()); //Номер магазина расш 52
            Tmp += GetStrWithOffcet(6, FiskData.DtZRep.ToString("HHmmss")); //Время отчета
            Tmp += GetStrWithOffcet(2, Environment.NewLine); //CR/NL

            Tmp += GetStrWithOffcet(2, Environment.NewLine); //CR/NL

            return Tmp;
        }
        /*
        static public string CreateSpoolStrings(Check Chk)
        { 
            string Tmp ;

            Tmp = "01"; //тип строки
            Tmp += GetStrWithOffcet(12, ""); //номер  чека (CMOS)
            Tmp += GetStrWithOffcet(6, (Chk.DiscountMGR_NUMBER == 0) ? "" : Chk.DiscountMGR_NUMBER.ToString()); //код менеджера - скидки
            Tmp += GetStrWithOffcet(6, (Chk.CompId==0) ? "" :Chk.CompId.ToString()); //код клиента - скидки
            Tmp += GetStrWithOffcet(6, Chk.SystemDate.ToString("ddMMyy")); //дата чека 

            if ((Chk.CompId >= 10) && (Chk.Summ == 0))
            {
                //Cod_Manager = Chk.DegustationMGR_NUMBER;
                Tmp += GetStrWithOffcet(4, Chk.DegustationMGR_NUMBER.ToString ()); //Код продавца
            }
            else
            {
                Tmp += GetStrWithOffcet(4, Chk.Waiter.ToString()); //Код продавца
            }

            //Tmp += GetStrWithOffcet(3, Utils.GetTermNum().ToString ()   ); //Номер кассы 
            Tmp += GetStrWithOffcet(3, Utils.GetTermNum ().ToString ()); //Номер кассы 
            Tmp += GetStrWithOffcet(4, Chk.Cassir.ToString()); //Код кассира
            Tmp += GetStrWithOffcet(2, Chk.ConSolidateDishez.Count.ToString ()); //Количество  наименований в чеке
            Tmp += GetStrWithOffcet(2, GetChkTypeForSpoolCaption(Chk)); //Тип чека
            Tmp += GetStrWithOffcet(5, Chk.SystemDate.ToString ("HH:mm")  ); //Время
           // Tmp += GetStrWithOffcet(3, AlohainiFile.DepNum.ToString ()); //Номер магазина
            Tmp += GetStrWithOffcet(3, iniFile.SpoolDepNum.ToString ()); 
          //  Tmp += GetStrWithOffcet(3, AlohainiFile.DepNum.ToString()); //Номер магазина
            if ((Chk.CompId >= 10) && (Chk.Summ == 0))
            {
                //Cod_Manager = Chk.DegustationMGR_NUMBER;
                Tmp += GetStrWithOffcet(6, Chk.DegustationMGR_NUMBER.ToString()); //Код продавца
            }
            else
            {
                Tmp += GetStrWithOffcet(6, Chk.Waiter.ToString()); //Код продавца
            }
            Tmp += GetStrWithOffcet(6, ""); //Доп.  Номер  чека 
            Tmp += GetStrWithOffcet(12, Chk.TableNumber.ToString ()); //Код стола
            Tmp += GetStrWithOffcet(12, Chk.CheckNum); //Номер ресторанного чека
            Tmp += GetStrWithOffcet(4, ""); //Значение Z-счетчика
            Tmp += GetStrWithOffcet(8, Chk.SystemDate.ToString("ddMMyyyy")); //Новое представление даты чека
            Tmp += GetStrWithOffcet(4, Chk.Guests.ToString ()); //Количество гостей
            Tmp += GetStrWithOffcet(17, Chk.SystemDateOfOpen.ToString ("ddMMyyHH:mm") ); //Время открытия
            Tmp += GetStrWithOffcet(1, Math.Min (9,Chk.PredcheckCount).ToString ()  ); //Кол-во предчеков
            Tmp += GetStrWithOffcet(1, "0"); //Х-з че такое..
            Tmp += GetStrWithOffcet(3, ""); //Х-з че такое..
            Tmp += GetStrWithOffcet(8, GetCompNameSpoolCaption (Chk)); //Имя компенсатора
        
            Tmp += GetStrWithOffcet(2, Environment.NewLine); //CR/NL
        
            decimal ChOSumm = 0;
            int IsVosvr = (Chk.Vozvr) ? (-1) : 1;
            foreach (Dish d in Chk.ConSolidateSpoolDishez)
            {
                
                decimal  Count = d.Count * d.QUANTITY * d.QtyQUANTITY    ;
                decimal  OSummNetto = (decimal)Math.Round(d.OPriceone * (double )Count,2,MidpointRounding.ToEven ) ;
                decimal SummNetto = (decimal)Math.Round(d.Priceone * (double)Count, 2, MidpointRounding.ToEven) +  d.Delta;
                
                ChOSumm += d.OPrice;
                Tmp += GetStrWithOffcet(2, "12"); //тип строки
                Tmp += GetStrWithOffcet(2, "00"); //тип операции
                Tmp += GetStrWithOffcet(16, d.BarCode.ToString()); //код  товара 
                Tmp += GetStrWithOffcet(11, (Math.Abs(d.OPriceone)*100).ToString ("0")); //цена продажи  в валюте кассы
                Tmp += GetStrWithOffcet(8, (Count * 100 * IsVosvr).ToString("0")); //Количество*100
                //Tmp += GetStrWithOffcet(11, ((d.Price)  * 100).ToString()); //Величина скидки с учетом знака 
                Tmp += GetStrWithOffcet(11, ""); //Величина скидки с учетом знака 

                Tmp += GetStrWithOffcet(11, (Math.Abs(SummNetto)*IsVosvr * 100).ToString("0")); //нетто сумма по строке
                Tmp += GetStrWithOffcet(3, ""); //код  валюты  цены товара
                Tmp += GetStrWithOffcet(12, (Math.Abs(d.OPriceone) * 100).ToString("0")); //Цена товара в валюте справочника
                Tmp += GetStrWithOffcet(8, (Count*1000*IsVosvr ).ToString("0")); //Количество*1000
                Tmp += GetStrWithOffcet(14, (Math.Abs( OSummNetto) * 100).ToString("0")); //Цена товара в валюте кассы
                Tmp += GetStrWithOffcet(20, d.LongName); //Наименование товара / Группы
                Tmp += GetStrWithOffcet(4, "3"); //Код секции.. ХЗ че такое
                Tmp += GetStrWithOffcet(2, Environment.NewLine); //CR/NL
                    
                //Пишем время заказа блюда
                
            }
            foreach (Dish d in Chk.Dishez)
            {
                Tmp += GetDishOrderTimeSpoolString(Chk, d.BarCode, SQL.ToSql.GetOrderTime(d.BarCode, Chk.AlohaCheckNum, d.AlohaNum, Chk.SystemDateOfOpen, Chk.Summ > 0, Chk.Waiter), (int)(Math.Abs(d.Priceone) * 100));
            }

            //итог чека
            Tmp += GetStrWithOffcet(2, "03"); //тип строки
            Tmp += GetStrWithOffcet(2, "00"); //тип операции
            Tmp += GetStrWithOffcet(15, ""); //Заполнитель
            
            Tmp += GetStrWithOffcet(12, (Chk.Summ*100).ToString("0")); //сумма к оплате в валюте кассы без итоговой скидки(якобы)
            Tmp += GetStrWithOffcet(8, ""); //Заполнитель
            Tmp += GetStrWithOffcet(11, ""); //Заполнитель
            Tmp += GetStrWithOffcet(11, (Chk.Summ*100).ToString("0")); //сумма к оплате в валюте кассы без итоговой скидки(якобы)
            Tmp += GetStrWithOffcet(2, Environment.NewLine); //CR/NL

            //Оплата
            Tmp += GetStrWithOffcet(2, "04"); //тип строки
            foreach (AlohaTender AT in Chk.Tenders)
            {
                Tmp += GetStrWithOffcet(2, AT.TenderId.ToString("##"));
               
                Tmp += GetStrWithOffcet(15, ""); //код вал + заплонитель
                Tmp += GetStrWithOffcet(12, (Chk.Oplata * 100).ToString("0")); //код вал + заплонитель
                Tmp += GetStrWithOffcet(8, ""); //код вал + заплонитель
                Tmp += GetStrWithOffcet(1, ""); //код вал + заплонитель
                Tmp += GetStrWithOffcet(10, ""); //код вал + заплонитель
                Tmp += GetStrWithOffcet(2, ""); //код вал + заплонитель
                Tmp += GetStrWithOffcet(20, ""); //код вал + заплонитель
                Tmp += GetStrWithOffcet(10, ""); //код вал + заплонитель
                Tmp += GetStrWithOffcet(1, ""); //код вал + заплонитель
                Tmp += GetStrWithOffcet(1, ""); //код вал + заплонитель
                Tmp += GetStrWithOffcet(14, (Chk.Oplata * 100).ToString("0")); //код вал + заплонитель
                Tmp += GetStrWithOffcet(2, Environment.NewLine); //CR/NL
            }
            //Сдача
            if ((Chk.Summ - Chk.Oplata)!=0)
            {
                Tmp += GetStrWithOffcet(2, "04"); //тип строки

                Tmp += GetStrWithOffcet(2, "01"); //тип операции


                Tmp += GetStrWithOffcet(15, ""); //код вал + заплонитель
                Tmp += GetStrWithOffcet(12, ((Chk.Summ - Chk.Oplata) * 100).ToString("0")); //код вал + заплонитель
                Tmp += GetStrWithOffcet(8, ""); //код вал + заплонитель
                Tmp += GetStrWithOffcet(1, ""); //код вал + заплонитель
                Tmp += GetStrWithOffcet(10, ""); //код вал + заплонитель
                Tmp += GetStrWithOffcet(2, ""); //код вал + заплонитель
                Tmp += GetStrWithOffcet(20, ""); //код вал + заплонитель
                Tmp += GetStrWithOffcet(10, ""); //код вал + заплонитель
                Tmp += GetStrWithOffcet(1, ""); //код вал + заплонитель
                Tmp += GetStrWithOffcet(1, ""); //код вал + заплонитель
                Tmp += GetStrWithOffcet(14, ((Chk.Summ - Chk.Oplata) * 100).ToString("0")); //код вал + заплонитель
                Tmp += GetStrWithOffcet(2, Environment.NewLine); //CR/NL

            }
            Tmp += GetStrWithOffcet(2, Environment.NewLine); //CR/NL
            return Tmp;
        }
        */
        internal static string GetCompNameSpoolCaption(Check Chk)
        {
            List<int> DiskcountList = new List<int>() { 11, 12, 14, 17 };
            if (Chk.Vozvr)
            {
                return "";
            }
            //if (DiskcountList.Contains (Chk.CompId) )
            {
                return Chk.CompDescription;
            }
            //            return "";

        }
        private static string GetChkTypeForSpoolCaption(Check Chk)
        {
            if (Chk.Vozvr)
            {
                return "01";
            }
            else
            {
                return "00";
            }
        }

        private static string GetStrWithOffcet(int Offcet, string val)
        {
            if (val == null) { val = ""; }

            string tmp = "";
            try
            {
                if (val.Length > Offcet)
                {
                    Utils.ToCardLog("[Error] Spool Обрезал поле |" + val + "| до " + Offcet.ToString());
                    return val.Substring(0, Offcet);
                }

                for (int i = 0; i < Offcet - val.Length; i++)
                {
                    tmp += " ";
                }
                tmp += val;

            }
            catch (Exception e)
            {
                Utils.ToCardLog("[Error] Spool GetStrWithOffcet val: " + val + " message: " + e.Message);
            }
            return tmp;
        }


        private static string GetDishOrderTimeSpoolString(Check Chk, int BarCode, DateTime OrderTime, int DishMoneySumm, Guid DId)
        {
            string RetStr = "";
            RetStr = "44"; //тип строки
            RetStr += OrderTime.ToString("ddMMyyyy");
            RetStr += OrderTime.ToString("HH:mm");
            RetStr += GetStrWithOffcet(4, iniFile.SpoolDepNum.ToString());
            RetStr += GetStrWithOffcet(12, Chk.CheckNum);
            RetStr += GetStrWithOffcet(16, BarCode.ToString());
            RetStr += GetStrWithOffcet(12, DishMoneySumm.ToString());
            RetStr += GetStrWithOffcet(40, DId.ToString());
            RetStr += GetStrWithOffcet(2, Environment.NewLine); //CR/NL
            return RetStr;
        }

    }
}
