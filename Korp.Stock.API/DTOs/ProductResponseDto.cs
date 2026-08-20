namespace Korp.Stock.API.DTOs;

public class ProductResponseDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Balance { get; set; }
}
