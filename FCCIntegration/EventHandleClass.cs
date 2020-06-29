using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Runtime.InteropServices;

namespace FCCIntegration
{
    class EventHandleClass
    {

        public delegate void ReplenishCountChangeEventHandler(object sender, FCCSrv2.CashType[] Ct);
        public delegate void ChangeMoneyEventHandler(object sender, int summ);
        public delegate void SetStatusEventHandler(object sender, bool StatusChange, int status, int DevId, string EventName);
        public delegate void mEventHandler(object sender);
        public delegate void SendErrorEventHandler(object sender, string Message, string Url);


        public event ReplenishCountChangeEventHandler OnReplenishCountChange;
        public event ChangeMoneyEventHandler OnSetDeposit;
        public event SetStatusEventHandler OnSetStatus;
        public event mEventHandler OnStartReplenishmentFromCassette;


        public event SendErrorEventHandler OnSendError;






        private System.Threading.Thread objListenThread;
        private System.Diagnostics.TraceSource appTrace;//System.Diagnostics.TraceSource("BrueBoxPosDemo") 
        //private frmMain pFrm;

        [DllImport("KERNEL32.DLL")]
        public static extern uint
            GetPrivateProfileString(string lpAppName,
                        string lpKeyName, string lpDefault,
                        StringBuilder lpReturnedString, uint nSize,
                        string lpFileName);

        public EventHandleClass()
        {

            thread_init();
        }

        private void thread_init()
        {
            appTrace = new System.Diagnostics.TraceSource("BrueBoxPosDemo");
            if (objListenThread == null)
            {
                objListenThread = new System.Threading.Thread(new System.Threading.ThreadStart(thread_start));
            }
            if (objListenThread.ThreadState == System.Threading.ThreadState.Stopped)
            {
                objListenThread = new System.Threading.Thread(new System.Threading.ThreadStart(thread_start));
            }

            objListenThread.IsBackground = true;
            objListenThread.Start();
        }

        TmpClass pFrm = new TmpClass();

        private void thread_start()
        {
            long nEventRecvCount = 0;

            appTrace.TraceEvent(System.Diagnostics.TraceEventType.Verbose, 0, "[thread_start] IN");
            appTrace.TraceEvent(System.Diagnostics.TraceEventType.Information, 0, "[thread_start] IN");

            Encoding enc = Encoding.UTF8;
            //System.Net.IPAddress ipAddsAny = System.Net.IPAddress.Any;
            //            System.Net.Sockets.TcpListener listener = new System.Net.Sockets.TcpListener(ipAddsAny, 55561);

            //  System.Net.IPAddress Ip = new System.Net.IPAddress(new byte[] { 192, 168, 0, 1 });
            // System.Net.IPAddress Ip = new System.Net.IPAddress(new byte[] { 192, 168, 77, 210 });
            //System.Net.IPAddress Ip = new System.Net.IPAddress(new byte[] { 192, 168, 77, 245 });

            System.Net.IPAddress Ip;
            string LocalIp = MainClass2.Setting.LocalIp;

            Utils.ToLog(String.Format("Запуск слушателя событий адрес {0}, порт {1}", LocalIp, MainClass2.Setting.EventLisenterPort));

            if (LocalIp.ToLower() == "localhost")
            {
                Ip = System.Net.IPAddress.Any;
            }
            else
            {
                try
                {
                    string s = LocalIp.Split("."[0])[0];
                    Ip = new System.Net.IPAddress(new byte[] { byte.Parse(LocalIp.Split("."[0])[0]), byte.Parse(LocalIp.Split("."[0])[1]), byte.Parse(LocalIp.Split("."[0])[2]), byte.Parse(LocalIp.Split("."[0])[3]) });
                }
                catch
                {
                    Ip = System.Net.IPAddress.Any;
                }
            }


            System.Net.Sockets.TcpListener listener = new System.Net.Sockets.TcpListener(Ip, MainClass2.Setting.EventLisenterPort);



            try
            {
                while (true)
                {
                    listener.Start();

                    Console.WriteLine("Start listern on Port{0}", MainClass2.Setting.EventLisenterPort);

                    System.Net.Sockets.TcpClient tcp;
                    tcp = listener.AcceptTcpClient();
                    Utils.ToLog(String.Format("Cлушатель событий запущен"));
                    Console.WriteLine("Client connected.");
                    appTrace.TraceInformation("Client connected.");

                    nEventRecvCount = 0;

                    System.Net.Sockets.NetworkStream ns = tcp.GetStream();

                    int resSize;
                    string cmd = "";
                    bool CloseSock = false;
                    do
                    {
                        byte[] bytes = new byte[tcp.ReceiveBufferSize];
                        resSize = ns.Read(bytes, 0, tcp.ReceiveBufferSize);

                        if (resSize == 0)
                        {
                            Console.WriteLine("Client is disconnected.");
                            appTrace.TraceInformation("Client is disconnected.");
                            CloseSock = true;
                            break;
                        }

                        long s = 0;
                        long e = 0;
                        for (e = 0; e < resSize; e++)
                        {
                            if (bytes[e] == 0)
                            {
                                cmd += System.Text.Encoding.ASCII.GetString(bytes, (int)s, (int)(e - s));
                                Console.WriteLine("cmd : " + cmd);
                                // Here the event handling should be located.>>>>>>>>>>>>>>>
                                CheckRecvComand(cmd, nEventRecvCount);

                                // if one packet includes several events, then the next event 
                                // also should be handled.
                                s = e + 1;

                                cmd = "";
                            }
                            else if (e == resSize - 1)
                            {
                                // If one event is separated to several packets, then they 
                                // should be merged to one string.
                                cmd += System.Text.Encoding.ASCII.GetString(bytes, (int)s, (int)(e - s + 1));
                            }
                        }
                    } while (true);

                    if (CloseSock == false)
                    {
                        tcp.Close();
                        Console.WriteLine("Disconnected");
                        appTrace.TraceEvent(System.Diagnostics.TraceEventType.Error, 0, "Disconnected");
                        continue;
                    }

                    listener.Stop();
                    Console.WriteLine("Listener is closed.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
                appTrace.TraceEvent(System.Diagnostics.TraceEventType.Critical, 0, ex.Message.ToString());
            }
        }

        //'*****************************************************
        //'Space
        //'*****************************************************
        public string Space(int num)
        {
            string ret = "";
            for (int i = 0; i < num; i++)
            {
                ret += " ";
            }
            return ret;
        }

        private static XmlNode GetNideByName(XmlNode Xn, string Name)
        {

            foreach (XmlNode Xnn in Xn.ChildNodes)
            {
                if (Xnn.Name == Name)
                {
                    return Xnn;
                }
            }
            return null;


        }
        private int LastFixedDispense = 0;

        private void CheckRecvComand(string returndata, long nEventRecvCount)
        {
            try
            {
                System.Xml.XmlDocument dom = new System.Xml.XmlDocument();
                dom.LoadXml(returndata);

                System.Xml.XmlNodeList nodelist1 = dom.SelectNodes("/BbxEventRequest/*");
                foreach (System.Xml.XmlNode node in nodelist1)
                {
                    String strName = node.Name;

                    //judge the kind of event



                    if (strName == "StatusChangeEvent")
                    {
                        //Get node object of each element
                        System.Xml.XmlNode nodeStatus = node.SelectSingleNode("./Status");
                        System.Xml.XmlNode nodeAmount = node.SelectSingleNode("./Amount");
                        System.Xml.XmlNode nodeError = node.SelectSingleNode("./Error");
                        System.Xml.XmlNode nodeUser = node.SelectSingleNode("./User");
                        System.Xml.XmlNode nodeSeqNo = node.SelectSingleNode("./SeqNo");

                        Utils.ToLog(String.Format("Event Name: {0},  nodeStatus: {1}", strName, nodeStatus.InnerText));

                        String convUser = "";

                        convUser = nodeUser.InnerText;


                        if (Int32.Parse(nodeStatus.InnerText) == 21)
                        {
                            LastFixedDispense = int.Parse(nodeAmount.InnerText);
                        }


                        if (OnSetStatus != null)
                        {
                            int Summ = int.Parse(nodeAmount.InnerText);
                            if (Int32.Parse(nodeStatus.InnerText) == 7) //Это для сдачи
                            {
                                Summ =     LastFixedDispense ; 
                            }
                            OnSetStatus(this, true, Int32.Parse(nodeStatus.InnerText), Summ, "");
                        }


                        //Check if the fcc is under change operation
                        if ((Int32.Parse(nodeStatus.InnerText) == CFCCApi.STATUS_CODE_DEPOSIT_WAIT) ||
                            (Int32.Parse(nodeStatus.InnerText) == CFCCApi.STATUS_CODE_DEPOSIT_COUNTING) ||
                            (Int32.Parse(nodeStatus.InnerText) == CFCCApi.STATUS_CODE_CALCREPLENISH))
                        {
                            //  pFrm.SetStatus(Int32.Parse(nodeStatus.InnerText),nodeStatus.InnerText);
                            //if doing change operation, then the amount should store deposit amount.
                            int dblCashin = int.Parse(nodeAmount.InnerText);

                            if (OnSetDeposit != null)
                            {
                                OnSetDeposit(this, dblCashin);
                            }
                        }
                        else if (Int32.Parse(nodeStatus.InnerText) == CFCCApi.STATUS_CODE_DISPENSE)
                        {
                            //If doing dispensing, then the amount should be dispense amount.
                            //The dispense amount is not result value, it just a parameter of dispense command.
                            //Actual dispensed amount will be reported after dispense command has been done.

                        }


                        // In the case of a StartReplenishmentFromCassette
                        // If status becomes an idol after lock normalcy, start a ReplenishmentFromCassette
                        //if (MainClass.repFC_state)
                        {
                            if (Int32.Parse(nodeStatus.InnerText) == CFCCApi.STATUS_CODE_IDLE)
                            {
                                if (OnStartReplenishmentFromCassette != null)
                                {
                                    OnStartReplenishmentFromCassette(this);
                                }
                            }
                        }

                        // pFrm.SetGuidance(pFrm.GetStatusString(Int32.Parse(nodeStatus.InnerText)));
                    }
                    else if (strName.Equals("GlyCashierEvent"))
                    {


                        string strOutput = "";

                        //Device Event
                        String sDevID = node.Attributes["devid"].InnerText;
                        String sUser = node.Attributes["user"].InnerText;

                        System.Xml.XmlNode nEventTypeNode = node.FirstChild;
                        String strEventName = nEventTypeNode.Name;
                        Utils.ToLog(String.Format("Event Name: {0},  strEventName: {1}, sDevID: {2}", strName, nEventTypeNode.Name, sDevID));

                        int Summ = 0;
                        FCCSrv2.CashType[] CTT = new FCCSrv2.CashType[1];

                        if ((strEventName.Equals("eventReplenishCountChange")) || (strEventName.Equals("eventCassetteInventoryOnRemoval")))
                        {



                            FCCSrv2.CashType CT = new FCCSrv2.CashType();

                           

                            System.Xml.XmlNode CashNode = nEventTypeNode;
                            if (strEventName.Equals("eventCassetteInventoryOnRemoval"))
                            {
                                CashNode =GetNideByName (nEventTypeNode,"Cash");
                            }
                            CT.Denomination = new FCCSrv2.DenominationType[CashNode.ChildNodes.Count];
                            CT.type = "4";
                            int i = 0;
                            



                                    foreach (System.Xml.XmlNode n in CashNode.ChildNodes)
                                    {
                                        if (n.Name == "Denomination")
                                        {
                                            FCCSrv2.DenominationType Den = new FCCSrv2.DenominationType();
                                            try
                                            {
                                                Den.cc = n.Attributes["cc"].Value;
                                                Den.fv = n.Attributes["fv"].Value;
                                                Den.devid = n.Attributes["devid"].Value;
                                                foreach (System.Xml.XmlNode nn in n.ChildNodes)
                                                {
                                                    try
                                                    {
                                                        if (nn.Name.Equals("Piece"))
                                                        {
                                                            Den.Piece = nn.InnerText;
                                                        }
                                                        else if (nn.Name.Equals("Status"))
                                                        {
                                                            Den.Status = nn.InnerText;
                                                        }
                                                    }
                                                    catch
                                                    { }
                                                }
                                                CT.Denomination[i] = Den;
                                                CTT[0] = CT;
                                                i++;

                                                Summ += int.Parse(Den.Piece) * int.Parse(Den.fv);
                                            }
                                            catch
                                            { }
                                        }
                                   


                            }

                        }
                        if (OnSetStatus != null)
                        {
                            /*
                            if (strEventName.Equals("eventWaitForRemoving"))
                            {
                            }
                            */
                            OnSetStatus(this, false, Summ, int.Parse(sDevID), strEventName);
                        }


                        if (strEventName.Equals("eventWaitForRemoving"))
                        {
                            //Wait removeing notes from RBW
                            System.Xml.XmlNode nDevicePositionID = nEventTypeNode.FirstChild;
                            string strIDName = GetDevicePositionIDString(nDevicePositionID.InnerText);
                            pFrm.SetStatus(Int32.Parse(sDevID), "WaitForRemoving " + strIDName);




                        }
                        else if (strEventName.Equals("eventRemoved"))
                        {
                            //Note was removed from RBW
                            System.Xml.XmlNode nDevicePositionID = nEventTypeNode.FirstChild;
                            string strIDName = GetDevicePositionIDString(nDevicePositionID.InnerText);
                            pFrm.SetStatus(Int32.Parse(sDevID), "Removed " + strIDName);
                        }
                        else if (strEventName.Equals("eventStatusChange"))
                        {
                            //Status Change
                            System.Xml.XmlNode nDeviceStatusID = nEventTypeNode.FirstChild;
                            string strIDName = GetDeviceStatusIDString(nDeviceStatusID.InnerText);

                            Utils.ToLog(String.Format("--------------eventStatusChange: StatusId{0},  str: {1}", nDeviceStatusID.InnerText, strIDName));

                            pFrm.SetStatus(Int32.Parse(sDevID), "StatusChange " + strIDName);


                        }
                        else if (strEventName.Equals("eventEmpty"))
                        {
                            //Empty
                            System.Xml.XmlNode nDevicePositionID = nEventTypeNode.FirstChild;
                            string strIDName = GetDevicePositionIDString(nDevicePositionID.InnerText);
                            pFrm.SetStatus(Int32.Parse(sDevID), "Empty " + strIDName);
                        }
                        else if (strEventName.Equals("eventLow"))
                        {
                            //Near Empty
                            System.Xml.XmlNode nDevicePositionID = nEventTypeNode.FirstChild;
                            string strIDName = GetDevicePositionIDString(nDevicePositionID.InnerText);
                            pFrm.SetStatus(Int32.Parse(sDevID), "Near Empty " + strIDName);
                        }
                        else if (strEventName.Equals("eventExist"))
                        {
                            //Exist 
                            System.Xml.XmlNode nDevicePositionID = nEventTypeNode.FirstChild;
                            string strIDName = GetDevicePositionIDString(nDevicePositionID.InnerText);
                            pFrm.SetStatus(Int32.Parse(sDevID), "Exist " + strIDName);
                        }
                        else if (strEventName.Equals("eventHigh"))
                        {
                            //Near Full
                            System.Xml.XmlNode nDevicePositionID = nEventTypeNode.FirstChild;
                            string strIDName = GetDevicePositionIDString(nDevicePositionID.InnerText);
                            pFrm.SetStatus(Int32.Parse(sDevID), "High " + strIDName);
                        }
                        else if (strEventName.Equals("eventFull"))
                        {
                            //Full
                            System.Xml.XmlNode nDevicePositionID = nEventTypeNode.FirstChild;
                            string strIDName = GetDevicePositionIDString(nDevicePositionID.InnerText);
                            pFrm.SetStatus(Int32.Parse(sDevID), "Full " + strIDName);
                        }
                        else if (strEventName.Equals("eventMissing"))
                        {
                            //Missing unit.
                            System.Xml.XmlNode nDevicePositionID = nEventTypeNode.FirstChild;
                            string strIDName = GetDevicePositionIDString(nDevicePositionID.InnerText);
                            pFrm.SetStatus(Int32.Parse(sDevID), "Missing " + strIDName);
                        }
                        else if (strEventName.Equals("eventDepositCountChange"))
                        {
                            //Depositing
                        }
                        else if (strEventName.Equals("eventReplenishCountChange"))
                        {
                            /*
                            FCCSrv2.CashType[] CTT = new FCCSrv2.CashType[1];
                            
                            FCCSrv2.CashType CT = new FCCSrv2.CashType();

                            CT.Denomination = new FCCSrv2.DenominationType[nEventTypeNode.ChildNodes.Count];
                            CT.type="4";
                            int i= 0;
                            int Summ = 0;
                            foreach (System.Xml.XmlNode n in nEventTypeNode.ChildNodes)
                            {
                                FCCSrv2.DenominationType Den = new FCCSrv2.DenominationType();
                                Den.cc = n.Attributes["cc"].Value;
                                Den.fv = n.Attributes["fv"].Value;
                                Den.devid = n.Attributes["devid"].Value;
                                foreach (System.Xml.XmlNode nn in n.ChildNodes)
                                {
                                    if (nn.Name.Equals("Piece"))
                                    {
                                        Den.Piece = nn.InnerText;    
                                    } else if (nn.Name.Equals("Status"))
                                    {
                                        Den.Status = nn.InnerText;
                                    }
                                }
                                CT.Denomination[i] = Den;
                                CTT[0] = CT;
                                i++;

                                Summ += int.Parse(Den.Piece) * int.Parse(Den.fv);



                            }
                            */
                            Utils.ToMoneyCountLog(MoneyChangeCommands.Replenish, Summ);


                            if (OnReplenishCountChange != null)
                            {
                                OnReplenishCountChange(this, CTT);
                            }

                            //Replenishing
                        }
                        else if (strEventName.Equals("eventError"))
                        {
                            //Error occurred
                            System.Xml.XmlNode nErrorCode = nEventTypeNode.FirstChild;
                            pFrm.SetStatus(Int32.Parse(sDevID), "Error " + nErrorCode.InnerText);
                            System.Xml.XmlNode nURL = nErrorCode.NextSibling;
                            String errCode = Int32.Parse(nErrorCode.InnerText).ToString("X");
                            int i;
                            for (i = 1; i <= 4 - errCode.Length; i++)
                                errCode = "0" + errCode;

                            if (!String.IsNullOrEmpty(nURL.InnerText))
                            {
                                if (OnSendError != null)
                                {
                                    OnSendError(this, errCode, nURL.InnerText);
                                }
                                pFrm.ShowRecoveryScreen(errCode, nURL.InnerText);

                            }

                        }
                        else if (strEventName.Equals("eventCassetteInserted"))
                        {
                            //Inserted cassette
                            System.Xml.XmlNode nCassetteID = nEventTypeNode.FirstChild;
                            pFrm.SetStatus(Int32.Parse(sDevID), "CassetteInserted " + nCassetteID.InnerText);
                        }
                        else if (strEventName.Equals("eventPowerOffOnRequest"))
                        {
                            //Power off/on request
                        }
                        else if (strEventName.Equals("eventDownloadProgress"))
                        {
                            //Download progress
                        }
                        else if (strEventName.Equals("eventLogreadProgress"))
                        {
                            //Log Read progress
                        }
                        else if (strEventName == "eventRequireVerifyDenomination")
                        {
                            //RequireVerify
                            int i = 0;
                            System.Xml.XmlNode nCash = nEventTypeNode.FirstChild;
                            //Initialize
                            pFrm.SetWarningLabel(3, sDevID, "");
                            for (i = 0; i <= nCash.ChildNodes.Count - 1; i++)
                            {
                                System.Xml.XmlElement denomi = null;
                                string fv = null;

                                denomi = (System.Xml.XmlElement)nCash.ChildNodes.Item(i);
                                fv = denomi.GetAttribute("fv");
                                pFrm.SetWarningLabel(1, sDevID, fv);
                            }
                        }
                        else if (strEventName == "eventRequireVerifyCollectionContainer")
                        {
                            //RequireVerify IF Cassette
                            pFrm.SetWarningLabel(2, sDevID, "");
                        }
                        else if (strEventName == "eventRequireVerifyMixStacker")
                        {
                            //RequireVerify MIX Stacker
                            pFrm.SetWarningLabel(5, sDevID, "");
                        }
                        else if (strEventName == "eventExactDenomination")
                        {
                            //Exact
                            pFrm.SetWarningLabel(3, sDevID, "");
                        }
                        else if (strEventName == "eventExactCollectionContainer")
                        {
                            //Exact IF Cassette
                            pFrm.SetWarningLabel(4, sDevID, "");
                        }
                        else if (strEventName == "eventExactMixStacker")
                        {
                            //Exact MIX Stacker
                            pFrm.SetWarningLabel(6, sDevID, "");
                        }
                        else if (strEventName.Equals("eventWaitForOpening"))
                        {
                            //Wait For Opening
                            System.Xml.XmlNode nDoorID = nEventTypeNode.FirstChild;
                            string strIDName = GetDoorIDString(nDoorID.InnerText);
                            pFrm.SetStatus(Int32.Parse(sDevID), "WaitForOpening " + strIDName);
                        }
                        else if (strEventName.Equals("eventOpened"))
                        {
                            //Opened
                            System.Xml.XmlNode nDoorID = nEventTypeNode.FirstChild;
                            string strIDName = GetDoorIDString(nDoorID.InnerText);
                            pFrm.SetStatus(Int32.Parse(sDevID), "Opened " + strIDName);
                        }
                        else if (strEventName.Equals("eventClosed"))
                        {
                            //Closed
                            System.Xml.XmlNode nDoorID = nEventTypeNode.FirstChild;
                            string strIDName = GetDoorIDString(nDoorID.InnerText);
                            pFrm.SetStatus(Int32.Parse(sDevID), "Closed " + strIDName);
                        }
                        else if (strEventName.Equals("eventLocked"))
                        {
                            //Locked
                            System.Xml.XmlNode nDoorID = nEventTypeNode.FirstChild;
                            string strIDName = GetDoorIDString(nDoorID.InnerText);
                            pFrm.SetStatus(Int32.Parse(sDevID), "Locked " + strIDName);
                        }
                        else if (strEventName.Equals("eventWaitForInsertion"))
                        {
                            //Wait For Insertion
                            System.Xml.XmlNode nDevicePositionID = nEventTypeNode.FirstChild;
                            string strIDName = GetDevicePositionIDString(nDevicePositionID.InnerText);
                            pFrm.SetStatus(Int32.Parse(sDevID), "WaitForInsertion " + strIDName);
                        }
                        else
                        {
                            //Unknown
                            strOutput = "";
                            strOutput += strEventName + "\n";
                        }
                    }
                    else if (strName == "InventoryResponse")
                    {
                        // SetInventory
                        pFrm.SetEventInventory(node);
                    }
                    else
                    {
                        String strOutput = "";

                        strOutput = "";
                        strOutput += ("(" + nEventRecvCount.ToString() + ")" + "\n");
                        strOutput += ("SOAP Completed." + "\n");

                        strOutput += (" res name  : " + strName + "\n");
                        strOutput += ("------------------------------" + "\n");

                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
                appTrace.TraceEvent(System.Diagnostics.TraceEventType.Critical, 0, ex.Message.ToString());
            }
        }

        public String GetDeviceStatusIDString(String strID)
        {
            String strRet;
            switch (Int32.Parse(strID))
            {
                case 0:
                    strRet = "STATUS_INTERNAL_ERROR";
                    break;
                case 1:
                    strRet = "STATUS_IDLE";
                    break;
                case 2:
                    strRet = "STATUS_COUNTING";
                    break;
                case 3:
                    strRet = "STATUS_USING_OWN";
                    break;
                case 4:
                    strRet = "STATUS_BUSY";
                    break;
                case 5:
                    strRet = "STATUS_ERROR";
                    break;
                case 6:
                    strRet = "STATUS_ERROR_COMMUNICATION";
                    break;
                case 7:
                    strRet = "STATUS_DLL_INITIALIZE_BUSY";
                    break;
                default:
                    strRet = "Unknown:" + strID;
                    break;
            }
            return (strRet);
        }

        public String GetDevicePositionIDString(String strID)
        {
            String strRet;

            switch (Int32.Parse(strID))
            {
                case 1:
                    strRet = "ENTRANCE";
                    break;
                case 2:
                    strRet = "EXIT";
                    break;
                case 3:
                    strRet = "CPS1_COUNTER";
                    break;
                case 4:
                    strRet = "CPS2_COUNTER";
                    break;
                case 5:
                    strRet = "CPS3_COUNTER";
                    break;
                case 6:
                    strRet = "CPS4_COUNTER";
                    break;
                case 7:
                    strRet = "CPS5_COUNTER";
                    break;
                case 8:
                    strRet = "CPS6_COUNTER";
                    break;
                case 9:
                    strRet = "CPS7_COUNTER";
                    break;
                case 10:
                    strRet = "CPS8_COUNTER";
                    break;
                case 11:
                    strRet = "CONTAINER_C1_COUNTER";
                    break;
                case 12:
                    strRet = "CONTAINER_C2_COUNTER";
                    break;
                case 13:
                    strRet = "CONTAINER_C3_COUNTER";
                    break;
                case 14:
                    strRet = "CONTAINER_C4A_COUNTER";
                    break;
                case 15:
                    strRet = "CONTAINER_C4B_COUNTER";
                    break;
                case 16:
                    strRet = "CPS1_C3_COUNTER";
                    break;
                case 17:
                    strRet = "CPS2_C3_COUNTER";
                    break;
                case 18:
                    strRet = "CPS3_C3_COUNTER";
                    break;
                case 19:
                    strRet = "CAPTUREBIN_COUNTER";
                    break;
                case 20:
                    strRet = "CONTAINER_FIT_COUNTER";
                    break;
                case 21:
                    strRet = "COLLECTION_BOX";
                    break;
                case 22:
                    strRet = "CPS4_C3_COUNTER";
                    break;
                case 23:
                    strRet = "CPS5_C3_COUNTER";
                    break;
                case 24:
                    strRet = "CPS6_C3_COUNTER";
                    break;
                case 25:
                    strRet = "CPS7_C3_COUNTER";
                    break;
                case 26:
                    strRet = "CPS8_C3_COUNTER";
                    break;
                case 27:
                    strRet = "ESCROW";
                    break;
                case 28:
                    strRet = "CPS9_COUNTER";
                    break;
                case 29:
                    strRet = "CPS10_COUNTER";
                    break;
                case 30:
                    strRet = "CPS9_C3_COUNTER";
                    break;
                case 31:
                    strRet = "CPS10_C3_COUNTER";
                    break;
                case 32:
                    strRet = "CPS1_C4B_COUNTER";
                    break;
                case 33:
                    strRet = "CPS2_C4B_COUNTER";
                    break;
                case 34:
                    strRet = "CPS3_C4B_COUNTER";
                    break;
                case 35:
                    strRet = "CPS4_C4B_COUNTER";
                    break;
                case 36:
                    strRet = "CPS5_C4B_COUNTER";
                    break;
                case 37:
                    strRet = "CPS6_C4B_COUNTER";
                    break;
                case 38:
                    strRet = "CPS7_C4B_COUNTER";
                    break;
                case 39:
                    strRet = "CPS8_C4B_COUNTER";
                    break;
                case 40:
                    strRet = "CPS9_C4B_COUNTER";
                    break;
                case 41:
                    strRet = "CPS10_C4B_COUNTER";
                    break;
                case 42:
                    strRet = "RCW100_JAM_DOOR";
                    break;
                case 43:
                    strRet = "RCW100_TRANSPORT_UNIT";
                    break;
                case 44:
                    strRet = "RCW100_ENTRANCE_UNIT";
                    break;
                case 45:
                    strRet = "RBW100_MAINTE_DOOR";
                    break;
                case 46:
                    strRet = "RBW100_UPPER_DOOR";
                    break;
                case 47:
                    strRet = "RBW100_IF_CASETTE";
                    break;
                case 48:
                    strRet = "RBW100_STACK_CASETTE";
                    break;
                case 49:
                    strRet = "CPS1_C2_COUNTER";
                    break;
                case 50:
                    strRet = "CPS2_C2_COUNTER";
                    break;
                case 51:
                    strRet = "CPS3_C2_COUNTER";
                    break;
                case 52:
                    strRet = "CPS4_C2_COUNTER";
                    break;
                case 53:
                    strRet = "CPS5_C2_COUNTER";
                    break;
                case 54:
                    strRet = "CPS6_C2_COUNTER";
                    break;
                case 55:
                    strRet = "CPS7_C2_COUNTER";
                    break;
                case 56:
                    strRet = "CPS8_C2_COUNTER";
                    break;
                case 57:
                    strRet = "CPS9_C2_COUNTER";
                    break;
                case 58:
                    strRet = "CPS10_C2_COUNTER";
                    break;
                case 64:
                    strRet = "UPPER_UNIT";
                    break;
                case 65:
                    strRet = "LOWER_UNIT";
                    break;
                case 73:
                    strRet = "COFB";
                    break;
                case 74:
                    strRet = "MIXED STACKER";
                    break;
                default:
                    strRet = "Unknown:" + strID;
                    break;
            }
            return (strRet);
        }

        public String GetDoorIDString(String strID)
        {
            String strRet;

            switch (Int32.Parse(strID))
            {
                case 1:
                    strRet = "COLLECTION DOOR";
                    break;
                case 2:
                    strRet = "MAINTENANCE DOOR";
                    break;
                case 73:
                    strRet = "COFB";
                    break;
                default:
                    strRet = "Unknown:" + strID;
                    break;
            }
            return (strRet);
        }
    }

    public class TmpClass
    {
        public TmpClass()
        { }


        public void SetStatus(int i, string s)
        {
            //MainClass.mEventHandleClass_OnSetStatus(this,i);
        }

        public void ShowRecoveryScreen(string s, string s1)
        {

        }
        public void SetWarningLabel(int i, string s1, string s2)
        {

        }
        public void SetEventInventory(XmlNode x)
        {
        }

    }

}
