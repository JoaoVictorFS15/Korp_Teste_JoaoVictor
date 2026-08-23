using Korp.Invoicing.API.DTOs;

namespace Korp.Invoicing.API.Services;

public interface IInvoiceAppService
{
    Task<IEnumerable<InvoiceResponseDto>> GetAllInvoicesAsync();
    Task<InvoiceResponseDto?> GetInvoiceByIdAsync(int id);
    Task<InvoiceResponseDto> CreateInvoiceAsync(CreateInvoiceRequestDto dto);
    Task<(bool Success, string Message, InvoiceResponseDto? Invoice)> PrintInvoiceAsync(int id, string idempotencyKey);
}
