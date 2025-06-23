using Core.UnitOfWork.Repos;

using Microsoft.EntityFrameworkCore;

using Models.Entities.Localization;

namespace Application.UnitOfWork.Repos
{
    public interface ILanguagesRepository : IGenericRepository<Languages>
    {
    }
}
