using AutoShare.Commands;
using AutoShare.Helper;
using AutoShare.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;

namespace AutoShare.ApiResponseHandler
{
    public class ApiResponse : BindableBase
    {

        public ApiResponse() { }

        private LogHelper _logger = new LogHelper();

        public LogHelper Logger
        {
            get { return _logger; }
            set { SetProperty(ref _logger, value); }
        }
        public async Task<string> GetApi(ImportFileModel stockdetail, int stockInMinute)
        {
            string apiResponse = null;
            try
            {
                // Call the method to get data from the API
                apiResponse = await CallApiAndGetResponse(stockdetail,stockInMinute);

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
            return apiResponse;

        }

        private async Task<string> CallApiAndGetResponse(ImportFileModel stockdetail, int stockInMinute)
        {
            string responseBody = null;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var startTimeInMilliSeconds = getStartTimeInMilliSeconds();
                    var endTimeInMilliSeconds = getEndTimeInMilliSeconds();

                    string apiUrl = $"https://groww.in/v1/api/charting_service/v2/chart/delayed/exchange/NSE/segment/CASH/{stockdetail.stockCodeName}?endTimeInMillis={endTimeInMilliSeconds}&intervalInMinutes={stockInMinute}&startTimeInMillis={startTimeInMilliSeconds}";
                    //Headers
                    client.DefaultRequestHeaders.Host = "groww.in";
                    client.DefaultRequestHeaders.Connection.Add("keep-alive");
                    client.DefaultRequestHeaders.Referrer = new Uri(stockdetail.stockUrl);
                    client.DefaultRequestHeaders.Add("Accept", "application/json");
                    client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/123.0.0.0 Safari/537.36");

                    client.DefaultRequestHeaders.Add("X-APP-ID", "growwWeb");
                    client.DefaultRequestHeaders.Add("x-platform", "web");



                    // Make a GET request to the API
                    HttpResponseMessage response = await client.GetAsync(apiUrl);

                    // Check if the response is successful
                    if (response.IsSuccessStatusCode)
                    {
                        // Read the response content as string
                        responseBody = await response.Content.ReadAsStringAsync();

                    }
                }
            }
            catch (Exception Ex)
            {
                Logger.Log($"Exception= {Ex.Message}");
            }
            return responseBody;
        }

        private string getEndTimeInMilliSeconds()
        {
            DateTime customTime = DateTime.Now;
            long epochTime = (long)(customTime - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
            return epochTime.ToString();
        }

        private string getStartTimeInMilliSeconds()
        {
            //substracting data because we need last 14days for data for working trading days. 
            DateTime customTime = DateTime.Now.AddDays(-30);
            long epochTime = (long)(customTime - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
            return epochTime.ToString();
        }


        public async Task<string> GetStockCodeName(string Url)
        {
            string stockCodeNameResponse = null;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    //Headers
                    client.DefaultRequestHeaders.Host = "groww.in";
                    client.DefaultRequestHeaders.Connection.Add("keep-alive");
                    client.DefaultRequestHeaders.Add("Accept", "application/json");
                    client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/123.0.0.0 Safari/537.36");

                    client.DefaultRequestHeaders.Add("Sec-Fetch-Site", "same-origin");
                    client.DefaultRequestHeaders.Add("Sec-Fetch-Mode", "navigate");
                    client.DefaultRequestHeaders.Add("Sec-Fetch-User", "?1");
                    client.DefaultRequestHeaders.Add("Sec-Fetch-Dest", "document");

                    // Make a GET request to the API
                    HttpResponseMessage response = await client.GetAsync(Url);

                    // Check if the response is successful
                    if (response.IsSuccessStatusCode)
                    {
                        // Read the response content as string
                        stockCodeNameResponse = await response.Content.ReadAsStringAsync();
                        
                    }
                }
            }
            catch (Exception Ex)
            {
                Logger.Log($"Exception= {Ex.Message}");
            }

            return stockCodeNameResponse;
        }

        
    }
}
