using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
namespace FCCIntegration
{
    class Utils
    {

        private static string LogPaths = @"C:\aloha\check\FCC\Log\";

        public static void LogDirCreate()
        {
            try
            {
                DirectoryInfo di = new DirectoryInfo(LogPaths);
                if (!di.Exists)
                {
                    di.Create();
                }
            }
            catch(Exception e)
            {
                System.Windows.Forms.MessageBox.Show("Ошибка при создании папки логов "+ e.Message);
            }
        }

        public static void EventsToLog(string Mess)
        {
            try
            {
                using (StreamWriter SW = new StreamWriter(LogPaths+ @"FCCEventslog.txt", true))
                {
                    SW.WriteLine(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss ") + Mess);
                }
            }
            catch { }
        }

        public static void ToLog(string Mess)
        {
            try
            {
                using (StreamWriter SW = new StreamWriter(LogPaths+ @"FCClog.txt", true))
                {
                    SW.WriteLine(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss ") + Mess);
                }
            }
            catch
            { }
        }

        public static void ToLogFCCApiLog(string Command)
        {
            ToLogFCCApiLog(Command, false, 0, "", "");
        }
        public static void ToLogFCCApiLog(string Command, bool Response, int Result, string Resultstr, string Error)
        {
            try
            {

                string Mess = String.Format("{0} | {1} | {2} | {3}  | {4} ", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), Command, Result, Resultstr, Error);
                
                using (StreamWriter SW = new StreamWriter(LogPaths +@"FCCApilog.txt", true, Encoding.GetEncoding(866)))
                {
                    SW.WriteLine(Mess );
                }

            }
            catch
            { }
        }

        static string MoneyCountLogPath = LogPaths+ @"FCCMoneyCountlog.txt";
        public static void ToMoneyCountLog(MoneyChangeCommands Command, int InsetSumm)
        {
            ToMoneyCountLog(Command, InsetSumm, 0, 0, 0,0);
        }
        public static void ToMoneyCountLog(MoneyChangeCommands Command, int InsetSumm, int Change, int ChangeHand, int ItogSumm, int CheckNumber)
        {
            try
            {
                string Mess = String.Format("{0} | {1} | {2} | {3}  | {4} | {5} | {6} ", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), Command.ToString(), InsetSumm, Change, ChangeHand, ItogSumm, CheckNumber);
                MainClass2.RaiseOnSendMoneyLog(Mess);
                using (StreamWriter SW = new StreamWriter(MoneyCountLogPath, true, Encoding.GetEncoding(866)))
                {
                    SW.WriteLine(Mess);
                }
            }
            catch
            { }
        }



        static public List<OrderData> GetOrdersFromFile()
        {
            List<OrderData> Tmp = new List<OrderData>();
            List<string> Tmp2 = new List<string>();
            try
            {
                using (StreamReader SW = new StreamReader(MoneyCountLogPath))
                {
                     while (!SW.EndOfStream)
                    
                    {
                        Tmp2.Add(SW.ReadLine());


                        


                    }

                }
                return GetOrders(Tmp2);
            }
            catch
            { }
            return Tmp;

        }


        static public List<OrderData> GetOrders(List<string> res)
        {
            List<OrderData> Tmp = new List<OrderData>();
            try
            {
                //using (StreamReader SW = new StreamReader(MoneyCountLogPath))
                {
                  //  while (!SW.EndOfStream)
                    foreach(string s in res )
                    {
                        //string[] strs = SW.ReadLine().Split("|"[0]);
                        string[] strs = s.Split("|"[0]);
                        MoneyChangeCommands MC = (MoneyChangeCommands)Enum.Parse(typeof(MoneyChangeCommands), strs[1].Trim());

                        if ((MC == MoneyChangeCommands.EndPayment) || (MC == MoneyChangeCommands.CancelPayment) || (MC == MoneyChangeCommands.CasseteInserted) || (MC == MoneyChangeCommands.CasseteRemoved)
                            || (MC == MoneyChangeCommands.CoinMixerInserted) || (MC == MoneyChangeCommands.CoinMixerRemoved) || (MC == MoneyChangeCommands.ReplenishEnd) || (MC == MoneyChangeCommands.ReplenishCancel) || (MC == MoneyChangeCommands.CashIncome)
                        ||(MC == MoneyChangeCommands.Razmen))
                        
                        {

                            OrderData Ord = new OrderData()
                            {
                                dt = Convert.ToDateTime(strs[0]),
                                Summ = Convert.ToDecimal(strs[2]) / 100,
                                Change = Convert.ToDecimal(strs[3]) / 100,
                                HandChange = Convert.ToDecimal(strs[4]) / 100,
                                Itog = Convert.ToDecimal(strs[5]) / 100,
                                AlohaNumber = Convert.ToInt32(strs[6]),
                                Command = MC
                            };

                            Tmp.Add(Ord);
                        }

                    }

                }
                return Tmp.OrderByDescending(a => a.dt).ToList();
            }
            catch
            { }
            return Tmp;

        }

    }
    public enum MoneyChangeCommands
    {
        Replenish, ReplenishCancel, ReplenishEnd, Collect, StartPayment, EndPayment, CancelPayment, CasseteRemoved, CasseteInserted, CoinMixerRemoved, CoinMixerInserted, CashIncome, Razmen
    }

}
