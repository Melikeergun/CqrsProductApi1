using CqrsProductApi.Features.Products.Commands.CreateProduct;
using CqrsProductApi.Features.Products.Dtos;
using FastEndpoints;

namespace CqrsProductApi1.Features.Products.Endpoints;

public class CreateProductEndpoint(CreateProductCommandHandler handler)
    : Endpoint<CreateProductRequest, object>
{
    public override void Configure()
    {
        Post("/api/fast/products");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CreateProductRequest req, CancellationToken ct)
    {
        var command = new CreateProductCommand
        {
            Name = req.Name,
            Price = req.Price
        };

        var id = await handler.Handle(command);

        await Send.CreatedAtAsync<GetProductByIdEndpoint>(
            new { id },
            new { id },
            cancellation: ct);
    }
}