using Microsoft.AspNetCore.Mvc;
using QRMenuBackend.Models; // ApiResponse'ın bulunduğu namespace

public class BaseController : ControllerBase
{
    protected IActionResult HandleResponse<T>(ApiResponse<T> response)
    {
        // Eğer Success ise 200 dönüyor, değilse 400 veya diğer status kodlarını döndürüyorsunuz
        return StatusCode(response.StatusCode, response);
    }

    protected IActionResult SuccessResponse<T>(T data, string message = null)
    {
        var response = new ApiResponse<T>(data, 200, message); // 200 StatusCode belirleniyor
        return HandleResponse(response);
    }

    protected IActionResult ErrorResponse(string message, List<string> errors = null, int statusCode = 400)
    {
        var response = new ApiResponse<string>(message, statusCode, errors);
        return HandleResponse(response);
    }
}
