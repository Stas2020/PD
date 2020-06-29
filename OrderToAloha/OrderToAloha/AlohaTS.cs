using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Xml;
using AlohaFOHLib;
using NLog;


namespace OrderToAloha
{
    /*
  internal static class AlohaTS
    {
        static IberDepot Depot;
        static IberFuncs AlohaFuncs;
     //  public static Logger _logger ;


        

        private const int INTERNAL_EMPLOYEES = 500;
        private const int INTERNAL_CHECKS = 540;
        private const int INTERNAL_TABLES = 520;
        private const int INTERNAL_TABLES_OPEN_CHECKS = 650;
        private const int INTERNAL_EMP_OPEN_TABLES = 501;
        private const int INTERNAL_CHECKS_PAYMENTS = 543;
        private const int INTERNAL_CHECKS_COMPS = 545;
        private const int INTERNAL_CHECKS_ENTRIES = 542;
      
        private const int FILE_TAB = 30;
        private const int FILE_ITM = 12;


      static internal void DisposeAloha()
      {
          try
          {
              AlohaFuncs = new IberFuncs();
              Depot = new IberDepot();
              GC.Collect();
          }
          catch
          { }
      }

        static internal void InitAloha()
        {
            try
            {
                
                AlohaFuncs = new IberFuncs();
                Depot = new IberDepot();
            }
            catch(Exception e)
            {
                AlohaException Ae = new AlohaException("Не могу подключиться к алохе",e);
                Ae.ErrorCode = 1;
                
                throw Ae;
            }
        
        }
        
        static private int GetJobCode(int EmpNum)
        {


            try
            {
                IberObject Employee = Depot.FindObjectFromId(INTERNAL_EMPLOYEES, EmpNum).First();
                return Employee.GetLongVal("JOBCODE1");
            }
            catch
            {

            }

            return 0;
        }
        static private void LoginEmpl()
        { 
             int CheckId = 0;
            try
            {
                AlohaFuncs.LogOut( Properties.Settings.Default.TerminalNumber);
            }
            catch
            { }

            try
            {
                AlohaFuncs.LogIn(Properties.Settings.Default.TerminalNumber, Properties.Settings.Default.EmployeeNumber, "", "");
            }
            catch(Exception e)
            {
                AlohaException ae = new AlohaException("Ошибка попытке  LogIn с параметрами Term = " + Properties.Settings.Default.TerminalNumber + "; Emp = " + Properties.Settings.Default.EmployeeNumber, e);
                ae.AlohaErrorDescription = (new CAlohaErrors(e.Message)).ValStr;
                throw ae;
                



            }
            //_logger.Debug("LogIn выполнен успешно" );
            try
            {
                AlohaFuncs.ClockIn(Properties.Settings.Default.TerminalNumber, GetJobCode(Properties.Settings.Default.EmployeeNumber));
            }
            catch
            { }
        }

        static private int GetEmptyTable()
        {
            return 10;
        }


        static private List<int> GetTables()
        {
            IberEnum FTables = Depot.GetEnum(FILE_TAB);
            List<int> Tmp = new List<int>();
            foreach (IberObject Tbl in FTables)
            {
                Tmp.Add(Tbl.GetLongVal("ID"));
            
            }
            return Tmp;
        }



        static private AlohaCheck GetCheckByAlohaId(int AlohaId)
        {
            AlohaCheck Chk = new AlohaCheck();
            try
            {
                IberEnumClass ChEnum = (IberEnumClass)Depot.FindObjectFromId(INTERNAL_CHECKS, AlohaId);
                IberObjectClass Ch = (IberObjectClass)ChEnum.First();




                IberObjectClass ChTable = null;
                try
                {
                    int TId = Ch.GetLongVal("TABLE_ID");
                    IberEnumClass TEnum = (IberEnumClass)Depot.FindObjectFromId(INTERNAL_TABLES, TId);
                    ChTable = (IberObjectClass)TEnum.First();
                    Chk.TableNumber = ChTable.GetLongVal("TABLEDEF_ID");
                    
                }
                catch (Exception e)
                {
                  
                }

 
                IberObjectClass PayMent = null;
                try
                {
 
                    IberEnumClass PayMentsEnum = (IberEnumClass)Ch.GetEnum(INTERNAL_CHECKS_PAYMENTS);
                    foreach (IberObject POb in PayMentsEnum)
                    {
                        CPayment Pay = new CPayment()
                        {
                            PType = PayMent.GetLongVal("TENDER_ID"),
                            Count = (decimal)PayMent.GetDoubleVal("AMOUNT")
                        };
                    }
                }
                catch (Exception ee)
                {
                    
                }
                double ChSumm = 0;
                double ChTax = 0;
                AlohaFuncs.GetCheckTotal(AlohaId, out ChSumm, out ChTax);
                Chk.Summ = (decimal)ChSumm;
                Chk.AlohaId = AlohaId;
                
                

                try
                {
                    IberEnumClass CompsEnum = (IberEnumClass)Ch.GetEnum(INTERNAL_CHECKS_COMPS);
                    IberObjectClass Comp = (IberObjectClass)CompsEnum.First();
                    Chk.DiscTypeId = Comp.GetLongVal("COMPTYPE_ID");
                    Chk.DiscId = Comp.GetLongVal("ID");
                    
                }
                catch
                {
                    
                }
                try
                {
                    Chk.items = GetDishesOfCheck(AlohaId);
                    
                    
                }
                catch (Exception e)
                {
                
                }
                return Chk;
            }
            catch (Exception ee)
            {
                
                return null;
            }
        
        
        }


        static internal List<Item> GetDishesOfCheck(int CheckNum)
        {
            List<Item> Tmp = new List<Item>();
            IberEnum Dishez = new IberEnum();
            IberObject MyCheck = Depot.FindObjectFromId(INTERNAL_CHECKS, CheckNum).First();
            try
            {
                Dishez = MyCheck.GetEnum(INTERNAL_CHECKS_ENTRIES);

            }
            catch
            {
                return Tmp;
            }
            // Dish Ad = null;

            



            string SpecMess = "";
            Item CurDish = new Item();
            Item CurMod = new Item();

            foreach (IberObject IntITM in Dishez)
            {
                //(IntITM.GetLongVal("MOD_CODE")!=8 &&(IntITM.GetLongVal("MOD_CODE")!=12) - Это отмены
                if ((IntITM.GetLongVal("TYPE") == 0) && ((IntITM.GetLongVal("MOD_CODE") != 8 && (IntITM.GetLongVal("MOD_CODE") != 12))))
                //Regular item                          MOD_DELETED                             MOD_PRINTED_DELETED
                {
                    
                    if (IntITM.GetLongVal("LEVEL") == 0)  // не Модификатор
                    {
                        Item mD = new Item()
                        {
                            Count = 1,
                            //BarCode = IntITM.GetLongVal("DATA").ToString(),
                            Price = Convert.ToDecimal(IntITM.GetDoubleVal("OPRICE")),
                            EntryId = IntITM.GetLongVal("ID")
                        };
                        int AlohaBC = IntITM.GetLongVal("DATA");
                        mD.BarCode = AlohaBC;
                        Tmp.Add(mD);
                    }
                }
                else if (IntITM.GetLongVal("TYPE") == 1) //- Это Special modifier message
                {
                  
                }
            }
            //пытаемся дать LongName и весовое количество

            
            
            return Tmp;
        }


      //Эту процедуру надо бы ускорить;
        static private int GetCheckBySTId(int StOrderId,out int Tnum,out int TId, out AlohaCheck Chk)
        {
            Chk = new AlohaCheck ();
            Tnum = 0;
            TId = 0;
            IberEnum Empls = new IberEnum();
            Empls = Depot.GetEnum(INTERNAL_EMPLOYEES);
            foreach (IberObject Empl in Empls)
            {
                if (!Convert.ToBoolean(Empl.GetBoolVal("TERMINATED")))
                {
                    try
                    {
                        IberEnum OpenTables = Empl.GetEnum(INTERNAL_EMP_OPEN_TABLES);
                        int empId = Empl.GetLongVal("ID");
                        foreach (IberObject OpenTable in OpenTables)
                        {
                            try
                            {
                                IberEnum OpenChks = OpenTable.GetEnum(INTERNAL_TABLES_OPEN_CHECKS);
                                Tnum = OpenTable.GetLongVal("TABLEDEF_ID");
                                TId = OpenTable.GetLongVal("ID");
                                foreach (IberObject OpenChk in OpenChks)
                                {
                                    string Tm = "";

                                    try
                                    {
                                        Tm = AlohaFuncs.GetObjectAttribute(INTERNAL_CHECKS, OpenChk.GetLongVal("ID"), "STId");

                                    }
                                    catch
                                    {

                                    }
                                    if (Tm.Trim() == StOrderId.ToString())
                                    {
                                        Chk = GetCheckByAlohaId(OpenChk.GetLongVal("ID"));
                                        return OpenChk.GetLongVal("ID");
                                    }
                                }
                            }
                            catch
                            { 
                            }
                        }
                    }
                    catch
                    { }

                }

                
            }
            return 0;
        }


        static private List<Item> GetTmpItms(List<Item> Itms)
        {
            List<Item> Tmp = new List<Item>();
            foreach (Item itm in Itms)
            {
                for (int i = 0; i < itm.Count; i++)
                {
                    Tmp.Add(itm);
                }
            }
            return Tmp;
        }
        static internal void GetDiff(List<Item> OldItms, List<Item> NewItms, out  List<Item> AddItms, out  List<Item> ClearItms)
        {
            List<Item> TmpOldItms = GetTmpItms(OldItms);
            List<Item> TmpNewItms = GetTmpItms(NewItms);
            ClearItms = new List<Item>();

            foreach(Item itm in TmpOldItms)
            {
                if (TmpNewItms.Where(a=>a.AlohaBarCode==itm.AlohaBarCode && a.Price==itm.Price).Count()>0)
                {
                    TmpNewItms.Remove(TmpNewItms.Where(a=>a.AlohaBarCode==itm.AlohaBarCode && a.Price==itm.Price).First());
                }
                else
                {
                    ClearItms.Add(itm);
                }
            }
            AddItms = TmpNewItms;
        }

        static internal int DeleteOrder(int OrderId)
        {
            Utils.ToLog(String.Format("DeleteOrder OrderId={0} ", OrderId));
            InitAloha();
            int TNum = 0;
            int OldTId = 0;
            AlohaCheck Achk = new AlohaCheck();
            int ChId = GetCheckBySTId(OrderId, out TNum, out OldTId, out Achk);
            if (ChId != 0) 
            {
                try
                {
                    

                    LoginEmpl();
                    AlohaFuncs.DeselectAllEntries(Properties.Settings.Default.TerminalNumber);
                    AlohaFuncs.SelectAllEntriesOnCheck(Properties.Settings.Default.TerminalNumber, ChId);
                    AlohaFuncs.ManagerVoidSelectedItems(Properties.Settings.Default.TerminalNumber, Properties.Settings.Default.ManagerNumber, ChId, Properties.Settings.Default.VoidReason);
                    AlohaFuncs.DeselectAllEntries(Properties.Settings.Default.TerminalNumber);
                    AlohaFuncs.CloseCheck(Properties.Settings.Default.TerminalNumber, ChId);

                }
                catch (Exception e)
                {
                    Utils.ToLog(String.Format("Ошибка удаления заказа. {0} ", e.Message));
                    AlohaException ae = new AlohaException("Ошибка удаления заказа № " + OrderId, e);
                    ae.AlohaErrorDescription = (new CAlohaErrors(e.Message)).ValStr;
                    throw ae;
                }
                finally
                { 
                    try
                    {
                        AlohaFuncs.LogOut(Properties.Settings.Default.TerminalNumber);
                       DisposeAloha();
                    }
                    catch
                    {}
                    
                }
                Utils.ToLog(String.Format("Заказ {0} успешно удален. ", OrderId));
                return 0;
            }
            else
            {
                Utils.ToLog(String.Format("Заказ {0} не найден. ", OrderId));
                return -1;
            }
            
        }


        public static int CloseOrder(int OrderId, int PaymentId)
        {
            Utils.ToLog(String.Format("CloseOrder OrderId={0} ", OrderId));
            InitAloha();
            int TNum = 0;
            int OldTId = 0;
            AlohaCheck Achk = new AlohaCheck();
            int ChId = GetCheckBySTId(OrderId, out TNum, out OldTId, out Achk);
            if (ChId != 0)
            {
                try
                {
            

                    LoginEmpl();
                    AlohaFuncs.ApplyPayment(Properties.Settings.Default.TerminalNumber, Achk.AlohaId, PaymentId, (double)Achk.Summ, 0, "", "", "", "");
                    AlohaFuncs.CloseCheck(Properties.Settings.Default.TerminalNumber, ChId);

                }
                catch (Exception e)
                {
                    Utils.ToLog(String.Format("Ошибка при закрытии чека. {0} ", e.Message));
                    AlohaException ae = new AlohaException("Ошибка при закрытии чека " + OrderId, e);
                    ae.AlohaErrorDescription = (new CAlohaErrors(e.Message)).ValStr;
                    throw ae;
                }
                finally
                {
                    try
                    {
                        AlohaFuncs.LogOut(Properties.Settings.Default.TerminalNumber);
                        DisposeAloha();
                    }
                    catch
                    { }

                }
                Utils.ToLog(String.Format("Заказ {0} успешно закрыт. ",OrderId));
                return 0;
            }
            else
            {
                Utils.ToLog(String.Format("Заказ {0} не найден. ", OrderId));
                return -1;
            }
        }

        static internal void AddOrder(int OrderId, List<Item> myItems, int CompanyId, string CompanyName, string BortName, int Discount, int Margin, DateTime TimeOfShipping, out int AlohaCheckId, out int TableNumId)
       {

           List<Item> Items = new List<Item>();
           Items.AddRange(myItems);
           Utils.ToLog(String.Format("AddOrder OrderId={0} CompanyId={1} Discount={2}, Margin={3}",OrderId,CompanyName,Discount,Margin));
           Utils.ToLog(String.Format("Список блюд:"));
           Utils.ToLog("Баркод, Цена, Количество");
           foreach (Item itm in Items)
           {
               Utils.ToLog(String.Format("{0}, {1}, {2}",itm.BarCode,itm.Price,itm.Count));
           }
           Utils.ToLog(String.Format("Окончание списка блюд:"));

           TableNumId = 0;
           AlohaCheckId = 0;
         //  _logger = new Logger();
           int k = 0;
           int TId = 0;
           try
           {
               InitAloha();

               LoginEmpl();

               int TNum = 0;
               int OldTId = 0;
               AlohaCheck Achk=new AlohaCheck ();
               int ChId = GetCheckBySTId(OrderId, out TNum, out OldTId, out Achk);
               if (ChId == 0) //Это новый заказ
               {
                   Utils.ToLog("Это новый заказ");
                   List<int> TNums = GetTables();
                   //List<int> BusyTNums = new List<int>();
                   do
                   {
                       try
                       {

                           if (TNums.Count > 0)
                           {
                               // _logger.Debug("Получил свободный стол № = " + TNum.ToString());
                               TId = AlohaFuncs.AddTable(Properties.Settings.Default.TerminalNumber, 0, TNums[0], OrderId.ToString(), 1);

                               //_logger.Debug("Открыл стол № " + TNums[0]);
                               // return TId;
                           }
                           else
                           {

                           }
                       }
                       catch (Exception e)
                       {
                           AlohaErr.CAlohaErrors Err = new AlohaErr.CAlohaErrors();
                           Err.SetVal(e.Message);
                           if (Err.Val != AlohaErr.AlohaErrEnum.ErrCOM_TableInUse)
                           {
                               AlohaException ae = new AlohaException("Ошибка открытия стола № " + TNums[0], e);
                               ae.AlohaErrorDescription = (new CAlohaErrors(e.Message)).ValStr;
                               throw ae;
                           }
                           else
                           {
                               TNums.Remove(TNums[0]);
                           }
                       }

                   } while ((TId == 0) && (TNums.Count > 0));
                   TableNumId = TNums[0];
                   ChId = AlohaFuncs.AddCheck(Properties.Settings.Default.TerminalNumber, TId);
                   AlohaFuncs.SetObjectAttribute(INTERNAL_CHECKS, ChId, "STId", OrderId.ToString());
                   AlohaFuncs.SetObjectAttribute(INTERNAL_CHECKS, ChId, "CompId", CompanyId.ToString());
                   AlohaFuncs.SetObjectAttribute(INTERNAL_CHECKS, ChId, "CompName", CompanyName.ToString());
                   AlohaFuncs.SetObjectAttribute(INTERNAL_CHECKS, ChId, "BortName", BortName.ToString());
                   AlohaFuncs.AddCheckInfo(Properties.Settings.Default.TerminalNumber, ChId, "CompId", CompanyId.ToString());
                   AlohaFuncs.AddCheckInfo(Properties.Settings.Default.TerminalNumber, ChId, "CompName", CompanyName.ToString());
                   AlohaCheckId = ChId;
               }
               else //Изменение старого заказа
               {
                   Utils.ToLog("Изменение старого заказа");
                   try
                   {
                       TableNumId = Achk.TableNumber;
                       TId = OldTId;

                       List<Item> AddItms = new List<Item>();
                       List<Item> DelItms = new List<Item>();

                       GetDiff(Achk.items, Items, out AddItms, out DelItms);

                       AlohaFuncs.DeselectAllEntries(Properties.Settings.Default.TerminalNumber);
                       foreach (Item itm in DelItms)
                       {
                           AlohaFuncs.SelectEntry(Properties.Settings.Default.TerminalNumber, Achk.AlohaId, itm.EntryId);
                           AlohaFuncs.ManagerVoidSelectedItems(Properties.Settings.Default.TerminalNumber, Properties.Settings.Default.ManagerNumber, Achk.AlohaId, Properties.Settings.Default.VoidReason);
                       }

                       Items.Clear();
                       foreach (Item itm in AddItms)
                       {
                           itm.Count=1;
                           Items.Add(itm);
                       }
                       
                       //AlohaFuncs.SelectAllEntriesOnCheck(Properties.Settings.Default.TerminalNumber, ChId);
                     //  AlohaFuncs.ManagerVoidSelectedItems(Properties.Settings.Default.TerminalNumber, Properties.Settings.Default.ManagerNumber, ChId, Properties.Settings.Default.VoidReason);
                   }
                   catch (Exception e)
                   {
                       AlohaException ae = new AlohaException("Ошибка удаления заказа № " + OrderId, e);
                       ae.AlohaErrorDescription = (new CAlohaErrors(e.Message)).ValStr;
                       throw ae;
                   }

                   AlohaCheckId = ChId;

               }
               foreach (Item Dish in Items)
               {
                   for (int i = 0; i < Dish.Count; i++)
                   {
                       try
                       {
                           int DId = AlohaFuncs.BeginItem(Properties.Settings.Default.TerminalNumber, ChId, Dish.AlohaBarCode, "", (double)Dish.Price);
                           AlohaFuncs.EndItem(Properties.Settings.Default.TerminalNumber);
                       }
                       catch (Exception e)
                       {
                           AlohaException ae = new AlohaException("Ошибка добавления блюда № " + Dish.BarCode, e);
                           ae.AlohaErrorDescription = (new CAlohaErrors(e.Message)).ValStr;
                        //   throw ae;
                       }
                   }
               }
               AlohaFuncs.DeselectAllEntries(Properties.Settings.Default.TerminalNumber);
               AlohaFuncs.SelectAllEntriesOnCheck(Properties.Settings.Default.TerminalNumber, ChId);
               int OrderMode = Properties.Settings.Default.OrderMode;
               try
               {
                   
                   if (Margin > 0)
                   {

                       OrderMode = Properties.Settings.Default.MarginOrderMode;
                   }
                   AlohaFuncs.OrderItems(Properties.Settings.Default.TerminalNumber, TId, OrderMode);
               }
               catch (Exception e)
               {
                   AlohaException ae = new AlohaException("Ошибка при отправлении заказа OrderMode = "+ OrderMode.ToString(), e);
                   ae.AlohaErrorDescription = (new CAlohaErrors(e.Message)).ValStr;
                   throw ae;
               }
               AlohaFuncs.DeselectAllEntries(Properties.Settings.Default.TerminalNumber);

               if (Achk.DiscId > 0)
               {
                   try
                   {
                       AlohaFuncs.DeleteComp(Properties.Settings.Default.TerminalNumber, Properties.Settings.Default.ManagerNumber, Achk.AlohaId, Achk.DiscId);
                   }
                   catch (Exception e)
                   {
                       AlohaException ae = new AlohaException("Ошибка при удалении старой скидки", e);
                       ae.AlohaErrorDescription = (new CAlohaErrors(e.Message)).ValStr;
                       throw ae;
                   }
               }

               if (Discount > 0)
               {
                   try
                   {
                       AlohaFuncs.ApplyComp(Properties.Settings.Default.TerminalNumber, Properties.Settings.Default.EmployeeNumber, ChId, Discount, 0, "", "");
                   }
                   catch (Exception e)
                   {
                       AlohaException ae = new AlohaException("Ошибка при наложении скидки " , e);
                       ae.AlohaErrorDescription = (new CAlohaErrors(e.Message)).ValStr;
                       throw ae;
                   }
               }
               


       //        AlohaFuncs.LogOut(Properties.Settings.Default.TerminalNumber);


               
           }
           catch (AlohaException e)
           {
               string InnerMess = "";
               if (e.InnerException != null)
               {
                   InnerMess = e.InnerException.Message;
               }
               Utils.ToLog("Ошибка из Алохи  "+e.Message+ " Inner: "+ InnerMess);
               throw e;
           }

           catch (Exception ee)
           {
               Utils.ToLog("Общая ошибка " + ee.Message);
               throw ee;
           }
           finally
           {
               try
               {
                   AlohaFuncs.LogOut(Properties.Settings.Default.TerminalNumber);
                   DisposeAloha();

               }
               catch
               {
               
               }
               Utils.ToLog(string.Format("Завершение AddOrder AlohaCheckId={0}, TableNumId={1}", AlohaCheckId, TableNumId));
           }
       }

        internal static List<ItemExt> GetAllItms()
        {
            InitAloha();
            List<ItemExt> Tmp = new List<ItemExt>();
            IberEnum DbfItms = Depot.GetEnum(FILE_ITM);
            foreach (IberObject DbfItm in DbfItms)
            {
                ItemExt itm = new ItemExt()
                {
                    BarCode = DbfItm.GetLongVal("ID"),
                    Count = 1,
                    Price = (decimal)DbfItm.GetDoubleVal ("PRICE"),
                    LongName = DbfItm.GetStringVal("LONGNAME")
                };

                

                Tmp.Add(itm);
            }
            DisposeAloha();
          
                return Tmp;
        
        }
    }

  [Serializable]
  public class AlohaException : Exception
  {

      private int _errorCode;
      public int ErrorCode
      {
          get
          {
              return _errorCode;
          }
          set
          {
              _errorCode = value;
          }
      
      }



      public string _alohaErrorDescription;

      public string AlohaErrorDescription
      {
          get
          {
              return _alohaErrorDescription;
          }
          set
          {
              _alohaErrorDescription = value;
          }

      }

      
      public AlohaException() { }
      public AlohaException(string message) : base(message) { }
      public AlohaException(string message, Exception inner) : base(message, inner) { }
      protected AlohaException(
        System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context)
          : base(info, context) {

              if (info != null)
              {
                  this._errorCode = int.Parse( info.GetString("ErrorCode"));
              }
      
      }


      public override void GetObjectData(SerializationInfo info, StreamingContext context)
      {
          base.GetObjectData(info, context);

          info.AddValue("ErrorCode", this.ErrorCode);
      }

      
  }
     * */

}
