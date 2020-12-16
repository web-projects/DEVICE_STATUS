using DeviceStatus.Helpers.api;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DeviceStatus.Helpers.Session
{
    public class SSESubscriber : IDisposable
    {
        private const string EVENT_STREAM_TYPE = "text/event-stream";

        private readonly HttpClient httpClient;
        public string ServerUrl { get; }

        private bool isDisposed = false;

        public delegate void OnMessageReceivedDelegate(string serializedMessage);
        public delegate void OnDisconnectDelegate();

        public event OnMessageReceivedDelegate OnMessageReceived;
        public event OnDisconnectDelegate OnDisconnect;

        private string sessionId;

        public string SessionId => sessionId;

        public SSESubscriber(string serverUrl) =>
            (ServerUrl, httpClient) = (serverUrl, new HttpClient(new HttpClientHandler()));

        public async Task Initialize(LinkRequest linkRequest)
        {
            var response = await httpClient.PostAsync(ServerUrl + "/InitSession", new StringContent(JsonConvert.SerializeObject(linkRequest), Encoding.UTF8, "application/json"));
            string body = await response.Content.ReadAsStringAsync();
            TCLinkResponse linkResponse = JsonConvert.DeserializeObject<TCLinkResponse>(body);
            foreach (var i in linkResponse.Responses)
            {
                // get the session id 
                if (i.SessionResponse != null)
                {
                    sessionId = i.SessionResponse.SessionID;
                    break;
                }
            }
        }

        /// <summary>
        /// Initialize a session with an existing session id.
        /// </summary>
        /// <param name="sessionId"></param>
        public void Initialize(string sessionId)
        {
            this.sessionId = sessionId;
        }

        public async Task Subscribe()
        {
            if (isDisposed)
                throw new ObjectDisposedException($"{nameof(SSESubscriber)} is currently disposed.");

            try
            {
                httpClient.DefaultRequestHeaders.Add("Accept", EVENT_STREAM_TYPE);
                httpClient.DefaultRequestHeaders.Add("Connection", "keep-alive");
                httpClient.DefaultRequestHeaders.Add("X-Last-Session-ID", sessionId);

                var connectURL = $"{ServerUrl}/Connect";

                using (Stream stream = await httpClient.GetStreamAsync(connectURL))
                {

                    using (StreamReader reader = new StreamReader(stream))
                    {
                        for (; ; )
                        {
                            string buffer = await reader.ReadLineAsync();
                            if (!string.IsNullOrWhiteSpace(buffer))
                            {
                                try
                                {
                                    MessageReceived(buffer);
                                }
                                catch (Exception xcp)
                                {
                                    Console.WriteLine(xcp.Message);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception xcp)
            {
                Console.WriteLine(xcp.Message);
                OnDisconnect();
            }
            Console.WriteLine("Adios");
        }

        private void MessageReceived(string serializedMessage)
        {
            OnMessageReceived?.Invoke(serializedMessage);
        }

        private void Disconnect()
        {
            OnDisconnect?.Invoke();
        }

        public void Dispose()
        {
            isDisposed = true;

            httpClient?.CancelPendingRequests();
            httpClient?.Dispose();
        }
    }

}
