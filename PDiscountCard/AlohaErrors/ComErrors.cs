using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDiscountCard.AlohaErrors
{
    public static class ComErrors
    {
        public static Dictionary<string, string> ErrDict = new Dictionary<string, string>()
        {
            { "0xC0068021","Блюдо отсутствует в меню" },
            { "0xC0068001","Некорректный терминал" },
            { "0xC0068007","Некорректный user" },

        };
            
        public static string GetErrors( string err)
        {
            if (ErrDict.TryGetValue(err, out string res))
            {
                return res;
            }
            return err;

        }



    }
}
