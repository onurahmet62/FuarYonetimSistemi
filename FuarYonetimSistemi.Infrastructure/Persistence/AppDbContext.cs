using FuarYonetimSistemi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FuarYonetimSistemi.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // DbSet'ler: Veritabanı tablosu ile ilişkilendirilmiş model sınıfları
        public DbSet<User> Users { get; set; }
        public DbSet<Participant> Participants { get; set; }
        public DbSet<Fair> Fairs { get; set; }
        public DbSet<Stand> Stands { get; set; }
     
       
        public DbSet<Category> Categories { get; set; } // Add DbSet for Category

        // Veritabanı model yapılandırması
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User - UserRole
            modelBuilder.Entity<User>()
                .Property(u => u.Role)
                .HasConversion<int>();

            // Fuar (Fair) - Stand ilişkilendirmesi
            modelBuilder.Entity<Fair>()
                .HasMany(f => f.Stands)
                .WithOne(s => s.Fair)
                .HasForeignKey(s => s.FairId);

            // Stand - Participant ilişkilendirmesi
            modelBuilder.Entity<Stand>()
                .HasOne(s => s.Participant)
                .WithMany(p => p.Stands)
                .HasForeignKey(s => s.ParticipantId)
                .OnDelete(DeleteBehavior.Cascade); // Katılımcı silindiğinde ilgili Stand'lar silinsin

            // Fuar (Fair) - Category ilişkilendirmesi
            modelBuilder.Entity<Fair>()
                .HasOne(f => f.Category)
                .WithMany(c => c.Fairs)
                .HasForeignKey(f => f.CategoryId)
                .OnDelete(DeleteBehavior.Restrict); // Category silindiğinde ilgili Fuar'lar silinmesin
        }

    }
}
