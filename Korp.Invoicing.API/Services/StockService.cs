using System.Text.Json;
using System.Text.Json.Serialization;

namespace Korp.Invoicing.API.Services;

public interface IStockService
{
    Task<bool> DeductStockAsync(int productId, int quantity);
    Task<(bool Success, string Message)> DeductStockBulkAsync(IEnumerable<Korp.Invoicing.API.Models.InvoiceItem> items);
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

    public async Task<(bool Success, string Message)> DeductStockBulkAsync(IEnumerable<Korp.Invoicing.API.Models.InvoiceItem> items)
    {
        var payload = new { Items = items.Select(i => new { ProductId = i.ProductId, Quantity = i.Quantity }) };
        var response = await _httpClient.PostAsJsonAsync("/api/products/deduct-bulk", payload);
        if (response.IsSuccessStatusCode) return (true, "");
        
        // Tenta ler a mensagem de erro que veio do Estoque
        try {
            var errorContent = await response.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();
            if (errorContent.TryGetProperty("message", out var msgProp))
                return (false, msgProp.GetString() ?? "Erro desconhecido no Estoque.");
        } catch {}
        
        return (false, "Serviço de estoque retornou um erro.");
    }
}

