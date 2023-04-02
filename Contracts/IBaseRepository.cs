using System;
namespace PropertyBase.Contracts
{
    public interface IBaseRepository<T> where T: class
    {
        Task<T> GetByIdAsync(Guid id);
        Task<IReadOnlyList<T>> ListAllAsync();
        Task<T> AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
        Task<IReadOnlyList<T>> GetPagedReponseAsync(int page, int size);
        Task<T> GetByIdAsync(long id);
        Task SaveChangesAsync();
        Task<long> GetTotalCount();
        IQueryable<T> GetQueryable();
        Task UpdateRangeAsync(List<T> entities);
        IQueryable<T> GetQueryableIgnoreQueryFilter();
    }
}

