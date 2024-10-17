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
[HttpGet("getFoodGroupsByUserId")]
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

[HttpGet("getMenu/{Id}")]
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

    // Şirkete ait gıda grupları ve yemekleri listele
    var foodGroups = company.FoodGroups.Select(fg => new
    {
        fg.Company.Name,
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



    #region   Company
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

      #endregion


#region Food Group
  
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

#endregion     


#region Food

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

     [HttpPut("updateFood/{id}")]
    public async Task<IActionResult> UpdateFood(int id, [FromBody] UpdateFoodDto updateDto)
    {
        var food = await _context.Foods.FindAsync(id);

        if (food == null)
        {
            return NotFound(new { message = "Food not found" });
        }

        // Sadece gelen verileri güncelleyebilirsin
        if (!string.IsNullOrEmpty(updateDto.Name))
            food.Name = updateDto.Name;

        if (!string.IsNullOrEmpty(updateDto.Description))
            food.Description = updateDto.Description;

        if (!string.IsNullOrEmpty(updateDto.ImageUrl))
            food.ImageUrl = updateDto.ImageUrl;

        if (updateDto.Price.HasValue)
            food.Price = updateDto.Price.Value;

        if (updateDto.Invalidated.HasValue)
            food.Invalidated = updateDto.Invalidated.Value;

        food.UpdatedAt = DateTime.UtcNow;  // Güncelleme zamanını ayarla

        _context.Foods.Update(food);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Food updated successfully" });
    }

  [HttpDelete("deleteFood/{id}")]
    public async Task<IActionResult> DeleteFood(int id)
    {
        var food = await _context.Foods.FindAsync(id);

        if (food == null)
        {
            return NotFound(new { message = "Food not found" });
        }

        // Invalidated alanını -1 yaparak soft delete işlemi yapıyoruz
        food.Invalidated = -1;
        food.UpdatedAt = DateTime.UtcNow; // Güncelleme zamanını güncelliyoruz

        _context.Foods.Update(food);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Food invalidated successfully" });
    }
#endregion



       [HttpGet("check")]
    public IActionResult Check()
    {
        return Ok(new { Message = "API çalışıyor!" });
    }

[FuPiCoSecurity]
       [HttpGet("check2")]
    public IActionResult Check2()
    {
        return Ok(new { Message = "API JWTLİ çalışıyor!" });
    }[FuPiCoSecurity]
[HttpPost("uploadImage")]
public async Task<IActionResult> UploadImage(IFormFile image)
{
    // Token'dan gelen userId'yi al
    var userId = HttpContext.Items["userId"]?.ToString();

    if (string.IsNullOrEmpty(userId))
    {
        return Unauthorized("Kullanıcı ID'si bulunamadı.");
    }

    if (image == null || image.Length == 0)
    {
        return BadRequest("Resim seçilmedi.");
    }

    // Resimlerin kaydedileceği wwwroot/images klasörünü oluşturuyoruz
    var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", userId);

    // Klasör yoksa oluştur
    if (!Directory.Exists(folderPath))
    {
        Directory.CreateDirectory(folderPath);
    }

    // Tarihi kullanarak dosya ismi oluşturuyoruz (userId ve anlık tarih ile)
    var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
    var fileName = $"{userId}_{timestamp}{Path.GetExtension(image.FileName)}";
    var filePath = Path.Combine(folderPath, fileName);

    // Resmi sunucuya kaydediyoruz
    using (var stream = new FileStream(filePath, FileMode.Create))
    {
        await image.CopyToAsync(stream);
    }

    // Resmin sunucudaki yolunu URL formatında döndürüyoruz
    var imageUrl = $"/images/{userId}/{fileName}";

    return Ok(new { imageUrl });
}



}
