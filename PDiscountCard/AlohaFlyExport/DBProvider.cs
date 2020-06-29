using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace PDiscountCard.AlohaFlyExport
{
    public class DBProvider
    {
        public static AlohaService.IAlohaService GetClient()
        {

            return Client;
        }

        public static AlohaService.IAlohaService Client
        {
            get
            {
                try
                {
                    var address = new EndpointAddress(new Uri(PDiscountCard.iniFile.AlohaFlyExportConnectionString));

                    /*
                    var binding = new BasicHttpBinding(BasicHttpSecurityMode.TransportWithMessageCredential);
                    binding.MaxReceivedMessageSize = 1000000000;
                    */

                    BinaryMessageEncodingBindingElement encoding = new BinaryMessageEncodingBindingElement();
                    HttpsTransportBindingElement transport = new HttpsTransportBindingElement();
                    transport.MaxReceivedMessageSize = 1000000000;
                    Binding binding = new CustomBinding(encoding, transport);
                 
                    var channelFactory = new System.ServiceModel.ChannelFactory<AlohaService.IAlohaService>(binding, address);
                    var credintialBehaviour = channelFactory.Endpoint.Behaviors.Find<System.ServiceModel.Description.ClientCredentials>();
                    credintialBehaviour.UserName.UserName = "aloha_user";
                    credintialBehaviour.UserName.Password = "Welcome01";
                    var client = channelFactory.CreateChannel();
                    System.Net.ServicePointManager.ServerCertificateValidationCallback = (senderX, certificate, chain, sslPolicyErrors) => { return true; };
                    return client;
                }
                catch (Exception e)
                {

                    Utils.ToLog("AlohaFlyExport Get Client error " + e.Message);
                    return null;
                }
            }
        }

        public static bool SendOrder(AlohaService.OrderFlight orderFlight)
        {
            Utils.ToLog("AlohaFlyExport SendOrder ");
            try {
                var cl = GetClient();
                if (cl == null)
                {
                    Utils.ToLog("AlohaFlyExport SendOrder cl == null");
                    return false;
                }

                var res = cl.InsertOrderFlightFromAloha(orderFlight);
                if (!res)
                {
                    Utils.ToLog("AlohaFlyExport SendOrder not Success ");
                    return false;
                }
                /*
                long Id = res.CreatedObjectId;
                Utils.ToLog("AlohaFlyExport Order inserted Id:" + Id);

                foreach (var d in orderFlight.DishPackages)
                {
                    int maxTryCount = 10;
                    int curentTry=1;
                    bool ok = false;
                    
                        while (!ok && curentTry<maxTryCount)
                        {
                        d.OrderFlightId = Id;
                        var resD =  cl.CreateDishPackageFlightOrder(d);
                        ok = resD.Success;
                        if (!ok) {
                            Utils.ToLog("AlohaFlyExport SendOrder add Dish error " + resD.ErrorMessage + " Count " + curentTry);
                        }
                        curentTry++;
                        }
                        if (!ok)
                        {
                            return false;
                        }
                    
                }

                Utils.ToLog("AlohaFlyExport SendOrder Success Id = " + res.CreatedObjectId);
                 * */
                return true;
            }
            catch (Exception e)
            {
                Utils.ToLog("Error AlohaFlyExport SendOrder " +e.Message);
                return false;
            }

        }



    }
}

