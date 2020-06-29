using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Sql;
using System.Data.SqlClient;

namespace PDiscountCard.SQL
{
    public static class ToSql
    {
        static string ConnectString = @"Data Source=" + Utils.GetFileServerName() + @"\sqlexpress;Initial Catalog=AlohaPDiscount;Persist Security Info=True;User ID=sa;Password=";
     //   static string ConnectString = @"Data Source=" + "PiskovStend1" + @"\sqlexpress;Initial Catalog=AlohaPDiscount;  Persist Security Info=True;User ID=sa;Password=";
        static AlohaPDiscountSQLDataContext MyBase = new AlohaPDiscountSQLDataContext(ConnectString);
        public static void CheckBase()
        {
            
                if (!iniFile.SQLCheckDisabled)
                {
                    if (!MyBase.DatabaseExists())
                    {
                        try
                        {
                            Utils.ToCardLog("Отсутствует база данных PDiscount. Создаю");
                            MyBase.CreateDatabase();

                        }
                        catch (Exception e)
                        {

                            Utils.ToCardLog("[Error CreateDatabase] " + e.Message);
                        }
                    }
                    CheckTables();
                }

            

        }
        private static void CheckTables()
        {
            try
            {
                Utils.ToCardLog("CheckTables");
                List<string> CommandsList = new List<string> ()
                {

                "if (select count(*) from information_schema.tables " +
    "Where Table_name = 'WestHEADER')=0 " +
    "begin  " +
    "CREATE TABLE WestHEADER ( " +
    "Name char(50), " +
    "DateStart datetime, " +
    "DateEnd datetime " +
    "); " +
    "end ",


    "if (select count(*) from information_schema.tables " +
    "Where Table_name = 'WestITM ')=0 " +
    "begin  " +
    "CREATE TABLE WestITM  ( " +
   "Barcode integer, " +
"Quantity integer " +
    "); " +
    "end ",


      "if (select count(*) from information_schema.tables " +
    "Where Table_name = 'WestPager')=0 " +
    "begin  " +
    "CREATE TABLE WestPager  ( " +
   "id_employee integer, " +
"NumPager integer " +
    "); " +
    "end ",



    "if (  "+
    "    SELECT Count(*) FROM information_schema.COLUMNS "+
    "    WHERE COLUMN_NAME='Sale' AND TABLE_NAME='OrderTime'  "+
    ") =0 "+
    "begin "+
    "    ALTER TABLE [dbo].[OrderTime]  ADD Sale [bit] "+
"end ",


"if (  "+
    "    SELECT Count(*) FROM information_schema.COLUMNS "+
    "    WHERE COLUMN_NAME='EmployeeOwner' AND TABLE_NAME='OrderTime'  "+
    ") =0 "+
    "begin "+
    "    ALTER TABLE [dbo].[OrderTime]  ADD EmployeeOwner [int] "+
"end ",

"if (  "+
    "    SELECT Count(*) FROM information_schema.COLUMNS "+
    "    WHERE COLUMN_NAME='Plan' AND TABLE_NAME='WestHEADER'  "+
    ") =0 "+
    "begin "+
    "    ALTER TABLE [dbo].[WestHEADER]  ADD [Plan] [int] "+
"end "

,


"if (  "+
    "    SELECT Count(*) FROM information_schema.COLUMNS "+
    "    WHERE COLUMN_NAME='DishClosed' AND TABLE_NAME='OrderTime'  "+
    ") =0 "+
    "begin "+
    "    ALTER TABLE [dbo].[OrderTime]  ADD DishClosed [bit] not null default 0 "+
"end "


            };
                SqlConnection sc = new SqlConnection(ConnectString);
                sc.Open();

                foreach(string SqlCommandTxt in CommandsList)
                {
                    try
                    {
                        SqlCommand Comm = new System.Data.SqlClient.SqlCommand(SqlCommandTxt, sc);
                        Comm.ExecuteNonQuery();
                    }
                    catch(Exception e)
                    {
                        Utils.ToCardLog("[Error CheckTables Command ] " + SqlCommandTxt +" [Mess]: "+ e.Message);
                    }
                }

                sc.Close();
            }
            catch (Exception e)
            {
                Utils.ToCardLog("[Error CheckTables] " + e.Message);
            }
        }


        internal static void InsertOrderTime(List<OrderedDish> BarCodes, int CheckId, int Tableid, DateTime dt, DateTime DOB, int QueueId, int EmployeeId, int ModeId, int EmployeeOwner)
        {
            try
            {
                if (!iniFile.SQLCheckDisabled)
                {
                    foreach (OrderedDish BC in BarCodes)
                    {
                        OrderTime Ot = new OrderTime()
                        {
                            BarCode = BC.BarCode,
                            BusinessDate = DOB,
                            CheckId = CheckId,
                            SystemDate = dt,
                            EmployeeId = EmployeeId,
                            ModeId = ModeId,
                            QueueId = QueueId,
                            TableId = Tableid,
                            EntryId = BC.EntryId,
                            Sale = false,
                            EmployeeOwner = EmployeeOwner,
                            DishClosed = false

                        };
                        Utils.ToCardLog(String.Format("Add To SQL EntryId {0} , BarCode {1}", BC.EntryId, BC.BarCode));
                        MyBase.OrderTimes.InsertOnSubmit(Ot);
                    }
                    MyBase.SubmitChanges();
                }
            }
            catch (Exception e)
            {
                Utils.ToCardLog("[Error InsertOrderTime] " + e.Message);
            }
        
        }

        /*
        internal static void UpdateOrderTime(OrderedChk OChk)
        { 
            foreach (OrderedDish Od in OChk.OrderedDishes)
            {
                try
                {
                    DateTime ChkDt = (from o in MyBase.OrderTimes where o.CheckId.Value == OChk.CheckId && o.BarCode == Od.BarCode select o.SystemDate).Max().Value;
                    OrderTime Ot = (from o in MyBase.OrderTimes where o.CheckId.Value == OChk.CheckId && o.BarCode == Od.BarCode && o.SystemDate == ChkDt select o).First();
                    Ot.EntryId = Od.EntryId;
                    MyBase.SubmitChanges();
                }
                catch
                { 
                
                }
            }
        }
        */


        internal static List<int> GetOrderTimeEntry()
        {
            List<int> Tmp = new List<int>();
            try
            {
                if (!iniFile.SQLCheckDisabled)
                {
                    Tmp = (from o in MyBase.OrderTimes where !o.Sale.Value && o.BusinessDate > AlohainiFile.BDate.AddDays(-1) select o.EntryId.Value).ToList();
                }


            }
            catch (Exception e)
            {

            }
            
            //Tmp.RemoveAll
            return Tmp;
        }

        internal static DateTime GetOrderTime(int BarCode, int CheckId, int EntryId, DateTime OpenTime, bool Sale, int EmployeeId)
        {
            Utils.ToCardLog(String.Format("GetOrderTime BarCode {0} EntryId {1} ", BarCode, EntryId));
            DateTime dt = DateTime.Now;
            if (!iniFile.SQLCheckDisabled)
            {
                try
                {

                    if ((from o in MyBase.OrderTimes where o.BarCode == BarCode && o.EntryId == EntryId select o).Count() < 1)
                    {
                        Utils.ToCardLog("GetOrderTime entry non find");
                        OrderTime Ot = new OrderTime()
                        {
                            BarCode = BarCode,
                            BusinessDate = AlohainiFile.BDate,
                            CheckId = CheckId,
                            SystemDate = dt,
                            EmployeeId = EmployeeId,
                            ModeId = 0,
                            QueueId = 0,
                            TableId = 0,
                            EntryId = EntryId,
                            Sale = Sale,
                            EmployeeOwner = EmployeeId,
                            DishClosed = true

                        };
                        MyBase.OrderTimes.InsertOnSubmit(Ot);
                    }
                    else
                    {
                        Utils.ToCardLog("GetOrderTime find dt "+dt.ToString());
                        //   dt = (from o in MyBase.OrderTimes where o.BarCode == BarCode && o.CheckId == CheckId && o.EntryId==EntryId select o.SystemDate).Max().Value;
                        dt = (from o in MyBase.OrderTimes where o.BarCode == BarCode && o.EntryId == EntryId select o.SystemDate).Max().Value; //уБИРАЕМ сравнение по чеку, т.к. возможен перенос блюда



                        OrderTime ot = (from o in MyBase.OrderTimes where o.BarCode == BarCode && o.EntryId == EntryId && o.SystemDate == dt select o).First();
                        ot.Sale = Sale;
                        ot.DishClosed = true;
                    }
                    Utils.ToCardLog("GetOrderTime before submit " );
                    MyBase.SubmitChanges();

                    


                }
                catch (Exception e)
                {
                    Utils.ToLog("Error GetOrderTime not find by EntryId "+ e.Message);
                    try
                    {
                        dt = (from o in MyBase.OrderTimes where o.BarCode == BarCode && o.CheckId == CheckId select o.SystemDate).Max().Value;
                    }
                    catch (Exception ee)
                    {
                        Utils.ToLog("Error GetOrderTime " + ee.Message);
                        //Отправить ошибку на сервер
                    }
                }
            }
            if (dt < OpenTime.AddHours(-24))
            {
                Utils.ToLog("GetOrderTime Not find");
                dt = OpenTime;
            }

            return dt;
        }



    }
}
