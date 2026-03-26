using CqrsProductApi.Entities;
using CqrsProductApi.Features.Auth;
using FastEndpoints;
using Microsoft.AspNetCore.Identity;
using System.Net;

namespace CqrsProductApi1.Features.Auth.Endpoints;


public class LoginEndpoint(
    UserManager<AppUser> userManager,
    SignInManager<AppUser> signInManager,
    IHttpClientFactory httpClientFactory) : Endpoint<LoginRequest, object>
{
    public override void Configure()
    {
        Post("/api/fast/auth/login");
        AllowAnonymous();
    }

    public override async Task HandleAsync(LoginRequest req, CancellationToken ct)
    {
        var user = await userManager.FindByEmailAsync(req.Email ?? string.Empty);
        if (user is null)
        {
            await Send.StringAsync("Kullanıcı bulunamadı.", 400, cancellation: ct);
            return;
        }

        var signInResult = await signInManager.CheckPasswordSignInAsync(
            user, req.Password ?? string.Empty, lockoutOnFailure: true);

        if (!signInResult.Succeeded)
        {
            await Send.StringAsync("E-posta veya şifre hatalı.", 400, cancellation: ct);
            return;
        }

        
        var client = httpClientFactory.CreateClient();
        var baseUrl =
            $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";

        var tokenRequest = new Dictionary<string, string>
        {
            ["grant_type"]    = "password",
            ["username"]      = req.Email!,
            ["password"]      = req.Password!,
            ["client_id"]     = "test-client",
            ["client_secret"] = "test-secret"
        };

        var tokenResponse = await client.PostAsync(
            $"{baseUrl}/connect/token",
            new FormUrlEncodedContent(tokenRequest),
            ct);

        var tokenJson = await tokenResponse.Content.ReadAsStringAsync(ct);

        if (!tokenResponse.IsSuccessStatusCode)
        {
            await Send.ResponseAsync(new { Message = "Token alınamadı.", Detail = tokenJson },
                (int)tokenResponse.StatusCode, cancellation: ct);
            return;
        }

        await Send.StringAsync(tokenJson, 200, "application/json", ct);
    }
}