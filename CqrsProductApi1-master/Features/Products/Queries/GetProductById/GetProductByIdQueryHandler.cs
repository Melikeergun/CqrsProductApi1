using CqrsProductApi.Data;
using CqrsProductApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace CqrsProductApi.Features.Products.Queries.GetProductById;

public class GetProductByIdQueryHandler
{
    private readonly AppDbContext _context;

    public GetProductByIdQueryHandler(AppDbContext context) => _context = context;

    public async Task<Product?> Handle(GetProductByIdQuery query)
        => await _context.Products.FirstOrDefaultAsync(x => x.Id == query.Id);
}
