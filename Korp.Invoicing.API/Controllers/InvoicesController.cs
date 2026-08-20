using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Korp.Invoicing.API.Data;
using Korp.Invoicing.API.Models;
using Korp.Invoicing.API.Services;

namespace Korp.Invoicing.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InvoicesController : ControllerBase
{
    private readonly InvoicingDbContext _context;
    private readonly IStockService _stockService;

    public InvoicesController(InvoicingDbContext context, IStockService stockService)
    {
        _context = context;
        _stockService = stockService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _context.Invoices.Include(i => i.Items).ToListAsync());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var invoice = await _context.Invoices.Include(i => i.Items).FirstOrDefaultAsync(i => i.Id == id);
        if (invoice == null) return NotFound();
        return Ok(invoice);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Invoice invoice)
    {
        // Regra: Numeração sequencial
        var lastInvoice = await _context.Invoices.OrderByDescending(i => i.SequentialNumber).FirstOrDefaultAsync();
        invoice.SequentialNumber = (lastInvoice?.SequentialNumber ?? 0) + 1;
        invoice.Status = InvoiceStatus.Aberta;
        invoice.CreatedAt = DateTime.UtcNow;

        _context.Invoices.Add(invoice);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = invoice.Id }, invoice);
    }

    [HttpPost("{id}/print")]
    public async Task<IActionResult> Print(int id, [FromHeader(Name = "Idempotency-Key")] string idempotencyKey)
    {
        // Requisito C: Idempotência (Mock Simples). Na prática usaríamos Redis ou Tabela auxiliar.
        // Aqui assumimos que se a nota já está 'Fechada', a requisição repetida foi bloqueada/já tratada.
        
        var invoice = await _context.Invoices.Include(i => i.Items).FirstOrDefaultAsync(i => i.Id == id);
        if (invoice == null) return NotFound();

        if (invoice.Status != InvoiceStatus.Aberta)
            return BadRequest(new { Message = "Apenas notas com status Aberta podem ser impressas." });

        // Processar itens e dar baixa no estoque
        foreach (var item in invoice.Items)
        {
            try 
            {
                var success = await _stockService.DeductStockAsync(item.ProductId, item.Quantity);
                if (!success)
                    return StatusCode(503, new { Message = $"Serviço de estoque indisponível ou saldo insuficiente para o produto {item.ProductId}." });
            }
            catch(Exception ex)
            {
                // Tratamento de Falhas (Requisito 2): Serviço de Estoque caiu
                return StatusCode(503, new { Message = "Erro na comunicação com o Serviço de Estoque. A nota não foi impressa. Tente novamente." });
            }
        }

        invoice.Status = InvoiceStatus.Fechada;
        await _context.SaveChangesAsync();

        return Ok(new { Message = "Nota impressa com sucesso e estoque atualizado.", Invoice = invoice });
    }
}
