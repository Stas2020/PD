using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Collections;
using System.Threading;

namespace PDiscountCard.Scale
{
    class ScaleCasSWN : IScale
    {
        System.IO.Ports.SerialPort port = new System.IO.Ports.SerialPort();

        public bool Connect()
        {
            port.WriteTimeout = 2000;
            port.ReadTimeout = 2000;
            port.BaudRate = iniFile.ScaleBaudRate;
            port.PortName = "com" + iniFile.ScalePort.ToString();
            port.NewLine = Environment.NewLine;
            port.DtrEnable = true;
            port.RtsEnable = true;
            port.Parity = Parity.None;
            port.ReadBufferSize = 1024;
            port.WriteBufferSize = 1024;
            port.Handshake = Handshake.None;
            port.StopBits = StopBits.One;

            try
            {
                port.Open();
                return port.IsOpen;
            }
            catch (Exception e)
            {
                Utils.ToCardLog("Error ScaleCasSWN Connect " + e.Message);
                return false;
            }

            //throw new NotImplementedException();
        }


        public void Disconnect()
        {
            try
            {
                port.Close();
            }
            catch (Exception e)
            {
                Utils.ToCardLog("Error ScaleCasSWN Disconnect " + e.Message);
            }
        }

        private static int getIntFromBitArray(BitArray bitArray)
        {

            if (bitArray.Length > 32)
                throw new ArgumentException("Argument length shall be at most 32 bits.");

            int[] array = new int[1];
            bitArray.CopyTo(array, 0);
            return array[0];

        }

        static private byte[] HexStringToByteArray(string s)
        {
            s = s.Replace(" ", "");
            byte[] buffer = new byte[s.Length / 2];
            for (int i = 0; i < s.Length; i += 2)
                buffer[i / 2] = (byte)Convert.ToByte(s.Substring(i, 2), 16);
            return buffer;
        }




        public int GetWeight(out double Weight, out bool Stable , out string ErrMess)
        {
            Weight = 0;
            ErrMess = "";
            Stable = true;
            try
            {
                
                Utils.ToLog("GetWeight ");
                port.DiscardInBuffer();
                port.DiscardOutBuffer();
                byte[] com = HexStringToByteArray("05");
                port.Write(com, 0, 1);
                Utils.ToLog("GetWeight Send 05");

                byte[] com2 = new byte[16];
                int k = (port.Read(com2, 0, 2));
                if (com2[0] != 6)
                {
                    Utils.ToLog("GetWeight Неверный ответ на инит запрос Get " + com2[0].ToString());
                    ErrMess = "Некорректный ответ от весов";
                    return -2; //Неверный ответ на инит запрос
                }

                com = HexStringToByteArray("11");
                port.Write(com, 0, 1);
                Thread.Sleep(500);
                Utils.ToLog("GetWeight Send 11");
                k = (port.Read(com2, 0, 15));
                string Prt = "";
                for (int j = 0; j < 15; j++)
                {
                    Prt += com2[j].ToString() + " ";
                }
                    Utils.ToLog("Answer " +Prt);

                if ((com2[0] != 1) || (com2[1] != 2))
                {
                    Utils.ToLog("GetWeight Неверный ответ на инит запрос 2 Get " + com2.ToString());
                    ErrMess = "Некорректный ответ от весов";
                    return -3; //Неверный ответ на инит запрос 2 
                }
                if (com2[2] != 0x53)
                {
                    Stable = false;
                    //Utils.ToLog("GetWeight Весы нестабильны " + com2.ToString());
                    //return -4; //Весы нестабильны 
                }
                int Plus = 1;
                if (com2[3] != 0x20)
                {
                    //Utils.ToLog("GetWeight Вес отрицателен " + com2.ToString());
                    Plus = -1;
                    //return -5; //Вес отрицателен 
                }
                string res = "";
                for (int i = 0; i < 6; i++)
                {
                    Utils.ToLog("sc res = "+res);
                    res += Convert.ToChar(com2[i + 4]).ToString();
                }
                Weight = Convert.ToDouble(ReplDemSep(res)) * Plus;
                return 0;

            }
            catch (Exception e)
            {
                Utils.ToLog("Error GetWeight " + e.Message);
                ErrMess = "Ошибка " +e.Message;
                return -1;
            }

           
        }
        private string ReplDemSep(string StrIn)
        {
            string depSep = (1.1).ToString()[1].ToString();
            return StrIn.Replace(",", depSep).Replace(".", depSep);
        }
    }
}
