using System.ComponentModel.DataAnnotations;

namespace WarehouseApi.DTOs;

public class ProductCreateDto
{
    [Required]
    [StringLength(40, MinimumLength = 2)]
    public string Sku { get; set; } = string.Empty;

    [Required]
    [StringLength(120, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    [Range(0, int.MaxValue)]
    public int Quantity { get; set; }

    [Range(0.01, 1000000)]
    public decimal UnitPrice { get; set; }

    [Required]
    public int SupplierId { get; set; }
}
