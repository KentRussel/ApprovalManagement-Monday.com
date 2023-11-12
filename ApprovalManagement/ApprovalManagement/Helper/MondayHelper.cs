using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;

namespace ApprovalManagement.Helper
{
    public class MondayHelper

    {
        private const string MondayApiKey = "eyJhbGciOiJIUzI1NiJ9.eyJ0aWQiOjE2NjA2MTI1NSwiYWFpIjoxMSwidWlkIjoxNzM1ODE1NCwiaWFkIjoiMjAyMi0wNi0xN1QwNTo0NjowMC4wMDBaIiwicGVyIjoibWU6d3JpdGUiLCJhY3RpZCI6Njc3NzM2MCwicmduIjoidXNlMSJ9.kHYwChnla-HsIRnc8fvtr1x4O8jp6xFLz-XRPZyYQNg";
        private const string MondayApiUrl = "https://api.monday.com/v2/";
        
        public async Task<string> QueryMondayApiV2(string query)

        {
            byte[] dataBytes = System.Text.Encoding.UTF8.GetBytes(query);

            HttpWebRequest request = (HttpWebRequest) WebRequest.Create(MondayApiUrl);
            request.ContentType = "application/json";
            request.Method = "POST";
            request.Headers.Add("Authorization", MondayApiKey);

            using (Stream requestBody = request.GetRequestStream())

            {
                await requestBody.WriteAsync(dataBytes, 0, dataBytes.Length);
            }

            using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))

            {
                return await reader.ReadToEndAsync();
            }
        }
    }
}