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
[FuPiCoSecurity]
[HttpPost("addOrUpdateCompany")]
public async Task<IActionResult> AddOrUpdateCompany([FromBody] AddCompanyDto companyDto)
{
    if (!ModelState.IsValid)
{
    return BadRequest(ModelState); // Eğer DTO geçersizse, hata döndür
}

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
            existingCompany.ImageUrl = companyDto.ImageUrl; // ImageUrl güncelleme
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
                ImageUrl = companyDto.ImageUrl, // ImageUrl oluşturma sırasında eklenmeli
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
[HttpPost("addFoodGroup")]
public async Task<IActionResult> AddFoodGroup([FromBody] AddFoodGroupDto foodGroupDto)
{
    if (!ModelState.IsValid)
    {
        return BadRequest(ModelState); // Eğer DTO geçersizse, hata döndür
    }

    try
    {
        // Şirketin var olup olmadığını kontrol et
        var company = await _context.Companies.FirstOrDefaultAsync(c => c.CompanyId == foodGroupDto.CompanyId);
        if (company == null)
        {
            return NotFound("Şirket bulunamadı.");
        }

        // Yeni gıda grubu oluştur
        var newFoodGroup = new FoodGroupEntity
        {
            CompanyId = foodGroupDto.CompanyId,
            GroupName = foodGroupDto.GroupName,
            Description = foodGroupDto.Description,
            ImageUrl = foodGroupDto.ImageUrl,
            CreatedAt = DateTime.Now,
            Invalidated = 1 // Default olarak aktif olsun
        };

        // Veritabanına ekle
        _context.FoodGroups.Add(newFoodGroup);
        await _context.SaveChangesAsync();

        // Başarılı işlem sonrası yeni gıda grubunu döndür
        return Ok(newFoodGroup);
    }
    catch (Exception ex)
    {
        return StatusCode(500, $"Bir hata oluştu: {ex.Message}");
    }
}

[FuPiCoSecurity]
[HttpGet("getFoodGroupsByUserId")]
public async Task<IActionResult> GetFoodGroupsByUserId()
{
    // Token'dan gelen userId'yi al
    var userId = HttpContext.Items["userId"]?.ToString();

    if (string.IsNullOrEmpty(userId))
    {
        return Unauthorized("Kullanıcı ID'si bulunamadı.");
    }

    // userId'ye göre şirketi bul
    var company = await _context.Companies
                                .Include(c => c.FoodGroups)
                                .FirstOrDefaultAsync(c => c.CreatedBy == userId);

    if (company == null)
    {
        return NotFound("Kullanıcının sahip olduğu bir şirket bulunamadı.");
    }

    // Şirketin gıda gruplarını DTO olarak döndür
    var foodGroups = company.FoodGroups.Select(fg => new FoodGroupDto
    {
        FoodGroupId = fg.FoodGroupId,
        GroupName = fg.GroupName,
        Description = fg.Description,
        ImageUrl = fg.ImageUrl,
        CreatedAt = fg.CreatedAt,
        UpdatedAt = fg.UpdatedAt
    }).ToList();

    return Ok(foodGroups);
}

[FuPiCoSecurity]
[HttpDelete("deleteFoodGroup/{id}")]
public async Task<IActionResult> DeleteFoodGroup(int id)
{
    var foodGroup = await _context.FoodGroups.FindAsync(id);
    if (foodGroup == null)
    {
        return NotFound("Gıda grubu bulunamadı.");
    }

    // Gıda grubunu "sil"
    foodGroup.Invalidated = -1;
    foodGroup.UpdatedAt = DateTime.Now;

    _context.FoodGroups.Update(foodGroup);
    await _context.SaveChangesAsync();

    return Ok("Gıda grubu başarıyla silindi (pasif hale getirildi).");
}
[FuPiCoSecurity]
[HttpPut("updateFoodGroup/{id}")]
public async Task<IActionResult> UpdateFoodGroup(int id, [FromBody] UpdateFoodGroupDto updateDto)
{
    if (!ModelState.IsValid)
    {
        return BadRequest(ModelState);  // Geçersiz veri hatası
    }

    var foodGroup = await _context.FoodGroups.FindAsync(id);
    if (foodGroup == null || foodGroup.Invalidated != 1)
    {
        return NotFound("Gıda grubu ya bulunamadı ya da aktif değil.");
    }

    // Gıda grubunu güncelle
    foodGroup.GroupName = updateDto.GroupName;
    foodGroup.Description = updateDto.Description;
    foodGroup.ImageUrl = updateDto.ImageUrl;
    foodGroup.UpdatedAt = DateTime.Now;

    _context.FoodGroups.Update(foodGroup);
    await _context.SaveChangesAsync();

    return Ok(foodGroup);  // Güncellenmiş gıda grubunu döndür
}
[FuPiCoSecurity]
[HttpGet("getActiveFoodGroupsByUserId")]
public async Task<IActionResult> GetActiveFoodGroupsByUserId()
{
    // Token'dan gelen userId'yi al
    var userId = HttpContext.Items["userId"]?.ToString();

    if (string.IsNullOrEmpty(userId))
    {
        return Unauthorized("Kullanıcı ID'si bulunamadı.");
    }

    // userId'ye göre şirketi bul
    var company = await _context.Companies
                                .Include(c => c.FoodGroups)
                                .FirstOrDefaultAsync(c => c.CreatedBy == userId);

    if (company == null)
    {
        return NotFound("Kullanıcının sahip olduğu bir şirket bulunamadı.");
    }

    // Sadece aktif olan gıda gruplarını listele
    var foodGroups = company.FoodGroups
                            .Where(fg => fg.Invalidated == 1)
                            .Select(fg => new FoodGroupDto
                            {
                                FoodGroupId = fg.FoodGroupId,
                                GroupName = fg.GroupName,
                                Description = fg.Description,
                                ImageUrl = fg.ImageUrl,
                                CreatedAt = fg.CreatedAt,
                                UpdatedAt = fg.UpdatedAt
                            })
                            .ToList();

    return Ok(foodGroups);
}


[FuPiCoSecurity]
[HttpPost("addFood")]
public async Task<IActionResult> AddFood([FromBody] AddFoodDto foodDto)
{
    if (!ModelState.IsValid)
    {
        return BadRequest(ModelState);  // Geçersiz veri hatası
    }

    // Gıda grubunun olup olmadığını kontrol et
    var foodGroup = await _context.FoodGroups.FindAsync(foodDto.FoodGroupId);
    if (foodGroup == null)
    {
        return NotFound("Gıda grubu bulunamadı.");
    }

    // Yeni yemek oluştur
    var newFood = new FoodEntity
    {
        FoodGroupId = foodDto.FoodGroupId,
        Name = foodDto.Name,
        Description = foodDto.Description,
        ImageUrl = foodDto.ImageUrl ?? string.Empty,  // Görsel URL boş ise boş string yap
        Price = foodDto.Price,
        CreatedAt = DateTime.Now,
        Invalidated = 1  // Default olarak aktif (Invalidated = 1)
    };

    // Veritabanına ekle
    _context.Foods.Add(newFood);
    await _context.SaveChangesAsync();

    // Başarılı işlem sonrası yeni yemeği döndür
    return Ok(newFood);
}


[FuPiCoSecurity]
[HttpGet("getFoodsByFoodGroupId/{foodGroupId}")]
public async Task<IActionResult> GetFoodsByFoodGroupId(int foodGroupId)
{
    // FoodGroup'ın olup olmadığını kontrol et
    var foodGroup = await _context.FoodGroups
                                  .Include(fg => fg.Foods)
                                  .FirstOrDefaultAsync(fg => fg.FoodGroupId == foodGroupId);

    if (foodGroup == null)
    {
        return NotFound("Gıda grubu bulunamadı.");
    }

    // Gıda grubuna ait yemekleri listele
    var foods = foodGroup.Foods.Select(f => new
    {
        f.FoodId,
        f.Name,
        f.Description,
        f.ImageUrl,
        f.Price,
        f.CreatedAt,
        f.UpdatedAt
    }).ToList();

    return Ok(foods);
}
[FuPiCoSecurity]
[HttpGet("getFoodGroupsAndFoodsByCompanyName/{companyName}")]
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

    // Şirkete ait gıda grupları ve yemekleri listele
    var foodGroups = company.FoodGroups.Select(fg => new
    {
        fg.FoodGroupId,
        fg.GroupName,
        fg.Description,
        fg.ImageUrl,
        fg.CreatedAt,
        fg.UpdatedAt,
        Foods = fg.Foods.Select(f => new
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
   [HttpGet("check")]
    public IActionResult Check()
    {
        return Ok(new { Message = "API çalışıyor!" });
    }
}
