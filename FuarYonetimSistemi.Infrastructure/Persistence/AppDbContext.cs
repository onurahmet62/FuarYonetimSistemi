using FuarYonetimSistemi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace FuarYonetimSistemi.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Existing DbSets
        public DbSet<User> Users { get; set; }
        public DbSet<Participant> Participants { get; set; }
        public DbSet<Fair> Fairs { get; set; }
        public DbSet<Stand> Stands { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<OfficeExpenseType> OfficeExpenseTypes { get; set; }
        public DbSet<OfficeExpense> OfficeExpenses { get; set; }
        public DbSet<FairExpense> FairExpenses { get; set; }
        public DbSet<FairExpenseType> FairExpenseTypes { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<RepresentativeCompany> RepresentativeCompanies { get; set; }
        public DbSet<ExhibitedProduct> ExhibitedProducts { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<SharedFile> SharedFiles { get; set; }

        // WorkTask Management DbSets
        public DbSet<WorkTask> WorkTasks { get; set; }
        public DbSet<WorkTaskHistory> WorkTaskHistories { get; set; }
        public DbSet<WorkTaskComment> WorkTaskComments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            #region Precision Settings for Stand & Payment
            var standProps = typeof(Stand).GetProperties()
                .Where(p => p.PropertyType == typeof(decimal?) || p.PropertyType == typeof(decimal));

            foreach (var prop in standProps)
            {
                modelBuilder.Entity<Stand>().Property(prop.Name).HasPrecision(18, 2);
            }

            modelBuilder.Entity<Payment>().Property(p => p.Amount).HasPrecision(18, 2);
            modelBuilder.Entity<Fair>().Property(p => p.ActualExpense).HasPrecision(18, 2);
            #endregion

            #region Relationships: Stand <-> Fair & Participant
            modelBuilder.Entity<Stand>()
                .HasOne(s => s.Participant)
                .WithMany(p => p.Stands)
                .HasForeignKey(s => s.ParticipantId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Stand>()
                .HasOne(s => s.Fair)
                .WithMany(f => f.Stands)
                .HasForeignKey(s => s.FairId)
                .OnDelete(DeleteBehavior.Cascade);
            #endregion

            #region Soft Delete Filters
            modelBuilder.Entity<User>().HasQueryFilter(u => !u.IsDeleted);
            modelBuilder.Entity<Participant>().HasQueryFilter(p => !p.IsDeleted);
            modelBuilder.Entity<Stand>().HasQueryFilter(s => !s.IsDeleted);
            modelBuilder.Entity<Payment>().HasQueryFilter(p => !p.IsDeleted);
            modelBuilder.Entity<WorkTask>().HasQueryFilter(t => !t.IsDeleted);
            modelBuilder.Entity<WorkTaskComment>().HasQueryFilter(tc => !tc.IsDeleted);
            #endregion

            #region Fair -> Category
            modelBuilder.Entity<Fair>()
                .HasOne(f => f.Category)
                .WithMany()
                .HasForeignKey(f => f.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
            #endregion

            #region OfficeExpense
            modelBuilder.Entity<OfficeExpense>()
                .HasOne(o => o.ExpenseType)
                .WithMany()
                .HasForeignKey(o => o.OfficeExpenseTypeId)
                .OnDelete(DeleteBehavior.Restrict);
            #endregion

            #region FairExpense
            modelBuilder.Entity<FairExpense>()
                .HasOne(f => f.Fair)
                .WithMany(f => f.FairExpenses)
                .HasForeignKey(f => f.FairId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<FairExpense>()
                .HasOne(f => f.ExpenseType)
                .WithMany(f => f.FairExpenses)
                .HasForeignKey(f => f.FairExpenseTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<FairExpense>()
                .Property(f => f.AccountCode)
                .HasMaxLength(100);
            #endregion

            #region Branch / Brand / ProductCategory / RepCompany / ExhibitedProduct
            modelBuilder.Entity<Branch>()
                .HasOne(b => b.Participant)
                .WithMany(p => p.Branches)
                .HasForeignKey(b => b.ParticipantId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Brand>()
                .HasOne(b => b.Participant)
                .WithMany(p => p.Brands)
                .HasForeignKey(b => b.ParticipantId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProductCategory>()
                .HasOne(pc => pc.Participant)
                .WithMany(p => p.ProductCategories)
                .HasForeignKey(pc => pc.ParticipantId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RepresentativeCompany>()
                .HasOne(rc => rc.Participant)
                .WithMany(p => p.RepresentativeCompanies)
                .HasForeignKey(rc => rc.ParticipantId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ExhibitedProduct>()
                .HasOne(ep => ep.Participant)
                .WithMany(p => p.ExhibitedProducts)
                .HasForeignKey(ep => ep.ParticipantId)
                .OnDelete(DeleteBehavior.Cascade);
            #endregion

            #region Payment -> Stand
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Stand)
                .WithMany(s => s.Payments)
                .HasForeignKey(p => p.StandId)
                .OnDelete(DeleteBehavior.NoAction);
            #endregion

            #region Message Relationships
            modelBuilder.Entity<Message>()
                .HasOne(m => m.Sender)
                .WithMany()
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Receiver)
                .WithMany()
                .HasForeignKey(m => m.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);
            #endregion

            #region WorkTask Management Configurations

            // WorkTask Relationships
            modelBuilder.Entity<WorkTask>()
                .HasOne(t => t.CreatedByUser)
                .WithMany()
                .HasForeignKey(t => t.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<WorkTask>()
                .HasOne(t => t.AssignedToUser)
                .WithMany()
                .HasForeignKey(t => t.AssignedToUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<WorkTask>()
                .HasMany(t => t.WorkTaskHistories)
                .WithOne(th => th.WorkTask)
                .HasForeignKey(th => th.WorkTaskId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<WorkTask>()
                .HasMany(t => t.WorkTaskComments)
                .WithOne(tc => tc.WorkTask)
                .HasForeignKey(tc => tc.WorkTaskId)
                .OnDelete(DeleteBehavior.Cascade);

            // WorkTask History Relationships
            modelBuilder.Entity<WorkTaskHistory>()
                .HasOne(th => th.ChangedByUser)
                .WithMany()
                .HasForeignKey(th => th.ChangedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // WorkTask Comment Relationships
            modelBuilder.Entity<WorkTaskComment>()
                .HasOne(tc => tc.CreatedByUser)
                .WithMany()
                .HasForeignKey(tc => tc.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Enum Conversions
            modelBuilder.Entity<WorkTask>()
                .Property(t => t.Status)
                .HasConversion<int>();

            modelBuilder.Entity<WorkTask>()
                .Property(t => t.Priority)
                .HasConversion<int>();

            // Performance Indexes
            modelBuilder.Entity<WorkTask>()
                .HasIndex(t => t.Status)
                .HasDatabaseName("IX_WorkTasks_Status");

            modelBuilder.Entity<WorkTask>()
                .HasIndex(t => t.AssignedToUserId)
                .HasDatabaseName("IX_WorkTasks_AssignedToUserId");

            modelBuilder.Entity<WorkTask>()
                .HasIndex(t => t.CreatedByUserId)
                .HasDatabaseName("IX_WorkTasks_CreatedByUserId");

            modelBuilder.Entity<WorkTask>()
                .HasIndex(t => t.DueDate)
                .HasDatabaseName("IX_WorkTasks_DueDate");

            modelBuilder.Entity<WorkTask>()
                .HasIndex(t => new { t.Status, t.AssignedToUserId })
                .HasDatabaseName("IX_WorkTasks_Status_AssignedToUserId");

            modelBuilder.Entity<WorkTaskHistory>()
                .HasIndex(th => th.WorkTaskId)
                .HasDatabaseName("IX_WorkTaskHistories_WorkTaskId");

            modelBuilder.Entity<WorkTaskComment>()
                .HasIndex(tc => tc.WorkTaskId)
                .HasDatabaseName("IX_WorkTaskComments_WorkTaskId");

            #endregion
        }
    }
}