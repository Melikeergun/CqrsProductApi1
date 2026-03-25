namespace CqrsProductApi.Features.Auth;

public class LoginRequest
{
    public string? Email { get; set; }
    public string? Password { get; set; }
}
