using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Disaster_Prediction_Alert_System_API.DataView;
using Microsoft.IdentityModel.Tokens;
using Serilog;
//using Twilio.Http;
namespace Disaster_Prediction_Alert_System_API.API
{
    public class OpenWeatherAPI
    {
        private readonly HttpClient _httpClient;
        private readonly IRedisCacheService _cacheService;
        public OpenWeatherAPI( IRedisCacheService cacheService, HttpClient httpClient)
        {
            _cacheService = cacheService;
            _httpClient = httpClient;
        }
        public async Task<WeatherResponse> GetWeatherData(string RegionID, double lat,double lon ,string exclude="",string lang="")
        {
            var result =new WeatherResponse();
            result =await _cacheService.GetCacheValueAsync<WeatherResponse>($"weather{RegionID}");
            var apiKey = "351e6ab0dc4ef26908a40e9a90bfe2ce"; 
            var ex = "";
            var la = "";
            if (!exclude.IsNullOrEmpty()) {
                ex = "&exclude="+exclude;
            }
            if (!lang.IsNullOrEmpty())
            {
                la = "&lang="+lang;
            }
            var url = "https://api.openweathermap.org/data/3.0/onecall?lat=" + lat + "&lon=" + lon + "&appid=" + apiKey + ex + "&units=metric";
            using var httpClient = new HttpClient();

            try
            {
                if (result == null)
                {
                    var response = await httpClient.GetAsync(url);
                    response.EnsureSuccessStatusCode();

                    var json = await response.Content.ReadAsStringAsync();
                    result = JsonConvert.DeserializeObject<WeatherResponse>(json);
                    await _cacheService.SetCacheValueAsync<WeatherResponse>($"weather{RegionID}", result, TimeSpan.FromMinutes(15));
                }
                
               
              
            }
            catch (HttpRequestException e)
            {
                Log.Error($"GetWeatherData {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")} Error:{e.Message}");
                throw;
               
            }
            return result;
        }



    }
}
