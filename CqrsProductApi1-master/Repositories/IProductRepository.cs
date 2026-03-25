using CqrsProductApi.Entities;

namespace CqrsProductApi.Repositories;

public interface IProductRepository
{
    Task AddAsync(Product product);
    Task<Product?> GetByIdAsync(Guid id);
    Task<List<Product>> GetAllAsync();
    void Delete(Product product);
    Task SaveChangesAsync();
}