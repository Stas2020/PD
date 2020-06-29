using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Threading;
using System.Net.Sockets;
using System.IO;

namespace PDiscountCard
{
    static class SendToVideo
    {


        internal static void AddItem(int ChId, int EntryId)
        {
            if (iniFile.SendToVideoEnable)
            {
                try
                {
                    string Mess = "Терм: " + Utils.GetTermNum().ToString() + " Чек: " + Check.GetCheckShortNum(ChId) + " Добавил блюдо: " + AlohaTSClass.GetEntryName(EntryId);
                    Send(Mess);
                }
                catch (Exception e)
                {
                    Utils.ToCardLog("[Error] SendToVideo.AddItem " + e.Message);
                }
            }

        }
        internal static void DeleteItem(int ChId)
        {
            if (iniFile.SendToVideoEnable)
            {
                try
                {
                    string Mess = "Терм: " + Utils.GetTermNum().ToString() + " Чек: " + Check.GetCheckShortNum(ChId) + " Удалил блюдо";
                    Send(Mess);
                }
                catch (Exception e)
                {
                    Utils.ToCardLog("[Error] SendToVideo.DeleteItem " + e.Message);
                }
            }

        }


        internal static void EOD()
        {
            if (iniFile.SendToVideoEnable)
            {
                try
                {
                    string Mess = "Терм: " + Utils.GetTermNum().ToString() + " Закрытие дня";
                    Send(Mess);
                }
                catch (Exception e)
                {
                    Utils.ToCardLog("[Error] SendToVideo.LogIn " + e.Message);
                }
            }

        }

        internal static void LogIn(int EmplId)
        {
            if (iniFile.SendToVideoEnable)
            {
                try
                {
                    string Mess = "Терм: " + Utils.GetTermNum().ToString() + " Сотрудник " +AlohaTSClass.GetWaterName (EmplId ) +"("+EmplId.ToString ()  +") зарегистрировался" ;
                    Send(Mess);
                }
                catch (Exception e)
                {
                    Utils.ToCardLog("[Error] SendToVideo.LogIn " + e.Message);
                }
            }

        }
        internal static void LogOut(int EmplId)
        {
            if (iniFile.SendToVideoEnable)
            {
                try
                {
                    string Mess = "Терм: " + Utils.GetTermNum().ToString() + " Сотрудник " + AlohaTSClass.GetWaterName(EmplId) + "(" + EmplId.ToString() + ") вышел";
                    Send(Mess);
                }
                catch (Exception e)
                {
                    Utils.ToCardLog("[Error] SendToVideo.LogIn " + e.Message);
                }
            }

        }

        internal static void CloseCheck(int ChId)
        {
            if (iniFile.SendToVideoEnable)
            {
                try
                {
                    string Mess = "Терм: " + Utils.GetTermNum().ToString() + " Чек закрыт. Номер: " + Check.GetCheckShortNum(ChId);
                    Send(Mess);
                }
                catch (Exception e)
                {
                    Utils.ToCardLog("[Error] SendToVideo.AddItem " + e.Message);
                }
            }

        }
        internal static void ApplyComp(int ChId, int CompTypeId)
        {
            if (iniFile.SendToVideoEnable)
            {
                try
                {
                    string Mess = "Терм: " + Utils.GetTermNum().ToString() + " Чек: " + Check.GetCheckShortNum(ChId) + " Скидка: " + AlohaTSClass.GetCompName(CompTypeId);
                    Send(Mess);
                }
                catch (Exception e)
                {
                    Utils.ToCardLog("[Error] SendToVideo.ApplyComp " + e.Message);
                }
            }

        }
        internal static void DeleteComp(int ChId, int CompTypeId)
        {
            if (iniFile.SendToVideoEnable)
            {
                try
                {
                    string Mess = "Терм: " + Utils.GetTermNum().ToString() + " Чек: " + Check.GetCheckShortNum(ChId) + " Удалил скидку: " + AlohaTSClass.GetCompName(CompTypeId);
                    Send(Mess);
                }
                catch (Exception e)
                {
                    Utils.ToCardLog("[Error] SendToVideo.ApplyComp " + e.Message);
                }
            }

        }
        internal static void ApplyPayment(int ChId, int PaymentTypeId)
        {
            if (iniFile.SendToVideoEnable)
            {
                try
                {
                    
                    string Mess = "Терм: " + Utils.GetTermNum().ToString() + " Чек: " +  Check.GetCheckShortNum(ChId) + " Оплата: " + AlohaTSClass.GetTenderName(PaymentTypeId);
                    Send(Mess);
                }
                catch (Exception e)
                {
                    Utils.ToCardLog("[Error] SendToVideo.ApplyPayment" + e.Message);
                }
            }

        }






        public static void Send()
        {
            const int CashControlPort = 3000; // порт для входящих на ресивере
            const string CashControlAddress = "192.168.3.98"; //ip ресивера.надеюсь статика.

            TcpClient client = null;
            try
            {

                string message = "Out message";

                client = new TcpClient(CashControlAddress, CashControlPort);
                NetworkStream stream = client.GetStream();
                StreamWriter writer = new StreamWriter(stream);
                writer.WriteLine(message); // отправляем сообщение
                writer.Flush();

                writer.Close();
                stream.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message); // вывод ошибки
            }
            finally
            {
                if (client != null)
                {
                    client.Close();
                }
            }
        }


        private static void SendAsinc(string Mess)
        {
            try
            {
                Utils.ToCardLog("SendToVideo.Send Start iniFile.SendToVideoIP = " + iniFile.SendToVideoIP + " Порт " + iniFile.SendToVideoPort + " Сообщение: " + Mess);
                


                if (iniFile.SendToVideoEnglish) {
                    var tr = new Transliter();
                    Mess = tr.GetTranslit(Mess);
                }

                if (iniFile.SendToVideoTCP)
                {
                    TcpClient client = null;
                    try
                    {


                        client = new TcpClient(iniFile.SendToVideoIP, Convert.ToInt32(iniFile.SendToVideoPort));
                        NetworkStream stream = client.GetStream();
                        StreamWriter writer = new StreamWriter(stream);
                        writer.WriteLine(Mess); // отправляем сообщение
                        writer.Flush();

                        writer.Close();
                        stream.Close();
                        Utils.ToCardLog("SendToVideo.Send TCP Отправил IP= " + iniFile.SendToVideoIP + " Порт " + iniFile.SendToVideoPort + " Сообщение: " + Mess);
                    }
                    catch (Exception ex)
                    {
                        Utils.ToCardLog("SendToVideo.Send TCP Error " + ex.Message);
                    }
                    finally
                    {
                        if (client != null)
                        {
                            client.Close();
                        }
                    }
                }
                else
                {
                    IPAddress ipaddress = IPAddress.Parse(iniFile.SendToVideoIP);
                    IPEndPoint ipendpoint = new IPEndPoint(ipaddress, iniFile.SendToVideoPort);
                    UdpClient Uc = new UdpClient();
                    byte[] message = Encoding.Default.GetBytes(Mess);

                    int sended = Uc.Send(message, message.Length, ipendpoint);

                    Uc.Close();
                    Utils.ToCardLog("SendToVideo.Send Отправил IP= " + iniFile.SendToVideoIP + " Порт " + iniFile.SendToVideoPort + " Сообщение: " + Mess);
                }
              
            }
            catch (Exception e)
            {
                Utils.ToCardLog("[Error] SendToVideo.Send " + e.Message);
            }
        }
        private  static void Send(string Mess)
        {
            System.Threading.Thread SendThresd =
                 new System.Threading.Thread(delegate() { SendAsinc(Mess); });
            SendThresd.Start();
            
        }
    }
    public  class Transliter
    {
         Dictionary<string, string> words = new Dictionary<string, string>();
         void Init()
        {
            words.Clear();
            words.Add("а", "a");
            words.Add("б", "b");
            words.Add("в", "v");
            words.Add("г", "g");
            words.Add("д", "d");
            words.Add("е", "e");
            words.Add("ё", "yo");
            words.Add("ж", "zh");
            words.Add("з", "z");
            words.Add("и", "i");
            words.Add("й", "j");
            words.Add("к", "k");
            words.Add("л", "l");
            words.Add("м", "m");
            words.Add("н", "n");
            words.Add("о", "o");
            words.Add("п", "p");
            words.Add("р", "r");
            words.Add("с", "s");
            words.Add("т", "t");
            words.Add("у", "u");
            words.Add("ф", "f");
            words.Add("х", "h");
            words.Add("ц", "c");
            words.Add("ч", "ch");
            words.Add("ш", "sh");
            words.Add("щ", "sch");
            words.Add("ъ", "j");
            words.Add("ы", "i");
            words.Add("ь", "j");
            words.Add("э", "e");
            words.Add("ю", "yu");
            words.Add("я", "ya");
            words.Add("А", "A");
            words.Add("Б", "B");
            words.Add("В", "V");
            words.Add("Г", "G");
            words.Add("Д", "D");
            words.Add("Е", "E");
            words.Add("Ё", "Yo");
            words.Add("Ж", "Zh");
            words.Add("З", "Z");
            words.Add("И", "I");
            words.Add("Й", "J");
            words.Add("К", "K");
            words.Add("Л", "L");
            words.Add("М", "M");
            words.Add("Н", "N");
            words.Add("О", "O");
            words.Add("П", "P");
            words.Add("Р", "R");
            words.Add("С", "S");
            words.Add("Т", "T");
            words.Add("У", "U");
            words.Add("Ф", "F");
            words.Add("Х", "H");
            words.Add("Ц", "C");
            words.Add("Ч", "Ch");
            words.Add("Ш", "Sh");
            words.Add("Щ", "Sch");
            words.Add("Ъ", "J");
            words.Add("Ы", "I");
            words.Add("Ь", "J");
            words.Add("Э", "E");
            words.Add("Ю", "Yu");
            words.Add("Я", "Ya");
        }

       public   string GetTranslit(string source)
        {

            Init();
            foreach (KeyValuePair<string, string> pair in words)
            {
                source = source.Replace(pair.Key, pair.Value);
            }
            return source;
        }


    }
}
