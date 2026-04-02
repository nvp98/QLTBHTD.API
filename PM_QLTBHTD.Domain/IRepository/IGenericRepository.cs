using System.Linq.Expressions;

namespace PM_QLTBHTD.Domain.IRepository
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(int id);
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task<(IEnumerable<T> Items, int Total)> GetPagedAsync(Expression<Func<T, bool>> predicate, int page, int pageSize);
        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
        Task<int> SaveChangesAsync();
    }
}
