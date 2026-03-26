using CqrsProductApi.Features.Products.Commands.DeleteProduct;
using FastEndpoints;

namespace CqrsProductApi1.Features.Products.Endpoints;

public class DeleteProductEndpoint(DeleteProductCommandHandler handler) : Endpoint<DeleteProductCommand, object>
{
    public override void Configure()
    {
        Delete("/api/fast/products/{id}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(DeleteProductCommand req, CancellationToken ct)
    {
        var deleted = await handler.Handle(req);

        if (!deleted)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        await Send.NoContentAsync(ct);
    }
}
