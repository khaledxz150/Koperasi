
using Core.UnitOfWork.Repos;


using Models.Entities.User;

namespace Application.UnitOfWork.Repos
{
    public interface IUserRepository : IGenericRepository<Users>
    {
    }
}
