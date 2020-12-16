using DeviceStatus.Helpers.api;
using DeviceStatus.Helpers.Session;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace DeviceStatus.Helpers
{
    public static class Helper
    {
        public static string GetIPv4()
        {
            var host = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("Local IP Address Not Found!");
        }

        public static string GetIPv6()
        {
            var host = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("Local IP Address Not Found!");
        }

        public static string GetHostName()
        {
            string ClientName = System.Net.Dns.GetHostName();
            return ClientName;
        }

        public static void SetSessionID(LinkRequest myRequest, string SessionID)
        {
            foreach (var action in myRequest.Actions)
            {
                action.SessionID = SessionID;
            }
        }

        public static string Stringify(object theObject)
        {
            return JsonConvert.SerializeObject(theObject, Formatting.Indented, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
        }

        public static LinkRequest DeserializeLinkRequest(string theJSON)
        {
            LinkRequest theRequest = null;
            try
            {
                theRequest = JsonConvert.DeserializeObject<LinkRequest>(theJSON);
            }
            catch (Exception xcp)
            {
                System.Diagnostics.Debug.WriteLine(xcp.Message);
            }
            return theRequest;
        }

        public static TCLinkResponse DeserializeLinkResponse(string theJSON)
        {
            TCLinkResponse theResponse = null;
            try
            {
                theResponse = JsonConvert.DeserializeObject<TCLinkResponse>(theJSON);
            }
            catch (Exception xcp)
            {
                System.Diagnostics.Debug.WriteLine(xcp.Message);
            }
            return theResponse;
        }

        private static async Task<string> CallWebService(string endpoint, string contents, int timeout = 100)
        {
            using HttpClient client = HttpClientFactory.Create();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
            client.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");
            client.Timeout = new TimeSpan(0, 0, 0, timeout);

            using StringContent content = new StringContent(contents, Encoding.UTF8, "application/json");
            content.Headers.ContentType = new MediaTypeHeaderValue("text/json");

            using HttpResponseMessage httpResponse = await client.PostAsync(endpoint, content).ConfigureAwait(false);
            return await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        }

        public static async Task SendRequest(string messageId, string endpoint, string contents, int timeout = 100)
        {
            if (endpoint?.StartsWith("http", StringComparison.OrdinalIgnoreCase) ?? false)
            {
                try
                {
                    var response = await CallWebService(endpoint, contents, timeout);
                    SessionData.SawOutput("SSEOutput", response);
                }
                catch (Exception xcp)
                {
                    //JsonMessageBox.Show("Error", xcp.Message);
                    SessionData.SawOutputError(messageId, xcp.Message);
                }
                return;
            }
            System.Diagnostics.Debug.WriteLine($"Unable to send message to {endpoint} :: {contents}");
        }
    }
}
