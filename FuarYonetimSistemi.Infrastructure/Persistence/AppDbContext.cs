using FuarYonetimSistemi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FuarYonetimSistemi.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // DbSet'ler
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
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Stand precision settings
            modelBuilder.Entity<Stand>()
                .Property(s => s.UnitPrice)
                .HasPrecision(18, 2);
            modelBuilder.Entity<Stand>()
                .Property(s => s.SaleAmountWithoutVAT)
                .HasPrecision(18, 2);
            modelBuilder.Entity<Stand>()
                .Property(s => s.ElectricityConnectionFee)
                .HasPrecision(18, 2);
            modelBuilder.Entity<Stand>()
                .Property(s => s.ThirdPartyInsuranceShare)
                .HasPrecision(18, 2);
            modelBuilder.Entity<Stand>()
                .Property(s => s.StandSetupIncome)
                .HasPrecision(18, 2);
            modelBuilder.Entity<Stand>()
                .Property(s => s.SolidWasteFee)
                .HasPrecision(18, 2);
            modelBuilder.Entity<Stand>()
                .Property(s => s.AdvertisingIncome)
                .HasPrecision(18, 2);
            modelBuilder.Entity<Stand>()
                .Property(s => s.ContractAmountWithoutVAT)
                .HasPrecision(18, 2);
            modelBuilder.Entity<Stand>()
                .Property(s => s.VAT10Amount)
                .HasPrecision(18, 2);
            modelBuilder.Entity<Stand>()
                .Property(s => s.VAT20Amount)
                .HasPrecision(18, 2);
            modelBuilder.Entity<Stand>()
                .Property(s => s.StampTaxAmount)
                .HasPrecision(18, 2);
            modelBuilder.Entity<Stand>()
                .Property(s => s.TotalAmountWithVAT)
                .HasPrecision(18, 2);
            modelBuilder.Entity<Stand>()
                .Property(s => s.TotalReturnInvoice)
                .HasPrecision(18, 2);
            modelBuilder.Entity<Stand>()
                .Property(s => s.BarterInvoiceAmount)
                .HasPrecision(18, 2);
            modelBuilder.Entity<Stand>()
                .Property(s => s.CashCollection)
                .HasPrecision(18, 2);
            modelBuilder.Entity<Stand>()
                .Property(s => s.DocumentCollection)
                .HasPrecision(18, 2);
            modelBuilder.Entity<Stand>()
                .Property(s => s.Balance)
                .HasPrecision(18, 2);
            modelBuilder.Entity<Stand>()
                .Property(s => s.ReceivablesInLaw)
                .HasPrecision(18, 2);
            modelBuilder.Entity<Stand>()
                .Property(s => s.CollectibleBalance)
                .HasPrecision(18, 2);
            modelBuilder.Entity<Stand>()
                .Property(s => s.BarterAmount)
                .HasPrecision(18, 2);
            modelBuilder.Entity<Stand>()
                .Property(s => s.BarterBalance)
                .HasPrecision(18, 2);

            // Payment precision settings
            modelBuilder.Entity<Payment>()
                .Property(p => p.Amount)
                .HasPrecision(18, 2);

            // Configure Payment -> Stand relationship with NO ACTION on delete
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Stand)
                .WithMany(s => s.Payments)
                .HasForeignKey(p => p.StandId)
                .OnDelete(DeleteBehavior.NoAction);

         

            // Configure Stand -> Participant relationship
            modelBuilder.Entity<Stand>()
                .HasOne(s => s.Participant)
                .WithMany(p => p.Stands)
                .HasForeignKey(s => s.ParticipantId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Stand -> Fair relationship
            modelBuilder.Entity<Stand>()
                .HasOne(s => s.Fair)
                .WithMany(f => f.Stands)
                .HasForeignKey(s => s.FairId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Fair -> Category relationship
            modelBuilder.Entity<Fair>()
                .HasOne(f => f.Category)
                .WithMany()
                .HasForeignKey(f => f.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<User>().HasQueryFilter(u => !u.IsDeleted);


            // Global query filters (Soft delete)
            modelBuilder.Entity<User>().HasQueryFilter(u => !u.IsDeleted);
            modelBuilder.Entity<Participant>().HasQueryFilter(p => !p.IsDeleted);
            modelBuilder.Entity<Stand>().HasQueryFilter(s => !s.IsDeleted);
            modelBuilder.Entity<Payment>().HasQueryFilter(p => !p.IsDeleted);

            // Configure OfficeExpense relationship
            modelBuilder.Entity<OfficeExpense>()
                .HasOne(o => o.ExpenseType)
                .WithMany() // Eğer bir ExpenseType birden fazla OfficeExpense’e sahip oluyorsa
                .HasForeignKey(o => o.OfficeExpenseTypeId)
                .OnDelete(DeleteBehavior.Restrict); // Silme işlemi yapıldığında ne olacağına karar verin.

            modelBuilder.Entity<FairExpense>()
               .HasOne(f => f.Fair)
               .WithMany(f => f.FairExpenses)
               .HasForeignKey(f => f.FairId);

            modelBuilder.Entity<FairExpense>()
                .HasOne(f => f.ExpenseType)
                .WithMany()
                .HasForeignKey(f => f.FairExpenseTypeId);

            modelBuilder.Entity<FairExpense>()
                .Property(f => f.AccountCode)
                .HasMaxLength(100);

            modelBuilder.Entity<Fair>()
               .HasMany(f => f.FairExpenses)
               .WithOne(fe => fe.Fair)
               .HasForeignKey(fe => fe.FairId)
               .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<FairExpense>()
                .HasOne(fe => fe.ExpenseType)
                .WithMany()
                .HasForeignKey(fe => fe.FairExpenseTypeId)
                .OnDelete(DeleteBehavior.Restrict); // Gider tipi silinirse gider tablosunu etkileme

            modelBuilder.Entity<Fair>()
                .Property(p => p.ActualExpense)
                .HasPrecision(18, 2);

        }
    }
}
