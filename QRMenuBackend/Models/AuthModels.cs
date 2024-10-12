namespace QRMenuBackend.Models
{
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string? Message { get; set; }  // Nullable işareti eklenmiş
    public T? Data { get; set; }  // Nullable işareti eklenmiş

    public List<string> Errors { get; set; } = new List<string>();
    public int StatusCode { get; set; }

    public ApiResponse(T data, int statusCode = 200, string? message = null)  // message nullable olabilir
    {
        Success = true;
        Message = message ?? "Success";
        Data = data;
        StatusCode = statusCode;
    }

    public ApiResponse(string? message, int statusCode = 400, List<string>? errors = null)  // errors nullable olabilir
    {
        Success = false;
        Message = message ?? "Error";
        StatusCode = statusCode;
        Errors = errors ?? new List<string>();
    }
}


public class LoginModel
{
    public string? Username { get; set; }
    public string? Password { get; set; }
}

public class RegisterDto
{
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? Email { get; set; }
}

}
