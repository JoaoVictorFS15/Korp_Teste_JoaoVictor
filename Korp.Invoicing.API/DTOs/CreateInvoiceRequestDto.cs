namespace Korp.Invoicing.API.DTOs
{
    public class CreateInvoiceRequestDto
    {
        public List<CreateInvoiceItemDto> Items { get; set; } = new();
    }
}
