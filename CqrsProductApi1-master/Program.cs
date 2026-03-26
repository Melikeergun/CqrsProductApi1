using CqrsProductApi.Data;
using CqrsProductApi.Entities;
using CqrsProductApi.Features.Products.Commands.CreateProduct;
using CqrsProductApi.Features.Products.Commands.DeleteProduct;
using CqrsProductApi.Features.Products.Queries.GetAllProducts;
using CqrsProductApi.Features.Products.Queries.GetProductById;
using FastEndpoints;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using OpenIddict.Abstractions;
using OpenIddict.Validation.AspNetCore;
using CqrsProductApi1.Extensions;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddFastEndpoints();
builder.Services.AddHttpClient();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "CqrsProductApi",
        Version = "v1"
    });

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
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "oauth2"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"));

    options.UseOpenIddict();
});

// Identity alaným
builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

// OpenIddict alaným
builder.Services.AddOpenIddict()
    .AddCore(options =>
    {
        options.UseEntityFrameworkCore()
               .UseDbContext<AppDbContext>();
    })
    .AddServer(options =>
    {
        options.SetTokenEndpointUris("/connect/token");

        options.AllowPasswordFlow();
        options.AllowClientCredentialsFlow();

        options.AcceptAnonymousClients();

        options.DisableAccessTokenEncryption();

        options.AddDevelopmentEncryptionCertificate();
        options.AddDevelopmentSigningCertificate();

        options.RegisterScopes("api");

        options.UseAspNetCore()
               .EnableTokenEndpointPassthrough();
    })
    .AddValidation(options =>
    {
        options.UseLocalServer();
        options.UseAspNetCore();
    });


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme =
        OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;

    options.DefaultChallengeScheme =
        OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
});

builder.Services.AddAuthorization();

builder.Services.AddScoped<CreateProductCommandHandler>();
builder.Services.AddScoped<DeleteProductCommandHandler>();
builder.Services.AddScoped<GetAllProductsQueryHandler>();
builder.Services.AddScoped<GetProductByIdQueryHandler>();

builder.Services.AddEndpoints(typeof(Program).Assembly);  //bu extension ile projemdeki tüm enpointeri otomatik olarak taramýþ oldum ve ekledim
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.UseFastEndpoints();
app.MapControllers();


using (var scope = app.Services.CreateScope())
{
    var manager = scope.ServiceProvider
        .GetRequiredService<IOpenIddictApplicationManager>();

    const string clientId = "test-client";

    var existingClient = await manager.FindByClientIdAsync(clientId);

    if (existingClient is not null)
        await manager.DeleteAsync(existingClient);

    await manager.CreateAsync(new OpenIddictApplicationDescriptor
    {
        ClientId = "test-client",
        ClientSecret = "test-secret",
        DisplayName = "Test Client",
        Permissions =
        {
            OpenIddictConstants.Permissions.Endpoints.Token,
            OpenIddictConstants.Permissions.GrantTypes.Password,
            OpenIddictConstants.Permissions.GrantTypes.ClientCredentials,
            OpenIddictConstants.Permissions.Prefixes.Scope + "api"
        }
    });
}
app.MapEndpoints();
app.Run();