using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;


namespace OrderToAlohaSrv
{
    public class RemoteLisenter
    {
        RemoteConnection RC;
        public void Init(int port)
        {
            
            try
            {
                RC = new RemoteConnection();
                RC.StartServerLisenter(port);
                RC.DataRecive += new RemoteConnection.DataReciveDelegate(RC_DataRecive);
                Utils.ToLog("RemoteLisenter Init " );
            }
            catch (Exception e)
            {
                Utils.ToLog("[Error] RemoteLisenter Init " + e.Message);
            }
        }

        void RC_DataRecive(object Fi)
        {
            /*
            try
            {
                Utils.ToLog("StopServerLisenter");
                RC.StopServerLisenter();
                RC.
            }
            catch(Exception e)
            { }
            */

            try
            {
                Utils.ToLog("RC_DataRecive InCommand");
                 DataReciver.STCommand InCommand = (DataReciver.STCommand)Fi;
                
                Utils.ToLog("RC_DataRecive тип " + InCommand.CommandType);
                
                if (InCommand.Ansver)
                {

                }
                else
                {
                    switch (InCommand.CommandType)
                    {
                        case DataReciver.STCommandType.AddOrder:
                            try
                            {
                                DataReciver.SendOrderToAlohaRequest req = InCommand.sendOrderToAlohaRequest;
                                /*
                                int Chid = 0;
                                int Tid = 0;
                               
                                SendFromNetToAloha.SendOrderToAloha(req.OrderId, req.Items, req.CompanyId, req.CompanyName, req.BortName, req.DiscountId, req.Margin, req.TimeOfShipping, req.FreeDisc, out Chid, out Tid);
                                 * */
                                int Chid = 1;
                                int Tid = 1;
                                SendFromNetToAloha.SendOrderToAloha(InCommand);
                                Utils.ToLog("SendFromNetToAloha.SendOrderToAloha" );
                                req.AlohaCheckId = Chid;
                                req.AlohaTableNum = Tid;
                                
                                InCommand.Result = true;
                                InCommand.ResultId = 1;

                            }
                            catch (Exception e)
                            {
                                InCommand.Result = false;
                                InCommand.ResultId = 0;
                                InCommand.ExeptionMessage = e.Message;
                                //InCommand.e
                            }


                            break;
                        case DataReciver.STCommandType.DeleteOrder:

                            try
                            {
                                DataReciver.DeleteOrderRequest req = InCommand.deleteOrderRequest;
                                

                                SendFromNetToAloha.DeleteOrder(req.OrderId);
                                InCommand.Result = true;
                                InCommand.ResultId = 1;

                            }
                            catch (Exception e)
                            {
                                InCommand.Result = false;
                                InCommand.ResultId = 0;
                                InCommand.ExeptionMessage = e.Message;
                                //InCommand.e
                            }
                            break;
                        default:
                            break;
                    }
                    InCommand.Ansver = true;
                    //Thread.Sleep(2000);
                    //bool r = RC.SendData(InCommand.Sender, InCommand.SenderPort, InCommand);
                   // Utils.ToLog("RC.SendData " + r.ToString());
                    /*
                    switch (InCommand.CommandType)
                    {
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
                                InCommand.EntIds = AlohaTSClass.GetStopList2();
                                InCommand.Result = true;

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
                                        ItemSpecialMessage = ""

                                    };

                                    Tmpit.Add(IT);
                                    List<RemoteOrderSrv.Modifier> TmpMods = new List<RemoteOrderSrv.Modifier>();

                                    foreach (DataReciver.Item mod in it.Mods)
                                    {
                                        RemoteOrderSrv.Modifier md = new RemoteOrderSrv.Modifier
                                        {
                                            ItemModifierID = mod.Barcode

                                        };
                                        TmpMods.Add(md);
                                    }

                                    IT.Modifiers = TmpMods.ToArray();


                                }
                                OI.Items = Tmpit.ToArray();



                                InCommand.ResultId = MainClass.CTG.AddRemoteOrderFromTCPCommand(OI, out InCommand.ExeptionMessage);
                                break;
                            }
                        default:
                            {
                                break;
                            }
                    }
                    InCommand.Ansver = true;

                 //   Utils.ToLog("RC.SendData port: " + iniFile.RemoteSenderPort + " CompName: " + InCommand.Sender);
                    RC.SendData(InCommand.Sender, iniFile.RemoteSenderPort, InCommand);
                     * * */
                }
                
            }
            catch (Exception e)
            {
                //Utils.ToLog(e.Message);
                Utils.ToLog("Error RC_DataRecive InCommand");

            }
            Utils.ToLog("Recive End");
                 
        }


    }
}
