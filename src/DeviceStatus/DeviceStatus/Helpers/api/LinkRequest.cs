using System.Collections.Generic;

namespace DeviceStatus.Helpers.api
{
    public class LinkRequest
    {
        public string MessageID { get; set; }
        public long TCCustID { get; set; }
        public string TCPassword { get; set; }
        public bool? Synchronous { get; set; }
        public int? Timeout { get; set; }
        public string AgencyKey { get; set; }
        public string IPALicenseKey { get; set; }
        public string UserName { get; set; }

        public List<LinkActionRequest> Actions { get; set; }
    }
}
