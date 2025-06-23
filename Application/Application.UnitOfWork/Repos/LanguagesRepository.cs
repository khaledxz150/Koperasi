using Infrastructure.Data;

using Models.Entities.Localization;

namespace Application.UnitOfWork.Repos
{
    public class LanguagesRepository : GenericRepository<Languages>, ILanguagesRepository
    {
        public LanguagesRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
