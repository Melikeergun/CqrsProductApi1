using CqrsProductApi.Data;
using CqrsProductApi.Entities;
using CqrsProductApi.Features.Products.Commands.CreateProduct;
using CqrsProductApi.Features.Products.Commands.DeleteProduct;
using CqrsProductApi.Features.Products.Queries.GetAllProducts;
using CqrsProductApi.Features.Products.Queries.GetProductById;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OpenIddict.Validation.AspNetCore;
using OpenIddict.Abstractions;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Swagger (Token almak için yaptım)
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.OAuth2,
        Flows = new OpenApiOAuthFlows
        {
            Password = new OpenApiOAuthFlow
            {
                TokenUrl = new Uri("/connect/token", UriKind.Relative)
            }
        }
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "oauth2" }
            },
            new List<string>()
        }
    });
});

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    options.UseOpenIddict();
});

builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
})
.AddEntityFrameworkStores<AppDbContext>() // Identity tablolarını AppDbContext'e bağlar
.AddDefaultTokenProviders();

builder.Services.AddOpenIddict()
    .AddCore(opt =>
    {
        opt.UseEntityFrameworkCore().UseDbContext<AppDbContext>();
    })
    .AddServer(opt =>
    {
        opt.SetTokenEndpointUris("connect/token");
        opt.AllowPasswordFlow();
        opt.AllowClientCredentialsFlow();
        opt.DisableAccessTokenEncryption();
        opt.AddDevelopmentEncryptionCertificate()
           .AddDevelopmentSigningCertificate();
        opt.UseAspNetCore()
           .EnableTokenEndpointPassthrough()
           .DisableTransportSecurityRequirement();
    })
    .AddValidation(opt =>
    {
        opt.UseLocalServer();
        opt.UseAspNetCore();
    });

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
});
builder.Services.AddAuthorization();


builder.Services.AddScoped<CreateProductCommandHandler>();
builder.Services.AddScoped<DeleteProductCommandHandler>();
builder.Services.AddScoped<GetAllProductsQueryHandler>();
builder.Services.AddScoped<GetProductByIdQueryHandler>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();


using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();

    var client = await manager.FindByClientIdAsync("test-client");
    if (client != null)
        await manager.DeleteAsync(client);

    await manager.CreateAsync(new OpenIddictApplicationDescriptor
    {
        ClientId = "test-client",
        ClientSecret = "test-secret",
        DisplayName = "Test Client",
        Permissions =
        {
            OpenIddictConstants.Permissions.Endpoints.Token,
            OpenIddictConstants.Permissions.GrantTypes.ClientCredentials,
            OpenIddictConstants.Permissions.GrantTypes.Password,
            OpenIddictConstants.Permissions.Prefixes.Scope + "api",
            OpenIddictConstants.Permissions.ResponseTypes.Token
        }
    });
}

app.Run();
