using System.ComponentModel.DataAnnotations;

namespace Korp.Stock.API.Models;

public class Product
{
    public int Id { get; set; }
    
    [Required]
    public string Code { get; set; } = string.Empty;
    
    [Required]
    public string Description { get; set; } = string.Empty;
    
    public int Balance { get; set; }

    // Requisito Opcional A: Tratamento de Concorrência
    // Este campo garante que se 2 notas baixarem o saldo simultaneamente, o EF lançará uma DbUpdateConcurrencyException
    [Timestamp]
    public byte[]? RowVersion { get; set; }
}
