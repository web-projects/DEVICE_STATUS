using System;

namespace DeviceStatus.Helpers.api
{
    public partial class LinkDALRequest
    {
        public LinkDALIdentifier DALIdentifier { get; set; }

        public LinkDeviceIdentifier DeviceIdentifier { get; set; }

        public Guid? HeldCardDataID { get; set; }
    }
}
