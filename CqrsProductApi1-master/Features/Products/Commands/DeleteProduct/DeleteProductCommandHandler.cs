using CqrsProductApi.Repositories;

namespace CqrsProductApi.Features.Products.Commands.DeleteProduct;

public class DeleteProductCommandHandler
{
    private readonly IProductRepository _repo;

    public DeleteProductCommandHandler(IProductRepository repo)
    {
        _repo = repo;
    }

    public async Task<bool> Handle(DeleteProductCommand command)
    {
        var product = await _repo.GetByIdAsync(command.Id);
        if (product == null) return false;

        _repo.Delete(product);
        await _repo.SaveChangesAsync();

        return true;
    }
}