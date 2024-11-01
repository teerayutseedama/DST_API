using System;
using System.Collections.Generic;

namespace Disaster_Prediction_Alert_System_API;

public partial class TbAlert
{
    public string Id { get; set; } = null!;

    public string? RegionId { get; set; }

    public string? DisasterType { get; set; }

    public string? RiskLevel { get; set; }

    public string? AlertMessage { get; set; }

    public DateTime? Timestamp { get; set; }
}
