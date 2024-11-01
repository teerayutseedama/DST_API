using System;
using System.Collections.Generic;

namespace Disaster_Prediction_Alert_System_API;

public partial class TbRegion
{
    public string Id { get; set; } = null!;

    public string? RegionId { get; set; }

    public double? Latitude { get; set; }

    public double? Longitude { get; set; }

    public string? DisasterType { get; set; }
}
