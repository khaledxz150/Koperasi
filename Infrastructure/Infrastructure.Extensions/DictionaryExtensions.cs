using System.Collections.Concurrent;
using System.Text;


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



}
