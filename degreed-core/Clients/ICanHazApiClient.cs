using degreed_core.Clients.Models;

namespace degreed_core.Clients {
  public interface ICanHazApiClient {
    Task<JokeResult?> Random();
    Task<SearchResult?> Search(params string[] searchTerms);
  }
}