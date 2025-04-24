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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Stand precision settings
            modelBuilder.Entity<Stand>()
                .Property(s => s.Price)
                .HasPrecision(18, 2);
            modelBuilder.Entity<Stand>()
                .Property(s => s.AmountPaid)
                .HasPrecision(18, 2);
            modelBuilder.Entity<Stand>()
                .Property(s => s.AmountRemaining)
                .HasPrecision(18, 2);

            // Payment precision
            modelBuilder.Entity<Payment>()
                .Property(p => p.Amount)
                .HasPrecision(18, 2);

            // Configure Payment -> Stand relationship with NO ACTION on delete
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Stand)
                .WithMany(s => s.Payments)
                .HasForeignKey(p => p.StandId)
                .OnDelete(DeleteBehavior.NoAction);

           

            // Configure Payment -> Participant relationship if needed
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Participant)
                .WithMany()
                .HasForeignKey(p => p.ParticipantId)
                .OnDelete(DeleteBehavior.NoAction);
        }

    }
}
