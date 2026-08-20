using Microsoft.EntityFrameworkCore;
using Korp.Stock.API.Models;

namespace Korp.Stock.API.Data;

public class StockDbContext : DbContext
{
    public StockDbContext(DbContextOptions<StockDbContext> options) : base(options) { }

    public DbSet<Product> Products { get; set; }
}
