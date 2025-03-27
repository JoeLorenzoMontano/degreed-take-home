using degreed.Clients.Models;
using System.Net.Http.Json;

namespace degreed.Clients {
  public class CanHazApiClient : ICanHazApiClient {
    public const string BASE_ADDRESS = "https://icanhazdadjoke.com/";
    private HttpClient _httpClient;

    public CanHazApiClient() {
      _httpClient = new HttpClient() {
        BaseAddress = new Uri(BASE_ADDRESS),
      };
      _httpClient.DefaultRequestHeaders.Add("User-Agent", "Take Home Assignment API Client");
      _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
    }

    public async Task<JokeResult?> Random() {
      try {
        return await _httpClient.GetFromJsonAsync<JokeResult>("/");
      }
      catch(Exception ex) {
        Console.Error.WriteLine(ex.Message);
      }
      return null;
    }

    public async Task<SearchResult?> Search(int page = 1, int limit = 20, params string[] searchTerms) {
      try {
        string term = string.Join(" ", searchTerms);
        return await _httpClient.GetFromJsonAsync<SearchResult>($"/search?term={Uri.EscapeDataString(term)}&page={page}&limit={limit}");
      }
      catch(Exception ex) {
        Console.Error.WriteLine(ex.Message);
      }
      return null;
    }
  }
}
