using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FCCIntegration
{
    public class FCCCheck
    {
        public int AlohId { set; get; }
        public int AlohNumber { set; get; }
        public int Ammount { set; get; }
        public List<FCCDish> Dishes = new List<FCCDish>();
        public bool AllChkOnTable = false;
        public int TableId { set; get; }

        public int RoundedAmount
        {
            get
            {
                int NotRounded = Ammount % 100;
                if (NotRounded > 0)
                {

                    return Ammount - NotRounded;
                }
                else
                {
                    return Ammount;
                }
            }
        }
    }
    public class FCCDish
    {
        public string Name { set; get; }
        public decimal Price { set; get; }
        public int AlohaCheckNum { set; get; }
    }
}
