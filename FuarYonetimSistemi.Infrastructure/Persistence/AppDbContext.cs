using FuarYonetimSistemi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FuarYonetimSistemi.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

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
                .HasForeignKey(f => f.FairId);

            modelBuilder.Entity<FairExpense>()
                .HasOne(f => f.ExpenseType)
                .WithMany()
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
        }
    }
}
