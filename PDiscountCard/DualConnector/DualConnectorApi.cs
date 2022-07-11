using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DualConnector;


namespace PDiscountCard.DualConnectorIntegration
{
    class DualConnectorApi
    {

        public delegate void OnExchangeEventHandler(object sender, int Code, string CodeDescr, ISAPacket response);
        public event OnExchangeEventHandler OnExchange;

        DCLink dclink;
        ISAPacket query = new SAPacket();
        ISAPacket response = new SAPacket();
        int res;
        bool asyncReturned = false;
        int threadDuratioMaxMin = 30;
        void dclink_OnExchange(int _res)
        {
            //Console.WriteLine("Inpas dclink_Exchange res:" + res);
            Utils.ToCardLog(String.Format("Inpas [dclink_Exchange(int res)] res: {0}", _res));
            res = _res;
            asyncReturned = true;
        }
        // private DualConnectorMain.OnExchangeComplitedFromApiEventHendler ExchangeComplitedCallback;
        public int Exchange(int OperId, int CommandMode, int CommandMode2, int Amount, string Terminal, string RRN, out String ResStr, out string receipt, out int status, out string statusDescr)
        {
            status = 0;
            receipt = "";
            ResStr = "";
            statusDescr = "";
            try
            {
                Utils.ToCardLog(String.Format("Inpas Exchange OperId = {0}, CommandMode = {1}, CommandMode2= {2}, Amount={3}, Terminal={4}, string RRN={5}", OperId, CommandMode, CommandMode2, Amount, Terminal, RRN));
                
                //ExchangeComplitedCallback = callback;
                dclink = new DCLink();

                // dclink.OnExchange += new OnExchangeHandler(dclink_OnExchange);

                query.Amount =Math.Abs(Amount).ToString();
                query.CurrencyCode = "643";
                query.CardEntryMode = 3;

                query.TerminalDateTime = DateTime.Now.ToString("yyyyMMddHHmmss");
                if ((OperId == 4) || (OperId == 29))
                {
                    query.OrigOperation = 1; // 29 - если отменяем возврат
                    query.AuthorizationCode = RRN;
                    //query.ReferenceNumber = RRN;


                }
                query.OperationCode = OperId;
                query.CommandMode2 = CommandMode2;
                query.CommandMode = CommandMode;

                //query.TerminalID = "40000112";
                query.TerminalID = Terminal.ToString();
                
                dclink.OnExchange += new DualConnector.OnExchangeHandler(dclink_OnExchange);

                Utils.ToCardLog("dclink.InitResources");
                res = dclink.InitResources();

                if (res != 0)
                {
                    Utils.ToCardLog(string.Format("Init resource:{0}-{1}", res, dclink.ErrorDescription));
                }
                /*
                if (res != 0)
                {
                    Utils.ToCardLog("dclink.InitResources res  = "+res +" " + GetErrCodeDescr(res));
                    ResStr = GetErrCodeDescr(res);
                    return res;
                }
                if (query.TerminalID == "0")
                {
                    dclink.Exchange(ref query, ref response, 18000);
                }
                */
                //Utils.ToCardLog(String.Format("Inpas Exchange Amount: {0}, OperationCode: {1}, CommandMode2: {2} ", query.Amount, query.OperationCode, query.CommandMode2));

                //Utils.ToCardLog("Inpas timeout:" + (iniFile.CreditTerminalTimeout * 1000).ToString());
                Utils.ToCardLog("Inpas Exchange query:" + Environment.NewLine + query.ToStringExt());
                //res = dclink.Exchange(ref query, ref response, iniFile.CreditTerminalTimeout * 1000); TimeOut Always Zero
                res = dclink.Exchange(ref query, ref response, 0);

                DateTime exchangeStart = DateTime.Now;
                while (!asyncReturned)
                {
                    System.Threading.Thread.Sleep(250);
                    if((DateTime.Now - exchangeStart).TotalMinutes > threadDuratioMaxMin)
                    //if ((DateTime.Now - exchangeStart).TotalSeconds > threadDuratioMaxMin)
                    {
                        Utils.ToCardLog(string.Format("!!! ПРОЦЕСС ОБРАБОТКИ КАРТ АКТИВЕН БОЛЕЕ {0} МИНУТ !!!", threadDuratioMaxMin));
                        //Utils.ToCardLog(string.Format("!!! ПРОЦЕСС ОБРАБОТКИ КАРТ АКТИВЕН БОЛЕЕ {0} СЕКУНД !!!", threadDuratioMaxMin));
                        exchangeStart = DateTime.Now;
                    }
                }

                Utils.ToCardLog("Inpas Exchange end res = " + res);
                if (response != null)
                {
                    status = response.Status;
                    statusDescr = GetStatusDescr(status);
                    Utils.ToCardLog("Inpas Status :"+ status +" (" +statusDescr+")");
                    Utils.ToCardLog("Inpas response:" + Environment.NewLine + response.ToStringExt());
                    
                }
                else
                {
                    Utils.ToCardLog("Inpas Exchange response=null" );
                }
                
                if (response != null)
                {
                    receipt = response.ReceiptData;
                }
                ResStr = GetErrCodeDescr(res);
                
                dclink.FreeResources();
                dclink.Dispose();
                dclink = null;
                return res;
            }
            catch(Exception e)
            {
                Utils.ToCardLog("Error Inpas Exchange :" + e.Message);
                ResStr = e.Message;
                return -1;
            }

        }





        /*
        void dclink_OnExchange(int result)
        {
            //response.Status



            
            ExchangeComplitedCallback(result, GetErrCodeDescr(result), response);
          
            try
            {
                dclink.FreeResources();
                dclink.Dispose();
                dclink = null;
            }
            catch
            { 
                

            }
            //GC.Collect();
                 
        }
         * */
        internal const int Err_OK = 0;
        internal const int Err_TIMEOUT = 1;// - истёк таймаут операции;
        internal const int Err_LOG_ERROR = 2;// - ошибка создания LOG файла;
        internal const int Err_SYSTEM_ERROR = 3;// - общая ошибка;
        internal const int Err_REQUEST_ERROR = 4;// - ошибка данных запроса;
        internal const int Err_CONFIG_NOT_FOUND = 6;// - не найден файл конфигурации;
        internal const int Err_CONFIG_ERROR_FORMAT = 7;// - ошибка формата файла конфигурации;
        internal const int Err_CONFIG_ERROR_LOG = 8;// - ошибка параметров логирования;
        internal const int Err_CONFIG_ERROR_DEVICES = 9;// - ошибка в параметрах терминала;
        internal const int Err_CONFIG_ERROR_DUBLCOMPORTS = 10;// - ошибка настройки устройства на COM порт
        internal const int Err_CONFIG_ERROR_OUTPUT = 11;// - ошибка в выходных параметрах;
        internal const int Err_PRINT_ERROR = 12;// - ошибка при передаче образа чека;
        internal const int Err_ERROR_CONNECT = 13;// - ошибка установки связи с устройством;
        internal const int Err_CONFIG_ERROR_GUI = 14;// - ошибка в параметрах настройки интерфейса

        static string GetErrCodeDescr(int Code)
        {
            Dictionary<int, string> ResultDict = new Dictionary<int, string>();

            ResultDict.Add(Err_OK, "ошибок нет");
            ResultDict.Add(Err_TIMEOUT, "истёк таймаут операции");
            ResultDict.Add(Err_LOG_ERROR, " ошибка создания LOG файла");
            ResultDict.Add(Err_SYSTEM_ERROR, "общая ошибка");
            ResultDict.Add(Err_REQUEST_ERROR, " ошибка данных запроса");
            ResultDict.Add(Err_CONFIG_NOT_FOUND, "не найден файл конфигурации");
            ResultDict.Add(Err_CONFIG_ERROR_FORMAT, "ошибка формата файла конфигурации");
            ResultDict.Add(Err_CONFIG_ERROR_LOG, "ошибка параметров логирования");
            ResultDict.Add(Err_CONFIG_ERROR_DEVICES, "ошибка в параметрах терминала");
            ResultDict.Add(Err_CONFIG_ERROR_DUBLCOMPORTS, "ошибка настройки устройства на COM порт");
            ResultDict.Add(Err_CONFIG_ERROR_OUTPUT, "ошибка в выходных параметрах");
            ResultDict.Add(Err_PRINT_ERROR, "ошибка при передаче образа чека");
            ResultDict.Add(Err_ERROR_CONNECT, "ошибка установки связи с устройством");
            ResultDict.Add(Err_CONFIG_ERROR_GUI, " ошибка в параметрах настройки интерфейса  ");


            string res = "";
            if (!ResultDict.TryGetValue(Code, out res))
            {
                res = "Неизвестная ошибка";
            }
            return res;



        }

        static string GetStatusDescr(int Code)
        { 
             Dictionary<int, string> ResultDict = new Dictionary<int, string>();

            ResultDict.Add(0, "Неопределённый статус");
            ResultDict.Add(1, "Одобрено");
            ResultDict.Add(16, "Отказано");
            ResultDict.Add(17, "Выполнено в OFFLINE");
            ResultDict.Add(34, "Нет соединения");
            ResultDict.Add(53, "Операция прервана");
            


            string res = "";
            if (!ResultDict.TryGetValue(Code, out res))
            {
                res = "Неизвестно";
            }
            return res;

        }

    }
    public static class DualConnectorExtensions
    {
        public static string ToStringExt(this ISAPacket pack)
        {
            var s = "";
            foreach (var p in pack.GetType().GetProperties())
            {
                s += Environment.NewLine + p.Name + ": " + p.GetValue(pack, null);
            }
            return s;
        }
    }

}
