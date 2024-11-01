

using Disaster_Prediction_Alert_System_API;
using Disaster_Prediction_Alert_System_API.API;
using Disaster_Prediction_Alert_System_API.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Serilog;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("appsettings.json");
ConfigurationManager configuration = builder.Configuration;
// Add services to the container.
var redisConfiguration = builder.Configuration.GetSection("Redis")["ConnectionString"];
var redis = ConnectionMultiplexer.Connect(redisConfiguration);
builder.Services.AddSingleton<IConnectionMultiplexer>(redis);
builder.Services.AddSingleton<IRedisCacheService, RedisCacheService>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient<EarthquakeService>();
builder.Services.AddHttpClient<OpenWeatherAPI>();
builder.Services.AddDbContext<DisasterDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("PRSConnectionString")));

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Error()
    .WriteTo.Console()
    .WriteTo.File("log.txt",
        rollingInterval: RollingInterval.Month,
        rollOnFileSizeLimit: true)
    .CreateLogger();


var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Web API V1");
    if (app.Environment.IsDevelopment())
        options.RoutePrefix = "swagger";
    else
        options.RoutePrefix = string.Empty;
}
);
//app.UseSerilogRequestLogging();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
