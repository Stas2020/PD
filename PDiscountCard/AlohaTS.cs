using System;
using System.Collections.Generic;

using System.Text;
using AlohaFOHLib;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;
using System.Globalization;
using System.Linq;
using System.IO;
using System.Windows.Media.Imaging;
using System.Web;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using PDiscountCard.MB;

namespace PDiscountCard
{




    internal class CurentState
    {
        internal int TerminalId;
        internal int WaterId;
        internal ulong CheckId;
        internal ulong TableId;
        internal long TermOfOpen;
        internal bool CompIsAppled = false;
        internal string CompName = "";
        internal int CompId = 0;
        internal decimal CompSumm = 0;
        internal bool PredCheckAndNotManager;
        internal int EmployeePositionCode;
        internal int EmployeeNumberCode;
        internal int CurrentSelectionCount;
        internal int CurrentSelectionEntrieId;
        internal string CheckNum
        {
            get
            {
                ulong Ch1 = CheckId >> 20;
                ulong Ch2 = CheckId & 0xFFFFF;
                ulong Ch3 = Ch1 * 10000 + Ch2;
                return Ch1.ToString() + AlohainiFile.BDate.ToString("ddMMyy") + Ch2.ToString("0000");
            }
        }
        internal DateTime BusinessDate
        {
            get
            {
                return AlohainiFile.BDate;
            }
        }
    }


    static public class AlohaTSClass
    {
        static IberDepot Depot;
        static IberFuncs AlohaFuncs;


        private const int INTERNAL_LOCALSTATE = 720; //текущий статус
        private const int INTERNAL_LOCALSTATE_CUR_CHECK = 722;
        private const int INTERNAL_LOCALSTATE_CUR_EMP = 723;
        private const int INTERNAL_LOCALSTATE_CUR_TABLE = 728;
        private const int INTERNAL_COMPS = 580; // описание компенсаций
        private const int INTERNAL_CATS = 750;
        private const int FILE_CMP = 5;
        private const int FILE_GCI = 9;

        private const int INTERNAL_PAYMENTS = 600;
        private const int INTERNAL_CATS_ITEMIDS = 751;
        private const int INTERNAL_CHECKS_COMPS = 545;
        private const int INTERNAL_CHECKS = 540;
        private const int INTERNAL_CHECKS_PAYMENTS = 543;
        private const int INTERNAL_CHECKS_TIPPABLE_EMP = 541;
        private const int INTERNAL_ITEMS = 740;
        private const int INTERNAL_TERMINALS = 780;
        private const int INTERNAL_EMPLOYEES = 500;
        private const int FILE_QTYPRICE = 106;
        private const int FILE_TAB = 30;
        private const int INTERNAL_CHECKS_ENTRIES = 542;
        private const int FILE_PRT = 22;
        //private const int INTERNAL_LOCALSTATE_CUR_ENTRY = 725;

        private const int INTERNAL_EMP_OPEN_TABLES = 501;
        private const int INTERNAL_ENTRIES = 560;
        private const int FILE_SUB = 28;
        private const int FILE_MNU = 15;
        private const int FILE_ITM = 12;
        private const int INTERNAL_TABLES = 520;
        private const int INTERNAL_TABLES_OPEN_CHECKS = 650;
        private const int INTERNAL_TABLES_CHECKS = 526;
        private const int FILE_MOD = 16;
        private const int FILE_JOB = 13;
        private const int FILE_TDR = 32;
        private const int FILE_BTN = 75;
        private const int FILE_PNL = 76;
        private const int FILE_PCID = 99;

        private const int INTERNAL_ENTRIES_PROMO_DATA = 564;

        private const int INTERNAL_CHECKS_PROMOS = 544;
        private const int INTERNAL_EMP_CLOSED_TABLES = 502;
        private const int INTERNAL_EMP_CLOSED_CHECKS = 506;

        static internal CurentState AlohaCurentState = new CurentState();

        public static System.Timers.Timer T1;

        static AlohaTSClass()
        {
            /*
            T1 = new System.Timers.Timer(5000);
            T1.Elapsed += new System.Timers.ElapsedEventHandler(T1_Elapsed);
            T1.Start();
            */

        }


        static public DateTime AlohaTime(string T)
        {
            try
            {
                DateTime dt1 = new DateTime(1899, 12, 31);
                DateTime Tmp = new DateTime(dt1.Ticks + Convert.ToInt64(T) * 10000000, DateTimeKind.Local);
                Tmp.AddHours(4);
                return Tmp;

            }
            catch
            {
                return DateTime.Now;
            }
        }


        static internal List<RemoteOrderSrv.OrderInfoForAloha> RemoteOrders = new List<PDiscountCard.RemoteOrderSrv.OrderInfoForAloha>();
        static internal List<OrderInfo> RemoteOrders2 = new List<OrderInfo>();
        static internal List<OrderInfo> NotOrderRemoteOrders = new List<OrderInfo>();


        static int TortSubMenuId = 18;
        //static int TortSubMenuId = 298;


        static public List<int> GetExDisc()
        {
            List<int> Tmp = new List<int>() { 8, 53 };
            return Tmp;
        }

        static public List<int> GetExDishez()
        {
            List<int> Tmp = new List<int>();
            Tmp.AddRange(AlcList);
            Tmp.AddRange(TortList);
            //Подарочные карты
            Tmp.Add(999903);
            Tmp.Add(999905);
            Tmp.Add(999910);
            Tmp.Add(999803);
            Tmp.Add(999805);
            Tmp.Add(999810);

            return Tmp;
        }


        public static List<int> AlcList = new List<int>();
        static public void InitAlc()
        {
            AlcList.Clear();
            AlcList.AddRange(GetDishesInCat(19));
            AlcList.AddRange(GetDishesInCat(20));
            AlcList.AddRange(GetDishesInCat(21));
            AlcList.AddRange(GetDishesInCat(32));
            AlcList.AddRange(GetDishesInCat(7));
        }



        private static List<int> TortList = new List<int>();
        static public void InitTorts()
        {
            try
            {
                TortList.Add(2227);
                /*
                IberEnum MySubMnus = Depot.GetEnum(28);
                IberObject MySubMnu = MySubMnus.FindFromLongAttr("ID", TortSubMenuId);

                for (int i = 1; i < 49; i++)
                {
                    try
                    {
                        int DNum = MySubMnu.GetLongVal("ITEM" + i.ToString("00"));
                        if (DNum > 0)
                        {
                            TortList.Add(DNum);
                        }
                    }
                    catch (Exception e)
                    {
                    }
                }
                 * */
            }
            catch (Exception ee)
            {
                Utils.ToCardLog("Error init torts " + ee.Message);
            }
        }

        /// <summary>
        /// Удаление всех вхождений одного блюда из стола
        /// </summary>
        /// <param name="itmBarcode">Баркод блюда из системы Алоха, он же item Id из itm.dbf</param>
        static internal void TryDeleteItemOnCurentCheck(int itmBarcode)
        {
            try
            {
                Utils.ToCardLog($"TryDeleteItemOnCurentCheck Start - #{itmBarcode}");

                Check chk = GetCurentCheck();

                if (chk.Dishez.Any(d => d.BarCode == itmBarcode))
                {
                    AlohaFuncs.DeselectAllEntries(AlohaCurentState.TerminalId);

                    foreach (var dish in chk.Dishez.Where(d => d.BarCode == itmBarcode).ToList())
                    {
                        AlohaFuncs.SelectEntry(AlohaCurentState.TerminalId, (int)AlohaCurentState.CheckId, dish.AlohaNum);
                    }
                    int pass = Config.ConfigSettings.ManagerPass;
                    AlohaFuncs.AuthorizeOverrideMgr(AlohaCurentState.TerminalId, 99921, pass.ToString(), "");
                    AlohaFuncs.VoidSelectedItems(AlohaCurentState.TerminalId, (int)AlohaCurentState.CheckId, 0);
                    AlohaFuncs.DeselectAllEntries(AlohaCurentState.TerminalId);
                    Utils.ToCardLog($"TryDeleteItemOnCurentCheck Ok - #{itmBarcode}");
                }
                else
                {
                    Utils.ToCardLog($"TryDeleteItemOnCurentCheck No item - #{itmBarcode}");
                }

            }
            catch (Exception e)
            {
                Utils.ToCardLog($"[Error] TryDeleteItemOnCurentCheck #{itmBarcode} " + e.Message);

            }
        }

        static internal void DeleteAllItemsOnCurentCheckandClose2(int TermNum, int EmpNum, int CheckId)
        {
            try
            {
                AlohaFuncs.DeselectAllEntries(TermNum);
                AlohaFuncs.SelectAllEntriesOnCheck(TermNum, CheckId);
                // AlohaFuncs.AuthorizeOverrideMgr(AlohaCurentState.TerminalId
                //AlohaFuncs.AuthorizeOverrideMgr(TermNum, EmpNum, "", "");
                //AlohaFuncs.ManagerVoidSelectedItems(AlohaCurentState.TerminalId,9267, (int)AlohaCurentState.CheckId, 0);
                AlohaFuncs.VoidSelectedItems(TermNum, CheckId, 0);
                //AlohaFuncs.AuthorizeOverrideMgr(TermNum, EmpNum, "", "");
                AlohaFuncs.CloseCheck(TermNum, CheckId);


                Utils.ToCardLog("DeleteItemsAndCloseCheck2 Ok");

            }
            catch (Exception e)
            {
                Utils.ToCardLog("[Error] DeleteItemsAndCloseCheck2 " + e.Message);

            }
        }

        static internal bool DeleteCardEntry(int CheckId, int EntryId)
        {
            try
            {
                Utils.ToCardLog("DeleteCardEntry");

                AlohaFuncs.VoidItem(GetTermNum(), CheckId, EntryId, 1);
                AlohaFuncs.RefreshCheckDisplay();
                Utils.ToCardLog("Sucsess");
                return true;
            }
            catch (Exception e)
            {

                Utils.ToCardLog("[Error] DeleteCardEntry " + e.Message);
                return false;
            }

        }



        static internal void DeleteAllItemsOnCurentCheckandClose()
        {
            try
            {
                if (CheckWindow())
                {
                    try
                    {
                        AlohaFuncs.ClockIn(GetTermNum(), 9267);

                    }
                    catch (Exception ee)
                    {
                        Utils.ToCardLog("[Error] DeleteItemsAndCloseCheck " + ee.Message);
                    }
                    try
                    {

                        AlohaFuncs.LogIn(GetTermNum(), 9267, "", "");
                    }
                    catch (Exception ee)
                    {
                        Utils.ToCardLog("[Error] DeleteItemsAndCloseCheck " + ee.Message);
                    }
                    AlohaFuncs.DeselectAllEntries(AlohaCurentState.TerminalId);
                    AlohaFuncs.SelectAllEntriesOnCheck(AlohaCurentState.TerminalId, (int)AlohaCurentState.CheckId);
                    // AlohaFuncs.AuthorizeOverrideMgr(AlohaCurentState.TerminalId
                    AlohaFuncs.AuthorizeOverrideMgr(AlohaCurentState.TerminalId, 9267, "", "");
                    //AlohaFuncs.ManagerVoidSelectedItems(AlohaCurentState.TerminalId,9267, (int)AlohaCurentState.CheckId, 0);
                    AlohaFuncs.VoidSelectedItems(AlohaCurentState.TerminalId, (int)AlohaCurentState.CheckId, 0);
                    AlohaFuncs.AuthorizeOverrideMgr(AlohaCurentState.TerminalId, 9267, "", "");
                    AlohaFuncs.CloseCheck(AlohaCurentState.TerminalId, (int)AlohaCurentState.CheckId);
                    AlohaFuncs.RefreshCheckDisplay();

                    Utils.ToCardLog("DeleteItemsAndCloseCheck Ok");
                }
            }
            catch (Exception e)
            {
                Utils.ToCardLog("[Error] DeleteItemsAndCloseCheck " + e.Message);

            }


        }

        static public bool DishIsTort(int DishID)
        {
            return TortList.Contains(DishID);

        }

        //private  static const int INTERNAL_LOCALSTATE_ITEMINFOS=731;
        private static int INTERNAL_CURRENT_LOCALSTATE = 800;
        static public void AddReassonToDeleteItems(int CheckId, int ReasonId)
        {
            SetDeleteReasonAttr(CheckId, ReasonId);


            /*
            try
            {
                
            IberEnumClass LocaleStateEnum = (IberEnumClass)Depot.GetEnum(INTERNAL_LOCALSTATE);
            IberObjectClass LocaleState = (IberObjectClass)LocaleStateEnum.First();
                
                IberEnumClass LocaleStateEnum = (IberEnumClass)Depot.GetEnum(INTERNAL_CURRENT_LOCALSTATE);
                IberObjectClass LocaleState = (IberObjectClass)LocaleStateEnum.First();
                IberEnumClass CurStateEnumItemInfos = (IberEnumClass)LocaleState.GetEnum(INTERNAL_LOCALSTATE_CUR_ENTRY);
            IberObjectClass CurStateEnumItemInfo = (IberObjectClass)CurStateEnumItemInfos.First();
                
            IberEnumClass CurStateEnumItemInfos = (IberEnumClass)LocaleState.GetEnum(INTERNAL_LOCALSTATE_ITEMINFOS);
            IberObjectClass CurStateEnumItemInfo = (IberObjectClass)CurStateEnumItemInfos.First();
            //IberObjectClass CurStateCheck = (IberObjectClass)CurStateEnumChecks.First();
            //int ChId = CurStateCheck.GetLongVal("ID");
                
            int Id = CurStateEnumItemInfo.GetLongVal ("ID");
            foreach (IberObjectClass ItemInfo in CurStateEnumItemInfos)
            { 
                int Id2 = ItemInfo.GetLongVal ("ID");
                SetDeleteReasonAttr(Id, ReasonId);

            
            }
            }
            catch(Exception e)
            {
                Utils.ToCardLog("Error AddReassonToDeleteItems "+e.Message);
            }
                */


        }
        /*
        static internal void LogInAllClockedInEmployees()
        {
            foreach (int k in GetClockedInWaitersList())
            {
                try
                {
                    AlohaFuncs.LogIn(Utils.GetTermNum(), k, "", "");
                    AlohaFuncs.LogOut(Utils.GetTermNum());

                    Utils.ToCardLog("LogInAllClockedInEmployees for num " + k.ToString());
                }
                catch (Exception e)
                {
                    Utils.ToCardLog("[Error] LogInAllClockedInEmployees for num " + k.ToString() + " " + e.Message);
                }

            }
        }
        */
        static int CurentMemberCheck = 0;
        static internal int GetAssingMemberState(int termid)
        {
            int i = -1;
            try
            {
                i = AlohaFuncs.GetEFreqStatus(termid, CurentMemberCheck);

            }
            catch (Exception e)
            {
                Utils.ToLog("[Error] GetAssingMemberState " + e.Message);
            }
            return i;

        }

        static internal bool AssingMember(int termid, int Empl, int TableId, int CheckId, string memberId, string TrInfo,
            int MgrId1, int MgrId2, int MgrId3, out string ErrorMsg)
        {
            ErrorMsg = "";
            try
            {
                /*
                if (!AlohaFuncs.IsTableService()) 
                {
                    Utils.ToLog("Это QS : TableId=0" );
                    TableId = 0;
                }
                 * */
                AlohaFuncs.AssignEFreqMember(termid, Empl, TableId, CheckId, memberId, TrInfo,
                    MgrId1, MgrId2, MgrId3);
                CurentMemberCheck = CheckId;
                AlohaTSClass.ShowMessage("Запрос отправлен на сервер.");
                return true;
            }
            catch (Exception e)
            {
                EventSenderClass.SendAlohaAsincEvent(StopListService.AlohaEventType.LoyaltyOtherError, ErrorMsg, AlohaTSClass.AlohaCurentState.WaterId,
                    AlohaTSClass.GetJobCode(AlohaTSClass.AlohaCurentState.WaterId),
                    MainClass.CurentCadrNumber,
                    0,
                    (int)AlohaTSClass.AlohaCurentState.TableId,
                    (int)AlohaTSClass.AlohaCurentState.CheckId);
                ErrorMsg = e.Message;
                Utils.ToCardLog("[Error AssignEFreqMember ]  " + e.Message);
                return false;
            }
        }

        static internal bool IsManager(int EmpNum)
        {
            try
            {
                IberObject IntITM = Depot.FindObjectFromId(INTERNAL_EMPLOYEES, Depot.GetIdFromUserNumber(INTERNAL_EMPLOYEES, EmpNum)).First();
                return (Convert.ToBoolean(IntITM.GetLongVal("JOBCODE1") == 10));
            }
            catch
            {
                return false;
            }
        }

        static internal List<CEmpl> GetManagersList()
        {
            try
            {
                List<CEmpl> Tmp = new List<CEmpl>();
                IberEnum Empl = new IberEnum();
                Empl = Depot.GetEnum(INTERNAL_EMPLOYEES);
                foreach (IberObject IntITM in Empl)
                {
                    if (!Convert.ToBoolean(IntITM.GetBoolVal("TERMINATED")))
                        if (Convert.ToBoolean(IntITM.GetLongVal("JOBCODE1") == 10))
                        {
                            CEmpl emp = new CEmpl()
                            {
                                Id = IntITM.GetLongVal("USERNUMBER"),
                                Name = IntITM.GetStringVal("FIRSTNAME") + " " + IntITM.GetStringVal("LASTNAME")
                            };
                            Tmp.Add(emp);

                        }
                }
                return Tmp;
            }
            catch (Exception e)
            {
                Utils.ToCardLog(e.Message);
                return null;
            }
        }


        static private List<int> GetClockedInWaitersList()
        {
            try
            {
                List<int> Tmp = new List<int>();

                IberEnum Empl = new IberEnum();
                Empl = Depot.GetEnum(INTERNAL_EMPLOYEES);
                foreach (IberObject IntITM in Empl)
                {
                    if (!Convert.ToBoolean(IntITM.GetBoolVal("TERMINATED")))
                        if (Convert.ToBoolean(IntITM.GetBoolVal("CLOCKED_IN")))
                        {
                            Tmp.Add(IntITM.GetLongVal("USERNUMBER"));
                            Utils.ToCardLog("Добавляю " + IntITM.GetLongVal("USERNUMBER"));
                        }
                }
                return Tmp;
            }
            catch (Exception e)
            {
                Utils.ToCardLog(e.Message);
                return null;
            }
        }

        static internal string GetDisplayBoardInfo(int CheckNum, int EntryId, out string Price, out double QUANTITY)
        {
            IberEnum Dishez = new IberEnum();
            IberObject MyCheck = Depot.FindObjectFromId(INTERNAL_CHECKS, CheckNum).First();
            Price = "0.00";
            QUANTITY = 0;
            try
            {
                Dishez = MyCheck.GetEnum(INTERNAL_CHECKS_ENTRIES);
                IberObject IntITM = Dishez.FindFromLongAttr("ID", EntryId);
                Price = IntITM.GetStringVal("DISP_PRICE");

                QUANTITY = IntITM.GetLongVal("QUANTITY");

                return IntITM.GetStringVal("DISP_NAME");
            }
            catch
            {
                return "";
            }
        }


        static public List<int> DishInSMnu(int SmnuId)
        {
            IberEnumClass SmnuEnum = (IberEnumClass)Depot.GetEnum(FILE_SUB);

            IberObjectClass Smnu = (IberObjectClass)SmnuEnum.FindFromLongAttr("ID", SmnuId);
            List<int> Tmp = new List<int>();
            for (int i = 0; i < 49; i++)
            {
                try
                {
                    int Num = Smnu.GetLongVal("ITEM" + i.ToString("00"));
                    Tmp.Add(Num);

                }
                catch
                {

                }

            }
            return Tmp;
        }

        static private bool DishInDiscount(int Barcode, int DId)
        {
            try
            {
                IberEnumClass CompsEnum = (IberEnumClass)Depot.GetEnum(FILE_CMP);

                IberObjectClass Comp = (IberObjectClass)CompsEnum.FindFromLongAttr("ID", DId);
                int CatId = Comp.GetLongVal("ITEMCAT");
                IberEnumClass CatsEnum = (IberEnumClass)Depot.FindObjectFromId(INTERNAL_CATS, CatId);
                IberObjectClass Cats = (IberObjectClass)CatsEnum.First();
                IberEnumClass CatItemsEnum = (IberEnumClass)Cats.GetEnum(INTERNAL_CATS_ITEMIDS);
                try
                {
                    CatItemsEnum.FindFromLongAttr("ID", Barcode);
                    return true;
                }
                catch
                { }
                return false;

            }
            catch
            {
                return false;
            }

        }

        static internal void SetWaterToCurentCheck(int WNum)
        {
            CheckWindow();
            SetWaterToCurentCheck(WNum, (int)AlohaCurentState.CheckId);
        }

        static internal void SetWaterToCurentCheck(int WNum, int CheckId)
        {
            try
            {
                //CheckWindow();


                string Tm = "";
                try
                {
                    Tm = AlohaFuncs.GetObjectAttribute(INTERNAL_CHECKS, CheckId, "WtOb");

                }
                catch
                {

                }
                if (Tm.Trim() == "")
                {
                    AlohaFuncs.SetObjectAttribute(INTERNAL_CHECKS, CheckId, "WtOb", WNum.ToString());
                    Utils.ToLog("SetObjectAttribute " + WNum.ToString() + " на чек " + CheckId);
                    Tm = AlohaFuncs.GetObjectAttribute(INTERNAL_CHECKS, CheckId, "WtOb");
                    Utils.ToLog("GetObjectAttribute " + Tm + " на чек " + CheckId);
                }
                else
                {
                    Utils.ToCardLog("Атрибут " + Tm + " уже наложен");
                }

            }
            catch (Exception e)
            {
                Utils.ToCardLog("[Error]чек SetWaterToCurentCheck(int WNum) WNum = " + WNum + " message: " + e.Message);
            }
        }

        static private string GetDemSep()
        {
            double d = 1.1;
            return d.ToString()[1].ToString();
        }

        static internal List<Dish> GetDishesOfCheck(int CheckNum, Check Ch, decimal DiscAmount)
        {
            List<Dish> Tmp = new List<Dish>();
            Ch.ASVCards = new List<string>();
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

            int CheckDiscCat = 0;
            double CheckDiscValue = 0;
            try
            {
                IberEnumClass CompsEnum = (IberEnumClass)MyCheck.GetEnum(INTERNAL_CHECKS_COMPS);
                IberObjectClass Comp = (IberObjectClass)CompsEnum.First();
                CheckDiscCat = Comp.GetLongVal("COMPTYPE_ID");

            }
            catch
            {

            }

            string SpecMess = "";
            Dish CurDish = new Dish();
            Dish CurMod = new Dish();

            foreach (IberObject IntITM in Dishez)
            {

                try { Utils.ToLog("Id: " + IntITM.GetLongVal("ID"), 6); }
                catch { }
                try { Utils.ToLog("Ent: " + IntITM.GetLongVal("TYPE"), 6); }
                catch { }

                try { Utils.ToLog("MOD_CODE: " + (IntITM.GetLongVal("MOD_CODE")), 6); }
                catch { }

                try { Utils.ToLog("DATA: " + IntITM.GetLongVal("DATA"), 6); }
                catch { }

                try { Utils.ToLog("DISP_NAME: " + IntITM.GetStringVal("DISP_NAME"), 6); }
                catch { }

                try
                {
                    Utils.ToLog("Ent N: " + IntITM.GetStringVal("DISP_NAME"), 6);
                    Utils.ToLog("Ent PR: " + IntITM.GetStringVal("DISP_PRICE"), 6);
                    Utils.ToLog("MOD_STRING: " + IntITM.GetStringVal("MOD_STRING"), 6);
                }
                catch { }

                try
                {
                    if (IntITM.GetLongVal("TYPE") == 52)
                    {
                        Ch.LoyaltyCard = IntITM.GetStringVal("DISP_NAME");
                    }
                }
                catch { }

                try
                {
                    if (IntITM.GetLongVal("TYPE") == 164)
                    {
                        Ch.ASVCard = IntITM.GetStringVal("DISP_NAME");
                        Ch.ASVCards.Add(IntITM.GetStringVal("DISP_NAME"));
                    }
                }
                catch { }

                try
                {
                    if (IntITM.GetLongVal("TYPE") == 163)
                    {
                        string str = IntITM.GetStringVal("DISP_NAME");
                        if (str.Contains("Remaining Balance:"))
                        {
                            Ch.ASVCardBalance = Convert.ToDecimal(str.Substring(18).Replace(".", GetDemSep()).Replace(".", GetDemSep()));
                        }
                    }
                }
                catch { }

                try
                {
                    if (IntITM.GetLongVal("TYPE") == 129) //Это надбавка
                    {
                        Ch.ServiceChargeName = IntITM.GetStringVal("DISP_NAME");
                        Ch.ServiceChargeSumm = Convert.ToDecimal(IntITM.GetDoubleVal("PRICE"));
                    }
                }
                catch { }


                //(IntITM.GetLongVal("MOD_CODE")!=8 &&(IntITM.GetLongVal("MOD_CODE")!=12) - Это отмены
                if (((IntITM.GetLongVal("TYPE") == 0) || (IntITM.GetLongVal("TYPE") == 6)) && ((IntITM.GetLongVal("MOD_CODE") != 8 && (IntITM.GetLongVal("MOD_CODE") != 12))))
                //Regular item                          MOD_DELETED                             MOD_PRINTED_DELETED
                {

                    if (IntITM.GetLongVal("LEVEL") > 0)  //Модификатор
                    {

                        //if (Math.Abs(IntITM.GetDoubleVal("PRICE")) > 0) 
                        //if ((IntITM.GetLongVal("DATA") < 933000) || (Math.Abs(IntITM.GetDoubleVal("PRICE")) > 0))
                        //{
                        Dish Md = new Dish()
                        {
                            Count = 1,
                            Level = IntITM.GetLongVal("LEVEL"),
                            Name = IntITM.GetStringVal("DISP_NAME"),
                            AlohaNum = IntITM.GetLongVal("ID"),
                            Price = Convert.ToDecimal(IntITM.GetDoubleVal("PRICE")),
                            OPrice = Convert.ToDecimal(IntITM.GetDoubleVal("OPRICE")),
                            QUANTITY = IntITM.GetLongVal("QUANTITY"),
                            BarCode = IntITM.GetLongVal("DATA"),
                            LongName = IntITM.GetStringVal("DISP_LONGNAME"),
                            CHITNAME = IntITM.GetStringVal("DISP_CHITNAME"),
                            //DISP_PRICE = Convert.ToDecimal(IntITM.GetDoubleVal("DISP_PRICE")),

                            Selected = (IntITM.GetBoolVal("SELECTED") == 1),

                            IsOrdered = !((IntITM.GetLongVal("MODE") == 80) || (IntITM.GetLongVal("MODE") == 82))


                        };
                        //        Utils.ToLog(Md.Name + " " + Md.Price + " " + Md.Level);

                        try
                        {
                            string s = IntITM.GetStringVal("DISP_PRICE").Replace(".", GetDemSep()).Replace(",", GetDemSep());
                            Md.DISP_PRICE = Convert.ToDecimal(s);
                        }
                        catch
                        {
                            Md.DISP_PRICE = 0;
                        }

                        if (IntITM.GetLongVal("LEVEL") == 1)
                        {
                            CurMod = Md;
                            CurDish.CurentModificators.Add(Md);
                        }
                        else
                        {
                            CurMod.CurentModificators.Add(Md);
                        }

                        if ((IntITM.GetLongVal("DATA") < iniFile.SpoolMaxDish) || (Math.Abs(IntITM.GetDoubleVal("PRICE")) > 0))
                        {
                            Tmp.Add(Md);
                        }

                        //}
                    }
                    else //Блюдо
                    {

                        Dish mD = new Dish()
                        {
                            Count = 1,
                            AlohaNum = IntITM.GetLongVal("ID"),
                            Level = IntITM.GetLongVal("LEVEL"),
                            Name = IntITM.GetStringVal("DISP_NAME"),
                            //Name = IntITM.GetStringVal("DISP_CHITNAME"),
                            Price = Convert.ToDecimal(IntITM.GetDoubleVal("PRICE")),
                            OPrice = Convert.ToDecimal(IntITM.GetDoubleVal("OPRICE")),
                            QUANTITY = IntITM.GetLongVal("QUANTITY"),
                            BarCode = IntITM.GetLongVal("DATA"),
                            LongName = IntITM.GetStringVal("DISP_LONGNAME"),
                            IsOrdered = !((IntITM.GetLongVal("MODE") == 80) || (IntITM.GetLongVal("MODE") == 82)),
                            Selected = (IntITM.GetBoolVal("SELECTED") == 1),
                            CHITNAME = IntITM.GetStringVal("DISP_CHITNAME"),

                            //DISP_PRICE = Convert.ToDecimal(IntITM.GetDoubleVal("DISP_PRICE")),
                            //UnikDishNum = IntITM.GetLongVal("ID"),
                            //QtyCount = IntITM.GetLongVal("QUANTITY"),
                            // LinkedObject = IntITM 

                            //DISP_NAME
                        };
                        try
                        {
                            string s = IntITM.GetStringVal("DISP_PRICE").Replace(".", GetDemSep()).Replace(",", GetDemSep());
                            mD.DISP_PRICE = Convert.ToDecimal(s);
                        }
                        catch
                        {
                            mD.DISP_PRICE = 0;
                        }



                        CurDish = mD;
                        try
                        {
                            if (mD.BarCode == 999901)
                            {
                                CheckDiscCat = Convert.ToInt32(AlohaFuncs.GetObjectAttribute(INTERNAL_ENTRIES, IntITM.GetLongVal("ID"), "CompId"));
                                Ch.CompId = CheckDiscCat;
                            }
                        }
                        catch
                        { }

                        Tmp.Add(mD);
                    }
                }
                else if (IntITM.GetLongVal("TYPE") == 1) //- Это Special modifier message
                {
                    if (IntITM.GetLongVal("LEVEL") > 0)
                    {

                        Dish Md = new Dish()
                        {
                            Count = 1,
                            AlohaNum = IntITM.GetLongVal("ID"),
                            Level = IntITM.GetLongVal("LEVEL"),
                            Name = IntITM.GetStringVal("DISP_LONGNAME"),
                            Price = Convert.ToDecimal(IntITM.GetDoubleVal("PRICE")),
                            OPrice = Convert.ToDecimal(IntITM.GetDoubleVal("OPRICE")),
                            QUANTITY = IntITM.GetLongVal("QUANTITY"),
                            CHITNAME = IntITM.GetStringVal("DISP_CHITNAME"),

                            BarCode = -1
                        };
                        Utils.ToLog(Md.Name + " " + Md.Price + " " + Md.Level);
                        if (IntITM.GetDoubleVal("PRICE") > 0)
                        {
                            Tmp.Add(Md);
                        }


                    }
                }
            }

            //пытаемся дать LongName и весовое количество
            foreach (Dish d in Tmp)
            {
                try
                {
                    string unname = "";
                    int Vozvr = (Ch.Vozvr) ? (-1) : 1;
                    double qPrice = DishIsQty(d.BarCode, out unname);
                    if (qPrice != -1)
                    {
                        qPrice = qPrice * Vozvr;
                        if (d.OPrice == 0)
                        {
                            d.QtyQUANTITY = 0;
                            d.Priceone = (double)d.Price;
                        }
                        else
                        {
                            d.QtyQUANTITY = (decimal)(d.OPrice / (decimal)qPrice);
                            d.Priceone = (double)d.Price / (double)d.QtyQUANTITY;
                        }
                        d.OPriceone = qPrice;
                    }
                    else
                    {
                        d.OPriceone = (double)d.OPrice;
                        d.Priceone = (double)d.Price;
                    }
                }
                catch
                {

                }
            }



            // Разбираемся с дискаунтным прайсом

            if (Tmp.Count() > 0)
            {
                if (DiscAmount > 0)
                {
                    if (iniFile.FRPriceFromDisplay)
                    {

                        decimal DiscPrecent = DiscAmount / Tmp.Sum(a => a.DISP_PRICE);
                        decimal DiscAmountAlreadySumm = 0;
                        Utils.ToCardLog("DiscAmount > 0 " + DiscAmount + "  DiscPrecent  " + DiscPrecent);
                        Dish DMaxPrice = Tmp.First();
                        foreach (Dish d in Tmp)
                        {
                            Utils.ToCardLog("Correct DiscAmount > 0 Old Price" + d.LongName + "  " + d.DISP_PRICE);
                            decimal Count = d.Count * d.QUANTITY * d.QtyQUANTITY;
                            decimal MaxDiscSumm = Math.Min(d.DISP_PRICE, Math.Round(d.DISP_PRICE * DiscPrecent, 2));
                            MaxDiscSumm = Math.Min(MaxDiscSumm, DiscAmount - DiscAmountAlreadySumm);
                            d.DISP_PRICE = d.DISP_PRICE - MaxDiscSumm;
                            d.Price = d.DISP_PRICE;
                            d.Priceone = (double)(d.DISP_PRICE / Count);
                            Utils.ToLog("line 964  Priceone: " + d.Priceone.ToString() + "  DISP_PRICE: " + d.DISP_PRICE.ToString() + "  Count: " + Count.ToString());
                            DiscAmountAlreadySumm += MaxDiscSumm;
                            if (DMaxPrice.DISP_PRICE < d.DISP_PRICE)
                            {
                                DMaxPrice = d;
                            }
                            Utils.ToCardLog("Correct DiscAmount > 0 " + d.LongName + "  " + d.DISP_PRICE);
                        }
                        if (DiscAmountAlreadySumm < DiscAmount)
                        {
                            DMaxPrice.DISP_PRICE = Math.Max(0, DMaxPrice.DISP_PRICE - (DiscAmount - DiscAmountAlreadySumm));
                            DMaxPrice.Price = DMaxPrice.DISP_PRICE;
                            DMaxPrice.Priceone = (double)(DMaxPrice.DISP_PRICE / (DMaxPrice.Count * DMaxPrice.QUANTITY * DMaxPrice.QtyQUANTITY));
                            Utils.ToCardLog("Correct DiscAmount Last  " + DMaxPrice.LongName + "  " + DMaxPrice.DISP_PRICE);
                        }
                    }
                    else
                    {

                        decimal DiscPrecent = DiscAmount / Tmp.Sum(a => a.Price * a.QUANTITY);
                        decimal DiscAmountAlreadySumm = 0;
                        Utils.ToCardLog("DiscAmount > 0 " + DiscAmount + "  DiscPrecent  " + DiscPrecent);
                        Dish DMaxPrice = Tmp.First();
                        foreach (Dish d in Tmp)
                        {
                            Utils.ToCardLog("Correct DiscAmount > 0 Old Price" + d.LongName + "  " + d.Price);
                            decimal MaxDiscSumm = Math.Min(d.Price, Math.Round(d.Price * DiscPrecent, 2));
                            MaxDiscSumm = Math.Min(MaxDiscSumm, DiscAmount - DiscAmountAlreadySumm);
                            //d.Price = d.Price - MaxDiscSumm;
                            //d.Priceone = d.Priceone - (double)MaxDiscSumm;

                            d.Price = (decimal)Math.Round((double)d.OPrice * (1 - (double)DiscPrecent), 2, MidpointRounding.ToEven);
                            d.Priceone = Math.Round((double)d.OPriceone * (1 - (double)DiscPrecent), 2, MidpointRounding.ToEven);
                            Utils.ToLog("line 996  Priceone: " + d.Priceone.ToString() + "  OPriceone: " + d.OPriceone.ToString() + "  DiscPrecent: " + DiscPrecent.ToString());

                            DiscAmountAlreadySumm += MaxDiscSumm;
                            if (DMaxPrice.Price < d.Price)
                            {
                                DMaxPrice = d;
                            }
                            Utils.ToCardLog("Correct DiscAmount > 0 " + d.LongName + "  " + d.Price);
                        }
                        if (DiscAmountAlreadySumm < DiscAmount)
                        {
                            DMaxPrice.Price = Math.Max(0, DMaxPrice.Price - (DiscAmount - DiscAmountAlreadySumm));
                            DMaxPrice.Priceone = (double)(DMaxPrice.Price / (DMaxPrice.Count * DMaxPrice.QUANTITY * DMaxPrice.QtyQUANTITY));

                            Utils.ToCardLog("Correct DiscAmount Last  " + DMaxPrice.LongName + "  " + DMaxPrice.Price);
                        }

                    }
                }
            }

            if (CheckDiscCat > 0)
            {
                /*
                IberEnumClass ItmsEnum = (IberEnumClass)Depot.GetEnum(FILE_ITM);


                IberObject Itm = ItmsEnum.FindFromLongAttr("ID", d.BarCode);
                */

                IberEnumClass CmpsEnum = (IberEnumClass)Depot.GetEnum(FILE_CMP);
                IberObject Cmp = CmpsEnum.FindFromLongAttr("ID", CheckDiscCat);


                //IberObjectClass Cmp = (IberObjectClass)CmpsEnum.First();
                CheckDiscValue = Cmp.GetDoubleVal("RATE");

                double CheckSum__ = GetCheckSum(CheckNum);
                double CheckSum_ = 0;
                bool distribute_discount_all_item = false;
                foreach (Dish d in Tmp)
                {
                    if (DishInDiscount(d.BarCode, CheckDiscCat))
                    {
                        CheckSum_ += (double)(d.OPrice * d.QUANTITY);
                    }
                        
                }

                CheckDiscValue = (double)Ch.Comp / CheckSum_;
                Utils.ToLog("CheckDiscValue: " + CheckDiscValue.ToString());

                if (CheckDiscValue > 1)
                {
                    CheckSum_ = 0;
                    distribute_discount_all_item = true;
                    foreach (Dish d in Tmp)
                    {
                        CheckSum_ += (double)(d.OPrice * d.QUANTITY);
                    }
                    CheckDiscValue = (double)Ch.Comp / CheckSum_;

                }
                Utils.ToLog("CheckDiscValue: " + CheckDiscValue.ToString());

                Utils.ToLog("Размер скидки: " + Ch.Comp.ToString() + "  Сумма чека: " + CheckSum__.ToString() + " Сумма позиций на которые распространяется скидка: " + CheckSum_);

                foreach (Dish d in Tmp)
                {
                    if (d.Price != 0)
                    {
                        if (d.Price == d.OPrice)
                        {
                            if (DishInDiscount(d.BarCode, CheckDiscCat) || distribute_discount_all_item)
                            {
                                d.Price = (decimal)Math.Round((double)d.OPrice * (1 - CheckDiscValue), 2, MidpointRounding.ToEven);
                                d.Priceone = (double)Math.Round(d.OPriceone * (1 - CheckDiscValue), 2, MidpointRounding.ToEven);
                                Utils.ToLog("line 1043  Priceone: " + d.Priceone.ToString() + "  OPriceone: " + d.OPriceone.ToString() + "  CheckDiscValue: " + CheckDiscValue.ToString());

                            }
                        }
                    }

                    //Это новогодняя акция РИБ
                    if (CheckDiscCat == 61)
                    {
                        if (d.Price == 0)
                        {
                            d.OPrice = 0;
                            d.OPriceone = 0;
                        }
                    }


                }

            }

            double CheckSum = GetCheckSum(CheckNum);

            

            // Коррекция разницы суммы по блюдам и суммы чека
            try
            {
                decimal DSumm = 0;
                Dish MaxDish = null;
                decimal MaxSumm = 0;
                foreach (Dish d in Tmp)
                {
                    if (d.BarCode == 999901) continue;

                    if (iniFile.FRPriceFromDisplay)
                    {
                        DSumm += (decimal)Math.Round(d.Price, 2, MidpointRounding.ToEven) + d.ServiceChargeSumm;
                    }
                    else
                    {
                        DSumm += (decimal)Math.Round(d.Priceone * (double)d.Count * (double)d.QUANTITY * (double)d.QtyQUANTITY, 2, MidpointRounding.ToEven) + d.ServiceChargeSumm;

                        Utils.ToLog("line 1084  Priceone: " + d.Priceone.ToString() + "  Count: " + d.Count.ToString() + "  QUANTITY: " + d.QUANTITY.ToString() + "  QtyQUANTITY: " + d.QtyQUANTITY.ToString());
                    }

                    if (Math.Abs(MaxSumm) < Math.Abs(d.Price))
                    {
                         MaxSumm = d.Price;
                        MaxDish = d;
                    }

                }



                double Delta = CheckSum - (double)DSumm;
                

                Utils.ToLog("Delta: " + Delta.ToString() + "  Сумма чека CheckSum: " + CheckSum.ToString() +  "  DSumm: " + DSumm.ToString());


                if (Math.Abs(Delta) >= 0.005)
                {
                    if (Math.Abs(Delta) > 0.05 * CheckSum)
                    {
                        Ch.ServiceChargeSumm = Math.Abs((decimal)Delta);

                    }
                    else
                    {
                        MaxDish.Price = MaxDish.Price + (decimal)Delta;
                        MaxDish.Delta = (decimal)Delta;
                        Utils.ToCardLog("Добавил разницу сумм: " + Delta.ToString() + " к блюду :" + MaxDish.Name);
                    }
                }


            }
            catch (Exception e)
            {
                Utils.ToCardLog("[Error] Коррекция разницы суммы по блюдам и суммы чека " + e.Message);
            }


            // Распределяем наценку
            try
            {
                if (Ch.ServiceChargeSumm > 0)
                {
                    foreach (Dish d in Tmp)
                    {

                        if (d.Equals(Tmp.Where(a => a.Price != 0).Last()))
                        {
                            d.ServiceChargeSumm = Ch.ServiceChargeSumm - Tmp.Sum(a => a.ServiceChargeSumm);
                            break;
                        }
                        else
                        {
                            decimal DSumm = (decimal)Math.Round(d.Priceone * (double)d.Count * (double)d.QUANTITY * (double)d.QtyQUANTITY, 2, MidpointRounding.ToEven);
                            d.ServiceChargeSumm = Math.Round((DSumm / ((decimal)CheckSum - Ch.ServiceChargeSumm)) * Ch.ServiceChargeSumm, 2, MidpointRounding.ToEven);

                        }
                    }
                }
            }
            catch
            {

            }


            return Tmp;
        }


        static internal List<StopListService.DishN> GetAllItms()
        {
            List<StopListService.DishN> tmp = new List<StopListService.DishN>();
            IberEnum Itms = Depot.GetEnum(FILE_ITM);
            foreach (IberObject ob in Itms)
            {
                StopListService.DishN d = new StopListService.DishN()
                {
                    BarCode = ob.GetLongVal("ID"),
                    Name = ob.GetStringVal("LONGNAME")
                };

                tmp.Add(d);
            }
            return tmp;
        }


        static internal decimal GetPriceChangePrice(int PrCh, int Barcode)
        {
            try
            {
                IberEnum PCID = Depot.GetEnum(FILE_PCID);
                foreach (IberObject Pc in PCID)
                {
                    int ID = Pc.GetLongVal("ID");
                    int ITEMID = Pc.GetLongVal("ITEMID");

                    if ((ID == PrCh) && (Barcode == ITEMID))
                    {
                        decimal Pr = Convert.ToDecimal(Pc.GetDoubleVal("PRICE"));
                        Utils.ToCardLog((new StringBuilder().AppendFormat("Нашел цену ChangePrice: ID={0} , Barcode = {1}, Price = {2} ", ID, ITEMID, Pr)).ToString());
                        return Pr;
                    }

                }
                return -1;
            }
            catch (Exception e)
            {
                Utils.ToCardLog("Error GetPriceChangePrice " + e.Message);
                return -1;
            }

        }

        static Dictionary<int, List<int>> Catigories = new Dictionary<int, List<int>>();
        static List<int> SalesCats = new List<int>();
        static List<StopListService.AlohaCategory> AlohaCats = new List<StopListService.AlohaCategory>();



        static private void InitCats()
        {
            try
            {
                IberEnumClass Cit = (IberEnumClass)Depot.GetEnum(INTERNAL_CATS);
                foreach (IberObject Catt in Cit)
                {
                    List<int> Tmp = new List<int>();

                    try
                    {
                        IberEnumClass ItmsInCat = (IberEnumClass)Catt.GetEnum(INTERNAL_CATS_ITEMIDS);
                        foreach (IberObject Ids in ItmsInCat)
                        {
                            Tmp.Add(Ids.GetLongVal("ID"));
                        }
                    }
                    catch
                    { }
                    Catigories.Add(Catt.GetLongVal("ID"), Tmp);

                    if (Catt.GetBoolVal("SALES") == 1)
                    {
                        SalesCats.Add(Catt.GetLongVal("ID"));
                    }

                    StopListService.AlohaCategory Cat = new StopListService.AlohaCategory();
                    Cat.Id = Catt.GetLongVal("ID");
                    Cat.Name = Catt.GetStringVal("NAME");
                    Cat.Dep = AlohainiFile.DepNum;
                    AlohaCats.Add(Cat);
                }
            }
            catch (Exception e)
            {
                Utils.ToCardLog(e.Message);
            }

        }


        //Это для галеры шереметьево
        static List<int> galAlco;
        static public List<int> GalAlco
        {
            get
            {
                if (galAlco == null)
                {
                    galAlco = GetDishesInCat(3);
                }
                return galAlco;
            }
        }

        static private List<int> GetDishesInCat(int CatNum)
        {
            List<int> Tmp = new List<int>();

            try
            {
                IberObject Cit = Depot.FindObjectFromId(INTERNAL_CATS, CatNum).First();
                int CatCount = Cit.GetLongVal("NUM_ITEMS");
                Utils.ToCardLog(String.Format("GetDishesInCat CatNum {0} CatCount {1}", CatNum, CatCount));
                IberEnumClass ItmsInCat = (IberEnumClass)Cit.GetEnum(INTERNAL_CATS_ITEMIDS);
                foreach (IberObject Ids in ItmsInCat)
                {
                    Tmp.Add(Ids.GetLongVal("ID"));
                }
            }
            catch { }
            return Tmp;
        }

        static private List<int> GetDishCats(int BarCode, out int Cat)
        {
            Cat = 0;
            List<int> Tmp = new List<int>();
            foreach (int k in Catigories.Keys)
            {
                List<int> Cats = Catigories[k];
                if (Cats.Contains(BarCode))
                {
                    if (SalesCats.Contains(k))
                    {
                        Cat = k;
                    }
                    else
                    {
                        Tmp.Add(k);
                    }
                }
            }
            return Tmp;


        }

        private static int GetModLikeThis(string ModName, List<StopListService.AlohaMod> mods, int letters)
        {
            if (ModName.Length <= letters)
            {
                return 0;
            }
            foreach (var mod in mods)
            {
                if (ModName.Substring(0, ModName.Length - letters).Trim().ToLower() == mod.Name.Trim().ToLower())
                {
                    return mod.BarCode;
                }
            }
            return 0;
        }


        static internal int GetModificatorOfDishByName(int dishBarCode, string name)
        {
            //  Utils.ToLog("GetModificatorsGropeOfDish " + DishBarCode.ToString (), 0);

            List<StopListService.AlohaModGroupe> Tmp = new List<StopListService.AlohaModGroupe>();
            try
            {
                IberEnum Itms = Depot.GetEnum(FILE_ITM);
                IberObject MyItm = Itms.FindFromLongAttr("ID", dishBarCode);
                string s = "MOD";
                for (int i = 1; i < 9; i++) //Так долго грузится т.к. у каждого блюда есть служ мод
                // for (int i = 1; i < 10; i++) А так не работает
                {
                    string n = i.ToString();
                    int MCode = MyItm.GetLongVal(s + n);
                    if (MCode != 0)
                    {
                        int miModLengft = 5;
                        var mods = GetModificatorsOfModGrope(MCode, out string GroupeName, shortName: false);
                        for (int l = 0; l < name.Length - miModLengft; l++)
                        {
                            var res = GetModLikeThis(name, mods, l);
                            if (res != 0)
                            {
                                return res;
                            }

                        }

                        /*
                        foreach (var mod in GetModificatorsOfModGrope(MCode, out string GroupeName, shortName:false))
                        {
                            if (mod.Name.ToLower().Trim() == name.ToLower().Trim())
                            {
                                return mod.BarCode;
                            }

                        }
                       */
                    }
                }
            }
            catch (Exception e)
            {
                Utils.ToLog("Error GetModificatorsGropeOfDish " + e.Message, 0);
            }
            return -1;
        }



        /*
        static internal List<StopListService.AlohaModGroupe> GetModificatorsGropeOfDish(IberObject Dish)
        {
            int BarCode = Dish.GetLongVal("DATA");
            return GetModificatorsGropeOfDish(BarCode);
        }
        */
        //static internal List<StopListService.AlohaModGroupe> GetModificatorsGropeOfDish(int DishBarCode)
        static internal List<StopListService.AlohaModGroupe> GetModificatorsGropeOfDish(IberObject MyItm)
        {
            //  Utils.ToLog("GetModificatorsGropeOfDish " + DishBarCode.ToString (), 0);

            List<StopListService.AlohaModGroupe> Tmp = new List<StopListService.AlohaModGroupe>();
            try
            {
                //IberEnum Itms = Depot.GetEnum(FILE_ITM);
                // IberObject MyItm = Itms.FindFromLongAttr("ID", DishBarCode);
                string s = "MOD";
                for (int i = 1; i < 9; i++) //Так долго грузится т.к. у каждого блюда есть служ мод
                // for (int i = 1; i < 10; i++) А так не работает
                {
                    string n = i.ToString();
                    int MCode = MyItm.GetLongVal(s + n);
                    if (MCode != 0)
                    {
                        StopListService.AlohaModGroupe Mg = new StopListService.AlohaModGroupe()
                        {
                            Id = MCode
                        };
                        string N = "";
                        Mg.Mods = GetModificatorsOfModGrope(Mg.Id, out N).ToArray();
                        Mg.Name = N;
                        Tmp.Add(Mg);
                    }
                }
            }
            catch (Exception e)
            {
                Utils.ToLog("Error GetModificatorsGropeOfDish " + e.Message, 0);
            }
            return Tmp;
        }


        static public CModificatorsDishCashe ModificatorsDishCashe = new CModificatorsDishCashe();

        static public List<StopListService.AlohaMod> GetModificatorsOfModGrope(int GroupeNum, out string GroupeName, bool shortName = true)
        {
            List<StopListService.AlohaMod> Tmp = new List<StopListService.AlohaMod>();
            string s = "ITEM";
            GroupeName = "";
            try
            {
                ModificatorGroupeCashe Mg = new ModificatorGroupeCashe();
                if (ModificatorsDishCashe.ModsCashe.TryGetValue(GroupeNum, out Mg))
                {
                    GroupeName = Mg.Name;
                    Tmp = Mg.Mods;
                }
                else
                {
                    IberEnum Mods = Depot.GetEnum(FILE_MOD);
                    IberEnum Itms = Depot.GetEnum(FILE_ITM);
                    IberObject Mod = Mods.FindFromLongAttr("ID", GroupeNum);

                    GroupeName = Mod.GetStringVal("SHORTNAME");
                    for (int i = 1; i < 55; i++)
                    {

                        string num = i.ToString();
                        if (num.Length == 1)
                        {
                            num = "0" + num;
                        }

                        int ModCode = Mod.GetLongVal(s + num);

                        if (ModCode != 0)
                        {
                            StopListService.AlohaMod Md = new StopListService.AlohaMod();
                            Md.BarCode = ModCode;
                            //   Md.Name = Itms.FindFromLongAttr("ID", ModCode).GetStringVal("SHORTNAME").Replace(@"\n", " ");
                            // if (ModCode != Program.Setting.WritingModDishBarcode)
                            {
                                StopListService.AlohaMod m = new StopListService.AlohaMod();
                                if (ModificatorsDishCashe.ItemsCashe.TryGetValue(ModCode, out m))
                                {
                                    Md.Name = m.Name;
                                }
                                else
                                {
                                    if (shortName)
                                    {
                                        Md.Name = Itms.FindFromLongAttr("ID", ModCode).GetStringVal("SHORTNAME").Replace(@"\n", " ");
                                    }
                                    else
                                    {
                                        Md.Name = Itms.FindFromLongAttr("ID", ModCode).GetStringVal("LONGNAME").Replace(@"\n", " ");
                                    }
                                    ModificatorsDishCashe.ItemsCashe.Add(Md.BarCode, Md);
                                }
                                Tmp.Add(Md);
                            }
                        }
                    }
                    ModificatorGroupeCashe mg = new ModificatorGroupeCashe();
                    mg.Name = GroupeName;
                    mg.Mods = Tmp;
                    ModificatorsDishCashe.ModsCashe.Add(GroupeNum, mg);

                }
            }
            catch (Exception e)
            {
                // System.Windows.Forms.MessageBox.Show("Error GetModificatorsOfModGrope " + GroupeNum.ToString() + " " + e.Message);
            }
            return Tmp;

            // return null;

        }







        private static bool CoffeeTerminal = true;
        public static StopListService.AlohaMnu CurentAlohaMnu = new StopListService.AlohaMnu();
        static internal StopListService.AlohaMnu GetMenu(int PrCh, int MnuNum)
        {

            StopListService.AlohaMnu Tmp = new StopListService.AlohaMnu();
            IberEnum Itms = Depot.GetEnum(FILE_ITM);
            Tmp.Id = MnuNum;

            Tmp.Dep = AlohainiFile.DepNum;
            mdepNum = Tmp.Dep;

            InitCats();
            Tmp.Cats = AlohaCats.ToArray();

            if (AlohaFuncs.IsTableService())
            {
                //MnuNum = Tmp.Id;

                List<long> SubMnuList = new List<long>();

                IberEnum Mnus = Depot.GetEnum(FILE_MNU);

                IberObject MyMnu = null;
                IberObject CofeMyMnu = null;
                //    MnuNum = 104;
                try
                {
                    MyMnu = Mnus.FindFromLongAttr("ID", MnuNum);
                    if (CoffeeTerminal)
                    {
                        CofeMyMnu = Mnus.FindFromLongAttr("ID", 900);
                    }

                }
                catch (Exception)
                {

                }

                for (int i = 1; i < 99; i++)
                {
                    int sNum = MyMnu.GetLongVal("MENU" + i.ToString("00"));
                    if (sNum > 0)
                    {
                        SubMnuList.Add(sNum);
                    }
                }
                if (CoffeeTerminal)
                {
                    if (CofeMyMnu != null)
                    {
                        for (int i = 1; i < 99; i++)
                        {
                            int sNum = CofeMyMnu.GetLongVal("MENU" + i.ToString("00"));
                            if (sNum > 0)
                            {
                                SubMnuList.Add(sNum);
                            }
                        }
                    }
                }
                Tmp.Name = MyMnu.GetStringVal("LONGNAME");
                IberEnum SubMnus = Depot.GetEnum(FILE_SUB);
                int CurentDishNum = 0;
                int DishCount = SubMnuList.Count;

                Tmp.Smnus = new StopListService.AlohaSMnu[SubMnuList.Count];


                foreach (long k in SubMnuList)
                {


                    IberObject MySubMnu = SubMnus.FindFromLongAttr("ID", (int)k);
                    Tmp.Smnus[CurentDishNum] = new StopListService.AlohaSMnu()
                    {
                        //Dishes = new  StopListService.AlohaDish(),
                        Id = (int)k,
                        Name = MySubMnu.GetStringVal("LONGNAME").Replace(@"\n", " ").ToLower(),
                        Dep = Tmp.Dep

                    };

                    List<StopListService.AlohaDish> DishTmp = new List<StopListService.AlohaDish>();

                    for (int i = 1; i < 49; i++)
                    {
                        try
                        {
                            int DNum = MySubMnu.GetLongVal("ITEM" + i.ToString("00"));
                            if (DNum > 0)
                            {
                                IberObject MyItm = Itms.FindFromLongAttr("ID", DNum);
                                decimal mPrice = 0;
                                int Barcode = MyItm.GetLongVal("ID");
                                if (!DishIsQty(Barcode, out mPrice))
                                {
                                    int PrType = MySubMnu.GetLongVal("PRMETHOD" + i.ToString("00"));
                                    if (PrType == 1)
                                    {
                                        mPrice = GetPriceChangePrice(PrCh, Barcode);
                                        if (mPrice == -1)
                                        {
                                            mPrice = (decimal)MyItm.GetDoubleVal("PRICE");
                                        }
                                    }
                                    else if (PrType == 0)
                                    {
                                        mPrice = (decimal)MySubMnu.GetDoubleVal("PRICE" + i.ToString("00"));
                                    }
                                }


                                StopListService.AlohaDish D = new StopListService.AlohaDish()
                                {
                                    BarCode = Barcode,
                                    delay = MyItm.GetLongVal("DELAYTIME"),
                                    Dep = Tmp.Dep,
                                    Price = mPrice,
                                    Name = MyItm.GetStringVal("LONGNAME"),
                                    EngName = MyItm.GetStringVal("SKU")
                                    //Name = MyItm.GetStringVal("SHORTNAME").Replace(@"\n", " ").ToLower()
                                };

                                // D.ModGroups = GetModificatorsGropeOfDish(MyItm).ToArray();

                                string En = MyItm.GetStringVal("SKU2");
                                if (En != "")
                                {
                                    D.EngName = D.EngName + " " + En;
                                }



                                int Cat = 0;
                                D.Categories = GetDishCats(D.BarCode, out Cat).ToArray();
                                D.OneCategory = Cat;


                                DishTmp.Add(D);
                            }
                        }

                        catch (Exception e)
                        {
                            Utils.ToLog("[Error] [GetMenu] Ошибка получения блюда в подменю " + k.ToString() + " " + e.Message, 0);
                        }
                    }


                    Utils.ToCardLog("Сформировал подменю " + Tmp.Dep + " " + Tmp.Id + " " + k.ToString());

                    Tmp.Smnus[CurentDishNum].Dishes = DishTmp.ToArray();



                    CurentDishNum++;


                }
                CurentAlohaMnu = Tmp;
                return Tmp;

            }
            else
            {
                Utils.ToCardLog("QS");
                SubMenuPnls = new List<int>();
                Tmp.Name = AlohainiFile.UNITNAME;

                List<int> MnuList = new List<int>();
                MnuList.AddRange(GetOtherQsMnuPnls(1));
                MnuList.AddRange(GetOtherQsMnuPnls(2));
                MnuList.AddRange(GetOtherQsMnuPnls(4));


                IberEnum Btns = Depot.GetEnum(FILE_BTN);

                Dictionary<int, string> MnuPnls = new Dictionary<int, string>();

                //Tmp.Smnus = new StopListService.AlohaSMnu[99];
                List<StopListService.AlohaSMnu> SMnuList = new List<StopListService.AlohaSMnu>();
                int CurrentSmnu = 0;
                foreach (IberObject Btn in Btns)
                {
                    try
                    {
                        if (MnuList.Contains(Btn.GetLongVal("PANELID")))
                        {
                            if (Btn.GetLongVal("FUNC") == 19)
                            {
                                string BtnName = Btn.GetStringVal("TEXT").Replace(@"\n", " ").ToLower();
                                if ((BtnName.Replace(" ", "").ToLower() == "далее") || ((BtnName.Replace(" ", "").ToLower() == "назад")) || ((BtnName.Replace(" ", "").ToLower() == "next")))
                                {
                                    continue;
                                }
                                string Params = Btn.GetStringVal("PARAMS");
                                int MId = Convert.ToInt32(Params.Split(","[0])[5]);
                                StopListService.AlohaSMnu Sm = new StopListService.AlohaSMnu()

                                {
                                    //ListOfDish = new SerializableDictionary<int, AlohaDish>(),
                                    Id = MId,
                                    Name = Btn.GetStringVal("TEXT").Replace(@"\n", " ").ToLower(),
                                    Dep = Tmp.Dep
                                };

                                Sm.Dishes = new StopListService.AlohaDish[99];
                                Utils.ToCardLog("Нашел подменю " + Sm.Name + " " + Sm.Id);

                                SMnuList.Add(Sm);
                                //Tmp.Smnus[CurrentSmnu] = Sm;

                                CurrentSmnu++;

                                List<StopListService.AlohaDish> DishTmp = GetQSDishesOfPanel(MId, PrCh);
                                Sm.Dishes = DishTmp.ToArray();
                                /*
                                int ii = 0;
                                foreach (StopListService.AlohaDish Ad in DishTmp)
                                {
                                    Ad.Dep = Tmp.Dep; 
                                    Sm.Dishes[ii] = Ad;
                                    
                                    ii++;
                                }
                                 * */
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Utils.ToCardLog("Error MnuList.Contains(Btn.GetLongVal(PANELID))) " + e.Message);
                    }
                }

                Tmp.Smnus = SMnuList.ToArray();
                Utils.ToCardLog("Сформировал меню " + Tmp.Dep + " " + Tmp.Id + " кол-во подменю " + Tmp.Smnus.Count());
                CurentAlohaMnu = Tmp;
                return Tmp;
            }



        }


        static private int mdepNum = 0;
        static private List<int> GetOtherQsMnuPnls(int FirstPnlNum)
        {
            Utils.ToLog("GetOtherQsMnuPnls " + FirstPnlNum);

            List<int> Tmp = new List<int>();
            Tmp.Add(FirstPnlNum);
            IberEnum Btns = Depot.GetEnum(FILE_BTN);
            foreach (IberObject Btn2 in Btns)
            {
                if (Btn2.GetLongVal("FUNC") == 19)
                {
                    if (Btn2.GetLongVal("PANELID") == FirstPnlNum)
                    {
                        if ((Btn2.GetStringVal("TEXT").ToLower() == "далее") || (Btn2.GetStringVal("TEXT").ToLower() == "next"))
                        {
                            string Params2 = Btn2.GetStringVal("PARAMS");
                            int Pan2Id = Convert.ToInt32(Params2.Split(","[0])[5]);
                            if (!Tmp.Contains(Pan2Id))
                            {

                                //Tmp.Add(Pan2Id);
                                Utils.ToLog("нашел панель меню " + Pan2Id);
                                Tmp.AddRange(GetOtherQsMnuPnls(Pan2Id));
                            }
                        }
                    }
                }

            }
            return Tmp;

        }


        static internal string GetDishName(int BC)
        {
            IberEnum Itms = Depot.GetEnum(FILE_ITM);
            foreach (IberObject itm in Itms)
            {
                if (itm.GetLongVal("ID") == BC)
                {
                    return itm.GetStringVal("LONGNAME");
                }
            }
            return "";
        }

        static private List<int> SubMenuPnls = new List<int>();
        static private List<StopListService.AlohaDish> GetQSDishesOfPanel(int PanelID, int PrCh)
        {
            IberEnum Itms = Depot.GetEnum(FILE_ITM);
            SubMenuPnls.Add(PanelID);
            List<StopListService.AlohaDish> DishTmp = new List<StopListService.AlohaDish>();
            IberEnum Btns = Depot.GetEnum(FILE_BTN);
            foreach (IberObject Btn2 in Btns)
            {
                try
                {
                    if (Btn2.GetLongVal("PANELID") == PanelID)
                    {
                        if (Btn2.GetLongVal("FUNC") == 21)
                        {
                            string Params2 = Btn2.GetStringVal("PARAMS");
                            //Utils.ToLog("Btn2.GetStringVal(PARAMS)  " + Params2 );
                            int DishId = Convert.ToInt32(Params2.Split(","[0])[0]);
                            //Utils.ToLog("DishId  " + DishId);
                            IberObject MyItm = Itms.FindFromLongAttr("ID", DishId);
                            //Utils.ToLog("MyItm  ok " );
                            decimal mPrice = 0;
                            if (!DishIsQty(DishId, out mPrice))
                            {



                                mPrice = GetPriceChangePrice(PrCh, DishId);
                                if (mPrice == -1)
                                {
                                    mPrice = (decimal)MyItm.GetDoubleVal("PRICE");
                                }


                                //mPrice = (decimal)MyItm.GetDoubleVal("PRICE");
                            }
                            //Utils.ToLog("mPrice ok " + mPrice );

                            StopListService.AlohaDish D = new StopListService.AlohaDish()
                            {
                                BarCode = DishId,
                                delay = MyItm.GetLongVal("DELAYTIME"),
                                Dep = mdepNum,
                                Price = mPrice,
                                Name = MyItm.GetStringVal("LONGNAME2"),
                                EngName = MyItm.GetStringVal("SKU")
                                //Name = Btn2.GetStringVal("TEXT").Replace(@"\n", " ").ToLower()
                            };
                            string En = MyItm.GetStringVal("SKU2");
                            if (En != "")
                            {
                                D.EngName = D.EngName + " " + En;
                            }

                            int Cat = 0;
                            D.Categories = GetDishCats(D.BarCode, out Cat).ToArray();
                            D.OneCategory = Cat;

                            Utils.ToLog("Добавляю блюдо  " + D.BarCode + " ");
                            DishTmp.Add(D);
                        }
                        else if (Btn2.GetLongVal("FUNC") == 19)
                        {
                            if ((Btn2.GetStringVal("TEXT").ToLower().Replace(@"\n", "") == "далее") || (Btn2.GetStringVal("TEXT").ToLower().Replace(@"\n", "") == "next"))
                            {
                                string Params2 = Btn2.GetStringVal("PARAMS");
                                int Pan2Id = Convert.ToInt32(Params2.Split(","[0])[5]);
                                if (!SubMenuPnls.Contains(Pan2Id))
                                {
                                    Utils.ToLog("Нашел кнопку далее на панель  " + Pan2Id);
                                    DishTmp.AddRange(GetQSDishesOfPanel(Pan2Id, PrCh));
                                }
                            }
                        }

                    }
                }
                catch (Exception e)
                {
                    Utils.ToCardLog("Error GetQSDishesOfPanel " + e.Message);
                }
            }
            //   DishTmp.Sort(CompareDishABC);
            return DishTmp;


        }

        static internal bool IsTableServise()
        {
            try
            {
                return AlohaFuncs.IsTableService();
            }
            catch
            {
                return true;
            }
        }

        static internal bool DeleteComp(int CompId)
        {
            try
            {
                Utils.ToCardLog("DeleteComp");
                AlohaFuncs.DeleteComp(GetTermNum(), CurentWaiter, (int)GetCurentCheckId(), CompId);
                return true;
            }
            catch (Exception e)
            {
                Utils.ToCardLog("Error DeleteComp" + e.Message);
                return false;
            }
        }

        static internal bool DeletePayment(int CheckId, int PaymentId)
        {
            try
            {
                Utils.ToCardLog("DeletePayment ");
                AlohaFuncs.DeletePayment(GetTermNum(), CheckId, PaymentId);
                return true;
            }
            catch (Exception e)
            {
                Utils.ToCardLog("Error DeletePayment " + e.Message);
                return false;
            }
        }


        private static int OldPrinter = 0;
        static internal int GetLocalPrinterNum()
        {

            try
            {
                IberObject Term = Depot.FindObjectFromId(INTERNAL_TERMINALS, GetTermNum()).First();
                OldPrinter = Term.GetLongVal("PRINTER");
            }
            catch (Exception e)
            {
                Utils.ToCardLog("Error GetLocalPrinterNum " + e.Message);
            }
            Utils.ToCardLog("return GetLocalPrinterNum " + OldPrinter);
            return OldPrinter;
        }



        static internal void PrintCardSlip(List<string> info)
        {
            try
            {
                string s = "";
                bool hasText = false;
                //foreach (string str in info)
                for (int i = 0; i < info.Count; i++)
                {
                    string str = info[i];

                    string Printstr = str;
                    //   Printstr = Printstr.Replace("~", " ");
                    //  Printstr = Printstr.Replace("^", " ");
                    //  Printstr = Printstr.Replace("0xDF", "   ");

                    //Utils.ToLog("PRINT: "+str);

                    if ((Printstr.Contains("0xDA") || (Printstr.Contains("0xDF"))))
                    {
                        Printstr = Printstr.Replace("0xDA", "");

                        if (i < info.Count - 1)
                            PrintINfo(s);

                        s = "<PRINTLINE>" + Printstr + "</PRINTLINE>";
                        hasText = !string.IsNullOrWhiteSpace(Printstr);
                        continue;
                    }

                    if (str.Contains("0xDE"))
                    {
                        //    continue;
                    }
                    if (str.Contains("0xDF"))
                    {
                        //continue;
                    }
                    //Printstr = str.Replace("0xD", "");
                    if (!string.IsNullOrWhiteSpace(Printstr))
                        hasText = true;
                    s += "<PRINTLINE>" + Printstr + "</PRINTLINE>";
                }

                if (hasText)
                    PrintINfo(s);
                /*
            string z = Ap.GetAllPrinters();
            Ap.PrintStream(s);
                 * */
            }
            catch (Exception e)
            {
                ShowMessage(e.Message);
            }
        }



        static internal void PrintFriendINfo(List<string> info)
        {
            try
            {

                if (!iniFile.PrintFriendInfoEnabled)
                {
                    return;
                }

                string s = "";
                s += "<PRINTLINE>" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "</PRINTLINE>";
                s += "<PRINTLINE>" + AlohainiFile.UNITNAME + "</PRINTLINE>";
                s += "<PRINTLINE>" + AlohainiFile.ADDRESS1 + "</PRINTLINE>";
                s += "<PRINTFILLED>*</PRINTFILLED>";
                s += "<LINEFEED>3</LINEFEED>";
                s += "<PRINTLINE>Спасибо за визит в Кофеманию!</PRINTLINE>";
                s += "<LINEFEED>3</LINEFEED>";
                foreach (string ss in info)
                {
                    s += "<PRINTLINE>" + ss.Substring(0, Math.Min(ss.Length, 42)) + "</PRINTLINE>";
                    s += "<LINEFEED>1</LINEFEED>";
                }

                s += "<LINEFEED>3</LINEFEED>";
                s += "<PRINTFILLED>*</PRINTFILLED>";


                if (System.IO.File.Exists(@"C:\Aloha\alohats\bmp\qr.bmp"))
                {
                    s += "<BITMAP>";
                    //s += "<PATH>qr.bmp</PATH>";
                    s += "<PATH>tdr013.bmp</PATH>";
                    s += "<SIZE>1</SIZE>";
                    s += "<JUST>0</JUST>";
                    s += "</BITMAP>";
                }
                PrintINfo(s);
                /*
            string z = Ap.GetAllPrinters();
            Ap.PrintStream(s);
                 * */
            }
            catch (Exception e)
            {
                ShowMessage(e.Message);
            }
        }

        static internal void PrintPredCheck(int CheckId, bool EgnoreError)
        {
            try
            {
                AlohaFuncs.PrintCheck(GetTermNum(), CheckId);
            }
            catch (Exception e)
            {
                if (!EgnoreError)
                {
                    // System.Windows.Forms.MessageBox.Show(e.Message);
                }
            }


        }



        static internal void PrintXReport(AllChecks info)
        {
            try
            {
                string s = "";
                s += "<PRINTLINE>" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "</PRINTLINE>";
                s += "<PRINTLINE>" + AlohainiFile.UNITNAME + "</PRINTLINE>";
                s += "<PRINTLINE>" + AlohainiFile.ADDRESS1 + "</PRINTLINE>";
                s += "<PRINTFILLED>*</PRINTFILLED>";
                s += "<LINEFEED>3</LINEFEED>";
                s += "<PRINTLINE>Выручка</PRINTLINE>";
                s += "<LINEFEED>3</LINEFEED>";

                decimal Cash1 = 0;
                decimal Cash2 = 0;
                decimal Card = 0;

                decimal VCash1 = 0;
                decimal VCash2 = 0;
                decimal VCard = 0;
                foreach (Check ss in info.Checks)
                {

                    foreach (AlohaTender tndr in ss.Tenders)
                    {

                        if (tndr.AlohaTenderId == 1)
                        {
                            if (!ss.Vozvr)
                            {
                                Cash1 += (decimal)tndr.Summ;
                            }
                            else
                            {
                                VCash1 += (decimal)tndr.Summ;
                            }
                        }
                        else if (tndr.AlohaTenderId == 2)
                        {
                            if (!ss.Vozvr)
                            {
                                Cash1 += (decimal)tndr.Summ;
                            }
                            else
                            {
                                VCash2 += (decimal)tndr.Summ;
                            }
                        }
                        else if (tndr.AlohaTenderId == 20)
                        {
                            if (!ss.Vozvr)
                            {
                                Card += (decimal)tndr.Summ;
                            }
                            else
                            {
                                VCard += (decimal)tndr.Summ;
                            }
                        }

                    }
                }


                //s += "<PRINTLINE>" + ss.Substring(0, Math.Min(ss.Length, 42)) + "</PRINTLINE>";
                s += "<PRINTLEFTRIGHT><LEFT>Наличные</LEFT>";
                s += "<RIGHT>" + (Cash1 + Cash2) + "</RIGHT></PRINTLEFTRIGHT>";
                if (VCash1 + VCash2 != 0)
                {
                    s += "<PRINTLEFTRIGHT><LEFT>Возврат наличные </LEFT>";
                    s += "<RIGHT>" + (VCash1 + VCash2) + "</RIGHT></PRINTLEFTRIGHT>";
                    s += "<PRINTLEFTRIGHT><LEFT>Итого наличные </LEFT>";
                    s += "<RIGHT>" + (Cash1 + Cash2 + VCash1 + VCash2) + "</RIGHT></PRINTLEFTRIGHT>";
                }
                s += "<PRINTLEFTRIGHT><LEFT>Кред карта</LEFT>";
                s += "<RIGHT>" + Card + "</RIGHT></PRINTLEFTRIGHT>";
                if (VCard != 0)
                {
                    s += "<PRINTLEFTRIGHT><LEFT>Возврат кред карта </LEFT>";
                    s += "<RIGHT>" + VCard + "</RIGHT></PRINTLEFTRIGHT>";
                    s += "<PRINTLEFTRIGHT><LEFT>Итого кред карта </LEFT>";
                    s += "<RIGHT>" + (Card + VCard) + "</RIGHT></PRINTLEFTRIGHT>";
                }


                s += "<LINEFEED>3</LINEFEED>";
                s += "<PRINTFILLED>*</PRINTFILLED>";



                PrintINfo(s);
                /*
            string z = Ap.GetAllPrinters();
            Ap.PrintStream(s);
                 * */
            }
            catch (Exception e)
            {
                ShowMessage(e.Message);
            }
        }


        static internal void PrintStopList(List<string> info)
        {
            try
            {
                string s = "";
                s += "<PRINTLINE>" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "</PRINTLINE>";
                s += "<PRINTLINE>" + AlohainiFile.UNITNAME + "</PRINTLINE>";
                s += "<PRINTLINE>" + AlohainiFile.ADDRESS1 + "</PRINTLINE>";
                s += "<PRINTFILLED>*</PRINTFILLED>";
                s += "<LINEFEED>3</LINEFEED>";
                s += "<PRINTLINE>Отчет по стоп-листу</PRINTLINE>";
                s += "<LINEFEED>3</LINEFEED>";
                foreach (string ss in info)
                {
                    s += "<PRINTLINE>" + ss + "</PRINTLINE>";
                    //s += "<PRINTLEFTRIGHT><LEFT>" + ss + "</LEFT>";
                    //  s += "<RIGHT>" + ss.Count + "</RIGHT></PRINTLEFTRIGHT>";

                }

                s += "<LINEFEED>3</LINEFEED>";
                s += "<PRINTFILLED>*</PRINTFILLED>";



                PrintINfo(s);
                /*
            string z = Ap.GetAllPrinters();
            Ap.PrintStream(s);
                 * */
            }
            catch (Exception e)
            {
                ShowMessage(e.Message);
            }
        }

        static internal void PrintSoldReport(List<StopListDish> info)
        {
            try
            {
                string s = "";
                s += "<PRINTLINE>" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "</PRINTLINE>";
                s += "<PRINTLINE>" + AlohainiFile.UNITNAME + "</PRINTLINE>";
                s += "<PRINTLINE>" + AlohainiFile.ADDRESS1 + "</PRINTLINE>";
                s += "<PRINTFILLED>*</PRINTFILLED>";
                s += "<LINEFEED>3</LINEFEED>";
                s += "<PRINTLINE>Отчет по товарам</PRINTLINE>";
                s += "<LINEFEED>3</LINEFEED>";
                foreach (StopListDish ss in info)
                {
                    //s += "<PRINTLINE>" + ss.Substring(0, Math.Min(ss.Length, 42)) + "</PRINTLINE>";
                    s += "<PRINTLEFTRIGHT><LEFT>" + ss.Name + "</LEFT>";
                    s += "<RIGHT>" + ss.Count + "</RIGHT></PRINTLEFTRIGHT>";

                }

                s += "<LINEFEED>3</LINEFEED>";
                s += "<PRINTFILLED>*</PRINTFILLED>";



                PrintINfo(s);
                /*
            string z = Ap.GetAllPrinters();
            Ap.PrintStream(s);
                 * */
            }
            catch (Exception e)
            {
                ShowMessage(e.Message);
            }
        }


        static internal Check GetCurentCheck()
        {
            CheckWindow();
            return GetCheckById((int)AlohaCurentState.CheckId);
        }

        static private void PrintCurentPredcheck(bool AllDishez, List<int> PrintableGropes, bool PrintableGropesPrint)
        {
            string s = "";
            try
            {
                CheckWindow();

                s = "<PRINT>";
                s += "<PRINTER>" + GetLocalPrinterNum().ToString() + "</PRINTER>";
                s += "<COMMANDS>";
                s += FormStringPrintPredcheck(GetCheckById((int)AlohaCurentState.CheckId), AllDishez, PrintableGropes, PrintableGropesPrint, false, null, "", false, false);
                // s += "<POSTLINEFEEDS>2</POSTLINEFEEDS> ";
                //s += FormStringPrintPredcheck(GetCheckById((int)AlohaCurentState.CheckId), AllDishez, PrintableGropes, PrintableGropesPrint, false, null, "", false);

                s += "<POSTLINEFEEDS>4</POSTLINEFEEDS> ";
                s += "<CUT>FULL</CUT>";
                s += "<POSTLINEFEEDS>3</POSTLINEFEEDS> ";
                s += "</COMMANDS>";
                s += @"</PRINT>";
                Utils.ToLog("PrintCurentPredcheck s: " + s);
            }
            catch (Exception e)
            {
                Utils.ToLog("Error PrintCurentPredcheck s: " + s + " Error :" + e.Message);
            }
            try
            {
                Ap.PrintStream(s);
            }
            catch (Exception e)
            {
                Utils.ToLog("Error PrintCurentPredcheck Ap.PrintStream(s); " + e.Message);
            }
        }
        public static string SaveQRTips(int head_place_code, int emp_id, int invoice_Id)
        {
            string fName = "QRTIPS.bmp";
            try
            {
                string BmpPath = @"c:\aloha\alohats\bmp\";
                DirectoryInfo di = new DirectoryInfo(BmpPath);

                string str = @"https://pay.cloudtips.ru/e/" + head_place_code + "/" + AlohainiFile.DepNum + "/" + emp_id + "? invoiceId=" + invoice_Id;
                var QrImg = FRSClientApp.FiscalCheckCreator.CreateQRBitmap(str, 260, 260);
                BitmapEncoder encoder = new BmpBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(QrImg));

                using (var fileStream = new System.IO.FileStream(BmpPath + fName, System.IO.FileMode.Create))
                {
                    encoder.Save(fileStream);
                }
            }
            catch (Exception e)
            {
                Utils.ToCardLog("Error Save QR TIPS" + e.Message);
            }
            return fName;
        }

        public static string SaveQREmpInfo(string EmpName)
        {
            string fName = "PRQR.bmp";
            try
            {
                string BmpPath = @"c:\aloha\alohats\bmp\";
                DirectoryInfo di = new DirectoryInfo(BmpPath);

                try
                {
                    foreach (FileInfo fi in di.GetFiles())
                    {
                        if (fi.Name.StartsWith("PRQR"))
                        {
                            fi.Delete();
                        }
                    }
                }
                catch
                { }
                string EncEmpl = HttpUtility.UrlEncode(EmpName);
                string QRStr = @"http://saycoffeemania.ru/?DepId=" + AlohainiFile.DepNum + "&Emp=" + EncEmpl;
                //string QRStr = @"http://saycoffeemania.ru/?DepId=" + "104" + "&Emp=" + EncEmpl;
                var QrImg = FRSClientApp.FiscalCheckCreator.CreateQRBitmap(QRStr, 260, 260);
                BitmapEncoder encoder = new BmpBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(QrImg));

                using (var fileStream = new System.IO.FileStream(BmpPath + fName, System.IO.FileMode.Create))
                {
                    encoder.Save(fileStream);
                }
            }
            catch (Exception e)
            {
                Utils.ToCardLog("Error SaveQR " + e.Message);
            }
            return fName;
        }


        static internal void PrintAllPredchecksById(int Checkid, int Printer, List<int> Checks)
        {
            DateTime NDt = DateTime.Now;
            string s = "";
            s = "<PRINT>";
            s += "<PRINTER>" + Printer.ToString() + "</PRINTER>";
            s += "<COMMANDS>";
            //s += "<SETCODEPAGE>11</SETCODEPAGE>";

            Check Ch = GetCheckById(Checkid);
            if (iniFile.DifferentPredcheckSaleGroupe == 0)
            {
                decimal AllSumm = 0;
                s += "<LINEFEED>5</LINEFEED>";

                s += "<PRINTCENTERED>" + AlohainiFile.ADDRESS1 + "</PRINTCENTERED>";
                s += "<PRINTCENTERED>" + AlohainiFile.ADDRESS2 + "</PRINTCENTERED>";

                string wName = GetWaterName(Ch.Waiter);
                /*
                if ((Config.ConfigSettings.QRPrinting) && ((Ch.IsClosed) ^ (IsTableServise())))
                {
                    
                    string fName = SaveQREmpInfo(wName);
                    s += "<LINEFEED>1</LINEFEED>";
                    s += "<PRINTCENTERED>Оставьте, пожалуйста, комментарий</PRINTCENTERED> ";
                    s += "<PRINTCENTERED>по Вашему визиту, наведя камеру на QR код</PRINTCENTERED>";
                    s += "<PRINTCENTERED>  </PRINTCENTERED>";
                    s += "<PRINTBITMAP><PATH>" + fName + "</PATH><SIZE>1</SIZE><JUSTIFY>1</JUSTIFY> </PRINTBITMAP>";
                    s += "<LINEFEED>1</LINEFEED>";
                }
                */


                if ((Ch.IsClosed) )
                {
                    string dir = @"C:\aloha\alohats\bmp\";
                    string fName2 = @"appQr.bmp";
                    if (File.Exists(dir+fName2))

                    {
                     //   s += "<LINEFEED>1</LINEFEED>";
                        s += "<PRINTCENTERED>  </PRINTCENTERED>";
                        s += "<PRINTBITMAP><PATH>" + fName2 + "</PATH><SIZE>1</SIZE><JUSTIFY>1</JUSTIFY> </PRINTBITMAP>";
                        s += "<LINEFEED>1</LINEFEED>";
                    }
                    else
                    {
                        Utils.ToLog("Error print QR. Not exists file " + fName2);
                    }

                }


                s += "<PRINTLEFTRIGHT><LEFT>Официант: " + wName + "</LEFT>";
                s += "<RIGHT>" + NDt.ToString("dd/MM/yyyy") + "</RIGHT></PRINTLEFTRIGHT>";
                if (IsTableServise())
                {
                    if (iniFile.PrintTableDesc)
                    {
                        s += "<PRINTLEFTRIGHT><LEFT>Стол: " + Ch.TableNumber + " " + Ch.TableDescription + "</LEFT>";
                    }
                    else
                    {
                        s += "<PRINTLEFTRIGHT><LEFT>Стол: " + Ch.TableNumber + "</LEFT>";
                    }
                }
                else
                {
                    s += "<PRINTLEFTRIGHT><LEFT>Стол: " + Ch.TableName + "</LEFT>";
                }
                s += "<RIGHT>" + NDt.ToString("HH:mm") + "</RIGHT></PRINTLEFTRIGHT>";
                foreach (int CheckN in Checks)
                {
                    Check Ch2 = GetCheckById(CheckN);
                    s += "<LINEFEED>1</LINEFEED>";
                    s += "<PRINTCENTERED>-------------------------------------------------------------------</PRINTCENTERED>";
                    s += "<PRINTCENTERED>ЧЕК #" + Ch2.CheckShortNum.ToString() + "</PRINTCENTERED>";
                    s += "<LINEFEED>1</LINEFEED>";
                    decimal PodIt2 = 0;
                    decimal Discount2 = 0;
                    foreach (Dish dd in Ch2.ConSolidateDishez)
                    {

                        string CountStr = (dd.Count == 1) ? "" : " (" + dd.Count.ToString() + "x " + dd.OPrice.ToString("0.00") + ")";
                        decimal Count = Math.Ceiling(dd.Count * dd.QUANTITY * dd.QtyQUANTITY);
                        if ((CountStr == "") && (Count > 1))
                        {
                            CountStr = "(" + Count.ToString() + "гр)";
                        }
                        s += "<PRINTLEFTRIGHT><LEFT>" + dd.LongName + CountStr + "</LEFT>";
                        s += "<RIGHT>" + (dd.OPrice * dd.Count).ToString("0.00") + "</RIGHT></PRINTLEFTRIGHT>";
                        PodIt2 += dd.OPrice * dd.Count;
                        Discount2 += (dd.OPrice - dd.Price) * dd.Count;
                    }
                    s += "<LINEFEED>1</LINEFEED>";
                    s += "<PRINTLEFTRIGHT><LEFT>Подытог чека: </LEFT>";
                    s += "<RIGHT>" + PodIt2.ToString("0.00") + "</RIGHT></PRINTLEFTRIGHT>";
                    decimal sum2 = PodIt2 - Discount2 + Ch2.ServiceChargeSumm;
                    if (Ch2.Comp != 0)
                    {
                        s += "<LINEFEED>1</LINEFEED>";
                        s += "<PRINTLEFTRIGHT><LEFT>" + Ch2.CompName + " </LEFT>";
                        s += "<RIGHT> -" + Discount2.ToString("0.00") + "</RIGHT></PRINTLEFTRIGHT>";
                        if ((Ch2.CompId > 9) && (Ch2.CompId < 25))
                        {
                            s += "<PRINTLEFTRIGHT><LEFT>" + Ch2.CompDescription + "</LEFT><RIGHT> " + Ch2.DegustationMGR_NUMBER + "</RIGHT></PRINTLEFTRIGHT></PRINTLEFTRIGHT>";
                        }


                    }
                    if (Ch2.ServiceChargeSumm > 0)
                    {
                        s += "<LINEFEED>1</LINEFEED>";
                        s += "<PRINTLEFTRIGHT><LEFT>" + Ch2.ServiceChargeName + " </LEFT>";
                        s += "<RIGHT> " + Ch2.ServiceChargeSumm.ToString("0.00") + "</RIGHT></PRINTLEFTRIGHT>";

                    }


                    s += "<LINEFEED>1</LINEFEED>";
                    s += "<PRINTLEFTRIGHT><LEFT>Итог чека: </LEFT>";
                    s += "<RIGHT> " + sum2.ToString("0.00") + "</RIGHT></PRINTLEFTRIGHT>";
                    s += "<LINEFEED>1</LINEFEED>";
                    if (!String.IsNullOrWhiteSpace(Ch2.PMSGuestName))
                    {
                        s += "<PRINTLEFTRIGHT><LEFT>" + Ch2.PMSGuestName + "</LEFT>";
                        s += "<RIGHT> </RIGHT></PRINTLEFTRIGHT>";
                        s += "<LINEFEED>1</LINEFEED>";
                        s += "<PRINTLEFTRIGHT><LEFT>________________________________</LEFT>";
                        s += "<RIGHT> </RIGHT></PRINTLEFTRIGHT>";
                    }
                    AllSumm += sum2;
                }
                s += "<PRINTCENTERED>========================================</PRINTCENTERED>";



                s += "<PRINTSTYLE><CPI>1</CPI><STYLE>1</STYLE></PRINTSTYLE>";



                s += "<PRINTLEFTRIGHT><LEFT>Общий итог: </LEFT>";
                s += "<RIGHT> " + AllSumm.ToString("0.00") + "</RIGHT></PRINTLEFTRIGHT>";
                s += "<PRINTSTYLE><CPI>1</CPI><STYLE>0</STYLE></PRINTSTYLE>";
                s += "<LINEFEED>1</LINEFEED>";


                s += GetReklamaStr();
                /*
                s += "<PRINTCENTERED>1 марта - 1 июня участвуйте </PRINTCENTERED>";
                s += "<PRINTCENTERED>в благотворительном проекте </PRINTCENTERED>";

                s += "<PRINTCENTERED>Кофемании с фондами 'Вера' и 'Я Есть!'.</PRINTCENTERED>";
                s += "<PRINTCENTERED>О подробностях Вам расскажут наши официанты.</PRINTCENTERED>";
                */

                /*
                s += "<PRINTCENTERED>Gratuity not included and always </PRINTCENTERED>";
                s += "<PRINTCENTERED>remains on guest’s discretion</PRINTCENTERED>";
                s += "<PRINTCENTERED>Thank you</PRINTCENTERED>";
                 * * */
                s += "<PRINTCENTERED>Спасибо</PRINTCENTERED>";


            }
            s += "<POSTLINEFEEDS>4</POSTLINEFEEDS> ";
            s += "<CUT>PARTIAL</CUT>";
            s += "<POSTLINEFEEDS>3</POSTLINEFEEDS> ";

            s += "</COMMANDS>";
            s += @"</PRINT>";
            Utils.ToLog("Ap.PrintStream(s): ");
            Utils.ToLog(s, 6);

            Ap.PrintStream(s);
        }

        static private string GetReklamaStr()
        {
            /*
            string       s = "<PRINTCENTERED>1 марта - 1 июня участвуйте </PRINTCENTERED>";
            s += "<PRINTCENTERED>в благотворительном проекте </PRINTCENTERED>";

            s += "<PRINTCENTERED>Кофемании с фондами 'Вера' и 'Я Есть!'.</PRINTCENTERED>";
            s += "<PRINTCENTERED>О подробностях Вам расскажут</PRINTCENTERED>";
            s += "<PRINTCENTERED>наши официанты.</PRINTCENTERED>";
            s += "<PRINTCENTERED>    </PRINTCENTERED>";
            s += "<PRINTCENTERED>    </PRINTCENTERED>";
                */
            string s = "";
            return s;
        }

        static internal void PrintPredCheckByXml(int Checkid, int Printer)
        {
            string s = GetPrintPredcheckById(Checkid, Printer, false, null, "", false);
            Ap.PrintStream(s);
        }
        static internal string GetPrintPredcheckById(int Checkid, int Printer, bool PrintAll, List<int> Checks, string innerStr, bool Closed)
        {
            string s = "";
            s = "<PRINT>";
            s += "<PRINTER>" + Printer.ToString() + "</PRINTER>";
            s += "<COMMANDS>";
            s += innerStr;
            //s += "<SETCODEPAGE>11</SETCODEPAGE>";

            Check Ch = GetCheckById(Checkid);
            if (iniFile.DifferentPredcheckSaleGroupe == 0)
            {
                s += AlohaTSClass.FormStringPrintPredcheck(Ch, true, null, false, PrintAll, Checks, innerStr, Closed, false);
                if (IsAlohaTS() && ((Ch.TableNumber >= 146 && Ch.TableNumber < 255) ||(Ch.TableNumber >= 900 && Ch.TableNumber<=999))  && !Closed)
                {
                    s += "<LINEFEED>4</LINEFEED> ";
                    s += "<CUT>PARTIAL</CUT>";
                    s += "<LINEFEED>3</LINEFEED> ";
                    s += "</COMMANDS>";
                    s += @"</PRINT>";
                    Ap.PrintStream(s); // Делаем так, потому что принтер не отрезает, если слать подряд 2 слипа
                    s = "<PRINT>";
                    s += "<PRINTER>" + Printer.ToString() + "</PRINTER>";
                    s += "<COMMANDS>";
                    s += AlohaTSClass.FormStringPrintPredcheck(Ch, true, null, false, PrintAll, Checks, innerStr, Closed, true); //Это с модификаторами

                }

            }
            else
            {
                List<int> Tmp = new List<int> { iniFile.DifferentPredcheckSaleGroupe };
                s += AlohaTSClass.FormStringPrintPredcheck(Ch, false, Tmp, true, PrintAll, Checks, innerStr, Closed, false);

                s += "<LINEFEED>4</LINEFEED> ";
                s += "<CUT>PARTIAL</CUT>";
                s += "<LINEFEED>3</LINEFEED> ";
                s += "</COMMANDS>";
                s += @"</PRINT>";
                Ap.PrintStream(s); // Делаем так, потому что принтер не отрезает, если слать подряд 2 слипа

                s = "<PRINT>";
                s += "<PRINTER>" + Printer.ToString() + "</PRINTER>";
                s += "<COMMANDS>";
                //s += "<SETCODEPAGE>11</SETCODEPAGE>";
                s += AlohaTSClass.FormStringPrintPredcheck(Ch, false, Tmp, false, PrintAll, Checks, innerStr, Closed, false);
            }


            s += "<POSTLINEFEEDS>4</POSTLINEFEEDS> ";
            s += "<CUT>PARTIAL</CUT>";
            s += "<POSTLINEFEEDS>3</POSTLINEFEEDS> ";

            s += "</COMMANDS>";
            s += "<DOCTYPE>2</DOCTYPE>";
            s += "<CHECKID>" + Checkid + @"</CHECKID>";

            s += @"</PRINT>";
            Utils.ToLog(s, 6);
            return s;
        }

        static internal void PrintCurentPredcheck()
        {
            //Используется для стронних заказчиков. QR не печатаем
            if (iniFile.DifferentPredcheckSaleGroupe == 0)
            {
                AlohaTSClass.PrintCurentPredcheck(true, null, false);
            }
            else
            {
                List<int> Tmp = new List<int> { iniFile.DifferentPredcheckSaleGroupe };
                AlohaTSClass.PrintCurentPredcheck(false, Tmp, true);
                AlohaTSClass.PrintCurentPredcheck(false, Tmp, false);
            }
        }


        public static void RefreshCheckDisplay()
        {
            try
            {
                Utils.ToCardLog("RefreshCheckDisplay");
                AlohaFuncs.RefreshCheckDisplay();
            }
            catch (Exception e)
            {
                Utils.ToCardLog("Error RefreshCheckDisplay " + e.Message);
            }
        }

        static public string GetEngName(int barcode)
        {
            IberEnum Itms = Depot.GetEnum(FILE_ITM);
            string s1 = "";
            string s2 = "";
            try
            {
                s1 = Itms.FindFromLongAttr("ID", barcode).GetStringVal("SKU").Replace(@"\n", " ");
            }
            catch
            { }
            try
            {
                s2 = Itms.FindFromLongAttr("ID", barcode).GetStringVal("SKU2").Replace(@"\n", " ");
            }
            catch { }

            return s1 + " " + s2;
        }

        private const int FILE_PC = 98;
        static public bool GetPriceStartDateIsActual(int PrCh)
        {
            try
            {
                Utils.ToCardLog("GetPriceStartDateIsActual " + PrCh);

                //  bool res = true;

                DateTime Bd = AlohainiFile.BDate;

                Utils.ToCardLog("DateTime Bd = AlohainiFile.BDate;");



                IberEnum PCs = Depot.GetEnum(FILE_PC);
                Utils.ToCardLog("IberEnum PCs = Depot.GetEnum(FILE_PC);");

                IberObject PC = PCs.FindFromLongAttr("ID", PrCh);
                Utils.ToCardLog("IberObject PC = PCs.FindFromLongAttr(PrCh);");

                string dtStart = PC.GetStringVal("STARTDATE");
                Utils.ToCardLog("dtStart " + dtStart);
                string dtStop = PC.GetStringVal("ENDDATE");
                Utils.ToCardLog("dtStop " + dtStop);
                if ((dtStart == "") || (dtStop == ""))
                {
                    return true;
                }

                //DateTime DtStart = Convert.ToDateTime(dtStart, CultureInfo.CurrentCulture);
                DateTime DtStart = new DateTime(int.Parse(dtStart.Split(@"/"[0])[2]), int.Parse(dtStart.Split(@"/"[0])[0]), int.Parse(dtStart.Split(@"/"[0])[1]));




                //DateTime DtStop = Convert.ToDateTime(dtStop, CultureInfo.CurrentCulture);
                DateTime DtStop = new DateTime(int.Parse(dtStop.Split(@"/"[0])[2]), int.Parse(dtStop.Split(@"/"[0])[0]), int.Parse(dtStop.Split(@"/"[0])[1]));





                return ((Bd >= DtStart) && (Bd < DtStop));
            }
            catch (Exception e)
            {
                Utils.ToCardLog("Error GetPriceStartDateIsActual " + e.Message);
                return false;
            }

        }

        static int PrintWideString = 35;//-10;
        static internal string PrintDishStr(Dish dd, bool modsNeed = false, bool printMod = false)
        {

            Utils.ToCardLog("PrintDishStr dd " + dd.Name + " modsNeed: " + modsNeed);
            int MaxWidth = PrintWideString - 10;
            string s = "";
            string CountStr = (dd.Count == 1) ? "" : " (" + dd.Count.ToString() + "x " + dd.OPrice.ToString("0.00") + ")";
            decimal Count = Math.Ceiling(dd.Count * dd.QUANTITY * dd.QtyQUANTITY);
            if ((CountStr == "") && (Count > 1))
            {
                CountStr = "(" + Count.ToString() + "гр)";
            }
            string DishName = dd.LongName;
            if (iniFile.DishPrintName == 1)
            {
                DishName = dd.Name;
            }
            else if (iniFile.DishPrintName == 2)
            {
                DishName = dd.CHITNAME;
            }
            if (printMod)
            {
                DishName = "----" + DishName;
            }

            string disStr = DishName + CountStr;
            if (disStr.Length > MaxWidth)
            {
                if (DishName.Length > MaxWidth)
                {
                    string FirstStr = DishName.Substring(0, MaxWidth);
                    int LastSpacePos = FirstStr.Trim().LastIndexOf(" ");
                    FirstStr = FirstStr.Substring(0, LastSpacePos);
                    string SecondStr = DishName.Substring(LastSpacePos + 1);
                    s += "<PRINTLEFTRIGHT><LEFT>" + FirstStr + "</LEFT>";
                    s += "<RIGHT> </RIGHT></PRINTLEFTRIGHT>";
                    s += "<PRINTLEFTRIGHT><LEFT>" + SecondStr + CountStr + "</LEFT>";
                    s += "<RIGHT>" + (dd.OPrice * dd.Count).ToString("0.00") + "</RIGHT></PRINTLEFTRIGHT>";
                }
                else
                {
                    s += "<PRINTLEFTRIGHT><LEFT>" + DishName + "</LEFT>";
                    s += "<RIGHT> </RIGHT></PRINTLEFTRIGHT>";
                    s += "<PRINTLEFTRIGHT><LEFT>" + CountStr + "</LEFT>";
                    s += "<RIGHT>" + (dd.OPrice * dd.Count).ToString("0.00") + "</RIGHT></PRINTLEFTRIGHT>";
                }
            }
            else
            {
                s += "<PRINTLEFTRIGHT><LEFT>" + DishName + CountStr + "</LEFT>";
                s += "<RIGHT>" + (dd.OPrice * dd.Count).ToString("0.00") + "</RIGHT></PRINTLEFTRIGHT>";
            }

            if (iniFile.PrintEnglishItemName)
            {
                string dEngName = GetEngName(dd.BarCode);
                if (!string.IsNullOrWhiteSpace(dEngName))
                {
                    if (dEngName.Length > MaxWidth)
                    {
                        string FirstStr = dEngName.Substring(0, MaxWidth);
                        int LastSpacePos = FirstStr.Trim().LastIndexOf(" ");
                        FirstStr = FirstStr.Substring(0, LastSpacePos);
                        string SecondStr = dEngName.Substring(LastSpacePos + 1);
                        s += "<PRINTLEFTRIGHT><LEFT>" + FirstStr + "</LEFT>";
                        s += "<RIGHT> </RIGHT></PRINTLEFTRIGHT>";
                        s += "<PRINTLEFTRIGHT><LEFT>" + SecondStr + CountStr + "</LEFT>";
                        s += "<RIGHT></RIGHT></PRINTLEFTRIGHT>";
                    }
                    else
                    {
                        s += "<PRINTLEFTRIGHT><LEFT>" + dEngName + "</LEFT>";
                        s += "<RIGHT> </RIGHT></PRINTLEFTRIGHT>";
                        s += "<PRINTLEFTRIGHT><LEFT>" + CountStr + "</LEFT>";
                        s += "<RIGHT></RIGHT></PRINTLEFTRIGHT>";
                    }
                }
            }

            if (modsNeed)
            {
                Utils.ToCardLog("PrintDishStr dd modscount " + dd.CurentModificators.Count);
                foreach (var m in dd.CurentModificators)
                {

                    s += PrintDishStr(m, true, true);

                }
            }

            return s;
        }


        static internal string GetFRPredcheck(int checkId)
        {
            DateTime NDt = DateTime.Now;
            var Ch = GetCheckById(checkId);
            string res = "";
            res += Ch.TableName + Environment.NewLine;

            foreach (var d in Ch.Dishez)
            {
                res += d.Name + "&&" + d.Count + Environment.NewLine;
            }
            return res;
        }

        [DllImport("SUROK.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        private static extern void UpdateScopeInfo();


        private static void DeleteQRTips()
        {
            string BmpPath = @"c:\aloha\alohats\bmp\";
            DirectoryInfo di = new DirectoryInfo(BmpPath);
            foreach (FileInfo Fi in di.GetFiles())
            {
                if (Fi.Name.Contains("QRTIPS"))
                {
                    Fi.Delete();
                }

            }
        }


        static internal string FormStringPrintPredcheck(Check Ch, bool AllDishez, List<int> PrintableGropes, bool PrintableGropesPrint, bool PrintAll, List<int> Checks, string InnerString, bool Closed, bool needMods)
        {
      
            try
            {
                Utils.ToCardLog("FormStringPrintPredcheck needMods" + needMods.ToString());

                DateTime NDt = DateTime.Now;
                string s = "";

                



                if (needMods)
                {
                    s += "<PRINTSTYLE><CPI>1</CPI><STYLE>1</STYLE></PRINTSTYLE>";
                    s += "<PRINTCENTERED>ЧЕК ДЛЯ СБОРКИ</PRINTCENTERED>";
                    s += "<PRINTSTYLE><CPI>1</CPI><STYLE>0</STYLE></PRINTSTYLE>";
                }


                s += "<PRINTCENTERED>" + AlohainiFile.ADDRESS1 + "</PRINTCENTERED>";
                s += "<PRINTCENTERED>" + AlohainiFile.ADDRESS2 + "</PRINTCENTERED>";


                string wName = GetWaterName(Ch.Waiter);

                string fName = "";

                /*
                if ((Config.ConfigSettings.QRPrinting) && (!Closed) && (!needMods))
                {
                    fName = SaveQREmpInfo(wName);
                    s += "<PRINTCENTERED>Оставьте, пожалуйста, комментарий</PRINTCENTERED> ";
                    s += "<PRINTCENTERED>по Вашему визиту, наведя камеру на QR код</PRINTCENTERED>";
                    s += "<PRINTCENTERED>  </PRINTCENTERED>";
                    s += "<PRINTBITMAP><PATH>" + fName + "</PATH><SIZE>1</SIZE><JUSTIFY>1</JUSTIFY> </PRINTBITMAP>";
                }
                */
                MBClient mBClient = new MBClient();
                var sett = mBClient.GetSettingTips();

                Utils.ToLog("Получил настройки QR Tips, tips_type: " + sett.tips_type);
                if (sett.tips_type == 1 && !needMods)
                {
                    Utils.ToLog("Удаляю старые QR.bmp");
                    DeleteQRTips();

                    var filename = SaveQRTips(sett.head_place_code, Ch.Waiter, Ch.AlohaCheckNum);
                    Utils.ToLog("Создал новый QR.bmp по пути: " + filename);

                    s += "<PRINTCENTERED>Отсканируйте QR-код </PRINTCENTERED>";
                    s += "<PRINTCENTERED>чтобы оставить чаевые</PRINTCENTERED>";
                    s += "<PRINTBITMAP><PATH>" + filename + "</PATH><SIZE>1</SIZE><JUSTIFY>1</JUSTIFY> </PRINTBITMAP>";
                    s += "<PRINTCENTERED>Чаевые для официантов</PRINTCENTERED>";
                    s += "<PRINTCENTERED>не включены в счет</PRINTCENTERED>";
                    s += "<PRINTCENTERED>и всегда остаются</PRINTCENTERED>";
                    s += "<PRINTCENTERED>на усмотрение гостя. Спасибо!</PRINTCENTERED>";
                    s += "<LINEFEED>1</LINEFEED>";
                }
                else if ((!Closed) && (!needMods))
                {
                    string dir = @"C:\aloha\alohats\bmp\";
                    string fName2 = @"appQr.bmp";
                    if (File.Exists(dir + fName2))
                    {
                        //s += "<LINEFEED>1</LINEFEED>";
                        s += "<PRINTCENTERED>  </PRINTCENTERED>";
                        s += "<PRINTBITMAP><PATH>" + fName2 + "</PATH><SIZE>1</SIZE><JUSTIFY>1</JUSTIFY> </PRINTBITMAP>";
                        s += "<LINEFEED>1</LINEFEED>";
                    }
                    else
                    {
                        Utils.ToLog("Error print QR. Not exists file " + fName2);
                    }

                }
                s += "<PRINTCENTERED>  </PRINTCENTERED>";

                s += "<PRINTLEFTRIGHT><LEFT>Официант: " + wName + "</LEFT>";
                s += "<RIGHT>" + NDt.ToString("dd/MM/yyyy") + "</RIGHT></PRINTLEFTRIGHT>";
                s += "<PRINTLEFTRIGHT><LEFT></LEFT><RIGHT>" + NDt.ToString("HH:mm") + "</RIGHT></PRINTLEFTRIGHT>";

                if (Closed)
                {
                    s += "<PRINTLEFTRIGHT><LEFT>Кассир: " + GetWaterName(Ch.Cassir) + "</LEFT>";
                    s += "<RIGHT></RIGHT></PRINTLEFTRIGHT>";
                }
                
                if (Ch.Vozvr)
                {
                    s += "<PRINTSTYLE><CPI>1</CPI><STYLE>1</STYLE></PRINTSTYLE>";

                    s += "<PRINTLEFTRIGHT>";
                    s += "<LEFT>" + "Возврат" + "</LEFT><RIGHT></RIGHT></PRINTLEFTRIGHT>";
                    s += "<PRINTSTYLE><CPI>1</CPI><STYLE>0</STYLE></PRINTSTYLE>";

                }

                if (IsTableServise())
                {
                    if (iniFile.PrintTableDesc)
                    {
                        s += "<PRINTLEFTRIGHT><LEFT>Стол: " + Ch.TableNumber + @"/" + Ch.NumberInTable + " " + Ch.TableDescription + "</LEFT><RIGHT></RIGHT></PRINTLEFTRIGHT>";
                    }
                    else
                    {
                        s += "<PRINTLEFTRIGHT><LEFT>Стол: " + Ch.TableNumber + @"/" + Ch.NumberInTable + "</LEFT><RIGHT></RIGHT></PRINTLEFTRIGHT>";
                    }
                }
                else
                {
                    s += "<LINEFEED>1</LINEFEED>";
                    s += "<PRINTSTYLE><CPI>0</CPI><STYLE>1</STYLE></PRINTSTYLE>";
                    s += "<PRINTCENTERED>Ваш заказ</PRINTCENTERED>";
                    s += "<DOUBLEHEIGHT>1</DOUBLEHEIGHT><PRINTSTYLE><CPI>0</CPI><STYLE>4</STYLE></PRINTSTYLE>";

                    s += "<PRINTCENTERED>" + Ch.TableName + "</PRINTCENTERED>";
                    s += "<DOUBLEHEIGHT>0</DOUBLEHEIGHT><PRINTSTYLE><CPI>1</CPI><STYLE>0</STYLE></PRINTSTYLE>";
                    s += "<LINEFEED>1</LINEFEED>";
                }

                if (!needMods)
                {
                    s += "<PRINTLEFTRIGHT><LEFT>Гостей: " + Ch.Guests + "</LEFT>";
                    s += "<RIGHT></RIGHT></PRINTLEFTRIGHT>";
                }

                s += "<LINEFEED>1</LINEFEED>";
                if (IsTableServise())
                {
                    s += "<PRINTSTYLE><CPI>1</CPI><STYLE>1</STYLE></PRINTSTYLE>";
                }
                s += "<PRINTLEFTRIGHT><LEFT></LEFT>";
                s += "<RIGHT>" + "#" + Ch.CheckShortNum + "</RIGHT></PRINTLEFTRIGHT>";
                s += "<PRINTSTYLE><CPI>1</CPI><STYLE>0</STYLE></PRINTSTYLE>";
                s += "<LINEFEED>1</LINEFEED>";
                decimal PodIt = 0;
                decimal Discount = 0;

                //Не печатаем модификатор надбавку 999800
                int idx = 0;               
                foreach (Dish dd in Ch.Dishez)
                {
                    if(dd.BarCode == 999800)
                    {
                        if (idx > 0)
                        {
                            Ch.Dishez[idx - 1].Price = Ch.Dishez[idx - 1].Price + dd.Price;
                            Ch.Dishez[idx - 1].OPrice = Ch.Dishez[idx - 1].OPrice  + dd.OPrice;
                        }
                    }
                    idx++;
                }
                Ch.Dishez.RemoveAll(d => d.BarCode == 999800);

                if (AllDishez)
                {

                    if (needMods)
                    {
                        foreach (Dish dd in Ch.Dishez.Where(a => a.Level == 0))
                        {
                            s += PrintDishStr(dd, needMods);
                            PodIt += dd.OPrice * dd.Count;
                            Discount += (dd.OPrice - dd.Price) * dd.Count;
                            if (dd.CurentModificators != null && dd.CurentModificators.Count > 0)
                            {
                                PodIt += dd.CurentModificators.Sum(a => a.OPrice * a.Count);
                                Discount += dd.CurentModificators.Sum(a => ((a.OPrice - a.Price) * a.Count));
                            }

                        }
                    }
                    else
                    {
                        foreach (Dish dd in Ch.ConSolidateDishez)
                        {
                            s += PrintDishStr(dd, needMods);
                            PodIt += dd.OPrice * dd.Count;
                            Discount += (dd.OPrice - dd.Price) * dd.Count;
                        }
                    }
                }
                else
                {
                    if (PrintableGropesPrint)
                    {
                        bool DishEn = false;
                        foreach (Dish dd in Ch.ConSolidateDishez)
                        {
                            if (PrintableGropes.Contains(GetSaleGrope(dd.BarCode)))
                            {
                                DishEn = true;
                                /*
                                string CountStr = (dd.Count == 1) ? "" : " (" + dd.Count.ToString() + "x " + dd.OPrice.ToString("0.00") + ")";
                                s += "<PRINTLEFTRIGHT><LEFT>" + dd.CHITNAME  + CountStr + "</LEFT>";
                                s += "<RIGHT>" + (dd.OPrice * dd.Count).ToString("0.00") + "</RIGHT></PRINTLEFTRIGHT>";
                                 * */
                                s += PrintDishStr(dd);
                                PodIt += dd.OPrice * dd.Count;
                                Discount += (dd.OPrice - dd.Price) * dd.Count;
                            }

                        }
                        if (!DishEn)
                        {
                            return "";
                        }
                    }
                    else
                    {
                        bool DishEn = false;
                        foreach (Dish dd in Ch.Dishez)
                        {

                            if (!PrintableGropes.Contains(GetSaleGrope(dd.BarCode)))
                            {
                                DishEn = true;
                                s += PrintDishStr(dd);
                                PodIt += dd.OPrice;
                                Discount += dd.OPrice - dd.Price;
                            }
                        }
                        if (!DishEn)
                        {
                            return "";
                        }
                    }
                }


                s += "<LINEFEED>1</LINEFEED>";
                s += "<PRINTLEFTRIGHT><LEFT>Подытог: </LEFT>";
                s += "<RIGHT>" + PodIt.ToString("0.00") + "</RIGHT></PRINTLEFTRIGHT>";

                
                decimal sum_discount = 0;

                foreach (var comp in Ch.Comps)
                {
                    sum_discount += comp.Amount;
                }

                decimal sum = PodIt - sum_discount + Ch.ServiceChargeSumm;
                decimal summ_not_duscount = PodIt + Ch.ServiceChargeSumm;

                foreach (var comp in Ch.Comps)
                {
                    if (comp.Amount != 0)
                    {
                        
                        string comp_name = comp.Name;
                        if (comp.Id == 77)
                        {
                            comp_name = "БАЛЛЫ";
                        }

                        s += "<LINEFEED>1</LINEFEED>";
                        s += "<PRINTLEFTRIGHT><LEFT>" + comp_name + " </LEFT>";
                        s += "<RIGHT> -" + comp.Amount.ToString("0.00") + "</RIGHT></PRINTLEFTRIGHT>";

                    }

                }

                if (Ch.Comp != 0)
                {
                    if ((Ch.CompId > 9) && (Ch.CompId < 25))
                    {
                        s += "<PRINTLEFTRIGHT><LEFT>" + Ch.DegustationMGR_NUMBER + "</LEFT><RIGHT> " + Ch.CompDescription + "</RIGHT></PRINTLEFTRIGHT>";
                    }

                    s += "<PRINTLEFTRIGHT><LEFT>Подытог с учетом скидки: </LEFT>";
                    s += "<RIGHT>" + sum.ToString("0.00") + "</RIGHT></PRINTLEFTRIGHT>";
                }
                

      
                if (Ch.ServiceChargeSumm > 0 )
                {
                    s += "<LINEFEED>1</LINEFEED>";
                    s += "<PRINTLEFTRIGHT><LEFT>" + Ch.ServiceChargeName + " </LEFT>";
                    s += "<RIGHT> " + Ch.ServiceChargeSumm.ToString("0.00") + "</RIGHT></PRINTLEFTRIGHT>";

                }
                

                s += "<LINEFEED>1</LINEFEED>";
                s += "<PRINTLEFTRIGHT><LEFT>Итого: </LEFT>";
                s += "<RIGHT> " + sum.ToString("0.00") + "</RIGHT></PRINTLEFTRIGHT>";
                s += "<LINEFEED>1</LINEFEED>";

                decimal Oplata = 0;
                foreach (AlohaTender AT in Ch.Tenders)
                {
                    Oplata += (decimal)AT.SummWithOverpayment;
                    s += "<PRINTLEFTRIGHT><LEFT>" + AT.Name + ": </LEFT>";
                    s += "<RIGHT> " + AT.SummWithOverpayment.ToString("0.00") + "</RIGHT></PRINTLEFTRIGHT>";
                }


                if ((sum - Oplata) > 0)
                {
                    s += "<PRINTLEFTRIGHT><LEFT>К оплате: </LEFT>";
                    s += "<RIGHT> " + (sum - Oplata).ToString("0.00") + "</RIGHT></PRINTLEFTRIGHT>";
                }
                else if ((sum - Oplata) < 0)
                {

                    s += "<PRINTLEFTRIGHT><LEFT>Сдача: </LEFT>";
                    s += "<RIGHT> " + (-sum + Oplata).ToString("0.00") + "</RIGHT></PRINTLEFTRIGHT>";
                }


                //TODO: Печать информации о баллах
                foreach (var comp in Ch.Comps)
                {
                    if (comp.Id == 77)
                    {
                        string point_total_str = AlohaFuncs.GetObjectAttribute(INTERNAL_CHECKS, Ch.AlohaCheckNum, "total_p");

                        int points_total = 0;
                        if (point_total_str.Length != 0)
                        {
                            points_total = int.Parse(point_total_str);
                        }

                        int percent = GetPercentOfPayment(summ_not_duscount, comp.Amount);
                        if (percent < 15)
                        {
                            s += "<PRINTLEFTRIGHT><LEFT>Баллов на счету: </LEFT>";
                            s += "<RIGHT> " + points_total.ToString("0") + "</RIGHT></PRINTLEFTRIGHT>";

                            s += "<PRINTLEFTRIGHT><LEFT>Скидка баллами: </LEFT>";
                            s += "<RIGHT> " + comp.Amount.ToString("0") + "</RIGHT></PRINTLEFTRIGHT>";
                        }
                        if (percent >= 15)
                        {
                            s += "<PRINTLEFTRIGHT><LEFT>Баллов на счету: </LEFT>";
                            s += "<RIGHT> " + points_total.ToString("0") + "</RIGHT></PRINTLEFTRIGHT>";

                            s += "<PRINTLEFTRIGHT><LEFT>Скидка баллами: -" + percent.ToString() + "% </LEFT>";
                            s += "<RIGHT> " + comp.Amount.ToString("0") + "</RIGHT></PRINTLEFTRIGHT>";
                        }

                    }
                }




                string type = AlohaFuncs.GetObjectAttribute(INTERNAL_CHECKS, Ch.AlohaCheckNum, "type");
                if (type.Equals("accum"))
                {
                    UpdateScopeInfo();
                    string points_total_ = AlohaFuncs.GetObjectAttribute(INTERNAL_CHECKS, Ch.AlohaCheckNum, "total_p");
                    string quantity_points_str = AlohaFuncs.GetObjectAttribute(INTERNAL_CHECKS, Ch.AlohaCheckNum, "quant_p");

                    double point_quant = 0;
                    double.TryParse(quantity_points_str, out point_quant);

                    int percent = 0;
                    if (sum > 0)
                    { 
                        percent = (int)Math.Round((point_quant * 100 /(double)summ_not_duscount), 0, MidpointRounding.ToEven); 
                    }
                    

                    s += "<PRINTLEFTRIGHT><LEFT>Баллов на счету: </LEFT>";
                    s += "<RIGHT> " + points_total_ + "</RIGHT></PRINTLEFTRIGHT>";

                    s += "<PRINTLEFTRIGHT><LEFT>Будет зачислено:  </LEFT>";
                    s += "<RIGHT> " + quantity_points_str + " [" + percent.ToString() + "%]</RIGHT></PRINTLEFTRIGHT>";
                }



                if (!String.IsNullOrWhiteSpace(Ch.PMSGuestName))
                {
                    s += "<PRINTLEFTRIGHT><LEFT>" + Ch.PMSGuestName + "</LEFT>";
                    s += "<RIGHT> </RIGHT></PRINTLEFTRIGHT>";
                    s += "<LINEFEED>1</LINEFEED>";
                    s += "<PRINTLEFTRIGHT><LEFT>________________________________</LEFT>";
                    s += "<RIGHT> </RIGHT></PRINTLEFTRIGHT>";
                }


                s += "<LINEFEED>1</LINEFEED>";

                s += "<LINEFEED>1</LINEFEED>";
                if (needMods)
                {
                    s += "<LINEFEED>5</LINEFEED>";
                }


                if (Closed)
                {
                    s += "<PRINTCENTERED>-----Чек закрыт-----</PRINTCENTERED>";
                }

                if (sett.tips_type != 1 && !needMods)
                {
                    s += "<PRINTCENTERED>Чаевые для официантов</PRINTCENTERED>";
                    s += "<PRINTCENTERED>не включены в счет</PRINTCENTERED>";
                    s += "<PRINTCENTERED>и всегда остаются</PRINTCENTERED>";
                    s += "<PRINTCENTERED>на усмотрение гостя. Спасибо!</PRINTCENTERED>";
                }


                s += "<LINEFEED>1</LINEFEED>";
                //s += GetVisitsStr(Ch.AlohaCheckNum);
                if (!needMods)
                {
                    if (!iniFile.NoReklama) s += GetReklamaString();
                }

                //    s += "<CUT>FULL</CUT>";



                if (PrintAll)
                {
                    decimal AllSumm = 0;
                    s += "<LINEFEED>3</LINEFEED>";

                    s += "<PRINTCENTERED>" + AlohainiFile.ADDRESS1 + "</PRINTCENTERED>";
                    s += "<PRINTCENTERED>" + AlohainiFile.ADDRESS2 + "</PRINTCENTERED>";
                    /*
                    if (Config.ConfigSettings.QRPrinting)
                    {
                        //string fName = SaveQREmpInfo(wName);
                        s += "<LINEFEED>1</LINEFEED>";
                        s += "<PRINTCENTERED>Оставьте, пожалуйста, комментарий</PRINTCENTERED> ";
                        s += "<PRINTCENTERED>по Вашему визиту, наведя камеру на QR код</PRINTCENTERED>";
                        s += "<PRINTCENTERED>  </PRINTCENTERED>";
                        s += "<PRINTBITMAP><PATH>" + fName + "</PATH><SIZE>1</SIZE><JUSTIFY>1</JUSTIFY> </PRINTBITMAP>";
                        s += "<LINEFEED>1</LINEFEED>";
                    }
                    */



                    string dir = @"C:\aloha\alohats\bmp\";
                    string fName2 = @"appQr.bmp";
                    if (File.Exists(dir + fName2))
                       

                        {
                          //  s += "<LINEFEED>1</LINEFEED>";
                            s += "<PRINTCENTERED>  </PRINTCENTERED>";
                            s += "<PRINTBITMAP><PATH>" + fName2 + "</PATH><SIZE>1</SIZE><JUSTIFY>1</JUSTIFY> </PRINTBITMAP>";
                            s += "<LINEFEED>1</LINEFEED>";
                        }
                        else
                        {
                            Utils.ToLog("Error print QR. Not exists file " + fName2);
                        }






                    s += "<PRINTLEFTRIGHT><LEFT>Официант: " + GetWaterName(Ch.Waiter) + "</LEFT>";
                    s += "<RIGHT>" + NDt.ToString("dd/MM/yyyy") + "</RIGHT></PRINTLEFTRIGHT>";



                    if (iniFile.PrintTableDesc)
                    {
                        s += "<PRINTLEFTRIGHT><LEFT>Стол: " + Ch.TableNumber + @"/" + Ch.NumberInTable + " " + Ch.TableDescription + "</LEFT>";
                    }
                    else
                    {
                        s += "<PRINTLEFTRIGHT><LEFT>Стол: " + Ch.TableNumber + @"/" + Ch.NumberInTable + "</LEFT>";
                    }
                    s += "<RIGHT>" + NDt.ToString("HH:mm") + "</RIGHT></PRINTLEFTRIGHT>";
                    foreach (int CheckN in Checks)
                    {
                        Check Ch2 = GetCheckById(CheckN);
                        s += "<LINEFEED>1</LINEFEED>";
                        s += "<PRINTCENTERED>-------------------------------------------------------------------</PRINTCENTERED>";
                        s += "<PRINTCENTERED>ЧЕК #" + Ch2.CheckShortNum.ToString() + "</PRINTCENTERED>";
                        if (Ch2.Vozvr)
                        {
                            s += "<PRINTSTYLE><CPI>1</CPI><STYLE>1</STYLE></PRINTSTYLE>";

                            s += "<PRINTLEFTRIGHT>";
                            s += "<LEFT>" + "Возврат" + "</LEFT><RIGHT></RIGHT></PRINTLEFTRIGHT>";
                            s += "<PRINTSTYLE><CPI>1</CPI><STYLE>0</STYLE></PRINTSTYLE>";

                        }
                        s += "<LINEFEED>1</LINEFEED>";
                        decimal PodIt2 = 0;
                        decimal Discount2 = 0;
                        foreach (Dish dd in Ch2.Dishez)
                        {
                            /*
                            s += "<PRINTLEFTRIGHT><LEFT>" + dd.CHITNAME + "</LEFT>";
                            s += "<RIGHT>" + dd.OPrice.ToString("0.00") + "</RIGHT></PRINTLEFTRIGHT>";
                             * */
                            s += PrintDishStr(dd);
                            PodIt2 += dd.OPrice;
                            Discount2 += dd.OPrice - dd.Price;
                        }
                        s += "<LINEFEED>1</LINEFEED>";
                        s += "<PRINTLEFTRIGHT><LEFT>Подытог чека: </LEFT>";
                        s += "<RIGHT>" + PodIt2.ToString("0.00") + "</RIGHT></PRINTLEFTRIGHT>";
                        decimal sum2 = PodIt2 - Discount2 + Ch2.ServiceChargeSumm;
                        if (Ch2.Comp != 0)
                        {
                            s += "<LINEFEED>1</LINEFEED>";
                            s += "<PRINTLEFTRIGHT><LEFT>" + Ch2.CompName + " </LEFT>";
                            s += "<RIGHT> -" + Discount2.ToString("0.00") + "</RIGHT></PRINTLEFTRIGHT>";
                            if ((Ch2.CompId > 9) && (Ch2.CompId < 25))
                            {
                                s += "<PRINTLEFTRIGHT><LEFT>" + Ch2.DegustationMGR_NUMBER + "</LEFT><RIGHT> " + Ch2.CompDescription + "</RIGHT></PRINTLEFTRIGHT>";
                            }
                        }

                        if (Ch2.ServiceChargeSumm > 0)
                        {
                            s += "<LINEFEED>1</LINEFEED>";
                            s += "<PRINTLEFTRIGHT><LEFT>" + Ch2.ServiceChargeName + " </LEFT>";
                            s += "<RIGHT> " + Ch2.ServiceChargeSumm.ToString("0.00") + "</RIGHT></PRINTLEFTRIGHT>";

                        }

                        s += "<LINEFEED>1</LINEFEED>";
                        s += "<PRINTLEFTRIGHT><LEFT>Итог чека: </LEFT>";
                        s += "<RIGHT> " + sum2.ToString("0.00") + "</RIGHT></PRINTLEFTRIGHT>";
                        s += "<LINEFEED>1</LINEFEED>";

                        if (!String.IsNullOrWhiteSpace(Ch2.PMSGuestName))
                        {
                            s += "<PRINTLEFTRIGHT><LEFT>" + Ch2.PMSGuestName + "</LEFT>";
                            s += "<RIGHT> </RIGHT></PRINTLEFTRIGHT>";
                            s += "<LINEFEED>1</LINEFEED>";
                            s += "<PRINTLEFTRIGHT><LEFT>________________________________</LEFT>";
                            s += "<RIGHT> </RIGHT></PRINTLEFTRIGHT>";
                        }

                        AllSumm += sum2;
                    }
                    s += "<PRINTCENTERED>========================================</PRINTCENTERED>";


                    s += "<PRINTSTYLE><CPI>1</CPI><STYLE>1</STYLE></PRINTSTYLE>";
                    s += "<PRINTLEFTRIGHT><LEFT>Общий итог: </LEFT>";
                    s += "<RIGHT> " + AllSumm.ToString("0.00") + "</RIGHT></PRINTLEFTRIGHT>";
                    s += "<PRINTSTYLE><CPI>1</CPI><STYLE>0</STYLE></PRINTSTYLE>";

                    s += "<LINEFEED>1</LINEFEED>";

                    s += GetReklamaStr();
                    /*

                    s += "<PRINTCENTERED>1 марта - 1 июня участвуйте </PRINTCENTERED>";
                    s += "<PRINTCENTERED>в благотворительном проекте </PRINTCENTERED>";

                    s += "<PRINTCENTERED>Кофемании с фондами 'Вера' и 'Я Есть!'.</PRINTCENTERED>";
                    s += "<PRINTCENTERED>О подробностях Вам расскажут наши официанты.</PRINTCENTERED>";
                    */

                    /*
                    s += "<PRINTCENTERED>Gratuity not included and always </PRINTCENTERED>";
                    s += "<PRINTCENTERED>remains on guest’s discretion</PRINTCENTERED>";
                    s += "<PRINTCENTERED>Thank you</PRINTCENTERED>";
                     * */
                    s += "<PRINTCENTERED>Спасибо</PRINTCENTERED>";
                    s += "<LINEFEED>1</LINEFEED>";

                    s += "<LINEFEED>1</LINEFEED>";

                    //s += GetVisitsStr(Ch.AlohaCheckNum);
                    //List<int> DomDep = new List<int>() { 212, 213, 216, 217, 220, 230, 300, 240, };


                    if (!iniFile.NoReklama) s += GetReklamaString();


                }
                return s;
                /*
            string z = Ap.GetAllPrinters();
            Ap.PrintStream(s);
                 * */
            }
            catch (Exception e)
            {
                Utils.ToLog("FormStringPrintPredcheck " + e.Message);
                ShowMessage(e.Message);
                return "";
            }
        }
        static private int GetPercentOfPayment(decimal total, decimal points_payment)
        {
            
            int result = (int)Math.Round((points_payment * 100)/total);
            return result;

        }

        static private string GetVisitsStr(int CheckId)
        {

            int P = 0;
            int V = 0;
            string tmp = "";

            string PreCard = AlohaTSClass.GetDiscountAttr(CheckId, out P, out V);
            if (PreCard.Length > 4)
            {
                if (PreCard.Substring(0, 3).ToUpper() == "PRE")
                {
                    tmp += "<LINEFEED>1</LINEFEED>";
                    tmp += "<PRINTCENTERED>" + "На Вашу карту «Друг Кофемании» " + "</PRINTCENTERED>";




                    tmp += "<PRINTCENTERED>" + "начислен 1 визит!" + "</PRINTCENTERED>";
                    tmp += "<PRINTCENTERED>" + "Осталось " + V.ToString() + " посещений за " + P.ToString() + " " + Utils.GetDayWord(P) + "</PRINTCENTERED>";
                    tmp += "<PRINTCENTERED>" + "Номер карты " + PreCard.Substring(4) + "</PRINTCENTERED>";
                    tmp += "<LINEFEED>1</LINEFEED>";

                }
            }
            return tmp;

        }

        static private string GetReklamaString()
        {
            string Tmp = "";

            try
            {
                Utils.ToCardLog("GetReklamaString");
                IberEnumClass CompsEnum = (IberEnumClass)Depot.GetEnum(FILE_GCI);

                foreach (IberObject gci in CompsEnum)
                {
                    try
                    {
                        string name = gci.GetStringVal("NAME");
                        if (name.Contains("Active"))
                        {
                            int id = gci.GetLongVal("ID");
                            Utils.ToCardLog("GetReklamaString Find id: " + id.ToString());
                            for (int i = 1; i <= 12; i++)
                            {
                                string mess = gci.GetStringVal("MESSAGE" + i.ToString());
                                if ((mess != null) && (mess != ""))
                                {
                                    Tmp += "<PRINTCENTERED>" + mess + "</PRINTCENTERED>";
                                }
                            }

                            Tmp += "<LINEFEED>1</LINEFEED>";
                            Tmp += "<LINEFEED>1</LINEFEED>";

                        }
                    }
                    catch (Exception e)
                    {
                        Utils.ToCardLog("Error 1 GetReklamaString " + e.Message);
                    }
                }
            }
            catch (Exception e)
            {
                Utils.ToCardLog("Error 2 GetReklamaString " + e.Message);
            }

            /*
            //С 1 по 30 октября Вы можете воспользоваться 25% скидкой от действующего тарифа

            //на перелеты из Москвы в Гонконг Etihad Airways при бронировании на сайте Etihad.com

            //с помощью промокода TRAVEL2016
            
            List<int> DomDep = new List<int>() { 212, 213, 216, 217, 220, 230, 300, 240, };
            List<int> SochiDep = new List<int>() { 450,451};
            if ((DateTime.Now > new DateTime(2016, 9, 29) && DateTime.Now < new DateTime(2017, 04, 30)) && (!DomDep.Contains(AlohainiFile.DepNum)))
            {
                Tmp += "<PRINTCENTERED>" + "Ждем Вас на Матчафест в Кофемании  " + "</PRINTCENTERED>";
                Tmp += "<PRINTCENTERED>" + "на Б.Полянке 21 - 30 апреля! " + "</PRINTCENTERED>";

                Tmp += "<LINEFEED>1</LINEFEED>";
                Tmp += "<LINEFEED>1</LINEFEED>";
            }

            if ((DateTime.Now > new DateTime(2019, 3, 01) && DateTime.Now < new DateTime(2019, 06, 01)) 
                //&& (!SochiDep.Contains(AlohainiFile.DepNum))
                )
            {


                Tmp = "<PRINTCENTERED>1 марта - 1 июня участвуйте </PRINTCENTERED>";
                Tmp += "<PRINTCENTERED>в благотворительном проекте </PRINTCENTERED>";

                Tmp += "<PRINTCENTERED>Кофемании с фондами 'Вера' и 'Я Есть!'.</PRINTCENTERED>";
                Tmp += "<PRINTCENTERED>О подробностях Вам расскажут</PRINTCENTERED>";
                Tmp += "<PRINTCENTERED>наши официанты.</PRINTCENTERED>";
                Tmp += "<PRINTCENTERED>    </PRINTCENTERED>";
                Tmp += "<PRINTCENTERED>    </PRINTCENTERED>";
            }
            /*

            string Tmp = "";
            Tmp += "<PRINTCENTERED>" + "С 1 по 30 октября Вы можете использовать " + "</PRINTCENTERED>";
            Tmp += "<PRINTCENTERED>" + "25% скидку от действующего тарифа " + "</PRINTCENTERED>";
            Tmp += "<PRINTCENTERED>" + "на перелеты Etihad Airways из Москвы " + "</PRINTCENTERED>";

            Tmp += "<PRINTCENTERED>" + "в Гонконг при бронировании на сайте " + "</PRINTCENTERED>";
            Tmp += "<PRINTCENTERED>" + "Etihad.com с помощью промокода " + "</PRINTCENTERED>";
            Tmp += "<PRINTCENTERED>" + "TRAVEL2016" + "</PRINTCENTERED>";

            Tmp += "<LINEFEED>1</LINEFEED>";
            Tmp += "<LINEFEED>1</LINEFEED>";
             */

            return Tmp;
        }

        static string PrintINfoSavePath = @"C:\Aloha\check\Discount\Tmp\";
        static internal void CheckPrintINfo()
        {
            if (Directory.Exists(PrintINfoSavePath))
            {

                foreach (FileInfo fi in new DirectoryInfo(PrintINfoSavePath).GetFiles())
                {
                    Utils.ToCardLog("PrintINfo find save file");
                    try
                    {
                        using (StreamReader sr = new StreamReader(fi.FullName))
                        {
                            string s = sr.ReadLine();
                            string z = Ap.GetAllPrinters();

                            Utils.ToCardLog("PrintINfo from save file" + s);
                            if (s.Length > 20)
                            {
                                Ap.PrintStream(s); //Эта функция зависает при пустой строке
                            }
                        }
                        fi.Delete();
                    }
                    catch (Exception e)
                    {
                        Utils.ToCardLog("Error PrintINfo from save file " + e.Message);
                    }
                }
            }
        }

        static private void PrintINfo(string info)
        {
            CheckPrintINfo();
            string s = "";
            try
            {


                s = "<PRINT>";
                s += "<PRINTER>" + GetLocalPrinterNum().ToString() + "</PRINTER>";
                //  s += "<PRINTER>" + 0 + "</PRINTER>";
                s += "<COMMANDS>";
                //  s += "<SETCODEPAGE/>";
                s += info;
                s += "<POSTLINEFEEDS>4</POSTLINEFEEDS> ";
                s += "<CUT>FULL</CUT>";
                s += "<POSTLINEFEEDS>3</POSTLINEFEEDS> ";
                s += "</COMMANDS>";
                s += @"</PRINT>";
                string z = Ap.GetAllPrinters();

                Utils.ToCardLog("PrintINfo " + s);

                Ap.PrintStream(s);
            }
            catch (Exception e)
            {
                Utils.ToLog("Error PrintINfo " + e.Message);

                if (!Directory.Exists(PrintINfoSavePath))
                {
                    Directory.CreateDirectory(PrintINfoSavePath);
                }
                using (StreamWriter sw = new StreamWriter(PrintINfoSavePath + Guid.NewGuid().ToString()))
                {
                    Utils.ToLog("Save PrintINfo " + e.Message);
                    sw.WriteLine(s);
                }

                ShowMessage(e.Message);
            }
        }




        static internal int GetEmptyTable()
        {
            try
            {
                List<int> ListOfTable = iniFile.RemoteOrderTablesList;

                //Utils.ToLog("Беру енум 500: " );
                IberEnum EmployeeEnum = Depot.GetEnum(500);

                try
                {


                    //  Utils.ToLog("взял енум 500 беру его столы ");
                    IberEnum TablesEnum = EmployeeEnum.FindFromLongAttr("ID", iniFile.RemoteOrderWaterNumber).GetEnum(501);
                    // Utils.ToLog("взял столы ");

                    foreach (int Tn in iniFile.RemoteOrderTablesList)
                    {
                        try
                        {
                            IberObject Table2 = TablesEnum.FindFromLongAttr("TABLEDEF_ID", Tn);

                        }
                        catch (Exception e)
                        {
                            Utils.ToLog("Возвращаю свободный стол " + Tn);
                            return Tn;
                        }
                    }
                    Utils.ToLog("Все столы заняты");
                    return -1;
                }
                catch
                {
                    Utils.ToLog("Возвращаю первый свободный стол ");
                    return iniFile.RemoteOrderTablesList[0];
                }
            }
            catch (Exception ee)
            {
                Utils.ToLog("Error GetEmptyTable(): " + ee.Message);

                //Depot = new IberDepot();
                return -1;
            }
            /*
           foreach (int TNum in ListOfTable)
           {
               try
               {

                   int TableId = AlohaFuncs.AddTable(iniFile.RemoteOrderTerminalNumber, 0, TNum, "", 1);
                   Utils.ToLog("Получил пустой стол и открыл его " + TNum);
                   return TableId;
               }
               catch
               { }
           }

           Utils.ToLog("Все столы заняты");
               return -1;
            * */
        }
        static internal bool IsAlohaTS()
        {
            return AlohaFuncs.IsTableService();
        }


        static internal bool DefTableInUse(int TNum, out int CheckId, out int OpenTId)
        {
            CheckId = 0;
            OpenTId = 0;
            Utils.ToCardLog("Check DefTableInUse");
            IberEnum EmployeeEnum = Depot.GetEnum(500);
            try
            {
                IberEnum TablesEnum = EmployeeEnum.FindFromLongAttr("ID", AlohaCurentState.EmployeeNumberCode).GetEnum(501);
                try
                {

                    IberObject Table2 = TablesEnum.FindFromLongAttr("TABLEDEF_ID", TNum);
                    OpenTId = Table2.GetLongVal("ID");
                    try
                    {
                        foreach (IberObject ch in Table2.GetEnum(INTERNAL_TABLES_OPEN_CHECKS))
                        {
                            CheckId = ch.GetLongVal("ID");
                        }
                    }
                    catch
                    { }
                    Utils.ToCardLog("DefTableInUse true");
                    return true;
                }
                catch (Exception e)
                {
                    Utils.ToCardLog("DefTableInUse false");
                    return false;
                }
            }
            catch
            {
                return false;
            }

        }
        static internal int OpenEmptyCheck()
        {

            int TableNumber = 0;
            string TableName = "";
            try
            {
                int QueueId = 0;
                int UnikNum = 0;
                if (!IsAlohaTS())
                {
                    QueueId = GetQueue(GetTermNum());
                    //TableNumber = AlohaFuncs.GetNewOrder(TermID);
                    TableNumber = GetLastQueue(GetTermNum()) + 1;
                    TableName = TableNumber.ToString();
                    Utils.ToLog("OpenEmptyCheck TableName = " + TableName);
                    UnikNum = AlohaFuncs.AddTable(GetTermNum(), QueueId, TableNumber, TableName, 1);
                }
                else
                {
                    bool Sucssess = false;
                    int CurentId = 0;
                    while ((!Sucssess) && (CurentId <= iniFile.DefaultTableForBartender.Count - 1))
                    {
                        try
                        {
                            TableNumber = iniFile.DefaultTableForBartender[CurentId];
                            Utils.ToLog("OpenEmptyCheck Открываю стол " + TableNumber);
                            UnikNum = AlohaFuncs.AddTable(GetTermNum(), QueueId, TableNumber, TableName, 1);
                            Sucssess = true;
                        }
                        catch (Exception e)
                        {
                            Utils.ToLog("[Error] OpenEmptyCheck Открываю стол " + e.Message);
                            CurentId++;
                        }
                    }
                    if (!Sucssess)
                    {
                        Utils.ToLog("[error] OpenEmptyCheck Все столы заняты");
                    }
                    //TableNumber = iniFile.DefaultTableForBartender;
                    TableName = "Fast Close";
                }

                Utils.ToLog("AlohaFuncs.AddTable " + UnikNum);
                if (IsAlohaTS())
                {
                    int TId = AlohaFuncs.AddCheck(GetTermNum(), UnikNum);
                    AlohaFuncs.RefreshCheckDisplay();
                    Utils.ToLog("AlohaFuncs.AddCheck " + TId);
                    return TId;
                }
                //AlohaFuncs.RefreshCheckDisplay();
                return UnikNum;
            }
            catch (Exception e)
            {
                Utils.ToLog("OpenCheck Error " + e.Message);
            }
            return -1;

        }

        static internal int OpenTable(int TNum, string OrderId, out int OrderedTable)
        {
            List<int> TList = iniFile.RemoteOrderTablesList;
            int k = 0;
            int TId = 0;
            OrderedTable = TNum;
            do
            {
                try
                {
                    /*
                    Utils.ToLog("Заказал на стол № " + TNum);
                    if (OrderId != "")
                    {
                        TId = AlohaFuncs.AddTable(iniFile.RemoteOrderTerminalNumber, 0, TNum, "Заказ № " + OrderId, 1);
                    }
                    else
                    {
                     * */
                    Utils.ToLog("OrderId=" + OrderId + "=");
                    TId = AlohaFuncs.AddTable(iniFile.RemoteOrderTerminalNumber, 0, TNum, "", 1);
                    //}
                    Utils.ToLog("Добавил на стол № " + TNum);
                    return TId;
                }
                catch
                {
                    TList.Remove(TNum);
                    if (TList.Count > 0)
                    {
                        TNum = TList[0];
                        OrderedTable = TNum;
                    }
                    else
                    {
                        TNum = -1;
                    }
                }

            } while (!((TId == 0) && (TNum == -1)));
            return -1;
        }



        static internal void SetManagerOverride(int TermID, int EmpId)
        {
            try
            {
                Utils.ToLog("AuthorizeOverrideMgr EmpId: " + EmpId + " TermID: " + TermID);
                AlohaFuncs.AuthorizeOverrideMgr(TermID, EmpId, "", "");
            }
            catch (Exception e)
            {
                Utils.ToLog("Error AuthorizeOverrideMgr " + e.Message);
            }

        }

        static internal void ApplyFullPaymentToCurentChk(int TenderId)
        {
            Utils.ToCardLog("ApplyFullPaymentToCurentChk ");
            try
            {
                Utils.ToCardLog("ApplyFullPaymentToCurentChk OrderAll");
                AlohaFuncs.DeselectAllEntries(AlohaCurentState.TerminalId);
                AlohaFuncs.SelectAllEntriesOnCheck(AlohaCurentState.TerminalId, (int)AlohaCurentState.CheckId);
                AlohaFuncs.OrderItems(AlohaCurentState.TerminalId, (int)AlohaCurentState.TableId, 2);
                AlohaFuncs.DeselectAllEntries(AlohaCurentState.TerminalId);
            }
            catch (Exception e)
            {
                Utils.ToCardLog("Error ApplyFullPaymentToCurentChk OrderAll " + e.Message);
            }

            try
            {

                double Total = 0;
                double Tax = 0;
                AlohaFuncs.GetCheckTotal((int)AlohaCurentState.CheckId, out Total, out Tax);
                AlohaFuncs.ApplyPayment(AlohaCurentState.TerminalId, (int)AlohaCurentState.CheckId, TenderId, Total, 0, "", "", "", "");
            }
            catch (Exception e)
            {
                Utils.ToCardLog("[Error] ApplyFullPaymentToCurentChk " + e.Message);
            }
        }


        static internal void ApplyPayment(int TermID, int CheckID, double Summ, int Type)
        {
            try
            {
                Utils.ToCardLog("ApplyPayment TermID " + TermID + " CheckID " + CheckID + " Summ " + Summ + " Type " + Type);
                AlohaFuncs.ApplyPayment(TermID, CheckID, Type, Summ, 0, "", "", "", "");
            }
            catch (Exception e)
            {
                Utils.ToCardLog("[Error] ApplyPayment TermID " + TermID + " CheckID " + CheckID + " Summ " + Summ + " Type " + Type + " " + e.Message);
            }
        }


        static internal void DeletePayments(Check Chk, int TermId)
        {
            try
            {
                foreach (int PN in Chk.PaymentsIds)
                {
                    AlohaFuncs.DeletePayment(TermId, Chk.AlohaCheckNum, PN);
                }
            }
            catch (Exception e)
            {
                Utils.ToCardLog("[Error] DeletePaymentPayment TermID " + e.Message);
            }
        }


        static internal void ApplyPayment(int TermID, int CheckID, int TableId, double Summ, int Type)
        {
            try
            {
                AlohaFuncs.ApplyPayment(TermID, CheckID, Type, Summ, 0, "", "", "", "");
            }
            catch (Exception e)
            {
                Utils.ToCardLog("[Error] ApplyPayment TermID " + TermID + " CheckID " + CheckID + " TableId " + TableId + " Summ " + Summ + " " + e.Message);
            }
        }


        static internal void CloseTable(int TermID, int TableID)
        {
            try
            {
                Utils.ToCardLog("CloseTable  " + TableID);
                IberObject Tbl = Depot.FindObjectFromId(INTERNAL_TABLES, TableID).First();
                Utils.ToCardLog("Depot.FindObjectFromId  " + TableID);
                int OpenChecksCount = 0;
                try
                {
                    IIberEnum Cheks = Tbl.GetEnum(INTERNAL_TABLES_OPEN_CHECKS);
                    OpenChecksCount = Cheks.Count;
                }
                catch
                {
                    Utils.ToCardLog("Открытых чеков нет  " + TableID);
                }
                if (OpenChecksCount > 0)
                {
                    Utils.ToCardLog("На столе есть открытые чеки закрывать не буду.");
                    return;
                }
                AlohaFuncs.CloseTable(TermID, TableID);
            }
            catch (Exception e)
            {
                Utils.ToCardLog("[Error] CloseTable  " + e.Message);
            }
        }

        static internal bool CheckIsClosed(int ChId)
        {
            bool Tmp = true;
            Utils.ToCardLog("CheckIsClosed ? " + ChId);
            try
            {
                IberEnumClass ChEnum = (IberEnumClass)Depot.FindObjectFromId(INTERNAL_CHECKS, ChId);
                IberObjectClass Ch = (IberObjectClass)ChEnum.First();
                Tmp = Ch.GetLongVal("CLOSED") == 1;
                if (Ch.GetLongVal("CLOSED") == 1)
                {
                    Utils.ToCardLog("CheckIsClosed = 1 " + ChId);
                }
                else
                {
                    Utils.ToCardLog("CheckIsClosed = 0 " + ChId);
                }

            }
            catch (Exception e)
            {
                Utils.ToCardLog("Error CheckIsClosed ? " + ChId + "  " + e.Message);

            }

            return Tmp;

        }

        static internal void CloseCurentCheckAndTableByCurentUser()
        {
            Utils.ToCardLog("CloseCurentCheckAndTableByCurentUser ");
            try
            {
                AlohaFuncs.CloseCheck(GetTermNum(), (int)AlohaCurentState.CheckId);
                AlohaFuncs.CloseTable(GetTermNum(), (int)AlohaCurentState.TableId);
                AlohaFuncs.RefreshCheckDisplay();
                if (IsAlohaTS())
                {
                    AlohaFuncs.LogOut(GetTermNum());
                }
            }
            catch (Exception e)
            {
                Utils.ToCardLog("Error CloseCurentCheckAndTableByCurentUser " + e.Message);
            }

        }

        static internal void CloseCheck(int TermID, int CheckID)
        {
            try
            {
                Utils.ToCardLog(String.Format("[CloseCheck]  Direct CheckID: {0} TermId: {1}", CheckID, TermID));
                AlohaFuncs.CloseCheck(TermID, CheckID);
            }
            catch (Exception e)
            {
                Utils.ToCardLog("[Error] CloseCheck  " + e.Message);
            }
        }


        static internal bool OrderAllDishez(int TermID, int CheckID, int TableId, int OrderMode = 1)
        {
            try
            {
                AlohaFuncs.SelectAllEntriesOnCheck(TermID, CheckID);

            }
            catch (Exception ee)
            {
                Utils.ToCardLog("[Error] OrderAllDishez SelectAllEntriesOnCheck TermID = " + TermID.ToString() + "TableId = " + TableId.ToString() + ee.Message);
            }

            bool sucsess = false;
            int TryCount = 0;
            while (!sucsess && TryCount < 5)
            {
                try
                {
                    AlohaFuncs.OrderItems(TermID, TableId, OrderMode);
                    sucsess = true;
                }
                catch (Exception e)
                {
                    TryCount++;
                    Utils.ToCardLog("[Error] OrderAllDishez TermID = " + TermID.ToString() + "TableId = " + TableId.ToString() + "trycount " + TryCount.ToString() + e.Message);
                    Thread.Sleep(1000);
                }
            }
            AlohaFuncs.DeselectAllEntries(TermID);
            return sucsess;

        }



        private const int FILE_TRM = 33;

        static internal int GetQueue(int TermNum)
        {
            //IberEnum IntTrm = (IberEnum)Depot.FindObjectFromId(FILE_TRM, TermNum );
            IberEnum IntTrms = (IberEnum)Depot.GetEnum(FILE_TRM);
            foreach (IberObject Trm in IntTrms)
            {
                if (Trm.GetLongVal("ID") == TermNum)
                {
                    int res = Trm.GetLongVal("QUEUE");
                    return res;
                }
            }
            return 0;
        }

        static internal string GetCurentPrtName()
        {
            //IberEnum IntTrm = (IberEnum)Depot.FindObjectFromId(FILE_TRM, TermNum );
            try
            {

                Utils.ToLog("GetCurentPrtName ");
                int TermNum = GetTermNum();
                IberEnum IntTrms = (IberEnum)Depot.GetEnum(FILE_TRM);
                foreach (IberObject Trm in IntTrms)
                {
                    if (Trm.GetLongVal("ID") == TermNum)
                    {
                        int PrtId = Trm.GetLongVal("PRINTER");
                        IberEnum IntPrts = (IberEnum)Depot.GetEnum(FILE_PRT);
                        foreach (IberObject Prt in IntPrts)
                        {
                            if (Prt.GetLongVal("ID") == PrtId)
                            {
                                string res = Prt.GetStringVal("NETNAME");
                                Utils.ToLog("GetCurentPrtName " + PrtId.ToString() + "  " + res);
                                return res;
                            }
                        }

                    }
                }

            }
            catch (Exception e)
            {
                Utils.ToLog("Error GetCurentPrtName " + e.Message);
            }
            return "";
        }


        private const int INTERNAL_QUEUES = 640;
        static internal int GetLastQueue(int TermID)
        {
            IberEnum IntQueues = (IberEnum)Depot.GetEnum(INTERNAL_QUEUES);
            foreach (IberObject IntQueue in IntQueues)
            {
                if (IntQueue.GetLongVal("ID") == GetQueue(TermID))
                {
                    return IntQueue.GetLongVal("LAST_ORDER_NUMBER");
                }
            }
            return 0;
        }




        static internal void ApplyCompExternal(AlohaExternal.ApplyDiscountsRequest Request, AlohaExternal.ApplyDiscountsResponse Resp)
        {
            if (!LoginExternal(Resp))
            {
                Resp.Success = false;
                return;
            }
            foreach (AlohaExternal.AlohaDiscountInfo AP in Request.Discounts)
            {
                try
                {
                    int i = AlohaFuncs.ApplyComp(iniFile.ExternalInterfaceTerminal, iniFile.ExternalInterfaceEmployee, Request.AlohaCheckId, AP.Id, 0, "", AP.Name);
                    AP.AlohaId = i;
                    Resp.AddedDiscounts.Add(AP);
                }
                catch (Exception e)
                {
                    AP.Success = false;
                    AP.AlohaErrorCode = CAlohaErrors.GetAlohaErrorVal(e.Message);
                    AP.ErrorMsg = e.Message;
                    Resp.ErrorDiscounts.Add(AP);
                    Resp.Success = false;
                    Resp.IntegrationErrorCode = -3;
                    Resp.ErrorMsg = "Error in child data";

                }
            }
            LogOut(iniFile.ExternalInterfaceTerminal);
        }
        static internal void AddPaymentExternal(AlohaExternal.AddPaymentsRequest Request, AlohaExternal.AddPaymentsResponse Resp)
        {
            if (!LoginExternal(Resp))
            {
                Resp.Success = false;
                return;
            }
            foreach (AlohaExternal.AlohaPaymentInfo AP in Request.Payments)
            {
                try
                {
                    int i = AlohaFuncs.ApplyPayment(iniFile.ExternalInterfaceTerminal, Request.AlohaCheckId, AP.Id, (double)AP.Summ, (double)AP.Tip, "", "", "", "");
                    AP.AlohaId = i;
                    Resp.AddedDiscounts.Add(AP);
                }
                catch (Exception e)
                {
                    AP.Success = false;
                    AP.AlohaErrorCode = CAlohaErrors.GetAlohaErrorVal(e.Message);
                    AP.ErrorMsg = e.Message;
                    Resp.ErrorDiscounts.Add(AP);
                    Resp.Success = false;
                    Resp.IntegrationErrorCode = -3;
                    Resp.ErrorMsg = "Error in child data";
                }
            }
            LogOut(iniFile.ExternalInterfaceTerminal);
        }


        static private int GetUnknownMod(int modId, int dishId)
        {
            return 0;
        }


        static internal void OpenTableFromRangeExternal(AlohaExternal.NewOrderRequest Request, AlohaExternal.NewOrderResponse Resp)
        {
            Utils.ToCardLog($"TOpenTableFromRangeExternal {Request?.TableRangeId}");
            if (!LoginExternal(Resp))
            {
                Resp.Success = false;
                return;
            }

            if (Request.TableRangeId > 0)
            {
                try
                {
                    if (Request.TableRangeId != 0)
                    {
                        List<int> Tables = new List<int>();
                        if (Request.TableRangeId == 1)
                        {
                            Tables = iniFile.AddOrderFromExtRange1;
                            if (Tables.Count == 0)
                            {
                                Utils.ToCardLog("Tables.Count == 0");
                                Resp.Success = false;
                                return;
                            }
                        }
                        else if  (!AlohaTSClass.IsAlohaTS())
                        {


                            var QueueId = GetQueue(iniFile.ExternalInterfaceTerminal);
                            Utils.ToLog("TOpenTableFromRangeExternal QS QueueId = " + QueueId);
                            var TableNumber = GetLastQueue(iniFile.ExternalInterfaceTerminal) + 1;

                            Tables.Add(TableNumber);
                            Utils.ToLog("TOpenTableFromRangeExternal QS TableNumber= " + TableNumber);
                            //var TableName = TableNumber.ToString();
                            //Utils.ToLog("OpenEmptyCheck TableName = " + TableName);
                           // UnikNum = AlohaFuncs.AddTable(GetTermNum(), QueueId, TableNumber, TableName, 1);
                        }

                        else if (Request.TableRangeId == 2)//Филиас онлайн авто 
                        {

                            for (int i = 185; i < 196; i++)
                            {
                                Tables.Add(i);
                            }
                            for (int i = 920; i < 930; i++)
                            {
                                Tables.Add(i);
                            }
                            for (int i = 951; i < 960; i++)
                            {
                                Tables.Add(i);
                            }

                        }
                        else if (Request.TableRangeId == 3)//Филиас онлайн пешком
                        {
                            for (int i = 180; i < 185; i++)
                            {
                                Tables.Add(i);
                            }
                           

                        }
                        else if (Request.TableRangeId == 4)//Филиас онлайн самовынос
                        {
                            for (int i = 196; i < 200; i++)
                            {
                                Tables.Add(i);
                            }
                            /**/
                            for (int i = 930; i <= 934; i++)
                            {
                                Tables.Add(i);
                            }
                            
                                
                        }
                        
                        else if (Request.TableRangeId == 5)//Яндекс
                        {
                            for (int i = 231; i <= 240; i++)
                            {
                                Tables.Add(i);
                            }
                            for (int i = 169; i <= 173; i++)
                            {
                                Tables.Add(i);
                            }
                            for (int i = 900; i <= 910; i++)
                            {
                                Tables.Add(i);
                            }

                        }
                        else if (Request.TableRangeId == 6)//Яндекс самовынос
                        {
                            for (int i = 163; i <= 168; i++)
                            {
                                Tables.Add(i);
                            }
                        }
                        else if (Request.TableRangeId == 7)//Delivery club 
                        {
                            for (int i = 200; i <= 207; i++)
                            {
                                Tables.Add(i);
                            }
                        }

                        else if (Request.TableRangeId == 8)//Delivery club самовынос
                        {
                            for (int i = 208; i <= 209; i++)
                            {
                                Tables.Add(i);
                            }
                        }

                        else if (Request.TableRangeId == 9)//ИМ с оплатой на месте
                        {
                            for (int i = 935; i <= 940; i++)
                            {
                                Tables.Add(i);
                            }
                        }
                        else if (Request.TableRangeId == 10)//ООО «ББ-АГЕНТ»
                        {
                            for (int i = 945; i <= 950; i++)
                            {
                                Tables.Add(i);
                            }
                        }
                        else if (Request.TableRangeId == 11)//Салют СберБанк
                        {
                            for (int i = 260; i <= 262; i++)
                            {
                                Tables.Add(i);
                            }
                        }
                        else if (Request.TableRangeId == 12)//My Ply
                        {
                            for (int i = 174; i <= 176; i++)
                            {
                                Tables.Add(i);
                            }
                        }
                        foreach (int TableNum in Tables)
                            {
                                try
                                {
                                    Utils.ToCardLog("TableNum=" + TableNum);
                                    try
                                    {

                                        if (!IsAlohaTS())
                                        {
                                             Request.TableName = TableNum.ToString();
                                        }

                                        Request.AlohaTableId = AlohaFuncs.AddTable(iniFile.ExternalInterfaceTerminal, Request.QueueId, TableNum, Request.TableName, Request.NumGuest);
                                        Request.TableNumber = TableNum;
                                        Resp.TableNum = TableNum;

                                        Utils.ToCardLog("AlohaFuncs.AddTable ok" );
                                }
                                    catch (Exception e)
                                    {
                                        Utils.ToCardLog("Error AddTable" + e.Message);
                                        Request.AlohaTableId = 0;
                                    }
                                    if (Request.AlohaTableId > 0)
                                    {
                                        try
                                        {

                                            Request.AlohaCheckId = AlohaFuncs.AddCheck(iniFile.ExternalInterfaceTerminal, Request.AlohaTableId);
                                            Resp.AlohaId = Request.AlohaCheckId;
                                        Utils.ToCardLog("AlohaFuncs.AddCheck ok");
                                    }
                                        catch (Exception e)
                                        {
                                            Utils.ToCardLog("Error AddCheck" + e.Message);
                                            Resp.Success = false;
                                            Resp.AlohaErrorCode = CAlohaErrors.GetAlohaErrorVal(e.Message);
                                            Resp.ErrorMsg = e.Message;
                                            return;
                                        }
                                        if (Request.TableRangeId == 1)
                                        {
                                            try
                                            {
                                                int rsi = AlohaFuncs.BeginItem(iniFile.ExternalInterfaceTerminal, Request.AlohaCheckId, iniFile.ExternalInterfaceRange1BarCode, iniFile.ExternalInterfaceRange1Name + " " + Request.TableName, -999999999.0);
                                                AlohaFuncs.EndItem(iniFile.ExternalInterfaceTerminal);
                                                AlohaFuncs.DeselectAllEntries(iniFile.ExternalInterfaceTerminal);
                                                AlohaFuncs.SelectEntry(iniFile.ExternalInterfaceTerminal, Request.AlohaCheckId, rsi);
                                                AlohaFuncs.OrderItems(iniFile.ExternalInterfaceTerminal, Request.AlohaTableId, iniFile.ExternalInterfaceRange1OrderMode);
                                                AlohaFuncs.DeselectAllEntries(iniFile.ExternalInterfaceTerminal);
                                            }
                                            catch (Exception e)
                                            {
                                                Utils.ToCardLog("Error AddRoomSrv dish " + e.Message);
                                            }
                                        }


                                    // это исправление ошибки
                                    //There was no order number assigned by master – This is QS only

                                    
                                        try
                                        {
                                        if (!IsAlohaTS())
                                        {
                                            int count = 0;
                                            while (count<5){

                                                try

                                                {
                                                    Utils.ToCardLog("try add dish QS count "+ count);

                                                    AlohaExternal.AlohaItemInfo itm = Request.Items.First();
                                                    double Price = (double)itm.Price;
                                                    int pId = AlohaFuncs.BeginItem(iniFile.ExternalInterfaceTerminal, Request.AlohaCheckId, itm.Barcode, "", Price);
                                                    AlohaFuncs.VoidItem(iniFile.ExternalInterfaceTerminal, Request.AlohaCheckId, pId, 1);
                                                    count = 5;
                                                }
                                                catch
                                                {
                                                    Thread.Sleep(1000);
                                                    count++;
                                                }
                                            }
                                        }
                                        }
                                        catch(Exception e)
                                        {
                                        


                                            Utils.ToCardLog("AddDish try fo qs error " +e.Message);
                                        }
                                        
                                    


                                        foreach (AlohaExternal.AlohaItemInfo itm in Request.Items)
                                        {
                                            Utils.ToCardLog("AddDish " + itm.Barcode + " " + itm.Name);
                                            double Price = (double)itm.Price;
                                            if (Price == -1)
                                            {
                                                Price = -999999999.0;
                                            }
                                            try
                                            {

                                                int pId = AlohaFuncs.BeginItem(iniFile.ExternalInterfaceTerminal, Request.AlohaCheckId, itm.Barcode, "", Price);
                                            Utils.ToCardLog("AddDish ok" + itm.Barcode + " " + itm.Name);
                                            bool modOk = true;
                                                if (itm.Mods != null)
                                                {
                                                    foreach (var mod in itm.Mods.Where(a => a.Barcode > 0))
                                                    {
                                                        Utils.ToCardLog("AddMod " + mod.Barcode + " " + mod.Name);

                                                        double mPrice = (double)mod.Price;
                                                        if (mPrice == -1)
                                                        {
                                                            mPrice = -999999999.0;
                                                        }
                                                        try
                                                        {
                                                            AlohaFuncs.ModItem(iniFile.ExternalInterfaceTerminal, pId, mod.Barcode, "", mPrice, 0);
                                                        }
                                                        catch (Exception ee)
                                                        {
                                                            Resp.Success = false;
                                                            itm.Success = false;
                                                            var err = new CAlohaErrors(ee.Message);
                                                            mod.AlohaErrorCode = err.Val;
                                                            mod.ErrorMsg = err.ValStr;
                                                            Resp.ErrorItems.Add(mod);
                                                            modOk = false;
                                                            Utils.ToCardLog("Error AddModd " + mod.Barcode + " " + ee.Message);
                                                        }
                                                    }
                                                }

                                                var chunkSize = 14;
                                                foreach (var mod in itm.Mods.Where(a => a.Barcode == 0))
                                                {
                                                    try
                                                    {
                                                        var result = (from Match m in Regex.Matches(mod.Name, @".{1," + chunkSize + "}")
                                                                      select m.Value).ToList();
                                                        foreach (string ss in result)
                                                        {
                                                            AlohaFuncs.ModItem(iniFile.ExternalInterfaceTerminal, pId, 999902, ss, -999999999.000000, 0);
                                                        }
                                                    }
                                                    catch (Exception eM)
                                                    {
                                                        Utils.ToCardLog("Error AddDish in itm.Mods " + itm.Barcode + " " + eM.Message);
                                                    }
                                                }
                                            Utils.ToCardLog("AddDishMods ok" + itm.Barcode + " " + itm.Name);
                                            if (itm.Comment?.Length > 0)
                                                {
                                                    try
                                                    {
                                                        var result = (from Match m in Regex.Matches(itm.Comment, @".{1," + chunkSize + "}")
                                                                      select m.Value).ToList();

                                                        foreach (string ss in result)
                                                        {
                                                            AlohaFuncs.ModItem(iniFile.ExternalInterfaceTerminal, pId, 999902, ss, -999999999.000000, 0);
                                                        }
                                                    }
                                                    catch (Exception eM)
                                                    {
                                                        Utils.ToCardLog("Error AddDish in Comment " + itm.Barcode + " " + eM.Message);
                                                    }
                                                }
                                            Utils.ToCardLog("AlohaFuncs.AddComment ok");
                                            itm.Success = modOk;
                                                if (modOk)
                                                {
                                                    AlohaFuncs.EndItem(iniFile.ExternalInterfaceTerminal);
                                                    Resp.AddedItems.Add(itm);

                                                }
                                                else
                                                {

                                                    itm.ErrorMsg = "Не смог добавить модификаторы";
                                                    Resp.Success = false;
                                                    itm.Success = false;
                                                    //var err = new CAlohaErrors(e.Message);
                                                    //itm.AlohaErrorCode = -5;
                                                    itm.ErrorMsg = "Не смог добавить модификаторы";
                                                    Resp.ErrorItems.Add(itm);
                                                    Utils.ToCardLog("Error AddDish " + itm.Barcode + " Не смог добавить модификаторы ");
                                                }


                                            }
                                            catch (Exception e)
                                            {
                                                Resp.Success = false;
                                                itm.Success = false;
                                                var err = new CAlohaErrors(e.Message);
                                                itm.AlohaErrorCode = err.Val;
                                                itm.ErrorMsg = err.ValStr;
                                                Resp.ErrorItems.Add(itm);
                                                Utils.ToCardLog("Error AddDish " + itm.Barcode + " " + e.Message);
                                            }
                                        }


                                        if (Request.DiscountId != 0)
                                        {
                                            var dres = ApplyComp(Request.DiscountId, "", out string ErMessage);
                                            if (dres == 0)
                                            {
                                                Resp.Success = false;
                                                var err = new CAlohaErrors(ErMessage);
                                                Resp.AlohaErrorCode = err.Val;
                                                Resp.ErrorMsg = err.ValStr;

                                            }
                                        }


                                    try
                                    {
                                        Utils.ToCardLog("Request.SendToKitchenOrderType  " + Request.SendToKitchenOrderType);
                                        if (!IsAlohaTS())
                                        {
                                            AlohaFuncs.SelectAllEntriesOnCheck(iniFile.ExternalInterfaceTerminal, Request.AlohaCheckId);
                                            AlohaFuncs.OrderItems(iniFile.ExternalInterfaceTerminal, (int)Request.AlohaTableId, Request.SendToKitchenOrderType);
                                            AlohaFuncs.DeselectAllEntries(iniFile.ExternalInterfaceTerminal);
                                        }
                                        else
                                        {
                                            if (Request.SendToKitchenOrderType > 0)
                                            {
                                                AlohaFuncs.SelectAllEntriesOnCheck(iniFile.ExternalInterfaceTerminal, Request.AlohaCheckId);
                                                AlohaFuncs.OrderItems(iniFile.ExternalInterfaceTerminal, (int)AlohaCurentState.TableId, Request.SendToKitchenOrderType);
                                                AlohaFuncs.DeselectAllEntries(iniFile.ExternalInterfaceTerminal);
                                            }
                                        }
                                    }
                                    catch (Exception ee)
                                    {
                                        Utils.ToCardLog("Error OpenTableFromExternal OrderItems " + ee.Message);
                                    }
                                    /*
                                    try
                                    {
                                        Utils.ToCardLog("Request.SendToKitchenOrderType  " + Request.SendToKitchenOrderType);
                                        if (Request.SendToKitchenOrderType > 0)
                                        {
                                            AlohaFuncs.SelectAllEntriesOnCheck(iniFile.ExternalInterfaceTerminal, Request.AlohaCheckId);
                                            AlohaFuncs.OrderItems(iniFile.ExternalInterfaceTerminal, (int)AlohaCurentState.TableId, Request.SendToKitchenOrderType);
                                            AlohaFuncs.DeselectAllEntries(iniFile.ExternalInterfaceTerminal);
                                        }
                                    }
                                    catch (Exception ee)
                                    {
                                        Utils.ToCardLog("Error OpenTableFromExternal OrderItems " + ee.Message);
                                    }
                                    */

                                    LogOut();
                                        return;

                                    }
                                }
                                catch (Exception e)
                                {

                                    var err = new CAlohaErrors(e.Message);
                                    if ((e.Message.Length < 3) || (e.Message.Substring(e.Message.Length - 2, 2) != "32"))
                                    //if (err.Val != AlohaErrEnum.ErrCOM_TableInUse)
                                    {
                                        Utils.ToCardLog("Error no one AddTables " + e.Message);
                                        Resp.ErrorMsg = err.ValStr;
                                        Resp.AlohaErrorCode = err.Val;
                                        return;
                                    }
                                    else
                                    {
                                        Utils.ToCardLog("Error AddTable " + e.Message);

                                    }

                                }
                            }
                        

                    }
                    else
                    {
                        Resp.Success = false;
                        Resp.ErrorMsg = "Invalid TableRangeId";
                        LogOut(iniFile.ExternalInterfaceTerminal);
                        return;
                    }
                }
                catch (Exception e)
                {
                    var err = new CAlohaErrors(e.Message);
                    Utils.ToCardLog("Error TOpenTableFromRangeExternal" + e.Message);
                    Resp.ErrorMsg = err.ValStr;
                    Resp.AlohaErrorCode = err.Val;
                    LogOut(iniFile.ExternalInterfaceTerminal);
                    return;

                }
                Resp.Success = false;
                Utils.ToCardLog("Error TOpenTableFromRangeExternal all tables in use");
                Resp.ErrorMsg = "В данном диапазоне нет свободных столов";
                Resp.IntegrationErrorCode = -2;
                LogOut(iniFile.ExternalInterfaceTerminal);
                return;
            }

        }



        static internal void OpenTableFromExternal(AlohaExternal.NewOrderRequest Request, AlohaExternal.NewOrderResponse Resp)
        {


            if (Request.AlohaTableId == 0)
            {
                if (!LoginExternal(Resp,Request.EmplId))
                {
                    Resp.Success = false;
                    return;
                }

                try
                {
                    Utils.ToCardLog($"OpenTableFromExternal AddTable iniFile.ExternalInterfaceTerminal: {iniFile.ExternalInterfaceTerminal}, " +
                        $"Request.QueueId:{Request.QueueId}, Request.TableNumber:{Request.TableNumber}, Request.TableName:{Request.TableName}, Request.NumGuest:{Request.NumGuest}");

                    Request.AlohaTableId = AlohaFuncs.AddTable(iniFile.ExternalInterfaceTerminal, Request.QueueId, Request.TableNumber, "", Request.NumGuest);
                    Resp.TableId = Request.AlohaTableId;
                    Resp.TableNum = Request.TableNumber;


                }
                catch (Exception e)
                {
                    Utils.ToCardLog($"OpenTableFromExternal AddTable Error" + e.Message);
                    Resp.Success = false;
                    Resp.AlohaErrorCode = CAlohaErrors.GetAlohaErrorVal(e.Message);
                    Resp.ErrorMsg = e.Message;
                    return;
                }
            }

            try
            {
                Utils.ToCardLog($"OpenTableFromExternal AddCheck iniFile.ExternalInterfaceTerminal: {iniFile.ExternalInterfaceTerminal}; Request.AlohaTableId:{Request.AlohaTableId}");
                Request.AlohaCheckId = AlohaFuncs.AddCheck(iniFile.ExternalInterfaceTerminal, Request.AlohaTableId);
                Resp.CheckId = Request.AlohaCheckId;
            }
            catch (Exception e)
            {
                Utils.ToCardLog($"Error OpenTableFromExternal.AlohaFuncs.AddCheck {e.Message}");
                Resp.Success = false;
                Resp.AlohaErrorCode = CAlohaErrors.GetAlohaErrorVal(e.Message);
                Resp.ErrorMsg = e.Message;
                return;
            }
            foreach (AlohaExternal.AlohaItemInfo itm in Request.Items)
            {
                Utils.ToCardLog("AddDish " + itm.Barcode + " " + itm.Name);
                double Price = (double)itm.Price;
                if (Price == -1)
                {
                    Price = -999999999.0;
                }
                try
                {
                    int pId = AlohaFuncs.BeginItem(iniFile.ExternalInterfaceTerminal, Request.AlohaCheckId, itm.Barcode, "", Price);
                    bool modOk = true;
                    if (itm.Mods != null)
                    {
                        foreach (var mod in itm.Mods.Where(a => a.Barcode > 0))
                        {
                            Utils.ToCardLog("AddMod " + mod.Barcode + " " + mod.Name);

                            double mPrice = (double)mod.Price;
                            if (mPrice == -1)
                            {
                                mPrice = -999999999.0;
                            }
                            try
                            {
                                AlohaFuncs.ModItem(iniFile.ExternalInterfaceTerminal, pId, mod.Barcode, "", mPrice, 0);
                            }
                            catch (Exception ee)
                            {
                                Resp.Success = false;
                                itm.Success = false;
                                var err = new CAlohaErrors(ee.Message);
                                mod.AlohaErrorCode = err.Val;
                                mod.ErrorMsg = err.ValStr;
                                Resp.ErrorItems.Add(mod);
                                modOk = false;
                                Utils.ToCardLog("Error AddModd " + mod.Barcode + " " + ee.Message);
                            }
                        }
                    }

                    var chunkSize = 14;
                    foreach (var mod in itm.Mods.Where(a => a.Barcode == 0))
                    {
                        try
                        {
                            var result = (from Match m in Regex.Matches(mod.Name, @".{1," + chunkSize + "}")
                                          select m.Value).ToList();
                            foreach (string ss in result)
                            {
                                AlohaFuncs.ModItem(iniFile.ExternalInterfaceTerminal, pId, 999902, ss, -999999999.000000, 0);
                            }
                        }
                        catch (Exception eM)
                        {
                            Utils.ToCardLog("Error AddDish in itm.Mods " + itm.Barcode + " " + eM.Message);
                        }
                    }

                    if (itm.Comment?.Length > 0)
                    {
                        try
                        {
                            var result = (from Match m in Regex.Matches(itm.Comment, @".{1," + chunkSize + "}")
                                          select m.Value).ToList();

                            foreach (string ss in result)
                            {
                                AlohaFuncs.ModItem(iniFile.ExternalInterfaceTerminal, pId, 999902, ss, -999999999.000000, 0);
                            }
                        }
                        catch (Exception eM)
                        {
                            Utils.ToCardLog("Error AddDish in Comment " + itm.Barcode + " " + eM.Message);
                        }
                    }
                    itm.Success = modOk;
                    if (modOk)
                    {
                        AlohaFuncs.EndItem(iniFile.ExternalInterfaceTerminal);
                        Resp.AddedItems.Add(itm);

                    }
                    else
                    {

                        itm.ErrorMsg = "Не смог добавить модификаторы";
                        Resp.Success = false;
                        itm.Success = false;
                        //var err = new CAlohaErrors(e.Message);
                        //itm.AlohaErrorCode = -5;
                        itm.ErrorMsg = "Не смог добавить модификаторы";
                        Resp.ErrorItems.Add(itm);
                        Utils.ToCardLog("Error AddDish " + itm.Barcode + " Не смог добавить модификаторы ");
                    }

                    try
                    {
                        Utils.ToCardLog("Request.SendToKitchenOrderType  " + Request.SendToKitchenOrderType );
                        if (Request.SendToKitchenOrderType > 0)
                        {
                            AlohaFuncs.SelectAllEntriesOnCheck(iniFile.ExternalInterfaceTerminal, Request.AlohaCheckId);
                            AlohaFuncs.OrderItems(iniFile.ExternalInterfaceTerminal, Resp.TableId, Request.SendToKitchenOrderType);
                            AlohaFuncs.DeselectAllEntries(iniFile.ExternalInterfaceTerminal);
                        }
                    }
                    catch(Exception ee)
                    {
                        Utils.ToCardLog("Error OpenTableFromExternal OrderItems " + ee.Message);
                    }
                    


                }
                catch (Exception e)
                {
                    Resp.Success = false;
                    itm.Success = false;
                    var err = new CAlohaErrors(e.Message);
                    itm.AlohaErrorCode = err.Val;
                    itm.ErrorMsg = err.ValStr;
                    Resp.ErrorItems.Add(itm);
                    Utils.ToCardLog("Error AddDish " + itm.Barcode + " " + e.Message);
                }
            }
            /*

            foreach (AlohaExternal.AlohaItemInfo itm in Request.Items)
            {
                
                double Price = (double)itm.Price;
                if (Price == -1)
                {
                    Price = -999999999.0;
                }
                try
                {
                    Utils.ToCardLog($"OpenTableFromExternal AddCheck AlohaFuncs.BeginItem iniFile.ExternalInterfaceTerminal:{iniFile.ExternalInterfaceTerminal}," +
                        $" Request.AlohaCheckId:{Request.AlohaCheckId}, itm.Barcode:{itm.Barcode}, Price:{Price}");
                    AlohaFuncs.BeginItem(iniFile.ExternalInterfaceTerminal, Request.AlohaCheckId, itm.Barcode, "", Price);
                    AlohaFuncs.EndItem(iniFile.ExternalInterfaceTerminal);
                    itm.Success = true;
                    Resp.AddedItems.Add(itm);
                }
                catch (Exception e)
                {
                    Utils.ToCardLog($"Error OpenTableFromExternal.AlohaFuncs.BeginItem {e.Message}");
                    Resp.Success = false;
                    itm.Success = false;
                    itm.AlohaErrorCode = CAlohaErrors.GetAlohaErrorVal(e.Message);
                    Resp.ErrorItems.Add(itm);
                }
            }

            */
            LogOut(iniFile.ExternalInterfaceTerminal);
            

        }


        static internal void AddDishFromExternal(AlohaExternal.AddItemsRequest Request, AlohaExternal.AddItemsResponse Resp)
        {
            Utils.ToCardLog("AddDishFromExternal");

            if (!LoginExternal(Resp))
            {
                Resp.Success = false;
                return;
            }

            foreach (AlohaExternal.AlohaItemInfo itm in Request.Items)
            {
                Utils.ToCardLog("AddDish " + itm.Barcode + " " + itm.Name);
                double Price = (double)itm.Price;
                if (Price == -1)
                {
                    Price = -999999999.0;
                }
                try
                {
                    int pId = AlohaFuncs.BeginItem(iniFile.ExternalInterfaceTerminal, Request.AlohaCheckId, itm.Barcode, "", Price);
                    bool modOk = true;
                    if (itm.Mods != null)
                    {
                        foreach (var mod in itm.Mods.Where(a => a.Barcode > 0))
                        {
                            Utils.ToCardLog("AddMod " + mod.Barcode + " " + mod.Name);

                            double mPrice = (double)mod.Price;
                            if (mPrice == -1)
                            {
                                mPrice = -999999999.0;
                            }
                            try
                            {
                                AlohaFuncs.ModItem(iniFile.ExternalInterfaceTerminal, pId, mod.Barcode, "", mPrice, 0);
                            }
                            catch (Exception ee)
                            {
                                Resp.Success = false;
                                itm.Success = false;
                                var err = new CAlohaErrors(ee.Message);
                                mod.AlohaErrorCode = err.Val;
                                mod.ErrorMsg = err.ValStr;
                                Resp.ErrorItems.Add(mod);
                                modOk = false;
                                Utils.ToCardLog("Error AddModd " + mod.Barcode + " " + ee.Message);
                            }
                        }
                    }

                    var chunkSize = 14;
                    foreach (var mod in itm.Mods.Where(a => a.Barcode == 0))
                    {
                        try
                        {
                            var result = (from Match m in Regex.Matches(mod.Name, @".{1," + chunkSize + "}")
                                          select m.Value).ToList();
                            foreach (string ss in result)
                            {
                                AlohaFuncs.ModItem(iniFile.ExternalInterfaceTerminal, pId, 999902, ss, -999999999.000000, 0);
                            }
                        }
                        catch (Exception eM)
                        {
                            Utils.ToCardLog("Error AddDish in itm.Mods " + itm.Barcode + " " + eM.Message);
                        }
                    }

                    if (itm.Comment?.Length > 0)
                    {
                        try
                        {
                            var result = (from Match m in Regex.Matches(itm.Comment, @".{1," + chunkSize + "}")
                                          select m.Value).ToList();

                            foreach (string ss in result)
                            {
                                AlohaFuncs.ModItem(iniFile.ExternalInterfaceTerminal, pId, 999902, ss, -999999999.000000, 0);
                            }
                        }
                        catch (Exception eM)
                        {
                            Utils.ToCardLog("Error AddDish in Comment " + itm.Barcode + " " + eM.Message);
                        }
                    }
                    itm.Success = modOk;
                    if (modOk)
                    {
                        AlohaFuncs.EndItem(iniFile.ExternalInterfaceTerminal);
                        Resp.AddedItems.Add(itm);

                    }
                    else
                    {

                        itm.ErrorMsg = "Не смог добавить модификаторы";
                        Resp.Success = false;
                        itm.Success = false;
                        //var err = new CAlohaErrors(e.Message);
                        //itm.AlohaErrorCode = -5;
                        itm.ErrorMsg = "Не смог добавить модификаторы";
                        Resp.ErrorItems.Add(itm);
                        Utils.ToCardLog("Error AddDish " + itm.Barcode + " Не смог добавить модификаторы ");
                    }

                    try
                    {
                        Utils.ToCardLog("Request.SendToKitchenOrderType  " + Request.SendToKitchenOrderType);
                        if (Request.SendToKitchenOrderType > 0)
                        {
                            AlohaFuncs.SelectEntryAndChildren(iniFile.ExternalInterfaceTerminal, Request.AlohaCheckId, pId); 
                            AlohaFuncs.OrderItems(iniFile.ExternalInterfaceTerminal, (int)Request.AlohaTableId, Request.SendToKitchenOrderType);
                            AlohaFuncs.DeselectAllEntries(iniFile.ExternalInterfaceTerminal);
                        }
                    }
                    catch (Exception ee)
                    {
                        Utils.ToCardLog("Error AddDishFromExternal OrderItems " + ee.Message);
                    }


                }
                catch (Exception e)
                {
                    Resp.Success = false;
                    itm.Success = false;
                    var err = new CAlohaErrors(e.Message);
                    itm.AlohaErrorCode = err.Val;
                    itm.ErrorMsg = err.ValStr;
                    Resp.ErrorItems.Add(itm);
                    Utils.ToCardLog("Error AddDish " + itm.Barcode + " " + e.Message);
                }
            }


            /*
            foreach (AlohaExternal.AlohaItemInfo itm in Request.Items)
            {
                try
                {
                    if (Request.AlohaTableId == null) { Utils.ToCardLog(String.Format("Request.AlohaTableId== null")); } else { Utils.ToCardLog(String.Format("Request.AlohaTableId: {0}", Request.AlohaTableId.ToString())); }
                    if (itm.Barcode == null) { Utils.ToCardLog(String.Format("itm.Barcode == null")); } else { Utils.ToCardLog(String.Format("itm.Barcode: {0}", itm.Barcode.ToString())); }
                    if (itm.Price == null) { Utils.ToCardLog(String.Format("itm.Price == null")); } else { Utils.ToCardLog(String.Format("itm.Price: {0}", itm.Price.ToString())); }


                    if (itm.Name == null) { Utils.ToCardLog(String.Format("itm.Name == null")); } else { Utils.ToCardLog(String.Format("itm.Name: {0}", itm.Name.ToString())); }
                    double Price = (double)itm.Price;
                    if (Price == -1)
                    {
                        Price = -999999999.0;
                    }
                    
                    int entryId = AlohaFuncs.BeginItem(iniFile.ExternalInterfaceTerminal, Request.AlohaCheckId, itm.Barcode, "", Price);
                    AlohaFuncs.EndItem(iniFile.ExternalInterfaceTerminal);

                    if (iniFile.ExternalInterfaceSendItemWhenAdd)
                    {
                        try
                        {
                            Utils.ToCardLog("ExternalInterfaceSendItemWhenAdd");
                            AlohaFuncs.DeselectAllEntries(iniFile.ExternalInterfaceTerminal);
                            AlohaFuncs.SelectEntry(iniFile.ExternalInterfaceTerminal, Request.AlohaCheckId, entryId);
                            AlohaFuncs.OrderItems(iniFile.ExternalInterfaceTerminal, Request.AlohaTableId, 2);
                            Utils.ToCardLog("ExternalInterfaceSendItemWhenAdd ok");
                        }
                        catch (Exception e)
                        {
                            Utils.ToCardLog("ExternalInterfaceSendItemWhenAdd error " + e.Message);
                        }

                    }

                    itm.Success = true;
                    Resp.AddedItems.Add(itm);
                    Utils.ToCardLog(String.Format("AddItemFromExternal Ok"));
                }
                catch (Exception e)
                {
                    Resp.Success = false;
                    itm.Success = false;
                    itm.AlohaErrorCode = CAlohaErrors.GetAlohaErrorVal(e.Message);
                    Resp.ErrorItems.Add(itm);
                    Resp.IntegrationErrorCode = -3;
                    Resp.ErrorMsg = "Error in child data";
                    Utils.ToCardLog(String.Format("AddItemFromExternal Error " + e.Message));
                }
            }

            */
            LogOut(iniFile.ExternalInterfaceTerminal);
        }

        static RemoteOrderSrv.Item[] AddDishAsinkItems;
        static int AddDishAsinkCheck = 0;
        static bool AddDishAsinkError = true;
        static internal void AddDishAsink()
        {
            foreach (RemoteOrderSrv.Item Rd in AddDishAsinkItems)
            {
                try
                {
                    Utils.ToLog("AddDishAsink Заказываю блюдо BeginPivotSeatItem" + Rd.ItemID + " на чек " + AddDishAsinkCheck);
                    int DishId = 0;
                    DishId = AlohaFuncs.BeginPivotSeatItem(iniFile.RemoteOrderTerminalNumber, AddDishAsinkCheck, Rd.ItemID, "", -999999999.0, -1);
                    Utils.ToLog("Заказал блюдо " + DishId);

                    foreach (RemoteOrderSrv.Modifier Rm in Rd.Modifiers)
                    {
                        if (Rm.ItemModifierID > 0)
                        {
                            try
                            {
                                Utils.ToLog("Добавляю модификатор " + Rm.ItemModifierID + "на блюдо " + DishId);
                                int modid = AlohaFuncs.ModItem(iniFile.RemoteOrderTerminalNumber, DishId, Rm.ItemModifierID, "", -999999999.0, 0);
                                Utils.ToLog("Добавил модификатор " + modid);
                            }
                            catch (Exception eMod)
                            {
                                Utils.ToLog("Ошибка при добавлении модификатора " + Rm.ItemModifierID + "на блюдо " + DishId + " " + eMod.Message);
                            }
                        }
                    }
                    AlohaFuncs.EndItem(iniFile.RemoteOrderTerminalNumber);
                    Utils.ToLog("AddDishAsink  EndItem " + DishId);
                    AddDishAsinkError = false;
                }
                catch (Exception e)
                {
                    Utils.ToLog("Error AddDishAsink " + e.Message);
                }
            }


        }



        static internal int OrderDishes(RemoteOrderSrv.OrderInfoForAloha Order, bool FromTCP, out int CheckId)
        {
            CheckId = 0;
            try
            {
                AlohaFuncs.LogOut(iniFile.RemoteOrderTerminalNumber);
            }
            catch
            { }
            Utils.ToCardLog("OrderDishes iniFile.RemoteOrderTerminalNumber:" + iniFile.RemoteOrderTerminalNumber + "iniFile.RemoteOrderWaterNumber: " + iniFile.RemoteOrderWaterNumber);
            AlohaFuncs.LogIn(iniFile.RemoteOrderTerminalNumber, iniFile.RemoteOrderWaterNumber, "", "");
            try
            {
                AlohaFuncs.ClockIn(iniFile.RemoteOrderTerminalNumber, GetJobCode(iniFile.RemoteOrderWaterNumber));
            }
            catch
            { }
            int TNum = 0;
            try
            {

                if (IsTableServise())
                {
                    TNum = GetEmptyTable();
                }
                if ((TNum != -1) || (!AlohaTSClass.IsTableServise()))
                {
                    int TableId = 0;

                    if (!AlohaTSClass.IsTableServise())
                    {
                        Utils.ToLog("Удаленный заказ через QS");
                        int QueueId = 1;
                        TNum = GetLastQueue(iniFile.RemoteOrderTerminalNumber);
                        QueueId = GetQueue(iniFile.RemoteOrderTerminalNumber);
                        Utils.ToLog("Очередь " + QueueId.ToString());
                        TableId = AlohaFuncs.AddTable(iniFile.RemoteOrderTerminalNumber, QueueId, TNum, "Кофе с собой " + TNum, 1);
                        Utils.ToLog("Стол открыт " + QueueId.ToString());

                        /*
                        Utils.ToLog("Открыл стол " + TableId);
                        CheckId = AlohaFuncs.AddCheck(iniFile.RemoteOrderTerminalNumber, TableId);
                        Utils.ToLog("Открыл чек " + CheckId);

                        AddDishAsinkItems = Order.Items;
                        AddDishAsinkCheck = CheckId;

                        AddDishAsinkError = true;
                        int i = 0;
                        do
                        {
                            Thread.Sleep(1000);
                            Utils.ToLog("AddDishAsink попытка " + i.ToString());
                            i++;
                            Thread ThOrderDishes = new Thread(AddDishAsink);
                            ThOrderDishes.Start();

                        } while (AddDishAsinkError);


                        return TNum;
                         * */
                    }
                    else
                    {

                        TableId = OpenTable(TNum, Order.OrderID, out TNum);
                        if (TableId == -1)
                        {
                            Utils.ToLog("Не смог открыть ни один стол ");
                            return -1;
                        }
                    }
                    //int TableId = AlohaFuncs.AddTable(iniFile.RemoteOrderTerminalNumber, 0, TNum, "Заказ № " + Order.OrderID, 1);
                    Utils.ToLog("Открыл стол " + TableId);
                    CheckId = AlohaFuncs.AddCheck(iniFile.RemoteOrderTerminalNumber, TableId);
                    Utils.ToLog("Открыл чек " + CheckId);
                    foreach (RemoteOrderSrv.Item Rd in Order.Items)
                    {
                        try
                        {
                            if (Rd.ItemID == 0)
                            {
                                Utils.ToLog("Блюдо с нулевым кодом" + Rd.ItemName + " на чек " + CheckId);
                                foreach (RemoteOrderSrv.Modifier Rm in Rd.Modifiers)
                                {
                                    if (Rm.ItemModifierID > 0)
                                    {
                                        try
                                        {
                                            Utils.ToLog("Заказываю блюдо-модфикатор " + Rm.ItemModifierID + " на чек " + CheckId);
                                            int DishId = AlohaFuncs.BeginItem(iniFile.RemoteOrderTerminalNumber, CheckId, Rm.ItemModifierID, "", -999999999.0);
                                            AlohaFuncs.EndItem(iniFile.RemoteOrderTerminalNumber);


                                            Utils.ToLog("Заказал блюдо-модфикатор " + Rm.ItemModifierID + " на чек " + CheckId);
                                        }
                                        catch (Exception eMod)
                                        {
                                            string s = eMod.Message.Substring(eMod.Message.Length - 2, 2);
                                            if ((s == "32") || (s == "1С") || (s == "14"))
                                            {
                                                Utils.ToLog("Обрабатываемая Ошибка при заказе  блюда-модфикатора" + Rm.ItemModifierID + " на чек " + CheckId + " " + eMod.Message);
                                                throw new Exception(eMod.Message);
                                            }

                                            Utils.ToLog("Ошибка при заказе  блюда-модфикатора" + Rm.ItemModifierID + " на чек " + CheckId + " " + eMod.Message);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                Utils.ToLog("Заказываю блюдо BeginPivotSeatItem" + Rd.ItemID + " на чек " + CheckId);
                                int DishId = 0;
                                try
                                {
                                    DishId = AlohaFuncs.BeginPivotSeatItem(iniFile.RemoteOrderTerminalNumber, CheckId, Rd.ItemID, "", -999999999.0, -1);
                                }
                                catch
                                {

                                    Thread.Sleep(10000);
                                    Utils.ToLog("Повторный заказ " + Rd.ItemID + " на чек " + CheckId);
                                    DishId = AlohaFuncs.BeginItem(iniFile.RemoteOrderTerminalNumber, CheckId, Rd.ItemID, "", -999999999.0);
                                }
                                Utils.ToLog("Заказал блюдо " + DishId);

                                foreach (RemoteOrderSrv.Modifier Rm in Rd.Modifiers)
                                {
                                    if (Rm.ItemModifierID > 0)
                                    {
                                        try
                                        {
                                            Utils.ToLog("Добавляю модификатор " + Rm.ItemModifierID + "на блюдо " + DishId);
                                            int modid = AlohaFuncs.ModItem(iniFile.RemoteOrderTerminalNumber, DishId, Rm.ItemModifierID, "", -999999999.0, 0);
                                            Utils.ToLog("Добавил модификатор " + modid);
                                        }
                                        catch (Exception eMod)
                                        {
                                            Utils.ToLog("Ошибка при добавлении модификатора " + Rm.ItemModifierID + "на блюдо " + DishId + " " + eMod.Message);
                                        }
                                    }
                                }
                                AlohaFuncs.EndItem(iniFile.RemoteOrderTerminalNumber);
                                Utils.ToLog("EndItem " + DishId);

                                foreach (RemoteOrderSrv.Modifier Rm in Rd.Modifiers)
                                {
                                    if (Rm.ItemModifierID == 0)
                                    {

                                        Rm.ItemModifierName = Rm.ItemModifierName.Trim();
                                        if (Rm.ItemModifierName.Length > 0)
                                        {
                                            Utils.ToLog("Добавляю модификатор как сообщение " + Rm.ItemModifierName + "на блюдо " + DishId);
                                            AlohaFuncs.ApplySpecialMessage(iniFile.RemoteOrderTerminalNumber, CheckId, DishId, Rm.ItemModifierName);
                                            Utils.ToLog("Добавил модификатор ");
                                        }
                                        else
                                        {
                                            Utils.ToLog("Пустой модификатор ");
                                        }
                                    }
                                }


                                Rd.ItemSpecialMessage = Rd.ItemSpecialMessage.Trim();
                                if ((Rd.ItemSpecialMessage.Length > 0) && (Rd.ItemSpecialMessage.Length < 20) && Rd.ItemSpecialMessage != "na")
                                {
                                    Utils.ToLog("Pre ApplySpecialMessage " + Rd.ItemSpecialMessage + " на чек " + CheckId + " блюдо " + DishId);
                                    AlohaFuncs.ApplySpecialMessage(iniFile.RemoteOrderTerminalNumber, CheckId, DishId, Rd.ItemSpecialMessage);
                                    Utils.ToLog("ApplySpecialMessage " + Rd.ItemSpecialMessage + " на чек " + CheckId + " блюдо " + DishId);
                                }

                            }
                        }
                        catch (Exception eItem)
                        {

                            string s = eItem.Message.Substring(eItem.Message.Length - 2, 2);
                            if ((s == "32") || (s == "1С") || (s == "14"))
                            {
                                Utils.ToLog("Ошибка при добавлении блюда. Пытаюсь добавить еще раз " + Rd.ItemID + " " + eItem.Message);
                                return 2;



                            }

                            Utils.ToLog("Ошибка при добавлении блюда: " + Rd.ItemID + " " + eItem.Message);
                        }


                    }
                    try
                    {
                        AlohaFuncs.DeselectAllEntries(iniFile.RemoteOrderTerminalNumber);
                        AlohaFuncs.SelectAllEntriesOnCheck(iniFile.RemoteOrderTerminalNumber, CheckId);
                        Utils.ToLog("SelectAllEntriesOnCheck");
                        AlohaFuncs.OrderItems(iniFile.RemoteOrderTerminalNumber, TableId, 0);
                        Utils.ToLog("OrderItems");
                        AlohaFuncs.SetObjectAttribute(520, TableId, "RemoteOrder", Order.OrderID);
                        Utils.ToLog("SetObjectAttribute " + Order.OrderID.ToString() + " на стол " + TableId);
                        string Tm = AlohaFuncs.GetObjectAttribute(520, TableId, "RemoteOrder");
                        Utils.ToLog("GetObjectAttribute " + Tm + " на стол " + TableId);
                    }
                    catch (Exception eItem2)
                    {
                        Utils.ToLog("Ошибка при выделении блюда: " + eItem2.Message);
                    }
                    if (!FromTCP)
                    {
                        MainClass.CTG.s2010Serv.OrderOrdered(AlohainiFile.DepNum, Order.ID);
                    }

                }
                else
                {
                    return -1;
                }
            }
            catch (Exception e)
            {
                string s = e.Message.Substring(e.Message.Length - 2, 2);
                if ((s == "32") || (s == "1С") || (s == "14"))
                {
                    Utils.ToLog("Ошибка при открытии стола. Пытаюсь открыть еще раз " + e.Message);
                    return 2;



                }
                Utils.ToLog("Ошибка " + e.Message);
                return -1;
            }

            AlohaFuncs.LogOut(iniFile.RemoteOrderTerminalNumber);
            if (FromTCP)
            {
                Utils.ToCardLog("return " + TNum.ToString());
                return TNum; //Convert.ToInt32(Check.GetCheckShortNum(CheckId));
            }
            else
            {
                return 0;
            }
        }




        static internal bool OrderDishes2(OrderInfo Order)
        {

            try
            {
                AlohaFuncs.LogOut(iniFile.RemoteOrderTerminalNumber);
            }
            catch
            { }
            AlohaFuncs.LogIn(iniFile.RemoteOrderTerminalNumber, iniFile.RemoteOrderWaterNumber, "", "");
            try
            {
                AlohaFuncs.ClockIn(GetTermNum(), GetJobCode(iniFile.RemoteOrderWaterNumber));
            }
            catch
            { }

            try
            {
                int TNum = GetEmptyTable();
                if (TNum != -1)
                {
                    int TableId = 0;
                    Utils.ToLog("Order.OrderID=" + TableId + "=");
                    if (Order.OrderID != "")
                    {
                        TableId = AlohaFuncs.AddTable(iniFile.RemoteOrderTerminalNumber, 0, TNum, "Заказ № " + Order.OrderID, 1);
                    }
                    else
                    {
                        TableId = AlohaFuncs.AddTable(iniFile.RemoteOrderTerminalNumber, 0, TNum, "", 1);
                    }
                    Utils.ToLog("Открыл стол " + TableId);
                    int CheckId = AlohaFuncs.AddCheck(iniFile.RemoteOrderTerminalNumber, TableId);
                    Utils.ToLog("Открыл чек " + CheckId);
                    foreach (Item Rd in Order.Items)
                    {


                        try
                        {
                            if (Rd.ItemID == 0)
                            {
                                Utils.ToLog("Блюдо с нулевым кодом" + Rd.ItemName + " на чек " + CheckId);
                                foreach (Modifier Rm in Rd.Modifiers)
                                {
                                    if (Rm.ItemModifierID > 0)
                                    {
                                        try
                                        {
                                            Utils.ToLog("Заказываю блюдо-модфикатор " + Rm.ItemModifierID + " на чек " + CheckId);
                                            int DishId = AlohaFuncs.BeginItem(iniFile.RemoteOrderTerminalNumber, CheckId, Rm.ItemModifierID, "", -999999999.0);
                                            AlohaFuncs.EndItem(iniFile.RemoteOrderTerminalNumber);


                                            Utils.ToLog("Заказал блюдо-модфикатор " + Rm.ItemModifierID + " на чек " + CheckId);
                                        }
                                        catch (Exception eMod)
                                        {
                                            string s = eMod.Message.Substring(eMod.Message.Length - 2, 2);
                                            if ((s == "32") || (s == "1С") || (s == "14"))
                                            {
                                                throw new Exception(eMod.Message);
                                            }

                                            Utils.ToLog("Ошибка при заказе  блюда-модфикатора" + Rm.ItemModifierID + " на чек " + CheckId + " " + eMod.Message);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                Utils.ToLog("Заказываю блюдо " + Rd.ItemID + " на чек " + CheckId);
                                int DishId = AlohaFuncs.BeginItem(iniFile.RemoteOrderTerminalNumber, CheckId, Rd.ItemID, "", -999999999.0);
                                Utils.ToLog("Заказал блюдо " + DishId);

                                foreach (Modifier Rm in Rd.Modifiers)
                                {
                                    if (Rm.ItemModifierID > 0)
                                    {
                                        try
                                        {
                                            Utils.ToLog("Добавляю модификатор " + Rm.ItemModifierID + "на блюдо " + DishId);
                                            int modid = AlohaFuncs.ModItem(iniFile.RemoteOrderTerminalNumber, DishId, Rm.ItemModifierID, "", -999999999.0, 0);
                                            Utils.ToLog("Добавил модификатор " + modid);
                                        }
                                        catch (Exception eMod)
                                        {
                                            Utils.ToLog("Ошибка при добавлении модификатора " + Rm.ItemModifierID + "на блюдо " + DishId + " " + eMod.Message);
                                        }
                                    }
                                }
                                AlohaFuncs.EndItem(iniFile.RemoteOrderTerminalNumber);
                                Utils.ToLog("EndItem " + DishId);

                                foreach (Modifier Rm in Rd.Modifiers)
                                {
                                    if (Rm.ItemModifierID == 0)
                                    {

                                        Rm.ItemModifierName = Rm.ItemModifierName.Trim();
                                        if (Rm.ItemModifierName.Length > 0)
                                        {
                                            Utils.ToLog("Добавляю модификатор как сообщение " + Rm.ItemModifierName + "на блюдо " + DishId);
                                            AlohaFuncs.ApplySpecialMessage(iniFile.RemoteOrderTerminalNumber, CheckId, DishId, Rm.ItemModifierName);
                                            Utils.ToLog("Добавил модификатор ");
                                        }
                                        else
                                        {
                                            Utils.ToLog("Пустой модификатор ");
                                        }
                                    }
                                }


                                Rd.ItemSpecialMessage = Rd.ItemSpecialMessage.Trim();
                                if ((Rd.ItemSpecialMessage.Length > 0) && (Rd.ItemSpecialMessage.Length < 20) && Rd.ItemSpecialMessage != "na")
                                {
                                    Utils.ToLog("Pre ApplySpecialMessage " + Rd.ItemSpecialMessage + " на чек " + CheckId + " блюдо " + DishId);
                                    AlohaFuncs.ApplySpecialMessage(iniFile.RemoteOrderTerminalNumber, CheckId, DishId, Rd.ItemSpecialMessage);
                                    Utils.ToLog("ApplySpecialMessage " + Rd.ItemSpecialMessage + " на чек " + CheckId + " блюдо " + DishId);
                                }

                            }
                        }
                        catch (Exception eItem)
                        {

                            string s = eItem.Message.Substring(eItem.Message.Length - 2, 2);
                            if ((s == "32") || (s == "1С") || (s == "14"))
                            {
                                Utils.ToLog("Ошибка при добавлении блюда. Пытаюсь добавить еще раз " + Rd.ItemID + " " + eItem.Message);
                                return OrderDishes2(Order);



                            }

                            Utils.ToLog("Ошибка при добавлении блюда: " + Rd.ItemID + " " + eItem.Message);
                        }


                    }
                    try
                    {
                        AlohaFuncs.DeselectAllEntries(iniFile.RemoteOrderTerminalNumber);
                        AlohaFuncs.SelectAllEntriesOnCheck(iniFile.RemoteOrderTerminalNumber, CheckId);
                        Utils.ToLog("SelectAllEntriesOnCheck");
                        AlohaFuncs.OrderItems(iniFile.RemoteOrderTerminalNumber, TableId, 0);
                        Utils.ToLog("OrderItems");
                        AlohaFuncs.SetObjectAttribute(520, TableId, "RemoteOrder", Order.OrderID);
                        Utils.ToLog("SetObjectAttribute " + Order.OrderID.ToString() + " на стол " + TableId);
                        string Tm = AlohaFuncs.GetObjectAttribute(520, TableId, "RemoteOrder");
                        Utils.ToLog("GetObjectAttribute " + Tm + " на стол " + TableId);
                    }
                    catch (Exception eItem2)
                    {
                        Utils.ToLog("Ошибка при выделении блюда: " + eItem2.Message);
                    }

                    MainClass.CTG.s2010Serv.OrderOrdered2Async(AlohainiFile.DepNum, Order.OrderID);


                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                Utils.ToLog("Ошибка " + e.Message);
                return false;
            }

            AlohaFuncs.LogOut(iniFile.RemoteOrderTerminalNumber);

            return true;
        }


        static internal void SetPaymentOperAttr(int CheckId, string bins, string RRN)
        {
            try
            {
                Utils.ToCardLog($"SetPaymentOperAttr CheckId: {CheckId}, bins {bins}, string {RRN}");
                AlohaFuncs.SetObjectAttribute(INTERNAL_CHECKS, CheckId, "PayOperBins", bins);
                AlohaFuncs.SetObjectAttribute(INTERNAL_CHECKS, CheckId, "PayOperRRN", RRN);
            }
            catch (Exception e)
            {
                Utils.ToCardLog($"Error SetPaymentOperAttr {e.Message}");
            }
        }


        static internal string DeleteReasonName = "DelRes";


        static internal void SetDeleteReasonAttr(int ItemId, int i)
        {
            AlohaFuncs.SetObjectAttribute(540, ItemId, DeleteReasonName, i.ToString());
        }

        static internal int GetDeleteReasonAttr(int CheckId)
        {
            try
            {
                return Convert.ToInt32(AlohaFuncs.GetObjectAttribute(540, CheckId, DeleteReasonName));
            }
            catch (Exception e)
            {
                return 0;
            }
        }


        static internal string PrintChkAttrName = "PrintChk";
        static internal void SetPrintChkAttr(int CheckId, int i)
        {
            AlohaFuncs.SetObjectAttribute(540, CheckId, PrintChkAttrName, i.ToString());
        }
        static internal int GetPrintChkAttr(int CheckId)
        {
            try
            {
                return Convert.ToInt32(AlohaFuncs.GetObjectAttribute(540, CheckId, PrintChkAttrName));
            }
            catch (Exception e)
            {
                return 0;
            }
        }
        static internal void SetPaymentAttr(int CheckId, int Num)
        {
            AlohaFuncs.SetObjectAttribute(540, CheckId, "PaymentNum", Num.ToString());
        }

        static internal void SetSVCardSaleAttr(int CheckId, String Card)
        {
            int Num = 0;
            string mCard = "";
            bool res = false;
            do
            {
                res = GetSVCardSaleAttr(CheckId, Num, out mCard);
                if (res)
                {
                    if (mCard == Card)
                    {
                        return;
                    }
                }
                else
                {
                    break;
                }
                Num++;
            } while (res);
            AlohaFuncs.SetObjectAttribute(540, CheckId, "SVCard" + Num.ToString(), Card);
        }

        static internal bool GetSVCardSaleAttr(int CheckId, int Num, out string Card)
        {
            Card = "";
            try
            {
                Card = AlohaFuncs.GetObjectAttribute(540, CheckId, "SVCard" + Num.ToString());
                if (Card == "") return false;
                return true;
            }
            catch { return false; }
        }



        static internal string ManagerDiscountNameAttrName = "ManDiscN";
        static internal string ManagerDiscountIDAttrName = "ManDiscId";
        static internal string DiscountAttrName = "DiscountCard";
        static internal string GoldDiscountAttrName = "GoldCard";
        static internal string GoldDiscountVisitsAttrName = "GoldCardV";
        static internal string GoldDiscountPeriodAttrName = "GoldCardP";


        static internal string TakeOutAttrName = "TakeOutTable";
        static internal void SetManagerDiscountAttr(int CheckId, string Name, int Id)
        {
            AlohaFuncs.SetObjectAttribute(540, CheckId, ManagerDiscountNameAttrName, Name);
            AlohaFuncs.SetObjectAttribute(540, CheckId, ManagerDiscountIDAttrName, Id.ToString());
        }
        static internal bool GetManagerDiscountAttr(int CheckId, out string Name, out int Id)
        {
            Name = "";
            Id = 0;
            try
            {
                Name = AlohaFuncs.GetObjectAttribute(540, CheckId, ManagerDiscountNameAttrName);
                Id = Convert.ToInt32(AlohaFuncs.GetObjectAttribute(540, CheckId, ManagerDiscountIDAttrName));
                return true;
            }
            catch
            {
                return false;
            }
        }


        static internal void SetVipToCurrentCheck()
        {
            CheckWindow();
            AlohaFuncs.SetObjectAttribute(540, (int)AlohaCurentState.CheckId, "VIP", "VIP");
            ShowMessage("Вип применен");
            Utils.ToCardLog("SetVipToCurrentCheck " + (int)AlohaCurentState.CheckId);


        }


        static internal void SetGuestCountAttr(int count)
        {
            CheckWindow();
            AlohaFuncs.SetObjectAttribute(540, (int)AlohaCurentState.CheckId, "Guests", count.ToString());
            Utils.ToCardLog("Set Число гостей " + count);
        }


        static internal int GetGuestCountAttr(int checkId)
        {
            try
            {
                CheckWindow();

                var res = Convert.ToInt32(AlohaFuncs.GetObjectAttribute(INTERNAL_CHECKS, checkId, "Guests"));

                Utils.ToCardLog("Get Число гостей " + res);
                return res;
            }
            catch
            {
                return 0;
            }
        }

        static internal bool GetVip(int CheckId)
        {

            try
            {
                string attr = AlohaFuncs.GetObjectAttribute(540, CheckId, "VIP");
                if (attr == "VIP")
                {
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }



        }


        static internal void SetTakeOutAttr(int TableId, string val)
        {

            AlohaFuncs.SetObjectAttribute(INTERNAL_TABLES, TableId, TakeOutAttrName, val);


        }


        static internal string GetTakeOutAttr(int TableId)
        {
            try
            {

                return AlohaFuncs.GetObjectAttribute(INTERNAL_TABLES, TableId, TakeOutAttrName);
            }
            catch
            {
                return "";
            }
        }



        static internal void SetDiscountAttr(int CheckId, string Str, bool Gold, int V, int P)
        {
            if (Gold)
            {
                AlohaFuncs.SetObjectAttribute(540, CheckId, GoldDiscountAttrName, Str);

            }
            AlohaFuncs.SetObjectAttribute(540, CheckId, DiscountAttrName, Str);
            AlohaFuncs.SetObjectAttribute(540, CheckId, GoldDiscountVisitsAttrName, V.ToString());
            AlohaFuncs.SetObjectAttribute(540, CheckId, GoldDiscountPeriodAttrName, P.ToString());
        }



        static internal string GetGoldDiscountAttr(int CheckId, out int P, out int V)
        {
            P = 0;
            V = 0;
            try
            {
                string s = AlohaFuncs.GetObjectAttribute(540, CheckId, GoldDiscountAttrName);
                P = Convert.ToInt32(AlohaFuncs.GetObjectAttribute(540, CheckId, GoldDiscountPeriodAttrName));
                V = Convert.ToInt32(AlohaFuncs.GetObjectAttribute(540, CheckId, GoldDiscountVisitsAttrName));
                return s;
            }
            catch
            {
                return "";
            }
            //Utils.ToLog("SetObjectAttribute " + Order.OrderID.ToString() + " на стол " + TableId);
            //string Tm = AlohaFuncs.GetObjectAttribute(520, TableId, "RemoteOrder");
        }


        static internal string GetDiscountAttr(int CheckId, out int P, out int V)
        {
            P = 0;
            V = 0;
            try
            {
                P = Convert.ToInt32(AlohaFuncs.GetObjectAttribute(540, CheckId, GoldDiscountPeriodAttrName));
                V = Convert.ToInt32(AlohaFuncs.GetObjectAttribute(540, CheckId, GoldDiscountVisitsAttrName));
                return AlohaFuncs.GetObjectAttribute(540, CheckId, DiscountAttrName);
            }
            catch
            {
                return "";
            }
        }


        static internal string GetDiscountAttr(int CheckId)
        {
            try
            {
                return AlohaFuncs.GetObjectAttribute(540, CheckId, DiscountAttrName);
            }
            catch
            {
                return "";
            }
        }

        static internal int GetPaymentAttr(int CheckId)
        {
            try
            {
                return Convert.ToInt32(AlohaFuncs.GetObjectAttribute(540, CheckId, "PaymentNum"));
            }
            catch
            {
                return 0;
            }
            //Utils.ToLog("SetObjectAttribute " + Order.OrderID.ToString() + " на стол " + TableId);
            //string Tm = AlohaFuncs.GetObjectAttribute(520, TableId, "RemoteOrder");
        }




        static internal bool DishIsQty(int DishID)
        {
            IberEnum Qty = Depot.GetEnum(FILE_QTYPRICE);

            try
            {
                IberObject MyDish = Qty.FindFromLongAttr("ITEMID", DishID);
                if (MyDish != null)
                {

                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        static internal bool DishIsQty(int DishID, out decimal Price)
        {
            Price = 0;
            IberEnum Qty = Depot.GetEnum(FILE_QTYPRICE);

            try
            {
                IberObject MyDish = Qty.FindFromLongAttr("ITEMID", DishID);
                Price = (decimal)MyDish.GetDoubleVal("UNITPRICE");
                if (MyDish != null)
                {

                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }



        static internal string GetTableDesc(int TableId)
        {
            IberEnum Tbls = Depot.GetEnum(FILE_TAB);

            try
            {
                IberObject MyTbl = Tbls.FindFromLongAttr("ID", TableId);
                if (MyTbl != null)
                {

                    return MyTbl.GetStringVal("DESC");
                }
                return "";
            }
            catch
            {
                return "";
            }
        }

        static internal bool TableExist(int TableNum)
        {
            IberEnum Tbls = Depot.GetEnum(FILE_TAB);
            try
            {
                IberObject MyTbl = Tbls.FindFromLongAttr("ID", TableNum);
                if (MyTbl != null)
                {
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }


        static internal double DishIsQty(int DishID, out string Unitname)
        {
            IberEnum Qty = Depot.GetEnum(FILE_QTYPRICE);
            Unitname = "";
            try
            {
                IberObject MyDish = Qty.FindFromLongAttr("ITEMID", DishID);
                double Price = MyDish.GetDoubleVal("UNITPRICE");
                Unitname = MyDish.GetStringVal("UNITNAME");
                return Price;
            }
            catch
            {
                return -1;
            }
        }


        static internal List<long> GetStopList()
        {
            List<long> Tmp = new List<long>();

            IberEnum Dishez = new IberEnum();
            Dishez = Depot.GetEnum(INTERNAL_ITEMS);
            foreach (IberObject IntITM in Dishez)
            {
                if (Convert.ToBoolean(IntITM.GetBoolVal("UNAVAILABLE")))
                    Tmp.Add(IntITM.GetLongVal("ID"));
            }
            GC.Collect();
            return Tmp;

        }

        static internal List<int> GetStopList2()
        {
            List<int> Tmp = new List<int>();
            IberEnum Dishez = new IberEnum();
            Dishez = Depot.GetEnum(INTERNAL_ITEMS);
            foreach (IberObject IntITM in Dishez)
            {
                if (Convert.ToBoolean(IntITM.GetBoolVal("UNAVAILABLE")))
                    Tmp.Add(IntITM.GetLongVal("ID"));
            }
            return Tmp;

        }


        static internal List<StopListDish> GetStopListForShow()
        {
            List<StopListDish> Tmp = new List<StopListDish>();

            IberEnum Dishez = new IberEnum();

            Dishez = Depot.GetEnum(INTERNAL_ITEMS);
            Utils.ToCardLog("GetStopListForShow start");
            foreach (IberObject IntITM in Dishez)
            {

                try
                {
                    StopListDish SLD = new StopListDish();
                    SLD.Name = IntITM.GetStringVal("LONGNAME");

                    SLD.Count = Convert.ToInt32(IntITM.GetStringVal("NUM_AVAILABLE"));

                    SLD.BarCode = IntITM.GetLongVal("ID");

                    if (Convert.ToBoolean(IntITM.GetBoolVal("UNAVAILABLE")))
                    {

                        Tmp.Add(SLD);

                    }
                }
                catch (Exception e)
                {
                    Utils.ToCardLog("Error Dishez " + e.Message);
                }
            }
            Utils.ToCardLog("GetStopListForShow end");
            return Tmp;
        }

        static internal List<StopListDish> GetItemSaleCount()
        {
            List<StopListDish> Tmp = new List<StopListDish>();

            IberEnum Dishez = new IberEnum();
            Dishez = Depot.GetEnum(INTERNAL_ITEMS);
            foreach (IberObject IntITM in Dishez)
            {

                StopListDish SLD = new StopListDish();
                SLD.Name = IntITM.GetStringVal("LONGNAME");
                SLD.Count = Convert.ToInt32(IntITM.GetStringVal("TOTAL_SOLD"));
                SLD.BarCode = IntITM.GetLongVal("ID");
                if (SLD.Count > 0)
                {
                    Tmp.Add(SLD);
                }
                //if (Convert.ToBoolean(IntITM.GetBoolVal("UNAVAILABLE")))
                // {
                //Tmp.Add(IntITM.GetLongVal("ID"));

                //}
            }
            return Tmp;
        }


        static internal int GetSaleGrope(int BarCode)
        {
            List<StopListDish> Tmp = new List<StopListDish>();

            IberEnum Dishez = new IberEnum();
            Dishez = Depot.GetEnum(INTERNAL_ITEMS);
            foreach (IberObject IntITM in Dishez)
            {

                if (IntITM.GetLongVal("ID") == BarCode)
                {
                    return IntITM.GetLongVal("CAT_ID");
                }

            }
            return 0;
        }

        //static private int termId = 3;

        static internal int GetJobCode(int EmpNum)
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

        static internal void LogOut()
        {
            try
            {
                AlohaFuncs.LogOut(iniFile.ExternalInterfaceTerminal);
            }
            catch (Exception e)
            {
                int j;
            }
        }

        static private int CurentLockEnt = 0;
        static internal void LockTable(int Dish, int TermNum, int CheckNum)
        {
            try
            {
                CurentLockEnt = AlohaFuncs.BeginItem(TermNum, CheckNum, Dish, "", 0);
            }
            catch (Exception e)
            {
                Utils.ToLog("LockTable Error " + e.Message);
            }
        }
        static internal void UnLockTable(int TermNum, int EmpId)
        {
            Utils.ToLog("UnLockTable");

            LogOut(TermNum);
            LogIn(TermNum, EmpId);


        }

        static internal void LogOut(int TermNum)
        {
            try
            {
                Utils.ToLog("LogOut TermNum " + TermNum);
                AlohaFuncs.LogOut(TermNum);
            }
            catch (Exception e)
            {

                if (e.Message.Length > 2 && (e.Message.Substring(e.Message.Length - 2) != "07"))
                {
                    Utils.ToLog("LogOut Error " + e.Message);
                }
            }
        }

        static internal bool LoginExternal(AlohaExternal.ICommandResponse Resp, int empId=0)
        {
            int TermNum = iniFile.ExternalInterfaceTerminal;
            int EmpId = iniFile.ExternalInterfaceEmployee;
            if (empId != 0)
            {
                EmpId = empId;
            }
            Utils.ToLog(String.Format("LogInExternal TermNum: {0}, EmpId: {1}", TermNum, EmpId));

            try
            {
                AlohaFuncs.LogOut(TermNum);
            }
            catch (Exception e)
            {
            }
            try
            {

                int i = AlohaFuncs.LogIn(TermNum, EmpId, "", "");
                try
                {
                    AlohaFuncs.ClockIn(TermNum, GetJobCode(EmpId));
                }
                catch (Exception e)
                {
                    int j;
                }
                Utils.ToLog(String.Format("LogInExternal sucseess TermNum: {0}, EmpId: {1}", TermNum, EmpId));
                return true;
            }
            catch (Exception e)
            {
                Resp.ErrorMsg = e.Message;
                try
                {
                    Resp.AlohaErrorCode = CAlohaErrors.GetAlohaErrorVal(e.Message);
                }
                catch
                { }
                Utils.ToLog("LogInExternal Error " + e.Message);
                return false;
            }

        }


        static internal void LogIn(int TermNum, int EmpId)
        {
            Utils.ToLog(String.Format("LogIn TermNum: {0}, EmpId: {1}", TermNum, EmpId));
            try
            {
                AlohaFuncs.LogOut(TermNum);
            }
            catch (Exception e)
            {
            }
            try
            {

                int i = AlohaFuncs.LogIn(TermNum, EmpId, "", "");


                try
                {
                    AlohaFuncs.ClockIn(TermNum, GetJobCode(EmpId));
                }
                catch (Exception e)
                {
                    int j;
                }


            }
            catch (Exception e)
            {

                Utils.ToLog("LogIn Error " + e.Message);


            }

        }





        static internal List<long> GetWaitersList()
        {
            List<long> Tmp = new List<long>();

            IberEnum Dishez = new IberEnum();
            Dishez = Depot.GetEnum(INTERNAL_EMPLOYEES);
            foreach (IberObject IntITM in Dishez)
            {
                if (!Convert.ToBoolean(IntITM.GetBoolVal("TERMINATED")))
                    Tmp.Add(IntITM.GetLongVal("USERNUMBER"));
            }
            return Tmp;

        }


        static internal string GetWaterName(int USERNUMBER)
        {
            try
            {
                IberObject Emp = Depot.FindObjectFromId(INTERNAL_EMPLOYEES, USERNUMBER).First();
                return Emp.GetStringVal("FIRSTNAME") + " " + Emp.GetStringVal("LASTNAME");

            }
            catch
            {

            }


            return "";
        }

        public static int CurentWaiter = 0;
        static internal string GetCurentWaterName()
        {
            try
            {
                IberObject Emp = Depot.FindObjectFromId(INTERNAL_EMPLOYEES, CurentWaiter).First();
                return Emp.GetStringVal("FIRSTNAME") + " " + Emp.GetStringVal("LASTNAME");
            }
            catch
            {
            }

            return "";
        }

        static internal bool CompEnable(int Id)
        {
            try
            {
                IberEnumClass CmpsEnum = (IberEnumClass)Depot.GetEnum(FILE_CMP);
                IberObject Cmp = CmpsEnum.FindFromLongAttr("ID", Id);
                string CompName = Cmp.GetStringVal("NAME");
                int ret = Cmp.GetBoolVal("ACTIVE");
                return (ret == 1);
            }
            catch
            {
                return false;
            }

        }
        static internal List<StopListService.DishN> GetListOfDish()
        {
            List<StopListService.DishN> Tmp = new List<PDiscountCard.StopListService.DishN>();
            IberEnum Dishez = new IberEnum();
            Dishez = Depot.GetEnum(INTERNAL_ITEMS);
            foreach (IberObject IntITM in Dishez)
            {
                StopListService.DishN d = new PDiscountCard.StopListService.DishN
                {
                    BarCode = IntITM.GetLongVal("ID"),
                    Name = IntITM.GetStringVal("LONGNAME"),
                };
                //Encoding.Convert (Encoding.UTF32   
                Tmp.Add(d);
            }
            return Tmp;
        }

        static internal IberObjectClass GetLast(IberEnumClass Enum)
        {
            IberObjectClass tmp = null;

            for (int i = 1; i < Enum.Count + 1; i++)
            {
                try
                {
                    tmp = (IberObjectClass)Enum.get_Item(i);
                }
                catch
                { }
            }
            return tmp;
        }

        static int CurentAddScaleDishBarCode = 0;
        static double CurentAddScaleDishBarWeight = 0;


        static internal void AddDishToCurentChk(int BarCode, double price = -999999999.000000)
        {
            Utils.ToCardLog("AddDishToCurentChk " + BarCode);
            try
            {
                AlohaFuncs.BeginItem(AlohaCurentState.TerminalId, (int)AlohaCurentState.CheckId, BarCode, "", price);
                AlohaFuncs.EndItem(AlohaCurentState.TerminalId);
                AlohaFuncs.SelectAllEntriesOnCheck(AlohaCurentState.TerminalId, (int)AlohaCurentState.CheckId);
                AlohaFuncs.OrderItems(AlohaCurentState.TerminalId, (int)AlohaCurentState.TableId, 2);
                AlohaFuncs.DeselectAllEntries(AlohaCurentState.TerminalId);

            }
            catch (Exception e)
            {
                Utils.ToCardLog("Error AddDishToCurentChk " + e.Message);
            }

        }


        /// <summary>
        /// Добавление блюда в чек с вариативным именем. Заказ только блюда
        /// </summary>
        /// <param name="BarCode"></param>
        /// <param name="Name"></param>
        /// <param name="price"></param>
        static internal void AddDishToCurentChkVarName(int BarCode, string Name, double price = -999999999.000000)
        {
            Utils.ToCardLog("AddDishToCurentChkVarName " + BarCode);
            try
            {
                int entId = AlohaFuncs.BeginItem(AlohaCurentState.TerminalId, (int)AlohaCurentState.CheckId, BarCode, Name, price);
                AlohaFuncs.EndItem(AlohaCurentState.TerminalId);
                AlohaFuncs.SelectEntry(AlohaCurentState.TerminalId, (int)AlohaCurentState.CheckId, entId);
                AlohaFuncs.OrderItems(AlohaCurentState.TerminalId, (int)AlohaCurentState.TableId, 2);
                AlohaFuncs.DeselectAllEntries(AlohaCurentState.TerminalId);
            }
            catch (Exception e)
            {
                Utils.ToCardLog("Error AddDishToCurentChkVarName " + e.Message);
            }
        }



        static internal void AddScaleDish()
        {
            Utils.ToCardLog("AddScaleDish() новый поток ");
            Thread.Sleep(1000);
            AddScaleDish(CurentAddScaleDishBarCode, CurentAddScaleDishBarWeight);
        }

        static internal void AddScaleDish(int BarCode, double Weight)
        {
            string UnitName = "";
            double Price = DishIsQty(BarCode, out UnitName);
            if (Price > -1)
            {

                double Stoim = Math.Floor(Price * Weight);
                bool Sucs = false;
                int PopCount = 0;
                //while (!Sucs)
                {
                    try
                    {
                        CheckWindow();
                        Utils.ToCardLog("CheckWindow(); ");
                        int Itmnum = AlohaFuncs.BeginPivotSeatItem(GetTermNum(), (int)AlohaCurentState.CheckId, BarCode, "", Stoim, -1);
                        Utils.ToCardLog("BeginPivotSeatItem ");
                        AlohaFuncs.EndItem(GetTermNum());
                        Utils.ToCardLog("EndItem");
                        AlohaFuncs.ApplySpecialMessage(GetTermNum(), (int)AlohaCurentState.CheckId, Itmnum, Weight + " " + UnitName);
                        Utils.ToCardLog("ApplySpecialMessage");
                        AlohaFuncs.RefreshCheckDisplay();
                        Utils.ToCardLog("RefreshCheckDisplay");
                        Sucs = true;
                    }
                    catch (Exception e)
                    {

                        Utils.ToCardLog("Error AddScaleDish " + PopCount.ToString() + "  " + e.Message);

                        if (e.Message.Substring(e.Message.Length - 3) == "086")
                        {
                            CurentAddScaleDishBarCode = BarCode;
                            CurentAddScaleDishBarWeight = Weight;
                            Thread ThAddScDish = new Thread(AddScaleDish);
                            ThAddScDish.Start();

                            /*
                            Thread.Sleep(200);
                            PopCount++;
                            try
                            {
                                Utils.ToCardLog("Start AddCheck");
                                int ChN= AlohaFuncs.AddCheck(GetTermNum(), (int)AlohaCurentState.TableId);
                                Utils.ToCardLog("AddCheck");
                                int Itmnum = AlohaFuncs.BeginPivotSeatItem(GetTermNum(), ChN, BarCode, "", Stoim, -1);
                                Utils.ToCardLog("BeginPivotSeatItem ");
                                AlohaFuncs.EndItem(GetTermNum());
                                Utils.ToCardLog("EndItem");
                                AlohaFuncs.ApplySpecialMessage(GetTermNum(), (int)AlohaCurentState.CheckId, Itmnum, Weight + " " + UnitName);
                                Utils.ToCardLog("ApplySpecialMessage");
                                AlohaFuncs.RefreshCheckDisplay();
                                Utils.ToCardLog("RefreshCheckDisplay");
                            }
                            catch (Exception ee)
                            {
                                Utils.ToCardLog("Error AddCheck "+ee.Message );
                            }
                            /*
                            Utils.ToCardLog("Еще одна попытка " );
                            try
                            {
                                
                                AlohaFuncs = new IberFuncs();
                                CheckWindow();
                                int Itmnum = AlohaFuncs.BeginPivotSeatItem(GetTermNum(), (int)AlohaCurentState.CheckId, BarCode, "", Stoim,-1);
                                AlohaFuncs.EndItem(GetTermNum());
                                AlohaFuncs.ApplySpecialMessage(GetTermNum(), (int)AlohaCurentState.CheckId, Itmnum, Weight + " " + UnitName);
                                AlohaFuncs.RefreshCheckDisplay();
                            }
                            catch(Exception ee)
                            {
                                Utils.ToCardLog("Снова Error AddScaleDish " + ee.Message);
                            }
                             * */
                        }

                        else
                        {
                            Sucs = true;
                        }

                    }
                    //AlohaCurentState. 
                }
            }
            else
            {
                Utils.ToCardLog("Блюдо " + BarCode + " не весовое ");
            }

        }

        static internal void SendCodiroffkaToDisplayBoard()
        {
            try
            {
                Utils.ToCardLog("Отправляю последовательность на ДИСПЛЕЙБОАРД");
                byte[] b = Utils.HexStringToByteArray("1B 74 11");

                string res = Encoding.UTF32.GetString(b);
                AlohaFuncs.SendStringToHardware(Utils.GetTermNum(), 0, 1, res);
                AlohaFuncs.SendStringToHardware(Utils.GetTermNum(), 0, 1, "ZZZ");

            }
            catch (Exception e)
            {
                Utils.ToCardLog("[Error] Отправляю последовательность на ДИСПЛЕЙБОАРД" + e.Message);
            }
        }

        static internal Double GetCheckSum(int Id)
        {

            double Sum;
            double mTax;
            AlohaFuncs.GetCheckTotal(Id, out Sum, out mTax);
            return Sum;
        }

        static internal bool ChkCanPrint(int ChId, int JCode)
        {
            try
            {
                IberEnumClass ChEnum = (IberEnumClass)Depot.FindObjectFromId(INTERNAL_CHECKS, ChId);
                IberObjectClass Ch = (IberObjectClass)ChEnum.First();
                int JobCodeN = GetJobCode(JCode);
                IberEnumClass JobCodesEnum = (IberEnumClass)Depot.GetEnum(FILE_JOB);
                IberObjectClass JobCodeOb = (IberObjectClass)JobCodesEnum.FindFromLongAttr("ID", JobCodeN);
                bool IsReprint = Convert.ToBoolean(JobCodeOb.GetBoolVal("REPRNTCHK"));
                bool ChkPintedFomOman = (AlohaTSClass.GetPrintChkAttr(ChId) > 0);
                return (IsReprint || ((Ch.GetBoolVal("PRINTED") == 0) && (!ChkPintedFomOman)));
            }
            catch
            {
                return false;
            }
        }

        static public Check GetCheckByIdExternal(AlohaExternal.AlohaCheckInfoResponse Resp)
        {
            Check Tmp = GetCheckById(Resp.CheckId);
            if (Tmp == null)
            {
                Resp.Success = false;
                Resp.AlohaErrorCode = AlohaErrEnum.ErrCOM_InvalidCheck;
                return null;
            }
            else
            {
                return Tmp;
            }

        }

        static public List<AlohaExternal.AlohaEmployeeInfo> GetEmplListExternal()
        {
            var res = new List<AlohaExternal.AlohaEmployeeInfo>();
            Utils.ToCardLog("GetEmplListExternal");
            IberEnum QEmpls = Depot.GetEnum(INTERNAL_EMPLOYEES);
            foreach (IberObject Empl in QEmpls)
            {
                try
                {
                    res.Add(new AlohaExternal.AlohaEmployeeInfo()
                    {
                        Id = Empl.GetLongVal("USERNUMBER"),
                        FirstName = Empl.GetStringVal("FIRSTNAME"),
                        SecondName = Empl.GetStringVal("LASTNAME")
                    });
                }
                catch (Exception e)
                {
                    Utils.ToCardLog($"Erorr GetEmplListExternal {e.Message}");
                }
            }


            return res;
        }


        static List<int> yandexEatTbls = new List<int>() { 231, 232, 233, 234, 235, 236, 237, 238, 239, 240 };
        static List<int> delEatTbls = new List<int>() { 200, 201, 202, 203, 204, 205, 206, 207, 208, 209 };
        static List<int> podnesTbls = new List<int>() { 241, 242, 243, 244, 245 };
        static List<int> toGo = new List<int>() { 210, 211, 212, 213, 214, 215, 216, 217, 218, 219, 220, 221, 222, 223, 224 };
        static List<int> SobsttoGo = new List<int>() { 226, 227, 228, 229, 230 };
        //  static List<int> otherEatTbls = new List<int>() { 200, 201, 202, 203, 204, 205, 206, 207, 208, 209 };
        static public List<AlohaExternal.AlohaCheckInfo> GetToGoOrdersExternal()
        {
            Utils.ToCardLog("GetToGoOrdersExternal");
            List<AlohaExternal.AlohaCheckInfo> res = new List<AlohaExternal.AlohaCheckInfo>();

            List<Check> Tmp = new List<Check>();
            IberEnum QEmpls = Depot.GetEnum(INTERNAL_EMPLOYEES);
            foreach (IberObject Empl in QEmpls)
            {
                try
                {
                    IberEnum Tbls;
                    try
                    {
                        Tbls = Empl.GetEnum(INTERNAL_EMP_OPEN_TABLES);
                        if (Tbls == null) continue;
                    }
                    catch
                    {
                        continue;
                    }

                    int emplId = Empl.GetLongVal("ID");
                    string emplName = GetWaterName(emplId);
                    foreach (IberObject tbl in Tbls)
                    {
                        try
                        {
                            int tn = tbl.GetLongVal("TABLEDEF_ID");
                            Utils.ToCardLog("GetToGoOrdersExternal find open table " + tn);
                            //if (yandexEatTbls.Contains(tn) || delEatTbls.Contains(tn) || podnesTbls.Contains(tn) || toGo.Contains(tn) || SobsttoGo.Contains(tn) || ((tn >= 180) && (tn <= 199)))
                            if ((tn >= 146 && tn <= 255) ||(tn >= 900 && tn <= 999))
                            {
                                Utils.ToCardLog("GetToGoOrdersExternal find open table in togo " + tn);
                                IberEnum chks = tbl.GetEnum(INTERNAL_TABLES_OPEN_CHECKS);
                                //var chks = GetCheckByIdShort(INTERNAL_TABLES_OPEN_CHECKS);

                                foreach (IberObject chk in chks)
                                {
                                    try
                                    {
                                        //var check = GetCheckByIdShort(INTERNAL_TABLES_OPEN_CHECKS);
                                        var toGoOrderInfo = new AlohaExternal.AlohaCheckInfo();
                                        toGoOrderInfo.AlohaId = chk.GetLongVal("ID");
                                        toGoOrderInfo.NumberInTable = chk.GetLongVal("NUMBER") + 1;


                                        double summ = 0;
                                        double tax = 0;
                                        AlohaFuncs.GetCheckTotal(toGoOrderInfo.AlohaId, out summ, out tax);
                                        toGoOrderInfo.Summ = (decimal)summ;
                                        toGoOrderInfo.TableNum = tn;
                                        toGoOrderInfo.WaiterId = emplId;
                                        toGoOrderInfo.WaiterName = emplName;
                                        toGoOrderInfo.TimeOfOpen = DateTime.UtcNow;
                                        toGoOrderInfo.TimeOfClose = DateTime.UtcNow;
                                        Utils.ToCardLog("GetToGoOrdersExternal Add table: " + tn + "; check: " + toGoOrderInfo.AlohaId + "; summ: " + summ);

                                        var alChk = GetCheckByIdShort(toGoOrderInfo.AlohaId);



                                        foreach (var d in alChk.Dishez.Where(a => a.Level == 0)) //На планшете не отображаем модификаторы
                                        {
                                            toGoOrderInfo.Dishez.Add(
                                                new AlohaExternal.AlohaItemInfo()
                                                {
                                                    Name = d.LongName,
                                                    Barcode = d.BarCode,
                                                    Price = d.Price
                                                }
                                                );
                                        }


                                        res.Add(toGoOrderInfo);
                                    }
                                    catch (Exception e)
                                    {
                                        Utils.ToCardLog("Error GetToGoOrdersExternal foreach " + e.Message);
                                    }
                                }

                            }


                            /*
                            Check AlChk = GetCheckById(Chk.GetLongVal("ID"));
                            //AlChk.SystemDate = Chk
                            Tmp.Add(AlChk);
                             * */
                        }
                        catch (Exception ee)
                        {
                            Utils.ToCardLog("Error GetToGoOrdersExternal table " + ee.Message);
                        }
                    }


                }
                catch (Exception e)
                {

                    Utils.ToCardLog("Error GetToGoOrdersExternal " + e.Message);
                }
            }
            //Resp.Checks = res;
            Utils.ToCardLog("GetToGoOrdersExternal return res count: " + res.Count);
            return res;
        }

        static public List<Check> GetChecksOfTableExternal(AlohaExternal.AlohaTableInfoResponse Resp)
        {
            List<Check> Tmp = new List<Check>();

            if (!TableExist(Resp.TNum))
            {
                Resp.Success = false;
                Resp.AlohaErrorCode = AlohaErrEnum.ErrCOM_TableNotFound;
                return null;
            }

            try
            {

                IberEnum QEmpls = Depot.GetEnum(INTERNAL_EMPLOYEES);
                foreach (IberObject Empl in QEmpls)
                {
                    try
                    {
                        IberEnum OpenTbls = Empl.GetEnum(INTERNAL_EMP_OPEN_TABLES);

                        foreach (IberObject ChTable in OpenTbls)
                        {
                            int TblDId = ChTable.GetLongVal("TABLEDEF_ID");
                            if (TblDId == Resp.TNum)
                            {
                                Resp.AlohaId = ChTable.GetLongVal("ID");

                                IberEnumClass ChecksOfTableEnum = (IberEnumClass)ChTable.GetEnum(INTERNAL_TABLES_CHECKS);

                                foreach (IberObject ChecksOfTable in ChecksOfTableEnum)
                                {
                                    try
                                    {
                                        if (ChecksOfTable.GetBoolVal("CLOSED") == 1)
                                        {
                                            continue;
                                        }
                                        int ChId = ChecksOfTable.GetLongVal("ID");
                                        Tmp.Add(GetCheckById(ChId));
                                    }
                                    catch
                                    { }
                                }

                            }
                        }
                    }
                    catch (Exception e)
                    {

                    }
                }
            }
            catch (Exception e)
            {
                Resp.Success = false;
                Resp.AlohaErrorCode = CAlohaErrors.GetAlohaErrorVal(e.Message);
                Resp.ErrorMsg = e.Message;
            }
            return Tmp;

        }

        static internal List<int> GetChecksIdInThisTable(int CheckId, out List<int> AlreadyPrintedCheck)
        {
            List<int> Tmp = new List<int>();
            AlreadyPrintedCheck = new List<int>();
            try
            {
                IberEnumClass ChEnum = (IberEnumClass)Depot.FindObjectFromId(INTERNAL_CHECKS, CheckId);

                IberObjectClass Ch = (IberObjectClass)ChEnum.First();

                int TId = Ch.GetLongVal("TABLE_ID");


                IberEnumClass TEnum = (IberEnumClass)Depot.FindObjectFromId(INTERNAL_TABLES, TId);

                IberObjectClass ChTable = (IberObjectClass)TEnum.First();

                IberEnumClass ChecksOfTableEnum = (IberEnumClass)ChTable.GetEnum(INTERNAL_TABLES_OPEN_CHECKS);

                if (ChecksOfTableEnum.Count > 1)
                {

                    foreach (IberObject ChecksOfTable in ChecksOfTableEnum)
                    {
                        try
                        {
                            if (ChecksOfTable.GetBoolVal("CLOSED") == 1)
                            {
                                continue;
                            }



                            int ChId = ChecksOfTable.GetLongVal("ID");
                            int JobCodeN = GetJobCode(AlohaCurentState.EmployeeNumberCode);
                            IberEnumClass JobCodesEnum = (IberEnumClass)Depot.GetEnum(FILE_JOB);
                            IberObjectClass JobCodeOb = (IberObjectClass)JobCodesEnum.FindFromLongAttr("ID", JobCodeN);
                            bool IsReprint = Convert.ToBoolean(JobCodeOb.GetBoolVal("REPRNTCHK"));
                            if (!IsReprint && (((ChecksOfTable.GetBoolVal("PRINTED") > 0) || (GetPrintChkAttr(ChId) > 0))) && (ChId != CheckId))
                            {
                                AlreadyPrintedCheck.Add(ChId);
                            }
                            else
                            {
                                Tmp.Add(ChId);
                            }
                        }
                        catch (Exception e)
                        {
                            Utils.ToCardLog("[Error] Ошибка во время получения параметров чека на столе " + e.Message);
                        };
                    }
                    //Tmp.Remove(CheckId);
                }
            }
            catch (Exception e)
            {
                Utils.ToCardLog("Error Запрос чеков на столе " + e.Message);
            }
            return Tmp;

        }

        static internal string GetCompName(int Id)
        {
            try
            {
                IberEnumClass CmpsEnum = (IberEnumClass)Depot.GetEnum(FILE_CMP);
                IberObject Cmp = CmpsEnum.FindFromLongAttr("ID", Id);
                string CompName = Cmp.GetStringVal("NAME");
                return CompName;
            }
            catch
            {
                return "";
            }
        }

        static internal int GetCompType(int Id)
        {
            try
            {
                IberEnumClass CmpsEnum = (IberEnumClass)Depot.GetEnum(FILE_CMP);
                IberObject Cmp = CmpsEnum.FindFromLongAttr("ID", Id);
                int CompType = Cmp.GetBoolVal("ENTERAMT");
                //Utils.ToCardLog($"GetCompType Id{}")
                return CompType;
            }
            catch
            {
                return 0;
            }
        }

        static internal string GetTenderName(int Id)
        {
            try
            {
                IberEnumClass CmpsEnum = (IberEnumClass)Depot.GetEnum(FILE_TDR);
                IberObject Cmp = CmpsEnum.FindFromLongAttr("ID", Id);
                string CompName = Cmp.GetStringVal("NAME");
                return CompName;
            }
            catch
            {
                return "";
            }
        }





        static internal string GetEntryName(int Id)
        {
            try
            {
                IberEnumClass ChEnum = (IberEnumClass)Depot.FindObjectFromId(INTERNAL_ENTRIES, Id);
                IberObjectClass Ch = (IberObjectClass)ChEnum.First();
                string EntrName = Ch.GetStringVal("DISP_NAME");
                return EntrName;
            }
            catch
            {
                return "";
            }
        }


        static internal int GetEntryBarCodeAndPrice(int Id, out double Price)
        {
            Price = 0;
            try
            {
                IberEnumClass ChEnum = (IberEnumClass)Depot.FindObjectFromId(INTERNAL_ENTRIES, Id);
                IberObjectClass Ch = (IberObjectClass)ChEnum.First();
                Price = Ch.GetDoubleVal("PRICE");
                return Ch.GetLongVal("DATA");

            }
            catch
            {
                return 0;
            }
        }

        static internal double GetPaymentSumm(int Id)
        {

            try
            {
                IberEnumClass ChEnum = (IberEnumClass)Depot.FindObjectFromId(INTERNAL_PAYMENTS, Id);
                IberObjectClass Ch = (IberObjectClass)ChEnum.First();
                double Price = Ch.GetDoubleVal("AMOUNT");
                return Price;

            }
            catch
            {
                return 0;
            }
        }



        static internal bool GetCheckDiscountById(int Id)
        {
            try
            {
                Check Ch2 = new Check();


                IberEnumClass ChEnum = (IberEnumClass)Depot.FindObjectFromId(INTERNAL_CHECKS, Id);
                IberObjectClass Ch = (IberObjectClass)ChEnum.First();

                IberEnumClass CompsEnum = (IberEnumClass)Ch.GetEnum(INTERNAL_CHECKS_COMPS);
                decimal Amount = 0;
                foreach (IberObject mComp in CompsEnum)
                {
                     Amount += Convert.ToDecimal(mComp.GetDoubleVal("AMOUNT"));
                }

                Ch2.Comp = Amount;
                IberObjectClass Comp = (IberObjectClass)CompsEnum.First();
                Ch2.CompId = Comp.GetLongVal("COMPTYPE_ID");
                return true;
            }
            catch
            {
                return false;
            }
        }

        static internal Check GetCheckByIdShort(int Id)
        {
            try
            {
                Utils.ToLog("Запрос чека короткий" + Id.ToString(), 6);
                Check Ch2 = new Check();
                if (Depot == null)
                {
                    Utils.ToLog("Depot == null", 6);
                    return null;
                }
                IberEnumClass ChEnum = (IberEnumClass)Depot.FindObjectFromId(INTERNAL_CHECKS, Id);
                IberObjectClass Ch = (IberObjectClass)ChEnum.First();
                IberEnum Dishez = new IberEnum();
                try { Dishez = Ch.GetEnum(INTERNAL_CHECKS_ENTRIES); }
                catch { }
                try
                {
                    int TId = Ch.GetLongVal("TABLE_ID");
                    IberObjectClass ChTable = null;
                    IberEnumClass TEnum = (IberEnumClass)Depot.FindObjectFromId(INTERNAL_TABLES, TId);
                    ChTable = (IberObjectClass)TEnum.First();
                    Ch2.TableId = TId;
                    Ch2.TableNumber = ChTable.GetLongVal("TABLEDEF_ID");
                }
                catch
                { }

                foreach (IberObject IntITM in Dishez)
                {

                    if (((IntITM.GetLongVal("TYPE") == 0) || (IntITM.GetLongVal("TYPE") == 6)) && ((IntITM.GetLongVal("MOD_CODE") != 8 && (IntITM.GetLongVal("MOD_CODE") != 12))))
                    //Regular item                          MOD_DELETED                             MOD_PRINTED_DELETED
                    {
                        Dish Md = new Dish()
                        {
                            Count = 1,
                            Level = IntITM.GetLongVal("LEVEL"),
                            Name = IntITM.GetStringVal("DISP_NAME"),

                            QUANTITY = IntITM.GetLongVal("QUANTITY"),
                            LongName = IntITM.GetStringVal("DISP_LONGNAME"),
                        };

                        try
                        {
                            string s = IntITM.GetStringVal("DISP_PRICE").Replace(".", GetDemSep()).Replace(",", GetDemSep());
                            Md.Price = Convert.ToDecimal(s);
                        }
                        catch
                        {
                            Md.Price = 0;
                        }

                        Ch2.Dishez.Add(Md);
                    }
                }

                //пытаемся дать весовое количество
                foreach (Dish d in Ch2.Dishez)
                {
                    try
                    {
                        string unname = "";
                        int Vozvr = (Ch2.Vozvr) ? (-1) : 1;
                        double qPrice = DishIsQty(d.BarCode, out unname);
                        if (qPrice != -1)
                        {
                            qPrice = qPrice * Vozvr;
                            if (d.OPrice == 0)
                            {
                                d.QtyQUANTITY = 0;
                                d.Priceone = (double)d.Price;
                            }
                            else
                            {
                                d.QtyQUANTITY = (decimal)(d.OPrice / (decimal)qPrice);
                                d.Priceone = (double)d.Price / (double)d.QtyQUANTITY;
                            }
                            d.OPriceone = qPrice;
                        }
                        else
                        {
                            d.OPriceone = (double)d.OPrice;
                            d.Priceone = (double)d.Price;
                        }
                    }
                    catch
                    {

                    }
                }
                //Оплата
                try
                {
                    IberEnumClass PayMentsEnum = (IberEnumClass)Ch.GetEnum(INTERNAL_CHECKS_PAYMENTS);
                    foreach (IberObject mPayMent in PayMentsEnum)
                    {
                        AlohaTender Tndr = new AlohaTender()
                        {
                            TenderId = mPayMent.GetLongVal("TENDER_ID"),
                            SummWithOverpayment = mPayMent.GetDoubleVal("AMOUNT")
                        };
                        Tndr.Name = GetTenderName(Tndr.TenderId);
                        Ch2.Tenders.Add(Tndr);
                    }
                }
                catch
                {

                }
                //Скидки
                try
                {
                    IberEnumClass CompsEnum = (IberEnumClass)Ch.GetEnum(INTERNAL_CHECKS_COMPS);

                    decimal Amount = 0;
                    foreach (IberObject mComp in CompsEnum)
                    {
                        Amount += Convert.ToDecimal(mComp.GetDoubleVal("AMOUNT"));
                    }

                    IberObjectClass Comp = (IberObjectClass)CompsEnum.First();
                    Ch2.Comp = Amount;
                    Ch2.CompId = Comp.GetLongVal("COMPTYPE_ID");
                    Ch2.CompName = GetCompName(Ch2.CompId);
                }
                catch
                { }


                double Sum1;
                double mTax1;
                AlohaFuncs.GetCheckTotal(Id, out Sum1, out mTax1);
                Ch2.Summ = (decimal)Sum1;

                return Ch2;
            }
            catch (Exception e)
            {
                Utils.ToLog("Error Короткий запрос чека " + Id.ToString() + " " + e.Message, 6);
                return null;
            }
        }


        static internal List<Check> GetAllChecks()
        {
            List<Check> Tmp = new List<Check>();
            IberEnum QEmpls = Depot.GetEnum(INTERNAL_EMPLOYEES);
            foreach (IberObject Empl in QEmpls)
            {
                try
                {
                    IberEnum Checks = Empl.GetEnum(INTERNAL_EMP_CLOSED_CHECKS);
                    foreach (IberObject Chk in Checks)
                    {
                        Check AlChk = GetCheckById(Chk.GetLongVal("ID"));
                        //AlChk.SystemDate = Chk
                        Tmp.Add(AlChk);
                    }
                }
                catch
                { }
            }
            return Tmp;
        }

        static internal Check GetCheckById(int Id)
        {
            try
            {
                Utils.ToLog("**************************************Запрос чека " + Id.ToString() + "****************************************", 6);
                Check Ch2 = new Check();

                if (Depot == null)
                {
                    Utils.ToLog("Depot == null", 6);
                    return null;
                }

                IberEnumClass ChEnum = (IberEnumClass)Depot.FindObjectFromId(INTERNAL_CHECKS, Id);
                //Utils.ToLog("Получил ChEnum", 6);
                IberObjectClass Ch = (IberObjectClass)ChEnum.First();
                //Utils.ToLog("Получил Ch", 6);
                Ch2.Vozvr = (!(Ch.GetLongVal("REFUND") == 0));
                IberObjectClass ChTable = null;
                try
                {
                    int TId = Ch.GetLongVal("TABLE_ID");
                    Ch2.TableId = TId;
                    Utils.ToLog("Получил TId=" + TId.ToString(), 6);
                    IberEnumClass TEnum = (IberEnumClass)Depot.FindObjectFromId(INTERNAL_TABLES, TId);
                    Utils.ToLog("Получил TEnum размер " + TEnum.Count, 6);
                    ChTable = (IberObjectClass)TEnum.First();
                    int g = GetGuestCountAttr(Id);
                    if (g == 0)
                    {
                        Ch2.Guests = ChTable.GetLongVal("NUM_GUESTS");
                    }
                    else
                    {
                        Ch2.Guests = g;
                    }
                    //Utils.ToLog(String.Format("Гостей на столе {0}, Гостей в чеке {1}", ChTable.GetLongVal("NUM_GUESTS"), Ch.GetLongVal("GUESTS")));
                    Ch2.TableNumber = ChTable.GetLongVal("TABLEDEF_ID");
                    Ch2.TableName = ChTable.GetStringVal("NAME");
                    Ch2.TableDescription = GetTableDesc(Ch2.TableNumber);
                    Utils.ToLog("Получил ChTable Ch2.TableNumber " + Ch2.TableNumber + " Ch2.TableName " + Ch2.TableName, 6);
                }
                catch (Exception e)
                {
                    Utils.ToCardLog("Error. При считывании стола " + e.Message);
                }

                try
                {
                    Utils.ToLog("Запрос текущего состояния.", 6);
                    if (AlohaTSClass.CheckWindow())
                    {
                        Ch2.Cassir = AlohaTSClass.AlohaCurentState.EmployeeNumberCode;
                        Ch2.TerminalId = AlohaTSClass.AlohaCurentState.TerminalId;
                    }
                    Utils.ToLog("Параметры текущего состояния получены.", 6);
                }
                catch
                {
                    Utils.ToLog("[Error] Ошибка получения Параметров текущего состояния.", 1);
                }



                if (AlohaFuncs == null)
                {
                    Utils.ToLog("AlohaFuncs == null", 6);
                }

                double Sum;
                double mTax;
                AlohaFuncs.GetCheckTotal(Id, out Sum, out mTax);
                Utils.ToLog("Получил GetCheckTotal " + Sum, 6);
                Ch2.Summ = Convert.ToDecimal(Sum);
                /*
                if (Sum == 0)
                { 
                    Utils.ToLog("Сумма 0. Выхожу из GetCheckById ", 6);
                    return Ch2;
                }
                */
                bool _IsNal = true;
                IberObjectClass PayMent = null;
                double NeedDiscountForCert = 0;
                try
                {
                    //Ch2.Oplata = 0;
                    IberEnumClass PayMentsEnum = (IberEnumClass)Ch.GetEnum(INTERNAL_CHECKS_PAYMENTS);
                    Utils.ToLog("Получил PayMentsEnum ", 6);
                    // IberObjectClass PayMent = (IberObjectClass)PayMentsEnum.First();

                    PayMent = GetLast(PayMentsEnum);
                    Utils.ToLog("Получил PayMent ", 6);
                    int Tender = PayMent.GetLongVal("TENDER_ID");
                    //Ch2.TenderId = Tender;
                    _IsNal = (Tender == 1);

                    Utils.ToLog("Получил _IsNal= " + _IsNal.ToString(), 6);

                    foreach (IberObject mPayMent in PayMentsEnum)
                    {
                        Ch2.PaymentsIds.Add(mPayMent.GetLongVal("ID"));
                        //Ch2.Oplata += Convert.ToDecimal(mPayMent.GetDoubleVal("AMOUNT"));

                        AlohaTender Tndr = new AlohaTender()
                        {
                            AlohaTenderId = mPayMent.GetLongVal("TENDER_ID"),
                            Summ = mPayMent.GetDoubleVal("AMOUNT"),
                            SummWithOverpayment = mPayMent.GetDoubleVal("AMOUNT"),
                        };

                        if (Tndr.AlohaTenderId == 22)
                        {
                            Tndr.AlohaTenderId = 21;
                        }
                        try { Tndr.AuthId = mPayMent.GetLongVal("AUTH_CODE"); }
                        catch { Utils.ToLog("Error Payment AUTH_CODE ", 6); }
                        try { Tndr.Ident = mPayMent.GetStringVal("IDENT"); }
                        catch { Utils.ToLog("Error Payment IDENT ", 6); }
                        try { Tndr.NR = mPayMent.GetDoubleVal("NR"); }
                        catch { Utils.ToLog("Error Payment NR ", 6); }
                        try { Tndr.GCAMOUNT = mPayMent.GetDoubleVal("GCAMOUNT"); }
                        catch { Utils.ToLog("Error Payment GCAMOUNT ", 6); }
                        try { Tndr.GCREDEEM = mPayMent.GetDoubleVal("GCREDEEM"); }
                        catch { Utils.ToLog("Error Payment GCREDEEM ", 6); }
                        try { Tndr.GCTYPE = mPayMent.GetStringVal("GCTYPE"); }
                        catch { Utils.ToLog("Error Payment GCTYPE ", 6); }


                        Tndr.Name = GetTenderName(Tndr.TenderId);
                        Utils.ToCardLog(String.Format("PayMent ID: {0}, Name: {1}, AUTH_CODE:{2}, IDENT:{3}, NR:{4}, GCTYPE:{5}, GCAMOUNT:{6}, GCREDEEM:{7} ",
                            Tndr.TenderId, Tndr.Name, Tndr.AuthId, Tndr.Ident, Tndr.NR, Tndr.GCTYPE, Tndr.GCAMOUNT, Tndr.GCREDEEM));

                        
                        if ((Tndr.AlohaTenderId == 25) && ((Tndr.CardPrefix == "77277") || (Tndr.CardPrefix == "NzcyN")))
                        {
                            NeedDiscountForCert += Tndr.Summ;
                            var cc = new AlohaClientCard()
                            {
                                TypeId = "03",
                                Number = Tndr.CardNumber,
                                Prefix = Tndr.CardPrefix,
                                Payment = Convert.ToInt32(Tndr.Summ * 100),
                                BonusRemove = Convert.ToInt32(Tndr.Summ * 100),
                            };

                            Ch2.AlohaClientCardListCertifDisk.Add(cc);
                            Utils.ToCardLog("Addd to AlohaClientCardListCertifDisk " + cc.Prefix + " " + cc.Number);

                            Ch2.Summ -= (decimal)Tndr.Summ;
                            continue;
                        }


                        Ch2.Tenders.Add(Tndr);
                    }


                }
                catch (Exception ee)
                {
                    Utils.ToLog("Оплата на стол не наложена ");
                }
                IberEnumClass EmplEnum = (IberEnumClass)Ch.GetEnum(INTERNAL_CHECKS_TIPPABLE_EMP);
                //Utils.ToLog("Получил EmplEnum long: " + EmplEnum., 6);
                Utils.ToLog("Перечисление INTERNAL_CHECKS_TIPPABLE_EMP", 6);
                IberObjectClass Emp = (IberObjectClass)EmplEnum.First();
                Utils.ToLog("Получил Emp ", 6);

                Ch2.AlohaCheckNum = Id;
                Ch2.NumberInTable = Ch.GetLongVal("NUMBER") + 1;

                Ch2.Vozvr = (!(Ch.GetLongVal("REFUND") == 0));
                Ch2.IsClosed = (Ch.GetLongVal("CLOSED") == 1);
                if (Ch2.IsClosed)
                {
                    Ch2.CloseTime = (long)Ch.GetDoubleVal("CLOSETIME");
                }


                Utils.ToLog("CLOSED: " + Ch.GetLongVal("CLOSED"));

                if (!Ch2.Vozvr)
                {
                    Ch2.Vozvr = Sum < 0;
                }

                Utils.ToLog("Получил Ch2.Vozvr " + Ch.GetLongVal("REFUND"), 6);
                try
                {
                    Ch2.OpenTime = (long)Ch.GetDoubleVal("OPENTIME");
                    Utils.ToLog("Получил Ch2.OpenTime  ", 6);
                    Ch2.TerminalId = Ch.GetLongVal("TERMINALID");
                    Utils.ToLog("Получил Ch2.TerminalId ", 6);

                    int WaterForOborot = 0;
                    try
                    {
                        string WaterForOborotStr = AlohaFuncs.GetObjectAttribute(INTERNAL_CHECKS, Id, "WtOb");
                        if (WaterForOborotStr != "")
                        {
                            WaterForOborot = Convert.ToInt32(WaterForOborotStr);
                        }
                    }
                    catch (Exception ee)
                    {
                        Utils.ToCardLog("Нестандартный оборот не используется " + ee.Message);
                    }
                    if (WaterForOborot > 0)
                    {
                        Ch2.Waiter = WaterForOborot;
                        Utils.ToLog("Назначен Ch2.Waiter " + WaterForOborot, 6);
                    }
                    else
                    {
                        Ch2.Waiter = Emp.GetLongVal("USERNUMBER");
                    }



                    try
                    {
                        if (iniFile.DisableNonWaiterOborot)
                        {
                            if (Ch2.Summ > 0)
                            {
                                int JobCode = GetJobCode(Ch2.Waiter);
                                if (JobCode != 20)
                                {
                                    Utils.ToCardLog("Waiter не является официантом. Его должность: " + JobCode.ToString() + " код: " + Ch2.Waiter.ToString() + " Меняю код на 1111");
                                    Ch2.Waiter = 1111;
                                }
                            }
                        }

                    }
                    catch
                    {

                    }

                    Utils.ToLog("Получил Ch2.Waiter: " + Ch2.Waiter.ToString(), 6);



                    Ch2.PredcheckCount = Ch.GetLongVal("PRINTED");
                    Utils.ToLog("Получил Ch2.PredcheckCount ", 6);
                }
                catch (Exception e)
                {
                    Utils.ToCardLog("[Error] Не удалось прочесть необязательные параметры чека " + e.Message);
                }

                double PromoAmmount = 0;
                try
                {
                    IberEnum Promos = Ch.GetEnum(INTERNAL_CHECKS_PROMOS);
                    Utils.ToCardLog("Промо получено ");
                    foreach (IberObject Promo in Promos)
                    {
                        PromoAmmount += Promo.GetDoubleVal("AMOUNT");
                        Utils.ToCardLog("Ammount " + PromoAmmount.ToString());
                    }
                }
                catch (Exception e)
                {
                    Utils.ToCardLog("[Error] Не удалось прочесть Promos  " + e.Message);
                }


                try
                {
                    IberEnumClass ChecksOfTableEnum = (IberEnumClass)ChTable.GetEnum(INTERNAL_TABLES_OPEN_CHECKS);
                    Utils.ToLog("Получил ChecksOfTableEnum ", 6);
                    if (ChecksOfTableEnum.Count > 1)
                    {
                        Utils.ToLog("ChecksOfTableEnum.Count > 1. Начал перебор чеков на этом столе ", 6);
                        foreach (IberObject ChecksOfTable in ChecksOfTableEnum)
                        {
                            try
                            {
                                if (ChecksOfTable.GetBoolVal("CLOSED") == 1)
                                {
                                    continue;
                                }

                                //int q= ChecksOfTable.GetLongVal("CLOSETIME");
                                Check Ch3 = new Check();
                                Ch3.AlohaCheckNum = ChecksOfTable.GetLongVal("ID");

                                Utils.ToLog("Получил ChecksOfTable.GetLongVal(ID) " + Ch3.AlohaCheckNum, 6);
                                double Sum1;
                                double mTax1;

                                AlohaFuncs.GetCheckTotal(Ch3.AlohaCheckNum, out Sum1, out mTax1);
                                Utils.ToLog("Получил GetCheckTotal", 6);
                                Ch3.Summ = (decimal)Sum1;
                                Ch2.ChecksOnTable.Add(Ch3);
                            }
                            catch (Exception e)
                            {
                                Utils.ToCardLog("[Error] Ошибка во время получения параметров чека на столе " + e.Message);
                            };
                        }
                        Utils.ToLog("Закончил перебор чеков на этом столе ", 6);
                    }
                }
                catch (Exception e)
                {
                    Utils.ToCardLog("Error Запрос чеков на столе " + e.Message);
                }

                try
                {
                    IberEnumClass CompsEnum = (IberEnumClass)Ch.GetEnum(INTERNAL_CHECKS_COMPS);
                    Utils.ToLog("Получил CompsEnum  ", 6);
                    IberObjectClass Comp = (IberObjectClass)CompsEnum.First();
                    Utils.ToLog("Получил Comp ", 6);

                    decimal Amount = 0;
                    foreach (IberObject mComp in CompsEnum)
                    {
                        Amount += Convert.ToDecimal(mComp.GetDoubleVal("AMOUNT"));
                    }

                    Ch2.Comp = Amount;
                    Utils.ToLog("Получил Ch2.Comp =" + Ch2.Comp, 6);
                    Ch2.CompId = Comp.GetLongVal("COMPTYPE_ID");
                    Utils.ToLog("Получил Ch2.CompId  " + Ch2.CompId, 6);
                    Ch2.CompDescription = Comp.GetStringVal("NAME");
                    Ch2.CompName = GetCompName(Ch2.CompId);
                    Utils.ToLog("Получил Ch2.CompName=" + Ch2.Comp, 6);

                    Ch2.DiscountMGR_NUMBER = Comp.GetLongVal("MGR_NUMBER");
                    try
                    {
                        string mName = "";
                        int mId = 0;
                        if (GetManagerDiscountAttr(Id, out mName, out mId))
                        {
                            Ch2.DegustationMGR_NUMBER = mId;
                            Ch2.CompDescription = mName;
                        }
                        else
                        {
                            Ch2.DegustationMGR_NUMBER = Convert.ToInt32(Comp.GetStringVal("UNIT"));
                        }
                    }
                    catch
                    { }
                    if (Ch2.DegustationMGR_NUMBER == 0)
                    {
                        Ch2.DegustationMGR_NUMBER = Ch2.DiscountMGR_NUMBER;
                    }
                    Utils.ToLog("Получил Ch2.DiscountMGR_NUMBER =" + Ch2.DiscountMGR_NUMBER, 6);

                    foreach (IberObject mComp in CompsEnum)
                    {
                        int CompId = mComp.GetLongVal("COMPTYPE_ID");
                        AlohaComp AlCmp = new AlohaComp()
                        {
                            Amount = Convert.ToDecimal(mComp.GetDoubleVal("AMOUNT")),
                            Description = mComp.GetStringVal("NAME"),
                            Id = CompId,
                            Name = GetCompName(CompId),
                            CompType = GetCompType(CompId)
                        };
                        Ch2.Comps.Add(AlCmp);
                    }


                }
                catch
                {
                    Utils.ToLog("Скидок на столе нет ", 3);
                }

                if ((Ch2.Comp == 0) && (PromoAmmount > 0))
                {

                    Ch2.CompId = 1;
                    //Ch2. = 1;
                }
                else
                {

                }
                Ch2.Comp += (decimal)PromoAmmount;

                FixOverPayment(Ch2);

                //  Ch2.CardPaymentId = GetPaymentAttr(Ch2.AlohaCheckNum);
                //  Utils.ToLog("Получил Ch2.CardPaymentId   " + Ch2.CardPaymentId, 6);
                try
                {
                    decimal ManualDiscAmout = 0;

                    if (Ch2.Comps.Count < 2)
                    {
                       ManualDiscAmout = Ch2.Comps.Where(a => a.CompType == 1).Sum(b => b.Amount);
                    }
                    else
                    {
                        
                        if (Ch2.Comps[0].Id == 77)
                        {
                            ManualDiscAmout = Ch2.Comp;
                        }
                    }
                    

                    Ch2.Dishez = GetDishesOfCheck(Id, Ch2, ManualDiscAmout);
                    //Ch2.SetConSolidateDishez();
                    Utils.ToLog("Получил Ch2.Dishez    ", 6);
                }
                catch (Exception e)
                {
                    Utils.ToCardLog("[Error] Ошибка во время получения параметров блюд в чеке " + e.Message);
                }

                CheckCertifDiscount(Ch2, (decimal)NeedDiscountForCert);


                if (AlohaTSClass.GetDiscountAttr(Ch2.AlohaCheckNum) != "")
                {
                    Ch2.DiscountCard = AlohaTSClass.GetDiscountAttr(Ch2.AlohaCheckNum);
                }

                Loyalty.LoyaltyBasik.InsertASVCardInfo(Ch2);


                Utils.ToLog("PMSGuestName", 6);
                try
                {
                    var lines = Ch.GetEnum(INTERNAL_CHECKS_ENTRIES);
                    foreach (IberObject l in lines)
                    {
                        if (l.GetLongVal("TYPE") == 145)
                        {
                            Ch2.PMSGuestName = l.GetStringVal("DISP_NAME");
                            Utils.ToLog("PMSGuestName =" + Ch2.PMSGuestName, 6);
                        }

                    }

                }
                catch (Exception e)
                {
                    Utils.ToLog("Error PMSGuestName " + e.Message, 6);
                }



                try
                {

                    if (!IsAlohaTS())
                    {
                        Utils.ToLog("For QS check 999991 togo");
                        if (Ch2.Dishez.Any(a => a.BarCode == 999991))
                        {
                            Utils.ToLog("Find 999991 barcode. Set 777 table");
                            Ch2.TableNumber = 777;
                        }
                    }
                }
                catch { }

                var chk = Loyalty.LoyaltyBasik.CheckConvertForBOnusCard(Ch2);
                Utils.ToLog("*****************Окончание запроса чека " + Id.ToString() + "****************************************", 6);
                return chk;


            }
            catch (Exception ee)
            {
                Utils.ToCardLog("***********************************[Error] Критическая Ошибка считывания чека **********************************" + ee.Message);
                return null;
            }
        }





        static private void CheckCertifDiscount(Check Ch2, decimal NeedDiscountForCert)
        {
            Utils.ToLog("CheckCertifDiscount " + NeedDiscountForCert.ToString(), 6);
            double Chs = GetCheckSum(Ch2.AlohaCheckNum);
            if (NeedDiscountForCert > 0 && Ch2.Dishez != null)
            {
                try
                {
                    Utils.ToLog("NeedDiscountForCert    ", 6);
                    if ((decimal)Chs <= NeedDiscountForCert)
                    {
                        foreach (var d in Ch2.Dishez)
                        {
                            d.Priceone = 0;
                            d.Price = 0;
                        }
                        Ch2.Summ = 0;
                    }
                    else
                    {
                        var k = 1 - ((double)NeedDiscountForCert) / Chs;
                        foreach (var d in Ch2.Dishez)
                        {
                            d.Priceone *= (double)k;
                            d.Price *= (decimal)k;
                        }
                        // Ch2.Summ *= (decimal)k;

                        //Все что ниже - это борьба с копейкой
                        try
                        {
                            decimal DSumm = 0;
                            Dish MaxDish = null;
                            decimal MaxSumm = 0;
                            foreach (Dish d in Ch2.Dishez)
                            {
                                if (d.BarCode == 999901) continue;

                                if (iniFile.FRPriceFromDisplay)
                                {
                                    DSumm += (decimal)Math.Round(d.Price, 2, MidpointRounding.ToEven) + d.ServiceChargeSumm;
                                }
                                else
                                {
                                    DSumm += (decimal)Math.Round(d.Priceone * (double)d.Count * (double)d.QUANTITY * (double)d.QtyQUANTITY, 2, MidpointRounding.ToEven) + d.ServiceChargeSumm;
                                }

                                if (Math.Abs(MaxSumm) < Math.Abs(d.Price))
                                {
                                    MaxSumm = d.Price;
                                    MaxDish = d;
                                }
                            }

                            double Delta = Chs - (double)DSumm;
                            if (Math.Abs(Delta) >= 0.005)
                            {
                                if (Math.Abs(Delta) > 0.05 * Chs)
                                {
                                    Ch2.ServiceChargeSumm = Math.Abs((decimal)Delta);

                                }
                                else
                                {
                                    MaxDish.Price = MaxDish.Price + (decimal)Delta;
                                    MaxDish.Delta = (decimal)Delta;
                                    Utils.ToCardLog("Добавил разницу сумм: " + Delta.ToString() + " к блюду :" + MaxDish.Name);
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Utils.ToCardLog("[Error] Коррекция разницы суммы по блюдам и суммы чека " + e.Message);
                        }
                    }
                }
                catch (Exception e)
                {

                }


            }

        }



        static private void FixOverPayment(Check chk)
        {
            if (chk.Vozvr) return;
            if (chk.Summ < (decimal)chk.Tenders.Where(a => a.AuthId == 2).Sum(a => a.Summ))
            {
                if (chk.Tenders.Where(a => a.TenderId == AlohaTender.CashTenderId).Count() > 0)
                {
                    chk.Tenders.Where(a => a.TenderId == AlohaTender.CashTenderId).First().Summ -=
 chk.Tenders.Where(a => a.AuthId == 2).Sum(a => a.Summ) - (double)chk.Summ;

                }
            }
        }

        //public const int BonusPaymentId = 25;
        //public const int CreditPaymentId = 25;
        public const int PredoplataPaymentId = 30;
        public const int BonusCompId = 101;


        static internal IberPrinterClass Ap;
        static public bool InitAlohaCom()
        {
            try
            {
                Utils.ToLog("InitAlohaCom...");
                Depot = new IberDepot();
                Utils.ToLog("Depot ");

                //AlohaFuncs = new IberFuncs();


                Utils.ToLog("Активирую AlohaFuncs");
                try
                {
                    WinApi.IClassFactory2 icf2 = WinApi.CoGetClassObject(typeof(IberFuncsClass).GUID, WinApi.CLSCTX.CLSCTX_ALL, new System.IntPtr(), typeof(WinApi.IClassFactory2).GUID) as WinApi.IClassFactory2;
                    string Lic = "1L0S0V063S311V0K4H2?1:2Z475=5H4G0<4T0U53224;1T4R4>432K1B5X5P515B0Z1P1K3C";
                    AlohaFuncs = icf2.CreateInstanceLic(null, null, typeof(IberFuncs).GUID, Lic) as IberFuncs;
                    Utils.ToLog("AlohaFuncs Success");
                }
                catch (Exception e)
                {
                    Utils.ToLog("Error Активирую AlohaFuncs " + e.Message);
                }




                try
                {
                    Ap = new IberPrinterClass(); //Это для корректной печати предчеков с нескольких дробей.
                    Utils.ToLog("Ap ");
                    CheckPrintINfo();
                }
                catch (Exception ee)
                {
                    Utils.ToLog("Error InitAp " + ee.Message);
                }
                //  SInit() // - удалить

                //if (!iniFile.FRModeDisabled)
                {
                    InitTorts();
                    Utils.ToLog("InitTorts ");
                    InitAlc();
                    Utils.ToLog("InitAlcs ");
                }

                return true;

            }
            catch (Exception e)
            {
                Utils.ToLog("Error InitAlohaCom " + e.Message);
                return false;
            }

        }



        static internal void WriteToDebout(string Message)
        {
            AlohaFuncs.LogDeboutMessage(Message);
        }


        public delegate void SendMessageEventHandler(string Mess);
        static public event SendMessageEventHandler SendMessageEvent;
        static private void SendMessageEventToUser(string Message)
        {
            if (SendMessageEvent != null)
            {
                //InternalConnections.
                SendMessageEvent(Message);
            }
        }


        static internal void DeleteLoyaltyMember(int Empl, int TableId, int Check)
        {
            try
            {
                Utils.ToCardLog("DeleteEFreqMember");
                AlohaFuncs.DeleteEFreqMember(GetTermNum(), Empl, TableId, Check);
            }
            catch (Exception e)
            {
                Utils.ToCardLog("[Error] DeleteEFreqMember " + e.Message);
            }
        }

        static internal void ShowMessageExternal(string Message)
        {
            AlohaFuncs.DisplayMessage(Message);
        }

        static internal void ShowMessage(string Message)
        {
            if (!GetCurentStateFromArgs)
            {


                if (MainClass.CurentfrmCardMooverEnable == true)
                {
                    Utils.ToLog("[ShowMessage] Показал сообщение на форме: " + Message);


                    MainClass.CurentfrmCardMoover.SetInfo(Message);
                }
                else
                {
                    Utils.ToLog("[ShowMessage] Показал сообщение: " + Message);
                    AlohaFuncs.DisplayMessage(Message);
                    //Thread.Sleep(2000);
                }

            }
            else
            {
                Utils.ToLog("[ShowMessage] Отправил сообщение: " + Message);
                SendMessageEventToUser(Message);
            }

        }
        static internal void ShowMessageInternal(string Message)
        {
            if (GetCurentStateFromArgs)
            {
                Utils.ToLog("[ShowMessage] Отправил сообщение: " + Message);
                SendMessageEventToUser(Message);
            }

        }
        const int INTERNAL_LOCALSTATE_CUR_ENTRY = 725;
        const int INTERNAL_LOCALSTATE_ITEMINFOS = 731;
        static internal List<OrderDish> GetSelectedItems(bool All)
        {
            List<OrderDish> Tmp = new List<OrderDish>();
            try
            {
                IberEnumClass LocaleStateEnum = (IberEnumClass)Depot.GetEnum(INTERNAL_LOCALSTATE);
                IberObjectClass LocaleState = (IberObjectClass)LocaleStateEnum.First();


                IberEnumClass CurStateEnumChecks = (IberEnumClass)LocaleState.GetEnum(INTERNAL_LOCALSTATE_CUR_CHECK);
                IberObjectClass CurStateCheck = (IberObjectClass)CurStateEnumChecks.First();
                int ChId = CurStateCheck.GetLongVal("ID");


                IberEnumClass ChEnum = (IberEnumClass)Depot.FindObjectFromId(INTERNAL_CHECKS, ChId);

                IberObjectClass Ch = (IberObjectClass)ChEnum.First();

                /*
                    IberEnum Dishez = new IberEnum();
                foreach (IberObject IntITM in Dishez)
                 */
                //Dishez =Ch.GetEnum(INTERNAL_CHECKS_ENTRIES)




                foreach (IberObject ChEntr in CurStateCheck.GetEnum(INTERNAL_CHECKS_ENTRIES))
                {
                    if (!All)
                    {
                        if ((ChEntr.GetBoolVal("SELECTED")) != 1)
                        {
                            continue;
                        }
                    }
                    if ((ChEntr.GetLongVal("TYPE") == 0) && ((ChEntr.GetLongVal("LEVEL") == 0)))
                    {
                        OrderDish Od = new OrderDish()
                        {
                            barccode = ChEntr.GetLongVal("DATA"),
                            Id = ChEntr.GetLongVal("ID"),

                            CheckId = ChId
                        };
                        try
                        {
                            Od.Vrouting = ChEntr.GetLongVal("VROUTING");
                        }
                        catch
                        {

                        }
                        Tmp.Add(Od);

                    }
                }
            }
            catch
            { }
            return Tmp;

        }



        static internal bool OrderAllItems(int CheckId, int TableId, out string ExeptionMessage)
        {
            try
            {
                ExeptionMessage = "";
                Utils.ToCardLog("OrderAllItems ");
                int termNum = GetTermNum();
                /*
                AlohaFuncs.DeselectAllEntries(termNum);
                foreach (int Od in OdList)
                {
                    AlohaFuncs.SelectEntryAndChildren(termNum, CheckId, Od);
                }
                 * */
                AlohaFuncs.SelectAllEntriesOnCheck(termNum, CheckId);
                AlohaFuncs.OrderItems(termNum, TableId, 1);

                AlohaFuncs.DeselectAllEntries(termNum);
                Utils.ToCardLog("OrderAllItems End");
                return true;
            }
            catch (Exception e)
            {
                Utils.ToCardLog("[Error] OrderAllItems " + e.Message);
                ExeptionMessage = e.Message;
                return false;
            }
        }

        static internal bool OrderItems(List<int> OdList, int CheckId, int TableId, out string ExeptionMessage)
        {
            try
            {
                ExeptionMessage = "";
                Utils.ToCardLog("OrderItems ");
                int termNum = GetTermNum();
                AlohaFuncs.DeselectAllEntries(termNum);
                foreach (int Od in OdList)
                {
                    AlohaFuncs.SelectEntryAndChildren(termNum, CheckId, Od);
                }
                AlohaFuncs.OrderItems(termNum, TableId, 1);
                AlohaFuncs.DeselectAllEntries(termNum);
                Utils.ToCardLog("OrderItems End");
                return true;
            }
            catch (Exception e)
            {
                Utils.ToCardLog("[Error] OrderItems " + e.Message);
                ExeptionMessage = e.Message;
                return false;
            }
        }

        static internal void OrderListDish(List<OrderDish> OdList)
        {
            try
            {

                AlohaFuncs.DeselectAllEntries(AlohaCurentState.TerminalId);
                foreach (OrderDish Od in OdList)
                {
                    AlohaFuncs.SelectEntryAndChildren(AlohaCurentState.TerminalId, Od.CheckId, Od.Id);
                }
                AlohaFuncs.OrderItems(AlohaCurentState.TerminalId, (int)AlohaCurentState.TableId, 1);
                AlohaFuncs.DeselectAllEntries(AlohaCurentState.TerminalId);
            }
            catch
            { }
        }

        static internal bool ImMaster()
        {
            MainClass.CurentMaster = DownTimeiniFile.GetMaster();

            if ((MainClass.CurentMaster == AlohaTSClass.GetTermNum()) || MainClass.IamIsMaster)
            {
                return true;
            }
            return false;



        }

        static internal int ApplyCompByCheckId(int CompTypeId, int WaiterId, int CheckId, out string ErMessage)
        {
            try
            {
                Utils.ToLog("[ApplyComp] Накладываю скидку: Терминал: " + GetTermNum().ToString() + " Официант: " + WaiterId.ToString() + " Чек: " +
                   CheckId.ToString() + " Тип: " + CompTypeId.ToString());

                int i = AlohaFuncs.ApplyComp(GetTermNum(), WaiterId, CheckId,
                  CompTypeId, 0, "", "");
                ErMessage = "";
                return i;
            }
            catch (Exception e)
            {
                Utils.ToLog("[ERROR] [ApplyComp] Не смог наложить скидку: Терминал: " + GetTermNum().ToString() + " Официант: " + WaiterId.ToString() + " Чек: " +
                   CheckId.ToString() + " Тип: " + CompTypeId.ToString() + " Причина: " + e.Message);
                ErMessage = e.Message;
                return 0;
            }

        }


        static internal void DeselectAllEntrysOnOtherChecsk(Check chk)
        {
            //Это чтобы снять выделение на других чеках этого стол


            foreach (Check Ch in chk.ChecksOnTable)
            {

                if (Ch.AlohaCheckNum == chk.AlohaCheckNum)
                {
                    continue;
                }
                Check Chk = GetCheckById(Ch.AlohaCheckNum);
                foreach (Dish d in Chk.Dishez)
                {
                    //if (d.Level > 0) continue;
                    try
                    {
                        Utils.ToCardLog("DeselectAllEntrysOnOtherChecsk " + d.AlohaNum);
                        if (d.Selected)
                        {
                            Utils.ToCardLog("DeselectAllEntrysOnOtherChecsk Selected " + d.AlohaNum);
                            AlohaFuncs.DeselectEntryAndChildren(GetTermNum(), Chk.AlohaCheckNum, d.AlohaNum);

                        }
                        else
                        {
                            foreach (Dish Md in d.CurentModificators)
                            {
                                if (Md.Selected)
                                {
                                    AlohaFuncs.DeselectEntryAndChildren(GetTermNum(), Chk.AlohaCheckNum, Md.AlohaNum);

                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Utils.ToCardLog("Error DeselectAllEntrysOnOtherChecsk " + d.AlohaNum);

                    }
                }
            }
        }

        static internal void AddRussMessage3(int NewDishID, string Mess)
        {
            //AlohaFuncs.ModItem(GetTermNum(), NewDishID, 999902, Mess, -999999999.000000, 0);
            AlohaFuncs.ModItem(GetTermNum(), NewDishID, 130006, Mess, -999999999.000000, 0);
        }


        static internal void AddRussMessage2(Dish D, Check Chk, List<string> Mess)
        {
            int MessId = 0;
            try
            {
                //AlohaFuncs.de (GetTermNum());
                //  DeselectAllEntrysOnOtherChecsk(Chk);
                AlohaFuncs.VoidItem(GetTermNum(), Chk.AlohaCheckNum, D.AlohaNum, 1);
                int NewDishID = D.AlohaNum;

                // if (D.QtyType)
                string UnitName = "";
                string WeightString = "";
                if (DishIsQty(D.BarCode, out UnitName) > 0)
                {
                    // NewDishID = AlohaFuncs.BeginItem(TermId, Ad.OwnerCheck.UnikNum, Ad.BarCode, "", Ad.QtyCount*Ad.QtyPrice);
                    NewDishID = AlohaFuncs.BeginPivotSeatItem(GetTermNum(), Chk.AlohaCheckNum, D.BarCode, "", (double)D.QtyQUANTITY * D.OPriceone, -1);
                    WeightString = D.QtyQUANTITY.ToString() + " " + UnitName;
                    AlohaFuncs.ModItem(GetTermNum(), NewDishID, 999902, WeightString, -999999999.000000, 0);

                }
                else
                {
                    //NewDishID = AlohaFuncs.BeginItem(TermId, Ad.OwnerCheck.UnikNum, Ad.BarCode, "", -999999999.000000);
                    if (Chk.Vozvr)
                    {
                        NewDishID = AlohaFuncs.BeginPivotSeatItem(GetTermNum(), Chk.AlohaCheckNum, D.BarCode, "", -(double)D.OPrice, -1);
                    }

                    else
                    {
                        NewDishID = AlohaFuncs.BeginPivotSeatItem(GetTermNum(), Chk.AlohaCheckNum, D.BarCode, "", (double)D.OPrice, -1);
                    }

                }

                List<string> SpecMsgs = new List<string>();

                try
                {
                    foreach (string ss in Mess)
                    {

                        AlohaFuncs.ModItem(GetTermNum(), NewDishID, 999902, ss, -999999999.000000, 0);
                    }
                }
                catch
                { }


                foreach (Dish Md in D.CurentModificators)
                {
                    if (Md.BarCode != -1)
                    {
                        if (Md.BarCode == 999902)
                        {
                            if (Md.Name == WeightString)
                            { continue; }
                            AlohaFuncs.ModItem(GetTermNum(), NewDishID, Md.BarCode, Md.Name, -999999999.000000, 0);
                        }
                        else
                        {
                            AlohaFuncs.ModItem(GetTermNum(), NewDishID, Md.BarCode, "", -999999999.000000, 0);
                        }
                    }
                    else
                    {
                        SpecMsgs.Add(Md.Name);
                    }
                }


                AlohaFuncs.EndItem(GetTermNum());
                AlohaFuncs.RefreshCheckDisplay();


            }
            catch (Exception e)
            {
                Utils.ToCardLog("[Error] AddRussMessage2 " + e.Message);
            }
        }

        /*
         static internal void AddRussMessage(Dish D, Check Chk, string Mess)
         {
             int MessId = 0;
             try
             {

                 //AlohaFuncs.VoidItem(GetTermNum (), Chk.AlohaCheckNum , D.AlohaNum, 1);
                 int NewDishID = D.AlohaNum;
                
                 List<string> SpecMsgs = new List<string>();
                
                 MessId = AlohaFuncs.ApplySpecialMessage(GetTermNum(), Chk.AlohaCheckNum, NewDishID, Mess);
                 //D.AlohaNum = NewDishID;
                 //   Ad.AlohaDishObj = Depot.FindObjectFromId(INTERNAL_ITEMS, NewDishID).First();

             }
             catch (Exception e)
             {
                 int i = 0;
             }
             //AlohaFuncs.ManagerVoidItem (
             AlohaFuncs.SelectEntry(GetTermNum(), Chk.AlohaCheckNum, MessId);
             RefreshCheckDisplay();
             AlohaFuncs.DeselectAllEntries(GetTermNum());
             //AlohaFuncs.SelectAllEntriesOnCheck(GetTermNum(), Chk.AlohaCheckNum);
             AlohaFuncs.SelectEntryAndChildren(GetTermNum(), Chk.AlohaCheckNum, D.AlohaNum);


             RefreshCheckDisplay();

         }

         */
        static internal int ApplyComp(int CompTypeId, out string ErMessage, double Val = 0)
        {

            return ApplyComp(CompTypeId, "", out ErMessage, Val);
        }



        static internal int ApplyComp(int CompTypeId, string CompName, out string ErMessage, double Val = 0)
        {
            try
            {
                Utils.ToLog("[ApplyComp] Накладываю скидку: Терминал: " + AlohaCurentState.TerminalId.ToString() + " Официант: " + AlohaCurentState.WaterId.ToString() + " Чек: " +
                   AlohaCurentState.CheckNum.ToString() + " Тип: " + CompTypeId.ToString());

                int i = AlohaFuncs.ApplyComp(AlohaCurentState.TerminalId, AlohaCurentState.WaterId, (int)AlohaCurentState.CheckId,
                  CompTypeId, Val, "", CompName);
                ErMessage = "";
                return i;
            }
            catch (Exception e)
            {
                Utils.ToLog("[ERROR] [ApplyComp] Не смог наложить скидку: Терминал: " + AlohaCurentState.TerminalId.ToString() + " Официант: " + AlohaCurentState.WaterId.ToString() + " Чек: " +
                   AlohaCurentState.CheckNum.ToString() + " Тип: " + CompTypeId.ToString() + " Причина: " + e.Message);
                ErMessage = e.Message;
                return 0;
            }
        }

        static internal int ApplyCompManagerOverride(int CompTypeId, out string ErMessage, double Val = 0)
        {
            try
            {
                Utils.ToLog("[ApplyComp] Накладываю скидку ManagerOverride: Терминал: " + AlohaCurentState.TerminalId.ToString() + " Официант: " + AlohaCurentState.WaterId.ToString() + " Чек: " +
                   AlohaCurentState.CheckNum.ToString() + " Тип: " + CompTypeId.ToString());
                int AuthorizeOverrideMgrRes = AlohaCurentState.WaterId;
                
                try
                {
                    int pass = Config.ConfigSettings.ManagerPass;
                    AuthorizeOverrideMgrRes = AlohaFuncs.AuthorizeOverrideMgr(AlohaCurentState.TerminalId, 99921, pass.ToString(), "");
                    Utils.ToLog($"AuthorizeOverrideMgrRes = {AuthorizeOverrideMgrRes}, pass={pass}" );
                }
                catch (Exception e)
                {
                    Utils.ToLog("Error [ApplyComp]  ManagerOverride " + e.Message);
                }


                //int i = AlohaFuncs.ApplyComp(AlohaCurentState.TerminalId, AlohaCurentState.WaterId, (int)AlohaCurentState.CheckId,CompTypeId, Val, "", "");
                int i = AlohaFuncs.ApplyComp(AlohaCurentState.TerminalId, AuthorizeOverrideMgrRes, (int)AlohaCurentState.CheckId, CompTypeId, Val, "", "");
                ErMessage = "";
                return i;
            }
            catch (Exception e)
            {
                Utils.ToLog("[ERROR] [ApplyComp] Не смог наложить скидку: Терминал: " + AlohaCurentState.TerminalId.ToString() + " Официант: " + AlohaCurentState.WaterId.ToString() + " Чек: " +
                   AlohaCurentState.CheckNum.ToString() + " Тип: " + CompTypeId.ToString() + " Причина: " + e.Message);
                ErMessage = e.Message;
                return 0;
            }
        }







        static internal int ApplyCardPayment(int CheckId, decimal Ammount)
        {
            try
            {
                MainClass.ComApplyPayment = true;

                int i = AlohaFuncs.ApplyPayment(GetTermNum(), CheckId, 20, (double)Ammount, 0, "", "", "", "");

                return i;
            }
            catch (Exception e)
            {
                string s = e.Message;
                return 0;
            }
        }

        static public int ApplySVCardPayment(int CheckId, decimal Ammount, string CardId, out string Error)
        {
            try
            {
                int PId = AlohaTender.CreditTenderIdIn;
                Error = "";
                Utils.ToCardLog(String.Format("Apply payment CardId {0}", CardId));
                MainClass.ComApplyPayment = true;

                int i = AlohaFuncs.ApplyPayment(GetTermNum(), CheckId, PId, (double)Ammount, 0, CardId, "", "", "");

                return i;
            }
            catch (Exception e)
            {
                Error = e.Message;
                Utils.ToCardLog("Error apply payment " + e.Message);
                string s = e.Message;
                return 0;
            }
        }

        static internal int ApplyPaymentAndClose(int CheckId, decimal Ammount, int PId)
        {
            try
            {
                MainClass.ComApplyPayment = true;
                Utils.ToLog("ApplyPaymentAndClose ApplyPayment PId = " + PId);
                int i = AlohaFuncs.ApplyPayment(GetTermNum(), CheckId, PId, (double)Ammount, 0, "", "", "", "");
            }
            catch (Exception e)
            {
                Utils.ToLog("ApplyPaymentAndClose ApplyPayment Error " + e.Message);
            }
            try
            {
                AlohaFuncs.CloseCheck(GetTermNum(), CheckId);
                return 1;
            }
            catch (Exception e)
            {
                Utils.ToLog("ApplyPaymentAndClose CloseCheck Error " + e.Message);
                return 0;
            }
        }
        static internal int GetTermNum()
        {
            try
            {
                if (Depot == null)
                {
                    Depot = new IberDepotClass();
                }
                IberEnumClass LocaleStateEnum = (IberEnumClass)Depot.GetEnum(INTERNAL_LOCALSTATE);
                IberObjectClass LocaleState = (IberObjectClass)LocaleStateEnum.First();
                return LocaleState.GetLongVal("TERMINAL_NUM");
            }
            catch
            {
                return 0;
            }
        }

        static public long GetCurentCheckId()
        {
            try
            {
                IberEnumClass LocaleStateEnum = (IberEnumClass)Depot.GetEnum(INTERNAL_LOCALSTATE);
                IberObjectClass LocaleState = (IberObjectClass)LocaleStateEnum.First();
                IberEnumClass CurStateEnum = (IberEnumClass)LocaleState.GetEnum(INTERNAL_LOCALSTATE_CUR_CHECK);
                IberObjectClass CurLocaleState = (IberObjectClass)CurStateEnum.First();
                return CurLocaleState.GetLongVal("ID");

            }
            catch (Exception e)
            {
                Utils.ToLog(e.Message);
                return -1;
            }
        }

        static internal double GetCurentCheckSumm()
        {
            try
            {
                IberEnumClass LocaleStateEnum = (IberEnumClass)Depot.GetEnum(INTERNAL_LOCALSTATE);
                IberObjectClass LocaleState = (IberObjectClass)LocaleStateEnum.First();
                IberEnumClass CurStateEnum = (IberEnumClass)LocaleState.GetEnum(INTERNAL_LOCALSTATE_CUR_CHECK);
                IberObjectClass CurLocaleState = (IberObjectClass)CurStateEnum.First();
                ulong CheckId = (ulong)CurLocaleState.GetLongVal("ID");
                return GetCheckSum((int)CheckId);
            }
            catch (Exception e)
            {
                Utils.ToLog(e.Message);
                return -1;
            }
        }


        private static IberObject GetObjectById(int Id, IberEnum Enum)
        {
            foreach (IberObject ob in Enum)
            {
                if (ob.GetLongVal("ID") == Id)
                {
                    return ob;
                }
            }
            return null;
        }


        public static void PrintSystemaInfo(string info)
        {
            Utils.ToCardLog("PrintSystemaInfo");
            try
            {
                string s = "";
                s += "<PRINTLINE>" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "</PRINTLINE>";
                s += "<PRINTLINE>" + AlohainiFile.UNITNAME + "</PRINTLINE>";
                s += "<PRINTLINE>" + AlohainiFile.ADDRESS1 + "</PRINTLINE>";
                s += "<PRINTFILLED>*</PRINTFILLED>";
                s += "<LINEFEED>3</LINEFEED>";
                //s += "<PRINTLINE>Отчет по стоп-листу</PRINTLINE>";
                s += "<LINEFEED>3</LINEFEED>";
                foreach (string ss in info.Split(Environment.NewLine.ToCharArray()))
                {
                    s += "<PRINTLINE>" + ss + "</PRINTLINE>";
                    //s += "<PRINTLEFTRIGHT><LEFT>" + ss + "</LEFT>";
                    //  s += "<RIGHT>" + ss.Count + "</RIGHT></PRINTLEFTRIGHT>";

                }

                s += "<LINEFEED>3</LINEFEED>";
                s += "<PRINTFILLED>*</PRINTFILLED>";



                PrintINfo(s);
                /*
            string z = Ap.GetAllPrinters();
            Ap.PrintStream(s);
                 * */
            }
            catch (Exception e)
            {
                Utils.ToCardLog("PrintSystemaInfo Error " + e.Message);
            }

        }

        public static string GetSistemaEatingsInfo(int CardId, string CardName)
        {
            string Res = "Карта: " + CardName + "." + Environment.NewLine + Environment.NewLine;


            IberObject Itm = Depot.FindObjectFromId(INTERNAL_ITEMS, CardId + 999700).First();

            string BtnName = Itm.GetStringVal("SHORTNAME");

            Res += "Кнопка: " + BtnName + Environment.NewLine + Environment.NewLine;

            Res += "Предложите гостю следующий выбор:" + Environment.NewLine + Environment.NewLine;
            for (int ModNum = 1; ModNum < 10; ModNum++)
            {
                try
                {
                    int ModGroupeNum = Itm.GetLongVal("MOD" + ModNum.ToString());
                    if (ModGroupeNum > 0)
                    {
                        IberObject ModGroupe = GetObjectById(ModGroupeNum, Depot.GetEnum(FILE_MOD));
                        string ModGroupeName = ModGroupe.GetStringVal("LONGNAME");
                        Res += "Группа " + ModGroupeName + ":" + Environment.NewLine;
                        for (int ItmNum = 1; ItmNum < 55; ItmNum++)
                        {
                            /*
                            string DopZero = "";
                            if (ItmNum < 10)
                            {
                                DopZero = "0";
                            }
                            */
                            int ModItmNum = ModGroupe.GetLongVal("ITEM" + ItmNum.ToString("00"));
                            if (ModItmNum > 0)
                            {
                                IberObject ModItm = Depot.FindObjectFromId(INTERNAL_ITEMS, ModItmNum).First();
                                string ModItmName = ModItm.GetStringVal("LONGNAME");
                                Res += "       " + ModItmName + Environment.NewLine;
                            }
                        }
                    }

                }
                catch (Exception e)
                {
                    Utils.ToLog("GetSistemaEatingsInfo Error " + e.Message);

                }

            }

            return Res;

        }

        static public int GetCatByItem(int item)
        {
            IberObject itm = Depot.FindObjectFromId(INTERNAL_ITEMS, item).First();
            int cat_id = itm.GetLongVal("CAT_ID");
            return cat_id;
        }

        static internal bool SelectedDishOnOtherTable(Check Ch)
        {
            foreach (Check Chk in Ch.ChecksOnTable)
            {

                if (Ch.AlohaCheckNum == Chk.AlohaCheckNum)
                {
                    continue;
                }
                Check mChk = GetCheckById(Chk.AlohaCheckNum);

                foreach (Dish d in mChk.Dishez)
                {
                    //if (d.Level > 0) continue;
                    if (d.Level > 0)
                    {
                        continue;
                    }
                    if (d.Selected)
                    {
                        return true;
                    }
                    else
                    {
                        foreach (Dish Md in d.CurentModificators)
                        {
                            if (Md.BarCode == -1)
                            {
                                continue;
                            }
                            if (Md.Selected)
                            {
                                return true;
                            }
                        }
                    }
                }


            }
            return false;
        }
        static internal Dish GetSelectedDish(out Check Ch, out int SelCount)
        {

            SelCount = 0;
            Ch = null;
            CheckWindow();
            Ch = GetCheckById((int)AlohaCurentState.CheckId);
            Dish tmp = null;
            foreach (Dish d in Ch.Dishez)
            {
                //if (d.Level > 0) continue;
                if (d.Level > 0)
                {
                    continue;
                }
                if (d.Selected)
                {
                    SelCount++;
                    tmp = d;
                }
                else
                {
                    foreach (Dish Md in d.CurentModificators)
                    {
                        if (Md.BarCode == -1)
                        {
                            continue;
                        }
                        if (Md.Selected)
                        {
                            SelCount++;
                            tmp = Md;
                        }
                    }
                }
            }





            //Dish tmp = GetDishById(Ch, AlohaCurentState.CurrentSelectionEntrieId);
            return tmp;
        }
        static internal Dish GetDishById(Check Ch, int id)
        {
            foreach (Dish D in Ch.Dishez)
            {
                if (D.AlohaNum == id)
                {
                    return D;
                }

            }
            return null;
        }

        static internal List<int> GetCurrentItemsPerebor()
        {
            Utils.ToCardLog("CheckWindow GetCurrentItems ");
            List<int> Itms = new List<int>();
            try
            {

                IberEnumClass LocaleStateEnum = (IberEnumClass)Depot.GetEnum(INTERNAL_LOCALSTATE);

                IberObjectClass LocaleState = (IberObjectClass)LocaleStateEnum.First();

                IberEnumClass ItemInfosEnum = (IberEnumClass)LocaleState.GetEnum(INTERNAL_LOCALSTATE_ITEMINFOS);

                string s = "";
                foreach (IberObject ItInf in ItemInfosEnum)
                {
                    try
                    {
                        string literals = " abcdefghijklmnopqrstuvwxyz";
                        for (int fLeter = 1; fLeter < 27; fLeter++)
                        {
                            for (int sLeter = 1; sLeter < 27; sLeter++)
                            {
                                string ss = new StringBuilder().Append(literals[fLeter]).Append(literals[sLeter]).ToString();
                                Utils.ToLog("GetCurrentItemsPerebor " + ss);

                                for (int Leter3 = 0; Leter3 < 27; Leter3++)
                                {
                                    for (int Leter4 = 0; Leter4 < 27; Leter4++)
                                    {
                                        try
                                        {
                                            string sss2 = new StringBuilder().Append(literals[fLeter]).Append(literals[sLeter]).Append(literals[Leter3]).Append(literals[Leter4]).ToString();
                                            string val2 = ItInf.GetStringVal(sss2);
                                            Utils.ToLog("GetCurrentItemsPerebor Okkkkkkkkkk " + sss2 + " val: " + val2);
                                        }
                                        catch
                                        { }
                                    }
                                }


                            }
                        }
                        int BarCode = ItInf.GetLongVal("DATA");
                        Itms.Add(BarCode);
                        Utils.ToCardLog(String.Format("Order BarCode {0}, Check {1}", BarCode, 0));
                    }
                    catch (Exception e)
                    {
                        Utils.ToCardLog("Error ItInf " + e.Message);
                    }

                }


            }
            catch (Exception e)
            {
                Utils.ToCardLog("Error GetCurrentItems " + e.Message);

            }
            return Itms;
        }


        static internal List<int> GetCurrentItems(int TableId)
        {
            Utils.ToCardLog("CheckWindow GetCurrentItems ");
            List<int> Itms = new List<int>();
            try
            {

                IberEnumClass LocaleStateEnum = (IberEnumClass)Depot.GetEnum(INTERNAL_LOCALSTATE);

                IberObject LocaleState = null; //= (IberObjectClass)LocaleStateEnum.First();


                foreach (IberObject LocaleStateTmp in LocaleStateEnum)
                {
                    if (LocaleStateTmp.GetLongVal("CURRENT_TABLE_ID") == TableId)
                    //  if (LocaleStateTmp.GetLongVal("CURRENT_CHECK_ID") == TableId)
                    {
                        LocaleState = LocaleStateTmp;
                        break;
                    }
                }
                if (LocaleState != null)
                {
                    IberEnumClass ItemInfosEnum = (IberEnumClass)LocaleState.GetEnum(INTERNAL_LOCALSTATE_ITEMINFOS);

                    string s = "";
                    foreach (IberObject ItInf in ItemInfosEnum)
                    {
                        try
                        {
                            int BarCode = ItInf.GetLongVal("data");
                            Itms.Add(BarCode);
                            Utils.ToCardLog(String.Format("Order BarCode {0}, TableId {1}", BarCode, TableId));
                        }
                        catch (Exception e)
                        {
                            Utils.ToCardLog("Error ItInf " + e.Message);
                        }

                    }
                }
                else
                {
                    Utils.ToCardLog("Error GetCurrentItems LocaleState for CheckID " + TableId + " non find");
                }

            }
            catch (Exception e)
            {
                Utils.ToCardLog("Error GetCurrentItems " + e.Message);

            }
            return Itms;
        }


        static internal bool GetCurentStateFromArgs = false;

        static internal bool CheckWindow()
        {

            if (!GetCurentStateFromArgs)
            {
                try
                {
                    //Utils.ToCardLog("Получил CurStateEnum");
                    IberEnumClass LocaleStateEnum = (IberEnumClass)Depot.GetEnum(INTERNAL_LOCALSTATE);
                    // Utils.ToCardLog("Получил LocaleStateEnum ");
                    IberObjectClass LocaleState = (IberObjectClass)LocaleStateEnum.First();
                    //Utils.ToCardLog("Получил LocaleState");
                    AlohaCurentState.TerminalId = LocaleState.GetLongVal("TERMINAL_NUM");
                    //Utils.ToCardLog("Получил TerminalId");
                    AlohaCurentState.TableId = (ulong)LocaleState.GetLongVal("CURRENT_TABLE_ID");
                    //Utils.ToCardLog("Получил TableId ");

                    IberEnumClass CurStateEnum = (IberEnumClass)LocaleState.GetEnum(INTERNAL_LOCALSTATE_CUR_EMP);
                    //Utils.ToCardLog("Получил CurStateEnum ");
                    IberObjectClass CurLocaleState = (IberObjectClass)CurStateEnum.First();
                    //Utils.ToCardLog("Получил CurLocaleState ");
                    AlohaCurentState.WaterId = CurLocaleState.GetLongVal("ID");
                    //Utils.ToCardLog("Получил WaterId");
                    AlohaCurentState.EmployeePositionCode = CurLocaleState.GetLongVal("JOBCODE1");
                    //Utils.ToCardLog("Получил EmployeePositionCode ");
                    AlohaCurentState.EmployeeNumberCode = CurLocaleState.GetLongVal("USERNUMBER");
                    //Utils.ToCardLog("Получил EmployeeNumberCode ");


                    CurStateEnum = (IberEnumClass)LocaleState.GetEnum(INTERNAL_LOCALSTATE_CUR_CHECK);
                    //Utils.ToCardLog("Получил CurStateEnum ");
                    CurLocaleState = (IberObjectClass)CurStateEnum.First();
                    //Utils.ToCardLog("Получил CurLocaleState ");
                    AlohaCurentState.CheckId = (ulong)CurLocaleState.GetLongVal("ID");
                    Utils.ToCardLog("Получил CheckId =" + AlohaCurentState.CheckId);
                    /*
                    CurStateEnum = (IberEnumClass)LocaleState.GetEnum(INTERNAL_LOCALSTATE_CUR_TABLE);
                    CurLocaleState = (IberObjectClass)CurStateEnum.First();
                    AlohaCurentState.TableId = (ulong)CurLocaleState.GetLongVal("ID");
                    */
                    AlohaCurentState.PredCheckAndNotManager = (CurLocaleState.GetLongVal("PRINTED") > 0 && AlohaCurentState.EmployeePositionCode != 10);
                    //Utils.ToCardLog("Получил PredCheckAndNotManager ");

                    try
                    {
                        CurStateEnum = (IberEnumClass)CurLocaleState.GetEnum(INTERNAL_CHECKS_COMPS);
                        //  Utils.ToCardLog("Получил CurStateEnum");
                        CurLocaleState = (IberObjectClass)CurStateEnum.First();
                        //Utils.ToCardLog("Получил CurStateEnum.First");
                        AlohaCurentState.CompIsAppled = true;
                        AlohaCurentState.CompSumm = (decimal)CurLocaleState.GetDoubleVal("AMOUNT");
                        AlohaCurentState.CompId = CurLocaleState.GetLongVal("COMPTYPE_ID");


                    }
                    catch (System.Runtime.InteropServices.COMException)
                    {
                        //Utils.ToCardLog("Error CurStateEnum");
                        AlohaCurentState.CompIsAppled = false;

                    }

                    /*
                    try
                    {
                        IberEnumClass SelEntrEnum = (IberEnumClass)LocaleState.GetEnum(INTERNAL_LOCALSTATE_CUR_ENTRY);
                        AlohaCurentState.CurrentSelectionCount = SelEntrEnum.Count;
                        AlohaCurentState.CurrentSelectionEntrieId = SelEntrEnum.First().GetLongVal("ID");
 


                    }
                    catch(Exception e)
                    {
                        ShowMessage(e.Message); 
                    }
                    */
                    return true;

                }
                catch (Exception e)
                {
                    Utils.ToCardLog("Error CheckWindow " + e.Message);
                    return false;
                }
            }
            else
            {
                try
                {
                    AlohaCurentState.TerminalId = InternalConnections.TermId;
                    AlohaCurentState.TableId = (ulong)InternalConnections.TableId;
                    AlohaCurentState.EmployeeNumberCode = InternalConnections.EmplId;
                    AlohaCurentState.CheckId = (ulong)InternalConnections.CheckId;
                    //Utils.ToLog("Ищу чек номер " + AlohaCurentState.CheckId);
                    IberEnumClass ChEnum = (IberEnumClass)Depot.FindObjectFromId(INTERNAL_CHECKS, (int)AlohaCurentState.CheckId);

                    IberObjectClass Ch = (IberObjectClass)ChEnum.First();
                    //Utils.ToLog("Нашел");
                    try
                    {
                        IberEnumClass CurStateEnum = (IberEnumClass)Ch.GetEnum(INTERNAL_CHECKS_COMPS);
                        IberObjectClass CurLocaleState = (IberObjectClass)CurStateEnum.First();
                        AlohaCurentState.CompIsAppled = true;
                        AlohaCurentState.CompSumm = (decimal)CurLocaleState.GetDoubleVal("AMOUNT");

                    }
                    catch (System.Runtime.InteropServices.COMException)
                    {
                        AlohaCurentState.CompIsAppled = false;
                    }
                    return true;
                }
                catch (Exception ee)
                {
                    ShowMessage(ee.Message);
                    return false;
                }
            }

        }

        // все что далее - удалить


        /*
        internal static Dictionary<int, int> TablesEmployez = new Dictionary<int, int>();
        internal static Dictionary<int, int> EnTableList = new Dictionary<int, int>();
        internal static Dictionary<int, string> EmployeeList = new Dictionary<int, string>();

        internal static void SInit()
        {
            EnTableList = GetTables();


            EmployeeList = GetEmployees();
        }


        internal static Dictionary<int, int> GetTables()
        {
            Dictionary<int, int> mTables = new Dictionary<int, int>();
            IberEnum TableEnum = new IberEnum();
            TableEnum = Depot.GetEnum(30);
            int i = 1;

            while (mTables.Count < 5)
            {

                foreach (IberObject Table in TableEnum)
                {
                    mTables.Add(i, Table.GetLongVal("ID"));
                    i++;
                }
            }
            return mTables;
        }





        internal static void ReadTablesEmployez()
        {
            Dictionary<int, int> _TablesEmployez = new Dictionary<int, int>();
            foreach (int TableNumber in EnTableList.Values)
            {
                _TablesEmployez.Add(TableNumber, -1);

            }

            IberEnum EmployeeEnum = new IberEnum();

            EmployeeEnum = Depot.GetEnum(500);

            IberEnum TableEnum = new IberEnum();

            foreach (IberObject Employee in EmployeeEnum)
            {
                //System.Windows.Forms.MessageBox.Show(IO.GetLongVal("ID").ToString()  );
                try
                {
                    TableEnum = Employee.GetEnum(501);

                    foreach (IberObject Table in TableEnum)
                    {
                        int TN = Table.GetLongVal("TABLEDEF_ID");
                        int val = 0;


                        if (_TablesEmployez.TryGetValue(TN, out val))
                        {
                            _TablesEmployez.Remove(TN);

                        }
                        _TablesEmployez.Add(TN, Employee.GetLongVal("ID"));


                        //return Employee.GetLongVal("ID");

                    }
                }
                catch
                {

                }
            }
            lock (TablesEmployez)
            {
                TablesEmployez.Clear();
                foreach (int k in _TablesEmployez.Keys)
                {
                    int i = 0;
                    _TablesEmployez.TryGetValue(k, out i);
                    TablesEmployez.Add(k, i);
                }
            }
        }

*/



    }

    public class ToGoOrderInfo
    {
        public ToGoOrderInfo() { }
        public int id { set; get; }
        public int TableNum { set; get; }
        public int numInTable { set; get; }
        public decimal Summ { set; get; }
    }



    public class ModificatorGroupeCashe
    {
        public String Name { set; get; }
        public List<StopListService.AlohaMod> Mods = new List<StopListService.AlohaMod>();

    }

    public class CModificatorsDishCashe
    {
        public Dictionary<int, ModificatorGroupeCashe> ModsCashe = new Dictionary<int, ModificatorGroupeCashe>();
        public Dictionary<int, StopListService.AlohaMod> ItemsCashe = new Dictionary<int, StopListService.AlohaMod>();
    }
}
