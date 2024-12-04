using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QRMenuBackend.Data;
using QRMenuBackend.Entities;



[Route("api/[controller]")]
[ApiController]
public class MenuController : ControllerBase
{
    private readonly AppDbContext _context;

    public MenuController(AppDbContext context)
    {
        _context = context;
    }



    [HttpGet("get-menu/{Id}")]
    public async Task<IActionResult> GetFoodGroupsAndFoodsFromToken(string Id)
    {
        if (string.IsNullOrEmpty(Id))
        {
            return Unauthorized("Kullanıcı ID'si bulunamadı.");
        }

        // userId'ye göre şirketi bul (CreatedBy alanına göre)
        var company = await _context.Companies
                                    .Include(c => c.FoodGroups)
                                    .ThenInclude(fg => fg.Foods)
                                    .FirstOrDefaultAsync(c => c.CreatedBy == Id);

        if (company == null)
        {
            return NotFound("Kullanıcının sahip olduğu bir şirket bulunamadı.");
        }

        // Şirkete ait gıda grupları ve yemekleri listele (invalidated değeri -1 olanlar hariç)
        var foodGroups = company.FoodGroups
            .Where(fg => fg.Invalidated != -1)  // invalidated != -1 filtreleme
            .Select(fg => new
            {
                fg.Company.Name,
                fg.FoodGroupId,
                fg.GroupName,
                fg.Description,
                fg.ImageUrl,
                fg.Company.CompanyUrl,
                fg.Company.IsActiveCompanyImage,
                fg.Company.SelectedMenu,
             
                Foods = fg.Foods
                         .Where(f => f.Invalidated != -1)  // invalidated != -1 filtreleme
                         .Select(f => new
                         {
                             f.FoodId,
                             f.Name,
                             f.Description,
                             f.ImageUrl,
                             Price = f.Price != 0 ? f.Price : 0.0m, // Fiyat boşsa 0.0 atanır
                             Price2 = f.Price2 ?? 0.0m, // Fiyat2 null ise 0.0 atanır
                             Price3 = f.Price3 ?? 0.0m, // Fiyat3 null ise 0.0 atanır
                             IsGroupPrice = f.IsGroupPrice != 0 ? f.IsGroupPrice : 1, // Gruplama fiyatı yoksa varsayılan 1
                             PriceDesc = string.IsNullOrEmpty(f.PriceDesc) ? "" : f.PriceDesc, // Açıklama yoksa varsayılan
                             PriceDesc2 = string.IsNullOrEmpty(f.PriceDesc2) ? "" : f.PriceDesc2,
                             PriceDesc3 = string.IsNullOrEmpty(f.PriceDesc3) ? "" : f.PriceDesc3,

                         }).ToList()
            }).ToList();

        return Ok(foodGroups);
    }
    [HttpGet("get-menu-with-company-name/{companyName}")]
    public async Task<IActionResult> GetFoodGroupsAndFoodsByCompanyName(string companyName)
    {
        // Şirketi companyName ile bul
        var company = await _context.Companies
                                    .Include(c => c.FoodGroups)
                                    .ThenInclude(fg => fg.Foods)
                                    .FirstOrDefaultAsync(c => c.Name == companyName);

        if (company == null)
        {
            return NotFound("Şirket bulunamadı.");
        }

        // Şirkete ait gıda grupları ve yemekleri listele (invalidated değeri -1 olanlar hariç)
        var foodGroups = company.FoodGroups
            .Where(fg => fg.Invalidated != -1)  // invalidated != -1 filtreleme
            .Select(fg => new
            {
                fg.FoodGroupId,
                fg.GroupName,
                fg.Description,
                fg.ImageUrl,
                fg.CreatedAt,
                fg.UpdatedAt,
                Foods = fg.Foods
                         .Where(f => f.Invalidated != -1)  // invalidated != -1 filtreleme
                         .Select(f => new
                         {
                             f.FoodId,
                             f.Name,
                             f.Description,
                             f.ImageUrl,
                             f.Price,
                             f.CreatedAt,
                             f.UpdatedAt
                         }).ToList()
            }).ToList();

        return Ok(foodGroups);
    }


}
