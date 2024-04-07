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
    public class ApiResponse
    {
        public ApiResponse() { }

        public async Task<string> GetApi(ImportFileModel stockdetail)
        {
            string apiResponse = null;
            try
            {
                // Call the method to get data from the API
                apiResponse = await CallApiAndGetResponse(stockdetail);

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
            return apiResponse;

        }

        private async Task<string> CallApiAndGetResponse(ImportFileModel stockdetail)
        {
            string responseBody = null;
            using (HttpClient client = new HttpClient())
            {
                // API endpoint URL

                // string apiUrl = "https://groww.in/v1/api/charting_service/v2/chart/delayed/exchange/NSE/segment/CASH/ADANIPORTS?endTimeInMillis=1711559723102&intervalInMinutes=1&startTimeInMillis=1711132200000";

                var currentTime = getCurrentEpochTime();
                var startTimeInMilliSeconds = getStartTimeInMilliSeconds();
                var endTimeInMilliSeconds = getEndTimeInMilliSeconds();

                //string apiUrl = $"https://groww.in/v1/api/charting_service/v2/chart/delayed/exchange/NSE/segment/CASH/ADANIPORTS?endTimeInMillis={endTimeInMilliSeconds}&intervalInMinutes=1440&startTimeInMillis={startTimeInMilliSeconds}";

                //apiUrl = "https://groww.in/v1/api/charting_service/v2/chart/delayed/exchange/NSE/segment/CASH/ADANIPORTS?endTimeInMillis=1712403537368&intervalInMinutes=1140&startTimeInMillis=1554748200000";
                string apiUrl = $"https://groww.in/v1/api/charting_service/v2/chart/delayed/exchange/NSE/segment/CASH/{stockdetail.stockCodeName}?endTimeInMillis={endTimeInMilliSeconds}&intervalInMinutes=1440&startTimeInMillis={startTimeInMilliSeconds}";
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
                    //Console.WriteLine(responseBody);
                    //if(responseBody.Contains("candles"))
                    //{
                    //    break;
                    //}
                }
                //else
                //{
                //    // Handle unsuccessful response
                //    throw new Exception($"Failed to call the API. Status code: {response.StatusCode}");
                //}
            }
            return responseBody;
        }

        private string getEndTimeInMilliSeconds()
        {
            DateTime customTime = new DateTime(2024, 04, 05, 09, 35, 00); // Year, Month, Day, Hour, Minute, Second


            TimeZoneInfo istZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
            DateTime istTime = TimeZoneInfo.ConvertTimeFromUtc(customTime, istZone);
            // Convert custom time to Unix Epoch time (seconds since January 1, 1970)
            long epochTime = (long)(customTime - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
            return epochTime.ToString();
        }

        private string getStartTimeInMilliSeconds()
        {
            DateTime customTime = new DateTime(2024, 03, 18, 09, 20, 00); // Year, Month, Day, Hour, Minute, Second


            TimeZoneInfo istZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
            DateTime istTime = TimeZoneInfo.ConvertTimeFromUtc(customTime, istZone);
            // Convert custom time to Unix Epoch time (seconds since January 1, 1970)
            long epochTime = (long)(customTime - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
            return epochTime.ToString();
        }

        private string getCurrentEpochTime()
        {
            DateTime utcNow = DateTime.UtcNow;
            TimeZoneInfo istZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
            DateTime istTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, istZone);

            long epochTime = (long)(istTime - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;

            return epochTime.ToString();
        }

        public async Task<string> GetStockCodeName(string Url)
        {
            string stockCodeNameResponse = null;
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
                    //Console.WriteLine(responseBody);
                    //if(responseBody.Contains("candles"))
                    //{
                    //    break;
                    //}
                }
                //else
                //{
                //    // Handle unsuccessful response
                //    throw new Exception($"Failed to call the API. Status code: {response.StatusCode}");
                //}
            }

            return stockCodeNameResponse;
        }

        
    }
}
