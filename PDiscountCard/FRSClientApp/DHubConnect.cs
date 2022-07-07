
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;

namespace PDiscountCard.FRSClientApp
{
    class DHConnect
    {
        public decimal GetHubXReport(DateTime db, int depNUm, out int count)
        {
            count = 0;

            try
            {
                //var departmentNo = 205;
                var departmentNo = depNUm;
                var businesDate = DateTime.Now;

                var responseTask = Task.Run(() => GetDailyRevenueReport(departmentNo, businesDate));
                responseTask.Wait();

                var zz = Math.Abs(responseTask.Result.RefundTotal);
                var vv = responseTask.Result.Total;
                count = responseTask.Result.OrdersCount;
                Utils.ToCardLog($"GetHubXReport Total:{responseTask.Result.Total}; count:{responseTask.Result.OrdersCount}");


                return vv - zz;
            }
            catch (Exception e)
            {
                return -1;
            }
        }

        private static string hubDepartmentDailyCardRevenueUri = "https://s2020reserve/deliveryhub/api/info/DepartmentDailyCardRevenue?";
        private static string alohaAccessToken = "601de19e418f4c4a837303398c18c2ce";
        private async Task<DailyRevenueInfo> GetDailyRevenueReport(int departmentNo, DateTime? businesDate)
        {
            DailyRevenueInfo result = null;

            Utils.ToCardLog($"GetDailyRevenueReport departmentNo:{departmentNo}; businesDate: {businesDate.ToString()}");
            if (departmentNo > 0)
            {
                var requestDate = businesDate?.Date ?? DateTime.Now.Date;

                try
                {
                    ServicePointManager.ServerCertificateValidationCallback = new
                                RemoteCertificateValidationCallback(delegate { return true; });

                    using (var client = new HttpClient())
                    {
                        client.Timeout = TimeSpan.FromSeconds(3);
                        var request = new HttpRequestMessage()
                        {
                            RequestUri = new Uri(hubDepartmentDailyCardRevenueUri + $"departmentNo={departmentNo}&date={requestDate.ToString("yyyy-MM-dd")}"),
                            Method = HttpMethod.Get,
                            Headers = { { "Authorization", alohaAccessToken } }
                        };

                        var response = await client.SendAsync(request);

                        if (response != null && response.StatusCode == HttpStatusCode.OK)
                        {
                            var jsonContent = await response.Content.ReadAsStringAsync();

                            var dailyRevenueResponse = JsonConvert.DeserializeObject<GenericResponse<DailyRevenueInfo>>(jsonContent);

                            result = dailyRevenueResponse?.Result;
                        }
                    }
                }
                catch (Exception ex)
                {
                    var mess = ex.Message;
                }
            }
            Utils.ToCardLog($"GetDailyRevenueReport end result:{result.Total}; businesDate: {businesDate.ToString()}");

            return result;
        }

        public class BasicResponse
        {
            public bool Success { get; set; }
            public List<string> Errors { get; set; } = new List<string>();
        }

        public class GenericResponse<T> : BasicResponse
        {
            public T Result { get; set; }
        }

        public class DailyRevenueInfo
        {
            public int DepNum { get; set; }
            public DateTime BusinesDate { get; set; }
            public decimal Total { get; set; }
            public decimal RefundTotal { get; set; }
            public int OrdersCount { get; set; }


        }
    }
}
