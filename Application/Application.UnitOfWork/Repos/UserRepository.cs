using Infrastructure.Data;

using Models.Entities.User;

namespace Application.UnitOfWork.Repos
{
    public class UserRepository : GenericRepository<Users>,IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
