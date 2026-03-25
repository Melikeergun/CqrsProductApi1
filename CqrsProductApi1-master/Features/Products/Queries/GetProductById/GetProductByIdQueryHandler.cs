using CqrsProductApi.Features.Products.Dtos;
using CqrsProductApi.Repositories;

namespace CqrsProductApi.Features.Products.Queries.GetProductById;

public class GetProductByIdQueryHandler
{
    private readonly IProductRepository _repo;

    public GetProductByIdQueryHandler(IProductRepository repo)
    {
        _repo = repo;
    }

    public async Task<ProductDto?> Handle(GetProductByIdQuery query)
    {
        var product = await _repo.GetByIdAsync(query.Id);
        if (product == null) return null;

        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            CreatedAt = product.CreatedAt
        };
    }
}