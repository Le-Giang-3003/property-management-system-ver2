using Microsoft.EntityFrameworkCore.Storage;
using PropertyManagementSystemVer2.DAL.Data;
using PropertyManagementSystemVer2.DAL.Repositories.Interfaces;

namespace PropertyManagementSystemVer2.DAL.Repositories.Implementations
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private readonly Dictionary<Type, object> _repositories;
        private IDbContextTransaction? _transaction;

        // Lazy-initialized specific repositories
        private IUserRepository? _users;
        private IPropertyRepository? _properties;
        private IPropertyImageRepository? _propertyImages;
        private IBookingRepository? _bookings;
        private IRentalApplicationRepository? _rentalApplications;
        private ILeaseRepository? _leases;
        private IPaymentRepository? _payments;
        private IMaintenanceRequestRepository? _maintenanceRequests;
        private IRevenueRepository? _revenues;
        private IConversationRepository? _conversations;
        private IMessageRepository? _messages;
        private ISystemConfigurationRepository? _systemConfigurations;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            _repositories = new Dictionary<Type, object>();
        }

        // Specific Repository Properties
        public IUserRepository Users => _users ??= new UserRepository(_context);
        public IPropertyRepository Properties => _properties ??= new PropertyRepository(_context);
        public IPropertyImageRepository PropertyImages => _propertyImages ??= new PropertyImageRepository(_context);
        public IBookingRepository Bookings => _bookings ??= new BookingRepository(_context);
        public IRentalApplicationRepository RentalApplications => _rentalApplications ??= new RentalApplicationRepository(_context);
        public ILeaseRepository Leases => _leases ??= new LeaseRepository(_context);
        public IPaymentRepository Payments => _payments ??= new PaymentRepository(_context);
        public IMaintenanceRequestRepository MaintenanceRequests => _maintenanceRequests ??= new MaintenanceRequestRepository(_context);
        public IRevenueRepository Revenues => _revenues ??= new RevenueRepository(_context);
        public IConversationRepository Conversations => _conversations ??= new ConversationRepository(_context);
        public IMessageRepository Messages => _messages ??= new MessageRepository(_context);
        public ISystemConfigurationRepository SystemConfigurations => _systemConfigurations ??= new SystemConfigurationRepository(_context);

        public IGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : class
        {
            var type = typeof(TEntity);
            if (!_repositories.ContainsKey(type))
            {
                _repositories[type] = new GenericRepository<TEntity>(_context);
            }
            return (IGenericRepository<TEntity>)_repositories[type];
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public int SaveChanges()
        {
            return _context.SaveChanges();
        }

        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        }

        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction == null)
                throw new InvalidOperationException("No transaction started");

            try
            {
                await _transaction.CommitAsync(cancellationToken);
            }
            catch
            {
                await RollbackTransactionAsync(cancellationToken);
                throw;
            }
            finally
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction == null)
                throw new InvalidOperationException("No transaction started");

            await _transaction.RollbackAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context?.Dispose();
        }
    }
}
