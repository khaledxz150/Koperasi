using Models.Entities.Localization;

namespace Core.Services.Localization
{
    public interface ILocalizationService
    {
        Task<Dictionary<int, string>> GetAllStringsAsync(int languageId);
        Task<IEnumerable<Languages>> GetLanguagesAsync();
        Task RefreshCacheAsync();
    }
}
