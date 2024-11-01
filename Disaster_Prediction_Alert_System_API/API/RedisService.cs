using StackExchange.Redis;
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

public interface IRedisCacheService
{
    Task SetCacheValueAsync<T>(string key, T value, TimeSpan expiration);
    Task<T> GetCacheValueAsync<T>(string key);
}
namespace Disaster_Prediction_Alert_System_API.Controllers
{
    public class RedisCacheService : IRedisCacheService
    {
        private readonly IConnectionMultiplexer _redis;

        public RedisCacheService(IConnectionMultiplexer redis)
        {
            _redis = redis;
        }

        public async Task SetCacheValueAsync<T>(string key, T value, TimeSpan expiration)
        {
            var db = _redis.GetDatabase();
            var json = JsonSerializer.Serialize(value);
            await db.StringSetAsync(key, json, expiration);
        }

        public async Task<T> GetCacheValueAsync<T>(string key)
        {
            var db = _redis.GetDatabase();
            var json = await db.StringGetAsync(key);
            return json.HasValue ? JsonSerializer.Deserialize<T>(json) : default;
        }
    }
}
