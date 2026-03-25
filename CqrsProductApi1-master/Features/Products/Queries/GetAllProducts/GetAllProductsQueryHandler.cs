using CqrsProductApi.Features.Products.Dtos;
using CqrsProductApi.Repositories;

namespace CqrsProductApi.Features.Products.Queries.GetAllProducts;

public class GetAllProductsQueryHandler
{
    private readonly IProductRepository _repo;

    public GetAllProductsQueryHandler(IProductRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<ProductDto>> Handle(GetAllProductsQuery query)
    {
        var products = await _repo.GetAllAsync();

        return products.Select(x => new ProductDto
        {
            Id = x.Id,
            Name = x.Name,
            Price = x.Price,
            CreatedAt = x.CreatedAt
        }).ToList();
    }
}