using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QRMenuBackend.Data;
using QRMenuBackend.Entities;



[Route("api/[controller]")]
[ApiController]
public class AdminController : ControllerBase
{
    private readonly AppDbContext _context;

    public AdminController(AppDbContext context)
    {
        _context = context;
    }


    #region   Company

    [FuPiCoSecurity]
    [HttpGet("get-company")]
    public async Task<IActionResult> GetCompanyByUserId()
    {
        // Token'dan alınan userId'yi al
        var userId = HttpContext.Items["userId"]?.ToString();

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized("Kullanıcı ID'si bulunamadı.");
        }

        // Mevcut bir şirket var mı kontrol et
        var company = await _context.Companies
            .FirstOrDefaultAsync(c => c.CreatedBy == userId);

        if (company == null)
        {
            return NotFound("Şirket bulunamadı.");
        }

        // Sadece istenen alanları döndüren bir DTO oluştur
        var companyDto = new CompanyDto
        {
            CompanyId = company.CompanyId,
            Name = company.Name,
            Domain = company.Domain,
            ImageUrl = company.ImageUrl
        };

        return Ok(companyDto); // Sadece DTO'yu döndür
    }
    [FuPiCoSecurity]
    [HttpPut("update-company")]
    public async Task<IActionResult> UpdateCompany([FromBody] UpdateCompanyDto companyDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized("Kullanıcı ID'si bulunamadı.");
        }

        // Şirketi userId ile buluyoruz, şirket kullanıcı tarafından daha önce oluşturulmuş olmalı
        var existingCompany = await _context.Companies
            .FirstOrDefaultAsync(c => c.CreatedBy == userId);

        if (existingCompany == null)
        {
            return NotFound("Kullanıcıya ait şirket bulunamadı.");
        }

        // Şirket bilgilerini güncelle
        existingCompany.Name = companyDto.Name;
        existingCompany.Domain = $"https://qrmenu.fupico.com/menu/{userId}"; // Domain, userId ile dinamik olarak güncelleniyor
        existingCompany.ImageUrl = companyDto.ImageUrl;
        existingCompany.UpdatedAt = DateTime.Now;
        existingCompany.UpdatedBy = userId;

        _context.Companies.Update(existingCompany);
        await _context.SaveChangesAsync();

        return Ok(existingCompany);
    }


    #endregion


    #region Food Group
    [FuPiCoSecurity]
    [HttpGet("get-food-groups")]
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
    [HttpPost("add-food-group")]
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
    [HttpPut("update-food-group/{id}")]
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
    [HttpDelete("delete-food-group/{id}")]
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
    [HttpGet("get-foods-by-food-group-id/{foodGroupId}")]
    public async Task<IActionResult> GetFoodsByFoodGroupId(int foodGroupId)
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

        // Belirtilen foodGroupId'ye göre yemekleri bul ve sadece aktif olanları getir
        var foods = await _context.Foods
                                  .Where(f => f.FoodGroupId == foodGroupId && f.Invalidated == 1)
                                  .Select(f => new
                                  {
                                      f.FoodId,
                                      f.Name,
                                      f.Description,
                                      f.ImageUrl,
                                      f.Price,
                                      f.CreatedAt,
                                      f.UpdatedAt
                                  })
                                  .ToListAsync();

        if (foods == null || !foods.Any())
        {
            return NotFound("Bu gruba ait yemekler bulunamadı.");
        }

        return Ok(foods);
    }


    [FuPiCoSecurity]
    [HttpPost("add-food")]
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
    [HttpPut("update-food/{id}")]
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
    [FuPiCoSecurity]
    [HttpDelete("delete-food/{id}")]
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

        return Ok(new { message = $"{food.FoodGroupId}" });
    }
    #endregion


    [FuPiCoSecurity]
    [HttpPost("upload-image")]
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
