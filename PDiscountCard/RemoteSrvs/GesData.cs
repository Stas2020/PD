using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDiscountCard.RemoteSrvs
{
    public class GesData
    {
        public GesData()
        { }
        private Gestory.Ges3ServicesObjClient Client
        {
            get
            {
                try
                {
                    Utils.ToCardLog(String.Format("GesData GetClient "));
                    System.ServiceModel.Channels.Binding binding = new System.ServiceModel.BasicHttpBinding();
                    ((System.ServiceModel.BasicHttpBinding)binding).MaxReceivedMessageSize = 1024 * 1024;
                  //  System.ServiceModel.EndpointAddress remoteAddress = new System.ServiceModel.EndpointAddress(@"http://vfiliasesb0:2580/process/Ges3ServicesProc");
                    System.ServiceModel.EndpointAddress remoteAddress = new System.ServiceModel.EndpointAddress(@"http://app:8000/process/process");

                
                    Gestory.Ges3ServicesObjClient GesCl = new Gestory.Ges3ServicesObjClient(binding, remoteAddress);
                    
                    GesCl.InnerChannel.OperationTimeout = new TimeSpan(0, 10, 0);
                    return GesCl;
                }
                catch(Exception e)
                {
                    Utils.ToCardLog(String.Format("GesData GetClient error {0}", e.Message));
                    return null;
                }
            }
        }

        private Gestory.Ges3ServicesObjClient StasClient
        {
            get
            {
                try
                {
                    Utils.ToCardLog(String.Format("GesData GetStatClient "));
                    System.ServiceModel.Channels.Binding binding = new System.ServiceModel.BasicHttpBinding();
                    ((System.ServiceModel.BasicHttpBinding)binding).MaxReceivedMessageSize = 1024 * 1024;
                    System.ServiceModel.EndpointAddress remoteAddress = new System.ServiceModel.EndpointAddress(@"http://vfiliasesb0:2580/process/Ges3ServicesUTF8Proc");
                    Gestory.Ges3ServicesObjClient GesCl = new Gestory.Ges3ServicesObjClient(binding, remoteAddress);

                    GesCl.InnerChannel.OperationTimeout = new TimeSpan(0, 10, 0);
                    return GesCl;
                }
                catch (Exception e)
                {
                    Utils.ToCardLog(String.Format("GesData GetClient error {0}", e.Message));
                    return null;
                }
            }
        }


      


        public Dictionary<int,int> GetItemExp(List<int> barcodes, out string err)
        {
            err = "";
            if (barcodes == null) return null;
            var res = new Dictionary<int, int> ();
            var cl = Client;
            if (Client == null)
            {
                err = "Ошибка при подключении к Джестори. не могу получить сроки хранения";
                return null;
            }

            try
            {   
                    Gestory.storage_times_T_goodsRow[] outArr ;
                    Gestory.storage_times_T_barcRow[] inArr = barcodes.Select(a => new Gestory.storage_times_T_barcRow() { bar_cod = a.ToString() }).ToArray();
                    cl.storage_times(inArr, out outArr);
                    if (outArr == null) return null;

                    foreach (var rec in outArr)
                    {
                        try
                        {
                            int m = 0;
                            if (!res.TryGetValue(Convert.ToInt32(rec.bar_cod), out m))
                            {
                                res.Add(Convert.ToInt32(rec.bar_cod),rec.param1.GetValueOrDefault());
                            }
                        }
                        catch
                        { }
                    }
                return res;

            }
            catch (Exception e)
            {
                err = "Ошибка при подключении к Джестори. "+e.Message;
                Utils.ToCardLog(String.Format("GetItemExp error {0}", e.Message));
                return null;
            }
        }

        public Dictionary<int, Tuple<int, string>> GetItemExp2(List<int> barcodes, out string err)
        {
            err = "";
            if (barcodes == null) return null;
           var res = new Dictionary<int, Tuple<int, string>>();
            var cl = Client;
            if (Client == null)
            {
                err = "Ошибка при подключении к Джестори. не могу получить сроки хранения";
                return null;
            }

            try
            {
                Gestory.storage_times2_T_goodsRow[] outArr;
                Gestory.storage_times2_T_barcRow[] inArr = barcodes.Select(a => new Gestory.storage_times2_T_barcRow() { bar_cod = a.ToString() }).ToArray();
                cl.storage_times2(inArr, out outArr);
                if (outArr == null) return null;

                foreach (var rec in outArr)
                {
                    try
                    {
                       // int m = 0;
                        if (!res.TryGetValue(Convert.ToInt32(rec.bar_cod), out var m))
                        {
                            res.Add(Convert.ToInt32(rec.bar_cod), new Tuple<int, string>( rec.param1.GetValueOrDefault(), rec.PARAM_usl ));
                        }
                    }
                    catch
                    { }
                }
                return res;

            }
            catch (Exception e)
            {
                err = "Ошибка при подключении к Джестори. " + e.Message;
                Utils.ToCardLog(String.Format("GetItemExp error {0}", e.Message));
                return null;
            }
        }
        public Dictionary<int, string > GetItemDescription(List<int> barcodes, out string err)
        {
            Utils.ToCardLog("---GetItemDescription bars:-----");
            err = "";
            if (barcodes == null) return null;
            Dictionary<int, string> res = new Dictionary<int, string>();
            var cl = Client;
            if (Client == null)
            {
                err = "Ошибка при подключении к Джестори. не могу получить состав";
                return null;
            }

            try
            {
                Gestory.sostav_barc_T_goodsRow[] outArr;
                Gestory.sostav_barc_T_barcRow[] inArr = barcodes.Select(a => new Gestory.sostav_barc_T_barcRow() { bar_cod = a.ToString() }).ToArray();
                cl.sostav_barc(inArr, out outArr);
                if (outArr == null) return null;

                foreach (var rec in outArr)
                {
                    Utils.ToCardLog("Bc: " + rec.bar_cod + " sost: " + rec.sostav);
                    try
                    {
                        string m = "";
                        if (!res.TryGetValue(Convert.ToInt32(rec.bar_cod), out m))
                        {
                            string s = rec.sostav;
                            byte[] textAsBytes = Encoding.GetEncoding(1251).GetBytes(s);
                            s = Encoding.UTF8.GetString(textAsBytes);
                            res.Add(Convert.ToInt32(rec.bar_cod), s);
                        }
                    }
                    catch
                    { }
                }
                return res;

            }
            catch (Exception e)
            {
                err = "Ошибка при подключении к Джестори. " + e.Message;
                Utils.ToCardLog(String.Format("GetItemExp error {0}", e.Message));
                return null;
            }
        }

    }
}
