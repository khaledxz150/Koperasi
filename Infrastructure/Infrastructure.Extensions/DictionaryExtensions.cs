using System.Collections.Concurrent;
using System.Text;

using Qimmah.Extensions;

namespace Infrastructure.Extensions
{
    public static class DictionaryExtensions
    {
     

        public static string ToMapFieldString(this Dictionary<int, string> dictionaryLocalization)
        {
            // Check for null
            if (!dictionaryLocalization.IsNotNullOrEmpty())
            {
                return "[]";
            }

            var keyValuePairs = new List<string>();

            var stringBuilder = new StringBuilder();
            stringBuilder.Append("[");
            var count = dictionaryLocalization.Count;
            var Index = 0;
            foreach (var kvp in dictionaryLocalization)
            {
                stringBuilder.Append("{");
                stringBuilder.Append($"{kvp.Key}:'{kvp.Value}'");
                stringBuilder.Append("}");

                if (Index < count - 1)
                {
                    stringBuilder.Append(",");
                }
                Index++;
            }
            stringBuilder.Append("]");

            return stringBuilder.ToString();
            //return string.Join(", ", keyValuePairs);
        }
    }



    public class WordUsageLogger
    {
        private static readonly ConcurrentDictionary<string, HashSet<int>> PageWordUsage = new();
        private static readonly ConcurrentDictionary<string, HashSet<int>> RequestedButNotFilled = new();
        private static ILogger<WordUsageLogger> _logger;

        public static void Initialize(ILogger<WordUsageLogger> logger)
        {
            _logger = logger;
        }
        public static void LogWordUsage(string pageName, int wordId, string word)
        {
            if (!PageWordUsage.ContainsKey(pageName))
            {
                PageWordUsage[pageName] = new HashSet<int>();
                PageWordUsage[pageName].Add(wordId);
            }
            else
            {
                RequestedButNotFilled[pageName] = new HashSet<int>();
                RequestedButNotFilled[pageName].Add(wordId);
            }


            // Optionally log to a persistent store
            _logger.LogInformation("Page: {PageName}, Word ID: {WordId}, Word: {Word}", pageName, wordId, word);
        }

        public static IEnumerable<int> GetUsedWordsUsedByPage(string pageName)
        {
            return PageWordUsage.TryGetValue(pageName, out var words) ? words : Enumerable.Empty<int>();
        }
        public static IEnumerable<int> GetNotUsedWordsUsedByPage(string pageName)
        {
            return PageWordUsage.TryGetValue(pageName, out var words) ? words : Enumerable.Empty<int>();
        }

        public static string GetWordUsageHtml(string pageName)
        {
            // Fetch the used and not used words
            var usedWords = GetUsedWordsUsedByPage(pageName);
            var notUsedWords = GetNotUsedWordsUsedByPage(pageName);

            // Convert the collections to comma-separated strings
            var usedWordsString = string.Join(", ", usedWords);
            var notUsedWordsString = string.Join(", ", notUsedWords);

            // Build the HTML structure
            return $@"
            <div>
                <span>Used Words: {usedWordsString}</span>
                <span>Not Used Words: {notUsedWordsString}</span>
            </div>";
        }

        public static Dictionary<string, HashSet<int>> GetAllWordUsage()
        {
            return new Dictionary<string, HashSet<int>>(PageWordUsage);
        }
    }
}
