using Microsoft.AspNetCore.Mvc;
using Korp.Stock.API.DTOs;
using Korp.Stock.API.Services;

namespace Korp.Stock.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductAppService _appService;

    public ProductsController(IProductAppService appService)
    {
        _appService = appService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _appService.GetAllProductsAsync();
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateProductRequestDto request)
    {
        var result = await _appService.CreateProductAsync(request);

        if (!result.Success)
            return BadRequest(new { Message = result.Message });
        return CreatedAtAction(nameof(GetAll), new { id = result.Product.Id }, result.Product);
    }

    [HttpPost("{id}/deduct")]
    public async Task<IActionResult> DeductStock(int id, [FromBody] DeductStockRequestDto request)
    {
        var result = await _appService.DeductStockAsync(id, request.Quantity);

        if (!result.Success)
        {
            if (result.Message.Contains("nÃ£o encontrado"))
                return NotFound(new { result.Message });

            if (result.Message.Contains("simultaneamente"))
                return Conflict(new { result.Message });

            return BadRequest(new { result.Message });
        }

        return Ok(new { result.Message, Balance = result.NewBalance });
    }

    [HttpPost("deduct-bulk")]
    public async Task<IActionResult> DeductStockBulk([FromBody] DeductStockBulkRequestDto request)
    {
        var result = await _appService.DeductStockBulkAsync(request.Items);
        if (!result.Success) return BadRequest(new { result.Message });
        return Ok(new { result.Message });
    }

    [HttpPost("ai/enhance-description")]
    public async Task<IActionResult> EnhanceDescription([FromBody] AiRequestDto req, [FromServices] IConfiguration config)
    {
        var apiKey = config["GeminiApiKey"];
        if (string.IsNullOrEmpty(apiKey) || apiKey.Contains("SUA_CHAVE_AQUI")) 
            return BadRequest(new { message = "Chave da API Gemini nÃ£o configurada. Lembre de rodar o dotnet user-secrets set." });
        
        using var httpClient = new HttpClient();
        var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-3.6-flash:generateContent?key={apiKey}";
        var payload = new { contents = new[] { new { parts = new[] { new { text = $"Melhore a descriÃ§Ã£o deste produto para ficar profissional e atrativa (mÃ¡ximo 12 palavras). Retorne APENAS o texto direto: " + req.Description } } } } };
        
        var response = await httpClient.PostAsJsonAsync(url, payload);
        if (!response.IsSuccessStatusCode) {
            var error = await response.Content.ReadAsStringAsync();
            return BadRequest(new { message = "Falha ao conectar com a IA: " + error });
        }

        try {
            var rawJson = await response.Content.ReadAsStringAsync();
            using var doc = System.Text.Json.JsonDocument.Parse(rawJson);
            var enhancedText = doc.RootElement.GetProperty("candidates")[0].GetProperty("content").GetProperty("parts")[0].GetProperty("text").GetString();
            return Ok(new { description = enhancedText?.Trim() });
        } catch (Exception ex) { 
            return BadRequest(new { message = "Erro ao processar resposta da IA: " + ex.Message }); 
        }
    }
}


