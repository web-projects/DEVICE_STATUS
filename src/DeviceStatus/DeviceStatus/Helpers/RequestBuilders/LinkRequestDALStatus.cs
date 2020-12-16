using DeviceStatus.Helpers.api;

namespace DeviceStatus.Helpers.RequestBuilders
{
    public static class LinkRequestDALStatus
    {
        public static string MessageID;
        public static string DALDNS;
        public static string UserName;
        public static string DALIPv4;
        public static string DALIPv6;
        public static string SessionID;
        public static string TCCustID;
        public static string Password;

        public static string GenerateStatusRequest(long custid = 0, string password = null)
        {
            LinkRequest myRequest = Samples.BuildLinkDALStatusRequest(false);  //Even though we are local, build as if not.
            if (MessageID != null) myRequest.MessageID = MessageID;
            myRequest.TCCustID = custid > 0 ? custid : myRequest.TCCustID;
            myRequest.TCPassword = password ?? myRequest.TCPassword;
            SetDALRequest(myRequest.Actions[0].DALRequest);
            Helper.SetSessionID(myRequest, SessionID);
            return Helper.Stringify(myRequest);
        }
        public static void SetDALRequest(LinkDALRequest request)
        {
            request.DALIdentifier.DnsName = DALDNS;
            request.DALIdentifier.WorkstationName = DALDNS;
            request.DALIdentifier.Username = UserName;
            request.DALIdentifier.IPv4 = DALIPv4;
            request.DALIdentifier.IPv6 = DALIPv6;
            request.DALIdentifier.LookupPreference = LinkDALLookupPreference.Username;
            MessageID = null;
        }
    }
}
