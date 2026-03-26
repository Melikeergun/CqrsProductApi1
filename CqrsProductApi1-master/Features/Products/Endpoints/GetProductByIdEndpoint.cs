using CqrsProductApi.Features.Products.Queries.GetProductById;
using CqrsProductApi.Entities;
using FastEndpoints;

namespace CqrsProductApi1.Features.Products.Endpoints;

public class GetProductByIdEndpoint(GetProductByIdQueryHandler handler) : Endpoint<GetProductByIdQuery, Product>
{
    public override void Configure()
    {
        Get("/api/fast/products/{id}");
        AllowAnonymous(); 
    }

    public override async Task HandleAsync(GetProductByIdQuery req, CancellationToken ct)
    {
        var product = await handler.Handle(req);

        if (product is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        await Send.OkAsync(product, ct);
    }
}
