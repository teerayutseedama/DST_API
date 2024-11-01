using System;
using System.Collections.Generic;

namespace Disaster_Prediction_Alert_System_API;

public partial class TbAlertSetting
{
    public string Id { get; set; } = null!;

    public string? RegionId { get; set; }

    public string? DisasterType { get; set; }

    public int? ThresholdScore { get; set; }
}
