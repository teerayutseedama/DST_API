using Disaster_Prediction_Alert_System_API.API;
using Disaster_Prediction_Alert_System_API.DataView;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Disaster_Prediction_Alert_System_API.Controllers
{
    [Route("api")]
    [ApiController]
    public class DisasterController : ControllerBase
    {
     
        private IHttpResponse _httpResponse;
        private readonly DisasterDbContext _dbContext;
        private readonly EarthquakeService _earthquakeService;
        private readonly OpenWeatherAPI _openWeather;
        protected readonly IConfiguration _configuration;
        public DisasterController(IConfiguration configuration, DisasterDbContext dbContext, EarthquakeService earthquakeService, OpenWeatherAPI openWeather)
        {
            _httpResponse = new IHttpResponse();
            _earthquakeService = earthquakeService;
            _dbContext = dbContext;
            _openWeather = openWeather;
            _configuration = configuration;
        }

        [HttpGet("disaster-risks")]
        public async Task<IEnumerable<IGetDisasterRiskDataViewRes>>  disasterrisks()
        {
            try
            {
                List<IGetDisasterRiskDataViewRes> results = new List<IGetDisasterRiskDataViewRes>();
                var list = await (from r in _dbContext.TbRegions
                                  from s in _dbContext.TbAlertSettings.Where(w => w.RegionId == r.RegionId)
                                  select new
                                  {
                                      RegionID = r.RegionId,
                                      DisasterType = r.DisasterType,
                                      RiskScore = s.ThresholdScore.Value,
                                      r.Latitude,
                                      r.Longitude,
                                  }).ToListAsync();

                EarthquakeDataView eartData = new EarthquakeDataView();
                foreach (var item in list)
                {
                    IGetDisasterRiskDataViewRes viewRes = new IGetDisasterRiskDataViewRes();

                    viewRes.RegionID = item.RegionID;
                    viewRes.DisasterType = item.DisasterType;
                    viewRes.RiskScore = item.RiskScore;
                    if (item.DisasterType == "earthquake")
                    {
                        eartData = await _earthquakeService.GetRecentEarthquakesAsync();

                        var mag = EarthquakeService.FindMagnitudeByCoordinates(eartData, item.RegionID.ToLower());
                        viewRes.RiskLevel = mag > item.RiskScore ? "High" : item.RiskScore == mag ? "Medium" : "Low";
                        viewRes.AlertTriggered = mag > item.RiskScore;
                        if (mag > item.RiskScore)
                        {
                            var alert = new TbAlert();
                            alert.Id = Guid.NewGuid().ToString();
                            alert.RegionId = item.RegionID;
                            alert.DisasterType = item.DisasterType;
                            alert.RiskLevel = viewRes.RiskLevel;
                            alert.AlertMessage = "Time:" + DateTime.Now.ToString("dd/MM/yyyy") + " Type:" + item.DisasterType + " Alert RiskLevel:" + viewRes.RiskLevel + " Region:" + item.RegionID;
                            alert.Timestamp = DateTime.Now;

                            await _dbContext.TbAlerts.AddAsync(alert);
                            await _dbContext.SaveChangesAsync();
                        }


                    }
                    if (item.DisasterType == "wildfire" || item.DisasterType == "flood")
                    {
                        var data = await _openWeather.GetWeatherData(item.RegionID, item.Latitude.Value, item.Longitude.Value, "", "th");

                        if (item.DisasterType == "wildfire")
                        {
                            string RiskLevel = "";
                            bool AlertTriggered = false;
                            var cure = data.Current;
                            if (cure.Temp > item.RiskScore && cure.Humidity < item.RiskScore)
                            {
                                RiskLevel = "High";
                                AlertTriggered = true;
                                var alert = new TbAlert();
                                alert.Id = Guid.NewGuid().ToString();
                                alert.RegionId = item.RegionID;
                                alert.DisasterType = item.DisasterType;
                                alert.RiskLevel = viewRes.RiskLevel;
                                alert.AlertMessage = "Time:" + DateTime.Now.ToString("dd/MM/yyyy") + " Type:" + item.DisasterType + " Alert RiskLevel:" + viewRes.RiskLevel + " Region:" + item.RegionID;
                                alert.Timestamp = DateTime.Now;

                                await _dbContext.TbAlerts.AddAsync(alert);
                                await _dbContext.SaveChangesAsync();

                            }
                            else if (cure.Temp > item.RiskScore && cure.Humidity > item.RiskScore)
                            {
                                RiskLevel = "Medium";
                            }
                            else
                            {
                                RiskLevel = "Low";
                            }
                            viewRes.RiskLevel = RiskLevel;
                            viewRes.AlertTriggered = AlertTriggered;
                        }
                        else
                        {
                            var rain = data.Daily.OrderBy(o => o.Dt).FirstOrDefault();
                            viewRes.RiskLevel = rain.Rain > item.RiskScore ? "High" : item.RiskScore == rain.Rain ? "Medium" : "Low";

                            viewRes.AlertTriggered = rain.Rain > item.RiskScore;
                            if (rain.Rain > item.RiskScore)
                            {
                                var alert = new TbAlert();
                                alert.Id = Guid.NewGuid().ToString();
                                alert.RegionId = item.RegionID;
                                alert.DisasterType = item.DisasterType;
                                alert.RiskLevel = viewRes.RiskLevel;
                                alert.AlertMessage = "Time:" + DateTime.Now.ToString("dd/MM/yyyy") + " Type:" + item.DisasterType + " Alert RiskLevel:" + viewRes.RiskLevel + " Region:" + item.RegionID;
                                alert.Timestamp = DateTime.Now;

                                await _dbContext.TbAlerts.AddAsync(alert);
                                await _dbContext.SaveChangesAsync();

                            }
                        }
                    }

                    results.Add(viewRes);
                }


                return results;
            }
            catch (Exception ex)
            {

                Log.Error($"disaster-risks {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")} Error:{ex.Message}");
                throw;
            }
        
           
           
        }

     
        [HttpGet("alert")]
        public async Task<IEnumerable<IAlertDataView>>  alert()
        {
            try
            {
                var list = await _dbContext.TbAlerts.Select(s => new IAlertDataView
                {
                    RegionId = s.RegionId,
                    DisasterType = s.DisasterType,
                    RiskLevel = s.RiskLevel,
                    AlertMessage = s.AlertMessage,
                    Timestamp = s.Timestamp
                }).ToListAsync();
                return list;
            }
            catch (Exception ex)
            {
                Log.Error($"alert {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")} Error:{ex.Message}");
                throw;
            }
           
        }


        [HttpPost("regions")]
        public async Task<IHttpResponse> regions(List<IRegionsReq> req)
        {
            try
            {
                List<TbRegion> regions = new List<TbRegion>();
                foreach (var item in req)
                {
                    foreach (var type in item.DisasterTypes)
                    {
                        TbRegion tbRegions = new TbRegion();
                        tbRegions.Id=Guid.NewGuid().ToString();
                        tbRegions.Longitude = item.LocationCoordinates.Longitude;
                        tbRegions.Latitude = item.LocationCoordinates.Latitude;
                        tbRegions.RegionId = item.RegionID;
                        tbRegions.DisasterType = type;
                        regions.Add(tbRegions);
                    }

                }
                await _dbContext.TbRegions.AddRangeAsync(regions);
                if (await _dbContext.SaveChangesAsync()>0)
                {
                    _httpResponse.SetAction(MessageType.Save);
                }
                else
                {
                    _httpResponse.SetAction(MessageType.Fail);
                }
               
            }
            catch (Exception ex)
            {
                _httpResponse.Error=ex.Message;
                _httpResponse.Status = 500;
                Log.Error($"regions {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")} Error:{ex.Message}");
            }
           return _httpResponse;
        }

        [HttpPost("alert-settings")]
        public async Task<IHttpResponse> alertsettings(IAlertSettingsReq req)
        {
            try
            {
                if(req != null)
                {
                    var newsetting=new TbAlertSetting();
                    newsetting.RegionId = req.RegionID;
                    newsetting.ThresholdScore=req.ThresholdScore;
                    newsetting.DisasterType=req.DisasterType;
                    newsetting.Id=Guid.NewGuid().ToString();
                    await _dbContext.TbAlertSettings.AddAsync(newsetting);

                    if(await _dbContext.SaveChangesAsync() > 0)
                    {
                        _httpResponse.SetAction(MessageType.Save);
                    }
                }
                else
                {
                     _httpResponse.SetAction(MessageType.Fail);
                }
            }
            catch (Exception ex)
            {

                _httpResponse.Error = ex.Message;
                _httpResponse.Status=500;
                Log.Error($"alert-settings {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")} Error:{ex.Message}");
            }
            return _httpResponse;
        }
        [HttpPost("alert/send")]
        public async Task alertsend()
        {
            try
            {
                string account = _configuration.GetValue<string>("SMS:accountSid")??"";
                string token = _configuration.GetValue<string>("SMS:accountSid")??"";
                string sendTo = _configuration.GetValue<string>("SMS:sendTo")??"";
                if (!string.IsNullOrEmpty(account) && !string.IsNullOrEmpty(token) && !string.IsNullOrEmpty(sendTo))
                {
                    SMSService sMSService = new SMSService(sendTo, account, token);
                    var alert = await _dbContext.TbAlerts.ToListAsync();
                    foreach (var item in alert)
                    {
                        sMSService.SendMessage(item.AlertMessage);
                    }
                }
               
            }
            catch (Exception ex)
            {
                Log.Error($"alert/send {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")} Error:{ex.Message}");
                throw;
            }
            
        }
    }
}
