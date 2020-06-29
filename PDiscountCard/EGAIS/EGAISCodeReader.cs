using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace PDiscountCard.EGAIS
{
    public static class EGAISCodeReader
    {

        static int DepNum = AlohainiFile.DepNum;
        //static int DepNum = 130;
        public static void Read()
        {
            wndEGAIS we = new wndEGAIS();
            we.Show();
        }

        public static string EgaisDecode(this string input)
        {
            string CharList = "0123456789abcdefghijklmnopqrstuvwxyz";
            long result = 0;
            int pos = 0;
            for (int i = input.Length - 1; i >= 0; i--)
            {
                result += CharList.IndexOf(input.ToLower()[i]) * (long)Math.Pow(36, pos);
                pos++;
            }
            return result.ToString("0000000000000000000");
        }
        private static string DeleteErrorChars(this string input)
        {
            return input.Replace(" ", string.Empty).Replace("\r", string.Empty).Replace("\n", string.Empty);
        }
        
        private static string GetEgaisIdFrom1C(string Code)
        {
            try
            {
                
                PDiscountCard.EGAISSrv2.wsExtAlco_WebServiceAlco srv = new PDiscountCard.EGAISSrv2.wsExtAlco_WebServiceAlco();
                NetworkCredential Cred = new NetworkCredential("ws", "ws1", "");
                srv.Credentials = Cred;
                string res = srv.stringGetItemByQRCode(Code.ToUpper());
                return res;
                
            }
            catch (Exception e)
                {
                    return "Ошибка. " + e.Message;
                }
            
        }

        public static bool GetEgaisAlreadyScanFrom1C(string Code, int Unit =-1 )
        {
            try
            {
                Utils.ToCardLog("GetEgaisAlreadyScanFrom1C " + Code);
                if (Unit == -1)
                {
                    Unit = iniFile.SpoolDepNum;
                }
                PDiscountCard.EGAISSrv2.wsExtAlco_WebServiceAlco srv = new PDiscountCard.EGAISSrv2.wsExtAlco_WebServiceAlco();
                NetworkCredential Cred = new NetworkCredential("ws", "ws1", "");
                srv.Credentials = Cred;
                bool res = srv.boolCheckQRCodeDoubleDayUse(Code.ToUpper(),Unit.ToString());
                Utils.ToCardLog("GetEgaisAlreadyScanFrom1C return " + res);
                return res;

            }
            catch (Exception e)
            {
                Utils.ToCardLog("GetEgaisAlreadyScanFrom1C Error" + e.Message);
                return false;
            }

        }


        public static EGAISSrv2.Element[] GetEgaisListFrom1C()
        {
            try
            {
              /*
                PDiscountCard.EGAISSrv.wsExtAlco_WebServiceAlco srv = new PDiscountCard.EGAISSrv.wsExtAlco_WebServiceAlco();
                NetworkCredential Cred = new NetworkCredential("ws", "ws1", "");
                srv.Credentials = Cred;
                //string res = srv.GetBeerStockAlcoCodes(AlohainiFile.DepNum.ToString());
                EGAISSrv.Element[] res = srv.GetBeerStockAlcoCodes(DepNum.ToString());
                return res;
           */
                PDiscountCard.EGAISSrv2.wsExtAlco_WebServiceAlco srv = new PDiscountCard.EGAISSrv2.wsExtAlco_WebServiceAlco();
                NetworkCredential Cred = new NetworkCredential("ws", "ws1", "");
                srv.Credentials = Cred;
                //string res = srv.GetBeerStockAlcoCodes(AlohainiFile.DepNum.ToString());
                EGAISSrv2.Element[] res = srv.GetBeerStockAlcoCodes(DepNum.ToString());

                Utils.ToLog(String.Format("GetBeerStockAlcoCodes Dep: {0}; Address {1}",DepNum,srv.Url ));
                foreach (EGAISSrv2.Element El in res)
                {
                    Utils.ToLog(El.AlcoCode +" " +El.AlcoName);
                }
                return res;


            }
            catch (Exception e)
            {
                Utils.ToLog("Error GetEgaisListFrom1C " + e.Message);
                return null;
            }
          //  return null;
        }


        public static bool SendBeerEgaisTo1C(string AlcoCode)
        {
            try
            {


                try
                {
                    Utils.ToCardLog(String.Format("Send beer to EGAIS: {0}", AlcoCode));
                    PDiscountCard.EGAISSrv.wsExtAlco_WebServiceAlco srv = new PDiscountCard.EGAISSrv.wsExtAlco_WebServiceAlco();
                    NetworkCredential Cred = new NetworkCredential("ws", "ws1", "");
                    srv.Credentials = Cred;
                    //   string res = srv.stringPutBottleOpen(QRCode, AlohainiFile.DepNum.ToString());
                    string res2 = srv.stringPutBeerOpen(AlcoCode, DepNum.ToString());
                    
                }
                catch
                {
                
                }

                Utils.ToCardLog(String.Format("Send beer to EGAIS: {0}", AlcoCode));
                PDiscountCard.EGAISSrv2.wsExtAlco_WebServiceAlco srv2 = new PDiscountCard.EGAISSrv2.wsExtAlco_WebServiceAlco();
                NetworkCredential Cred2 = new NetworkCredential("ws", "ws1", "");
                srv2.Credentials = Cred2;
                //   string res = srv.stringPutBottleOpen(QRCode, AlohainiFile.DepNum.ToString());
                string res = srv2.stringPutBeerOpen(AlcoCode, DepNum.ToString());
                return res == "0";       


                

            }
            catch (Exception e)
            {
                Utils.ToCardLog(String.Format("Error beer Send to EGAIS: {0}; Err:{1}", AlcoCode, e.Message));
                return false;
            }
            
        }


        public static bool SendEgaisTo1C(string QRCode)
        {

            try
            {

                Utils.ToCardLog(String.Format("Send to EGAIS: {0}", QRCode));
                PDiscountCard.EGAISSrv.wsExtAlco_WebServiceAlco srv = new PDiscountCard.EGAISSrv.wsExtAlco_WebServiceAlco();
                NetworkCredential Cred = new NetworkCredential("ws", "ws1", "");
                srv.Credentials = Cred;
                string res = srv.stringPutBottleOpen(QRCode.ToUpper(), DepNum.ToString());
              //  return res == "0";

            }
            catch (Exception e)
            {
                Utils.ToCardLog(String.Format("Error Send to EGAIS: {0}; Err:{1}", QRCode, e.Message));
              //  return false;
            }


            try
            {

                Utils.ToCardLog(String.Format("Send to EGAIS2: {0}", QRCode));
                PDiscountCard.EGAISSrv2.wsExtAlco_WebServiceAlco srv2 = new PDiscountCard.EGAISSrv2.wsExtAlco_WebServiceAlco();
                NetworkCredential Cred = new NetworkCredential("ws", "ws1", "");
                srv2.Credentials = Cred;
                string res = srv2.stringPutBottleOpen(QRCode.ToUpper(), DepNum.ToString());
                Utils.ToCardLog(String.Format("Send to EGAIS2 end res: {0}", res));
                return res == "0";
            }
            catch (Exception e)
            {
                Utils.ToCardLog(String.Format("Error Send to EGAIS2: {0}; Err:{1}", QRCode, e.Message));
                return false;
            }

            //http://server1c/alco/ws/wsAlco.1cws?wsdl
            //http://server1c/retail/ws/wsAlco.1cws?wsdl



           
            
        }

        public static bool GetEgaisCode(string Code, out string Mess)
        {
            Mess = "";
            Code = Code.DeleteErrorChars();
            if (Code.Length >= 68)
            {
                string res = GetEgaisIdFrom1C(Code);
                if (res == "")
                {
                    Mess = "Данная бутылка отсутствует в базе товаров";
                    return false;
                }
                else
                {
                    Mess = res;
                    return true;

                }
            }
            //return GetEgaisIdFrom1C(Code.Substring(3, 16).EgaisDecode());
            //return  ;
            else
                Mess = "Неверный код.";
                return false;

        }
    }
}
