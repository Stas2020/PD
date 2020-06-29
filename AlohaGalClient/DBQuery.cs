using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using AlohaGalClient.DbSrv;
using System.ServiceModel.Channels;


namespace AlohaGalClient
{
    class DBQuery
    {
        public static DbSrv.IAlohaService GetClient()
        {

            return Client;
        }

        public static DbSrv.IAlohaService Client
        {
            get
            {
                try
                {

                    var address = new EndpointAddress(new Uri(Properties.Settings.Default.DBAddress));
                    // var address = new EndpointAddress(new Uri(Properties.Settings.Default.DBAddress));
                    
                    //var binding = new BasicHttpBinding(BasicHttpSecurityMode.TransportWithMessageCredential);
                    //binding.MaxReceivedMessageSize = 1000000000;
                    //binding.Security = new WSHttpBinding(SecurityMode.TransportWithMessageCredential);

                    BinaryMessageEncodingBindingElement encoding = new BinaryMessageEncodingBindingElement();
                    HttpsTransportBindingElement transport = new HttpsTransportBindingElement();
                    transport.MaxReceivedMessageSize = 1000000000;

                    Binding binding = new CustomBinding(encoding, transport);
                    var channelFactory = new System.ServiceModel.ChannelFactory<DbSrv.IAlohaService>(binding, address);
                    var credintialBehaviour = channelFactory.Endpoint.Behaviors.Find<System.ServiceModel.Description.ClientCredentials>();
                    credintialBehaviour.UserName.UserName = "aloha_user";
                    credintialBehaviour.UserName.Password = "Welcome01";
                    var client = channelFactory.CreateChannel();


                    //var client = new var(binding, address);

                    System.Net.ServicePointManager.ServerCertificateValidationCallback = (senderX, certificate, chain, sslPolicyErrors) => { return true; };
                    return client;
                }
                catch (Exception e)
                {

                    Utils.ToLog("Get Client error " + e.Message);
                    return null;
                }
            }
        }

        List<Payment> payments;
        public List<Payment> Payments
        {
            get
            {
                /*
                if (payments == null)
                {
                    payments = GetPayments();
                }
                 * */
                return GetPayments();
            }
        }

        List<Payment> GetPayments()
        {
           var cl = GetClient();
            try
            {
                var client = GetClient();
                if (client == null) { return null; }
                var res = client.GetPaymentList();
                if (res == null) { Utils.ToLog("GetPayments res==null"); return null; }
                if (!res.Success)
                {
                    Utils.ToLog("GetPayments data recived err: " + res.ErrorMessage);
                    return null;
                }
                return res.Result.ToList();

            }
            catch (Exception e)
            {
                Utils.ToLog("GetPayments error " + e.Message);
                return null;
            }
        }


        List<Dish> GetDishes()
        {
          var cl = GetClient();
            try
            {
                var client = GetClient();
                if (client == null) { return null; }
                var res = client.GetDishList();
                if (res == null) { Utils.ToLog("GetDishList res==null"); return null; }
                if (!res.Success)
                {
                    Utils.ToLog("GetDishList data recived err: " + res.ErrorMessage);
                    return null;
                }
                return res.Result.ToList();
            }
            catch (Exception e)
            {
                Utils.ToLog("GetDishList error " + e.Message);
                return null;
            }
        }

        List<AirCompany> airs;
        public List<AirCompany> Airs
        {
            get
            {/*
                if (airs == null)
                {
                    airs = GetAirs();
                }
              * */
                return GetAirs(); ;
            }
        }

        List<AirCompany> GetAirs()
        {
            
            try
            {
                

                var client = GetClient();
                if (client == null) { return null; }
                var res = client.GetAirCompanyList();
                if (res == null) { Utils.ToLog("GetAirs res==null"); return null; }
                if (!res.Success)
                {
                    Utils.ToLog("GetAirs data recived err: " + res.ErrorMessage);
                    return null;
                }

                foreach (var comp in res.Result)
                {
                    try
                    {
                        if (comp.PaymentId != null)
                        {
                            comp.PaymentType = Payments.SingleOrDefault(a => a.Id == comp.PaymentId);
                        }
                    }
                    catch
                    { }
                }
                return res.Result.ToList();

            }
            catch (Exception e)
            {
                Utils.ToLog("GetAirs error " + e.Message);
                return null;
            }
        }


        public bool UpdateClosingCheck(OrderFlight ord, bool FRPreented, bool PreCheckPreented)
        {

            try
            {
                Utils.ToLog("UpdateClosingCheck " +ord.Id);
                var client = GetClient();
                if (client == null) { return false; ; }
                ord.NeedPrintFR = false;
                ord.NeedPrintPrecheck = false;
                ord.FRPrinted = FRPreented;
                ord.PreCheckPrinted = PreCheckPreented;
                var res =  client.UpdateOrderFlight(ord,1);
                if (!res.Success)
                {
                    Utils.ToLog("Error UpdateClosingCheck from service" + res.ErrorMessage);
                    return false;
                }
                Utils.ToLog("UpdateClosingCheck ok" +ord.Id);
                return true;
            }
            catch(Exception e)
            {
            Utils.ToLog("Error UpdateClosingCheck " +e.Message);
            return false;
            }
        }

        public bool UpdateClosingCheck(OrderToGo ord, bool FRPreented, bool PreCheckPreented)
        {

            try
            {
                Utils.ToLog("UpdateClosingCheck " + ord.Id);
                var client = GetClient();
                if (client == null) { return false; ; }
                ord.NeedPrintFR = false;
                ord.NeedPrintPrecheck = false;
                ord.FRPrinted = FRPreented;
                ord.PreCheckPrinted = PreCheckPreented;
                var res = client.UpdateOrderToGo(ord);
                if (!res.Success)
                {
                    Utils.ToLog("Error UpdateClosingCheck from service" + res.ErrorMessage);
                    return false;
                }
                Utils.ToLog("UpdateClosingCheck ok" + ord.Id);
                return true;
            }
            catch (Exception e)
            {
                Utils.ToLog("Error UpdateClosingCheck " + e.Message);
                return false;
            }
        }

        public List<OrderFlight> GetNonClosingCheck()
        {

            try
            {

                var client = GetClient();
                if (client == null) { return null; }


                var res = client.GetOrderFlightListNeedToFR();
                if (res == null) { Utils.ToLog("res==null");return  null; }
                //Utils.ToLog("data recived res "+ res.Success +" err: "+ res.ErrorMessage);
                if (!res.Success)
                {
                    Utils.ToLog("data recived err: " + res.ErrorMessage);

                    return null;
                }

                var result = res.Result;
                //var result = res.Result;
                if (result.Count() > 0)
                {
                    //var ord = result.FirstOrDefault();
                    var dd = GetDishes();
                    foreach (var ord in result)
                    {
                        if (ord.AirCompanyId != null)
                        {
                            ord.AirCompany = Airs.SingleOrDefault(a => a.Id == ord.AirCompanyId);
                        }
                        foreach (var d in ord.DishPackages)
                        {
                            d.Dish = dd.SingleOrDefault(a => a.Id == d.DishId);
                        }
                    }
                Utils.ToLog("GetNonClosingCheck ok" +result.Count());

                    return result.ToList();
                }
                return null;
            }
            catch (Exception e)
            {
                Utils.ToLog("GetOrders error " + e.Message);
                return null;
            }
        }


        public List<OrderToGo> GetNonToGoClosingCheck()
        {

            try
            {

                var client = GetClient();
                if (client == null) { return null; }


                var res = client.GetOrderToGoListNeedToFR();
                if (res == null) { Utils.ToLog("res==null"); return null; }
                //Utils.ToLog("data recived res "+ res.Success +" err: "+ res.ErrorMessage);
                if (!res.Success)
                {
                    Utils.ToLog("data recived err: " + res.ErrorMessage);

                    return null;
                }

                var result = res.Result;
                //var result = res.Result;
                if (result.Count() > 0)
                {
                    //var ord = result.FirstOrDefault();
                    var dd = GetDishes();
                    foreach (var ord in result)
                    {
                        if (ord.PaymentId != null)
                        {
                            ord.PaymentType = Payments.SingleOrDefault(a => a.Id == ord.PaymentId);
                        }
                        foreach (var d in ord.DishPackages)
                        {
                            d.Dish = dd.SingleOrDefault(a => a.Id == d.DishId);
                        }
                    }
                    Utils.ToLog("GetNonClosingCheck ok" + result.Count());

                    return result.ToList();
                }
                return null;
            }
            catch (Exception e)
            {
                Utils.ToLog("GetOrders error " + e.Message);
                return null;
            }
        }


        public OrderToGo GetCheckByIdToGo(long id)
        {

            try
            {

                var client = GetClient();
                if (client == null) { return null; }


                var res = client.GetOrderToGo(id);
                if (res == null) { Utils.ToLog("res==null"); return null; }
                //Utils.ToLog("data recived res "+ res.Success +" err: "+ res.ErrorMessage);
                if (!res.Success)
                {
                    Utils.ToLog("data recived err: " + res.ErrorMessage);

                    return null;
                }

                //var result = res.Result.Where(a => a.NeedPrintFR || a.NeedPrintPrecheck).OrderByDescending(a => a.DeliveryDate).ToList();
                var result = res.Result;
                //  if (result.Count > 0)
                {
                    var ord = result;
                    var dd = GetDishes();
                    var pp = GetPayments();

                    //foreach (var ord in result)
                    {
                        
                        if (ord.PaymentId != null)
                        {
                            ord.PaymentType = pp.SingleOrDefault(a => a.Id == ord.PaymentId);
                        }
                        
                        foreach (var d in ord.DishPackages)
                        {
                            d.Dish = dd.SingleOrDefault(a => a.Id == d.DishId);
                        }
                    }
                    Utils.ToLog("GetNonClosingCheck ok" + ord.Id);

                    return ord;
                }

            }
            catch (Exception e)
            {
                Utils.ToLog("GetOrders error " + e.Message);
                return null;
            }
        }

        public OrderFlight GetCheckById(long id)
        {

            try
            {

                var client = GetClient();
                if (client == null) { return null; }


                var res = client.GetOrderFlight(id);
                if (res == null) { Utils.ToLog("res==null"); return null; }
                //Utils.ToLog("data recived res "+ res.Success +" err: "+ res.ErrorMessage);
                if (!res.Success)
                {
                    Utils.ToLog("data recived err: " + res.ErrorMessage);

                    return null;
                }

                //var result = res.Result.Where(a => a.NeedPrintFR || a.NeedPrintPrecheck).OrderByDescending(a => a.DeliveryDate).ToList();
                var result = res.Result;
              //  if (result.Count > 0)
                {
                    var ord = result;
                    var dd = GetDishes();
                    //foreach (var ord in result)
                    {
                        if (ord.AirCompanyId != null)
                        {
                            ord.AirCompany = Airs.SingleOrDefault(a => a.Id == ord.AirCompanyId);
                        }
                        foreach (var d in ord.DishPackages)
                        {
                            d.Dish = dd.SingleOrDefault(a => a.Id == d.DishId);
                        }
                    }
                    Utils.ToLog("GetNonClosingCheck ok" + ord.Id);

                    return ord;
                }
                
            }
            catch (Exception e)
            {
                Utils.ToLog("GetOrders error " + e.Message);
                return null;
            }
        }

    }
}
