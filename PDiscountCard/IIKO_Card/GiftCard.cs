using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDiscountCard.IIKO_Card
{
    class GiftCard
    {
        private string card_code;
        private DateTime dt_create;
        private int num_shop;
        private double balance;

        public GiftCard(string card_code_, DateTime dt_create_, int num_shop_, double balance_)
        {
            card_code = card_code_;
            dt_create = dt_create_;
            num_shop = num_shop_;
            balance = balance_;

        }
        public string CardCode   
        {
            get { return card_code; }            
        }
        public DateTime DTCreate
        {
            get { return dt_create; }
        }

        public int NumShop
        {
            get { return num_shop; }
        }

        public double Balance
        {
            get { return balance; }
        }
    }
}
