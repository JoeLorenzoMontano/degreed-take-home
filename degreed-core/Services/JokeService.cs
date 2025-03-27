using degreed.Clients;
using degreed.Clients.Models;
using degreed.Services.Interfaces;
using degreed.Services.Models;
using degreed.Utils;

namespace degreed.Services {
  public class JokeService : IJokeService {
    private readonly ICanHazApiClient _apiClient;

    public JokeService(ICanHazApiClient apiClient) {
      _apiClient = apiClient;
    }

    public async Task<JokeResult> GetRandomJoke() {
      return await _apiClient.Random();
    }

    public async Task<JokeSearchViewModel> SearchJokes(string searchTerm, int page = 1, int pageSize = 30) {
      // Default to empty string if search term is null or whitespace
      searchTerm = string.IsNullOrWhiteSpace(searchTerm) ? string.Empty : searchTerm;

      // Get search results from API
      var searchResult = await _apiClient.Search(page, pageSize, searchTerm);

      if(searchResult == null)
        return new JokeSearchViewModel();

      // Group jokes by length
      var groupedJokes = searchResult.GroupByLength();

      // Prepare highlighted jokes for display
      var highlightedJokes = searchResult.results
          .Take(pageSize)
          .Select(joke => new HighlightedJoke {
            Id = joke.id,
            OriginalJoke = joke.joke,
            HighlightedText = joke.joke.HighlightSearchTerm(searchTerm)
          })
          .ToList();

      // Create view model
      var viewModel = new JokeSearchViewModel {
        SearchTerm = searchTerm,
        TotalJokes = searchResult.total_jokes,
        CurrentPage = searchResult.current_page,
        TotalPages = searchResult.total_pages,
        PageSize = pageSize,
        Jokes = highlightedJokes,
        ShortJokes = groupedJokes.ContainsKey(Extensions.WordCountBucket.Short)
              ? ConvertToHighlightedJokes(groupedJokes[Extensions.WordCountBucket.Short], searchTerm)
              : new List<HighlightedJoke>(),
        MediumJokes = groupedJokes.ContainsKey(Extensions.WordCountBucket.Medium)
              ? ConvertToHighlightedJokes(groupedJokes[Extensions.WordCountBucket.Medium], searchTerm)
              : new List<HighlightedJoke>(),
        LongJokes = groupedJokes.ContainsKey(Extensions.WordCountBucket.Long)
              ? ConvertToHighlightedJokes(groupedJokes[Extensions.WordCountBucket.Long], searchTerm)
              : new List<HighlightedJoke>()
      };

      return viewModel;
    }

    private List<HighlightedJoke> ConvertToHighlightedJokes(List<JokeResult> jokes, string searchTerm) {
      return jokes.Select(joke => new HighlightedJoke {
        Id = joke.id,
        OriginalJoke = joke.joke,
        HighlightedText = joke.joke.HighlightSearchTerm(searchTerm)
      }).ToList();
    }
  }
}