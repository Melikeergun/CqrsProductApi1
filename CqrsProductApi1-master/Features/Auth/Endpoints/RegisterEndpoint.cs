using CqrsProductApi.Entities;
using CqrsProductApi.Features.Auth;
using FastEndpoints;
using Microsoft.AspNetCore.Identity;

namespace CqrsProductApi1.Features.Auth.Endpoints;

public class RegisterEndpoint(UserManager<AppUser> userManager)
    : Endpoint<RegisterRequest, object>
{
    public override void Configure()
    {
        Post("/api/fast/auth/register");
        AllowAnonymous();
    }

    public override async Task HandleAsync(RegisterRequest req, CancellationToken ct)
    {
        var userExists = await userManager.FindByEmailAsync(req.Email ?? string.Empty);

        if (userExists is not null)
        {
            await Send.StringAsync(
                "Bu e-posta adresi zaten kayıtlı.",
                400,
                cancellation: ct);
            return;
        }

        var user = new AppUser
        {
            UserName = req.Email,
            Email = req.Email,
            FullName = req.FullName
        };

        var result = await userManager.CreateAsync(user, req.Password ?? string.Empty);

        if (!result.Succeeded)
        {
            var errorsList = result.Errors.Select(e => e.Description).ToList();

            await Send.ResponseAsync(new
            {
                Message = "Kayıt başarısız.",
                Errors = errorsList
            }, 400, cancellation: ct);
            return;
        }

        await Send.OkAsync(new
        {
            Message = "Kayıt başarılı."
        }, ct);
    }
}