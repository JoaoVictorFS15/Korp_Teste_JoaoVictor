using Microsoft.AspNetCore.Mvc;
using Korp.Invoicing.API.DTOs;
using Korp.Invoicing.API.Services;

namespace Korp.Invoicing.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InvoicesController : ControllerBase
{
    private readonly IInvoiceAppService _appService;

    public InvoicesController(IInvoiceAppService appService)
    {
        _appService = appService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _appService.GetAllInvoicesAsync();
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _appService.GetInvoiceByIdAsync(id);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateInvoiceRequestDto request)
    {
        var result = await _appService.CreateInvoiceAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPost("{id}/print")]
    public async Task<IActionResult> Print(int id, [FromHeader(Name = "Idempotency-Key")] string idempotencyKey)
    {
        var result = await _appService.PrintInvoiceAsync(id, idempotencyKey);
        
        if (!result.Success)
        {
            if (result.Message.Contains("não encontrada"))
                return NotFound(new { result.Message });
            
            if (result.Message.Contains("Apenas notas"))
                return BadRequest(new { result.Message });

            return StatusCode(503, new { result.Message });
        }

        return Ok(new { result.Message, Invoice = result.Invoice });
    }
}
