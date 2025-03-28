using degreed.Services.Interfaces;
using StackExchange.Redis;
using System.Text.Json;
using System.Threading.Tasks;

namespace degreed.Services {
  public class RedisCacheService : ICacheService {
    private readonly IConnectionMultiplexer _redis;
    private readonly IDatabase _database;

    public RedisCacheService(string connectionString) {
      _redis = ConnectionMultiplexer.Connect(connectionString);
      _database = _redis.GetDatabase();
    }

    public async Task<T?> GetAsync<T>(string key) where T : class {
      var value = await _database.StringGetAsync(key);
      if (value.IsNullOrEmpty) {
        return null;
      }

      return JsonSerializer.Deserialize<T>(value!);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null) where T : class {
      var serializedValue = JsonSerializer.Serialize(value);
      await _database.StringSetAsync(key, serializedValue, expiry);
    }

    public async Task<bool> RemoveAsync(string key) {
      return await _database.KeyDeleteAsync(key);
    }
  }
}