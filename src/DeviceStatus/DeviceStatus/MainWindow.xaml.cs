using DeviceStatus.Helpers;
using DeviceStatus.Helpers.RequestBuilders;
using DeviceStatus.Helpers.Session;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;

namespace DeviceStatus
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SessionData _sessionData;
        private bool responseReceived;
        private bool requestIsActive;
        private System.Timers.Timer intervalTimer = null;
        private int intervalTime = 0;

        public static string DALDNS;
        public static string UserName;
        public static string DALIpv4;
        public static string DALIpv6;
        public static string EndPoint;
        public static string SessionID;
        public static long CustId;
        public static string Password;
        public string DefCustID = ConfigurationManager.AppSettings["TCCustId"] ?? "1152702";
        public string DefPassword = ConfigurationManager.AppSettings["TCPassword"] ?? "testipa1";
        public string DefInterval = ConfigurationManager.AppSettings["Interval"] ?? "1000";

        public MainWindow()
        {
            InitializeComponent();
            Initialize();
        }

        private void Initialize()
        {
            CustIdTB.Text = DefCustID;
            PasswordTB.Text = DefPassword;
            IntervalTB.Text = DefInterval;

            _sessionData = new SessionData(RefreshEvent);

            ResponseTB.Text = "Press 'Start' to submit request...";

            SessionID = SessionData.GetSessionID();

            EndPoint = _sessionData?.PostURL ?? "No EndPoint Set";

            if (DALDNS == null)
                DALDNS = Helper.GetHostName();

            if (UserName == null)
                UserName = Environment.UserName;

            if (DALIpv4 == null)
                DALIpv4 = Helper.GetIPv4();

            if (DALIpv6 == null)
                DALIpv6 = Helper.GetIPv6();
        }

        private void Dialog_Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            StopIntervalTimer();

            // Make another request
            if (requestIsActive)
            {
                Task.Run(() =>
                {
                    this.Dispatcher.Invoke((Action)delegate ()
                    {
                        GetDalStatus();
                    });
                });
            }
        }

        private void StartIntervalTimer()
        {
            //Debug.WriteLine("start interval timer...");
            intervalTimer = new System.Timers.Timer();
            intervalTimer.Interval = intervalTime;
            intervalTimer.Elapsed += new ElapsedEventHandler(Dialog_Timer_Elapsed);
            intervalTimer.Start();
        }

        private void StopIntervalTimer()
        {
            //Debug.WriteLine("stop interval timer...");
            intervalTimer?.Stop();
            intervalTimer?.Dispose();
            intervalTimer = null;
        }

        private void StartProgressNotification()
        {
            int itr = intervalTime / 100;

            for (int i = 0; i < 5; i++)
            {
                Thread.Sleep(10);
                this.Dispatcher.Invoke(() =>
                {
                    RequestProgress.Value = i;
                });
            }

            Task.Run(() =>
            {
                //Debug.WriteLine("progressbar updating...");
                Thread.Sleep(750);
                for (int i = 0; i < intervalTime; i += itr)
                {
                    Thread.Sleep(10);
                    this.Dispatcher.Invoke(() =>
                    {
                        RequestProgress.Value = i;
                    });
                }
                //Debug.WriteLine("progressbar done!");
            });
        }

        private string GenerateRequest(string action)
        {
            LinkRequestDALStatus.MessageID = Samples.BuildRandomString(7) + "TstHrns";
            LinkRequestDALStatus.DALDNS = DALDNS;
            LinkRequestDALStatus.SessionID = SessionID;
            LinkRequestDALStatus.DALIPv4 = DALIpv4;
            LinkRequestDALStatus.DALIPv6 = DALIpv6;
            LinkRequestDALStatus.UserName = UserName;

            long.TryParse(CustIdTB.Text, out long tempCustid);

            if (tempCustid <= 0)
            {
                MessageBox.Show("Invalid Custid");
                return "Invalid Custid";
            }

            // Variables
            CustId = tempCustid;
            Password = PasswordTB.Text;


            string output = "ERROR";
            if (action.Equals("Get Status"))
            {
                output = LinkRequestDALStatus.GenerateStatusRequest(Convert.ToInt32(CustIdTB.Text), PasswordTB.Text);
            }
            else
            {
                MessageBox.Show($"Action: {action} is not yet implemented.");
            }
            return output;
        }

        private void GetDalStatus()
        {
            StartProgressNotification();

            string request = GenerateRequest("Get Status");
            ActionBtn.Content = "Stop";
            ResponseTB.Text = $"Request Submitted to '{EndPoint}', awaiting results...";
            var curRequest = Helper.DeserializeLinkRequest(request);
            if (curRequest == null)
            {
                ResponseTB.Text = "Error in Test Harness.\n\n\nUnable to create LinkRequest from the Text in the Request Box!\n\n\nPlease double check it is properly formatted.";
                MessageBox.Show("Error Parsing LinkRequest.  Please use a different tool to submit poorly formatted LinkRequests.");
                return;
            }
            SessionData.RegisterMessage(curRequest.MessageID, LoadResponseRTB);
            _ = Helper.SendRequest(curRequest.MessageID, EndPoint, request);

            responseReceived = false;
        }

        private void SetControlsMode(bool mode)
        {
            CustIdTB.IsEnabled = mode;
            PasswordTB.IsEnabled = mode;
            IntervalTB.IsEnabled = mode;
        }

        private void Action_Click(object sender, RoutedEventArgs e)
        {
            if (string.Equals(ActionBtn.Content.ToString(), "Start", StringComparison.OrdinalIgnoreCase))
            {
                SetControlsMode(false);
                requestIsActive = true;
                intervalTime = Convert.ToInt32(IntervalTB.Text);
                GetDalStatus();
            }
            else
            {
                StopRequestInterval();
            }
        }

        private void StopRequestInterval()
        {
            requestIsActive = false;
            StopIntervalTimer();
            RequestProgress.Value = 0;
            ActionBtn.Content = "Start";
            ResponseTB.Text = "Press 'Start' to submit request...";
            SetControlsMode(true);
        }

        public void RefreshEvent()
        {
            var events = _sessionData.GetEventData("Event");
        }

        public void LoadResponseRTB(string theJson)
        {
            if (!responseReceived)
            {
                responseReceived = true;

                string responseString = theJson;

                if (theJson.StartsWith("ERROR|"))
                {
                    responseString = theJson.Substring(6);
                    StopIntervalTimer();
                    StopRequestInterval();
                }
                else
                { 
                    StartIntervalTimer();
                }

                this.Dispatcher.Invoke((Action)delegate ()
                {
                    LoadResponseRTB(responseString);
                    ResponseTB.Text = responseString;
                });
            }
        }
    }
}
