namespace Korp.Stock.API.DTOs;

public class DeductStockBulkItemDto { public int ProductId { get; set; } public int Quantity { get; set; } }

public class DeductStockBulkRequestDto { public List<DeductStockBulkItemDto> Items { get; set; } = new(); }
