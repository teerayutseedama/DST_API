namespace Disaster_Prediction_Alert_System_API.DataView
{
    public class IAlertDataView
    {
        public string? RegionId { get; set; }

        public string? DisasterType { get; set; }

        public string? RiskLevel { get; set; }

        public string? AlertMessage { get; set; }

        public DateTime? Timestamp { get; set; }
    }
}
