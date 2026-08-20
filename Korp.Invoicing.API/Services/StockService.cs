using System.Text.Json;
using System.Text.Json.Serialization;

namespace Korp.Invoicing.API.Services;

public interface IStockService
{
    Task<bool> DeductStockAsync(int productId, int quantity);
}

public class StockService : IStockService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<StockService> _logger;

    public StockService(HttpClient httpClient, ILogger<StockService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<bool> DeductStockAsync(int productId, int quantity)
    {
        var content = new StringContent(JsonSerializer.Serialize(new { Quantity = quantity }), System.Text.Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync($"/api/products/{productId}/deduct", content);

        if (response.IsSuccessStatusCode)
            return true;
            
        _logger.LogWarning($"Falha ao deduzir estoque para o produto {productId}. Status: {response.StatusCode}");
        return false;
    }
}
