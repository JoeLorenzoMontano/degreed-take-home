namespace degreed.Services.Models {
  public class JokeSearchViewModel {
    public string SearchTerm { get; set; }
    public int TotalJokes { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public int PageSize { get; set; }
    public List<HighlightedJoke> Jokes { get; set; } = new List<HighlightedJoke>();
    public List<HighlightedJoke> ShortJokes { get; set; } = new List<HighlightedJoke>();
    public List<HighlightedJoke> MediumJokes { get; set; } = new List<HighlightedJoke>();
    public List<HighlightedJoke> LongJokes { get; set; } = new List<HighlightedJoke>();
  }

  public class HighlightedJoke {
    public string Id { get; set; }
    public string OriginalJoke { get; set; }
    public string HighlightedText { get; set; }
  }
}