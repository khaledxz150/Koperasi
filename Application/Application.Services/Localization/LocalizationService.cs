using Core.Services.Localization;

using Infrastructure.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Application.Services.Localization
{
    public class LocalizationService : ILocalizationService
    {
        private readonly ApplicationDbContext _context; 
        private readonly IMemoryCache _cache;
        private readonly ILogger<LocalizationService> _logger;
        public LocalizationService(
            ApplicationDbContext context,
            IMemoryCache cache,
            ILogger<LocalizationService> logger)
        {
            _context = context;
            _cache = cache;
            _logger = logger;
        }
        public async Task RefreshCacheAsync()
        {
            var languages = await _context.Languages.ToListAsync();

            foreach (var language in languages)
            {
                var cacheKey = $"{language.ID}-DictionaryLocalization";
                _cache.Remove(cacheKey);

                // Reload cache
                await GetAllStringsAsync(language.ID);
            }

            _logger.LogInformation("Localization cache refreshed for all languages");
        }

        public async Task<Dictionary<int, string>> GetAllStringsAsync(int languageId)
        {
            var cacheKey = $"{languageId}-DictionaryLocalization";

            if (_cache.TryGetValue(cacheKey, out Dictionary<int, string> cachedStrings))
            {
                return cachedStrings;
            }

            var strings = await _context.DictionaryLocalizations
                .Where(dl => dl.LanguageID == languageId)
                .Include(dl => dl.Dictionary)
                .ToDictionaryAsync(dl => dl.Dictionary.ID, dl => dl.Description);

            _cache.Set(cacheKey, strings);

            _logger.LogInformation($"Cached {strings.Count} localization strings for language ID: {languageId}");

            return strings;
        }
    }
}
