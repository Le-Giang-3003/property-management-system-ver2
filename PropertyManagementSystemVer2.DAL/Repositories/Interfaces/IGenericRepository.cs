using System.Linq.Expressions;

namespace PropertyManagementSystemVer2.DAL.Repositories.Interfaces
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        // Query
        Task<TEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<TEntity?> GetByIdAsync(int id, params Expression<Func<TEntity, object>>[] includes);
        Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
        Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
        IQueryable<TEntity> Query();
        IQueryable<TEntity> QueryAsNoTracking();

        // Count & Exists
        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
        Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default);

        // Add
        Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);
        Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

        // Update
        void Update(TEntity entity);
        void UpdateRange(IEnumerable<TEntity> entities);

        // Delete
        void Delete(TEntity entity);
        void DeleteRange(IEnumerable<TEntity> entities);
        Task<bool> DeleteByIdAsync(int id, CancellationToken cancellationToken = default);
    }
}