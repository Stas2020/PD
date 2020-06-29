using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PDiscountCard.MBProxi;

namespace PDiscountCard.MB
{
    /// <summary>
    /// Interaction logic for PBAuthorization.xaml
    /// </summary>
    public partial class PBAuthorization : UserControl
    {
        private Window OwnerWnd;
        private long _lastId;

        public MBProxi.PBCustomerInfo ProcUser;

       
        public PBAuthorization()
        {
            InitializeComponent();
            _lastId = 0;
            digitKeyboard.OnOk += new ctrlDigitalKeyBoard.OkEventHandler(digitKeyboardOkClc);
            digitKeyboard.OnCancel += new ctrlDigitalKeyBoard.OkEventHandler(digitKeyboardCancelClc);
        }

       
        void digitKeyboardOkClc(object sender, double Value)
        {
            if (digitKeyboard.CurentDouble > 0)
            {                
               long validationCode = (long)digitKeyboard.CurentDouble;                

                if(_lastId != validationCode)
                {
                    try
                    {
                        _lastId = validationCode;

                        using (CardServiceClient client = GetWCFClient())
                        {

                            if (client != null && client.Online())
                            { 
                                var response = client.PBCustomerAuth(validationCode);

                                if(response != null && response.Result != null)
                                {
                                    if (response.Result.Auth)
                                    {
                                        ProcUser = response.Result;
                                        Out();
                                    }
                                    else
                                    {
                                        this.footerText.Foreground = Brushes.Black;
                                        this.footerText.Text = "  Гость не найден";
                                    }
                                }
                                else
                                {
                                    this.footerText.Foreground = Brushes.Red;
                                    this.footerText.Text = "  Ошибка ответа сервиса";
                                }                                
                            }
                            else
                            {
                                this.footerText.Foreground = Brushes.Red;
                                this.footerText.Text = "  CardService offline";
                            }
                        }
                    }
                    catch { }
                }                              
            }
        }

        

        void digitKeyboardCancelClc(object sender, double Value)
        {
            if (ProcUser != null)
            {
                ProcUser = null;
            }
            Out();
        }

        void Out()
        {
            OwnerWnd.Close();
        }

        internal void SetOwnerWnd(Window _OwnerWnd)
        {
            OwnerWnd = _OwnerWnd;
        }

        private CardServiceClient GetWCFClient()
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

    }
}
