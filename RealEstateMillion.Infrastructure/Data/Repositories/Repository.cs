using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using RealEstateMillion.Domain.Interfaces;
using RealEstateMillion.Infrastructure.Data.Context;

namespace RealEstateMillion.Infrastructure.Data.Repositories
{

    public class Repository<T>(RealEstateMillionDbContext context) : IRepository<T> where T : class
    {
        protected readonly RealEstateMillionDbContext _context = context;
        protected readonly DbSet<T> _dbSet = context.Set<T>();

        public virtual async Task<T?> GetByIdAsync(Guid id)
        {
            return await _dbSet.FindAsync(id);
        }

        public virtual async Task<T?> GetByIdAsync(Guid id, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet;
            query = includes.Aggregate(query, (current, include) => current.Include(include));
            return await query.FirstOrDefaultAsync(e => EF.Property<Guid>(e, "Id") == id);
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet;
            query = includes.Aggregate(query, (current, include) => current.Include(include));
            return await query.ToListAsync();
        }

        public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet;
            query = includes.Aggregate(query, (current, include) => current.Include(include));
            return await query.Where(predicate).ToListAsync();
        }

        public virtual async Task<T?> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.SingleOrDefaultAsync(predicate);
        }

        public virtual async Task<T?> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet;
            query = includes.Aggregate(query, (current, include) => current.Include(include));
            return await query.SingleOrDefaultAsync(predicate);
        }

        public virtual async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.AnyAsync(predicate);
        }

        public virtual async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null)
        {
            return predicate == null ? await _dbSet.CountAsync() : await _dbSet.CountAsync(predicate);
        }

        public virtual async Task<T> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            return entity;
        }

        public virtual async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities)
        {
            await _dbSet.AddRangeAsync(entities);
            return entities;
        }

        public virtual void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public virtual void UpdateRange(IEnumerable<T> entities)
        {
            _dbSet.UpdateRange(entities);
        }

        public virtual void Remove(T entity)
        {
            _dbSet.Remove(entity);
        }

        public virtual void RemoveRange(IEnumerable<T> entities)
        {
            _dbSet.RemoveRange(entities);
        }

        public virtual async Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync<TKey>(
            int pageNumber,
            int pageSize,
            Expression<Func<T, bool>>? filter = null,
            Expression<Func<T, TKey>>? orderBy = null,
            bool orderByDescending = false,
            params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet;

            query = includes.Aggregate(query, (current, include) => current.Include(include));

            if (filter != null)
                query = query.Where(filter);

            var totalCount = await query.CountAsync();

            if (orderBy != null)
            {
                query = orderByDescending ? query.OrderByDescending(orderBy) : query.OrderBy(orderBy);
            }

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }
    }
}
