using Core.Services.Localization;
using Core.UnitOfWork;

using Infrastructure.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using Models.Entities.Localization;

namespace Application.Services.Localization
{
    public class LocalizationService : ILocalizationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMemoryCache _cache;
        private readonly ILogger<LocalizationService> _logger;
        public LocalizationService(
            ApplicationDbContext context,
            IMemoryCache cache,
            ILogger<LocalizationService> logger,
            IUnitOfWork unitOfWork)
        {
            _cache = cache;
            _logger = logger;
            _unitOfWork = unitOfWork;
        }
        public async Task RefreshCacheAsync()
        {
            var languages = await _unitOfWork.__languagesRepository.GetAllAsync();

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

            var strings = await _unitOfWork._dictionaryLocalizationRepository.FindAsDictionaryAsync(dl => dl.LanguageID == languageId, dl => dl.ID, dl => dl.Description);

            _cache.Set(cacheKey, strings);

            _logger.LogInformation($"Cached {strings.Count} localization strings for language ID: {languageId}");

            return strings;
        }
        public async Task<IEnumerable<Languages>> GetLanguagesAsync()
        {
            return await _unitOfWork.__languagesRepository.GetAllAsync();
        }
    }
}
