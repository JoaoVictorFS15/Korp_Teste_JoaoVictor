using Korp.Stock.API.Models;

namespace Korp.Stock.API.Repositories;

public interface IProductRepository
{
    Task<IEnumerable<Product>> GetAllAsync();
    Task<Product?> GetByIdAsync(int id);
    Task AddAsync(Product product);
    Task SaveChangesAsync();
}
