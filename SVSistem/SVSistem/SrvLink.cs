using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SVSistem
{
    interface SrvLink
    {
        int GetBalanse(string CardNum);
        void Sale(string CardNum);
        void Redemtion(string CardNum, Int32 Summ);
    }
}
