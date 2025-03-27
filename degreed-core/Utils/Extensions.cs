using degreed_core.Clients.Models;
using System.Text.RegularExpressions;

namespace degreed_core.Utils {
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

      string pattern = $"({Regex.Escape(searchTerm)})";
      return Regex.Replace(text, pattern, "<strong>$1</strong>", RegexOptions.IgnoreCase);
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
