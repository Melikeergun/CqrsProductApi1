using CqrsProductApi.Data;
using CqrsProductApi.Features.Products.Commands.CreateProduct;
using CqrsProductApi.Features.Products.Commands.DeleteProduct;
using CqrsProductApi.Features.Products.Queries.GetAllProducts;
using CqrsProductApi.Features.Products.Queries.GetProductById;
using CqrsProductApi.Repositories;
using Microsoft.EntityFrameworkCore;
using OpenIddict.Validation.AspNetCore;
using OpenIddict.Abstractions;
using OpenIddict.Server;
using OpenIddict.Server.AspNetCore;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.OAuth2,
        Flows = new OpenApiOAuthFlows
        {
            ClientCredentials = new OpenApiOAuthFlow
            {
                TokenUrl = new Uri("/connect/token", UriKind.Relative)
            },
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
    options.UseSqlite(builder.Configuration.GetConnectionString("SqlServer"));
    options.UseOpenIddict();
});

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

builder.Services.AddAuthentication(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);
builder.Services.AddAuthorization();

builder.Services.AddScoped<IProductRepository, ProductRepository>();
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

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
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
