using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using System.Security.Claims;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace CqrsProductApi.Controllers;

[ApiController]
public class AuthorizationController : ControllerBase
{
    [HttpPost("~/connect/token")]
    [Consumes("application/x-www-form-urlencoded")]
    [IgnoreAntiforgeryToken]
    [Produces("application/json")]
    public async Task<IActionResult> Exchange()
    {
        var request = HttpContext.GetOpenIddictServerRequest();

        if (request == null)
            return BadRequest("İstek okunamadı.");

        var identity = new ClaimsIdentity(
            TokenValidationParameters.DefaultAuthenticationType,
            Claims.Name,
            Claims.Role);

        if (request.GrantType == "password")
        {
            if (request.Username == "admin" && request.Password == "password123")
            {
                identity.SetClaim(Claims.Subject, "123");
                identity.SetClaim(Claims.Name, "Admin");
                identity.SetClaim(Claims.Role, "Yönetici");
            }
            else
            {
                return BadRequest("Kullanıcı adı veya şifre yanlış.");
            }
        }
        else if (request.GrantType == "client_credentials")
        {
            identity.SetClaim(Claims.Subject, "sistem-istemci");
            identity.SetClaim(Claims.Name, "Sistem");
        }
        else
        {
            return BadRequest($"Desteklenmeyen grant type: {request.GrantType}");
        }

        identity.SetDestinations(claim => new[] { Destinations.AccessToken });

        return SignIn(new ClaimsPrincipal(identity), OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }
}
