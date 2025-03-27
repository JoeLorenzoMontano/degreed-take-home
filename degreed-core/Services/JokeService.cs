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

      // Create a dictionary of highlighted jokes by id to avoid highlighting the same joke multiple times
      var highlightedJokesDict = searchResult.results
          .ToDictionary(
              joke => joke.id,
              joke => new HighlightedJoke {
                Id = joke.id,
                OriginalJoke = joke.joke,
                HighlightedText = joke.joke.HighlightSearchTerm(searchTerm)
              }
          );

      // Group jokes by length
      var groupedJokes = searchResult.GroupByLength();

      // Create view model using the dictionary to avoid re-highlighting
      var viewModel = new JokeSearchViewModel {
        SearchTerm = searchTerm,
        TotalJokes = searchResult.total_jokes,
        CurrentPage = searchResult.current_page,
        TotalPages = searchResult.total_pages,
        PageSize = pageSize,
        // Use the highlighted jokes we already created
        Jokes = searchResult.results
            .Take(pageSize)
            .Select(joke => highlightedJokesDict[joke.id])
            .ToList(),
        // Map from grouped jokes to highlighted jokes using our dictionary
        ShortJokes = GetHighlightedJokesFromGroup(
            groupedJokes, 
            Extensions.WordCountBucket.Short, 
            highlightedJokesDict
        ),
        MediumJokes = GetHighlightedJokesFromGroup(
            groupedJokes, 
            Extensions.WordCountBucket.Medium,
            highlightedJokesDict
        ),
        LongJokes = GetHighlightedJokesFromGroup(
            groupedJokes, 
            Extensions.WordCountBucket.Long,
            highlightedJokesDict
        )
      };

      return viewModel;
    }

    private List<HighlightedJoke> GetHighlightedJokesFromGroup(
        IDictionary<Extensions.WordCountBucket, List<JokeResult>> groupedJokes,
        Extensions.WordCountBucket bucket,
        Dictionary<string, HighlightedJoke> highlightedJokesDict)
    {
        if (!groupedJokes.ContainsKey(bucket))
            return new List<HighlightedJoke>();

        return groupedJokes[bucket]
            .Select(joke => highlightedJokesDict[joke.id])
            .ToList();
    }
  }
}