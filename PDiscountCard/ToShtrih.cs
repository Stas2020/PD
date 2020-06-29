using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;
using System.Linq;


namespace PDiscountCard
{
    public static class ToShtrih
    {

        static string KKMNUmber;
        static string INNNUmber;
        static int LastXReportNumber;

        static string TotalSum;
        static int OpenDocumentNumber;
        static string CurentDate;
        static string CurentTime;

        static double TotalSumStart;
        static double TotalSumEnd;
        static int TotalChecks;
        static int TotalVozvratChecks;

        /*
        static  public  bool  Init()
        {
            T1 = new System.Timers.Timer();
            T1.Stop(); 
            T1.Elapsed += new System.Timers.ElapsedEventHandler(T1_Elapsed);
            T1.Interval = 200;
            return  Shtrih.CreateShtrih();
        }

        static void T1_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            T1.Stop ();
        


        }
        */

        /*
        static public  bool GetPrinterStatus()
        { 
            Shtrih.GetShortECRStatus();
            switch (Shtrih.ECRMode )
            {
                case 0: //Принтер в рабочем режиме
                    return true ;
                    break;
               case 1: //Выдача данных
                    return false;
                        break;
                case 2: //Открытая смена, 24 часа не кончились
                        return true; 
                        break;
                case 3: //Открытая смена, 24 часа кончились
                    return false;
                        break;
                case 4: //Закрытая смена
                case 5: //Блокировка по неправильному паролю налогового инспектора                    
                    break;
                case 6: //Ожидание подтверждения ввода даты
                case 7: //Разрешение изменения положения десятичной точки
                case 8: //Открытый документ
                    Shtrih.CancelCheck();
                    GetPrinterStatus();
                    break;
                case 9: //Режим разрешения технологического обнуления
                case 10: //Тестовый прогон
                case 11: //Печать полного фискального отчета
                case 12: //Печать длинного отчета ЭКЛЗ
                case 13: //Работа с фискальным подкладным документом
                case 14: //Печать подкладного документа
                case 15: //Фискальный подкладной документ сформирован
                default:
                    break;
            }
            return false;
        }
        */
        /*
        static internal void WriteBalanceToShtrih(Balanse B)
        {
            Shtrih.Password = 30;

            Shtrih.TableNumber = 4;
            Shtrih.GetTableStruct();
            Shtrih.RowNumber = 1;
            Shtrih.FieldNumber = 1;
            Shtrih.ValueOfFieldString = B.Nal.ToString() + ";" + B.Card.ToString() + ";" +
                B.VozvrNal.ToString() + ";" + B.VozvrCard.ToString() + ";" +
                B.CountNal.ToString() + ";" + B.CountCard.ToString() + ";" +
                B.CountVozvrNal.ToString() + ";" + B.CountVozvrCard.ToString();
            Shtrih.WriteTable();
        }
        */
        /*
        internal static void WriteTableInfo(List<TableRowInfo> Ti)
        {
            
            Shtrih.TableNumber = 1;
            Shtrih.RowNumber = 1;

            Shtrih.GetTableStruct();
            int MaxRows = Shtrih.FieldNumber;
            for (int i = 1; i < MaxRows + 1; i++)
            {
                Shtrih.FieldNumber = i;
                Shtrih.GetFieldStruct();


                if (Ti[i - 1].IsCange)
                {


                    if (Shtrih.FieldType)
                    {
                        Shtrih.ValueOfFieldString = Ti[i - 1].ValueOfFieldString;
                    }
                    else
                    {
                        Shtrih.ValueOfFieldInteger = Ti[i - 1].ValueOfFieldInteger;

                    }
                    Shtrih.WriteTable(); 
                }
                
            }   




        }

        internal static void WriteCaption(List<string> Caption)
        {
            
            Shtrih.TableNumber = 4;
            Shtrih.FieldNumber = 1;

            for (int i = 12; i < 17; i++)
            {
                Shtrih.RowNumber = i;
                Shtrih.ValueOfFieldString = Caption[i-12];
                Shtrih.GetFieldStruct();
                Shtrih.WriteTable();

                
            }

            
        }
        */



        /*
        internal static List<TableRowInfo> GetTableInfo()
        {
            List<TableRowInfo> Tmp = new List<TableRowInfo>();
            Shtrih.TableNumber = 1;
            Shtrih.RowNumber = 1;

            Shtrih.GetTableStruct();
            if (Shtrih.ResultCode != 0)
            {
                return Tmp;
            }

            int MaxRows = Shtrih.FieldNumber ;
            for (int i = 1; i < MaxRows + 1; i++)
            {
                Shtrih.FieldNumber = i;
                Shtrih.GetFieldStruct();
                


                TableRowInfo TrI = new TableRowInfo
                { Num = i,
                    FieldSize = Shtrih.FieldSize,
                    MAXValueOfField = Shtrih.MAXValueOfField,
                    MINValueOfField = Shtrih.MINValueOfField,
                    Name = Shtrih.FieldName,
                    FieldType = Shtrih.FieldType,
                    
                };
                Shtrih.ReadTable();
                if (TrI.FieldType)
                {
                    TrI.ValueOfFieldString = Shtrih.ValueOfFieldString;
                }
                else
                {
                    TrI.ValueOfFieldInteger = Shtrih.ValueOfFieldInteger;
                }

                Tmp.Add(TrI);  
            }

            
 
            return Tmp;
        }
        */
        /*
        internal static void Disconnect()
        {
            try
            {
                Shtrih.Disconnect(); 
            }
            catch(Exception e)
            {
                
                
            }
        }
        */
        /*
        internal static bool GoodState()
        {
            Conn();
            Shtrih.GetShortECRStatus();
            bool b = ((Shtrih.ECRMode == 0) || (Shtrih.ECRMode == 2) || (Shtrih.ECRMode == 4));
            Console.WriteLine("Status Printera: " + Shtrih.ECRMode.ToString());
            Shtrih.Disconnect();
            return b;

        }
         * */
        internal static bool PrinterColorIsGray()
        {
            /*
            //Shtrih.Connect ();
            Shtrih.Password = 30;
            Shtrih.TableNumber = 1;
            Shtrih.RowNumber = 1;
            Shtrih.FieldNumber = 49;
            Shtrih.ReadTable();
            bool  i =(Shtrih.ValueOfFieldInteger == 1) ;
            //Shtrih.Disconnect();
            return i;
            */

            return (Shtrih2.ReadTableInt(1, 1, 49) == 1);
        }

        private static void ChangePos(bool White, ShtrihCommandBlock CB)
        {
            List<int> IgnoreList = new List<int>() { -9, 126, 51, 122 };
            if (White)
            {

                CB.WriteTableInt(1, 1, 49, 0, IgnoreList);

            }
            else
            {
                CB.WriteTableInt(1, 1, 49, 1, IgnoreList);

            }

        }
        /*
        internal static DateTime LastCheckTime()
        {
            try
            {
                //Conn();
                Shtrih.GetEKLZCode1Report();
                DateTime DTDate = Shtrih.LastKPKDate;
                DateTime DTTime = Shtrih.LastKPKTime;
                return new DateTime(DTDate.Year, DTDate.Month, DTDate.Day, DTTime.Hour, DTTime.Minute, DTTime.Second);
            }
            catch
            {
                return DateTime.Now;
            }
        }
         * */
        private static int GetTermNum()
        {
            return Utils.GetTermNum();


        }
        /*
        internal  static void SetTime(DateTime dt)
        {
            if (dt < LastCheckTime())
            {
                dt = LastCheckTime().AddSeconds(1);
            }
            Shtrih.Date = dt;
            Shtrih.Time = dt;
            Shtrih.SetDate();
            Shtrih.ConfirmDate();
            Shtrih.SetTime();
        }
        */
        internal static void SetCurentTime(ShtrihCommandBlock CB)
        {

            //DateTime dt = DateTime.Now;
            CB.SetDate(DateTime.Now);
            /*
            Shtrih.Date = dt;
            Shtrih.Time = dt;
            Shtrih.SetDate();
            Shtrih.ConfirmDate();
            Shtrih.SetTime();
             * */
        }
        internal static void ChangePrint(bool IsPrint, ShtrihCommandBlock CB)
        {
            List<int> IgnoreList = new List<int>() { -9, 126, 51, 122 };
            if (IsPrint)
            {

                CB.WriteTableInt(1, 1, 50, 0, IgnoreList);

            }
            else
            {

                CB.WriteTableInt(1, 1, 50, 1, IgnoreList);

            }

        }
        internal static void ChangeCut(bool IsCut, ShtrihCommandBlock CB)
        {
            if (iniFile.NoCut)
            {
                IsCut = false;
            }
            List<int> IgnoreList = new List<int>() { -9, 126, 51, 122 };
            if (IsCut)
            {

                CB.WriteTableInt(1, 1, 8, 1, IgnoreList);

            }
            else
            {

                CB.WriteTableInt(1, 1, 8, 0, IgnoreList);

            }

        }
        /*
        internal static string  GetClishe()
        {
            string tmp = "";
            Shtrih.Password = 30;
            Shtrih.TableNumber = 4;
            
            Shtrih.FieldNumber = 1;
            for (int i = 11; i < 17; i++)
            {
                Shtrih.RowNumber = i;
                Shtrih.GetFieldStruct();
                Shtrih.ReadTable();
                string k="";
                if (Shtrih.ValueOfFieldString != null)
                {
                    k = Shtrih.ValueOfFieldString.Replace(" ", "");
                }
                if (k.Length > 0)
                {
                    tmp += Shtrih.ValueOfFieldString+Environment.NewLine  ;
                }
            }
            return tmp;
        }
        */

        /*
        public  static void CloseCheck(double Pr, DateTime dt)
        {
            SetTime(dt);
            CloseCheck((decimal )Pr);

        }
        */
        /*
        private static void mSale()
        {
            /*
            MessageForm Mf = new MessageForm(Shtrih.ResultCodeDescription);
            do
            {
                Shtrih.Sale();

                if (Shtrih.ResultCode == 0)
                {
                    Shtrih.WaitForPrinting();
                }
                else if (Shtrih.ResultCode == 88)
                {
                    Shtrih.ContinuePrint(); 
                }
                else
                {
                    Mf.SetCpt(Shtrih.ResultCodeDescription);
                    Mf.ShowDialog();
                }
            } while ((Mf.Result == 1)&&(Shtrih.ResultCode != 0));
            Mf.Close();
            Mf.Dispose ();
             * */
        //        Shtrih.Sale();
        //  }

        //private static void mReturnSale()
        //{
        /*
        MessageForm Mf = new MessageForm(Shtrih.ResultCodeDescription);
        do
        {
            Shtrih.ReturnSale ();

            if (Shtrih.ResultCode == 0)
            {
                Shtrih.WaitForPrinting();
            }
            else if (Shtrih.ResultCode == 88)
            {
                Shtrih.ContinuePrint();
            }
            else
            {
                Mf.SetCpt(Shtrih.ResultCodeDescription);
                Mf.ShowDialog();
            }
        } while ((Mf.Result == 1) && (Shtrih.ResultCode != 0));
        Mf.Close();
        Mf.Dispose();
         * */
        //  Shtrih.ReturnSale ();
        // }

        //private static void mCloseCheck()
        //{
        /*
        MessageForm Mf = new MessageForm(Shtrih.ResultCodeDescription);

        do{
        Shtrih.CloseCheck ();

        if (Shtrih.ResultCode == 0)
        {
            Shtrih.WaitForPrinting();
        }
        else if (Shtrih.ResultCode == 88)
        {
            Shtrih.ContinuePrint();
        }
        else
        {
            Mf.SetCpt(Shtrih.ResultCodeDescription);
            Mf.ShowDialog();
        }
        } while ((Mf.Result == 1)&&(Shtrih.ResultCode != 0));
        Mf.Close();
        Mf.Dispose ();
         * */
        //       Shtrih.CloseCheck();
        //  }

        //    private static int mPrintString()
        //   {
        /*
        MessageForm Mf = new MessageForm(Shtrih.ResultCodeDescription);
            
        int res = 0;
        do
        {
            Utils.ToLog(Shtrih.StringForPrinting);
            Shtrih.PrintString ();

            if (Shtrih.ResultCode == 0)
            {
                Shtrih.WaitForPrinting();
            }
            else if (Shtrih.ResultCode == 88)
            {
                Shtrih.ContinuePrint();
            }
            else
            {
                Mf.TopMost = true;
               // Mf.button3.Visible = true; 
                Mf.SetCpt(Shtrih.ResultCodeDescription);
                Mf.ShowDialog();
                res = Mf.Result;
            }
        } while ((Mf.Result != -1) && (Shtrih.ResultCode != 0));
        Mf.Close();
            
        Mf.Dispose();
        if (Shtrih.ResultCode == 0)
        {
            res = 0;
        }
        return res;
         * */
        //        Shtrih.PrintString();
        //       return 0;
        //  }

        //        private static void mPrintStringWithFont()
        //      {
        /*
        MessageForm Mf = new MessageForm(Shtrih.ResultCodeDescription);

        do
        {
            Shtrih.PrintStringWithFont ();

            if (Shtrih.ResultCode == 0)
            {
                Shtrih.WaitForPrinting();
            }
            else if (Shtrih.ResultCode == 88)
            {
                Shtrih.ContinuePrint();
            }
            else
            {
                Mf.SetCpt(Shtrih.ResultCodeDescription);
                Mf.ShowDialog();
            }
        } while ((Mf.Result == 1) && (Shtrih.ResultCode != 0));
        Mf.Close();
        Mf.Dispose();
         * */
        //       Shtrih.PrintStringWithFont();
        //  }



        public static void ZReportWithCashIncome(decimal IncomeSumm)
        {


            ShtrihCommandBlock CashIncomeCommandBlock = new ShtrihCommandBlock();

            string status = "";
            if (Shtrih2.ClosedSmena(out status))
            {
                CashIncomeCommandBlock.Sale(1, 0, "", 1, 0);
                CashIncomeCommandBlock.CloseCheck(0, 0, 0, 0, 1, 0, 0, 0, "", null);
            }

            CashIncomeCommandBlock.WriteTableInt(1, 1, 2, 1, new List<int>());
            CashIncomeCommandBlock.PrintReportWithCleaning();
            if (IncomeSumm > 0)
            {
                CashIncomeCommandBlock.CashIncome(IncomeSumm);
            }
            CashIncomeCommandBlock.CommandBlockToQwery();
        }



        public static void CashIncome(decimal Summ)
        {
            ShtrihCommandBlock CashIncomeCommandBlock = new ShtrihCommandBlock();
            if (Summ > 0)
            {
                CashIncomeCommandBlock.CashIncome(Summ);
            }
            CashIncomeCommandBlock.CommandBlockToQwery();
        }

        public static void CashOutCome(decimal Summ)
        {
            ShtrihCommandBlock CashIncomeCommandBlock = new ShtrihCommandBlock();
            if (Summ > 0)
            {
                CashIncomeCommandBlock.CashOutCome(Summ);
            }
            CashIncomeCommandBlock.CommandBlockToQwery();
        }





        public static void CloseCheck2(Check Ch)
        {
            Utils.ToCardLog("CloseCheck2  Start"+Ch.CheckShortNum);

            ShtrihCommandBlock CloseCheckCommandBlock = new ShtrihCommandBlock();
            CloseCheckCommandBlock.ChkOwner = Ch;

            string cassirName = AlohaTSClass.GetCurentWaterName();
            if (cassirName == "") { cassirName = "Администратор"; }

            CloseCheckCommandBlock.WriteTableStr(2, 30, 2, AlohaTSClass.GetCurentWaterName(), new List<int>());//Пишем имя кассира

            if (!iniFile.FRModeDisabled)
            {
                SetCurentTime(CloseCheckCommandBlock);
                ChangePos(Ch.OpenTimem == 1, CloseCheckCommandBlock);
                ChangePrint(true, CloseCheckCommandBlock);
                ChangeCut(true, CloseCheckCommandBlock);
            }

            int FChType = 0;
            if (Ch.Vozvr) FChType = 2;
            CloseCheckCommandBlock.OpenCheck(FChType);
            decimal Summ = 0;
            foreach (string s in Ch.FrStringsBefore)
            {
                if (s.Length > 36)
                {
                    CloseCheckCommandBlock.PrintString(s.Substring(0, 35));
                }
                else
                {
                    CloseCheckCommandBlock.PrintString(s);
                }
            }
            /*
            if (AlohaTSClass.GetVip(Ch.AlohaCheckNum))
            {
                CloseCheckCommandBlock.PrintString("ЧЕК " + Ch.CheckShortNum + "   (" + AlohainiFile.DepNum.ToString() + ")" + "   Стол" + Ch.TableNumber);
            }
            else
            {
                CloseCheckCommandBlock.PrintString("ЧЕК " + Ch.CheckShortNum + "   (" + AlohainiFile.DepNum.ToString() + ")" + "   Стол " + Ch.TableNumber);
            }
            */
            List<Dish> Tmp = new List<Dish>();

            if (iniFile.DishConsolidate)
            {
                Tmp = Ch.ConSolidateDishez;
            }
            else
            {
                Tmp = Ch.Dishez;
            }
            foreach (Dish D in Tmp)
            {
                Summ += D.OPrice * D.Count;
                if (!Ch.Vozvr)
                {
                    //  CloseCheckCommandBlock.Sale((double)(D.QUANTITY * D.Count), Math.Abs((decimal)D.OPrice), D.Name + " " + D.CardPrefix + D.CardNumber, 1, 0);
                    //

                    int Tax = 1;
                    if (iniFile.FRNoTax) { Tax = 0; }
                   decimal Price =Math.Abs((decimal)D.OPrice) + Math.Abs((decimal)D.ServiceChargeSumm) / ((decimal)(D.QUANTITY * D.Count));
                   if (iniFile.FRPriceFromDisplay) { Price = Math.Abs((decimal)D.DISP_PRICE) + Math.Abs((decimal)D.ServiceChargeSumm) / ((decimal)(D.QUANTITY * D.Count));  }
                   if (iniFile.FRDiscountMode) { Price = Math.Abs((decimal)D.Price) + Math.Abs((decimal)D.ServiceChargeSumm) / ((decimal)(D.QUANTITY * D.Count)); }
                    CloseCheckCommandBlock.Sale((double)(D.QUANTITY * D.Count), Price, D.CHITNAME + " " + D.CardPrefix + D.CardNumber, Tax, 0);
                }
                else
                {
                    if (D.OPrice > 0)
                    {
                        CloseCheckCommandBlock.Discount(Math.Abs(D.OPrice), "Комплексная");
                    }
                    else
                    {
                        int Tax = 1;
                        if (iniFile.FRNoTax) { Tax = 0; }
                        decimal Price = Math.Abs((decimal)D.OPrice) + Math.Abs((decimal)D.ServiceChargeSumm) / ((decimal)(D.QUANTITY * D.Count));
                        if (iniFile.FRPriceFromDisplay) { Price = Math.Abs((decimal)D.DISP_PRICE) + Math.Abs((decimal)D.ServiceChargeSumm) / ((decimal)(D.QUANTITY * D.Count)); ; }
                        if (iniFile.FRDiscountMode) { Price = Math.Abs((decimal)D.Price) + Math.Abs((decimal)D.ServiceChargeSumm) / ((decimal)(D.QUANTITY * D.Count)); }
                        //CloseCheckCommandBlock.ReturnSale((double)(D.QUANTITY * D.Count), Math.Abs((decimal)D.OPrice), D.Name, 1, 0);
                        
                            CloseCheckCommandBlock.ReturnSale((double)(D.QUANTITY * D.Count), Price, D.CHITNAME, Tax, 0);
                        
                    }
                }
            }

            if (Math.Abs(Ch.Comp) > 0)
            {
                string DiscCaption = "Комплексная";
                if (Ch.CompId == AlohaTSClass.BonusCompId){DiscCaption = "Оплата баллами";}

                if (iniFile.FRDiscountMode)
                {
                    CloseCheckCommandBlock.PrintString(DiscCaption + " скидка              =" + Math.Abs(Ch.Comp).ToString("0.00"));
                }
                else
                {
                    CloseCheckCommandBlock.Discount(Math.Abs(Ch.Comp), DiscCaption);
                }
            }

            if (Ch.Comps.Count>1)
            { 
                foreach(AlohaComp Cmp in Ch.Comps)
                {
                    if (Cmp != Ch.Comps.First())
                    {
                        if (iniFile.FRDiscountMode)
                        {
                            CloseCheckCommandBlock.PrintString(Cmp.Name + "               =-" + Math.Abs(Cmp.Amount).ToString("0.00"));
                        }
                    }
                }
            }

            foreach (string s in Ch.FrStringsAfter)
            {
                if (s.Length > 36)
                {
                    CloseCheckCommandBlock.PrintString(s.Substring(0, 35));
                }
                else
                {
                    CloseCheckCommandBlock.PrintString(s);
                }
            }
            /*
            if (Ch.LoyaltyCard != "")
            {

                CloseCheckCommandBlock.PrintString("    ");
                CloseCheckCommandBlock.PrintString("   Программа лояльности КОФЕМАНИЯ АЭРО");
                CloseCheckCommandBlock.PrintString("   Начисление баллов");
                CloseCheckCommandBlock.PrintString("   Карта " + Ch.LoyaltyCard);
                CloseCheckCommandBlock.PrintString("   Начислено " + Ch.LoyaltyBonus.ToString("0.00") + " баллов");
            }



            foreach (AlohaTender AT in Ch.CreditPayments())
            {

                CloseCheckCommandBlock.PrintString("    ");
                CloseCheckCommandBlock.PrintString("   Списание средств");
                CloseCheckCommandBlock.PrintString("   Карта " + AT.Ident);
                CloseCheckCommandBlock.PrintString("   Списано " + AT.Summ.ToString("0.00"));


                DateTime dt = DateTime.Now;
                string Err = "";
                decimal Bal = Loyalty.LoyaltyBasik.GetASVCardBalance(AT.Ident, out dt, out Err);
                if (Bal != -1)
                {
                    CloseCheckCommandBlock.PrintString("   Текущий баланс " + Bal.ToString("0.00"));
                    CloseCheckCommandBlock.PrintString("   Срок действия карты " + dt.ToString("dd.MM.yyyy"));
                }
                CloseCheckCommandBlock.PrintString("    ");
                //CloseCheckCommandBlock.PrintString("Доступных баллов: " + Ch.ASVCardBalance.ToString("0.00"));
            }
            */
            /*
            if (Ch.IsNal)
            {
                CloseCheckCommandBlock.CloseCheck(Math.Abs(Ch.Oplata), 0, 0, 0, 0, 1, 0, 0, 0, "", 0);

            }
            else if (Ch.Tender == TenderType.GloryCash)
            {
                CloseCheckCommandBlock.CloseCheck(0, Math.Abs(Ch.Summ), 0, 0, 0, 1, 0, 0, 0, "", 0);

            }
            else if (Ch.Tender == TenderType.Credit)
            {
                CloseCheckCommandBlock.CloseCheck(0, Math.Abs(Ch.Summ), 0, 0, 0, 1, 0, 0, 0, "", 0);
            }
            else
            {
                CloseCheckCommandBlock.CloseCheck(0, 0, 0, Math.Abs(Ch.Oplata), 0, 1, 0, 0, 0, "", 0);
            }
            */
            CloseCheckCommandBlock.CloseCheck(Math.Abs(Ch.CashSummWithOverpayment2), Math.Abs(Ch.CreditSumm2), 0, Math.Abs(Ch.CardSumm2), 1, 0, 0, 0, "", Ch);



            CloseCheckCommandBlock.CommandBlockToQwery();
            Utils.ToCardLog("CloseCheck2  End" + Ch.CheckShortNum);
        }




        /*
        internal static void XReport()
        {
            //Conn();
            bool b = false;
            do
            {
                Shtrih.GetShortECRStatus();
                b = ((Shtrih.ECRMode == 0) || (Shtrih.ECRMode == 2));
            } while (!b);
            ChangePos(true);
            ChangePrint(true);

            do
            {
                Shtrih.PrintReportWithoutCleaning();
            }
            while (Shtrih.ResultCode != 0);
            Shtrih.CutCheck(); 
            //Shtrih.Disconnect();
        }
        internal static void ZReport()
        {
            
            //Conn();

            bool b = false;
            do
            {
                Shtrih.GetShortECRStatus();
                b = ((Shtrih.ECRMode == 0) || (Shtrih.ECRMode == 2));
            } while (!b);
            
            Shtrih.WaitForPrinting();
            ChangePos(true);
            ChangePrint(true);

          //  do
           // {

            ToShtrih.SetTime(    ToShtrih.LastCheckTime().AddMinutes(1).AddSeconds(13)); 
            
                Shtrih.PrintReportWithCleaning();
                Shtrih.WaitForPrinting();
          //  }
           // while (Shtrih.ResultCode != 0);
            Shtrih.CutCheck();
            WriteBalanceToShtrih(new Balanse());
            //Shtrih.Disconnect();
        }
        */
        /*
        internal static void HideZReport()
        {

            //Conn();

            bool b = false;
            do
            {
                Shtrih.GetShortECRStatus();
                b = ((Shtrih.ECRMode == 0) || (Shtrih.ECRMode == 2));
            } while (!b);

            Shtrih.WaitForPrinting();
            ChangePos(true);
            ChangePrint(true );

            //  do
            // {

            ToShtrih.SetTime(ToShtrih.LastCheckTime().AddMinutes(1).AddSeconds(13));

            Shtrih.PrintReportWithCleaning();
            Shtrih.WaitForPrinting();
            //  }
            // while (Shtrih.ResultCode != 0);
            Shtrih.CutCheck();
            WriteBalanceToShtrih(new Balanse());
            //Shtrih.Disconnect();
        }

        */
        /*
         public static decimal GetEKLZSale()
         {
             Shtrih.Password = 30;
             Shtrih.GetEKLZCode2Report();
             return Shtrih.Summ1;
         }

         public static decimal GetEKLZVozvrSale()
         {
             Shtrih.Password = 30;
             Shtrih.GetEKLZCode2Report();
             return Shtrih.Summ3;
         }

         */
        /*
        static internal string  GetKkmNum()
        {
            Shtrih.Password = 30;
            Shtrih.GetShortECRStatus();
            return Shtrih.SerialNumber;

        }
         
        static internal int GetLastKpkNum()
        {
            Shtrih.Password = 30;
            Shtrih.GetEKLZCode1Report();
            return Shtrih.LastKPKNumber;

        }
        /*
        static internal bool ClosedSmena(out String status)
        {
            Shtrih.Password = 30;
            Shtrih.GetECRStatus();
            status = Shtrih.ECRModeDescription;
            return (Shtrih.ECRMode ==4);

        }
        */
        private static byte[] HexStringToByteArray(string s)
        {
            s = s.Replace(" ", "");
            byte[] buffer = new byte[s.Length / 2];
            for (int i = 0; i < s.Length; i += 2)
                buffer[i / 2] = (byte)Convert.ToByte(s.Substring(i, 2), 16);
            return buffer;
        }

        //public static  bool WriteReg(int RegNum, int RegVal)
        //{
        //    System.IO.Ports.SerialPort port = new System.IO.Ports.SerialPort();  
        //    try
        //    {
        //        /*
        //        while (!ToShtrih.Conn())
        //        {
        //            label1.Text = (Environment.NewLine + "Внимание! Вы не подключили фискальный регистратор. " + Environment.NewLine +
        //            "Продолжение работы невозможно." + Environment.NewLine +
        //            "Поменяйте регистратор и нажмите на кнопку продолжить.");

        //        }
        //        ToShtrih.Disconnect();
        //        */
        //        //Shtrih.Disconnect();
        //        port.WriteTimeout = 5000;
        //        port.ReadTimeout = 20000;
        //        port.BaudRate = 115200;
        //        port.PortName = "com" + Shtrih.ComNumber;
        //        port.NewLine = Environment.NewLine;
        //        port.DtrEnable = true;
        //        port.RtsEnable = true;
        //        port.Parity = Parity.None;
        //        port.ReadBufferSize = 1024;
        //        port.WriteBufferSize = 1024;
        //        port.Handshake = Handshake.None;

        //        port.Open();
        //        port.DiscardInBuffer();
        //        port.DiscardOutBuffer();
        //        byte[] com = HexStringToByteArray("05");

        //        port.Write(com, 0, 1);
        //        int k = (port.Read(com, 0, 1));

        //        string sRegNum = Convert.ToString(RegNum, 16);
        //        if (sRegNum.Length == 1)
        //        {
        //            sRegNum = "0" + sRegNum;
        //        }
        //        string sRegVal = Convert.ToString(RegVal, 16);
        //        if (sRegVal.Length == 1)
        //        {
        //            sRegVal = "0" + sRegVal + "00";
        //        }
        //        else if (sRegVal.Length == 2)
        //        {
        //            sRegVal = sRegVal + " 00";
        //        }
        //        else if (sRegVal.Length == 3)
        //        {
        //            sRegVal = sRegVal[1].ToString() + sRegVal[2].ToString() + "0" + sRegVal[0].ToString();
        //        }
        //        else if (sRegVal.Length == 4)
        //        {
        //            sRegVal = sRegVal[2].ToString() + sRegVal[3].ToString() + sRegVal[0].ToString() + sRegVal[1].ToString();
        //        }

        //        byte[] b = HexStringToByteArray("02 0A FF 00" + sRegNum + "00 " + sRegVal + " 00 00 00 00 00");

        //        byte xor = 0;
        //        for (int i = 1; i < b.Length - 1; i++)
        //        {
        //            if (i < 3)
        //            {
        //                xor = (byte)(b[1] ^ b[2]);
        //            }
        //            else
        //            {
        //                xor = (byte)(xor ^ b[i]);
        //            }
        //        }

        //        b[b.Length - 1] = xor;

        //        port.Write(b, 0, b.Length);
        //        int kk = (port.Read(com, 0, 1));
        //        byte[] bb = HexStringToByteArray("06");


        //        port.Write(bb, 0, bb.Length);
        //        //string s = port.ReadLine();
        //        port.Close();
        //        return true;

        //    }
        //    catch (Exception e)
        //    {
        //        port.Close();
        //        return false;
        //    }
        //}

        public static void PrintOutKassetaReport(double Summ, bool Kasseta)
        {
            string CurentDate = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            ShtrihCommandBlock ShtrihCommandBlockPrint = new ShtrihCommandBlock();
            ShtrihCommandBlockPrint.PrintString(CurentDate);
            if (Kasseta)
            {
                ShtrihCommandBlockPrint.PrintString("ОТЧЕТ ИЗЪЯТИЯ КАССЕТЫ ");
            }
            else
            {
                ShtrihCommandBlockPrint.PrintString("ОТЧЕТ ИЗЪЯТИЯ МОНЕТ ");
            }
            ShtrihCommandBlockPrint.PrintString("-------------------------------");
            ShtrihCommandBlockPrint.PrintString("  ");
            ShtrihCommandBlockPrint.PrintWideString("СУММА   " + Summ.ToString("0.00").Replace(",", "."));
            ShtrihCommandBlockPrint.PrintString("  ");
            ShtrihCommandBlockPrint.PrintString("-------------------------------");
            ShtrihCommandBlockPrint.PrintString("  ");
            if (Kasseta)
            {
                ShtrihCommandBlockPrint.PrintString("КАССЕТА ИЗЪЯТА");
            }
            else
            {
                ShtrihCommandBlockPrint.PrintString("МОНЕТЫ ИЗЪЯТЫ");
            }
            ShtrihCommandBlockPrint.FeedDocument();


            ShtrihCommandBlockPrint.CutCheck();
            //ShtrihCommandBlockPrint.FinishDocument();
            PrintCaptionWithFont(1, ShtrihCommandBlockPrint);
            ShtrihCommandBlockPrint.CommandBlockToQwery();
        }

        private static void PrintCaptionWithFont(int FontType, ShtrihCommandBlock Cb)
        {

            List<string> Tmp = Shtrih2.GetCaption();
            //Shtrih.FontType = FontType;
            foreach (string s in Tmp)
            {
                Cb.PrintStringWithFont(s, FontType);
                //  Shtrih.StringForPrinting = s;
                //  mPrintStringWithFont();
            }
        }

        /*
        static private void GetKkmData()
        {
            Shtrih.Password = 30;
            Shtrih.GetECRStatus();
            KKMNUmber = Shtrih.SerialNumber;
            INNNUmber = Shtrih.INN;
            OpenDocumentNumber = Shtrih.OpenDocumentNumber;
            CurentDate = Shtrih.Date.ToString("dd.MM.yy");
            CurentTime = Shtrih.Time.ToString("HH:mm");
            Shtrih.RegisterNumber = 158;
            Shtrih.GetOperationReg();
            LastXReportNumber = Shtrih.ContentsOfOperationRegister;
            Shtrih.RegisterNumber = 244;
            Shtrih.GetCashReg();
            TotalSumStart = (double)Shtrih.ContentsOfCashRegister;
            Shtrih.RegisterNumber = 148;
            Shtrih.GetOperationReg();
            TotalChecks = Shtrih.ContentsOfOperationRegister;
            Shtrih.RegisterNumber = 150;
            Shtrih.GetOperationReg();
            TotalVozvratChecks = Shtrih.ContentsOfOperationRegister;
        }
        */
        private static string AddMiddleSpace(string InString)
        {
            if (InString.Contains("&&"))
            {
                if (InString.Count() - 2 <= Shtrih2.CharCountWidth)
                {
                    InString = InString.Replace("&&", new String(' ', Shtrih2.CharCountWidth + 2 - InString.Count()));
                }
            }
            return InString;
        }

        internal static void PrintCardCheck(List<string> Slip, String DocName)
        {
            ShtrihCommandBlock ShtrihCommandBlockPrint = new ShtrihCommandBlock();
            ShtrihCommandBlockPrint.PrintDocumentTitle(DocName, 1);
            foreach (string s in Slip)
            {
                String NewS = AddMiddleSpace(s);
                ShtrihCommandBlockPrint.PrintString(NewS);
            }

            ShtrihCommandBlockPrint.CutCheck();

            ShtrihCommandBlockPrint.CommandBlockToQwery();
        }

        internal static void PrintCardCheck(object _Rcp,bool printCaption=true)
        {
            Utils.ToCardLog("PrintCardCheck start");
            ShtrihCommandBlock ShtrihCommandBlockPrint = new ShtrihCommandBlock();
            string Rcp = (string)_Rcp;
            try
            {
                string[] Str = Rcp.Split(char.ConvertFromUtf32(10)[0]);

                ShtrihCommandBlockPrint.PrintString("   ");
                bool ShtrihCut = false;
                foreach (string str in Str)
                {
                    if (str.Length < 1) continue;
                    string str1 = str.Substring(0, str.Length - 1);
                    int StingWidth = 36;
                    if (str1.Length > 36)
                    {
                        str1 = str1.Replace("  ", " ");
                        if (str1.Length > 36)
                        {
                            str1 = str1.Substring(0, 36);
                        }

                    }
                    str1 = str1.Replace("&&", new string(" "[0], StingWidth - (str1.Length - 2)));

                    if (str1.Length == 0)
                    {
                        ShtrihCommandBlockPrint.PrintString("      ");
                        ShtrihCut = false;
                    }
                        
                    else if (str1.ToCharArray()[0] == 31)
                    {
                        ShtrihCommandBlockPrint.PrintString("      ");
                        ShtrihCommandBlockPrint.PrintString("      ");
                        ShtrihCommandBlockPrint.PrintString("      ");
                        ShtrihCommandBlockPrint.PrintString("      ");
                        ShtrihCommandBlockPrint.PrintString("      ");
                        ShtrihCommandBlockPrint.CutCheck();
                        ShtrihCut = true;
                    }
                        else if (str1.Contains("0xDA"))
                    {
                        str1 = str1.Replace("0xDA", "");
                        ShtrihCommandBlockPrint.PrintString(str1);
                        ShtrihCommandBlockPrint.PrintString("      ");
                        ShtrihCommandBlockPrint.PrintString("      ");
                        ShtrihCommandBlockPrint.PrintString("      ");
                        ShtrihCommandBlockPrint.CutCheck();
                        ShtrihCut = true;
                     }
                    
                    else if (str1.Contains(Convert.ToChar(1)))
                    {
                        //str1 = str1.Replace("0xDA", "");
                        Utils.ToCardLog("Cutting");
                        ShtrihCommandBlockPrint.PrintString("      ");
                        ShtrihCommandBlockPrint.PrintString("      ");
                        ShtrihCommandBlockPrint.PrintString("      ");
                        ShtrihCommandBlockPrint.CutCheck();
                        ShtrihCommandBlockPrint.PrintString(str1);
                       
                        ShtrihCut = true;
                    }

                    else
                    {

                        ShtrihCommandBlockPrint.PrintString(str1);
                        ShtrihCut = false;
                    }
                }
                if (!ShtrihCut)
                {
                    ShtrihCommandBlockPrint.PrintString("      ");
                    ShtrihCommandBlockPrint.PrintString("      ");
                    ShtrihCommandBlockPrint.PrintString("      ");
                    ShtrihCommandBlockPrint.PrintString("      ");
                    ShtrihCommandBlockPrint.PrintString("      ");
                    ShtrihCommandBlockPrint.CutCheck();
                }
                if (printCaption) { PrintCaptionWithFont(1, ShtrihCommandBlockPrint); };
                ShtrihCommandBlockPrint.CommandBlockToQwery();
                Utils.ToCardLog("PrintCardCheck End");
            }
            catch (Exception e)
            {
                Utils.ToLog("[Error] PrintCardCheck " + e.Message);
            }
            //Shtrih.Disconnect(); 
        }
        /*
        public static  Dictionary<int, int> GetCodes(int StartcCode)
        {
            Dictionary<int, int> tmp = new Dictionary<int,int> ();

            for (int i = StartcCode; i < StartcCode + 1000; i++)
            {
                try
                {
                    Shtrih.KPKNumber = i;
                    Shtrih.GetEKLZDocument();
                    Shtrih.GetEKLZData();
                    //Shtrih.GetShortECRStatus();

                    if (Shtrih.ResultCode != 0)
                    {
                        break;
                    }


                    int k = 0;
                    while (Shtrih.ResultCode == 0)
                    {
                        Shtrih.GetEKLZData();
                    }

                    string s = Shtrih.EKLZData;
                    string kpCode = s.Substring(10, 6);
                    tmp.Add(i, Convert.ToInt32(kpCode));
                }
                catch
                { 
                
                }
            }


            return tmp;
        }

*/


        /*
        static internal Balanse GetRealSum()
        {

            Shtrih.Password = 30;
            Shtrih.TableNumber = 4;
            Shtrih.RowNumber = 1;
            Shtrih.FieldNumber = 1;
            Shtrih.GetFieldStruct();
            Shtrih.ReadTable();
            try
            {
                string[] S = Shtrih.ValueOfFieldString.Split(";"[0]);
                Balanse Ret = new Balanse
                {
                    Card = GetDoubleFromString(S, 1),
                    Nal = GetDoubleFromString(S, 0),
                    VozvrCard = GetDoubleFromString(S, 3),
                    VozvrNal = GetDoubleFromString(S, 2),
                    CountNal = (int)GetDoubleFromString(S, 4),
                    CountCard = (int)GetDoubleFromString(S, 5),
                    CountVozvrNal = (int)GetDoubleFromString(S, 6),
                    CountVozvrCard = (int)GetDoubleFromString(S, 7),
                };
                return Ret;
            }
            catch(Exception e)
            {
                throw new Exception(e.Message);  
                return new Balanse();
            }

        }
         * */
        static private double GetDoubleFromString(string[] s, int Index)
        {
            try
            {
                return Convert.ToDouble(s[Index]);
            }
            catch
            {
                return 0;
            }
        }


        /*
          static public  void PrintSample()
          {

              for (int j = 1; j < 8
                  ; j++)
              {
                  string[] k = new string[256];
                  Shtrih.FontType = j;
                  for (int i = 4; i < 256; i++)
                  {
                      k[i] = "      " + Char.ConvertFromUtf32(i) + " " + j.ToString() + " " + i.ToString();
                      Shtrih.StringForPrinting = k[i];
                      Shtrih.PrintStringWithFont(); 
                  }
                  /*
                                  do


                                  { } while (!Printing(k));
                   * */


        //}
        //Shtrih.StringQuantity = 5;
        //Shtrih.FeedDocument();
        //Shtrih.WaitForPrinting();

        //}

        static private string ToShtrihFormat(double d)
        {
            //      string Eq2 = "&&" + Char.ConvertFromUtf32(16); //Это был знак тритравно. Исчез в новых прошивках!
            string Eq2 = "&&" + "=";



            //  string Eq2 = "&& K"  ;
            return Eq2 + d.ToString("0.00").Replace(",", ".");

        }



        internal static bool PrinterIsWhite()
        {
            /*
                Shtrih.Password = 30;
                Shtrih.TableNumber = 1;
                Shtrih.RowNumber = 1;
                Shtrih.FieldNumber = 49;
                Shtrih.GetFieldStruct();

                Shtrih.ReadTable();
            */

            return (Shtrih2.ReadTableInt(1, 1, 49) == 0);
        }

        /*
        private void CheckResCode()
        {
            string mess = "";
            switch (Shtrih.ResultCode )
            {
                case -1:
                    mess = ""
                default:
            }

        }

        */
    }


    [Serializable]
    public class TableRowInfo
    {
        public TableRowInfo()
        { }
        public int Num = 0;
        public string Name = "";
        public string ValueOfFieldString = "";
        public int ValueOfFieldInteger = 0;
        public bool FieldType = true;
        public int MINValueOfField = 0;
        public int MAXValueOfField = 0;
        public int FieldSize = 0;
        public bool IsCange = false;
    }
    public class Balanse
    {
        public double Nal = 0;
        public double Card = 0;
        public double VozvrNal = 0;
        public double VozvrCard = 0;
        public int CountNal = 0;
        public int CountCard = 0;
        public int CountVozvrNal = 0;
        public int CountVozvrCard = 0;
        public decimal AllSumm
        {
            get
            {
                return (decimal)(Nal + Card - VozvrNal - VozvrCard);
            }
        }
        public Balanse()
        {
        }
        public void Print()
        {
            Console.WriteLine("Nal: " + Nal.ToString());
            Console.WriteLine("Card: " + Card.ToString());
            Console.WriteLine("VozvrNal: " + VozvrNal.ToString());
            Console.WriteLine("VozvrCard: " + VozvrCard.ToString());
            Console.WriteLine("CountNal: " + CountNal.ToString());
            Console.WriteLine("CountCard: " + CountCard.ToString());
            Console.WriteLine("CountVozvrNal: " + CountVozvrNal.ToString());
            Console.WriteLine("CountVozvrCard: " + CountVozvrCard.ToString());

        }

        internal const double DoubleMin = 0.001;
        internal bool IsEqv(Balanse B)
        {
            if (((Math.Abs(B.Nal - Nal) > DoubleMin)
               || (Math.Abs(B.Card - Card) > DoubleMin)
               || (Math.Abs(B.VozvrCard - VozvrCard) > DoubleMin)
               || (Math.Abs(B.VozvrNal - VozvrNal) > DoubleMin)
               || (B.CountNal != CountNal)
               || (B.CountCard != CountCard)
               || (B.CountVozvrCard != CountVozvrCard)
               || (B.CountVozvrNal != CountVozvrNal)))
            {
                return false;
            }
            return true;

        }
    }
}
