using CqrsProductApi.Entities;
using CqrsProductApi.Repositories;

namespace CqrsProductApi.Features.Products.Commands.CreateProduct;

public class CreateProductCommandHandler
{
    private readonly IProductRepository _repo;

    public CreateProductCommandHandler(IProductRepository repo)
    {
        _repo = repo;
    }

    public async Task<Guid> Handle(CreateProductCommand command)
    {
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = command.Name,
            Price = command.Price,
            CreatedAt = DateTime.UtcNow
        };

        await _repo.AddAsync(product);
        await _repo.SaveChangesAsync();

        return product.Id;
    }
}