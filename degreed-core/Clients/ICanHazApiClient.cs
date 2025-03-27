using degreed.Clients.Models;

namespace degreed.Clients {
  public interface ICanHazApiClient {
    Task<JokeResult?> Random();
    Task<SearchResult?> Search(int page = 1, int limit = 20, params string[] searchTerms);
  }
}