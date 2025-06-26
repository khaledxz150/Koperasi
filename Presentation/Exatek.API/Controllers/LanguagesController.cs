using Core.Services.Localization;

using Microsoft.AspNetCore.Mvc;

namespace Koperasi.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LanguagesController : ControllerBase
    {
        private readonly ILocalizationService _localizationService;

        public LanguagesController(ILocalizationService localizationService)
        {
            _localizationService = localizationService;
        }




        // GET: api/Languages
        [HttpGet]
        public async Task<IActionResult> GetLanguages()
        {
            var languages = await _localizationService.GetLanguagesAsync();
            return Ok(languages);
        }

        // GET: api/Languages/{languageId}/dictionaries
        [HttpGet("{languageId}/dictionaries")]
        public async Task<IActionResult> GetLanguageDictionaries(int languageId)
        {
            var dictionaries = await _localizationService.GetAllStringsAsync(languageId);

            return Ok(dictionaries);
        }
    }
}
