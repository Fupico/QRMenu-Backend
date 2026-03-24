using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using QRMenuBackend.Entities;

namespace QRMenuBackend.Data;

public class AppDbContext : IdentityDbContext<IdentityUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<CompanyEntity> Companies => Set<CompanyEntity>();
    public DbSet<FoodGroupEntity> FoodGroups => Set<FoodGroupEntity>();
    public DbSet<FoodEntity> Foods => Set<FoodEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<CompanyEntity>()
            .HasMany(company => company.FoodGroups)
            .WithOne(group => group.Company)
            .HasForeignKey(group => group.CompanyId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<FoodGroupEntity>()
            .HasMany(group => group.Foods)
            .WithOne(food => food.FoodGroup)
            .HasForeignKey(food => food.FoodGroupId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
