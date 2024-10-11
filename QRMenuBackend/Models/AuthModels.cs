namespace QRMenuBackend.Models
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }

        public ApiResponse(T data, string message = null)
        {
            Success = true;
            Message = message;
            Data = data;
        }

        public ApiResponse(string message)
        {
            Success = false;
            Message = message;
            Data = default;
        }
    }

    public class LoginModel
    {
        public string? Username { get; set; }
        public string? Password { get; set; }
    }

    public class RegisterDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
    }
}
