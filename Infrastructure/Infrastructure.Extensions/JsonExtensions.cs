using System.Text.Json;

namespace Infrastructure.Extensions
{
    public static class JsonExtensions
    {
        public static string ToJson<T>(this List<T> list)
        {
            return JsonSerializer.Serialize(list);
        }
    }
}