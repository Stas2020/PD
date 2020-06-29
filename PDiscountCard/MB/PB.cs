using PDiscountCard.MBProxi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace PDiscountCard.MB
{
    class PB
    {
        //private static MBProxi.PBCustomerInfo ProcessedGuest;
        // айтем заведен в cfc с таким айди
        private const int IdItem = 999906;
        public  static void SendCurentChk()
        {
            Check chk = AlohaTSClass.GetCurentCheck();

            if (chk.Dishez.Any(d => d.BarCode == IdItem))
            {
                string messageIdStr = chk.Dishez.First(d => d.BarCode == IdItem).Name;
                if (!string.IsNullOrWhiteSpace(messageIdStr))
                {
                    if(messageIdStr.IndexOf(':') > -1)
                    {
                        string idStr = new string(messageIdStr.Substring(messageIdStr.IndexOf(':')).Where(c=>char.IsDigit(c)).ToArray());

                        if (!string.IsNullOrWhiteSpace(idStr))
                        {
                            long id = long.Parse(idStr);
                            
                            if(id > 0)
                            {
                                var customer = AuthWithoutWindow(id);
                                
                                if(customer!= null && customer.Auth)
                                {
                                    ShowInfohWnd(chk, customer);
                                }
                                else
                                {
                                    AlohaTSClass.TryDeleteItemOnCurentCheck(IdItem);
                                    ShowAuthWnd(chk);
                                }
                            }
                            else
                            {
                                AlohaTSClass.TryDeleteItemOnCurentCheck(IdItem);
                                ShowAuthWnd(chk);
                            }
                        }
                        else
                        {
                            AlohaTSClass.TryDeleteItemOnCurentCheck(IdItem);
                            ShowAuthWnd(chk);
                        }
                    }
                    else
                    {
                        AlohaTSClass.TryDeleteItemOnCurentCheck(IdItem);
                        ShowAuthWnd(chk);
                    }                  
                }
                else
                {
                    AlohaTSClass.TryDeleteItemOnCurentCheck(IdItem);
                    ShowAuthWnd(chk);
                }
            }
            else
            {
                ShowAuthWnd(chk);
            }            
        }

        public static void ShowAuthWnd(Check chk)
        {
            var authCtrl = new MB.PBAuthorization();
            var mW = PDSystem.ModalWindowsForegraund.GetModalWindow(authCtrl);
            authCtrl.SetOwnerWnd(mW);

            if (!(bool)mW.ShowDialog())
            {
                if (authCtrl.ProcUser != null && authCtrl.ProcUser.Auth)
                {
                    AlohaTSClass.AddDishToCurentChkVarName(IdItem, $"Id гостя:{authCtrl.ProcUser.PhoneStr}");
                    ShowInfohWnd(chk, authCtrl.ProcUser);
                }
            }
        }

        public static void ShowInfohWnd(Check chk,PBCustomerInfo customer)
        {
           
            PBUserInfoViewModel model = new PBUserInfoViewModel(customer);
            var viewCtrl = new MB.PBUserInfo() { DataContext = model };

            var vmW = PDSystem.ModalWindowsForegraund.GetModalWindow(viewCtrl);

            model.SetChkSumm(chk.Summ);
            model.SetOwnerWnd(vmW);

            if (model.CloseAction == null)
            {
                model.CloseAction = new Action(() => vmW.Close());
            }

            if (model.RemoveGuestAction == null)
            {
                model.RemoveGuestAction = new Action(() => { AuthClear(); vmW.Close(); });
            }

            vmW.ShowDialog();
        }


        internal static MBProxi.PBCustomerInfo AuthWithoutWindow(long id)
        {
            MBProxi.PBCustomerInfo result = null;

            try
            {
                if (id > 0)
                {
                    using (CardServiceClient client = GetWCFClient())
                    {

                        if (client != null && client.Online())
                        {
                            var response = client.PBCustomerAuth(id);

                            if (response != null && response.Result != null)
                            {
                                if (response.Result.Auth)
                                {
                                    result = response.Result;
                                }
                            }
                        }
                    }
                }
            }
            catch { }

            return result;
        }

        internal static void AuthClear()
        {          
            try
            {
                AlohaTSClass.TryDeleteItemOnCurentCheck(IdItem);
            }
            catch { }           
        }

        private static CardServiceClient GetWCFClient()
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
