using System.Buffers.Text;

namespace Disaster_Prediction_Alert_System_API.DataView
{
    public class IGetDisasterRiskDataViewRes
    {
     public string   RegionID { get; set; }
        public string DisasterType { get; set; }
        public int RiskScore { get; set; }
        public string RiskLevel { get; set; }
        public bool AlertTriggered { get; set; }
    }

    
}
