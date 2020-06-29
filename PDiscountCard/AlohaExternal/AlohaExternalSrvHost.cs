using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace PDiscountCard.AlohaExternal
{
    static public class UniversalHost
    {
        static Uri baseAddress = new Uri(String.Format("http://{0}:{1}/AlohaService", iniFile.ExternalInterfaceIp,iniFile.ExternalInterfacePort));
        static Uri baseAddressJSON = new Uri(String.Format("http://{0}:{1}/AlohaService", iniFile.ExternalInterfaceIp, iniFile.ExternalInterfaceJSONPort));
        static ServiceHost selfHostJSON;
        static ServiceHost selfHost;
        static public void Open()
        {
            try
            {
                Utils.ToCardLog(String.Format("UniversalHost.Open"));
                selfHost = new ServiceHost(typeof(AlohaExternal), baseAddress);
                try
                {


                    // Step 3 of the hosting procedure: Add a service endpoint.
                    selfHost.AddServiceEndpoint(typeof(IAlohaExternal), new BasicHttpBinding(), "AlohaExternal");
                    //var webHttp = new WebHttpBinding();

                    //var ep =  selfHost.AddServiceEndpoint(typeof(IAlohaExternal), new WebHttpBinding(), "AlohaExternal");

                    // ep.Behaviors.Add(new WebHttpBehavior());

                    // Step 4 of the hosting procedure: Enable metadata exchange.
                    ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
                    smb.HttpGetEnabled = true;

                    selfHost.Description.Behaviors.Add(smb);


                    // Step 5 of the hosting procedure: Start (and then stop) the service.
                    selfHost.Open();
                    Utils.ToCardLog("AlohaExternal The service is ready. " + baseAddress);



                    // Close the ServiceHostBase to shutdown the service.

                }
                catch (CommunicationException ce)
                {
                    Utils.ToCardLog(String.Format("An exception occurred: {0}", ce.Message));
                    selfHost.Abort();
                }


                selfHostJSON = new ServiceHost(typeof(AlohaExternal), baseAddressJSON);
                try
                {
                    // Step 3 of the hosting procedure: Add a service endpoint.
                    var ep2 = selfHostJSON.AddServiceEndpoint(typeof(IAlohaExternal), new WebHttpBinding(), "AlohaExternal");

                    // Step 4 of the hosting procedure: Enable metadata exchange.

                    ep2.Behaviors.Add(new WebHttpBehavior());

                    // Step 4 of the hosting procedure: Enable metadata exchange.
                    ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
                    smb.HttpGetEnabled = true;

                    selfHostJSON.Description.Behaviors.Add(smb);

                    // Step 5 of the hosting procedure: Start (and then stop) the service.
                    selfHostJSON.Open();
                    Utils.ToCardLog("AlohaExternalJSON The service is ready. " + baseAddressJSON);


                    RemoteSrvs.HubData hSrv = new RemoteSrvs.HubData();
                    hSrv.HubHello();

                    // Close the ServiceHostBase to shutdown the service.

                }
                catch (CommunicationException ce)
                {
                    Utils.ToCardLog(String.Format("An exception occurred selfHostJSON: {0}", ce.Message));
                    selfHostJSON.Abort();
                }
            }
            catch(Exception allE)
            {
                Utils.ToCardLog(String.Format("UniversalHost.Open error: {0}", allE.Message));
            }

        }



        private static Dictionary<Guid, ICommandResponse> RecivedCommands = new Dictionary<Guid, ICommandResponse>();

        static public bool AddRecivedCommand(PDiscountCard.AlohaExternal.ICommandResponse RecivedCommand)
        {
            PDiscountCard.AlohaExternal.ICommandResponse RC;
            if (!RecivedCommands.TryGetValue(RecivedCommand.RequestId,out RC))
            {
                RecivedCommand.Status = 0;
                RecivedCommands.Add(RecivedCommand.RequestId,RecivedCommand);
                return true ;
            }
            else
            {
                if (!RC.Success)
                {
                    return false;
                }
                else
                {
                    RC = RecivedCommand;
                    return true;
                }
            }
        }

        static public bool GetRecivedCommand(Guid guid, out PDiscountCard.AlohaExternal.ICommandResponse RecivedCommand)
        {
            if (RecivedCommands.TryGetValue(guid, out RecivedCommand))
            {
                return true ;
            }
            return false;
        }


        static public void ComplitedRecivedCommand(PDiscountCard.AlohaExternal.ICommandResponse RecivedCommand)
        {
            PDiscountCard.AlohaExternal.ICommandResponse RC;
            if (RecivedCommands.TryGetValue(RecivedCommand.RequestId, out RC))
            {
                if (RecivedCommand.Success)
                {
                    RecivedCommand.Status = 1;
                }
                else
                {
                    RecivedCommand.Status = 2;
                }
                RC = RecivedCommand;
                
            }
        }

        static public void Close()
        {
            selfHost.Close();
        }

    }
}
