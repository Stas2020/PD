using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AlohaFOHLib;
using Interop.INTERCEPTACTIVITYLib;
using System.Runtime.InteropServices ;
using System.Threading;

using System.Diagnostics;
using System.ComponentModel;
using System.IO;
using PDiscountCard.MB;

namespace PDiscountCard
{

    [ComVisible (true)]
    public class AlohaActiviti : IInterceptAlohaActivity5
    {

        public AlohaActiviti()
        {
            Utils.ToLog("[AlohaActiviti] Инициализация.");
         //   AppDomain.CurrentDomain.SetupInformation.PrivateBinPath = @"..\check\trposx\";
        }

        #region IInterceptAlohaActivity5 Members

        public void AcceptTable(int EmployeeId, int FromTableId, int ToTableId)
        {
            throw new NotImplementedException();
        }

        public void AddItem(int EmployeeId, int QueueId, int TableId, int CheckId, int EntryId)
        {
            //throw new NotImplementedException();
            AlohaEventVoids.AddItem(EmployeeId, QueueId, TableId, CheckId, EntryId);

            
            

            /*
            if (iniFile.FCCEnable)
            {
                FCC.UpdateChk(AlohaTSClass.GetCheckById(CheckId));
            }
            */
        }

        public void AddTab(int EmployeeId, int FromTableId, int ToTableId)
        {
            throw new NotImplementedException();
        }

        public void AdjustPayment(int ManagerId, int EmployeeId, int QueueId, int TableId, int CheckId, int TenderId, int PaymentId)
        {
            throw new NotImplementedException();
        }

        public void AdvanceOrder(int EmployeeId, int QueueId, int TableId)
        {
            throw new NotImplementedException();
        }

        public void ApplyComp(int ManagerId, int EmployeeId, int QueueId, int TableId, int CheckId, int CompTypeId, int CompId)
        {
            try
            {

                AlohaTSClass.WriteToDebout("Событие Применена скидка тип: " + CompTypeId + ", чек: " + (new Check(CheckId)).CheckNum + " Официант: " + EmployeeId);

            }
            catch
            { }
            if (iniFile.CustomerDisplayEnabled)
            {
                RemoteCommands.CustomerDisplayEventSender.AddSendChkToCustDispToQueue(CheckId, true);
            }

            SendToVideo.ApplyComp(CheckId, CompTypeId);
        }

        public  void ApplyPayment(int ManagerId, int EmployeeId, int QueueId, int TableId, int CheckId, int TenderId, int PaymentId)
        {
            if (iniFile.CustomerDisplayEnabled)
            {
                RemoteCommands.CustomerDisplayEventSender.AddSendChkToCustDispToQueue(CheckId, true);
            }
            try
            {
                //DisplayBoardClass.ApplyPaymentEvent(CheckId);
                DisplayBoardClass.ApplyPaymentEvent();
                SendToVideo.ApplyPayment(CheckId, TenderId);
                AlohaEventVoids.ApplyCardPayment(ManagerId, EmployeeId, QueueId, TableId, CheckId, TenderId, PaymentId);
              
            }
            catch(Exception e)
            {
                string Mess = "Ошибка программы " + e.Message + Environment.NewLine + "Призведите оплату вручную";
                AlohaTSClass.ShowMessage(Mess);
                Utils.ToCardLog(Mess);
                MainClass.PlasticTransactionInProcess = false;
            }
        }

        public void ApplyPromo(int ManagerId, int EmployeeId, int QueueId, int TableId, int CheckId, int PromotionId, int PromoId)
        {
            if (iniFile.CustomerDisplayEnabled)
            {
                RemoteCommands.CustomerDisplayEventSender.AddSendChkToCustDispToQueue(CheckId, true);
            }
            
        }

        public void Bump(int TableId)
        {
            throw new NotImplementedException();
        }

        public void CancelAddItem(int EmployeeId, int QueueId, int TableId, int CheckId, int EntryId)
        {
            throw new NotImplementedException();
        }

        public void CarryoverId(int Type, int OldId, int NewId)
        {
            throw new NotImplementedException();
        }

        public void ChangeItemSize(int EmployeeId, int QueueId, int TableId, int CheckId, int EntryId)
        {
            throw new NotImplementedException();
        }

        public void ClockIn(int EmployeeId, string EmpName, int JobcodeId, string JobName)
        {
            throw new NotImplementedException();
        }

        public void ClockOut(int EmployeeId, string EmpName)
        {
            throw new NotImplementedException();
        }

        public void CloseCheck(int EmployeeId, int QueueId, int TableId, int CheckId)
        {
            if (QueueId == 5)
            {
                Utils.ToLog($"CheckId ={CheckId} Чек из очереди №5. Это иp хаба Закрываем без Джестори. В Джестори отправит хаб", 2); 
                return;
            }

            if (iniFile.CustomerDisplayEnabled)
            {
                RemoteCommands.CustomerDisplayEventSender.AddSendCloseChkToCustDisp(CheckId);
            }

            EventSenderClass.SendAlohaAsincEvent(PDiscountCard.StopListService.AlohaEventType.CloseCheck,"",0,EmployeeId ,"",0,TableId ,CheckId);
            AlohaEventVoids.CloseCheck(EmployeeId, QueueId, TableId, CheckId);
            SendToVideo.CloseCheck(CheckId);

            //  AlohaTSClass.CloseTable(AlohaTSClass.GetTermNum(), TableId);
             
        }
        public void CloseTable(int EmployeeId, int QueueId, int TableId)
        {
            try
            {
                Utils.ToLog("[CloseTable] Закрыл стол " + TableId.ToString());
            }
            catch
            { }
        }

        public void CombineOrder(int EmployeeId, int SrcQueueId, int SrcTableId, int SrcCheckId, int DstQueueId, int DstTableId, int DstCheckId)
        {
            throw new NotImplementedException();
        }

        public void Custom(string Name)
        {

            try
            {
                Utils.ToLog("Custom " + Name);
                if (Name == "DeleteItemsAndCloseCheck")
                {
                    Utils.ToCardLog("DeleteItemsAndCloseCheck");
                    AlohaEventVoids.DeleteItemsAndCloseCheck ();
                }
                if (Name == "PrintCheck")
              //  else if (Name == "PlasticCopySlip")
                {

                    if (iniFile.PrintPrecheckOnFR)
                    {
                        AlohaEventVoids.PrintCurentPrecheckOnFR();
                    }
                    else
                    {
                        AlohaTSClass.PrintCurentPredcheck();
                    }
                    return;
                    //AlohaEventVoids.CloseCheck();
                }
                if (Name == "CloseCheck")
                {
                    AlohaEventVoids.CloseCheck();
                }
                if (Name == "XReport")
                {
                    Utils.ToCardLog("Custom XReport");
                    AlohaEventVoids.XReport();
                }
                if (Name == "IMReport")
                {
                    //Utils.ToCardLog("Custom XReport");
                    AlohaEventVoids.IMReport();
                }

                if (Name == "XReportHamster")
                {
                    AlohaEventVoids.XReportHamster();
                }
                if (Name == "ZReport")
                {
                    AlohaEventVoids.ZReport();
                }
                if (Name == "OrderItems")
                {
                    AlohaEventVoids.OrderItems();
                }
                if (Name == "ApplyPayment5000")
                {
                    AlohaEventVoids.ApplyPayment(5000);
                }
                if (Name == "ShowReportSale")
                {
                    AlohaEventVoids.ShowReportSale();
                }
                if (Name == "ShowTotalSumm")
                {
                    AlohaEventVoids.ShowTotalSumm();
                    //AlohaTSClass.GetSelectedItems();
                    //OrderDivider.OrderItems(); 
                    //OrderDivider.HideWindow();
                }
                if (Name == "OrderItemsWithDivide")
                {   
                    OrderDivider.OrderItems(false ); 
                }
                if (Name == "OrderAllItemsWithDivide")
                {
                    OrderDivider.OrderItems(true);
                }


                if (Name == "AddWaiterToCheck")
                {
                    AlohaEventVoids.AddWaiterToCheck();
                }

                if (Name == "Plastik")
                {
                    if (iniFile.ArcusEnabled)
                    {
                        ArcusAlohaIntegrator.XReport();
                    }
                    else if (iniFile.TRPOSXEnables)
                    {
                        TrPosXAlohaIntegrator.FoolReport();
                    }
                }
                if (Name == "PlastikXReport")
                {
                    if (iniFile.ArcusEnabled)
                    {
                        ArcusAlohaIntegrator.PrintShortReport();
                        //AlohaEventVoids.TestPrintWithPause(); 

                    }
                    else if (iniFile.TRPOSXEnables)
                    {
                        TrPosXAlohaIntegrator.XReport();
                    }
                    else
                    {
                        AlohaTSClass.ShowMessage("Нет подключенных безналичных терминалов. TRPOSXEnables=0 и ArcusEnabled=0");
                    }
                }
                else if (Name == "PlasticSverka")
                {

                    if (iniFile.ArcusEnabled)
                    {
                        ArcusAlohaIntegrator.SverkaWithQ();
                    }
                    else
                    {
                        TrPosXAlohaIntegrator.SverkaWithQ();
                    }


                }
                else if (Name == "PlasticCopySlip")
                {
                    if (iniFile.ArcusEnabled)
                    {
                        ArcusAlohaIntegrator.GetSipCopy();
                    }
                    else if (iniFile.TRPOSXEnables)
                    {
                        TrPosXAlohaIntegrator.GetSlipCopy();
                    }
                    else
                    {
                        AlohaTSClass.ShowMessage("Нет подключенных безналичных терминалов. TRPOSXEnables=0 и ArcusEnabled=0 ");
                    }
                }

                else if (Name == "ShowStopList")
                {
                  //  AlohaEventVoids.ShowStopListReason();
                    AlohaEventVoids.ShowStopList();
                 
                }
                else if (Name == "ShowStopListReason")
                {
                    AlohaEventVoids.ShowStopListReason();
                }
                else if (Name == "Degustations")
                {
                    AlohaEventVoids.Degustations ();
                }
                if (Name == "Payment")
                {
                    DisplayBoardClass.ApplyPaymentEvent();
                }
                if (Name == "ShowfrmCard")
                {
                    AlohaEventVoids.ShowfrmCard();
                }
                if (Name == "ShowfrmModifItem")
                {
                    AlohaEventVoids.ShowfrmModifItem();
                }
                if (Name.Length > 5)
                {
                    if (Name.Substring(0, 5) == "Scale")
                    {
                        AlohaEventVoids.AddScaleDish2(Name);
                    }
                }
                if (Name == "VIP")
                {
                    AlohaEventVoids.SetVip();
                }
                if (Name == "FCCShowAdmin")
                {
                    FCC.ShowAdmin();
                }
                if (Name == "FCCSetBill")
                {
                    FCC.SetBill();
                }
                if (Name == "FCCSetBillWithHands")
                {
                    //FCC.SetBillWithHands();
                }
                if (Name == "FCCShowCassir")
                {
                    FCC.ShowCassirFrm();
                }
                if (Name == "FCCInspectSmallChange")
                {
                    FCC.InpectSmallChange(true );
                }
                
                
                if (Name == "ShowCashIncome")
                {
                    AlohaEventVoids.ShowFrmCashIn();
                    

                }
                if (Name == "FCCShowRazmen")
                {
                    FCC.ShowRazmen();
                   // DualConnector.DualConnectorMain.InFuncsfrm();
               }

                if (Name == "CloseByWaiter")
                {
                    AlohaEventVoids.CloseByWaiter();
                }
                if (Name == "CloseByWaiterCard")
                {
                    AlohaEventVoids.CloseByWaiter(2);
                }
                if (Name == "CloseByWaiterGlory")
                {
                    if (!AlohaTSClass.IsAlohaTS() && !iniFile.FCCEnable)
                    {
                        Utils.ToLog("CloseByWaiterGlory && (!AlohaTSClass.IsAlohaTS() && !iniFile.FCCEnable)");
                        AlohaEventVoids.CloseCheck();
                    }
                    else
                    {
                        AlohaEventVoids.CloseByWaiter(1);
                    }


                }
                /*

                if (Name == "InPasShortReport")
                {
                    DualConnector.DualConnectorMain.ShortReport();
                }
                if (Name == "InPasFullReport")
                {
                    DualConnector.DualConnectorMain.LongReport();
                }
                if (Name == "InPasSverka")
                {
                    DualConnector.DualConnectorMain.Sverka();
                }
              
                if (Name == "InPasLastChk")
                {
                    DualConnector.DualConnectorMain.GetCopyLastSlip();
                }

                if (Name == "InPasAnyChk")
                {
                    DualConnector.DualConnectorMain.GetCopySlip();
                }
                 * */
                if ((Name == "InPasFuncsfrm") || ((Name == "PlasticFuncsfrm")))
                {
                    /*
                    if (iniFile.InPasEnabled)
                    {
                        DualConnector.DualConnectorMain.InPasFuncsfrm();
                    }
                    else
                     * */
                    {
                        CreditCardAlohaIntegration.ShowFuncsfrm();
                    }
                    
                }
                 
                if (Name == "WestReport0")
                {
                    West.WestMain.ShowSaleReport(0);
                }
                if (Name == "WestReport1")
                {
                    West.WestMain.ShowSaleReport(1);
                }
                if (Name == "WestPager")
                {
                    West.WestMain.mShowPagerDialog ();
                }
                if (Name == "BonusCardReport")
                {
                    Loyalty.LoyaltyBasik.PrintLongSlipReport();
                }

                if (Name == "EGAISScan")
                {
                    EGAIS.EGAISCodeReader.Read();
                }
                if (Name == "ReprintCheck")
                {
                    FRSClientApp.FRSClient.PrintFCheckShowWnd();
                }
                if (Name == "FayRetailCard")
                {
                    FayRetail.FayRetailMain.ShowWndApplyCardWithCurentCheck();
                }
                if (Name == "PBFirstRequest")
                {
                    MB.PB.SendCurentChk();
                }

            }
            catch(Exception e)
            {
                Utils.ToCardLog("[Error] Custom " + Name + ", Mess: " + e.Message); 
            }
                /*
            else if (Name == "PlastikVozvrat")
            {
                TrPosXClass.Void(100) ;
            }
                 * */

        }

        public void DeleteComp(int ManagerId, int EmployeeId, int QueueId, int TableId, int CheckId, int CompTypeId, int CompId)
        {
            SendToVideo.DeleteComp(CheckId, CompTypeId);
            if (iniFile.CustomerDisplayEnabled)
            {
                RemoteCommands.CustomerDisplayEventSender.AddSendChkToCustDispToQueue(CheckId, true);
            }
        }

        public void DeleteItems(int ManagerId, int EmployeeId, int QueueId, int TableId, int CheckId, int ReasonId)
        {
            SendToVideo.DeleteItem(CheckId);
            if (iniFile.CustomerDisplayEnabled)
            {
                RemoteCommands.CustomerDisplayEventSender.AddSendChkToCustDispToQueue(CheckId, false);
            }
            
        }

        public void DeletePayment(int ManagerId, int EmployeeId, int QueueId, int TableId, int CheckId, int TenderId, int PaymentId)
        {
            if (iniFile.CustomerDisplayEnabled)
            {
                RemoteCommands.CustomerDisplayEventSender.AddSendChkToCustDispToQueue(CheckId, true );
            }
        }

        public void DeletePromo(int ManagerId, int EmployeeId, int QueueId, int TableId, int CheckId, int PromotionId, int PromoId)
        {
            if (iniFile.CustomerDisplayEnabled)
            {
                RemoteCommands.CustomerDisplayEventSender.AddSendChkToCustDispToQueue(CheckId, false);
            }
        }

        public void EndOfDay(int IsMaster)
        {
            AlohaEventVoids.EOD();
            SendToVideo.EOD();
            try
            {
                AlohaTSClass.ShowMessageInternal("Засыпаю");
                Thread.Sleep(30000);
                AlohaTSClass.ShowMessageInternal("Просыпаюсь");
            }
            catch
            { }
        }

        

        public void EnrollEmployee(int EmployeeId, int numTries)
        {
            throw new NotImplementedException();
        }

        public void EventNotification(int EmployeeId, int TableId, ALOHA_ACTIVITY_EVENT_NOTIFICATION_TYPES EventNotification)
        {
            
            if (EventNotification == ALOHA_ACTIVITY_EVENT_NOTIFICATION_TYPES.ALOHAACTIVITY_EVENT_ENTER_CLOSE_SCREEN)
            {
                DisplayBoardClass.EventNotification_ENTER_CLOSE_SCREEN ()       ;
                if (iniFile.CustomerDisplayEnabled)
                {
                    RemoteCommands.CustomerDisplayEventSender.AddSendCurentChkToCustDispToQueue(true);
                }
               
            }

            //throw new NotImplementedException();
        }

        public void FileServer(string serverName)
        {
            throw new NotImplementedException();
        }

        public void FileServerDown()
        {
            throw new NotImplementedException();
        }

        public void HoldItems(int EmployeeId, int QueueId, int TableId, int CheckId)
        {
            throw new NotImplementedException();
        }

        public void IAmMaster()
        {
            MainClass.IamIsMaster = true; 
        }

        public void InitializationComplete()
        {

            try
            {
                MainClass.InitData();
                MainClass.SendDishList();
                MainClass.CurentMaster = DownTimeiniFile.GetMaster();  
            }
            catch (Exception e)
            {
                Utils.ToLog("InitializationComplete() " + e.Message);
            }
        }

        public void LockOrder(int TableId)
        {

          //  Utils.ToCardLog("LockOrder" + TableId); 
        }

        public void LogIn(int EmployeeId, string Name)
        {
         //   AlohaTSClass.ShowMessage("Привет");
            try
            {
                AlohaTSClass.CurentWaiter = EmployeeId;
                EventSenderClass.SendAlohaAsincEvent(PDiscountCard.StopListService.AlohaEventType.Login, "", EmployeeId, 0, Name, 0, 0,0);
            }
            catch { }
            SendToVideo.LogIn(EmployeeId);
        }

        public void LogOut(int EmployeeId, string Name)
        {
           
            DisplayBoardClass.InitDisplayBoard();
            SendToVideo.LogOut(EmployeeId);
        }

        public void MasterDown()
        {
            throw new NotImplementedException();
        }

        public void ModifyItem(int EmployeeId, int QueueId, int TableId, int CheckId, int EntryId)
        {
            throw new NotImplementedException();
        }

        public void NameOrder(int EmployeeId, int QueueId, int TableId, string Name)
        {
            throw new NotImplementedException();
        }

        public bool inOrders(int Id)
        {
            foreach (RemoteOrderSrv.OrderInfoForAloha  oi in AlohaTSClass.RemoteOrders)
            {
                if (oi.ID  ==Id)
                {
                    return true ;
                }
            }
            return false ;
        }
        public void OnClockTick()
        {
            //if (File.Exists(@"C:\Aloha\Check\FiskalChanger.exe")) //А касса ли это? 

           //AlohaEventVoids.TrySendZReport();

            /*
            if (MainClass.TrPosxEnable)
            {
                if (MainClass.DeletedPayment > -1)
                {

                    if (MainClass.DeletedPaymentTry < 10)
                    {
                        if (DateTime.Now > MainClass.DeletedPaymentDateTime.AddSeconds(1))
                        {
                            Utils.ToCardLog("Удаляяю оплату. Попытка: " + MainClass.DeletedPaymentTry);
                            if (AlohaTSClass.DeletePayment(MainClass.DeletedPaymentCheck, MainClass.DeletedPayment))
                            {
                                Utils.ToCardLog("Успешно удалил оплату. ");
                                MainClass.DeletedPayment = -1;
                                MainClass.DeletedPaymentTry = 0;
                            }
                            else
                            {
                                MainClass.DeletedPaymentTry++;
                            }
                        }
                    }
                    else
                    {
                        MainClass.DeletedPayment = -1;
                        MainClass.DeletedPaymentTry = 0;
                    }
                }
             
            }
             */



            //Стоп лист
            if (MainClass.TimeTic.AddSeconds(300) < DateTime.Now)
            {
                MainClass.TimeTic = DateTime.Now;
                /*
                if (iniFile.StopListOff)
                {
                    return;
                }
                */
                if ((MainClass.CurentMaster != -1)||MainClass.IamIsMaster ) 
                {
                    if ((MainClass.CurentMaster == AlohaTSClass.GetTermNum())||MainClass.IamIsMaster)
                    {
                        MainClass.WHThreadThreadStopList.Set();
  
                    }
                }

                else
                {
                    MainClass.CurentMaster = DownTimeiniFile.GetMaster();
                }
            }


           //Сообщение
            if (Shtrih2.NeedShowMsg)
            {
                Shtrih2.ShowMessageExt();
            }

            //throw new NotImplementedException();
        }

        List<Int32> Checks = new List<int>();
       
        public void OpenCheck(int EmployeeId, int QueueId, int TableId, int CheckId)
        {
            //AlohaEvent.NewOrderVoid ();
            Utils.ToCardLog(String.Format("OpenCheck {0} on table {1}",CheckId  ,TableId));
        }
       
        public void OpenItem(int EmployeeId, int EntryId, int ItemId, string Description, double Price)
        {
            throw new NotImplementedException();
        }

        public void OpenTable(int EmployeeId, int QueueId, int TableId, int TableDefId, string Name)
        {
            Utils.ToCardLog("OpenTable" + TableId);

           

        }

        public void OrderItems(int EmployeeId, int QueueId, int TableId, int CheckId, int ModeId)
        {
            
            Utils.ToCardLog("[Event]OrderItems EmployeeId: " + EmployeeId + " QueueId: " + QueueId + " TableId: " + TableId + " CheckId: " + CheckId + " ModeId: " + ModeId);

            if (CheckId > 0)
            {

                AlohaEventVoids.WriteTimeOfSend(TableId, CheckId, QueueId, EmployeeId, ModeId);
                AlohaEventVoids.TryAddDiscount(ModeId, CheckId, EmployeeId);
            }
            
            //Thread.Sleep(1000);
            //throw new NotImplementedException();
            

        }

        public void OrderScreen_TableCheckSeatChanged(int ManagerId, int EmployeeId, int QueueId, int TableId, int CheckId, int SeatNum)
        {
            Utils.ToCardLog("OrderScreen_TableCheckSeatChanged" + TableId); 
        }

        public void PostDeleteComp(int ManagerId, int EmployeeId, int QueueId, int TableId, int CheckId, int CompTypeId, int CompId)
        {
            throw new NotImplementedException();
        }

        public void PostDeleteItems(int ManagerId, int EmployeeId, int QueueId, int TableId, int CheckId, int ReasonId)
        {
            throw new NotImplementedException();
        }

        public void PostDeletePromo(int ManagerId, int EmployeeId, int QueueId, int TableId, int CheckId, int PromotionId, int PromoId)
        {
            throw new NotImplementedException();
        }

        public void PreModifyItem(int EmployeeId, int QueueId, int TableId, int CheckId, int EntryId)
        {
            int i = 0;
            //throw new NotImplementedException();
        }

        public void RerouteDisplayBoard(int EmployeeId, int QueueId, int TableId, int CheckId, int DisplayBoardID, int ControllingTerminalID, int DefaultOrderModeOverride, int CurrentOrderOnly)
        {
            throw new NotImplementedException();
        }

        public void SaveTab(int EmployeeId, int TableId, string Name)
        {
            throw new NotImplementedException();
        }

        public void SetMasterTerminal(int TerminalId)
        {
            
            
                MainClass.IamIsMaster =  (TerminalId == AlohaTSClass.GetTermNum());
            
        }

        public void SetQuickComboLevel(int EmployeeId, int QueueId, int TableId, int CheckId, int PromotionId, int PromoId, int nLevel, int nContext)
        {
            throw new NotImplementedException();
        }

        public void Shutdown()
        {

            MainClass.StopAllThreads();
        }

        public void SpecialMessage(int EmployeeId, int MessageId, string Message)
        {
            throw new NotImplementedException();
        }

        public void StartAddItem(int EmployeeId, int QueueId, int TableId, int CheckId, int EntryId, int ParentEntryId, int ModCodeId, int ItemId, string ItemName, double ItemPrice)
        {
          //  return 0;
            //throw new NotImplementedException();
            Utils.ToCardLog(String.Format("StartAddItem ParentEntryId ={0}, ItemId={1}, ItemName={2}", ParentEntryId, ItemId, ItemName));
            /*
            //if ((ParentEntryId!=0)&&(ItemId<933000))
            {
                Utils.ToCardLog("Try AddItem modif for kitchen " + ItemName);
                try
                {
                    Check Ch = AlohaTSClass.GetCheckById(CheckId);
                    Utils.ToCardLog("Try AddItem modif for kitchen  AlohaTSClass.GetCheckById(CheckId)");
                    Dish D = Ch.Dishez.Where(a => a.AlohaNum == EntryId).Single();
                    List<string> tmp = new List<string>() { ItemName + " Кухн" };
                    //AlohaTSClass.AddRussMessage2(D, Ch, tmp);
                    AlohaTSClass.AddRussMessage3(EntryId, ItemName + " Кухн");
                }
                catch(Exception e)
                {
                    Utils.ToCardLog("Error Try AddItem modif for kitchen "+ e.Message);
                }
            }
             * */

        }

        public void Startup(int hMainWnd)
        {
            int i;
            //throw new NotImplementedException();
            MBClient mBClient = new MBClient();
            mBClient.GetSettingTips();


        }

        public void TableToShowOnDispBChanged(int nTermID, int TableId)
        {
            throw new NotImplementedException();
        }

        public void TransferTable(int FromEmployeeId, int ToEmployeeId, int TableId, string NewName, int IsGetCheck)
        {
            try
            {
                EventSenderClass.SendAlohaAsincEvent(PDiscountCard.StopListService.AlohaEventType.TransferTable, "", FromEmployeeId, 0, "", ToEmployeeId, TableId,0);
            }
            catch { }
        }

        public void UnlockOrder(int TableId)
        {
         //PDiscountCard.CloseCheck.CheckIsClosed(TableId);
        }

        public void UpdateItems(int EmployeeId, int QueueId, int TableId, int CheckId)
        {
            throw new NotImplementedException();
        }

        #endregion


        #region IInterceptAlohaActivity12 Members


        public void AssignCashDrawer(int EmployeeId, int DrawerId, int IsPublic)
        {
            throw new NotImplementedException();
        }

        public void AuthorizePayment(int TableId, int CheckId, int PaymentId, int TransactionType, int TransactionResult)
        {
            throw new NotImplementedException();
        }

        public void CurrentCheckChanged(int TermId, int TableId, int CheckId)
        {
            throw new NotImplementedException();
        }

        public void DeassignCashDrawer(int EmployeeId, int DrawerId, int IsPublic)
        {
            throw new NotImplementedException();
        }

        public void EnterIberScreen(int TermId, int ScreenId)
        {
            throw new NotImplementedException();
        }

        public void ExitIberScreen(int TermId, int ScreenId)
        {
            throw new NotImplementedException();
        }

        public void FinalBump(int TableId)
        {
            throw new NotImplementedException();
        }

        public void NameOrder(int EmployeeId, int QueueId, int TableId, string Name, int CheckId)
        {
            throw new NotImplementedException();
        }

        public void ReassignCashDrawer(int EmployeeId, int DrawerId)
        {
            throw new NotImplementedException();
        }

        public void RenameTab(int TermId, int CheckId, string tabName)
        {
            if (iniFile.TakeOutEnabled)
            {
                if (tabName != "")
                {
                    Utils.ToCardLog("RenameTab "+ tabName);
                    Check Ch = AlohaTSClass.GetCheckById(CheckId);
                    TakeOut.TOLoaltyConnect.ApplyDisc(Ch.TableId, tabName);
                }
            }


        }

        public void ReopenCheck(int EmployeeId, int QueueId, int TableId, int CheckId)
        {
            throw new NotImplementedException();
        }

        public void SettleInfoChanged(string SettleInfo)
        {
            throw new NotImplementedException();
        }

        public void SplitCheck(int CheckId, int TableId, int QueueId, int EmployeeNumber, int NumberOfSplits, int SplitType)
        {
            throw new NotImplementedException();
        }

        #endregion


        #region IInterceptAlohaActivity13 Members


        public void KitchenOrderStatus(string Orders)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IInterceptAlohaActivity14 Members


        public void PostLockOrder(int TermId)
        {
            throw new NotImplementedException();
        }

        public void PostUnlockOrder(int TermId)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IInterceptAlohaActivity15 Members


        public void SpoolMode(int TermId, int SpoolMode)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
