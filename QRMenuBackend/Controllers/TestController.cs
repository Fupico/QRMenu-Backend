using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QRMenuBackend.Data;
using QRMenuBackend.Entities;



[Route("api/[controller]")]
[ApiController]
public class TestController : ControllerBase
{
    private readonly AppDbContext _context;

    public TestController(AppDbContext context)
    {
        _context = context;
    }


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
    }

}
