using CqrsProductApi.Features.Products.Queries.GetAllProducts;
using CqrsProductApi.Entities;
using FastEndpoints;

namespace CqrsProductApi1.Features.Products.Endpoints;

public class GetAllProductsEndpoint(GetAllProductsQueryHandler handler) : Endpoint<EmptyRequest, List<Product>>
{
    public override void Configure()
    {
        Get("/api/fast/products");
        AllowAnonymous();
    }

    public override async Task HandleAsync(EmptyRequest req, CancellationToken ct)
    {
        var products = await handler.Handle(new GetAllProductsQuery());
        await Send.OkAsync(products, ct);
    }
}
