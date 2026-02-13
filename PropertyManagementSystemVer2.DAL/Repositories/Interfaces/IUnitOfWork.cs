namespace PropertyManagementSystemVer2.DAL.Repositories.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        // Specific Repositories
        IUserRepository Users { get; }
        IPropertyRepository Properties { get; }
        IPropertyImageRepository PropertyImages { get; }
        IBookingRepository Bookings { get; }
        IRentalApplicationRepository RentalApplications { get; }
        ILeaseRepository Leases { get; }
        IPaymentRepository Payments { get; }
        IMaintenanceRequestRepository MaintenanceRequests { get; }
        IRevenueRepository Revenues { get; }
        IConversationRepository Conversations { get; }
        IMessageRepository Messages { get; }
        ISystemConfigurationRepository SystemConfigurations { get; }
        IRefreshTokenRepository RefreshTokens { get; }
        IEmailVerificationTokenRepository EmailVerificationTokens { get; }
        IOtpVerificationRepository OtpVerifications { get; }
        // Generic Repository
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
