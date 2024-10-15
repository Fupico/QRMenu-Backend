using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QRMenuBackend.Data;
using QRMenuBackend.Entities;



[Route("api/[controller]")]
[ApiController]
public class FoodController : ControllerBase
{
    private readonly AppDbContext _context;

    public FoodController(AppDbContext context)
    {
        _context = context;
    }
[FuPiCoSecurity]
[HttpPost("addOrUpdateCompany")]
public async Task<IActionResult> AddOrUpdateCompany([FromBody] AddCompanyDto companyDto)
{
    // Token'dan alınan userId'yi al
    var userId = HttpContext.Items["userId"]?.ToString();
    
    if (string.IsNullOrEmpty(userId))
    {
        return Unauthorized("Kullanıcı ID'si bulunamadı.");
    }

    if (ModelState.IsValid)
    {
        // Mevcut bir şirket var mı kontrol et
        var existingCompany = await _context.Companies
            .FirstOrDefaultAsync(c => c.CreatedBy == userId);

        if (existingCompany != null)
        {
            // Şirket var, güncelleme yap
            existingCompany.Name = companyDto.Name;
            existingCompany.Domain = companyDto.Domain;
            existingCompany.UpdatedAt = DateTime.Now;  // Güncelleme zamanı eklenir
            existingCompany.UpdatedBy =userId;  // Güncelleme zamanı eklenir
            _context.Companies.Update(existingCompany);
            await _context.SaveChangesAsync();
            return Ok(existingCompany);  // Güncellenmiş şirketi döndür
        }
        else
        {
            // Şirket yok, yeni şirket ekle
            var newCompany = new CompanyEntity
            {
                Name = companyDto.Name,
                Domain = companyDto.Domain,
                CreatedBy = userId,  // CreatedBy olarak userId'yi kullanıyoruz
                CreatedAt = DateTime.Now  // Şirketin oluşturulma zamanı otomatik olarak atanır
            };

            _context.Companies.Add(newCompany);
            await _context.SaveChangesAsync();
            return Ok(newCompany);  // Yeni eklenmiş şirketi döndür
        }
    }

    return BadRequest("Geçersiz veri.");
}





[FuPiCoSecurity]
[HttpPost("addOrUpdateFoodGroup")]
public async Task<IActionResult> AddOrUpdateFoodGroup([FromBody] AddFoodGroupDto foodGroupDto)
{
    // Token'dan gelen userId'yi al
    var userId = HttpContext.Items["userId"]?.ToString();

    if (string.IsNullOrEmpty(userId))
    {
        return Unauthorized("Kullanıcı ID'si bulunamadı.");
    }

    // Kullanıcının oluşturduğu şirketi al
    var company = await _context.Companies.FirstOrDefaultAsync(c => c.CreatedBy == userId);
    if (company == null)
    {
        return NotFound("Şirket bulunamadı.");
    }

    // Aynı şirket altında aynı grup ismi ve açıklamaya sahip bir gıda grubu var mı kontrol et
    var existingFoodGroup = await _context.FoodGroups
        .FirstOrDefaultAsync(fg => fg.CompanyId == company.CompanyId 
&& fg.GroupName == foodGroupDto.GroupName
&& fg.Description == foodGroupDto.Description);
    
    if (existingFoodGroup != null)
    {
        // Gıda grubu var, güncelleme yap
        existingFoodGroup.UpdatedAt = DateTime.Now;  // Güncelleme zamanı
        _context.FoodGroups.Update(existingFoodGroup);
        await _context.SaveChangesAsync();
        return Ok(existingFoodGroup);  // Güncellenmiş gıda grubunu döndür
    }
    else
    {
        // Yeni gıda grubu ekle
        var newFoodGroup = new FoodGroupEntity
        {
            CompanyId = company.CompanyId,
            GroupName = foodGroupDto.GroupName,
            Description = foodGroupDto.Description,
            CreatedAt = DateTime.Now  // Oluşturulma zamanı
        };

        _context.FoodGroups.Add(newFoodGroup);
        await _context.SaveChangesAsync();
        return Ok(newFoodGroup);  // Yeni eklenen gıda grubunu döndür
    }
}



[FuPiCoSecurity]

[HttpPost("addFood")]
public async Task<IActionResult> AddFood([FromBody] AddFoodDto foodDto)
{
    var foodGroup = await _context.FoodGroups.FindAsync(foodDto.FoodGroupId);
    if (foodGroup == null)
        return NotFound("FoodGroup bulunamadı.");

    var food = new FoodEntity
    {
        FoodGroupId = foodDto.FoodGroupId,
        Name = foodDto.Name,
        Description = foodDto.Description,
        ImageUrl = foodDto.ImageUrl,
        Price = foodDto.Price,
        CreatedAt = DateTime.Now
    };

    _context.Foods.Add(food);
    await _context.SaveChangesAsync();
    return Ok(food);
}
 
 
 
 
 
 
 
 
 
 
 
 
 
 
 
[FuPiCoSecurity]
 
 
    [HttpGet("company/{companyName}/menu")]
public async Task<IActionResult> GetMenuByCompanyName(string companyName)
{
    var company = await _context.Companies
                                .Include(c => c.FoodGroups)
                                .ThenInclude(fg => fg.Foods)
                                .FirstOrDefaultAsync(c => c.Name == companyName);

    if (company == null) return NotFound("Şirket bulunamadı.");

    return Ok(company.FoodGroups.Select(fg => new
    {
        fg.GroupName,
        fg.Description,
        Foods = fg.Foods.Select(f => new { f.Name, f.Description, f.Price, f.ImageUrl })
    }));
}














    // // Yemek Ekle
    // [HttpPost("add")]
    // public async Task<IActionResult> AddFood([FromBody] FoodEntity food)
    // {
    //     if (ModelState.IsValid)
    //     {
    //         _context.Foods.Add(food);
    //         await _context.SaveChangesAsync();
    //         return Ok("Yemek başarıyla eklendi.");
    //     }
    //     return BadRequest("Geçersiz veri.");
    // }

    // // Yemek Güncelle
    // [HttpPut("update/{id}")]
    // public async Task<IActionResult> UpdateFood(int id, [FromBody] FoodEntity updatedFood)
    // {
    //     var existingFood = await _context.Foods.FindAsync(id);
    //     if (existingFood == null) return NotFound("Yemek bulunamadı.");

    //     existingFood.Name = updatedFood.Name;
    //     existingFood.Description = updatedFood.Description;
    //     existingFood.Price = updatedFood.Price;
    //     existingFood.ImageUrl = updatedFood.ImageUrl;
    //     existingFood.UpdatedAt = DateTime.Now;

    //     await _context.SaveChangesAsync();
    //     return Ok("Yemek başarıyla güncellendi.");
    // }

    // // Yemek Sil
    // [HttpDelete("delete/{id}")]
    // public async Task<IActionResult> DeleteFood(int id)
    // {
    //     var food = await _context.Foods.FindAsync(id);
    //     if (food == null) return NotFound("Yemek bulunamadı.");

    //     _context.Foods.Remove(food);
    //     await _context.SaveChangesAsync();
    //     return Ok("Yemek başarıyla silindi.");
    // }
}
