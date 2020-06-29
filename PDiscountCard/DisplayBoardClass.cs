using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;
using System.Windows.Forms;
using System.Threading;

namespace PDiscountCard
{
    static class DisplayBoardClass
    {
        static System.IO.Ports.SerialPort port = new System.IO.Ports.SerialPort();



    

        static public void ApplyPaymentEventAsync(object stateInfo)
        {
            try
            {
                if (!iniFile.DisplayBoardEnabled)
                {
                    return;
                }
                Utils.ToCardLog("ApplyPaymentEventAsync");

                double Pr = 0;
                int CheckId = 0;
                if (stateInfo != null)
                {
                    try
                    {
                        CheckId = (int)stateInfo;
                        Pr = (double)AlohaTSClass.GetCheckSum(CheckId);
                    }
                    catch
                    {

                    }
                }
                else
                {
                    Pr = (double)AlohaTSClass.GetCurentCheckSumm();
                }
                if (iniFile.DisplayBoardEnabled)
                {
                    SendString("ИТОГО: ", Pr.ToString("0.00"));
                }
                /*
                if (iniFile.RemoteEventEnabled)
                {
                    AlohaEvent.ShowTotalVoid(Pr.ToString("0.00"));
                }
                 * */
            }
            catch (Exception e)
            {
                Utils.ToCardLog("[Error] ApplyPaymentEvent" + e);
            }
        }

        static public void ApplyPaymentEvent()
        {
            if (!iniFile.DisplayBoardEnabled) 
            {
                return;
            }
            ThreadPool.QueueUserWorkItem(new WaitCallback(ApplyPaymentEventAsync));
        }
        static public void EventNotification_ENTER_CLOSE_SCREEN()
        {
            if (!iniFile.DisplayBoardEnabled)
            {
                return;
            }
            ThreadPool.QueueUserWorkItem(new WaitCallback(ApplyPaymentEventAsync));
        }


        static private void AddDishEventAsinc(object stateInfo)
        {
            try
            {

                Utils.ToCardLog("AddDishEventAsync ");
                if (iniFile.DisplayBoardEnabled)
                {
                    string Pr = "";
                    double Q = 0;
                    string s = AlohaTSClass.GetDisplayBoardInfo(CurentCheckId, CurentEntryId, out Pr, out Q);
                    Utils.ToCardLog("AddDishEvent " + s);
                    if (Q > 1)
                    {
                        s = s + " x" + Q.ToString();
                        Pr = (Utils.ConvertToDoubleWithRepDemSep(Pr) * Q).ToString();
                    }

                    SendString(s, Pr);
                }
                
            }
            catch (Exception e)
            {
                Utils.ToCardLog("[Error] AddDishEvent" + e);
            }
        }

        static int CurentCheckId = 0;
        static int CurentEntryId = 0;
        static public void AddDishEvent(int CheckId, int EntryId)
        {
            if (!iniFile.DisplayBoardEnabled) 
            {

                return;
            }
            CurentCheckId = CheckId;
            CurentEntryId = EntryId;
            ThreadPool.QueueUserWorkItem(new WaitCallback(AddDishEventAsinc));
        }

        static public void InitDisplayBoard()
        {
            try
            {
                if (!iniFile.DisplayBoardEnabled)
                {
                    return;
                }
                InitDisplay();
                SendString(iniFile.DisplayBoardMessageStr1, iniFile.DisplayBoardMessageStr2);
                Utils.ToCardLog("InitDisplayBoard завершен");
            }
            catch (Exception e)
            {
                Utils.ToCardLog("[Error] InitDisplayBoard" + e);
            }

        }


        static private void InitDisplay()
        {
            try
            {
                InitCom();

                byte[] com = Utils.HexStringToByteArray("1B 40");

                port.Write(com, 0, 2);

                port.Close();
            }
            catch (Exception e)
            {
                Utils.ToCardLog("[Error] InitDisplay" + e);
            }
            finally
            {
                try
                {
                    port.Close();
                }
                catch
                { }
            }
        }


        static private void SendString(string Name, string Price)
        {
            try
            {

                InitCom();
                Utils.ToCardLog("SendString" + Name + " " + Price.ToString());


                if (iniFile.DisplayBoardType == 1)
                {
                    int CodePage = iniFile.DisplayBoardCodePage;


                    byte[] com = Utils.HexStringToByteArray("1B 74 " + iniFile.DisplayBoardSendCodePage.ToString("00"));

                    port.Write(com, 0, 3);

                    byte[] com3 = Utils.HexStringToByteArray("1F 24 00 00 00 00");
                    port.Write(com3, 0, com3.Length);
                    byte[] outbts = Encoding.GetEncoding(CodePage).GetBytes(Name.PadRight(iniFile.DisplayBoardScreenLenght, " "[0]));

                    port.Write(outbts, 0, outbts.Length);
                    byte[] com2 = Utils.HexStringToByteArray("1F 24 00 00 01 00");
                    port.Write(com2, 0, com2.Length);
                    byte[] outbts2 = Encoding.GetEncoding(CodePage).GetBytes("   " + Price.PadRight(iniFile.DisplayBoardScreenLenght - 4, " "[0]));
                    port.Write(outbts2, 0, outbts2.Length);
                    port.Close();
                }
                else if (iniFile.DisplayBoardType == 2)
                {
                    int CodePage = iniFile.DisplayBoardCodePage;


                    byte[] com00 = Utils.HexStringToByteArray("1B 52 00");
                    port.Write(com00, 0, 3);
                    byte[] com = Utils.HexStringToByteArray("1B 74 " + iniFile.DisplayBoardSendCodePage.ToString("00"));

                    port.Write(com, 0, 3);



                    byte[] outbts = Encoding.GetEncoding(CodePage).GetBytes(Name.PadRight(iniFile.DisplayBoardScreenLenght, " "[0]));

                    port.Write(outbts, 0, outbts.Length);
                    byte[] outbts2 = Encoding.GetEncoding(CodePage).GetBytes("   " + Price.PadRight(iniFile.DisplayBoardScreenLenght - 4, " "[0]));

                    port.Write(outbts2, 0, outbts2.Length);
                    port.Close();
                }
                else if (iniFile.DisplayBoardType == 3)
                {
                    int CodePage = iniFile.DisplayBoardCodePage;

                    byte[] com00 = Utils.HexStringToByteArray("1B 3D 02");
                    port.Write(com00, 0, com00.Length);

                    byte[] com1 = Utils.HexStringToByteArray("1B 40");
                    port.Write(com1, 0, com1.Length);


                    byte[] com2 = Utils.HexStringToByteArray("1F 01");
                    port.Write(com00, 0, com00.Length);
                    port.Write(com2, 0, com2.Length);

                    byte[] com3 = Utils.HexStringToByteArray("1B 74 06" + iniFile.DisplayBoardSendCodePage.ToString("00"));
                    port.Write(com00, 0, com00.Length);
                    port.Write(com3, 0, com3.Length);


                    byte[] com4 = Utils.HexStringToByteArray("1F 43 00");
                    port.Write(com00, 0, com00.Length);
                    port.Write(com4, 0, com4.Length);

                    port.Write(com00, 0, com00.Length);
                    port.Write(com1, 0, com1.Length);


                    port.Write(com00, 0, com00.Length);
                    port.Write(com3, 0, com3.Length);






                    byte[] outbts = Encoding.GetEncoding(CodePage).GetBytes(Name.PadRight(iniFile.DisplayBoardScreenLenght, " "[0]));
                    port.Write(com00, 0, com00.Length);
                    port.Write(outbts, 0, outbts.Length);
                    byte[] outbts2 = Encoding.GetEncoding(CodePage).GetBytes("   " + Price.PadRight(iniFile.DisplayBoardScreenLenght - 4, " "[0]));
                    port.Write(com00, 0, com00.Length);
                    port.Write(outbts2, 0, outbts2.Length);
                    port.Close();
                }
            }
            catch (Exception e)
            {
                Utils.ToCardLog("[Error] SendString" + e);
            }
            finally
            {
                try
                {
                    port.Close();
                }
                catch
                { }
            }


        }
        private static void InitCom()
        {
            if (iniFile.DisplayBoardType == 1)
            {
                port.WriteTimeout = 2000;
                port.ReadTimeout = 20000;
                port.BaudRate = iniFile.DisplayBoardPortBaudRate;
                port.PortName = "com" + iniFile.DisplayBoardPort;
                port.NewLine = Environment.NewLine;
                port.DtrEnable = true;
                port.RtsEnable = true;
                port.Parity = Parity.None;
                port.ReadBufferSize = 1024;
                port.WriteBufferSize = 1024;
                port.Handshake = Handshake.None;
                port.Open();
                port.DiscardInBuffer();
                port.DiscardOutBuffer();
            }
            else if ((iniFile.DisplayBoardType == 2) || (iniFile.DisplayBoardType == 3))
            {
                port.WriteTimeout = -1;
                port.ReadTimeout = -1;
                port.BaudRate = iniFile.DisplayBoardPortBaudRate;
                port.PortName = "com" + iniFile.DisplayBoardPort;
                port.NewLine = Environment.NewLine;
                port.DtrEnable = true;
                port.RtsEnable = true;
                port.Parity = Parity.None;
                port.ReadBufferSize = 4096;
                port.WriteBufferSize = 2048;
                port.Handshake = Handshake.None;
                //port.ParityReplace = 20;

                port.Open();
                //port.DiscardInBuffer();
                //port.DiscardOutBuffer();
            }
        }
        private static string FormatStr(string str1, string str2, int Lenght)
        {
            str1 = str1.TrimStart(" "[0]);
            str1 = str1.TrimEnd(" "[0]);
            if (Lenght - (str2.Length + 1) < str1.Length)
            {
                str1.Substring(0, Lenght - (str2.Length + 1));
            }
            int Dist = Lenght - str1.Length - str2.Length;
            string outstr = str1 + new String(" "[0], Dist) + str2;
            return outstr;

        }

    }
}
