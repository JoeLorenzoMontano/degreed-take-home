using System.Threading.Tasks;

namespace degreed.Services.Interfaces {
  public interface ICacheService {
    Task<T?> GetAsync<T>(string key) where T : class;
    Task SetAsync<T>(string key, T value, TimeSpan? expiry = null) where T : class;
    Task<bool> RemoveAsync(string key);
  }
}