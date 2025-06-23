using System.Linq.Expressions;

using Microsoft.EntityFrameworkCore;

namespace Core.UnitOfWork.Repos
{
    public interface IGenericRepository<TEntity> where TEntity : class  
    {
        Task<TEntity?> GetByIdAsync(object id);
        Task<TResult?> GetByIdAsync<TResult>(object id, Expression<Func<TEntity, TResult>> selector);

        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<IEnumerable<TResult>> GetAllAsync<TResult>(Expression<Func<TEntity, TResult>> selector);

        Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate);
        Task<IEnumerable<TResult>> FindAsync<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector);
        Task<IEnumerable<TResult>> FindAsync<TResult>(Expression<Func<TEntity, TResult>> selector);

        Task AddAsync(TEntity entity);
        Task AddRangeAsync(IEnumerable<TEntity> entities);

        void Update(TEntity entity);

        void Remove(TEntity entity);
        void RemoveRange(IEnumerable<TEntity> entities);
        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> value);

    }
}
