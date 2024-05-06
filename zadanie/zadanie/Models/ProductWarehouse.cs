using System.ComponentModel.DataAnnotations;

namespace zadanie.Models;

public class ProductWarehouse
{
    [Required] public int IdProduct { get; set; }
    
    [Required] public int IdWarehouse { get; set; }
    
    [Range(1, int.MaxValue)]
    [Required] public int Amount { get; set; }
    
    [Required] public DateTime CreatedAt { get; set; }
}