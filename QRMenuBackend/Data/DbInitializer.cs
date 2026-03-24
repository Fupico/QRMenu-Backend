using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QRMenuBackend.Entities;

namespace QRMenuBackend.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(IServiceProvider services, IConfiguration configuration)
    {
        await using var scope = services.CreateAsyncScope();

        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

        await context.Database.MigrateAsync();
        await SeedAdminAsync(userManager, configuration);
        await SeedBaseMenuAsync(context);
    }

    private static async Task SeedAdminAsync(UserManager<IdentityUser> userManager, IConfiguration configuration)
    {
        const string adminUserName = "admin";
        var adminEmail = configuration["SeedData:AdminEmail"] ?? "admin@qrmenu.local";
        var adminPassword = configuration["SeedData:AdminPassword"] ?? "ChangeMe123!";

        if (await userManager.FindByNameAsync(adminUserName) is not null)
        {
            return;
        }

        var adminUser = new IdentityUser
        {
            UserName = adminUserName,
            Email = adminEmail,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(adminUser, adminPassword);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(error => error.Description));
            throw new InvalidOperationException($"Seed admin user could not be created: {errors}");
        }
    }

    private static async Task SeedBaseMenuAsync(AppDbContext context)
    {
        if (await context.Companies.AnyAsync())
        {
            return;
        }

        var now = DateTime.UtcNow;

        var company = new CompanyEntity
        {
            Name = "Demo QR Menu",
            Domain = "demo.qrmenu.local",
            CompanyUrl = "https://demo.qrmenu.local",
            Phone = "+90 555 000 0000",
            Adress = "Istanbul",
            IsActiveCompanyImage = false,
            SelectedMenu = 1,
            CreatedAt = now,
            CreatedBy = "seed"
        };

        var group = new FoodGroupEntity
        {
            Company = company,
            GroupName = "Baslangiclar",
            Description = "Ornek kategori",
            CreatedAt = now,
            Invalidated = 0
        };

        var food = new FoodEntity
        {
            FoodGroup = group,
            Name = "Gunun Corbasi",
            Description = "Ornek urun",
            ImageUrl = string.Empty,
            IsGroupPrice = 0,
            PriceDesc = "Tek Fiyat",
            PriceDesc2 = string.Empty,
            PriceDesc3 = string.Empty,
            Price = 120,
            CreatedAt = now,
            Invalidated = 0
        };

        await context.Companies.AddAsync(company);
        await context.FoodGroups.AddAsync(group);
        await context.Foods.AddAsync(food);
        await context.SaveChangesAsync();
    }
}
