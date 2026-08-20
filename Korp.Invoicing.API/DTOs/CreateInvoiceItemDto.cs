namespace Korp.Invoicing.API.DTOs
{
    public class CreateInvoiceItemDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
