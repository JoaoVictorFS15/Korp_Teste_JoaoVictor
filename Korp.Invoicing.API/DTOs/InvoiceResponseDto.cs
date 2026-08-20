namespace Korp.Invoicing.API.DTOs
{
    public class InvoiceResponseDto
    {
        public int Id { get; set; }
        public int SequentialNumber { get; set; }
        public string Status { get; set; } = string.Empty; 
        public DateTime CreatedAt { get; set; }
        public List<InvoiceItemResponseDto> Items { get; set; } = new();
    }
}
