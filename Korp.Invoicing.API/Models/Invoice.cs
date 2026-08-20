using System;
using System.Collections.Generic;

namespace Korp.Invoicing.API.Models;

public enum InvoiceStatus
{
    Aberta,
    Fechada
}

public class Invoice
{
    public int Id { get; set; }
    public int SequentialNumber { get; set; }
    public InvoiceStatus Status { get; set; } = InvoiceStatus.Aberta;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<InvoiceItem> Items { get; set; } = new();
}
