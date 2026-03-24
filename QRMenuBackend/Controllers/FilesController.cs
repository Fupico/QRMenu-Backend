using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QRMenuBackend.Data;
using QRMenuBackend.Entities;



[Route("api/[controller]")]
[ApiController]
public class FilesController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _environment;

    public FilesController(AppDbContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }




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
        var folderPath = Path.Combine(_environment.WebRootPath, "images", userId);

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
