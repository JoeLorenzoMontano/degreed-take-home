using degreed.Clients.Models;
using degreed.Services.Models;

namespace degreed.Services.Interfaces {
  public interface IJokeService {
    Task<JokeResult?> GetRandomJoke();
    Task<JokeSearchViewModel?> SearchJokes(string searchTerm, int page = 1, int pageSize = 30);
  }
}