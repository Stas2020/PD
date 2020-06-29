using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CustomerDisplay
{
    public class CustomerCheck
    {
            public int AlohId { set; get; }
            public int Ammount { set; get; }
            public List<CustomerDish> Dishes = new List<CustomerDish>();
    }
        public class CustomerDish
        {
            public string Name { set; get; }
            public decimal Price { set; get; }

        }

    
}
