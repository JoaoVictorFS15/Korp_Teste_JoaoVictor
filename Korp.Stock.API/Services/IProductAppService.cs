using Korp.Stock.API.DTOs;

namespace Korp.Stock.API.Services;

public interface IProductAppService
{
    Task<IEnumerable<ProductResponseDto>> GetAllProductsAsync();
    Task<(bool Success, string Message, ProductResponseDto? Product)> CreateProductAsync(CreateProductRequestDto dto);
    Task<(bool Success, string Message, int? NewBalance)> DeductStockAsync(int id, int quantity);
    Task<(bool Success, string Message)> DeductStockBulkAsync(List<DeductStockBulkItemDto> items);
}

