using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Korp.Stock.API.Data;
using Korp.Stock.API.Models;

namespace Korp.Stock.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly StockDbContext _context;

    public ProductsController(StockDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _context.Products.ToListAsync());
    }

    [HttpPost]
    public async Task<IActionResult> Create(Product product)
    {
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetAll), new { id = product.Id }, product);
    }

    public class DeductStockRequest { public int Quantity { get; set; } }

    [HttpPost("{id}/deduct")]
    public async Task<IActionResult> DeductStock(int id, [FromBody] DeductStockRequest request)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null) return NotFound();

        if (product.Balance < request.Quantity)
            return BadRequest(new { Message = "Saldo insuficiente no estoque." });

        product.Balance -= request.Quantity;

        try
        {
            await _context.SaveChangesAsync();
            return Ok(new { Message = "Estoque atualizado.", product.Balance });
        }
        catch (DbUpdateConcurrencyException)
        {
            return Conflict(new { Message = "O saldo deste produto foi modificado simultaneamente por outra transação. Tente novamente." });
        }
    }
}
