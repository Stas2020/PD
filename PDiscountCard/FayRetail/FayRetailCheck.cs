using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDiscountCard.FayRetail
{
    public  class FayRetailCheckInfo
    {
        public List<ChequeLine> Items { set; get; }
        public string ChequeNumber { set; get; }
        public DateTime ChequeDate { set; get; }
        
        public List<Pay> Pays { set; get; }
        public string PurchaseID
        {
            get
            {
                return AlohainiFile.BDate.ToString("ddMMyyyy") + ChequeNumber;
            }
        }

        public double TotalSumm
        {
            get
            { 
                if ((Items!=null)&&(Items.Count>0))
                {
                    return Items.Sum(a => a.Amount);
                }
                return 0;
            }
        }


    }
}
