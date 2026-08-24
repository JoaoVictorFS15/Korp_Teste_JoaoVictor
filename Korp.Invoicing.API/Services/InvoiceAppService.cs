using Korp.Invoicing.API.DTOs;
using Korp.Invoicing.API.Models;
using Korp.Invoicing.API.Repositories;

namespace Korp.Invoicing.API.Services;



public class InvoiceAppService : IInvoiceAppService
{
    private readonly IInvoiceRepository _repository;
    private readonly IStockService _stockService;

    public InvoiceAppService(IInvoiceRepository repository, IStockService stockService)
    {
        _repository = repository;
        _stockService = stockService;
    }

    public async Task<IEnumerable<InvoiceResponseDto>> GetAllInvoicesAsync()
    {
        var invoices = await _repository.GetAllAsync();
        return invoices.Select(MapToResponseDto);
    }

    public async Task<InvoiceResponseDto?> GetInvoiceByIdAsync(int id)
    {
        var invoice = await _repository.GetByIdAsync(id);
        if (invoice == null) return null;
        return MapToResponseDto(invoice);
    }

    public async Task<InvoiceResponseDto> CreateInvoiceAsync(CreateInvoiceRequestDto dto)
    {
        var invoice = new Invoice
        {
            SequentialNumber = await _repository.GetNextSequentialNumberAsync(),
            Status = InvoiceStatus.Aberta,
            CreatedAt = DateTime.UtcNow,
            Items = dto.Items.Select(i => new InvoiceItem
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity
            }).ToList()
        };

        await _repository.AddAsync(invoice);
        await _repository.SaveChangesAsync();

        return MapToResponseDto(invoice);
    }

    public async Task<(bool Success, string Message, InvoiceResponseDto? Invoice)> PrintInvoiceAsync(int id, string idempotencyKey)
    {
        var invoice = await _repository.GetByIdAsync(id);
        if (invoice == null)
            return (false, "Nota Fiscal nÃ£o encontrada.", null);

        if (invoice.Status != InvoiceStatus.Aberta)
            return (false, "Apenas notas com status Aberta podem ser impressas.", null);

        try 
        {
            var result = await _stockService.DeductStockBulkAsync(invoice.Items);
            if (!result.Success) return (false, result.Message, null);
        }
        catch(Exception)
        {
            return (false, "Erro na comunicação com o Serviço de Estoque. A nota não foi impressa. Tente novamente.", null);
        }

        invoice.Status = InvoiceStatus.Fechada;
        await _repository.UpdateAsync(invoice);
        await _repository.SaveChangesAsync();

        return (true, "Nota impressa com sucesso e estoque atualizado.", MapToResponseDto(invoice));
    }

    private static InvoiceResponseDto MapToResponseDto(Invoice invoice)
    {
        return new InvoiceResponseDto
        {
            Id = invoice.Id,
            SequentialNumber = invoice.SequentialNumber,
            Status = invoice.Status.ToString(),
            CreatedAt = invoice.CreatedAt,
            Items = invoice.Items.Select(i => new InvoiceItemResponseDto
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity
            }).ToList()
        };
    }
}



