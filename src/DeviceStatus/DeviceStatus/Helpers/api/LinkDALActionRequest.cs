using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DeviceStatus.Helpers.api
{
    public partial class LinkDALActionRequest
    {
        public LinkDALActionType? DALAction { get; set; }

        //public LinkDeviceUIRequest DeviceUIRequest { get; set; }

        public string HeldCardDataID { get; set; }
    }

    //DAL action selection
    [JsonConverter(typeof(StringEnumConverter))]
    public enum LinkDALActionType
    {
        StartADAMode,
        EndADAMode,
        DeviceUI,
        GetStatus,
        GetIdentifier,
        StartPreSwipeMode,
        WaitForCardPreSwipeMode,
        EndPreSwipeMode,
        PurgeHeldCardData
    }
}
