using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization; 

namespace PDiscountCard
{
    [Serializable]
    public  class CardMooverInfo
    {
        [XmlElement]
        public  string Prefix;
        [XmlElement ]
        public string CardNum;
        [XmlElement]
        public int CodSh;
        [XmlElement]
        public string CheckNum;
        [XmlElement]
        public int TermNum;
        [XmlElement]
        public int Count=0;
        [XmlElement]
        public decimal Summ = 0;
        [XmlElement]
        public DateTime CDT;



        public CardMooverInfo()
        { 
            
        }
        internal int DoVizit()
        {
            int s1;
            int s2;
            int s3;
            int s4;
            int s5;

            int s6;
            bool s7;
            //return ToBase.DoVizit2(Prefix, CardNum, CodSh, CheckNum, TermNum, Summ, CDT, true, out s1, out s2, out s3, out s4, out s5, Count);
            try
            {
                MB.MBClient mbClient = new MB.MBClient();
                if (mbClient.UsingMB())
                {
                    return mbClient.GetFrendConvertCodeCardProcessing(null,Prefix, CardNum, out  s1, out s2, out  s3, out s4,out s6, out s7);
                }
                else
                {
                    return -1;
                }
            }
            catch
            {
                return -1;
            }
        }

        internal int DoVizitOld()
        {
            int s1;
            int s2;
            int s3;
            int s4;
            int s5;
            return ToBase.DoVizit2(Prefix, CardNum, CodSh, CheckNum, TermNum, Summ, CDT, true, out s1, out s2, out s3, out s4, out s5, Count);
        }
    }
}
