using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Threading;


namespace PDiscountCard
{
   public  class PScale
    {
        static System.IO.Ports.SerialPort port = new System.IO.Ports.SerialPort();

       /*
        static public int ScaleWeight()
        { 
            string  ErrStr="";
            int Comnum = iniFile.ScalePort; 
            bool b = Connect(Comnum, out ErrStr);
            if (!b)
            {
                ScaleFrm Sf = new ScaleFrm();
                Sf.SetMsg("Невозможно открыть порт Com" + Comnum + Environment.NewLine + ErrStr);
                Sf.ShowDialog  ();
                return -1;
            }
            else
            {

                bool Compl = WeightCompleted(out ErrStr);
                if (ErrStr != "")
                {
                    ScaleFrm Sf = new ScaleFrm();
                    Sf.SetMsg("Невозможно соединится с весами порт Com" + Comnum + Environment.NewLine + ErrStr);
                    Sf.ShowDialog();
                    return -1;
                }
                else
                { 
                    int Pcount =0;
                    while (!Compl)
                    {
                        Thread.Sleep(100);
                        Pcount++;
                        Compl = WeightCompleted(out ErrStr);
                        if (Pcount > 5)
                        {

                            ScaleFrm Sf = new ScaleFrm();
                            if (ErrStr != "")
                            {
                                Sf.SetMsg("Нет связи с весами порт Com" + Comnum);
                            }
                            else
                            {
                                Sf.SetMsg("Не могу выставить точный вес на весах порт Com" + Comnum);
                            }
                            Sf.ShowDialog();
                            return -1;
                        }
                    }

                    int W = GetWeight();
                    if (W == -1)
                    {
                        ScaleFrm Sf = new ScaleFrm();
                        Sf.SetMsg("Ошибка взвешивания на весах порт Com" + Comnum);
                        
                        Sf.ShowDialog();
                        return -1;
                    }
                    else
                    {
                        return W ; 
                    }
                }
            }
        }
       */
        static public bool Connect(int Comnum, out string  ErrStr)
        {
            
            ErrStr ="";
            try
            {
                if (iniFile.ScaleType == 1)
                {
                    port.WriteTimeout = 1000;
                    port.ReadTimeout = 1000;
                    port.BaudRate = 4800;
                    port.PortName = "com" + Comnum;
                    port.NewLine = Environment.NewLine;
                    port.DtrEnable = true;
                    port.RtsEnable = true;
                    port.Parity = Parity.Even;
                    port.ReadBufferSize = 1024;
                    port.WriteBufferSize = 1024;
                    port.Handshake = Handshake.None;
                    port.StopBits = StopBits.One;
                    port.Open();

                
                }
                else if (iniFile.ScaleType == 2)
                {
                    ScaleCasAD.Connect();
                
                }
                return true;
            }
            catch (Exception e)
            {
                ErrStr = e.Message;
                return false;
            }
        }

        static public void DisConnect()
        {
            try
            {
                if (iniFile.ScaleType == 1)
                {
                    port.Close();
                }
                 else if (iniFile.ScaleType == 2)
                {
                    ScaleCasAD.DisConnect();
                }
            }
            
            catch
            { }
        }

        static public int GetWeight()
        {
            try
            {
                int W = 0;
                if (iniFile.ScaleType == 1)
                {
                    port.DiscardInBuffer();
                    port.DiscardOutBuffer();
                    byte[] com = HexStringToByteArray("45");
                    port.Write(com, 0, 1);
                    int k = (port.Read(com, 0, 1));
                    BitArray Ba = new BitArray(com);

                    int k2 = (port.Read(com, 0, 1));
                    BitArray Ba2 = new BitArray(com);

                    BitArray ba3 = new BitArray(16);

                    for (int i = 0; i < 16; i++)
                    {
                        if (i < 8)
                        {
                            ba3.Set(i, Ba[i]);
                        }
                        else
                        {
                            ba3.Set(i, Ba2[i - 8]);
                        }
                    }


                    W = getIntFromBitArray(ba3);

                    
                }
                else if (iniFile.ScaleType == 2)
                {
                    W=ScaleCasAD.GetWeight();
                }
                return W;
            }
            catch(Exception e)
            {
                Utils.ToCardLog("[Error] GetWeight "+e.Message );
                return -1;
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


        static private string GetBitStr(BitArray bt)
        {
            string str = "";
            foreach (bool b in bt)
            {
                if (b)
                {
                    str += " 1";
                }
                else
                {
                    str += " 0";
                }

            }
            return str;
        }
        static public bool WeightCompleted(out string  ErrMess)
        {
            ErrMess = "";
            try

            {
                bool res = false;
                if (iniFile.ScaleType == 1)
                {

                    port.DiscardInBuffer();
                    port.DiscardOutBuffer();
                    byte[] com = HexStringToByteArray("44");

                    port.Write(com, 0, 1);

                    int k = (port.Read(com, 0, 1));
                    BitArray Ba = new BitArray(com);

                    res= Ba[7];
                }
                else if (iniFile.ScaleType == 2)
                {
                    res= ScaleCasAD.Stable();
                }
                return res;


            }
            catch(Exception e)
            {
                Utils.ToCardLog("[Error] WeightCompleted " + e.Message);
                ErrMess = e.Message;
                return false;
            }




        }



        static private byte[] HexStringToByteArray(string s)
        {
            s = s.Replace(" ", "");
            byte[] buffer = new byte[s.Length / 2];
            for (int i = 0; i < s.Length; i += 2)
                buffer[i / 2] = (byte)Convert.ToByte(s.Substring(i, 2), 16);
            return buffer;
        }

    }
}
