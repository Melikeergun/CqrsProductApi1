using CqrsProductApi.Data;
using CqrsProductApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace CqrsProductApi.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _context;

    public ProductRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Product product)
        => await _context.Products.AddAsync(product);

    public async Task<Product?> GetByIdAsync(Guid id)
        => await _context.Products.FirstOrDefaultAsync(x => x.Id == id);

    public async Task<List<Product>> GetAllAsync()
        => await _context.Products.AsNoTracking()
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();

    public void Delete(Product product)
        => _context.Products.Remove(product);

    public async Task SaveChangesAsync()
        => await _context.SaveChangesAsync();
}