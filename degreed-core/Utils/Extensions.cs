using degreed.Clients.Models;
using degreed.Services.Models;
using System.Text.RegularExpressions;

namespace degreed.Utils {
  public static class Extensions {
    public static IDictionary<WordCountBucket, List<JokeResult>> GroupByLength(this SearchResult searchResult) {
      if(searchResult?.results == null) {
        return new Dictionary<WordCountBucket, List<JokeResult>>();
      }
      return searchResult.results
          .GroupBy(joke => GetBucketLabel(CountWords(joke.joke)))
          .ToDictionary(g => g.Key, g => g.ToList());
    }

    public static string HighlightSearchTerm(this string text, string searchTerm) {
      if (string.IsNullOrWhiteSpace(text) || string.IsNullOrWhiteSpace(searchTerm))
        return text;

      // Split search term into words and highlight each one
      foreach (var word in searchTerm.Split(' ', StringSplitOptions.RemoveEmptyEntries))
      {
        if (word.Length < 2) continue; // Skip very short words
        string pattern = $"\\b({Regex.Escape(word)})\\b";
        text = Regex.Replace(text, pattern, "<strong>$1</strong>", RegexOptions.IgnoreCase);
      }
      
      return text;
    }

    public static List<HighlightedJoke> GetHighlightedJokesFromGroup(
        this IDictionary<Extensions.WordCountBucket, List<JokeResult>> groupedJokes,
        Extensions.WordCountBucket bucket,
        Dictionary<string, HighlightedJoke> highlightedJokesDict) {
      if(!groupedJokes.ContainsKey(bucket))
        return [];
      return groupedJokes[bucket]
          .Select(joke => highlightedJokesDict[joke.id])
          .ToList();
    }

    private static int CountWords(string text) {
      if(string.IsNullOrWhiteSpace(text)) return 0;
      return text.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;
    }

    private static WordCountBucket GetBucketLabel(int wordCount) {
      if(wordCount < 10)
        return WordCountBucket.Short;
      else if(wordCount < 20)
        return WordCountBucket.Medium;
      else
        return WordCountBucket.Long;
    }

    public enum WordCountBucket {
      Short,
      Medium,
      Long
    }
  }
}
