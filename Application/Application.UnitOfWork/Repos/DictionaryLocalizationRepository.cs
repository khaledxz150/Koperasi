using Infrastructure.Data;

using Models.Entities.Localization;

namespace Application.UnitOfWork.Repos
{
    public class DictionaryLocalizationRepository : GenericRepository<DictionaryLocalization>, IDictionaryLocalizationRepository
    {
        public DictionaryLocalizationRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
