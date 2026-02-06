namespace PropertyManagementSystemVer2.DAL.Repositories.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        // Repository
        IGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : class;

        // Save
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        int SaveChanges();

        // Transaction
        Task BeginTransactionAsync(CancellationToken cancellationToken = default);
        Task CommitTransactionAsync(CancellationToken cancellationToken = default);
        Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
    }
}
