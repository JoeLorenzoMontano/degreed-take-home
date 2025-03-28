using degreed.Clients;
using degreed.Clients.Models;
using degreed.Services.Interfaces;
using degreed.Services.Models;
using degreed.Utils;

namespace degreed.Services {
  public class JokeService : IJokeService {
    private readonly ICanHazApiClient _apiClient;
    private readonly ICacheService? _cacheService;
    private const int CACHE_EXPIRY_MINUTES = 15;

    public JokeService(ICanHazApiClient apiClient, ICacheService? cacheService = null) {
      _apiClient = apiClient;
      _cacheService = cacheService;
    }

    public async Task<JokeResult?> GetRandomJoke() {
      return await _apiClient.Random();
    }

    public async Task<JokeSearchViewModel?> SearchJokes(string searchTerm, int page = 1, int pageSize = 30) {
      // Default to empty string if search term is null or whitespace
      searchTerm = string.IsNullOrWhiteSpace(searchTerm) ? string.Empty : searchTerm;

      // Create a cache key based on search parameters
      string cacheKey = $"jokes:search:{searchTerm}:page:{page}:size:{pageSize}";

      // Try to get data from cache first if cache is available
      if (_cacheService != null) {
        var cachedViewModel = await _cacheService.GetAsync<JokeSearchViewModel>(cacheKey);
        if (cachedViewModel != null) {
          return cachedViewModel;
        }
      }

      // Cache miss or no cache service, get from API
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
        Jokes = [.. highlightedJokesDict.Values],
        // Map from grouped jokes to highlighted jokes using our dictionary
        ShortJokes = groupedJokes.GetHighlightedJokesFromGroup(Extensions.WordCountBucket.Short, highlightedJokesDict),
        MediumJokes = groupedJokes.GetHighlightedJokesFromGroup(Extensions.WordCountBucket.Medium, highlightedJokesDict),
        LongJokes = groupedJokes.GetHighlightedJokesFromGroup(Extensions.WordCountBucket.Long, highlightedJokesDict)
      };

      // Store in cache if cache service is available
      if (_cacheService != null) {
        await _cacheService.SetAsync(cacheKey, viewModel, TimeSpan.FromMinutes(CACHE_EXPIRY_MINUTES));
      }

      return viewModel;
    }
  }
}