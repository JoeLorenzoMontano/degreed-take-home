using degreed_core.Clients.Models;

namespace degreed_core.Clients {
  public interface ICanHazApiClient {
    Task<JokeResult?> Random();
    Task<SearchResult?> Search(int page = 1, int limit = 20, params string[] searchTerms);
  }
}