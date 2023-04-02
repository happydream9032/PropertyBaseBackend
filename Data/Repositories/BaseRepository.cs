using System;
using PropertyBase.Contracts;
using Microsoft.EntityFrameworkCore;

namespace PropertyBase.Data.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T> where T: class
    {
        private readonly PropertyBaseDbContext _dbContext;
        public BaseRepository(PropertyBaseDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public virtual async Task<T> GetByIdAsync(Guid id)
        {
            return await _dbContext.Set<T>().FindAsync(id);
        }

        public virtual async Task<T> GetByIdAsync(long id)
        {
            return await _dbContext.Set<T>().FindAsync(id);
        }

        public virtual async Task<IReadOnlyList<T>> GetPagedReponseAsync(int page, int size)
        {
            return await _dbContext.Set<T>()
                          .Skip((page - 1) * size)
                          .Take(size)
                          .AsNoTracking()
                          .ToListAsync();
        }

        public virtual IQueryable<T> GetQueryable()
        {
            return _dbContext.Set<T>();
        }

        public virtual IQueryable<T> GetQueryableIgnoreQueryFilter()
        {
            return _dbContext.Set<T>().IgnoreQueryFilters();
        }

        public virtual async Task<long> GetTotalCount()
        {
            return await _dbContext.Set<T>().CountAsync();
        }

        public virtual async Task<IReadOnlyList<T>> ListAllAsync()
        {
            return await _dbContext.Set<T>().ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }

        public async Task<T> AddAsync(T entity)
        {
            await _dbContext.Set<T>().AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task DeleteAsync(T entity)
        {
            _dbContext.Set<T>().Remove(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(T entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateRangeAsync(List<T> entities)
        {
            foreach (var entity in entities)
            {
                _dbContext.Entry(entity).State = EntityState.Modified;
            }

            await _dbContext.SaveChangesAsync();
        }
    }
}

