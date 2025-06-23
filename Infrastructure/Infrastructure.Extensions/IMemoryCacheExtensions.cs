
using System.Runtime.CompilerServices;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;


namespace Infrastructure.Extensions;
public static class IMemoryCacheExtensions
{


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string GetWord(this IMemoryCache memoryCache, int LanguageID, int DictionaryID)
    {
        return memoryCache.TryGetValue($"{LanguageID}-DictionaryLocalization", out  Dictionary<int, string> allLocalizations) &&
               allLocalizations.TryGetValue(DictionaryID, out var word) 
               ? word
               : string.Empty;
    }



}
