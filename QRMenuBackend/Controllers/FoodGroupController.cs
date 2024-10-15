using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QRMenuBackend.Data;
using QRMenuBackend.Entities;

[Route("api/[controller]")]
[ApiController]
public class FoodGroupController : ControllerBase
{
    private readonly AppDbContext _context;

    public FoodGroupController(AppDbContext context)
    {
        _context = context;
    }

    // // Menü Grubu Ekle
    // [HttpPost("add")]
    // public async Task<IActionResult> AddFoodGroup([FromBody] FoodGroupEntity foodGroup)
    // {
    //     if (ModelState.IsValid)
    //     {
    //         _context.FoodGroups.Add(foodGroup);
    //         await _context.SaveChangesAsync();
    //         return Ok("Menü grubu başarıyla eklendi.");
    //     }
    //     return BadRequest("Geçersiz veri.");
    // }

    // // Menü Grubu Güncelle
    // [HttpPut("update/{id}")]
    // public async Task<IActionResult> UpdateFoodGroup(int id, [FromBody] FoodGroupEntity updatedGroup)
    // {
    //     var existingGroup = await _context.FoodGroups.FindAsync(id);
    //     if (existingGroup == null) return NotFound("Menü grubu bulunamadı.");

    //     existingGroup.GroupName = updatedGroup.GroupName;
    //     existingGroup.Description = updatedGroup.Description;
    //     existingGroup.UpdatedAt = DateTime.Now;

    //     await _context.SaveChangesAsync();
    //     return Ok("Menü grubu başarıyla güncellendi.");
    // }

    // // Menü Grubu Sil
    // [HttpDelete("delete/{id}")]
    // public async Task<IActionResult> DeleteFoodGroup(int id)
    // {
    //     var foodGroup = await _context.FoodGroups.FindAsync(id);
    //     if (foodGroup == null) return NotFound("Menü grubu bulunamadı.");

    //     _context.FoodGroups.Remove(foodGroup);
    //     await _context.SaveChangesAsync();
    //     return Ok("Menü grubu başarıyla silindi.");
    // }

    // // Firma ismiyle tüm menü gruplarını listele
    // [HttpGet("company/{companyName}")]
    // public async Task<IActionResult> GetMenuByCompanyName(string companyName)
    // {
    //     var company = await _context.Companies
    //                                 .Include(c => c.FoodGroups)
    //                                 .ThenInclude(fg => fg.Foods)
    //                                 .FirstOrDefaultAsync(c => c.Name == companyName);

    //     if (company == null) return NotFound("Firma bulunamadı.");

    //     return Ok(company.FoodGroups.Select(fg => new
    //     {
    //         fg.GroupName,
    //         fg.Description,
    //         Foods = fg.Foods.Select(f => new { f.Name, f.Description, f.ImageUrl, f.Price })
    //     }));
    // }
}
