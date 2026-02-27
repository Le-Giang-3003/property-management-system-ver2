using Microsoft.EntityFrameworkCore;
using PropertyManagementSystemVer2.DAL.Entities;

namespace PropertyManagementSystemVer2.DAL.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Core
        public DbSet<User> Users => Set<User>();
        public DbSet<Property> Properties => Set<Property>();
        public DbSet<PropertyImage> PropertyImages => Set<PropertyImage>();
        public DbSet<RentalApplication> RentalApplications => Set<RentalApplication>();
        public DbSet<Lease> Leases => Set<Lease>();
        public DbSet<Payment> Payments => Set<Payment>();
        public DbSet<MaintenanceRequest> MaintenanceRequests => Set<MaintenanceRequest>();
        public DbSet<Booking> Bookings => Set<Booking>();
        public DbSet<Revenue> Revenues => Set<Revenue>();
        public DbSet<SystemConfiguration> SystemConfigurations => Set<SystemConfiguration>();

        // Chat
        public DbSet<Conversation> Conversations => Set<Conversation>();
        public DbSet<Message> Messages => Set<Message>();
        public DbSet<DeletedMessage> DeletedMessages => Set<DeletedMessage>();
        public DbSet<TypingIndicator> TypingIndicators => Set<TypingIndicator>();
        public DbSet<UserPresence> UserPresences => Set<UserPresence>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ===== USER =====
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Email).HasMaxLength(256).IsRequired();
                entity.Property(e => e.PasswordHash).HasMaxLength(500).IsRequired();
                entity.Property(e => e.FullName).HasMaxLength(200).IsRequired();
                entity.Property(e => e.PhoneNumber).HasMaxLength(20).IsRequired();
                entity.Property(e => e.Address).HasMaxLength(500);
                entity.Property(e => e.AvatarUrl).HasMaxLength(500);
                entity.Property(e => e.Role).HasConversion<string>();

                // Dual-role flags
                entity.Property(e => e.IsTenant).HasDefaultValue(true);
                entity.Property(e => e.IsLandlord).HasDefaultValue(false);

                // Landlord-specific info
                entity.Property(e => e.IdentityNumber).HasMaxLength(20);
                entity.Property(e => e.BankAccountNumber).HasMaxLength(30);
                entity.Property(e => e.BankName).HasMaxLength(100);
                entity.Property(e => e.BankAccountHolder).HasMaxLength(200);

                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasIndex(e => e.PhoneNumber);
                entity.HasIndex(e => e.Role);
                entity.HasIndex(e => e.IsLandlord);
            });

            // ===== PROPERTY =====
            modelBuilder.Entity<Property>(entity =>
            {
                entity.ToTable("Properties");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Title).HasMaxLength(300).IsRequired();
                entity.Property(e => e.Description).HasMaxLength(5000);
                entity.Property(e => e.Address).HasMaxLength(500).IsRequired();
                entity.Property(e => e.City).HasMaxLength(100).IsRequired();
                entity.Property(e => e.District).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Ward).HasMaxLength(100);
                entity.Property(e => e.Currency).HasMaxLength(10).HasDefaultValue("VND");
                entity.Property(e => e.Amenities).HasMaxLength(2000);
                entity.Property(e => e.RejectionReason).HasMaxLength(1000);

                entity.Property(e => e.MonthlyRent).HasPrecision(18, 2);
                entity.Property(e => e.DepositAmount).HasPrecision(18, 2);
                entity.Property(e => e.Area).HasPrecision(10, 2);
                entity.Property(e => e.Latitude).HasPrecision(10, 8);
                entity.Property(e => e.Longitude).HasPrecision(11, 8);

                entity.Property(e => e.PropertyType).HasConversion<string>();
                entity.Property(e => e.Status).HasConversion<string>();

                entity.HasOne(e => e.Landlord)
                    .WithMany(u => u.OwnedProperties)
                    .HasForeignKey(e => e.LandlordId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => e.LandlordId);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.PropertyType);
                entity.HasIndex(e => e.City);
                entity.HasIndex(e => new { e.City, e.District });
                entity.HasIndex(e => e.MonthlyRent);
                entity.HasIndex(e => e.IsAvailable);
            });

            // ===== PROPERTY IMAGE =====
            modelBuilder.Entity<PropertyImage>(entity =>
            {
                entity.ToTable("PropertyImages");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.ImageUrl).HasMaxLength(500).IsRequired();
                entity.Property(e => e.Caption).HasMaxLength(300);

                entity.HasOne(e => e.Property)
                    .WithMany(p => p.Images)
                    .HasForeignKey(e => e.PropertyId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.PropertyId);
                entity.HasIndex(e => new { e.PropertyId, e.IsPrimary });
            });

            // ===== RENTAL APPLICATION =====
            modelBuilder.Entity<RentalApplication>(entity =>
            {
                entity.ToTable("RentalApplications");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Message).HasMaxLength(2000);
                entity.Property(e => e.Occupation).HasMaxLength(200);
                entity.Property(e => e.EmployerName).HasMaxLength(300);
                entity.Property(e => e.EmployerContact).HasMaxLength(100);
                entity.Property(e => e.ReferenceName).HasMaxLength(200);
                entity.Property(e => e.ReferenceContact).HasMaxLength(100);
                entity.Property(e => e.ReferenceRelationship).HasMaxLength(100);
                entity.Property(e => e.RejectionReason).HasMaxLength(1000);
                entity.Property(e => e.MonthlyIncome).HasPrecision(18, 2);
                entity.Property(e => e.Status).HasConversion<string>();

                entity.HasOne(e => e.Property)
                    .WithMany(p => p.RentalApplications)
                    .HasForeignKey(e => e.PropertyId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Tenant)
                    .WithMany(u => u.RentalApplications)
                    .HasForeignKey(e => e.TenantId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => e.PropertyId);
                entity.HasIndex(e => e.TenantId);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => new { e.PropertyId, e.TenantId, e.Status });
            });

            // ===== LEASE =====
            modelBuilder.Entity<Lease>(entity =>
            {
                entity.ToTable("Leases");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.LeaseNumber).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Currency).HasMaxLength(10).HasDefaultValue("VND");
                entity.Property(e => e.Terms).HasMaxLength(10000);
                entity.Property(e => e.SpecialConditions).HasMaxLength(5000);
                entity.Property(e => e.TerminationReason).HasMaxLength(1000);

                entity.Property(e => e.MonthlyRent).HasPrecision(18, 2);
                entity.Property(e => e.DepositAmount).HasPrecision(18, 2);
                entity.Property(e => e.LateFeePercentage).HasPrecision(5, 2);
                entity.Property(e => e.Status).HasConversion<string>();

                entity.HasOne(e => e.Property)
                    .WithMany(p => p.Leases)
                    .HasForeignKey(e => e.PropertyId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Landlord)
                    .WithMany(u => u.LandlordLeases)
                    .HasForeignKey(e => e.LandlordId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Tenant)
                    .WithMany(u => u.TenantLeases)
                    .HasForeignKey(e => e.TenantId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.RentalApplication)
                    .WithOne()
                    .HasForeignKey<Lease>(e => e.RentalApplicationId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasIndex(e => e.LeaseNumber).IsUnique();
                entity.HasIndex(e => e.PropertyId);
                entity.HasIndex(e => e.LandlordId);
                entity.HasIndex(e => e.TenantId);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => new { e.StartDate, e.EndDate });
            });

            // ===== PAYMENT =====
            modelBuilder.Entity<Payment>(entity =>
            {
                entity.ToTable("Payments");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Currency).HasMaxLength(10).HasDefaultValue("VND");
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.TransactionId).HasMaxLength(100);
                entity.Property(e => e.PaymentProof).HasMaxLength(500);
                entity.Property(e => e.Notes).HasMaxLength(1000);

                entity.Property(e => e.Amount).HasPrecision(18, 2);
                entity.Property(e => e.LateFeeAmount).HasPrecision(18, 2);
                entity.Property(e => e.PaymentType).HasConversion<string>();
                entity.Property(e => e.Status).HasConversion<string>();
                entity.Property(e => e.PaymentMethod).HasConversion<string>();

                entity.HasOne(e => e.Lease)
                    .WithMany(l => l.Payments)
                    .HasForeignKey(e => e.LeaseId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => e.LeaseId);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.DueDate);
                entity.HasIndex(e => e.TransactionId);
                entity.HasIndex(e => new { e.BillingYear, e.BillingMonth });
            });

            // ===== MAINTENANCE REQUEST =====
            modelBuilder.Entity<MaintenanceRequest>(entity =>
            {
                entity.ToTable("MaintenanceRequests");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Title).HasMaxLength(300).IsRequired();
                entity.Property(e => e.Description).HasMaxLength(3000).IsRequired();
                entity.Property(e => e.ImageUrls).HasMaxLength(2000);
                entity.Property(e => e.Resolution).HasMaxLength(2000);
                entity.Property(e => e.Status).HasConversion<string>();
                entity.Property(e => e.Priority).HasConversion<string>();
                entity.Property(e => e.Category).HasConversion<string>();

                entity.HasOne(e => e.Property)
                    .WithMany(p => p.MaintenanceRequests)
                    .HasForeignKey(e => e.PropertyId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Lease)
                    .WithMany(l => l.MaintenanceRequests)
                    .HasForeignKey(e => e.LeaseId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Requester)
                    .WithMany()
                    .HasForeignKey(e => e.RequestedBy)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => e.PropertyId);
                entity.HasIndex(e => e.LeaseId);
                entity.HasIndex(e => e.RequestedBy);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.Priority);
                entity.HasIndex(e => e.Category);
            });

            // ===== BOOKING =====
            modelBuilder.Entity<Booking>(entity =>
            {
                entity.ToTable("Bookings");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Message).HasMaxLength(1000);
                entity.Property(e => e.ConfirmationNotes).HasMaxLength(500);
                entity.Property(e => e.CancellationReason).HasMaxLength(500);
                entity.Property(e => e.CompletionNotes).HasMaxLength(500);
                entity.Property(e => e.Status).HasConversion<string>();

                entity.HasOne(e => e.Property)
                    .WithMany(p => p.Bookings)
                    .HasForeignKey(e => e.PropertyId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Tenant)
                    .WithMany(u => u.Bookings)
                    .HasForeignKey(e => e.TenantId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => e.PropertyId);
                entity.HasIndex(e => e.TenantId);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.ScheduledDate);
                entity.HasIndex(e => new { e.PropertyId, e.ScheduledDate });
            });

            // ===== REVENUE =====
            modelBuilder.Entity<Revenue>(entity =>
            {
                entity.ToTable("Revenues");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.TotalRentCollected).HasPrecision(18, 2);
                entity.Property(e => e.TotalDeposit).HasPrecision(18, 2);
                entity.Property(e => e.TotalServiceFees).HasPrecision(18, 2);
                entity.Property(e => e.TotalUtilities).HasPrecision(18, 2);
                entity.Property(e => e.TotalLateFees).HasPrecision(18, 2);
                entity.Property(e => e.TotalOtherIncome).HasPrecision(18, 2);
                entity.Property(e => e.TotalMaintenanceCost).HasPrecision(18, 2);
                entity.Property(e => e.TotalRefunds).HasPrecision(18, 2);
                entity.Property(e => e.TotalOtherExpenses).HasPrecision(18, 2);
                entity.Property(e => e.GrossRevenue).HasPrecision(18, 2);
                entity.Property(e => e.NetRevenue).HasPrecision(18, 2);
                entity.Property(e => e.OccupancyRate).HasPrecision(5, 2);

                entity.HasOne(e => e.Property)
                    .WithOne(p => p.Revenue)
                    .HasForeignKey<Revenue>(e => e.PropertyId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.PropertyId);
                entity.HasIndex(e => new { e.Year, e.Month });
                entity.HasIndex(e => new { e.PropertyId, e.Year, e.Month }).IsUnique();
            });

            // ===== SYSTEM CONFIGURATION =====
            modelBuilder.Entity<SystemConfiguration>(entity =>
            {
                entity.ToTable("SystemConfigurations");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Key).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Value).HasMaxLength(500).IsRequired();
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.Unit).HasMaxLength(20);
                entity.Property(e => e.Type).HasConversion<string>();

                entity.HasIndex(e => e.Key).IsUnique();
                entity.HasIndex(e => e.Type);
            });

            // ===== CONVERSATION =====
            modelBuilder.Entity<Conversation>(entity =>
            {
                entity.ToTable("Conversations");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Title).HasMaxLength(200);

                entity.HasOne(e => e.User1)
                    .WithMany()
                    .HasForeignKey(e => e.User1Id)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.User2)
                    .WithMany()
                    .HasForeignKey(e => e.User2Id)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Property)
                    .WithMany()
                    .HasForeignKey(e => e.PropertyId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(e => e.Lease)
                    .WithMany()
                    .HasForeignKey(e => e.LeaseId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(e => e.Booking)
                    .WithMany()
                    .HasForeignKey(e => e.BookingId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasIndex(e => e.User1Id);
                entity.HasIndex(e => e.User2Id);
                entity.HasIndex(e => new { e.User1Id, e.User2Id });
                entity.HasIndex(e => e.PropertyId);
                entity.HasIndex(e => e.LastMessageAt);
                entity.HasIndex(e => e.IsActive);
            });

            // ===== MESSAGE =====
            modelBuilder.Entity<Message>(entity =>
            {
                entity.ToTable("Messages");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Content).HasMaxLength(4000).IsRequired();
                entity.Property(e => e.AttachmentUrl).HasMaxLength(500);
                entity.Property(e => e.AttachmentName).HasMaxLength(255);
                entity.Property(e => e.AttachmentMimeType).HasMaxLength(100);
                entity.Property(e => e.Metadata).HasMaxLength(2000);
                entity.Property(e => e.MessageType).HasConversion<string>();

                entity.HasOne(e => e.Conversation)
                    .WithMany(c => c.Messages)
                    .HasForeignKey(e => e.ConversationId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Sender)
                    .WithMany()
                    .HasForeignKey(e => e.SenderId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.ReplyToMessage)
                    .WithMany(m => m.Replies)
                    .HasForeignKey(e => e.ReplyToMessageId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasIndex(e => e.ConversationId);
                entity.HasIndex(e => e.SenderId);
                entity.HasIndex(e => e.SentAt);
                entity.HasIndex(e => new { e.ConversationId, e.SentAt });
                entity.HasIndex(e => new { e.ConversationId, e.IsDeleted });
                entity.HasIndex(e => e.MessageType);
            });

            // ===== DELETED MESSAGE =====
            modelBuilder.Entity<DeletedMessage>(entity =>
            {
                entity.ToTable("DeletedMessages");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.OriginalContent).HasMaxLength(4000).IsRequired();
                entity.Property(e => e.Reason).HasMaxLength(500);

                entity.HasIndex(e => e.MessageId);
                entity.HasIndex(e => e.ConversationId);
                entity.HasIndex(e => e.DeletedBy);
                entity.HasIndex(e => e.DeletedAt);
            });

            // ===== TYPING INDICATOR =====
            modelBuilder.Entity<TypingIndicator>(entity =>
            {
                entity.ToTable("TypingIndicators");
                entity.HasKey(e => e.Id);

                entity.HasOne(e => e.Conversation)
                    .WithMany()
                    .HasForeignKey(e => e.ConversationId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => new { e.ConversationId, e.UserId }).IsUnique();
                entity.HasIndex(e => e.IsTyping);
            });

            // ===== USER PRESENCE =====
            modelBuilder.Entity<UserPresence>(entity =>
            {
                entity.ToTable("UserPresences");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.ConnectionId).HasMaxLength(100);
                entity.Property(e => e.DeviceInfo).HasMaxLength(500);
                entity.Property(e => e.Status).HasConversion<string>();

                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.UserId).IsUnique();
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.ConnectionId);
                entity.HasIndex(e => e.LastSeenAt);
            });
        }
    }
}
