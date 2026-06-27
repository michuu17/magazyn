using System.ComponentModel.DataAnnotations;

namespace WarehouseApi.DTOs;

public class StockOperationDto
{
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }
}
