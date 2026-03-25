using CqrsProductApi.Data;
using CqrsProductApi.Entities;

namespace CqrsProductApi.Features.Products.Commands.CreateProduct;

public class CreateProductCommandHandler
{
    private readonly AppDbContext _context;

    public CreateProductCommandHandler(AppDbContext context) => _context = context;

    public async Task<Guid> Handle(CreateProductCommand command)
    {
        var product = new Product {
            Id = Guid.NewGuid(), Name = command.Name,
            Price = command.Price, CreatedAt = DateTime.UtcNow
        };
        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();
        return product.Id;
    }
}
