using System.Text.Json.Serialization;

namespace Korp.Invoicing.API.Models;

public class InvoiceItem
{
    public int Id { get; set; }
    
    public int InvoiceId { get; set; }
    
    [JsonIgnore]
    public Invoice? Invoice { get; set; }

    public int ProductId { get; set; }
    public int Quantity { get; set; }
}
