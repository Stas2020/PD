using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace PDiscountCard.RemoteSrvs
{
    class HubData
    {

        public HubData()
        { }

        private HubSrv.DeliveryHubConnectorServiceClient Client
        {
            get
            {
                try
                {
                    /*
                    Utils.ToCardLog(String.Format("DeliveryHubConnectorServiceClient GetClient "));
                    System.ServiceModel.Channels.Binding binding = new System.ServiceModel.BasicHttpBinding();
                    ((System.ServiceModel.BasicHttpBinding)binding).MaxReceivedMessageSize = 1024 * 1024;




                    System.ServiceModel.EndpointAddress remoteAddress = new System.ServiceModel.EndpointAddress(@"net.tcp://192.168.254.163/DeliveryHubConnector/DeliveryHubConnectorService.svc");
                    var GesCl = new HubSrv.DeliveryHubConnectorServiceClient(binding, remoteAddress);

                    GesCl.InnerChannel.OperationTimeout = new TimeSpan(0, 10, 0);
                    return GesCl;
                    */



                    Utils.ToCardLog("DeliveryHubConnectorServiceClient GetClient ");
                    NetTcpBinding binding = new NetTcpBinding(SecurityMode.Transport);

                    binding.Security.Message.ClientCredentialType = MessageCredentialType.Windows;
                    binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
                    binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;

                    string uri = @"net.tcp://192.168.254.163/DeliveryHubConnector/DeliveryHubConnectorService.svc";

                    EndpointAddress endpoint = new EndpointAddress(new Uri(uri));

                   var _client = new HubSrv.DeliveryHubConnectorServiceClient(binding, endpoint);

                    _client.ClientCredentials.Windows.ClientCredential.Domain = "";
                    _client.ClientCredentials.Windows.ClientCredential.UserName = "aloha";
                    _client.ClientCredentials.Windows.ClientCredential.Password = "Fil123fil123";
                    return _client;

                }
                catch (Exception e)
                {
                    Utils.ToCardLog(String.Format("DeliveryHubConnectorServiceClient GetClient error {0}", e.Message));
                    return null;
                }
            }
        }

        public void HubHello()
        {
            try
            {
                Utils.ToCardLog("HubHello");
                var req = new HubSrv.OperationRequestFromPOSInfo()
                {
                    DepNum = AlohainiFile.DepNum,
                    RequestData = new HubSrv.POSInfo()
                    {
                        MainPOSFlag = true
                    }
                };
                Client.Hello(req);
            }
            catch(Exception e)
            {
                Utils.ToCardLog("Error HubHello "+e.Message);
            }


        }
        



    }



}
