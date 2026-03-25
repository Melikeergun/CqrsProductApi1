using CqrsProductApi.Data;
using CqrsProductApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace CqrsProductApi.Features.Products.Queries.GetAllProducts;

public class GetAllProductsQueryHandler
{
    private readonly AppDbContext _context;

    public GetAllProductsQueryHandler(AppDbContext context) => _context = context;

    public async Task<List<Product>> Handle(GetAllProductsQuery query)
        => await _context.Products.AsNoTracking().OrderByDescending(x => x.CreatedAt).ToListAsync();
}
