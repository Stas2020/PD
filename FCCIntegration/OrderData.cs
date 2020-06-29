using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FCCIntegration
{
    public class OrderData
    {
        public OrderData()
        {

        }

        internal DateTime dt { set; get; }

        internal MoneyChangeCommands Command { set; get; }

        public string dtStr
        {
            get
            {
                return dt.ToString("dd/MM/yyyy HH:mm:ss");
            }
        }
        
        public string Status 
        {
             
             get
             {
                 switch (Command)
                        {
                            case MoneyChangeCommands.EndPayment:
                                return "Оплачен";
                                break;
                            case MoneyChangeCommands.CancelPayment:
                                return "Отменен";
                                break;
                            case MoneyChangeCommands.CasseteRemoved:
                                return "Кассета удалена";
                                break;
                            case MoneyChangeCommands.CasseteInserted:
                                return "Кассета вставлена";
                                break;
                            case MoneyChangeCommands.CoinMixerInserted:
                                return "Монетница вставлена";
                                break;
                            case MoneyChangeCommands.CoinMixerRemoved:
                                return "Монетница удалена";
                                break;
                            case MoneyChangeCommands.ReplenishEnd:
                                return "Внесение";
                                break;
                            case MoneyChangeCommands.ReplenishCancel:
                                return "Внесение отменено";
                                break;
                            case MoneyChangeCommands.CashIncome:
                                return "Внес. в ден. ящик";
                                break;
                           case MoneyChangeCommands.Razmen:
                                return "Размен";
                                break;
                            default:
                                break;
                        }
                 return "";
             }
        }

        public decimal Itog { set; get; }
            /*
        {
            
            get
            {
                return Summ - Change;
            }
        }
             * */
        public decimal Summ { set; get; }
        public decimal Change { set; get; }
        public decimal HandChange { set; get; }


        public int AlohaNumber { set; get; }

    }
}
