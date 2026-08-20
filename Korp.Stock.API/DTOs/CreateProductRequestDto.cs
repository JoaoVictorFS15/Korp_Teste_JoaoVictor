namespace Korp.Stock.API.DTOs;

public class CreateProductRequestDto
{
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Balance { get; set; }
}
