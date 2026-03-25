using Microsoft.AspNetCore.Identity;

namespace CqrsProductApi.Entities;

public class AppUser : IdentityUser
{
    public string? FullName { get; set; }
}
