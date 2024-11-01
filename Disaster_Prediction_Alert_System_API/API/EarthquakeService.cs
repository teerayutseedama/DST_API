using Disaster_Prediction_Alert_System_API.DataView;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Nodes;
using Newtonsoft.Json;
using Serilog;
namespace Disaster_Prediction_Alert_System_API.API
{


    public class EarthquakeService
    {
        private readonly HttpClient _httpClient;
        private readonly IRedisCacheService _cacheService;
        public EarthquakeService(HttpClient httpClient, IRedisCacheService cacheService)
        {
            _httpClient = httpClient;
            _cacheService = cacheService;
        }
        //
        public async Task<EarthquakeDataView> GetRecentEarthquakesAsync()
        {
            var earthquakeData = new EarthquakeDataView();
         
            try
            {
                earthquakeData = await _cacheService.GetCacheValueAsync<EarthquakeDataView>("earthquakeData");
                if (earthquakeData == null)
                {
                    var requestUri = "https://earthquake.usgs.gov/earthquakes/feed/v1.0/summary/all_day.geojson";

                    var response = await _httpClient.GetAsync(requestUri);

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        earthquakeData = System.Text.Json.JsonSerializer.Deserialize<EarthquakeDataView>(content, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });
                        await _cacheService.SetCacheValueAsync<EarthquakeDataView>("earthquakeData", earthquakeData, TimeSpan.FromMinutes(15));

                    }
                }
                
                return earthquakeData;
            }
            catch (Exception ex)
            {
                Log.Error($"GetRecentEarthquakesAsync {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")} Error:{ex.Message}");
                 throw ;

            }



        }
        public static double? FindMagnitudeByCoordinates(EarthquakeDataView earthquakeData, string net)
        {
            var data = JsonNode.Parse(JsonConvert.SerializeObject(earthquakeData));
            var features = data["Features"]?.AsArray();

            foreach (var feature in features)
            {
                var nets = feature?["Properties"]?["Net"];

                if (nets != null &&
                    nets?.GetValue<string>() == net
                    )
                {
                    var mag= feature?["Properties"]?["Mag"]?.GetValue<double>();
                    return mag;
                }
            }

            return null;
        }

    }




    public class EarthquakeData
    {
        public string Type { get; set; }
        public Metadata Metadata { get; set; }
        public Feature[] Features { get; set; }
        public double[] bbox { get; set; }
    }

    public class Metadata
    {
        public long Generated { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
        public int Status { get; set; }
        public string Api { get; set; }
        public int Count { get; set; }


    }

    public class Feature
    {
        public string Type { get; set; }
        public EarthquakeProperties Properties { get; set; }
    }

    public class EarthquakeProperties
    {
        public double Mag { get; set; }
        public string Place { get; set; }
        public long Time { get; set; }
        public long Updated { get; set; }
        public int Tz { get; set; }
        public string Url { get; set; }
    }


}