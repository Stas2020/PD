using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using PDiscountCard.MBProxi;

namespace PDiscountCard.Config
{
    public  class ConfigSettings
    {
        static MBProxi.MyTermSettings Settings = null;

        public static void SetSettings()
        {
            Settings = GetSettings();

        }

        private static  CardServiceClient GetWCFClient()
        {
            CardServiceClient _client = null;
            bool succes = false;

            try
            {
                Utils.ToCardLog("CreateWCFClient");
                NetTcpBinding binding = new NetTcpBinding(SecurityMode.Transport);

                binding.Security.Message.ClientCredentialType = MessageCredentialType.Windows;
                binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
                binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;

                string uri = @"net.tcp://192.168.254.139/CardService/CardService.svc";

                EndpointAddress endpoint = new EndpointAddress(new Uri(uri));

                _client = new PDiscountCard.MBProxi.CardServiceClient(binding, endpoint);

                _client.ClientCredentials.Windows.ClientCredential.Domain = "";
                _client.ClientCredentials.Windows.ClientCredential.UserName = "администратор";
                _client.ClientCredentials.Windows.ClientCredential.Password = "Fil123fil123";


                return _client;


            }
            catch (Exception ex)
            {
                string mess = ex.Message;
                succes = false;
                _client = null;
                Utils.ToCardLog("Error CreateWCFClient " + ex.Message);
                return null;
            }
        }

        public static bool QRPrinting
        {
            get
            {
                if (Settings == null)
                {
                    SetSettings();
                }
                if (Settings == null)
                {
                    return false;
                }
                Utils.ToLog("Settings.Precheck.QrPrint= " + Settings.Precheck.QrPrint);
                return Settings.Precheck.QrPrint; 
            }
        }


        private static MBProxi.MyTermSettings GetSettings()
        {
            try
            {
                Utils.ToLog("GetSettings " );
                var request = new MBProxi.TermSettingsRequest
                {
                    DepartmentNumber = AlohainiFile.DepNum,
                    TerminalNumber = AlohaTSClass.GetTermNum()
                };
                MBProxi.CardServiceClient client = GetWCFClient();
                var result = client.GetSettingsForTerm(request);
                if (result.Success)
                {
                    Utils.ToLog("GetSettings ok result.Result = " +result.Result.Precheck.QrPrint);
                    return result.Result;
                }
                return null;
            }
            catch(Exception e)
            {
                Utils.ToLog("Error GetSettings " + e.Message);
                return null;
            }
        }

    }
}
