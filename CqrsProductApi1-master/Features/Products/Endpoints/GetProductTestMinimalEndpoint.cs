using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using CqrsProductApi1.Features.Common;

namespace CqrsProductApi1.Features.Products.Endpoints; 
public class GetProductTestMinimalEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        
        app.MapGet("/test-products", () =>
        {
            return Results.Ok(new
            {
                Message = "Minimal API harika bir şekilde çalışıyor!",
                Status = true
            });
        })
        .WithName("GetTestProducts") 

        .WithTags("Minimal API Test"); 

    }
}
