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
}
