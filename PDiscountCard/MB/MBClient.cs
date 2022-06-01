using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using PDiscountCard.MBProxi;
using System.IO;

namespace PDiscountCard.MB
{
    public class MBClient
    {
        public MBClient()
        {
            client = GetWCFClient();

        }
        CardServiceClient client = null;

        public bool UsingMB()
        {
            return true;
            if (client == null) return false;
            try
            {
                bool res = client.Online();
                Utils.ToCardLog("UsingMB " + res);
                return res;
            }
            catch (Exception e)
            {
                Utils.ToCardLog("Error UsingMB " + e.Message);
                return false;
            }
        }

        public struct SettQRTips
        {

            public int tips_type;
            public int head_place_code;
        }

        private void SaveData(SettQRTips sett)
        {
            string path = @"C:\aloha\alohats\tmp\sett_tips.tmp";         

            using (StreamWriter fs = File.CreateText(path))
            {
                fs.WriteLine(sett.tips_type.ToString());
                fs.WriteLine(sett.head_place_code.ToString());
            }
        }


        public void GetSettingTips()
        {

            SettQRTips sett = new SettQRTips
            {
                tips_type = -1,
                head_place_code = 0
            };
            var request = new TermSettingsRequest
            {
                DepartmentNumber = AlohainiFile.DepNum,
                TerminalNumber = AlohaTSClass.GetTermNum(),
            };

            var client = GetWCFClient();
            try
            {
                var res = client.GetSettingsForTerm(request);

                if (res.Success)
                {
                    sett.tips_type = res.Result.Main.QRTipsType;
                    sett.head_place_code = res.Result.Main.QRTipsTypeHeadPlaceCode;
                    SaveData(sett);
                }
            }
            catch (Exception ex)
            {

            }

        }

        public int GetFrendConvertCodeCardProcessing(Check check, string prefix, string number, out int CountV, out int CountD, out int VisitTotal, out int DayTotal, out int compId, out bool showValidateMess)
        {
            VisitTotal = 100;
            DayTotal = 365+60;
            CountV = 0;
            CountD = 0;
            compId = 0;
            showValidateMess = false;
            // VisitTotal = 0;
            //DayTotal = 0;
            var res = CardProcessing(check,prefix, number);
            
            if (res == null || res.Result == null)
            {
                return -1;
            }
            showValidateMess = !res.Result.DataConfirmed;
            compId = res.Result.CompId;
            /*
            if (res.Result.TemporarilyInactive)
            {//"Карта временно заблокирована."
                CountV = -7;
                return 1;
            }
            */
            if (!res.Result.CardRegistered) // не полтинник
            {//"Карта не зарег.."
                CountV = -5;
                return 1;
            }

            if (!res.Result.Active) // полтинник , но не работает для этого подразделения
            {//"Карта заблокирована."
                CountV = -1;
                return 1;
            }
            
            if (!res.Result.PurchaseIsRegistered || !res.Result.VisitCounterIncreased)
            {
                CountV = -3;
                return 1;
            }
            if (res.Result.DaysLeft < 0)
            {
                CountV = -2;
                return 1;
            }

            if (res.Result.DaysLeft < 0)
            {
                CountV = -2;
                return 1;
            }

            if (res.Result.NumberOfVisits == 100)
            {
                CountV = 1;
                return 1;
            }
            CountV = VisitTotal - res.Result.NumberOfVisits;
            CountD = res.Result.DaysLeft;
            return 1;

        }

        private string GetResStr(MyCardInfo info)
        {
            try
            {
                string str = "";
                str += "info.Active:" + info.Active + Environment.NewLine;
                str += "info.CardRegistered:" + info.CardRegistered + Environment.NewLine;
                str += "info.DaysLeft:" + info.DaysLeft + Environment.NewLine;
                str += "info.Discount:" + info.Discount + Environment.NewLine;
                str += "info.FirstVisitDate:" + info.FirstVisitDate + Environment.NewLine;
                str += "info.NumberOfVisits:" + info.NumberOfVisits + Environment.NewLine;
                str += "info.PersonsFullName:" + info.PersonsFullName + Environment.NewLine;
                str += "info.PurchaseIsRegistered:" + info.PurchaseIsRegistered + Environment.NewLine;
                str += "info.ShowInfo:" + info.ShowInfo + Environment.NewLine;
                str += "info.TemporarilyInactive:" + info.TemporarilyInactive + Environment.NewLine;
                str += "info.VisitCounterIncreased:" + info.VisitCounterIncreased + Environment.NewLine;
                return str;
            }
            catch(ExecutionEngineException e)
            {
                return "info error " +e.Message;
            }
            
        }

        public MBProxi.OperationResultOfMyCardInfo CardProcessing(Check check, string prefix, string number)
        {
            if (client == null) return null;
            try
            {

                MBProxi.ProcessedCheck data = new MBProxi.ProcessedCheck()
                            {
                                DepartmentNumber = AlohainiFile.DepNum,
                            };
                prefix = prefix.Replace("PRE", "86738");
                prefix = prefix.Replace("ZAV", "90658");
                prefix = prefix.Replace("VIP", "80826");
                if (number.Length > 9)
                {
                    number = number.Substring(number.Length - 9);
                }
                data.UsedCardNumber = prefix.PadLeft(5, '0') + number.PadLeft(9, '0');
                data.GestoryCheckNumber = check.CheckNum;
                 data.AlohaCheckNumber  = check.AlohaCheckNum;
                 data.CheckAmount = check.Summ;
                 data.WaiterNumber = check.Waiter;
                 data.TerminalNumber = AlohaTSClass.GetTermNum();      

                Utils.ToCardLog("data.UsedCardNumber : " + data.UsedCardNumber);
                var res = client.CardProcessingNew(data);
                Utils.ToCardLog("CardProcessing.Success: " + res.Success );
                if (res.Success &&res.Result!=null)
                {
                    Utils.ToCardLog("CardProcessing.Result: " + Environment.NewLine + GetResStr(res.Result));
                }

                if ((!res.Success)&&(res.Errors!=null))
                {
                    foreach (var err in res.Errors)
                    {
                        Utils.ToCardLog("CardProcessing.Error: " + err.ErrorMessage);
                    }
                }
                return res;
            }
            catch (Exception e)
            {
                Utils.ToCardLog("Error UsingMB " + e.Message);
                return null;
            }
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
