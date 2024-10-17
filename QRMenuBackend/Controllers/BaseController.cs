using Microsoft.AspNetCore.Mvc;
using QRMenuBackend.Models;

public class BaseController : ControllerBase
{
    protected IActionResult HandleResponse<T>(ApiResponse<T> response)
    {
        // Eğer Success ise 200, değilse status code'a göre dönüyor
        return StatusCode(response.StatusCode, response);
    }

    protected IActionResult SuccessResponse<T>(T data, string? message = null)  // Nullable message kontrolü
    {
        var response = new ApiResponse<T>(data, 200, message ?? "Success");
        return HandleResponse(response);
    }

    protected IActionResult ErrorResponse(string message, List<string>? errors = null, int statusCode = 400)  // Nullable errors kontrolü
    {
        var response = new ApiResponse<string>(message, statusCode, errors ?? new List<string>());  // errors null ise boş liste atanır
        return HandleResponse(response);
    }
}
