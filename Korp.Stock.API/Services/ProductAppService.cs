using Microsoft.EntityFrameworkCore;
using Korp.Stock.API.DTOs;
using Korp.Stock.API.Models;
using Korp.Stock.API.Repositories;

namespace Korp.Stock.API.Services;



public class ProductAppService : IProductAppService
{
    private readonly IProductRepository _repository;

    public ProductAppService(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<ProductResponseDto>> GetAllProductsAsync()
    {
        var products = await _repository.GetAllAsync();
        return products.Select(p => new ProductResponseDto
        {
            Id = p.Id,
            Code = p.Code,
            Description = p.Description,
            Balance = p.Balance
        });
    }

    public async Task<(bool Success, string Message, ProductResponseDto? Product)> CreateProductAsync(CreateProductRequestDto dto)
    {
        var existingProducts = await _repository.GetAllAsync();
        if (existingProducts.Any(p => p.Code.Equals(dto.Code, StringComparison.OrdinalIgnoreCase)))
        {
            return (false, "JÃ¡ existe um produto cadastrado com este cÃ³digo.", null);
        }
        var product = new Product
        {
            Code = dto.Code,
            Description = dto.Description,
            Balance = dto.Balance
        };
        await _repository.AddAsync(product);
        await _repository.SaveChangesAsync();
        var responseDto = new ProductResponseDto
        {
            Id = product.Id,
            Code = product.Code,
            Description = product.Description,
            Balance = product.Balance
        };
        return (true, "Produto criado com sucesso.", responseDto);
    }

    public async Task<(bool Success, string Message, int? NewBalance)> DeductStockAsync(int id, int quantity)
    {
        var product = await _repository.GetByIdAsync(id);
        if (product == null)
            return (false, "Produto nÃ£o encontrado.", null);

        if (product.Balance < quantity)
            return (false, "Saldo insuficiente no estoque.", null);

        product.Balance -= quantity;

        try
        {
            await _repository.SaveChangesAsync();
            return (true, "Estoque atualizado.", product.Balance);
        }
        catch (DbUpdateConcurrencyException)
        {
            return (false, "O saldo deste produto foi modificado simultaneamente por outra transaÃ§Ã£o. Tente novamente.", null);
        }
    }
}

