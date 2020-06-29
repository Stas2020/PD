using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace PDiscountCard.RemoteCommands
{
    static public class RemoteLisenter
    {
        static RemoteConnection RC;
        internal static void Init()
        {
            try
            {
                RC = new RemoteConnection();
                RC.StartServerLisenter(iniFile.RemoteLisenterPort);
                RC.DataRecive += new RemoteConnection.DataReciveDelegate(RC_DataRecive);
            }
            catch (Exception e)
            {
                Utils.ToCardLog("[Error] RemoteLisenter Init " + e.Message);
            }
        }

        static void RC_DataRecive(object Fi)
        {
            try
            {
                DataReciver.PDiscountCommand InCommand = (DataReciver.PDiscountCommand)Fi;
                Utils.ToCardLog("RC_DataRecive тип " + InCommand.CommandType);
                if (InCommand.Ansver)
                {
                    
                }
                else
                {
                    switch (InCommand.CommandType)
                    {

                        case DataReciver.PDiscountCommandType.CloseCheck:
                            {
                               // if (!iniFile.RemoteCloseCheckEnabled)
                                {
                                 //   InCommand.Result = AlohaTSClass.OrderItems(InCommand.EntIds, InCommand.CheckId, InCommand.TableId, out InCommand.ExeptionMessage);
                                    InCommand.Result = true;
                                    RemoteCloseCheck.AddRemoteChkToQuereLocal(InCommand.CheckId, InCommand.PaymentId, InCommand.EmployeeId, InCommand.PaymentSumm);
                                }
                                break;
                            }
                        case DataReciver.PDiscountCommandType.OrderItems:
                            {
                                InCommand.Result = AlohaTSClass.OrderItems(InCommand.EntIds, InCommand.CheckId, InCommand.TableId, out InCommand.ExeptionMessage);
                                break;
                            }
                        case DataReciver.PDiscountCommandType.OrderAllItems:
                            {
                                InCommand.Result = AlohaTSClass.OrderAllItems(InCommand.CheckId, InCommand.TableId, out InCommand.ExeptionMessage);
                                break;
                            }
                        case DataReciver.PDiscountCommandType.GetStopList:
                            {
                                //InCommand.Result = AlohaTSClass.OrderAllItems(InCommand.CheckId, InCommand.TableId, out InCommand.ExeptionMessage);
                                InCommand.EntIds= AlohaTSClass.GetStopList2();
                                InCommand.Result = true;

                                break;
                            }
                        case DataReciver.PDiscountCommandType.DeleteOrder:
                            {
                                //RemoteCloseCheck.RemoteDeleteCheck(InCommand.CheckId);
                                AlohaTSClass.LogIn(iniFile.RemoteOrderTerminalNumber, iniFile.RemoteOrderWaterNumber);
                                AlohaTSClass.DeleteAllItemsOnCurentCheckandClose2(iniFile.RemoteOrderTerminalNumber,
                                    iniFile.RemoteOrderWaterNumber, InCommand.CheckId);
                                    break;
                            }
                        case DataReciver.PDiscountCommandType.PrintSlip:
                            {
                                if (iniFile.CreditCardSlipPrintPreCheck)
                                {
                                    try
                                    {
                                        // string[] stringSeparators = new string[] { "\n\r", "\n\n", Environment.NewLine};

                                        string[] stringSeparators = new string[] { "\n" };

                                        string sres = InCommand.Slip.Replace("\r", "");

                                        //if (iniFile.pri)
                                        AlohaTSClass.PrintCardSlip(sres.Split(stringSeparators, StringSplitOptions.None).ToList());
                                    }
                                    catch (Exception e)
                                    {
                                        Utils.ToCardLog("Ошибка печати слипа " + e.Message);
                                    }
                                }
                                else
                                {
                                   // Slip += Convert.ToChar(31);
                                    ToShtrih.PrintCardCheck(InCommand.Slip);
                                }
                                ToShtrih.PrintCardCheck(InCommand.Slip);
                                break;
                            }
                        case DataReciver.PDiscountCommandType.AddRemoteOrder:
                            {

                                RemoteOrderSrv.OrderInfoForAloha OI = new RemoteOrderSrv.OrderInfoForAloha();
                                
                                List<RemoteOrderSrv.Item> Tmpit = new List<RemoteOrderSrv.Item>();

                                OI.OrderID = "0";
                                
                                foreach (DataReciver.Item it in InCommand.OrderBody)
                                {
                                    
                                    RemoteOrderSrv.Item IT = new RemoteOrderSrv.Item
                                    {
                                        ItemID = it.Barcode,
                                        ItemSpecialMessage=""

                                    };
                                
                                    Tmpit.Add(IT);
                                    List<RemoteOrderSrv.Modifier> TmpMods = new List<RemoteOrderSrv.Modifier>();
                                
                                    foreach (DataReciver.Item mod in it.Mods)
                                    {
                                        RemoteOrderSrv.Modifier md = new RemoteOrderSrv.Modifier { 
                                        ItemModifierID = mod.Barcode

                                        };
                                        TmpMods.Add(md);
                                    }
                                
                                    IT.Modifiers = TmpMods.ToArray();
                                    

                                }
                                OI.Items = Tmpit.ToArray();

                                

                                InCommand.ResultId = MainClass.CTG.AddRemoteOrderFromTCPCommand(OI, out InCommand.ExeptionMessage, out  InCommand.CheckId);
                                break;
                            }
                        default:
                            {
                                break;
                            }
                    }
                    InCommand.Ansver = true;

                 //   Utils.ToLog("RC.SendData port: " + iniFile.RemoteSenderPort + " CompName: " + InCommand.Sender);
                 //   RC.SendData(InCommand.Sender, iniFile.RemoteSenderPort   , InCommand);
                    if (InCommand.SenderPort == 0)
                    {
                        InCommand.SenderPort = iniFile.RemoteSenderPort;
                    }
                    Utils.ToLog("RC.SendData port: " + InCommand.SenderPort + " CompName: " + InCommand.Sender);

                    RC.SendData(InCommand.Sender, InCommand.SenderPort, InCommand);
                }

            }
            catch(Exception e)
            {
                Utils.ToLog(e.Message);
               
            }
        }

    }
}
