using System;
using System.Linq.Expressions;

using Core.UnitOfWork.Repos;

using Infrastructure.Data;

using Microsoft.EntityFrameworkCore;

namespace Application.UnitOfWork.Repos
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class 
    {
        protected readonly ApplicationDbContext _context;
        private readonly DbSet<TEntity> _dbSet;

        public GenericRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _dbSet = _context.Set<TEntity>();
        }

        public async Task AddAsync(TEntity entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            await _dbSet.AddAsync(entity);
        }

        public async Task AddRangeAsync(IEnumerable<TEntity> entities)
        {
            if (entities == null) throw new ArgumentNullException(nameof(entities));
            await _dbSet.AddRangeAsync(entities);
        }

        public Task<bool> AnyAsync(Expression<Func<TEntity, bool>> value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            return _dbSet.AnyAsync(value); 
        }

        public async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public async Task<IEnumerable<TResult>> FindAsync<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            if (selector == null) throw new ArgumentNullException(nameof(selector));
            return await _dbSet.Where(predicate).Select(selector).ToListAsync();
        }

        public async Task<IEnumerable<TResult>> FindAsync<TResult>(Expression<Func<TEntity, TResult>> selector)
        {
            if (selector == null) throw new ArgumentNullException(nameof(selector));
            return await _dbSet.Select(selector).ToListAsync();
        }

        public async Task<Dictionary<TKey, TValue>> FindAsDictionaryAsync<TKey, TValue>(
        Expression<Func<TEntity, bool>> predicate,
        Func<TEntity, TKey> keySelector,
        Func<TEntity, TValue> valueSelector) where TKey : notnull
        {
            return await _context.Set<TEntity>()
                .Where(predicate)
                .ToDictionaryAsync(keySelector, valueSelector);
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<IEnumerable<TResult>> GetAllAsync<TResult>(Expression<Func<TEntity, TResult>> selector)
        {
            if (selector == null) throw new ArgumentNullException(nameof(selector));
            return await _dbSet.Select(selector).ToListAsync();
        }

        public async Task<TEntity?> GetByIdAsync(object id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));
            return await _dbSet.FindAsync(id);
        }

        public async Task<TResult?> GetByIdAsync<TResult>(object id, Expression<Func<TEntity, TResult>> selector)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));
            if (selector == null) throw new ArgumentNullException(nameof(selector));
            var entity = await _dbSet.FindAsync(id);
            if (entity == null) return default;
            return selector.Compile().Invoke(entity);
        }

        public void Remove(TEntity entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            _dbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<TEntity> entities)
        {
            if (entities == null) throw new ArgumentNullException(nameof(entities));
            _dbSet.RemoveRange(entities);
        }

        public void Update(TEntity entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            _dbSet.Update(entity);
        }
    }
}
