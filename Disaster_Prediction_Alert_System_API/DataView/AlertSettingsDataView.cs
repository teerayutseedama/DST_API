using System.Threading;

namespace Disaster_Prediction_Alert_System_API.DataView
{
    
    public class IAlertSettingsReq
    {
        public string RegionID { get; set; }
        public string DisasterType { get; set; }
        public int ThresholdScore { get; set; }
    }
}
