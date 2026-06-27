using WarehouseApi.DTOs;

namespace WarehouseApi.Services;

public interface IProductService
{
    Task<PagedResult<ProductResponseDto>> GetPagedAsync(PaginationQuery query);
    Task<ProductResponseDto> GetByIdAsync(int id);
    Task<ProductResponseDto> CreateAsync(ProductCreateDto dto);
    Task<ProductResponseDto> UpdateAsync(int id, ProductUpdateDto dto);
    Task DeleteAsync(int id);
    Task<ProductResponseDto> ReceiveStockAsync(int id, int quantity);
    Task<ProductResponseDto> IssueStockAsync(int id, int quantity);
}
