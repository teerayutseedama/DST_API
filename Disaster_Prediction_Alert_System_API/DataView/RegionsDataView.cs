namespace Disaster_Prediction_Alert_System_API.DataView
{
    public class RegionsDataView
    {
    }
    public class IRegionsReq
    {
        public string RegionID { get; set; }
        public LocationCoordinates? LocationCoordinates { get; set; }
        public string[]? DisasterTypes{ get; set; }
    }

    public class IRegionsRes
    {
    }
    public class LocationCoordinates
    {
       public double Latitude { get; set; }
        public double Longitude { get; set; }
    }





}
