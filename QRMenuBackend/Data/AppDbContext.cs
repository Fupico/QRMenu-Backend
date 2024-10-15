using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using QRMenuBackend.Entities;

namespace QRMenuBackend.Data
{
    public class AppDbContext : IdentityDbContext<IdentityUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Tabloları buraya ekliyoruz
        public DbSet<CompanyEntity> Companies { get; set; }
        public DbSet<FoodGroupEntity> FoodGroups { get; set; }
        public DbSet<FoodEntity> Foods { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Company -> FoodGroup ilişkisi (1-to-many)
            modelBuilder.Entity<CompanyEntity>()
                .HasMany(c => c.FoodGroups)
                .WithOne(fg => fg.Company)
                .HasForeignKey(fg => fg.CompanyId);

            // FoodGroup -> Food ilişkisi (1-to-many)
            modelBuilder.Entity<FoodGroupEntity>()
                .HasMany(fg => fg.Foods)
                .WithOne(f => f.FoodGroup)
                .HasForeignKey(f => f.FoodGroupId);
        }
    }
}
