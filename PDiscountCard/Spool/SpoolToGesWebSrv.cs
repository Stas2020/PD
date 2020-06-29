using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Xml;
using System.IO;

namespace PDiscountCard.Spool
{

    
    public class SpoolToGesWebSrv
    {

        public int Cod_Podr = 0;
        public DateTime Dt_Check = new DateTime();

        public string Cod_Check = "";
        public int Cod_Manager = 0;
        public int Id_Discnt = 0;
        public int Cod_Table = 0;
        public DateTime Dt_Open = new DateTime();

        public int PredCheck = 0;
        public int Sum_Check = 0;
        public int Id_Paymnt = 0;
        public string Person_name = "";
        public int Id_Info = 0;
        public string T_goodsRow = "";
        public Check mChk = null;
        public SpoolToGesWebSrv(Check Chk)
        {
            mChk = Chk;
            //Cod_Podr = AlohainiFile.DepNum;
            Cod_Podr = iniFile.SpoolDepNum;
            Dt_Check = Chk.SystemDate;
            Cod_Check = Chk.CheckNum;
            Cod_Manager = Chk.Waiter;
            Id_Discnt = Chk.CompId;
            Cod_Table = Chk.TableNumber;
            Dt_Open = Chk.SystemDateOfOpen;
            PredCheck = Chk.PredcheckCount;
            Sum_Check = (int)(Chk.Summ * 100);

            //Id_Paymnt = Chk.Tenders[0].TenderId;


            //Id_Paymnt = Chk.IsNal ? 1 : 20; 

            Person_name = SpoolCreator.GetCompNameSpoolCaption(Chk);

            if ((Id_Discnt >= 10) && (Sum_Check == 0))
            {
                Cod_Manager = Chk.DegustationMGR_NUMBER;
            }

        }


        public string GetDishOrderTimeXMLList2(Check Chk)
        {
            string Tmp = "";
            Tmp += "<?xml version=\"1.0\" encoding=\"windows-1251\"?> <SOAP-ENV:Envelope xmlns:SOAP-ENV=\"";
            Tmp += @"http://schemas.xmlsoap.org/soap/envelope/";
            Tmp += "\" xmlns:SOAP-ENC=\"/http://schemas.xmlsoap.org/soap/encoding/\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">";
            Tmp += "<SOAP-ENV:Body>";
            Tmp += "<m:AlohaOrderTime xmlns:m=\"urn:coffeemania:AlohaOrderTime:AlohaOrderTime\">";

            foreach (Dish d in Chk.Dishez)
            {
                try
                {
                    Tmp += GetOtXml(GetDishOrderTimeXML(Chk, d.BarCode, SQL.ToSql.GetOrderTime(d.BarCode, Chk.AlohaCheckNum, d.AlohaNum, Chk.SystemDateOfOpen, Chk.Summ > 0, Chk.Waiter), (int)(Math.Abs(d.Priceone) * 100), d.Id));
                }
                catch (Exception e)
                {
                    Utils.ToLog("Error GetGetDishOrderTimeXMLList " + e.Message);
                }



            }
            Tmp += @"</m:AlohaOrderTime>";
            Tmp += @"</SOAP-ENV:Body>";
            Tmp += @"</SOAP-ENV:Envelope>";
            return Tmp;
        }


        public string GetOtXml(OrderTimeNote data)
        {
            string Tmp = "";
          
            Tmp += @"<m:OrderTimeNote>";
            Tmp += @"<m:OrderTime>" + data.OrderTime.ToString("yyyy-MM-ddTHH:mm:ss.ffffffzzz") + "</m:OrderTime>";
            Tmp += @"<m:Dep>" + data.Dep + "</m:Dep>";
            Tmp += @"<m:ChkNumber>" + data.ChkNumber + "</m:ChkNumber>";
            Tmp += @"<m:Barcode>" + data.Barcode + "</m:Barcode>";
            Tmp += @"<m:Price>" + (((double)data.Price)/100).ToString() + "</m:Price>";
            Tmp += @"<m:Id>" + data.Id.ToString() + "</m:Id>";

            Tmp += @"</m:OrderTimeNote>";
          
            return Tmp;
        }


        public string GetXmlStr2()
        {
            string Tmp = "";
            Tmp += "<?xml version=\"1.0\" encoding=\"windows-1251\"?> <SOAP-ENV:Envelope xmlns:SOAP-ENV=\"";
            Tmp += @"http://schemas.xmlsoap.org/soap/envelope/";
            Tmp += "\" xmlns:SOAP-ENC=\"/http://schemas.xmlsoap.org/soap/encoding/\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">";
            Tmp += "<SOAP-ENV:Body>";
            Tmp += "<m:AlohaReceiptsV3 xmlns:m=\"urn:coffeemania:AlohaReceipts:AlohaReceiptsService\">";

            Tmp += @"<m:Cod_Podr>" + Cod_Podr.ToString() + @"</m:Cod_Podr>";
            Tmp += @"<m:Dt_Check>" + Dt_Check.ToString("yyyy-MM-dd") + @"</m:Dt_Check>";
            Tmp += @"<m:Tm_Check>" + Dt_Check.ToString("HH:mm") + @"</m:Tm_Check>";
            Tmp += @"<m:Cod_Check>" + Cod_Check + @"</m:Cod_Check>";
            Tmp += @"<m:Cod_Manag>" + Cod_Manager.ToString() + @"</m:Cod_Manag>";
            Tmp += @"<m:Id_Discnt>" + Id_Discnt.ToString() + @"</m:Id_Discnt>";
            Tmp += @"<m:Cod_Table>" + Cod_Table.ToString() + @"</m:Cod_Table>";
            Tmp += @"<m:Dt_Open>" + Dt_Open.ToString("yyyy-MM-dd") + @"</m:Dt_Open>";
            Tmp += @"<m:Tm_Open>" + Dt_Open.ToString("HH:mm") + @"</m:Tm_Open>";
            Tmp += @"<m:PredCheck>" + PredCheck.ToString() + @"</m:PredCheck>";
            //Tmp += @"<m:Sum_Check>" + Sum_Check.ToString() + @"</m:Sum_Check>";
            //Tmp += @"<m:Id_Paymnt>" + Id_Paymnt.ToString() + @"</m:Id_Paymnt>";
            Tmp += @"<m:Person_name>" + Person_name + @"</m:Person_name>";
            Tmp += @"<m:Id_Info>" + mChk.Guests+ @"</m:Id_Info>"; //Кол-во гостей
            Tmp += @"<m:KKMnumber>" + Convert.ToInt32(mChk.KkmNum) + @"</m:KKMnumber>";
            Tmp += @"<m:KLZnumber>" + mChk.EKLZNumInt.ToString() + @"</m:KLZnumber>";
            Tmp += @"<m:CheckType>" + mChk.RealOpenTimem + @"</m:CheckType>";
            Tmp += @"<m:T_goodsRow>";
            foreach (Dish d in mChk.ConSolidateSpoolDishez)
            {

                T_goodsRow Tr = new T_goodsRow(d, mChk.Vozvr);
                Tmp += Tr.GetXmlStr();
            }
            Tmp += @"</m:T_goodsRow>";
            Tmp += @"<m:T_cash>";
            foreach (AlohaTender AT in mChk.Tenders)
            {
                Tmp += @"<m:T_cashRow>";
                Tmp += @"<m:Id_paym>" + AT.TenderId.ToString() + @"</m:Id_paym>";
                Tmp += @"<m:Sum_paym>" + (AT.Summ * 100).ToString() + @"</m:Sum_paym>";
                if (AT.CardPrefix == "")
                {
                    Tmp += @"<m:prefix/>";
                }
                else
                {
                    Tmp += @"<m:prefix>" + AT.CardPrefix + @"</m:prefix>";
                }

                if (AT.CardNumber == "")
                {
                    Tmp += @"<m:cardNumb/>";
                }
                else
                {
                    Tmp += @"<m:cardNumb>" + AT.CardNumber + @"</m:cardNumb>";
                }

                Tmp += @"</m:T_cashRow>";
            }
            Tmp += @"</m:T_cash>";

            Tmp += @"<m:T_card>";
            foreach (AlohaClientCard CC in mChk.AlohaClientCardList)
            {
                Tmp += @"<m:T_cardRow>";
                Tmp += @"<m:ktype>" + CC.TypeId + @"</m:ktype>";
                if (CC.Prefix == "")
                {
                    Tmp += @"<m:kpref/>";
                }
                else
                {
                    Tmp += @"<m:kpref>" + CC.Prefix + @"</m:kpref>";
                }
                if (CC.Number == "")
                {
                    Tmp += @"<m:knumb/>";
                }
                else
                {
                    Tmp += @"<m:knumb>" + CC.Number + @"</m:knumb>";
                }
                Tmp += @"<m:balln>" + CC.BonusAdd + @"</m:balln>";
                Tmp += @"<m:balls>" + CC.BonusRemove + @"</m:balls>";
                Tmp += @"<m:summs>" + CC.Discount + @"</m:summs>";
                Tmp += @"<m:summo>" + CC.Payment + @"</m:summo>";
                Tmp += @"<m:summn>" + CC.CardPrice + @"</m:summn>";
                Tmp += @"</m:T_cardRow>";

            }
            Tmp += @"</m:T_card>";
            Tmp += @"<m:TableID>"+mChk.TableId.ToString();
            Tmp += @"</m:TableID>";
            Tmp += @"</m:AlohaReceiptsV3>";
            Tmp += @"</SOAP-ENV:Body>";
            Tmp += @"</SOAP-ENV:Envelope>";
            return Tmp;
        }


        public string GetXmlStr()
        {
            string Tmp = "";
            Tmp += "<?xml version=\"1.0\" encoding=\"windows-1251\"?> <SOAP-ENV:Envelope xmlns:SOAP-ENV=\"";
            Tmp += @"http://schemas.xmlsoap.org/soap/envelope/";
            Tmp += "\" xmlns:SOAP-ENC=\"/http://schemas.xmlsoap.org/soap/encoding/\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">";
            Tmp += "<SOAP-ENV:Body>";
            //Tmp += "<m:AlohaReceiptsV2 xmlns:m=\"urn:coffeemania:AlohaReceipts:AlohaReceiptsService\">";
            Tmp += "<m:AlohaReceipts xmlns:m=\"urn:coffeemania:AlohaReceipts:AlohaReceiptsService\">";
            Tmp += @"<m:Cod_Podr>" + Cod_Podr.ToString() + @"</m:Cod_Podr>";
            Tmp += @"<m:Dt_Check>" + Dt_Check.ToString("yyyy-MM-dd") + @"</m:Dt_Check>";
            Tmp += @"<m:Tm_Check>" + Dt_Check.ToString("HH:mm") + @"</m:Tm_Check>";
            Tmp += @"<m:Cod_Check>" + Cod_Check + @"</m:Cod_Check>";
            Tmp += @"<m:Cod_Manag>" + Cod_Manager.ToString() + @"</m:Cod_Manag>";
            Tmp += @"<m:Id_Discnt>" + Id_Discnt.ToString() + @"</m:Id_Discnt>";
            Tmp += @"<m:Cod_Table>" + Cod_Table.ToString() + @"</m:Cod_Table>";
            Tmp += @"<m:Dt_Open>" + Dt_Open.ToString("yyyy-MM-dd") + @"</m:Dt_Open>";
            Tmp += @"<m:Tm_Open>" + Dt_Open.ToString("HH:mm") + @"</m:Tm_Open>";
            Tmp += @"<m:PredCheck>" + PredCheck.ToString() + @"</m:PredCheck>";
            Tmp += @"<m:Sum_Check>" + Sum_Check.ToString() + @"</m:Sum_Check>";
            Tmp += @"<m:Id_Paymnt>" + Id_Paymnt.ToString() + @"</m:Id_Paymnt>";
            Tmp += @"<m:Person_name>" + Person_name + @"</m:Person_name>";
            Tmp += @"<m:Id_Info>" + "0" + @"</m:Id_Info>";
            Tmp += @"<m:T_goodsRow>";
            foreach (Dish d in mChk.ConSolidateSpoolDishez)
            {

                T_goodsRow Tr = new T_goodsRow(d, mChk.Vozvr);
                Tmp += Tr.GetXmlStr();
            }
            Tmp += @"</m:T_goodsRow>";


            Tmp += @"</m:AlohaReceipts>";
            Tmp += @"</SOAP-ENV:Body>";
            Tmp += @"</SOAP-ENV:Envelope>";
            return Tmp;
        }


        public string GetDishOrderTimeXMLList(Check Chk)
        {
            List<OrderTimeNote> Tmp = new List<OrderTimeNote>();

            foreach (Dish d in Chk.Dishez)
            {
                try
                {
                    Tmp.Add(GetDishOrderTimeXML(Chk, d.BarCode, SQL.ToSql.GetOrderTime(d.BarCode, Chk.AlohaCheckNum, d.AlohaNum, Chk.SystemDateOfOpen, Chk.Summ > 0, Chk.Waiter), (int)(Math.Abs(d.Priceone) * 100), d.Id));
                }
                catch (Exception e)
                {
                    Utils.ToLog("Error GetGetDishOrderTimeXMLList " + e.Message);
                }



            }
            XmlSerializer XS = new XmlSerializer(typeof(List<OrderTimeNote>));
            using (StringWriter writer = new StringWriter())
            {
                XS.Serialize(writer, Tmp);
                return writer.ToString();
            }
        }

        private OrderTimeNote GetDishOrderTimeXML(Check Chk, int BarCode, DateTime OrderTime, int DishMoneySumm, Guid DId)
        {
            return new OrderTimeNote
            {
                Barcode = BarCode,
                ChkNumber = Chk.CheckNum,
                Dep = AlohainiFile.DepNum,
                Id = DId,
                OrderTime = OrderTime,
                Price = DishMoneySumm
            };

        }

    }

    public class OrderTimeNote
    {
        public DateTime OrderTime { set; get; }
        public int Dep { set; get; }
        public string ChkNumber { set; get; }
        public int Barcode { set; get; }
        public int Price { set; get; }
        public Guid Id { set; get; }
    }



    public class T_goodsRow
    {
        int barcodt = 0;
        int i_price = 0;
        int i_quant = 0;
        int i_summa = 0;

        public T_goodsRow(Dish d, bool IsVosvrB)
        {
            int IsVosvr = (IsVosvrB) ? (-1) : 1;
            decimal CountRound = Math.Ceiling(d.Count * d.QUANTITY * d.QtyQUANTITY);
            decimal Count = d.Count * d.QUANTITY * d.QtyQUANTITY;
            barcodt = d.BarCode;
            i_price = (int)(Math.Abs(d.OPriceone) * 100);
            i_quant = (int)((double)CountRound * 1000 * IsVosvr);
            i_summa = (int)Math.Round(((Math.Abs(d.Priceone * (double)Count) * IsVosvr)) * 100, MidpointRounding.ToEven) + (int)Math.Round(d.Delta * 100, MidpointRounding.ToEven) + (int)Math.Round(d.ServiceChargeSumm * 100, MidpointRounding.ToEven) * IsVosvr;

            /*
            if ((i_quant % 1000) != 0)
            {
                i_quant = i_quant - i_quant % 1000;
                i_price = i_summa / (i_quant/1000);
            }
            */
        }


        public string GetXmlStr()
        {
            string Tmp = @"<m:T_goodsRowRow>";
            Tmp += @"<m:barcodt>" + barcodt.ToString() + @"</m:barcodt>";
            Tmp += @"<m:i_price>" + i_price.ToString() + @"</m:i_price>";
            Tmp += @"<m:i_quant>" + i_quant.ToString() + @"</m:i_quant>";
            Tmp += @"<m:i_summa>" + i_summa.ToString() + @"</m:i_summa>";
            Tmp += @"</m:T_goodsRowRow>";
            return Tmp;

        }
    }
}
