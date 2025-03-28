using System.Text.Json.Serialization;

namespace degreed.Services.Models {
  public class JokeSearchViewModel {
    [JsonInclude]
    public string SearchTerm { get; set; } = string.Empty;
    
    [JsonInclude]
    public int TotalJokes { get; set; }
    
    [JsonInclude]
    public int CurrentPage { get; set; }
    
    [JsonInclude]
    public int TotalPages { get; set; }
    
    [JsonInclude]
    public int PageSize { get; set; }
    
    [JsonInclude]
    public List<HighlightedJoke> Jokes { get; set; } = new List<HighlightedJoke>();
    
    [JsonInclude]
    public List<HighlightedJoke> ShortJokes { get; set; } = new List<HighlightedJoke>();
    
    [JsonInclude]
    public List<HighlightedJoke> MediumJokes { get; set; } = new List<HighlightedJoke>();
    
    [JsonInclude]
    public List<HighlightedJoke> LongJokes { get; set; } = new List<HighlightedJoke>();
  }

  public class HighlightedJoke {
    [JsonInclude]
    public string Id { get; set; } = string.Empty;
    
    [JsonInclude]
    public string OriginalJoke { get; set; } = string.Empty;
    
    [JsonInclude]
    public string HighlightedText { get; set; } = string.Empty;
  }
}